using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsAnalysis
{
	/*
		Holds a bunch of stats to show the player when the game has finished.
	
	 */
	
	static public List<GraphEntry> productivityPerSec = new List<GraphEntry>();
	
	static public void OnGUI()
	{
		//Drawing.DrawLine( new Vector2(50,50), new Vector2(50,250), 2f);
		DrawProductivityGraph();
	}
	
	static private void DrawProductivityGraph()
	{
		if(productivityPerSec.Count > 1)
		{
			for(int i = 0; i < productivityPerSec.Count - 1;i++)
			{
				Drawing.DrawLine( new Vector2(i * 10, Screen.height - productivityPerSec[i].val * 10), new Vector2((i + 1) * 10, Screen.height - productivityPerSec[i + 1].val * 10), 2f);
			}
		}
	}
}
