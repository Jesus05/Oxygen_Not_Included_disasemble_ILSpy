using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Database
{
	public class BuildingStatusItems : StatusItems
	{
		public MaterialsStatusItem MaterialsUnavailable;

		public MaterialsStatusItem MaterialsUnavailableForRefill;

		public StatusItem AngerDamage;

		public StatusItem ClinicOutsideHospital;

		public StatusItem DigUnreachable;

		public StatusItem MopUnreachable;

		public StatusItem ConstructableDigUnreachable;

		public StatusItem ConstructionUnreachable;

		public StatusItem NewDuplicantsAvailable;

		public StatusItem NeedPlant;

		public StatusItem NeedPower;

		public StatusItem NotEnoughPower;

		public StatusItem NeedLiquidIn;

		public StatusItem NeedGasIn;

		public StatusItem NeedResourceMass;

		public StatusItem NeedSolidIn;

		public StatusItem NeedLiquidOut;

		public StatusItem NeedGasOut;

		public StatusItem NeedSolidOut;

		public StatusItem InvalidBuildingLocation;

		public StatusItem PendingDeconstruction;

		public StatusItem PendingSwitchToggle;

		public StatusItem GasVentObstructed;

		public StatusItem LiquidVentObstructed;

		public StatusItem LiquidPipeEmpty;

		public StatusItem LiquidPipeObstructed;

		public StatusItem GasPipeEmpty;

		public StatusItem GasPipeObstructed;

		public StatusItem SolidPipeObstructed;

		public StatusItem PartiallyDamaged;

		public StatusItem Broken;

		public StatusItem PendingRepair;

		public StatusItem PendingUpgrade;

		public StatusItem RequiresSkillPerk;

		public StatusItem DigRequiresSkillPerk;

		public StatusItem ColonyLacksRequiredSkillPerk;

		public StatusItem PendingWork;

		public StatusItem Flooded;

		public StatusItem PowerButtonOff;

		public StatusItem SwitchStatusActive;

		public StatusItem SwitchStatusInactive;

		public StatusItem ChangeDoorControlState;

		public StatusItem CurrentDoorControlState;

		public StatusItem Entombed;

		public MaterialsStatusItem WaitingForMaterials;

		public StatusItem WaitingForRepairMaterials;

		public StatusItem MissingFoundation;

		public StatusItem NeutroniumUnminable;

		public StatusItem NoStorageFilterSet;

		public StatusItem PendingFish;

		public StatusItem NoFishableWaterBelow;

		public StatusItem GasVentOverPressure;

		public StatusItem LiquidVentOverPressure;

		public StatusItem NoWireConnected;

		public StatusItem NoLogicWireConnected;

		public StatusItem NoTubeConnected;

		public StatusItem NoTubeExits;

		public StatusItem StoredCharge;

		public StatusItem NoPowerConsumers;

		public StatusItem PressureOk;

		public StatusItem UnderPressure;

		public StatusItem AssignedTo;

		public StatusItem Unassigned;

		public StatusItem AssignedPublic;

		public StatusItem AssignedToRoom;

		public StatusItem RationBoxContents;

		public StatusItem ConduitBlocked;

		public StatusItem OutputPipeFull;

		public StatusItem ConduitBlockedMultiples;

		public StatusItem MeltingDown;

		public StatusItem UnderConstruction;

		public StatusItem UnderConstructionNoWorker;

		public StatusItem Normal;

		public StatusItem ManualGeneratorChargingUp;

		public StatusItem ManualGeneratorReleasingEnergy;

		public StatusItem GeneratorOffline;

		public StatusItem Pipe;

		public StatusItem Conveyor;

		public StatusItem FabricatorIdle;

		public StatusItem FabricatorEmpty;

		public StatusItem FlushToilet;

		public StatusItem FlushToiletInUse;

		public StatusItem Toilet;

		public StatusItem ToiletNeedsEmptying;

		public StatusItem DesalinatorNeedsEmptying;

		public StatusItem Unusable;

		public StatusItem NoResearchSelected;

		public StatusItem NoApplicableResearchSelected;

		public StatusItem NoApplicableAnalysisSelected;

		public StatusItem NoResearchOrDestinationSelected;

		public StatusItem Researching;

		public StatusItem ValveRequest;

		public StatusItem EmittingLight;

		public StatusItem EmittingElement;

		public StatusItem EmittingOxygenAvg;

		public StatusItem EmittingGasAvg;

		public StatusItem PumpingLiquidOrGas;

		public StatusItem NoLiquidElementToPump;

		public StatusItem NoGasElementToPump;

		public StatusItem PipeFull;

		public StatusItem PipeMayMelt;

		public StatusItem ElementConsumer;

		public StatusItem ElementEmitterOutput;

		public StatusItem AwaitingWaste;

		public StatusItem AwaitingCompostFlip;

		public StatusItem JoulesAvailable;

		public StatusItem Wattage;

		public StatusItem SolarPanelWattage;

		public StatusItem SteamTurbineWattage;

		public StatusItem Wattson;

		public StatusItem WireConnected;

		public StatusItem WireNominal;

		public StatusItem WireDisconnected;

		public StatusItem Cooling;

		public StatusItem CoolingStalledHotEnv;

		public StatusItem CoolingStalledColdGas;

		public StatusItem CoolingStalledHotLiquid;

		public StatusItem CoolingStalledColdLiquid;

		public StatusItem Working;

		public StatusItem CannotCoolFurther;

		public StatusItem NeedsValidRegion;

		public StatusItem NeedSeed;

		public StatusItem AwaitingSeedDelivery;

		public StatusItem AwaitingBaitDelivery;

		public StatusItem NoAvailableSeed;

		public StatusItem NeedEgg;

		public StatusItem AwaitingEggDelivery;

		public StatusItem NoAvailableEgg;

		public StatusItem Grave;

		public StatusItem GraveEmpty;

		public StatusItem NoFilterElementSelected;

		public StatusItem NoLureElementSelected;

		public StatusItem BuildingDisabled;

		public StatusItem Overheated;

		public StatusItem Overloaded;

		public StatusItem Expired;

		public StatusItem PumpingStation;

		public StatusItem EmptyPumpingStation;

		public StatusItem GeneShuffleCompleted;

		public StatusItem DirectionControl;

		public StatusItem WellPressurizing;

		public StatusItem WellOverpressure;

		public StatusItem ReleasingPressure;

		public StatusItem NoSuitMarker;

		public StatusItem SuitMarkerWrongSide;

		public StatusItem SuitMarkerTraversalAnytime;

		public StatusItem SuitMarkerTraversalOnlyWhenRoomAvailable;

		public StatusItem TooCold;

		public StatusItem NotInAnyRoom;

		public StatusItem NotInRequiredRoom;

		public StatusItem NotInRecommendedRoom;

		public StatusItem IncubatorProgress;

		public StatusItem HabitatNeedsEmptying;

		public StatusItem DetectorScanning;

		public StatusItem IncomingMeteors;

		public StatusItem HasGantry;

		public StatusItem MissingGantry;

		public StatusItem DisembarkingDuplicant;

		public StatusItem RocketName;

		public StatusItem PathNotClear;

		public StatusItem InvalidPortOverlap;

		public StatusItem EmergencyPriority;

		public StatusItem SkillPointsAvailable;

		public StatusItem Baited;

		[CompilerGenerated]
		private static Func<HashedString, object, bool> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Func<HashedString, object, bool> _003C_003Ef__mg_0024cache1;

		public BuildingStatusItems(ResourceSet parent)
			: base("BuildingStatusItems", parent)
		{
			CreateStatusItems();
		}

		private StatusItem CreateStatusItem(string id, string prefix, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, bool showWorldIcon = true, int status_overlays = 129022)
		{
			return Add(new StatusItem(id, prefix, icon, icon_type, notification_type, allow_multiples, render_overlay, showWorldIcon, status_overlays));
		}

		private StatusItem CreateStatusItem(string id, string name, string tooltip, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, int status_overlays = 129022)
		{
			return Add(new StatusItem(id, name, tooltip, icon, icon_type, notification_type, allow_multiples, render_overlay, status_overlays));
		}

		private void CreateStatusItems()
		{
			AngerDamage = CreateStatusItem("AngerDamage", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 129022);
			AssignedTo = CreateStatusItem("AssignedTo", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			AssignedTo.resolveStringCallback = delegate(string str, object data)
			{
				Assignable assignable2 = (Assignable)data;
				IAssignableIdentity assignee2 = assignable2.assignee;
				if (assignee2 != null)
				{
					string properName2 = assignee2.GetProperName();
					str = str.Replace("{Assignee}", properName2);
				}
				return str;
			};
			AssignedToRoom = CreateStatusItem("AssignedToRoom", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			AssignedToRoom.resolveStringCallback = delegate(string str, object data)
			{
				Assignable assignable = (Assignable)data;
				IAssignableIdentity assignee = assignable.assignee;
				if (assignee != null)
				{
					string properName = assignee.GetProperName();
					str = str.Replace("{Assignee}", properName);
				}
				return str;
			};
			Broken = CreateStatusItem("Broken", "BUILDING", "status_item_broken", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Broken.resolveStringCallback = delegate(string str, object data)
			{
				BuildingHP.SMInstance sMInstance3 = (BuildingHP.SMInstance)data;
				return str.Replace("{DamageInfo}", sMInstance3.master.GetDamageSourceInfo().ToString());
			};
			Broken.conditionalOverlayCallback = ShowInUtilityOverlay;
			ChangeDoorControlState = CreateStatusItem("ChangeDoorControlState", "BUILDING", "status_item_pending_switch_toggle", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			ChangeDoorControlState.resolveStringCallback = delegate(string str, object data)
			{
				Door door2 = (Door)data;
				return str.Replace("{ControlState}", door2.RequestedState.ToString());
			};
			CurrentDoorControlState = CreateStatusItem("CurrentDoorControlState", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			CurrentDoorControlState.resolveStringCallback = delegate(string str, object data)
			{
				Door door = (Door)data;
				string newValue10 = Strings.Get("STRINGS.BUILDING.STATUSITEMS.CURRENTDOORCONTROLSTATE." + door.CurrentState.ToString().ToUpper());
				return str.Replace("{ControlState}", newValue10);
			};
			ClinicOutsideHospital = CreateStatusItem("ClinicOutsideHospital", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022);
			ConduitBlocked = CreateStatusItem("ConduitBlocked", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			OutputPipeFull = CreateStatusItem("OutputPipeFull", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			ConstructionUnreachable = CreateStatusItem("ConstructionUnreachable", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			ConduitBlockedMultiples = CreateStatusItem("ConduitBlocked", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, true, OverlayModes.None.ID, true, 129022);
			DigUnreachable = CreateStatusItem("DigUnreachable", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			MopUnreachable = CreateStatusItem("MopUnreachable", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			DirectionControl = CreateStatusItem("DirectionControl", BUILDING.STATUSITEMS.DIRECTION_CONTROL.NAME, BUILDING.STATUSITEMS.DIRECTION_CONTROL.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022);
			DirectionControl.resolveStringCallback = delegate(string str, object data)
			{
				DirectionControl directionControl = (DirectionControl)data;
				string newValue9 = BUILDING.STATUSITEMS.DIRECTION_CONTROL.DIRECTIONS.BOTH;
				switch (directionControl.allowedDirection)
				{
				case WorkableReactable.AllowedDirection.Left:
					newValue9 = BUILDING.STATUSITEMS.DIRECTION_CONTROL.DIRECTIONS.LEFT;
					break;
				case WorkableReactable.AllowedDirection.Right:
					newValue9 = BUILDING.STATUSITEMS.DIRECTION_CONTROL.DIRECTIONS.RIGHT;
					break;
				}
				str = str.Replace("{Direction}", newValue9);
				return str;
			};
			ConstructableDigUnreachable = CreateStatusItem("ConstructableDigUnreachable", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Entombed = CreateStatusItem("Entombed", "BUILDING", "status_item_entombed", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Entombed.AddNotification(null, null, null, 0f);
			Flooded = CreateStatusItem("Flooded", "BUILDING", "status_item_flooded", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Flooded.AddNotification(null, null, null, 0f);
			GasVentObstructed = CreateStatusItem("GasVentObstructed", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID, true, 129022);
			GasVentOverPressure = CreateStatusItem("GasVentOverPressure", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID, true, 129022);
			GeneShuffleCompleted = CreateStatusItem("GeneShuffleCompleted", "BUILDING", "status_item_pending_upgrade", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			InvalidBuildingLocation = CreateStatusItem("InvalidBuildingLocation", "BUILDING", "status_item_missing_foundation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			LiquidVentObstructed = CreateStatusItem("LiquidVentObstructed", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID, true, 129022);
			LiquidVentOverPressure = CreateStatusItem("LiquidVentOverPressure", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID, true, 129022);
			MaterialsUnavailable = new MaterialsStatusItem("MaterialsUnavailable", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, true, OverlayModes.None.ID);
			MaterialsUnavailable.AddNotification(null, null, null, 0f);
			MaterialsUnavailable.resolveStringCallback = delegate(string str, object data)
			{
				string text7 = string.Empty;
				Dictionary<Tag, float> dictionary = null;
				if (data is IFetchList)
				{
					IFetchList fetchList3 = (IFetchList)data;
					dictionary = fetchList3.GetRemainingMinimum();
				}
				else if (data is Dictionary<Tag, float>)
				{
					dictionary = (data as Dictionary<Tag, float>);
				}
				if (dictionary.Count > 0)
				{
					bool flag4 = true;
					foreach (KeyValuePair<Tag, float> item in dictionary)
					{
						if (item.Value != 0f)
						{
							if (!flag4)
							{
								text7 += "\n";
							}
							text7 = ((!Assets.IsTagCountable(item.Key)) ? (text7 + string.Format(BUILDING.STATUSITEMS.MATERIALSUNAVAILABLE.LINE_ITEM_MASS, item.Key.ProperName(), GameUtil.GetFormattedMass(item.Value, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"))) : (text7 + string.Format(BUILDING.STATUSITEMS.MATERIALSUNAVAILABLE.LINE_ITEM_UNITS, GameUtil.GetUnitFormattedName(item.Key.ProperName(), item.Value, false))));
							flag4 = false;
						}
					}
				}
				str = str.Replace("{ItemsRemaining}", text7);
				return str;
			};
			MaterialsUnavailableForRefill = new MaterialsStatusItem("MaterialsUnavailableForRefill", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, true, OverlayModes.None.ID);
			MaterialsUnavailableForRefill.resolveStringCallback = delegate(string str, object data)
			{
				IFetchList fetchList2 = (IFetchList)data;
				string text6 = string.Empty;
				Dictionary<Tag, float> remaining2 = fetchList2.GetRemaining();
				if (remaining2.Count > 0)
				{
					bool flag3 = true;
					foreach (KeyValuePair<Tag, float> item2 in remaining2)
					{
						if (item2.Value != 0f)
						{
							if (!flag3)
							{
								text6 += "\n";
							}
							text6 += string.Format(BUILDING.STATUSITEMS.MATERIALSUNAVAILABLEFORREFILL.LINE_ITEM, item2.Key.ProperName());
							flag3 = false;
						}
					}
				}
				str = str.Replace("{ItemsRemaining}", text6);
				return str;
			};
			Func<string, object, string> resolveStringCallback = delegate(string str, object data)
			{
				RoomType roomType = Db.Get().RoomTypes.Get((string)data);
				if (roomType != null)
				{
					return string.Format(str, roomType.Name);
				}
				return str;
			};
			NotInAnyRoom = CreateStatusItem("NotInAnyRoom", "BUILDING", "status_item_room_required", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NotInRequiredRoom = CreateStatusItem("NotInRequiredRoom", "BUILDING", "status_item_room_required", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NotInRequiredRoom.resolveStringCallback = resolveStringCallback;
			NotInRecommendedRoom = CreateStatusItem("NotInRecommendedRoom", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			NotInRecommendedRoom.resolveStringCallback = resolveStringCallback;
			WaitingForRepairMaterials = CreateStatusItem("WaitingForRepairMaterials", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Exclamation, NotificationType.Neutral, true, OverlayModes.None.ID, false, 129022);
			WaitingForRepairMaterials.resolveStringCallback = delegate(string str, object data)
			{
				KeyValuePair<Tag, float> keyValuePair = (KeyValuePair<Tag, float>)data;
				if (keyValuePair.Value != 0f)
				{
					string newValue8 = string.Format(BUILDING.STATUSITEMS.WAITINGFORMATERIALS.LINE_ITEM_MASS, keyValuePair.Key.ProperName(), GameUtil.GetFormattedMass(keyValuePair.Value, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
					str = str.Replace("{ItemsRemaining}", newValue8);
				}
				return str;
			};
			WaitingForMaterials = new MaterialsStatusItem("WaitingForMaterials", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, true, OverlayModes.None.ID);
			WaitingForMaterials.resolveStringCallback = delegate(string str, object data)
			{
				IFetchList fetchList = (IFetchList)data;
				string text5 = string.Empty;
				Dictionary<Tag, float> remaining = fetchList.GetRemaining();
				if (remaining.Count > 0)
				{
					bool flag2 = true;
					foreach (KeyValuePair<Tag, float> item3 in remaining)
					{
						if (item3.Value != 0f)
						{
							if (!flag2)
							{
								text5 += "\n";
							}
							text5 = ((!Assets.IsTagCountable(item3.Key)) ? (text5 + string.Format(BUILDING.STATUSITEMS.WAITINGFORMATERIALS.LINE_ITEM_MASS, item3.Key.ProperName(), GameUtil.GetFormattedMass(item3.Value, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"))) : (text5 + string.Format(BUILDING.STATUSITEMS.WAITINGFORMATERIALS.LINE_ITEM_UNITS, GameUtil.GetUnitFormattedName(item3.Key.ProperName(), item3.Value, false))));
							flag2 = false;
						}
					}
				}
				str = str.Replace("{ItemsRemaining}", text5);
				return str;
			};
			MeltingDown = CreateStatusItem("MeltingDown", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			MissingFoundation = CreateStatusItem("MissingFoundation", "BUILDING", "status_item_missing_foundation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NeutroniumUnminable = CreateStatusItem("NeutroniumUnminable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NeedGasIn = CreateStatusItem("NeedGasIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID, true, 129022);
			NeedGasIn.resolveStringCallback = delegate(string str, object data)
			{
				Tuple<ConduitType, Tag> tuple2 = (Tuple<ConduitType, Tag>)data;
				string newValue7 = string.Format(BUILDING.STATUSITEMS.NEEDGASIN.LINE_ITEM, tuple2.second.ProperName());
				str = str.Replace("{GasRequired}", newValue7);
				return str;
			};
			NeedGasOut = CreateStatusItem("NeedGasOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, true, OverlayModes.GasConduits.ID, true, 129022);
			NeedLiquidIn = CreateStatusItem("NeedLiquidIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID, true, 129022);
			NeedLiquidIn.resolveStringCallback = delegate(string str, object data)
			{
				Tuple<ConduitType, Tag> tuple = (Tuple<ConduitType, Tag>)data;
				string newValue6 = string.Format(BUILDING.STATUSITEMS.NEEDLIQUIDIN.LINE_ITEM, tuple.second.ProperName());
				str = str.Replace("{LiquidRequired}", newValue6);
				return str;
			};
			NeedLiquidOut = CreateStatusItem("NeedLiquidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, true, OverlayModes.LiquidConduits.ID, true, 129022);
			NeedSolidIn = CreateStatusItem("NeedSolidIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.SolidConveyor.ID, true, 129022);
			NeedSolidOut = CreateStatusItem("NeedSolidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, true, OverlayModes.SolidConveyor.ID, true, 129022);
			NeedResourceMass = CreateStatusItem("NeedResourceMass", "BUILDING", "status_item_need_resource", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NeedResourceMass.resolveStringCallback = delegate(string str, object data)
			{
				string text4 = string.Empty;
				EnergyGenerator.Formula formula = (EnergyGenerator.Formula)data;
				if (formula.inputs.Length > 0)
				{
					bool flag = true;
					EnergyGenerator.InputItem[] inputs = formula.inputs;
					for (int i = 0; i < inputs.Length; i++)
					{
						EnergyGenerator.InputItem inputItem = inputs[i];
						if (!flag)
						{
							text4 += "\n";
							flag = false;
						}
						text4 += string.Format(BUILDING.STATUSITEMS.NEEDRESOURCEMASS.LINE_ITEM, inputItem.tag.ProperName());
					}
				}
				str = str.Replace("{ResourcesRequired}", text4);
				return str;
			};
			LiquidPipeEmpty = CreateStatusItem("LiquidPipeEmpty", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID, true, 129022);
			LiquidPipeObstructed = CreateStatusItem("LiquidPipeObstructed", "BUILDING", "status_item_wrong_resource_in_pipe", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.LiquidConduits.ID, true, 129022);
			GasPipeEmpty = CreateStatusItem("GasPipeEmpty", "BUILDING", "status_item_no_gas_to_pump", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID, true, 129022);
			GasPipeObstructed = CreateStatusItem("GasPipeObstructed", "BUILDING", "status_item_wrong_resource_in_pipe", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.GasConduits.ID, true, 129022);
			SolidPipeObstructed = CreateStatusItem("SolidPipeObstructed", "BUILDING", "status_item_wrong_resource_in_pipe", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.SolidConveyor.ID, true, 129022);
			NeedPlant = CreateStatusItem("NeedPlant", "BUILDING", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NeedPower = CreateStatusItem("NeedPower", "BUILDING", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.Power.ID, true, 129022);
			NotEnoughPower = CreateStatusItem("NotEnoughPower", "BUILDING", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.Power.ID, true, 129022);
			NewDuplicantsAvailable = CreateStatusItem("NewDuplicantsAvailable", "BUILDING", "status_item_new_duplicants_available", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NewDuplicantsAvailable.AddNotification(null, null, null, 0f);
			NewDuplicantsAvailable.notificationClickCallback = delegate(object data)
			{
				Telepad telepad2 = (Telepad)data;
				ImmigrantScreen.InitializeImmigrantScreen(telepad2);
				Game.Instance.Trigger(288942073, null);
			};
			NoStorageFilterSet = CreateStatusItem("NoStorageFilterSet", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NoSuitMarker = CreateStatusItem("NoSuitMarker", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			SuitMarkerWrongSide = CreateStatusItem("suitMarkerWrongSide", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			SuitMarkerTraversalAnytime = CreateStatusItem("suitMarkerTraversalAnytime", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			SuitMarkerTraversalOnlyWhenRoomAvailable = CreateStatusItem("suitMarkerTraversalOnlyWhenRoomAvailable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			NoFishableWaterBelow = CreateStatusItem("NoFishableWaterBelow", "BUILDING", "status_item_no_fishable_water_below", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NoPowerConsumers = CreateStatusItem("NoPowerConsumers", "BUILDING", "status_item_no_power_consumers", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.Power.ID, true, 129022);
			NoWireConnected = CreateStatusItem("NoWireConnected", "BUILDING", "status_item_no_wire_connected", StatusItem.IconType.Custom, NotificationType.BadMinor, true, OverlayModes.Power.ID, true, 129022);
			NoLogicWireConnected = CreateStatusItem("NoLogicWireConnected", "BUILDING", "status_item_no_logic_wire_connected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.Logic.ID, true, 129022);
			NoTubeConnected = CreateStatusItem("NoTubeConnected", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NoTubeExits = CreateStatusItem("NoTubeExits", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			StoredCharge = CreateStatusItem("StoredCharge", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			StoredCharge.resolveStringCallback = delegate(string str, object data)
			{
				TravelTubeEntrance.SMInstance sMInstance2 = (TravelTubeEntrance.SMInstance)data;
				if (sMInstance2 != null)
				{
					str = string.Format(str, GameUtil.GetFormattedRoundedJoules(sMInstance2.master.AvailableJoules), GameUtil.GetFormattedRoundedJoules(sMInstance2.master.TotalCapacity), GameUtil.GetFormattedRoundedJoules(sMInstance2.master.UsageJoules));
				}
				return str;
			};
			PendingDeconstruction = CreateStatusItem("PendingDeconstruction", "BUILDING", "status_item_pending_deconstruction", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			PendingDeconstruction.conditionalOverlayCallback = ShowInUtilityOverlay;
			PendingRepair = CreateStatusItem("PendingRepair", "BUILDING", "status_item_pending_repair", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			PendingRepair.resolveStringCallback = delegate(string str, object data)
			{
				Repairable.SMInstance sMInstance = (Repairable.SMInstance)data;
				BuildingHP component4 = sMInstance.master.GetComponent<BuildingHP>();
				return str.Replace("{DamageInfo}", component4.GetDamageSourceInfo().ToString());
			};
			PendingRepair.conditionalOverlayCallback = ((HashedString mode, object data) => true);
			RequiresSkillPerk = CreateStatusItem("RequiresSkillPerk", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			RequiresSkillPerk.resolveStringCallback = delegate(string str, object data)
			{
				string id = (string)data;
				SkillPerk perk = Db.Get().SkillPerks.Get(id);
				List<Skill> skillsWithPerk = Db.Get().Skills.GetSkillsWithPerk(perk);
				List<string> list = new List<string>();
				foreach (Skill item4 in skillsWithPerk)
				{
					list.Add(item4.Name);
				}
				str = str.Replace("{Skills}", string.Join(", ", list.ToArray()));
				return str;
			};
			DigRequiresSkillPerk = CreateStatusItem("DigRequiresSkillPerk", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			DigRequiresSkillPerk.resolveStringCallback = RequiresSkillPerk.resolveStringCallback;
			ColonyLacksRequiredSkillPerk = CreateStatusItem("ColonyLacksRequiredSkillPerk", "BUILDING", "status_item_role_required", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			ColonyLacksRequiredSkillPerk.resolveStringCallback = RequiresSkillPerk.resolveStringCallback;
			SwitchStatusActive = CreateStatusItem("SwitchStatusActive", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			SwitchStatusInactive = CreateStatusItem("SwitchStatusInactive", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			PendingFish = CreateStatusItem("PendingFish", "BUILDING", "status_item_pending_fish", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			PendingSwitchToggle = CreateStatusItem("PendingSwitchToggle", "BUILDING", "status_item_pending_switch_toggle", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			PendingUpgrade = CreateStatusItem("PendingUpgrade", "BUILDING", "status_item_pending_upgrade", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			PendingWork = CreateStatusItem("PendingWork", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			PowerButtonOff = CreateStatusItem("PowerButtonOff", "BUILDING", "status_item_power_button_off", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			PressureOk = CreateStatusItem("PressureOk", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.Oxygen.ID, true, 129022);
			UnderPressure = CreateStatusItem("UnderPressure", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.Oxygen.ID, true, 129022);
			Unassigned = CreateStatusItem("Unassigned", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.Rooms.ID, true, 129022);
			AssignedPublic = CreateStatusItem("AssignedPublic", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Rooms.ID, true, 129022);
			UnderConstruction = CreateStatusItem("UnderConstruction", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			UnderConstructionNoWorker = CreateStatusItem("UnderConstructionNoWorker", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Normal = CreateStatusItem("Normal", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			ManualGeneratorChargingUp = CreateStatusItem("ManualGeneratorChargingUp", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 129022);
			ManualGeneratorReleasingEnergy = CreateStatusItem("ManualGeneratorReleasingEnergy", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 129022);
			GeneratorOffline = CreateStatusItem("GeneratorOffline", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.Power.ID, true, 129022);
			Pipe = CreateStatusItem("Pipe", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.LiquidConduits.ID, true, 129022);
			Pipe.resolveStringCallback = delegate(string str, object data)
			{
				Conduit conduit = (Conduit)data;
				int cell2 = Grid.PosToCell(conduit);
				ConduitFlow flowManager = conduit.GetFlowManager();
				ConduitFlow.ConduitContents contents2 = flowManager.GetContents(cell2);
				string text3 = BUILDING.STATUSITEMS.PIPECONTENTS.EMPTY;
				if (contents2.mass > 0f)
				{
					Element element4 = ElementLoader.FindElementByHash(contents2.element);
					text3 = string.Format(BUILDING.STATUSITEMS.PIPECONTENTS.CONTENTS, GameUtil.GetFormattedMass(contents2.mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), element4.name, GameUtil.GetFormattedTemperature(contents2.temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
					if ((UnityEngine.Object)OverlayScreen.Instance != (UnityEngine.Object)null && OverlayScreen.Instance.mode == OverlayModes.Disease.ID && contents2.diseaseIdx != 255)
					{
						text3 += string.Format(BUILDING.STATUSITEMS.PIPECONTENTS.CONTENTS_WITH_DISEASE, GameUtil.GetFormattedDisease(contents2.diseaseIdx, contents2.diseaseCount, true));
					}
				}
				str = str.Replace("{Contents}", text3);
				return str;
			};
			Conveyor = CreateStatusItem("Conveyor", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.SolidConveyor.ID, true, 129022);
			Conveyor.resolveStringCallback = delegate(string str, object data)
			{
				SolidConduit cmp = (SolidConduit)data;
				int cell = Grid.PosToCell(cmp);
				SolidConduitFlow solidConduitFlow = Game.Instance.solidConduitFlow;
				SolidConduitFlow.ConduitContents contents = solidConduitFlow.GetContents(cell);
				string text2 = BUILDING.STATUSITEMS.CONVEYOR_CONTENTS.EMPTY;
				if (contents.pickupableHandle.IsValid())
				{
					Pickupable pickupable = solidConduitFlow.GetPickupable(contents.pickupableHandle);
					if ((bool)pickupable)
					{
						PrimaryElement component3 = pickupable.GetComponent<PrimaryElement>();
						float mass = component3.Mass;
						if (mass > 0f)
						{
							Element element3 = ElementLoader.FindElementByHash(component3.ElementID);
							text2 = string.Format(BUILDING.STATUSITEMS.CONVEYOR_CONTENTS.CONTENTS, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), element3.name, GameUtil.GetFormattedTemperature(component3.Temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
							if ((UnityEngine.Object)OverlayScreen.Instance != (UnityEngine.Object)null && OverlayScreen.Instance.mode == OverlayModes.Disease.ID && component3.DiseaseIdx != 255)
							{
								text2 += string.Format(BUILDING.STATUSITEMS.CONVEYOR_CONTENTS.CONTENTS_WITH_DISEASE, GameUtil.GetFormattedDisease(component3.DiseaseIdx, component3.DiseaseCount, true));
							}
						}
					}
				}
				str = str.Replace("{Contents}", text2);
				return str;
			};
			FabricatorIdle = CreateStatusItem("FabricatorIdle", "BUILDING", "status_item_fabricator_select", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			FabricatorEmpty = CreateStatusItem("FabricatorEmpty", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Toilet = CreateStatusItem("Toilet", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Toilet.resolveStringCallback = delegate(string str, object data)
			{
				Toilet.StatesInstance statesInstance5 = (Toilet.StatesInstance)data;
				if (statesInstance5 != null)
				{
					str = str.Replace("{FlushesRemaining}", statesInstance5.GetFlushesRemaining().ToString());
				}
				return str;
			};
			ToiletNeedsEmptying = CreateStatusItem("ToiletNeedsEmptying", "BUILDING", "status_item_toilet_needs_emptying", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			DesalinatorNeedsEmptying = CreateStatusItem("DesalinatorNeedsEmptying", "BUILDING", "status_item_desalinator_needs_emptying", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Unusable = CreateStatusItem("Unusable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NoResearchSelected = CreateStatusItem("NoResearchSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NoResearchSelected.AddNotification(null, null, null, 0f);
			StatusItem noResearchSelected = NoResearchSelected;
			noResearchSelected.resolveTooltipCallback = (Func<string, object, string>)Delegate.Combine(noResearchSelected.resolveTooltipCallback, (Func<string, object, string>)delegate(string str, object data)
			{
				BindingEntry bindingEntry3 = GameInputMapping.FindEntry(Action.ManageResearch);
				string newValue5 = bindingEntry3.mKeyCode.ToString();
				str = str.Replace("{RESEARCH_MENU_KEY}", newValue5);
				return str;
			});
			NoApplicableResearchSelected = CreateStatusItem("NoApplicableResearchSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NoApplicableResearchSelected.AddNotification(null, null, null, 0f);
			NoApplicableAnalysisSelected = CreateStatusItem("NoApplicableAnalysisSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NoApplicableAnalysisSelected.AddNotification(null, null, null, 0f);
			StatusItem noApplicableAnalysisSelected = NoApplicableAnalysisSelected;
			noApplicableAnalysisSelected.resolveTooltipCallback = (Func<string, object, string>)Delegate.Combine(noApplicableAnalysisSelected.resolveTooltipCallback, (Func<string, object, string>)delegate(string str, object data)
			{
				BindingEntry bindingEntry2 = GameInputMapping.FindEntry(Action.ManageStarmap);
				string newValue4 = bindingEntry2.mKeyCode.ToString();
				str = str.Replace("{STARMAP_MENU_KEY}", newValue4);
				return str;
			});
			NoResearchOrDestinationSelected = CreateStatusItem("NoResearchOrDestinationSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NoResearchOrDestinationSelected.AddNotification(null, null, null, 0f);
			ValveRequest = CreateStatusItem("ValveRequest", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			ValveRequest.resolveStringCallback = delegate(string str, object data)
			{
				Valve valve = (Valve)data;
				str = str.Replace("{QueuedMaxFlow}", GameUtil.GetFormattedMass(valve.QueuedMaxFlow, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			EmittingLight = CreateStatusItem("EmittingLight", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			EmittingLight.resolveStringCallback = delegate(string str, object data)
			{
				BindingEntry bindingEntry = GameInputMapping.FindEntry(Action.Overlay5);
				string newValue3 = bindingEntry.mKeyCode.ToString();
				str = str.Replace("{LightGridOverlay}", newValue3);
				return str;
			};
			RationBoxContents = CreateStatusItem("RationBoxContents", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			RationBoxContents.resolveStringCallback = delegate(string str, object data)
			{
				RationBox rationBox = (RationBox)data;
				if ((UnityEngine.Object)rationBox == (UnityEngine.Object)null)
				{
					return str;
				}
				Storage component = rationBox.GetComponent<Storage>();
				if ((UnityEngine.Object)component == (UnityEngine.Object)null)
				{
					return str;
				}
				float num2 = 0f;
				foreach (GameObject item5 in component.items)
				{
					Edible component2 = item5.GetComponent<Edible>();
					if ((bool)component2)
					{
						num2 += component2.Calories;
					}
				}
				str = str.Replace("{Stored}", GameUtil.GetFormattedCalories(num2, GameUtil.TimeSlice.None, true));
				return str;
			};
			EmittingElement = CreateStatusItem("EmittingElement", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			EmittingElement.resolveStringCallback = delegate(string str, object data)
			{
				IElementEmitter elementEmitter2 = (IElementEmitter)data;
				string newValue2 = ElementLoader.FindElementByHash(elementEmitter2.Element).tag.ProperName();
				str = str.Replace("{ElementType}", newValue2);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(elementEmitter2.AverageEmitRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			EmittingOxygenAvg = CreateStatusItem("EmittingOxygenAvg", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			EmittingOxygenAvg.resolveStringCallback = delegate(string str, object data)
			{
				Sublimates sublimates2 = (Sublimates)data;
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(sublimates2.AvgFlowRate(), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			EmittingGasAvg = CreateStatusItem("EmittingGasAvg", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			EmittingGasAvg.resolveStringCallback = delegate(string str, object data)
			{
				Sublimates sublimates = (Sublimates)data;
				str = str.Replace("{Element}", ElementLoader.FindElementByHash(sublimates.info.sublimatedElement).name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(sublimates.AvgFlowRate(), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			PumpingLiquidOrGas = CreateStatusItem("PumpingLiquidOrGas", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.LiquidConduits.ID, true, 129022);
			PumpingLiquidOrGas.resolveStringCallback = delegate(string str, object data)
			{
				HandleVector<int>.Handle handle = (HandleVector<int>.Handle)data;
				float averageRate = Game.Instance.accumulators.GetAverageRate(handle);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(averageRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			PipeMayMelt = CreateStatusItem("PipeMayMelt", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NoLiquidElementToPump = CreateStatusItem("NoLiquidElementToPump", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID, true, 129022);
			NoGasElementToPump = CreateStatusItem("NoGasElementToPump", "BUILDING", "status_item_no_gas_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID, true, 129022);
			NoFilterElementSelected = CreateStatusItem("NoFilterElementSelected", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NoLureElementSelected = CreateStatusItem("NoLureElementSelected", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			ElementConsumer = CreateStatusItem("ElementConsumer", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 129022);
			ElementConsumer.resolveStringCallback = delegate(string str, object data)
			{
				ElementConsumer elementConsumer = (ElementConsumer)data;
				string newValue = ElementLoader.FindElementByHash(elementConsumer.elementToConsume).tag.ProperName();
				str = str.Replace("{ElementTypes}", newValue);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(elementConsumer.AverageConsumeRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			ElementEmitterOutput = CreateStatusItem("ElementEmitterOutput", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 129022);
			ElementEmitterOutput.resolveStringCallback = delegate(string str, object data)
			{
				ElementEmitter elementEmitter = (ElementEmitter)data;
				if ((UnityEngine.Object)elementEmitter != (UnityEngine.Object)null)
				{
					str = str.Replace("{ElementTypes}", elementEmitter.outputElement.Name);
					str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(elementEmitter.outputElement.massGenerationRate / elementEmitter.emissionFrequency, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				}
				return str;
			};
			AwaitingWaste = CreateStatusItem("AwaitingWaste", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 129022);
			AwaitingCompostFlip = CreateStatusItem("AwaitingCompostFlip", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 129022);
			JoulesAvailable = CreateStatusItem("JoulesAvailable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 129022);
			JoulesAvailable.resolveStringCallback = delegate(string str, object data)
			{
				IEnergyProducer energyProducer = (IEnergyProducer)data;
				str = str.Replace("{JoulesAvailable}", GameUtil.GetFormattedJoules(energyProducer.JoulesAvailable, "F1", GameUtil.TimeSlice.None));
				return str;
			};
			Wattage = CreateStatusItem("Wattage", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 129022);
			Wattage.resolveStringCallback = delegate(string str, object data)
			{
				Generator generator = (Generator)data;
				str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(generator.WattageRating, GameUtil.WattageFormatterUnit.Automatic));
				return str;
			};
			SolarPanelWattage = CreateStatusItem("SolarPanelWattage", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 129022);
			SolarPanelWattage.resolveStringCallback = delegate(string str, object data)
			{
				SolarPanel solarPanel = (SolarPanel)data;
				str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(solarPanel.CurrentWattage, GameUtil.WattageFormatterUnit.Automatic));
				return str;
			};
			SteamTurbineWattage = CreateStatusItem("SteamTurbineWattage", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 129022);
			SteamTurbineWattage.resolveStringCallback = delegate(string str, object data)
			{
				SteamTurbine steamTurbine = (SteamTurbine)data;
				str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(steamTurbine.CurrentWattage, GameUtil.WattageFormatterUnit.Automatic));
				return str;
			};
			Wattson = CreateStatusItem("Wattson", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Wattson.resolveStringCallback = delegate(string str, object data)
			{
				Telepad telepad = (Telepad)data;
				str = (((UnityEngine.Object)GameFlowManager.Instance != (UnityEngine.Object)null && GameFlowManager.Instance.IsGameOver()) ? ((string)BUILDING.STATUSITEMS.WATTSONGAMEOVER.NAME) : ((!telepad.GetComponent<Operational>().IsOperational) ? str.Replace("{TimeRemaining}", BUILDING.STATUSITEMS.WATTSON.UNAVAILABLE) : str.Replace("{TimeRemaining}", GameUtil.GetFormattedCycles(telepad.GetTimeRemaining(), "F1"))));
				return str;
			};
			FlushToilet = CreateStatusItem("FlushToilet", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			FlushToiletInUse = CreateStatusItem("FlushToiletInUse", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			WireNominal = CreateStatusItem("WireNominal", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 129022);
			WireConnected = CreateStatusItem("WireConnected", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 129022);
			WireDisconnected = CreateStatusItem("WireDisconnected", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.Power.ID, true, 129022);
			Overheated = CreateStatusItem("Overheated", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 129022);
			Overloaded = CreateStatusItem("Overloaded", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 129022);
			Cooling = CreateStatusItem("Cooling", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Func<string, object, string> resolveStringCallback2 = delegate(string str, object data)
			{
				AirConditioner airConditioner2 = (AirConditioner)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(airConditioner2.lastGasTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
			};
			CoolingStalledColdGas = CreateStatusItem("CoolingStalledColdGas", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			CoolingStalledColdGas.resolveStringCallback = resolveStringCallback2;
			CoolingStalledColdLiquid = CreateStatusItem("CoolingStalledColdLiquid", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			CoolingStalledColdLiquid.resolveStringCallback = resolveStringCallback2;
			Func<string, object, string> resolveStringCallback3 = delegate(string str, object data)
			{
				AirConditioner airConditioner = (AirConditioner)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(airConditioner.lastEnvTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(airConditioner.lastGasTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(airConditioner.maxEnvironmentDelta, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative, true, false));
			};
			CoolingStalledHotEnv = CreateStatusItem("CoolingStalledHotEnv", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			CoolingStalledHotEnv.resolveStringCallback = resolveStringCallback3;
			CoolingStalledHotLiquid = CreateStatusItem("CoolingStalledHotLiquid", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			CoolingStalledHotLiquid.resolveStringCallback = resolveStringCallback3;
			Working = CreateStatusItem("Working", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			NeedsValidRegion = CreateStatusItem("NeedsValidRegion", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			NeedSeed = CreateStatusItem("NeedSeed", "BUILDING", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			AwaitingSeedDelivery = CreateStatusItem("AwaitingSeedDelivery", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			AwaitingBaitDelivery = CreateStatusItem("AwaitingBaitDelivery", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			NoAvailableSeed = CreateStatusItem("NoAvailableSeed", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			NeedEgg = CreateStatusItem("NeedEgg", "BUILDING", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			AwaitingEggDelivery = CreateStatusItem("AwaitingEggDelivery", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			NoAvailableEgg = CreateStatusItem("NoAvailableEgg", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			Grave = CreateStatusItem("Grave", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Grave.resolveStringCallback = delegate(string str, object data)
			{
				Grave.StatesInstance statesInstance4 = (Grave.StatesInstance)data;
				string text = str.Replace("{DeadDupe}", statesInstance4.master.graveName);
				string[] strings = LocString.GetStrings(typeof(NAMEGEN.GRAVE.EPITAPHS));
				int num = statesInstance4.master.epitaphIdx % strings.Length;
				return text.Replace("{Epitaph}", strings[num]);
			};
			GraveEmpty = CreateStatusItem("GraveEmpty", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			CannotCoolFurther = CreateStatusItem("CannotCoolFurther", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			BuildingDisabled = CreateStatusItem("BuildingDisabled", "BUILDING", "status_item_building_disabled", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			Expired = CreateStatusItem("Expired", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			PumpingStation = CreateStatusItem("PumpingStation", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			PumpingStation.resolveStringCallback = delegate(string str, object data)
			{
				LiquidPumpingStation liquidPumpingStation = (LiquidPumpingStation)data;
				if ((UnityEngine.Object)liquidPumpingStation != (UnityEngine.Object)null)
				{
					return liquidPumpingStation.ResolveString(str);
				}
				return str;
			};
			EmptyPumpingStation = CreateStatusItem("EmptyPumpingStation", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			WellPressurizing = CreateStatusItem("WellPressurizing", BUILDING.STATUSITEMS.WELL_PRESSURIZING.NAME, BUILDING.STATUSITEMS.WELL_PRESSURIZING.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022);
			WellPressurizing.resolveStringCallback = delegate(string str, object data)
			{
				OilWellCap.StatesInstance statesInstance3 = (OilWellCap.StatesInstance)data;
				if (statesInstance3 != null)
				{
					return string.Format(str, GameUtil.GetFormattedPercent(100f * statesInstance3.GetPressurePercent(), GameUtil.TimeSlice.None));
				}
				return str;
			};
			WellOverpressure = CreateStatusItem("WellOverpressure", BUILDING.STATUSITEMS.WELL_OVERPRESSURE.NAME, BUILDING.STATUSITEMS.WELL_OVERPRESSURE.TOOLTIP, string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022);
			ReleasingPressure = CreateStatusItem("ReleasingPressure", BUILDING.STATUSITEMS.RELEASING_PRESSURE.NAME, BUILDING.STATUSITEMS.RELEASING_PRESSURE.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022);
			TooCold = CreateStatusItem("TooCold", BUILDING.STATUSITEMS.TOO_COLD.NAME, BUILDING.STATUSITEMS.TOO_COLD.TOOLTIP, string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022);
			IncubatorProgress = CreateStatusItem("IncubatorProgress", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			IncubatorProgress.resolveStringCallback = delegate(string str, object data)
			{
				EggIncubator eggIncubator = (EggIncubator)data;
				str = str.Replace("{Percent}", GameUtil.GetFormattedPercent(eggIncubator.GetProgress() * 100f, GameUtil.TimeSlice.None));
				return str;
			};
			HabitatNeedsEmptying = CreateStatusItem("HabitatNeedsEmptying", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			DetectorScanning = CreateStatusItem("DetectorScanning", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			IncomingMeteors = CreateStatusItem("IncomingMeteors", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			HasGantry = CreateStatusItem("HasGantry", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			MissingGantry = CreateStatusItem("MissingGantry", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			DisembarkingDuplicant = CreateStatusItem("DisembarkingDuplicant", "BUILDING", "status_item_new_duplicants_available", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			RocketName = CreateStatusItem("RocketName", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			RocketName.resolveStringCallback = delegate(string str, object data)
			{
				RocketModule rocketModule2 = (RocketModule)data;
				if ((UnityEngine.Object)rocketModule2 != (UnityEngine.Object)null)
				{
					return str.Replace("{0}", rocketModule2.GetParentRocketName());
				}
				return str;
			};
			RocketName.resolveTooltipCallback = delegate(string str, object data)
			{
				RocketModule rocketModule = (RocketModule)data;
				if ((UnityEngine.Object)rocketModule != (UnityEngine.Object)null)
				{
					return str.Replace("{0}", rocketModule.GetParentRocketName());
				}
				return str;
			};
			PathNotClear = new StatusItem("PATH_NOT_CLEAR", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			PathNotClear.resolveTooltipCallback = delegate(string str, object data)
			{
				ConditionFlightPathIsClear conditionFlightPathIsClear = (ConditionFlightPathIsClear)data;
				if (conditionFlightPathIsClear != null)
				{
					str = string.Format(str, conditionFlightPathIsClear.GetObstruction());
				}
				return str;
			};
			InvalidPortOverlap = CreateStatusItem("InvalidPortOverlap", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			InvalidPortOverlap.AddNotification(null, null, null, 0f);
			EmergencyPriority = CreateStatusItem("EmergencyPriority", BUILDING.STATUSITEMS.TOP_PRIORITY_CHORE.NAME, BUILDING.STATUSITEMS.TOP_PRIORITY_CHORE.TOOLTIP, "status_item_doubleexclamation", StatusItem.IconType.Custom, NotificationType.Bad, false, OverlayModes.None.ID, 129022);
			StatusItem emergencyPriority = EmergencyPriority;
			string notification_text = BUILDING.STATUSITEMS.TOP_PRIORITY_CHORE.NOTIFICATION_NAME;
			emergencyPriority.AddNotification(null, notification_text, BUILDING.STATUSITEMS.TOP_PRIORITY_CHORE.NOTIFICATION_TOOLTIP, 0f);
			SkillPointsAvailable = CreateStatusItem("SkillPointsAvailable", BUILDING.STATUSITEMS.SKILL_POINTS_AVAILABLE.NAME, BUILDING.STATUSITEMS.SKILL_POINTS_AVAILABLE.TOOLTIP, "status_item_jobs", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, 129022);
			Baited = CreateStatusItem("Baited", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false, 129022);
			Baited.resolveStringCallback = delegate(string str, object data)
			{
				CreatureBait.StatesInstance statesInstance2 = (CreatureBait.StatesInstance)data;
				Element element2 = ElementLoader.FindElementByName(statesInstance2.master.baitElement.ToString());
				str = str.Replace("{0}", element2.name);
				return str;
			};
			Baited.resolveTooltipCallback = delegate(string str, object data)
			{
				CreatureBait.StatesInstance statesInstance = (CreatureBait.StatesInstance)data;
				Element element = ElementLoader.FindElementByName(statesInstance.master.baitElement.ToString());
				str = str.Replace("{0}", element.name);
				return str;
			};
		}

		private static bool ShowInUtilityOverlay(HashedString mode, object data)
		{
			Transform transform = (Transform)data;
			bool result = false;
			if (mode == OverlayModes.GasConduits.ID)
			{
				Tag prefabTag = transform.GetComponent<KPrefabID>().PrefabTag;
				result = OverlayScreen.GasVentIDs.Contains(prefabTag);
			}
			else if (mode == OverlayModes.LiquidConduits.ID)
			{
				Tag prefabTag2 = transform.GetComponent<KPrefabID>().PrefabTag;
				result = OverlayScreen.LiquidVentIDs.Contains(prefabTag2);
			}
			else if (mode == OverlayModes.Power.ID)
			{
				Tag prefabTag3 = transform.GetComponent<KPrefabID>().PrefabTag;
				result = OverlayScreen.WireIDs.Contains(prefabTag3);
			}
			else if (mode == OverlayModes.Logic.ID)
			{
				Tag prefabTag4 = transform.GetComponent<KPrefabID>().PrefabTag;
				result = OverlayModes.Logic.HighlightItemIDs.Contains(prefabTag4);
			}
			else if (mode == OverlayModes.SolidConveyor.ID)
			{
				Tag prefabTag5 = transform.GetComponent<KPrefabID>().PrefabTag;
				result = OverlayScreen.SolidConveyorIDs.Contains(prefabTag5);
			}
			return result;
		}
	}
}
