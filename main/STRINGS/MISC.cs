namespace STRINGS
{
	public class MISC
	{
		public class TAGS
		{
			public static LocString OTHER = "Miscellaneous";

			public static LocString FILTER = "Filtration Medium";

			public static LocString ICEORE = "Ice";

			public static LocString PHOSPHORUS = "Phosphorus";

			public static LocString BUILDABLERAW = "Raw Mineral";

			public static LocString BUILDABLEPROCESSED = "Refined Mineral";

			public static LocString BUILDABLEANY = "Generic Buildable";

			public static LocString REFINEDMETAL = "Refined Metal";

			public static LocString METAL = "Metal Ore";

			public static LocString PRECIOUSMETAL = "Precious Metal";

			public static LocString RAWPRECIOUSMETAL = "Precious Metal Ore";

			public static LocString PRECIOUSROCK = "Precious Rock";

			public static LocString ALLOY = "Alloy";

			public static LocString BUILDINGFIBER = "Fiber";

			public static LocString CRUSHABLE = "Crushable";

			public static LocString BAGABLECREATURE = "Critter";

			public static LocString SWIMMINGCREATURE = "Aquatic Critter";

			public static LocString LIFE = "Life";

			public static LocString LIQUIFIABLE = "Liquefiable";

			public static LocString LIQUID = "Liquid";

			public static LocString SPECIAL = "Special";

			public static LocString FARMABLE = "Cultivable Soil";

			public static LocString AGRICULTURE = "Agriculture";

			public static LocString COAL = "Coal";

			public static LocString BLEACHSTONE = "Bleach Stone";

			public static LocString ORGANICS = "Organic";

			public static LocString CONSUMABLEORE = "Consumable Ore";

			public static LocString ORE = "Ore";

			public static LocString BREATHABLE = "Breathable Gas";

			public static LocString UNBREATHABLE = "Unbreathable Gas";

			public static LocString GAS = "Gas";

			public static LocString BURNS = "Flammable";

			public static LocString UNSTABLE = "Unstable";

			public static LocString TOXIC = "Toxic";

			public static LocString MIXTURE = "Mixture";

			public static LocString SOLID = "Solid";

			public static LocString INDUSTRIALPRODUCT = "Industrial Product";

			public static LocString INDUSTRIALINGREDIENT = "Industrial Ingredient";

			public static LocString CLOTHES = "Clothing";

			public static LocString EMITSLIGHT = "Light Emitter";

			public static LocString BED = "Bed";

			public static LocString MESSSTATION = "Dining Table";

			public static LocString SUIT = "Suit";

			public static LocString MULTITOOL = "Multitool";

			public static LocString CLINIC = "Clinic";

			public static LocString RELAXATION_POINT = "Leisure Area";

			public static LocString SOLIDMATERIAL = "Solid Material";

			public static LocString EXTRUDABLE = "Extrudable";

			public static LocString PLUMBABLE = "Plumbable";

			public static LocString COMPOSTABLE = "Compostable";

			public static LocString COMPOSTBASICPLANTFOOD = "Compost Muckroot";

			public static LocString EDIBLE = "Edible";

			public static LocString COOKINGINGREDIENT = "Cooking Ingredient";

			public static LocString MEDICINE = "Medicine";

			public static LocString SEED = "Seed";

			public static LocString ANYWATER = "Water Based";

			public static LocString MARKEDFORCOMPOST = "Marked For Compost";

			public static LocString MARKEDFORCOMPOSTINSTORAGE = "In Compost Storage";

			public static LocString COMPOSTMEAT = "Compost Meat";

			public static LocString PICKLED = "Pickled";

			public static LocString PLASTIC = "Plastic";

			public static LocString TOILET = "Toilet";

			public static LocString MASSAGE_TABLE = "Massage Table";

			public static LocString POWERSTATION = "Power Station";

			public static LocString FARMSTATION = "Farm Station";

			public static LocString MACHINE_SHOP = "Machine Shop";

			public static LocString ANTISEPTIC = "Antiseptic";

			public static LocString OIL = "Hydrocarbon";

			public static LocString DECORATION = "Decoration";

			public static LocString EGG = "Critter Egg";

			public static LocString EGGSHELL = "Egg Shell";

			public static LocString MANUFACTUREDMATERIAL = "Manufactured Material";

			public static LocString STEEL = "Steel";

			public static LocString RAW = "Raw Animal Product";

			public static LocString ANY = "Any";

			public static LocString TRANSPARENT = "Transparent";

			public static LocString RAREMATERIALS = "Rare Resource";

			public static LocString FARMINGMATERIAL = "Fertilizer";

			public static LocString COMMAND_MODULE = "Command Module";

			public static LocString GENE_SHUFFLER = "Neural Vacillator";

			public static LocString FARMING = "Farm Build-Delivery";

			public static LocString RESEARCH = "Research Delivery";

			public static LocString POWER = "Generator Delivery";

			public static LocString BUILDING = "Build Dig-Delivery";

			public static LocString COOKING = "Cook Delivery";

			public static LocString FABRICATING = "Fabricate Delivery";

			public static LocString WIRING = "Wire Build-Delivery";

			public static LocString ART = "Art Build-Delivery";

			public static LocString DOCTORING = "Care Delivery";

			public static LocString CONVEYOR = "Shipping Build";

			public static LocString COMPOST_FORMAT = "{Item}";
		}

		public class STATUSITEMS
		{
			public class OXYROCK
			{
				public class NEIGHBORSBLOCKED
				{
					public static LocString NAME = "Oxylite blocked";

					public static LocString TOOLTIP = "This " + UI.FormatAsLink("Oxylite", "OXYROCK") + " deposit is not exposed to air and cannot emit " + UI.FormatAsLink("Oxygen", "OXYGEN");
				}

				public class OVERPRESSURE
				{
					public static LocString NAME = "Inert";

					public static LocString TOOLTIP = "Environmental air pressure is too high for this " + UI.FormatAsLink("Oxylite", "OXYROCK") + " deposit to emit " + UI.FormatAsLink("Oxygen", "OXYGEN");
				}
			}

			public class OXYROCKBLOCKED
			{
				public static LocString NAME = "{BlockedString}";

				public static LocString TOOLTIP = "This " + UI.FormatAsLink("Oxylite", "OXYROCK") + " deposit has no room to emit " + UI.FormatAsLink("Oxygen", "OXYGEN");
			}

			public class OXYROCKEMITTING
			{
				public static LocString NAME = BUILDING.STATUSITEMS.EMITTINGOXYGENAVG.NAME;

				public static LocString TOOLTIP = BUILDING.STATUSITEMS.EMITTINGOXYGENAVG.TOOLTIP;
			}

			public class SPACE
			{
				public static LocString NAME = "Space exposure";

				public static LocString TOOLTIP = "This region is exposed to the vacuum of space and will result in the loss of gas and liquid resources";
			}

			public class OXYROCKINACTIVE
			{
				public static LocString NAME = "Inert";

				public static LocString TOOLTIP = "Environmental air pressure is too high for this " + UI.FormatAsLink("Oxylite", "OXYROCK") + " deposit to emit " + ELEMENTS.OXYGEN.NAME;
			}

			public class BLEACHSTONE
			{
				public class NEIGHBORSBLOCKED
				{
					public static LocString NAME = "Bleachstone blocked";

					public static LocString TOOLTIP = "This " + UI.FormatAsLink("Bleachstone", "BLEACHSTONE") + " deposit is not exposed to air and cannot emit " + UI.FormatAsLink("Chlorine", "CHLORINE");
				}

				public class OVERPRESSURE
				{
					public static LocString NAME = "Inert";

					public static LocString TOOLTIP = "Environmental air pressure is too high for this " + UI.FormatAsLink("Bleachstone", "BLEACHSTONE") + " deposit to emit " + UI.FormatAsLink("Chlorine", "CHLORINE");
				}
			}

			public class BLEACHSTONEBLOCKED
			{
				public static LocString NAME = "{BlockedString}";

				public static LocString TOOLTIP = "This " + UI.FormatAsLink("Bleachstone", "BLEACHSTONE") + " deposit has no room to emit " + UI.FormatAsLink("Chlorine", "CHLORINE");
			}

			public class BLEACHSTONEEMITTING
			{
				public static LocString NAME = BUILDING.STATUSITEMS.EMITTINGGASAVG.NAME;

				public static LocString TOOLTIP = BUILDING.STATUSITEMS.EMITTINGGASAVG.TOOLTIP;
			}

			public class BLEACHSTONEINACTIVE
			{
				public static LocString NAME = "Inert";

				public static LocString TOOLTIP = "Environmental air pressure is too high for this " + UI.FormatAsLink("Bleachstone", "BLEACHSTONE") + " deposit to emit " + UI.FormatAsLink("Chlorine", "CHLORINE");
			}

			public class EDIBLE
			{
				public static LocString NAME = "Rations: {0}";

				public static LocString TOOLTIP = "Can provide " + UI.FormatAsLink("{0}", "KCAL") + " of energy to Duplicants";
			}

			public class MARKEDFORDISINFECTION
			{
				public static LocString NAME = "Disinfect Errand";

				public static LocString TOOLTIP = "Building will be disinfected once a Duplicant is available";
			}

			public class PENDINGCLEAR
			{
				public static LocString NAME = "Sweep Errand";

				public static LocString TOOLTIP = "Debris will be swept once a Duplicant is available";
			}

			public class MARKEDFORCOMPOST
			{
				public static LocString NAME = "Compost Errand";

				public static LocString TOOLTIP = "Object is marked and will be moved to compost once a Duplicant is available";
			}

			public class NOCLEARLOCATIONSAVAILABLE
			{
				public static LocString NAME = "No Sweep Destination";

				public static LocString TOOLTIP = "There are no valid destinations for this object to be swept to";
			}

			public class PENDINGHARVEST
			{
				public static LocString NAME = "Harvest Errand";

				public static LocString TOOLTIP = "Plant will be harvested once a Duplicant is available";
			}

			public class PENDINGUPROOT
			{
				public static LocString NAME = "Uproot Errand";

				public static LocString TOOLTIP = "Plant will be uprooted once a Duplicant is available";
			}

			public class WAITINGFORDIG
			{
				public static LocString NAME = "Dig Errand";

				public static LocString TOOLTIP = "Tile will be dug out once a Duplicant is available";
			}

			public class WAITINGFORMOP
			{
				public static LocString NAME = "Mop Errand";

				public static LocString TOOLTIP = "Spill will be mopped once a Duplicant is available";
			}

			public class NOTMARKEDFORHARVEST
			{
				public static LocString NAME = "No Harvest Pending";

				public static LocString TOOLTIP = "Use the Harvest Tool to mark this plant for harvest";
			}

			public class ELEMENTALCATEGORY
			{
				public static LocString NAME = "{Category}";

				public static LocString TOOLTIP = "The selected object belongs to the {Category} resource category";
			}

			public class ELEMENTALMASS
			{
				public static LocString NAME = "{Mass}";

				public static LocString TOOLTIP = "The selected object has a mass of {Mass}";
			}

			public class ELEMENTALDISEASE
			{
				public static LocString NAME = "{Disease}";

				public static LocString TOOLTIP = "Current disease: {Disease}";
			}

			public class ELEMENTALTEMPERATURE
			{
				public static LocString NAME = "{Temp}";

				public static LocString TOOLTIP = "The selected object is currently {Temp}";
			}

			public class MARKEDFORCOMPOSTINSTORAGE
			{
				public static LocString NAME = "Composted";

				public static LocString TOOLTIP = "The selected object is currently in the compost";
			}

			public class BURIEDITEM
			{
				public static LocString NAME = "Buried Object";

				public static LocString TOOLTIP = "Something seems to be hidden here";

				public static LocString NOTIFICATION = "Buried object discovered";

				public static LocString NOTIFICATION_TOOLTIP = "My Duplicants have uncovered a {Uncoverable}!\n\nClick to jump to its location.";
			}

			public class HEALTHSTATUS
			{
				public class PERFECT
				{
					public static LocString NAME = "None";

					public static LocString TOOLTIP = "This Duplicant is in peak condition";
				}

				public class ALRIGHT
				{
					public static LocString NAME = "None";

					public static LocString TOOLTIP = "This Duplicant is none the worse for wear";
				}

				public class SCUFFED
				{
					public static LocString NAME = "Minor";

					public static LocString TOOLTIP = "This Duplicant has a few scrapes and bruises";
				}

				public class INJURED
				{
					public static LocString NAME = "Moderate";

					public static LocString TOOLTIP = "This Duplicant needs some patching up";
				}

				public class CRITICAL
				{
					public static LocString NAME = "Severe";

					public static LocString TOOLTIP = "This Duplicant is in serious need of medical attention";
				}

				public class INCAPACITATED
				{
					public static LocString NAME = "Paralyzing";

					public static LocString TOOLTIP = "This Duplicant will die if they do not receive medical attention";
				}

				public class DEAD
				{
					public static LocString NAME = "Conclusive";

					public static LocString TOOLTIP = "This Duplicant won't be getting back up";
				}
			}

			public class HIT
			{
				public static LocString NAME = "{targetName} took {damageAmount} damage from {attackerName}'s attack!";
			}

			public class OREMASS
			{
				public static LocString NAME = ELEMENTALMASS.NAME;

				public static LocString TOOLTIP = ELEMENTALMASS.TOOLTIP;
			}

			public class ORETEMP
			{
				public static LocString NAME = ELEMENTALTEMPERATURE.NAME;

				public static LocString TOOLTIP = ELEMENTALTEMPERATURE.TOOLTIP;
			}

			public class TREEFILTERABLETAGS
			{
				public static LocString NAME = "{Tags}";

				public static LocString TOOLTIP = "{Tags}";
			}

			public class SPOUTOVERPRESSURE
			{
				public static LocString NAME = "Overpressure {StudiedDetails}";

				public static LocString TOOLTIP = "Spout cannot vent due to high environmental pressure";

				public static LocString STUDIED = "(idle in {Time})";
			}

			public class SPOUTEMITTING
			{
				public static LocString NAME = "Venting {StudiedDetails}";

				public static LocString TOOLTIP = "This geyser is erupting";

				public static LocString STUDIED = "(idle in {Time})";
			}

			public class SPOUTPRESSUREBUILDING
			{
				public static LocString NAME = "Rising pressure {StudiedDetails}";

				public static LocString TOOLTIP = "This geyser's internal pressure is steadily building";

				public static LocString STUDIED = "(erupts in {Time})";
			}

			public class SPOUTIDLE
			{
				public static LocString NAME = "Idle {StudiedDetails}";

				public static LocString TOOLTIP = "This geyser is not currently erupting";

				public static LocString STUDIED = "(erupts in {Time})";
			}

			public class SPOUTDORMANT
			{
				public static LocString NAME = "Dormant";

				public static LocString TOOLTIP = "This geyser's geoactivity has halted" + UI.HORIZONTAL_BR_RULE + "It won't erupt again for some time";
			}

			public class PICKUPABLEUNREACHABLE
			{
				public static LocString NAME = "Unreachable";

				public static LocString TOOLTIP = "Duplicants cannot reach this object";
			}

			public class PRIORITIZED
			{
				public static LocString NAME = "High Priority";

				public static LocString TOOLTIP = "This errand has been mark as important and will be preferred over other pending errands";
			}

			public class USING
			{
				public static LocString NAME = "Using {Target}";

				public static LocString TOOLTIP = "{Target} is currently in use";
			}

			public class ORDERATTACK
			{
				public static LocString NAME = "Pending Attack";

				public static LocString TOOLTIP = "Waiting for a Duplicant to murderize this defenseless critter";
			}

			public class ORDERCAPTURE
			{
				public static LocString NAME = "Pending Wrangle";

				public static LocString TOOLTIP = "Waiting for a Duplicant to capture this critter" + UI.HORIZONTAL_BR_RULE + "Only Duplicants trained as Ranchers can catch critters without traps";
			}

			public class OPERATING
			{
				public static LocString NAME = "In Use";

				public static LocString TOOLTIP = "This object is currently being used";
			}

			public class CLEANING
			{
				public static LocString NAME = "Cleaning";

				public static LocString TOOLTIP = "This building is currently being cleaned";
			}

			public class REGIONISBLOCKED
			{
				public static LocString NAME = "Blocked";

				public static LocString TOOLTIP = "Undug material is blocking off an essential tile";
			}

			public class STUDIED
			{
				public static LocString NAME = "Analysis Complete";

				public static LocString TOOLTIP = "Information on this Natural Feature has been compiled below.";
			}

			public class AWAITINGSTUDY
			{
				public static LocString NAME = "Analysis Pending";

				public static LocString TOOLTIP = "New information on this Natural Feature will be compiled once the field study is complete";
			}
		}

		public class POPFX
		{
			public static LocString RESOURCE_EATEN = "Resource Eaten";
		}

		public class NOTIFICATIONS
		{
			public class BASICCONTROLS
			{
				public static LocString NAME = "Tutorial: Basic Controls";

				public static LocString MESSAGEBODY = "• The <color=#F44A47><b>" + UI.FormatAsLink("[WASD]", "CONTROLS") + "</b></color> keys pan my view and the <color=#F44A47><b>" + UI.FormatAsLink("[MOUSE WHEEL]", "CONTROLS") + "</b></color> zooms it in and out.\n\n• <color=#F44A47><b>[H]</b></color> returns my view to the Printing Pod.\n\n• I can speed or slow my perception of time using the top left corner buttons, or by pressing <color=#F44A47><b>[TAB]</b></color>. <color=#F44A47><b>[SPACE]</b></color> will pause the flow of time entirely.\n\n• I'll keep records of everything I discover in my personal DATABASE <color=#F44A47><b>[U]</b></color> to refer back to if I forget anything important.";

				public static LocString TOOLTIP = "Notes on using my HUD";
			}

			public class WELCOMEMESSAGE
			{
				public static LocString NAME = "Tutorial: Colony Management";

				public static LocString MESSAGEBODY = "I can use the " + UI.FormatAsLink("DIG TOOL", "TOOLS") + " <color=#F44A47><b>[G]</b></color> and the " + UI.FormatAsLink("Build Menu", "MISC") + " in the lower left of the screen to begin planning my first construction tasks.\n\nOnce I've placed a few errands my Duplicants will automatically get to work, without me needing to direct them individually.";

				public static LocString TOOLTIP = "Notes on getting Duplicants to do my bidding";
			}

			public class STRESSMANAGEMENTMESSAGE
			{
				public static LocString NAME = "Tutorial: Stress Management";

				public static LocString MESSAGEBODY = "At 100% " + UI.FormatAsLink("Stress", "STRESS") + ", a Duplicant will have a nervous breakdown and be unable to work.\n\nBreakdowns can manifest in different colony-threatening ways, such as the destruction of buildings or the binge eating of food.\n\nI can select a Duplicant and mouse over " + UI.FormatAsLink("Stress", "STRESS") + " in their STATUS TAB to view their individual " + UI.FormatAsLink("Stress Factors", "STRESS") + ", and hopefully minimize them before they become a problem.";

				public static LocString TOOLTIP = "Notes on keeping Duplicants happy and productive";
			}

			public class TASKPRIORITIESMESSAGE
			{
				public static LocString NAME = "Tutorial: Priority";

				public static LocString MESSAGEBODY = "Duplicants always perform errands in order of highest to lowest priority. They will harvest " + UI.FormatAsLink("Food", "FOOD") + " before they build, for example, or always build new structures before they mine materials.\n\nI can open the " + UI.FormatAsLink("PRIORITIES SCREEN", "PRIORITIES") + " <color=#F44A47><b>[L]</b></color> to set which Errand Types Duplicants may or may not perform, or to specialize skilled Duplicants for particular Errand Types.";

				public static LocString TOOLTIP = "Notes on managing Duplicants' errands";
			}

			public class MOPPINGMESSAGE
			{
				public static LocString NAME = "Tutorial: Polluted Water";

				public static LocString MESSAGEBODY = UI.FormatAsLink("Polluted Water", "DIRTYWATER") + " slowly emits " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " which accelerates the spread of " + UI.FormatAsLink("Disease", "DISEASE") + ".\n\nDuplicants will also be " + UI.FormatAsLink("Stressed", "STRESS") + " by walking through Polluted Water, so I should have my Duplicants clean up spills by clicking and dragging the " + UI.FormatAsLink("MOP TOOL", "MISC") + " <color=#F44A47><b>[M]</b></color>.";

				public static LocString TOOLTIP = "Notes on handling polluted materials";
			}

			public class LOCOMOTIONMESSAGE
			{
				public static LocString NAME = "Tutorial: Duplicant Movement";

				public static LocString MESSAGEBODY = "Duplicants can only climb two tiles high and cannot fit into spaces smaller that two tiles, which I should keep in mind while placing errands.\n\nTo check if an errand I've placed is accessible, I can select a Duplicant and click <color=#F44A47><b>SHOW NAVIGATION</b></color> to view all areas within their reach.";

				public static LocString TOOLTIP = "Notes on my Duplicants' maneuverability";
			}

			public class PRIORITIESMESSAGE
			{
				public static LocString NAME = "Tutorial: Errand Priorities";

				public static LocString MESSAGEBODY = "Duplicants will choose where to work based on the priority of the errands that I give them. I can open the " + UI.FormatAsLink("PRIORITIES SCREEN", "PRIORITIES") + " <color=#F44A47><b>[L]</b></color> to set their Errand Type priorities, and the " + UI.FormatAsLink("PRIORITY TOOL", "PRIORITIES") + " <color=#F44A47><b>[P]</b></color> to fine tune Specific Errand Priority. Many buildings will also let me change their Priority level when I select them.";

				public static LocString TOOLTIP = "Notes on my Duplicants' priorities";
			}

			public class FETCHINGWATERMESSAGE
			{
				public static LocString NAME = "Tutorial: Fetching Water";

				public static LocString MESSAGEBODY = "By building a " + UI.FormatAsLink("Pitcher Pump", "LIQUIDPUMPINGSTATION") + " from the " + UI.FormatAsLink("PLUMBING TAB", "MISC") + " <color=#F44A47><b>[5]</b></color> over a pool of liquid, my Duplicants will be able to bottle it up and manually deliver it wherever it needs to go.";

				public static LocString TOOLTIP = "Notes on liquid resource gathering";
			}

			public class SCHEDULEMESSAGE
			{
				public static LocString NAME = "Tutorial: Scheduling";

				public static LocString MESSAGEBODY = "My Duplicants will only eat, sleep, work, or bathe during the times I allot for such activities.\n\nTo make the best use of their time, I can open the SCHEDULE TAB <color=#F44A47><b>[U]</b></color> to adjust the colony's schedule and plan how they should utilize their day.";

				public static LocString TOOLTIP = "Notes on scheduling my Duplicants' time";
			}

			public class THERMALCOMFORT
			{
				public static LocString NAME = "Tutorial: Duplicant Temperature";

				public static LocString TOOLTIP = "Notes on helping Duplicants keep their cool";

				public static LocString MESSAGEBODY = "Environments that are extremely " + UI.FormatAsLink("Hot", "HEAT") + " or " + UI.FormatAsLink("Cold", "HEAT") + " affect my Duplicants' internal body temperature and cause undue " + UI.FormatAsLink("Stress", "STRESS") + ".\n\nThe THERMAL TOLERANCE OVERLAY <color=#F44A47><b>[F4]</b></color> allows me to view all areas where my Duplicants will feel discomfort and be unable to regulate their internal body temperature.";
			}

			public class TUTORIAL_OVERHEATING
			{
				public static LocString NAME = "Tutorial: Building Temperature";

				public static LocString TOOLTIP = "Notes on preventing meltdowns";

				public static LocString MESSAGEBODY = "When constructing buildings, I should always take note of their " + UI.FormatAsLink("Overheat Temperature", "HEAT") + " and plan their locations accordingly. Maintaining low ambient temperatures and good ventilation in the colony will also help keep building temperatures down.\n\nIf I allow buildings to exceed their Overheat Temperature they will begin to take damage, and if left unattended, they will meltdown be unusable until repaired.";
			}

			public class LOTS_OF_GERMS
			{
				public static LocString NAME = "Tutorial: Germs and Disease";

				public static LocString TOOLTIP = "Notes on Duplicant disease risks";

				public static LocString MESSAGEBODY = UI.FormatAsLink("Germs", "DISEASE") + " such as " + UI.FormatAsLink("Food Poisoning", "FOODPOISONING") + " and " + UI.FormatAsLink("Slimelung", "SLIMELUNG") + " can cause " + UI.FormatAsLink("Disease", "DISEASE") + " in my Duplicants. I can use the " + UI.FormatAsLink("GERM OVERLAY", "MISC") + " <color=#F44A47><b>[F9]</b></color> to view all germ concentrations in my colony and even detect the sources spawning them.\n\nBuilding Wash Basins from the " + UI.FormatAsLink("MEDICINE TAB", "MISC") + " <color=#F44A47><b>[8]</b></color> by colony toilets will tell my Duplicants they need to wash up.\n\nIf I keep my colony free of contaminated materials and encourage good Duplicant hygiene, their natural " + UI.FormatAsLink("Immunity", "IMMUNE SYSTEM") + " should handle the rest.";
			}

			public class BEING_INFECTED
			{
				public static LocString NAME = "Tutorial: Immune Systems";

				public static LocString TOOLTIP = "Notes on keeping Duplicants in peak health";

				public static LocString MESSAGEBODY = "When Duplicants come into contact with various " + UI.FormatAsLink("Germs", "DISEASE") + ", they'll need to expend points of " + UI.FormatAsLink("Immunity", "IMMUNE SYSTEM") + " to resist them and remain healthy. If repeated exposes causes their Immunity to drop to 0%, they'll be unable to resist germs and will contract the next disease they encounter.\n\nDoors with Access Permissions can be built from the BASE TAB<color=#F44A47> <b>[1]</b></color> of the " + UI.FormatAsLink("Build menu", "misc") + " to block Duplicants from entering biohazardous areas while they recover their spent immunity points.";
			}

			public class DISEASE_COOKING
			{
				public static LocString NAME = "Tutorial: Food Safety";

				public static LocString TOOLTIP = "Notes on managing food contamination";

				public static LocString MESSAGEBODY = "The " + UI.FormatAsLink("Food", "FOOD") + " my Duplicants cook will only ever be as clean as the ingredients used to make it. Storing food in sterile or " + UI.FormatAsLink("Refrigerated", "REFRIGERATOR") + " environments will keep food free of " + UI.FormatAsLink("Germs", "DISEASE") + ", while carefully placed hygiene stations like " + BUILDINGS.PREFABS.WASHBASIN.NAME + " or " + BUILDINGS.PREFABS.SHOWER.NAME + " will prevent the cooks from infecting the food by handling it.\n\nDangerously contaminated food can be sent to compost by clicking the " + UI.FormatAsLink("Compost", "misc") + " button on the selected item.";
			}

			public class SUITS
			{
				public static LocString NAME = "Tutorial: Atmo Suits";

				public static LocString TOOLTIP = "Notes on using atmo suits";

				public static LocString MESSAGEBODY = UI.FormatAsLink("Atmo Suits", "ATMO_SUIT") + " can be equipped to protect my Duplicants from environmental hazards like extreme " + UI.FormatAsLink("Heat", "Heat") + ", airborne " + UI.FormatAsLink("Germs", "DISEASE") + ", or unbreathable " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ". In order to utilize these suits, I'll need to hook up an Atmo Suit Dock to an Atmo Suit Checkpoint, then store one of the suits inside.\n\nDuplicants will equip a suit when they walk past the checkpoint in the chosen direction, and will unequip their suit when walking back the opposite way.";
			}

			public class MORALE
			{
				public static LocString NAME = "Tutorial: Morale";

				public static LocString TOOLTIP = "Notes on Duplicant expectations";

				public static LocString MESSAGEBODY = "The Foods, Rooms, Decor, and Recreation a Duplicant experiences will have an effect on their Morale. Good experiences improve their Morale and poor experiences drop it. When Morale is below their Expectations, Duplicants will become Stressed.\n\nAs Duplicants are assigned to Jobs, they gain increased Expectations, and so the colony will have to be improved to keep up their Morale. An overview of Morale and Stress can be viewed on the Vitals screen.";
			}

			public class DTU
			{
				public static LocString NAME = "Tutorial: Duplicant Thermal Units";

				public static LocString TOOLTIP = "Notes on measuring heat energy";

				public static LocString MESSAGEBODY = "My Duplicants measure heat energy in Duplicant Thermal Units or DTU.\n\n1 DTU = 1055.06 J";
			}

			public class NOMESSAGES
			{
				public static LocString NAME = "";

				public static LocString TOOLTIP = "";
			}

			public class NOALERTS
			{
				public static LocString NAME = "";

				public static LocString TOOLTIP = "";
			}

			public class NEWTRAIT
			{
				public static LocString NAME = "{0} has developed a trait";

				public static LocString TOOLTIP = "{0} has developed the trait(s):\n    • {1}";
			}

			public class RESEARCHCOMPLETE
			{
				public static LocString NAME = "Research Complete";

				public static LocString MESSAGEBODY = "Eureka! My Duplicants have discovered {0} Technology.\n\nNew buildings have become available:\n  • {1}";

				public static LocString TOOLTIP = "{0} research complete!";
			}

			public class ROLEMASTERED
			{
				public static LocString NAME = "Jobs Mastered";

				public static LocString MESSAGEBODY = "These Duplicants have mastered their jobs and may be eligible for promotion:\n{0}";

				public static LocString LINE = "\n• <b>{0}</b> mastered the {1} job";

				public static LocString TOOLTIP = "Job Mastered";
			}

			public class DUPLICANTABSORBED
			{
				public static LocString NAME = "New Duplicants have been reabsorbed";

				public static LocString MESSAGEBODY = "New Duplicants are no longer available for printing.\nCountdown to the next production was rebooted.";

				public static LocString TOOLTIP = "The printable Duplicants have been reabsorbed";
			}

			public class DUPLICANTDIED
			{
				public static LocString NAME = "Duplicants have died";

				public static LocString TOOLTIP = "These Duplicants have died:";
			}

			public class FOODROT
			{
				public static LocString NAME = "Food has decayed";

				public static LocString TOOLTIP = "These " + UI.FormatAsLink("Food", "FOOD") + " items have rotted and are no longer edible:\n• {0}";
			}

			public class FOODSTALE
			{
				public static LocString NAME = "Food has become stale";

				public static LocString TOOLTIP = "These " + UI.FormatAsLink("Food", "FOOD") + " items have become stale and could rot if not stored:";
			}

			public class REDALERT
			{
				public static LocString NAME = "Red Alert";

				public static LocString TOOLTIP = "The colony is prioritizing work over their individual well-being";
			}

			public class HEALING
			{
				public static LocString NAME = "Healing";

				public static LocString TOOLTIP = "This Duplicant is recovering from an injury";
			}

			public class UNREACHABLEITEM
			{
				public static LocString NAME = "Unreachable resources";

				public static LocString TOOLTIP = "Duplicants cannot retrieve these resources:";
			}

			public class INVALIDCONSTRUCTIONLOCATION
			{
				public static LocString NAME = "Invalid construction location";

				public static LocString TOOLTIP = "These buildings cannot be constructed in the planned areas:\n";
			}

			public class MISSINGMATERIALS
			{
				public static LocString NAME = "Missing materials";

				public static LocString TOOLTIP = "These resources are not available:";
			}

			public class BUILDINGOVERHEATED
			{
				public static LocString NAME = "Damage: Overheated";

				public static LocString TOOLTIP = "Extreme heat is damaging these buildings:\n";
			}

			public class NO_OXYGEN_GENERATOR
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Oxygen Diffuser", "MINERALDEOXIDIZER") + " built";

				public static LocString TOOLTIP = "My colony is not producing any new " + UI.FormatAsLink("Oxygen", "OXYGEN") + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Oxygen Diffusers", "MINERALDEOXIDIZER") + " can be built from the " + UI.FormatAsLink("OXYGEN TAB", "MISC") + " <color=#F44A47><b>[2]</b></color> of the Build Menu";
			}

			public class INSUFFICIENTOXYGENLASTCYCLE
			{
				public static LocString NAME = "Insufficient Oxygen generation";

				public static LocString TOOLTIP = "My colony consumed more " + UI.FormatAsLink("Oxygen", "OXYGEN") + " last cycle than it produced, and will exhaust its air supply without increased generation\n\nI should check my existing oxygen production buildings to ensure they're operating correctly" + UI.HORIZONTAL_BR_RULE + "• " + UI.FormatAsLink("Oxygen", "OXYGEN") + " produced last cycle: {EmittingRate}\n• Consumed last cycle: {ConsumptionRate}";
			}

			public class UNREFRIGERATEDFOOD
			{
				public static LocString NAME = "Unrefrigerated Food";

				public static LocString TOOLTIP = "These " + UI.FormatAsLink("Food", "FOOD") + " items are in storage but are not refrigerated:\n";
			}

			public class FOODLOW
			{
				public static LocString NAME = "Food shortage";

				public static LocString TOOLTIP = "The colony's " + UI.FormatAsLink("Food", "FOOD") + " reserves are low:" + UI.HORIZONTAL_BR_RULE + "    • {0} are currently available\n    • {1} is being consumed per cycle\n\n" + UI.FormatAsLink("Microbe Mushers", "MICROBEMUSHER") + " can be built from the " + UI.FormatAsLink("FOOD TAB", "MISC") + " <color=#F44A47><b>[4]</b></color>";
			}

			public class NO_MEDICAL_COTS
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Med-Bed", "MEDICALCOT") + " built";

				public static LocString TOOLTIP = "There is nowhere for sick Duplicants receive medical care" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Med-Beds", "MEDICALCOT") + " can be built from the " + UI.FormatAsLink("MEDICINE TAB", "MISC") + " <color=#F44A47><b>[8]</b></color>";
			}

			public class NEEDTOILET
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + " built";

				public static LocString TOOLTIP = "My Duplicants have nowhere to relieve themselves" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Outhouses", "OUTHOUSE") + " can be built from the " + UI.FormatAsLink("PLUMBING TAB", "MISC") + " <color=#F44A47><b>[5]</b></color>";
			}

			public class NEEDFOOD
			{
				public static LocString NAME = "Colony requires a food source";

				public static LocString TOOLTIP = "The colony will exhaust their supplies without a new " + UI.FormatAsLink("Food", "FOOD") + " source" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Microbe Mushers", "MICROBEMUSHER") + " can be built from the " + UI.FormatAsLink("FOOD TAB", "MISC") + " <color=#F44A47><b>[4]</b></color>";
			}

			public class HYGENE_NEEDED
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Wash Basin", "WASHBASIN") + " built";

				public static LocString TOOLTIP = UI.FormatAsLink("Germs", "DISEASE") + " are spreading in the colony because my Duplicants have nowhere to clean up" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Wash Basins", "WASHBASIN") + " can be built from the " + UI.FormatAsLink("MEDICINE TAB", "MISC") + " <color=#F44A47><b>[8]</b></color>";
			}

			public class NEEDSLEEP
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Cots", "COT") + " built";

				public static LocString TOOLTIP = "My Duplicants would appreciate a place to sleep" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Cots", "COTS") + " can be built from the " + UI.FormatAsLink("FURNITURE TAB", "MISC") + " <color=#F44A47><b>[9]</b></color>";
			}

			public class NEEDENERGYSOURCE
			{
				public static LocString NAME = "Colony requires a " + UI.FormatAsLink("Power", "POWER") + " source";

				public static LocString TOOLTIP = UI.FormatAsLink("Power", "POWER") + " is required to operate electrical buildings" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Manual Generators", "MANUALGENERATOR") + " and " + UI.FormatAsLink("Wire", "WIRE") + " can be built from the " + UI.FormatAsLink("POWER TAB", "MISC") + " <color=#F44A47><b>[3]</b></color>";
			}

			public class RESOURCEMELTED
			{
				public static LocString NAME = "Resources melted";

				public static LocString TOOLTIP = "These resources have melted:";
			}

			public class VENTOVERPRESSURE
			{
				public static LocString NAME = "Vent overpressurized";

				public static LocString TOOLTIP = "These pipe systems have exited the ideal pressure range:";
			}

			public class VENTBLOCKED
			{
				public static LocString NAME = "Vent blocked";

				public static LocString TOOLTIP = "Blocked pipes have stopped these systems from functioning:";
			}

			public class OUTPUTBLOCKED
			{
				public static LocString NAME = "Output blocked";

				public static LocString TOOLTIP = "Blocked pipes have stopped these systems from functioning:";
			}

			public class BROKENMACHINE
			{
				public static LocString NAME = "Building broken";

				public static LocString TOOLTIP = "These buildings have taken significant damage and are nonfunctional:";
			}

			public class STRUCTURALDAMAGE
			{
				public static LocString NAME = "Structural damage";

				public static LocString TOOLTIP = "These buildings' structural integrity has been compromised";
			}

			public class STRUCTURALCOLLAPSE
			{
				public static LocString NAME = "Structural collapse";

				public static LocString TOOLTIP = "These buildings have collapsed:";
			}

			public class GASCLOUDWARNING
			{
				public static LocString NAME = "A gas cloud approaches";

				public static LocString TOOLTIP = "A toxic gas cloud will soon envelop the colony";
			}

			public class GASCLOUDARRIVING
			{
				public static LocString NAME = "The colony is entering a cloud of gas";

				public static LocString TOOLTIP = "";
			}

			public class GASCLOUDPEAK
			{
				public static LocString NAME = "The gas cloud is at its densest point";

				public static LocString TOOLTIP = "";
			}

			public class GASCLOUDDEPARTING
			{
				public static LocString NAME = "The gas cloud is receding";

				public static LocString TOOLTIP = "";
			}

			public class GASCLOUDGONE
			{
				public static LocString NAME = "The colony is once again in open space";

				public static LocString TOOLTIP = "";
			}

			public class AVAILABLE
			{
				public static LocString NAME = "Resource available";

				public static LocString TOOLTIP = "These resources have become available:";
			}

			public class ALLOCATED
			{
				public static LocString NAME = "Resource allocated";

				public static LocString TOOLTIP = "These resources are reserved for a planned building:";
			}

			public class INCREASEDEXPECTATIONS
			{
				public static LocString NAME = "Duplicants' expectations increased";

				public static LocString TOOLTIP = "Duplicants require better amenities over time.\nThese Duplicants have increased their expectations:";
			}

			public class NEARLYDRY
			{
				public static LocString NAME = "Nearly dry";

				public static LocString TOOLTIP = "These Duplicants will dry off soon:";
			}

			public class IMMIGRANTSLEFT
			{
				public static LocString NAME = "New Duplicants have been reabsorbed";

				public static LocString TOOLTIP = "The printable Duplicants have been Oozed";
			}

			public class LEVELUP
			{
				public static LocString NAME = "Skill increase";

				public static LocString TOOLTIP = "These Duplicants' skills have improved:";

				public static LocString SUFFIX = " - {0} raised to {1}";
			}

			public class SCHEDULE_CHANGED
			{
				public static LocString NAME = "{0}: {1}!";

				public static LocString TOOLTIP = "Duplicants assigned to '{0}' have started their {1} block.\n\n{2}\n\nOpen the Schedule Screen to change blocks or assignments.";
			}

			public class GENESHUFFLER
			{
				public static LocString NAME = "Genes Shuffled";

				public static LocString TOOLTIP = "These Duplicants had their genetic makeup modified:";

				public static LocString SUFFIX = " has developed {0}";
			}

			public class HEALINGTRAITGAIN
			{
				public static LocString NAME = "New trait";

				public static LocString TOOLTIP = "These Duplicants' injuries weren't set and healed improperly. They developed traits as a result:";

				public static LocString SUFFIX = " has developed {0}";
			}

			public class COLONYLOST
			{
				public static LocString NAME = "Colony Lost";

				public static LocString TOOLTIP = "All Duplicants are dead or incapacitated";
			}

			public class FABRICATOREMPTY
			{
				public static LocString NAME = "Fabricator idle";

				public static LocString TOOLTIP = "These fabricators have no recipes queued:";
			}

			public class SUIT_DROPPED
			{
				public static LocString NAME = "No Docks available";

				public static LocString TOOLTIP = "An exosuit was dropped because there were no empty docks available";
			}

			public class DEATH_SUFFOCATION
			{
				public static LocString NAME = "Duplicants suffocated";

				public static LocString TOOLTIP = "These Duplicants died from a lack of " + ELEMENTS.OXYGEN.NAME + ":";
			}

			public class DEATH_FROZENSOLID
			{
				public static LocString NAME = "Duplicants have frozen";

				public static LocString TOOLTIP = "These Duplicants died from extremely low " + UI.FormatAsLink("Temperatures", "HEAT") + ":";
			}

			public class DEATH_OVERHEATING
			{
				public static LocString NAME = "Duplicants have overheated";

				public static LocString TOOLTIP = "These Duplicants died from extreme " + UI.FormatAsLink("Heat", "HEAT") + ":";
			}

			public class DEATH_STARVATION
			{
				public static LocString NAME = "Duplicants have starved";

				public static LocString TOOLTIP = "These Duplicants died from a lack of " + UI.FormatAsLink("Food", "FOOD") + ":";
			}

			public class DEATH_FELL
			{
				public static LocString NAME = "Duplicants splattered";

				public static LocString TOOLTIP = "These Duplicants fell to their deaths:";
			}

			public class DEATH_CRUSHED
			{
				public static LocString NAME = "Duplicants crushed";

				public static LocString TOOLTIP = "These Duplicants have been crushed:";
			}

			public class DEATH_SUFFOCATEDTANKEMPTY
			{
				public static LocString NAME = "Duplicants have suffocated";

				public static LocString TOOLTIP = "These Duplicants were unable to reach " + UI.FormatAsLink("oxygen", "OXYGEN") + " and died:";
			}

			public class DEATH_SUFFOCATEDAIRTOOHOT
			{
				public static LocString NAME = "Duplicants have suffocated";

				public static LocString TOOLTIP = "These Duplicants have asphyxiated in " + UI.FormatAsLink("heat", "HEAT") + " air:";
			}

			public class DEATH_SUFFOCATEDAIRTOOCOLD
			{
				public static LocString NAME = "Duplicants have suffocated";

				public static LocString TOOLTIP = "These Duplicants have asphyxiated in " + UI.FormatAsLink("cold", "HEAT") + " air:";
			}

			public class DEATH_DROWNED
			{
				public static LocString NAME = "Duplicants have drowned";

				public static LocString TOOLTIP = "These Duplicants have drowned:";
			}

			public class DEATH_ENTOUMBED
			{
				public static LocString NAME = "Duplicants have been entombed";

				public static LocString TOOLTIP = "These Duplicants are trapped and need assistance:";
			}

			public class DEATH_RAPIDDECOMPRESSION
			{
				public static LocString NAME = "Duplicants pressurized";

				public static LocString TOOLTIP = "These Duplicants died in a low pressure environment:";
			}

			public class DEATH_OVERPRESSURE
			{
				public static LocString NAME = "Duplicants pressurized";

				public static LocString TOOLTIP = "These Duplicants died in a high pressure environment:";
			}

			public class DEATH_POISONED
			{
				public static LocString NAME = "Duplicants poisoned";

				public static LocString TOOLTIP = "These Duplicants died as a result of poisoning:";
			}

			public class DEATH_DISEASE
			{
				public static LocString NAME = "Duplicants have succumb to illness";

				public static LocString TOOLTIP = "These Duplicants died from an untreated " + UI.FormatAsLink("Disease", "DISEASE") + ":";
			}

			public class CIRCUIT_OVERLOADED
			{
				public static LocString NAME = "Circuit Overloaded";

				public static LocString TOOLTIP = "These wires melted due to excessive current demands on their circuits";
			}

			public class DISCOVERED_SPACE
			{
				public static LocString NAME = "ALERT - Surface Breach";

				public static LocString TOOLTIP = "Amazing!\n\nMy Duplicants have managed to breach the surface of our rocky prison.\n\nI should be careful; the region is extremely inhospitable and I could easily lose resources to the vacuum of space.";
			}
		}

		public class TUTORIAL
		{
			public static LocString DONT_SHOW_AGAIN = "Don't Show Again";
		}

		public class PLACERS
		{
			public class DIGPLACER
			{
				public static LocString NAME = "Dig";
			}

			public class MOPPLACER
			{
				public static LocString NAME = "Mop";
			}
		}
	}
}
