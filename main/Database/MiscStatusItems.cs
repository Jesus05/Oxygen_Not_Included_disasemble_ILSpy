using STRINGS;
using System;
using UnityEngine;

namespace Database
{
	public class MiscStatusItems : StatusItems
	{
		public StatusItem MarkedForDisinfection;

		public StatusItem MarkedForCompost;

		public StatusItem MarkedForCompostInStorage;

		public StatusItem PendingClear;

		public StatusItem Edible;

		public StatusItem WaitingForDig;

		public StatusItem WaitingForMop;

		public StatusItem OreMass;

		public StatusItem OreTemp;

		public StatusItem ElementalCategory;

		public StatusItem ElementalState;

		public StatusItem ElementalTemperature;

		public StatusItem ElementalMass;

		public StatusItem ElementalDisease;

		public StatusItem TreeFilterableTags;

		public StatusItem OxyRockInactive;

		public StatusItem OxyRockEmitting;

		public StatusItem OxyRockBlocked;

		public StatusItem BuriedItem;

		public StatusItem SpoutOverPressure;

		public StatusItem SpoutEmitting;

		public StatusItem SpoutPressureBuilding;

		public StatusItem SpoutIdle;

		public StatusItem SpoutDormant;

		public StatusItem OrderAttack;

		public StatusItem OrderCapture;

		public StatusItem PendingHarvest;

		public StatusItem NotMarkedForHarvest;

		public StatusItem PendingUproot;

		public StatusItem PickupableUnreachable;

		public StatusItem Prioritized;

		public StatusItem Using;

		public StatusItem Operating;

		public StatusItem Cleaning;

		public StatusItem RegionIsBlocked;

		public StatusItem NoClearLocationsAvailable;

		public StatusItem AwaitingStudy;

		public StatusItem Studied;

		public StatusItem StudiedGeyserTimeRemaining;

		public StatusItem Space;

		public MiscStatusItems(ResourceSet parent)
			: base("MiscStatusItems", parent)
		{
			CreateStatusItems();
		}

		private StatusItem CreateStatusItem(string id, string prefix, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, bool showWorldIcon = true, int status_overlays = 63486)
		{
			return Add(new StatusItem(id, prefix, icon, icon_type, notification_type, allow_multiples, render_overlay, showWorldIcon, status_overlays));
		}

		private StatusItem CreateStatusItem(string id, string name, string tooltip, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, int status_overlays = 63486)
		{
			return Add(new StatusItem(id, name, tooltip, icon, icon_type, notification_type, allow_multiples, render_overlay, status_overlays));
		}

