using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
	public Shot shot;
	
	private Rigidbody2D rb;
	private float speed;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		speed = GameManager.instance.shipSpeed;
	}

	private void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		Vector2 movement = new Vector2(moveHorizontal, moveVertical);
		rb.velocity = movement * speed;

		rb.position = new Vector2
			(
			Mathf.Clamp(rb.position.x, -GameManager.instance.sceneEdge, GameManager.instance.sceneEdge),
			Mathf.Clamp(rb.position.y, -GameManager.instance.sceneEdge, 0f)
			);
	}

	private void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			Instantiate(shot, rb.position + new Vector2(0f, 0.1f), Quaternion.identity);
		}
	}

	private void OnBecameInvisible()
	{
		Destroy(gameObject);
	}
}