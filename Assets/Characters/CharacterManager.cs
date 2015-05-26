using UnityEngine;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {

	public Character[] characters;

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
	
	// Update is called once per frame
	void Update () {
		return;
	}
}
