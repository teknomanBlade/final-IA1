using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    private FSM _myFSM;

    public State(FSM fsm)
    {
        _myFSM = fsm;
    }

    public abstract void Awake();
    public abstract void Execute();
    public abstract void ExecuteFixed();
    public abstract void Sleep();


}
