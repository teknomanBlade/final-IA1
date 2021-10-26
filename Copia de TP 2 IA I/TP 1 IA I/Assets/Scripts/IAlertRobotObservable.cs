using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAlertRobotObservable
{
    void AddObserverAlertRobot(IAlertRobotObserver obs);
    void RemoveObserverAlertRobot(IAlertRobotObserver obs);
    void Trigger(string message);
}
