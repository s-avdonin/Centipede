using System.Collections;
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
	public float rowHeight;
	public Text lifeText;
	public Text scoreText;
	public Text centerMessageText;
	public float timeToReload;
	public float sceneEdge;
	public float shipSpeed;
	public int startLife;
	public Vector2 startChainPosition;
	public int centipedeChainLength;
	public Centipede centipede;

	internal List<Centipede> chain;

	private int life;
	private int score = 0;

	private List<Vector2> mushroomsGrid;

	// Start is called before the first frame update
	void Awake()
	{
		instance = this;
		chain = new List<Centipede>();
		SetMushrooms();
		SetCentipedeChain();
	}

	private void Start()
	{
		AddScore(PlayerPrefs.GetInt("Score"));
		ChangeLifeCount(PlayerPrefs.GetInt("Life"));
	}

	private void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			ReloadLevel();
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

	private void SetCentipedeChain()
	{
		// chain = new List<Centipede>();
		for (int i = 0; i < centipedeSize; i++)
		{
			chain.Add(Instantiate(centipede, startChainPosition, Quaternion.identity));
		}

		chain[0].SetHead();
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
		for (float i = -sceneEdge; i <= sceneEdge; i += rowHeight)
		{
			for (float j = -sceneEdge + (rowsAvailableForMushrooms.fromValue * rowHeight);
				j <= sceneEdge - (rowsAvailableForMushrooms.toValue * rowHeight);
				j += rowHeight)
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
		if (trueWillAdd)
			life++;
		else
		{
			life--;
			if (life < 1)
			{
				LoseGame();
			}
			else
			{
				Invoke(nameof(ReloadLevel), timeToReload);
			}
		}

		PrintLife();
	}


	private void ChangeLifeCount(int setValue)
	{
		life = setValue < 1 ? startLife : setValue;
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
		centerMessageText.text = "GAME OVER";
		PlayerPrefs.SetInt("Life", startLife);
		PlayerPrefs.SetInt("Score", 0);
		centerMessageText.gameObject.SetActive(true);
	}

	internal void WinRound()
	{
		centerMessageText.text = "You've won!\nGet ready for the next round";
		centerMessageText.gameObject.SetActive(true);
		Invoke(nameof(ReloadLevel), 5f);
	}


	private void ReloadLevel()
	{
		PlayerPrefs.SetInt("Score", score);
		PlayerPrefs.SetInt("Life", life);
		SceneManager.LoadScene("MainScene");
	}
}