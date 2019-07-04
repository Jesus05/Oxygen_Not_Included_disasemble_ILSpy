namespace STRINGS
{
	public class ROOMS
	{
		public class TYPES
		{
			public class NEUTRAL
			{
				public static LocString NAME = "Miscellaneous Room";

				public static LocString EFFECT = "- No effect";

				public static LocString TOOLTIP = "This area has walls and doors but no dedicated use.";
			}

			public class LATRINE
			{
				public static LocString NAME = "Latrine";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Using a toilet in an enclosed room will improve Duplicants' Morale.";
			}

			public class PLUMBEDBATHROOM
			{
				public static LocString NAME = "Washroom";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Using a fully plumbed Washroom will improve Duplicants' Morale.";
			}

			public class BARRACKS
			{
				public static LocString NAME = "Barracks";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Sleeping in Barracks will improve Duplicants' Morale.";
			}

			public class BEDROOM
			{
				public static LocString NAME = "Bedroom";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Sleeping in a private Bedroom will improve Duplicants' Morale.";
			}

			public class MESSHALL
			{
				public static LocString NAME = "Mess Hall";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Eating at a Mess Table in a Mess Hall will improve Duplicants' Morale.";
			}

			public class GREATHALL
			{
				public static LocString NAME = "Great Hall";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Eating in a Great Hall will significantly improve Duplicants' Morale.";
			}

			public class HOSPITAL
			{
				public static LocString NAME = "Hospital";

				public static LocString EFFECT = "- Quarantine sick Duplicants";

				public static LocString TOOLTIP = "Sick Duplicants assigned to medical buildings located within a Hospital are less likely to spread Disease.";
			}

			public class MASSAGE_CLINIC
			{
				public static LocString NAME = "Massage Clinic";

				public static LocString EFFECT = "- Massage stress relief bonus";

				public static LocString TOOLTIP = "Receiving massages at a Massage Clinic will significantly improve Stress reduction.";
			}

			public class POWER_PLANT
			{
				public static LocString NAME = "Power Plant";

				public static LocString EFFECT = "- Enables Power Control Station use";

				public static LocString TOOLTIP = "Generators built within a Power Plant can be tuned up using Power Control Stations to improve their power production.";
			}

			public class MACHINE_SHOP
			{
				public static LocString NAME = "Machine Shop";

				public static LocString EFFECT = "- Increased fabrication efficiency";

				public static LocString TOOLTIP = "Duplicants working in a Machine Shop can maintain buildings and increase their production speed.";
			}

			public class FARM
			{
				public static LocString NAME = "Greenhouse";

				public static LocString EFFECT = "- Enables Farm Station use";

				public static LocString TOOLTIP = "Crops grown within a Greenhouse can be tended with Farm Station fertilizer to increase their growth speed.";
			}

			public class CREATUREPEN
			{
				public static LocString NAME = "Stable";

				public static LocString EFFECT = "- Enables Grooming Station use";

				public static LocString TOOLTIP = "Stabled critters can be tended at a Grooming Station to hasten their domestication and increase their production.";
			}

			public class REC_ROOM
			{
				public static LocString NAME = "Recreation Room";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Scheduled Downtime will further improve Morale for Duplicants visiting a Recreation Room.";
			}

			public class PARK
			{
				public static LocString NAME = "Park";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Seeing natural spaces regularly thoughout the day will improve the Morale of Duplicants.";
			}

			public class NATURERESERVE
			{
				public static LocString NAME = "Nature Reserve";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Seeing natural spaces regularly thoughout the day will improve the Morale of Duplicants.";
			}

			public class PRIVATE_BEDROOM
			{
				public static LocString NAME = "Private Bedroom";

				public static LocString EFFECT = "- Stamina recovery bonus";

				public static LocString TOOLTIP = "Duplicants recover even more stamina while sleeping in a Private Bedroom than in Barracks.";
			}

			public class PRIVATE_BATHROOM
			{
				public static LocString NAME = "Private Bathroom";

				public static LocString EFFECT = "- Stress relief bonus";

				public static LocString TOOLTIP = "Duplicants relieve even more stress when using the toilet in a Private Bathroom than in a Latrine.";
			}

			public static LocString CONFLICTED = "Conflicted Room";
		}

		public class CRITERIA
		{
			public class CRITERIA_FAILED
			{
				public static LocString MISSING_BUILDING = "Missing {0}";

				public static LocString FAILED = "{0}";
			}

			public class CEILING_HEIGHT
			{
				public static LocString NAME = "Minimum height: {0} tiles";

				public static LocString DESCRIPTION = "Must have a ceiling height of at least {0} tiles";
			}

			public class MINIMUM_SIZE
			{
				public static LocString NAME = "Minimum size: {0} tiles";

				public static LocString DESCRIPTION = "Must have an area of at least {0} tiles";
			}

			public class MAXIMUM_SIZE
			{
				public static LocString NAME = "Maximum size: {0} tiles";

				public static LocString DESCRIPTION = "Must have an area no larger than {0} tiles";
			}

			public class BED_SINGLE
			{
				public static LocString NAME = "Single bed";

				public static LocString DESCRIPTION = "Requires one Cot or Comfy Bed";
			}

			public class LUXURY_BED_SINGLE
			{
				public static LocString NAME = "Single Comfy Bed";

				public static LocString DESCRIPTION = "Requires a Comfy Bed";
			}

			public class NO_COTS
			{
				public static LocString NAME = "No Cots";

				public static LocString DESCRIPTION = "Room cannot contain a Cot";
			}

			public class BED_MULTIPLE
			{
				public static LocString NAME = "Beds";

				public static LocString DESCRIPTION = "Requires two or more Cots or Comfy Beds";
			}

			public class BUILDING_DECOR_POSITIVE
			{
				public static LocString NAME = "Positive decor";

				public static LocString DESCRIPTION = "Requires at least one building with positive decor";
			}

			public class DECORATIVE_ITEM
			{
				public static LocString NAME = "Decor item";

				public static LocString DESCRIPTION = "Requires one or more Paintings, Sculptures, or Vases";
			}

			public class DECORATIVE_ITEM_N
			{
				public static LocString NAME = "Decor item: {0}+ Decor";

				public static LocString DESCRIPTION = "Requires a decorative item with a minimum Decor value of {0}";
			}

			public class CLINIC
			{
				public static LocString NAME = "Medical equipment";

				public static LocString DESCRIPTION = "Requires one or more Sick Bays or Disease Clinics";
			}

			public class POWER_STATION
			{
				public static LocString NAME = "Power Control Station";

				public static LocString DESCRIPTION = "Requires a single Power Control Station";
			}

			public class FARM_STATION
			{
				public static LocString NAME = "Farm Station";

				public static LocString DESCRIPTION = "Requires a single Farm Station";
			}

			public class CREATURE_RELOCATOR
			{
				public static LocString NAME = "Critter Relocator";

				public static LocString DESCRIPTION = "Requires a single Critter Drop-Off";
			}

			public class CREATURE_FEEDER
			{
				public static LocString NAME = "Critter Feeder";

				public static LocString DESCRIPTION = "Requires a single Critter Feeder";
			}

			public class RANCH_STATION
			{
				public static LocString NAME = "Grooming Station";

				public static LocString DESCRIPTION = "Requires a single Grooming Station";
			}

			public class REC_BUILDING
			{
				public static LocString NAME = "Recreational building";

				public static LocString DESCRIPTION = "Requires one or more recreational buildings";
			}

			public class PARK_BUILDING
			{
				public static LocString NAME = "Park building";

				public static LocString DESCRIPTION = "Requires one or more park buildings";
			}

			public class MACHINE_SHOP
			{
				public static LocString NAME = "Mechanics Station";

				public static LocString DESCRIPTION = "Requires requires one or more Mechanics Stations";
			}

			public class FOOD_BOX
			{
				public static LocString NAME = "Food storage";

				public static LocString DESCRIPTION = "Requires one or more Ration Boxes or Refrigerators";
			}

			public class LIGHT
			{
				public static LocString NAME = "Light source";

				public static LocString DESCRIPTION = "Requires one or more light sources";
			}

			public class MASSAGE_TABLE
			{
				public static LocString NAME = "Massage Table";

				public static LocString DESCRIPTION = "Requires one or more Massage Tables";
			}

			public class MESS_STATION_SINGLE
			{
				public static LocString NAME = "Mess Table";

				public static LocString DESCRIPTION = "Requires a single Mess Table";
			}

			public class MESS_STATION_MULTIPLE
			{
				public static LocString NAME = "Mess Tables";

				public static LocString DESCRIPTION = "Requires two or more Mess Tables";
			}

			public class RESEARCH_STATION
			{
				public static LocString NAME = "Research station";

				public static LocString DESCRIPTION = "Requires one or more Research Stations or Super Computers";
			}

			public class TOILET
			{
				public static LocString NAME = "Toilet";

				public static LocString DESCRIPTION = "Requires one or more Outhouses or Lavatories";
			}

			public class FLUSH_TOILET
			{
				public static LocString NAME = "Flush Toilet";

				public static LocString DESCRIPTION = "Requires one or more Lavatories";
			}

			public class NO_OUTHOUSES
			{
				public static LocString NAME = "No Outhouses";

				public static LocString DESCRIPTION = "Cannot contain basic Outhouses";
			}

			public class WASH_STATION
			{
				public static LocString NAME = "Wash station";

				public static LocString DESCRIPTION = "Requires one or more Wash Basins, Sinks, Hand Sanitizers, or Showers";
			}

			public class ADVANCED_WASH_STATION
			{
				public static LocString NAME = "Plumbed wash station";

				public static LocString DESCRIPTION = "Requires one or more Sinks, Hand Sanitizers, or Showers";
			}

			public class NO_INDUSTRIAL_MACHINERY
			{
				public static LocString NAME = "No industrial machinery";

				public static LocString DESCRIPTION = "Cannot contain any building labeled Industrial Machinery";
			}

			public class WILDANIMAL
			{
				public static LocString NAME = "Wildlife";

				public static LocString DESCRIPTION = "At least one wild creature.";
			}

			public class WILDANIMALS
			{
				public static LocString NAME = "More Wildlife";

				public static LocString DESCRIPTION = "At least two wild creatures.";
			}

			public class WILDPLANT
			{
				public static LocString NAME = "At least two Wild Plants";

				public static LocString DESCRIPTION = "At least two wild plants.";
			}

			public class WILDPLANTS
			{
				public static LocString NAME = "At least four Wild Plants";

				public static LocString DESCRIPTION = "At least two wild plants.";
			}

			public static LocString HEADER = "<b>Requirements:</b>";

			public static LocString NEUTRAL_TYPE = "Enclosed by wall tile";

			public static LocString POSSIBLE_TYPES_HEADER = "Possible Room Types";

			public static LocString NO_TYPE_CONFLICTS = "Remove conflicting buildings";
		}

		public class DETAILS
		{
			public class ASSIGNED_TO
			{
				public static LocString NAME = "<b>Assignments:</b>\n{0}";

				public static LocString UNASSIGNED = "Unassigned";
			}

			public class AVERAGE_TEMPERATURE
			{
				public static LocString NAME = "Average temperature: {0}";
			}

			public class AVERAGE_ATMO_MASS
			{
				public static LocString NAME = "Average air pressure: {0}";
			}

			public class SIZE
			{
				public static LocString NAME = "Room size: {0} Tiles";
			}

			public class BUILDING_COUNT
			{
				public static LocString NAME = "Buildings: {0}";
			}

			public class CREATURE_COUNT
			{
				public static LocString NAME = "Critters: {0}";
			}

			public class PLANT_COUNT
			{
				public static LocString NAME = "Plants: {0}";
			}

			public static LocString HEADER = "Room Details";
		}

		public class EFFECTS
		{
			public static LocString HEADER = "<b>Effects:</b>";
		}
	}
}
