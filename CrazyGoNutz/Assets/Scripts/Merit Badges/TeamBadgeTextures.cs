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
			this.name = "All Aboard! Choo! Choo!";
			this.tooltip = "Complete a Meeting with every team member in the Conference Room.";
			this.completedTex = TeamBadgeTextures.textures.texture01;
			this.emptyTex = TeamBadgeTextures.textures.textureOutline01;
			break;
		case 1:
			this.name = "Pizza Party!";
			this.tooltip = "Have all the Students in the Recreation Room at once while ahead of schedule.";
			this.completedTex = TeamBadgeTextures.textures.texture01;
			this.emptyTex = TeamBadgeTextures.textures.textureOutline01;
			break;
		case 2:
			this.name = "Flawless Victory!";
			this.tooltip = "Achieve every Milestone.";
			this.completedTex = TeamBadgeTextures.textures.texture01;
			this.emptyTex = TeamBadgeTextures.textures.textureOutline01;
			break;
		case 3:
			this.name = "Cool as a Cucumber";
			this.tooltip = "Don't let any Student become 'Angry'.";
			this.completedTex = TeamBadgeTextures.textures.texture01;
			this.emptyTex = TeamBadgeTextures.textures.textureOutline01;
			break;
		case 4:
			this.name = "Mind Blown";
			this.tooltip = "Have all Students obtain 5 Stars and Complete all Personal Badges.";
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
		this.tooltip = this.name + "\n" + this.tooltip;
	}
	
	static public void CreateTeamBadges()
	{
		for(int i = 0; i < 5; i++)
		{
			TeamBadge badge = new TeamBadge(i);
			teamBadges.Add(badge);
			//if(Random.Range(0f,1f) > 0.5f) badge.completed = true;
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


