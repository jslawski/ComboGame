using UnityEngine;
using System.Collections;

//Utilizes the NavMeshAgent component on the enemy to navigate towards the player's position
[RequireComponent(typeof(NavMeshAgent))]
public class BatAI : Enemy {
	NavMeshAgent agent;
	public Transform aggroTarget;

	float hoverDistance = 5f;           //Distance the enemy will try to stay away from the enemy

	bool inAttackCoroutine = false;
	float timeUntilNextAttack = 0f;
	float minTimeBetweenAttacks = 3f;
	float maxTimeBetweenAttacks = 6f;
	float distanceToPlayerThreshold = 1.5f;
	float attackSpeed = 75f;
	float normalSpeed;

	// Use this for initialization
	protected override void Start() {
		base.Start();

		experienceOnDeath = 20;
		agent = GetComponent<NavMeshAgent>();
		aggroTarget = GameManager.S.player.transform;
		agent.destination = aggroTarget.position;
		normalSpeed = agent.speed;
		timeUntilNextAttack = Random.Range(minTimeBetweenAttacks, maxTimeBetweenAttacks);
	}

	// Update is called once per frame
	protected override void Update() {
		base.Update();

		agent.enabled = !stunned;
		if (!stunned) {
			agent.destination = FindTargetPosition();
		}

		if (timeUntilNextAttack > 0) {
			timeUntilNextAttack -= Time.deltaTime;
		}
		else if (IsCloseEnoughToAttack() && !inAttackCoroutine && GameManager.enemiesAggroed < GameManager.maxAggroEnemies) {
			StartCoroutine(PerformAttack());
		}
	}

	Vector3 FindTargetPosition() {
		Vector3 targetDir = (transform.position - aggroTarget.position).normalized;
		Vector3 targetPos = aggroTarget.position + targetDir*hoverDistance;
		if (inAttackCoroutine) {
			targetPos = aggroTarget.position + targetDir * 0.5f;
		}
		return targetPos;
	}

	IEnumerator PerformAttack() {
		inAttackCoroutine = true;
		GameManager.enemiesAggroed++;

		agent.speed = attackSpeed;

		float distanceToPlayer = (transform.position - aggroTarget.position).magnitude;
		while (distanceToPlayer > distanceToPlayerThreshold) {
			distanceToPlayer = (transform.position - aggroTarget.position).magnitude;
			yield return 0;
		}
		yield return new WaitForSeconds(0.5f);

		agent.speed = normalSpeed;
		timeUntilNextAttack = Random.Range(minTimeBetweenAttacks, maxTimeBetweenAttacks);

		inAttackCoroutine = false;

		yield return new WaitForSeconds(1f);
		GameManager.enemiesAggroed--;
	}

	bool IsCloseEnoughToAttack() {
		float distance = (transform.position - aggroTarget.position).magnitude;
		return Mathf.Abs(distance - hoverDistance) < distanceToPlayerThreshold;
	}
}
