using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Player {None, One, Two};
public enum Situation {None, Dance, Fireworks};

public class GameManager : MonoBehaviour {

	enum GameState { WaitForOne, RevealOne, WaitForTwo, RevealTwo, Countdown, Play, End, Restart};
	
	public CharacterManager charMgr;

	public Camera cam;
	public Text timerText;
	public Text situationText;
		
	private int countdownTimer;
	private int situationTimer;

	private GameState gamestate;
	private Player winner;
	private bool pressToContinue;

	// Use this for initialization
	void Awake () {
		Application.targetFrameRate = 30;

		gamestate = GameState.WaitForOne;
		winner = Player.None;

		countdownTimer = 150;
		timerText.enabled = false;

		situationTimer = Random.Range (150, 300);

		pressToContinue = true;
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
				situationTimer = Random.Range (150, 450);
			} else {
				situationText.text = "None";
				charMgr.changeSituation(Situation.None);

				situationTimer = Random.Range (150, 300);
			}
		} else
			situationTimer--;
	}

	// Update is called once per frame
	void Update () {
		if(Input.anyKeyDown && pressToContinue) {
			gamestate++;
			switch (gamestate) {
			case GameState.RevealOne:
				charMgr.revealPlayer (Player.One);
				break;
			case GameState.WaitForTwo:
				charMgr.revealPlayer (Player.None);
				break;
			case GameState.RevealTwo:
				charMgr.revealPlayer (Player.Two);
				break;
			case GameState.Countdown:
				pressToContinue = false;
				charMgr.revealPlayer (Player.None);
				countdownTimer = 150;
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
			timerText.text = Mathf.CeilToInt (countdownTimer/30.0f).ToString ();
			countdownTimer--;

			if (countdownTimer <= 0) {
				timerText.enabled = false;
				charMgr.setAllPaused (0);
				gamestate = GameState.Play;
			}
		} else if (gamestate == GameState.Play) {
			updateSituation ();

			if (winner != Player.None) {
				gamestate = GameState.End;
				pressToContinue = true;
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
