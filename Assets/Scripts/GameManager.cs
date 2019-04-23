
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class Dispersion
{ 
	public int fromValue, toValue;

	public Dispersion(int min, int max)
	{
		if (min > max)
		{
			int temp = min;
			this.fromValue = max;
			this.toValue = min;
		}
	}
}

public class GameManager : MonoBehaviour
{
	// ReSharper disable once MemberCanBePrivate.Global
	public static GameManager instance = null;
	public int centipedeSize;
	public float centipedeSpeed;
	public Dispersion mushroomsQty;
	public Dispersion rowsAvailableForMushrooms;
	public Mushroom mushroom;
	public Text lifeText;
	public Text scoreText;
	public Text gameOverText;
	public float timeToReload;
	public float sceneEdge;
	public Borders shipMovementBorders;
	public float shipSpeed; 

	private int life = 4;
	private int score = 0;

	private List<Vector2> mushroomsGrid;

	// Start is called before the first frame update
	void Awake()
	{
		instance = this;
		SetMushrooms();
	}

	private void Start()
	{
		AddScore(PlayerPrefs.GetInt("Score"));
		SetLife();
	}

	private void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			ReloadLevel();
		}
	}

	private void SetLife()
	{
		int lifeSaved = PlayerPrefs.GetInt("Life");
		if (lifeSaved > 0)
		{
			ChangeLifeCount(PlayerPrefs.GetInt("Life"));
		}
	}


	private void SetMushrooms()
	{
		mushroomsGrid = GetGrid();
		int qty = Random.Range(mushroomsQty.fromValue - 1, mushroomsQty.toValue);
		for (int i = 0; i < qty; i++)
		{
			MushroomAtRandomPosition();
		}
	}

	private void MushroomAtRandomPosition()
	{
		int randomIndex = Random.Range(0, mushroomsGrid.Count);
		Instantiate(mushroom, mushroomsGrid[randomIndex], Quaternion.identity);
		mushroomsGrid.RemoveAt(randomIndex);
	}

	private List<Vector2> GetGrid()
	{
		List<Vector2> coords = new List<Vector2>();
		for (float i = -sceneEdge; i <= sceneEdge; i += 0.25f)
		{
			for (float j = -sceneEdge + (rowsAvailableForMushrooms.fromValue * 0.25f);
				j <= sceneEdge - (rowsAvailableForMushrooms.toValue * 0.25f);
				j += 0.25f)
			{
				coords.Add(new Vector2(i, j));
			}
		}

		return coords;
	}

	public void AddScore(int value)
	{
		score += value;
		scoreText.text = "score: " + score;
	}

	public void ChangeLifeCount(bool trueWillAdd)
	{
		if (!trueWillAdd) life--;
		else life++;

		PrintLife();

		if (life < 1)
		{
			LoseGame();
		}
		else if (!trueWillAdd && life > 0)
		{
			Invoke(nameof(ReloadLevel), timeToReload);
		}
	}


	private void ChangeLifeCount(int setValue)
	{
		life = setValue;
		PrintLife();
	}

	private void PrintLife()
	{
		string addText = "";

		for (int i = 0; i < life; i++)
		{
			addText += "♥ ";
		}

		lifeText.text = "life: " + addText;
	}

	private void LoseGame()
	{
		gameOverText.text = "GAME OVER";
		PlayerPrefs.SetInt("Score", 0);
		gameOverText.gameObject.SetActive(true);
	}

	internal void WinRound()
	{
		gameOverText.text = "You've won!\nGet ready for the next round";
		gameOverText.gameObject.SetActive(true);
		Invoke(nameof(ReloadLevel), 5f);
	}
	
	

	private void ReloadLevel()
	{
		PlayerPrefs.SetInt("Score", score);
		PlayerPrefs.SetInt("Life", life);
		SceneManager.LoadScene("MainScene");
	}
}
