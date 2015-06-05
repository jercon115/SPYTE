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

	// SETUP PHASE //
	public CanvasGroup setupCanvGroup;
	public CanvasGroup playCanvGroup;
	public Toggle p1RdyToggle;
	public Toggle p2RdyToggle;

	private bool setupPhase;
	private bool playerOneReady;
	private bool playerTwoReady;

	// Use this for initialization
	void Awake () {
		Application.targetFrameRate = 30;

		gamestate = GameState.WaitForReady;
		winner = Player.None;

		countdownTimer = 5;
		timerText.enabled = false;

		situationTimer = Random.Range (5.0f, 10.0f);

		setupPhase = true; playerOneReady = false; playerTwoReady = false;
	}

	private void updateSituation() {
		if (situationTimer <= 0) {
			situationTimer = 0;

			if (charMgr.currentSituation == Situation.None) {
				switch(Random.Range (0,2)) {
				case 0:
					situationText.text = "Dance";
					charMgr.changeSituation(Situation.Dance);
					break;
				case 1:
					situationText.text = "Fireworks";
					charMgr.changeSituation(Situation.Fireworks);
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
		if(setupPhase) {
			switch (gamestate) {
			case GameState.RevealOne:
				charMgr.revealPlayer (Player.One);
				setupCanvGroup.alpha = 0;
				playerOneReady = false; playerTwoReady = false;
				p1RdyToggle.isOn = false; p2RdyToggle.isOn = false;
				if (!Input.GetKey (KeyCode.Q))  gamestate = GameState.WaitForReady;
				break;
			case GameState.RevealTwo:
				charMgr.revealPlayer (Player.Two);
				setupCanvGroup.alpha = 0;
				playerOneReady = false; playerTwoReady = false;
				p1RdyToggle.isOn = false; p2RdyToggle.isOn = false;
				if (!Input.GetKey (KeyCode.U)) gamestate = GameState.WaitForReady;
				break;
			case GameState.WaitForReady:
				charMgr.revealPlayer (Player.None);
				setupCanvGroup.alpha = 1;
				if (Input.GetKeyDown (KeyCode.Q)) gamestate = GameState.RevealOne;
				if (Input.GetKeyDown (KeyCode.U)) gamestate = GameState.RevealTwo;

				if (Input.GetKeyDown (KeyCode.E)) {
					if (!playerOneReady) { playerOneReady = true; p1RdyToggle.isOn = true;
					} else if (playerOneReady) { playerOneReady = false; p1RdyToggle.isOn = false;}
				}
				if (Input.GetKeyDown (KeyCode.O)) {
					if (!playerTwoReady) { playerTwoReady = true; p2RdyToggle.isOn = true;
					} else if (playerTwoReady) { playerTwoReady = false; p2RdyToggle.isOn = false; }
				}

				if (playerOneReady && playerTwoReady) gamestate = GameState.Countdown;
				break;
			case GameState.Countdown:
				setupPhase = false;
				setupCanvGroup.alpha = 0; playCanvGroup.alpha = 1;
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
				setupPhase = true;
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

			if (Input.anyKeyDown)
				gamestate = GameState.Restart;
		}
	}

	public void setWinner(Player newWinner) {
		winner = newWinner;
	}

	public void SetScene (string scene) {
		Application.LoadLevel (scene);
	}

	public void SetScene (int scene) {
		Application.LoadLevel (scene);
	}
}
