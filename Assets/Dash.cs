using UnityEngine;
using System.Collections;

public class Dash : Ability {
	float dashTime = 0.1f;
	float dashDistance = 4f;
	float dashSpeed;            //Calculated from the other two variables

	float dashAssistedSpacing = 1.5f;		//How far from an enemy the player is placed if it locked on while dashing

	// Use this for initialization
	protected override void Start () {
		base.Start();

		cooldown = 2f;
		activateKey = KeyCode.Q;
		activateButton = (thisPlayer.curDevice != null) ? thisPlayer.curDevice.LeftTrigger : null;

		dashSpeed = dashDistance / dashTime;
	}

	public override void Activate() {
		base.Activate();

		StartCoroutine(DashCoroutine());
	}
	IEnumerator DashCoroutine() {
		thisPlayer.DisableControls(dashTime);

		Transform homingTarget;
		Vector3 homingDir = HomingDirection(out homingTarget);
		Vector3 direction = (homingDir != Vector3.zero) ? homingDir : thisPlayer.transform.forward;
		float thisDashTime = (homingDir != Vector3.zero) ? ((homingTarget.position - thisPlayer.transform.position).magnitude - dashAssistedSpacing)/dashSpeed : dashTime;

		float timeElapsed = 0;
		while (timeElapsed < thisDashTime) {
			timeElapsed += Time.deltaTime;
			//float percent = timeElapsed / dashTime;

			thisPlayer.thisRigidbody.velocity = (direction * dashSpeed);

			yield return 0;
		}
	}

	//Will return a vector facing towards the nearest target to the dash direction, or Vector3.zero if there was no target found close enough
	Vector3 HomingDirection(out Transform homingTarget) {
		Vector3 startPos = thisPlayer.transform.position;
		Vector3 defaultEndPos = thisPlayer.transform.position + thisPlayer.transform.forward * dashDistance;

		float maxDot = 0.9f;
		homingTarget = null;

		//Look at all targets around the player
		foreach (var collider in Physics.OverlapSphere(startPos, dashDistance+1)) {
			//Ignore the player using this ability
			if (collider.gameObject == thisPlayer.gameObject) {
				continue;
			}
			//Only consider objects which can be attacked by the player
			DamageableObject candidate = collider.gameObject.GetComponent<DamageableObject>();
			if (candidate == null) {
				continue;
			}

			Vector3 candidateEndPos = collider.transform.position;

			float dot = Vector3.Dot((defaultEndPos - startPos).normalized, (candidateEndPos - startPos).normalized);
			//print((defaultEndPos - startPos) + "\t" + (candidateEndPos - startPos));
			//print(dot);

			if (dot > maxDot) {
				maxDot = dot;
				homingTarget = collider.transform;
			}
		}
		return (homingTarget != null) ? (homingTarget.position - thisPlayer.transform.position).normalized : Vector3.zero;
	}
}
