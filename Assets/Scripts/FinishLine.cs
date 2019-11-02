using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
	// flag to prevent double usage of this object
	private bool used = false;

	// processing entering centipede into this collider
	private void OnTriggerEnter2D(Collider2D other)
	{
		// if entered object is a centipede and this function called for the first time
		if (other.GetComponent<Centipede>() && !used)
		{
			used = true;
			GameManager.instance.LoseRound();
		}
	}
}