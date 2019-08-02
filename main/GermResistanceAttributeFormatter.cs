using Klei.AI;
using UnityEngine;

public class GermResistanceAttributeFormatter : StandardAttributeFormatter
{
	public GermResistanceAttributeFormatter()
		: base(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.None)
	{
	}

	public override string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance)
	{
		return GameUtil.GetGermResistanceModifierString(modifier.Value, false);
	}
}
