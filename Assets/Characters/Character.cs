using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour {
	enum Direction {None, Left, Up, Right, Down};
	enum Action {Wait, Interact};
	enum AIState {None, Go, Do};

	public Player player;
	public Character enemy;

	public KeyCode left;
	public KeyCode right;
	public KeyCode up;
	public KeyCode down;
	public KeyCode interact;
	public KeyCode attack;

	// sprites
	public GameObject legs;
	public GameObject body;
	public GameObject head;
	private Animator legsAnimator;
	private Animator bodyAnimator;
	private Animator headAnimator;

	// movement variables
	const float speed = 1.0f;
	private List<Direction> movement; // 0: Left, 1: Up, 2: Right, 3: Down
	private Direction currentMove;
	private bool paused;

	// AI variables
	private Action currentAction;
	private AIState currentState;
	private Vector2 moveTo; // used for AI's, to move to a point
	private int actionTimer;
	private int moveTimer;

	// Use this for initialization
	void Awake () {
		paused = true;
		movement = new List<Direction>();
		currentMove = Direction.None;

		currentAction = Action.Wait;
		moveTo = transform.localPosition;
		actionTimer = 0;

		legsAnimator = legs.GetComponent<Animator>();
		bodyAnimator = body.GetComponent<Animator>();
		headAnimator = head.GetComponent<Animator>();
		legsAnimator.Play ("idle", -1, float.NegativeInfinity);
		bodyAnimator.Play("idle", -1, float.NegativeInfinity);
		headAnimator.Play("idle", -1, float.NegativeInfinity);
	}

	public void setPaused(bool newPaused) {
		paused = newPaused;
	}

	public void setAppearance(RuntimeAnimatorController legsCont, RuntimeAnimatorController bodyCont, RuntimeAnimatorController headCont) {
		legsAnimator.runtimeAnimatorController = legsCont;
		bodyAnimator.runtimeAnimatorController = bodyCont;;
		headAnimator.runtimeAnimatorController = headCont;
	}

	public void setColor(Color color) {
		legs.GetComponent<SpriteRenderer> ().color = color;
		body.GetComponent<SpriteRenderer> ().color = color;
		head.GetComponent<SpriteRenderer> ().color = color;
	}

	private void PlayerAttack() {
		if (Input.GetKey (attack)) {
			legsAnimator.Play ("interact_down", -1, float.NegativeInfinity);
			bodyAnimator.Play("interact_down", -1, float.NegativeInfinity);
			headAnimator.Play("interact_down", -1, float.NegativeInfinity);
		}
		if (Input.GetKeyDown (attack)) {
			float dX = enemy.transform.localPosition.x - transform.localPosition.x;
			float dY = enemy.transform.localPosition.y - transform.localPosition.y;
			float dist = Mathf.Sqrt (dX*dX + dY*dY);
			if (dist < 0.5) {
				if (player == Player.One) {
					Application.LoadLevel ("Player1Wins");
				} else {
					Application.LoadLevel ("Player2Wins");
				}
			}
		}
	}

	private void PlayerMovement() {
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

	private void NPCNext() {
		switch (Random.Range (0, 1)) {
		case 0: // Wait at a random point
			currentAction = Action.Wait;
			currentState = AIState.Go;
			moveTo = new Vector2 (Random.Range (-7.0f, 7.0f), Random.Range (-3.0f, 3.0f));
			actionTimer = Random.Range (15, 180);
			break;
		}
	}

	private void NPCAction() { 
		if (actionTimer == 0) {
			currentState = AIState.None;
		} else
			actionTimer--;
	}

	private void NPCMovement() {
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
				moveTimer = 0;
				currentState = AIState.Do;
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
		if (paused)
			return;

		transform.Translate (new Vector3 (0.0f, 0.0f, transform.localPosition.y - transform.localPosition.z));

		if (player != Player.None) {
			PlayerMovement (); PlayerAttack ();
		} else {
			switch(currentState) {
			case AIState.None:
				NPCNext ();
				break;
			case AIState.Go:
				NPCMovement ();
				break;
			case AIState.Do:
				NPCAction ();
				break;
			}
		}

		Rigidbody2D body = GetComponent<Rigidbody2D>();

		if (currentMove != Direction.None) {
			legsAnimator.Play ("walk", -1, float.NegativeInfinity);
			bodyAnimator.Play ("walk", -1, float.NegativeInfinity);
			headAnimator.Play ("walk", -1, float.NegativeInfinity);
		}
		switch (currentMove) {
		case Direction.Left:
			transform.localScale = new Vector3 (-1, 1, 1);
			body.velocity = new Vector2 (-speed, 0.0f);
			break;
		case Direction.Up:
			body.velocity = new Vector2 (0.0f, speed);
			break;
		case Direction.Right:
			transform.localScale = new Vector3 (1, 1, 1);
			body.velocity = new Vector2 (speed, 0.0f);
			break;
		case Direction.Down:
			body.velocity = new Vector2 (0.0f, -speed);
			break;
		default:
			body.velocity = new Vector2 (0.0f, 0.0f);
			if (!Input.GetKey (attack)) {
				legsAnimator.Play ("idle", -1, float.NegativeInfinity);
				bodyAnimator.Play ("idle", -1, float.NegativeInfinity);
				headAnimator.Play ("idle", -1, float.NegativeInfinity);
			}
			break;
		}
	}
}
