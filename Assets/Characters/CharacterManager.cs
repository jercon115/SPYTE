using UnityEngine;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {

	public Character[] characters;
	public Character One;
	public Character Two;

	public GameObject[] objects;

	public RuntimeAnimatorController[] legs;
	public RuntimeAnimatorController[] bodies;
	public RuntimeAnimatorController[] heads;


	// Use this for initialization
	void Start () {
		List<Character> setCharacters = new List<Character>(characters);

		foreach (RuntimeAnimatorController leg in legs) {
			foreach (RuntimeAnimatorController body in bodies) {
				foreach (RuntimeAnimatorController head in heads) {
					int i = Random.Range (0, setCharacters.Count);

					setCharacters[i].setAppearance(leg, body, head);

					float x = Random.Range (-7.0f,7.0f); float y = Random.Range (-3.0f, 3.0f);
					setCharacters[i].transform.localPosition = new Vector3(x, y, y);
					setCharacters.RemoveAt (i);
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

	public void pauseAll() {
		foreach (Character character in characters)
			character.setPaused (true);
	}

	public void unpauseAll() {
		foreach (Character character in characters)
			character.setPaused (false);
	}

	public GameObject getClosestObject(float x, float y, float radius) {
		GameObject foundObj = null;

		float maxDist = radius;
		foreach(GameObject obj in objects) {
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
	
	// Update is called once per frame
	void Update () {
		return;
	}
}
