using UnityEngine;
using System.Collections;

/*
	WorkerScript goes on the worker GameObjects within the scene
	
	-Animator
	-Handles animation

*/

public class WorkerScript : MonoBehaviour 
{
	// Worker
	Worker worker = null;
	
	//private bool mouseOver = false;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		/*if(gameObject.layer == 2)
		{
			if(worker.GetSnapTarget() == null) gameObject.layer = 0;
		}*/
	}
	
	/////////////////////////// GET VARS //////////////////////////////
	
	/*public Vector3 GetPosition()
	{
		return transform.position;
	}*/

	public Worker GetWorker()	// Returns the Worker Class Instance
	{
		return worker;
	}
	public void SetWorker(Worker worker)	// Sets the Worker Class Instance
	{
		this.worker = worker;
	}
}
