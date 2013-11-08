using UnityEngine;
using System.Collections;

public class Task
{
	// Task Requirements
	public float programming = 0;		// CURRENT programming amount
	public float art = 0;				// CURRENT art amount
	public float sound = 0;				// CURRENT sound amount
	
	public float programmingReq = 0;	// TOTAL programming needed
	public float artReq = 0;			// TOTAL art needed
	public float soundReq = 0;			// TOTAL sound needed
	
	bool complete = false;
	
	float taskWeight = 0;		// How much this Task affects total completion
	
	public Task(float weight)
	{
		this.taskWeight = weight;
		
		// Task weight is the percent of the total project, currently 5%-15%
		// GameController.deadlineMax is the number of seconds it takes for the deadline to reach the end, thus causing the lose state
		// GameController.deadlineMax / weight is fastest possible time a task can be completed with ONLY one worker
		float maxTime = GameController.deadlineMax / weight;
		
		// minTime is then divided up amongst the sub tasks
		float minTime = maxTime;
		
		// Set minimum task amounts, in seconds
		minTime -= 6.0f;
		programmingReq += 2.0f;
		artReq += 2.0f;
		soundReq += 2.0f;
		
		// Divid up the leftovers, based on job amounts
		int counter = 0;
		while(minTime > 0 && counter < 10)
		{
			int job = (int)Random.Range(0,3);
			float amount = 0;
			float percent = 0;
			
			if(job == 0) percent = (float)GameController.Programmers / (float)GameController.TotalWorkers;
			else if(job == 1) percent = (float)GameController.Artists / (float)GameController.TotalWorkers;
			else if(job == 2) percent = (float)GameController.AudioDesigners / (float)GameController.TotalWorkers;
			
			// Generate an amount based on percent, meaning that if the player has 4 programmers and 1 artist,
			// the artist won't have as much work as the programmers
			amount = Random.Range(0, percent * maxTime);
			if(amount < minTime)
			{
				minTime -= amount;
			}
			else 
			{
				amount = minTime;
				minTime = 0;
			}
			
			if(job == 0) programmingReq += amount;
			else if(job == 1) artReq += amount;
			else if(job == 2) soundReq += amount;
			
			counter++;
		}
		
		// Inflate Requirements
		// Used to multiply current requirements by a random number between 1 and Number of Workers for a given Job
		//programmingReq *= Random.Range(1, (float)GameController.Programmers);
		//artReq *= Random.Range(1, (float)GameController.Artists);
		//soundReq *= Random.Range(1, (float)GameController.AudioDesigners);
		
		// Adjust Requirements based on WORK_RATE
		//programmingReq *= GameController.WORK_RATE * 2.0f;
		//artReq *= GameController.WORK_RATE * 2.0f;
		//soundReq *= GameController.WORK_RATE * 2.0f;
	}
	
	/////////////////////////// UPDATE //////////////////////////////
	
	public void Update()
	{
		if(programming >= programmingReq && art >= artReq && sound >= soundReq) complete = true;
		else complete = false;
	}
	
	/////////////////////////// WORK ON //////////////////////////////
	
	public void WorkOn(WorkerType type, float amount)
	{
		switch(type)
		{
			case WorkerType.Artist:
				art += amount;
				break;
			case WorkerType.Programmer:
				programming += amount;
				break;
			case WorkerType.AudioDesigner:
				sound += amount;
				break;
		}
		
		if(programming > programmingReq) programming = programmingReq;
		if(art > artReq) art = artReq;
		if(sound > soundReq) sound = soundReq;
		
	}
	
	/////////////////////////// GET VARS //////////////////////////////
	
	public float GetCompletionAmount()
	{
		//float percent = (programming / programmingReq) * (art / artReq) * (sound / soundReq);
		//float percent = (programming / programmingReq / taskWeight) + (art / artReq / taskWeight) + (sound / soundReq / taskWeight);
		float percent = (programming + art + sound) / (programmingReq + artReq + soundReq);
		float completionAmount = taskWeight * percent;
		//float completionAmount = percent;
		return completionAmount;
	}
	
	public bool Complete()
	{
		return complete;	
	}
	
	public float GetTaskWeight()
	{
		return taskWeight;	
	}
	
}
