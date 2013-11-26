using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour 
{
	/*
		This Script is placed on the GameController and handles Game Flow
	 		-Holds static vars
	 		-Holds consts
	 		-Handles Completion
	 		-Handles Deadline
	 		-Handles Milestones
	 		-Handles GUI
	 */
	
	// Statics
	static private List<Worker> workers = new List<Worker>();
	//static private List<GameObject> workerObjects = new List<GameObject>();
	static private List<SnapTarget> snaptargets = new List<SnapTarget>();
	
	static public int Programmers = 0;
	static public int Artists = 0;
	static public int AudioDesigners = 0;
	static public int TotalWorkers = 0;
	
	
	
	// Component Reference
	CameraRaycaster cameraRaycaster = null;
	
	// Object Reference
	GameObject ConferenceRoomObj = null;
	Vector3 ConferenceRoomPos = new Vector3();
	
	// Worker Prefab
	public GameObject workerPrefab = null;
	
	// Number of Workers per Room
	static public int WorkersInWorkroom = 0;
	static public int WorkersInRecreation = 0;
	static public int WorkersInConference = 0;
	
	// Ratios and Rates
	public const float INCREASE = 1.0f;
	public const float SLIGHT_INCREASE = 0.25f;
	public const float DECREASE = -2.0f;
	public const float SLIGHT_DECREASE = -0.25f;
	
	public const float WORK_RATE = 0.6f;	// How much work a Worker does per second
	public const float TARGET_WORK_PERCENTAGE = 0.2f;	// Percentage of work capacity needed to keep up with deadline
		
	public const float MILESTONE_MOOD_INCREASE = 10.0f;
	public const float MEETING_MOOD_INCREASE = 20.0f;
	
	// Productivity Per Sec
	private float totalProductivity = 0f;
	private float artProductivity = 0f;
	private float programmingProductivity = 0f;
	private float soundProductivity = 0f;
	
	private float lastTotalProductivity = 0f;
	private float lastArtProductivity = 0f;
	private float lastProgrammingProductivity = 0f;
	private float lastSoundProductivity = 0f;
	
	private float productivityCounter = 0f;	// Counts per sec
	
	// Completion Bar
	private float completion = 0.0f;				// TODO: This is only temporary
	private Rect completionBarRect = new Rect(0,0,100,16);
	private float currentTaskProgramming = 0.0f;	// TODO: This is only temporary
	private float currentTaskArt = 0.0f;			// TODO: This is only temporary
	
	// Current Task
	private List<Task> taskList = new List<Task>();
	private Task currentTask = null;
	private int currentTaskIndex = 0;
	private float taskDelayCounter = 0f;
	
	// Meeting
	private Meeting meeting = null;
	bool tab = false;
	
	// Milestones
	//private List<Milestone> milestones = new List<Milestone>();
	static public List<Milestone> milestones = new List<Milestone>();
	//private Task currentTask = null;
	//private int currentTaskIndex = 0;
	
	// Deadline
	static public float deadlineMax = 840f;//840f; // total length of game before losing, in seconds	// 8min?, 14min? //TODO: Adjust this.	
	private float deadlineCurrent = 0f;	// current length of the game
	private float initialDeadlineDelay = 5f;	//in seconds
	
	// Team Badges
	/*private TeamBadge badge01 = null;
	private TeamBadge badge02 = null;
	private TeamBadge badge03 = null;*/
	
	// Textures
	public Texture2D solidColorTex = null;
	
	// GUIStyles
	public GUIStyle mouseOverStyle;
	public GUIStyle floatingTextStyle;
	
	// PerSecond Counter
	private float counter = 0;
	
	// Use this for initialization
	void Start () 
	{
		cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
		
		//deadlineMax = gameLengthMax;
		//if(workers.Count == 0) workers = new List<Worker>();
		if(workers.Count == 0) workers = CharSelectScript.selectedworkers;	// Get worker class objects from CharSelectScript
		Debug.Log("workers.Count " + workers.Count);
		
		// Create Worker GameObjects
		if(workers.Count == 0) SpawnWorkers(true);
		else SpawnWorkers(false);
		CountWorkers();
		
		// Create Tasks
		GenerateTaskList();
		
		// Find Rooms
		ConferenceRoomObj = GameObject.Find("ConferenceRoom");
		
		// Create Team Badges
		//CreateTeamBadges();
		TeamBadge.CreateTeamBadges();
		
		FloatingText.style = floatingTextStyle;
	}
	
	// Update is called once per frame
	void Update () 
	{	
		UpdateProductivityRates();
		
		// Update and Count Workers Per Room
		UpdateWorkers();			// Tells workers to do work, and other workstation stuff
		
		// Updates once per second
		counter += Time.deltaTime;
		if(counter >= 1f)
		{
			CountWorkersPerRoom();		// Finds out which room the workers are in
		}
		
		// Update Task
		UpdateCurrentTask();
		
		// Calculate Completion
		CalculateCompletion();
		
		// Check Milestones
		CheckMilestones();
		
		// Deadline
		UpdateDeadline();
		
		if(Input.GetKey(KeyCode.Tab)) tab = true;
		else tab = false;
	}
	
	void OnGUI()
	{
		// DrawWorkerStats();		// Loops through workers and draws their current stats
		
		// Completion Meter
		DrawCompletionMeter();
		
		// Meeting
		DrawMeetingMeter();
		CheckMeeting();
		
		// StatsAnalysis
		if(tab)StatsAnalysis.OnGUI();
		
		// Team Badges
		//TeamBadge.DrawTeamBadges(new Vector2( (Screen.width  * 0.5f) - (TeamBadge.teamBadges.Count * 64f * 0.5f),64f), 64f,64f);
		float badgesize = 48f;
		TeamBadge.DrawTeamBadges(new Vector2( (Screen.width  * 0.5f) - (TeamBadge.teamBadges.Count * badgesize * 0.5f),badgesize), badgesize + 2f,badgesize);
		
		
		// Temp for Debug
		string avgProdNeeded = " (" + (WORK_RATE * TotalWorkers * TARGET_WORK_PERCENTAGE).ToString("f2") + ")";
		GUI.Label( new Rect(10, 24, 256, 24), "completion: " + completion.ToString("f2") + " (+" + lastTotalProductivity.ToString("f2") + ")" + avgProdNeeded);
		GUI.Label( new Rect(10, 48, 256, 24), "deadline: " + deadlineCurrent.ToString("f2") + " / " + deadlineMax);
		if(currentTask != null)
		{
			string text = "currentTask Programming: " + currentTask.programming.ToString("f2") + " / " + currentTask.programmingReq.ToString("f2") + " (+" + lastProgrammingProductivity.ToString("f2") + ")";
			GUI.Label( new Rect(10, 72, 512, 24), text);
			text = "currentTask Art: " + currentTask.art.ToString("f2") + " / " + currentTask.artReq.ToString("f2") + " (+" + lastArtProductivity.ToString("f2") + ")";
			GUI.Label( new Rect(10, 96, 512, 24), text);
			text = "currentTask Sound: " + currentTask.sound.ToString("f2") + " / " + currentTask.soundReq.ToString("f2") + " (+" + lastSoundProductivity.ToString("f2") + ")";
			GUI.Label( new Rect(10, 120, 512, 24), text);
		}
		
		// Draw MouseOver or Selected Worker Stats
		DrawWorkerStats();
	
		// Floating Text
		FloatingText.DrawText();
		Banner.DrawText();
		
		// Draw Tooltip
		DrawTooltips();
		
		//Drawing.DrawLine( new Vector2(50,50), new Vector2(50,250), 2f);
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

			Banner.AddScalingBanner("Task Complete!!", 82f, new Vector3(Screen.width /2, Screen.height/2 - 256f,0), 0.55f, Interpolate.EaseType.EaseOutSine, new float[]{0,0.85f,1f}, new float[]{1f,0f,2.5f},1f);
			taskDelayCounter -= Time.deltaTime;
		}
		
		if(currentTask == null && currentTaskIndex < taskList.Count && taskDelayCounter <= 0) currentTask = taskList[currentTaskIndex];
		
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
	
	private void GenerateTaskList()	// Generate a List of Tasks with random Weights
	{
		/*
			How Tasks are Generated
			
			- First, we assume that a project is percent based, meaning 100% is completed.
			- Then, we divide 100% into chunks of 5%-15%, meaning there will be between 7 to 20 Tasks.
			- When a task is created, we pass it's weight into it. Then we divid this weight by deadlineMax(the length of the game)
				 to find how many secs it will take to complete.
		*/
		float totalCompletion = 100;
		float currentPos = 0;
		while(totalCompletion > 0)
		{
			// 5-15 at 360 secs means 18-54secs for a task, that is a max, so youd need to complete each task before that amount of time
			float weight = Random.Range(5,15);				// Get Random TaskWeight, this is a percentage of total work, not an amount of time
			float difference = totalCompletion - weight;	// Find Difference
			
			if(difference < 0) weight = totalCompletion;	// If taskweight is greater than total completion, weight = total completion
			totalCompletion -= weight;
			
			Task task = new Task(weight);	
			
			taskList.Add(task);
			
			// Generate Milestones
			if(currentPos > 0)
			{
				Milestone milestone = new Milestone(currentPos);
				milestones.Add(milestone);
			}
			
			currentPos += weight;
			//Debug.Log("GenerateTaskList -> Task" + taskList.Count + " weight is " + weight);
		}
		
		//Debug.Log("GenerateTaskList -> Count is " + taskList.Count + ", with " + milestones.Count + "milestones");
	}
	
	private void WorkOnCurrentTask(WorkerType type, float productivity)
	{
		if(currentTask ==  null) return;
		
		float workamount = productivity * WORK_RATE * Time.deltaTime;
		
		currentTask.WorkOn(type, workamount);
		
		AdjustProductivityRates(type, workamount);
	}
	
	/////////////////////////// MEETING //////////////////////////////
	
	private void DrawMeetingMeter()
	{
		if(meeting != null)
		{
			// Get World Pos of ConferenceRoom to draw gui near it, this is temporary.
			if(ConferenceRoomPos == Vector3.zero) ConferenceRoomPos = Camera.main.WorldToScreenPoint(ConferenceRoomObj.transform.position);
			
			Rect meetingRect = new Rect(ConferenceRoomPos.x, Screen.height - ConferenceRoomPos.y, 128f, 10f);
			GUI.Label( new Rect(meetingRect.x, meetingRect.y - 18, meetingRect.width, 24), "Meeting");
			
			// Draw Meter BG
			GUI.color = Color.grey;
			GUI.DrawTexture( meetingRect, solidColorTex);
			// Draw Meter
			GUI.color = Color.cyan;
			meetingRect.width = (meeting.completion / meeting.completionMax) * meetingRect.width;
			GUI.DrawTexture( meetingRect, solidColorTex);
			
			GUI.color = Color.white;
		}
	}
	
	private void WorkOnCurrentMeeting(float amount)
	{
		meeting.completion += amount;
	}
	private void CheckMeeting()
	{
		if(meeting != null && meeting.completion >= meeting.completionMax)
		{
			AdjustGroupMood(MEETING_MOOD_INCREASE);
			StatsAnalysis.completedMeetings += 1;
			meeting = null;
		}
	}
	private void SpawnMeeting()
	{
		meeting = new Meeting(50f * WORK_RATE);		//TODO: Adjust this as needed.
	}
	
	/////////////////////////// COMPLETION METER //////////////////////////////
	
	private void CalculateCompletion()
	{
		completion = 0;
		foreach(Task task in taskList) completion += task.GetCompletionAmount();	// Gets task completion percent * taskWeight
		
		if(completion >= 100) //YOU WIN!! 
		{ 
			completion = 100;
			EndGame(true);
		} 
	}
	
	/////////////////////////// CHECK MILESTONES //////////////////////////////
	
	private void CheckMilestones()	// Check if a Milestone is Achieved or Failed
	{
		foreach(Milestone milestone in milestones) 
		{
			if(!milestone.Complete()) 
			{
				milestone.Check(completion, ((float)deadlineCurrent / (float)deadlineMax) * 100);
				if(milestone.Complete())
				{
					if(milestone.Failed()) { SpawnMeeting(); }	// Spawn a Meeting
					if(milestone.Achieved())  // Reduce Frustration, 25% chance for Meeting?
					{ 
						AdjustGroupMood(MILESTONE_MOOD_INCREASE); 
						if(Random.Range(0,100) < 25) SpawnMeeting();
					}	
				}
			}
		}
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
		for(int i = 0; i < milestones.Count; i++)
		{
			if(!milestones[i].Complete()) GUI.color = Color.white;
			else if(milestones[i].Failed()) GUI.color = Color.red;
			else GUI.color = Color.blue;
			Rect rect = new Rect((milestones[i].position * 0.01f * Screen.width),0,2,16);	// TODO: Fix this.
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
	
	/////////////////////////// TOOLTIPS //////////////////////////////
	
	private void DrawTooltips()
	{
		if(GUI.tooltip.Length > 0)
		{
			float height = GUI.tooltip.Length / 64f * 24f + 24f;
			float width = 352f;
			float x = Input.mousePosition.x + 16;
			float y = Screen.height - Input.mousePosition.y;
			
			if(x + width > Screen.width) x = Screen.width - width - TeamBadgeTextures.textures.marqueeStyle.border.right * 2;
			
			//GUI.Box(new Rect(Input.mousePosition.x + 16, Screen.height - Input.mousePosition.y, 352, 96), "", TeamBadgeTextures.textures.marqueeStyle);
			GUI.Box(new Rect( x, y, width, height), "", TeamBadgeTextures.textures.marqueeStyle);
			GUI.Label(new Rect( x, y, width, 96), GUI.tooltip);
		}
	}
	
	/////////////////////////// DEADLINE //////////////////////////////
	
	// ------------------------ Update Deadline -------------------------------
	private void UpdateDeadline()
	{
		initialDeadlineDelay -= Time.deltaTime;	// Delay before the deadline starts counting down
		if(initialDeadlineDelay <= 0) deadlineCurrent += Time.deltaTime;
		
		if(deadlineCurrent >= deadlineMax) //YOU WIN!! 
		{ 
			deadlineCurrent = deadlineMax;
			EndGame(false);
		}
	}
	
	/////////////////////////// END GAME //////////////////////////////
	
	private void EndGame(bool win)
	{
		StatsAnalysis.completionNum = completion;
		StatsAnalysis.deadlineNum = deadlineCurrent;
		StatsAnalysis.milestones = milestones;
		Debug.Log("GameController.EndGame() -> Win " + win);
		Application.LoadLevel("EndScene");
	}
	
	/////////////////////////// PRODUCTIVITY RATES //////////////////////////////
	
	private void UpdateProductivityRates()
	{
		productivityCounter += Time.deltaTime;
		if(productivityCounter > 1f)
		{
			lastTotalProductivity = totalProductivity;
			lastArtProductivity = artProductivity;
			lastProgrammingProductivity = programmingProductivity;
			lastSoundProductivity = soundProductivity;
			
			// Add to StatsAnalysis.productivityPerSec
			StatsAnalysis.productivityPerSec.Add( new GraphEntry(totalProductivity, deadlineCurrent));
			
			StatsAnalysis.deadline.Add( new GraphEntry(0, deadlineCurrent));
			StatsAnalysis.completion.Add( new GraphEntry(completion - (deadlineCurrent / deadlineMax * 100), deadlineCurrent));
			
			ResetProductivityRates();
			productivityCounter = 0f;
		}
	}
	private void ResetProductivityRates()
	{
		totalProductivity = 0;
		artProductivity = 0;
		programmingProductivity = 0;
		soundProductivity = 0;
	}
	private void AdjustProductivityRates(WorkerType type, float amount)
	{
		switch(type)
		{
			case WorkerType.Artist:
				artProductivity += amount;
				break;
			case WorkerType.Programmer:
				programmingProductivity += amount;
				break;
			case WorkerType.AudioDesigner:
				soundProductivity += amount;
				break;
		}
		totalProductivity += amount;
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
	
	
	
	/////////////////////////// GROUP EFFECTS //////////////////////////////
	
	private void AdjustGroupMood(float howMuch)
	{
		foreach(Worker worker in workers) worker.AdjustMood(howMuch);
	}
	/*private void ReduceGroupFrustration(float howMuch)
	{
		foreach(Worker worker in workers) worker.AdjustMood(howMuch);
	}*/
	/*private void AdjustGroupCommunication(float howMuch)
	{
		foreach(Worker worker in workers) worker.AdjustMood(howMuch);
	}*/
	
	/////////////////////////// WORKERS  //////////////////////////////
	
	// ------------------------ Update Workers -------------------------------
	
	private void UpdateWorkers()
	{
		foreach(Worker worker in workers)
		{
			worker.UpdateStats();	// Calc roadblock chance
			if(worker.AtWorkstation() && !worker.Roadblocked())
			{
				WorkOnCurrentTask( worker.GetWorkerType(), worker.GetMood() * 0.01f );	// If worker at a workerstation, add to current task
			}
			else if(worker.InConferenceRoom() && WorkersInConference > 1)	// If this worker in conference and not alone, add to meeting.completion
			{
				if(meeting != null) WorkOnCurrentMeeting( WORK_RATE * Time.deltaTime );
			}
		}
	}
	
	// ------------------------ Draw Workers -------------------------------
	
	private void DrawWorkerStats()
	{
		Worker worker = null;
		if(cameraRaycaster != null)
		{
			if(cameraRaycaster.mouseOverWorker != null) worker =  cameraRaycaster.mouseOverWorker;
			else if(cameraRaycaster.selectedworker != null) worker =  cameraRaycaster.selectedworker;
		}
		
		if(worker != null)
		{
			Vector2 offset = new Vector2(Screen.width - 320f, Screen.height - 256f);
			mouseOverStyle.fontSize = 24;
			GUI.Label( new Rect(offset.x, offset.y, 256, 24), "" + worker.name, mouseOverStyle);
			mouseOverStyle.fontSize = 20;
			GUI.Label( new Rect(offset.x, offset.y + 32f, 256, 24), "" + worker.GetWorkerType(), mouseOverStyle);
			
			// Draw Stars
			Texture2D starbg = Textures.textures.starsbg;	// 480x96, 240x48, 160x32
			Texture2D stars = Textures.textures.stars;
			if(starbg && stars)
			{
				//Rect rect = new Rect(offset.x + (320f * 0.5f - 112f),  offset.y + 64f, 160f, 32f);
				Rect rect = new Rect(offset.x + (320f * 0.5f - 112f),  offset.y + 64f, 160f, 32f);
				GUI.DrawTexture( rect, starbg, ScaleMode.ScaleAndCrop);
				Rect rect2 = new Rect(0, 0, 1, 1);
				rect2.width *= (worker.starfloat / 5f);
				rect.width *= (worker.starfloat / 5f);
				GUI.DrawTextureWithTexCoords( rect, stars, rect2);
			}
			
			Texture2D texture = worker.GetMoodTexture();
			if(texture != null) 
			{
				Rect rect = new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 100f, 64f, 64f);
				GUI.DrawTexture( rect, texture);
				float mood = worker.GetMood();
				GUI.Label(rect, new GUIContent("", worker.GetCurrentMood() + " (" + mood.ToString("f0") + "/100)"));
			}
			/*float mood = worker.GetMood();
			if(mood > 66f) GUI.DrawTexture( new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 64f, 64f, 64f), MoodTextures.textures.happy);
			else if(mood <= 66f && mood > 33f) GUI.DrawTexture( new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 64f, 64f, 64f), MoodTextures.textures.neutral);
			else if(mood <= 33f) GUI.DrawTexture( new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 64f, 64f, 64f), MoodTextures.textures.angry);*/
			
			worker.DrawBadges(new Vector2(offset.x + (320f * 0.5f - 104f), offset.y + 172f), 48f, 48f);
		}
		
		/*if(cameraRaycaster != null && cameraRaycaster.mouseOverWorker != null)
		{
			Vector2 offset = new Vector2(Screen.width - 320f, Screen.height - 256f);
			Worker worker =  cameraRaycaster.mouseOverWorker;
			mouseOverStyle.fontSize = 24;
			GUI.Label( new Rect(offset.x, offset.y, 256, 24), "" + worker.name, mouseOverStyle);
			mouseOverStyle.fontSize = 20;
			GUI.Label( new Rect(offset.x, offset.y + 32f, 256, 24), "" + worker.GetWorkerType(), mouseOverStyle);
			float mood = worker.GetMood();
			if(mood > 66f) GUI.DrawTexture( new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 64f, 64f, 64f), MoodTextures.textures.happy);
			else if(mood <= 66f && mood > 33f) GUI.DrawTexture( new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 64f, 64f, 64f), MoodTextures.textures.neutral);
			else if(mood <= 33f) GUI.DrawTexture( new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 64f, 64f, 64f), MoodTextures.textures.angry);
			worker.DrawBadges(new Vector2(offset.x + (320f * 0.5f - 104f), Screen.height - 112f), 48f, 48f);
		}
		else if(cameraRaycaster != null && cameraRaycaster.selectedworker != null)
		{
			Vector2 offset = new Vector2(Screen.width - 320f, Screen.height - 256f);
			Worker worker =  cameraRaycaster.selectedworker;
			mouseOverStyle.fontSize = 24;
			GUI.Label( new Rect(offset.x, offset.y, 256, 24), "" + worker.name, mouseOverStyle);
			mouseOverStyle.fontSize = 20;
			GUI.Label( new Rect(offset.x, offset.y + 32f, 256, 24), "" + worker.GetWorkerType(), mouseOverStyle);
			float mood = worker.GetMood();
			if(mood > 66f) GUI.DrawTexture( new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 64f, 64f, 64f), MoodTextures.textures.happy);
			else if(mood <= 66f && mood > 33f) GUI.DrawTexture( new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 64f, 64f, 64f), MoodTextures.textures.neutral);
			else if(mood <= 33f) GUI.DrawTexture( new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 64f, 64f, 64f), MoodTextures.textures.angry);
			worker.DrawBadges(new Vector2(offset.x + (320f * 0.5f - 104f), Screen.height - 112f), 48f, 48f);
		}*/
	}
	
	// ------------------------ Spawn Workers -------------------------------
	private void SpawnWorkers()
	{
		if(workers.Count == 0)
		{
			for(int i = 0; i < 6; i++)
			{
				GameObject obj = null;
				Vector3 pos = new Vector3();
				pos.x = Random.Range(-2.0f, 2.0f);
				pos.y = 0.0f;
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
	
	private void SpawnWorkers(bool generateNewWorkers)
	{
		if(generateNewWorkers)
		{
			SpawnWorkers();
			return;
		}
		
		for(int i = 0; i < workers.Count - 1; i++)
		{
			//GameObject obj = workers[i].gameObject;
			Vector3 pos = workers[i].GetPosition();
			pos.x -= 3f;
			workers[i].SetPosition(pos);
		}
	}
	
	// ------------------------ Worker List Management -------------------------------
	
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
		
		//Debug.Log("Workers: " + workers.Count + "; Programmers " + Programmers + "; Artists " + Artists + "; AudioDesigners " + AudioDesigners);
	}
	private void CountWorkersPerRoom()
	{
		WorkersInWorkroom = 0;
		WorkersInConference = 0;
		WorkersInRecreation = 0;
		foreach(Worker worker in workers)
		{
			if(worker.AtWorkstation()) WorkersInWorkroom++;
			else if(worker.InConferenceRoom()) WorkersInConference++;
			else if(worker.InRecreationRoom()) WorkersInRecreation++;
		}
		
		//Debug.Log("Workers at Workstations " + workersInWorkroom + "; In Conference " + workersInConference + "; In Recreation " + workersInRecreation);
	}
	
	static public void SetWorkerList(List<Worker> workers)
	{
		workers = workers;
		Debug.Log("Set workers.Count " + workers.Count);
	}
	static public void SetWorkerObjectList(List<GameObject> workers)
	{
		//workerObjects = workers;
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
	
	/////////////////////////// MAP //////////////////////////////
	
	static public float Map(float current, float from1, float from2, float to1, float to2)
	{
	    return to1 + (current - from1) * (to2 - to1) / (from2 - from1);
	}
	
}
