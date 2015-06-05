using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Player {None, One, Two};

public class GameManager : MonoBehaviour {

	enum GameState { WaitForOne, RevealOne, WaitForTwo, RevealTwo, Play, End };
	
	public CharacterManager charMgr;

	public Text MissionTimerText;
	public Image MissionImage;
	public Sprite MissionSprite;
	public GameObject Mission;
	public GameObject[] Objects;

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
	private bool MissionFound;
	private int PlayerOneClues;
	private int PlayerTwoClues;
	private GameState gamestate;

	// Use this for initialization
	void Start () {
		gamestate = GameState.WaitForOne;

		PlayerOneClues = 0;
		PlayerTwoClues = 0;

		MissionTimer = 1200; MissionFound = false;
		int RandomMission = Random.Range (0, 10);
		Mission = Objects [RandomMission];
		Mission.GetComponent<SpriteRenderer> ().color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
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
			case GameState.End:
				charMgr.pauseAll();
				break;
			}
		}

		if (gamestate == GameState.Play) {
			if (MissionTimer > 0) {
				MissionTimer--;
				int MissionTimerSeconds = MissionTimer/40;
				MissionTimerText.text = MissionTimerSeconds.ToString();
			} else {
				updateAppearances();

				MissionTimer = 1200; MissionFound = false;
				int RandomMission = Random.Range (0, 10);
				Mission.GetComponent<SpriteRenderer> ().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				Mission = Objects [RandomMission];
				Mission.GetComponent<SpriteRenderer> ().color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
			}
		}
	}

	public void EndGame() {
		gamestate = GameState.End;
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

	public void updateClues(GameObject obj, Player player) {
		if (MissionFound)
			return;

		if (obj == Mission) {
			if (player == Player.One) {
				PlayerOneClues++;
			} else if (player == Player.Two) {
				PlayerTwoClues++;
			}
			MissionFound = true;
		}
	}

	private void updateAppearances() {
		switch(PlayerOneClues) {
		case 1:
			PlayerTwoHead.sprite = charMgr.Two.headSprite;
			break;
		case 2:
			PlayerTwoBody.sprite = charMgr.Two.bodySprite;
			break;
		case 3:
			PlayerTwoLegs.sprite = charMgr.Two.legSprite;
			break;
		}

		switch(PlayerTwoClues) {
		case 1:
			PlayerOneHead.sprite = charMgr.One.headSprite;
			break;
		case 2:
			PlayerOneBody.sprite = charMgr.One.bodySprite;
			break;
		case 3:
			PlayerOneLegs.sprite = charMgr.One.legSprite;
			break;
		}
	}
}
