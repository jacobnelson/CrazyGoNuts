using UnityEngine;
using System.Collections;

	/*
		Merit Badges are used similar to an experince system in that Workers aquire them through various acts
			-2 textures, one for empty, one for full
			-mouse over for tooltip on how to aquire the badge
			-can be a personal badge or a team badge
			
			-when a worker performs a task, check their merit badges by 'index' for it? or name?
	*/

abstract public class MeritBadge
{	
	// DATA
	public string name = "Merit Badge";
	public string tooltip = "Tooltip";
	public int index = 0;					// Used to store the GetMeritBadge index on creation
	public bool completed = false;
	
	//TEXTURES
	public Texture2D emptyTex = null;
	public Texture2D completedTex = null;
		
	public MeritBadge()
	{

	}
	public MeritBadge(string name, Texture2D emptyTex, Texture2D completedTex)
	{
		this.name = name;
		this.emptyTex = emptyTex;
		this.completedTex = completedTex;
	}
		
	// METHODS
	public Texture2D GetTexture()
	{
		if(completed) return completedTex;
		return emptyTex;
	}
	
}



