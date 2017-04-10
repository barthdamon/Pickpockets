using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectablePlayer : MonoBehaviour, IDetectableObject {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Vector3 GetPosition()
	{
		//Debug.Log ("Returning position " + transform.position);
		return transform.position;
	}

	public bool UsesFacing()
	{
		return true;
	}

	public Vector3 GetFacingDirection()
	{
		//Debug.Log ("Returning forward facing direction " + transform.forward);
		return transform.forward;
	}

}
