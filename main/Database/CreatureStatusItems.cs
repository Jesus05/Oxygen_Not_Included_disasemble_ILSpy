using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Database
{
	public class CreatureStatusItems : StatusItems
	{
		public StatusItem HealthStatus;

		public StatusItem Hot;

		public StatusItem Hot_Crop;

		public StatusItem Scalding;

		public StatusItem Cold;

		public StatusItem Cold_Crop;

		public StatusItem Crop_Too_Dark;

		public StatusItem Crop_Too_Bright;

		public StatusItem Hypothermia;

		public StatusItem Hyperthermia;

		public StatusItem Suffocating;

		public StatusItem Hatching;

		public StatusItem Incubating;

		public StatusItem Drowning;

		public StatusItem Saturated;

		public StatusItem DryingOut;

		public StatusItem Growing;

		public StatusItem CropSleeping;

		public StatusItem ReadyForHarvest;

		public StatusItem EnvironmentTooWarm;

		public StatusItem EnvironmentTooCold;

		public StatusItem Entombed;

		public StatusItem Wilting;

		public StatusItem WiltingDomestic;

		public StatusItem WiltingNonGrowing;

		public StatusItem WiltingNonGrowingDomestic;

		public StatusItem WrongAtmosphere;

		public StatusItem AtmosphericPressureTooLow;

		public StatusItem AtmosphericPressureTooHigh;

		public StatusItem Barren;

		public StatusItem NeedsFertilizer;

		public StatusItem NeedsIrrigation;

		public StatusItem WrongTemperature;

		public StatusItem WrongFertilizer;

		public StatusItem WrongIrrigation;

		public StatusItem WrongFertilizerMajor;

		public StatusItem WrongIrrigationMajor;

		public StatusItem CantAcceptFertilizer;

		public StatusItem CantAcceptIrrigation;

		public StatusItem Rotting;

		public StatusItem Fresh;

		public StatusItem Stale;

		public StatusItem Spoiled;

		public StatusItem Refrigerated;

		public StatusItem Unrefrigerated;

		public StatusItem SterilizingAtmosphere;

		public StatusItem ContaminatedAtmosphere;

		public StatusItem Old;

		public StatusItem ExchangingElementOutput;

		public StatusItem ExchangingElementConsume;

		public StatusItem Hungry;

		public CreatureStatusItems(ResourceSet parent)
			: base("CreatureStatusItems", parent)
		{
			CreateStatusItems();
		}

		private void CreateStatusItems()
		{
			Hot = new StatusItem("Hot", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			Hot.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable4 = (TemperatureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(temperatureVulnerable4.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(temperatureVulnerable4.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
			};
			Hot_Crop = new StatusItem("Hot_Crop", "CREATURES", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			Hot_Crop.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable3 = (TemperatureVulnerable)data;
				str = str.Replace("{low_temperature}", GameUtil.GetFormattedTemperature(temperatureVulnerable3.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("{high_temperature}", GameUtil.GetFormattedTemperature(temperatureVulnerable3.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			Scalding = new StatusItem("Scalding", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, true, OverlayModes.None.ID, true, 129022);
			Scalding.resolveTooltipCallback = delegate(string str, object data)
			{
				float averageExternalTemperature = ((ExternalTemperatureMonitor.Instance)data).AverageExternalTemperature;
				float scaldingThreshold = ((ExternalTemperatureMonitor.Instance)data).GetScaldingThreshold();
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(averageExternalTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(scaldingThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			Scalding.AddNotification(null, null, null, 0f);
			Cold = new StatusItem("Cold", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			Cold.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable2 = (TemperatureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(temperatureVulnerable2.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(temperatureVulnerable2.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
			};
			Cold_Crop = new StatusItem("Cold_Crop", "CREATURES", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			Cold_Crop.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable = (TemperatureVulnerable)data;
				str = str.Replace("low_temperature", GameUtil.GetFormattedTemperature(temperatureVulnerable.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("high_temperature", GameUtil.GetFormattedTemperature(temperatureVulnerable.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			Crop_Too_Dark = new StatusItem("Crop_Too_Dark", "CREATURES", "status_item_plant_light", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			Crop_Too_Dark.resolveStringCallback = ((string str, object data) => str);
			Crop_Too_Bright = new StatusItem("Crop_Too_Bright", "CREATURES", "status_item_plant_light", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			Crop_Too_Bright.resolveStringCallback = ((string str, object data) => str);
			Hyperthermia = new StatusItem("Hyperthermia", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 129022);
			Hyperthermia.resolveTooltipCallback = delegate(string str, object data)
			{
				float value2 = ((TemperatureMonitor.Instance)data).temperature.value;
				float hyperthermiaThreshold = ((TemperatureMonitor.Instance)data).HyperthermiaThreshold;
				str = str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(value2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(hyperthermiaThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			Hypothermia = new StatusItem("Hypothermia", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 129022);
			Hypothermia.resolveTooltipCallback = delegate(string str, object data)
			{
				float value = ((TemperatureMonitor.Instance)data).temperature.value;
				float hypothermiaThreshold = ((TemperatureMonitor.Instance)data).HypothermiaThreshold;
				str = str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(hypothermiaThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			Suffocating = new StatusItem("Suffocating", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Hatching = new StatusItem("Hatching", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Incubating = new StatusItem("Incubating", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Drowning = new StatusItem("Drowning", "CREATURES", "status_item_flooded", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Drowning.resolveStringCallback = ((string str, object data) => str);
			Saturated = new StatusItem("Saturated", "CREATURES", "status_item_flooded", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Saturated.resolveStringCallback = ((string str, object data) => str);
			DryingOut = new StatusItem("DryingOut", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 1026);
			DryingOut.resolveStringCallback = ((string str, object data) => str);
			ReadyForHarvest = new StatusItem("ReadyForHarvest", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 1026);
			Growing = new StatusItem("Growing", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 1026);
			Growing.resolveStringCallback = delegate(string str, object data)
			{
				Crop component = ((Growing)data).GetComponent<Crop>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					float seconds = ((Growing)data).TimeUntilNextHarvest();
					str = str.Replace("{TimeUntilNextHarvest}", GameUtil.GetFormattedCycles(seconds, "F1"));
				}
				float val = 100f * ((Growing)data).PercentGrown();
				str = str.Replace("{PercentGrow}", Math.Floor((double)Math.Max(val, 0f)).ToString("F0"));
				return str;
			};
			CropSleeping = new StatusItem("Crop_Sleeping", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 1026);
			CropSleeping.resolveStringCallback = delegate(string str, object data)
			{
				CropSleepingMonitor.Instance instance8 = (CropSleepingMonitor.Instance)data;
				return str.Replace("{REASON}", (!instance8.def.prefersDarkness) ? CREATURES.STATUSITEMS.CROP_SLEEPING.REASON_TOO_DARK : CREATURES.STATUSITEMS.CROP_SLEEPING.REASON_TOO_BRIGHT);
			};
			CropSleeping.resolveTooltipCallback = delegate(string str, object data)
			{
				CropSleepingMonitor.Instance instance7 = (CropSleepingMonitor.Instance)data;
				string newValue4 = string.Format(CREATURES.STATUSITEMS.CROP_SLEEPING.REQUIREMENT_LUMINANCE, instance7.def.lightIntensityThreshold);
				return str.Replace("{REQUIREMENTS}", newValue4);
			};
			EnvironmentTooWarm = new StatusItem("EnvironmentTooWarm", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			EnvironmentTooWarm.resolveStringCallback = delegate(string str, object data)
			{
				float temp3 = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float temp4 = ((TemperatureVulnerable)data).internalTemperatureLethal_High - 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(temp3, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(temp4, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			EnvironmentTooCold = new StatusItem("EnvironmentTooCold", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			EnvironmentTooCold.resolveStringCallback = delegate(string str, object data)
			{
				float temp = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float temp2 = ((TemperatureVulnerable)data).internalTemperatureLethal_Low + 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(temp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(temp2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			Entombed = new StatusItem("Entombed", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Entombed.resolveStringCallback = ((string str, object go) => str);
			Entombed.resolveTooltipCallback = delegate(string str, object go)
			{
				GameObject go2 = go as GameObject;
				return string.Format(str, GameUtil.GetIdentityDescriptor(go2));
			};
			Wilting = new StatusItem("Wilting", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 1026);
			Wilting.resolveStringCallback = delegate(string str, object data)
			{
				if (data is Growing && data != null)
				{
					str = str.Replace("{TimeUntilNextHarvest}", GameUtil.GetFormattedCycles(Mathf.Min(((Growing)data).growthTime, ((Growing)data).TimeUntilNextHarvest()), "F1"));
				}
				str = str.Replace("{Reasons}", (data as KMonoBehaviour).GetComponent<WiltCondition>().WiltCausesString());
				return str;
			};
			WiltingDomestic = new StatusItem("WiltingDomestic", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 1026);
			WiltingDomestic.resolveStringCallback = delegate(string str, object data)
			{
				if (data is Growing && data != null)
				{
					str = str.Replace("{TimeUntilNextHarvest}", GameUtil.GetFormattedCycles(Mathf.Min(((Growing)data).growthTime, ((Growing)data).TimeUntilNextHarvest()), "F1"));
				}
				str = str.Replace("{Reasons}", (data as KMonoBehaviour).GetComponent<WiltCondition>().WiltCausesString());
				return str;
			};
			WiltingNonGrowing = new StatusItem("WiltingNonGrowing", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 1026);
			WiltingNonGrowing.resolveStringCallback = delegate(string str, object data)
			{
				str = CREATURES.STATUSITEMS.WILTING_NON_GROWING_PLANT.NAME;
				str = str.Replace("{Reasons}", (data as WiltCondition).WiltCausesString());
				return str;
			};
			WiltingNonGrowingDomestic = new StatusItem("WiltingNonGrowing", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 1026);
			WiltingNonGrowingDomestic.resolveStringCallback = delegate(string str, object data)
			{
				str = CREATURES.STATUSITEMS.WILTING_NON_GROWING_PLANT.NAME;
				str = str.Replace("{Reasons}", (data as WiltCondition).WiltCausesString());
				return str;
			};
			WrongAtmosphere = new StatusItem("WrongAtmosphere", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			WrongAtmosphere.resolveStringCallback = delegate(string str, object data)
			{
				string text = string.Empty;
				foreach (Element safe_atmosphere in (data as PressureVulnerable).safe_atmospheres)
				{
					text = text + "\n    •  " + safe_atmosphere.name;
				}
				str = str.Replace("{elements}", text);
				return str;
			};
			AtmosphericPressureTooLow = new StatusItem("AtmosphericPressureTooLow", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			AtmosphericPressureTooLow.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable2 = (PressureVulnerable)data;
				str = str.Replace("{low_mass}", GameUtil.GetFormattedMass(pressureVulnerable2.pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				str = str.Replace("{high_mass}", GameUtil.GetFormattedMass(pressureVulnerable2.pressureWarning_High, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			AtmosphericPressureTooHigh = new StatusItem("AtmosphericPressureTooHigh", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			AtmosphericPressureTooHigh.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable = (PressureVulnerable)data;
				str = str.Replace("{low_mass}", GameUtil.GetFormattedMass(pressureVulnerable.pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				str = str.Replace("{high_mass}", GameUtil.GetFormattedMass(pressureVulnerable.pressureWarning_High, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			HealthStatus = new StatusItem("HealthStatus", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			HealthStatus.resolveStringCallback = delegate(string str, object data)
			{
				string newValue3 = string.Empty;
				switch ((Health.HealthState)data)
				{
				case Health.HealthState.Perfect:
					newValue3 = MISC.STATUSITEMS.HEALTHSTATUS.PERFECT.NAME;
					break;
				case Health.HealthState.Scuffed:
					newValue3 = MISC.STATUSITEMS.HEALTHSTATUS.SCUFFED.NAME;
					break;
				case Health.HealthState.Injured:
					newValue3 = MISC.STATUSITEMS.HEALTHSTATUS.INJURED.NAME;
					break;
				case Health.HealthState.Critical:
					newValue3 = MISC.STATUSITEMS.HEALTHSTATUS.CRITICAL.NAME;
					break;
				case Health.HealthState.Incapacitated:
					newValue3 = MISC.STATUSITEMS.HEALTHSTATUS.INCAPACITATED.NAME;
					break;
				case Health.HealthState.Dead:
					newValue3 = MISC.STATUSITEMS.HEALTHSTATUS.DEAD.NAME;
					break;
				}
				str = str.Replace("{healthState}", newValue3);
				return str;
			};
			HealthStatus.resolveTooltipCallback = delegate(string str, object data)
			{
				string newValue2 = string.Empty;
				switch ((Health.HealthState)data)
				{
				case Health.HealthState.Perfect:
					newValue2 = MISC.STATUSITEMS.HEALTHSTATUS.PERFECT.TOOLTIP;
					break;
				case Health.HealthState.Scuffed:
					newValue2 = MISC.STATUSITEMS.HEALTHSTATUS.SCUFFED.TOOLTIP;
					break;
				case Health.HealthState.Injured:
					newValue2 = MISC.STATUSITEMS.HEALTHSTATUS.INJURED.TOOLTIP;
					break;
				case Health.HealthState.Critical:
					newValue2 = MISC.STATUSITEMS.HEALTHSTATUS.CRITICAL.TOOLTIP;
					break;
				case Health.HealthState.Incapacitated:
					newValue2 = MISC.STATUSITEMS.HEALTHSTATUS.INCAPACITATED.TOOLTIP;
					break;
				case Health.HealthState.Dead:
					newValue2 = MISC.STATUSITEMS.HEALTHSTATUS.DEAD.TOOLTIP;
					break;
				}
				str = str.Replace("{healthState}", newValue2);
				return str;
			};
			Barren = new StatusItem("Barren", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NeedsFertilizer = new StatusItem("NeedsFertilizer", "CREATURES", "status_item_plant_solid", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			Func<string, object, string> resolveStringCallback = (string str, object data) => str;
			NeedsFertilizer.resolveStringCallback = resolveStringCallback;
			NeedsIrrigation = new StatusItem("NeedsIrrigation", "CREATURES", "status_item_plant_liquid", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			Func<string, object, string> resolveStringCallback2 = (string str, object data) => str;
			NeedsIrrigation.resolveStringCallback = resolveStringCallback2;
			WrongFertilizer = new StatusItem("WrongFertilizer", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Func<string, object, string> resolveStringCallback3 = (string str, object data) => str;
			WrongFertilizer.resolveStringCallback = resolveStringCallback3;
			WrongFertilizerMajor = new StatusItem("WrongFertilizerMajor", "CREATURES", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			WrongFertilizerMajor.resolveStringCallback = resolveStringCallback3;
			WrongIrrigation = new StatusItem("WrongIrrigation", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Func<string, object, string> resolveStringCallback4 = (string str, object data) => str;
			WrongIrrigation.resolveStringCallback = resolveStringCallback4;
			WrongIrrigationMajor = new StatusItem("WrongIrrigationMajor", "CREATURES", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			WrongIrrigationMajor.resolveStringCallback = resolveStringCallback4;
			CantAcceptFertilizer = new StatusItem("CantAcceptFertilizer", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Rotting = new StatusItem("Rotting", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Rotting.resolveStringCallback = ((string str, object data) => str.Replace("{RotTemperature}", GameUtil.GetFormattedTemperature(277.15f, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
			Fresh = new StatusItem("Fresh", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Fresh.resolveStringCallback = delegate(string str, object data)
			{
				Rottable.Instance instance6 = (Rottable.Instance)data;
				return str.Replace("{RotPercentage}", "(" + Util.FormatWholeNumber(instance6.RotConstitutionPercentage * 100f) + "%)");
			};
			Fresh.resolveTooltipCallback = delegate(string str, object data)
			{
				Rottable.Instance instance5 = (Rottable.Instance)data;
				return str.Replace("{RotTooltip}", instance5.GetToolTip());
			};
			Stale = new StatusItem("Stale", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Stale.resolveStringCallback = delegate(string str, object data)
			{
				Rottable.Instance instance4 = (Rottable.Instance)data;
				return str.Replace("{RotPercentage}", "(" + Util.FormatWholeNumber(instance4.RotConstitutionPercentage * 100f) + "%)");
			};
			Stale.resolveTooltipCallback = delegate(string str, object data)
			{
				Rottable.Instance instance3 = (Rottable.Instance)data;
				return str.Replace("{RotTooltip}", instance3.GetToolTip());
			};
			Spoiled = new StatusItem("Spoiled", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Refrigerated = new StatusItem("Refrigerated", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Unrefrigerated = new StatusItem("Unrefrigerated", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Unrefrigerated.resolveStringCallback = ((string str, object data) => str.Replace("{RotTemperature}", GameUtil.GetFormattedTemperature(277.15f, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
			SterilizingAtmosphere = new StatusItem("SterilizingAtmosphere", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			ContaminatedAtmosphere = new StatusItem("ContaminatedAtmosphere", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Old = new StatusItem("Old", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Old.resolveTooltipCallback = delegate(string str, object data)
			{
				AgeMonitor.Instance instance2 = (AgeMonitor.Instance)data;
				return str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedCycles(instance2.CyclesUntilDeath * 600f, "F1"));
			};
			ExchangingElementConsume = new StatusItem("ExchangingElementConsume", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			ExchangingElementConsume.resolveStringCallback = delegate(string str, object data)
			{
				EntityElementExchanger.StatesInstance statesInstance4 = (EntityElementExchanger.StatesInstance)data;
				str = str.Replace("{ConsumeElement}", ElementLoader.FindElementByHash(statesInstance4.master.consumedElement).tag.ProperName());
				str = str.Replace("{ConsumeRate}", GameUtil.GetFormattedMass(statesInstance4.master.consumeRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			ExchangingElementConsume.resolveTooltipCallback = delegate(string str, object data)
			{
				EntityElementExchanger.StatesInstance statesInstance3 = (EntityElementExchanger.StatesInstance)data;
				str = str.Replace("{ConsumeElement}", ElementLoader.FindElementByHash(statesInstance3.master.consumedElement).tag.ProperName());
				str = str.Replace("{ConsumeRate}", GameUtil.GetFormattedMass(statesInstance3.master.consumeRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			ExchangingElementOutput = new StatusItem("ExchangingElementOutput", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			ExchangingElementOutput.resolveStringCallback = delegate(string str, object data)
			{
				EntityElementExchanger.StatesInstance statesInstance2 = (EntityElementExchanger.StatesInstance)data;
				str = str.Replace("{OutputElement}", ElementLoader.FindElementByHash(statesInstance2.master.emittedElement).tag.ProperName());
				str = str.Replace("{OutputRate}", GameUtil.GetFormattedMass(statesInstance2.master.consumeRate * statesInstance2.master.exchangeRatio, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			ExchangingElementOutput.resolveTooltipCallback = delegate(string str, object data)
			{
				EntityElementExchanger.StatesInstance statesInstance = (EntityElementExchanger.StatesInstance)data;
				str = str.Replace("{OutputElement}", ElementLoader.FindElementByHash(statesInstance.master.emittedElement).tag.ProperName());
				str = str.Replace("{OutputRate}", GameUtil.GetFormattedMass(statesInstance.master.consumeRate * statesInstance.master.exchangeRatio, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			Hungry = new StatusItem("Hungry", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Hungry.resolveTooltipCallback = delegate(string str, object data)
			{
				CreatureCalorieMonitor.Instance instance = (CreatureCalorieMonitor.Instance)data;
				CreatureCalorieMonitor.Def def = instance.master.gameObject.GetDef<CreatureCalorieMonitor.Def>();
				Diet diet = def.diet;
				if (diet.consumedTags.Count > 0)
				{
					string[] array = (from t in diet.consumedTags
					select t.Key.ProperName()).ToArray();
					if (array.Length > 3)
					{
						string[] array2 = new string[4]
						{
							array[0],
							array[1],
							array[2],
							"..."
						};
						array = array2;
					}
					string newValue = string.Join(", ", array);
					return str + "\n" + UI.BUILDINGEFFECTS.DIET_CONSUMED.text.Replace("{Foodlist}", newValue);
				}
				return str;
			};
		}
	}
}
