using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	Workers are the characters that perform work of various jobs
	-This is the Worker class, for the Monobehavior, see WorkerScript.cs
	
	Stats:
	-Performance
	-Communication
	-Frustraction
	
	Roadblock:
	-bool roadblocked
	-float roadblockBar

*/

public class Worker
{	
	public string name = "WorkerName";	
	WorkerType workerType;						// Artist, AudioDesigner, or Programmer
	public int gender = 0;						// 0 - female; 1 - male
	bool mouseOver = false;
	public GameObject gameObject = null;		// Associated GameObject within the Scene
	public GameObject charObject = null;
	public Animator anim = null;
	ParticleSystem roadblockParticle = null;	// ParticleSystem used to show Roadblock
	
	public bool selected = false;
	
	public Vector3 startingPos = new Vector3();
	
	// PersonalBadges
	public List<PersonalBadge> badges = new List<PersonalBadge>();
	
	float mood = 100f;
	string currentMood = "Happy";
	string currentAnimation = "MouseDrag";
	
	SnapTarget lastSnapTarget = null;		// if currentSnapTarget is null, workers snap back to lastSnapTarget
	SnapTarget currentSnapTarget = null;
	
	// Badge stats
	float timeInRoom = 0f;	//2min straight needed for Working Hard Badge
	int meetingsCompleted = 0;
	int studentsHelped = 0;
	
	// Stars
	public float starfloat = 0f;
	int stars = 0;
	
	// Roadblock
	bool roadblocked = false;
	float secondCounter = 0;
	float clickCounter = 0;
	
	// Materials
	//private List<Renderer> renderers = new List<Renderer>();
	private Renderer[] renderers;
	
	public Worker(GameObject gameObject, WorkerType workerType, int gender)
	{
		this.gameObject = gameObject;
		this.charObject = gameObject.transform.Find("Character").gameObject;
		this.anim = charObject.GetComponent<Animator>();
		this.workerType = workerType;
		
		this.startingPos = gameObject.transform.position;
		this.gender = gender;
		
		roadblockParticle = gameObject.GetComponentInChildren<ParticleSystem>();
		if(roadblockParticle != null) roadblockParticle.Stop();
		
		starfloat = Random.Range(0f,5f);
		
		// Get 3 Random Personal Badges
		List<string> badgeNames = new List<string>();
		for(int i = 0; i < 3; i++)
		{
			PersonalBadge badge = new PersonalBadge(Random.Range(0,5));
			if(!badgeNames.Contains(badge.name))
			{
				badges.Add(badge);
				badgeNames.Add(badge.name);
			} else i--;
		}
		
		// Get Renderers
		GetRenderers();
		
		SetToNormal();
	}
	
	public void UpdateStats()
	{
		//SnapTarget currentSnapTarget = worker.GetSnapTarget();
		if(currentSnapTarget != null)
		{
			SnapTargetType type = currentSnapTarget.GetType();
			timeInRoom += Time.deltaTime;
			switch(type)
			{
				case SnapTargetType.Conference:
					if(GameController.WorkersInConference > 1)
					{
						if(roadblocked) RemoveRoadblock();
					}
					// Check For Badge
				
					break;
				case SnapTargetType.Recreation:
					AdjustMood(GameController.SLIGHT_INCREASE * (float)GameController.WorkersInRecreation * Time.deltaTime);	// More workers in RecRoom reduces frustration faster
					AdjustMood(GameController.INCREASE * Time.deltaTime);	
					// Check For Badge "Takin' It Easy"
					if(timeInRoom > 60) CheckForBadgeCompletion("Takin' It Easy");
				break;
				case SnapTargetType.Workstation:
					AdjustMood(GameController.SLIGHT_DECREASE * Time.deltaTime);
					if(roadblocked) AdjustMood(GameController.DECREASE * Time.deltaTime);
					// Check For Badge "Working Hard"
					if(timeInRoom > 120) CheckForBadgeCompletion("Working Hard");
					break;
			}
		}
		
		// Calculate Roadblock Chance
		if(AtWorkstation())
		{
			secondCounter -= Time.deltaTime;
			//clickCounter -= Time.deltaTime;
			if(secondCounter <= 0)
			{
				if(!roadblocked)
				{
					//float rand = Random.Range(frustration, 100);
					//if(rand >= 99) SpawnRoadblock();
					float rand = Random.Range(0f, mood);
					if(rand <= 1f) SpawnRoadblock();
				}
				secondCounter = 1.0f;	
			}
			/*if(clickCounter <= 0)
			{
				clickCounter = 0.5f;	
			}*/
		}
		
		// Update Mood
		UpdateMood();

	}
	
