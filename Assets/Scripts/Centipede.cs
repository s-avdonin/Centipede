using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Centipede : Destructible
{
	public int scoreValue;
	public Sprite headSprite;
	public Mushroom mushroom;
	public float distanceBetween;
	public float nextRowHeight;

	internal List<Centipede> chain;
	internal bool isHead;
	internal Vector2 direction = Vector2.right;
	internal List<Vector2> positionsList;

	private float speed;
	internal Rigidbody2D rb;
	private Transform tf;
	private int leaderIndex = 0;
	private int myIndexInChain = 0;

	private SpriteRenderer sr;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		tf = GetComponent<Transform>();
		sr = GetComponent<SpriteRenderer>();
		positionsList = new List<Vector2>();
	}

	private void Start()
	{
		chain = GameManager.instance.chain;
		speed = GameManager.instance.centipedeSpeed;
		if (isHead)
		{
			rb.velocity = -Vector2.up * speed;
		}
		else
		{
			FindMyIndex();
		}
	}

	internal void FindMyIndex()
	{
		myIndexInChain = chain.IndexOf(this);
		leaderIndex = (myIndexInChain == 0) ? 0 : (myIndexInChain - 1);
	}

	internal override void ReceiveShot()
	{
		base.ReceiveShot();

		if (myIndexInChain + 1 < chain.Count)
		{
			Centipede next = chain[myIndexInChain + 1];
			// set next as head
			next.SetHead();
			next.StartMovement(rb.position);
		}

		Instantiate(mushroom, tf.position, Quaternion.identity);
		// add score 
		GameManager.instance.AddScore(scoreValue);
		// removing self from list
		chain.RemoveAt(myIndexInChain);
		// refresh indices
		foreach (Centipede centipede in chain)
		{
			centipede.FindMyIndex();
		}
		GameManager.instance.CheckWin(1f);
		Destroy(gameObject);
	}

	internal void StartMovement(Vector2 targetPosition)
	{
		Vector2 nextStep = targetPosition - rb.position;
		float x = (nextStep.x < 0 ? -1 : 1);
		direction = new Vector2(x, 0);
		if (Mathf.Abs(nextStep.x) > Mathf.Abs(nextStep.y))
		{
			tf.Rotate(0, 0, direction.x * 90f);
			rb.velocity = direction * speed;
		}
		else
		{
			rb.velocity = Vector2.down * speed;
		}
	}

	internal void SetHead()
	{
		isHead = true;
		sr.sprite = headSprite;
	}

	private void FixedUpdate()
	{
		Move();
		SavePosition();
		// todo check and change rotation
	}

	
	private void SavePosition()
	{
		// if last → do not save
		if (myIndexInChain == chain.Count - 1) return;

		if (positionsList.Count == 0) positionsList.Add(rb.position);
		else if (positionsList[positionsList.Count - 1] != rb.position) positionsList.Add(rb.position);
	}

	/************* head segments movement ↓↓↓ ***********/

	private void Move()
	{
		if (isHead) MoveHead();
		else Follow();
	}

	private void MoveHead()
	{
		if (Mathf.Abs(rb.position.x) > GameManager.instance.sceneEdge)
		{
			rb.position = new Vector2(direction.x * GameManager.instance.sceneEdge, rb.position.y);
			GoDown();
		}
		else if (rb.position.y < nextRowHeight)
		{
			rb.position = new Vector2(rb.position.x, nextRowHeight);
			// change nextRowHeight for all next part after this head before next head
			int i = myIndexInChain;
			do
			{
				chain[i].nextRowHeight -= GameManager.instance.rowHeight;
				i++;
			} while (i < chain.Count && !chain[i].isHead);
			
			ToLeftOrToRight();
		}
	}
	
	private void ToLeftOrToRight()
	{
		direction = -direction;
		tf.Rotate(0, 0, direction.x * 90f);
		rb.velocity = direction * speed;
	}

	private void GoDown()
	{
		if (Math.Abs(tf.rotation.z) > float.Epsilon)
		{
			tf.Rotate(0, 0, -direction.x * 90f);
		}

		rb.velocity = Vector2.down * speed;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (isHead && other.gameObject.GetComponent<Mushroom>())
		{
			GoDown();
		}
	}


	/*************** body segments movement ↓↓↓ ****************/
	private void Follow()
	{
		float distanceToLeader = Vector2.Distance(rb.position, chain[leaderIndex].rb.position);
		if (distanceToLeader > distanceBetween)
		{
			rb.position = chain[leaderIndex].positionsList[0];
			chain[leaderIndex].positionsList.RemoveAt(0);
		}
	}
}