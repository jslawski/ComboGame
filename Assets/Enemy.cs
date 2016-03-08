using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, DamageableObject {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void TakeDamage(float damageIn) {
		print("OW");
	}
}
