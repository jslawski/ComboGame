using UnityEngine;
using System.Collections;
using System;

public class PhaseShift : ActivatedAbility {
	float duration = 3f;			//How long the player is allowed to pass through obstacles
	
	protected override void Awake() {
		base.Awake();

		cooldown = 12f;
		activateKey = KeyCode.Alpha2;
		activateButton = (thisPlayer.curDevice != null) ? thisPlayer.curDevice.Action2 : null;
	}

	protected override void SetAbilityInfo() {
		info.abilityName = "Phase Shift";
		info.abilityDescription = "Shift dimensions to temporarily pass through obstacles.";
		info.abilityImage = Resources.Load<Sprite>("Images/AbilityImages/PhaseShift.png");
		info.type = typeof(PhaseShift);
	}

	public override void Activate() {
		base.Activate();

		StartCoroutine(PhaseShiftCoroutine());
	}
	IEnumerator PhaseShiftCoroutine() {
		thisPlayer.gameObject.layer = LayerMask.NameToLayer("Ghost");
		yield return new WaitForSeconds(duration);
		thisPlayer.gameObject.layer = LayerMask.NameToLayer("Default");
	}
}
