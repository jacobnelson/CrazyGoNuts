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
			this.name = "Personal Badge 1";
			this.tooltip = "Team Badge 1\nObtain this Team Badge by sucessfully completely 1 Milestone.";
			this.completedTex = PersonalBadgeTextures.textures.texture01;
			this.emptyTex = PersonalBadgeTextures.textures.textureOutline01;
			break;
		case 1:
			this.name = "Personal Badge 2";
			this.tooltip = "Team Badge 2\nObtain this Team Badge by sucessfully completely 1 Milestone.";
			this.completedTex = PersonalBadgeTextures.textures.texture01;
			this.emptyTex = PersonalBadgeTextures.textures.textureOutline01;
			break;
		case 2:
			this.name = "Personal Badge 3";
			this.tooltip = "Team Badge 4\nObtain this Team Badge by sucessfully completely 1 Milestone.";
			this.completedTex = PersonalBadgeTextures.textures.texture01;
			this.emptyTex = PersonalBadgeTextures.textures.textureOutline01;
			break;
		case 3:
			this.name = "Personal Badge 4";
			this.tooltip = "Team Badge 4\nObtain this Team Badge by sucessfully completely 1 Milestone.";
			this.completedTex = PersonalBadgeTextures.textures.texture01;
			this.emptyTex = PersonalBadgeTextures.textures.textureOutline01;
			break;
		default:
			this.name = "This is badge is Broken, aye oh!";
			this.tooltip = "Obtain this Personal Badge by sucessfully failing at something important.";
			this.completedTex = PersonalBadgeTextures.textures.texture01;
			this.emptyTex = PersonalBadgeTextures.textures.textureOutline01;
			break;
			break;
		}
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
