public class PlayerSpeedUp : Bonus
{
	protected override void ActivateInBonusManager()
	{
		GameManager.instance.bonusManager.ActivatePlayerSpeedUp();
	}
}