using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCState : State
{
    protected NPC _owner;

    public NPCState(FSM fsm, NPC owner) : base(fsm)
    {
        _owner = owner;
    }

}
