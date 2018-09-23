using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayer : MonoBehaviour {

	public Transform player;
	public Transform playerStartingPosition;
	AudioManager audioManager;

	private void Start() {
		audioManager = FindObjectOfType<AudioManager>();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "Player") {

			//Sound effects
			audioManager.Play("PlayerDeath");

			player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			player.position = playerStartingPosition.position;
		}
	}
}
