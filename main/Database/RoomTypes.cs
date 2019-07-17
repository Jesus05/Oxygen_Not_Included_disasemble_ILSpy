using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
	public class RoomTypes : ResourceSet<RoomType>
	{
		public RoomType Neutral;

		public RoomType Latrine;

		public RoomType PlumbedBathroom;

		public RoomType Barracks;

		public RoomType Bedroom;

		public RoomType MessHall;

		public RoomType GreatHall;

		public RoomType Hospital;

		public RoomType MassageClinic;

		public RoomType PowerPlant;

		public RoomType Farm;

		public RoomType CreaturePen;

		public RoomType MachineShop;

		public RoomType RecRoom;

		public RoomType Park;

		public RoomType NatureReserve;

		public RoomTypes(ResourceSet parent)
			: base("RoomTypes", parent)
		{
			Initialize();
			Neutral = Add(new RoomType("Neutral", ROOMS.TYPES.NEUTRAL.NAME, ROOMS.TYPES.NEUTRAL.TOOLTIP, ROOMS.TYPES.NEUTRAL.EFFECT, Db.Get().RoomTypeCategories.None, null, null, new RoomDetails.Detail[4]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT,
				RoomDetails.CREATURE_COUNT,
				RoomDetails.PLANT_COUNT
			}, 0, null, false, false, null));
			string id = "PlumbedBathroom";
			string name = ROOMS.TYPES.PLUMBEDBATHROOM.NAME;
			string tooltip = ROOMS.TYPES.PLUMBEDBATHROOM.TOOLTIP;
			string effect = ROOMS.TYPES.PLUMBEDBATHROOM.EFFECT;
			RoomTypeCategory bathroom = Db.Get().RoomTypeCategories.Bathroom;
			RoomConstraints.Constraint fLUSH_TOILET = RoomConstraints.FLUSH_TOILET;
			RoomConstraints.Constraint[] additional_constraints = new RoomConstraints.Constraint[5]
			{
				RoomConstraints.ADVANCED_WASH_STATION,
				RoomConstraints.NO_OUTHOUSES,
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			};
			RoomDetails.Detail[] display_details = new RoomDetails.Detail[2]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			};
			int priority = 1;
			string[] effects = new string[1]
			{
				"RoomBathroom"
			};
			PlumbedBathroom = Add(new RoomType(id, name, tooltip, effect, bathroom, fLUSH_TOILET, additional_constraints, display_details, priority, null, false, false, effects));
			effect = "Latrine";
			tooltip = ROOMS.TYPES.LATRINE.NAME;
			name = ROOMS.TYPES.LATRINE.TOOLTIP;
			id = ROOMS.TYPES.LATRINE.EFFECT;
			bathroom = Db.Get().RoomTypeCategories.Bathroom;
			fLUSH_TOILET = RoomConstraints.TOILET;
			additional_constraints = new RoomConstraints.Constraint[4]
			{
				RoomConstraints.WASH_STATION,
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			};
			display_details = new RoomDetails.Detail[2]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			};
			priority = 1;
			RoomType[] upgrade_paths = new RoomType[1]
			{
				PlumbedBathroom
			};
			effects = new string[1]
			{
				"RoomLatrine"
			};
			Latrine = Add(new RoomType(effect, tooltip, name, id, bathroom, fLUSH_TOILET, additional_constraints, display_details, priority, upgrade_paths, false, false, effects));
			id = "Bedroom";
			name = ROOMS.TYPES.BEDROOM.NAME;
			tooltip = ROOMS.TYPES.BEDROOM.TOOLTIP;
			effect = ROOMS.TYPES.BEDROOM.EFFECT;
			bathroom = Db.Get().RoomTypeCategories.Sleep;
			fLUSH_TOILET = RoomConstraints.LUXURY_BED_SINGLE;
			additional_constraints = new RoomConstraints.Constraint[6]
			{
				RoomConstraints.NO_COTS,
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64,
				RoomConstraints.DECORATIVE_ITEM,
				RoomConstraints.CEILING_HEIGHT_4
			};
			display_details = new RoomDetails.Detail[2]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			};
			priority = 1;
			effects = new string[1]
			{
				"RoomBedroom"
			};
			Bedroom = Add(new RoomType(id, name, tooltip, effect, bathroom, fLUSH_TOILET, additional_constraints, display_details, priority, null, false, false, effects));
			effect = "Barracks";
			tooltip = ROOMS.TYPES.BARRACKS.NAME;
			name = ROOMS.TYPES.BARRACKS.TOOLTIP;
			id = ROOMS.TYPES.BARRACKS.EFFECT;
			bathroom = Db.Get().RoomTypeCategories.Sleep;
			fLUSH_TOILET = RoomConstraints.BED_SINGLE;
			additional_constraints = new RoomConstraints.Constraint[3]
			{
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			};
			display_details = new RoomDetails.Detail[2]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			};
			priority = 1;
			upgrade_paths = new RoomType[1]
			{
				Bedroom
			};
			effects = new string[1]
			{
				"RoomBarracks"
			};
			Barracks = Add(new RoomType(effect, tooltip, name, id, bathroom, fLUSH_TOILET, additional_constraints, display_details, priority, upgrade_paths, false, false, effects));
			id = "GreatHall";
			name = ROOMS.TYPES.GREATHALL.NAME;
			tooltip = ROOMS.TYPES.GREATHALL.TOOLTIP;
			effect = ROOMS.TYPES.GREATHALL.EFFECT;
			bathroom = Db.Get().RoomTypeCategories.Food;
			fLUSH_TOILET = RoomConstraints.MESS_STATION_SINGLE;
			additional_constraints = new RoomConstraints.Constraint[5]
			{
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_32,
				RoomConstraints.MAXIMUM_SIZE_120,
				RoomConstraints.DECORATIVE_ITEM_20,
				RoomConstraints.REC_BUILDING
			};
			display_details = new RoomDetails.Detail[2]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			};
			priority = 1;
			effects = new string[1]
			{
				"RoomGreatHall"
			};
			GreatHall = Add(new RoomType(id, name, tooltip, effect, bathroom, fLUSH_TOILET, additional_constraints, display_details, priority, null, false, false, effects));
			effect = "MessHall";
			tooltip = ROOMS.TYPES.MESSHALL.NAME;
			name = ROOMS.TYPES.MESSHALL.TOOLTIP;
			id = ROOMS.TYPES.MESSHALL.EFFECT;
			bathroom = Db.Get().RoomTypeCategories.Food;
			fLUSH_TOILET = RoomConstraints.MESS_STATION_SINGLE;
			additional_constraints = new RoomConstraints.Constraint[3]
			{
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			};
			display_details = new RoomDetails.Detail[2]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			};
			priority = 1;
			upgrade_paths = new RoomType[1]
			{
				GreatHall
			};
			effects = new string[1]
			{
				"RoomMessHall"
			};
			MessHall = Add(new RoomType(effect, tooltip, name, id, bathroom, fLUSH_TOILET, additional_constraints, display_details, priority, upgrade_paths, false, false, effects));
			MassageClinic = Add(new RoomType("MassageClinic", ROOMS.TYPES.MASSAGE_CLINIC.NAME, ROOMS.TYPES.MASSAGE_CLINIC.TOOLTIP, ROOMS.TYPES.MASSAGE_CLINIC.EFFECT, Db.Get().RoomTypeCategories.Hospital, RoomConstraints.MASSAGE_TABLE, new RoomConstraints.Constraint[4]
			{
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.DECORATIVE_ITEM,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			}, new RoomDetails.Detail[2]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 2, null, true, true, null));
			Hospital = Add(new RoomType("Hospital", ROOMS.TYPES.HOSPITAL.NAME, ROOMS.TYPES.HOSPITAL.TOOLTIP, ROOMS.TYPES.HOSPITAL.EFFECT, Db.Get().RoomTypeCategories.Hospital, RoomConstraints.CLINIC, new RoomConstraints.Constraint[5]
			{
				RoomConstraints.TOILET,
				RoomConstraints.MESS_STATION_SINGLE,
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_96
			}, new RoomDetails.Detail[2]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 2, null, true, true, null));
			PowerPlant = Add(new RoomType("PowerPlant", ROOMS.TYPES.POWER_PLANT.NAME, ROOMS.TYPES.POWER_PLANT.TOOLTIP, ROOMS.TYPES.POWER_PLANT.EFFECT, Db.Get().RoomTypeCategories.Industrial, RoomConstraints.POWER_STATION, new RoomConstraints.Constraint[2]
			{
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_96
			}, new RoomDetails.Detail[2]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 2, null, true, true, null));
			Farm = Add(new RoomType("Farm", ROOMS.TYPES.FARM.NAME, ROOMS.TYPES.FARM.TOOLTIP, ROOMS.TYPES.FARM.EFFECT, Db.Get().RoomTypeCategories.Agricultural, RoomConstraints.FARM_STATION, new RoomConstraints.Constraint[2]
			{
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_96
			}, new RoomDetails.Detail[2]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 2, null, true, true, null));
			CreaturePen = Add(new RoomType("CreaturePen", ROOMS.TYPES.CREATUREPEN.NAME, ROOMS.TYPES.CREATUREPEN.TOOLTIP, ROOMS.TYPES.CREATUREPEN.EFFECT, Db.Get().RoomTypeCategories.Agricultural, RoomConstraints.RANCH_STATION, new RoomConstraints.Constraint[2]
			{
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_96
			}, new RoomDetails.Detail[3]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT,
				RoomDetails.CREATURE_COUNT
			}, 2, null, true, true, null));
			MachineShop = new RoomType("MachineShop", ROOMS.TYPES.MACHINE_SHOP.NAME, ROOMS.TYPES.MACHINE_SHOP.TOOLTIP, ROOMS.TYPES.MACHINE_SHOP.EFFECT, Db.Get().RoomTypeCategories.Industrial, RoomConstraints.MACHINE_SHOP, new RoomConstraints.Constraint[2]
			{
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_96
			}, new RoomDetails.Detail[2]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 2, null, true, true, null);
			RecRoom = Add(new RoomType("RecRoom", ROOMS.TYPES.REC_ROOM.NAME, ROOMS.TYPES.REC_ROOM.TOOLTIP, ROOMS.TYPES.REC_ROOM.EFFECT, Db.Get().RoomTypeCategories.Recreation, RoomConstraints.REC_BUILDING, new RoomConstraints.Constraint[4]
			{
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.DECORATIVE_ITEM,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			}, new RoomDetails.Detail[2]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 0, null, true, true, null));
			id = "NatureReserve";
			name = ROOMS.TYPES.NATURERESERVE.NAME;
			tooltip = ROOMS.TYPES.NATURERESERVE.TOOLTIP;
			effect = ROOMS.TYPES.NATURERESERVE.EFFECT;
			bathroom = Db.Get().RoomTypeCategories.Park;
			fLUSH_TOILET = RoomConstraints.PARK_BUILDING;
			additional_constraints = new RoomConstraints.Constraint[4]
			{
				RoomConstraints.WILDPLANTS,
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_32,
				RoomConstraints.MAXIMUM_SIZE_120
			};
			display_details = new RoomDetails.Detail[4]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT,
				RoomDetails.CREATURE_COUNT,
				RoomDetails.PLANT_COUNT
			};
			priority = 1;
			effects = new string[1]
			{
				"RoomNatureReserve"
			};
			NatureReserve = Add(new RoomType(id, name, tooltip, effect, bathroom, fLUSH_TOILET, additional_constraints, display_details, priority, null, false, false, effects));
			effect = "Park";
			tooltip = ROOMS.TYPES.PARK.NAME;
			name = ROOMS.TYPES.PARK.TOOLTIP;
			id = ROOMS.TYPES.PARK.EFFECT;
			bathroom = Db.Get().RoomTypeCategories.Park;
			fLUSH_TOILET = RoomConstraints.PARK_BUILDING;
			additional_constraints = new RoomConstraints.Constraint[4]
			{
				RoomConstraints.WILDPLANT,
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			};
			display_details = new RoomDetails.Detail[4]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT,
				RoomDetails.CREATURE_COUNT,
				RoomDetails.PLANT_COUNT
			};
			priority = 1;
			upgrade_paths = new RoomType[1]
			{
				NatureReserve
			};
			effects = new string[1]
			{
				"RoomPark"
			};
			Park = Add(new RoomType(effect, tooltip, name, id, bathroom, fLUSH_TOILET, additional_constraints, display_details, priority, upgrade_paths, false, false, effects));
		}

		public Assignables[] GetAssignees(Room room)
		{
			if (room != null)
			{
				RoomType roomType = room.roomType;
				if (roomType.primary_constraint != null)
				{
					List<Assignables> list = new List<Assignables>();
					foreach (KPrefabID building in room.buildings)
					{
						if (!((UnityEngine.Object)building == (UnityEngine.Object)null) && roomType.primary_constraint.building_criteria(building))
						{
							Assignable component = building.GetComponent<Assignable>();
							if (component.assignee != null)
							{
								foreach (Ownables owner in component.assignee.GetOwners())
								{
									if (!list.Contains(owner))
									{
										list.Add(owner);
									}
								}
							}
						}
					}
					return list.ToArray();
				}
				return new Assignables[0];
			}
			return new Assignables[0];
		}

		public RoomType GetRoomTypeForID(string id)
		{
			foreach (RoomType resource in resources)
			{
				if (resource.Id == id)
				{
					return resource;
				}
			}
			return null;
		}

		public RoomType GetRoomType(Room room)
		{
			foreach (RoomType resource in resources)
			{
				if (resource != Neutral && resource.isSatisfactory(room) == RoomType.RoomIdentificationResult.all_satisfied)
				{
					bool flag = false;
					foreach (RoomType resource2 in resources)
					{
						if (resource != resource2 && resource2 != Neutral && HasAmbiguousRoomType(room, resource, resource2))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return resource;
					}
				}
			}
			return Neutral;
		}

		public bool HasAmbiguousRoomType(Room room, RoomType suspected_type, RoomType potential_type)
		{
			RoomType.RoomIdentificationResult roomIdentificationResult = potential_type.isSatisfactory(room);
			RoomType.RoomIdentificationResult roomIdentificationResult2 = suspected_type.isSatisfactory(room);
			if (roomIdentificationResult == RoomType.RoomIdentificationResult.all_satisfied && roomIdentificationResult2 == RoomType.RoomIdentificationResult.all_satisfied)
			{
				if (potential_type.priority > suspected_type.priority)
				{
					return true;
				}
				if (suspected_type.upgrade_paths != null && Array.IndexOf(suspected_type.upgrade_paths, potential_type) != -1)
				{
					return true;
				}
				if (potential_type.upgrade_paths != null && Array.IndexOf(potential_type.upgrade_paths, suspected_type) != -1)
				{
					return false;
				}
			}
			if (roomIdentificationResult != RoomType.RoomIdentificationResult.primary_unsatisfied)
			{
				if (suspected_type.upgrade_paths != null && Array.IndexOf(suspected_type.upgrade_paths, potential_type) != -1)
				{
					return false;
				}
				if (suspected_type.primary_constraint != potential_type.primary_constraint)
				{
					bool flag = false;
					if (suspected_type.primary_constraint.stomp_in_conflict != null && suspected_type.primary_constraint.stomp_in_conflict.Contains(potential_type.primary_constraint))
					{
						flag = true;
					}
					else if (suspected_type.additional_constraints != null)
					{
						RoomConstraints.Constraint[] additional_constraints = suspected_type.additional_constraints;
						foreach (RoomConstraints.Constraint constraint in additional_constraints)
						{
							if (constraint == potential_type.primary_constraint || (constraint.stomp_in_conflict != null && constraint.stomp_in_conflict.Contains(potential_type.primary_constraint)))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						return true;
					}
					return false;
				}
				suspected_type = Neutral;
			}
			return false;
		}

		public RoomType[] GetPossibleRoomTypes(Room room)
		{
			RoomType[] array = new RoomType[Count];
			int num = 0;
			foreach (RoomType resource in resources)
			{
				if (resource != Neutral && (resource.isSatisfactory(room) == RoomType.RoomIdentificationResult.all_satisfied || resource.isSatisfactory(room) == RoomType.RoomIdentificationResult.primary_satisfied))
				{
					array[num] = resource;
					num++;
				}
			}
			if (num == 0)
			{
				array[num] = Neutral;
				num++;
			}
			Array.Resize(ref array, num);
			return array;
		}
	}
}