	///////////////////////////  BADGES //////////////////////////////
	
	public void DrawBadges(Vector2 offset, float margin, float size)	// Only run from OnGUI() in GameController
	{
		for(int i = 0; i < badges.Count; i++)
		{
			Rect rect = new Rect(offset.x + margin * i, offset.y, size,size);
			GUI.DrawTexture( rect, badges[i].GetTexture());
			if(badges[i].completed) GUI.Label(rect, new GUIContent("",badges[i].tooltip + " Completed!"));
			else GUI.Label(rect, new GUIContent("",badges[i].tooltip));
		}
	}
	
	public void CheckForBadgeCompletion(string badgeName)
	{
		foreach(PersonalBadge badge in badges)
		{
			if(badgeName == badge.name && !badge.completed)
			{
				badge.completed = true;
				// Spawn Particle!
				Vector3 pos = gameObject.transform.position;
				pos.y = 1.5f;
				SpawnParticle.SpawnBadgearticle( pos, gameObject.transform, badge.completedTex);
				return;
			}
		}
	}
	public bool HasBadge(string badgeName)
	{
		foreach(PersonalBadge badge in badges) if(badgeName == badge.name && !badge.completed) return true;
		return false;
	}

	public bool CompletedAllBadges()
	{
		foreach(PersonalBadge badge in badges) if(!badge.completed) return false;
		return true;
	}
	
	/////////////////////////// ROADBLOCK //////////////////////////////
	
	private void SpawnRoadblock()
	{
		roadblocked = true;
		if(roadblockParticle != null) roadblockParticle.Play();
	}
	public void RemoveRoadblock()
	{
		roadblocked = false;
		if(roadblockParticle != null) 
		{
			roadblockParticle.Stop();
			roadblockParticle.Clear();
		}
	}
	
	/////////////////////////// MOOD //////////////////////////////
	
	// ------------------------ Update Mood -------------------------------
	private void UpdateMood()
	{
		string newMood = "";
		if(mood > 80f) newMood = "Happy";
		else if(mood <= 80f && mood > 60f) newMood = "Less Happy";
		else if(mood <= 60f && mood > 40f) newMood = "Neutral";
		else if(mood <= 40f && mood > 20f) newMood = "Discontent";
		else if(mood <= 20f) newMood = "Angry";
		
		if(newMood != currentMood)
		{
			// Spawn Particle!
			currentMood = newMood;
			Vector3 pos = gameObject.transform.position;
			pos.y = 1.5f;
			SpawnParticle.SpawnMoodParticle( pos, GetMoodTexture());

			if(currentMood == "Angry") GameController.studentBecameAngry = true;
		}
	}
	
	public void AdjustMood(float howMuch)
	{
		mood += howMuch;
		if(mood > 100f) mood = 100f;
		if(mood < 0.1f) mood = 0.1f;
		//CalculateProductivity();
	}
	public float GetMood()
	{
		return mood;
	}
	public string GetCurrentMood()
	{
		return currentMood;
	}
	public Texture2D GetMoodTexture()
	{
		Texture2D texture = null;
		if(mood > 80f) texture = Textures.textures.happy;
		else if(mood <= 80f && mood > 60f) texture = Textures.textures.lesshappy;
		else if(mood <= 60f && mood > 40f) texture = Textures.textures.neutral;
		else if(mood <= 40f && mood > 20f) texture = Textures.textures.lessangry;
		else if(mood <= 20f) texture = Textures.textures.angry;
		
		return texture;
	}
	
	/////////////////////////// STATS //////////////////////////////
	
	public void AddToStarPower(float amount)
	{
		starfloat += amount;
		if(starfloat > 5.0f) starfloat = 5.0f;
		if(starfloat - 1.0f > stars) // Completed a Star
		{
			stars++;
			Debug.Log("gained a star!");
		}
	}
	public int GetStars()
	{
		return stars;
	}
	
