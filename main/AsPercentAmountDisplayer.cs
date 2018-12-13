using Klei.AI;
using STRINGS;
using UnityEngine;

public class AsPercentAmountDisplayer : IAmountDisplayer
{
	protected StandardAttributeFormatter formatter;

	public IAttributeFormatter Formatter => formatter;

	public GameUtil.TimeSlice DeltaTimeSlice
	{
		get
		{
			return formatter.DeltaTimeSlice;
		}
		set
		{
			formatter.DeltaTimeSlice = value;
		}
	}

	public AsPercentAmountDisplayer(GameUtil.TimeSlice deltaTimeSlice)
	{
		formatter = new StandardAttributeFormatter(GameUtil.UnitClass.Percent, deltaTimeSlice);
	}

	public string GetValueString(Amount master, AmountInstance instance)
	{
		return formatter.GetFormattedValue(ToPercent(instance.value, instance), GameUtil.TimeSlice.None, null);
	}

	public virtual string GetDescription(Amount master, AmountInstance instance)
	{
		return $"{master.Name}: {formatter.GetFormattedValue(ToPercent(instance.value, instance), GameUtil.TimeSlice.None, null)}";
	}

	public virtual string GetTooltipDescription(Amount master, AmountInstance instance)
	{
		return string.Format(master.description, formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None, null));
	}

	public virtual string GetTooltip(Amount master, AmountInstance instance)
	{
		string name = master.Name;
		name = name + UI.HORIZONTAL_BR_RULE + string.Format(master.description, formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None, null));
		name += "\n\n";
		name = ((formatter.DeltaTimeSlice != GameUtil.TimeSlice.PerCycle) ? (name + string.Format(UI.CHANGEPERSECOND, formatter.GetFormattedValue(ToPercent(instance.deltaAttribute.GetTotalDisplayValue(), instance), GameUtil.TimeSlice.PerSecond, null))) : (name + string.Format(UI.CHANGEPERCYCLE, formatter.GetFormattedValue(ToPercent(instance.deltaAttribute.GetTotalDisplayValue(), instance), GameUtil.TimeSlice.PerCycle, null))));
		for (int i = 0; i != instance.deltaAttribute.Modifiers.Count; i++)
		{
			AttributeModifier attributeModifier = instance.deltaAttribute.Modifiers[i];
			float modifierContribution = instance.deltaAttribute.GetModifierContribution(attributeModifier);
			name = name + "\n" + string.Format(UI.MODIFIER_ITEM_TEMPLATE, attributeModifier.GetDescription(), formatter.GetFormattedValue(ToPercent(modifierContribution, instance), formatter.DeltaTimeSlice, null));
		}
		return name;
	}

	public string GetFormattedAttribute(AttributeInstance instance)
	{
		return formatter.GetFormattedAttribute(instance);
	}

	public string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance)
	{
		if (modifier.IsMultiplier)
		{
			return GameUtil.GetFormattedPercent(modifier.Value * 100f, GameUtil.TimeSlice.None);
		}
		return formatter.GetFormattedModifier(modifier, parent_instance);
	}

	public string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice, GameObject parent_instance)
	{
		return formatter.GetFormattedValue(value, timeSlice, parent_instance);
	}

	protected float ToPercent(float value, AmountInstance instance)
	{
		return 100f * value / instance.GetMax();
	}
}
