using Klei;
using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public static class GameUtil
{
	public enum UnitClass
	{
		SimpleFloat,
		SimpleInteger,
		Temperature,
		Mass,
		Calories,
		Percent,
		Distance,
		Disease
	}

	public enum TemperatureUnit
	{
		Celsius,
		Fahrenheit,
		Kelvin
	}

	public enum MassUnit
	{
		Kilograms,
		Pounds
	}

	public enum MetricMassFormat
	{
		UseThreshold,
		Kilogram,
		Gram,
		Tonne
	}

	public enum TemperatureInterpretation
	{
		Absolute,
		Relative
	}

	public enum TimeSlice
	{
		None,
		ModifyOnly,
		PerSecond,
		PerCycle
	}

	public enum MeasureUnit
	{
		mass,
		kcal,
		quantity
	}

	public enum WattageFormatterUnit
	{
		Watts,
		Kilowatts,
		Automatic
	}

	public enum HeatEnergyFormatterUnit
	{
		DTU_S,
		KDTU_S,
		Automatic,
		None
	}

	public struct FloodFillInfo
	{
		public int cell;

		public int depth;
	}

	public enum Hardness
	{
		NA = 0,
		VERY_SOFT = 0,
		SOFT = 10,
		FIRM = 25,
		VERY_FIRM = 50,
		NEARLY_IMPENETRABLE = 150,
		IMPENETRABLE = 0xFF
	}

	public static TemperatureUnit temperatureUnit;

	public static MassUnit massUnit;

	private static string[] adjectives;

	[ThreadStatic]
	public static Queue<FloodFillInfo> FloodFillNext = new Queue<FloodFillInfo>();

	[ThreadStatic]
	public static HashSet<int> FloodFillVisited = new HashSet<int>();

	public static TagSet foodTags = new TagSet("BasicPlantFood", "MushBar", "ColdWheatSeed", "ColdWheatSeed", "SpiceNut", "PrickleFruit", "Meat", "Mushroom", "ColdWheat", GameTags.Compostable.Name);

	public static TagSet solidTags = new TagSet("Filter", "Coal", "BasicFabric", "SwampLilyFlower", "RefinedMetal");

	public static string GetTemperatureUnitSuffix()
	{
		switch (temperatureUnit)
		{
		case TemperatureUnit.Celsius:
			return UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
		case TemperatureUnit.Fahrenheit:
			return UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
		default:
			return UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
		}
	}

	private static string AddTemperatureUnitSuffix(string text)
	{
		return text + GetTemperatureUnitSuffix();
	}

	public static float GetTemperatureConvertedFromKelvin(float temperature, TemperatureUnit targetUnit)
	{
		switch (targetUnit)
		{
		case TemperatureUnit.Celsius:
			return temperature - 273.15f;
		case TemperatureUnit.Fahrenheit:
			return temperature * 1.8f - 459.67f;
		default:
			return temperature;
		}
	}

	public static float GetConvertedTemperature(float temperature, bool roundOutput = false)
	{
		float num = 0f;
		switch (temperatureUnit)
		{
		case TemperatureUnit.Celsius:
			num = temperature - 273.15f;
			return (!roundOutput) ? num : Mathf.Round(num);
		case TemperatureUnit.Fahrenheit:
			num = temperature * 1.8f - 459.67f;
			return (!roundOutput) ? num : Mathf.Round(num);
		default:
			return (!roundOutput) ? temperature : Mathf.Round(temperature);
		}
	}

	public static float GetTemperatureConvertedToKelvin(float temperature, TemperatureUnit fromUnit)
	{
		switch (fromUnit)
		{
		case TemperatureUnit.Celsius:
			return temperature + 273.15f;
		case TemperatureUnit.Fahrenheit:
			return (temperature + 459.67f) * 5f / 9f;
		default:
			return temperature;
		}
	}

	public static float GetTemperatureConvertedToKelvin(float temperature)
	{
		switch (temperatureUnit)
		{
		case TemperatureUnit.Celsius:
			return temperature + 273.15f;
		case TemperatureUnit.Fahrenheit:
			return (temperature + 459.67f) * 5f / 9f;
		default:
			return temperature;
		}
	}

	private static float GetConvertedTemperatureDelta(float kelvin_delta)
	{
		switch (temperatureUnit)
		{
		case TemperatureUnit.Celsius:
			return kelvin_delta;
		case TemperatureUnit.Fahrenheit:
			return kelvin_delta * 1.8f;
		case TemperatureUnit.Kelvin:
			return kelvin_delta;
		default:
			return kelvin_delta;
		}
	}

	public static float ApplyTimeSlice(float val, TimeSlice timeSlice)
	{
		if (timeSlice == TimeSlice.PerCycle)
		{
			return val * 600f;
		}
		return val;
	}

	public static string AddTimeSliceText(string text, TimeSlice timeSlice)
	{
		switch (timeSlice)
		{
		case TimeSlice.PerSecond:
			return text + UI.UNITSUFFIXES.PERSECOND;
		case TimeSlice.PerCycle:
			return text + UI.UNITSUFFIXES.PERCYCLE;
		default:
			return text;
		}
	}

	public static string AddPositiveSign(string text, bool positive)
	{
		if (positive)
		{
			return string.Format(UI.POSITIVE_FORMAT, text);
		}
		return text;
	}

	public static float AttributeSkillToAlpha(AttributeInstance attributeInstance)
	{
		return Mathf.Min(attributeInstance.GetTotalValue() / 10f, 1f);
	}

	public static float AttributeSkillToAlpha(float attributeSkill)
	{
		return Mathf.Min(attributeSkill / 10f, 1f);
	}

	public static float AptitudeToAlpha(float aptitude)
	{
		return Mathf.Min(aptitude / 10f, 1f);
	}

	public static float GetThermalEnergy(PrimaryElement pe)
	{
		return pe.Temperature * pe.Mass * pe.Element.specificHeatCapacity;
	}

	public static float CalculateTemperatureChange(float shc, float mass, float kilowatts)
	{
		return kilowatts / (shc * mass);
	}

	public static void DeltaThermalEnergy(PrimaryElement pe, float kilowatts)
	{
		pe.Temperature += CalculateTemperatureChange(pe.Element.specificHeatCapacity, pe.Mass, kilowatts);
	}

	public static BindingEntry ActionToBinding(Action action)
	{
		BindingEntry[] keyBindings = GameInputMapping.KeyBindings;
		for (int i = 0; i < keyBindings.Length; i++)
		{
			BindingEntry result = keyBindings[i];
			if (result.mAction == action)
			{
				return result;
			}
		}
		throw new ArgumentException(action.ToString() + " is not bound in GameInputBindings");
	}

	public static string GetIdentityDescriptor(GameObject go)
	{
		if ((bool)go.GetComponent<MinionIdentity>())
		{
			return DUPLICANTS.STATS.SUBJECTS.DUPLICANT;
		}
		if ((bool)go.GetComponent<CreatureBrain>())
		{
			return DUPLICANTS.STATS.SUBJECTS.CREATURE;
		}
		return DUPLICANTS.STATS.SUBJECTS.PLANT;
	}

	public static float GetEnergyInPrimaryElement(PrimaryElement element)
	{
		return 0.001f * (element.Temperature * (element.Mass * 1000f * element.Element.specificHeatCapacity));
	}

	public static float EnergyToTemperatureDelta(float kilojoules, PrimaryElement element)
	{
		float energyInPrimaryElement = GetEnergyInPrimaryElement(element);
		float num = Mathf.Max(energyInPrimaryElement - kilojoules, 1f);
		float temperature = element.Temperature;
		float num2 = num / (0.001f * (element.Mass * (element.Element.specificHeatCapacity * 1000f)));
		return num2 - temperature;
	}

	public static float CalculateEnergyDeltaForElement(PrimaryElement element, float startTemp, float endTemp)
	{
		return CalculateEnergyDeltaForElementChange(element.Mass, element.Element.specificHeatCapacity, startTemp, endTemp);
	}

	public static float CalculateEnergyDeltaForElementChange(float mass, float shc, float startTemp, float endTemp)
	{
		float num = endTemp - startTemp;
		return num * mass * shc;
	}

	public static float GetFinalTemperature(float t1, float m1, float t2, float m2)
	{
		float num = m1 + m2;
		float num2 = t1 * m1 + t2 * m2;
		float value = num2 / num;
		float num3 = Mathf.Min(t1, t2);
		float num4 = Mathf.Max(t1, t2);
		value = Mathf.Clamp(value, num3, num4);
		if (float.IsNaN(value) || float.IsInfinity(value))
		{
			Debug.LogError($"Calculated an invalid temperature: t1={t1}, m1={m1}, t2={t2}, m2={m2}, min_temp={num3}, max_temp={num4}");
		}
		return value;
	}

	public static string FloatToString(float f, string format = null)
	{
		if (float.IsPositiveInfinity(f))
		{
			return UI.POS_INFINITY;
		}
		if (float.IsNegativeInfinity(f))
		{
			return UI.NEG_INFINITY;
		}
		return f.ToString(format);
	}

	public static string GetUnitFormattedName(GameObject go, bool upperName = false)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null && Assets.IsTagCountable(component.PrefabTag))
		{
			PrimaryElement component2 = go.GetComponent<PrimaryElement>();
			return GetUnitFormattedName(go.GetProperName(), component2.Units, upperName);
		}
		return (!upperName) ? go.GetProperName() : StringFormatter.ToUpper(go.GetProperName());
	}

	public static string GetUnitFormattedName(string name, float count, bool upperName = false)
	{
		if (upperName)
		{
			name = name.ToUpper();
		}
		return StringFormatter.Replace(UI.NAME_WITH_UNITS, "{0}", name).Replace("{1}", $"{count:0.##}");
	}

	public static string GetFormattedUnits(float units, TimeSlice timeSlice = TimeSlice.None, bool displaySuffix = true)
	{
		string str = UI.UNITSUFFIXES.UNITS;
		units = ApplyTimeSlice(units, timeSlice);
		string empty = string.Empty;
		empty = ((units == 0f) ? "0" : ((Mathf.Abs(units) < 1f) ? FloatToString(units, "#,##0.#") : ((!(Mathf.Abs(units) < 10f)) ? FloatToString(units, "#,###") : FloatToString(units, "#,###.#"))));
		if (displaySuffix)
		{
			empty += str;
		}
		return AddTimeSliceText(empty, timeSlice);
	}

	public static string ApplyBoldString(string source)
	{
		return "<b>" + source + "</b>";
	}

	public static float GetRoundedTemperatureInKelvin(float kelvin)
	{
		float result = 0f;
		switch (temperatureUnit)
		{
		case TemperatureUnit.Celsius:
			result = GetTemperatureConvertedToKelvin(Mathf.Round(GetConvertedTemperature(Mathf.Round(kelvin), true)));
			break;
		case TemperatureUnit.Fahrenheit:
		{
			float temperature = (float)Mathf.RoundToInt(GetTemperatureConvertedFromKelvin(kelvin, TemperatureUnit.Fahrenheit));
			result = GetTemperatureConvertedToKelvin(temperature, TemperatureUnit.Fahrenheit);
			break;
		}
		case TemperatureUnit.Kelvin:
			result = (float)Mathf.RoundToInt(kelvin);
			break;
		}
		return result;
	}

	public static string GetFormattedTemperature(float temp, TimeSlice timeSlice = TimeSlice.None, TemperatureInterpretation interpretation = TemperatureInterpretation.Absolute, bool displayUnits = true, bool roundInDestinationFormat = false)
	{
		switch (interpretation)
		{
		case TemperatureInterpretation.Absolute:
			temp = GetConvertedTemperature(temp, roundInDestinationFormat);
			break;
		default:
			temp = GetConvertedTemperatureDelta(temp);
			break;
		}
		temp = ApplyTimeSlice(temp, timeSlice);
		string empty = string.Empty;
		empty = ((!(Mathf.Abs(temp) < 0.1f)) ? FloatToString(temp, "##0.#") : FloatToString(temp, "##0.####"));
		if (displayUnits)
		{
			empty = AddTemperatureUnitSuffix(empty);
		}
		return AddTimeSliceText(empty, timeSlice);
	}

	public static string GetFormattedCaloriesForItem(Tag tag, float amount, TimeSlice timeSlice = TimeSlice.None, bool forceKcal = true)
	{
		EdiblesManager.FoodInfo foodInfo = Game.Instance.ediblesManager.GetFoodInfo(tag.Name);
		float calories = foodInfo.CaloriesPerUnit * amount;
		return GetFormattedCalories(calories, timeSlice, forceKcal);
	}

	public static string GetFormattedCalories(float calories, TimeSlice timeSlice = TimeSlice.None, bool forceKcal = true)
	{
		string str = UI.UNITSUFFIXES.CALORIES.CALORIE;
		if (Mathf.Abs(calories) >= 1000f || forceKcal)
		{
			calories /= 1000f;
			str = UI.UNITSUFFIXES.CALORIES.KILOCALORIE;
		}
		calories = ApplyTimeSlice(calories, timeSlice);
		string empty = string.Empty;
		empty = ((calories == 0f) ? ("0" + str) : ((Mathf.Abs(calories) < 1f) ? (FloatToString(calories, "#,##0.#") + str) : ((!(Mathf.Abs(calories) < 10f)) ? (FloatToString(calories, "#,###") + str) : (FloatToString(calories, "#,###.#") + str))));
		return AddTimeSliceText(empty, timeSlice);
	}

	public static string GetFormattedPercent(float percent, TimeSlice timeSlice = TimeSlice.None)
	{
		percent = ApplyTimeSlice(percent, timeSlice);
		string empty = string.Empty;
		empty = ((Mathf.Abs(percent) == 0f) ? "0" : ((Mathf.Abs(percent) < 0.1f) ? "##0.##" : ((!(Mathf.Abs(percent) < 1f)) ? "##0" : "##0.#")));
		string text = FloatToString(percent, empty) + UI.UNITSUFFIXES.PERCENT;
		return AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedRoundedJoules(float joules)
	{
		if (Mathf.Abs(joules) > 1000f)
		{
			return FloatToString(joules / 1000f, "F1") + UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE;
		}
		return FloatToString(joules, "F1") + UI.UNITSUFFIXES.ELECTRICAL.JOULE;
	}

	public static string GetFormattedJoules(float joules, string floatFormat = "F1", TimeSlice timeSlice = TimeSlice.None)
	{
		joules = ApplyTimeSlice(joules, timeSlice);
		string text = (Math.Abs(joules) > 1000000f) ? (FloatToString(joules / 1000000f, floatFormat) + UI.UNITSUFFIXES.ELECTRICAL.MEGAJOULE) : ((!(Mathf.Abs(joules) > 1000f)) ? (FloatToString(joules, floatFormat) + UI.UNITSUFFIXES.ELECTRICAL.JOULE) : (FloatToString(joules / 1000f, floatFormat) + UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE));
		return AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedWattage(float watts, WattageFormatterUnit unit = WattageFormatterUnit.Automatic)
	{
		LocString loc_string = string.Empty;
		switch (unit)
		{
		case WattageFormatterUnit.Automatic:
			if (Mathf.Abs(watts) > 1000f)
			{
				watts /= 1000f;
				loc_string = UI.UNITSUFFIXES.ELECTRICAL.KILOWATT;
			}
			else
			{
				loc_string = UI.UNITSUFFIXES.ELECTRICAL.WATT;
			}
			break;
		case WattageFormatterUnit.Kilowatts:
			watts /= 1000f;
			loc_string = UI.UNITSUFFIXES.ELECTRICAL.KILOWATT;
			break;
		case WattageFormatterUnit.Watts:
			loc_string = UI.UNITSUFFIXES.ELECTRICAL.WATT;
			break;
		}
		return FloatToString(watts, "###0.##") + loc_string;
	}

	public static string GetFormattedHeatEnergy(float dtu, HeatEnergyFormatterUnit unit = HeatEnergyFormatterUnit.Automatic)
	{
		LocString loc_string = string.Empty;
		switch (unit)
		{
		case HeatEnergyFormatterUnit.Automatic:
			if (Mathf.Abs(dtu) > 1000f)
			{
				dtu /= 1000f;
				loc_string = UI.UNITSUFFIXES.HEAT.KDTU;
			}
			else
			{
				loc_string = UI.UNITSUFFIXES.HEAT.DTU;
			}
			break;
		case HeatEnergyFormatterUnit.KDTU_S:
			dtu /= 1000f;
			loc_string = UI.UNITSUFFIXES.HEAT.KDTU;
			break;
		case HeatEnergyFormatterUnit.DTU_S:
			loc_string = UI.UNITSUFFIXES.HEAT.DTU;
			break;
		}
		return FloatToString(dtu, "###0.##") + loc_string;
	}

	public static string GetFormattedHeatEnergyRate(float dtu_s, HeatEnergyFormatterUnit unit = HeatEnergyFormatterUnit.Automatic)
	{
		LocString loc_string = string.Empty;
		switch (unit)
		{
		case HeatEnergyFormatterUnit.Automatic:
			if (Mathf.Abs(dtu_s) > 1000f)
			{
				dtu_s /= 1000f;
				loc_string = UI.UNITSUFFIXES.HEAT.KDTU_S;
			}
			else
			{
				loc_string = UI.UNITSUFFIXES.HEAT.DTU_S;
			}
			break;
		case HeatEnergyFormatterUnit.KDTU_S:
			dtu_s /= 1000f;
			loc_string = UI.UNITSUFFIXES.HEAT.KDTU_S;
			break;
		case HeatEnergyFormatterUnit.DTU_S:
			loc_string = UI.UNITSUFFIXES.HEAT.DTU_S;
			break;
		}
		return FloatToString(dtu_s, "###0.##") + loc_string;
	}

	public static string GetFormattedInt(float num, TimeSlice timeSlice = TimeSlice.None)
	{
		num = ApplyTimeSlice(num, timeSlice);
		return AddTimeSliceText(FloatToString(num, "F0"), timeSlice);
	}

	public static string GetFormattedSimple(float num, TimeSlice timeSlice = TimeSlice.None, string formatString = null)
	{
		num = ApplyTimeSlice(num, timeSlice);
		string empty = string.Empty;
		empty = ((formatString != null) ? FloatToString(num, formatString) : ((num == 0f) ? "0" : ((Mathf.Abs(num) < 1f) ? FloatToString(num, "#,##0.##") : ((!(Mathf.Abs(num) < 10f)) ? FloatToString(num, "#,###.##") : FloatToString(num, "#,###.##")))));
		return AddTimeSliceText(empty, timeSlice);
	}

	public static string GetLightDescription(int lux)
	{
		if (lux == 0)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.NO_LIGHT;
		}
		if (lux < 100)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.VERY_LOW_LIGHT;
		}
		if (lux < 1000)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.LOW_LIGHT;
		}
		if (lux < 10000)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.MEDIUM_LIGHT;
		}
		if (lux < 50000)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.HIGH_LIGHT;
		}
		if (lux < 100000)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.VERY_HIGH_LIGHT;
		}
		return UI.OVERLAYS.LIGHTING.RANGES.MAX_LIGHT;
	}

	public static string GetFormattedByTag(Tag tag, float amount, TimeSlice timeSlice = TimeSlice.None)
	{
		if (GameTags.DisplayAsCalories.Contains(tag))
		{
			return GetFormattedCaloriesForItem(tag, amount, timeSlice, true);
		}
		if (GameTags.DisplayAsUnits.Contains(tag))
		{
			return GetFormattedUnits(amount, timeSlice, true);
		}
		return GetFormattedMass(amount, timeSlice, MetricMassFormat.UseThreshold, true, "{0:0.#}");
	}

	public static string GetFormattedFoodQuality(int quality)
	{
		if (adjectives == null)
		{
			adjectives = LocString.GetStrings(typeof(DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVES));
		}
		LocString loc_string = (quality < 0) ? DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_FORMAT_NEGATIVE : DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_FORMAT_POSITIVE;
		int value = quality - DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_INDEX_OFFSET;
		value = Mathf.Clamp(value, 0, adjectives.Length);
		return string.Format(loc_string, adjectives[value], AddPositiveSign(quality.ToString(), quality > 0));
	}

	public static string GetFormattedInfomation(float amount, TimeSlice timeSlice = TimeSlice.None)
	{
		amount = ApplyTimeSlice(amount, timeSlice);
		string arg = string.Empty;
		if (amount < 1024f)
		{
			arg = UI.UNITSUFFIXES.INFORMATION.KILOBYTE;
		}
		else if (amount < 1048576f)
		{
			amount /= 1000f;
			arg = UI.UNITSUFFIXES.INFORMATION.MEGABYTE;
		}
		else if (amount < 1.07374182E+09f)
		{
			amount /= 1048576f;
			arg = UI.UNITSUFFIXES.INFORMATION.GIGABYTE;
		}
		return AddTimeSliceText(amount + arg, timeSlice);
	}

	public static LocString GetCurrentMassUnit(bool useSmallUnit = false)
	{
		LocString result = null;
		switch (massUnit)
		{
		case MassUnit.Kilograms:
			result = ((!useSmallUnit) ? UI.UNITSUFFIXES.MASS.KILOGRAM : UI.UNITSUFFIXES.MASS.GRAM);
			break;
		case MassUnit.Pounds:
			result = UI.UNITSUFFIXES.MASS.POUND;
			break;
		}
		return result;
	}

	public static string GetFormattedMass(float mass, TimeSlice timeSlice = TimeSlice.None, MetricMassFormat massFormat = MetricMassFormat.UseThreshold, bool includeSuffix = true, string floatFormat = "{0:0.#}")
	{
		if (mass == -3.40282347E+38f)
		{
			return UI.CALCULATING;
		}
		mass = ApplyTimeSlice(mass, timeSlice);
		string str;
		if (massUnit == MassUnit.Kilograms)
		{
			str = UI.UNITSUFFIXES.MASS.TONNE;
			switch (massFormat)
			{
			case MetricMassFormat.UseThreshold:
			{
				float num = Mathf.Abs(mass);
				if (0f < num)
				{
					if (num < 5E-06f)
					{
						str = UI.UNITSUFFIXES.MASS.MICROGRAM;
						mass = Mathf.Floor(mass * 1E+09f);
					}
					else if (num < 0.005f)
					{
						mass *= 1000000f;
						str = UI.UNITSUFFIXES.MASS.MILLIGRAM;
					}
					else if (Mathf.Abs(mass) < 5f)
					{
						mass *= 1000f;
						str = UI.UNITSUFFIXES.MASS.GRAM;
					}
					else if (Mathf.Abs(mass) < 5000f)
					{
						str = UI.UNITSUFFIXES.MASS.KILOGRAM;
					}
					else
					{
						mass /= 1000f;
						str = UI.UNITSUFFIXES.MASS.TONNE;
					}
				}
				else
				{
					str = UI.UNITSUFFIXES.MASS.KILOGRAM;
				}
				break;
			}
			case MetricMassFormat.Kilogram:
				str = UI.UNITSUFFIXES.MASS.KILOGRAM;
				break;
			case MetricMassFormat.Gram:
				mass *= 1000f;
				str = UI.UNITSUFFIXES.MASS.GRAM;
				break;
			case MetricMassFormat.Tonne:
				mass /= 1000f;
				str = UI.UNITSUFFIXES.MASS.TONNE;
				break;
			}
		}
		else
		{
			mass /= 2.2f;
			str = UI.UNITSUFFIXES.MASS.POUND;
			if (massFormat == MetricMassFormat.UseThreshold)
			{
				float num2 = Mathf.Abs(mass);
				if (num2 < 5f && num2 > 0.001f)
				{
					mass *= 256f;
					str = UI.UNITSUFFIXES.MASS.DRACHMA;
				}
				else
				{
					mass *= 7000f;
					str = UI.UNITSUFFIXES.MASS.GRAIN;
				}
			}
		}
		if (!includeSuffix)
		{
			str = string.Empty;
			timeSlice = TimeSlice.None;
		}
		return AddTimeSliceText(string.Format(floatFormat, mass) + str, timeSlice);
	}

	public static string GetFormattedTime(float seconds)
	{
		return string.Format(UI.FORMATSECONDS, seconds.ToString("F0"));
	}

	public static string GetFormattedEngineEfficiency(float amount)
	{
		return amount + " km /" + (string)UI.UNITSUFFIXES.MASS.KILOGRAM;
	}

	public static string GetFormattedDistance(float meters)
	{
		if (Mathf.Abs(meters) < 1f)
		{
			string text = (meters * 100f).ToString();
			string text2 = text.Substring(0, text.LastIndexOf('.') + Mathf.Min(3, text.Length - text.LastIndexOf('.')));
			if (text2 == "-0.0")
			{
				text2 = "0";
			}
			return text2 + " cm";
		}
		if (meters < 1000f)
		{
			return meters + " m";
		}
		return Util.FormatOneDecimalPlace(meters / 1000f) + " km";
	}

	public static string GetFormattedCycles(float seconds, string formatString = "F1")
	{
		if (Mathf.Abs(seconds) > 100f)
		{
			return string.Format(UI.FORMATDAY, FloatToString(seconds / 600f, formatString));
		}
		return GetFormattedTime(seconds);
	}

	public static float GetDisplaySHC(float shc)
	{
		if (temperatureUnit == TemperatureUnit.Fahrenheit)
		{
			shc /= 1.8f;
		}
		return shc;
	}

	public static string GetSHCSuffix()
	{
		return $"(DTU/g)/{GetTemperatureUnitSuffix()}";
	}

	public static string GetFormattedSHC(float shc)
	{
		shc = GetDisplaySHC(shc);
		return string.Format("{0} (DTU/g)/{1}", shc.ToString("0.000"), GetTemperatureUnitSuffix());
	}

	public static float GetDisplayThermalConductivity(float tc)
	{
		if (temperatureUnit == TemperatureUnit.Fahrenheit)
		{
			tc /= 1.8f;
		}
		return tc;
	}

	public static string GetThermalConductivitySuffix()
	{
		return $"(DTU/(m*s))/{GetTemperatureUnitSuffix()}";
	}

	public static string GetFormattedThermalConductivity(float tc)
	{
		tc = GetDisplayThermalConductivity(tc);
		return string.Format("{0} (DTU/(m*s))/{1}", tc.ToString("0.000"), GetTemperatureUnitSuffix());
	}

	public static string GetElementNameByElementHash(SimHashes elementHash)
	{
		return ElementLoader.FindElementByHash(elementHash).tag.ProperName();
	}

	public static bool HasTrait(GameObject go, string traitName)
	{
		Traits component = go.GetComponent<Traits>();
		return !((UnityEngine.Object)component == (UnityEngine.Object)null) && component.HasTrait(traitName);
	}

	public static HashSet<int> GetFloodFillCavity(int startCell, bool allowLiquid)
	{
		HashSet<int> hashSet = new HashSet<int>();
		if (!allowLiquid)
		{
			return FloodCollectCells(startCell, (int cell) => Grid.Element[cell].IsVacuum || Grid.Element[cell].IsGas, 300, null, true);
		}
		return FloodCollectCells(startCell, (int cell) => !Grid.Solid[cell], 300, null, true);
	}

	public static HashSet<int> FloodCollectCells(int start_cell, Func<int, bool> is_valid, int maxSize = 300, HashSet<int> AddInvalidCellsToSet = null, bool clearOversizedResults = true)
	{
		HashSet<int> hashSet = new HashSet<int>();
		HashSet<int> hashSet2 = new HashSet<int>();
		probeFromCell(start_cell, is_valid, hashSet, hashSet2, maxSize);
		if (AddInvalidCellsToSet != null)
		{
			AddInvalidCellsToSet.UnionWith(hashSet2);
			if (hashSet.Count > maxSize)
			{
				AddInvalidCellsToSet.UnionWith(hashSet);
			}
		}
		if (hashSet.Count > maxSize && clearOversizedResults)
		{
			hashSet.Clear();
		}
		return hashSet;
	}

	public static HashSet<int> FloodCollectCells(HashSet<int> results, int start_cell, Func<int, bool> is_valid, int maxSize = 300, HashSet<int> AddInvalidCellsToSet = null, bool clearOversizedResults = true)
	{
		HashSet<int> hashSet = new HashSet<int>();
		probeFromCell(start_cell, is_valid, results, hashSet, maxSize);
		if (AddInvalidCellsToSet != null)
		{
			AddInvalidCellsToSet.UnionWith(hashSet);
			if (results.Count > maxSize)
			{
				AddInvalidCellsToSet.UnionWith(results);
			}
		}
		if (results.Count > maxSize && clearOversizedResults)
		{
			results.Clear();
		}
		return results;
	}

	private static void probeFromCell(int start_cell, Func<int, bool> is_valid, HashSet<int> cells, HashSet<int> invalidCells, int maxSize = 300)
	{
		if (cells.Count > maxSize || !Grid.IsValidCell(start_cell) || invalidCells.Contains(start_cell) || cells.Contains(start_cell) || !is_valid(start_cell))
		{
			invalidCells.Add(start_cell);
		}
		else
		{
			cells.Add(start_cell);
			probeFromCell(Grid.CellLeft(start_cell), is_valid, cells, invalidCells, maxSize);
			probeFromCell(Grid.CellRight(start_cell), is_valid, cells, invalidCells, maxSize);
			probeFromCell(Grid.CellAbove(start_cell), is_valid, cells, invalidCells, maxSize);
			probeFromCell(Grid.CellBelow(start_cell), is_valid, cells, invalidCells, maxSize);
		}
	}

	public static bool FloodFillCheck<ArgType>(Func<int, ArgType, bool> fn, ArgType arg, int start_cell, int max_depth, bool stop_at_solid, bool stop_at_liquid)
	{
		int num = FloodFillFind(fn, arg, start_cell, max_depth, stop_at_solid, stop_at_liquid);
		return num != -1;
	}

	public static int FloodFillFind<ArgType>(Func<int, ArgType, bool> fn, ArgType arg, int start_cell, int max_depth, bool stop_at_solid, bool stop_at_liquid)
	{
		FloodFillNext.Enqueue(new FloodFillInfo
		{
			cell = start_cell,
			depth = 0
		});
		int result = -1;
		while (FloodFillNext.Count > 0)
		{
			FloodFillInfo floodFillInfo = FloodFillNext.Dequeue();
			if (floodFillInfo.depth < max_depth && Grid.IsValidCell(floodFillInfo.cell))
			{
				Element element = Grid.Element[floodFillInfo.cell];
				if ((!stop_at_solid || !element.IsSolid) && (!stop_at_liquid || !element.IsLiquid) && !FloodFillVisited.Contains(floodFillInfo.cell))
				{
					FloodFillVisited.Add(floodFillInfo.cell);
					if (fn(floodFillInfo.cell, arg))
					{
						result = floodFillInfo.cell;
						break;
					}
					FloodFillNext.Enqueue(new FloodFillInfo
					{
						cell = Grid.CellLeft(floodFillInfo.cell),
						depth = floodFillInfo.depth + 1
					});
					FloodFillNext.Enqueue(new FloodFillInfo
					{
						cell = Grid.CellRight(floodFillInfo.cell),
						depth = floodFillInfo.depth + 1
					});
					FloodFillNext.Enqueue(new FloodFillInfo
					{
						cell = Grid.CellAbove(floodFillInfo.cell),
						depth = floodFillInfo.depth + 1
					});
					FloodFillNext.Enqueue(new FloodFillInfo
					{
						cell = Grid.CellBelow(floodFillInfo.cell),
						depth = floodFillInfo.depth + 1
					});
				}
			}
		}
		FloodFillVisited.Clear();
		FloodFillNext.Clear();
		return result;
	}

	public static void FloodFillConditional(int start_cell, Func<int, bool> condition, ICollection<int> visited_cells, ICollection<int> valid_cells = null)
	{
		FloodFillNext.Enqueue(new FloodFillInfo
		{
			cell = start_cell,
			depth = 0
		});
		FloodFillConditional(FloodFillNext, condition, visited_cells, valid_cells, 10000);
	}

	public static void FloodFillConditional(Queue<FloodFillInfo> queue, Func<int, bool> condition, ICollection<int> visited_cells, ICollection<int> valid_cells = null, int max_depth = 10000)
	{
		while (queue.Count > 0)
		{
			FloodFillInfo floodFillInfo = queue.Dequeue();
			if (floodFillInfo.depth < max_depth && Grid.IsValidCell(floodFillInfo.cell) && !visited_cells.Contains(floodFillInfo.cell))
			{
				visited_cells.Add(floodFillInfo.cell);
				if (condition(floodFillInfo.cell))
				{
					valid_cells?.Add(floodFillInfo.cell);
					queue.Enqueue(new FloodFillInfo
					{
						cell = Grid.CellLeft(floodFillInfo.cell),
						depth = floodFillInfo.depth + 1
					});
					queue.Enqueue(new FloodFillInfo
					{
						cell = Grid.CellRight(floodFillInfo.cell),
						depth = floodFillInfo.depth + 1
					});
					queue.Enqueue(new FloodFillInfo
					{
						cell = Grid.CellAbove(floodFillInfo.cell),
						depth = floodFillInfo.depth + 1
					});
					queue.Enqueue(new FloodFillInfo
					{
						cell = Grid.CellBelow(floodFillInfo.cell),
						depth = floodFillInfo.depth + 1
					});
				}
			}
		}
		queue.Clear();
	}

	public static Hardness GetHardness(Element element)
	{
		if (!element.IsSolid)
		{
			return Hardness.NA;
		}
		if (element.hardness >= 255)
		{
			return Hardness.IMPENETRABLE;
		}
		if (element.hardness >= 150)
		{
			return Hardness.NEARLY_IMPENETRABLE;
		}
		if (element.hardness >= 50)
		{
			return Hardness.VERY_FIRM;
		}
		if (element.hardness >= 25)
		{
			return Hardness.FIRM;
		}
		if (element.hardness >= 10)
		{
			return Hardness.SOFT;
		}
		return Hardness.NA;
	}

	public static string GetHardnessString(Element element, bool addColor = true)
	{
		if (!element.IsSolid)
		{
			return ELEMENTS.HARDNESS.NA;
		}
		Color color = new Color(0.831372559f, 0.286274523f, 0.282352954f);
		Color color2 = new Color(0.7411765f, 0.349019617f, 0.498039216f);
		Color color3 = new Color(0.6392157f, 0.392156869f, 0.6039216f);
		Color color4 = new Color(0.5254902f, 0.419607848f, 0.647058845f);
		Color color5 = new Color(0.427450985f, 0.482352942f, 0.75686276f);
		Color color6 = new Color(0.443137258f, 0.670588255f, 0.8117647f);
		Color c = color4;
		string text = string.Empty;
		switch (GetHardness(element))
		{
		case Hardness.IMPENETRABLE:
			c = color;
			text = string.Format(ELEMENTS.HARDNESS.IMPENETRABLE, element.hardness);
			break;
		case Hardness.NEARLY_IMPENETRABLE:
			c = color2;
			text = string.Format(ELEMENTS.HARDNESS.NEARLYIMPENETRABLE, element.hardness);
			break;
		case Hardness.VERY_FIRM:
			c = color3;
			text = string.Format(ELEMENTS.HARDNESS.VERYFIRM, element.hardness);
			break;
		case Hardness.FIRM:
			c = color4;
			text = string.Format(ELEMENTS.HARDNESS.FIRM, element.hardness);
			break;
		case Hardness.SOFT:
			c = color5;
			text = string.Format(ELEMENTS.HARDNESS.SOFT, element.hardness);
			break;
		case Hardness.NA:
			c = color6;
			text = string.Format(ELEMENTS.HARDNESS.VERYSOFT, element.hardness);
			break;
		}
		if (addColor)
		{
			text = $"<color=#{c.ToHexString()}>{text}</color>";
		}
		return text;
	}

	public static string GetThermalConductivityString(Element element, bool addColor = true, bool addValue = true)
	{
		Color color = new Color(0.831372559f, 0.286274523f, 0.282352954f);
		Color color2 = new Color(0.7411765f, 0.349019617f, 0.498039216f);
		Color color3 = new Color(0.6392157f, 0.392156869f, 0.6039216f);
		Color color4 = new Color(0.5254902f, 0.419607848f, 0.647058845f);
		Color color5 = new Color(0.427450985f, 0.482352942f, 0.75686276f);
		Color color6 = color3;
		string empty = string.Empty;
		if (element.thermalConductivity >= 50f)
		{
			color6 = color5;
			empty = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VERY_HIGH_CONDUCTIVITY;
		}
		else if (element.thermalConductivity >= 10f)
		{
			color6 = color4;
			empty = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.HIGH_CONDUCTIVITY;
		}
		else if (element.thermalConductivity >= 2f)
		{
			color6 = color3;
			empty = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.MEDIUM_CONDUCTIVITY;
		}
		else if (element.thermalConductivity >= 1f)
		{
			color6 = color2;
			empty = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.LOW_CONDUCTIVITY;
		}
		else
		{
			color6 = color;
			empty = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VERY_LOW_CONDUCTIVITY;
		}
		if (addColor)
		{
			empty = $"<color=#{color6.ToHexString()}>{empty}</color>";
		}
		if (addValue)
		{
			empty = string.Format(UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VALUE_WITH_ADJECTIVE, element.thermalConductivity.ToString(), empty);
		}
		return empty;
	}

	public static string GetBreathableString(Element element, float Mass)
	{
		if (!element.IsGas && !element.IsVacuum)
		{
			return string.Empty;
		}
		Color color = new Color(0.443137258f, 0.670588255f, 0.8117647f);
		Color color2 = new Color(0.6392157f, 0.392156869f, 0.6039216f);
		Color color3 = new Color(0.831372559f, 0.286274523f, 0.282352954f);
		Color color4 = color;
		LocString arg;
		switch (element.id)
		{
		case SimHashes.Oxygen:
			if (Mass >= SimDebugView.optimallyBreathable)
			{
				color4 = color;
				arg = UI.OVERLAYS.OXYGEN.LEGEND1;
			}
			else if (Mass >= SimDebugView.minimumBreathable + (SimDebugView.optimallyBreathable - SimDebugView.minimumBreathable) / 2f)
			{
				color4 = color;
				arg = UI.OVERLAYS.OXYGEN.LEGEND2;
			}
			else if (Mass >= SimDebugView.minimumBreathable)
			{
				color4 = color2;
				arg = UI.OVERLAYS.OXYGEN.LEGEND3;
			}
			else
			{
				color4 = color3;
				arg = UI.OVERLAYS.OXYGEN.LEGEND4;
			}
			break;
		case SimHashes.ContaminatedOxygen:
			if (Mass >= 0.3f)
			{
				color4 = color2;
				arg = UI.OVERLAYS.OXYGEN.LEGEND6;
			}
			else if (Mass > 0.05f)
			{
				color4 = color2;
				arg = UI.OVERLAYS.OXYGEN.LEGEND5;
			}
			else
			{
				color4 = color3;
				arg = UI.OVERLAYS.OXYGEN.LEGEND4;
			}
			break;
		default:
			color4 = color3;
			arg = UI.OVERLAYS.OXYGEN.LEGEND4;
			break;
		}
		return string.Format(ELEMENTS.BREATHABLEDESC, color4.ToHexString(), arg);
	}

	public static string AppendHotkeyString(string template, Action action)
	{
		Color c = new Color(0.956862748f, 0.2901961f, 0.2784314f);
		return template + "<color=#" + c.ToHexString() + ">(" + GetActionString(action) + ")</color>";
	}

	public static string ReplaceHotkeyString(string template, Action action)
	{
		Color c = new Color(0.956862748f, 0.2901961f, 0.2784314f);
		return template.Replace("{Hotkey}", "<color=#" + c.ToHexString() + ">(" + GetActionString(action) + ")</color>");
	}

	public static string ReplaceHotkeyString(string template, Action action1, Action action2)
	{
		Color c = new Color(0.956862748f, 0.2901961f, 0.2784314f);
		return template.Replace("{Hotkey}", "<color=#" + c.ToHexString() + ">(" + GetActionString(action2) + ") + (" + GetActionString(action2) + ")</color>");
	}

	public static string GetKeycodeLocalized(KKeyCode key_code)
	{
		string result = key_code.ToString();
		switch (key_code)
		{
		case KKeyCode.Return:
			result = INPUT.ENTER;
			break;
		case KKeyCode.Escape:
			result = INPUT.ESCAPE;
			break;
		case KKeyCode.Backslash:
			result = "\\";
			break;
		case KKeyCode.Backspace:
			result = INPUT.BACKSPACE;
			break;
		case KKeyCode.Plus:
			result = "+";
			break;
		case KKeyCode.Slash:
			result = "/";
			break;
		case KKeyCode.Space:
			result = INPUT.SPACE;
			break;
		case KKeyCode.Tab:
			result = INPUT.TAB;
			break;
		case KKeyCode.LeftBracket:
			result = "[";
			break;
		case KKeyCode.RightBracket:
			result = "]";
			break;
		case KKeyCode.Semicolon:
			result = ";";
			break;
		case KKeyCode.Colon:
			result = ":";
			break;
		case KKeyCode.Period:
			result = INPUT.PERIOD;
			break;
		case KKeyCode.Comma:
			result = ",";
			break;
		case KKeyCode.BackQuote:
			result = INPUT.BACKQUOTE;
			break;
		case KKeyCode.MouseScrollUp:
			result = INPUT.MOUSE_SCROLL_UP;
			break;
		case KKeyCode.MouseScrollDown:
			result = INPUT.MOUSE_SCROLL_DOWN;
			break;
		case KKeyCode.Minus:
			result = "-";
			break;
		case KKeyCode.Equals:
			result = "=";
			break;
		case KKeyCode.LeftShift:
			result = INPUT.LEFT_SHIFT;
			break;
		case KKeyCode.RightShift:
			result = INPUT.RIGHT_SHIFT;
			break;
		case KKeyCode.LeftAlt:
			result = INPUT.LEFT_ALT;
			break;
		case KKeyCode.RightAlt:
			result = INPUT.RIGHT_ALT;
			break;
		case KKeyCode.LeftControl:
			result = INPUT.LEFT_CTRL;
			break;
		case KKeyCode.RightControl:
			result = INPUT.RIGHT_CTRL;
			break;
		case KKeyCode.Mouse0:
			result = INPUT.MOUSE + " 0";
			break;
		case KKeyCode.Mouse1:
			result = INPUT.MOUSE + " 1";
			break;
		case KKeyCode.Mouse2:
			result = INPUT.MOUSE + " 2";
			break;
		case KKeyCode.Mouse3:
			result = INPUT.MOUSE + " 3";
			break;
		case KKeyCode.Mouse4:
			result = INPUT.MOUSE + " 4";
			break;
		case KKeyCode.Mouse5:
			result = INPUT.MOUSE + " 5";
			break;
		case KKeyCode.Mouse6:
			result = INPUT.MOUSE + " 6";
			break;
		case KKeyCode.Keypad0:
			result = INPUT.NUM + " 0";
			break;
		case KKeyCode.Keypad1:
			result = INPUT.NUM + " 1";
			break;
		case KKeyCode.Keypad2:
			result = INPUT.NUM + " 2";
			break;
		case KKeyCode.Keypad3:
			result = INPUT.NUM + " 3";
			break;
		case KKeyCode.Keypad4:
			result = INPUT.NUM + " 4";
			break;
		case KKeyCode.Keypad5:
			result = INPUT.NUM + " 5";
			break;
		case KKeyCode.Keypad6:
			result = INPUT.NUM + " 6";
			break;
		case KKeyCode.Keypad7:
			result = INPUT.NUM + " 7";
			break;
		case KKeyCode.Keypad8:
			result = INPUT.NUM + " 8";
			break;
		case KKeyCode.Keypad9:
			result = INPUT.NUM + " 9";
			break;
		case KKeyCode.KeypadMultiply:
			result = INPUT.NUM + " *";
			break;
		case KKeyCode.KeypadPeriod:
			result = INPUT.NUM + " " + INPUT.PERIOD;
			break;
		case KKeyCode.KeypadPlus:
			result = INPUT.NUM + " +";
			break;
		case KKeyCode.KeypadMinus:
			result = INPUT.NUM + " -";
			break;
		case KKeyCode.KeypadDivide:
			result = INPUT.NUM + " /";
			break;
		case KKeyCode.KeypadEnter:
			result = INPUT.NUM + " " + INPUT.ENTER;
			break;
		default:
			if (KKeyCode.A <= key_code && key_code <= KKeyCode.Z)
			{
				result = ((ushort)(65 + (key_code - 97))).ToString();
			}
			else if (KKeyCode.Alpha0 <= key_code && key_code <= KKeyCode.Alpha9)
			{
				result = ((ushort)(48 + (key_code - 48))).ToString();
			}
			else if (KKeyCode.F1 <= key_code && key_code <= KKeyCode.F12)
			{
				result = "F" + (key_code - 282 + 1).ToString();
			}
			else
			{
				Debug.LogWarning("Unable to find proper string for KKeyCode: " + key_code.ToString() + " using key_code.ToString()");
			}
			break;
		case KKeyCode.None:
			break;
		}
		return result;
	}

	public static string GetActionString(Action action)
	{
		string empty = string.Empty;
		if (action == Action.NumActions)
		{
			return empty;
		}
		BindingEntry bindingEntry = ActionToBinding(action);
		KKeyCode mKeyCode = bindingEntry.mKeyCode;
		if (bindingEntry.mModifier == Modifier.None)
		{
			return GetKeycodeLocalized(mKeyCode).ToUpper();
		}
		string str = string.Empty;
		switch (bindingEntry.mModifier)
		{
		case Modifier.Shift:
			str = GetKeycodeLocalized(KKeyCode.LeftShift).ToUpper();
			break;
		case Modifier.Ctrl:
			str = GetKeycodeLocalized(KKeyCode.LeftControl).ToUpper();
			break;
		case Modifier.CapsLock:
			str = GetKeycodeLocalized(KKeyCode.CapsLock).ToUpper();
			break;
		case Modifier.Alt:
			str = GetKeycodeLocalized(KKeyCode.LeftAlt).ToUpper();
			break;
		}
		return str + " + " + GetKeycodeLocalized(mKeyCode).ToUpper();
	}

	public static void CreateExplosion(Vector3 explosion_pos)
	{
		Vector2 b = new Vector2(explosion_pos.x, explosion_pos.y);
		float num = 5f;
		float num2 = num * num;
		foreach (Health item in Components.Health.Items)
		{
			Vector3 position = item.transform.GetPosition();
			Vector2 a = new Vector2(position.x, position.y);
			float sqrMagnitude = (a - b).sqrMagnitude;
			if (num2 >= sqrMagnitude && (UnityEngine.Object)item != (UnityEngine.Object)null)
			{
				item.Damage(item.maxHitPoints);
			}
		}
	}

	private static void GetNonSolidCells(int x, int y, List<int> cells, int min_x, int min_y, int max_x, int max_y)
	{
		int num = Grid.XYToCell(x, y);
		if (Grid.IsValidCell(num) && !Grid.Solid[num] && !Grid.DupePassable[num] && x >= min_x && x <= max_x && y >= min_y && y <= max_y && !cells.Contains(num))
		{
			cells.Add(num);
			GetNonSolidCells(x + 1, y, cells, min_x, min_y, max_x, max_y);
			GetNonSolidCells(x - 1, y, cells, min_x, min_y, max_x, max_y);
			GetNonSolidCells(x, y + 1, cells, min_x, min_y, max_x, max_y);
			GetNonSolidCells(x, y - 1, cells, min_x, min_y, max_x, max_y);
		}
	}

	public static void GetNonSolidCells(int cell, int radius, List<int> cells)
	{
		int x = 0;
		int y = 0;
		Grid.CellToXY(cell, out x, out y);
		GetNonSolidCells(x, y, cells, x - radius, y - radius, x + radius, y + radius);
	}

	public static float GetMaxStress()
	{
		if (Components.LiveMinionIdentities.Count <= 0)
		{
			return 0f;
		}
		float num = 0f;
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			num = Mathf.Max(num, Db.Get().Amounts.Stress.Lookup(item).value);
		}
		return num;
	}

	public static float GetAverageStress()
	{
		if (Components.LiveMinionIdentities.Count <= 0)
		{
			return 0f;
		}
		float num = 0f;
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			num += Db.Get().Amounts.Stress.Lookup(item).value;
		}
		return num / (float)Components.LiveMinionIdentities.Count;
	}

	public static string MigrateFMOD(FMODAsset asset)
	{
		if ((UnityEngine.Object)asset == (UnityEngine.Object)null)
		{
			return null;
		}
		return (asset.path == null) ? asset.name : asset.path;
	}

	private static void SortDescriptors(List<IEffectDescriptor> descriptorList)
	{
		descriptorList.Sort(delegate(IEffectDescriptor e1, IEffectDescriptor e2)
		{
			int num = TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e1.GetType());
			int value = TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e2.GetType());
			return num.CompareTo(value);
		});
	}

	private static void SortGameObjectDescriptors(List<IGameObjectEffectDescriptor> descriptorList)
	{
		descriptorList.Sort(delegate(IGameObjectEffectDescriptor e1, IGameObjectEffectDescriptor e2)
		{
			int num = TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e1.GetType());
			int value = TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e2.GetType());
			return num.CompareTo(value);
		});
	}

	public static void IndentListOfDescriptors(List<Descriptor> list, int indentCount = 1)
	{
		for (int i = 0; i < list.Count; i++)
		{
			Descriptor value = list[i];
			for (int j = 0; j < indentCount; j++)
			{
				value.IncreaseIndent();
			}
			list[i] = value;
		}
	}

	public static List<Descriptor> GetAllDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IEffectDescriptor> list2 = new List<IEffectDescriptor>(def.BuildingComplete.GetComponents<IEffectDescriptor>());
		SortDescriptors(list2);
		foreach (IEffectDescriptor item in list2)
		{
			List<Descriptor> descriptors = item.GetDescriptors(def);
			if (descriptors != null)
			{
				list.AddRange(descriptors);
			}
		}
		return list;
	}

	public static List<Descriptor> GetAllDescriptors(GameObject go, bool simpleInfoScreen = false)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IGameObjectEffectDescriptor> list2 = new List<IGameObjectEffectDescriptor>(go.GetComponents<IGameObjectEffectDescriptor>());
		StateMachineController component = go.GetComponent<StateMachineController>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			list2.AddRange(component.GetDescriptors());
		}
		SortGameObjectDescriptors(list2);
		foreach (IGameObjectEffectDescriptor item in list2)
		{
			List<Descriptor> descriptors = item.GetDescriptors(go);
			if (descriptors != null)
			{
				foreach (Descriptor item2 in descriptors)
				{
					Descriptor current2 = item2;
					if (!current2.onlyForSimpleInfoScreen || simpleInfoScreen)
					{
						list.Add(current2);
					}
				}
			}
		}
		KPrefabID component2 = go.GetComponent<KPrefabID>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.AdditionalRequirements != null)
		{
			foreach (Descriptor additionalRequirement in component2.AdditionalRequirements)
			{
				Descriptor current3 = additionalRequirement;
				if (!current3.onlyForSimpleInfoScreen || simpleInfoScreen)
				{
					list.Add(current3);
				}
			}
		}
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.AdditionalEffects != null)
		{
			foreach (Descriptor additionalEffect in component2.AdditionalEffects)
			{
				Descriptor current4 = additionalEffect;
				if (!current4.onlyForSimpleInfoScreen || simpleInfoScreen)
				{
					list.Add(current4);
				}
			}
			return list;
		}
		return list;
	}

	public static List<Descriptor> GetDetailDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			Descriptor current = descriptor;
			if (current.type == Descriptor.DescriptorType.Detail)
			{
				list.Add(current);
			}
		}
		IndentListOfDescriptors(list, 1);
		return list;
	}

	public static List<Descriptor> GetRequirementDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			Descriptor current = descriptor;
			if (current.type == Descriptor.DescriptorType.Requirement)
			{
				list.Add(current);
			}
		}
		IndentListOfDescriptors(list, 1);
		return list;
	}

	public static List<Descriptor> GetEffectDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			Descriptor current = descriptor;
			if (current.type == Descriptor.DescriptorType.Effect || current.type == Descriptor.DescriptorType.DiseaseSource)
			{
				list.Add(current);
			}
		}
		IndentListOfDescriptors(list, 1);
		return list;
	}

	public static List<Descriptor> GetInformationDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			Descriptor current = descriptor;
			if (current.type == Descriptor.DescriptorType.Lifecycle)
			{
				list.Add(current);
			}
		}
		IndentListOfDescriptors(list, 1);
		return list;
	}

	public static List<Descriptor> GetCropOptimumConditionDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			Descriptor current = descriptor;
			if (current.type == Descriptor.DescriptorType.Lifecycle)
			{
				Descriptor item = current;
				item.text = "• " + item.text;
				list.Add(item);
			}
		}
		IndentListOfDescriptors(list, 1);
		return list;
	}

	public static List<Descriptor> GetGameObjectRequirements(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IGameObjectEffectDescriptor> list2 = new List<IGameObjectEffectDescriptor>(go.GetComponents<IGameObjectEffectDescriptor>());
		StateMachineController component = go.GetComponent<StateMachineController>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			list2.AddRange(component.GetDescriptors());
		}
		SortGameObjectDescriptors(list2);
		foreach (IGameObjectEffectDescriptor item in list2)
		{
			List<Descriptor> descriptors = item.GetDescriptors(go);
			if (descriptors != null)
			{
				foreach (Descriptor item2 in descriptors)
				{
					Descriptor current2 = item2;
					if (current2.type == Descriptor.DescriptorType.Requirement)
					{
						list.Add(current2);
					}
				}
			}
		}
		KPrefabID component2 = go.GetComponent<KPrefabID>();
		if (component2.AdditionalRequirements != null)
		{
			list.AddRange(component2.AdditionalRequirements);
		}
		return list;
	}

	public static List<Descriptor> GetGameObjectEffects(GameObject go, bool simpleInfoScreen = false)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IGameObjectEffectDescriptor> list2 = new List<IGameObjectEffectDescriptor>(go.GetComponents<IGameObjectEffectDescriptor>());
		StateMachineController component = go.GetComponent<StateMachineController>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			list2.AddRange(component.GetDescriptors());
		}
		SortGameObjectDescriptors(list2);
		foreach (IGameObjectEffectDescriptor item in list2)
		{
			List<Descriptor> descriptors = item.GetDescriptors(go);
			if (descriptors != null)
			{
				foreach (Descriptor item2 in descriptors)
				{
					Descriptor current2 = item2;
					if ((!current2.onlyForSimpleInfoScreen || simpleInfoScreen) && (current2.type == Descriptor.DescriptorType.Effect || current2.type == Descriptor.DescriptorType.DiseaseSource))
					{
						list.Add(current2);
					}
				}
			}
		}
		KPrefabID component2 = go.GetComponent<KPrefabID>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.AdditionalEffects != null)
		{
			foreach (Descriptor additionalEffect in component2.AdditionalEffects)
			{
				Descriptor current3 = additionalEffect;
				if (!current3.onlyForSimpleInfoScreen || simpleInfoScreen)
				{
					list.Add(current3);
				}
			}
			return list;
		}
		return list;
	}

	public static List<Descriptor> GetPlantRequirementDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<Descriptor> allDescriptors = GetAllDescriptors(go, false);
		List<Descriptor> requirementDescriptors = GetRequirementDescriptors(allDescriptors);
		if (requirementDescriptors.Count > 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.PLANTREQUIREMENTS, UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.PLANTREQUIREMENTS, Descriptor.DescriptorType.Requirement);
			list.Add(item);
			list.AddRange(requirementDescriptors);
		}
		return list;
	}

	public static List<Descriptor> GetPlantLifeCycleDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<Descriptor> informationDescriptors = GetInformationDescriptors(GetAllDescriptors(go, false));
		if (informationDescriptors.Count > 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.LIFECYCLE, UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.PLANTLIFECYCLE, Descriptor.DescriptorType.Lifecycle);
			list.Add(item);
			list.AddRange(informationDescriptors);
		}
		return list;
	}

	public static List<Descriptor> GetPlantEffectDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Growing component = go.GetComponent<Growing>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			return list;
		}
		List<Descriptor> allDescriptors = GetAllDescriptors(go, false);
		List<Descriptor> list2 = new List<Descriptor>();
		list2.AddRange(GetEffectDescriptors(allDescriptors));
		if (list2.Count > 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.PLANTEFFECTS, UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.PLANTEFFECTS, Descriptor.DescriptorType.Effect);
			list.Add(item);
			list.AddRange(list2);
		}
		return list;
	}

	public static string GetGameObjectEffectsTooltipString(GameObject go)
	{
		string text = string.Empty;
		List<Descriptor> gameObjectEffects = GetGameObjectEffects(go, false);
		if (gameObjectEffects.Count > 0)
		{
			text = text + UI.BUILDINGEFFECTS.OPERATIONEFFECTS + "\n";
		}
		foreach (Descriptor item in gameObjectEffects)
		{
			text = text + item.IndentedText() + "\n";
		}
		return text;
	}

	public static List<Descriptor> GetEquipmentEffects(EquipmentDef def)
	{
		Debug.Assert((UnityEngine.Object)def != (UnityEngine.Object)null);
		List<Descriptor> list = new List<Descriptor>();
		List<AttributeModifier> attributeModifiers = def.AttributeModifiers;
		if (attributeModifiers != null)
		{
			foreach (AttributeModifier item in attributeModifiers)
			{
				Klei.AI.Attribute attribute = Db.Get().Attributes.Get(item.AttributeId);
				string name = attribute.Name;
				string formattedString = item.GetFormattedString(null);
				string newValue = (!(item.Value >= 0f)) ? "consumed" : "produced";
				string text = UI.GAMEOBJECTEFFECTS.EQUIPMENT_MODS.text.Replace("{Attribute}", name).Replace("{Style}", newValue).Replace("{Value}", formattedString);
				list.Add(new Descriptor(text, text, Descriptor.DescriptorType.Effect, false));
			}
			return list;
		}
		return list;
	}

	public static string GetRecipeDescription(Recipe recipe)
	{
		string text = null;
		if (recipe != null)
		{
			text = recipe.recipeDescription;
		}
		if (text == null)
		{
			text = "MISSING RECIPEDESCRIPTION";
			Debug.LogWarning("Missing recipeDescription");
		}
		return text;
	}

	public static int GetCurrentCycle()
	{
		return GameClock.Instance.GetCycle() + 1;
	}

	public static GameObject GetTelepad()
	{
		if (Components.Telepads.Count > 0)
		{
			return Components.Telepads[0].gameObject;
		}
		return null;
	}

	public static GameObject KInstantiate(GameObject original, Vector3 position, Grid.SceneLayer sceneLayer, string name = null, int gameLayer = 0)
	{
		return KInstantiate(original, position, sceneLayer, null, name, gameLayer);
	}

	public static GameObject KInstantiate(GameObject original, Vector3 position, Grid.SceneLayer sceneLayer, GameObject parent, string name = null, int gameLayer = 0)
	{
		position.z = Grid.GetLayerZ(sceneLayer);
		Vector3 position2 = position;
		Quaternion identity = Quaternion.identity;
		return Util.KInstantiate(original, position2, identity, parent, name, true, gameLayer);
	}

	public static GameObject KInstantiate(GameObject original, Grid.SceneLayer sceneLayer, string name = null, int gameLayer = 0)
	{
		return KInstantiate(original, Vector3.zero, sceneLayer, name, gameLayer);
	}

	public static GameObject KInstantiate(Component original, Grid.SceneLayer sceneLayer, string name = null, int gameLayer = 0)
	{
		return KInstantiate(original.gameObject, Vector3.zero, sceneLayer, name, gameLayer);
	}

	public unsafe static void IsEmissionBlocked(int cell, out bool all_not_gaseous, out bool all_over_pressure)
	{
		int* ptr = stackalloc int[4];
		*ptr = Grid.CellBelow(cell);
		ptr[1] = Grid.CellLeft(cell);
		ptr[2] = Grid.CellRight(cell);
		ptr[3] = Grid.CellAbove(cell);
		all_not_gaseous = true;
		all_over_pressure = true;
		for (int i = 0; i < 4; i++)
		{
			int num = ptr[i];
			if (Grid.IsValidCell(num))
			{
				Element element = Grid.Element[num];
				all_not_gaseous = (all_not_gaseous && !element.IsGas && !element.IsVacuum);
				all_over_pressure = (all_over_pressure && ((!element.IsGas && !element.IsVacuum) || Grid.Mass[num] >= 1.8f));
			}
		}
	}

	public static float GetDecorAtCell(int cell)
	{
		float result = 0f;
		if (!Grid.Solid[cell])
		{
			result = Grid.Decor[cell];
			result += (float)DecorProvider.GetLightDecorBonus(cell);
		}
		return result;
	}

	public static string GetKeywordStyle(Tag tag)
	{
		Element element = ElementLoader.GetElement(tag);
		if (element == null)
		{
			if (!foodTags.Contains(tag))
			{
				if (!solidTags.Contains(tag))
				{
					return null;
				}
				return "solid";
			}
			return "food";
		}
		return GetKeywordStyle(element);
	}

	public static string GetKeywordStyle(SimHashes hash)
	{
		Element element = ElementLoader.FindElementByHash(hash);
		if (element != null)
		{
			return GetKeywordStyle(element);
		}
		return null;
	}

	public static string GetKeywordStyle(Element element)
	{
		if (element.id == SimHashes.Oxygen)
		{
			return "oxygen";
		}
		if (element.IsSolid)
		{
			return "solid";
		}
		if (element.IsLiquid)
		{
			return "liquid";
		}
		if (element.IsGas)
		{
			return "gas";
		}
		if (element.IsVacuum)
		{
			return "vacuum";
		}
		return null;
	}

	public static string GetKeywordStyle(GameObject go)
	{
		string result = string.Empty;
		Edible component = go.GetComponent<Edible>();
		Equippable component2 = go.GetComponent<Equippable>();
		MedicinalPill component3 = go.GetComponent<MedicinalPill>();
		ResearchPointObject component4 = go.GetComponent<ResearchPointObject>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			result = "food";
		}
		else if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			result = "equipment";
		}
		else if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
		{
			result = "medicine";
		}
		else if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
		{
			result = "research";
		}
		return result;
	}

	public static string GenerateRandomDuplicantName()
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string empty = string.Empty;
		bool flag = UnityEngine.Random.Range(0f, 1f) >= 0.5f;
		List<string> list = new List<string>(LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.NAME.NB)));
		list.AddRange((!flag) ? LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.NAME.FEMALE)) : LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.NAME.MALE)));
		empty = list.GetRandom();
		if (UnityEngine.Random.Range(0f, 1f) > 0.7f)
		{
			List<string> list2 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.PREFIX.NB)));
			list2.AddRange((!flag) ? LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.PREFIX.FEMALE)) : LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.PREFIX.MALE)));
			text = list2.GetRandom();
		}
		if (!string.IsNullOrEmpty(text))
		{
			text += " ";
		}
		if (UnityEngine.Random.Range(0f, 1f) >= 0.9f)
		{
			List<string> list3 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.SUFFIX.NB)));
			list3.AddRange((!flag) ? LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.SUFFIX.FEMALE)) : LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.SUFFIX.MALE)));
			text2 = list3.GetRandom();
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text2 = " " + text2;
		}
		return text + empty + text2;
	}

	public static string GenerateRandomRocketName()
	{
		string empty = string.Empty;
		string newValue = string.Empty;
		string newValue2 = string.Empty;
		string newValue3 = string.Empty;
		int num = 1;
		int num2 = 2;
		int num3 = 4;
		List<string> tList = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.NOUN)));
		empty = tList.GetRandom();
		int num4 = 0;
		if (UnityEngine.Random.value > 0.7f)
		{
			List<string> tList2 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.PREFIX)));
			newValue = tList2.GetRandom();
			num4 |= num;
		}
		if (UnityEngine.Random.value > 0.5f)
		{
			List<string> tList3 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.ADJECTIVE)));
			newValue2 = tList3.GetRandom();
			num4 |= num2;
		}
		if (UnityEngine.Random.value > 0.1f)
		{
			List<string> tList4 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.SUFFIX)));
			newValue3 = tList4.GetRandom();
			num4 |= num3;
		}
		string text = (num4 == (num | num2 | num3)) ? ((string)NAMEGEN.ROCKET.FMT_PREFIX_ADJECTIVE_NOUN_SUFFIX) : ((num4 == (num2 | num3)) ? ((string)NAMEGEN.ROCKET.FMT_ADJECTIVE_NOUN_SUFFIX) : ((num4 == (num | num3)) ? ((string)NAMEGEN.ROCKET.FMT_PREFIX_NOUN_SUFFIX) : ((num4 == num3) ? ((string)NAMEGEN.ROCKET.FMT_NOUN_SUFFIX) : ((num4 == (num | num2)) ? ((string)NAMEGEN.ROCKET.FMT_PREFIX_ADJECTIVE_NOUN) : ((num4 == num) ? ((string)NAMEGEN.ROCKET.FMT_PREFIX_NOUN) : ((num4 != num2) ? ((string)NAMEGEN.ROCKET.FMT_NOUN) : ((string)NAMEGEN.ROCKET.FMT_ADJECTIVE_NOUN)))))));
		DebugUtil.LogArgs("Rocket name bits:", Convert.ToString(num4, 2));
		return text.Replace("{Prefix}", newValue).Replace("{Adjective}", newValue2).Replace("{Noun}", empty)
			.Replace("{Suffix}", newValue3);
	}

	public static float GetThermalComfort(int cell, float tolerance = -0.0836800039f)
	{
		float num = 0f;
		Element element = ElementLoader.FindElementByHash(SimHashes.Creature);
		Element element2 = Grid.Element[cell];
		if (element2.thermalConductivity != 0f)
		{
			num = SimUtil.CalculateEnergyFlowCreatures(cell, 310.15f, element.specificHeatCapacity, element.thermalConductivity, 1f, 0.0045f);
		}
		num -= tolerance;
		return num * 1000f;
	}

	public static string GetFormattedDiseaseName(byte idx, bool color = false)
	{
		Disease disease = Db.Get().Diseases[idx];
		if (color)
		{
			return string.Format(UI.OVERLAYS.DISEASE.DISEASE_NAME_FORMAT, disease.Name, ColourToHex(disease.overlayColour));
		}
		return string.Format(UI.OVERLAYS.DISEASE.DISEASE_NAME_FORMAT_NO_COLOR, disease.Name);
	}

	public static string GetFormattedDisease(byte idx, int units, bool color = false)
	{
		if (idx != 255 && units > 0)
		{
			Disease disease = Db.Get().Diseases[idx];
			if (color)
			{
				return string.Format(UI.OVERLAYS.DISEASE.DISEASE_FORMAT, disease.Name, GetFormattedDiseaseAmount(units), ColourToHex(disease.overlayColour));
			}
			return string.Format(UI.OVERLAYS.DISEASE.DISEASE_FORMAT_NO_COLOR, disease.Name, GetFormattedDiseaseAmount(units));
		}
		return UI.OVERLAYS.DISEASE.NO_DISEASE;
	}

	public static string GetFormattedDiseaseAmount(int units)
	{
		return units.ToString("#,##0") + UI.UNITSUFFIXES.DISEASE.UNITS;
	}

	public static string ColourizeString(Color32 colour, string str)
	{
		return $"<color=#{ColourToHex(colour)}>{str}</color>";
	}

	public static string ColourToHex(Color32 colour)
	{
		return $"{colour.r:X2}{colour.g:X2}{colour.b:X2}{colour.a:X2}";
	}

	public static string GetFormattedDecor(float value, bool enforce_max = false)
	{
		string arg = string.Empty;
		LocString loc_string = (!(value > DecorMonitor.MAXIMUM_DECOR_VALUE) || !enforce_max) ? UI.OVERLAYS.DECOR.VALUE : UI.OVERLAYS.DECOR.MAXIMUM_DECOR;
		if (enforce_max)
		{
			value = Math.Min(value, DecorMonitor.MAXIMUM_DECOR_VALUE);
		}
		if (value > 0f)
		{
			arg = "+";
		}
		else if (!(value < 0f))
		{
			loc_string = UI.OVERLAYS.DECOR.VALUE_ZERO;
		}
		return string.Format(loc_string, arg, value);
	}

	public static Color GetDecorColourFromValue(int decor)
	{
		Color black = Color.black;
		float num = (float)decor / 100f;
		if (!(num > 0f))
		{
			return Color.Lerp(new Color(0.15f, 0f, 0f), new Color(1f, 0f, 0f), Mathf.Abs(num));
		}
		return Color.Lerp(new Color(0.15f, 0f, 0f), new Color(0f, 1f, 0f), Mathf.Abs(num));
	}

	public static List<Descriptor> GetMaterialDescriptors(Element element)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (element.attributeModifiers.Count > 0)
		{
			foreach (AttributeModifier attributeModifier in element.attributeModifiers)
			{
				string txt = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString(null));
				string tooltip = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString(null));
				Descriptor item = default(Descriptor);
				item.SetupDescriptor(txt, tooltip, Descriptor.DescriptorType.Effect);
				item.IncreaseIndent();
				list.Add(item);
			}
		}
		list.AddRange(GetSignificantMaterialPropertyDescriptors(element));
		return list;
	}

	public static string GetMaterialTooltips(Element element)
	{
		string str = element.tag.ProperName();
		foreach (AttributeModifier attributeModifier in element.attributeModifiers)
		{
			string name = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId).Name;
			string formattedString = attributeModifier.GetFormattedString(null);
			str = str + "\n    • " + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, name, formattedString);
		}
		return str + GetSignificantMaterialPropertyTooltips(element);
	}

	public static string GetSignificantMaterialPropertyTooltips(Element element)
	{
		string text = string.Empty;
		List<Descriptor> significantMaterialPropertyDescriptors = GetSignificantMaterialPropertyDescriptors(element);
		if (significantMaterialPropertyDescriptors.Count > 0)
		{
			text += "\n";
			for (int i = 0; i < significantMaterialPropertyDescriptors.Count; i++)
			{
				string str = text;
				Descriptor descriptor = significantMaterialPropertyDescriptors[i];
				text = str + "    • " + Util.StripTextFormatting(descriptor.text) + "\n";
			}
		}
		return text;
	}

	public static List<Descriptor> GetSignificantMaterialPropertyDescriptors(Element element)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (element.thermalConductivity > 10f)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(string.Format(ELEMENTS.MATERIAL_MODIFIERS.HIGH_THERMAL_CONDUCTIVITY, GetThermalConductivityString(element, false, false)), string.Format(ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.HIGH_THERMAL_CONDUCTIVITY, element.name, element.thermalConductivity.ToString("0.#####")), Descriptor.DescriptorType.Effect);
			item.IncreaseIndent();
			list.Add(item);
		}
		if (element.thermalConductivity < 1f)
		{
			Descriptor item2 = default(Descriptor);
			item2.SetupDescriptor(string.Format(ELEMENTS.MATERIAL_MODIFIERS.LOW_THERMAL_CONDUCTIVITY, GetThermalConductivityString(element, false, false)), string.Format(ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.LOW_THERMAL_CONDUCTIVITY, element.name, element.thermalConductivity.ToString("0.#####")), Descriptor.DescriptorType.Effect);
			item2.IncreaseIndent();
			list.Add(item2);
		}
		if (element.specificHeatCapacity <= 0.2f)
		{
			Descriptor item3 = default(Descriptor);
			item3.SetupDescriptor(ELEMENTS.MATERIAL_MODIFIERS.LOW_SPECIFIC_HEAT_CAPACITY, string.Format(ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.LOW_SPECIFIC_HEAT_CAPACITY, element.name, element.specificHeatCapacity * 1f), Descriptor.DescriptorType.Effect);
			item3.IncreaseIndent();
			list.Add(item3);
		}
		if (element.specificHeatCapacity >= 1f)
		{
			Descriptor item4 = default(Descriptor);
			item4.SetupDescriptor(ELEMENTS.MATERIAL_MODIFIERS.HIGH_SPECIFIC_HEAT_CAPACITY, string.Format(ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.HIGH_SPECIFIC_HEAT_CAPACITY, element.name, element.specificHeatCapacity * 1f), Descriptor.DescriptorType.Effect);
			item4.IncreaseIndent();
			list.Add(item4);
		}
		return list;
	}

	public static int NaturalBuildingCell(this KMonoBehaviour cmp)
	{
		return Grid.PosToCell(cmp.transform.GetPosition());
	}

	public static List<Descriptor> GetMaterialDescriptors(Tag tag)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.GetElement(tag);
		if (element != null)
		{
			if (element.attributeModifiers.Count > 0)
			{
				foreach (AttributeModifier attributeModifier in element.attributeModifiers)
				{
					string txt = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString(null));
					string tooltip = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString(null));
					Descriptor item = default(Descriptor);
					item.SetupDescriptor(txt, tooltip, Descriptor.DescriptorType.Effect);
					item.IncreaseIndent();
					list.Add(item);
				}
			}
			list.AddRange(GetSignificantMaterialPropertyDescriptors(element));
		}
		else
		{
			GameObject gameObject = Assets.TryGetPrefab(tag);
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				PrefabAttributeModifiers component = gameObject.GetComponent<PrefabAttributeModifiers>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					foreach (AttributeModifier descriptor in component.descriptors)
					{
						string txt2 = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + descriptor.AttributeId.ToUpper())), descriptor.GetFormattedString(null));
						string tooltip2 = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + descriptor.AttributeId.ToUpper())), descriptor.GetFormattedString(null));
						Descriptor item2 = default(Descriptor);
						item2.SetupDescriptor(txt2, tooltip2, Descriptor.DescriptorType.Effect);
						item2.IncreaseIndent();
						list.Add(item2);
					}
					return list;
				}
			}
		}
		return list;
	}

	public static string GetMaterialTooltips(Tag tag)
	{
		string text = tag.ProperName();
		Element element = ElementLoader.GetElement(tag);
		if (element != null)
		{
			foreach (AttributeModifier attributeModifier in element.attributeModifiers)
			{
				string name = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId).Name;
				string formattedString = attributeModifier.GetFormattedString(null);
				text = text + "\n    • " + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, name, formattedString);
			}
			text += GetSignificantMaterialPropertyTooltips(element);
		}
		else
		{
			GameObject gameObject = Assets.TryGetPrefab(tag);
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				PrefabAttributeModifiers component = gameObject.GetComponent<PrefabAttributeModifiers>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					foreach (AttributeModifier descriptor in component.descriptors)
					{
						string name2 = Db.Get().BuildingAttributes.Get(descriptor.AttributeId).Name;
						string formattedString2 = descriptor.GetFormattedString(null);
						text = text + "\n    • " + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, name2, formattedString2);
					}
					return text;
				}
			}
		}
		return text;
	}

	public static bool AreChoresUIMergeable(Chore.Precondition.Context choreA, Chore.Precondition.Context choreB)
	{
		if (choreA.chore.target.isNull || choreB.chore.target.isNull)
		{
			return false;
		}
		ChoreType choreType = choreB.chore.choreType;
		ChoreType choreType2 = choreA.chore.choreType;
		if (choreA.chore.choreType == choreB.chore.choreType && choreA.chore.target.GetComponent<KPrefabID>().PrefabTag == choreB.chore.target.GetComponent<KPrefabID>().PrefabTag)
		{
			return true;
		}
		if (choreA.chore.choreType == Db.Get().ChoreTypes.Dig && choreB.chore.choreType == Db.Get().ChoreTypes.Dig)
		{
			return true;
		}
		if (choreA.chore.choreType == Db.Get().ChoreTypes.Relax && choreB.chore.choreType == Db.Get().ChoreTypes.Relax)
		{
			return true;
		}
		if ((choreType2 == Db.Get().ChoreTypes.ReturnSuitIdle || choreType2 == Db.Get().ChoreTypes.ReturnSuitUrgent) && (choreType == Db.Get().ChoreTypes.ReturnSuitIdle || choreType == Db.Get().ChoreTypes.ReturnSuitUrgent))
		{
			return true;
		}
		if ((UnityEngine.Object)choreA.chore.target.gameObject == (UnityEngine.Object)choreB.chore.target.gameObject && choreA.chore.choreType == choreB.chore.choreType)
		{
			return true;
		}
		return false;
	}

	public static string GetChoreName(Chore chore, object choreData)
	{
		string result = string.Empty;
		if (chore.choreType == Db.Get().ChoreTypes.Fetch || chore.choreType == Db.Get().ChoreTypes.MachineFetch || chore.choreType == Db.Get().ChoreTypes.FabricateFetch || chore.choreType == Db.Get().ChoreTypes.FetchCritical || chore.choreType == Db.Get().ChoreTypes.PowerFetch)
		{
			result = chore.GetReportName(chore.gameObject.GetProperName());
		}
		else if (chore.choreType == Db.Get().ChoreTypes.StorageFetch || chore.choreType == Db.Get().ChoreTypes.FoodFetch)
		{
			FetchChore fetchChore = chore as FetchChore;
			FetchAreaChore fetchAreaChore = chore as FetchAreaChore;
			if (fetchAreaChore != null)
			{
				GameObject getFetchTarget = fetchAreaChore.GetFetchTarget;
				KMonoBehaviour kMonoBehaviour = choreData as KMonoBehaviour;
				result = (((UnityEngine.Object)getFetchTarget != (UnityEngine.Object)null) ? chore.GetReportName(getFetchTarget.GetProperName()) : ((!((UnityEngine.Object)kMonoBehaviour != (UnityEngine.Object)null)) ? chore.GetReportName(null) : chore.GetReportName(kMonoBehaviour.GetProperName())));
			}
			else if (fetchChore != null)
			{
				Pickupable fetchTarget = fetchChore.fetchTarget;
				KMonoBehaviour kMonoBehaviour2 = choreData as KMonoBehaviour;
				result = (((UnityEngine.Object)fetchTarget != (UnityEngine.Object)null) ? chore.GetReportName(fetchTarget.GetProperName()) : ((!((UnityEngine.Object)kMonoBehaviour2 != (UnityEngine.Object)null)) ? chore.GetReportName(null) : chore.GetReportName(kMonoBehaviour2.GetProperName())));
			}
		}
		else
		{
			result = chore.GetReportName(null);
		}
		return result;
	}

	public static string ChoreGroupsForChoreType(ChoreType choreType)
	{
		if (choreType.groups == null || choreType.groups.Length == 0)
		{
			return null;
		}
		string text = string.Empty;
		for (int i = 0; i < choreType.groups.Length; i++)
		{
			if (i != 0)
			{
				text += UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CHORE_GROUP_SEPARATOR;
			}
			text += choreType.groups[i].Name;
		}
		return text;
	}
}
