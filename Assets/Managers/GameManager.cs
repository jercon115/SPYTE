using UnityEngine;
using System.Collections;

public enum Player {None, One, Two};

public class GameManager : MonoBehaviour {

	enum GameState { WaitForOne, RevealOne, WaitForTwo, RevealTwo, Play };

	public CharacterManager charMgr;

	private GameState gamestate;

	// Use this for initialization
	void Start () {
		gamestate = GameState.WaitForOne;
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
	}
}
