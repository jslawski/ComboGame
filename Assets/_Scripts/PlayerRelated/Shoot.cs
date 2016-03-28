using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Shoot : MonoBehaviour {
	Character thisPlayer;											//Reference to the player component

	int clipSize = 7;                                               //Number of bullets the player can shoot before reloading
	int bulletsLeft;                                                //Number of bullets left in the current clip
	WaitForSeconds shotDelay = new WaitForSeconds(0.2f);            //Delay between bullets if the player is holding the button down
	float reloadTime = 1.5f;                                        //Time it takes to reload a clip after it's emptied
	Bullet bulletPrefab;                                            //General Bullet prefab
	Bullet curBullet;                                               //Reference to the most recent bullet fired
	public Text ammoText;                                           //Text showing how many shots are left JPS: I would like to represent this with images ala Splattershot
	public Slider reloadSlider;                                     //Visual representation of how much time is left for reload
	float shootDistance = 5f;                                       //Distance to check in front of the player for a potential homing target
	bool isLockedOn = false;										//Whether or not the player is currently locked on to an enemy

	// Use this for initialization
	void Start () {
		thisPlayer = GetComponent<Character>();

		bulletPrefab = (Bullet)Resources.Load("Prefabs/Bullet", typeof(Bullet));
		bulletsLeft = clipSize;

		ammoText.text = clipSize.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	//The player is present, pressing Y, not currently shooting, not currently attacking, and has bullets to shoot
	if (thisPlayer.curDevice != null && thisPlayer.curDevice.RightStick && !thisPlayer.attacking && !thisPlayer.shooting && bulletsLeft > 0) {
			thisPlayer.shooting = true;
			thisPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;

			StartCoroutine(FireBullet());
		}
	}

	IEnumerator FireBullet() {
		//Continue looping as long as the player has bullets to shoot and is still holdin the shoot button
		//The player must also not be mid-attack
		while (thisPlayer.curDevice.RightStick && bulletsLeft > 0) {
			Vector2 stickVector = thisPlayer.curDevice.RightStick.Vector;
			Vector3 fireDirection = new Vector3(stickVector.x, 0, stickVector.y);

			curBullet = Instantiate(bulletPrefab, thisPlayer.transform.position + fireDirection, new Quaternion()) as Bullet;

			Transform homingTarget;
			Vector3 homingDir = HomingDirection(out homingTarget, fireDirection);
			thisPlayer.transform.forward = (homingDir != Vector3.zero) ? homingDir : thisPlayer.transform.forward;

			Vector3 bulletDirection = (homingDir != Vector3.zero) ? homingDir : fireDirection;

			curBullet.GetComponent<Rigidbody>().velocity = bulletDirection * curBullet.bulletSpeed;
			curBullet.thisPlayer = thisPlayer;

			bulletsLeft--;

			ammoText.text = bulletsLeft.ToString();

			yield return shotDelay;
		}

		if (bulletsLeft <= 0) {
			StartCoroutine(Reload());
		}

		thisPlayer.shooting = false;
	}

	IEnumerator Reload() {
		ammoText.enabled = false;
		reloadSlider.enabled = true;

		float increment = reloadTime / 50f;
		for (float i = 0; i < reloadTime; i += increment) {
			yield return new WaitForFixedUpdate();
			reloadSlider.value = i / reloadTime;
		}

		reloadSlider.value = 0;
		bulletsLeft = clipSize;

		ammoText.enabled = true;
		ammoText.text = bulletsLeft.ToString();
		reloadSlider.enabled = false;
	}

	//Will return a vector facing towards the nearest target to the dash direction, or Vector3.zero if there was no target found close enough
	Vector3 HomingDirection(out Transform homingTarget, Vector3 fireDirection) {
		Vector3 startPos = thisPlayer.transform.position;
		Vector3 defaultEndPos = thisPlayer.transform.position + fireDirection * shootDistance;

		float maxDot = 0.7f;
		homingTarget = null;

		//Look at all targets around the player
		foreach (var collider in Physics.OverlapSphere(startPos, shootDistance + 1)) {
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
