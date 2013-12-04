using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharSelectScript : MonoBehaviour 
{
	
	static private List<Worker> workers = new List<Worker>();
	static public List<Worker> selectedworkers = new List<Worker>();
	static public int numberOfSelectedWorkers = 0;
	
	// Component Reference
	CameraRaycaster cameraRaycaster = null;
	
	// Worker Prefab
	public GameObject workerPrefab = null;
	public GameObject artistMale = null;
	public GameObject artistFemale = null;
	public GameObject programmerMale = null;
	public GameObject programmerFemale = null;
	public GameObject sounddesignerMale = null;
	public GameObject sounddesignerFemale = null;
	
	// GUIStyles
	public GUIStyle mouseOverStyle;
	public GUIStyle buttonStyle;
	
	// Use this for initialization
	void Start () 
	{
		cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
		
		SpawnWorkers();
		
		//Banner.AddMovingBanner("Choose Your Team!!", 82f, new Vector3(Screen.width /2, Screen.height/2,0), 0.65f, Interpolate.EaseType.EaseOutSine, new float[]{-512f, Screen.height * 0.5f + 256f, 3f}, new float[]{0f, -1 *(Screen.height * 0.5f + 256f), 3f}, 2.5f);
		Banner.AddScalingBanner("Choose Your Team!!", 82f, new Vector3(Screen.width /2, Screen.height/2,0), 0.65f, Interpolate.EaseType.EaseOutSine, new float[]{0,0.75f,3f}, new float[]{1f,0f,2.5f},1f, BannerColor.Yellow);
	}
	
	// Update is called once per frame 
	void Update () 
	{
		UpdateCurrentSelectedWorkers();
		
		//Debug.Log(selectedworkers.Count);
	}
	
	void OnGUI()
	{
		if(numberOfSelectedWorkers == 6)
		{
			if (GUI.Button(new Rect(Screen.width * 0.5f - 64f, Screen.height - 128, 128, 48), "", buttonStyle))
			{
				GameController.SetWorkerList(selectedworkers);
				foreach(Worker worker in selectedworkers) worker.gameObject.SendMessage("DontDestroy");
				//GameController.SetWorkerObjectList(workerObjects);
           		Application.LoadLevel("GameScene");
			}
		}
		else
		{
			GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
			GUI.Box(new Rect(Screen.width * 0.5f - 64f, Screen.height - 128, 128, 48), "", buttonStyle);
			GUI.color = Color.white;
		}
		
		Banner.DrawText();
	}
	
	/////////////////////////// TEAM SELECTION  //////////////////////////////
	
	private void UpdateCurrentSelectedWorkers()
	{
		Vector3 selectionCirclePos = new Vector3(4,0,0);
		float maxdist = 2.8f;
		
		selectedworkers.Clear();
		
		foreach(Worker worker in workers)
		{
			float dist = Vector3.Distance(worker.GetPosition(), selectionCirclePos);
			if(dist <= maxdist) selectedworkers.Add(worker);
		}
		
		numberOfSelectedWorkers = selectedworkers.Count;
	}
	
	/////////////////////////// WORKERS  //////////////////////////////
	
	// ------------------------ Update Workers -------------------------------
	
	/*private void UpdateWorkers()
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
	}*/
	
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
			
			Texture2D texture = worker.GetMoodTexture();
			if(texture != null) 
			{
				Rect rect = new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 64f, 64f, 64f);
				GUI.DrawTexture( rect, texture);
				float mood = worker.GetMood();
				GUI.Label(rect, new GUIContent("", worker.GetCurrentMood() + " (" + mood.ToString("f0") + "/100)"));
			}
			/*float mood = worker.GetMood();
			if(mood > 66f) GUI.DrawTexture( new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 64f, 64f, 64f), MoodTextures.textures.happy);
			else if(mood <= 66f && mood > 33f) GUI.DrawTexture( new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 64f, 64f, 64f), MoodTextures.textures.neutral);
			else if(mood <= 33f) GUI.DrawTexture( new Rect(offset.x + (320f * 0.5f - 64f),  offset.y + 64f, 64f, 64f), MoodTextures.textures.angry);*/
			
			//worker.DrawBadges(new Vector2(offset.x + (320f * 0.5f - 104f), Screen.height - 112f), 48f, 48f);
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
			for(int i = 0; i < 15; i++)
			{
				// Get type, make sure at least one of each class
				int type = i;
				if(type > 2) type = Random.Range(0,3);
				// Get Gender
				int gender = Random.Range (0,2);	//0 girl, 1 boy

				if(type == 0) // Artist
				{

				} 
				else if(type == 1) // AudioDesigner
				{
					
				}
				else if(type == 2) // Programmer
				{
					
				}

				GameObject obj = null;
				Vector3 pos = new Vector3();
				pos.x = Random.Range(-2.0f, 2.0f) - 3f;
				pos.y = 0.0f;
				pos.z = Random.Range(-2.0f, 2.0f);
				obj = Instantiate(workerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				obj.transform.localEulerAngles = new Vector3(0f,180f,0f);
				obj.transform.position = pos;
				
				WorkerScript script = obj.GetComponent<WorkerScript>();
				

				
				script.SetWorker( AddWorker(obj, (WorkerType)type) );
			}
		}
	}
	/*private void CreateWorker(WorkerType type)
	{

	}*/
	
	// ------------------------ Worker List Management -------------------------------
	
	static public Worker AddWorker(GameObject newWorker, WorkerType type)
	{
		Worker worker = null;
		worker = new Worker(newWorker, type);
		workers.Add(worker);
		
		return worker;
	}
	
	/////////////////////////// MOUSE OVER  //////////////////////////////
	
	static public void MouseOver(GameObject mouseOverObj)
	{
		//foreach(SnapTarget snap in snaptargets) snap.SetMouseOver(false);
		foreach(Worker worker in workers) worker.SetMouseOver(false);
		
		//foreach(SnapTarget snap in snaptargets) if(mouseOverObj == snap.workStation) snap.SetMouseOver(true);
		foreach(Worker worker in workers) if(mouseOverObj == worker.gameObject) worker.SetMouseOver(true);
	}
	
	
}
