using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class StandardAttributeFormatter : IAttributeFormatter
{
	public GameUtil.UnitClass unitClass;

	public GameUtil.TimeSlice DeltaTimeSlice
	{
		get;
		set;
	}

	public StandardAttributeFormatter(GameUtil.UnitClass unitClass, GameUtil.TimeSlice deltaTimeSlice)
	{
		this.unitClass = unitClass;
		DeltaTimeSlice = deltaTimeSlice;
	}

	public virtual string GetFormattedAttribute(AttributeInstance instance)
	{
		return GetFormattedValue(instance.GetTotalDisplayValue(), GameUtil.TimeSlice.None, null);
	}

	public virtual string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance)
	{
		return GetFormattedValue(modifier.Value, DeltaTimeSlice, null);
	}

	public virtual string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, GameObject parent_instance = null)
	{
		switch (unitClass)
		{
		case GameUtil.UnitClass.SimpleInteger:
			return GameUtil.GetFormattedInt(value, timeSlice);
		case GameUtil.UnitClass.Mass:
			return GameUtil.GetFormattedMass(value, timeSlice, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
		case GameUtil.UnitClass.Temperature:
			return GameUtil.GetFormattedTemperature(value, timeSlice, (timeSlice != 0) ? GameUtil.TemperatureInterpretation.Relative : GameUtil.TemperatureInterpretation.Absolute, true, false);
		case GameUtil.UnitClass.Percent:
			return GameUtil.GetFormattedPercent(value, timeSlice);
		case GameUtil.UnitClass.Calories:
			return GameUtil.GetFormattedCalories(value, timeSlice, true);
		case GameUtil.UnitClass.Distance:
			return GameUtil.GetFormattedDistance(value);
		case GameUtil.UnitClass.Disease:
			return GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(value));
		default:
			return GameUtil.GetFormattedSimple(value, timeSlice, null);
		}
	}

	public virtual string GetTooltipDescription(Attribute master, AttributeInstance instance)
	{
		return master.Description;
	}

	public virtual string GetTooltip(Attribute master, AttributeInstance instance)
	{
		string tooltipDescription = GetTooltipDescription(master, instance);
		tooltipDescription += string.Format(DUPLICANTS.ATTRIBUTES.TOTAL_VALUE, GetFormattedValue(instance.GetTotalDisplayValue(), GameUtil.TimeSlice.None, null), instance.Name);
		if (instance.GetBaseValue() != 0f)
		{
			tooltipDescription += string.Format(DUPLICANTS.ATTRIBUTES.BASE_VALUE, instance.GetBaseValue());
		}
		List<AttributeModifier> list = new List<AttributeModifier>();
		for (int i = 0; i < instance.Modifiers.Count; i++)
		{
			list.Add(instance.Modifiers[i]);
		}
		list.Sort((AttributeModifier p1, AttributeModifier p2) => p2.Value.CompareTo(p1.Value));
		for (int j = 0; j != list.Count; j++)
		{
			AttributeModifier attributeModifier = list[j];
			string formattedString = attributeModifier.GetFormattedString(instance.gameObject);
			if (formattedString != null)
			{
				tooltipDescription += string.Format(DUPLICANTS.ATTRIBUTES.MODIFIER_ENTRY, attributeModifier.GetDescription(), formattedString);
			}
		}
		string text = string.Empty;
		AttributeConverters component = instance.gameObject.GetComponent<AttributeConverters>();
		if ((Object)component != (Object)null && master.converters.Count > 0)
		{
			foreach (AttributeConverterInstance converter in component.converters)
			{
				if (converter.converter.attribute == master)
				{
					string text2 = converter.DescriptionFromAttribute(converter.Evaluate(), converter.gameObject);
					if (text2 != null)
					{
						text = text + "\n" + text2;
					}
				}
			}
		}
		if (text.Length > 0)
		{
			tooltipDescription = tooltipDescription + "\n" + text;
		}
		return tooltipDescription;
	}
}
