using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour, DamageableObject {
	Rigidbody thisRigidbody;
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
	void Start () {
		thisRigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (thisRigidbody.IsSleeping()) thisRigidbody.WakeUp();
	}

	public void TakeDamage(float damageIn, Vector3 knockback) {
		enemyHealth -= damageIn;

		thisRigidbody.AddForce(knockback, ForceMode.Impulse);
		print("Took " + damageIn + " damage with " + knockback + " knockback.");
	}
}
