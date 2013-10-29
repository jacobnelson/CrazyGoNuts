using UnityEngine;
using System.Collections;

/*
	Workers are the characters that perform work of various jobs
	
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
	string name = "workername";	
	WorkerType workerType;
	bool mouseOver = false;
	public GameObject gameObject = null;
	
	float performance = 100;	// Ratio of communication AND frustration
	float communication = 100;	// Descreased by WORKING, Increased by CONFERRING
	float frustration = 100;	// Increased by WORKING and FAILURE, Descreased by RECREATION
	
	public Worker(GameObject gameObject)
	{
		this.gameObject = gameObject;
	}
	
	/////////////////////////// MOUSE OVER //////////////////////////////
	
	public void SetMouseOver(bool mouseOver)
	{
		this.mouseOver = mouseOver;
	}
	public bool GetMouseOver()
	{
		return mouseOver;
	}
	
}

public enum WorkerType
{
	Artist,
	AudioDesigner,
	Programmer
}
