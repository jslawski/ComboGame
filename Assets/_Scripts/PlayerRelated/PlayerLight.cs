using UnityEngine;
using System.Collections;

public class PlayerLight : MonoBehaviour {
	Color normalColor = Color.white;
	Color strobeColor = new Color(0, 0, 1, 1);
	Light thisLight;

	float strobeFrequency = 4f;

	// Use this for initialization
	void Start () {
		thisLight = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Strobe(float duration) {
		StartCoroutine(StrobeCoroutine(duration));
	}

	IEnumerator StrobeCoroutine(float duration) {
		float timeElapsed = 0;

		thisLight.color = strobeColor;
		while (timeElapsed < duration) {
			timeElapsed += Time.deltaTime;

			thisLight.color = (timeElapsed % (1 / strobeFrequency) < (1 / (strobeFrequency*2))) ? strobeColor : normalColor;

			yield return 0;
		}

		thisLight.color = normalColor;
	}
}
