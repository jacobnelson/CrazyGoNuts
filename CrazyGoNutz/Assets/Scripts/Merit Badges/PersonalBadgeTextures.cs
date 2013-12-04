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
	
	void Awake()
	{
		textures = this;
	}
}


/////////////////////////// CLASS  //////////////////////////////

public class PersonalBadge : MeritBadge			// These are only for the WORKERS
{
	public PersonalBadge(int index) : base()
	{
		this.index = index;
		switch(index)
		{
		case 0:
			this.name = "Helping Hands";
			this.tooltip = "Help another Student get through a Problem.";
			this.completedTex = PersonalBadgeTextures.textures.texture01;
			this.emptyTex = PersonalBadgeTextures.textures.textureOutline01;
			break;
		case 1:
			this.name = "Wrapping Things Up";
			this.tooltip = "Be at a Workstation when a Milestone is Achieved.";
			this.completedTex = PersonalBadgeTextures.textures.texture01;
			this.emptyTex = PersonalBadgeTextures.textures.textureOutline01;
			break;
		case 2:
			this.name = "Takin' It Easy";
			this.tooltip = "Spend 1 minute straight in the Recreation Room while 'Happy'";
			this.completedTex = PersonalBadgeTextures.textures.texture01;
			this.emptyTex = PersonalBadgeTextures.textures.textureOutline01;
			break;
		case 3:
			this.name = "Working Hard";
			this.tooltip = "Spend 2 minutes straight at a Workstation.";
			this.completedTex = PersonalBadgeTextures.textures.texture01;
			this.emptyTex = PersonalBadgeTextures.textures.textureOutline01;
			break;
		case 4:
			this.name = "Chatterbox";
			this.tooltip = "Complete 3 Meetings";
			this.completedTex = PersonalBadgeTextures.textures.texture01;
			this.emptyTex = PersonalBadgeTextures.textures.textureOutline01;
			break;
		default:
			this.name = "Chatterbox";
			this.tooltip = "Complete 3 Meetings";
			this.completedTex = PersonalBadgeTextures.textures.texture01;
			this.emptyTex = PersonalBadgeTextures.textures.textureOutline01;
			break;
		}
		this.tooltip = this.name + "\n" + this.tooltip;
	}
	
	/*static public void DrawBadges(Vector2 offset, float margin, float size)	// Only run from OnGUI() in GameController
	{
		for(int i = 0; i < teamBadges.Count; i++)
		{
			Rect rect = new Rect(offset.x + margin * i, offset.y, size,size);
			GUI.DrawTexture( rect, teamBadges[i].GetTexture());
			if(teamBadges[i].completed) GUI.Label(rect, new GUIContent("",teamBadges[i].tooltip + " Completed!"));
			else GUI.Label(rect, new GUIContent("",teamBadges[i].tooltip));
		}
	}*/
}
