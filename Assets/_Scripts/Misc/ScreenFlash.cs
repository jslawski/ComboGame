using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour {
	public Character thisPlayer;
	Image thisImage;

	// Use this for initialization
	void Start () {
		thisImage = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		Color curCol = thisImage.color;
		curCol.a = 1 - thisPlayer.health / thisPlayer.maxHealth;
		thisImage.color = curCol;
	}
}
