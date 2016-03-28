using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackManager : MonoBehaviour {

	public static AttackManager S;

	public GameObject[] fists;						//Reference to player's fists
	public GameObject[] legs;						//Reference to player's legs
	public float curComboTime = 0;					//Time left on current combo
	public float comboTimeReset = 0.5f;				//Time allotted to perform next attack in combo
	Character thisPlayer;							//Reference to player
	Transform playerTransform;                      //Reference to player transform
	Attack playerAttack;                            //Reference to the Attack script on the player
	Rigidbody thisRigidbody;

	public Queue<string> attackQueue;       //Queue of the attacks that are to be executed next

	float punchTime = 0.3f;                 //Duration of a punch
	float kickTime = 0.5f;
	float attackDistance = 0.5f;            //Distance the player jumps when executing an attack


	/*********All Possible Combos**********
	Each possible combo has a corresponding coroutine that executes when it is called
	Punch Punch Punch Punch	
	Punch Punch Punch Punch Punch
	Punch Punch Punch Punch Kick

	Kick Kick Kick
	Kick Kick Kick Kick

	Any other attack combination resets the combo queue to a basic kick or punch
	***************************************/

	void Awake() {
		S = this;
	}

	void Start() {
		thisPlayer = GetComponent<Character>();
		playerTransform = GetComponent<Transform>();
		playerAttack = GetComponent<Attack>();
		attackQueue = new Queue<string>();
		thisRigidbody = thisPlayer.gameObject.GetComponent<Rigidbody>();
	}

	//Execute the next attack in the queue
	public void ExecuteAttack() {
		//Execute the correct coroutine and remove it from the queued up attacks
		StartCoroutine(attackQueue.Peek());
		thisRigidbody.velocity = new Vector3(0, 0, 0);
	}

	//Remove the most recently executed attack from the queue, and start executing the next one
	void ExecuteNextAttack() {
		//Dequeue the attack once it has been completed
		attackQueue.Dequeue();
		//Execute the next attack in the queue, if one exists
		if (attackQueue.Count != 0) {
			ExecuteAttack();
		}
	}

	//Reset the combo to start with a punch
	void ResetToPunch() {
		//Clear the attack queue
		attackQueue.Clear();

		//Set the most recent node to an initial punch
		playerAttack.curNode = playerAttack.rootNode.punch;

		//Add the punch to the queue and execute it
		attackQueue.Enqueue(playerAttack.curNode.attackCoroutine);
		ExecuteAttack();
	}

	//Reset the combo to start with a kick
	void ResetToKick() {
		//Clear the attack queue
		attackQueue.Clear();

		//Set the most recent node to an initial punch
		playerAttack.curNode = playerAttack.rootNode.kick;

		//Add the punch to the queue and execute it
		attackQueue.Enqueue(playerAttack.curNode.attackCoroutine);
		ExecuteAttack();
	}

	void FixedUpdate() {
		if (S.curComboTime > 0) {
			S.curComboTime -= Time.fixedDeltaTime;
		}
		else {
			//JPS:  STOP USING THESE FLAGS!!!
			thisPlayer.attacking = false;
			
			//Reset the combo chain
			playerAttack.curNode = playerAttack.rootNode;
			playerAttack.curTarget = null;
		}
	}

	/**********Attack Coroutines**********/
	IEnumerator Punch() {
		fists[0].SetActive(true);
		curComboTime = comboTimeReset;

		//Move the player forward if we aren't comboing on anything
		Vector3 targetTranslate = playerTransform.forward * attackDistance;
		//Else move the player towards the comboed target
		if (playerAttack.curTarget != null) {
			targetTranslate = GetTargetVector();
		}

		playerTransform.Translate(targetTranslate, Space.World);
		yield return new WaitForSeconds(punchTime);
		fists[0].SetActive(false);

		ExecuteNextAttack();
	}

	IEnumerator PunchPunch() {
		fists[1].SetActive(true);
		curComboTime = comboTimeReset;
		
		//Move the player forward if we aren't comboing on anything
		Vector3 targetTranslate = playerTransform.forward * attackDistance;
		//Else move the player towards the comboed target
		if (playerAttack.curTarget != null) {
			targetTranslate = GetTargetVector();
		}

		playerTransform.Translate(targetTranslate, Space.World);
		yield return new WaitForSeconds(punchTime);
		fists[1].SetActive(false);

		ExecuteNextAttack();
	}

	IEnumerator PunchPunchPunch() {
		StartCoroutine("Punch");
		yield return null;
	}

	//FLURRY PUNCH
	IEnumerator PunchPunchPunchPunch() {
		curComboTime = comboTimeReset;
		float lungeDistance = 0.5f;                                     //Distance the player lunges in front of them
		float lungeTime = 0.05f;

		int punchesThrown = 4;                                          //Number of punches thrown during the flurry
		WaitForSeconds flurryPunchTime = new WaitForSeconds(0.1f);      //Time between punches during the flurry

		//Move the player forward if we aren't comboing on anything
		Vector3 targetTranslate = playerTransform.forward * lungeDistance;
		//Else move the player towards the comboed target
		if (playerAttack.curTarget != null) {
			targetTranslate = GetTargetVector();
		}

		//Character lunges forward...
		Vector3 startPos = playerTransform.position;
		Vector3 endPos = playerTransform.position + targetTranslate;
		float lungeTimeElapsed = 0;
		while (lungeTimeElapsed < lungeTime) {
			lungeTimeElapsed += Time.deltaTime;

			//Check for Vector3.zero to avoid weird look rotation == zero vector bug
			if (targetTranslate != Vector3.zero) {
				thisPlayer.targetForward = playerTransform.forward = targetTranslate.normalized;
			}
			playerTransform.Translate(targetTranslate*Time.deltaTime / lungeTime, Space.World);

			yield return null;
		}

		curComboTime = comboTimeReset;

		//...and throws a flurry of punches
		thisPlayer.lockSpinning = true;
		fists[0].SetActive(true);
		yield return flurryPunchTime;
		for (int i = 0; i < punchesThrown; i++) {
			curComboTime = comboTimeReset;
			fists[0].SetActive(!fists[0].activeSelf);
			fists[1].SetActive(!fists[1].activeSelf);
			yield return flurryPunchTime;
		}
		thisPlayer.lockSpinning = false;

		fists[0].SetActive(false);
		fists[1].SetActive(false);

		ExecuteNextAttack();
	}

	//SPINNING KICK
	IEnumerator PunchPunchPunchPunchKick() {
		StartCoroutine("KickKickKickKick");
		yield return null;
	}

	//MASSIVE PUNCH
	IEnumerator PunchPunchPunchPunchPunch() {
		int backupFrames = 15;                                          //Number of frames the player spends backing up

		float backupSpeed = 0.05f;                                      //Speed the player steps back to prepare for attack
		float lungeTime = 0.05f;
		float lungeDistance = 1f;

		WaitForSeconds massivePunchCooldown = new WaitForSeconds(0.5f);     //Time to wait until the attack "finishes"

		//Character backs up...
		for (int i = 0; i < backupFrames; i++) {
			curComboTime = comboTimeReset;
			playerTransform.Translate(-playerTransform.forward * backupSpeed, Space.World);
			yield return null;
		}

		//Move the player forward if we aren't comboing on anything
		Vector3 targetTranslate = playerTransform.forward * lungeDistance;
		//Else move the player towards the comboed target
		if (playerAttack.curTarget != null) {
			targetTranslate = GetTargetVector();
		}

		//Character lunges forward...
		Vector3 startPos = playerTransform.position;
		Vector3 endPos = playerTransform.position + targetTranslate;
		float lungeTImeElapsed = 0;
		while (lungeTImeElapsed < lungeTime) {
			lungeTImeElapsed += Time.deltaTime;

			//Check for Vector3.zero to avoid weird look rotation == zero vector bug
			if (targetTranslate != Vector3.zero) {
				thisPlayer.targetForward = playerTransform.forward = targetTranslate.normalized;
			}
			playerTransform.Translate(targetTranslate * Time.deltaTime / lungeTime, Space.World);

			yield return null;
		}

		//And throws a massive punch with both fists
		thisPlayer.lockSpinning = true;
		fists[2].SetActive(true);
		fists[3].SetActive(true);
		curComboTime = comboTimeReset;

		yield return massivePunchCooldown;

		fists[2].SetActive(false);
		fists[3].SetActive(false);
		thisPlayer.lockSpinning = false;

		ExecuteNextAttack();
	}

	IEnumerator PunchKick() {
		ResetToKick();

		yield return null;
	}

	IEnumerator PunchPunchKick() {
		ResetToKick();

		yield return null;
	}

	IEnumerator PunchPunchPunchKick() {
		ResetToKick();

		yield return null;
	}

	IEnumerator Kick() {
		legs[0].SetActive(true);
		curComboTime = comboTimeReset;

		//Move the player forward if we aren't comboing on anything
		Vector3 targetTranslate = playerTransform.forward * attackDistance;
		//Else move the player towards the comboed target
		if (playerAttack.curTarget != null) {
			targetTranslate = GetTargetVector();
		}

		playerTransform.Translate(targetTranslate, Space.World);
		yield return new WaitForSeconds(kickTime);
		legs[0].SetActive(false);

		ExecuteNextAttack();
	}

	IEnumerator KickKick() {
		legs[1].SetActive(true);
		curComboTime = comboTimeReset;

		//Move the player forward if we aren't comboing on anything
		Vector3 targetTranslate = playerTransform.forward * attackDistance;
		//Else move the player towards the comboed target
		if (playerAttack.curTarget != null) {
			targetTranslate = GetTargetVector();
		}

		playerTransform.Translate(targetTranslate, Space.World);
		yield return new WaitForSeconds(kickTime);
		legs[1].SetActive(false);

		ExecuteNextAttack();
	}

	//STORM KICK
	IEnumerator KickKickKick() {
		float lungeTime = 0.05f;                //Number of frames the player spends lunges forward

		float lungeDistance = 0.5f;            //Distance the player lunges in front of them

		WaitForSeconds stormKickTime = new WaitForSeconds(0.75f);           //How long a spin kick lasts

		curComboTime = comboTimeReset;

		//Move the player forward if we aren't comboing on anything
		Vector3 targetTranslate = playerTransform.forward * lungeDistance;
		//Else move the player towards the comboed target
		if (playerAttack.curTarget != null) {
			targetTranslate = GetTargetVector();
		}

		//Character lunges forward...
		Vector3 startPos = playerTransform.position;
		Vector3 endPos = playerTransform.position + targetTranslate;
		float lungeTImeElapsed = 0;
		while (lungeTImeElapsed < lungeTime) {
			lungeTImeElapsed += Time.deltaTime;

			//Check for Vector3.zero to avoid weird look rotation == zero vector bug
			if (targetTranslate != Vector3.zero) {
				thisPlayer.targetForward = playerTransform.forward = targetTranslate.normalized;
			}
			playerTransform.Translate(targetTranslate * Time.deltaTime / lungeTime, Space.World);

			yield return null;
		}

		//...and executes a storm kick
		thisPlayer.lockSpinning = true;
		legs[3].SetActive(true);
		curComboTime = 1f;
		yield return stormKickTime;
		legs[3].SetActive(false);
		thisPlayer.lockSpinning = false;

		ExecuteNextAttack();
	}

	//SPIN KICK
	IEnumerator KickKickKickKick() {
		int backupFrames = 15;              //Number of frames the player spends backing up
		float lungeTime = 0.1f;				//Number of frames the player spends lunges forward

		float backupSpeed = 0.05f;			//Speed the player steps back to prepare for attack
		float lungeDistance = 1f;            //Distance the player lunges in front of them

		WaitForSeconds spinKickTime = new WaitForSeconds(1f);           //How long a spin kick lasts

		curComboTime = comboTimeReset;

		//Character backs up...
		for (int i = 0; i < backupFrames; i++) {
			curComboTime = comboTimeReset;
			playerTransform.Translate(-playerTransform.forward * backupSpeed, Space.World);
			yield return null;
		}

		//Move the player forward if we aren't comboing on anything
		Vector3 targetTranslate = playerTransform.forward * lungeDistance;
		//Else move the player towards the comboed target
		if (playerAttack.curTarget != null) {
			targetTranslate = GetTargetVector();
		}

		//Character lunges forward...
		Vector3 startPos = playerTransform.position;
		Vector3 endPos = playerTransform.position + targetTranslate;
		float lungeTImeElapsed = 0;
		while (lungeTImeElapsed < lungeTime) {
			lungeTImeElapsed += Time.deltaTime;

			//Check for Vector3.zero to avoid weird look rotation == zero vector bug
			if (targetTranslate != Vector3.zero) {
				thisPlayer.targetForward = playerTransform.forward = targetTranslate.normalized;
			}
			playerTransform.Translate(targetTranslate * Time.deltaTime / lungeTime, Space.World);

			yield return null;
		}

		//...and executes a spin kick
		thisPlayer.lockSpinning = true;
		legs[2].SetActive(true);
		curComboTime = 1f;
		yield return spinKickTime;
		legs[2].SetActive(false);
		thisPlayer.lockSpinning = false;

		ExecuteNextAttack();
	}

	IEnumerator KickKickKickKickKick() {
		ResetToKick();

		yield return null;
	}

	IEnumerator KickPunch() {
		ResetToPunch();

		yield return null;
	}

	IEnumerator KickKickPunch() {
		ResetToPunch();

		yield return null;
	}

	IEnumerator KickKickKickPunch() {
		ResetToPunch();

		yield return null;
	}

	IEnumerator KickKickKickKickPunch() {
		ResetToPunch();

		yield return null;
	}

	Vector3 GetTargetVector() {
		Vector3 targetDir = playerAttack.curTarget.position - playerTransform.position;
		//Ignore differences in height
		targetDir.y = 0;

		playerTransform.forward = targetDir.normalized;
		thisPlayer.targetForward = playerTransform.forward;
		return targetDir - playerTransform.forward * ((playerTransform.lossyScale.x + playerAttack.curTarget.lossyScale.x) / 2f);
	}
}
