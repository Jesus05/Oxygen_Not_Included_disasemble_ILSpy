using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;

[Serializable]
[DebuggerDisplay("{name}")]
public class Element : IComparable<Element>
{
	[Serializable]
	public enum State : byte
	{
		Vacuum = 0,
		Gas = 1,
		Liquid = 2,
		Solid = 3,
		Unbreakable = 4,
		Unstable = 8,
		TemperatureInsulated = 0x10
	}

	public SimHashes id;

	public Tag tag;

	public byte idx;

	public float specificHeatCapacity;

	public float thermalConductivity = 1f;

	public float molarMass = 1f;

	public float strength;

	public float flow = 0f;

	public float maxCompression = 0f;

	public float viscosity = 0f;

	public float minHorizontalFlow = float.PositiveInfinity;

	public float minVerticalFlow = float.PositiveInfinity;

	public float maxMass = 10000f;

	public float solidSurfaceAreaMultiplier;

	public float liquidSurfaceAreaMultiplier;

	public float gasSurfaceAreaMultiplier;

	public State state;

	public byte hardness = 0;

	public float lowTemp;

	public SimHashes lowTempTransitionTarget;

	public Element lowTempTransition;

	public float highTemp;

	public SimHashes highTempTransitionTarget;

	public Element highTempTransition;

	public SimHashes highTempTransitionOreID = SimHashes.Vacuum;

	public float highTempTransitionOreMassConversion = 0f;

	public SimHashes lowTempTransitionOreID = SimHashes.Vacuum;

	public float lowTempTransitionOreMassConversion = 0f;

	public SimHashes sublimateId;

	public SimHashes convertId;

	public SpawnFXHashes sublimateFX;

	public float lightAbsorptionFactor;

	public Sim.PhysicsData defaultValues;

	public float toxicity;

	public Substance substance;

	public Tag materialCategory;

	public int buildMenuSort;

	public Tag[] oreTags = new Tag[0];

	public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();

	public bool disabled;

	public const byte StateMask = 3;

	public bool IsUnstable => HasTag(GameTags.Unstable);

	public bool IsLiquid => (state & State.Solid) == State.Liquid;

	public bool IsGas => (state & State.Solid) == State.Gas;

	public bool IsSolid => (state & State.Solid) == State.Solid;

	public bool IsVacuum => (state & State.Solid) == State.Vacuum;

	public bool IsTemperatureInsulated => (state & State.TemperatureInsulated) != State.Vacuum;

	public bool HasTransitionUp => highTempTransitionTarget != 0 && highTempTransitionTarget != SimHashes.Unobtanium && highTempTransition != null && highTempTransition != this;

	public string name
	{
		get;
		set;
	}

	public string nameUpperCase
	{
		get;
		set;
	}

	public string description
	{
		get;
		set;
	}

	public float PressureToMass(float pressure)
	{
		return pressure / defaultValues.pressure;
	}

	public bool IsState(State expected_state)
	{
		return (state & State.Solid) == expected_state;
	}

	public string GetStateString()
	{
		return GetStateString(state);
	}

	public static string GetStateString(State state)
	{
		if ((state & State.Solid) != State.Solid)
		{
			if ((state & State.Solid) != State.Liquid)
			{
				if ((state & State.Solid) != State.Gas)
				{
					return ELEMENTS.STATE.VACUUM;
				}
				return ELEMENTS.STATE.GAS;
			}
			return ELEMENTS.STATE.LIQUID;
		}
		return ELEMENTS.STATE.SOLID;
	}

	public string FullDescription(bool addHardnessColor = true)
	{
		string str = Description();
		if (IsSolid)
		{
			str += "\n\n";
			str += string.Format(ELEMENTS.ELEMENTDESCSOLID, GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetHardnessString(this, addHardnessColor));
		}
		else if (IsLiquid)
		{
			str += "\n\n";
			str += string.Format(ELEMENTS.ELEMENTDESCLIQUID, GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
		}
		else if (!IsVacuum)
		{
			str += "\n\n";
			str += string.Format(ELEMENTS.ELEMENTDESCGAS, GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
		}
		string text = ELEMENTS.THERMALPROPERTIES;
		text = text.Replace("{SPECIFIC_HEAT_CAPACITY}", GameUtil.GetFormattedSHC(specificHeatCapacity));
		text = text.Replace("{THERMAL_CONDUCTIVITY}", GameUtil.GetFormattedThermalConductivity(thermalConductivity));
		str = str + "\n" + text;
		if (oreTags.Length > 0 && !IsVacuum)
		{
			str += "\n\n";
			string text2 = "";
			for (int i = 0; i < oreTags.Length; i++)
			{
				Tag tag = new Tag(oreTags[i]);
				text2 += tag.ProperName();
				if (i < oreTags.Length - 1)
				{
					text2 += ", ";
				}
			}
			str += string.Format(ELEMENTS.ELEMENTPROPERTIES, text2);
		}
		if (attributeModifiers.Count > 0)
		{
			foreach (AttributeModifier attributeModifier in attributeModifiers)
			{
				string name = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId).Name;
				string formattedString = attributeModifier.GetFormattedString(null);
				str = str + "\n" + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, name, formattedString);
			}
		}
		return str;
	}

	public string Description()
	{
		return description;
	}

	public bool HasTag(Tag search_tag)
	{
		if (!(tag == search_tag))
		{
			return Array.IndexOf(oreTags, search_tag) != -1;
		}
		return true;
	}

	public Tag GetMaterialCategoryTag()
	{
		return materialCategory;
	}

	public int CompareTo(Element other)
	{
		return id - other.id;
	}
}
