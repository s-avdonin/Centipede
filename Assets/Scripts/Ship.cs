using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
	// interval between two shots, if button is kept pressed
	public float timeToRepeatShot;
	// prefab of a shot
	public Shot shot;
	
	private Rigidbody2D rb;
	// ship's movement speed
	private float speed;
	// time passed from game load when shot was fired last time 
	private float lastShotTime;

	private void Awake()
	{
		Debug.Log("Awake called");
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		Debug.Log("Start called");
		// reading ship speed from Game Manager
		speed = GameManager.instance.shipSpeed;
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

	// shooting
	private void Update()
	{
		Debug.Log("Update called");
		// reading is fire pressed once or hold on for certain time
		if (Input.GetButtonDown("Fire1") || Input.GetButton("Fire1") && Time.time -lastShotTime > timeToRepeatShot)
		{
			// save current time as last shot time
			lastShotTime = Time.time;
			// create new shot in front of a ship
			Instantiate(shot, rb.position + new Vector2(0f, 0.1f), Quaternion.identity);
		}
	}

	// if shot leaves scene it has to be destroyed
	private void OnBecameInvisible()
	{
		Destroy(gameObject);
	}
}