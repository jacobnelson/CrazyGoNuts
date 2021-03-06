﻿using UnityEngine;
using System.Collections;

public class SnapTarget
{
	private SnapTargetType snapTargetType;
	//private SnapTargetScript script = null;
	private bool room = false;
	private bool zone = false;
	
	private bool empty = true;
	public Worker currentWorker = null;
	public GameObject workStation = null;	// Associated GameObject within the Scene
	
	private bool mouseOver = false;

	public string animation = "Idle";
	
	public SnapTarget(GameObject gameObject, SnapTargetType snapTargetType, bool room)
	{
		this.workStation = gameObject;
		this.snapTargetType = snapTargetType;
		this.room = room;
		
		SnapTargetScript script = workStation.GetComponent<SnapTargetScript>();
		this.zone = script.isAZone();
		this.animation = script.animation.ToString();
	}
	
	/////////////////////////// MOUSE OVER //////////////////////////////
	
	public void SetMouseOver(bool mouseOver)
	{
		this.mouseOver = mouseOver;
	}
	public bool GetMouseOver()
	{
		return mouseOver;
	}
	
	/////////////////////////// WORKER //////////////////////////////
	
	public void SetWorker(Worker worker)
	{
		currentWorker = worker;
		if(currentWorker == null) empty = true;
		else empty = false;
	}
	
	/////////////////////////// GET VARS //////////////////////////////
	
	public bool isRoom()
	{
		return room;
	}
	public bool isZone()
	{
		return zone;
	}
	public bool isEmpty()
	{
		return empty;
	}
	public Vector3 GetPosition()
	{
		return workStation.transform.position;
	}
	
	public bool MatchesType(SnapTargetType type)
	{
		return (this.snapTargetType == type);
	}
	public SnapTargetType GetType()
	{
		return snapTargetType;
	}
}

public enum SnapTargetType
{
	Workstation,
	Conference,
	Recreation
}
