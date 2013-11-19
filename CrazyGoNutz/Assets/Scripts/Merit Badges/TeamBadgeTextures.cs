using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamBadgeTextures  : MonoBehaviour 
{
	static public TeamBadgeTextures textures = null;
	
	public Texture2D marqueeTex = null;
	public GUIStyle marqueeStyle;
	
	public Texture2D texture01 = null;
	public Texture2D textureOutline01 = null;
	
	public Texture2D texture02 = null;
	public Texture2D textureOutline02 = null;
	
	public Texture2D texture03 = null;
	public Texture2D textureOutline03 = null;
	
	void Awake()
	{
		textures = this;
	}
}


/////////////////////////// CLASS  //////////////////////////////

public class TeamBadge : MeritBadge				// These are only for the TEAM
{
	//static public TeamBadge teamBadge01 = null;
	static public List<TeamBadge> teamBadges = new List<TeamBadge>();
	
	public TeamBadge(int index) : base()
	{
		this.index = index;
		switch(index)
		{
		case 0:
			this.name = "Team Badge 1";
			this.tooltip = "Team Badge 1\nObtain this Team Badge by sucessfully completely 1 Milestone.";
			this.completedTex = TeamBadgeTextures.textures.texture01;
			this.emptyTex = TeamBadgeTextures.textures.textureOutline01;
			break;
		case 1:
			this.name = "Team Badge 2";
			this.tooltip = "Team Badge 2\nObtain this Team Badge by sucessfully completely 1 Milestone.";
			this.completedTex = TeamBadgeTextures.textures.texture01;
			this.emptyTex = TeamBadgeTextures.textures.textureOutline01;
			break;
		default:
			this.name = "This is badge is Broken, aye oh!";
			this.tooltip = "Obtain this Team Badge by sucessfully failing at something important.";
			this.completedTex = TeamBadgeTextures.textures.texture01;
			this.emptyTex = TeamBadgeTextures.textures.textureOutline01;
			break;
			break;
		}
	}
	
	static public void CreateTeamBadges()
	{
		for(int i = 0; i < 6; i++)
		{
			TeamBadge badge = new TeamBadge(i);
			teamBadges.Add(badge);
			if(Random.Range(0f,1f) > 0.5f) badge.completed = true;
		}
	}
	
	static public void DrawTeamBadges(Vector2 offset, float margin, float size)	// Only run from OnGUI() in GameController
	{
		for(int i = 0; i < teamBadges.Count; i++)
		{
			Rect rect = new Rect(offset.x + margin * i, offset.y, size,size);
			GUI.DrawTexture( rect, teamBadges[i].GetTexture());
			if(teamBadges[i].completed) GUI.Label(rect, new GUIContent("",teamBadges[i].tooltip + " Completed!"));
			else GUI.Label(rect, new GUIContent("",teamBadges[i].tooltip));
		}
		
	}
}


