using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour {

	public enum Player {None, One, Two};
	enum Direction {Left, Up, Right, Down};

	public Player player;

	public KeyCode left;
	public KeyCode right;
	public KeyCode up;
	public KeyCode down;
	public KeyCode interact;
	public KeyCode attack;

	public float speed;

	private List<Direction> movement; // 0: Left, 1: Up, 2: Right, 3: Down

	// Use this for initialization
	void Start () {
		movement = new List<Direction>();
	}

	void PlayerMovement() {
		if (Input.GetKeyDown (left)) {
			movement.Add (Direction.Left);
		} else
			if (Input.GetKeyUp (left))
			movement.Remove (Direction.Left);

		if (Input.GetKeyDown (up)) {
			movement.Add (Direction.Up);
		} else
			if (Input.GetKeyUp (up))
				movement.Remove (Direction.Up);

		if (Input.GetKeyDown (right)) {
			movement.Add (Direction.Right);
		} else
			if (Input.GetKeyUp (right))
				movement.Remove (Direction.Right);

		if (Input.GetKeyDown (down)) {
			movement.Add (Direction.Down);
		} else
			if (Input.GetKeyUp (down))
				movement.Remove (Direction.Down);
	}

	void NPCMovement() {
		movement.Clear ();
	}

	// Update is called once per frame
	void Update () {
		if (player > 0) {
			PlayerMovement ();
		} else
			NPCMovement();

		Rigidbody2D body = GetComponent<Rigidbody2D>();
		if (movement.Count > 0) {
			switch(movement[movement.Count-1]) {
			case Direction.Left:
				body.velocity = new Vector2(-speed,0.0f);
				break;
			case Direction.Up:
				body.velocity = new Vector2(0.0f, speed);
				break;
			case Direction.Right:
				body.velocity = new Vector2(speed,0.0f);
				break;
			case Direction.Down:
				body.velocity = new Vector2(0.0f,-speed);
				break;
			}
		} else
			body.velocity = new Vector2(0.0f,0.0f);
	}
}
