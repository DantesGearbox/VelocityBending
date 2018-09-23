using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour {

	//Key inputs
	public KeyCode jumpKey;
	public KeyCode leftKey;
	public KeyCode rightKey;
	public KeyCode velocityBendingKey;

	//Unity components
	Rigidbody2D rb;
	BoxCollider2D boxColl;
	ScaleWithVelocity scale;
	AudioManager audioManager;
	public FollowPlayer cam;

	//Physics variables - We set these
	private float maxJumpHeight = 6.0f;                 // If this could be in actual unity units somehow, that would be great
	private float minJumpHeight = 0.5f;                 // If this could be in actual unity units somehow, that would be great
	private float timeToJumpApex = 0.3f;                // This is in actual seconds
	private float maxMovespeed = 15;					// If this could be in actual unity units per second somehow, that would be great
	private float accelerationTime = 0.1f;              // This is in actual seconds
	private float deccelerationTime = 0.1f;             // This is in actual seconds
	private float velocityBending = 1.1f;				// The multiplier for vertical momentum

	//Physics variables - These get set for us
	//private float maxFallingSpeed;
	private float gravity;
	private float maxJumpVelocity;
	private float minJumpVelocity;
	private float acceleration;
	private float decceleration;

	//Sprite settings variables
	private float inputDirection = 1.0f;
	private bool gettingInput = false;

	//Collision variables
	public bool onGround = false;

	//Coyote time variables
	public float savedYSpeed = 0.0f;
	public bool coyoteTimeActive = false;
	private float coyoteLength = 0.05f;
	private float coyoteTimer = 0.0f;

	//Camera shake variables
	public float previousFrameYSpeed = 0.0f;

	//Other variables
	private bool playerIsPaused = false;

	// Use this for initialization
	void Start() {
		rb = GetComponent<Rigidbody2D>();
		boxColl = GetComponent<BoxCollider2D>();
		scale = GetComponentInChildren<ScaleWithVelocity>();
		audioManager = FindObjectOfType<AudioManager>();
		SetupMoveAndJumpSpeed();
	}

	private void CheckGroundCollision(){
		//Get the 10 first contacts of the box collider
		ContactPoint2D[] contacts = new ContactPoint2D[10];
		int count = boxColl.GetContacts(contacts);

		//If we find any horizontal surfaces, we are on the ground
		onGround = false;
		for (int i = 0; i < count; i++) {

			//If the angle between the normal and up is less than 5, we are on the ground
			if (Vector2.Angle(contacts[i].normal, Vector2.up) < 5.0f) {

				//Sound effects
				audioManager.Play("LandingSound");

				//Debug.Log("PrevFrameSpeed: " + previousFrameYSpeed);

				//Camera shake
				if (Mathf.Abs(previousFrameYSpeed) > 50) {
					float speedFraction = (Mathf.Abs(previousFrameYSpeed) - 50) / 150;
					float trauma = Mathf.Lerp(0, 1, speedFraction);
					cam.AddTrauma(trauma);

					//Debug.Log("Trauma: " + trauma + ", Speed fraction: " + speedFraction);
				}

				//Coyote time setup
				coyoteTimeActive = true;

				//Ground collision physics
				onGround = true;
				rb.velocity = new Vector2(rb.velocity.x, 0);
			}
		}
	}

	//Whenever we collide, check if we are touching the ground
	private void OnCollisionEnter2D(Collision2D collision) {
		CheckGroundCollision();
	}

	private void OnCollisionExit2D(Collision2D collision) {
		CheckGroundCollision();
	}

	public void PauseThePlayer(){
		playerIsPaused = true;
		rb.Sleep();
	}

	public void UnpauseThePlayer() {
		playerIsPaused = false;
		rb.WakeUp();
	}

	// Update is called once per frame
	void Update() {

		if(!playerIsPaused){
			Jumping();
			HorizontalMovement();
			VelocityBending();
			Gravity();
		}

		if (Input.GetKeyDown(KeyCode.S)) {
			cam.AddTrauma(0.5f);
		}
	}

	public float GetInputDirection() {
		return inputDirection;
	}

	public bool GetGettingInput(){
		return gettingInput;
	}

	public float GetMinJumpVelocity(){
		return minJumpVelocity;
	}

	public float GetMaxJumpVelocity() {
		return maxJumpVelocity;
	}

	void Jumping() {
		//Setting the initial jump velocity

		if (Input.GetKeyDown(jumpKey)) {
			if (onGround) {

				//Sound effects
				audioManager.PlayWithRandomPitch("JumpSound");

				rb.velocity = new Vector2(rb.velocity.x, 0);
				rb.velocity += new Vector2(0, maxJumpVelocity);
			}
		} 

		if(Input.GetKeyUp(jumpKey)){
			if (rb.velocity.y > minJumpVelocity) {
				rb.velocity = new Vector2(rb.velocity.x, minJumpVelocity);
			}
		}
	}

	void VelocityBending(){

		//Remember the maximum yspeed for coyote time
		if (Mathf.Abs(rb.velocity.y) > savedYSpeed) {
			savedYSpeed = Mathf.Abs(rb.velocity.y);
		}

		//House keeping the coyote time timer
		if (coyoteTimeActive) {
			coyoteTimer += Time.deltaTime;
			if(coyoteTimer > coyoteLength) {
				coyoteTimeActive = false;
				coyoteTimer = 0.0f;
				savedYSpeed = 0.0f;
			}
		}

		if(Input.GetKeyDown(velocityBendingKey)){

			//Debug.Log("Velocity magnitude: " + rb.velocity.magnitude);

			//Sound effects with pitch dependent on speed
			if(Mathf.Abs(savedYSpeed) > 1.0f) {

				float minPitch = 1.0f;
				float maxPitch = 1.5f;
				float maxSpeed = 150.0f;
				float speedFraction = savedYSpeed / maxSpeed;
				float pitch = Mathf.Lerp(minPitch, maxPitch, speedFraction);

				audioManager.PlayWithPitch("LittleExplosion", pitch);
				audioManager.PlayWithPitch("VelocityBending", pitch);
			}

			//Convert all downwards momentum to upwards momentum
			rb.velocity = new Vector2(rb.velocity.x, Mathf.Abs(rb.velocity.y) * velocityBending);

			//If we are in coyote time, override the current momentum with the previous
			if (coyoteTimeActive) {
				rb.velocity = new Vector2(rb.velocity.x, Mathf.Abs(savedYSpeed) * velocityBending);
			}
		}
	}

	void Gravity(){

		//Gravity
		if(!onGround){
			rb.velocity -= new Vector2(0, gravity * Time.deltaTime);
		}

		//House keeping previous Y speed
		previousFrameYSpeed = rb.velocity.y;

		//Reset savedYSpeed at the apex of a jump
		if (rb.velocity.y < (gravity*Time.deltaTime) && rb.velocity.y > -(gravity * Time.deltaTime)) {
			previousFrameYSpeed = 0;
		}
	}


	void HorizontalMovement() {

		gettingInput = false;

		if (Input.GetKey(leftKey)) {

			float temp = rb.velocity.x;
			float change = acceleration * Time.deltaTime;
			temp = Mathf.Clamp(temp - change, -maxMovespeed, 0);
			rb.velocity = new Vector2(temp, rb.velocity.y);

		} else {
			if (rb.velocity.x < 0) {

				float temp = rb.velocity.x;
				float change = decceleration * Time.deltaTime;
				temp = Mathf.Clamp(temp + change, -maxMovespeed, 0);
				rb.velocity = new Vector2(temp, rb.velocity.y);

			}
		}
		
		if (Input.GetKey(rightKey)) {

			float temp = rb.velocity.x;
			float change = acceleration * Time.deltaTime;
			temp = Mathf.Clamp(temp + change, 0, maxMovespeed);
			rb.velocity = new Vector2(temp, rb.velocity.y);

		} else {
			if(rb.velocity.x > 0) {

				float temp = rb.velocity.x;
				float change = decceleration * Time.deltaTime;
				temp = Mathf.Clamp(temp - change, 0, maxMovespeed);
				rb.velocity = new Vector2(temp, rb.velocity.y);

			}
		}
	}

	void SetupMoveAndJumpSpeed() {
		//Scale gravity and jump velocity to jumpHeights and timeToJumpApex
		gravity = (2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = gravity * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * gravity * minJumpHeight);

		//Scale acceleration values to the movespeed and wanted acceleration times
		acceleration = maxMovespeed / accelerationTime;
		decceleration = maxMovespeed / deccelerationTime;

		//Set variables for the velocity scaling
		scale.SetMaxYSpeed(100);
		scale.SetMaxXSpeed(maxMovespeed);
	}
}
