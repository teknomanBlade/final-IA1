using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResetLevelObservable 
{
    void AddObserverResetLevel(IResetLevelObserver obs);
    void RemoveObserverResetLevel(IResetLevelObserver obs);
    void Trigger(string message);
}
