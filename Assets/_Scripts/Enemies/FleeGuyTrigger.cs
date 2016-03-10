using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class FleeGuyTrigger : MonoBehaviour {

	public NavMeshAgent agent;
	PolarCoordinate potentialTarget;
	NavMeshPath potentialPath;
	float lookRadius = 5f;

	void Start() {
		agent = GetComponentInParent<NavMeshAgent>();
		agent.speed = 5f;
		potentialPath = new NavMeshPath();
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag == "Player") {
			//The first potential runaway target will be in the exact opposite direction of the player
			Vector3 direction = (transform.position - other.transform.position).normalized;
			potentialTarget = new PolarCoordinate(lookRadius, direction);
			agent.CalculatePath(transform.position + potentialTarget.PolarToCartesian(), potentialPath);
            if (potentialPath.status != NavMeshPathStatus.PathComplete) {
				FindValidTarget();
			}

			agent.SetPath(potentialPath);
		}
	}

	/*void OnTriggerStay(Collider other) {
		if (other.gameObject.tag == "Player") {
			Vector3 direction = (transform.position - other.transform.position).normalized;
			PolarCoordinate polarDirection = new PolarCoordinate(lookRadius, direction);
			Debug.DrawRay(transform.position, polarDirection.PolarToCartesian().normalized * 5, Color.red);

		}
	}*/

	//Scan around the enemy in a circle, looking for a valid target to start moving towards
	void FindValidTarget() {
		float startingAngle = potentialTarget.angle;
		float panningEdgeRight = startingAngle + (5.0f * Mathf.PI) / 6.0f;		//150 degrees from starting angle rotating right
		float panningEdgeLeft = startingAngle - (5.0f * Mathf.PI) / 6.0f;       //150 degrees from starting angle rotating left
		float increment = Mathf.PI / 4.0f;				                        //Increment 45 degrees for each check

		int directionRoll = Random.Range(0, 2);     //Roll of 0 or 1, to determine which direction the enemy start looking to flee

		if (directionRoll == 0) {
			//Pan right 150 degrees from starting position to search for a valid position
			for (float i = startingAngle; i < panningEdgeRight; i += increment) {
				potentialTarget = new PolarCoordinate(lookRadius, i);
				//Debug.DrawRay(transform.position, potentialTarget.PolarToCartesian(), Color.blue, 10f);
				agent.CalculatePath(transform.position + potentialTarget.PolarToCartesian(), potentialPath);
                if (potentialPath.status == NavMeshPathStatus.PathComplete) {
					return;
				}
			}
		}
		else {
			//Pan left 150 degrees froms starting position to search for a valid position
			for (float i = startingAngle; i > panningEdgeLeft; i -= increment) {
				potentialTarget = new PolarCoordinate(lookRadius, i);
				//Debug.DrawRay(transform.position, potentialTarget.PolarToCartesian(), Color.blue, 10f);
				agent.CalculatePath(transform.position + potentialTarget.PolarToCartesian(), potentialPath);
                if (potentialPath.status == NavMeshPathStatus.PathComplete) {
					return;
				}
			}
		}
	}
}
