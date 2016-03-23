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
			S.curComboTime -= Time.deltaTime;
		}
		else {
			thisPlayer.attacking = false;
			//Reset the combo chain
			playerAttack.curNode = playerAttack.rootNode;
		}
	}

	/**********Attack Coroutines**********/
	IEnumerator Punch() {
		fists[0].SetActive(true);
		curComboTime = comboTimeReset;
		playerTransform.Translate(playerTransform.forward * attackDistance, Space.World);
		yield return new WaitForSeconds(punchTime);
		fists[0].SetActive(false);

		ExecuteNextAttack();
	}

	IEnumerator PunchPunch() {
		fists[1].SetActive(true);
		curComboTime = comboTimeReset;
		playerTransform.Translate(playerTransform.forward * attackDistance, Space.World);
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
		int lungeFrames = 3;											//Number of frames the player spends lunges forward

		float lungeSpeed = 0.5f;                                        //Distance the player lunges in front of them
		int punchesThrown = 4;                                          //Number of punches thrown during the flurry
		WaitForSeconds flurryPunchTime = new WaitForSeconds(0.1f);      //Time between punches during the flurry

		//Character lunges forward...
		for (int i = 0; i < lungeFrames; i++) {
			curComboTime = comboTimeReset;
			playerTransform.Translate(playerTransform.forward * lungeSpeed, Space.World);
			yield return null;
		}

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
		int lungeFrames = 3;                                            //Number of frames the player spends lunges forward

		float backupSpeed = 0.05f;                                      //Speed the player steps back to prepare for attack
		float lungeSpeed = 0.5f;                                        //Distance the player lunges in front of them

		WaitForSeconds massivePunchCooldown = new WaitForSeconds(0.5f);     //Time to wait until the attack "finishes"

		//Character backs up...
		for (int i = 0; i < backupFrames; i++) {
			curComboTime = comboTimeReset;
			playerTransform.Translate(-playerTransform.forward * backupSpeed, Space.World);
			yield return null;
		}

		//...Then lunges forward...
		for (int i = 0; i < lungeFrames; i++) {
			curComboTime = comboTimeReset;
			playerTransform.Translate(playerTransform.forward * lungeSpeed, Space.World);
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
		playerTransform.Translate(playerTransform.forward * attackDistance, Space.World);
		yield return new WaitForSeconds(kickTime);
		legs[0].SetActive(false);

		ExecuteNextAttack();
	}

	IEnumerator KickKick() {
		legs[1].SetActive(true);
		curComboTime = comboTimeReset;
		playerTransform.Translate(playerTransform.forward * attackDistance, Space.World);
		yield return new WaitForSeconds(kickTime);
		legs[1].SetActive(false);

		ExecuteNextAttack();
	}

	//STORM KICK
	IEnumerator KickKickKick() {
		int lungeFrames = 2;                //Number of frames the player spends lunges forward

		float lungeSpeed = 0.5f;            //Distance the player lunges in front of them

		WaitForSeconds stormKickTime = new WaitForSeconds(0.75f);           //How long a spin kick lasts

		curComboTime = comboTimeReset;

		//Player lunges forward
		for (int i = 0; i < lungeFrames; i++) {
			curComboTime = comboTimeReset;
			playerTransform.Translate(playerTransform.forward * lungeSpeed, Space.World);
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
		int lungeFrames = 5;				//Number of frames the player spends lunges forward

		float backupSpeed = 0.05f;			//Speed the player steps back to prepare for attack
		float lungeSpeed = 0.5f;            //Distance the player lunges in front of them

		WaitForSeconds spinKickTime = new WaitForSeconds(1f);           //How long a spin kick lasts

		curComboTime = comboTimeReset;

		//Character backs up...
		for (int i = 0; i < backupFrames; i++) {
			curComboTime = comboTimeReset;
			playerTransform.Translate(-playerTransform.forward * backupSpeed, Space.World);
			yield return null;
		}

		//...Then lunges forward...
		for (int i = 0; i < lungeFrames; i++) {
			curComboTime = comboTimeReset;
			playerTransform.Translate(playerTransform.forward * lungeSpeed, Space.World);
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
}
