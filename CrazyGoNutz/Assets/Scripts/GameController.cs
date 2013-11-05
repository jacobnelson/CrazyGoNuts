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
	
	static public int Programmers = 0;
	static public int Artists = 0;
	static public int AudioDesigners = 0;
	static public int TotalWorkers = 0;
	
	static public float deadlineMax = 480f;
	
	// Component Reference
	CameraRaycaster cameraRaycaster = null;
	
	// Worker Prefab
	public GameObject workerPrefab = null;
	
	// MouseOver
	//GameObject mouseOverWorker = null;
	
	// Ratios and Rates
	public const float INCREASE = 1.0f;
	public const float SLIGHT_INCREASE = 0.25f;
	public const float DECREASE = -2.0f;
	public const float SLIGHT_DECREASE = -0.25f;
	
	private const float WORK_RATE = 1.0f;
	
	// Productivity Per Sec
	
	
	// Completion Bar
	private float completion = 0.0f;				// TODO: This is only temporary
	private Rect completionBarRect = new Rect(0,0,100,16);
	private float currentTaskProgramming = 0.0f;	// TODO: This is only temporary
	private float currentTaskArt = 0.0f;			// TODO: This is only temporary
	
	// Current Task
	private List<Task> taskList = new List<Task>();
	private Task currentTask = null;
	private int currentTaskIndex = 0;
	
	// Deadline
	private float gameLengthMax = 480f;	// total length of game before losing, in seconds	// 8min?
	private float deadlineCurrent = 0f;	// current length of the game
	
	// Textures
	public Texture2D solidColorTex = null;
	
	// Use this for initialization
	void Start () 
	{
		cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
		
		deadlineMax = gameLengthMax;
		
		// Create Worker GameObjects
		SpawnWorkers();
		CountWorkers();
		
		// Create Tasks
		GenerateTaskList();

	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateWorkers();	// Tells workers to do work, and other workstation stuff
		
		// Update Task
		UpdateCurrentTask();
		
		// Calculate Completion
		CalculateCompletion();
		
		// Deadline
		UpdateDeadline();
	}
	
	void OnGUI()
	{
		// DrawWorkerStats();		// Loops through workers and draws their current stats
		
		// Completion Meter
		DrawCompletionMeter();

		
		// Temp for Debug
		GUI.Label( new Rect(10, 24, 256, 24), "completion: " + completion);
		GUI.Label( new Rect(10, 48, 256, 24), "deadline: " + deadlineCurrent.ToString("f2") + " / " + gameLengthMax);
		if(currentTask != null)
		{
			GUI.Label( new Rect(10, 72, 256, 24), "currentTask Programming: " + currentTask.programming.ToString("f2") + " / " + currentTask.programmingReq.ToString("f2"));
			GUI.Label( new Rect(10, 96, 256, 24), "currentTask Art: " + currentTask.art.ToString("f2") + " / " + currentTask.artReq.ToString("f2"));
			GUI.Label( new Rect(10, 120, 256, 24), "currentTask Sound: " + currentTask.sound.ToString("f2") + " / " + currentTask.soundReq.ToString("f2"));
		}
		
		// Debug mouseOverWorker stats
		if(cameraRaycaster != null && cameraRaycaster.mouseOverWorker != null)
		{
			Worker worker =  cameraRaycaster.mouseOverWorker;
			GUI.Label( new Rect(Screen.width - 256, Screen.height - 86, 256, 24), "WorkerType: " + worker.GetWorkerType());
			GUI.Label( new Rect(Screen.width - 256, Screen.height - 72, 256, 24), "Communication: " + worker.GetCommunication());
			GUI.Label( new Rect(Screen.width - 256, Screen.height - 48, 256, 24), "Frustration: " + worker.GetFrustration());
			GUI.Label( new Rect(Screen.width - 256, Screen.height - 24, 256, 24), "Productivity: " + worker.GetProductivity());
		}
	}
	
	/////////////////////////// UPDATE WORKERS //////////////////////////////
	
	private void UpdateWorkers()
	{
		foreach(Worker worker in workers)
		{
			worker.UpdateStats();
			if(worker.AtWorkstation()) WorkOnCurrentTask( worker.GetWorkerType(), worker.ProductivityPercent() );
		}
	}
	
	/////////////////////////// TASKS //////////////////////////////
	
	private void UpdateCurrentTask()
	{
		// Check if currentTask is Complete
		if(currentTask != null && currentTask.Complete())
		{
			// Check Milestone?
			currentTaskIndex++;
			currentTask = null;
		}
		
		if(currentTask == null && currentTaskIndex < taskList.Count) currentTask = taskList[currentTaskIndex];
		
		if(currentTask != null) currentTask.Update();
	}
	private void GetCurrentTask()	// Get taskList[0], then remove it from list
	{
		
		/*if(taskList.Count > 0)
		{
			currentTask = taskList[0];
			taskList.RemoveAt(0);
		}*/
	}
	
	private void GenerateTaskList()
	{
		float totalCompletion = 100;
		
		while(totalCompletion > 0)
		{
			// 5-15 at 360 secs means 18-54secs for a task, that is a max, so youd need to complete each task before that amount of time
			float weight = Random.Range(5,15);				// Get Random TaskWeight, this is a percentage of total work, not an amount of time
			float difference = totalCompletion - weight;	// Find Difference
			
			if(difference < 0) weight = totalCompletion;	// If taskweight is greater than total completion, weight = total completion
			totalCompletion -= weight;
			
			Task task = new Task(weight);	
			
			taskList.Add(task);
		}
		
		Debug.Log("GenerateTaskList -> Count is " + taskList.Count);
	}
	
	private void WorkOnCurrentTask(WorkerType type, float productivity)
	{
		if(currentTask ==  null) return;
		
		float workamount = productivity * WORK_RATE * Time.deltaTime;
		
		currentTask.WorkOn(type, workamount);
		
		/*switch(type)
		{
			case WorkerType.Artist:
				currentTaskArt += 
				
				break;
			case WorkerType.Programmer:
				currentTaskProgramming += productivity * WORK_RATE * Time.deltaTime;
				break;
			case WorkerType.AudioDesigner:
				
				break;
		}*/
	}
	
	/////////////////////////// COMPLETION METER //////////////////////////////
	
	private void CalculateCompletion()
	{
		completion = 0;
		foreach(Task task in taskList) completion += task.GetCompletionAmount();	// Gets task completion percent * taskWeight
		
		//completion /= taskList.Count;
		
		if(completion >= 100) {   } //YOU WIN!! 
	}
	
	/////////////////////////// DRAW COMPLETION METER AND CURRENT TASK //////////////////////////////
	
	private void DrawCompletionMeter()
	{
		// Draw Meter BG
		
		// Draw Completion Bar
		completionBarRect.width = Screen.width * (completion / 100f);
		GUI.color = Color.green;
		GUI.DrawTexture( completionBarRect, solidColorTex);
		
		// Draw Milestones
		for(int i = 0; i < taskList.Count; i++)
		{
			GUI.color = Color.black;
			Rect rect = new Rect((taskList[i].GetTaskWeight() * 0.01f) * (i + 1) * Screen.width,0,2,16);
			GUI.DrawTexture( rect, solidColorTex);
		}
		
		// Draw Deadline
		GUI.color = Color.red;
		Rect deadlineRect =  new Rect( ((float)deadlineCurrent / (float)deadlineMax) * Screen.width,0,4,32);
		GUI.DrawTexture(deadlineRect , solidColorTex);
		
		GUI.color = Color.white;
	}
	
	private void DrawCurrentTask()
	{
		// Draw Meter BG
		
		// Draw Completion Bar
		//completionBarRect.width = Screen.width * (completion / 100f);
		//GUI.DrawTexture( completionBarRect, solidColorTex);
		
		// Draw Milestones
	}
	
	/////////////////////////// DEADLINE //////////////////////////////
	
	private void UpdateDeadline()
	{
		deadlineCurrent += Time.deltaTime;
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
				
				// type, make sure at least one of each class
				int type = i;
				if(type > 2) type = Random.Range(0,3);
				
				script.SetWorker( AddWorker(obj, (WorkerType)type) );
			}
		}
	}
	
	/////////////////////////// WORKER LIST //////////////////////////////
	
	static public Worker AddWorker(GameObject newWorker, WorkerType type)
	{
		Worker worker = null;
		worker = new Worker(newWorker, type);
		workers.Add(worker);
		
		return worker;
	}
	
	static public void CountWorkers()
	{
		Artists = 0;
		Programmers = 0;
		AudioDesigners = 0;
		TotalWorkers = 0;
		foreach(Worker worker in workers)
		{
			switch(worker.GetWorkerType())
			{
				case WorkerType.Artist:
					Artists++;
					break;
				case WorkerType.Programmer:
					Programmers++;
					break;
				case WorkerType.AudioDesigner:
					AudioDesigners++;
					break;
			}
		}
		TotalWorkers = workers.Count;
		
		Debug.Log("Workers: " + workers.Count + "; Programmers " + Programmers + "; Artists " + Artists + "; AudioDesigners " + AudioDesigners);
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
		
		//Debug.Log("GameController -> snaptargets.Count = " + snaptargets.Count);
		
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
