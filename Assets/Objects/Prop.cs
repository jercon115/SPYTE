using UnityEngine;
using System.Collections;

public class Prop : MonoBehaviour {

	private int oneTimer;
	private int twoTimer;

	// Use this for initialization
	void Start () {
		oneTimer = 0;
		twoTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (oneTimer > 0) oneTimer--;
		if (twoTimer > 0) twoTimer--;
	}

	public bool Interact(Player player) {
		if (player == Player.One) {
			oneTimer = 450;

			if (twoTimer > 0)
				return true;
		}

		if (player == Player.Two) {
			twoTimer = 450;

			if (oneTimer > 0)
				return true;
		}

		return false;
	}
}
