using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FloatingText
{
	// Static Variables
	static List<FloatingText> floatingTextList = new List<FloatingText>();
	static public GUIStyle style = null;
	
	// Instance Variables
	string text = "";
	Vector3 position = new Vector3();
	Vector3 screenPosition = new Vector3();
	Vector3 speed = new Vector3();
	
	float lifeTime = 1.5f;
	float startLife = 1.5f;
	bool dead = false;
	float fade = 0.5f;	// when in lifeTime begins fade, 1.0 for no fade
	
	float fontSize = 10.0f; //16.0f;
	Color color = Color.white;
	float alpha = 1.0f;	// 0 - 1
	
	public FloatingText(string text, Vector3 position, Vector3 speed, float lifeTime, Color color)
	{
		this.text = text;
		this.position = position;
		this.speed = speed;
		this.lifeTime = lifeTime;
		this.color = color;
		
		this.startLife = lifeTime;
	}
	
	public FloatingText(string text, Vector3 position, Vector3 speed, float fontSize, float lifeTime, Color color)
	{
		this.text = text;
		this.position = position;
		this.fontSize = fontSize;
		this.speed = speed;
		this.lifeTime = lifeTime;
		this.color = color;
		
		this.startLife = lifeTime;
	}

	////////////////////////////////////////////////////////////////
	
	static public void AddText(string text, Vector3 position, Vector3 speed, float lifeTime, Color color)
	{
		var floatingText = new FloatingText(text, position, speed, lifeTime, color);
		floatingTextList.Add(floatingText);
	}
	static public void AddText(string text, Vector3 position, Vector3 speed, float fontSize, float lifeTime, Color color)
	{
		var floatingText = new FloatingText(text, position, speed, fontSize, lifeTime, color);
		floatingTextList.Add(floatingText);
	}
	static public void DrawText()		// MUST BE DONE FROM OnGUI!!!
	{
		//Loops through floatingTextList, updating postions and drawing them to the screen
		Update();
		Draw();
	}
	static private void Update()
	{
		//Loops through floatingTextList, updating postions removing dead ones
		// Calc screen pos
		for(int i = 0; i < floatingTextList.Count; i++)
		{
			FloatingText text = floatingTextList[i];
			// Update Position
			text.screenPosition = Camera.main.WorldToScreenPoint(text.position);
			text.position += text.speed * Time.deltaTime;
			// Update Color
			text.lifeTime -= Time.deltaTime;
			if(text.lifeTime <= 0) text.dead = true;
			else if(text.fade > text.lifeTime / text.startLife) text.alpha = GameController.Map(text.lifeTime, text.startLife, 0F, 1.0F, 0.0F);
			text.color.a = text.alpha;
			
			if(text.dead) floatingTextList.RemoveAt(i);
		}
	}
	static private void Draw()
	{
		//Loops through floatingTextList drawing them to the screen
		for(int i = 0; i < floatingTextList.Count; i++)
		{
			FloatingText text = floatingTextList[i];
			style.fontSize = (int)(text.fontSize);// + (text.fontSize * (1.0f - CameraRaycaster.cameraZoomScale)) * GameController.guiScale);
			//style.fontSize = text.fontSize + (text.fontSize * (1.0 - CameraRaycaster.cameraZoomScale));
			GUI.color = new Color(text.color.r, text.color.g, text.color.b, text.alpha);
			GUI.Label( new Rect(text.screenPosition.x, Screen.height - text.screenPosition.y, 128, 32), text.text, style); 
		}
	}
}
