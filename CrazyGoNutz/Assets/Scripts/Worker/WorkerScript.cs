using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	
	// Textures
	public List<Texture2D> textures = new List<Texture2D>();

	void Start()
	{
		// Get all renderers and apply random texture
		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
		if(textures.Count > 0)
		{
			foreach(Renderer renderer in renderers)
			{
				if(renderer.GetType() != typeof(SkinnedMeshRenderer)) continue;
				Texture2D tex = textures[ Random.Range(0,textures.Count) ];
				if(tex != null && renderer != null) renderer.material.mainTexture = tex;
			}
		}
	}

	/////////////////////////// GET VARS //////////////////////////////

	public Worker GetWorker()				// Returns the Worker Class Instance
	{
		return worker;
	}
	public void SetWorker(Worker worker)	// Sets the Worker Class Instance
	{
		this.worker = worker;
		worker.name = "Worker" + (int)Random.Range(0,1000);	//TODO: Temp for debug.
		name = worker.name;
	}
	public void DontDestroy()
	{
		DontDestroyOnLoad(gameObject);
	}
}
