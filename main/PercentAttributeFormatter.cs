using Klei.AI;
using UnityEngine;

public class PercentAttributeFormatter : StandardAttributeFormatter
{
	public PercentAttributeFormatter()
		: base(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.None)
	{
	}

	public override string GetFormattedAttribute(AttributeInstance instance)
	{
		return GetFormattedValue(instance.GetTotalDisplayValue(), base.DeltaTimeSlice, instance.gameObject);
	}

	public override string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance)
	{
		return GetFormattedValue(modifier.Value, base.DeltaTimeSlice, parent_instance);
	}

	public override string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice, GameObject parent_instance)
	{
		return GameUtil.GetFormattedPercent(value * 100f, timeSlice);
	}
}
