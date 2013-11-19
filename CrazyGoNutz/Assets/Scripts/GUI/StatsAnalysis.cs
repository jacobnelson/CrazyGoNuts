using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsAnalysis
{
	/*
		Holds a bunch of stats to show the player when the game has finished.
	
	 */
	
	// Lists
	static public List<GraphEntry> productivityPerSec = new List<GraphEntry>();
	
	static public List<GraphEntry> deadline = new List<GraphEntry>();
	static public List<GraphEntry> completion = new List<GraphEntry>();
	
	static public List<Milestone> milestones = new List<Milestone>();
	
	// Numbers
	static public float completionNum = 0f;
	static public float deadlineNum = 0f;
	static public float completedMeetings = 0f;
	
	static public void OnGUI()
	{
		//Drawing.DrawLine( new Vector2(50,50), new Vector2(50,250), 2f);
		DrawProductivityGraph();
		
		DrawCompletionVsDeadline();
	}
	
	/////////////////////////// DRAWING //////////////////////////////
	
	static private void DrawProductivityGraph()
	{
		if(productivityPerSec.Count > 1)
		{
			for(int i = 0; i < productivityPerSec.Count - 1;i++)
			{
				Drawing.DrawLine( new Vector2(i * 10, Screen.height - productivityPerSec[i].val * 10), new Vector2((i + 1) * 10, Screen.height - productivityPerSec[i + 1].val * 10), 2f);
				//Drawing.DrawLine( new Vector2(productivityPerSec[i].time / GameController.deadlineMax * Screen.width, Screen.height - productivityPerSec[i].val * 10), new Vector2(productivityPerSec[i + 1].time / GameController.deadlineMax * Screen.width, Screen.height - productivityPerSec[i + 1].val * 10), 2f);
			}
		}
	}
	static public void DrawProductivityGraph(Rect rect, int stepnum)	// Rect rect, int countevery
	{
		Vector2 offset = new Vector2();	// How much each iteration of the graph is spaced from the one before it
		offset.x = (float)(rect.width / (productivityPerSec.Count / stepnum)); // Make sure to fit the whole graph within the rect
		float step = (float)stepnum;
		GUI.Box( rect, "");
		if(productivityPerSec.Count > step)
		{
			for(int i = 0; i < productivityPerSec.Count - 1 - stepnum;i += stepnum)
			{
				//Drawing.DrawLine( new Vector2(i * offset.x / countEvery, Screen.height - productivityPerSec[i].val * 10), new Vector2((i + 1) * offset.x / countEvery, Screen.height - productivityPerSec[i + countEvery].val * 10), 2f);
				Drawing.DrawLine( new Vector2(rect.x + (i * offset.x / step), Screen.height - productivityPerSec[i].val * 10), new Vector2(rect.x + ((i + step) * offset.x / step), Screen.height - productivityPerSec[i + stepnum].val * 10), 2f);
			}
		}
	}
	
	static private void DrawCompletionVsDeadline()
	{
		float offsetY = 100;
		for(int i = 0; i < deadline.Count - 1;i++)
		{
			Drawing.DrawLine( new Vector2(i * 10, Screen.height - offsetY - deadline[i].val * 10), new Vector2((i + 1) * 10, Screen.height - offsetY - deadline[i + 1].val * 10), Color.red, 2);
		}
		for(int i = 0; i < completion.Count - 1;i++)
		{
			Drawing.DrawLine( new Vector2(i * 10, Screen.height - offsetY - completion[i].val * 10), new Vector2((i + 1) * 10, Screen.height - offsetY - completion[i + 1].val * 10), Color.green, 2);
		}
	}
	
	/////////////////////////// COUNT MILESTONES //////////////////////////////
	
	static public int GetAchievedMilestones()
	{
		int achievedMilestones = 0;
		foreach(Milestone milestone in milestones) if(milestone.Achieved()) achievedMilestones += 1;
		return achievedMilestones;
	}
	static public int GetFailedMilestones()
	{
		int failedMilestones = 0;
		foreach(Milestone milestone in milestones) if(milestone.Failed()) failedMilestones += 1;
		return failedMilestones;
	}
}
