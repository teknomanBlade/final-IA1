using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : NPCState
{
    private Animator _anim;

    public StateIdle(FSM fsm, NPC owner) : base(fsm, owner)
    {
        _anim = owner.GetComponent<Animator>();
    }


    public override void Awake()
    {
        Debug.Log("START IDLE STATE...");
    }

    public override void Execute()
    {
        _anim.SetFloat("Speed", 0f);
    }

    public override void ExecuteFixed()
    {

    }

    public override void Sleep()
    {
        Debug.Log("END IDLE STATE...");
    }
}
