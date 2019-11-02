using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Centipede : Destructible
{
	// points given to player for this object destruction
	public int scoreValue;
	// mushroom prefab
	public Mushroom mushroom;
	// distance between centipede parts
	public float distanceBetween;
	// Y value of the next row
	public float nextRowHeight;
	// head sprites: left, down and right
	public ThreeSprites headSprites;

	// flag is this centipede part a head part
	internal bool isHead;
	internal Rigidbody2D rb;
	
	// reference to the list of all centipede parts
	private List<Centipede> chain;
	// horizontal movement direction in Vector2 format
	private Vector2 leftOrRight = Vector2.right;
	// list of positions at each frame for this centipede part
	private List<Vector2> positionsList;
	// speed of this centipede part
	private float speed;
	// store index of previous centipede part in chain 
	private int leaderIndex = 0;
	// store index of this centipede part in chain
	private int myIndexInChain = 0;
	// reference to a SpriteRenderer of this object
	private SpriteRenderer sr;

	private void Awake()
	{
		// initialize objects
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		positionsList = new List<Vector2>();
	}

	private void Start()
	{
		// set reference to the list of centipede parts 
		chain = GameManager.instance.chain;
		// read speed from Game Manager
		speed = GameManager.instance.centipedeSpeed;
		if (isHead)
		{
			// start movement for the first head object
			rb.velocity = -Vector2.up * speed;
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
		myIndexInChain = chain.IndexOf(this);
		leaderIndex = (myIndexInChain == 0) ? 0 : (myIndexInChain - 1);
	}

	// processing receiving a shot
	internal override void ReceiveShot()
	{
		// log message
		base.ReceiveShot();

		// if this is not last centipede part in list
		if (myIndexInChain != chain.Count - 1)
		{
			// define next centipede part
			Centipede next = chain[myIndexInChain + 1];
			// set next as head
			next.SetHead();
			// start next centipede part's movement in direction of this object
			next.StartMovementAsHead(rb.position);
		}

		// instantiate mushroom at this object's current position
		Instantiate(mushroom, transform.position, Quaternion.identity);
		// add score 
		GameManager.instance.AddScore(scoreValue);
		// removing self from list
		chain.RemoveAt(myIndexInChain);
		// refresh all indices
		foreach (Centipede centipede in chain)
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
			rb.velocity = leftOrRight * speed;
		}
		else // if direction is rather vertical
		{
			// move down
			rb.velocity = Vector2.down * speed;
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
		if (myIndexInChain == chain.Count - 1) return;

		// save first position to list
		if (positionsList.Count == 0) positionsList.Add(rb.position);
		// save current position to list if it is not the same as at last frame 
		else if (positionsList[positionsList.Count - 1] != rb.position) positionsList.Add(rb.position);
	}

	// fork movement function for head and usual parts of centipede
	private void Move()
	{
		// movement is defined by independent logic
		if (isHead) MoveHead();
		// follow the previous centipede part
		else Follow();
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
				chain[i].nextRowHeight -= GameManager.instance.rowHeight;
				i++;
			} while (i < chain.Count && !chain[i].isHead);
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
		rb.velocity = leftOrRight * speed;
	}

	// move down
	private void GoDown()
	{
		rb.velocity = Vector2.down * speed;
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

		if (other.gameObject.GetComponent<Ship>())
		{
			GameManager.instance.LoseRound();
		}
	}
	/************* head segments movement block ↑↑↑ ***********/


	/*************** body segments movement ↓↓↓ ****************/
	// follow the previous centipede part
	private void Follow()
	{
		// todo: no need to check this every time, once at the very beginning should be enough 
		// check distance to previous centipede part 
		float distanceToLeader = Vector2.Distance(rb.position, chain[leaderIndex].rb.position);
		// if distance is greater than defined → start movement
		if (distanceToLeader > distanceBetween)
		{
			// change this object's position to previous centipede part's position
			rb.position = chain[leaderIndex].positionsList[0];
			// remove this position from previous centipede part's positions list
			chain[leaderIndex].positionsList.RemoveAt(0);
		}
	}
}
