using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotSpeedUp : Bonus
{
	protected override void ActivateInBonusManager()
	{
		GameManager.instance.bonusManager.ActivateShotSpeedUp();
	}
}
