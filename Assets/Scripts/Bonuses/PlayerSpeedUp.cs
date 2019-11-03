using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeedUp : Bonus
{
	protected override void ActivateInBonusManager()
	{
		GameManager.instance.bonusManager.ActivatePlayerSpeedUp();
	}
}