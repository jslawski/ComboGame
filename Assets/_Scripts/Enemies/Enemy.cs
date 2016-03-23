using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour, DamageableObject {
	Rigidbody thisRigidbody;

	int experienceOnDeath;

	float enemyMaxHealth;
	float enemyHealth;

	/*~~~~~~~~~~Properties~~~~~~~~~~*/
	public float maxHealth {
		get { return enemyMaxHealth; }
		set { enemyMaxHealth = value; }
	}

	public float health {
		get { return enemyHealth; }
		set { enemyHealth = value; }
	}

	// Use this for initialization
	virtual protected void Start () {
		thisRigidbody = GetComponent<Rigidbody>();

		//Debug stats
		experienceOnDeath = 10;
		enemyMaxHealth = 100f;
		enemyHealth = enemyMaxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		if (thisRigidbody.IsSleeping()) thisRigidbody.WakeUp();
	}

	public void TakeDamage(float damageIn, Vector3 knockback, float stunDuration) {
		enemyHealth -= damageIn;

		if (enemyHealth <= 0) {
			Die();
			return;
		}

		thisRigidbody.AddForce(knockback, ForceMode.VelocityChange);
		//print("Took " + damageIn + " damage with " + knockback + " knockback.");
	}

	void Die() {
		//print("Alas, I have been slain.");

		GameManager.S.DropExperienceAtLocation(transform.position, experienceOnDeath);
		Destroy(this.gameObject);
	}
}
