using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DonePlayerMovement : MonoBehaviour, ITargetHeardObservable
{
	public AudioClip shoutingClip;		// Audio clip of the player shouting.
	public float turnSmoothing = 15f;	// A smoothing value for turning the player.
	public float speedDampTime = 0.1f;  // The damping for the speed parameter
    private float rangeSoundHearing;
    private bool shout;
    private bool footsteps;
	private Animator anim;				// Reference to the animator component.
	private DoneHashIDs hash;           // Reference to the HashIDs.
    protected List<ITargetHeardObserver> _myObservers = new List<ITargetHeardObserver>();

    void Awake ()
	{
        footsteps = false;
        var NPCs = FindObjectsOfType<NPC>();
        NPCs.ToList().ForEach(x =>
        {
            AddObserverTargetHeard(x);
        });
        // Setting up the references.
        anim = GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag(DoneTags.gameController).GetComponent<DoneHashIDs>();
        rangeSoundHearing = 3.5f;
        // Set the weight of the shouting layer to 1.
        anim.SetLayerWeight(1, 1f);
	}
	
	
	void FixedUpdate ()
	{
		// Cache the inputs.
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		bool sneak = Input.GetButton("Sneak");
		
		MovementManagement(h, v, sneak);
	}
	
	
	void Update ()
	{
		// Cache the attention attracting input.
	    shout = Input.GetButtonDown("Attract");
		
		// Set the animator shouting parameter.
		anim.SetBool(hash.shoutingBool, shout);
		
		AudioManagement(shout);
        SoundDetection();

    }
	
	
	void MovementManagement (float horizontal, float vertical, bool sneaking)
	{
		// Set the sneaking parameter to the sneak input.
		anim.SetBool(hash.sneakingBool, sneaking);
		
		// If there is some axis input...
		if(horizontal != 0f || vertical != 0f)
		{
			// ... set the players rotation and set the speed parameter to 5.5f.
			Rotating(horizontal, vertical);
			anim.SetFloat(hash.speedFloat, 5.5f, speedDampTime, Time.deltaTime);
		}
		else
			// Otherwise set the speed parameter to 0.
			anim.SetFloat(hash.speedFloat, 0);
	}
	
	
	void Rotating (float horizontal, float vertical)
	{
		// Create a new vector of the horizontal and vertical inputs.
		Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
		
		// Create a rotation based on this new vector assuming that up is the global y axis.
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
		
		// Create a rotation that is an increment closer to the target rotation from the player's rotation.
		Quaternion newRotation = Quaternion.Lerp(GetComponent<Rigidbody>().rotation, targetRotation, turnSmoothing * Time.deltaTime);
		
		// Change the players rotation to this new rotation.
		GetComponent<Rigidbody>().MoveRotation(newRotation);
	}
	
	
	void AudioManagement (bool shout)
	{
        // If the player is currently in the run state...
        if (anim.GetCurrentAnimatorStateInfo(0).nameHash == hash.locomotionState)
        {
            // ... and if the footsteps are not playing...
            if (!GetComponent<AudioSource>().isPlaying)
            {
                // ... play them.
                footsteps = true;
                GetComponent<AudioSource>().Play();
            }

        }
        else {
            footsteps = false;
            // Otherwise stop the footsteps.
            GetComponent<AudioSource>().Stop();
        }
		
		
		// If the shout input has been pressed...
		if(shout)
			// ... play the shouting clip where we are.
			AudioSource.PlayClipAtPoint(shoutingClip, transform.position);
	}

    public void SoundDetection() {
        var possibleEnemiesWhoHeardMe = Physics.OverlapSphere(transform.position, rangeSoundHearing, 1 << 13);

        if (possibleEnemiesWhoHeardMe.Length > 0 && (shout || footsteps))
        {
            
            Debug.Log("ENEMIES HEARD YOU: " + possibleEnemiesWhoHeardMe.Length);
            foreach (Collider enemy in possibleEnemiesWhoHeardMe)
            {
                TriggerTargetHeard("TargetHeard");
                Debug.Log("EL ENEMIGO TE ESCUCHÓ??");
            }

        }
        else {
            TriggerTargetHeard("OutOfHearingRange");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, rangeSoundHearing);
    }

    public void AddObserverTargetHeard(ITargetHeardObserver obs)
    {
        _myObservers.Add(obs);
    }

    public void RemoveObserverTargetHeard(ITargetHeardObserver obs)
    {
        if (_myObservers.Contains(obs))
        {
            _myObservers.Remove(obs);
        }
    }

    public void TriggerTargetHeard(string message)
    {
        foreach (var observer in _myObservers)
        {
            observer.OnNotifyTargetHeard(message);
        }
    }
}
