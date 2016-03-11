using UnityEngine;
using System.Collections;

public class ExperienceTriggerZone : MonoBehaviour {
	float defaultFollowSpeed = 0.01f;
	float acceleration = 1.05f;				//Multiplier on follow speed as the player stays in the trigger zone
    float curFollowSpeed;					//Increases the longer the player stays in the trigger zone

	// Use this for initialization
	void Start () {
		curFollowSpeed = defaultFollowSpeed;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag != "Player") {
			return;
		}

		transform.position = Vector3.Lerp(transform.position, other.gameObject.transform.position, curFollowSpeed);
		curFollowSpeed *= acceleration;
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag != "Player") {
			return;
		}

		curFollowSpeed = defaultFollowSpeed;
	}
}
