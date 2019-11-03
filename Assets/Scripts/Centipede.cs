﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Centipede : MonoBehaviour, IDestructible
{
	public int scoreValue;
	public Mushroom mushroomPrefab;
	public float distanceBetweenParts;
	public float nextRowHeight;
	public ThreeSprites headSprites;

	internal bool isHead;
	internal Rigidbody2D rb;
	internal float movementSpeed;
	
	private List<Centipede> centipedeChain;
	private Vector2 leftOrRight = Vector2.right;
	private List<Vector2> previousPositions;

	// store index of previous centipede part in chain 
	private int leaderIndex = 0;

	// store index of this centipede part in chain
	private int myIndexInChain = 0;

	// reference to a SpriteRenderer of this object
	private SpriteRenderer sr;
	private Action MoveBody = () => { };
	private bool wait = true;

	private void Awake()
	{
		// initialize objects
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		previousPositions = new List<Vector2>();
		MoveBody += CheckDistance;
	}


	private void Start()
	{
		// set reference to the list of centipede parts 
		centipedeChain = GameManager.instance.centipedeChain;
		// read speed from Game Manager
		movementSpeed = GameManager.instance.centipedeSpeed;
		if (isHead)
		{
			// start movement for the first head object
			rb.velocity = -Vector2.up * movementSpeed;
		}
		else // if this is not head
		{
			// set index of this object in centipede parts list
			FindMyIndex();
		}
	}

	// find indices of this object and its leader in centipede parts list
	private void FindMyIndex()
	{
		myIndexInChain = centipedeChain.IndexOf(this);
		leaderIndex = (myIndexInChain == 0) ? 0 : (myIndexInChain - 1);
	}

	public void ReceiveShot()
	{
		// if this is not last centipede part in list
		if (myIndexInChain != centipedeChain.Count - 1)
		{
			// define next centipede part
			Centipede next = centipedeChain[myIndexInChain + 1];
			// set next as head
			next.SetHead();
			// start next centipede part's movement in direction of this object
			next.StartMovementAsHead(rb.position);
		}

		// instantiate mushroom at this object's current position
		Instantiate(mushroomPrefab, transform.position, Quaternion.identity);
		// add score 
		GameManager.instance.AddScore(scoreValue);
		// removing self from list
		centipedeChain.RemoveAt(myIndexInChain);
		// refresh all indices
		foreach (Centipede centipede in centipedeChain)
		{
			centipede.FindMyIndex();
		}

		// check if this was the last centipede part
		GameManager.instance.CheckWin(1f);
		// destroy this object
		Destroy(gameObject);
	}

	// start movement to received position
	private void StartMovementAsHead(Vector2 targetPosition)
	{
		// save direction of movement in Vector2
		Vector2 nextStep = targetPosition - rb.position;
		// define current horizontal movement direction
		float x = (nextStep.x < 0 ? -1 : 1);
		// save horizontal movement direction as Vector2
		leftOrRight = new Vector2(x, 0);
		// check if direction is rather horizontal
		if (Mathf.Abs(nextStep.x) > Mathf.Abs(nextStep.y))
		{
			// move horizontally 
			rb.velocity = leftOrRight * movementSpeed;
		}
		else // if direction is rather vertical
		{
			// move down
			rb.velocity = Vector2.down * movementSpeed;
		}

		// check and set sprite in accordance to movement direction
		CheckHeadSprite();
	}

	// set this object's behavior as a head
	internal void SetHead()
	{
		// set flag
		isHead = true;
	}

	private void FixedUpdate()
	{
		// movement function
		Move();
		// saving current position
		SavePosition();
	}

	// save current position to list if it differs from last frame
	private void SavePosition()
	{
		// if this is last of centipede parts → do not save
		if (myIndexInChain == centipedeChain.Count - 1) return;

		// save first position to list
		if (previousPositions.Count == 0) previousPositions.Add(rb.position);
		// save current position to list if it is not the same as at last frame 
		else if (previousPositions[previousPositions.Count - 1] != rb.position || !wait)
		{
			previousPositions.Add(rb.position);
			wait = false;
		}
	}

	// fork movement function for head and usual parts of centipede
	private void Move()
	{
		// movement is defined by independent logic
		if (isHead) MoveHead();

		// follow the previous centipede part
		else MoveBody();
	}

	/************* head segment's movement logic block ↓↓↓ ***********/
	// move to left or right or down
	private void MoveHead()
	{
		// if reached the screen edge
		if (Mathf.Abs(rb.position.x) > GameManager.instance.sceneEdge)
		{
			// return to screen edge X
			rb.position = new Vector2(leftOrRight.x * GameManager.instance.sceneEdge, rb.position.y);
			// and start moving down
			GoDown();
		}
		// if passed next row height
		else if (rb.position.y < nextRowHeight)
		{
			// return to row height Y
			rb.position = new Vector2(rb.position.x, nextRowHeight);
			// and change nextRowHeight for all next part after this head before next head
			int i = myIndexInChain;
			do
			{
				// reduce this objects next row height with special value
				centipedeChain[i].nextRowHeight -= GameManager.instance.rowHeight;
				i++;
			} while (i < centipedeChain.Count && !centipedeChain[i].isHead);

			// change direction of horizontal movement and move
			ToLeftOrToRight();
		}

		// check and change head sprite
		CheckHeadSprite();
	}

	// change direction of horizontal movement and move
	private void ToLeftOrToRight()
	{
		leftOrRight = -leftOrRight;
		rb.velocity = leftOrRight * movementSpeed;
	}

	// move down
	private void GoDown()
	{
		rb.velocity = Vector2.down * movementSpeed;
	}

	// check and set sprite in accordance to movement direction
	private void CheckHeadSprite()
	{
		// initialize the sprite that should be set
		Sprite correctSprite = sr.sprite;

		// set the correct sprite according to movement direction
		// moving left
		if (rb.velocity.x < -0.1f)
		{
			correctSprite = headSprites.sprite1;
		}
		// moving right
		else if (rb.velocity.x > 0.1f)
		{
			correctSprite = headSprites.sprite3;
		}
		// moving down
		else if (rb.velocity.y < -0.1f)
		{
			correctSprite = headSprites.sprite2;
		}

		// check if current sprite is correct
		if (sr.sprite != correctSprite)
		{
			// set correct if it is not
			sr.sprite = correctSprite;
		}
	}

	// if collide with mushrooms move down
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (isHead && other.gameObject.GetComponent<Mushroom>())
		{
			GoDown();
		}

		if (other.gameObject.GetComponent<Player>())
		{
			GameManager.instance.LoseRound();
		}
	}
	/************* head segments movement block ↑↑↑ ***********/


	/*************** body segments movement ↓↓↓ ****************/
	private void CheckDistance()
	{
		// check distance to previous centipede part 
		float distanceToLeader = Vector2.Distance(rb.position, centipedeChain[leaderIndex].rb.position);
		// if distance is greater than defined → start movement
		if (distanceToLeader > distanceBetweenParts)
		{
			MoveBody -= CheckDistance;
			MoveBody += Follow;
		}
	}


	// follow the previous centipede part
	private void Follow()
	{
		// change this object's position to previous centipede part's position
		rb.position = centipedeChain[leaderIndex].previousPositions[0];
		// remove this position from previous centipede part's positions list
		centipedeChain[leaderIndex].previousPositions.RemoveAt(0);
	}
}