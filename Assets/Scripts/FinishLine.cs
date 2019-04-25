using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
	public Ship ship;
	public GameObject deadShip;

	private bool used = false;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<Centipede>() && !used)
		{
			used = true;
			ship.gameObject.SetActive(false);
			Instantiate(deadShip, ship.transform.position, Quaternion.identity);
			GameManager.instance.StopAll();
			GameManager.instance.ChangeLifeCount(false);
		}
	}
}