using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetSceneGame () {
		Application.LoadLevel (0);
		Debug.Log("Button was pressed");
	}

	public void SetScenePlayer1Wins () {
		Application.LoadLevel ("Player1Wins");
	}

	public void SetScenePlayer2Wins () {
		Application.LoadLevel ("Player2Wins");
	}
}
