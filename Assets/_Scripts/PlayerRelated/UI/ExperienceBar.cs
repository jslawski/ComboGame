using UnityEngine;
using System.Collections;

public class ExperienceBar : MonoBehaviour {
	public float targetPercent;		//The true percent of a level-up of the player currently
	float displayedPercent;			//Lerps between current value and targetExp every frame

	Renderer expRenderer;

	// Use this for initialization
	void Start () {
		expRenderer = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		displayedPercent = Mathf.Lerp(displayedPercent, targetPercent, 0.1f);
		expRenderer.material.SetFloat("_Cutoff", 1-displayedPercent);
	}
}
