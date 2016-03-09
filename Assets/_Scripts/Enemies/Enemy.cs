using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, DamageableObject {

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
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void TakeDamage(float damageIn) {
		enemyHealth -= damageIn;
	}
}
