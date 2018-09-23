using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

	public Transform player;
	public Rigidbody2D rb;
	private Camera cam;

	private float trauma = 0.0f;
	private float traumaRemoval = 1.0f; //Per second
	private float maxOffset = 0.8f;
	
	private Vector3 notShakenCam;

	private float minFOV = 80;
	private float maxFOV = 120;
	private float maxSpeed = 150.0f;

	private float followBy = 0.15f;

	private void Start() {
		cam = GetComponent<Camera>();
	}

	private void Update() {

		//Zoom-in effect
		cam.fieldOfView = Mathf.Lerp(maxFOV, minFOV, Time.deltaTime);
		if(Mathf.Abs(rb.velocity.y) > 50) {
			float speedFraction = (Mathf.Abs(rb.velocity.y)-50) / maxSpeed;
			float FOV = Mathf.SmoothStep(maxFOV, minFOV, speedFraction);
			cam.fieldOfView = FOV;
			followBy = Mathf.SmoothStep(0.15f, 1.0f, speedFraction);

			trauma = 0.5f;
		}
	}

	public void AddTrauma(float addedTrauma) {

		trauma += Mathf.Clamp01(addedTrauma);
	}

	void FixedUpdate () {
		//Camera shake effect
		trauma = Mathf.Clamp01(trauma);
		float xOffset = maxOffset * Mathf.Pow(trauma, 2) * Random.Range(-1.0f, 1.0f);
		float yOffset = maxOffset * Mathf.Pow(trauma, 2) * Random.Range(-1.0f, 1.0f);
		trauma -= traumaRemoval * Time.deltaTime;

		//Camera rotate effect
		float value = Mathf.Cos(Time.time) * 2;
		transform.localRotation = Quaternion.Euler(0, 0, value);

		//Camera follow effect
		notShakenCam = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z), followBy);

		//Apply camera shake and follow effects
		cam.transform.position = notShakenCam + new Vector3(xOffset, yOffset, 0);
	}
}
