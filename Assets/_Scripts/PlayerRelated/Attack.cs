using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

	Character thisPlayer;                       //Reference to the player component
	public Transform curTarget;						//The transform of the enemy we're currently combo-ing
	public AttackNode rootNode;						//Starting node that every combo starts at
	public AttackNode curNode;                      //Current node in the player's combo that determines the next attack

	float curInputBuffer = 0;				//Player can only queue up the next attack when this is zero
	float inputBuffer = 0.2f;				//Amount of buffer time before the next attack button input gets queued up for an attack

	// Use this for initialization
	void Start () {
		thisPlayer = GetComponent<Character>();

		rootNode = GetComponent<AttackTree>().mainTree;
		curNode = rootNode;
	}
	
	void EnqueueAttack() {
		//Set player to attacking state
		thisPlayer.attacking = true;

		//Reset the input buffer
		curInputBuffer = inputBuffer;

		//If the attack queue is empty, enqueue the attack and execute it immediately
		if (AttackManager.S.attackQueue.Count == 0) {
			AttackManager.S.attackQueue.Enqueue(curNode.attackCoroutine);
			AttackManager.S.ExecuteAttack();
		}
		//Otherwise enqueue the attack to prepare it for future coroutines to execute
		else {
			AttackManager.S.attackQueue.Enqueue(curNode.attackCoroutine);
		}

		//Set the combo timer
		AttackManager.S.curComboTime = AttackManager.S.comboTimeReset;
	}

	// Update is called once per frame
	void Update() {
		//Execute punch if there is a device, the player presses X, the current input buffer is 0, and the current node has a "punch" child
		if (thisPlayer.curDevice != null && thisPlayer.curDevice.Action3.WasPressed && curInputBuffer <= 0 && curNode.punch != null) {
			//Set the next node in the combo chain
			curNode = curNode.punch;

			EnqueueAttack();
		}

		//Execute kick if there is a device, the player presses A, the current input buffer is 0, and the current node has a "kick" child
		else if (thisPlayer.curDevice != null && thisPlayer.curDevice.Action1.WasPressed && curInputBuffer <= 0 && curNode.kick != null) {
			//Set the next node in the combo chain
			curNode = curNode.kick;

			EnqueueAttack();
		}
	}

	void FixedUpdate() {
		if (curInputBuffer > 0) {
			curInputBuffer -= Time.deltaTime;
		}
	}

}
