using Klei.AI;
using UnityEngine;

public class ToPercentAttributeFormatter : StandardAttributeFormatter
{
	public float max = 1f;

	public ToPercentAttributeFormatter(float max, GameUtil.TimeSlice deltaTimeSlice = GameUtil.TimeSlice.None)
		: base(GameUtil.UnitClass.Percent, deltaTimeSlice)
	{
		this.max = max;
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
		return GameUtil.GetFormattedPercent(value / max * 100f, timeSlice);
	}
}
