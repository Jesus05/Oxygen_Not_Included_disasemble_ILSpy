using Klei.AI;
using UnityEngine;

public class CrewRationsEntry : CrewListEntry
{
	public KButton incRationPerDayButton;

	public KButton decRationPerDayButton;

	public LocText rationPerDayText;

	public LocText rationsEatenToday;

	public LocText currentCaloriesText;

	public LocText currentStressText;

	public LocText currentHealthText;

	public ValueTrendImageToggle stressTrendImage;

	private RationMonitor.Instance rationMonitor;

	public override void Populate(MinionIdentity _identity)
	{
		base.Populate(_identity);
		rationMonitor = _identity.GetSMI<RationMonitor.Instance>();
		Refresh();
	}

	public override void Refresh()
	{
		base.Refresh();
		rationsEatenToday.text = GameUtil.GetFormattedCalories(rationMonitor.GetRationsAteToday(), GameUtil.TimeSlice.None, true);
		if (!((Object)identity == (Object)null))
		{
			Amounts amounts = identity.GetAmounts();
			foreach (AmountInstance item in amounts)
			{
				float min = item.GetMin();
				float max = item.GetMax();
				float num = max - min;
				float num2 = (num - (max - item.value)) / num;
				string str = Mathf.RoundToInt(num2 * 100f).ToString();
				if (item.amount == Db.Get().Amounts.Stress)
				{
					currentStressText.text = item.GetValueString();
					currentStressText.GetComponent<ToolTip>().toolTip = item.GetTooltip();
					stressTrendImage.SetValue(item);
				}
				else if (item.amount == Db.Get().Amounts.Calories)
				{
					currentCaloriesText.text = str + "%";
					currentCaloriesText.GetComponent<ToolTip>().toolTip = item.GetTooltip();
				}
				else if (item.amount == Db.Get().Amounts.HitPoints)
				{
					currentHealthText.text = str + "%";
					currentHealthText.GetComponent<ToolTip>().toolTip = item.GetTooltip();
				}
			}
		}
	}
}
