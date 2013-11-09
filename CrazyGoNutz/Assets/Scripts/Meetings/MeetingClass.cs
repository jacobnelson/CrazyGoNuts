using UnityEngine;
using System.Collections;

public class Meeting
{
	/*
		Meetings have a chance to spawn when the Player misses a Milestone.
		-Completing a Meeting gives the whole team a boost to Communication
	*/
	public float completion = 0;
	public float completionMax = 0;
	public bool completed = false;
	
	public Meeting(float completionMax)
	{
		this.completionMax = completionMax;	
	}
}
