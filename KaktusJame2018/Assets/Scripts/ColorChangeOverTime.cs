using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChangeOverTime : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private Image image;
	private Camera cam;
	private Color startColor = new Color (253f/255f, 237f/255f, 171f/255f, 255f/255f);
	private Color endColor = new Color (234f/255f, 237f/255f, 171f/255f, 255f/255f);

	public Color editorColor1 = new Color (0, 0, 0, 1);
	public Color editorColor2 = new Color (0, 0, 0, 1);
	public bool useEditorColors = false;

	public float switchFrequency = 1.0f;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		if(spriteRenderer == null){
			image = GetComponent<Image> ();

			if(image == null) {
				cam = GetComponent<Camera>();
			}
		}

		if(useEditorColors){
			startColor = editorColor1;
			endColor = editorColor2;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(spriteRenderer != null){
			spriteRenderer.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * (1/switchFrequency), 1));	
		} else if(image != null){
			image.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * (1/switchFrequency), 1));	
		} else {
			cam.backgroundColor = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * (1 / switchFrequency), 1));
		}
	}
}
