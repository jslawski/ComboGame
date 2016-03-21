using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelUpDisplay : MonoBehaviour {
	public Sprite debugSprite;

	LevelUpAbilityPanel[] abilityPanels;

	// Use this for initialization
	void Start () {
		abilityPanels = GetComponentsInChildren<LevelUpAbilityPanel>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
