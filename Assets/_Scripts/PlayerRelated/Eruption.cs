using UnityEngine;
using System.Collections;
using System;

public class Eruption : ActivatedAbility {
	GameObject eruptionProjectilePrefab;
	float damageDonePerTick = 4f;

	// Use this for initialization
	protected override void Awake() {
		base.Awake();

		cooldown = 8f;
		activateKey = KeyCode.Alpha4;
		activateButton = (thisPlayer.curDevice != null) ? thisPlayer.curDevice.Action4 : null;
		eruptionProjectilePrefab = Resources.Load<GameObject>("Prefabs/EruptionProjectile");
	}

	protected override void SetAbilityInfo() {
		info.abilityName = "Eruption";
		info.abilityDescription = "Releases an expanding wave of fire that damages enemies it passes through.";
		info.abilityImage = Resources.Load<Sprite>("Images/AbilityImages/Eruption");
		info.type = typeof(Eruption);
	}

	public override void Activate() {
		base.Activate();

		GameObject newObj = Instantiate(eruptionProjectilePrefab, thisPlayer.transform.position, new Quaternion()) as GameObject;
		EruptionProjectile newEruption = newObj.GetComponent<EruptionProjectile>();
		newEruption.maxDamage = damageDonePerTick;
		newEruption.thisPlayer = thisPlayer;
	}
}
