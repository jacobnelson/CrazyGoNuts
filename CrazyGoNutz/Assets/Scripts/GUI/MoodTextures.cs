using UnityEngine;
using System.Collections;

public class MoodTextures : MonoBehaviour 
{
	static public MoodTextures textures = null;
	
	public Texture2D happygreen = null;
	public Texture2D neutralyellow = null;
	public Texture2D angryred = null;
	
	void Awake()
	{
		textures = this;
	}
}