	/////////////////////////// STATS //////////////////////////////
	
	/*public float GetMoodPercent()
	{
		return mood / 100f + (badges.Count * 0.15f);	// Badges make you more productive
	}*/
	
	/*public void AdjustCommunication(float howMuch)
	{
		communication += howMuch;
		if(communication > 100) communication = 100;
		if(communication < 1) communication = 1;
		CalculateProductivity();
	}
	public void AdjustFrustration(float howMuch)
	{
		frustration += howMuch;
		if(frustration > 100) frustration = 100;
		if(frustration < 1) frustration = 1;
		CalculateProductivity();
	}
	private void CalculateProductivity()
	{
		productivity = 100 - (frustration) + (communication - 50);		//TODO: Change this ratio.
		if(productivity < 1) productivity = 1;
		if(productivity > 100) productivity = 100;
	}
	
	public float ProductivityPercent()
	{
		return productivity / 100f;
	}
	
	public float GetCommunication()
	{
		return communication;
	}
	public float GetFrustration()
	{
		return frustration;
	}
	public float GetProductivity()
	{
		return productivity;
	}*/
	
	public bool Roadblocked()
	{
		return roadblocked;	
	}
	public void CompletedMeeting()
	{
		this.meetingsCompleted++;
		// Check For Badge "Chatterbox"
		if(meetingsCompleted >= 3) CheckForBadgeCompletion("Chatterbox");
	}
	public void HelpedOtherStudent()
	{
		this.studentsHelped++;
		// Check For Badge "Helping Hands"
		CheckForBadgeCompletion("Helping Hands");
	}
	
	/////////////////////////// WORKER TYPE //////////////////////////////
	
	public WorkerType GetWorkerType()
	{
		return workerType;	
	}
	
	/////////////////////////// MOUSE OVER AND SELECTION //////////////////////////////
	
	public void SetMouseOver(bool mouseOver)
	{
		this.mouseOver = mouseOver;
		if(!selected)
		{
			if(mouseOver) SetToMouseOver();
			else SetToNormal();
		}
	}
	public bool GetMouseOver()
	{
		return mouseOver;
	}
	
	public void SetSelected(bool selected)
	{
		this.selected = selected;
		if(selected) SetToSelected();
		else SetToNormal();
	}
	public bool GetSelected()
	{
		return selected;
	}
	
	private void SetToMouseOver()
	{
		if(gameObject.renderer) if(gameObject.renderer.material.HasProperty("_OutlineColor")) gameObject.renderer.material.SetColor("_OutlineColor", new Color(0,1,1,1));
		if(renderers.Length > 0)
		{
			foreach(Renderer renderer in renderers)
			{
				if(renderer.material.HasProperty("_OutlineColor")) renderer.material.SetColor("_OutlineColor", new Color(0,1,1,1));
			}
		}
	}
	private void SetToSelected()
	{
		if(gameObject.renderer) if(gameObject.renderer.material.HasProperty("_OutlineColor")) gameObject.renderer.material.SetColor("_OutlineColor", new Color(1,1,1,1));
		if(renderers.Length > 0)
		{
			foreach(Renderer renderer in renderers)
			{
				if(renderer.material.HasProperty("_OutlineColor")) renderer.material.SetColor("_OutlineColor", new Color(1,1,1,1));
			}
		}
	}
	private void SetToNormal()
	{
		if(gameObject.renderer) if(gameObject.renderer.material.HasProperty("_OutlineColor")) gameObject.renderer.material.SetColor("_OutlineColor", new Color(0,0,0,0));
		if(renderers.Length > 0)
		{
			foreach(Renderer renderer in renderers)
			{
				if(renderer.material.HasProperty("_OutlineColor")) renderer.material.SetColor("_OutlineColor", new Color(0,0,0,0));
			}
		}
	}
	
	private void GetRenderers()
	{
		//Renderer[] rendererComponents = gameObject.GetComponentsInChildren<Renderer>();
		//Debug.Log(rendererComponents.Length);
		renderers = gameObject.GetComponentsInChildren<Renderer>();
	}
	
	/////////////////////////// MOUSE CLICK //////////////////////////////
	
