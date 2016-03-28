using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelUpDisplay : MonoBehaviour {
	public static LevelUpDisplay S;
	public Sprite debugSprite;

	LevelUpAbilityPanel[] abilityPanels;

	RectTransform thisRect;
	float menuInOutTime = 0.5f;			//How long it takes the LevelUp screen to lerp into and out of position

	void Awake() {
		S = this;
	}

	// Use this for initialization
	void Start () {
		abilityPanels = GetComponentsInChildren<LevelUpAbilityPanel>();
		thisRect = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space")) {
			LevelUp(GameManager.S.player);
		}
	}

	public void LevelUp(Character thisPlayer) {
		StartCoroutine(LevelUpCoroutine(thisPlayer));
	}
	IEnumerator LevelUpCoroutine(Character player) {
		Vector2 startOffset = new Vector2(0, Screen.height);
		Vector2 endOffset = Vector2.zero;
		thisRect.offsetMin = thisRect.offsetMax = startOffset;

		//Lerp in
		float timeElapsed = 0;
		while (timeElapsed < menuInOutTime) {
			timeElapsed += Time.deltaTime;
			float t = timeElapsed/menuInOutTime;

			thisRect.offsetMax = thisRect.offsetMin = Vector2.Lerp(startOffset, endOffset, t*t*t);

			yield return 0;
		}

		List<AbilityInfo> levelUpAbilities = AbilityPool.S.GetRandomAbilitiesFromPool(4);
		for (int i = 0; i < levelUpAbilities.Count; i++) {
			abilityPanels[i].SetLevelUpAbilityPanel(levelUpAbilities[i]);
		}

		//Wait for player input
		while (true) {
			if (player.curDevice != null) {
				if (player.curDevice.Action1.WasPressed) {
					player.gameObject.AddComponent(levelUpAbilities[0].type);
					break;
				}
				else if (player.curDevice.Action2.WasPressed) {
					player.gameObject.AddComponent(levelUpAbilities[1].type);
					break;
				}
				else if (player.curDevice.Action3.WasPressed) {
					player.gameObject.AddComponent(levelUpAbilities[2].type);
					break;
				}
				else if (player.curDevice.Action4.WasPressed) {
					player.gameObject.AddComponent(levelUpAbilities[3].type);
					break;
				}
			}
			yield return 0;
		}

		//Lerp out
		timeElapsed = 0;
		while (timeElapsed < menuInOutTime) {
			timeElapsed += Time.deltaTime;
			float t = timeElapsed/menuInOutTime;

			thisRect.offsetMax = thisRect.offsetMin = Vector2.Lerp(endOffset, startOffset, t*t*t);

			yield return 0;
		}
		//thisRect.offsetMax = thisRect.offsetMin = new Vector2(0, Screen.height);
	}
}
