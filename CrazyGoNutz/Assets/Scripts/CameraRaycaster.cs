﻿using UnityEngine;
using System.Collections;

public class CameraRaycaster : MonoBehaviour 
{
	/*
		This Script is placed on the Main Camera Object and handles Mouse Input
	 		-Dragging and Dropping of Workers
	 		-MouseOver
	 		-MouseClickInput
	 */
	GameController gameController = null;
	
	
	// Camera Zoom
	float zoom = 55.0F;
	float zoomMin = 20.0F;	// was 20, maybe should be like 40-50
	float zoomMax = 130.0F;
	
	// Camera Position
	private Vector3 cameraPos = new Vector3();	// Camera Follows this
	
	// Mouse Stuff
	Vector3 mousePos = Vector3.zero;
	Vector3 lastMousePos = Vector3.zero;
	GameObject mouseOverObject = null;
	public Worker mouseOverWorker = null;
	SnapTarget mouseOverSnapTarget = null;
	
	// Drag and Drop Stuff
	Worker dragObject = null;
	
	// Selection Stuff
	public Worker selectedworker = null;
	
	void Start () 
	{
		//gameController = GameObject.Find("GameController").GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Get Mouse Pos
		float mouseX = Input.mousePosition.x;
		float mouseY = Input.mousePosition.y;
		float mouseWheelDelta = Input.GetAxis("Mouse ScrollWheel");
		mousePos = Input.mousePosition;
		
		// Check for Mouse Click Input
		MouseClickInput();
		
		// Check for MouseOver
		MouseOver();
		
		//if(dragObject) Debug.Log("Dragging " + dragObject.name);
		//if(mouseOverObject) Debug.Log("MouseOver " + mouseOverObject.name);
		
		lastMousePos = mousePos;
	}
	
	
	
	/////////////////////////// MOUSE INPUT //////////////////////////////
	
	private void MouseClickInput()
	{	
		if (Input.GetButtonDown ("Fire1"))	// Left Click down first frame
		{
			GameObject clickedObject = RaycastFromMouse();	// Raycast from Camera to Scene and return GameObject
			if(!clickedObject) return;
			
			Vector3 vector = clickedObject.transform.position;
			
			if(clickedObject.CompareTag("SnapTarget") || clickedObject.layer == 8)
         	{
         		//Debug.Log("CameraRaycaster.MouseClickInput -> Clicked on 'SnapTarget' " + clickedObject.name + ".");
				if(selectedworker != null)
				{
					selectedworker.SetSelected(false);
					selectedworker = null;
				}
         	}
			else if(clickedObject.CompareTag("Worker"))
         	{
				if(mouseOverWorker != null) mouseOverWorker.MouseClick();
				if(selectedworker != null && selectedworker != mouseOverWorker)
				{
					selectedworker.SetSelected(false);
					selectedworker = null;
				}
				selectedworker = mouseOverWorker;
				if(selectedworker != null) selectedworker.SetSelected(true);
         	}
         	else
         	{
         		//Debug.Log("CameraRaycaster -> Clicked on " + clickedObject.name + ", has no effect.");
				if(selectedworker != null)
				{
					selectedworker.SetSelected(false);
					selectedworker = null;
				}
         	}	
		}
		else if(Input.GetButton ("Fire1")) // Left Click down continuous 
		{
			if(dragObject == null) BeginMouseDrag();
			else MouseDrag();
		}
		else // Left Click is not down
		{
			if(dragObject != null) MouseDrop();
		}
	}
	
	private void MouseOver()
	{
		if(mousePos != lastMousePos)
		{
			mouseOverObject = RaycastFromMouse();
			GameController.MouseOver(mouseOverObject);
			
			mouseOverWorker = null;
			mouseOverSnapTarget = null;
			if(!mouseOverObject) return;
			
			if(mouseOverObject.CompareTag("SnapTarget") || mouseOverObject.layer == 8)
         	{
				SnapTargetScript script = mouseOverObject.GetComponent<SnapTargetScript>();
				mouseOverSnapTarget = script.GetSnapTarget();
         	}
			else if(mouseOverObject.CompareTag("Worker"))
         	{
				//mouseOverWorker = mouseOverObject;
				WorkerScript script = mouseOverObject.GetComponent<WorkerScript>();
				mouseOverWorker = script.GetWorker();
         	}
         	else
         	{

         	}
		}
	}
	
