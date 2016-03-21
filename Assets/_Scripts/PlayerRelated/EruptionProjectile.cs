using UnityEngine;
using System.Collections;

public class EruptionProjectile : MonoBehaviour {
	public Character thisPlayer;

	public float curDamage;
	public float maxDamage;
	public float minDamage;
	float knockbackForce = 0.25f;
	float speedOfSizeChange = 0.01f;            //Smaller values == slower size change
	float maxSize = 20f;                        //Limit as t -> infinity of localScale.x & z
	float lifespan = 3f;                        //How long this hitbox remains
	float timeAlive = 0;                        //How long this object has existed

	Material thisMat;
	Color startCol;
	Color endCol;

	// Use this for initialization
	void Start () {
		thisMat = GetComponent<Renderer>().material;
		startCol = thisMat.GetColor("_TintColor");
		endCol = new Color(startCol.r, startCol.g, startCol.b, 0);
		minDamage = maxDamage / 3f;
	}
	
	// Update is called once per frame
	void Update () {
		//Expand the circular region so that it slows down its rate of increase over time
		Vector3 curScale = transform.localScale;
		//Don't ask about the math behind this. It just works
		curScale.x = curScale.z = curScale.x * (1-speedOfSizeChange) + (maxSize*speedOfSizeChange);
		transform.localScale = curScale;

		if ((timeAlive += Time.deltaTime) >= lifespan) {
			Destroy(this.gameObject);
		}

		float percent = timeAlive / lifespan;
		thisMat.SetColor("_TintColor", Color.Lerp(startCol, endCol, percent));
		curDamage = Mathf.Lerp(maxDamage, minDamage, percent);
		//print(curDamage);
	}

	void OnTriggerStay(Collider other) {
		//OnTrigger only works for convex meshes, so this code ensures that the other object
		//is actually colliding with the mesh, and is not sitting within the empty inner circle
		float outerDistance = transform.lossyScale.x / 2f;
		float innerDistance = outerDistance * 0.8f;
		Vector2 outVector = (new Vector2(other.transform.position.x, other.transform.position.z) -
							 new Vector2(transform.position.x, transform.position.z));
		float centerToCenterDistance = outVector.magnitude;

		//If the center point of the other object does not lay within the outer and inner radii, we ignore it
		if (!(centerToCenterDistance < outerDistance && centerToCenterDistance > innerDistance)) {
			return;
		}

		//print(other.name + " " + outerDistance + " " + innerDistance + " " + centerToCenterDistance);

		DamageableObject damageableOther = other.gameObject.GetComponent<DamageableObject>();
		if (damageableOther == null || other.gameObject.tag == "Player") {
			return;
		}

		damageableOther.TakeDamage(curDamage, knockbackForce*(new Vector3(outVector.x, 0, outVector.y)).normalized);
		thisPlayer.health += curDamage / 10f;
	}
}
