using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using InControl;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour, DamageableObject {
	public List<Ability> abilities;

	public float maxHealth;					//The player's maximum health/combo amount
	public float health;					//The player's current health/combo meter
	public float healthDecayRate;			//The player's rate of health/combo decay over time (health/(second^2))
	public float timeSinceLastCombo = 0;    //Time since the last time the player added to the combo meter
	float timeBeforeDecay;					//Time after the last time the player added to the combo meter before health/combo starts to decay

	public float movespeed;					//The player's max speed
	float acceleration = 150f;				//How quickly a player gets up to max speed
	float decelerationRate = 0.15f;			//(0-1) How quickly a player returns to rest after releasing movement buttons

	public Rigidbody thisRigidbody;			//Reference to the attached Rigidbody
	Collider thisCollider;                  //Reference to the attached Collider

	public InputDevice curDevice;

	// Use this for initialization
	void Start () {
		thisRigidbody = GetComponent<Rigidbody>();
		thisCollider = GetComponent<Collider>();
		curDevice = InputManager.ActiveDevice;

		//Debug characteristics and stats:
		abilities.Add(this.gameObject.AddComponent<Dash>());
		maxHealth = 100;
		health = maxHealth;
		healthDecayRate = 0.025f;
		timeBeforeDecay = 1.5f;
	}
	
	// Update is called once per frame
	void Update () {

		CharacterMovement();

		CharacterAttack();

		//Lose health (combo) over time
		timeSinceLastCombo += Time.deltaTime;
		float timeInDecay = timeSinceLastCombo - timeBeforeDecay;
		if (timeInDecay > 0 && health > 0) {
			health -= timeInDecay * timeInDecay * healthDecayRate;

			//Bottom out at 0 health
			if (health < 0) {
				health = 0;
			}
		}
		GetComponent<MeshRenderer>().material.SetFloat("_Percent", 1 - health / maxHealth);
		GetComponentInChildren<TextMesh>().text = ((int)(health)).ToString();
	}

	void CharacterAttack() {
		if (Input.GetKeyDown(KeyCode.Q)) {
			timeSinceLastCombo = 0;
		}
	}

	void CharacterMovement() {
		/*~~~~~~~~~~~~~~~~~~~~~~~~DEBUG MOVESPEED CHANGE~~~~~~~~~~~~~~~~~~*/
		if (Input.GetKeyDown(KeyCode.PageUp))
			movespeed++;
		if (Input.GetKeyDown(KeyCode.PageDown))
			movespeed--;
		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

		if (curDevice.LeftStick.Vector.magnitude != 0) {
			Move(curDevice.LeftStick.Vector);
		}
		if (curDevice.RightStick.Vector.magnitude != 0) {
			Turn(curDevice.RightStick.Vector);
		}

		/*
		//Movement up
		if (Input.GetKey(KeyCode.UpArrow)) {
			Move(Vector3.forward);
		}
		//Movement down
		else if (Input.GetKey(KeyCode.DownArrow)) {
			Move(Vector3.back);
		}

		//Movement right
		if (Input.GetKey(KeyCode.RightArrow)) {
			Move(Vector3.right);
		}
		//Movement left
		else if (Input.GetKey(KeyCode.LeftArrow)) {
			Move(Vector3.left);
		}
		*/
		//If no direction is being pressed, decelerate the player
		if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) &&
			!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow)) {
			thisRigidbody.velocity = Vector3.Lerp(thisRigidbody.velocity, Vector3.zero, decelerationRate);
		}

		//Limit the player's movement speed to the maximum movespeed
		if (thisRigidbody.velocity.magnitude > movespeed) {
			thisRigidbody.velocity = thisRigidbody.velocity.normalized * movespeed;
		}
	}
	
	void Move(Vector2 direction) {
		Move(new Vector3(direction.x, 0, direction.y));
	}
	void Move(Vector3 direction) {
		thisRigidbody.AddForce(direction * acceleration, ForceMode.Acceleration);
		//thisRigidbody.AddForce(new Vector3(0, 0, acceleration), ForceMode.Acceleration);
		//if (thisRigidbody.velocity.z > movespeed) {
		//	Vector3 cur_vel = thisRigidbody.velocity;
		//	cur_vel.z = movespeed;
		//	thisRigidbody.velocity = cur_vel;
		//}
	}

	void Turn(Vector2 facingDirection) {
		Turn(new Vector3(facingDirection.x, 0, facingDirection.y));
	}
	void Turn(Vector3 facingDirection) {
		transform.forward = facingDirection;
	}

	public void TakeDamage(float damageIn) {
		print("<color=red>TakeDamage() not implemented yet.</color>");
	}
}
