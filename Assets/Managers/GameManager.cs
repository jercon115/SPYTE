using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Player {None, One, Two};
public enum Situation {None, Dance, Fireworks};

public class GameManager : MonoBehaviour {

	enum GameState { WaitForReady, RevealOne, RevealTwo, Countdown, Play, End};
	
	public CharacterManager charMgr;

	public Camera cam;
	public Text timerText;
	public Text gameTimerText;
		
	private float countdownTimer;
	private float situationTimer;
	private float gameTimer;

	private GameState gamestate;
	private Player winner;

	// SETUP PHASE //
	public CanvasGroup setupCanvGroup;
	public CanvasGroup positionPrompt;
	public CanvasGroup playCanvGroup;
	public CanvasGroup endCanvGroup;
	public CanvasGroup missPrompt;
	public Toggle p1RdyToggle;
	public Toggle p2RdyToggle;

	public GameObject room;
	public GameObject windows;

	public Image[] redStrikes;
	public Image[] blueStrikes;

	private SpriteRenderer roomSprite;
	private SpriteRenderer windowsSprite;
	private float[] roomRGB; private float[] roomRGBTarg;
	private float[] windowsRGB;
	private float fireworksTimer;
	private bool setupPhase;
	private bool playerOneReady;
	private bool playerTwoReady;

	// Use this for initialization
	void Awake () {
		if (Application.loadedLevelName != "Scene")
			return;
		roomSprite = room.GetComponent<SpriteRenderer> ();
		roomRGB = new float[3] {1.0f, 1.0f, 1.0f};
		roomRGBTarg = new float[3] {1.0f, 1.0f, 1.0f};
		windowsSprite = windows.GetComponent<SpriteRenderer> ();
		windowsRGB = new float[3] {0.0f, 0.0f, 0.0f};
		windowsSprite.color = new Color (0, 0, 0);

		for (int i = 0; i < 3; i++) {
			redStrikes[i].transform.localScale = new Vector3(0.0f,0.0f);
			blueStrikes[i].transform.localScale = new Vector3(0.0f,0.0f);
		}

		Application.targetFrameRate = 30;

		gamestate = GameState.WaitForReady;
		winner = Player.None;

		countdownTimer = 5;
		fireworksTimer = 0.0f;
		gameTimer = 90;
		timerText.enabled = false;
		gameTimerText.enabled = false;

		situationTimer = Random.Range (10.0f, 20.0f);

		setupPhase = true; playerOneReady = false; playerTwoReady = false;


	}

	private void updateSituation() {
		if (roomRGB[0] > roomRGBTarg[0]) roomRGB[0] -= 0.05f; if (roomRGB[1] > roomRGBTarg[1]) roomRGB[1] -= 0.05f; if (roomRGB[2] > roomRGBTarg[2]) roomRGB[2] -= 0.05f;
		if (roomRGB[0] < roomRGBTarg[0]) roomRGB[0] += 0.05f; if (roomRGB[1] < roomRGBTarg[1]) roomRGB[1] += 0.05f; if (roomRGB[2] < roomRGBTarg[2]) roomRGB[2] += 0.05f;
		roomSprite.color = new Color (roomRGB[0], roomRGB[1], roomRGB[2], 1.0f);
		if (windowsRGB[0] > 0.0f) windowsRGB[0] -= 0.01f; if (windowsRGB[1] > 0.0f) windowsRGB[1] -= 0.01f; if (windowsRGB[2] > 0.0f) windowsRGB[2] -= 0.01f;
		windowsSprite.color = new Color (windowsRGB[0], windowsRGB[1], windowsRGB[2], 1.0f);

		if (charMgr.currentSituation == Situation.Dance) {
			if (charMgr.danceTimer > 0.5f) {
				roomRGBTarg[0] = 1.0f; roomRGBTarg[1] = 0.5f; roomRGBTarg[2] = 1.0f;
			} else
				roomRGBTarg[0] = 0.5f; roomRGBTarg[1] = 0.5f; roomRGBTarg[2] = 1.0f;
		} else {
			roomRGBTarg[0] = 1.0f; roomRGBTarg[1] = 1.0f; roomRGBTarg[2] = 1.0f;
		}

		if (charMgr.currentSituation == Situation.Fireworks) {
			if (fireworksTimer <= 0) {
				fireworksTimer = Random.Range (0.2f, 1.2f);
				switch (Random.Range (0, 3)) {
				case 0:
					windowsRGB [0] = 1.0f; windowsRGB [1] = 0.0f; windowsRGB [2] = 0.0f;
					break;
				case 1:
					windowsRGB [0] = 0.0f; windowsRGB [1] = 1.0f; windowsRGB [2] = 0.0f;
					break;
				case 2:
					windowsRGB [0] = 0.0f; windowsRGB [1] = 0.0f; windowsRGB [2] = 1.0f;
					break;
				}
			} else
				fireworksTimer -= Time.deltaTime;
		} else {
			fireworksTimer = 0.0f;
		}
		
		if (situationTimer <= 0) {
			situationTimer = 0;

			if (charMgr.currentSituation == Situation.None) {
				switch(Random.Range (0,2)) {
				case 0:
					charMgr.changeSituation(Situation.Dance);
					break;
				case 1:
					charMgr.changeSituation(Situation.Fireworks);
					break;
				}
				situationTimer = Random.Range (5.0f, 15.0f);
			} else {
				charMgr.changeSituation(Situation.None);

				situationTimer = Random.Range (10.0f, 20.0f);
			}
		} else
			situationTimer -= Time.deltaTime;
	}

