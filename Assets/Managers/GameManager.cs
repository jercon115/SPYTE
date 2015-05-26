using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Player {None, One, Two};

public class GameManager : MonoBehaviour {

	enum GameState { WaitForOne, RevealOne, WaitForTwo, RevealTwo, Play };

	public CharacterManager charMgr;

	public Text MissionTimerText;
	public Image MissionSprite;
	public GameObject Mission;

	public Sprite HeadBlack;
	public Sprite HeadBlond;
	public Sprite BodyRed;
	public Sprite BodyBlue;
	public Sprite LegsRed;
	public Sprite LegsBlue;

	public Image PlayerOneHead;
	public Image PlayerOneBody;
	public Image PlayerOneLegs;
	public Image PlayerTwoHead;
	public Image PlayerTwoBody;
	public Image PlayerTwoLegs;

	private int MissionTimer;
	private GameState gamestate;

	// Use this for initialization
	void Start () {
		gamestate = GameState.WaitForOne;
		
		MissionTimer = 2400;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.anyKeyDown && gamestate != GameState.Play) {
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
			case GameState.Play:
				charMgr.revealPlayer (Player.None);
				charMgr.unpauseAll();
				break;
			}
		}

		if (gamestate == GameState.Play) {
			Debug.Log("TIMING");
			if (MissionTimer > 0) {
				MissionTimer--;
				int MissionTimerSeconds = MissionTimer/40;
				MissionTimerText.text = MissionTimerSeconds.ToString();
			} else {
				MissionTimer = 2400;
			}
		}
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
