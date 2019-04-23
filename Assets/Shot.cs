using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
	public float speed;
	
	private Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = new Vector2(0f, 1f) * speed;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.GetComponent<Destructible>())
		{
			other.gameObject.GetComponent<Destructible>().ReceiveShot();
		}
	}
}
