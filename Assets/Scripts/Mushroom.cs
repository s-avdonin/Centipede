using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// convenient class to store some three sprites
[System.Serializable]
public class ThreeSprites
{
	public Sprite sprite1, sprite2, sprite3;
}

public class Mushroom : Destructible
{
	// points given to player for this object's destruction
	public int scoreValue;
	// sprites of this object's damaged 
	public ThreeSprites threeSprites;

	// reference to this object's SpriteRenderer
	private SpriteRenderer sr;
	// value of damage taken by this mushroom
	private int damage = 0;

	private void Awake()
	{
		// set reference
		sr = GetComponent<SpriteRenderer>();
	}

	// process receiving damage from shot
	internal override void ReceiveShot()
	{
		// log message
		base.ReceiveShot();
		// increase damage count
		damage++;
		// change sprite accordingly to damage taken
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
				// give points to player
				GameManager.instance.AddScore(scoreValue);
				Destroy(gameObject);
				break;
		}
	}
}