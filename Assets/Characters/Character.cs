using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour {

	public enum Player {None, One, Two};
	enum Direction {None, Left, Up, Right, Down};
	enum Action {Wait, Move};

	public Player player;

	public KeyCode left;
	public KeyCode right;
	public KeyCode up;
	public KeyCode down;
	public KeyCode interact;
	public KeyCode attack;

	// movement variables
	const float speed = 2.5f;
	private List<Direction> movement; // 0: Left, 1: Up, 2: Right, 3: Down
	private Direction currentMove;

	// AI variables
	private Action currentAction;
	private Vector2 moveTo; // used for AI's, to move to a point
	private int actionTimer;
	private int moveTimer;

	// Use this for initialization
	void Start () {
		movement = new List<Direction>();
		currentMove = Direction.None;

		currentAction = Action.Wait;
		moveTo = transform.localPosition;
		actionTimer = 0;
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

		if (movement.Count > 0) {
			currentMove = movement [movement.Count - 1];
		} else
			currentMove = Direction.None;
	}

	void NPCAction() { 
		if (actionTimer == 0) {
			switch (Random.Range (0, 2)) {
			case 0: // Move to a random point
				currentAction = Action.Move;
				moveTo = new Vector2 (Random.Range (-5.0f, 5.0f), Random.Range (-5.0f, 5.0f));
				actionTimer = 1;
				break;
			case 1: // Wait
				currentAction = Action.Wait;
				actionTimer = Random.Range (15, 180);
				break;
			}
		} else {
			if (currentAction != Action.Move) actionTimer--;
		}
	}

	void NPCMovement() {
		float distX = Mathf.Abs (moveTo.x - transform.localPosition.x);
		float distY = Mathf.Abs (moveTo.y - transform.localPosition.y);

		bool axisTargetReached = false;

		if (distX <= speed / 30.0f && (currentMove == Direction.Left || currentMove == Direction.Right))
		    axisTargetReached = true;

		if (distY <= speed / 30.0f && (currentMove == Direction.Down || currentMove == Direction.Up))
			axisTargetReached = true;

		if (moveTimer == 0 || axisTargetReached) {
			int movX = 0, movY = 0;
			currentMove = Direction.None;

			// DETERMINE WHAT DIRECTIONS (OR NONE) IN EACH AXES IS REQUIRED
			if (distX > speed / 30.0f) {
				if (moveTo.x > transform.localPosition.x) {
					movX = 1; // target is to the right
				} else
					movX = -1; // otherwise it is to the left
			}

			if (distY > speed / 30.0f) {
				if (moveTo.y > transform.localPosition.y) {
					movY = 1; // target is to the above
				} else
					movY = -1; // otherwise it is to the bottom
			}

			// IF NEITHER NEEDED, DONE
			if (movX == 0 && movY == 0) {
				actionTimer = 0; // Set up new action
				moveTimer = 0;
				return;
			}

			// PICK ONE IF BOTH ARE NEEDED
			if (movX != 0 && movY != 0) {
				if (Random.Range (0.0f,1.0f) >= 0.5f) {
					movX = 0;
				} else
					movY = 0;
			}

			// MOVE ON DECIDED AXIS
			if (movX != 0) { // move on X axis
				if (movX == -1) {
					currentMove = Direction.Left;
				} else
					currentMove = Direction.Right;
			} else { // move on Y axis
				if (movY == -1) {
					currentMove = Direction.Down;
				} else
					currentMove = Direction.Up;
			}

			// SET ACTION TIMER TO A RANDOM VALUE UNDER WHAT IS NEEDED
			moveTimer = Random.Range (15, 180);
		} else
			moveTimer--;
	}

	// Update is called once per frame
	void Update () {
		transform.Translate (new Vector3 (0.0f, 0.0f, transform.localPosition.y - transform.localPosition.z));

		if (player != Player.None) {
			PlayerMovement ();
		} else {
			if (currentAction == Action.Move) NPCMovement ();
			NPCAction();
		}

		Rigidbody2D body = GetComponent<Rigidbody2D>();
		switch (currentMove) {
		case Direction.Left:
			body.velocity = new Vector2 (-speed, 0.0f);
			break;
		case Direction.Up:
			body.velocity = new Vector2 (0.0f, speed);
			break;
		case Direction.Right:
			body.velocity = new Vector2 (speed, 0.0f);
			break;
		case Direction.Down:
			body.velocity = new Vector2 (0.0f, -speed);
			break;
		default:
			body.velocity = new Vector2 (0.0f, 0.0f);
			break;
		}
	}
}
