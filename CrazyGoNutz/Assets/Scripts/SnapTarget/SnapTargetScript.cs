using UnityEngine;
using System.Collections;

public class SnapTargetScript : MonoBehaviour
{
	public bool isRoom = false;
	public SnapTargetType snapTargetType;
	private SnapTarget snapTarget = null;
	
	void Awake () 
	{
		tag = "SnapTarget";
	}
	
	void Start()
	{
		if(snapTarget == null) snapTarget = GameController.AddSnapTarget(gameObject);	// TODO: This will change.
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
