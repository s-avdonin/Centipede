using System;
using System.Collections.Generic;
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

	// store index of forward centipede part in chain 
	private int leaderIndex;
	private int myIndex;
	private SpriteRenderer sr;
	private Action MoveBody = () => { };
	private float sceneEdge;
	private float rowHeight;
	private Vector2 rigidbodyPosition;

	private void Awake()
	{
		sceneEdge = GameManager.instance.sceneEdge;
		rowHeight = GameManager.instance.rowHeight;
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		previousPositions = new List<Vector2>();
		MoveBody += CheckDistance;
	}


	private void Start()
	{
		centipedeChain = GameManager.instance.centipedeChain;
		movementSpeed = GameManager.instance.centipedeSpeed;
		if (isHead)
			rb.velocity = -Vector2.up * movementSpeed;
		else
			DefineIndices();
	}

	private void DefineIndices()
	{
		myIndex = centipedeChain.IndexOf(this);
		leaderIndex = (myIndex == 0) ? 0 : (myIndex - 1);
	}

	public void ReceiveShot()
	{
		// if this is not last centipede part in list
		if (myIndex != centipedeChain.Count - 1)
		{
			Centipede next = centipedeChain[myIndex + 1];
			if (!next.isHead)
			{
				next.MarkAsHead();
				next.StartHeadMovement(rb.position);
			}
		}
		Instantiate(mushroomPrefab, transform.position, Quaternion.identity);
		GameManager.instance.AddScore(scoreValue);
		centipedeChain.RemoveAt(myIndex);
		foreach (Centipede centipede in centipedeChain) 
			centipede.DefineIndices();
		// check if this was the last centipede part
		GameManager.instance.CheckWin(1f);
		Destroy(gameObject);
	}

	private void StartHeadMovement(Vector2 targetPosition)
	{
		// save direction of movement 
		Vector2 nextStep = targetPosition - rb.position;
		// define current horizontal direction  
		leftOrRight = nextStep.x < 0 ? Vector2.left : Vector2.right; // new Vector2(x, 0);
		// check if direction is rather horizontal → move horizontally
		if (Mathf.Abs(nextStep.x) > Mathf.Abs(nextStep.y))
			rb.velocity = leftOrRight * movementSpeed;
		else // if direction is rather vertical → move down
			rb.velocity = Vector2.down * movementSpeed;
		// check and set sprite in accordance to movement direction
		RefreshHeadSprite();
	}

	internal void MarkAsHead()
	{
		isHead = true;
	}

	private void FixedUpdate()
	{
		rigidbodyPosition = rb.position;
		SavePosition();
		if (isHead) MoveHead();
		else MoveBody();
	}
	
	// save current position to list if it differs from last frame
	private void SavePosition()
	{
		// if this part is last in chain 
		if (myIndex == centipedeChain.Count - 1) return;
		// save first position
		if (previousPositions.Count == 0) previousPositions.Add(rigidbodyPosition);
		// save current position if changed 
		else if (previousPositions[previousPositions.Count - 1] != rigidbodyPosition) 
			previousPositions.Add(rigidbodyPosition);
	}

	/************* head segment's movement ***********/
	private void MoveHead()
	{
		if (Mathf.Abs(rigidbodyPosition.x) > sceneEdge)
		{
			// return to screen edge X
			rb.position = new Vector2(leftOrRight.x * sceneEdge, rigidbodyPosition.y);
			MoveDown();
		}
		// if passed next row height
		else if (rigidbodyPosition.y < nextRowHeight)
		{
			// return to row height Y
			rb.position = new Vector2(rigidbodyPosition.x, nextRowHeight);
			// and change nextRowHeight for all next part after this head before next head
			int i = myIndex;
			do
			{
				centipedeChain[i].nextRowHeight -= rowHeight;
				i++;
			} while (i < centipedeChain.Count && !centipedeChain[i].isHead);

			// change direction and move horizontally  
			ToLeftOrToRight();
		}
		RefreshHeadSprite();
	}

	// change direction of horizontal movement and move
	private void ToLeftOrToRight()
	{
		leftOrRight = -leftOrRight;
		rb.velocity = leftOrRight * movementSpeed;
	}

	private void MoveDown()
	{
		rb.velocity = Vector2.down * movementSpeed;
	}

	// check and set sprite in accordance to movement direction
	private void RefreshHeadSprite()
	{
		Sprite correctSprite = sr.sprite;
		Vector2 velocity = rb.velocity;
		if (velocity.x < -0.1f)
			correctSprite = headSprites.sprite1;
		else if (velocity.x > 0.1f)
			correctSprite = headSprites.sprite3;
		else if (velocity.y < -0.1f) correctSprite = headSprites.sprite2;
		if (sr.sprite != correctSprite) 
			sr.sprite = correctSprite;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (isHead && other.gameObject.GetComponent<Mushroom>()) 
			MoveDown();

		if (other.gameObject.GetComponent<Player>()) 
			GameManager.instance.LoseRound();
	}
	/************* head segments movement block ↑↑↑ ***********/


	/*************** body segments movement ↓↓↓ ****************/
	// defines when to start movement
	private void CheckDistance()
	{
		float distanceToLeader = Vector2.Distance(rb.position, centipedeChain[leaderIndex].rb.position);
		if (!(distanceToLeader > distanceBetweenParts)) return;
		// ReSharper disable once DelegateSubtraction
		MoveBody -= CheckDistance;
		MoveBody += Follow;
	}


	// follow forward centipede part
	private void Follow()
	{
		rb.position = centipedeChain[leaderIndex].previousPositions[0];
		centipedeChain[leaderIndex].previousPositions.RemoveAt(0);
	}
}