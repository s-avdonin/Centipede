public class CentipedeSlowDown : Bonus
{
	protected override void ActivateInBonusManager()
	{
		GameManager.instance.bonusManager.ActivateCentipedeSlowDown();
	}
}
