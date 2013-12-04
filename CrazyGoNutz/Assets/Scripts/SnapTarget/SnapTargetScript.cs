using UnityEngine;
using System.Collections;

public class SnapTargetScript : MonoBehaviour
{
	public bool isRoom = false;
	public bool isZone = false;
	public SnapTargetType snapTargetType;
	private SnapTarget snapTarget = null;

	public WorkerAnimation animation;
	
	void Awake () 
	{
		tag = "SnapTarget";
	}
	
	void Start()
	{
		if(snapTarget == null) snapTarget = GameController.AddSnapTarget(gameObject);	// TODO: This will change.
	}
	
	/////////////////////////// WORKER STUFF //////////////////////////////
	
	public bool CatchWorker(Worker worker)
	{
		if(snapTarget != null && snapTarget.currentWorker == null)
		{
			snapTarget.currentWorker = worker;
			return true;
		}
		return false;
	}
	
	/////////////////////////// GET VARS //////////////////////////////
	
	public bool isARoom()
	{
		return isRoom;
	}
	public bool isAZone()
	{
		return isZone;
	}
	public Vector3 GetPosition()
	{
		return transform.position;
	}
	public SnapTarget GetSnapTarget()	// Returns the SnapTarget Class Instance
	{
		return snapTarget;
	}
}