	private void updateStrikes() {
		for (int i = 0; i < 3; i++) {
			Vector3 redScale = new Vector3(0.0f,0.0f);
			if (i < charMgr.One.killAttempts)
				redScale = new Vector3(1.0f,1.0f);

			Vector3 blueScale = new Vector3(0.0f,0.0f);
			if (i < charMgr.Two.killAttempts)
				blueScale = new Vector3(1.0f,1.0f);

			redStrikes[i].transform.localScale += (redScale - redStrikes[i].transform.localScale)*0.1f;
			blueStrikes[i].transform.localScale += (blueScale - blueStrikes[i].transform.localScale)*0.1f;
		}
	}

	// Update is called once per frame
	void Update () {
		if (Application.loadedLevelName != "Scene")
			return;

		if(setupPhase) {
			switch (gamestate) {
			case GameState.RevealOne:
				charMgr.revealPlayer (Player.One);
				setupCanvGroup.alpha = 0;
				playerOneReady = false; playerTwoReady = false;
				p1RdyToggle.isOn = false; p2RdyToggle.isOn = false;
				positionPrompt.alpha = 1;
				if (!Input.GetKey (KeyCode.Q))  {
					gamestate = GameState.WaitForReady;
					positionPrompt.alpha = 0;
				}
				break;
			case GameState.RevealTwo:
				charMgr.revealPlayer (Player.Two);
				setupCanvGroup.alpha = 0;
				playerOneReady = false; playerTwoReady = false;
				p1RdyToggle.isOn = false; p2RdyToggle.isOn = false;
				positionPrompt.alpha = 1;
				if (!Input.GetKey (KeyCode.U)) {
					gamestate = GameState.WaitForReady;
					positionPrompt.alpha = 0;
				}
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
			}
		}

		updateStrikes ();

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

			// Game timer
			if (!gameTimerText.enabled) gameTimerText.enabled = true;
			gameTimerText.text = Mathf.CeilToInt (gameTimer).ToString ();
			gameTimer -= Time.deltaTime;

			if (winner != Player.None || gameTimer <= 0) {
				gamestate = GameState.End;
				if (winner == Player.None) charMgr.setAllPaused (2);
			}
		} else if (gamestate == GameState.End) {
			playCanvGroup.alpha = 0; endCanvGroup.alpha = 1;

			if (winner != Player.None) {
				Vector3 xyDiff;
				if (winner == Player.One) {
					xyDiff = (charMgr.Two.transform.localPosition - cam.transform.localPosition);
				} else
					xyDiff = (charMgr.One.transform.localPosition - cam.transform.localPosition);
				xyDiff.Scale (new Vector3(1.0f,1.0f,0.0f));
				cam.transform.Translate (xyDiff*0.03f);

				cam.orthographicSize += (4.0f - cam.orthographicSize)*0.03f;
			}

			if (Input.anyKeyDown) {
				if (winner == Player.One) {
					Application.LoadLevel ("Player1Wins");
				} else if (winner == Player.Two) {
					Application.LoadLevel ("Player2Wins");
				} else {
					print ("DRAW!!!");
					Application.LoadLevel ("Draw");
				}
			}
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
