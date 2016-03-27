using UnityEngine;
using System.Collections;

public class DealDamage : MonoBehaviour {

	float baseDamage = 10f;
	static public float damageMultiplier = 1f;
	static public float knockbackScalar = 0f;

	Character thisPlayer;
	Attack playerAttack;
	ParticleSystem impactParticleSystem;

	// Use this for initialization
	void Start () {
		thisPlayer = GetComponentInParent<Character>();
		playerAttack = GetComponentInParent<Attack>();
		impactParticleSystem = thisPlayer.gameObject.GetComponentInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider other) {
		//Try to grab the hit object's DamageableObject component, if it is damagable
		DamageableObject hitObj = other.GetComponent<DamageableObject>();

		//If the object is not damagable, do not proceed with the rest of the function
		if (hitObj == null || hitObj == thisPlayer.GetComponent<DamageableObject>()) {
			return;
		}

		//Calculate damage, and apply it
		float damageDone = baseDamage;
		hitObj.TakeDamage(baseDamage, new Vector3(0, 0, 0));

		//Lock on to the target
		playerAttack.curTarget = other.transform;

		//Add to the player's combo meter
		thisPlayer.health += damageDone / 10f;

		//Add camera shake
		CameraFollow.S.CameraShake(0.1f, 0.75f);

		//Play the impact particle system
		impactParticleSystem.gameObject.transform.position = transform.position;
		impactParticleSystem.Play();
	}
}
