using UnityEngine;
using System.Collections;

public interface DamageableObject {

	float maxHealth { get; set; }
	float health { get; set; }

	void TakeDamage(float damageIn, Vector3 knockback=default(Vector3));
}
