public class ShotSpeedUp : Bonus
{
	protected override void ActivateInBonusManager()
	{
		GameManager.instance.bonusManager.ActivateShotSpeedUp();
	}
}
