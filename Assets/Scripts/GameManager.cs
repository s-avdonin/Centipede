using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// limit int values in range
[System.Serializable]
public class Dispersion
{
	// fromValue - min, toValue - max
	public int fromValue, toValue;

	// swap if min > max
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

// main game logic class
public class GameManager : MonoBehaviour
{
	// public reference to current instance of GameManager 
	public static GameManager instance = null;
	// length of centipede (number of parts)
	public int centipedeSize;
	// speed of centipede movement
	public float centipedeSpeed;
	// range of mushrooms number to instantiate
	public Dispersion mushroomsQty;
	// range were mushrooms can be instantiated
	public Dispersion rowsAvailableForMushrooms;
	// prefab of mushroom
	public Mushroom mushroom;
	// the height of each row where mushrooms will be set
	public float rowHeight;
	// reference to Text object that indicates count of lives
	public Text lifeText;
	// reference to Text object that indicates points score
	public Text scoreText;
	// reference to Text object that indicates current round
	public Text roundText;
	// reference to Text object that show messages at the center of the screen
	public Text centerMessageText;
	// time before loading next round after win or lose this round
	public float timeToReload;
	// edges of square borders where all game is going to take place
	public float sceneEdge;
	// ship movement speed 
	public float shipSpeed;
	// lives number at the start of new game
	public int startLife;
	// where from centipedes are going to start movement
	public Vector2 startChainPosition;
	// prefab of centipede part
	public Centipede centipede;
	// reference to restart button
	public Button restartButton;
	// centipede parts list
	internal List<Centipede> chain;
	// current life and score
	private int life;
	private int score = 0;
	// list of positions where mushrooms can be set
	private List<Vector2> mushroomsGrid;
	
	// todo implement levels with growing centipede speed
	private int round = 1;

	void Awake()
	{
		instance = this;
		chain = new List<Centipede>();
		// set current round
		SetRound();
		// set mushrooms within the game field
		SetMushrooms();
		// create chain of centipede parts
		SetCentipedeChain();
		// load and show score from previous round
		AddScore(PlayerPrefs.GetInt("Score"));
		// load and show lives count from previous round
		ChangeLifeCount(PlayerPrefs.GetInt("Life"));
	}

	private void Update()
	{
		// todo Pause on Esc
	}

	private void Start()
	{
	}

	//************ for testing only ***********	
	// private void Update()
	// {
	// 	if (Input.GetButtonDown("Cancel"))
	// 	{
	// 		ReloadLevel();
	// 	}
	// }

	// set mushrooms within the game field
	private void SetMushrooms()
	{
		// create list of possible positions for mushrooms
		mushroomsGrid = GetGrid();
		// maximum half of available positions may be occupied
		mushroomsQty.fromValue = CheckIfOverMaximum(mushroomsQty.fromValue, mushroomsGrid.Count / 2);
		mushroomsQty.toValue = CheckIfOverMaximum(mushroomsQty.toValue, mushroomsGrid.Count / 2);
		// set number of mushrooms to instantiate
		int qty = Random.Range(mushroomsQty.fromValue - 1, mushroomsQty.toValue);
		// instantiate mushrooms at random positions 
		for (int i = 0; i < qty; i++)
		{
			MushroomAtRandomPosition();
		}
	}

	// take half of valueToCheck if it is over maximum
	private int CheckIfOverMaximum(int valueToCheck, int maximum)
	{
		// repeat if valueToCheck is still over maximum
		if (valueToCheck > maximum) 
			return CheckIfOverMaximum(valueToCheck / 2, maximum);
		else return valueToCheck;
	}

	// instantiate centipede parts at startChainPosition
	private void SetCentipedeChain()
	{
		for (int i = 0; i < centipedeSize; i++)
		{
			chain.Add(Instantiate(centipede, startChainPosition, Quaternion.identity));
		}
		// make first part a head
		chain[0].SetHead();
	}

