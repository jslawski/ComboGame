using UnityEngine;
using System.Collections;

public class Ability : MonoBehaviour {
	public Character thisPlayer;			//The player who has this ability
	public KeyCode activateKey;             //Button used to activate the ability

	public float manaCost;					//How much mana it costs to use this ability

	public float cooldown;					//Time it takes for a player to be able to use this ability again
	public float cooldownRemaining = 0;		//Time remaining on the cooldown

	// Use this for initialization
	protected virtual void Start () {
		print("Ability.Start()");
		thisPlayer = gameObject.GetComponent<Character>();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (Input.GetKeyDown(activateKey) && cooldownRemaining <= 0) {
			Activate();
		}

		if (cooldownRemaining > 0) {
			cooldownRemaining -= Time.deltaTime;
		}
	}

	public virtual void Activate() {
		print("Ability activated");
		cooldownRemaining = cooldown;
	}
}
