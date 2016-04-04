using UnityEngine;
using System.Collections;

public class BomberAI : MonoBehaviour {

	NavMeshAgent agent;							//Reference to the navmesh agent
	bool inPursuit = false;                     //Whether or not the enemy is currently chasing the player
	float detonateDistanceThreshold = 1.5f;     //Distance the enemy has to be from the player before it explodes
	Transform thisTransform;					//Reference to the enemy transform
	Transform characterTransform;               //Reference to the target player transform
	GameObject explosionPrefab;					//Reference to the explosion prefab that replaces the bomber after it detonates

	// Use this for initialization
	void Start () {
		agent = GetComponentInParent<NavMeshAgent>();
		thisTransform = gameObject.transform;
		explosionPrefab = Resources.Load("Prefabs/BomberExplosion") as GameObject;
	}
	
	void Detonate() {
		//Shake camera heavily
		CameraFollow.S.CameraShake(0.1f, 1.5f);

		Instantiate(explosionPrefab, thisTransform.position, new Quaternion());
		Destroy(thisTransform.parent.gameObject);
	}

	float GetDistance(Vector3 p1, Vector3 p2) {
		return (Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2) + Mathf.Pow(p2.z - p1.z, 2)));
	}

	// Update is called once per frame
	void Update () {
		//Explode if it's in pursuit and gets close enough to the player
		if (inPursuit) {
			if (GetDistance(thisTransform.position, characterTransform.position) <= detonateDistanceThreshold) {
				Detonate();
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			inPursuit = true;
			characterTransform = other.gameObject.transform;
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.tag == "Player" && agent.isActiveAndEnabled) {
			agent.SetDestination(characterTransform.position);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			inPursuit = false;
			agent.destination = gameObject.transform.position;
		}
	}
}
