using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public void loadScene(string scene) {
		Application.LoadLevel (scene);
	}
}
