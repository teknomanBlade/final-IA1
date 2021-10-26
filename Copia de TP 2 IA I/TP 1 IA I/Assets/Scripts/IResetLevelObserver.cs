using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResetLevelObserver 
{
    void OnNotifyResetLevel(string message);
}
