using UnityEngine;

public class FinishLine : MonoBehaviour
{
	// flag to prevent double usage of this object
	private bool used = false;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.GetComponent<Centipede>() || used) return;
		used = true;
		GameManager.instance.LoseRound();
	}
}