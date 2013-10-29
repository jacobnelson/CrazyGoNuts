using UnityEngine;
using System.Collections;

public class SnapTarget
{
	private SnapTargetType snapTargetType;
	//private SnapTargetScript script = null;
	private bool isRoom = false;
	
	private bool empty = true;
	private Worker currentWorker = null;
	public GameObject workStation = null;
	
	private bool mouseOver = false;
	
	public SnapTarget(GameObject gameObject, SnapTargetType snapTargetType, bool isRoom)
	{
		this.workStation = gameObject;
		this.snapTargetType = snapTargetType;
		this.isRoom = isRoom;
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
}

public enum SnapTargetType
{
	Workstation,
	Conference,
	Recreation
}
