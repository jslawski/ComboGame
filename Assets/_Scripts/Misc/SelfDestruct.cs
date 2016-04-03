using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {
	public float timeUntilSelfDestruct = float.MaxValue;

	// Use this for initialization
	IEnumerator Start () {
		float timeElapsed = 0;
		while (timeElapsed < timeUntilSelfDestruct) {
			timeElapsed += Time.deltaTime;

			yield return null;
		}

		Destroy(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
