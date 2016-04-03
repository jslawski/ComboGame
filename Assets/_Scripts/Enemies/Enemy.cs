using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour, DamageableObject {
	Rigidbody thisRigidbody;
	GameObject deathParticleSystem;

	protected int experienceOnDeath;

	float enemyMaxHealth;
	float enemyHealth;
	float stunDurationLeft;

	/*~~~~~~~~~~Properties~~~~~~~~~~*/
	public float maxHealth {
		get { return enemyMaxHealth; }
		set { enemyMaxHealth = value; }
	}

	public float health {
		get { return enemyHealth; }
		set { enemyHealth = value; }
	}

	public bool stunned {
		get { return stunDurationLeft > 0; }
	}

	// Use this for initialization
	virtual protected void Start () {
		thisRigidbody = GetComponent<Rigidbody>();

		//Debug stats
		experienceOnDeath = 10;
		enemyMaxHealth = 100f;
		enemyHealth = enemyMaxHealth;
		deathParticleSystem = Resources.Load<GameObject>("Prefabs/EnemyDeathParticleSystem");
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (thisRigidbody.IsSleeping()) thisRigidbody.WakeUp();
		
		if (stunDurationLeft > 0) {
			stunDurationLeft -= Time.deltaTime;
		}
	}

	public void TakeDamage(float damageIn, Vector3 knockback, float stunDuration) {
		enemyHealth -= damageIn;

		if (enemyHealth <= 0) {
			Die();
			return;
		}

		thisRigidbody.AddForce(knockback, ForceMode.Impulse);
		stunDurationLeft = Mathf.Max(stunDuration, stunDurationLeft);
		//print("Took " + damageIn + " damage with " + knockback + " knockback.");
	}

	void Die() {
		//print("Alas, I have been slain.");

		GameManager.S.DropExperienceAtLocation(transform.position, experienceOnDeath);
		GameObject particleSystem = Instantiate(deathParticleSystem, transform.position, new Quaternion()) as GameObject;
		particleSystem.GetComponent<SelfDestruct>().timeUntilSelfDestruct = particleSystem.GetComponent<ParticleSystem>().duration;

		Destroy(this.gameObject);
	}
}
