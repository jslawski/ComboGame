using UnityEngine;
using System.Collections;
using InControl;

public abstract class ActivatedAbility : MonoBehaviour {
	public AbilityInfo info;
	public Character thisPlayer;			//The player who has this ability
	public KeyCode activateKey;             //Keyboard key used to activate the ability
	public InputControl activateButton;		//Controller button used to activate the ability

	public float manaCost;					//How much mana it costs to use this ability

	public float cooldown;					//Time it takes for a player to be able to use this ability again
	public float cooldownRemaining = 0;		//Time remaining on the cooldown

	// Use this for initialization
	protected virtual void Awake () {
		thisPlayer = gameObject.GetComponent<Character>();

		info = new AbilityInfo();
		SetAbilityInfo();
	}

	protected abstract void SetAbilityInfo();
	
	// Update is called once per frame
	protected virtual void Update () {
		if ((Input.GetKeyDown(activateKey) || (activateButton != null && activateButton.IsPressed)) && cooldownRemaining <= 0) {
			Activate();
		}

		if (cooldownRemaining > 0) {
			cooldownRemaining -= Time.deltaTime;
		}
	}

	public virtual void Activate() {
		print(this.GetType().ToString() + " activated");
		cooldownRemaining = cooldown;
	}
}