		private void CreateStatusItems()
		{
			Edible = CreateStatusItem("Edible", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Edible.resolveStringCallback = delegate(string str, object data)
			{
				Edible edible = (Edible)data;
				str = string.Format(str, GameUtil.GetFormattedCalories(edible.Calories, GameUtil.TimeSlice.None, true));
				return str;
			};
			PendingClear = CreateStatusItem("PendingClear", "MISC", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			MarkedForCompost = CreateStatusItem("MarkedForCompost", "MISC", "status_item_pending_compost", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			MarkedForCompostInStorage = CreateStatusItem("MarkedForCompostInStorage", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			MarkedForDisinfection = CreateStatusItem("MarkedForDisinfection", "MISC", "status_item_disinfect", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.Disease.ID, true, 63486);
			NoClearLocationsAvailable = CreateStatusItem("NoClearLocationsAvailable", "MISC", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			WaitingForDig = CreateStatusItem("WaitingForDig", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			WaitingForMop = CreateStatusItem("WaitingForMop", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			OreMass = CreateStatusItem("OreMass", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			OreMass.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject2 = (GameObject)data;
				str = str.Replace("{Mass}", GameUtil.GetFormattedMass(gameObject2.GetComponent<PrimaryElement>().Mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			OreTemp = CreateStatusItem("OreTemp", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			OreTemp.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject = (GameObject)data;
				str = str.Replace("{Temp}", GameUtil.GetFormattedTemperature(gameObject.GetComponent<PrimaryElement>().Temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			ElementalState = CreateStatusItem("ElementalState", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			ElementalState.resolveStringCallback = delegate(string str, object data)
			{
				Element element2 = ((Func<Element>)data)();
				str = str.Replace("{State}", element2.GetStateString());
				return str;
			};
			ElementalCategory = CreateStatusItem("ElementalCategory", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			ElementalCategory.resolveStringCallback = delegate(string str, object data)
			{
				Element element = ((Func<Element>)data)();
				str = str.Replace("{Category}", element.GetMaterialCategoryTag().ProperName());
				return str;
			};
			ElementalTemperature = CreateStatusItem("ElementalTemperature", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			ElementalTemperature.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject6 = (CellSelectionObject)data;
				str = str.Replace("{Temp}", GameUtil.GetFormattedTemperature(cellSelectionObject6.temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			ElementalMass = CreateStatusItem("ElementalMass", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			ElementalMass.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject5 = (CellSelectionObject)data;
				str = str.Replace("{Mass}", GameUtil.GetFormattedMass(cellSelectionObject5.Mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			ElementalDisease = CreateStatusItem("ElementalDisease", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			ElementalDisease.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject4 = (CellSelectionObject)data;
				str = str.Replace("{Disease}", GameUtil.GetFormattedDisease(cellSelectionObject4.diseaseIdx, cellSelectionObject4.diseaseCount, false));
				return str;
			};
			ElementalDisease.resolveTooltipCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject3 = (CellSelectionObject)data;
				str = str.Replace("{Disease}", GameUtil.GetFormattedDisease(cellSelectionObject3.diseaseIdx, cellSelectionObject3.diseaseCount, true));
				return str;
			};
			TreeFilterableTags = CreateStatusItem("TreeFilterableTags", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			TreeFilterableTags.resolveStringCallback = delegate(string str, object data)
			{
				TreeFilterable treeFilterable = (TreeFilterable)data;
				str = str.Replace("{Tags}", treeFilterable.GetTagsAsStatus(6));
				return str;
			};
			OxyRockEmitting = CreateStatusItem("OxyRockEmitting", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			OxyRockEmitting.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject2 = (CellSelectionObject)data;
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(cellSelectionObject2.FlowRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			OxyRockBlocked = CreateStatusItem("OxyRockBlocked", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			OxyRockBlocked.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject = (CellSelectionObject)data;
				GameUtil.IsEmissionBlocked(cellSelectionObject.SelectedCell, out bool all_not_gaseous, out bool all_over_pressure);
				string newValue = null;
				if (all_not_gaseous)
				{
					newValue = MISC.STATUSITEMS.OXYROCK.NEIGHBORSBLOCKED.NAME;
				}
				else if (all_over_pressure)
				{
					newValue = MISC.STATUSITEMS.OXYROCK.OVERPRESSURE.NAME;
				}
				str = str.Replace("{BlockedString}", newValue);
				return str;
			};
			OxyRockInactive = CreateStatusItem("OxyRockInactive", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Space = CreateStatusItem("Space", "MISC", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 63486);
			BuriedItem = CreateStatusItem("BuriedItem", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			SpoutOverPressure = CreateStatusItem("SpoutOverPressure", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			SpoutOverPressure.resolveStringCallback = delegate(string str, object data)
			{
				Geyser.StatesInstance statesInstance4 = (Geyser.StatesInstance)data;
				Studyable component5 = statesInstance4.GetComponent<Studyable>();
				str = ((statesInstance4 == null || !((UnityEngine.Object)component5 != (UnityEngine.Object)null) || !component5.Studied) ? str.Replace("{StudiedDetails}", "") : str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTOVERPRESSURE.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance4.master.RemainingEruptTime(), "F1"))));
				return str;
			};
			SpoutEmitting = CreateStatusItem("SpoutEmitting", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			SpoutEmitting.resolveStringCallback = delegate(string str, object data)
			{
				Geyser.StatesInstance statesInstance3 = (Geyser.StatesInstance)data;
				Studyable component4 = statesInstance3.GetComponent<Studyable>();
				str = ((statesInstance3 == null || !((UnityEngine.Object)component4 != (UnityEngine.Object)null) || !component4.Studied) ? str.Replace("{StudiedDetails}", "") : str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTEMITTING.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance3.master.RemainingEruptTime(), "F1"))));
				return str;
			};
			SpoutPressureBuilding = CreateStatusItem("SpoutPressureBuilding", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			SpoutPressureBuilding.resolveStringCallback = delegate(string str, object data)
			{
				Geyser.StatesInstance statesInstance2 = (Geyser.StatesInstance)data;
				Studyable component3 = statesInstance2.GetComponent<Studyable>();
				str = ((statesInstance2 == null || !((UnityEngine.Object)component3 != (UnityEngine.Object)null) || !component3.Studied) ? str.Replace("{StudiedDetails}", "") : str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTPRESSUREBUILDING.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance2.master.RemainingNonEruptTime(), "F1"))));
				return str;
			};
			SpoutIdle = CreateStatusItem("SpoutIdle", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			SpoutIdle.resolveStringCallback = delegate(string str, object data)
			{
				Geyser.StatesInstance statesInstance = (Geyser.StatesInstance)data;
				Studyable component2 = statesInstance.GetComponent<Studyable>();
				str = ((statesInstance == null || !((UnityEngine.Object)component2 != (UnityEngine.Object)null) || !component2.Studied) ? str.Replace("{StudiedDetails}", "") : str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTIDLE.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance.master.RemainingNonEruptTime(), "F1"))));
				return str;
			};
			SpoutDormant = CreateStatusItem("SpoutDormant", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			OrderAttack = CreateStatusItem("OrderAttack", "MISC", "status_item_attack", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			OrderCapture = CreateStatusItem("OrderCapture", "MISC", "status_item_capture", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			PendingHarvest = CreateStatusItem("PendingHarvest", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			NotMarkedForHarvest = CreateStatusItem("NotMarkedForHarvest", "MISC", "status_item_building_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NotMarkedForHarvest.conditionalOverlayCallback = delegate(HashedString viewMode, object o)
			{
				if (!(viewMode != OverlayModes.None.ID))
				{
					return true;
				}
				return false;
			};
			PendingUproot = CreateStatusItem("PendingUproot", "MISC", "status_item_pending_uproot", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			PickupableUnreachable = CreateStatusItem("PickupableUnreachable", "MISC", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Prioritized = CreateStatusItem("Prioritized", "MISC", "status_item_prioritized", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Using = CreateStatusItem("Using", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Using.resolveStringCallback = delegate(string str, object data)
			{
				Workable workable = (Workable)data;
				if ((UnityEngine.Object)workable != (UnityEngine.Object)null)
				{
					KSelectable component = workable.GetComponent<KSelectable>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						str = str.Replace("{Target}", component.GetName());
					}
				}
				return str;
			};
			Operating = CreateStatusItem("Operating", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Cleaning = CreateStatusItem("Cleaning", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			RegionIsBlocked = CreateStatusItem("RegionIsBlocked", "MISC", "status_item_solids_blocking", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			AwaitingStudy = CreateStatusItem("AwaitingStudy", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Studied = CreateStatusItem("Studied", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
		}
	}
}
