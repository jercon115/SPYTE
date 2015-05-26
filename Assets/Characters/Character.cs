using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour {
	// constants
	const float speed = 1.0f;
	const float interactRad = 1.0f;

	enum Move {None, Left, Up, Right, Down};
	enum Action {None, Wait, Interact, Attack};
	enum AIState {None, Go, Do};

	public GameManager gameMgr;
	public Player player;
	public Character enemy;
	public CharacterManager charMgr;
	
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
	public Sprite legSprite;
	public Sprite bodySprite;
	public Sprite headSprite;
	private Animator legsAnimator;
	private Animator bodyAnimator;
	private Animator headAnimator;
	
	// control variables
	private List<Move> movement; // 0: Left, 1: Up, 2: Right, 3: Down
	private Move currentMove;
	private Action currentAction;
	private int pauseTimer;
	
	private bool paused;
	
	// AI variables
	private Action nextAction;
	private AIState currentState;
	private Vector2 moveTo; // used for AI's, to move to a point
	private int moveTimer;
	
	// Use this for initialization
	void Awake () {
		paused = true; pauseTimer = 0;
		movement = new List<Move>();
		currentMove = Move.None;
		currentAction = Action.None;
		
		nextAction = Action.None;
		currentState = AIState.None;
		moveTo = transform.localPosition;
		
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

	public void setSprites(Sprite legSpr, Sprite bodySpr, Sprite headSpr) {
		legSprite = legSpr;
		bodySprite = bodySpr;
		headSprite = headSpr;
	}

	public void setColor(Color color) {
		legs.GetComponent<SpriteRenderer> ().color = color;
		body.GetComponent<SpriteRenderer> ().color = color;
		head.GetComponent<SpriteRenderer> ().color = color;
	}
	
	private void Interact() {
		// Get closest object
		GameObject interactObj = charMgr.getClosestObject (transform.localPosition.x, transform.localPosition.y, interactRad);

		if (interactObj != null) {
			float dX = interactObj.transform.localPosition.x - transform.localPosition.x;
			float dY = interactObj.transform.localPosition.y - transform.localPosition.y;

			if (Mathf.Abs (dX) > Mathf.Abs (dY)) {
				if (dX < 0) {
					transform.localScale = new Vector3 (-1, 1, 1);
				} else
					transform.localScale = new Vector3 (1, 1, 1);

				legsAnimator.Play ("interact_side", -1, float.NegativeInfinity);
				bodyAnimator.Play ("interact_side", -1, float.NegativeInfinity);
				headAnimator.Play ("interact_side", -1, float.NegativeInfinity);
			} else {
				if (dY < 0) {
					legsAnimator.Play ("interact_down", -1, float.NegativeInfinity);
					bodyAnimator.Play ("interact_down", -1, float.NegativeInfinity);
					headAnimator.Play ("interact_down", -1, float.NegativeInfinity);
				} else {
					legsAnimator.Play ("interact_up", -1, float.NegativeInfinity);
					bodyAnimator.Play ("interact_up", -1, float.NegativeInfinity);
					headAnimator.Play ("interact_right", -1, float.NegativeInfinity);
				}
			}

			gameMgr.updateClues(interactObj, player);

			pauseTimer = 30;
		}
		return;
	}
	
	private void Attack() {
		legsAnimator.Play ("interact_down", -1, float.NegativeInfinity);
		bodyAnimator.Play ("interact_down", -1, float.NegativeInfinity);
		headAnimator.Play ("interact_down", -1, float.NegativeInfinity);
		pauseTimer = 30;

		float dX = enemy.transform.localPosition.x - transform.localPosition.x;
		float dY = enemy.transform.localPosition.y - transform.localPosition.y;
		
		float dist = Mathf.Sqrt (dX * dX + dY * dY);
		if (dist < 0.8) {
			if (player == Player.One) {
				Application.LoadLevel ("Player1Wins");
			} else
				Application.LoadLevel ("Player2Wins");
		}
	}
	
	private void PlayerAction() {
		if (Input.GetKeyDown (interact)) {
			currentAction = Action.Interact;
		}
		
		if (Input.GetKeyDown (attack)) {
			currentAction = Action.Attack;
		}
	}
	
	private void PlayerMovement() {
		if (Input.GetKeyDown (left)) {
			movement.Add (Move.Left);
		} else
			if (Input.GetKeyUp (left))
				movement.Remove (Move.Left);
		
		if (Input.GetKeyDown (up)) {
			movement.Add (Move.Up);
		} else
			if (Input.GetKeyUp (up))
				movement.Remove (Move.Up);
		
		if (Input.GetKeyDown (right)) {
			movement.Add (Move.Right);
		} else
			if (Input.GetKeyUp (right))
				movement.Remove (Move.Right);
		
		if (Input.GetKeyDown (down)) {
			movement.Add (Move.Down);
		} else
			if (Input.GetKeyUp (down))
				movement.Remove (Move.Down);
		
		if (movement.Count > 0) {
			currentMove = movement [movement.Count - 1];
		} else
			currentMove = Move.None;
	}
	
	private void NPCNext() {
		switch (Random.Range (0, 1)) {
		case 0: // Wait at a random point
			nextAction = Action.Wait;
			currentState = AIState.Go;
			moveTo = new Vector2 (Random.Range (-7.0f, 7.0f), Random.Range (-3.0f, 3.0f));
			break;
		}
	}
	
	private void NPCAction() { 
		currentAction = nextAction;
		currentState = AIState.None;
	}
	
	private void NPCMovement() {
		float distX = Mathf.Abs (moveTo.x - transform.localPosition.x);
		float distY = Mathf.Abs (moveTo.y - transform.localPosition.y);
		
		bool axisTargetReached = false;
		
		if (distX <= speed / 30.0f && (currentMove == Move.Left || currentMove == Move.Right))
			axisTargetReached = true;
		
		if (distY <= speed / 30.0f && (currentMove == Move.Down || currentMove == Move.Up))
			axisTargetReached = true;
		
		if (moveTimer == 0 || axisTargetReached) {
			int movX = 0, movY = 0;
			currentMove = Move.None;
			
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
					currentMove = Move.Left;
				} else
					currentMove = Move.Right;
			} else { // move on Y axis
				if (movY == -1) {
					currentMove = Move.Down;
				} else
					currentMove = Move.Up;
			}
			
			// SET ACTION TIMER TO A RANDOM VALUE UNDER WHAT IS NEEDED
			moveTimer = Random.Range (15, 180);
		} else
			moveTimer--;
	}
	
	// Update is called once per frame
	void Update () {
		if (paused) // if paused, return
			return;

		Rigidbody2D body = GetComponent<Rigidbody2D>();

		// Controls
		transform.Translate (new Vector3 (0.0f, 0.0f, transform.localPosition.y - transform.localPosition.z));
		
		if (player != Player.None) {
			PlayerMovement (); PlayerAction ();
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

		if (pauseTimer > 0) {
			pauseTimer--;
			return;
		}

		// Movement and Actions
		if (currentAction != Action.None) {
			switch (currentAction) {
			case Action.Wait:
				pauseTimer = Random.Range (15, 180);
				break;
			case Action.Interact:
				Interact ();
				break;
			case Action.Attack:
				Attack ();
				break;
			}

			body.velocity = new Vector2 (0.0f, 0.0f);
			currentAction = Action.None;
		}
		
		if (currentMove != Move.None) {
			legsAnimator.Play ("walk", -1, float.NegativeInfinity);
			bodyAnimator.Play ("walk", -1, float.NegativeInfinity);
			headAnimator.Play ("walk", -1, float.NegativeInfinity);
		}
		switch (currentMove) {
		case Move.Left:
			transform.localScale = new Vector3 (-1, 1, 1);
			body.velocity = new Vector2 (-speed, 0.0f);
			break;
		case Move.Up:
			body.velocity = new Vector2 (0.0f, speed);
			break;
		case Move.Right:
			transform.localScale = new Vector3 (1, 1, 1);
			body.velocity = new Vector2 (speed, 0.0f);
			break;
		case Move.Down:
			body.velocity = new Vector2 (0.0f, -speed);
			break;
		default:
			body.velocity = new Vector2 (0.0f, 0.0f);
			legsAnimator.Play ("idle", -1, float.NegativeInfinity);
			bodyAnimator.Play ("idle", -1, float.NegativeInfinity);
			headAnimator.Play ("idle", -1, float.NegativeInfinity);
			break;
		}
	}
}
