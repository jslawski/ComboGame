using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {
	public Character thisPlayer;
	Quaternion defaultWeaponRot;

	int consecutiveAttacksPerformed = 0;							//Which attack the player is on
	float consecutiveAttackTimingWindow = 0.5f;						//How long after an attack the player has to perform a followup attack
	float internalAttackCooldown = 0.05f;							//Minimum time between attacks

	float baseDamage = 20f;											//Base damage of the attacks
	float[] attackMultipliers = {1f, 1.75f, 3.5f};					//Base damage multiplier on each attack

	float[] startAttackRotations = {-45f, -45f, -75f};				//Starting euler rotations for each attack
	float[] attackRotations = {90f, 120f, 360f};					//Total degrees the weapon turns during each attack
	Vector3[] attackAxes = {Vector3.up, Vector3.down, Vector3.up};  //Axis of rotation for each attack
	float[] attackTimes = {0.15f, 0.25f, 0.5f};                     //Time it takes to perform each attack
	float[] attackMovement = {0.5f, 1.2f, 2f};						//Force forward the player steps with each attack

	bool inAttackCoroutine = false;

	// Use this for initialization
	void Awake () {
		thisPlayer = GetComponentInParent<Character>();

		defaultWeaponRot = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		if (thisPlayer.curDevice != null && thisPlayer.curDevice.RightTrigger.WasPressed && !inAttackCoroutine) {
			StartCoroutine(AttackCoroutine());
		}
		else if (!inAttackCoroutine) {
			//Return the weapon back to its default rotation
			transform.localRotation = Quaternion.Lerp(transform.localRotation, defaultWeaponRot, 0.1f);
		}
	}

	IEnumerator AttackCoroutine() {
		inAttackCoroutine = true;

		while (consecutiveAttacksPerformed < 3) {
			//print("Swing " + (consecutiveAttacksPerformed+1));

			//Calculate attack parameters
			Vector3 startEulerRot = attackAxes[consecutiveAttacksPerformed] * startAttackRotations[consecutiveAttacksPerformed];
			Vector3 endEulerRot = startEulerRot + attackAxes[consecutiveAttacksPerformed] * attackRotations[consecutiveAttacksPerformed];

			//print(startEulerRot + "\t" + endEulerRot);
			transform.localRotation = Quaternion.Euler(startEulerRot);

			float attackTime = attackTimes[consecutiveAttacksPerformed];

			//thisPlayer.DisableControls(attackTime);

			//Perform attack
			float elapsedAttackTime = 0;
			thisPlayer.thisRigidbody.AddForce(thisPlayer.transform.forward * attackMovement[consecutiveAttacksPerformed] / attackTime, ForceMode.Impulse);
			GetComponentInChildren<BoxCollider>().enabled = true;
			while (elapsedAttackTime < attackTime) {
				elapsedAttackTime += Time.deltaTime;
				float percent = elapsedAttackTime / attackTime;

				transform.localRotation = Quaternion.Euler(Vector3.Lerp(startEulerRot, endEulerRot, percent));

				yield return null;
			}
			//Weapon shouldn't do damage when not attacking
			GetComponentInChildren<BoxCollider>().enabled = false;

			consecutiveAttacksPerformed++;

			//Post-attack

			bool queueUpNextAttack = false; //Whether we should perform next attack
			float timeAfterAttack = 0;      //How much time has passed since completing last attack

			//While we haven't waited the minimum time required yet, OR we are still waiting to see if the player wants to perform next attack
			while (timeAfterAttack < internalAttackCooldown || (!queueUpNextAttack && timeAfterAttack < consecutiveAttackTimingWindow + internalAttackCooldown)) {
				timeAfterAttack += Time.deltaTime;
				if (!queueUpNextAttack && thisPlayer.curDevice != null && thisPlayer.curDevice.RightTrigger.WasPressed) {
					queueUpNextAttack = true;
				}

				yield return null;
			}

			if (!queueUpNextAttack) {
				consecutiveAttacksPerformed = 0;
				inAttackCoroutine = false;
				yield break;    //Exit coroutine
			}
		}

		consecutiveAttacksPerformed = 0;
		inAttackCoroutine = false;
	}

	void OnTriggerEnter(Collider other) {
		DamageableObject hitObj = other.GetComponent<DamageableObject>();
		if (hitObj == null || hitObj == thisPlayer.GetComponent<DamageableObject>()) {
			return;
		}

		float attackTime = attackTimes[consecutiveAttacksPerformed];
		Vector3 knockbackForce = thisPlayer.transform.forward * attackMovement[consecutiveAttacksPerformed] / attackTime;
		float damageDone = baseDamage * attackMultipliers[consecutiveAttacksPerformed];
        hitObj.TakeDamage(baseDamage, knockbackForce);

		thisPlayer.health += damageDone/10f;
	}
}
