using UnityEngine;
using System.Collections;

public class EndGameScript : MonoBehaviour 
{
	/*
	 	This script runs when the game is complete by either winning or losing.
	 	-Generate a 'Score'
	 		-Based on Milestones, Overall Completion, Average Productivity, Average Time ahead of Deadline
	 	
	*/
	
	float score = 0;
	
	void Start()
	{
		GenerateScore();
	}
	void Update()
	{
		
	}
	void OnGUI()
	{
		GUI.Label(new Rect(10,0,512,24), "Score: $" + score.ToString("f0"));
		//StatsAnalysis.DrawProductivityGraph(new Rect(100, Screen.height/2, 512,512), 1);
	}
	
	
	/////////////////////////// GENERATE SCORE //////////////////////////////
	
	private void GenerateScore()
	{
		float calculatedScore = 0;
		float completion = StatsAnalysis.completionNum;
		float deadline = StatsAnalysis.deadlineNum;
		float completedMeetings = StatsAnalysis.completedMeetings;
		float achievedMilestones = StatsAnalysis.GetAchievedMilestones();
		float failedMilestones = StatsAnalysis.GetFailedMilestones();
		
		calculatedScore = completion * (deadline / GameController.deadlineMax); // results in a number between 0 and 100
		calculatedScore += completedMeetings * 125;								// adds 125 for each compeleted meeting
		calculatedScore += achievedMilestones * 125;							// adds 125 per achieved milestone
		calculatedScore -= failedMilestones * 50;								// minus 50 per failed milestone
		
		score = calculatedScore;
		
		Debug.Log("Calculated Score:" + calculatedScore.ToString("f0"));
		Debug.Log("Completion:" + completion.ToString("f0"));
		Debug.Log("Deadline:" + deadline.ToString("f0"));
		Debug.Log("Completed Meetings:" + completedMeetings.ToString("f0"));
		Debug.Log("Achieved Milestones:" + achievedMilestones.ToString("f0"));
		Debug.Log("Failed Milestones:" + failedMilestones.ToString("f0"));
	}
	
	
}
