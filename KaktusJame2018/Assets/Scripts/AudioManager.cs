using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour {

	public Sound[] sounds;

	public static AudioManager instance;

	private bool themeLooping = false;

	// Use this for initialization
	void Awake () {

		if(instance == null) {
			instance = this;
		} else {
			Destroy(this.gameObject);
			return;
		}

		DontDestroyOnLoad(this.gameObject);

		foreach(Sound s in sounds) {
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;

			s.source.volume = s.volume;
			s.source.pitch = s.pitch;

			s.source.loop = s.loop;
		}
	}

	private void Start() {
		Play("ThemeIntro");
	}

	private void Update() {
		Sound s = Array.Find(sounds, sound => sound.name == "ThemeIntro");
		if (!s.source.isPlaying && !themeLooping) {
			themeLooping = true;
			Play("ThemeLoop");
		}
	}

	//Other classes must call this to play sounds
	public void Play(string name) {
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if(s == null) {
			Debug.Log("Tried to play sound with name that doesn't exist");
			return;
		}
		s.source.Play();
	}

	public void PlayWithRandomPitch(string name) {
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (s == null) {
			Debug.Log("Tried to play sound with name that doesn't exist");
			return;
		}

		//Pitch randomization
		s.source.pitch = UnityEngine.Random.Range(0.9f, 1.3f);

		s.source.Play();
	}

	public void PlayWithPitch(string name, float pitch) {
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (s == null) {
			Debug.Log("Tried to play sound with name that doesn't exist");
			return;
		}

		//Pitch randomization
		s.source.pitch = pitch;

		s.source.Play();
	}
}
