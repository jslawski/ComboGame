using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public static GameManager S;

	public GameObject experiencePrefab;
	public Character player;

	float experienceDropDelay = 0.1f;		//Time in seconds between each experience dropped by an enemy

	// Use this for initialization
	void Awake () {
		S = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DropExperienceAtLocation(Vector3 location, int amount) {
		StartCoroutine(DropExperienceCoroutine(location, amount));
	}
	IEnumerator DropExperienceCoroutine(Vector3 location, int amount) {
		for (int i = 0; i < amount; i++) {
			GameObject newExperienceObj = Instantiate(experiencePrefab, location, new Quaternion()) as GameObject;
			Rigidbody newExperienceRigidbody = newExperienceObj.GetComponent<Rigidbody>();

			Vector2 randDirectionCircle = Random.insideUnitCircle;
			Vector3 randDirection = new Vector3(randDirectionCircle.x, 0, randDirectionCircle.y);

			newExperienceRigidbody.AddForce(randDirection, ForceMode.Impulse);

			yield return new WaitForSeconds(experienceDropDelay);
		}
	}
}
