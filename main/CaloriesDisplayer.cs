using Klei.AI;
using UnityEngine;

public class CaloriesDisplayer : StandardAmountDisplayer
{
	public class CaloriesAttributeFormatter : StandardAttributeFormatter
	{
		public CaloriesAttributeFormatter()
			: base(GameUtil.UnitClass.Calories, GameUtil.TimeSlice.PerCycle)
		{
		}

		public override string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance)
		{
			if (!modifier.IsMultiplier)
			{
				return base.GetFormattedModifier(modifier, parent_instance);
			}
			return GameUtil.GetFormattedPercent((0f - modifier.Value) * 100f, GameUtil.TimeSlice.None);
		}

		public override string GetTooltip(Attribute master, AttributeInstance instance)
		{
			return "TEST";
		}

		public override string GetTooltipDescription(Attribute master, AttributeInstance instance)
		{
			return "TEST";
		}
	}

	public CaloriesDisplayer()
		: base(GameUtil.UnitClass.Calories, GameUtil.TimeSlice.PerCycle, null)
	{
		formatter = new CaloriesAttributeFormatter();
	}
}
