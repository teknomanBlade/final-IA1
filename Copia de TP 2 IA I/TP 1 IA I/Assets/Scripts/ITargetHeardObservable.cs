using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetHeardObservable
{
    void AddObserverTargetHeard(ITargetHeardObserver obs);
    void RemoveObserverTargetHeard(ITargetHeardObserver obs);
    void TriggerTargetHeard(string message);
}
