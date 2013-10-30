using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour 
{
	/*
	 -Hold List of Workers
	 -Handles Current Task
	 -Handles Completion Meter
	 -Spawns Roadblocks?
	 -Handles GUI
	 */
	
	// Statics
	static private List<Worker> workers = new List<Worker>();
	static private List<SnapTarget> snaptargets = new List<SnapTarget>();
	
	// Component Reference
	CameraRaycaster cameraRaycaster = null;
	
	// Worker Prefab
	public GameObject workerPrefab = null;
	
	// MouseOver
	//GameObject mouseOverWorker = null;
	
	// Ratios and Rates
	private const float INCREASE = 1.0f;
	private const float SLIGHT_INCREASE = 0.25f;
	private const float DECREASE = -1.0f;
	private const float SLIGHT_DECREASE = -0.25f;
	
	// Use this for initialization
	void Start () 
	{
		cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
		
		// Create Worker GameObjects
		SpawnWorkers();
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateWorkers();	// Tells workers to do work, and other workstation stuff
	}
	
	void OnGUI()
	{
		// DrawWorkerStats();		// Loops through workers and draws their current stats
	}
	
	/////////////////////////// UPDATE WORKERS //////////////////////////////
	
	private void UpdateWorkers()
	{
		foreach(Worker worker in workers)
		{
			SnapTarget currentSnapTarget = worker.GetSnapTarget();
			if(currentSnapTarget != null)
			{
				SnapTargetType type = currentSnapTarget.GetType();
				switch(type)
				{
					case SnapTargetType.Conference:
						worker.AdjustCommunication(INCREASE * Time.deltaTime);
						break;
					case SnapTargetType.Recreation:
						worker.AdjustFrustration(DECREASE * Time.deltaTime);
						worker.AdjustCommunication(SLIGHT_INCREASE * Time.deltaTime);
						break;
					case SnapTargetType.Workstation:
						worker.AdjustFrustration(INCREASE * Time.deltaTime);
						worker.AdjustCommunication(SLIGHT_DECREASE * Time.deltaTime);
						break;
				}
			}
		}
	}
	
	/////////////////////////// MOUSE INPUT //////////////////////////////
	
	static public void MouseOver(GameObject mouseOverObj)
	{
		// set all workers.mouseOver to false;
		foreach(SnapTarget snap in snaptargets) snap.SetMouseOver(false);
		foreach(Worker worker in workers) worker.SetMouseOver(false);
		// check workers List for mouseOverObj
		// if found, set .mouseOver to true
		foreach(SnapTarget snap in snaptargets) if(mouseOverObj == snap.workStation) snap.SetMouseOver(true);
		foreach(Worker worker in workers) if(mouseOverObj == worker.gameObject) worker.SetMouseOver(true);
	}
	
	/////////////////////////// SPAWN WORKERS //////////////////////////////
	
	private void SpawnWorkers()
	{
		if(workers.Count == 0)
		{
			for(int i = 0; i < 6; i++)
			{
				GameObject obj = null;
				Vector3 pos = new Vector3();
				pos.x = Random.Range(-2.0f, 2.0f);
				pos.y = 0.5f;
				pos.z = Random.Range(-2.0f, 2.0f);
				obj = Instantiate(workerPrefab, pos, Quaternion.identity) as GameObject;
				
				WorkerScript script = obj.GetComponent<WorkerScript>();
				
				script.SetWorker( AddWorker(obj) );
			}
		}
	}
	
	/////////////////////////// WORKER LIST //////////////////////////////
	
	static public Worker AddWorker(GameObject newWorker)
	{
		Worker worker = null;
		int rand = Random.Range(0,3);
		worker = new Worker(newWorker, (WorkerType)rand);
		workers.Add(worker);
		
		Debug.Log("GameController -> workers.Count = " + workers.Count);
		
		return worker;
	}
	
	/////////////////////////// SNAPTARGET LIST //////////////////////////////
	
	static public SnapTarget AddSnapTarget(GameObject newSnapTarget)
	{
		SnapTarget snapTarget = null;
		SnapTargetType type = SnapTargetType.Workstation;
		bool isRoom = false;
		if(newSnapTarget)
		{
			SnapTargetScript script = newSnapTarget.GetComponent<SnapTargetScript>();
			if(script != null)
			{
				type = script.snapTargetType;
				isRoom = script.isRoom;
			}
		}
		
		snapTarget = new SnapTarget(newSnapTarget, type, isRoom);
		snaptargets.Add(snapTarget);
		
		Debug.Log("GameController -> snaptargets.Count = " + snaptargets.Count);
		
		return snapTarget;
	}
	
	/////////////////////////// FIND OPEN SNAPTARGET  //////////////////////////////
	
	static public SnapTarget FindOpenSnapTargetOfType(SnapTargetType type)
	{
		foreach(SnapTarget snapTarget in snaptargets) 
			if(snapTarget != null && !snapTarget.isRoom() && snapTarget.MatchesType(type) && snapTarget.isEmpty()) return snapTarget;
		
		return null;
	}
	
}
