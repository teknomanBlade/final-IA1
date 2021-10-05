using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : NodeDTree
{
    public delegate void Action();
    public Action _action;

    public ActionNode(Action action)
    {
        _action = action;
    }

    public override void Execute()
    {
        _action();
    }
}
