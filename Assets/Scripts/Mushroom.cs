using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ThreeSprites
{
	public Sprite sprite1, sprite2, sprite3;
}

public class Mushroom : Destructible
{
	public int scoreValue;
	public ThreeSprites threeSprites;

	private SpriteRenderer sr;
	private int damage = 0;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	internal override void ReceiveShot()
	{
		base.ReceiveShot();
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
			case 4:
				GameManager.instance.AddScore(scoreValue);
				Destroy(gameObject);
				break;
		}
	}
}