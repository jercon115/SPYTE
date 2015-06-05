using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour {
	// constants
	const float speed = 1.3f;
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
	public Animator legsAnimator;
	public Animator bodyAnimator;
	public Animator headAnimator;

	[HideInInspector]

	public int legsIndex;
	public int bodyIndex;
	public int headIndex;
	
	// control variables
	private List<Move> movement; // 0: Left, 1: Up, 2: Right, 3: Down
	private Move currentMove;
	private Action currentAction;
	private float pauseTimer;
	
	private int paused;
	
	// AI variables
	private Action nextAction;
	private AIState currentState;
	private Vector2 moveTo; // used for AI's, to move to a point
	private GameObject targetObject;
	private float moveToRad; // stop when within certain distance of moveTo
	private float moveTimer;

	// Situation variables
	private float situationReaction;
	private float danceReaction;
	private bool dancePause;

	public Situation currentSituation;
	private Situation nextSituation;
	
	// Use this for initialization
	void Awake () {
		paused = 2; pauseTimer = 0;
		movement = new List<Move>();
		currentMove = Move.None;
		currentAction = Action.None;
		
		nextAction = Action.None;
		currentState = AIState.None;
		moveTo = transform.localPosition;
		targetObject = null;
		moveToRad = 0.05f;
		moveTimer = 0;

		danceReaction = 0;
		dancePause = false;
		situationReaction = Random.Range (0.5f,1.5f);
		nextSituation = Situation.None;
		
		legsAnimator = legs.GetComponent<Animator>();
		bodyAnimator = body.GetComponent<Animator>();
		headAnimator = head.GetComponent<Animator>();
		legsAnimator.Play ("idle", -1, float.NegativeInfinity);
		bodyAnimator.Play("idle", -1, float.NegativeInfinity);
		headAnimator.Play("idle", -1, float.NegativeInfinity);

		legsIndex = 0;
		bodyIndex = 0;
		headIndex = 0;
	}
	
	public void setPaused(int newPaused) {
		paused = newPaused;
	}
	
	public void setAppearance(RuntimeAnimatorController legsCont,
	                          RuntimeAnimatorController bodyCont,
	                          RuntimeAnimatorController headCont,
	                          int legsI, int bodyI, int headI) {
		legsAnimator.runtimeAnimatorController = legsCont;
		bodyAnimator.runtimeAnimatorController = bodyCont;
		headAnimator.runtimeAnimatorController = headCont;

		legsIndex = legsI; bodyIndex = bodyI; headIndex = headI;
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
		Prop interactProp = charMgr.getClosestObject (transform.localPosition.x, transform.localPosition.y, interactRad);

		// Get closest character
		Character interactChar = charMgr.getClosestCharacter (this, interactRad);

		// Nothing found
		if (interactProp == null && interactChar == null) return;

		// Only object found
		if (interactProp != null && interactChar == null) {
			InteractProp (interactProp);
			return;
		}

		// Only character found
		if (interactProp == null && interactChar != null) {
			InteractCharacter(interactChar);
			return;
		}
		
		// Both found, use closest one
		float objDist = (interactProp.transform.localPosition - transform.localPosition).magnitude;
		float charDist = (interactChar.transform.localPosition - transform.localPosition).magnitude;

		if (charDist < objDist) {
			InteractCharacter(interactChar);
		} else
			InteractProp (interactProp);
	}

	public int CompareCharacter(Character you) {
		int numCorrect = 0;

		if (you.legsIndex == legsIndex) numCorrect++;
		if (you.bodyIndex == bodyIndex) numCorrect++;
		if (you.headIndex == headIndex) numCorrect++;

		return numCorrect;
	}

	private void InteractProp(Prop interactProp) {
		bool happy;
		if (player != Player.None) {
			happy = interactProp.Interact (player);
		} else
			happy = (Random.Range (0, 2) == 1);
		
		if (happy) {
			Instantiate (charMgr.happySpeech, transform.localPosition + new Vector3(0.0f, 1.3f), Quaternion.identity);
		} else
			Instantiate (charMgr.sadSpeech, transform.localPosition + new Vector3(0.0f, 1.3f), Quaternion.identity);
		
		InteractAnimation(interactProp.gameObject);
	}

	private void InteractCharacter(Character interactChar) {
		bool happy;
		if (player != Player.None) {
			happy = (enemy.CompareCharacter (interactChar) >= 2);
		} else
			happy = (Random.Range (0, 2) == 1);

		if (happy) {
			Instantiate (charMgr.happySpeech, transform.localPosition + new Vector3(0.0f, 1.3f), Quaternion.identity);
		} else
			Instantiate (charMgr.sadSpeech, transform.localPosition + new Vector3(0.0f, 1.3f), Quaternion.identity);

		InteractAnimation(interactChar.gameObject);
	}

	private void InteractAnimation(GameObject interactObj) {
		float dX = interactObj.transform.localPosition.x - transform.localPosition.x;
		float dY = interactObj.transform.localPosition.y - transform.localPosition.y;
		
		if (Mathf.Abs (dX) > Mathf.Abs (dY)) {
			if (dX < 0) {
				transform.localScale = new Vector3 (-1, 1, 1);
			} else
				transform.localScale = new Vector3 (1, 1, 1);
			
			legsAnimator.Play ("use_side", -1, float.NegativeInfinity);
			bodyAnimator.Play ("use_side", -1, float.NegativeInfinity);
			headAnimator.Play ("use_side", -1, float.NegativeInfinity);
		} else {
			if (dY < 0) {
				legsAnimator.Play ("use_down", -1, float.NegativeInfinity);
				bodyAnimator.Play ("use_down", -1, float.NegativeInfinity);
				headAnimator.Play ("use_down", -1, float.NegativeInfinity);
			} else {
				legsAnimator.Play ("use_up", -1, float.NegativeInfinity);
				bodyAnimator.Play ("use_up", -1, float.NegativeInfinity);
				headAnimator.Play ("use_up", -1, float.NegativeInfinity);
			}
		}

		pauseTimer = 1.0f;
	}
	
	private void Attack() {
		legsAnimator.Play ("use_down", -1, float.NegativeInfinity);
		bodyAnimator.Play ("use_down", -1, float.NegativeInfinity);
		headAnimator.Play ("use_down", -1, float.NegativeInfinity);
		pauseTimer = 1.0f;

		float dX = enemy.transform.localPosition.x - transform.localPosition.x;
		float dY = enemy.transform.localPosition.y - transform.localPosition.y;
		
		float dist = Mathf.Sqrt (dX * dX + dY * dY);
		if (dist < 0.8) {
			enemy.Die();
			charMgr.setAllPaused (2);
			charMgr.setPlayersPaused (1);
			gameMgr.setWinner (player);
		}
	}

	public void Die() {
		legsAnimator.Play ("death", -1, float.NegativeInfinity);
		bodyAnimator.Play ("death", -1, float.NegativeInfinity);
		headAnimator.Play ("death", -1, float.NegativeInfinity);
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
		switch (Random.Range (0, 3)) {
		case 0: // Wait at a random point
			nextAction = Action.Wait;
			targetObject = null;
			moveTo = new Vector2 (Random.Range (-7.0f, 7.0f), Random.Range (-3.0f, 3.0f));
			moveToRad = speed / 30.0f;
			break;
		case 1: // Interact with random object
			nextAction = Action.Interact;
			GameObject interactObj = charMgr.getRandomObject();
			targetObject = interactObj;
			moveTo = targetObject.transform.localPosition;
			moveToRad = 0.5f;
			break;
		case 2: // Interact with random character
			nextAction = Action.Interact;
			GameObject interactChar = charMgr.getRandomCharacter();
			targetObject = interactChar;
			moveTo = targetObject.transform.localPosition;
			moveToRad = 0.5f;
			break;
		}

		// Add a little delay before moving
		currentMove = Move.None;
		moveTimer = Random.Range (0.0f, 2.5f);

		currentState = AIState.Go;
	}
	
	private void NPCAction() { 
		currentAction = nextAction;
		currentState = AIState.None;
		targetObject = null;
	}
	
	private void NPCMovement() {
		if (currentSituation == Situation.Dance) {
			if (charMgr.danceTimer == 0) {
				danceReaction = Random.Range (-0.2f,0.2f);
			} else {
				if (charMgr.danceTimer > 0.5f + danceReaction) {
					dancePause = true;
					return;
				} else
					dancePause = false;
			}
		}

		if (targetObject != null)
			moveTo = targetObject.transform.localPosition;

		float distX = Mathf.Abs (moveTo.x - transform.localPosition.x);
		float distY = Mathf.Abs (moveTo.y - transform.localPosition.y);
		
		bool axisTargetReached = false;
		
		if (distX <= moveToRad && (currentMove == Move.Left || currentMove == Move.Right))
			axisTargetReached = true;
		
		if (distY <= moveToRad && (currentMove == Move.Down || currentMove == Move.Up))
			axisTargetReached = true;
		
		if (moveTimer <= 0 || axisTargetReached) {
			int movX = 0, movY = 0;
			currentMove = Move.None;
			
			// DETERMINE WHAT DIRECTIONS (OR NONE) IN EACH AXES IS REQUIRED
			if (distX > moveToRad) {
				if (moveTo.x > transform.localPosition.x) {
					movX = 1; // target is to the right
				} else
					movX = -1; // otherwise it is to the left
			}
			
			if (distY > moveToRad) {
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
			moveTimer = Random.Range (0.5f, 2.0f);
		} else
			moveTimer -= Time.deltaTime;
	}
	private void NPCSituation() {
		if (currentSituation != nextSituation) {
			if (situationReaction <= 0) {
				currentSituation = nextSituation;
				dancePause = false;
			} else
				situationReaction -= Time.deltaTime;
		}
	}

	public void setSituation(Situation sit) {
		situationReaction = Random.Range (0.2f, 1.5f);
		nextSituation = sit;
	}

	// Update is called once per frame
	void Update () {
		NPCSituation ();

		Rigidbody2D body = GetComponent<Rigidbody2D>();

		// Controls
		transform.Translate (new Vector3 (0.0f, 0.0f, transform.localPosition.y - transform.localPosition.z));
		
		if (player != Player.None) {
			PlayerMovement (); PlayerAction ();
		}

		// paused = 1 (paused), paused = 2 (frozen)
		if (paused > 0 || pauseTimer > 0) {
			body.velocity = new Vector2 (0.0f, 0.0f);

			if (paused == 2) { //frozen
				if (legsAnimator.speed != 0) legsAnimator.speed = 0;
				if (bodyAnimator.speed != 0) bodyAnimator.speed = 0;
				if (headAnimator.speed != 0) headAnimator.speed = 0;
			}

			pauseTimer -= Time.deltaTime;
			return;
		} else {
			if (legsAnimator.speed != 1) legsAnimator.speed = 1;
			if (bodyAnimator.speed != 1) bodyAnimator.speed = 1;
			if (headAnimator.speed != 1) headAnimator.speed = 1;
		}

		// AI
		if (player == Player.None) {
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

		// Movement and Actions
		if (currentAction != Action.None) {
			switch (currentAction) {
			case Action.Wait:
				legsAnimator.Play ("idle", -1, float.NegativeInfinity);
				bodyAnimator.Play ("idle", -1, float.NegativeInfinity);
				headAnimator.Play ("idle", -1, float.NegativeInfinity);
				pauseTimer = Random.Range (0.5f, 3.0f);
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

		Move move = currentMove;
		if (dancePause && currentSituation == Situation.Dance)
			move = Move.None;

		if (move != Move.None) {
			legsAnimator.Play ("walk", -1, float.NegativeInfinity);
			bodyAnimator.Play ("walk", -1, float.NegativeInfinity);
			headAnimator.Play ("walk", -1, float.NegativeInfinity);
		}
		switch (move) {
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
