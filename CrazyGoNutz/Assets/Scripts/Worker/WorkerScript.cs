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
		worker.name = GetRandomName(worker.gender);
		name = worker.name;

		//name = GetRandomName(worker.gender);
	}
	public void DontDestroy()
	{
		DontDestroyOnLoad(gameObject);
	}
	private string GetRandomName(int gender)
	{
		string newname = "Name";
		string[] names;

		if(gender == 0) names = new string[]{"Sally","Jane","Britney","Becky","Sophia","Emma","Isabella","Olivia","Ava","Emily","Mia","Madison","Elizabeth","Megan","Lily","Angela","Zoey","Amelia"};
		else names = new string[]{"Jim","Bert","Ben","John","Jacob","Jayson","Kevin","Eric","Ethan","Will","Michael","James","Andy","Luke","Isaac","Dylan","Dan","Evan","Adam","Tom","Chris","Levi"};

		int rand = Random.Range (0,names.Length);
		newname = names[rand];

		return newname;
	}
}
