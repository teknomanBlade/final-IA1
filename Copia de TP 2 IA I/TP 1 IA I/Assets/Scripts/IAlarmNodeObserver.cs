using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAlarmNodeObserver
{
    void OnNotifyAlarmNode(string message, Node node);
}
