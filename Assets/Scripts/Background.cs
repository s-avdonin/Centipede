using UnityEngine;

public class Background : MonoBehaviour
{
	public GameObject[] backgroundTiles;

	void Start()
	{
		foreach (Vector2 position in GameManager.instance.grid)
		{
			GameObject temp = Instantiate(backgroundTiles[Random.Range(0, backgroundTiles.Length)],
				new Vector3(position.x, position.y),
				Quaternion.identity);
			temp.transform.SetParent(transform);
		}
	}
}