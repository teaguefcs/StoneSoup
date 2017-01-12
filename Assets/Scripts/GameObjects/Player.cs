﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Tile {

	public float moveSpeed = 10f;
	public float moveAcceleration = 10f;

	protected int _walkDirection = 2;

	// Like the GameManager, there should always only be one player, globally accessible
	protected static Player _instance = null;
	public static Player instance {
		get { return _instance; }
	}
	void Awake() {
		_instance = this;
	}
	void OnDestroy() {
		_instance = null;
	}

	void FixedUpdate() {
		// Let's move via the keyboard controls

		bool tryToMoveUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
		bool tryToMoveRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
		bool tryToMoveDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
		bool tryToMoveLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);

		Vector2 attemptToMoveDir = Vector2.zero;

		if (tryToMoveUp) {
			attemptToMoveDir += Vector2.up;
		}
		else if (tryToMoveDown) {
			attemptToMoveDir -= Vector2.up;			
		}

		if (tryToMoveRight) {
			attemptToMoveDir += Vector2.right;
		}
		else if (tryToMoveLeft) {
			attemptToMoveDir -= Vector2.right;
		}

		attemptToMoveDir.Normalize();

		if (attemptToMoveDir.x > 0) {
			_sprite.flipX = false;
		}
		else if (attemptToMoveDir.x < 0) {
			_sprite.flipX = true;
		}


		if (attemptToMoveDir.y > 0 && attemptToMoveDir.x == 0) {
			_walkDirection = 0;
		}
		else if (attemptToMoveDir.y < 0 && attemptToMoveDir.x == 0) {
			_walkDirection = 2;
		}
		else if (attemptToMoveDir.x != 0) {
			_walkDirection = 1;
		}
		_anim.SetBool("Walking", attemptToMoveDir.x != 0 || attemptToMoveDir.y != 0);
		_anim.SetInteger("Direction", _walkDirection);
		moveViaVelocity(attemptToMoveDir, moveSpeed, moveAcceleration);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			// Check to see if we're on top of an item that can be held
			RaycastHit2D[] maybeResults = new RaycastHit2D[10];
			int numObjectsFound = _body.Cast(Vector2.zero, maybeResults);
			Debug.Log(numObjectsFound);
			for (int i = 0; i < numObjectsFound && i < maybeResults.Length; i++) {
				RaycastHit2D result = maybeResults[i];
				if (result.transform.gameObject.tag != "Tile") {
					continue;
				}
				Tile tileHit = result.transform.GetComponent<Tile>();

				if (tileHit.hasTag(TileTags.CanBeHeld)) {
					Debug.Log("Found a thing to pick up" + tileHit);
					tileHit.pickUp(this);
				}

			}
		}
	}

}
