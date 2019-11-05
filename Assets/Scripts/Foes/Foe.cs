using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Foe : MovingObject, IDestructible
{
	public int scoreValue;

	protected Vector2[,] grid;
	protected FoesManager foesManager;

	protected virtual void Start()
	{
		transform.SetParent(GameManager.instance.foesManager.transform);
		grid = GameManager.instance.grid;
		foesManager = GameManager.instance.foesManager;
		SetStartPosition();
		StartMovement();
	}

	protected abstract void SetStartPosition();

	public void ReceiveShot()
	{
		GameManager.instance.AddScore(scoreValue);
		Destroy(gameObject);
	}

	protected virtual void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.GetComponent<Player>())
		{
			GameManager.instance.LoseRound();
		}
	}

	protected void OnBecameInvisible()
	{
		Destroy(gameObject);
	}
}