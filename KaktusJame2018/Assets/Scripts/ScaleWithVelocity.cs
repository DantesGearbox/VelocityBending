using UnityEngine;

public class ScaleWithVelocity : MonoBehaviour {

	private Rigidbody2D rb;
	private CharacterController2D cc;

	//The speeds to reach before maximum scaling is acheived
	private float maxYSpeed = 0.0f;
	private float maxXSpeed = 0.0f;

	private void Start() {
		rb = GetComponentInParent<Rigidbody2D>();
		cc = GetComponentInParent<CharacterController2D>();
	}

	void FixedUpdate() {

		//Going fast
		float fastScaleX = 0.5f;
		float fastScaleY = 2.0f;

		//Going slow
		float slowScaleX = 1.0f;
		float slowScaleY = 1.0f;

		//How lerped will the scale be
		float ySpeedRatio = Mathf.Abs(rb.velocity.y) / maxYSpeed;
		float xSpeedRatio = Mathf.Abs(rb.velocity.x) / maxXSpeed;
		float xLerp = Mathf.Lerp(slowScaleX, fastScaleX, ySpeedRatio);
		float yLerp = Mathf.Lerp(slowScaleY, fastScaleY, ySpeedRatio);
		//Debug.Log("ySpeedRatio: " + ySpeedRatio + "xSpeedRatio: " + xSpeedRatio + ", xLerp: " + xLerp + ", yLerp: " + yLerp);

		transform.localScale = new Vector2(xLerp, yLerp);
	}

	public void SetMaxYSpeed(float speed) {
		maxYSpeed = speed;
	}

	public void SetMaxXSpeed(float speed) {
		maxXSpeed = speed;
	}
}
