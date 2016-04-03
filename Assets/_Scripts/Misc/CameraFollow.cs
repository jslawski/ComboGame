using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public static CameraFollow S;
	Camera thisCamera;

	public Transform followObject;		//Object for the camera to follow
	Vector3 startOffset;                //Starting position of the camera relative to the object it is following
	Vector3 curOffset;                  //Used for camera shake, otherwise == startOffset

	float startSize;					//Initial orthographic size of the camera
	float curSize;                      //Used for camera zoom, otherwise == startSize
	float zoomSpeed = 0.1f;				//Percent per frame the camera's orthographic size changes to its target size

	float followSpeed = 0.1f;           //Percent per frame the camera moves towards its target position

	bool inCameraShakeCoroutine = false;
	float cameraShakeFrequency = 40f;   //How many times per second the random offset for camera shake changes

	bool inCameraZoomCoroutine = false;

	// Use this for initialization
	void Start () {
		S = this;
		startOffset = transform.position;
		curOffset = startOffset;

		followObject = GameManager.S.player.gameObject.transform;
		thisCamera = GetComponent<Camera>();
		startSize = thisCamera.orthographicSize;
		curSize = startSize;
	}

	//DEBUG INPUT FOR CAMERA SHAKE
	void Update() {
		if (Input.GetKey("x")) {
			CameraShake(0.2f, 2);
		}
		if (Input.GetKey("z")) {
			CameraZoom(0.4f, 0.5f);
		}
	}
	
	//Camera movement in FixedUpdate() for smoother following of the physics calculations
	void FixedUpdate () {
		transform.position = Vector3.Lerp(transform.position, followObject.position + curOffset, followSpeed);
		thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, curSize, zoomSpeed);
	}

	public void CameraShake(float duration, float intensity) {
		if (!inCameraShakeCoroutine) {
			StartCoroutine(CameraShakeCoroutine(duration, intensity));
		}
	}
	IEnumerator CameraShakeCoroutine(float duration, float intensity) {
		inCameraShakeCoroutine = true;

		float timeElapsed = 0;
		while (timeElapsed < duration) {
			Vector2 tempVector2 = Random.insideUnitCircle;
			curOffset = startOffset + intensity*(new Vector3(tempVector2.x, 0, tempVector2.y));

			timeElapsed += 1 / cameraShakeFrequency;
			yield return new WaitForSeconds(1 / cameraShakeFrequency);
		}

		curOffset = startOffset;
		inCameraShakeCoroutine = false;
	}

	public void CameraZoom(float duration, float percent) {
		if (!inCameraZoomCoroutine) {
			StartCoroutine(CameraZoomCoroutine(duration, percent));
		}
	}
	IEnumerator CameraZoomCoroutine(float duration, float percent) {
		inCameraZoomCoroutine = true;

		//Move the camera offset towards the ground
		curSize = startSize * percent;
		yield return new WaitForSeconds(duration);
		curSize = startSize;

		inCameraZoomCoroutine = false;
	}
}
