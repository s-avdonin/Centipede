using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spider : Foe
{
	private float sceneEdge;
	private float topBorder;
	private List<float> possibleHeights;
	private float rowHeight;
	private float lastRowHeight;
	private int chanceToChangeDirection;
	private int chanceToEatMushroom;
	private Vector2 target;
	private Vector2 currentPosition;
	private Vector2 currentDirection;
	private float step;
	private readonly Vector2[] directions =
	{
		Vector2.down,
		Vector2.up,
		-Vector2.one,
	};

	protected override void Start()
	{
		sceneEdge = GameManager.instance.sceneEdge;
		topBorder = GameManager.instance.topBorderForPlayer;
		rowHeight = GameManager.instance.rowHeight;
		step = movementSpeed * Time.deltaTime;
		base.Start();
		chanceToChangeDirection = foesManager.chanceForSpiderToChangeDirection;
		chanceToEatMushroom = foesManager.chanceForSpiderToEatMushroom;
	}

	protected override void OnCollisionEnter2D(Collision2D other)
	{
		base.OnCollisionEnter2D(other);
		if (other.transform.GetComponent<Mushroom>() && Random.Range(0, 100) < chanceToEatMushroom)
		{
			Destroy(other.gameObject);
		}
	}

	protected override void StartMovement()
	{
		currentDirection = Vector2.down;
		target = (Vector2) transform.position + currentDirection;
	}

	protected override void SetStartPosition()
	{
		possibleHeights = foesManager.possibleHeightsForSpider;
		// set left or right screen edge randomly
		directions[2].x = (Random.Range(0, 2) == 0) ? 1 : -1;
		lastRowHeight = possibleHeights[Random.Range(0, possibleHeights.Count)];
		transform.position = new Vector3(
			(sceneEdge + rowHeight) * -directions[2].x,
			lastRowHeight);
	}
	
	private void FixedUpdate()
	{
		currentPosition = transform.position;
		Move();
	}

	private void Move()
	{
		transform.position = Vector2.MoveTowards(currentPosition, target, step);
		CheckHeight();
	}

	private void CheckHeight()
	{
		if (currentPosition.y < -sceneEdge)
		{
			
			ChangeDirection(Vector2.up);
			return;
		}

		if (currentPosition.y > topBorder - rowHeight)
		{
			ChangeDirection(Vector2.down);
			return;
		}
		// every row height passed → try to change movement direction
		if (currentPosition.y - lastRowHeight > rowHeight || lastRowHeight - currentPosition.y > rowHeight)
		{
			lastRowHeight = currentPosition.y;
			if (Random.Range(0, 100) < chanceToChangeDirection) 
				ChangeDirection(directions[Random.Range(0, directions.Length)]);
			// if failed to change → continue further
			else target += currentDirection;
		}
	}

	private void ChangeDirection(Vector2 direction)
	{
		currentDirection = direction;
		target = currentPosition + currentDirection; 
	}
}