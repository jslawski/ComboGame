using UnityEngine;
using System.Collections;

//Node of every attack possible, given it's position in the attack tree
public class AttackNode {
	public AttackNode punch;
	public AttackNode kick;
	public string attackCoroutine;
};

public class AttackTree : MonoBehaviour {

	public AttackNode mainTree = new AttackNode();

	//Build the combo tree recursively
	AttackNode BuildTree(ref AttackNode parentNode, string attack, int depth) {
		//Set the name of the coroutine to call for the attack
		parentNode.attackCoroutine = attack;

		//If the max depth of the tree has been reached, return out
		if (depth >= 5) return parentNode;

		//Determine the left node of the current node (punches)
		AttackNode leftNode = new AttackNode();
		parentNode.punch = BuildTree(ref leftNode, attack + "Punch", depth + 1);

		//Determine the right node of the current node (kicks)
		AttackNode rightNode = new AttackNode();
		parentNode.kick = BuildTree(ref rightNode, attack + "Kick", depth + 1);

		//Return the parent node
		return parentNode;
	}

	void Awake() {
		mainTree = BuildTree(ref mainTree, "", 0);
	}
}
