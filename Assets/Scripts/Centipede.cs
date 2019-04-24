using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Centipede : Destructible
{
	public float speed;
	public float nextRowHeight;
	public int scoreValue;

	internal List<Centipede> chain;

	// TODO delete "true" if test finished
	internal bool isHead = true;
	internal Vector2 direction = Vector2.right;

	private Rigidbody2D rb;
	private Transform tf;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		tf = GetComponent<Transform>();
		rb.velocity = -Vector2.up * speed;
	}

	internal override void ReceiveShot()
	{
		base.ReceiveShot();
		Destroy(gameObject);
	}

	private void FixedUpdate()
	{
		Move();
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
			nextRowHeight -= GameManager.instance.rowHeight;
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
		if (isHead) GoDown();
	}


	/*************** body segments movement ↓↓↓ ****************/
	private void Follow()
	{
	}
}