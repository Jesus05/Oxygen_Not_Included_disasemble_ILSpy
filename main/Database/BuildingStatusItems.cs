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

		public StatusItem Broken;

		public StatusItem PendingRepair;

		public StatusItem PendingUpgrade;

		public StatusItem RequiresRolePerk;

		public StatusItem DigRequiresRolePerk;

		public StatusItem ColonyLacksRequiredRolePerk;

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

		public StatusItem FabricatorEmpty;

		public StatusItem FlushToilet;

		public StatusItem FlushToiletInUse;

		public StatusItem Toilet;

		public StatusItem ToiletNeedsEmptying;

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

		[CompilerGenerated]
		private static Func<HashedString, object, bool> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Func<HashedString, object, bool> _003C_003Ef__mg_0024cache1;

		public BuildingStatusItems(ResourceSet parent)
			: base("BuildingStatusItems", parent)
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
			AngerDamage = CreateStatusItem("AngerDamage", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 63486);
			AssignedTo = CreateStatusItem("AssignedTo", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
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
			AssignedToRoom = CreateStatusItem("AssignedToRoom", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
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
			Broken = CreateStatusItem("Broken", "BUILDING", "status_item_broken", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Broken.resolveStringCallback = delegate(string str, object data)
			{
				BuildingHP.SMInstance sMInstance3 = (BuildingHP.SMInstance)data;
				return str.Replace("{DamageInfo}", sMInstance3.master.GetDamageSourceInfo().ToString());
			};
			Broken.conditionalOverlayCallback = ShowInUtilityOverlay;
			ChangeDoorControlState = CreateStatusItem("ChangeDoorControlState", "BUILDING", "status_item_pending_switch_toggle", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			ChangeDoorControlState.resolveStringCallback = delegate(string str, object data)
			{
				Door door2 = (Door)data;
				return str.Replace("{ControlState}", door2.RequestedState.ToString());
			};
			CurrentDoorControlState = CreateStatusItem("CurrentDoorControlState", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			CurrentDoorControlState.resolveStringCallback = delegate(string str, object data)
			{
				Door door = (Door)data;
				string newValue10 = Strings.Get("STRINGS.BUILDING.STATUSITEMS.CURRENTDOORCONTROLSTATE." + door.CurrentState.ToString().ToUpper());
				return str.Replace("{ControlState}", newValue10);
			};
			ClinicOutsideHospital = CreateStatusItem("ClinicOutsideHospital", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 63486);
			ConduitBlocked = CreateStatusItem("ConduitBlocked", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			OutputPipeFull = CreateStatusItem("OutputPipeFull", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			ConstructionUnreachable = CreateStatusItem("ConstructionUnreachable", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			ConduitBlockedMultiples = CreateStatusItem("ConduitBlocked", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, true, OverlayModes.None.ID, true, 63486);
			DigUnreachable = CreateStatusItem("DigUnreachable", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			MopUnreachable = CreateStatusItem("MopUnreachable", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			DirectionControl = CreateStatusItem("DirectionControl", BUILDING.STATUSITEMS.DIRECTION_CONTROL.NAME, BUILDING.STATUSITEMS.DIRECTION_CONTROL.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 63486);
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
			ConstructableDigUnreachable = CreateStatusItem("ConstructableDigUnreachable", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Entombed = CreateStatusItem("Entombed", "BUILDING", "status_item_entombed", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Entombed.AddNotification(null, null, null, 0f);
			Flooded = CreateStatusItem("Flooded", "BUILDING", "status_item_flooded", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Flooded.AddNotification(null, null, null, 0f);
			GasVentObstructed = CreateStatusItem("GasVentObstructed", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID, true, 63486);
			GasVentOverPressure = CreateStatusItem("GasVentOverPressure", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID, true, 63486);
			GeneShuffleCompleted = CreateStatusItem("GeneShuffleCompleted", "BUILDING", "status_item_pending_upgrade", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			InvalidBuildingLocation = CreateStatusItem("InvalidBuildingLocation", "BUILDING", "status_item_missing_foundation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			LiquidVentObstructed = CreateStatusItem("LiquidVentObstructed", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID, true, 63486);
			LiquidVentOverPressure = CreateStatusItem("LiquidVentOverPressure", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID, true, 63486);
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
			NotInAnyRoom = CreateStatusItem("NotInAnyRoom", "BUILDING", "status_item_room_required", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NotInRequiredRoom = CreateStatusItem("NotInRequiredRoom", "BUILDING", "status_item_room_required", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NotInRequiredRoom.resolveStringCallback = resolveStringCallback;
			NotInRecommendedRoom = CreateStatusItem("NotInRecommendedRoom", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			NotInRecommendedRoom.resolveStringCallback = resolveStringCallback;
			WaitingForRepairMaterials = CreateStatusItem("WaitingForRepairMaterials", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Exclamation, NotificationType.Neutral, true, OverlayModes.None.ID, false, 63486);
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
			MeltingDown = CreateStatusItem("MeltingDown", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			MissingFoundation = CreateStatusItem("MissingFoundation", "BUILDING", "status_item_missing_foundation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NeutroniumUnminable = CreateStatusItem("NeutroniumUnminable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NeedGasIn = CreateStatusItem("NeedGasIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID, true, 63486);
			NeedGasIn.resolveStringCallback = delegate(string str, object data)
			{
				Tuple<ConduitType, Tag> tuple2 = (Tuple<ConduitType, Tag>)data;
				string newValue7 = string.Format(BUILDING.STATUSITEMS.NEEDGASIN.LINE_ITEM, tuple2.second.ProperName());
				str = str.Replace("{GasRequired}", newValue7);
				return str;
			};
			NeedGasOut = CreateStatusItem("NeedGasOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, true, OverlayModes.GasConduits.ID, true, 63486);
			NeedLiquidIn = CreateStatusItem("NeedLiquidIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID, true, 63486);
			NeedLiquidIn.resolveStringCallback = delegate(string str, object data)
			{
				Tuple<ConduitType, Tag> tuple = (Tuple<ConduitType, Tag>)data;
				string newValue6 = string.Format(BUILDING.STATUSITEMS.NEEDLIQUIDIN.LINE_ITEM, tuple.second.ProperName());
				str = str.Replace("{LiquidRequired}", newValue6);
				return str;
			};
			NeedLiquidOut = CreateStatusItem("NeedLiquidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, true, OverlayModes.LiquidConduits.ID, true, 63486);
			NeedSolidIn = CreateStatusItem("NeedSolidIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.SolidConveyor.ID, true, 63486);
			NeedSolidOut = CreateStatusItem("NeedSolidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, true, OverlayModes.SolidConveyor.ID, true, 63486);
			NeedResourceMass = CreateStatusItem("NeedResourceMass", "BUILDING", "status_item_need_resource", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
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
			LiquidPipeEmpty = CreateStatusItem("LiquidPipeEmpty", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID, true, 63486);
			LiquidPipeObstructed = CreateStatusItem("LiquidPipeObstructed", "BUILDING", "status_item_wrong_resource_in_pipe", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.LiquidConduits.ID, true, 63486);
			GasPipeEmpty = CreateStatusItem("GasPipeEmpty", "BUILDING", "status_item_no_gas_to_pump", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID, true, 63486);
			GasPipeObstructed = CreateStatusItem("GasPipeObstructed", "BUILDING", "status_item_wrong_resource_in_pipe", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.GasConduits.ID, true, 63486);
			SolidPipeObstructed = CreateStatusItem("SolidPipeObstructed", "BUILDING", "status_item_wrong_resource_in_pipe", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.SolidConveyor.ID, true, 63486);
			NeedPlant = CreateStatusItem("NeedPlant", "BUILDING", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NeedPower = CreateStatusItem("NeedPower", "BUILDING", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.Power.ID, true, 63486);
			NotEnoughPower = CreateStatusItem("NotEnoughPower", "BUILDING", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.Power.ID, true, 63486);
			NewDuplicantsAvailable = CreateStatusItem("NewDuplicantsAvailable", "BUILDING", "status_item_new_duplicants_available", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NewDuplicantsAvailable.AddNotification(null, null, null, 0f);
			NewDuplicantsAvailable.notificationClickCallback = delegate(object data)
			{
				Telepad telepad2 = (Telepad)data;
				ImmigrantScreen.InitializeImmigrantScreen(telepad2);
			};
			NoStorageFilterSet = CreateStatusItem("NoStorageFilterSet", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NoSuitMarker = CreateStatusItem("NoSuitMarker", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			SuitMarkerWrongSide = CreateStatusItem("suitMarkerWrongSide", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			SuitMarkerTraversalAnytime = CreateStatusItem("suitMarkerTraversalAnytime", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			SuitMarkerTraversalOnlyWhenRoomAvailable = CreateStatusItem("suitMarkerTraversalOnlyWhenRoomAvailable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			NoFishableWaterBelow = CreateStatusItem("NoFishableWaterBelow", "BUILDING", "status_item_no_fishable_water_below", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NoPowerConsumers = CreateStatusItem("NoPowerConsumers", "BUILDING", "status_item_no_power_consumers", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.Power.ID, true, 63486);
			NoWireConnected = CreateStatusItem("NoWireConnected", "BUILDING", "status_item_no_wire_connected", StatusItem.IconType.Custom, NotificationType.BadMinor, true, OverlayModes.Power.ID, true, 63486);
			NoLogicWireConnected = CreateStatusItem("NoLogicWireConnected", "BUILDING", "status_item_no_logic_wire_connected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.Logic.ID, true, 63486);
			NoTubeConnected = CreateStatusItem("NoTubeConnected", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NoTubeExits = CreateStatusItem("NoTubeExits", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			StoredCharge = CreateStatusItem("StoredCharge", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			StoredCharge.resolveStringCallback = delegate(string str, object data)
			{
				TravelTubeEntrance.SMInstance sMInstance2 = (TravelTubeEntrance.SMInstance)data;
				if (sMInstance2 != null)
				{
					str = string.Format(str, GameUtil.GetFormattedRoundedJoules(sMInstance2.master.AvailableJoules), GameUtil.GetFormattedRoundedJoules(sMInstance2.master.TotalCapacity), GameUtil.GetFormattedRoundedJoules(sMInstance2.master.UsageJoules));
				}
				return str;
			};
			PendingDeconstruction = CreateStatusItem("PendingDeconstruction", "BUILDING", "status_item_pending_deconstruction", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			PendingDeconstruction.conditionalOverlayCallback = ShowInUtilityOverlay;
			PendingRepair = CreateStatusItem("PendingRepair", "BUILDING", "status_item_pending_repair", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			PendingRepair.resolveStringCallback = delegate(string str, object data)
			{
				Repairable.SMInstance sMInstance = (Repairable.SMInstance)data;
				BuildingHP component4 = sMInstance.master.GetComponent<BuildingHP>();
				return str.Replace("{DamageInfo}", component4.GetDamageSourceInfo().ToString());
			};
			PendingRepair.conditionalOverlayCallback = ((HashedString mode, object data) => true);
			RequiresRolePerk = CreateStatusItem("RequiresRolePerk", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			RequiresRolePerk.resolveStringCallback = delegate(string str, object data)
			{
				HashedString perk_id3 = (HashedString)data;
				List<RoleConfig> rolesWithPerk3 = Game.Instance.roleManager.GetRolesWithPerk(perk_id3);
				List<string> list3 = new List<string>();
				foreach (RoleConfig item4 in rolesWithPerk3)
				{
					list3.Add(item4.GetProperName());
				}
				str = str.Replace("{Roles}", string.Join(", ", list3.ToArray()));
				return str;
			};
			DigRequiresRolePerk = CreateStatusItem("DigRequiresRolePerk", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			DigRequiresRolePerk.resolveStringCallback = delegate(string str, object data)
			{
				HashedString perk_id2 = (HashedString)data;
				List<RoleConfig> rolesWithPerk2 = Game.Instance.roleManager.GetRolesWithPerk(perk_id2);
				List<string> list2 = new List<string>();
				foreach (RoleConfig item5 in rolesWithPerk2)
				{
					list2.Add(item5.GetProperName());
				}
				str = str.Replace("{Roles}", string.Join(", ", list2.ToArray()));
				return str;
			};
			ColonyLacksRequiredRolePerk = CreateStatusItem("ColonyLacksRequiredRolePerk", "BUILDING", "status_item_role_required", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			ColonyLacksRequiredRolePerk.resolveStringCallback = delegate(string str, object data)
			{
				HashedString perk_id = (HashedString)data;
				List<RoleConfig> rolesWithPerk = Game.Instance.roleManager.GetRolesWithPerk(perk_id);
				List<string> list = new List<string>();
				foreach (RoleConfig item6 in rolesWithPerk)
				{
					list.Add(item6.GetProperName());
				}
				str = str.Replace("{Roles}", string.Join(", ", list.ToArray()));
				return str;
			};
			SwitchStatusActive = CreateStatusItem("SwitchStatusActive", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			SwitchStatusInactive = CreateStatusItem("SwitchStatusInactive", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			PendingFish = CreateStatusItem("PendingFish", "BUILDING", "status_item_pending_fish", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			PendingSwitchToggle = CreateStatusItem("PendingSwitchToggle", "BUILDING", "status_item_pending_switch_toggle", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			PendingUpgrade = CreateStatusItem("PendingUpgrade", "BUILDING", "status_item_pending_upgrade", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			PendingWork = CreateStatusItem("PendingWork", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			PowerButtonOff = CreateStatusItem("PowerButtonOff", "BUILDING", "status_item_power_button_off", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			PressureOk = CreateStatusItem("PressureOk", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.Oxygen.ID, true, 63486);
			UnderPressure = CreateStatusItem("UnderPressure", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.Oxygen.ID, true, 63486);
			Unassigned = CreateStatusItem("Unassigned", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.Rooms.ID, true, 63486);
			AssignedPublic = CreateStatusItem("AssignedPublic", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Rooms.ID, true, 63486);
			UnderConstruction = CreateStatusItem("UnderConstruction", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			UnderConstructionNoWorker = CreateStatusItem("UnderConstructionNoWorker", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Normal = CreateStatusItem("Normal", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			ManualGeneratorChargingUp = CreateStatusItem("ManualGeneratorChargingUp", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 63486);
			ManualGeneratorReleasingEnergy = CreateStatusItem("ManualGeneratorReleasingEnergy", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 63486);
			GeneratorOffline = CreateStatusItem("GeneratorOffline", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.Power.ID, true, 63486);
			Pipe = CreateStatusItem("Pipe", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.LiquidConduits.ID, true, 63486);
			Pipe.resolveStringCallback = delegate(string str, object data)
			{
				Conduit conduit = (Conduit)data;
				int cell2 = Grid.PosToCell(conduit);
				ConduitFlow flowManager = conduit.GetFlowManager();
				ConduitFlow.ConduitContents contents2 = flowManager.GetContents(cell2);
				string text3 = BUILDING.STATUSITEMS.PIPECONTENTS.EMPTY;
				if (contents2.mass > 0f)
				{
					Element element2 = ElementLoader.FindElementByHash(contents2.element);
					text3 = string.Format(BUILDING.STATUSITEMS.PIPECONTENTS.CONTENTS, GameUtil.GetFormattedMass(contents2.mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), element2.name, GameUtil.GetFormattedTemperature(contents2.temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
					if ((UnityEngine.Object)OverlayScreen.Instance != (UnityEngine.Object)null && OverlayScreen.Instance.mode == OverlayModes.Disease.ID && contents2.diseaseIdx != 255)
					{
						text3 += string.Format(BUILDING.STATUSITEMS.PIPECONTENTS.CONTENTS_WITH_DISEASE, GameUtil.GetFormattedDisease(contents2.diseaseIdx, contents2.diseaseCount, true));
					}
				}
				str = str.Replace("{Contents}", text3);
				return str;
			};
			Conveyor = CreateStatusItem("Conveyor", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.SolidConveyor.ID, true, 63486);
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
							Element element = ElementLoader.FindElementByHash(component3.ElementID);
							text2 = string.Format(BUILDING.STATUSITEMS.CONVEYOR_CONTENTS.CONTENTS, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), element.name, GameUtil.GetFormattedTemperature(component3.Temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
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
			FabricatorEmpty = CreateStatusItem("FabricatorEmpty", "BUILDING", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Toilet = CreateStatusItem("Toilet", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Toilet.resolveStringCallback = delegate(string str, object data)
			{
				Toilet.StatesInstance statesInstance3 = (Toilet.StatesInstance)data;
				if (statesInstance3 != null)
				{
					str = str.Replace("{FlushesRemaining}", statesInstance3.GetFlushesRemaining().ToString());
				}
				return str;
			};
			ToiletNeedsEmptying = CreateStatusItem("ToiletNeedsEmptying", "BUILDING", "status_item_toilet_needs_emptying", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Unusable = CreateStatusItem("Unusable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NoResearchSelected = CreateStatusItem("NoResearchSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NoResearchSelected.AddNotification(null, null, null, 0f);
			StatusItem noResearchSelected = NoResearchSelected;
			noResearchSelected.resolveTooltipCallback = (Func<string, object, string>)Delegate.Combine(noResearchSelected.resolveTooltipCallback, (Func<string, object, string>)delegate(string str, object data)
			{
				BindingEntry bindingEntry3 = GameInputMapping.FindEntry(Action.ManageResearch);
				string newValue5 = bindingEntry3.mKeyCode.ToString();
				str = str.Replace("{RESEARCH_MENU_KEY}", newValue5);
				return str;
			});
			NoApplicableResearchSelected = CreateStatusItem("NoApplicableResearchSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NoApplicableResearchSelected.AddNotification(null, null, null, 0f);
			NoApplicableAnalysisSelected = CreateStatusItem("NoApplicableAnalysisSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NoApplicableAnalysisSelected.AddNotification(null, null, null, 0f);
			StatusItem noApplicableAnalysisSelected = NoApplicableAnalysisSelected;
			noApplicableAnalysisSelected.resolveTooltipCallback = (Func<string, object, string>)Delegate.Combine(noApplicableAnalysisSelected.resolveTooltipCallback, (Func<string, object, string>)delegate(string str, object data)
			{
				BindingEntry bindingEntry2 = GameInputMapping.FindEntry(Action.ManageStarmap);
				string newValue4 = bindingEntry2.mKeyCode.ToString();
				str = str.Replace("{STARMAP_MENU_KEY}", newValue4);
				return str;
			});
			NoResearchOrDestinationSelected = CreateStatusItem("NoResearchOrDestinationSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NoResearchOrDestinationSelected.AddNotification(null, null, null, 0f);
			ValveRequest = CreateStatusItem("ValveRequest", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			ValveRequest.resolveStringCallback = delegate(string str, object data)
			{
				Valve valve = (Valve)data;
				str = str.Replace("{QueuedMaxFlow}", GameUtil.GetFormattedMass(valve.QueuedMaxFlow, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			EmittingLight = CreateStatusItem("EmittingLight", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			EmittingLight.resolveStringCallback = delegate(string str, object data)
			{
				BindingEntry bindingEntry = GameInputMapping.FindEntry(Action.Overlay5);
				string newValue3 = bindingEntry.mKeyCode.ToString();
				str = str.Replace("{LightGridOverlay}", newValue3);
				return str;
			};
			RationBoxContents = CreateStatusItem("RationBoxContents", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
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
				foreach (GameObject item7 in component.items)
				{
					Edible component2 = item7.GetComponent<Edible>();
					if ((bool)component2)
					{
						num2 += component2.Calories;
					}
				}
				str = str.Replace("{Stored}", GameUtil.GetFormattedCalories(num2, GameUtil.TimeSlice.None, true));
				return str;
			};
			EmittingElement = CreateStatusItem("EmittingElement", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			EmittingElement.resolveStringCallback = delegate(string str, object data)
			{
				IElementEmitter elementEmitter2 = (IElementEmitter)data;
				string newValue2 = ElementLoader.FindElementByHash(elementEmitter2.Element).tag.ProperName();
				str = str.Replace("{ElementType}", newValue2);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(elementEmitter2.AverageEmitRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			EmittingOxygenAvg = CreateStatusItem("EmittingOxygenAvg", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			EmittingOxygenAvg.resolveStringCallback = delegate(string str, object data)
			{
				Sublimates sublimates2 = (Sublimates)data;
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(sublimates2.AvgFlowRate(), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			EmittingGasAvg = CreateStatusItem("EmittingGasAvg", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			EmittingGasAvg.resolveStringCallback = delegate(string str, object data)
			{
				Sublimates sublimates = (Sublimates)data;
				str = str.Replace("{Element}", ElementLoader.FindElementByHash(sublimates.info.sublimatedElement).name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(sublimates.AvgFlowRate(), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			PumpingLiquidOrGas = CreateStatusItem("PumpingLiquidOrGas", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.LiquidConduits.ID, true, 63486);
			PumpingLiquidOrGas.resolveStringCallback = delegate(string str, object data)
			{
				HandleVector<int>.Handle handle = (HandleVector<int>.Handle)data;
				float averageRate = Game.Instance.accumulators.GetAverageRate(handle);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(averageRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			PipeMayMelt = CreateStatusItem("PipeMayMelt", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NoLiquidElementToPump = CreateStatusItem("NoLiquidElementToPump", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID, true, 63486);
			NoGasElementToPump = CreateStatusItem("NoGasElementToPump", "BUILDING", "status_item_no_gas_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID, true, 63486);
			NoFilterElementSelected = CreateStatusItem("NoFilterElementSelected", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NoLureElementSelected = CreateStatusItem("NoLureElementSelected", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			ElementConsumer = CreateStatusItem("ElementConsumer", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 63486);
			ElementConsumer.resolveStringCallback = delegate(string str, object data)
			{
				ElementConsumer elementConsumer = (ElementConsumer)data;
				string newValue = ElementLoader.FindElementByHash(elementConsumer.elementToConsume).tag.ProperName();
				str = str.Replace("{ElementTypes}", newValue);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(elementConsumer.AverageConsumeRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			ElementEmitterOutput = CreateStatusItem("ElementEmitterOutput", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 63486);
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
			AwaitingWaste = CreateStatusItem("AwaitingWaste", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 63486);
			AwaitingCompostFlip = CreateStatusItem("AwaitingCompostFlip", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 63486);
			JoulesAvailable = CreateStatusItem("JoulesAvailable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 63486);
			JoulesAvailable.resolveStringCallback = delegate(string str, object data)
			{
				IEnergyProducer energyProducer = (IEnergyProducer)data;
				str = str.Replace("{JoulesAvailable}", GameUtil.GetFormattedJoules(energyProducer.JoulesAvailable, "F1", GameUtil.TimeSlice.None));
				return str;
			};
			Wattage = CreateStatusItem("Wattage", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 63486);
			Wattage.resolveStringCallback = delegate(string str, object data)
			{
				Generator generator = (Generator)data;
				str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(generator.WattageRating, GameUtil.WattageFormatterUnit.Automatic));
				return str;
			};
			SolarPanelWattage = CreateStatusItem("SolarPanelWattage", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 63486);
			SolarPanelWattage.resolveStringCallback = delegate(string str, object data)
			{
				SolarPanel solarPanel = (SolarPanel)data;
				str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(solarPanel.CurrentWattage, GameUtil.WattageFormatterUnit.Automatic));
				return str;
			};
			Wattson = CreateStatusItem("Wattson", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Wattson.resolveStringCallback = delegate(string str, object data)
			{
				Telepad telepad = (Telepad)data;
				str = (((UnityEngine.Object)GameFlowManager.Instance != (UnityEngine.Object)null && GameFlowManager.Instance.IsGameOver()) ? ((string)BUILDING.STATUSITEMS.WATTSONGAMEOVER.NAME) : ((!telepad.GetComponent<Operational>().IsOperational) ? str.Replace("{TimeRemaining}", BUILDING.STATUSITEMS.WATTSON.UNAVAILABLE) : str.Replace("{TimeRemaining}", GameUtil.GetFormattedCycles(telepad.GetTimeRemaining(), "F1"))));
				return str;
			};
			FlushToilet = CreateStatusItem("FlushToilet", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			FlushToiletInUse = CreateStatusItem("FlushToiletInUse", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			WireNominal = CreateStatusItem("WireNominal", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 63486);
			WireConnected = CreateStatusItem("WireConnected", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID, true, 63486);
			WireDisconnected = CreateStatusItem("WireDisconnected", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.Power.ID, true, 63486);
			Overheated = CreateStatusItem("Overheated", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 63486);
			Cooling = CreateStatusItem("Cooling", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Func<string, object, string> resolveStringCallback2 = delegate(string str, object data)
			{
				AirConditioner airConditioner2 = (AirConditioner)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(airConditioner2.lastGasTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
			};
			CoolingStalledColdGas = CreateStatusItem("CoolingStalledColdGas", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			CoolingStalledColdGas.resolveStringCallback = resolveStringCallback2;
			CoolingStalledColdLiquid = CreateStatusItem("CoolingStalledColdLiquid", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			CoolingStalledColdLiquid.resolveStringCallback = resolveStringCallback2;
			Func<string, object, string> resolveStringCallback3 = delegate(string str, object data)
			{
				AirConditioner airConditioner = (AirConditioner)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(airConditioner.lastEnvTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(airConditioner.lastGasTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(airConditioner.maxEnvironmentDelta, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative, true, false));
			};
			CoolingStalledHotEnv = CreateStatusItem("CoolingStalledHotEnv", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			CoolingStalledHotEnv.resolveStringCallback = resolveStringCallback3;
			CoolingStalledHotLiquid = CreateStatusItem("CoolingStalledHotLiquid", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			CoolingStalledHotLiquid.resolveStringCallback = resolveStringCallback3;
			Working = CreateStatusItem("Working", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			NeedsValidRegion = CreateStatusItem("NeedsValidRegion", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			NeedSeed = CreateStatusItem("NeedSeed", "BUILDING", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			AwaitingSeedDelivery = CreateStatusItem("AwaitingSeedDelivery", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			AwaitingBaitDelivery = CreateStatusItem("AwaitingBaitDelivery", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			NoAvailableSeed = CreateStatusItem("NoAvailableSeed", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			NeedEgg = CreateStatusItem("NeedEgg", "BUILDING", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			AwaitingEggDelivery = CreateStatusItem("AwaitingEggDelivery", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			NoAvailableEgg = CreateStatusItem("NoAvailableEgg", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			Grave = CreateStatusItem("Grave", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Grave.resolveStringCallback = delegate(string str, object data)
			{
				Grave.StatesInstance statesInstance2 = (Grave.StatesInstance)data;
				string text = str.Replace("{DeadDupe}", statesInstance2.master.graveName);
				string[] strings = LocString.GetStrings(typeof(NAMEGEN.GRAVE.EPITAPHS));
				int num = statesInstance2.master.epitaphIdx % strings.Length;
				return text.Replace("{Epitaph}", strings[num]);
			};
			GraveEmpty = CreateStatusItem("GraveEmpty", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			CannotCoolFurther = CreateStatusItem("CannotCoolFurther", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			BuildingDisabled = CreateStatusItem("BuildingDisabled", "BUILDING", "status_item_building_disabled", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			Expired = CreateStatusItem("Expired", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			PumpingStation = CreateStatusItem("PumpingStation", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			PumpingStation.resolveStringCallback = delegate(string str, object data)
			{
				LiquidPumpingStation liquidPumpingStation = (LiquidPumpingStation)data;
				if ((UnityEngine.Object)liquidPumpingStation != (UnityEngine.Object)null)
				{
					return liquidPumpingStation.ResolveString(str);
				}
				return str;
			};
			EmptyPumpingStation = CreateStatusItem("EmptyPumpingStation", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			WellPressurizing = CreateStatusItem("WellPressurizing", BUILDING.STATUSITEMS.WELL_PRESSURIZING.NAME, BUILDING.STATUSITEMS.WELL_PRESSURIZING.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 63486);
			WellPressurizing.resolveStringCallback = delegate(string str, object data)
			{
				OilWellCap.StatesInstance statesInstance = (OilWellCap.StatesInstance)data;
				if (statesInstance != null)
				{
					return string.Format(str, GameUtil.GetFormattedPercent(100f * statesInstance.GetPressurePercent(), GameUtil.TimeSlice.None));
				}
				return str;
			};
			WellOverpressure = CreateStatusItem("WellOverpressure", BUILDING.STATUSITEMS.WELL_OVERPRESSURE.NAME, BUILDING.STATUSITEMS.WELL_OVERPRESSURE.TOOLTIP, string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, 63486);
			ReleasingPressure = CreateStatusItem("ReleasingPressure", BUILDING.STATUSITEMS.RELEASING_PRESSURE.NAME, BUILDING.STATUSITEMS.RELEASING_PRESSURE.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 63486);
			TooCold = CreateStatusItem("TooCold", BUILDING.STATUSITEMS.TOO_COLD.NAME, BUILDING.STATUSITEMS.TOO_COLD.TOOLTIP, string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, 63486);
			IncubatorProgress = CreateStatusItem("IncubatorProgress", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			IncubatorProgress.resolveStringCallback = delegate(string str, object data)
			{
				EggIncubator eggIncubator = (EggIncubator)data;
				str = str.Replace("{Percent}", GameUtil.GetFormattedPercent(eggIncubator.GetProgress() * 100f, GameUtil.TimeSlice.None));
				return str;
			};
			HabitatNeedsEmptying = CreateStatusItem("HabitatNeedsEmptying", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			DetectorScanning = CreateStatusItem("DetectorScanning", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			IncomingMeteors = CreateStatusItem("IncomingMeteors", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			HasGantry = CreateStatusItem("HasGantry", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			MissingGantry = CreateStatusItem("MissingGantry", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			DisembarkingDuplicant = CreateStatusItem("DisembarkingDuplicant", "BUILDING", "status_item_new_duplicants_available", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			RocketName = CreateStatusItem("RocketName", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
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
			PathNotClear = new StatusItem("PATH_NOT_CLEAR", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			PathNotClear.resolveTooltipCallback = delegate(string str, object data)
			{
				ConditionFlightPathIsClear conditionFlightPathIsClear = (ConditionFlightPathIsClear)data;
				if (conditionFlightPathIsClear != null)
				{
					str = string.Format(str, conditionFlightPathIsClear.GetObstruction());
				}
				return str;
			};
			InvalidPortOverlap = CreateStatusItem("InvalidPortOverlap", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			InvalidPortOverlap.AddNotification(null, null, null, 0f);
			EmergencyPriority = CreateStatusItem("EmergencyPriority", BUILDING.STATUSITEMS.EMERGENCY_PRIORITY.NAME, BUILDING.STATUSITEMS.EMERGENCY_PRIORITY.TOOLTIP, "status_item_doubleexclamation", StatusItem.IconType.Custom, NotificationType.Bad, false, OverlayModes.None.ID, 63486);
			StatusItem emergencyPriority = EmergencyPriority;
			string notification_text = BUILDING.STATUSITEMS.EMERGENCY_PRIORITY.NOTIFICATION_NAME;
			emergencyPriority.AddNotification(null, notification_text, BUILDING.STATUSITEMS.EMERGENCY_PRIORITY.NOTIFICATION_TOOLTIP, 0f);
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
			return result;
		}
	}
}
