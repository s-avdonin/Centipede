using UnityEngine;

public class Shot : MonoBehaviour
{

	// flag if object has already hit smth. 
	private bool used;
	private float speed;
	private Rigidbody2D rb;
	private float speedMultiplier;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		speed = GameManager.instance.shotMovementSpeed;
		speedMultiplier = GameManager.instance.bonusManager.activeShotMultiplier;
		rb.velocity = speedMultiplier * speed * new Vector2(0f, 1f);
	}

	// give damage if can
	private void OnTriggerEnter2D(Collider2D other)
	{
		// stop function if already hit anything or object can not be hit 
		if (used || (other.gameObject.GetComponent<IDestructible>() == null)) return;
		// prevent double hit of close objects 
		used = true;
		other.gameObject.GetComponent<IDestructible>().ReceiveShot();
		Destroy(gameObject);
	}

	private void OnBecameInvisible()
	{
		Destroy(gameObject);
	}
}