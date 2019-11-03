using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoesManager : MonoBehaviour
{
	public float secondsToNewFoe;
	[Range(1, 100)] public int chanceForAntToCreateMushroom;
	public List<Foe> foesPrefabs;

	private Coroutine CreateNewFoeCoroutine;

	private void Start()
	{
		CreateNewFoeCoroutine = StartCoroutine(CreateNewFoe(secondsToNewFoe));
	}

	// ReSharper disable once FunctionRecursiveOnAllPaths
	private IEnumerator CreateNewFoe(float secondsToSpawn)
	{
		yield return new WaitForSeconds(secondsToSpawn);
		Instantiate(foesPrefabs[Random.Range(0, foesPrefabs.Count)]);
		// repeat in defined time
		CreateNewFoeCoroutine = StartCoroutine(CreateNewFoe(secondsToNewFoe));
	}
}