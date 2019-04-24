using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
	public float speed;
	
	private Rigidbody2D rb;
	// flag if object has already hit smth. 
	private bool used = false;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = new Vector2(0f, 1f) * speed;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (used || !other.gameObject.GetComponent<Destructible>()) return;
		// prevent double hit of close objects 
		used = true;
		other.gameObject.GetComponent<Destructible>().ReceiveShot();
		Destroy(gameObject);
	}

}
