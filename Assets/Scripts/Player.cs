﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	// interval between two shots, if button is kept pressed
	public float timeToRepeatShot;
	public Shot shotPrefab;
	public GameObject deathAnimation;

	internal List<Bonus> bonuses = new List<Bonus>();
	internal float speed;

	private Rigidbody2D rb;
	private Shot shot;

	// time passed from game load when shot was fired last time 
	private float lastShotTime;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		GameManager.instance.LoseRound += Killed;
		GameManager.instance.bonusManager.player = this;
	}

	private void Start()
	{
		speed = GameManager.instance.playerMovementSpeed;
	}

	// ship movement
	private void FixedUpdate()
	{
		// reading controls 
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		// saving controls as Vector2 directions
		Vector2 movement = new Vector2(moveHorizontal, moveVertical);
		// set velocity in movement direction with movement speed
		rb.velocity = movement * speed;

		// stop ship if out of borders
		rb.position = new Vector2
			(
			Mathf.Clamp(rb.position.x, -GameManager.instance.sceneEdge, GameManager.instance.sceneEdge),
			Mathf.Clamp(rb.position.y, -GameManager.instance.sceneEdge, 0f)
			);
	}

	private void Update()
	{
		// reading is fire pressed once or hold on for certain time
		if (((Input.GetButtonDown("Fire1") || Input.GetButton("Fire1")) && shot == null) &&
			Time.time - lastShotTime > timeToRepeatShot)
		{
			lastShotTime = Time.time;
			shot = Instantiate(shotPrefab, rb.position + new Vector2(0f, 0.1f), Quaternion.identity);
		}
	}

	private void Killed()
	{
		Instantiate(deathAnimation, transform.position, transform.rotation);
		Destroy(gameObject);
	}
}