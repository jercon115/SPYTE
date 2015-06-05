using UnityEngine;
using System.Collections;

public class SpeechBubble : MonoBehaviour {

	private float timer;

	// Use this for initialization
	void Start () {
		timer = 90;
		transform.localScale = new Vector3(0.0f,0.0f,0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (new Vector3 (0.0f, 0.005f));

		if (timer > 0) {
			if (timer > 30) {
				transform.localScale += (new Vector3(1.0f,1.0f,1.0f) - transform.localScale)*0.1f;
			} else
				transform.localScale += (new Vector3(0.0f,0.0f,0.0f) - transform.localScale)*0.1f;

			timer--;
		} else
			Destroy (gameObject);
	}
}
