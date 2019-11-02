using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
	// interval between two shots, if button is kept pressed
	public float timeToRepeatShot;
	public Shot shot;
	public GameObject deathAnimation;

	private Rigidbody2D rb;
	private float speed;

	// time passed from game load when shot was fired last time 
	private float lastShotTime;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		GameManager.instance.LoseRound += Killed;
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
		// reading is fire pressed once or hold on for certain time
		if (Input.GetButtonDown("Fire1") || Input.GetButton("Fire1") && Time.time - lastShotTime > timeToRepeatShot)
		{
			lastShotTime = Time.time;
			Instantiate(shot, rb.position + new Vector2(0f, 0.1f), Quaternion.identity);
		}
	}

	public void Killed()
	{
		Instantiate(deathAnimation, transform.position, transform.rotation);
		Destroy(gameObject);
	}

}