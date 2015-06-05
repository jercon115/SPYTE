using UnityEngine;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {

	private Character[] characters;
	public Character One;
	public Character Two;

	private Prop[] props;

	public RuntimeAnimatorController[] legs;
	public RuntimeAnimatorController[] bodies;
	public RuntimeAnimatorController[] heads;
	public Sprite[] legSprites;
	public Sprite[] bodySprites;
	public Sprite[] headSprites;

	public GameObject sadSpeech;
	public GameObject happySpeech;

	// Use this for initialization
	void Start () {
		props = (Prop[])FindObjectsOfType (typeof(Prop));
		characters = (Character[])FindObjectsOfType (typeof(Character));

		List<Character> setCharacters = new List<Character>(characters);

		for (int i = 0; i < legs.Length; i++) {
			for (int j = 0; j < bodies.Length; j++) {
				for (int k = 0; k < heads.Length; k++) {
					int c = Random.Range (0, setCharacters.Count);

					setCharacters[c].setAppearance(legs[i], bodies[j], heads[k], i, j, k);
					setCharacters[c].setSprites(legSprites[i], bodySprites[j], headSprites[k]);

					float x = Random.Range (-7.0f,7.0f); float y = Random.Range (-3.0f, 3.0f);
					setCharacters[c].transform.localPosition = new Vector3(x, y, y);
					setCharacters.RemoveAt (c);
				}
			}
		}
	}

	public void revealPlayer(Player player) {
		if (player == Player.None) {
			foreach(Character character in characters)
				character.setColor (new Color(1.0f, 1.0f, 1.0f, 1.0f));
		} else
			foreach(Character character in characters)
				character.setColor (new Color(1.0f, 1.0f, 1.0f, 0.5f));

		if (player == Player.One) {
			One.setColor (new Color(1.0f, 1.0f, 1.0f, 1.0f));
		} else
			Two.setColor (new Color(1.0f, 1.0f, 1.0f, 1.0f));
	}

	public void setPlayersPaused(int pausedLevel) {
		One.setPaused (pausedLevel);
		Two.setPaused (pausedLevel);
	}

	public void setAllPaused(int pausedLevel) {
		foreach (Character character in characters)
			character.setPaused (pausedLevel);
	}

	public GameObject getRandomObject () {
		int i = Random.Range (0, props.Length);
		return props [i].gameObject;
	}

	public Prop getClosestObject(float x, float y, float radius) {
		Prop foundObj = null;

		float maxDist = radius;
		foreach(Prop obj in props) {
			float dX = obj.transform.localPosition.x - x;
			float dY = obj.transform.localPosition.y - y;
			float dist = Mathf.Sqrt (dX*dX + dY*dY);
			if (dist < maxDist) {
				maxDist = dist;
				foundObj = obj;
			}
		}

		return foundObj;
	}

	public GameObject getRandomCharacter () {
		int i = Random.Range (0, characters.Length);
		return characters [i].gameObject;
	}

	public Character getClosestCharacter(Character me, float radius) {
		Character foundChar = null;

		float maxDist = radius;
		foreach(Character character in characters) {
			if (character == me) continue; // Skip if it is me

			float dist = (character.transform.localPosition - me.transform.localPosition).magnitude;
			if (dist < maxDist) {
				maxDist = dist;
				foundChar = character;
			}
		}
		
		return foundChar;
	}
	
	// Update is called once per frame
	void Update () {
		return;
	}
}
