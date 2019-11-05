using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public class Dispersion
{
	public int lowValue, highValue;

	public Dispersion(int min, int max)
	{
		if (min < max) return;
		this.lowValue = max;
		this.highValue = min;
	}
}

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public Vector2[,] grid { get; private set; }
	public int centipedeSize;
	public float centipedeSpeed;
	public int mushroomsQty;
	public Dispersion rowsWithoutMushrooms;
	public Player playerPrefab;
	public Mushroom mushroomPrefab;
	public float rowHeight;
	public Text lifeText;
	public Text scoreText;
	public Text roundText;
	public Text centerMessageText;
	public float timeToReload;
	public float sceneEdge;
	public float topBorderForPlayer;
	public float playerMovementSpeed;
	public float shotMovementSpeed;
	public int startLifeCount;
	public Vector2 startChainPosition;
	public Centipede centipedePrefab;
	public Button restartButton;
	public GameObject mushroomsParent;
	public GameObject centipedeParent;
	public BonusManager bonusManager;
	public FoesManager foesManager;

	internal List<Centipede> centipedeChain;
	internal Action LoseRound = () => { };

	private int life;
	private int score;
	private int round = 1;
	private int maxMushrooms;
	private bool isPaused;

	void Awake()
	{
		instance = this;
		centipedeChain = new List<Centipede>();
		LoseRound += StopCentipede;
		LoseRound += ReduceLife;
		GetGrid();
		SetRound();
		SetMushrooms();
		SetCentipedeChain();
		Instantiate(playerPrefab);
		// load and show score and lives from previous round
		AddScore(PlayerPrefs.GetInt("Score"));
		SetLife(PlayerPrefs.GetInt("Life"));
	}

	private void Update()
	{
		if (Input.GetButtonDown("Cancel")) SetPause();
	}

	
	internal static bool HasMushroom(Vector2 point)
	{
		// select all colliders at defined point
		// ReSharper disable once Unity.PreferNonAllocApi
		var colliders = Physics2D.OverlapPointAll(point);
		// select mushrooms' colliders among others
		var mushroomColliders = colliders.Where(col => (col.gameObject.GetComponent<Mushroom>() != null));
		return mushroomColliders.Any();
	}

	private void SetPause()
	{
		if (!isPaused)
		{
			centerMessageText.text = "PAUSED";
			centerMessageText.gameObject.SetActive(true);
			Time.timeScale = 0;

			isPaused = true;
		}
		else
		{
			centerMessageText.gameObject.SetActive(false);
			Time.timeScale = 1;
			isPaused = false;
		}
	}

	private void SetMushrooms()
	{
		mushroomsQty = TrimToMax(mushroomsQty, maxMushrooms);
		for (int i = 0; i < mushroomsQty; i++)
		{
			Vector3 coordinates = grid[
				Random.Range(
					1 + rowsWithoutMushrooms.lowValue,
					grid.GetLength(0) - rowsWithoutMushrooms.highValue),
				Random.Range(1, grid.GetLength(1) - 1)];
			if (HasMushroom(coordinates))
			{
				i--;
				continue;
			}
			Instantiate(mushroomPrefab, coordinates, Quaternion.identity);
		}
	}

	private int TrimToMax(int valueToCheck, int maximum)
	{
		if (valueToCheck > maximum) return maximum;
		return valueToCheck;
	}

	private void SetCentipedeChain()
	{
		for (int i = 0; i < centipedeSize; i++)
		{
			Centipede centipede = Instantiate(centipedePrefab, startChainPosition, Quaternion.identity);
			centipedeChain.Add(centipede);
			centipede.transform.SetParent(centipedeParent.transform);
		}

		centipedeChain[0].MarkAsHead();
	}

	// save all available points into array
	private void GetGrid()
	{
		// count available rows and columns plus screen edges
		int count = Convert.ToInt32(sceneEdge * 2 / rowHeight) + 3;
		grid = new Vector2[count, count];
		float horizontal = -sceneEdge - rowHeight;
		float vertical = horizontal;

		for (int i = 0; i < count; i++, vertical += rowHeight)
		{
			for (int j = 0; j < count; j++, horizontal += rowHeight) 
				grid[i, j] = new Vector2(horizontal, vertical);

			horizontal = -sceneEdge - rowHeight;
		}

		// set maximum of mushrooms which could be instantiated
		maxMushrooms =
			((count - rowsWithoutMushrooms.lowValue - rowsWithoutMushrooms.highValue) * (count - 2)) / 3;
	}


	public void AddScore(int value)
	{
		score += value;
		scoreText.text = "Score: " + score;
	}

	private void ReduceLife()
	{
		life--;
		if (life < 1)
		{
			LoseGame();
		}
		else // if lives left → restart current level
		{
			centerMessageText.text = "Try harder!";
			centerMessageText.gameObject.SetActive(true);
			Invoke(nameof(NewRound), timeToReload);
		}
	}

	private void SetLife(int setValue)
	{
		// set default if zero (for the start of new game)
		life = (setValue < 1) ? startLifeCount : setValue;
		PrintLife();
	}

	private void PrintLife()
	{
		string addText = "";
		for (int i = 0; i < life; i++) addText += "♥ ";

		lifeText.text = addText;
	}

	// set current round and dependent params: speed and length of centipede and mushrooms quantity
	private void SetRound()
	{
		round = PlayerPrefs.GetInt("Round", round);
		roundText.text = "Round " + round;
		centipedeSpeed += round * 0.05f;
		centipedeSize += round / 3;
		mushroomsQty += round * 3;
	}

	private void StopCentipede()
	{
		foreach (Centipede part in centipedeChain)
		{
			part.enabled = false;
			if (part.isHead)
				part.rb.velocity = Vector2.zero;
		}
	}

	private void LoseGame()
	{
		centerMessageText.text = "GAME OVER\nYour score is " + score;
		restartButton.gameObject.SetActive(true);
		// set defaults
		PlayerPrefs.SetInt("Life", startLifeCount);
		PlayerPrefs.SetInt("Score", 0);
		PlayerPrefs.SetInt("Round", 1);
		centerMessageText.gameObject.SetActive(true);
	}
	
	internal void CheckWin(float delayTime)
	{
		Invoke(nameof(CheckWin), delayTime);
	}

	private void CheckWin()
	{
		if (centipedeChain.Count == 0) WinRound();
	}


	internal void WinRound()
	{
		centerMessageText.text = "You've won!\nGet ready for the next round";
		centerMessageText.gameObject.SetActive(true);
		PlayerPrefs.SetInt("Round", ++round);
		Invoke(nameof(NewRound), timeToReload);
	}

	// start a new game after lose (assigned as a button onClick function)
	public void Restart()
	{
		PlayerPrefs.SetInt("Score", 0);
		ReloadLevel();
	}

	private void NewRound()
	{
		PlayerPrefs.SetInt("Score", score);
		PlayerPrefs.SetInt("Life", life);
		ReloadLevel();
	}

	private void ReloadLevel()
	{
		SceneManager.LoadScene("MainScene");
	}
}