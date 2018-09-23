using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadow : MonoBehaviour {

	public Color shadowColor;

	private Vector2 offset = new Vector2(-2.0f, 0.5f);

	private SpriteRenderer shadowCaster;
	private SpriteRenderer shadow;

	private Transform shadowTransform;
	private Transform casterTransform;

	private Bounds casterBounds;
	private float extentsX;
	private float extentsY;

	private Transform playerTransform;

	// Use this for initialization
	void Start () {

		playerTransform = FindObjectOfType<CharacterController2D>().transform;

		casterTransform = transform;
		shadowTransform = new GameObject().transform;
		shadowTransform.parent = casterTransform;
		shadowTransform.gameObject.name = "DropShadow";
		shadowTransform.localRotation = Quaternion.identity;
		shadowTransform.localScale = Vector3.one;

		casterBounds = casterTransform.GetComponent<BoxCollider2D>().bounds;
		extentsX = casterBounds.extents.x;
		extentsY = casterBounds.extents.y;
		//Debug.Log("Extents X: " + extentsX + "Extents Y: " + extentsY);

		shadowCaster = GetComponent<SpriteRenderer>();
		shadow = shadowTransform.gameObject.AddComponent<SpriteRenderer>();
		
		shadow.sortingOrder = shadowCaster.sortingOrder - 1;
		shadow.color = shadowColor;

		shadowTransform.position = new Vector2(casterTransform.position.x + offset.x, casterTransform.position.y + offset.y);
		shadow.sprite = shadowCaster.sprite;
	}

	// After update method
	void FixedUpdate () {

		//Vector2 diff = (casterTransform.position - playerTransform.position);
		//diff = diff.normalized * 2;

		//float maxDist = 2.0f;
		//float minDist = -2.0f;

		//float maxCharDistX = casterBounds.extents.x;
		//float maxCharDistY = casterBounds.extents.y;

		//float speedFractionX = (diff.x - 0) / maxCharDistX;
		//float distX = Mathf.Lerp(minDist, maxDist, speedFractionX);

		//float speedFractionY = (diff.y - 0) / maxCharDistY;
		//float distY = Mathf.Lerp(minDist, maxDist, speedFractionY);

		//float val = 0.0f;
		//val = Mathf.PingPong(Time.time, Mathf.PI/2);



		//float xOffset = (offset.x + Mathf.Cos(Time.time * 0.5f))/2;
		//float yOffset = offset.y;



		//shadowTransform.position = new Vector2(casterTransform.position.x + offset.x, casterTransform.position.y + offset.y);
		//shadowTransform.position = new Vector2(casterTransform.position.x + xOffset, casterTransform.position.y + yOffset);
		//shadowTransform.position = new Vector2(casterTransform.position.x + distX, casterTransform.position.y + distY);
		//shadowTransform.position = new Vector2(casterTransform.position.x + diff.x, casterTransform.position.y + diff.y);
		//shadow.sprite = shadowCaster.sprite;
	}
}
