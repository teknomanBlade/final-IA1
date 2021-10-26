using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAlarmNodeObservable 
{
    void AddObserverAlarmNode(IAlarmNodeObserver obs);
    void RemoveObserverAlarmNode(IAlarmNodeObserver obs);
    void TriggerAlarmNode(string message, Node node);
}
