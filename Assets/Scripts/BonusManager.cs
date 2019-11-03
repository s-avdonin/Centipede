using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// todo: would be better to have a class for storing bonuses 
public class BonusManager : MonoBehaviour
{
	[UnityEngine.Range(0, 100)] public int bonusesChanceToDrop;
	public float bonusesLifeTime;
	[UnityEngine.Range(1f, 10f)] public float playerSpeedMultiplier;
	[Range(1f, 10f)] public float shotSpeedMultiplier;
	[Range(0.1f, 1f)] public float centipedeSlowMultiplier;

	public List<Bonus> bonusPrefabs;

	// Player object write it's own reference into this variable on Awake
	internal Player player;

	// new shots take this param to multiply speed
	internal float activeShotMultiplier = 1f;

	private List<Centipede> centipedeChain;

	private bool isPlayerSpeedUpCoroutineRunning;
	private IEnumerator PlayerSpeedUpCoroutine;
	private bool isShotSpeedUpCoroutineRunning;
	private Coroutine ShotSpeedUpCoroutine;
	private bool isCentipedeSlowDownCoroutineRunning;
	private Coroutine CentipedeSlowDownCoroutine;

	private void Start()
	{
		centipedeChain = GameManager.instance.centipedeChain;
		PlayerSpeedUpCoroutine = ResetPlayerSpeed(bonusesLifeTime);
	}

	internal void DropBonus(Vector2 position)
	{
		if (Random.Range(0, 100) < bonusesChanceToDrop)
		{
			Bonus bonus = Instantiate(bonusPrefabs[Random.Range(0, bonusPrefabs.Count)], position, Quaternion.identity);
			bonus.transform.SetParent(transform);
		}
	}

	// centipede movement speed down ↓
	internal void ActivateCentipedeSlowDown()
	{
		if (!isCentipedeSlowDownCoroutineRunning)
		{
			foreach (Centipede centipede in centipedeChain)
			{
				centipede.rb.velocity *= centipedeSlowMultiplier;
				centipede.movementSpeed *= centipedeSlowMultiplier;
			}

			CentipedeSlowDownCoroutine = StartCoroutine(ResetCentipedeSpeed(bonusesLifeTime));
			isCentipedeSlowDownCoroutineRunning = true;
		}
		else
		{
			StopCoroutine(CentipedeSlowDownCoroutine);
			CentipedeSlowDownCoroutine = StartCoroutine(ResetCentipedeSpeed(bonusesLifeTime));
		}
	}

	private IEnumerator ResetCentipedeSpeed(float secondsToReset)
	{
		yield return new WaitForSeconds(secondsToReset);
		foreach (Centipede centipede in centipedeChain)
		{
			centipede.rb.velocity /= centipedeSlowMultiplier;
			centipede.movementSpeed = GameManager.instance.centipedeSpeed;
		}

		isCentipedeSlowDownCoroutineRunning = false;
	}

	// shot movement speed up ↓
	internal void ActivateShotSpeedUp()
	{
		if (isShotSpeedUpCoroutineRunning)
		{
			StopCoroutine(ShotSpeedUpCoroutine);
			ShotSpeedUpCoroutine = StartCoroutine(ResetShotSpeed(bonusesLifeTime));
		}
		else
		{
			isShotSpeedUpCoroutineRunning = true;
			activeShotMultiplier = shotSpeedMultiplier;
			ShotSpeedUpCoroutine = StartCoroutine(ResetShotSpeed(bonusesLifeTime));
		}
	}

	private IEnumerator ResetShotSpeed(float secondsToReset)
	{
		yield return new WaitForSeconds(secondsToReset);
		activeShotMultiplier = 1;
		isShotSpeedUpCoroutineRunning = false;
	}

	// player movement speed up ↓
	internal void ActivatePlayerSpeedUp()
	{
		if (!isPlayerSpeedUpCoroutineRunning)
		{
			isPlayerSpeedUpCoroutineRunning = true;
			player.speed *= playerSpeedMultiplier;
			StartCoroutine(PlayerSpeedUpCoroutine);
		}
		else // refresh bonus time if already running
		{
			StopCoroutine(PlayerSpeedUpCoroutine);
			StartCoroutine(PlayerSpeedUpCoroutine);
		}
	}

	private IEnumerator ResetPlayerSpeed(float secondsToReset)
	{
		yield return new WaitForSeconds(secondsToReset);
		player.speed = GameManager.instance.playerMovementSpeed;
		isPlayerSpeedUpCoroutineRunning = false;
	}
}