using UnityEngine;
using System.Collections;

public class Bomber : Enemy {

	// Use this for initialization
	protected override void Start () {
		base.Start();

		maxHealth = 50f;
		health = maxHealth;
	}
}
