using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePatrol : NPCState
{
    private float _speed;
    private Animator _anim;
    private List<Node> _waypoints;
    int _currentWaypoint;
    int _indexModifier;
    int _patrolCount;

    public StatePatrol(FSM fsm, NPC owner, float speed, List<Node> waypoints, string message = null) : base(fsm, owner)
    {
        _currentWaypoint = 0;
        _indexModifier = 1;
        _patrolCount = 0;
        _waypoints = waypoints;
        _speed = speed;
        _anim = owner.GetComponent<Animator>();
        OnNotifyResetLevel(message);
        EventsManager.SubscribeToEvent("OnReachEndPath", OnReachEndPath);
    }

    private void OnReachEndPath(object[] parameterContainer)
    {
        _currentWaypoint = 0;
        _indexModifier = 1;
        _patrolCount = 0;
    }

    public override void Awake()
    {
        //Debug.Log("START PATROL STATE...");
    }

    public override void Execute()
    {
        DetectPlayer();

        Advance();

        if (_patrolCount > 1)
        {
            EventsManager.TriggerEvent("OnTwoPatrolIterations");
            _patrolCount = 0;
        }
    }

    public override void ExecuteFixed()
    {

    }

    public override void Sleep()
    {
        //Debug.Log("END PATROL STATE...");
    }

    public void Advance()
    {
        _anim.SetFloat("Speed", 1.0f);
        if (Vector3.Distance(_waypoints[_currentWaypoint].transform.position, _owner.transform.position) < 0.5f)
        {
            if (_currentWaypoint + _indexModifier >= _waypoints.Count || _currentWaypoint + _indexModifier < 0)
            {
                _indexModifier *= -1;
            }
            if (_currentWaypoint == (_waypoints.Count / 2))
            {
                _patrolCount++;
            }

            _currentWaypoint += _indexModifier;
        }
        //EventsManager.TriggerEvent("OnCurrentWaypoint", _waypoints[_currentWaypoint]);

        Vector3 dir = _waypoints[_currentWaypoint].transform.position - _owner.transform.position;
        _owner.transform.forward = dir;
        _owner.GetComponent<Rigidbody>().AddForce(_owner.transform.forward * _speed);
    }

    public void DetectPlayer()
    {
        _owner.DistanceToTarget = Vector3.Distance(_owner.transform.position, _owner.target.transform.position);
        _owner.DirToTarget = _owner.transform.position - _owner.target.transform.position;

        //print("DISTANCIA AL OBJETIVO: " + DistanceToTarget);
        _owner.AngleToTarget = Vector3.Angle(-_owner.transform.forward, _owner.DirToTarget);
        RaycastHit hit;

        if (_owner.DistanceToTarget < _owner.DistanceThreshold && _owner.AngleToTarget < _owner.AngleThreshold)
        {
            Debug.DrawRay(_owner.head.transform.position, -_owner.DirToTarget, Color.red);

            if (Physics.Raycast(_owner.head.transform.position, -_owner.DirToTarget, out hit, _owner.DistanceToTarget))
            {
                //Una vez que descartamos las primeras posibilidades, vamos a utilizar un raycast.            
                if (hit.collider.gameObject.layer == Layers.PLAY_AREA)
                {
                    _owner._obstaclesBetween = true;  //En caso de colisionar contra una pared, esto se vuelve verdadero.
                }
                else
                {
                    _owner._obstaclesBetween = false;
                }
                //Debug.Log("OBSTACLES BETWEEN? " + obstaclesBetween);
                //Si el raycast no colisionó contra ningún objeto informamos que lo tiene en el rango de visión            
                if (_owner._obstaclesBetween)
                {
                    _owner.TargetInSight = false;
                }
                else
                {
                    _owner.TargetInSight = true;
                    _anim.SetBool("PlayerInSight", _owner.TargetInSight);
                }
            }
        }
        else
        {
            _owner._obstaclesBetween = false;
            _owner.TargetInSight = false;
            _anim.SetBool("PlayerInSight", _owner.TargetInSight);
        }
        //print("DIRECTION AL OBJETIVO: " + DirToTarget);

    }

    public void OnNotifyResetLevel(string message)
    {
        if (message != null && message.Equals("ResetLevel"))
        {
            _currentWaypoint = 0;
            _indexModifier = 1;
            _patrolCount = 0;
        }
    }
}
