using UnityEngine;
using System.Collections;

public class CameraRaycaster : MonoBehaviour 
{
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
	GameObject mouseOverWorker = null;
	GameObject mouseOverSnapTarget = null;
	
	// Drag and Drop Stuff
	GameObject dragObject = null;	// Ignores Raycasting
	
	// Camera Bounds
	
	// Use this for initialization
	void Start () 
	{
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
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
		if(mouseOverObject) Debug.Log("MouseOver " + mouseOverObject.name);
		
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
         		Debug.Log("CameraRaycaster.MouseClickInput -> Clicked on 'SnapTarget' " + clickedObject.name + ".");
         	}
			else if(clickedObject.CompareTag("Worker"))
         	{
         		Debug.Log("CameraRaycaster.MouseClickInput -> Clicked on 'Worker' " + clickedObject.name + ".");
         	}
         	else
         	{
         		Debug.Log("CameraRaycaster -> Clicked on " + clickedObject.name + ", has no effect.");
         	}	
		}
		else if(Input.GetButton ("Fire1")) // Left Click down continuous 
		{
			if(!dragObject) BeginMouseDrag();
			else MouseDrag();
		}
		else // Left Click is not down
		{
			if(dragObject) MouseDrop();
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
				mouseOverSnapTarget = mouseOverObject;
         	}
			else if(mouseOverObject.CompareTag("Worker"))
         	{
				mouseOverWorker = mouseOverObject;
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
	        Debug.DrawLine (ray.origin, hit.point);
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
		
	    if (Physics.Raycast (ray,out hit, 350f)) 
	    {
	        Debug.DrawLine (ray.origin, hit.point);
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
		if(mouseOverWorker)
		{
			dragObject = mouseOverWorker;
			dragObject.layer = 2;
		} else Debug.Log("mouseOverWorker is null");
	}
	
	private void MouseDrag()
	{
		// dragObject is follows the mouse until Dropped.
		Vector3 pos = RaycastVectorFromMouse();
		
		if(mouseOverSnapTarget)
		{
			pos = mouseOverSnapTarget.transform.position;
			dragObject.transform.position = new Vector3(pos.x, 0.5f, pos.z);
		}
		else if(pos != Vector3.zero)
		{
			dragObject.transform.position = new Vector3(pos.x, 0.5f, pos.z);
		}
	}
	
	private void MouseDrop()
	{
		// check for snapTarget, if one is found and is valid, snap dragObject to it
		// else, snap to last snapTarget
		if(dragObject)
		{
			dragObject.layer = 0;
			dragObject = null;
		}
	}
	
}
