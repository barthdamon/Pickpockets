using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDetection : MonoBehaviour {

	[SerializeField]
	public GameObject[] DetectableObjects;

	public GameObject Head;
	public float DetectionThreshold = 100f;

	// Scalar floats for tuning detection formula
	public float Scalar_EyeContact = 1f;
	public float Scalar_Onlooker = 1f;
	public float Static_Scalar_Distance = 1f;
	public float Scalar_FOV = 1f;
	// TODO: SOUND detection

	private bool bNoticesPlayer;
	private GameObject CurrentPrimaryDetectedObject;


	// Use this for initialization
	void Start () {
		bNoticesPlayer = false;
	}
	
	// Update is called once per frame
	void Update () {

		// loop through all valid attention objects in the world and calculate detection
		// If any are over the threshold check if they are over the threshold by more of an amount than the others
		// set max over threshold as current primarydetectedobject

		// need a central manager singleton that objects register with and that AI tell what they are paying most attention to
		// so that agents can query to see what other agents are looking at
		IDetectableObject DetectableObject = DetectableObjects[0].GetComponent<DetectablePlayer>() as IDetectableObject;
		if (DetectableObject != null) 
		{
			bNoticesPlayer = CalculateDetection (DetectableObject) > DetectionThreshold;

			if (bNoticesPlayer) {
				FollowObject (DetectableObject);
			} else {
				Idle ();
			}
		}

	}

	// TODO: Calculate detection for every gameobject, then train attention to most detected
	float CalculateDetection(IDetectableObject DetectableObject)
	{
		float DetectionSensitivity = 0f;

		// #Distance from agent#
		Vector3 ToObjectVector = DetectableObject.GetPosition() - transform.position;
		Vector3 DirToObject = ToObjectVector.normalized;

		if (ToObjectVector.magnitude > 1) {
			DetectionSensitivity += (1 / ToObjectVector.magnitude) * Static_Scalar_Distance;
		} else {
			DetectionSensitivity += Static_Scalar_Distance;
		}


		if (DetectableObject.UsesFacing ()) {
			
			// #Sensing onlooker#
			float SenseOnlookerDot = Vector3.Dot (DirToObject, DetectableObject.GetFacingDirection());
			// Multiply by -1 here cause we want the sensitivity of them facing away from each other
			//DetectionSensitivity += SenseOnlookerDot * -1 * Scalar_Onlooker;

			// #Eye contact#
			float EyeContactDot = Vector3.Dot (Head.transform.forward, DetectableObject.GetFacingDirection());
			// again by -1
			//DetectionSensitivity += EyeContactDot * -1 * Scalar_EyeContact;
		}

		// #Within FOV of agent#
		float FOVDot = Vector3.Dot(Head.transform.forward, DirToObject);
		//DetectionSensitivity += FOVDot * Scalar_FOV;

		//TODO: Sound, alertness of other agents to object

		Debug.Log ("D: " + DetectionSensitivity);
		return DetectionSensitivity;
	}


	void FollowObject(IDetectableObject Object)
	{
		Vector3 DirToObject = (Object.GetPosition() - Head.transform.position).normalized;
		Quaternion DirToObjectQuat = Quaternion.LookRotation (DirToObject);

		// lerp rotation
		Quaternion HeadRotation = Head.transform.rotation;
		Quaternion LerpedRotation = Quaternion.Slerp(HeadRotation, DirToObjectQuat, 0.5f);
		Head.transform.rotation = LerpedRotation;
		//Debug.Log ("Rotation" + DirToObjectQuat);

	}

	void Idle()
	{
		// do idle things with head swirling around randomly to get vector with player sometimes when they arent careful

		// This should be the agents goal mostly. Moving agents should have goals whetther that is a 
		// nav position or current focused object

	}
}



public interface IDetectableObject
{
	Vector3 GetPosition();
	bool UsesFacing();
	Vector3 GetFacingDirection();
}