using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DoneLaserPlayerDetection : MonoBehaviour, IAlertRobotObservable
{
    private GameObject player;								// Reference to the player.
    private DoneLastPlayerSighting lastPlayerSighting;      // Reference to the global last sighting of the player.
    private List<IAlertRobotObserver> _myObservers = new List<IAlertRobotObserver>();
    void Awake ()
    {
        var NPCs = FindObjectsOfType<NPC>();
        NPCs.ToList().ForEach(x =>
        {
            AddObserverAlertRobot(x);
        });
       
        // Setting up references.
        player = GameObject.FindGameObjectWithTag(DoneTags.player);
        lastPlayerSighting = GameObject.FindGameObjectWithTag(DoneTags.gameController).GetComponent<DoneLastPlayerSighting>();
    }


    void OnTriggerStay(Collider other)
    {
        // If the beam is on...
        if (GetComponent<Renderer>().enabled)
            // ... and if the colliding gameobject is the player...
            if (other.gameObject == player) {
                
                // ... set the last global sighting of the player to the colliding object's position.
                lastPlayerSighting.position = other.transform.position;
                Trigger("AlertRobot");
            }

    }

    public void AddObserverAlertRobot(IAlertRobotObserver obs)
    {
        _myObservers.Add(obs);
    }

    public void RemoveObserverAlertRobot(IAlertRobotObserver obs)
    {
        if (_myObservers.Contains(obs))
        {
            _myObservers.Remove(obs);
        }
    }

    public void Trigger(string message)
    {
        Debug.Log("ENTRA EN TRIGGER DONE LASER??" + message);
        foreach (var observer in _myObservers)
        {
            observer.OnNotifyAlertRobot(message);
        }
    }
}