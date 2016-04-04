using UnityEngine;
using System.Collections;

public class BomberExplosion : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			other.gameObject.GetComponent<Character>().TakeDamage(30f, Vector3.zero, 0);
		}
	}
}
