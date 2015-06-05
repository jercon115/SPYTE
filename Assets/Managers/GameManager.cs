using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Player {None, One, Two};
public enum Situation {None, Dance, Fireworks};

public class GameManager : MonoBehaviour {

	enum GameState { WaitForReady, RevealOne, RevealTwo, Countdown, Play, End, Restart};
	
	public CharacterManager charMgr;

	public Camera cam;
	public Text timerText;
	public Text situationText;
		
	private float countdownTimer;
	private float situationTimer;

	private GameState gamestate;
	private Player winner;
	private bool setup;
	private bool playerOneReady;
	private bool playerTwoReady;

	// Use this for initialization
	void Awake () {
		Application.targetFrameRate = 30;

		gamestate = GameState.WaitForReady;
		winner = Player.None;

		countdownTimer = 150;
		timerText.enabled = false;

		situationTimer = Random.Range (5.0f, 10.0f);

		setup = true; playerOneReady = false; playerTwoReady = false;
	}

	private void updateSituation() {
		if (situationTimer <= 0) {
			situationTimer = 0;

			if (charMgr.currentSituation == Situation.None) {
				switch(Random.Range (0,1)) {
				case 0:
					situationText.text = "Dance";
					charMgr.changeSituation(Situation.Dance);
					break;
				}
				situationTimer = Random.Range (5.0f, 15.0f);
			} else {
				situationText.text = "None";
				charMgr.changeSituation(Situation.None);

				situationTimer = Random.Range (10.0f, 20.0f);
			}
		} else
			situationTimer -= Time.deltaTime;
	}

	// Update is called once per frame
	void Update () {
		if(setup) {
			switch (gamestate) {
			case GameState.RevealOne:
				charMgr.revealPlayer (Player.One);
				playerOneReady = false; playerTwoReady = false;
				if (!Input.GetKey (KeyCode.Q))  gamestate = GameState.WaitForReady;
				break;
			case GameState.RevealTwo:
				charMgr.revealPlayer (Player.Two);
				playerOneReady = false; playerTwoReady = false;
				if (!Input.GetKey (KeyCode.U)) gamestate = GameState.WaitForReady;
				break;
			case GameState.WaitForReady:
				charMgr.revealPlayer (Player.None);
				if (Input.GetKeyDown (KeyCode.Q)) gamestate = GameState.RevealOne;
				if (Input.GetKeyDown (KeyCode.U)) gamestate = GameState.RevealTwo;

				if (Input.GetKeyDown (KeyCode.E)) playerOneReady = true;
				if (Input.GetKeyDown (KeyCode.O)) playerTwoReady = true;

				if (playerOneReady && playerTwoReady) gamestate = GameState.Countdown;
				break;
			case GameState.Countdown:
				setup = false;
				charMgr.revealPlayer (Player.None);
				countdownTimer = 5;
				timerText.enabled = true;
				break;
			case GameState.Restart:
				if (winner == Player.One) {
					Application.LoadLevel ("Player1Wins");
				} else
					Application.LoadLevel ("Player2Wins");
				break;
			}
		}

		if (gamestate == GameState.Countdown) {
			timerText.text = Mathf.CeilToInt (countdownTimer).ToString ();
			countdownTimer -= Time.deltaTime;

			if (countdownTimer <= 0) {
				timerText.enabled = false;
				charMgr.setAllPaused (0);
				gamestate = GameState.Play;
			}
		} else if (gamestate == GameState.Play) {
			updateSituation ();

			if (winner != Player.None) {
				gamestate = GameState.End;
				setup = true;
			}
		} else if (gamestate == GameState.End) {
			Vector3 xyDiff;
			if (winner == Player.One) {
				xyDiff = (charMgr.One.transform.localPosition - cam.transform.localPosition);
			} else
				xyDiff = (charMgr.Two.transform.localPosition - cam.transform.localPosition);
			xyDiff.Scale (new Vector3(1.0f,1.0f,0.0f));
			cam.transform.Translate (xyDiff*0.03f);

			cam.orthographicSize += (4.0f - cam.orthographicSize)*0.03f;
		}
	}

	public void setWinner(Player newWinner) {
		winner = newWinner;
	}

	public void SetSceneGame () {
		Application.LoadLevel (0);
	}
	
	public void SetScenePlayer1Wins () {
		Application.LoadLevel ("Player1Wins");
	}
	
	public void SetScenePlayer2Wins () {
		Application.LoadLevel ("Player2Wins");
	}
}
