using UnityEngine;
using System.Collections;

public class SwitchSceneTimer : MonoBehaviour 
{
	public float secsTillSceneChange = 2.5f;
	public string nextScene = "";
	
	void Update () 
	{
		secsTillSceneChange -= Time.deltaTime;
		if(secsTillSceneChange <= 0)
		{
			Application.LoadLevel(nextScene);	
		}
	}
}