	public void MouseClick()
	{
		if(roadblocked)
		{
			Debug.Log("You've clicked me, oh my." + clickCounter);
			clickCounter++;
			if(clickCounter > 5) 
			{
				RemoveRoadblock();
				clickCounter = 0;
			}
		}
	}
	
	/////////////////////////// SNAP TARGET //////////////////////////////
	
	public SnapTarget GetSnapTarget()
	{
		return currentSnapTarget;
	}
	public bool AtWorkstation()
	{
		return (currentSnapTarget != null && currentSnapTarget.GetType() == SnapTargetType.Workstation);
	}
	public bool InConferenceRoom()
	{
		return (currentSnapTarget != null && currentSnapTarget.GetType() == SnapTargetType.Conference);
	}
	public bool InRecreationRoom()
	{
		return (currentSnapTarget != null && currentSnapTarget.GetType() == SnapTargetType.Recreation);
	}
	
	/////////////////////////// DRAG //////////////////////////////
	
	public void StartDrag()
	{
		gameObject.layer = 2;													// Ignore Raycasting
		lastSnapTarget = currentSnapTarget;										// Set lastSnapTarget to currentSnapTarget for later reference
		if(currentSnapTarget != null) currentSnapTarget.SetWorker(null);		// Empty currentSnapTarget
		currentSnapTarget = null;

		anim.SetBool(currentAnimation, false);
		currentAnimation = "MouseDrag";
		anim.SetBool(currentAnimation, true);
	}
	
	public void EndDrag(SnapTarget snapTarget)
	{
		if(snapTarget != null)		// Set pos to snapTarget 
		{
			if(currentSnapTarget != null) 
			{
				lastSnapTarget = currentSnapTarget;
				//Debug.Log("setting lastSnapTarget to " + currentSnapTarget.ToString());
			}
			if(snapTarget.isEmpty()) currentSnapTarget = snapTarget; 	// If snapTarget is empty, snap to it
			else currentSnapTarget = GameController.FindOpenSnapTargetOfType( snapTarget.GetType() );	// else, find empty snapTarget of same type
		}
		else 						// Set pos to lastSnapTarget
		{
			if(lastSnapTarget == null || !lastSnapTarget.isEmpty())		// If lastSnapTarget is null or not empty, find empty snapTarget of same type
				lastSnapTarget = GameController.FindOpenSnapTargetOfType( SnapTargetType.Workstation );			

			currentSnapTarget = lastSnapTarget;
		}
		
		//Debug.Log("currentSnapTarget " + currentSnapTarget + " , lastSnapTarget " + lastSnapTarget);
		if(currentSnapTarget != null && lastSnapTarget != null)	// Check For timeInRoom reset
		{
			//Debug.Log(""+currentSnapTarget.GetType() +", "+lastSnapTarget.GetType());
			if(currentSnapTarget.GetType() != lastSnapTarget.GetType())	timeInRoom = 0f; // If switched room type, reset timeInRoom
		}
		Debug.Log("Worker.timeInRoom = " + timeInRoom);
		
		
		if(currentSnapTarget == null) return;
		
		currentSnapTarget.SetWorker(this);					// Fill currentSnapTarget
		
		Vector3 pos = currentSnapTarget.GetPosition();
		//pos.y = 0.5f;
		pos.y = 0.0f;
		
		SetPosition( pos );
		gameObject.layer = 9;			// Default Layer

		// Get Animation Name from currentSnapTarget
		Debug.Log ("Play animation " + currentSnapTarget.animation +", Stop animation "+ currentAnimation);
		anim.SetBool(currentAnimation, false);
		currentAnimation = currentSnapTarget.animation;
		anim.SetBool(currentAnimation, true);


	}
	
	/////////////////////////// POSITION //////////////////////////////
	
	public Vector3 GetPosition()
	{
		return gameObject.transform.position;
	}
	public Vector3 GetStartingPosition()
	{
		return startingPos;
	}
	public void SetPosition(Vector3 pos)
	{
		gameObject.transform.position = pos;
	}
	
}

public enum WorkerType
{
	Artist,
	AudioDesigner,
	Programmer
}
public enum WorkerAnimation
{
	Idle,
	Working,
	Frustrated,
	Meeting,
	Happy,
	LookAround
}
