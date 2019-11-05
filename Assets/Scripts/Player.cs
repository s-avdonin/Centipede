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
	private float sceneEdge;
	private float topBorder;

	// time passed after game load when shot was fired last time 
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
		sceneEdge = GameManager.instance.sceneEdge;
		topBorder = GameManager.instance.topBorderForPlayer;
	}

	// player movement
	private void FixedUpdate()
	{
		// reading controls 
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		Vector2 movement = new Vector2(moveHorizontal, moveVertical);
		rb.velocity = movement * speed;
		// stop player if out of borders
		rb.position = new Vector2
			(
			Mathf.Clamp(rb.position.x, -sceneEdge, sceneEdge),
			Mathf.Clamp(rb.position.y, -sceneEdge, topBorder)
			);
	}

	private void Update()
	{
		// reading is fire pressed once or is hold
		// ReSharper disable once InvertIf
		if (((Input.GetButtonDown("Fire1") || Input.GetButton("Fire1")) && shot == null) &&
			Time.time - lastShotTime > timeToRepeatShot)
		{
			lastShotTime = Time.time;
			shot = Instantiate(shotPrefab, rb.position + new Vector2(0f, 0.1f), Quaternion.identity);
		}
	}

	private void Killed()
	{
		Transform tf = transform;
		Instantiate(deathAnimation, tf.position, tf.rotation);
		Destroy(gameObject);
	}
}