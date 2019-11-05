using UnityEngine;

[System.Serializable]
public class ThreeSprites
{
	public Sprite sprite1, sprite2, sprite3;
}

public class Mushroom : MonoBehaviour, IDestructible
{
	public int scoreValue;
	public ThreeSprites damagedSprites;
	private SpriteRenderer sr;
	private int damage;

	private void Awake()
	{
		transform.SetParent(GameManager.instance.mushroomsParent.transform);
		sr = GetComponent<SpriteRenderer>();
	}

	public void ReceiveShot()
	{
		damage++;
		switch (damage)
		{
			case 1:
				sr.sprite = damagedSprites.sprite1;
				break;
			case 2:
				sr.sprite = damagedSprites.sprite2;
				break;
			case 3:
				sr.sprite = damagedSprites.sprite3;
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