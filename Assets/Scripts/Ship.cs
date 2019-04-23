using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Borders
{
	public float xMin, xMax, yMin, yMax;
}

public class Ship : MonoBehaviour
{
	private Rigidbody2D rb;
	private float speed;
	private Borders borders;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		speed = GameManager.instance.shipSpeed;
		borders = GameManager.instance.shipMovementBorders;
	}

	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		Vector2 movement = new Vector2(moveHorizontal, moveVertical);
		rb.velocity = movement * speed;

		rb.position = new Vector2
			(
			Mathf.Clamp(rb.position.x, borders.xMin, borders.xMax),
			Mathf.Clamp(rb.position.y, borders.yMin, borders.yMax)
			);
	}
}