using UnityEngine;
using System.Collections;

public class Milestone
{
	bool achieved = false;
	bool failed = false;
	
	public float position = 0;
	
	public Milestone(float where)
	{
		this.position = where;
	}
	
	public bool Achieved()
	{
		return achieved;
	}
	public bool Failed()
	{
		return failed;
	}
	public bool Complete()
	{
		if(achieved || failed) return true;
		else return false;
	}
	
	public int Check(float completion, float deadline)
	{
		if(completion >= position) 
		{
			achieved = true;
			return 1;
		}
		else if(deadline >= position) 
		{
			failed = true;
			return -1;
		}
		
		return 0;
	}
}
