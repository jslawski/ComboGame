using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public static GameManager S;

	public GameObject experiencePrefab;
	public Character player;
	private Character playerInstance;

	public Enemy bouncer;
	private Enemy enemyInstance;

	public Maze mazePrefab;
	Maze mazeInstance;

	float experienceDropDelay = 0.1f;       //Time in seconds between each experience dropped by an enemy
	public static int enemiesAggroed = 0;
	public static readonly int maxAggroEnemies = 4;

	bool inBulletTimeCoroutine = false;

	// Use this for initialization
	void Awake () {
		S = this;

		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
		StartCoroutine(GenerateMaze());
	}
	
	// Update is called once per frame
	void Update () {
		//DEBUG BULLET TIME
		if (Input.GetKeyDown(",")) {
			StartBulletTime(0.5f);
		}
		if (Input.GetKeyDown(".")) {
			EndBulletTime();
		}
	}

	public void StartBulletTime(float timescale, float realTimeDuration = float.MaxValue) {
		if (!inBulletTimeCoroutine) {
			StartCoroutine(BulletTimeCoroutine(timescale, realTimeDuration));
		}
	}
	public void EndBulletTime() {
		if (inBulletTimeCoroutine) {
			StopCoroutine("BulletTimeCoroutine");
			inBulletTimeCoroutine = false;
			Time.timeScale = 1;
			Time.fixedDeltaTime = 0.02f;
		}
	}
	IEnumerator BulletTimeCoroutine(float timescale, float realTimeDuration) {
		inBulletTimeCoroutine = true;

		//We need to keep track of real time passed while modifying timescales
		float realTimeAtStart = Time.realtimeSinceStartup;
		Time.timeScale *= timescale;
		Time.fixedDeltaTime *= timescale;

		//print("Time.timeScale: " + Time.timeScale);
		float timeElapsed = Time.realtimeSinceStartup - realTimeAtStart;
		while (timeElapsed < realTimeDuration) {
			timeElapsed = Time.realtimeSinceStartup - realTimeAtStart;
			yield return null;
		}

		//Restore normal timescale
		Time.timeScale /= timescale;
		Time.fixedDeltaTime /= timescale;
		//print("Time.timeScale: " + Time.timeScale);

		inBulletTimeCoroutine = false;
	}

	IEnumerator GenerateMaze() {
		if (mazePrefab == null) {
			yield break;
		}
		mazeInstance = Instantiate(mazePrefab) as Maze;
		yield return StartCoroutine(mazeInstance.Generate());
		//Instantiate the player and enable camera follow
		playerInstance = Instantiate(player) as Character;
		Vector3 cellPosition = mazeInstance.GetCell(mazeInstance.RandomCoordinates).transform.localPosition;
		playerInstance.transform.localPosition = new Vector3(cellPosition.x, cellPosition.y + playerInstance.gameObject.GetComponent<Collider>().bounds.extents.y + 0.1f, cellPosition.z);
		GetComponent<CameraFollow>().followObject = playerInstance.transform;
		GetComponent<CameraFollow>().enabled = true;

		int numEnemies = Random.Range(5, 15);
		for (int i = 0; i < numEnemies; i++) {
			GenerateEnemies();
		}
	}

	void GenerateEnemies() {
		enemyInstance = Instantiate(bouncer) as Enemy;
		Vector3 cellPosition = mazeInstance.GetCell(mazeInstance.RandomCoordinates).transform.localPosition;
		enemyInstance.transform.localPosition = new Vector3(cellPosition.x, cellPosition.y + enemyInstance.gameObject.GetComponent<Collider>().bounds.extents.y + 0.1f, cellPosition.z);
	}

	public void DropExperienceAtLocation(Vector3 location, int amount) {
		StartCoroutine(DropExperienceCoroutine(location, amount));
	}
	IEnumerator DropExperienceCoroutine(Vector3 location, int amount) {
		for (int i = 0; i < amount; i++) {
			GameObject newExperienceObj = Instantiate(experiencePrefab, location, new Quaternion()) as GameObject;
			Rigidbody newExperienceRigidbody = newExperienceObj.GetComponent<Rigidbody>();

			Vector2 randDirectionCircle = Random.insideUnitCircle;
			Vector3 randDirection = new Vector3(randDirectionCircle.x, 0, randDirectionCircle.y);

			newExperienceRigidbody.AddForce(randDirection, ForceMode.Impulse);

			yield return new WaitForSeconds(experienceDropDelay);
		}
	}
}
