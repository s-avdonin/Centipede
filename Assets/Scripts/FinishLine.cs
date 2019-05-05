using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 
public class FinishLine : MonoBehaviour
{
	// reference to a ship object
	public Ship ship;
	// dead ship prefab
	public GameObject deadShip;

	// flag to prevent double usage of this object
	private bool used = false;

	// processing entering centipede into this collider
	private void OnTriggerEnter2D(Collider2D other)
	{
		// if entered object is centipede and this function called for the first time
		if (other.GetComponent<Centipede>() && !used)
		{
			// set flag of this object usage
			used = true;
			// deactivate ship object
			ship.gameObject.SetActive(false);
			// instantiate dead ship prefab
			Instantiate(deadShip, ship.transform.position, Quaternion.identity);
			// stop all centipede's parts
			GameManager.instance.StopAll();
			// reduce life by one
			GameManager.instance.ChangeLifeCount(false);
		}
	}
}