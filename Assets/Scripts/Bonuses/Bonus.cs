using UnityEngine;

public abstract class Bonus : MovingObject
{
	protected void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.GetComponent<Player>()) return;
		
		ActivateInBonusManager();
		Destroy(gameObject);
	}

	protected abstract void ActivateInBonusManager();
}