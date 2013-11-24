using UnityEngine;
using System.Collections;

public class Textures : MonoBehaviour 
{
	static public Textures textures = null;
	
	// Mood Textures
	public Texture2D happy = null;
	public Texture2D lesshappy = null;
	public Texture2D neutral = null;
	public Texture2D lessangry = null;
	public Texture2D angry = null;
	
	//Star Textures
	public Texture2D star = null;
	public Texture2D stars = null;
	public Texture2D starsbg = null;
	
	void Awake()
	{
		textures = this;
	}
}
