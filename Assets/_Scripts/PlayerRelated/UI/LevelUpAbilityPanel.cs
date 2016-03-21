using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelUpAbilityPanel : MonoBehaviour {
	Image abilityImage;
	Text abilityName;
	Text abilityDescription;

	// Use this for initialization
	void Start () {
		abilityImage = transform.GetChild(0).GetComponent<Image>();
		abilityName = transform.GetChild(1).GetComponent<Text>();
		abilityDescription = transform.GetChild(2).GetComponent<Text>();
	}

	public void SetLevelUpAbilityPanel(Sprite image, string name, string description) {
		abilityImage.sprite = image;
		abilityName.text = name;
		abilityDescription.text = description;
	}
}
