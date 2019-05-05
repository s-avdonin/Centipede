using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
	// shot movement speed
	public float speed;
	
	private Rigidbody2D rb;
	// flag if object has already hit smth. 
	private bool used = false;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		// immediate movement start
		rb.velocity = new Vector2(0f, 1f) * speed;
	}

	// give damage if can
	private void OnTriggerEnter2D(Collider2D other)
	{
		// stop function if already hit anything or object can not be hit 
		if (used || !other.gameObject.GetComponent<Destructible>()) return;
		// prevent double hit of close objects → set flag 
		used = true;
		// if object can be hit call it's function of receiving shot
		other.gameObject.GetComponent<Destructible>().ReceiveShot();
		// destroy this projectile
		Destroy(gameObject);
	}

	// if leaves borders of screen must be destroyed
	private void OnBecameInvisible()
	{
		Destroy(gameObject);
	}
}
