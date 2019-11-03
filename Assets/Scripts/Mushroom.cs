using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// convenient class to store any three sprites
[System.Serializable]
public class ThreeSprites
{
	public Sprite sprite1, sprite2, sprite3;
}

public class Mushroom : MonoBehaviour, IDestructible
{
	public int scoreValue;

	// sprites of this object's damaged 
	public ThreeSprites threeSprites;
	private SpriteRenderer sr;
	private int damage = 0;

	private void Awake()
	{
		transform.SetParent(GameManager.instance.mushroomsParent.transform);
		// set reference
		sr = GetComponent<SpriteRenderer>();
	}

	public void ReceiveShot()
	{
		damage++;
		switch (damage)
		{
			case 1:
				sr.sprite = threeSprites.sprite1;
				break;
			case 2:
				sr.sprite = threeSprites.sprite2;
				break;
			case 3:
				sr.sprite = threeSprites.sprite3;
				break;
			// max damage → object destroyed
			case 4:
				GameManager.instance.AddScore(scoreValue);
				// call for bonuses
				GameManager.instance.bonusManager.DropBonus(GetComponent<Rigidbody2D>().position);
				Destroy(gameObject);
				break;
		}
	}
}