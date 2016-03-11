using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public Transform followObject;		//Object for the camera to follow
	Vector3 startOffset;                //Starting position of the camera relative to the object it is following

	float followSpeed = 0.1f;			//Percent per frame the camera moves towards its target position

	// Use this for initialization
	void Start () {
		startOffset = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Lerp(transform.position, followObject.position + startOffset, followSpeed);
	}
}
