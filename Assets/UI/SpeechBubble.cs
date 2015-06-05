using UnityEngine;
using System.Collections;

public class SpeechBubble : MonoBehaviour {

	private float timer;

	// Use this for initialization
	void Start () {
		timer = 2.5f;
		transform.localScale = new Vector3(0.0f,0.0f,0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (new Vector3 (0.0f, 0.2f*Time.deltaTime));

		if (timer > 0) {
			if (timer > 1.25f) {
				transform.localScale += (new Vector3(1.0f,1.0f,1.0f) - transform.localScale)*0.1f;
			} else
				transform.localScale += (new Vector3(0.0f,0.0f,0.0f) - transform.localScale)*0.1f;

			timer -= Time.deltaTime;
		} else
			Destroy (gameObject);
	}
}
