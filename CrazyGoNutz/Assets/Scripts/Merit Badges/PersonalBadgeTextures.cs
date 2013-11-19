using UnityEngine;
using System.Collections;

public class PersonalBadgeTextures: MonoBehaviour 
{
	static public PersonalBadgeTextures textures = null;
	
	public Texture2D marqueeTex = null;
	public GUIStyle marqueeStyle;
	
	public Texture2D texture01 = null;
	public Texture2D textureOutline01 = null;
	
	public Texture2D texture02 = null;
	public Texture2D textureOutline02 = null;
	
	public Texture2D texture03 = null;
	public Texture2D textureOutline03 = null;
	
	void Start()
	{
		textures = this;
	}
}


/////////////////////////// CLASS  //////////////////////////////

public class PersonalBadge : MeritBadge			// These are only for the WORKERS
{
	public PersonalBadge(string name, Texture2D emptyTex, Texture2D completedTex) : base(name, emptyTex, completedTex)
	{
			
	}
}
