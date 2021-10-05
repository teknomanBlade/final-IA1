using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    
    #region public Properties
   
    public QuestionNode rootQuestion;
    public GameObject target;
    public GameObject laserGun;
    public GameObject head;
    public List<Node> waypoints;
    public float DistanceToTarget;
    public float AngleToTarget;
    public bool TargetInSight;
    public bool TargetHeard;
    public float DistanceThreshold;
    public float AngleThreshold;
    public bool IsIdle;
    public bool _obstaclesBetween;
    [HideInInspector]
    public Vector3 DirToTarget;
    #endregion
    #region private Properties
    FSM _mFSM;
    private List<Node> Path;
    private AStar _aStar;
    private QuestionNode _isIdleQuestion;
    private QuestionNode _playerInSightQuestion;
    private QuestionNode _playerHeardQuestion;
    private QuestionNode _isAlertQuestion;
    private ActionNode _idleAction;
    private ActionNode _patrolAction;
    private ActionNode _alertAction;
    private ActionNode _alertSoundAction;
    private ActionNode _shootAction;
    private Vector3 DirBulletToTarget;
    public Node _initialNode;
    private Node _finalNode;
    private float SpeedRun;
    private float SpeedWalk;
    private Rigidbody _RB;
    private Animator _anim;
    private PoolObject<BulletLaser> _bulletLaserPool;
    private int InitialStock;
    private BulletLaser prefabBulletLaser;
    private bool IsAlertRun;
    private float Tick;
    private float FireRateBulletLaser;
    private StateAlert _stateAlert;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        _obstaclesBetween = false;
        IsAlertRun = false;
        FireRateBulletLaser = 1.5f;
        InitialStock = 10;
        SpeedRun = 50f;
        SpeedWalk = 18f;
        DistanceThreshold = 10f;
        AngleThreshold = 60f;
        _aStar = FindObjectOfType<AStar>();
        #region FiniteStateMachine
            _mFSM = new FSM();
            _stateAlert = new StateAlert(_mFSM, this, SpeedRun, _aStar);
            _mFSM.AddState(new StateIdle(_mFSM, this));
            _mFSM.AddState(new StatePatrol(_mFSM, this, SpeedWalk, waypoints));
            _mFSM.AddState(new StateShoot(_mFSM, this, target));
            _mFSM.AddState(_stateAlert);
            _mFSM.AddState(new StateSoundAlert(_mFSM, this, target));
        #endregion

        prefabBulletLaser = Resources.Load<BulletLaser>("LaserShot");
        _anim = GetComponent<Animator>();
        _RB = GetComponent<Rigidbody>();
        _bulletLaserPool = new PoolObject<BulletLaser>(BulletLaserFactory, ActivateBullet, DeactivateBullet, InitialStock, true);

        #region Events
            EventsManager.SubscribeToEvent("OnTwoPatrolIterations", OnTwoPatrolIterations);
            EventsManager.SubscribeToEvent("OnAlertRobot", OnAlertRobot);
            EventsManager.SubscribeToEvent("OnTurnBackPath", OnTurnBackPath);
            EventsManager.SubscribeToEvent("OnAlarmNode", OnAlarmNode);
            EventsManager.SubscribeToEvent("OnPlayerOutOfHearingRange", OnPlayerOutOfHearingRange);
            EventsManager.SubscribeToEvent("ResetLevel", OnResetLevel);
        #endregion

        #region DecisionTree
             _idleAction = new ActionNode(() => {
                _mFSM.SetState<StateIdle>();
            });

            _patrolAction = new ActionNode(() => {
                _mFSM.SetState<StatePatrol>();
            });

            _shootAction = new ActionNode(() => {
                _mFSM.SetState<StateShoot>();
            });

            _alertAction = new ActionNode(() => {
                _mFSM.SetState<StateAlert>();
            });

            _alertSoundAction = new ActionNode(() => {
                _mFSM.SetState<StateSoundAlert>();     
            });

            _playerHeardQuestion = new QuestionNode(_alertSoundAction, _patrolAction, () => {
                return TargetHeard;
            });

            _isAlertQuestion = new QuestionNode(_alertAction, _playerHeardQuestion, () => {
                return IsAlertRun;
            });

            _playerInSightQuestion = new QuestionNode(_shootAction, _isAlertQuestion, () => {
                return TargetInSight;
            });

            _isIdleQuestion = new QuestionNode(_idleAction, _playerInSightQuestion, () => {
                return IsIdle;
            });

        #endregion

        rootQuestion = _isIdleQuestion;
        
    }

    private void OnPlayerOutOfHearingRange(object[] parameterContainer)
    { 
        TargetHeard = false;
    }

    private void OnResetLevel(object[] parameterContainer)
    {
        IsIdle = false;
        IsAlertRun = false;
        TargetInSight = false;
        _obstaclesBetween = false;
        TargetHeard = false;
    }

    private void OnTurnBackPath(object[] parameterContainer)
    {
        IsAlertRun = false;
    }

    private void OnAlertRobot(object[] parameterContainer)
    {
        IsAlertRun = true;
        IsIdle = false;
    }

    private void OnAlarmNode(object[] parameterContainer)
    {
        _finalNode = (Node)parameterContainer[0];
        _stateAlert.SetInitialAndFinalNode(_initialNode, _finalNode);
        //Debug.Log("INSTANCES COUNTER: " + NodesGenerator.Counter);
    }


    private void OnTwoPatrolIterations(object[] parameterContainer)
    {
        IsIdle = true;
        if(this != null)
            StartCoroutine(ChangeIdleToFalse());
    }

    private BulletLaser BulletLaserFactory()
    {
        return Instantiate(prefabBulletLaser);
    }
    public PoolObject<BulletLaser> GetLaserBulletPool()
    {
        return _bulletLaserPool;
    }
    private void FixedUpdate()
    {
        _mFSM.LastUpdate();
    }

    // Update is called once per frame
    void Update()
    {

        if (rootQuestion != null)
        {
            rootQuestion.Execute();
            _mFSM.Update();
        }
    }

    public IEnumerator ChangeIdleToFalse() {
        
        yield return new WaitForSeconds(12f);
        IsIdle = false;
    }

    public GameObject GetLaserGun() {
        return laserGun;
    }

    private void ActivateBullet(BulletLaser b)
    {
        b.gameObject.SetActive(true);
        b.transform.localPosition = laserGun.transform.GetChild(0).position;
        DirBulletToTarget = b.gameObject.transform.position - target.transform.position;
        b.transform.forward = DirBulletToTarget;
        
    }

    private void DeactivateBullet(BulletLaser b)
    {
        b.gameObject.SetActive(false);
        b.transform.localPosition = new Vector2(0f, 0f);
        b.transform.localRotation = Quaternion.Euler(0f,0f,0f);
    }

    public void FireLaser()
    {
        Tick += Time.deltaTime;
        if (Tick > FireRateBulletLaser)
        {
            Tick = 0f;
            GetLaserBulletPool().GetObject().PlayBulletSound();
        }

    }

    void OnDrawGizmos()
    {
        /*
        Dibujamos una línea desde el NPC hasta el enemigo.
        Va a ser de color verde si lo esta viendo, roja sino.
        */
        /*if (TargetInSight)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, target.transform.position);*/

        /*
        Dibujamos los límites del campo de visión.
        */
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, DistanceThreshold);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * DistanceThreshold));

        Vector3 rightLimit = Quaternion.AngleAxis(AngleThreshold, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * DistanceThreshold));

        Vector3 leftLimit = Quaternion.AngleAxis(-AngleThreshold, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * DistanceThreshold));
    }

    private void OnTriggerEnter(Collider other)
    {

        if (gameObject.name.Contains("South") && other.gameObject.GetComponent<Node>() != null && !other.gameObject.GetComponent<Node>().isBlocked)
        {
            _initialNode = other.gameObject.GetComponent<Node>();
        }
        if (gameObject.name.Contains("North") && other.gameObject.GetComponent<Node>() != null && !other.gameObject.GetComponent<Node>().isBlocked)
        {
            _initialNode = other.gameObject.GetComponent<Node>();
        }
    }
}
