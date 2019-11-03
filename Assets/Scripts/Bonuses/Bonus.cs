﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public abstract class Bonus : MovingObject
{
	protected void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<Player>())
		{
			ActivateInBonusManager();
			Destroy(gameObject);
		}
	}

	protected abstract void ActivateInBonusManager();
}