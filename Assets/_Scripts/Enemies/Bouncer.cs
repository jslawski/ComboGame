using UnityEngine;
using System.Collections;

public class Bouncer : Enemy {

	Rigidbody thisRigidbody;
	float moveSpeed = 5;
	float acceleration = 500f;
	Vector3 direction = new Vector3(1, 0, 1);

	// Use this for initialization
	void Start () {
		thisRigidbody = GetComponent<Rigidbody>();
		
	}
	
	// Update is called once per frame
	void Update () {
		thisRigidbody.AddForce(direction*moveSpeed, ForceMode.Acceleration);

		//Limit bouncer's maximum movespeed
		if (thisRigidbody.velocity.magnitude > moveSpeed) {
			thisRigidbody.velocity = thisRigidbody.velocity.normalized * moveSpeed;
		}
	}


	//NOTE:  We have to do it this way because using a physic material is wildly inconsistent
	void OnCollisionEnter(Collision other) {
		Vector3 collisionPoint = other.contacts[0].point;
		
		//Reflect the bouncer's trajectory, based on what wall it collided with
		//Top and bottom walls (flip sign of z-direction)
		if (Mathf.Abs(collisionPoint.z - transform.position.z) >= Mathf.Abs(collisionPoint.x - transform.position.x)) {
			direction = new Vector3(direction.x, direction.y, -direction.z);
		}
		//Left and right walls (flip sign of x-direction)
		else if (Mathf.Abs(collisionPoint.z - transform.position.z) <= Mathf.Abs(collisionPoint.x - transform.position.x)) {
			direction = new Vector3(-direction.x, direction.y, direction.z);	
		}
		//Bounce the bouncer off the wall, so it maintains its speed
		thisRigidbody.AddForce(direction * moveSpeed, ForceMode.Impulse);
	}
}
