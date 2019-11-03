using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
	public GameObject[] backgroundTiles;

	// Start is called before the first frame update
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

	// Update is called once per frame
	void Update()
	{
	}
}