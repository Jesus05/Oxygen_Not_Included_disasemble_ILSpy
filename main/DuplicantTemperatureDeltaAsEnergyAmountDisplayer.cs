using Klei.AI;
using STRINGS;

public class DuplicantTemperatureDeltaAsEnergyAmountDisplayer : StandardAmountDisplayer
{
	public DuplicantTemperatureDeltaAsEnergyAmountDisplayer(GameUtil.UnitClass unitClass, GameUtil.TimeSlice timeSlice)
		: base(unitClass, timeSlice, null)
	{
	}

	public override string GetTooltip(Amount master, AmountInstance instance)
	{
		string name = master.Name;
		name = name + UI.HORIZONTAL_BR_RULE + string.Format(master.description, formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None, null), formatter.GetFormattedValue(310.15f, GameUtil.TimeSlice.None, null));
		float num = ElementLoader.FindElementByHash(SimHashes.Creature).specificHeatCapacity * 30f * 1000f;
		name += "\n\n";
		if (formatter.DeltaTimeSlice == GameUtil.TimeSlice.PerCycle)
		{
			name += string.Format(UI.CHANGEPERCYCLE, formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerCycle, null));
		}
		else
		{
			name += string.Format(UI.CHANGEPERSECOND, formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerSecond, null));
			name = name + "\n" + string.Format(UI.CHANGEPERSECOND, GameUtil.GetFormattedJoules(instance.deltaAttribute.GetTotalDisplayValue() * num, "F1", GameUtil.TimeSlice.None));
		}
		for (int i = 0; i != instance.deltaAttribute.Modifiers.Count; i++)
		{
			AttributeModifier attributeModifier = instance.deltaAttribute.Modifiers[i];
			name = name + "\n" + string.Format(UI.MODIFIER_ITEM_TEMPLATE, attributeModifier.GetDescription(), GameUtil.GetFormattedHeatEnergyRate(attributeModifier.Value * num * 1f, GameUtil.HeatEnergyFormatterUnit.Automatic));
		}
		return name;
	}
}
