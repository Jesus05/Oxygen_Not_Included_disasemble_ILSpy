public class KSelectableHealthBar : KSelectable
{
	[MyCmpGet]
	private ProgressBar progressBar;

	private int scaleAmount = 100;

	public override string GetName()
	{
		int num = (int)(progressBar.PercentFull * (float)scaleAmount);
		return $"{entityName} {num}/{scaleAmount}";
	}
}
