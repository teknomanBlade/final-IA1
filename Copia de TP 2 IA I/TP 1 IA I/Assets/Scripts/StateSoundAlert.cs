using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSoundAlert : NPCState
{
    private Animator _anim;
    private GameObject _target;
    public StateSoundAlert(FSM fsm, NPC owner, GameObject target) : base(fsm, owner)
    {
        _target = target;
        _anim = owner.GetComponent<Animator>();
    }

    public override void Awake()
    {

    }

    public override void Execute()
    {
        Vector3 dir = _target.transform.position - _owner.transform.position;
        _owner.transform.forward = dir;
    }

    public override void ExecuteFixed()
    {
        DetectPlayer();
    }

    public override void Sleep()
    {

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
}
