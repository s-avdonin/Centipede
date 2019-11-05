using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ant : Foe
{
	private float heightOfCurrentRow;
	private float rowHeight;
	private float bottomBorder;

	protected override void Start()
	{
		base.Start();

		rowHeight = GameManager.instance.rowHeight;
		bottomBorder = -GameManager.instance.sceneEdge + rowHeight;
		float positionY = transform.position.y;
		heightOfCurrentRow = positionY - (positionY % rowHeight) - rowHeight;
	}

	private void Update()
	{
		Vector3 position = transform.position;

		if (position.y < heightOfCurrentRow && heightOfCurrentRow > bottomBorder)
		{
			if (Random.Range(0, 100) < foesManager.chanceForAntToCreateMushroom)
			{
				Instantiate(GameManager.instance.mushroomPrefab, new Vector3(position.x, heightOfCurrentRow),
					Quaternion.identity);
			}
			heightOfCurrentRow -= rowHeight;
		}
	}

	protected override void SetStartPosition()
	{
		// between scene edges
		Vector2 position = grid[grid.GetLength(0) - 1, Random.Range(2, grid.GetLength(1)) - 1];
		transform.position = position;
	}
}