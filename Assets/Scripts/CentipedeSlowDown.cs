using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeSlowDown : Bonus
{
	protected override void ActivateInBonusManager()
	{
		GameManager.instance.bonusManager.ActivateCentipedeSlowDown();
	}
}
