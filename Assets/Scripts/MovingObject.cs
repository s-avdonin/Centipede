using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
	public float movementSpeed;

	protected Rigidbody2D rb;

	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		Move();
	}

	protected virtual void Move()
	{
		rb.velocity = Vector2.down * movementSpeed;
	}
}