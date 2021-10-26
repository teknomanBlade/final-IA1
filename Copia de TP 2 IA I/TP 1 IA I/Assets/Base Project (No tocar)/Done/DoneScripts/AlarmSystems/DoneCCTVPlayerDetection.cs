using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DoneCCTVPlayerDetection : MonoBehaviour, IAlertRobotObservable
{
    private Node node;
    private GameObject player;								// Reference to the player.
    private DoneLastPlayerSighting lastPlayerSighting;      // Reference to the global last sighting of the player.
    private List<IAlertRobotObserver> _myObservers = new List<IAlertRobotObserver>();
    void Awake ()
	{
        node = FindObjectOfType<Node>();
        var NPCs = FindObjectsOfType<NPC>();
        NPCs.ToList().ForEach(x =>
        {
            AddObserverAlertRobot(x);
        });
        // Setting up the references.
        player = GameObject.FindGameObjectWithTag(DoneTags.player);
        lastPlayerSighting = GameObject.FindGameObjectWithTag(DoneTags.gameController).GetComponent<DoneLastPlayerSighting>();
	}
	
	
	void OnTriggerStay (Collider other)
	{
		// If the colliding gameobject is the player...
		if(other.gameObject == player)
		{
            
            // ... raycast from the camera towards the player.
            Vector3 relPlayerPos = player.transform.position - transform.position;
			RaycastHit hit;

            if (Physics.Raycast(transform.position, relPlayerPos, out hit))
                // If the raycast hits the player...
                if (hit.collider.gameObject == player) {
                    // ... set the last global sighting of the player to the player's position.
                    lastPlayerSighting.position = player.transform.position;
                    Trigger("AlertRobot");
                }
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
        Debug.Log("ENTRA EN TRIGGER DONE CCTV??" + message);
        foreach (var observer in _myObservers)
        {
            observer.OnNotifyAlertRobot(message);
        }
    }
}
