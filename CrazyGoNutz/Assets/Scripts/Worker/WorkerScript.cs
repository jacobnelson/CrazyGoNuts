﻿using UnityEngine;
using System.Collections;

/*
	WorkerScript goes on the worker GameObjects within the scene
	-This is the MonoBehavior attached to the GameObject within the Scene, for the Worker class, see WorkerClass.cs
	
	-Animator
	-Handles animation

*/

public class WorkerScript : MonoBehaviour 
{
	// Worker Class Instance
	Worker worker = null;
	
	
	/////////////////////////// GET VARS //////////////////////////////

	public Worker GetWorker()				// Returns the Worker Class Instance
	{
		return worker;
	}
	public void SetWorker(Worker worker)	// Sets the Worker Class Instance
	{
		this.worker = worker;
	}
}
