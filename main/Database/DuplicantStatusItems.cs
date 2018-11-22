using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
	public class DuplicantStatusItems : StatusItems
	{
		public StatusItem Idle;

		public StatusItem Pacified;

		public StatusItem PendingPacification;

		public StatusItem Dead;

		public StatusItem MoveToSuitNotRequired;

		public StatusItem DroppingUnusedInventory;

		public StatusItem MovingToSafeArea;

		public StatusItem BedUnreachable;

		public StatusItem Hungry;

		public StatusItem Starving;

		public StatusItem Rotten;

		public StatusItem Quarantined;

		public StatusItem NoRationsAvailable;

		public StatusItem RationsUnreachable;

		public StatusItem RationsNotPermitted;

		public StatusItem DailyRationLimitReached;

		public StatusItem Scalding;

		public StatusItem Hot;

		public StatusItem Cold;

		public StatusItem QuarantineAreaUnassigned;

		public StatusItem QuarantineAreaUnreachable;

		public StatusItem Tired;

		public StatusItem NervousBreakdown;

		public StatusItem NoRole;

		public StatusItem Unhappy;

		public StatusItem Suffocating;

		public StatusItem HoldingBreath;

		public StatusItem ToiletUnreachable;

		public StatusItem NoUsableToilets;

		public StatusItem NoToilets;

		public StatusItem Vomiting;

		public StatusItem Coughing;

		public StatusItem BreathingO2;

		public StatusItem EmittingCO2;

		public StatusItem LowOxygen;

		public StatusItem RedAlert;

		public StatusItem Digging;

		public StatusItem Eating;

		public StatusItem Sleeping;

		public StatusItem SleepingInterruptedLight;

		public StatusItem SleepingInterrupted;

		public StatusItem SleepingPeacefully;

		public StatusItem SleepingBadly;

		public StatusItem SleepingTerribly;

		public StatusItem Cleaning;

		public StatusItem PickingUp;

		public StatusItem Mopping;

		public StatusItem Cooking;

		public StatusItem Arting;

		public StatusItem Mushing;

		public StatusItem Researching;

		public StatusItem Tinkering;

		public StatusItem Storing;

		public StatusItem Building;

		public StatusItem Equipping;

		public StatusItem WarmingUp;

		public StatusItem GeneratingPower;

		public StatusItem Harvesting;

		public StatusItem Uprooting;

		public StatusItem Emptying;

		public StatusItem Toggling;

		public StatusItem Deconstructing;

		public StatusItem Disinfecting;

		public StatusItem Relocating;

		public StatusItem Upgrading;

		public StatusItem Fabricating;

		public StatusItem Processing;

		public StatusItem Clearing;

		public StatusItem BodyRegulatingHeating;

		public StatusItem BodyRegulatingCooling;

		public StatusItem EntombedChore;

		public StatusItem EarlyMorning;

		public StatusItem NightTime;

		public StatusItem PoorDecor;

		public StatusItem PoorQualityOfLife;

		public StatusItem PoorFoodQuality;

		public StatusItem GoodFoodQuality;

		public StatusItem SevereWounds;

		public StatusItem Incapacitated;

		public StatusItem Fighting;

		public StatusItem Fleeing;

		public StatusItem Stressed;

		public StatusItem LashingOut;

		public StatusItem LowImmunity;

		public StatusItem Role;

		public StatusItem Studying;

		public StatusItem Socializing;

		public StatusItem Dancing;

		public StatusItem Gaming;

		public StatusItem Mingling;

		public DuplicantStatusItems(ResourceSet parent)
			: base("DuplicantStatusItems", parent)
		{
			CreateStatusItems();
		}

		private void CreateStatusItems()
		{
			Func<string, object, string> resolveStringCallback = delegate(string str, object data)
			{
				Workable workable3 = (Workable)data;
				if ((UnityEngine.Object)workable3 != (UnityEngine.Object)null)
				{
					str = str.Replace("{Target}", workable3.GetComponent<KSelectable>().GetName());
				}
				return str;
			};
			Func<string, object, string> resolveStringCallback2 = delegate(string str, object data)
			{
				Workable workable2 = (Workable)data;
				if ((UnityEngine.Object)workable2 != (UnityEngine.Object)null)
				{
					str = str.Replace("{Target}", workable2.GetComponent<KSelectable>().GetName());
					IHasBuildQueue hasBuildQueue = workable2 as IHasBuildQueue;
					if (hasBuildQueue != null)
					{
						using (List<IBuildQueueOrder>.Enumerator enumerator = hasBuildQueue.Orders.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								IBuildQueueOrder current = enumerator.Current;
								str = str.Replace("{Item}", current.Result.ProperName());
							}
						}
					}
				}
				return str;
			};
			BedUnreachable = new StatusItem("BedUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			BedUnreachable.AddNotification(null, null, null, 0f);
			DailyRationLimitReached = new StatusItem("DailyRationLimitReached", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			DailyRationLimitReached.AddNotification(null, null, null, 0f);
			HoldingBreath = new StatusItem("HoldingBreath", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Hungry = new StatusItem("Hungry", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Unhappy = new StatusItem("Unhappy", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Unhappy.AddNotification(null, null, null, 0f);
			NervousBreakdown = new StatusItem("NervousBreakdown", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 63486);
			NervousBreakdown.AddNotification(null, null, null, 0f);
			NoRationsAvailable = new StatusItem("NoRationsAvailable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 63486);
			PendingPacification = new StatusItem("PendingPacification", "DUPLICANTS", "status_item_pending_pacification", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			QuarantineAreaUnassigned = new StatusItem("QuarantineAreaUnassigned", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			QuarantineAreaUnassigned.AddNotification(null, null, null, 0f);
			QuarantineAreaUnreachable = new StatusItem("QuarantineAreaUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			QuarantineAreaUnreachable.AddNotification(null, null, null, 0f);
			Quarantined = new StatusItem("Quarantined", "DUPLICANTS", "status_item_quarantined", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			RationsUnreachable = new StatusItem("RationsUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			RationsUnreachable.AddNotification(null, null, null, 0f);
			RationsNotPermitted = new StatusItem("RationsNotPermitted", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			RationsNotPermitted.AddNotification(null, null, null, 0f);
			Rotten = new StatusItem("Rotten", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Starving = new StatusItem("Starving", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 63486);
			Starving.AddNotification(null, null, null, 0f);
			Suffocating = new StatusItem("Suffocating", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, false, OverlayModes.None.ID, true, 63486);
			Suffocating.AddNotification(null, null, null, 0f);
			Tired = new StatusItem("Tired", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Idle = new StatusItem("Idle", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Idle.AddNotification(null, null, null, 0f);
			Pacified = new StatusItem("Pacified", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Dead = new StatusItem("Dead", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Dead.resolveStringCallback = delegate(string str, object data)
			{
				Death death = (Death)data;
				return str.Replace("{Death}", death.Name);
			};
			MoveToSuitNotRequired = new StatusItem("MoveToSuitNotRequired", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			DroppingUnusedInventory = new StatusItem("DroppingUnusedInventory", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			MovingToSafeArea = new StatusItem("MovingToSafeArea", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			ToiletUnreachable = new StatusItem("ToiletUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			ToiletUnreachable.AddNotification(null, null, null, 0f);
			NoUsableToilets = new StatusItem("NoUsableToilets", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NoUsableToilets.AddNotification(null, null, null, 0f);
			NoRole = new StatusItem("NoRole", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NoToilets = new StatusItem("NoToilets", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NoToilets.AddNotification(null, null, null, 0f);
			BreathingO2 = new StatusItem("BreathingO2", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 130);
			BreathingO2.resolveStringCallback = delegate(string str, object data)
			{
				OxygenBreather oxygenBreather2 = (OxygenBreather)data;
				float averageRate = Game.Instance.accumulators.GetAverageRate(oxygenBreather2.O2Accumulator);
				return str.Replace("{ConsumptionRate}", GameUtil.GetFormattedMass(0f - averageRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			};
			EmittingCO2 = new StatusItem("EmittingCO2", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 130);
			EmittingCO2.resolveStringCallback = delegate(string str, object data)
			{
				OxygenBreather oxygenBreather = (OxygenBreather)data;
				return str.Replace("{EmittingRate}", GameUtil.GetFormattedMass(oxygenBreather.CO2EmitRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			};
			Vomiting = new StatusItem("Vomiting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Coughing = new StatusItem("Coughing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			LowOxygen = new StatusItem("LowOxygen", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			LowOxygen.AddNotification(null, null, null, 0f);
			RedAlert = new StatusItem("RedAlert", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Sleeping = new StatusItem("Sleeping", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Sleeping.resolveTooltipCallback = delegate(string str, object data)
			{
				if (data is SleepChore.StatesInstance)
				{
					SleepChore.StatesInstance statesInstance2 = (SleepChore.StatesInstance)data;
					string stateChangeNoiseSource = statesInstance2.stateChangeNoiseSource;
					if (!string.IsNullOrEmpty(stateChangeNoiseSource))
					{
						string text = DUPLICANTS.STATUSITEMS.SLEEPING.TOOLTIP;
						text = text.Replace("{Disturber}", stateChangeNoiseSource);
						str += text;
					}
				}
				return str;
			};
			SleepingInterrupted = new StatusItem("SleepingInterrupted", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Eating = new StatusItem("Eating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Eating.resolveStringCallback = resolveStringCallback;
			Digging = new StatusItem("Digging", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Cleaning = new StatusItem("Cleaning", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Cleaning.resolveStringCallback = resolveStringCallback;
			PickingUp = new StatusItem("PickingUp", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			PickingUp.resolveStringCallback = resolveStringCallback;
			Mopping = new StatusItem("Mopping", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Cooking = new StatusItem("Cooking", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Cooking.resolveStringCallback = resolveStringCallback2;
			Mushing = new StatusItem("Mushing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Mushing.resolveStringCallback = resolveStringCallback2;
			Researching = new StatusItem("Researching", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Researching.resolveStringCallback = delegate(string str, object data)
			{
				TechInstance activeResearch = Research.Instance.GetActiveResearch();
				if (activeResearch == null)
				{
					return str;
				}
				return str.Replace("{Tech}", activeResearch.tech.Name);
			};
			Tinkering = new StatusItem("Tinkering", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Tinkering.resolveStringCallback = delegate(string str, object data)
			{
				Tinkerable tinkerable = (Tinkerable)data;
				if (!((UnityEngine.Object)tinkerable != (UnityEngine.Object)null))
				{
					return str;
				}
				return string.Format(str, tinkerable.tinkerMaterialTag.ProperName());
			};
			Role = new StatusItem("Role", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Role.resolveStringCallback = delegate(string str, object data)
			{
				RoleConfig role = Game.Instance.roleManager.GetRole((data as MinionResume).CurrentRole);
				return str.Replace("{Role}", role.name).Replace("{Progress}", GameUtil.GetFormattedPercent(Mathf.Floor(100f * ((data as MinionResume).ExperienceByRoleID[role.id] / Game.Instance.roleManager.GetRole(role.id).experienceRequired)), GameUtil.TimeSlice.None));
			};
			Storing = new StatusItem("Storing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Storing.resolveStringCallback = delegate(string str, object data)
			{
				Workable workable = (Workable)data;
				if ((UnityEngine.Object)workable != (UnityEngine.Object)null && (UnityEngine.Object)workable.worker != (UnityEngine.Object)null)
				{
					KSelectable component = workable.GetComponent<KSelectable>();
					if ((bool)component)
					{
						str = str.Replace("{Target}", component.GetName());
					}
					Pickupable pickupable = workable.worker.workCompleteData as Pickupable;
					if ((UnityEngine.Object)workable.worker != (UnityEngine.Object)null && (bool)pickupable)
					{
						KSelectable component2 = pickupable.GetComponent<KSelectable>();
						if ((bool)component2)
						{
							str = str.Replace("{Item}", component2.GetName());
						}
					}
				}
				return str;
			};
			Building = new StatusItem("Building", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Building.resolveStringCallback = resolveStringCallback;
			Equipping = new StatusItem("Equipping", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Equipping.resolveStringCallback = resolveStringCallback;
			WarmingUp = new StatusItem("WarmingUp", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			WarmingUp.resolveStringCallback = resolveStringCallback;
			GeneratingPower = new StatusItem("GeneratingPower", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			GeneratingPower.resolveStringCallback = resolveStringCallback;
			Harvesting = new StatusItem("Harvesting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Harvesting.resolveStringCallback = resolveStringCallback;
			Uprooting = new StatusItem("Uprooting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Uprooting.resolveStringCallback = resolveStringCallback;
			Emptying = new StatusItem("Emptying", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Emptying.resolveStringCallback = resolveStringCallback;
			Toggling = new StatusItem("Toggling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Toggling.resolveStringCallback = resolveStringCallback;
			Deconstructing = new StatusItem("Deconstructing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Deconstructing.resolveStringCallback = resolveStringCallback;
			Disinfecting = new StatusItem("Disinfecting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Disinfecting.resolveStringCallback = resolveStringCallback;
			Upgrading = new StatusItem("Upgrading", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Upgrading.resolveStringCallback = resolveStringCallback;
			Fabricating = new StatusItem("Fabricating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Fabricating.resolveStringCallback = resolveStringCallback2;
			Processing = new StatusItem("Processing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Processing.resolveStringCallback = resolveStringCallback2;
			Clearing = new StatusItem("Clearing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Clearing.resolveStringCallback = resolveStringCallback;
			GeneratingPower = new StatusItem("GeneratingPower", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			GeneratingPower.resolveStringCallback = resolveStringCallback;
			Cold = new StatusItem("Cold", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Cold.resolveTooltipCallback = delegate(string str, object data)
			{
				str = str.Replace("{StressModification}", GameUtil.GetFormattedPercent(Db.Get().effects.Get("ColdAir").SelfModifiers[0].Value, GameUtil.TimeSlice.PerCycle));
				float dtu_s2 = ((ExternalTemperatureMonitor.Instance)data).temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage * 1000f;
				str = str.Replace("{currentTransferWattage}", GameUtil.GetFormattedHeatEnergyRate(dtu_s2, GameUtil.HeatEnergyFormatterUnit.Automatic));
				AttributeInstance attributeInstance2 = ((ExternalTemperatureMonitor.Instance)data).attributes.Get("ThermalConductivityBarrier");
				string formattedValue2 = attributeInstance2.GetFormattedValue();
				formattedValue2 += UI.HORIZONTAL_BR_RULE;
				for (int j = 0; j != attributeInstance2.Modifiers.Count; j++)
				{
					AttributeModifier attributeModifier2 = attributeInstance2.Modifiers[j];
					formattedValue2 = formattedValue2 + attributeModifier2.GetDescription() + " " + attributeModifier2.GetFormattedString(attributeInstance2.gameObject, false);
					formattedValue2 += "\n";
				}
				str = str.Replace("{conductivityBarrier}", formattedValue2);
				return str;
			};
			Hot = new StatusItem("Hot", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Hot.resolveTooltipCallback = delegate(string str, object data)
			{
				str = str.Replace("{StressModification}", GameUtil.GetFormattedPercent(Db.Get().effects.Get("WarmAir").SelfModifiers[0].Value, GameUtil.TimeSlice.PerCycle));
				float dtu_s = ((ExternalTemperatureMonitor.Instance)data).temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage * 1000f;
				str = str.Replace("{currentTransferWattage}", GameUtil.GetFormattedHeatEnergyRate(dtu_s, GameUtil.HeatEnergyFormatterUnit.Automatic));
				AttributeInstance attributeInstance = ((ExternalTemperatureMonitor.Instance)data).attributes.Get("ThermalConductivityBarrier");
				string formattedValue = attributeInstance.GetFormattedValue();
				formattedValue += UI.HORIZONTAL_BR_RULE;
				for (int i = 0; i != attributeInstance.Modifiers.Count; i++)
				{
					AttributeModifier attributeModifier = attributeInstance.Modifiers[i];
					formattedValue = formattedValue + attributeModifier.GetDescription() + " " + attributeModifier.GetFormattedString(attributeInstance.gameObject, false);
					formattedValue += "\n";
				}
				str = str.Replace("{conductivityBarrier}", formattedValue);
				return str;
			};
			BodyRegulatingHeating = new StatusItem("BodyRegulatingHeating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			BodyRegulatingHeating.resolveStringCallback = delegate(string str, object data)
			{
				WarmBlooded.StatesInstance statesInstance = (WarmBlooded.StatesInstance)data;
				return str.Replace("{TempDelta}", GameUtil.GetFormattedTemperature(statesInstance.TemperatureDelta, GameUtil.TimeSlice.PerSecond, GameUtil.TemperatureInterpretation.Relative, true));
			};
			BodyRegulatingCooling = new StatusItem("BodyRegulatingCooling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			BodyRegulatingCooling.resolveStringCallback = BodyRegulatingHeating.resolveStringCallback;
			EntombedChore = new StatusItem("EntombedChore", "DUPLICANTS", "status_item_entombed", StatusItem.IconType.Custom, NotificationType.DuplicantThreatening, false, OverlayModes.None.ID, true, 63486);
			EntombedChore.AddNotification(null, null, null, 0f);
			EarlyMorning = new StatusItem("EarlyMorning", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			NightTime = new StatusItem("NightTime", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			PoorDecor = new StatusItem("PoorDecor", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			PoorQualityOfLife = new StatusItem("PoorQualityOfLife", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			PoorFoodQuality = new StatusItem("PoorFoodQuality", DUPLICANTS.STATUSITEMS.POOR_FOOD_QUALITY.NAME, DUPLICANTS.STATUSITEMS.POOR_FOOD_QUALITY.TOOLTIP, "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, 63486);
			GoodFoodQuality = new StatusItem("GoodFoodQuality", DUPLICANTS.STATUSITEMS.GOOD_FOOD_QUALITY.NAME, DUPLICANTS.STATUSITEMS.GOOD_FOOD_QUALITY.TOOLTIP, "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, 63486);
			Arting = new StatusItem("Arting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Arting.resolveStringCallback = resolveStringCallback;
			SevereWounds = new StatusItem("SevereWounds", "DUPLICANTS", "status_item_broken", StatusItem.IconType.Custom, NotificationType.Bad, false, OverlayModes.None.ID, true, 63486);
			SevereWounds.AddNotification(null, null, null, 0f);
			Incapacitated = new StatusItem("Incapacitated", "DUPLICANTS", "status_item_broken", StatusItem.IconType.Custom, NotificationType.DuplicantThreatening, false, OverlayModes.None.ID, true, 63486);
			Incapacitated.AddNotification(null, null, null, 0f);
			Incapacitated.resolveStringCallback = delegate(string str, object data)
			{
				IncapacitationMonitor.Instance instance = (IncapacitationMonitor.Instance)data;
				float bleedLifeTime = instance.GetBleedLifeTime(instance);
				str = str.Replace("{CauseOfIncapacitation}", instance.GetCauseOfIncapacitation().Name);
				return str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedTime(bleedLifeTime));
			};
			Relocating = new StatusItem("Relocating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Relocating.resolveStringCallback = resolveStringCallback;
			Fighting = new StatusItem("Fighting", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 63486);
			Fighting.AddNotification(null, null, null, 0f);
			Fleeing = new StatusItem("Fleeing", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 63486);
			Fleeing.AddNotification(null, null, null, 0f);
			Stressed = new StatusItem("Stressed", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Stressed.AddNotification(null, null, null, 0f);
			LashingOut = new StatusItem("LashingOut", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 63486);
			LashingOut.AddNotification(null, null, null, 0f);
			LowImmunity = new StatusItem("LowImmunity", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			LowImmunity.AddNotification(null, null, null, 0f);
			Studying = new StatusItem("Studying", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Socializing = new StatusItem("Socializing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 63486);
			Dancing = new StatusItem("Dancing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 63486);
			Gaming = new StatusItem("Gaming", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 63486);
			Mingling = new StatusItem("Mingling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 63486);
		}
	}
}
