using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoesManager : MonoBehaviour
{
	public float secondsToNewFoe;
	[Range(0, 100)] public int chanceForAntToCreateMushroom;
	[Range(0, 100)] public int chanceForSpiderToChangeDirection;
	[Range(0, 100)] public int chanceForSpiderToEatMushroom;
	public List<Foe> foesPrefabs;

	internal List<float> possibleHeightsForSpider;

	private void Awake()
	{
	}

	private void Start()
	{
		SetPossibleHeights();
		StartCoroutine(CreateNewFoe(secondsToNewFoe));
	}

	private void SetPossibleHeights()
	{
		possibleHeightsForSpider = new List<float>();
		for (float height = -GameManager.instance.sceneEdge;
			height <= GameManager.instance.topBorderForPlayer;
			height +=
				GameManager.instance.rowHeight)
		{
			possibleHeightsForSpider.Add(height);
		}
	}

	// ReSharper disable once FunctionRecursiveOnAllPaths
	private IEnumerator CreateNewFoe(float secondsToSpawn)
	{
		yield return new WaitForSeconds(secondsToSpawn);
		Instantiate(foesPrefabs[Random.Range(0, foesPrefabs.Count)]);
		// repeat in defined time
		StartCoroutine(CreateNewFoe(secondsToNewFoe));
	}
}