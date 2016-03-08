using UnityEngine;
using System.Collections;

public class Dash : Ability {
	float dashTime = 0.1f;
	float dashDistance = 4f;

	// Use this for initialization
	protected override void Start () {
		base.Start();

		cooldown = 2f;
		activateKey = KeyCode.Q;
		activateButton = thisPlayer.curDevice.LeftTrigger;
	}

	public override void Activate() {
		base.Activate();

		StartCoroutine(DashCoroutine());
	}
	IEnumerator DashCoroutine() {
		float timeElapsed = 0;
		while (timeElapsed < dashTime) {
			timeElapsed += Time.deltaTime;
			float percent = timeElapsed / dashTime;

			thisPlayer.thisRigidbody.AddForce(thisPlayer.transform.forward * (dashDistance / dashTime), ForceMode.VelocityChange);

			yield return 0;
		}
	}
}
