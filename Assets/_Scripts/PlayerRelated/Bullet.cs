using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float bulletSpeed;
	public Character thisPlayer;

	// Use this for initialization
	void Awake () {
		bulletSpeed = 20;
		StartCoroutine(DestroyBullet());
	}
	
	IEnumerator DestroyBullet() {
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other) {
		Destroy(gameObject);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
