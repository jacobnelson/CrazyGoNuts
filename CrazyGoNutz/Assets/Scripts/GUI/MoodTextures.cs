using UnityEngine;
using System.Collections;

public class MoodTextures : MonoBehaviour 
{
	static public MoodTextures textures = null;
	
	public Texture2D happy = null;
	public Texture2D lesshappy = null;
	public Texture2D neutral = null;
	public Texture2D lessangry = null;
	public Texture2D angry = null;
	
	void Awake()
	{
		textures = this;
	}
}
