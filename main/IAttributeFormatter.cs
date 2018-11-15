using Klei.AI;
using UnityEngine;

public interface IAttributeFormatter
{
	GameUtil.TimeSlice DeltaTimeSlice
	{
		get;
		set;
	}

	string GetFormattedAttribute(AttributeInstance instance);

	string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance);

	string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice, GameObject parent_instance);

	string GetTooltip(Attribute master, AttributeInstance instance);
}
