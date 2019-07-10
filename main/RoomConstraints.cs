using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class RoomConstraints
{
	public static class ConstraintTags
	{
		public static Tag Bed = "Bed".ToTag();

		public static Tag LuxuryBed = "LuxuryBed".ToTag();

		public static Tag Toilet = "Toilet".ToTag();

		public static Tag FlushToilet = "FlushToilet".ToTag();

		public static Tag MessTable = "MessTable".ToTag();

		public static Tag Clinic = "Clinic".ToTag();

		public static Tag FoodStorage = "FoodStorage".ToTag();

		public static Tag WashStation = "WashStation".ToTag();

		public static Tag AdvancedWashStation = "AdvancedWashStation".ToTag();

		public static Tag ResearchStation = "ResearchStation".ToTag();

		public static Tag LightSource = "LightSource".ToTag();

		public static Tag MassageTable = "MassageTable".ToTag();

		public static Tag IndustrialMachinery = "IndustrialMachinery".ToTag();

		public static Tag PowerStation = "PowerStation".ToTag();

		public static Tag FarmStation = "FarmStation".ToTag();

		public static Tag CreatureRelocator = "CreatureRelocator".ToTag();

		public static Tag CreatureFeeder = "CreatureFeeder".ToTag();

		public static Tag RanchStation = "RanchStation".ToTag();

		public static Tag RecBuilding = "RecBuilding".ToTag();

		public static Tag MachineShop = "MachineShop".ToTag();

		public static Tag Park = "Park".ToTag();

		public static Tag NatureReserve = "NatureReserve".ToTag();

		public static Tag Decor20 = "Decor20".ToTag();
	}

	public class Constraint
	{
		public string name;

		public string description;

		public int times_required = 1;

		public Func<Room, bool> room_criteria;

		public Func<KPrefabID, bool> building_criteria;

		public List<Constraint> stomp_in_conflict;

		public Constraint(Func<KPrefabID, bool> building_criteria, Func<Room, bool> room_criteria, int times_required = 1, string name = "", string description = "", List<Constraint> stomp_in_conflict = null)
		{
			this.room_criteria = room_criteria;
			this.building_criteria = building_criteria;
			this.times_required = times_required;
			this.name = name;
			this.description = description;
			this.stomp_in_conflict = stomp_in_conflict;
		}

		public bool isSatisfied(Room room)
		{
			int num = 0;
			if (room_criteria != null && room_criteria(room))
			{
				num++;
			}
			if (building_criteria != null)
			{
				foreach (KPrefabID building in room.buildings)
				{
					if (!((UnityEngine.Object)building == (UnityEngine.Object)null) && building_criteria(building))
					{
						num++;
					}
				}
			}
			if (num >= times_required)
			{
				return true;
			}
			return false;
		}
	}

	public static Constraint CEILING_HEIGHT_4 = new Constraint(null, (Room room) => 1 + room.cavity.maxY - room.cavity.minY >= 4, 1, string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.NAME, "4"), string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.DESCRIPTION, "4"), null);

	public static Constraint CEILING_HEIGHT_6 = new Constraint(null, (Room room) => 1 + room.cavity.maxY - room.cavity.minY >= 6, 1, string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.NAME, "6"), string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.DESCRIPTION, "6"), null);

	public static Constraint MINIMUM_SIZE_12 = new Constraint(null, (Room room) => room.cavity.numCells >= 12, 1, string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.NAME, "12"), string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, "12"), null);

	public static Constraint MINIMUM_SIZE_32 = new Constraint(null, (Room room) => room.cavity.numCells >= 32, 1, string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.NAME, "32"), string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, "32"), null);

	public static Constraint MAXIMUM_SIZE_64 = new Constraint(null, (Room room) => room.cavity.numCells <= 64, 1, string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "64"), string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "64"), null);

	public static Constraint MAXIMUM_SIZE_96 = new Constraint(null, (Room room) => room.cavity.numCells <= 96, 1, string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "96"), string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "96"), null);

	public static Constraint MAXIMUM_SIZE_120 = new Constraint(null, (Room room) => room.cavity.numCells <= 120, 1, string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "120"), string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "120"), null);

	public static Constraint NO_INDUSTRIAL_MACHINERY = new Constraint(null, delegate(Room room)
	{
		foreach (KPrefabID building in room.buildings)
		{
			if (building.HasTag(ConstraintTags.IndustrialMachinery))
			{
				return false;
			}
		}
		return true;
	}, 1, ROOMS.CRITERIA.NO_INDUSTRIAL_MACHINERY.NAME, ROOMS.CRITERIA.NO_INDUSTRIAL_MACHINERY.DESCRIPTION, null);

	public static Constraint NO_COTS = new Constraint(null, delegate(Room room)
	{
		foreach (KPrefabID building2 in room.buildings)
		{
			if (building2.HasTag(ConstraintTags.Bed) && !building2.HasTag(ConstraintTags.LuxuryBed))
			{
				return false;
			}
		}
		return true;
	}, 1, ROOMS.CRITERIA.NO_COTS.NAME, ROOMS.CRITERIA.NO_COTS.DESCRIPTION, null);

	public static Constraint NO_OUTHOUSES = new Constraint(null, delegate(Room room)
	{
		foreach (KPrefabID building3 in room.buildings)
		{
			if (building3.HasTag(ConstraintTags.Toilet) && !building3.HasTag(ConstraintTags.FlushToilet))
			{
				return false;
			}
		}
		return true;
	}, 1, ROOMS.CRITERIA.NO_OUTHOUSES.NAME, ROOMS.CRITERIA.NO_OUTHOUSES.DESCRIPTION, null);

	public static Constraint LUXURY_BED_SINGLE = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.LuxuryBed), null, 1, ROOMS.CRITERIA.LUXURY_BED_SINGLE.NAME, ROOMS.CRITERIA.LUXURY_BED_SINGLE.DESCRIPTION, null);

	public static Constraint BED_SINGLE = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.Bed) && !bc.HasTag(ConstraintTags.Clinic), null, 1, ROOMS.CRITERIA.BED_SINGLE.NAME, ROOMS.CRITERIA.BED_SINGLE.DESCRIPTION, null);

	public static Constraint BUILDING_DECOR_POSITIVE = new Constraint(delegate(KPrefabID bc)
	{
		DecorProvider component3 = bc.GetComponent<DecorProvider>();
		if ((UnityEngine.Object)component3 != (UnityEngine.Object)null && component3.baseDecor > 0f)
		{
			return true;
		}
		return false;
	}, null, 1, ROOMS.CRITERIA.BUILDING_DECOR_POSITIVE.NAME, ROOMS.CRITERIA.BUILDING_DECOR_POSITIVE.DESCRIPTION, null);

	public static Constraint DECORATIVE_ITEM = new Constraint((KPrefabID bc) => bc.HasTag(GameTags.Decoration), null, 1, ROOMS.CRITERIA.DECORATIVE_ITEM.NAME, ROOMS.CRITERIA.DECORATIVE_ITEM.DESCRIPTION, null);

	public static Constraint DECORATIVE_ITEM_20 = new Constraint((KPrefabID bc) => bc.HasTag(GameTags.Decoration) && bc.HasTag(ConstraintTags.Decor20), null, 1, string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM_N.NAME, "20"), string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM_N.DESCRIPTION, "20"), null);

	public static Constraint POWER_STATION = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.PowerStation), null, 1, ROOMS.CRITERIA.POWER_STATION.NAME, ROOMS.CRITERIA.POWER_STATION.DESCRIPTION, null);

	public static Constraint FARM_STATION = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.FarmStation), null, 1, ROOMS.CRITERIA.FARM_STATION.NAME, ROOMS.CRITERIA.FARM_STATION.DESCRIPTION, null);

	public static Constraint RANCH_STATION = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.RanchStation), null, 1, ROOMS.CRITERIA.RANCH_STATION.NAME, ROOMS.CRITERIA.RANCH_STATION.DESCRIPTION, null);

	public static Constraint REC_BUILDING = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.RecBuilding), null, 1, ROOMS.CRITERIA.REC_BUILDING.NAME, ROOMS.CRITERIA.REC_BUILDING.DESCRIPTION, null);

	public static Constraint MACHINE_SHOP = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.MachineShop), null, 1, ROOMS.CRITERIA.MACHINE_SHOP.NAME, ROOMS.CRITERIA.MACHINE_SHOP.DESCRIPTION, null);

	public static Constraint FOOD_BOX = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.FoodStorage), null, 1, ROOMS.CRITERIA.FOOD_BOX.NAME, ROOMS.CRITERIA.FOOD_BOX.DESCRIPTION, null);

	public static Constraint LIGHT = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.LightSource), null, 1, ROOMS.CRITERIA.LIGHT.NAME, ROOMS.CRITERIA.LIGHT.DESCRIPTION, null);

	public static Constraint MASSAGE_TABLE = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.MassageTable), null, 1, ROOMS.CRITERIA.MASSAGE_TABLE.NAME, ROOMS.CRITERIA.MASSAGE_TABLE.DESCRIPTION, null);

	public static Constraint MESS_STATION_SINGLE = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.MessTable), null, 1, ROOMS.CRITERIA.MESS_STATION_SINGLE.NAME, ROOMS.CRITERIA.MESS_STATION_SINGLE.DESCRIPTION, new List<Constraint>
	{
		REC_BUILDING
	});

	public static Constraint RESEARCH_STATION = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.ResearchStation), null, 1, ROOMS.CRITERIA.RESEARCH_STATION.NAME, ROOMS.CRITERIA.RESEARCH_STATION.DESCRIPTION, null);

	public static Constraint TOILET = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.Toilet), null, 1, ROOMS.CRITERIA.TOILET.NAME, ROOMS.CRITERIA.TOILET.DESCRIPTION, null);

	public static Constraint FLUSH_TOILET = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.FlushToilet), null, 1, ROOMS.CRITERIA.FLUSH_TOILET.NAME, ROOMS.CRITERIA.FLUSH_TOILET.DESCRIPTION, null);

	public static Constraint WASH_STATION = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.WashStation), null, 1, ROOMS.CRITERIA.WASH_STATION.NAME, ROOMS.CRITERIA.WASH_STATION.DESCRIPTION, null);

	public static Constraint ADVANCED_WASH_STATION = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.AdvancedWashStation), null, 1, ROOMS.CRITERIA.ADVANCED_WASH_STATION.NAME, ROOMS.CRITERIA.ADVANCED_WASH_STATION.DESCRIPTION, null);

	public static Constraint CLINIC = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.Clinic), null, 1, ROOMS.CRITERIA.CLINIC.NAME, ROOMS.CRITERIA.CLINIC.DESCRIPTION, new List<Constraint>
	{
		TOILET,
		FLUSH_TOILET,
		MESS_STATION_SINGLE
	});

	public static Constraint PARK_BUILDING = new Constraint((KPrefabID bc) => bc.HasTag(ConstraintTags.Park), null, 1, ROOMS.CRITERIA.PARK_BUILDING.NAME, ROOMS.CRITERIA.PARK_BUILDING.DESCRIPTION, null);

	public static Constraint ORIGINALTILES = new Constraint(null, (Room room) => 1 + room.cavity.maxY - room.cavity.minY >= 4, 1, "", "", null);

	public static Constraint WILDANIMAL = new Constraint(null, delegate(Room room)
	{
		int num4 = room.cavity.creatures.Count + room.cavity.eggs.Count;
		return num4 > 0;
	}, 1, ROOMS.CRITERIA.WILDANIMAL.NAME, ROOMS.CRITERIA.WILDANIMAL.DESCRIPTION, null);

	public static Constraint WILDANIMALS;

	public static Constraint WILDPLANT;

	public static Constraint WILDPLANTS;

	public static string RoomCriteriaString(Room room)
	{
		string str = "";
		RoomType roomType = room.roomType;
		if (roomType != Db.Get().RoomTypes.Neutral)
		{
			str = str + "<b>" + ROOMS.CRITERIA.HEADER + "</b>";
			str = str + "\n    • " + roomType.primary_constraint.name;
			if (roomType.additional_constraints != null)
			{
				Constraint[] additional_constraints = roomType.additional_constraints;
				foreach (Constraint constraint in additional_constraints)
				{
					str = ((!constraint.isSatisfied(room)) ? (str + "\n<color=#F44A47FF>    • " + constraint.name + "</color>") : (str + "\n    • " + constraint.name));
				}
			}
		}
		else
		{
			RoomType[] possibleRoomTypes = Db.Get().RoomTypes.GetPossibleRoomTypes(room);
			str += ((possibleRoomTypes.Length <= 1) ? "" : ("<b>" + ROOMS.CRITERIA.POSSIBLE_TYPES_HEADER + "</b>"));
			RoomType[] array = possibleRoomTypes;
			foreach (RoomType roomType2 in array)
			{
				if (roomType2 != Db.Get().RoomTypes.Neutral)
				{
					if (str != "")
					{
						str += "\n";
					}
					string text = str;
					str = text + "<b><color=#BCBCBC>    • " + roomType2.Name + "</b> (" + roomType2.primary_constraint.name + ")</color>";
					bool flag = false;
					if (roomType2.additional_constraints != null)
					{
						Constraint[] additional_constraints2 = roomType2.additional_constraints;
						foreach (Constraint constraint2 in additional_constraints2)
						{
							if (!constraint2.isSatisfied(room))
							{
								flag = true;
								str = ((constraint2.building_criteria == null) ? (str + "\n<color=#F44A47FF>        • " + string.Format(ROOMS.CRITERIA.CRITERIA_FAILED.FAILED, constraint2.name) + "</color>") : (str + "\n<color=#F44A47FF>        • " + string.Format(ROOMS.CRITERIA.CRITERIA_FAILED.MISSING_BUILDING, constraint2.name) + "</color>"));
							}
						}
					}
					if (!flag)
					{
						bool flag2 = false;
						foreach (RoomType resource in Db.Get().RoomTypes.resources)
						{
							if (resource != roomType2 && resource != Db.Get().RoomTypes.Neutral && Db.Get().RoomTypes.HasAmbiguousRoomType(room, roomType2, resource))
							{
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							str = str + "\n<color=#F44A47FF>        • " + ROOMS.CRITERIA.NO_TYPE_CONFLICTS + "</color>";
						}
					}
				}
			}
		}
		return str;
	}

	static RoomConstraints()
	{
		Func<KPrefabID, bool> building_criteria = null;
		Func<Room, bool> room_criteria = delegate(Room room)
		{
			int num3 = 0;
			foreach (KPrefabID creature in room.cavity.creatures)
			{
				if (creature.HasTag(GameTags.Creatures.Wild))
				{
					num3++;
				}
			}
			return num3 >= 2;
		};
		string name = ROOMS.CRITERIA.WILDANIMALS.NAME;
		WILDANIMALS = new Constraint(building_criteria, room_criteria, 1, name, ROOMS.CRITERIA.WILDANIMALS.DESCRIPTION, null);
		building_criteria = null;
		room_criteria = delegate(Room room)
		{
			int num2 = 0;
			foreach (KPrefabID plant in room.cavity.plants)
			{
				if ((UnityEngine.Object)plant != (UnityEngine.Object)null)
				{
					ReceptacleMonitor component2 = plant.GetComponent<ReceptacleMonitor>();
					if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && !component2.Replanted)
					{
						num2++;
					}
				}
			}
			return num2 >= 2;
		};
		name = ROOMS.CRITERIA.WILDPLANT.NAME;
		WILDPLANT = new Constraint(building_criteria, room_criteria, 1, name, ROOMS.CRITERIA.WILDPLANT.DESCRIPTION, null);
		WILDPLANTS = new Constraint(null, delegate(Room room)
		{
			int num = 0;
			foreach (KPrefabID plant2 in room.cavity.plants)
			{
				if ((UnityEngine.Object)plant2 != (UnityEngine.Object)null)
				{
					ReceptacleMonitor component = plant2.GetComponent<ReceptacleMonitor>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null && !component.Replanted)
					{
						num++;
					}
				}
			}
			return num >= 4;
		}, 1, ROOMS.CRITERIA.WILDPLANTS.NAME, ROOMS.CRITERIA.WILDPLANTS.DESCRIPTION, null);
	}
}