	// instantiate mushrooms at random positions
	private void MushroomAtRandomPosition()
	{
		// take some random index within count of available positions
		int randomIndex = Random.Range(0, mushroomsGrid.Count);
		// instantiate mushroom 
		Instantiate(mushroom, mushroomsGrid[randomIndex], Quaternion.identity);
		// remove used position from list
		mushroomsGrid.RemoveAt(randomIndex);
	}

	// get list of possible positions
	private List<Vector2> GetGrid()
	{
		// create temp list
		List<Vector2> coords = new List<Vector2>();
		// add each available position within edges
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

	// add value to score and refresh it on screen
	public void AddScore(int value)
	{
		score += value;
		scoreText.text = "score: " + score;
	}

	// increase(true) or decrease(false) number of lives by 1
	public void ChangeLifeCount(bool trueWillAdd)
	{
		if (trueWillAdd)
			life++;
		else
		{
			life--;
			// game lost if lives ended
			if (life < 1)
			{
				LoseGame();
			}
			else
			// restart level if there is more lives
			{
				// motivation :)
				centerMessageText.text = "Try harder!";
				centerMessageText.gameObject.SetActive(true);
				// new round in a few seconds 
				Invoke(nameof(NewRound), timeToReload);
			}
		}

		// show current life after change
		PrintLife();
	}

	// direct set of current lives number with set value
	private void ChangeLifeCount(int setValue)
	{
		// set default if zero (used in new game)
		life = (setValue < 1) ? startLife : setValue;
		// show current life
		PrintLife();
	}

	// show current life number
	private void PrintLife()
	{
		// temp empty string
		string addText = "";
		// set lives number text string
		for (int i = 0; i < life; i++)
		{
			addText += "♥ ";
		}
		// show lives number 
		lifeText.text = "life: " + addText;
	}

	// set current round and dependent params: speed and length of centipede and mushrooms quantity
	private void SetRound()
	{
		round = PlayerPrefs.GetInt("Round", round);
		roundText.text = "Round " + round;
		centipedeSpeed += round * 0.05f;
		centipedeSize += round / 3;
		mushroomsQty.fromValue += round * 3;
		mushroomsQty.toValue += round * 3;
	}

	// check if won in a few seconds
	internal void CheckWin(float delayTime)
	{
		Invoke(nameof(CheckWin), delayTime);
	}

	// check if there is any centipede parts in game
	private void CheckWin()
	{
		// if none - win this round
		if (chain.Count == 0) WinRound();
	}

	// stop movement for all centipede parts
	internal void StopAll()
	{
		foreach (Centipede centi in chain)
		{
			// turn off script
			centi.enabled = false;
			if (centi.isHead)
			{
				// set velocity to zero for all head parts
				centi.rb.velocity = Vector2.zero;
			}
		}
	}

	// show result and offer to start a new game
	private void LoseGame()
	{
		// show lose message
		centerMessageText.text = "GAME OVER\nYour score is " + score;
		// activate button, which starts a new game
		restartButton.gameObject.SetActive(true);
		// set default lives and score
		PlayerPrefs.SetInt("Life", startLife);
		PlayerPrefs.SetInt("Score", 0);
		PlayerPrefs.SetInt("Round", 1);
		// activate text object
		centerMessageText.gameObject.SetActive(true);
	}

	// show congratulations and start a new round
	private void WinRound()
	{
		// show win round message
		centerMessageText.text = "You've won!\nGet ready for the next round";
		// activate message object
		centerMessageText.gameObject.SetActive(true);
		// save round number
		PlayerPrefs.SetInt("Round", ++round);
		// new round in a few seconds
		Invoke(nameof(NewRound), timeToReload);
	}

	// start a new game after lose (assigned as a button onClick function)
	public void Restart()
	{
		// reset score
		PlayerPrefs.SetInt("Score", 0);
		ReloadLevel();
	}

	// start a new round if lost or won
	private void NewRound()
	{
		// save current score and lives
		PlayerPrefs.SetInt("Score", score);
		PlayerPrefs.SetInt("Life", life);
		// reload scene
		ReloadLevel();
	}

	// reload current scene
	private void ReloadLevel()
	{
		SceneManager.LoadScene("MainScene");
	}
}