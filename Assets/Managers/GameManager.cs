using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Player {None, One, Two};

public class GameManager : MonoBehaviour {

	enum GameState { WaitForOne, RevealOne, WaitForTwo, RevealTwo, Countdown, Play, End, Restart};
	
	public CharacterManager charMgr;

	public Camera cam;
	public Text timerText;
		
	private int timer;

	private GameState gamestate;
	private Player winner;
	private bool pressToContinue;

	// Use this for initialization
	void Start () {
		gamestate = GameState.WaitForOne;
		winner = Player.None;

		timer = 150;
		timerText.enabled = false;

		pressToContinue = true;
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
				timer = 150;
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
			timerText.text = Mathf.CeilToInt (timer/30.0f).ToString ();
			timer--;

			if (timer <= 0) {
				timerText.enabled = false;
				charMgr.setAllPaused (0);
				gamestate = GameState.Play;
			}
		} else if (gamestate == GameState.Play) {
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
