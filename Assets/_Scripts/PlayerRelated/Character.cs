using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using InControl;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour, DamageableObject {
	float bulletTimeScale_c = 0.05f;

	public InputDevice curDevice;
	
	public List<Ability> abilities;

	public float playerMaxHealth;				//The player's maximum health/combo amount
	public float playerHealth;                  //The player's current health/combo meter
	public float healthDecayRate;				//The player's rate of health/combo decay over time (health/second)
	public float timeSinceLastCombo = 0;        //Time since the last time the player added to the combo meter
	float timeBeforeDecay;						//Time after the last time the player added to the combo meter before health/combo starts to decay

	public float movespeed;					//The player's max speed
	float acceleration = 150f;				//How quickly a player gets up to max speed
	float decelerationRate = 0.15f;         //(0-1) How quickly a player returns to rest after releasing movement buttons

	int playerLevel = 1;					//Player levels up when playerExperience >= expForNextLevel
	int expForNextLevel = 100;           //Increases every time the player levels up
	int playerExperience = 0;			//Decreases to ~0 whenever player levels up

	public Rigidbody thisRigidbody;			//Reference to the attached Rigidbody
	Collider thisCollider;                  //Reference to the attached Collider
	Vector3 targetForward;					//Which direction the player should attempt to be facing
	float turnSpeed = 0.4f;                 //(0-1) How quickly the player reaches the desired direction to face

	bool controlsDisabled = false;

	/*~~~~~~~~~~Properties~~~~~~~~~~*/
	public float maxHealth {
		get { return playerMaxHealth; }
		set { playerMaxHealth = value; }
	}

	public float health {
		get { return playerHealth; }
		set {
			//If our health went up, reset the decay timer
			if (value > playerHealth) {
				timeSinceLastCombo = 0;
				//If this would put us above our max health, put us at our max health instead
				if (value > playerMaxHealth) {
					playerHealth = playerMaxHealth;
					return;
				}
			}
			//Bottom out at 0 health
			else if (value < 0) {
				playerHealth = 0;
				return;
			}
			playerHealth = value;
		}
	}

	public int experience {
		get { return playerExperience; }
		set {
			playerExperience = value;
			if (playerExperience > expForNextLevel) {
				playerExperience -= expForNextLevel;
				playerLevel++;
				expForNextLevel = GetExpNeededForNextLevel();
				print("PLAYER LEVELED UP TO LEVEL " + playerLevel + "!\nExperience needed to level up to Level " + (playerLevel+1) + ": " + expForNextLevel);
			}
		}
	}

	// Use this for initialization
	void Start () {
		thisRigidbody = GetComponent<Rigidbody>();
		thisCollider = GetComponent<Collider>();
		curDevice = (InputManager.ActiveDevice.Name == "NullInputDevice") ? null : InputManager.ActiveDevice;

		//Debug characteristics and stats:
		abilities.Add(this.gameObject.AddComponent<Dash>());
		maxHealth = 100;
		health = maxHealth;
		healthDecayRate = 0.0625f;
		timeBeforeDecay = 1f;
	}
	
	// Update is called once per frame
	void Update () {
		if (curDevice == null) {
			foreach (var device in InputManager.Devices) {
				if (device.AnyButton) {
					print("Controller added");
					curDevice = device;
				}
			}
		}

		//Lose health (combo) over time
		timeSinceLastCombo += Time.deltaTime;
		float timeInDecay = timeSinceLastCombo - timeBeforeDecay;
		if (timeInDecay > 0 && health > 0) {
			health -= timeInDecay * healthDecayRate;
		}

		//TODO: Don't do GetComponent calls on Update()
		GetComponent<MeshRenderer>().material.SetFloat("_Percent", 1 - health / maxHealth);
		GetComponentInChildren<TextMesh>().text = ((int)(health)).ToString();

		CharacterMovement();
	}

	void CharacterMovement() {
		if (controlsDisabled) {
			return;
		}
		/*~~~~~~~~~~~~~~~~~~~~~~~~DEBUG MOVESPEED CHANGE~~~~~~~~~~~~~~~~~~*/
		if (Input.GetKeyDown(KeyCode.PageUp))
			movespeed++;
		if (Input.GetKeyDown(KeyCode.PageDown))
			movespeed--;
		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
		
		//Controller movement
		if (curDevice != null) {
			//Change back to normal-time
			if (!curDevice.Action1) {
				Time.timeScale = 1;
				Time.fixedDeltaTime = 0.02f;
			}

			if (curDevice.LeftStick.Vector.magnitude != 0) {
				Move(curDevice.LeftStick.Vector/Time.timeScale);
			}
			else {
				//If no direction is being pressed, decelerate the player
				thisRigidbody.velocity = Vector3.Lerp(thisRigidbody.velocity, Vector3.zero, decelerationRate);
			}
			if (curDevice.RightStick.Vector.magnitude != 0) {
				Turn(curDevice.RightStick.Vector/Time.timeScale);
			}

			//Change to bullet-time
			if (curDevice.Action1) {
				Time.timeScale = bulletTimeScale_c;
				Time.fixedDeltaTime = 0.02f * bulletTimeScale_c;
			}
		}
		//Keyboard movement
		else {
			//Movement up
			if (Input.GetKey(KeyCode.W)) {
				Move(Vector3.forward);
			}
			//Movement down
			else if (Input.GetKey(KeyCode.S)) {
				Move(Vector3.back);
			}
			//Movement right
			if (Input.GetKey(KeyCode.D)) {
				Move(Vector3.right);
			}
			//Movement left
			else if (Input.GetKey(KeyCode.A)) {
				Move(Vector3.left);
			}

			//If no direction is being pressed, decelerate the player
			if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) &&
				!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow)) {
				thisRigidbody.velocity = Vector3.Lerp(thisRigidbody.velocity, Vector3.zero, decelerationRate);
			}

			//Turning
			if (Input.GetKey(KeyCode.UpArrow)) {
				Turn(Vector3.Lerp(transform.forward, Vector3.forward, 0.3f));
			}
			else if (Input.GetKey(KeyCode.DownArrow)) {
				Turn(Vector3.Lerp(transform.forward, Vector3.back, 0.3f));
			}
			else if (Input.GetKey(KeyCode.RightArrow)) {
				Turn(Vector3.Lerp(transform.forward, Vector3.right, 0.3f));
			}
			else if (Input.GetKey(KeyCode.LeftArrow)) {
				Turn(Vector3.Lerp(transform.forward, Vector3.left, 0.3f));
			}
		}

		//If no direction is being pressed, decelerate the player
		/*if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) &&
			!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow)) {
			thisRigidbody.velocity = Vector3.Lerp(thisRigidbody.velocity, Vector3.zero, decelerationRate);
		}
		*/
		//Limit the player's movement speed to the maximum movespeed
		if (thisRigidbody.velocity.magnitude > movespeed) {
			thisRigidbody.velocity = thisRigidbody.velocity.normalized * movespeed;
		}
		//Lerp the player's forward vector to the desired forward vector
		transform.forward = Vector3.Lerp(transform.forward, targetForward, turnSpeed);
	}
	
	void Move(Vector2 direction) {
		Move(new Vector3(direction.x, 0, direction.y));
	}
	void Move(Vector3 direction) {
		thisRigidbody.AddForce(((direction / Time.timeScale) * acceleration), ForceMode.Acceleration);
	}

	void OnTriggerEnter(Collider other) {
		//Gain experience
		if (other.gameObject.CompareTag("Experience")) {
			experience++;
			Destroy(other.gameObject);
		}
	}

	int GetExpNeededForNextLevel() {
		return 10 * (playerLevel - 1) * (playerLevel - 1) + 100;
	}

	void Turn(Vector2 facingDirection) {
		Turn(new Vector3(facingDirection.x, 0, facingDirection.y));
	}
	void Turn(Vector3 facingDirection) {
		targetForward = facingDirection;
	}

	public void DisableControls(float duration) {
		StartCoroutine(DisableControlsCoroutine(duration));
	}
	IEnumerator DisableControlsCoroutine(float duration) {
		controlsDisabled = true;
		yield return new WaitForSeconds(duration);
		controlsDisabled = false;
	}

	public void TakeDamage(float damageIn, Vector3 knockback) {
		print("<color=red>TakeDamage() not implemented yet.</color>");
	}
}
