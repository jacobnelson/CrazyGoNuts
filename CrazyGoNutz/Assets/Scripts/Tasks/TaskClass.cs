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
		// maxTime is the fastest amount of time a task can take with all workers working at full productivity
		float maxTime = GameController.deadlineMax / weight * (GameController.WORK_RATE * GameController.TotalWorkers);
		
		// Since it's not possible for all workers to be working at once at their full productivity, we adjust this number so the task is possible
		maxTime *= GameController.TARGET_WORK_PERCENTAGE;	// at 0.2f, it means you need to work at at least 20% capacity to win
		
		// minTime is then divided up amongst the sub tasks
		float minTime = maxTime;
		
		// Set minimum sub task amounts, so each sub task has at least some work
		programmingReq += (float)GameController.Programmers;
		artReq += (float)GameController.Artists;
		soundReq += (float)GameController.AudioDesigners;
		minTime -= (programmingReq + artReq + soundReq);
		
		// Divid up the leftovers by interating through the jobs and dividing up the minTime amoungst the sub tasks
		int counter = 0;
		int job = 0;	// itterate through jobs using this
		while(minTime > 0 && counter < 100)
		{
			float amount = 0;
			float percent = 0;
			
			if(job == 0) percent = (float)GameController.Programmers / (float)GameController.TotalWorkers;
			else if(job == 1) percent = (float)GameController.Artists / (float)GameController.TotalWorkers;
			else if(job == 2) percent = (float)GameController.AudioDesigners / (float)GameController.TotalWorkers;
			
			// Generate an amount based on percent, meaning that if the player has 4 programmers and 1 artist,
			// the artist won't have as much work as the programmers
			amount = Random.Range(0, percent);	// Get a random number from 0 to percent
			
			amount *= maxTime;	// Set amount to a percentage of the maxTime
			
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
			job++;
			if(job > 2) job = 0;
		}
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
