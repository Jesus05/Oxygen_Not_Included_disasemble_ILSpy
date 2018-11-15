using Klei.AI;
using UnityEngine;

public class FoodQualityAttributeFormatter : StandardAttributeFormatter
{
	public FoodQualityAttributeFormatter()
		: base(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None)
	{
	}

	public override string GetFormattedAttribute(AttributeInstance instance)
	{
		return GetFormattedValue(instance.GetTotalDisplayValue(), GameUtil.TimeSlice.None, instance.gameObject);
	}

	public override string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance)
	{
		return GameUtil.GetFormattedInt(modifier.Value, GameUtil.TimeSlice.None);
	}

	public override string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice, GameObject parent_instance)
	{
		int quality = (int)value;
		return Util.StripTextFormatting(GameUtil.GetFormattedFoodQuality(quality));
	}
}
