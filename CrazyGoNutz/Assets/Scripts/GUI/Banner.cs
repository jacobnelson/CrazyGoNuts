using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Banner
{
	// Static Variables
	static List<Banner> bannerList = new List<Banner>();
	static public GUIStyle style = null;
	static public Texture2D bannerTexture = null;
	
	// Instance Variables
	string text = "";
	Vector3 position = new Vector3();
	
	float startValue = 1.0f;
	float endValue = 3.0f;
	float duration = 1.0f;
	private float elapsedTime = 0.0f;
	private Interpolate.EaseType easeType;
	
	float scale = 1.0f;
	float[] introAction;
	float[] outroAction;
	float delay = 0f;
	bool enter = true;
	
	bool dead = false;

	float fontSize = 10.0f;
	
	bool moving = true;
	bool scaling = false;
	
	/*public Banner(string text, Vector3 position, Interpolate.EaseType easeType, float startValue, float endValue, float duration)
	{
		this.text = text;
		this.position = position;
		this.easeType = easeType;
		this.startValue = startValue;
		this.endValue = endValue;
		this.duration = duration;
	}*/
	
	public Banner(string text, float fontSize, Vector3 position, float scale, Interpolate.EaseType easeType, float[] introAction, float[] outroAction, float delay)
	{
		this.text = text;
		this.position = position;
		this.easeType = easeType;
		this.introAction = introAction;
		this.outroAction = outroAction;
		this.delay = delay;
		this.scale = scale;
		this.fontSize = fontSize;
	}
	
	public void Update()
	{
		//Ease ease = new Ease( Interpolate.Ease(easeType) );
		if(enter)
		{
			if(moving) position.y = Interpolate.EaseOutCirc(introAction[0], introAction[1], elapsedTime, introAction[2]);
			if(scaling)
			{
				//introAction[0] = scale;
				//scale = Interpolate.EaseOutCirc(introAction[0], introAction[1], elapsedTime, introAction[2]);
				scale = Mathf.Lerp(scale, introAction[1], (elapsedTime/introAction[2]));
			}
			elapsedTime += Time.deltaTime;
			if(elapsedTime > introAction[2])
			{
				delay -= Time.deltaTime;
				if(delay <= 0)
				{
					enter = false;
					elapsedTime = 0f;
					outroAction[0] = position.y;
				}
			}
		}
		else
		{
			if(moving) position.y = Interpolate.EaseOutCirc(outroAction[0], outroAction[1], elapsedTime, outroAction[2]);
			if(scaling)
			{
				//outroAction[0] = scale;
				//scale = Interpolate.EaseOutCirc(outroAction[0], outroAction[1], elapsedTime, outroAction[2]);
				scale = Mathf.Lerp(scale, outroAction[1], (elapsedTime/outroAction[2]));
			}
			elapsedTime += Time.deltaTime;
			if(elapsedTime >= outroAction[2]) dead = true;
		}
	}
	
	/*public void Update()
	{
		//Ease ease = new Ease( Interpolate.Ease(easeType) );
		position.y = Interpolate.EaseInOutSine(startValue, endValue, elapsedTime, duration);
		elapsedTime += Time.deltaTime;
	}*/

	////////////////////////////////////////////////////////////////
	
	static public void AddMovingBanner(string text, float fontSize, Vector3 position, float scale, Interpolate.EaseType easeType, float[] introAction, float[] outroAction, float delay)
	{
		var banner = new Banner( text, fontSize, position, scale, easeType,  introAction, outroAction, delay);
		bannerList.Add(banner);
		banner.moving = true;
		banner.scaling = false;
	}
	static public void AddScalingBanner(string text, float fontSize, Vector3 position, float scale, Interpolate.EaseType easeType, float[] introAction, float[] outroAction, float delay)
	{
		var banner = new Banner( text, fontSize, position, scale, easeType,  introAction, outroAction, delay);
		bannerList.Add(banner);
		banner.moving = false;
		banner.scaling = true;
	}
	
	static public void DrawText()		// MUST BE DONE FROM OnGUI!!!
	{
		//Loops through floatingTextList, updating postions and drawing them to the screen
		UpdateBanners();
		Draw();
	}
	static private void UpdateBanners()
	{
		//Loops through floatingTextList, updating postions removing dead ones
		// Calc screen pos
		for(int i = 0; i < bannerList.Count; i++)
		{
			/*Banner banner = bannerList[i];
			// Update Position
			//banner.screenPosition = Camera.main.WorldToScreenPoint(banner.position);
			banner.position += banner.speed * Time.deltaTime;
			// Update Color
			banner.lifeTime -= Time.deltaTime;
			if(banner.lifeTime <= 0) banner.dead = true;
			else if(banner.fade > banner.lifeTime / banner.startLife) banner.alpha = GameController.Map(banner.lifeTime, banner.startLife, 0F, 1.0F, 0.0F);
			banner.color.a = banner.alpha;
			
			if(banner.dead) bannerList.RemoveAt(i);*/
			Banner banner = bannerList[i];
			banner.Update();
			if(banner.dead) bannerList.RemoveAt(i);
		}
	}
	static private void Draw()
	{
		//Loops through floatingTextList drawing them to the screen
		for(int i = 0; i < bannerList.Count; i++)
		{
			/*Banner banner = bannerList[i];
			style.fontSize = (int)(banner.fontSize);// + (text.fontSize * (1.0f - CameraRaycaster.cameraZoomScale)) * GameController.guiScale);
			//style.fontSize = text.fontSize + (text.fontSize * (1.0 - CameraRaycaster.cameraZoomScale));
			GUI.color = new Color(banner.color.r, banner.color.g, banner.color.b, banner.alpha);
			GUI.Label( new Rect(banner.position.x, Screen.height - banner.position.y, 128, 32), banner.text, style); */
			Banner banner = bannerList[i];
			Rect rect = new Rect(banner.position.x - (1024 * banner.scale * 0.5f), banner.position.y - (512 * banner.scale * 0.5f), 1024 * banner.scale, 512 * banner.scale);
			float fontSize = banner.fontSize * banner.scale;
			style.fontSize = (int)fontSize;
			if(style.fontSize > 200) style.fontSize = 200;
			if(style.fontSize <= 1) style.fontSize = 0;
			GUI.DrawTexture(rect, Banner.bannerTexture);
			if(style.fontSize > 1)GUI.Label(rect, banner.text, style);
		}
	}
}