	private GameObject RaycastFromMouse()
	{
		GameObject raytargetObject = null;
		var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
	    RaycastHit hit;
		
	    if (Physics.Raycast (ray,out hit, 350f)) 
	    {
	        //Debug.DrawLine (ray.origin, hit.point);
	        if(hit.transform != null)
	        {
	        	raytargetObject = hit.transform.gameObject;         	
	        }
		    
		}
		
		return raytargetObject;
	}
	private Vector3 RaycastVectorFromMouse()
	{
		Vector3 rayhitpos = Vector3.zero;
		var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
	    RaycastHit hit;
		
		int layer1 = 0;		// Worker Layer
		int layer2 = 8;		// Worker Layer
		int layerMask = (1 << layer2);
		
		//LayerMask mask = 0;
		
	    if (Physics.Raycast (ray,out hit, 350f)) 
		//if (Physics.Raycast (ray,out hit, 350f, layerMask)) 
	    {
	        Debug.DrawLine (ray.origin, hit.point, Color.red);
	        if(hit.transform != null)
	        {
	        	rayhitpos = hit.point;        	
	        }
		    
		}
		
		return rayhitpos;
	}
	
	/////////////////////////// MOUSE DRAG //////////////////////////////
	
	private void BeginMouseDrag()
	{
		// mouseOverObject becomes dragObject, layer is set to 3
		if(mouseOverWorker != null)
		{
			dragObject = mouseOverWorker;
			//dragObject.layer = 2;
			dragObject.StartDrag();
		} //else Debug.Log("mouseOverWorker is null");
	}
	
	private void MouseDrag()
	{
		// dragObject is follows the mouse until Dropped.
		Vector3 pos = RaycastVectorFromMouse();
		
		if(mouseOverSnapTarget != null && mouseOverSnapTarget.isEmpty() && !mouseOverSnapTarget.isRoom() && !mouseOverSnapTarget.isZone())
		{
			pos = mouseOverSnapTarget.GetPosition();
			//dragObject.transform.position = new Vector3(pos.x, 0.5f, pos.z);
			dragObject.SetPosition( new Vector3(pos.x, 0.5f, pos.z) );
		}
		else if(pos != Vector3.zero)
		{
			//dragObject.transform.position = new Vector3(pos.x, 0.5f, pos.z);
			dragObject.SetPosition( new Vector3(pos.x, 0.5f, pos.z) );
		}
		
		// Highlight SnapTargets Green or Red
	}
	
	private void MouseDrop()
	{
		// check for snapTarget, if one is found and is valid, snap dragObject to it
		// else, snap to last snapTarget
		if(dragObject != null)
		{
			if(mouseOverSnapTarget != null)
			{
				if(mouseOverSnapTarget.isRoom())
				{
					//Debug.Log("Dropped in Room");
					DropInRoom(dragObject, mouseOverSnapTarget);
				}
				else if(mouseOverSnapTarget.isZone())
				{
					DropInZone(dragObject);
				}
				else
				{
					//Debug.Log("Dropped on SnapTarget");
					dragObject.EndDrag(mouseOverSnapTarget);
				}
			}
			else if(mouseOverWorker != dragObject && mouseOverWorker != null)
			{
				DropOnWorker(dragObject, mouseOverWorker);
				dragObject.EndDrag(null);
			}
			else dragObject.EndDrag(null);

			dragObject = null;
		}
	}
	
	private void DropInRoom(Worker worker, SnapTarget snapTarget)
	{
		SnapTarget openSnapTarget = GameController.FindOpenSnapTargetOfType( snapTarget.GetType() );
		
		dragObject.EndDrag(openSnapTarget);
	}
	
	private void DropInZone(Worker worker)
	{
		//SnapTarget openSnapTarget = GameController.FindOpenSnapTargetOfType( snapTarget.GetType() );
		
		dragObject.EndDrag(null);
		
		Vector3 pos = dragObject.GetPosition();
		pos.y = 0;
		dragObject.SetPosition( pos );
	}
	
	private void DropOnWorker(Worker droppedWorker, Worker targetWorker)
	{
		// By dropping one Worker onto another, you remove the Roadblock of the targetWorker, 
		// but increase the frustration of the dropped Worker, as long as they are the same WorkerType
		//if(targetWorker.Roadblocked() && targetWorker.GetWorkerType() == droppedWorker.GetWorkerType())
		if(targetWorker.Roadblocked())
		{
			targetWorker.RemoveRoadblock();		// Remove targetWorker's Roadblock
			droppedWorker.AdjustMood(15);		// Reduce droppedWorker's Mood by 15
			droppedWorker.HelpedOtherStudent();	// Add to droppedWorker.studentsHelped
		}
	}
	
}
