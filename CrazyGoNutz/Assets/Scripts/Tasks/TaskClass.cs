using UnityEngine;
using System.Collections;

public class Task
{
	// Task Requirements
	public float programming = 0;
	public float art = 0;
	public float sound = 0;
	
	public float programmingReq = 0;	// total programming needed
	public float artReq = 0;			// total art needed
	public float soundReq = 0;			// total sound needed
	
	bool complete = false;
	
	float taskWeight = 0;		// How much this Task affects total completion
	
	public Task(float weight)
	{
		this.taskWeight = weight;
		
		/*programmingReq = Random.Range(1, 5) * weight;
		artReq = Random.Range(1, 5) * weight;
		soundReq = Random.Range(1, 5) * weight;*/
		
		// weight is a percent, so gameLengthMax / weight is fastest possible time with ONLY one worker
		float maxTime = GameController.deadlineMax / weight;
		// times this by a percent like 0.6 to make a completion window
		//maxTime *= 0.6f;
		//maxTime *= Random.Range(1,3);
		float minTime = maxTime;
		
		// Set minimum task amounts
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
			
			if(job == 0) percent = (float)GameController.Programmers / GameController.TotalWorkers;
			else if(job == 1) percent = (float)GameController.Artists / GameController.TotalWorkers;
			else if(job == 2) percent = (float)GameController.AudioDesigners / GameController.TotalWorkers;
					
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
			
			//Debug.Log("amount " + amount + " minTime " + minTime);
			counter++;
		}
		
		// Calculate Requirements
		programmingReq *= Random.Range(1, (float)GameController.Programmers);
		artReq *= Random.Range(1, (float)GameController.Artists);
		soundReq *= Random.Range(1, (float)GameController.AudioDesigners);
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
	
	/*public float GetCompletionPercent()
	{
		float percent = (programming / programmingReq) * (art / artReq) * (sound / soundReq);
		return percent;
	}*/
	public float GetCompletionAmount()
	{
		float percent = (programming / programmingReq) * (art / artReq) * (sound / soundReq);
		float completionAmount = taskWeight * percent;
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
