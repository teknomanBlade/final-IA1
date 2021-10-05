using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAlert : NPCState
{

    private float _speed;
    private Animator _anim;
    private AStar _aStar;
    private Node _initialNode;
    private Node _finalNode;
    private List<Node> _path;
    int _currentNode = 0;
    int _index = 1;

    public StateAlert(FSM fsm, NPC owner, float speed, AStar aStar) : base(fsm, owner)
    {
        _speed = speed;
        _anim = owner.GetComponent<Animator>();
        _aStar = aStar;
        _path = new List<Node>();
        EventsManager.SubscribeToEvent("ResetLevel", OnResetLevel);
    }

    private void OnResetLevel(object[] parameterContainer)
    {
        _currentNode = 0;
        _index = 1;
    }

    public override void Awake()
    {
        if (_initialNode != null && _finalNode != null) {
            _path = _aStar.GetPath(_initialNode, _finalNode);
        }
    }

    public override void Execute()
    {
        GoToTarget();
    }

    public override void ExecuteFixed()
    {
        DetectPlayer();
    }

    public override void Sleep()
    {
        ClearPathAndFalse();
    }

    public void GoToTarget()
    {
        if (_index == -1)
        {
            _anim.SetFloat("Speed", 1.0f);
        }
        else
        {
            _anim.SetFloat("Speed", 5f);
        }

        //Debug.Log("PATH COUNT SOUTH: " + _path.Count);
        foreach (var item in _path)
        {
            item.isPath = true;
        }
        if (_currentNode == 0 && _index == -1)
        {
            _index = 1;
            ClearPathAndFalse();
            EventsManager.TriggerEvent("OnTurnBackPath");
        }

        if (_currentNode < _path.Count - 1)
        {
            if (Vector3.Distance(_path[_currentNode].transform.position, _owner.transform.position) < 0.5f)
            {
                if ((_currentNode + _index) + 1 >= _path.Count || _currentNode + _index < 0)
                {
                    _index *= -1;
                    _speed = 18f;
                    EventsManager.TriggerEvent("OnReachEndPath");
                }
                else
                   _currentNode += _index;
            }
        
            Vector3 dir = _path[_currentNode].transform.position - _owner.transform.position;
            _owner.transform.forward = dir;
            _owner.GetComponent<Rigidbody>().AddForce(_owner.transform.forward * _speed);
        }

    }

    public void ClearPathAndFalse()
    {
        foreach (var item in _path)
        {
            item.isPath = false;
        }
        _path.Clear();
    }

    public StateAlert SetInitialAndFinalNode(Node initial, Node final) {
        _initialNode = initial;
        _finalNode = final;
        return this;
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
