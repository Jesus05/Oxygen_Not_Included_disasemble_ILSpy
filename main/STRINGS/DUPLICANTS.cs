using TUNING;

namespace STRINGS
{
	public class DUPLICANTS
	{
		public class GENDER
		{
			public class MALE
			{
				public class PLURALS
				{
					public static LocString ONE = "he";

					public static LocString TWO = "his";
				}

				public static LocString NAME = "M";
			}

			public class FEMALE
			{
				public class PLURALS
				{
					public static LocString ONE = "she";

					public static LocString TWO = "her";
				}

				public static LocString NAME = "F";
			}

			public class NB
			{
				public class PLURALS
				{
					public static LocString ONE = "they";

					public static LocString TWO = "their";
				}

				public static LocString NAME = "X";
			}
		}

		public class STATS
		{
			public class SUBJECTS
			{
				public static LocString DUPLICANT = "Duplicant";

				public static LocString CREATURE = "critter";

				public static LocString PLANT = "plant";
			}

			public class BREATH
			{
				public static LocString NAME = "Breath";

				public static LocString TOOLTIP = "A Duplicant with zero remaining " + UI.PRE_KEYWORD + "Breath" + UI.PST_KEYWORD + " will begin suffocating";
			}

			public class STAMINA
			{
				public static LocString NAME = "Stamina";

				public static LocString TOOLTIP = "Duplicants will pass out from fatigue when " + UI.PRE_KEYWORD + "Stamina" + UI.PST_KEYWORD + " reaches zero";
			}

			public class CALORIES
			{
				public static LocString NAME = "Calories";

				public static LocString TOOLTIP = "This {1} can burn {0} before starving";
			}

			public class TEMPERATURE
			{
				public static LocString NAME = "Body Temperature";

				public static LocString TOOLTIP = "A healthy Duplicant's " + UI.PRE_KEYWORD + "Body Temperature" + UI.PST_KEYWORD + " is {1}";
			}

			public class EXTERNALTEMPERATURE
			{
				public static LocString NAME = "External Temperature";

				public static LocString TOOLTIP = "This Duplicant's environment is {0}";
			}

			public class DECOR
			{
				public static LocString NAME = "Decor";

				public static LocString TOOLTIP = "Duplicants become stressed in areas with " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " lower than their expectations\nOpen the <b>Decor Overlay</b> <color=#F44A47><b>[F8]</b></color> to view current " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " values";

				public static LocString TOOLTIP_CURRENT = "\n\nCurrent Environmental Decor: {0}";

				public static LocString TOOLTIP_AVERAGE_TODAY = "\nAverage Decor This Cycle: {0}";

				public static LocString TOOLTIP_AVERAGE_YESTERDAY = "\nAverage Decor Last Cycle: {0}";
			}

			public class STRESS
			{
				public static LocString NAME = "Stress";

				public static LocString TOOLTIP = "Duplicants exhibit their Stress Reactions at one hundred percent " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD;
			}

			public class BLADDER
			{
				public static LocString NAME = "Bladder";

				public static LocString TOOLTIP = "Duplicants make \"messes\" if no toilets are available at one hundred percent " + UI.PRE_KEYWORD + "Bladder" + UI.PST_KEYWORD;
			}

			public class HITPOINTS
			{
				public static LocString NAME = "Health";

				public static LocString TOOLTIP = "Combatants become incapacitated at zero " + UI.PRE_KEYWORD + "Health" + UI.PST_KEYWORD;
			}

			public class SKIN_THICKNESS
			{
				public static LocString NAME = "Skin Thickness";
			}

			public class SKIN_DURABILITY
			{
				public static LocString NAME = "Skin Durability";
			}

			public class DISEASERECOVERYTIME
			{
				public static LocString NAME = "Disease Recovery";
			}

			public class TRUNKHEALTH
			{
				public static LocString NAME = "Trunk Health";

				public static LocString TOOLTIP = "Tree branches will die if they do not have a healthy trunk to grow from";
			}
		}

		public class DEATHS
		{
			public class GENERIC
			{
				public static LocString NAME = "Death";

				public static LocString DESCRIPTION = "{Target} has died.";
			}

			public class FROZEN
			{
				public static LocString NAME = "Frozen";

				public static LocString DESCRIPTION = "{Target} has frozen to death.";
			}

			public class SUFFOCATION
			{
				public static LocString NAME = "Suffocation";

				public static LocString DESCRIPTION = "{Target} has suffocated to death.";
			}

			public class STARVATION
			{
				public static LocString NAME = "Starvation";

				public static LocString DESCRIPTION = "{Target} has starved to death.";
			}

			public class OVERHEATING
			{
				public static LocString NAME = "Overheated";

				public static LocString DESCRIPTION = "{Target} overheated to death.";
			}

			public class DROWNED
			{
				public static LocString NAME = "Drowned";

				public static LocString DESCRIPTION = "{Target} has drowned.";
			}

			public class EXPLOSION
			{
				public static LocString NAME = "Explosion";

				public static LocString DESCRIPTION = "{Target} has died in an explosion.";
			}

			public class COMBAT
			{
				public static LocString NAME = "Slain";

				public static LocString DESCRIPTION = "{Target} succumbed to their wounds after being incapacitated.";
			}

			public class FATALDISEASE
			{
				public static LocString NAME = "Succumbed to Disease";

				public static LocString DESCRIPTION = "{Target} has died of a fatal illness.";
			}
		}

		public class CHORES
		{
			public class THINKING
			{
				public static LocString NAME = "Ponder";

				public static LocString STATUS = "Pondering";

				public static LocString TOOLTIP = "This Duplicant is mulling over what they should do next";
			}

			public class ASTRONAUT
			{
				public static LocString NAME = "Space Mission";

				public static LocString STATUS = "On space mission";

				public static LocString TOOLTIP = "This Duplicant is exploring the vast universe";
			}

			public class DIE
			{
				public static LocString NAME = "Die";

				public static LocString STATUS = "Dying";

				public static LocString TOOLTIP = "Fare thee well, brave soul";
			}

			public class ENTOMBED
			{
				public static LocString NAME = "Entombed";

				public static LocString STATUS = "Entombed";

				public static LocString TOOLTIP = "Entombed Duplicants are at risk of suffocating and must be dug out by others in the colony";
			}

			public class BEINCAPACITATED
			{
				public static LocString NAME = "Incapacitated";

				public static LocString STATUS = "Dying";

				public static LocString TOOLTIP = "This Duplicant will die soon if they do not receive assistance";
			}

			public class GENESHUFFLE
			{
				public static LocString NAME = "Use Neural Vacillator";

				public static LocString STATUS = "Using Neural Vacillator";

				public static LocString TOOLTIP = "This Duplicant is being experimented on!";
			}

			public class DEBUGGOTO
			{
				public static LocString NAME = "DebugGoTo";

				public static LocString STATUS = "DebugGoTo";
			}

			public class DISINFECT
			{
				public static LocString NAME = "Disinfect";

				public static LocString STATUS = "Going to disinfect";

				public static LocString TOOLTIP = "Buildings can be disinfected to remove contagious Germs from their surface";
			}

			public class EQUIPPINGSUIT
			{
				public static LocString NAME = "Equip Exosuit";

				public static LocString STATUS = "Equipping exosuit";

				public static LocString TOOLTIP = "This Duplicant is putting on protective gear";
			}

			public class STRESSIDLE
			{
				public static LocString NAME = "Antsy";

				public static LocString STATUS = "Antsy";

				public static LocString TOOLTIP = "This Duplicant is a workaholic and gets stressed when they have nothing to do";
			}

			public class MOVETO
			{
				public static LocString NAME = "Move to";

				public static LocString STATUS = "Moving to location";

				public static LocString TOOLTIP = "This Duplicant was manually directed to move to a specific location";
			}

			public class DROPUNUSEDINVENTORY
			{
				public static LocString NAME = "Drop Inventory";

				public static LocString STATUS = "Dropping unused inventory";

				public static LocString TOOLTIP = "This Duplicant is dropping carried items they no longer need";
			}

			public class PEE
			{
				public static LocString NAME = "Relieve Self";

				public static LocString STATUS = "Relieving self";

				public static LocString TOOLTIP = "This Duplicant didn't find a toilet in time. Oops";
			}

			public class BREAK_PEE
			{
				public static LocString NAME = "Downtime: Use Toilet";

				public static LocString STATUS = "Downtime: Going to use toilet";

				public static LocString TOOLTIP = "This Duplicant has scheduled Downtime and is using their break to go to the toilet\n\nDuplicants have to use the toilet at least once per day";
			}

			public class STRESSVOMIT
			{
				public static LocString NAME = "Stress Vomit";

				public static LocString STATUS = "Stress vomiting";

				public static LocString TOOLTIP = "Some people deal with stress better than others";
			}

			public class UGLY_CRY
			{
				public static LocString NAME = "Ugly Cry";

				public static LocString STATUS = "Ugly crying";

				public static LocString TOOLTIP = "This Duplicant is having a healthy cry to alleviate their negative emotions";
			}

			public class BINGE_EAT
			{
				public static LocString NAME = "Binge Eat";

				public static LocString STATUS = "Binge eating";

				public static LocString TOOLTIP = "This Duplicant is attempting to eat their emotions due to stress";
			}

			public class EMOTEHIGHPRIORITY
			{
				public static LocString NAME = "Express Themselves";

				public static LocString STATUS = "Expressing themselves";

				public static LocString TOOLTIP = "This Duplicant needs a moment to express their feelings, then they'll be on their way";
			}

			public class FLEE
			{
				public static LocString NAME = "Flee";

				public static LocString STATUS = "Fleeing";

				public static LocString TOOLTIP = "Run away!";
			}

			public class RECOVERBREATH
			{
				public static LocString NAME = "Recover Breath";

				public static LocString STATUS = "Recovering breath";

				public static LocString TOOLTIP = "";
			}

			public class MOVETOQUARANTINE
			{
				public static LocString NAME = "Move to Quarantine";

				public static LocString STATUS = "Moving to quarantine";

				public static LocString TOOLTIP = "This Duplicant will isolate themselves to keep their illness away from the colony";
			}

			public class ATTACK
			{
				public static LocString NAME = "Attack";

				public static LocString STATUS = "Attacking";

				public static LocString TOOLTIP = "Chaaaarge!";
			}

			public class CAPTURE
			{
				public static LocString NAME = "Wrangle";

				public static LocString STATUS = "Wrangling";

				public static LocString TOOLTIP = "Duplicants that possess the Critter Ranching Skill can wrangle most critters without traps";
			}

			public class SINGTOEGG
			{
				public static LocString NAME = "Sing To Egg";

				public static LocString STATUS = "Singing to egg";

				public static LocString TOOLTIP = "A gentle lullaby from a supportive Duplicant encourages developing eggs\n\nIncreases Incubation Rate\n\nDuplicants must possess the Critter Ranching Skill to sing to an egg";
			}

			public class USETOILET
			{
				public static LocString NAME = "Use Toilet";

				public static LocString STATUS = "Going to use toilet";

				public static LocString TOOLTIP = "Duplicants have to use the toilet at least once per day";
			}

			public class WASHHANDS
			{
				public static LocString NAME = "Wash Hands";

				public static LocString STATUS = "Washing hands";

				public static LocString TOOLTIP = "Good hygiene removes germs and prevents the spread of disease";
			}

			public class CHECKPOINT
			{
				public static LocString NAME = "Wait at Checkpoint";

				public static LocString STATUS = "Waiting at Checkpoint";

				public static LocString TOOLTIP = "This Duplicant is waiting for permission to pass";
			}

			public class TRAVELTUBEENTRANCE
			{
				public static LocString NAME = "Enter Transit Tube";

				public static LocString STATUS = "Entering Transit Tube";

				public static LocString TOOLTIP = "Nyoooom!";
			}

			public class SCRUBORE
			{
				public static LocString NAME = "Scrub Ore";

				public static LocString STATUS = "Scrubbing ore";

				public static LocString TOOLTIP = "Material ore can be scrubbed to remove Germs present on its surface";
			}

			public class EAT
			{
				public static LocString NAME = "Eat";

				public static LocString STATUS = "Going to eat";

				public static LocString TOOLTIP = "Duplicants eat to replenish their calorie stores";
			}

			public class VOMIT
			{
				public static LocString NAME = "Vomit";

				public static LocString STATUS = "Vomiting";

				public static LocString TOOLTIP = "Vomiting produces Polluted Water and can spread disease";
			}

			public class COUGH
			{
				public static LocString NAME = "Cough";

				public static LocString STATUS = "Coughing";

				public static LocString TOOLTIP = "Coughing is a symptom of illness and spreads airborne Germs";
			}

			public class SLEEP
			{
				public static LocString NAME = "Sleep";

				public static LocString STATUS = "Sleeping";

				public static LocString TOOLTIP = "Zzzzzz...";
			}

			public class NARCOLEPSY
			{
				public static LocString NAME = "Narcoleptic Nap";

				public static LocString STATUS = "Narcoleptic napping";

				public static LocString TOOLTIP = "Zzzzzz...";
			}

			public class FLOORSLEEP
			{
				public static LocString NAME = "Sleep on Floor";

				public static LocString STATUS = "Sleeping on floor";

				public static LocString TOOLTIP = "Zzzzzz...\n\nSleeping on the floor will give Duplicants a Sore Back";
			}

			public class TAKEMEDICINE
			{
				public static LocString NAME = "Take Medicine";

				public static LocString STATUS = "Taking medicine";

				public static LocString TOOLTIP = "This Duplicant is taking a dose of medicine to ward off illness";
			}

			public class GETDOCTORED
			{
				public static LocString NAME = "Visit Doctor";

				public static LocString STATUS = "Visiting doctor";

				public static LocString TOOLTIP = "This Duplicant is visiting a doctor to receive treatment";
			}

			public class DOCTOR
			{
				public static LocString NAME = "Treat Patient";

				public static LocString STATUS = "Treating patient";

				public static LocString TOOLTIP = "This Duplicant is trying to make one of their peers feel better";
			}

			public class DELIVERFOOD
			{
				public static LocString NAME = "Deliver Food";

				public static LocString STATUS = "Delivering food";

				public static LocString TOOLTIP = "Under thirty minutes or it's free";
			}

			public class SHOWER
			{
				public static LocString NAME = "Shower";

				public static LocString STATUS = "Showering";

				public static LocString TOOLTIP = "This Duplicant is having a refreshing shower";
			}

			public class SIGH
			{
				public static LocString NAME = "Sigh";

				public static LocString STATUS = "Sighing";

				public static LocString TOOLTIP = "Ho-hum.";
			}

			public class RESTDUETODISEASE
			{
				public static LocString NAME = "Rest";

				public static LocString STATUS = "Resting";

				public static LocString TOOLTIP = "This Duplicant isn't feeling well and is taking a rest";
			}

			public class HEAL
			{
				public static LocString NAME = "Heal";

				public static LocString STATUS = "Healing";

				public static LocString TOOLTIP = "This Duplicant is taking some time to recover from their wounds";
			}

			public class STRESSACTINGOUT
			{
				public static LocString NAME = "Lash Out";

				public static LocString STATUS = "Lashing out";

				public static LocString TOOLTIP = "This Duplicant is having a stress-induced tantrum";
			}

			public class RELAX
			{
				public static LocString NAME = "Relax";

				public static LocString STATUS = "Relaxing";

				public static LocString TOOLTIP = "This Duplicant is taking it easy";
			}

			public class STRESSHEAL
			{
				public static LocString NAME = "De-Stress";

				public static LocString STATUS = "De-stressing";

				public static LocString TOOLTIP = "This Duplicant taking some time to recover from their stress";
			}

			public class EQUIP
			{
				public static LocString NAME = "Equip";

				public static LocString STATUS = "Moving to equip";

				public static LocString TOOLTIP = "This Duplicant is putting on a piece of equipment";
			}

			public class LEARNSKILL
			{
				public static LocString NAME = "Learn Skill";

				public static LocString STATUS = "Learning skill";

				public static LocString TOOLTIP = "This Duplicant is learning a new skill";
			}

			public class RECHARGE
			{
				public static LocString NAME = "Recharge Equipment";

				public static LocString STATUS = "Recharging equipment";

				public static LocString TOOLTIP = "This Duplicant is recharging their equipment";
			}

			public class UNEQUIP
			{
				public static LocString NAME = "Unequip";

				public static LocString STATUS = "Moving to unequip";

				public static LocString TOOLTIP = "This Duplicant is removing a piece of their equipment";
			}

			public class MOURN
			{
				public static LocString NAME = "Mourn";

				public static LocString STATUS = "Mourning";

				public static LocString TOOLTIP = "This Duplicant is mourning the loss of a friend";
			}

			public class WARMUP
			{
				public static LocString NAME = "Warm Up";

				public static LocString STATUS = "Going to warm up";

				public static LocString TOOLTIP = "This Duplicant got too cold and is going somewhere to warm up";
			}

			public class COOLDOWN
			{
				public static LocString NAME = "Cool Off";

				public static LocString STATUS = "Going to cool off";

				public static LocString TOOLTIP = "This Duplicant got too hot and is going somewhere to cool off";
			}

			public class EMPTYSTORAGE
			{
				public static LocString NAME = "Empty Storage";

				public static LocString STATUS = "Going to empty storage";

				public static LocString TOOLTIP = "This Duplicant is taking items out of storage";
			}

			public class ART
			{
				public static LocString NAME = "Decorate";

				public static LocString STATUS = "Going to decorate";

				public static LocString TOOLTIP = "This Duplicant is going to work on their art";
			}

			public class MOP
			{
				public static LocString NAME = "Mop";

				public static LocString STATUS = "Going to mop";

				public static LocString TOOLTIP = "Mopping removes liquids from the floor and bottles them for transport";
			}

			public class RELOCATE
			{
				public static LocString NAME = "Relocate";

				public static LocString STATUS = "Going to relocate";

				public static LocString TOOLTIP = "This Duplicant is moving a building to a new location";
			}

			public class TOGGLE
			{
				public static LocString NAME = "Change Setting";

				public static LocString STATUS = "Going to change setting";

				public static LocString TOOLTIP = "This Duplicant is going to change the settings on a building";
			}

			public class RESCUEINCAPACITATED
			{
				public static LocString NAME = "Rescue Friend";

				public static LocString STATUS = "Rescuing friend";

				public static LocString TOOLTIP = "This Duplicant is rescuing another Duplicant that has been incapacitated";
			}

			public class REPAIR
			{
				public static LocString NAME = "Repair";

				public static LocString STATUS = "Going to repair";

				public static LocString TOOLTIP = "This Duplicant is fixing a broken building";
			}

			public class DECONSTRUCT
			{
				public static LocString NAME = "Deconstruct";

				public static LocString STATUS = "Going to deconstruct";

				public static LocString TOOLTIP = "This Duplicant is demolishing a building";
			}

			public class RESEARCH
			{
				public static LocString NAME = "Research";

				public static LocString STATUS = "Going to research";

				public static LocString TOOLTIP = "This Duplicant is working on the current research focus";
			}

			public class RETURNSUIT
			{
				public static LocString NAME = "Dock Exosuit";

				public static LocString STATUS = "Docking exosuit";

				public static LocString TOOLTIP = "This Duplicant is plugging an exosuit in for refilling";
			}

			public class GENERATEPOWER
			{
				public static LocString NAME = "Generate Power";

				public static LocString STATUS = "Going to generate power";

				public static LocString TOOLTIP = "This Duplicant is producing electrical power";
			}

			public class HARVEST
			{
				public static LocString NAME = "Harvest";

				public static LocString STATUS = "Going to harvest";

				public static LocString TOOLTIP = "This Duplicant is harvesting usable materials from a mature plant";
			}

			public class UPROOT
			{
				public static LocString NAME = "Uproot";

				public static LocString STATUS = "Going to uproot";

				public static LocString TOOLTIP = "This Duplicant is uprooting a plant to retrieve a seed";
			}

			public class CLEANTOILET
			{
				public static LocString NAME = "Clean Outhouse";

				public static LocString STATUS = "Going to clean";

				public static LocString TOOLTIP = "This Duplicant is cleaning out the Outhouse";
			}

			public class EMPTYDESALINATOR
			{
				public static LocString NAME = "Empty Desalinator";

				public static LocString STATUS = "Going to clean";

				public static LocString TOOLTIP = "This Duplicant is emptying out the Desalinator";
			}

			public class LIQUIDCOOLEDFAN
			{
				public static LocString NAME = "Use Fan";

				public static LocString STATUS = "Going to use fan";

				public static LocString TOOLTIP = "This Duplicant is attempting to cool down the area";
			}

			public class ICECOOLEDFAN
			{
				public static LocString NAME = "Use Fan";

				public static LocString STATUS = "Going to use fan";

				public static LocString TOOLTIP = "This Duplicant is attempting to cool down the area";
			}

			public class COOK
			{
				public static LocString NAME = "Cook";

				public static LocString STATUS = "Going to cook";

				public static LocString TOOLTIP = "This Duplicant is cooking food";
			}

			public class COMPOUND
			{
				public static LocString NAME = "Compound Medicine";

				public static LocString STATUS = "Going to compound medicine";

				public static LocString TOOLTIP = "This Duplicant is fabricating medicine";
			}

			public class TRAIN
			{
				public static LocString NAME = "Train";

				public static LocString STATUS = "Training";

				public static LocString TOOLTIP = "This Duplicant is busy training";
			}

			public class MUSH
			{
				public static LocString NAME = "Mush";

				public static LocString STATUS = "Going to mush";

				public static LocString TOOLTIP = "This Duplicant is producing food";
			}

			public class COMPOSTWORKABLE
			{
				public static LocString NAME = "Compost";

				public static LocString STATUS = "Going to compost";

				public static LocString TOOLTIP = "This Duplicant is dropping off organic material at the compost";
			}

			public class FLIPCOMPOST
			{
				public static LocString NAME = "Flip";

				public static LocString STATUS = "Going to flip compost";

				public static LocString TOOLTIP = "Composts need to be flipped in order for their contents to compost";
			}

			public class DEPRESSURIZE
			{
				public static LocString NAME = "Depressurize Well";

				public static LocString STATUS = "Going to depressurize well";

				public static LocString TOOLTIP = "Oil Wells need to be periodically depressurized to function";
			}

			public class FABRICATE
			{
				public static LocString NAME = "Fabricate";

				public static LocString STATUS = "Going to fabricate";

				public static LocString TOOLTIP = "This Duplicant is crafting something";
			}

			public class BUILD
			{
				public static LocString NAME = "Build";

				public static LocString STATUS = "Going to build";

				public static LocString TOOLTIP = "This Duplicant is constructing a new building";
			}

			public class BUILDDIG
			{
				public static LocString NAME = "Construction Dig";

				public static LocString STATUS = "Going to construction dig";

				public static LocString TOOLTIP = "This Duplicant is making room for a planned construction task by performing this dig";
			}

			public class DIG
			{
				public static LocString NAME = "Dig";

				public static LocString STATUS = "Going to dig";

				public static LocString TOOLTIP = "This Duplicant is digging out a tile";
			}

			public class FETCH
			{
				public static LocString NAME = "Deliver";

				public static LocString STATUS = "Delivering";

				public static LocString TOOLTIP = "This Duplicant is delivering materials where they need to go";

				public static LocString REPORT_NAME = "Deliver to {0}";
			}

			public class STORAGEFETCH
			{
				public static LocString NAME = "Store Materials";

				public static LocString STATUS = "Storing materials";

				public static LocString TOOLTIP = "This Duplicant is moving materials into storage for later use";

				public static LocString REPORT_NAME = "Store {0}";
			}

			public class REPAIRFETCH
			{
				public static LocString NAME = "Repair Supply";

				public static LocString STATUS = "Supplying repair materials";

				public static LocString TOOLTIP = "This Duplicant is delivering materials to where they'll be needed to repair buildings";
			}

			public class RESEARCHFETCH
			{
				public static LocString NAME = "Research Supply";

				public static LocString STATUS = "Supplying research materials";

				public static LocString TOOLTIP = "This Duplicant is delivering materials where they'll be needed to conduct research";
			}

			public class FARMFETCH
			{
				public static LocString NAME = "Farming Supply";

				public static LocString STATUS = "Supplying farming materials";

				public static LocString TOOLTIP = "This Duplicant is delivering farming materials where they're needed to tend crops";
			}

			public class FETCHCRITICAL
			{
				public static LocString NAME = "Life Support Supply";

				public static LocString STATUS = "Supplying critical materials";

				public static LocString TOOLTIP = "This Duplicant is delivering materials required to perform Life Support Errands";

				public static LocString REPORT_NAME = "Life Support Supply to {0}";
			}

			public class MACHINEFETCH
			{
				public static LocString NAME = "Operational Supply";

				public static LocString STATUS = "Supplying operational materials";

				public static LocString TOOLTIP = "This Duplicant is delivering materials to where they'll be needed for machine operation";

				public static LocString REPORT_NAME = "Operational Supply to {0}";
			}

			public class COOKFETCH
			{
				public static LocString NAME = "Cook Supply";

				public static LocString STATUS = "Supplying cook ingredients";

				public static LocString TOOLTIP = "This Duplicant is delivering materials required to cook food";
			}

			public class DOCTORFETCH
			{
				public static LocString NAME = "Medical Supply";

				public static LocString STATUS = "Supplying medical resources";

				public static LocString TOOLTIP = "This Duplicant is delivering the materials that will be needed to treat sick patients";

				public static LocString REPORT_NAME = "Medical Supply to {0}";
			}

			public class FOODFETCH
			{
				public static LocString NAME = "Store Food";

				public static LocString STATUS = "Storing food";

				public static LocString TOOLTIP = "This Duplicant is moving edible resources into proper storage";

				public static LocString REPORT_NAME = "Store {0}";
			}

			public class POWERFETCH
			{
				public static LocString NAME = "Power Supply";

				public static LocString STATUS = "Supplying power materials";

				public static LocString TOOLTIP = "This Duplicant is delivering materials to where they'll be needed for power";

				public static LocString REPORT_NAME = "Power Supply to {0}";
			}

			public class FABRICATEFETCH
			{
				public static LocString NAME = "Fabrication Supply";

				public static LocString STATUS = "Supplying fabrication materials";

				public static LocString TOOLTIP = "This Duplicant is delivering materials required to fabricate new objects";

				public static LocString REPORT_NAME = "Fabrication Supply to {0}";
			}

			public class BUILDFETCH
			{
				public static LocString NAME = "Construction Supply";

				public static LocString STATUS = "Supplying construction materials";

				public static LocString TOOLTIP = "This delivery will provide materials to a planned construction site";
			}

			public class FETCHCREATURE
			{
				public static LocString NAME = "Relocate Critter";

				public static LocString STATUS = "Relocating critter";

				public static LocString TOOLTIP = "This Duplicant is moving a critter to a new location";
			}

			public class TRANSPORT
			{
				public static LocString NAME = "Sweep";

				public static LocString STATUS = "Going to sweep";

				public static LocString TOOLTIP = "Moving debris off the ground and into storage improves colony decor";
			}

			public class MOVETOSAFETY
			{
				public static LocString NAME = "Find Safe Area";

				public static LocString STATUS = "Finding safer area";

				public static LocString TOOLTIP = "This Duplicant is idle and looking for somewhere safe and comfy to chill";
			}

			public class POWER_TINKER
			{
				public static LocString NAME = "Tinker";

				public static LocString STATUS = "Tinkering";

				public static LocString TOOLTIP = "Tinkering with buildings improves their functionality";
			}

			public class RANCH
			{
				public static LocString NAME = "Ranch";

				public static LocString STATUS = "Ranching";

				public static LocString TOOLTIP = "This Duplicant is tending to a critter's wellbeing";

				public static LocString REPORT_NAME = "Deliver to {0}";
			}

			public class CROP_TEND
			{
				public static LocString NAME = "Tend";

				public static LocString STATUS = "Tending plant";

				public static LocString TOOLTIP = "Tending to plants increases their growth rate";
			}

			public class IDLE
			{
				public static LocString NAME = "Idle";

				public static LocString STATUS = "Idle";

				public static LocString TOOLTIP = "This Duplicant cannot reach any pending errands";
			}

			public class PRECONDITIONS
			{
				public static LocString HEADER = "The selected {Selected} could:";

				public static LocString SUCCESS_ROW = "{Duplicant} -- {Rank}";

				public static LocString CURRENT_ERRAND = "Current Errand";

				public static LocString RANK_FORMAT = "#{0}";

				public static LocString FAILURE_ROW = "{Duplicant} -- {Reason}";

				public static LocString CONTAINS_OXYGEN = "Not enough Oxygen";

				public static LocString IS_PREEMPTABLE = "Already assigned to {Assignee}";

				public static LocString HAS_URGE = "No current need";

				public static LocString IS_VALID = "Invalid";

				public static LocString IS_PERMITTED = "Not permitted";

				public static LocString IS_ASSIGNED_TO_ME = "Not assigned to {Selected}";

				public static LocString IS_IN_MY_ROOM = "Outside {Selected}'s room";

				public static LocString IS_PREFERRED_ASSIGNABLE = "Not preferred assignment";

				public static LocString IS_PREFERRED_ASSIGNABLE_OR_URGENT_BLADDER = "Not preferred assignment";

				public static LocString HAS_SKILL_PERK = "Requires learned skill";

				public static LocString IS_MORE_SATISFYING = "Low priority";

				public static LocString CAN_CHAT = "Unreachable";

				public static LocString IS_NOT_RED_ALERT = "Unavailable in Red Alert";

				public static LocString NO_DEAD_BODIES = "Unburied Duplicant";

				public static LocString VALID_MOURNING_SITE = "Nowhere to mourn";

				public static LocString HAS_PLACE_TO_STAND = "Nowhere to stand";

				public static LocString IS_SCHEDULED_TIME = "Not allowed by schedule";

				public static LocString CAN_MOVE_TO = "Unreachable";

				public static LocString CAN_PICKUP = "Cannot pickup";

				public static LocString IS_AWAKE = "{Selected} is sleeping";

				public static LocString IS_STANDING = "{Selected} must stand";

				public static LocString IS_MOVING = "{Selected} is not moving";

				public static LocString IS_OFF_LADDER = "{Selected} is busy climbing";

				public static LocString NOT_IN_TUBE = "{Selected} is busy in transit";

				public static LocString HAS_TRAIT = "Missing required trait";

				public static LocString IS_OPERATIONAL = "Not operational";

				public static LocString IS_MARKED_FOR_DECONSTRUCTION = "Being deconstructed";

				public static LocString IS_NOT_BURROWED = "Is not burrowed";

				public static LocString IS_CREATURE_AVAILABLE_FOR_RANCHING = "No Critters Available";

				public static LocString IS_CREATURE_AVAILABLE_FOR_FIXED_CAPTURE = "Pen Status OK";

				public static LocString IS_MARKED_FOR_DISABLE = "Building Disabled";

				public static LocString IS_FUNCTIONAL = "Not functioning";

				public static LocString IS_OVERRIDE_TARGET_NULL_OR_ME = "DebugIsOverrideTargetNullOrMe";

				public static LocString NOT_CHORE_CREATOR = "DebugNotChoreCreator";

				public static LocString IS_GETTING_MORE_STRESSED = "{Selected}'s stress is decreasing";

				public static LocString IS_ALLOWED_BY_AUTOMATION = "Automated";

				public static LocString CAN_DO_RECREATION = "Not Interested";

				public static LocString DOES_SUIT_NEED_RECHARGING_IDLE = "Suit is currently charged";

				public static LocString DOES_SUIT_NEED_RECHARGING_URGENT = "Suit is currently charged";

				public static LocString HAS_SUIT_MARKER = "No Suit Checkpoint";

				public static LocString ALLOWED_TO_DEPRESSURIZE = "Not currently overpressure";

				public static LocString IS_STRESS_ABOVE_ACTIVATION_RANGE = "{Selected} is not stressed right now";

				public static LocString IS_NOT_ANGRY = "{Selected} is too angry";

				public static LocString IS_NOT_BEING_ATTACKED = "{Selected} is in combat";

				public static LocString IS_CONSUMPTION_PERMITTED = "Disallowed by consumable permissions";

				public static LocString CAN_CURE = "No applicable illness";

				public static LocString TREATMENT_AVAILABLE = "No treatable illness";

				public static LocString DOCTOR_AVAILABLE = "No doctors available\n(Duplicants cannot treat themselves)";

				public static LocString IS_OKAY_TIME_TO_SLEEP = "No current need";

				public static LocString IS_NARCOLEPSING = "{Selected} is currently napping";

				public static LocString IS_FETCH_TARGET_AVAILABLE = "No pending deliveries";

				public static LocString EDIBLE_IS_NOT_NULL = "Consumable Permission not allowed";

				public static LocString HAS_MINGLE_CELL = "Nowhere to Mingle";

				public static LocString EXCLUSIVELY_AVAILABLE = "Building Already Busy";

				public static LocString BLADDER_FULL = "Bladder isn't full";

				public static LocString BLADDER_NOT_FULL = "Bladder too full";

				public static LocString CURRENTLY_PEEING = "Currently Peeing";
			}

			public static LocString NOT_EXISTING_TASK = "Not Existing";

			public static LocString IS_DEAD_TASK = "Dead";
		}

		public class CHOREGROUPS
		{
			public class ART
			{
				public static LocString NAME = "Decorating";

				public static LocString DESC = "Sculpt or paint to improve colony decor.";

				public static LocString ARCHETYPE_NAME = "Decorator";
			}

			public class COMBAT
			{
				public static LocString NAME = "Attacking";

				public static LocString DESC = "Fight wild critters.";

				public static LocString ARCHETYPE_NAME = "Attacker";
			}

			public class LIFESUPPORT
			{
				public static LocString NAME = "Life Support";

				public static LocString DESC = "Maintain algae terrariums, deodorizers, and water sieves to support colony life.";

				public static LocString ARCHETYPE_NAME = "Life Supporter";
			}

			public class TOGGLE
			{
				public static LocString NAME = "Toggling";

				public static LocString DESC = "Tune machinery and set or flip switches and sensors.";

				public static LocString ARCHETYPE_NAME = "Toggler";
			}

			public class COOK
			{
				public static LocString NAME = "Cooking";

				public static LocString DESC = "Operate food preparation buildings.";

				public static LocString ARCHETYPE_NAME = "Cooker";
			}

			public class RESEARCH
			{
				public static LocString NAME = "Researching";

				public static LocString DESC = "Use research stations to unlock new technologies.";

				public static LocString ARCHETYPE_NAME = "Researcher";
			}

			public class REPAIR
			{
				public static LocString NAME = "Repairing";

				public static LocString DESC = "Repair damaged buildings.";

				public static LocString ARCHETYPE_NAME = "Repairer";
			}

			public class FARMING
			{
				public static LocString NAME = "Farming";

				public static LocString DESC = "Gather crops from mature plants.";

				public static LocString ARCHETYPE_NAME = "Farmer";
			}

			public class RANCHING
			{
				public static LocString NAME = "Ranching";

				public static LocString DESC = "Tend to domesticated critters.";

				public static LocString ARCHETYPE_NAME = "Rancher";
			}

			public class BUILD
			{
				public static LocString NAME = "Building";

				public static LocString DESC = "Construct new buildings.";

				public static LocString ARCHETYPE_NAME = "Builder";
			}

			public class HAULING
			{
				public static LocString NAME = "Supplying";

				public static LocString DESC = "Run resources to critical buildings and urgent storage.";

				public static LocString ARCHETYPE_NAME = "Supplier";
			}

			public class STORAGE
			{
				public static LocString NAME = "Storing";

				public static LocString DESC = "Fill storage buildings with resources when no other errands are available.";

				public static LocString ARCHETYPE_NAME = "Storer";
			}

			public class BASEKEEPING
			{
				public static LocString NAME = "Tidying";

				public static LocString DESC = "Sweep, mop, and disinfect objects within the colony.";

				public static LocString ARCHETYPE_NAME = "Tidier";
			}

			public class DIG
			{
				public static LocString NAME = "Digging";

				public static LocString DESC = "Mine raw resources.";

				public static LocString ARCHETYPE_NAME = "Digger";
			}

			public class MEDICALAID
			{
				public static LocString NAME = "Doctoring";

				public static LocString DESC = "Treat sick and injured Duplicants.";

				public static LocString ARCHETYPE_NAME = "Doctor";
			}

			public class MASSAGE
			{
				public static LocString NAME = "Relaxing";

				public static LocString DESC = "Take breaks for massages.";

				public static LocString ARCHETYPE_NAME = "Relaxer";
			}

			public class MACHINEOPERATING
			{
				public static LocString NAME = "Operating";

				public static LocString DESC = "Operating machinery for production, fabrication, and utility purposes.";

				public static LocString ARCHETYPE_NAME = "Operator";
			}

			public class SUITS
			{
				public static LocString ARCHETYPE_NAME = "Suit Wearer";
			}
		}

		public class STATUSITEMS
		{
			public class GENERIC_DELIVER
			{
				public static LocString NAME = "Delivering resources to {Target}";

				public static LocString TOOLTIP = "This Duplicant is transporting materials to {Target}";
			}

			public class SLEEPING
			{
				public static LocString NAME = "Sleeping";

				public static LocString TOOLTIP = "This Duplicant is recovering stamina";

				public static LocString TOOLTIP_DISTURBER = "\n\nThey were sleeping peacefully until they were disturbed by {Disturber}";
			}

			public class SLEEPINGPEACEFULLY
			{
				public static LocString NAME = "Sleeping peacefully";

				public static LocString TOOLTIP = "This Duplicant is getting well-deserved, quality sleep\n\nAt this rate they're sure to feel " + UI.FormatAsLink("Well Rested", "SLEEP") + " tomorrow morning";
			}

			public class SLEEPINGBADLY
			{
				public static LocString NAME = "Sleeping badly";

				public static LocString TOOLTIP = "This Duplicant's having trouble falling asleep due to noise from {Disturber}\n\nThey're going to feel a bit " + UI.FormatAsLink("Unrested", "SLEEP") + " tomorrow morning";
			}

			public class SLEEPINGTERRIBLY
			{
				public static LocString NAME = "Can't sleep";

				public static LocString TOOLTIP = "This Duplicant was woken up by noise from {Disturber} and can't get back to sleep\n\nThey're going to feel " + UI.FormatAsLink("Dead Tired", "SLEEP") + " tomorrow morning";
			}

			public class SLEEPINGINTERRUPTEDBYLIGHT
			{
				public static LocString NAME = "Interrupted Sleep: Bright Light";

				public static LocString TOOLTIP = "This Duplicant can't sleep because the lights are still on";
			}

			public class SLEEPINGINTERRUPTEDBYNOISE
			{
				public static LocString NAME = "Interrupted Sleep: Snoring Friend";

				public static LocString TOOLTIP = "This Duplicant is having trouble sleeping thanks to a certain noisy someone";
			}

			public class REDALERT
			{
				public static LocString NAME = "Red Alert!";

				public static LocString TOOLTIP = "The colony is in a state of Red Alert. Duplicants will not eat, sleep, use the bathroom, or engage in leisure activities while the Red Alert is active";
			}

			public class ROLE
			{
				public static LocString NAME = "{Role}: {Progress} Mastery";

				public static LocString TOOLTIP = "This Duplicant is working as a {Role}" + UI.HORIZONTAL_BR_RULE + "They have {Progress} mastery of this job";
			}

			public class LOWOXYGEN
			{
				public static LocString NAME = "Oxygen low";

				public static LocString TOOLTIP = "This Duplicant is working in a low breathability area";

				public static LocString NOTIFICATION_NAME = "Low " + ELEMENTS.OXYGEN.NAME + " area entered";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are working in areas with low " + ELEMENTS.OXYGEN.NAME + ":";
			}

			public class SEVEREWOUNDS
			{
				public static LocString NAME = "Severely injured";

				public static LocString TOOLTIP = "This Duplicant is badly hurt";

				public static LocString NOTIFICATION_NAME = "Severely injured";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are badly hurt and require medical attention";
			}

			public class INCAPACITATED
			{
				public static LocString NAME = "Incapacitated: {CauseOfIncapacitation}\nTime until death: {TimeUntilDeath}\n";

				public static LocString TOOLTIP = "This Duplicant is near death!" + UI.HORIZONTAL_BR_RULE + "Assign them to a Triage Cot for rescue";

				public static LocString NOTIFICATION_NAME = "Incapacitated";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are near death.\nA Triage Cot is required for rescue:";
			}

			public class BEDUNREACHABLE
			{
				public static LocString NAME = "Cannot reach bed";

				public static LocString TOOLTIP = "This Duplicant cannot reach their bed";

				public static LocString NOTIFICATION_NAME = "Unreachable bed";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants cannot sleep because their beds are beyond their reach:";
			}

			public class COLD
			{
				public static LocString NAME = "Chilly surroundings";

				public static LocString TOOLTIP = "This Duplicant cannot retain enough heat to stay warm and may be under insulated for this area\nStress Modification: {StressModification}\n\nCurrent Environmental Exchange: {currentTransferWattage}\n\nInsulation Thickness: {conductivityBarrier}";
			}

			public class DAILYRATIONLIMITREACHED
			{
				public static LocString NAME = "Daily calorie limit reached";

				public static LocString TOOLTIP = "This Duplicant has consumed their allotted rations for the day";

				public static LocString NOTIFICATION_NAME = "Daily calorie limit reached";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have consumed their allotted " + UI.FormatAsLink("Rations", "FOOD") + " for the day:";
			}

			public class DOCTOR
			{
				public static LocString NAME = "Treating Patient";

				public static LocString STATUS = "This Duplicant is going to administer medical care to an ailing friend";
			}

			public class HOLDINGBREATH
			{
				public static LocString NAME = "Holding breath";

				public static LocString TOOLTIP = "This Duplicant cannot breathe in their current location";
			}

			public class RECOVERINGBREATH
			{
				public static LocString NAME = "Recovering breath";

				public static LocString TOOLTIP = "This Duplicant held their breath too long and needs a moment";
			}

			public class HOT
			{
				public static LocString NAME = "Toasty surroundings";

				public static LocString TOOLTIP = "This Duplicant cannot let off enough heat to stay cool and may be over insulated for this area\nStress Modification: {StressModification}\n\nCurrent Environmental Exchange: {currentTransferWattage}\n\nInsulation Thickness: {conductivityBarrier}";
			}

			public class HUNGRY
			{
				public static LocString NAME = "Hungry";

				public static LocString TOOLTIP = "This Duplicant would really like something to eat";
			}

			public class POORDECOR
			{
				public static LocString NAME = "Drab decor";

				public static LocString TOOLTIP = "This Duplicant is depressed by the lack of decor in this area";
			}

			public class POORQUALITYOFLIFE
			{
				public static LocString NAME = "Low Morale";

				public static LocString TOOLTIP = "The bad in this Duplicant's life is starting to outweigh the good\n\nImproved amenities and additional Downtime would help improve their Morale";
			}

			public class POOR_FOOD_QUALITY
			{
				public static LocString NAME = "Lousy Meal";

				public static LocString TOOLTIP = "The last meal this Duplicant ate didn't quite meet their expectations";
			}

			public class GOOD_FOOD_QUALITY
			{
				public static LocString NAME = "Decadent Meal";

				public static LocString TOOLTIP = "The last meal this Duplicant ate exceeded their expectations!";
			}

			public class NERVOUSBREAKDOWN
			{
				public static LocString NAME = "Nervous breakdown";

				public static LocString TOOLTIP = "Stress has completely eroded this Duplicant's ability to function";

				public static LocString NOTIFICATION_NAME = "Nervous breakdown";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have cracked under the " + UI.FormatAsLink("Stress", "STRESS") + " and need assistance:";
			}

			public class STRESSED
			{
				public static LocString NAME = "Stressed";

				public static LocString TOOLTIP = "This Duplicant is feeling the pressure";

				public static LocString NOTIFICATION_NAME = "High stress";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are " + UI.FormatAsLink("Stressed", "STRESS") + " and need to unwind:";
			}

			public class NORATIONSAVAILABLE
			{
				public static LocString NAME = "No food available";

				public static LocString TOOLTIP = "There's nothing in the colony for this Duplicant to eat";

				public static LocString NOTIFICATION_NAME = "No food available";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have nothing to eat:";
			}

			public class QUARANTINEAREAUNREACHABLE
			{
				public static LocString NAME = "Cannot reach quarantine";

				public static LocString TOOLTIP = "This Duplicant cannot reach their quarantine zone";

				public static LocString NOTIFICATION_NAME = "Unreachable quarantine";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants cannot reach their assigned quarantine zones:";
			}

			public class QUARANTINED
			{
				public static LocString NAME = "Quarantined";

				public static LocString TOOLTIP = "This Duplicant has been isolated from the colony";
			}

			public class RATIONSUNREACHABLE
			{
				public static LocString NAME = "Cannot reach food";

				public static LocString TOOLTIP = "There is food in the colony that this Duplicant cannot reach";

				public static LocString NOTIFICATION_NAME = "Unreachable food";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants cannot access the colony's " + UI.FormatAsLink("Food", "FOOD") + ":";
			}

			public class RATIONSNOTPERMITTED
			{
				public static LocString NAME = "Food Type Not Permitted";

				public static LocString TOOLTIP = "This Duplicant is not allowed to eat any of the " + UI.FormatAsLink("Food", "FOOD") + " in their reach" + UI.HORIZONTAL_BR_RULE + "Enter the <color=#833A5FFF>CONSUMABLES</color> <color=#F44A47><b>[F]</b></color> to adjust their food permissions";

				public static LocString NOTIFICATION_NAME = "Unpermitted food";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants' <color=#833A5FFF>CONSUMABLES</color> <color=#F44A47><b>[F]</b></color> permissions prevent them from eating any of the " + UI.FormatAsLink("Food", "FOOD") + " within their reach:";
			}

			public class ROTTEN
			{
				public static LocString NAME = "Rotten";

				public static LocString TOOLTIP = "Gross!";
			}

			public class STARVING
			{
				public static LocString NAME = "Starving";

				public static LocString TOOLTIP = "This Duplicant is about to die and needs " + UI.FormatAsLink("Food", "FOOD") + "!";

				public static LocString NOTIFICATION_NAME = "Starvation";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are starving and will die if they can't find " + UI.FormatAsLink("Food", "FOOD") + ":";
			}

			public class STRESS_SIGNAL_AGGRESIVE
			{
				public static LocString NAME = "Frustrated";

				public static LocString TOOLTIP = "This Duplicant is trying to keep their cool" + UI.HORIZONTAL_BR_RULE + "Improve this Duplicant's Morale before they destroy something to let off steam";
			}

			public class STRESS_SIGNAL_BINGE_EAT
			{
				public static LocString NAME = "Stress Cravings";

				public static LocString TOOLTIP = "This Duplicant is consumed by hunger" + UI.HORIZONTAL_BR_RULE + "Improve this Duplicant's Morale before they eat all the colony's food stores";
			}

			public class STRESS_SIGNAL_UGLY_CRIER
			{
				public static LocString NAME = "Misty Eyed";

				public static LocString TOOLTIP = "This Duplicant is trying and failing to swallow their emotions" + UI.HORIZONTAL_BR_RULE + "Improve this Duplicant's Morale before they have a good ugly cry";
			}

			public class STRESS_SIGNAL_VOMITER
			{
				public static LocString NAME = "Stress Burp";

				public static LocString TOOLTIP = "Sort of like having butterflies in your stomach, except they're burps" + UI.HORIZONTAL_BR_RULE + "Improve this Duplicant's Morale before they start to stress vomit";
			}

			public class ENTOMBEDCHORE
			{
				public static LocString NAME = "Entombed";

				public static LocString TOOLTIP = "This Duplicant needs someone to help dig them out!";

				public static LocString NOTIFICATION_NAME = "Entombed";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are trapped:";
			}

			public class EARLYMORNING
			{
				public static LocString NAME = "Early Bird";

				public static LocString TOOLTIP = "This Duplicant is jazzed to start the day\n• +2 to all Attributes in morning";
			}

			public class NIGHTTIME
			{
				public static LocString NAME = "Night Owl";

				public static LocString TOOLTIP = "This Duplicant is more efficient on a nighttime " + UI.FormatAsLink("Schedule", "SCHEDULE") + "\n• +3 to all Attributes at night";
			}

			public class SUFFOCATING
			{
				public static LocString NAME = "Suffocating";

				public static LocString TOOLTIP = "This Duplicant cannot breathe!";

				public static LocString NOTIFICATION_NAME = "Suffocating";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants cannot breathe:";
			}

			public class TIRED
			{
				public static LocString NAME = "Tired";

				public static LocString TOOLTIP = "This Duplicant could use a nice nap";
			}

			public class IDLE
			{
				public static LocString NAME = "Idle";

				public static LocString TOOLTIP = "This Duplicant cannot reach any pending errands";

				public static LocString NOTIFICATION_NAME = "Idle";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants cannot reach any pending errands:";
			}

			public class FIGHTING
			{
				public static LocString NAME = "In combat";

				public static LocString TOOLTIP = "This Duplicant is attacking a critter!";

				public static LocString NOTIFICATION_NAME = "Combat!";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have engaged a critter in combat:";
			}

			public class FLEEING
			{
				public static LocString NAME = "Fleeing";

				public static LocString TOOLTIP = "This Duplicant is trying to escape combat!";

				public static LocString NOTIFICATION_NAME = "Fleeing!";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are trying to escape combat:";
			}

			public class DEAD
			{
				public static LocString NAME = "Dead: {Death}";

				public static LocString TOOLTIP = "This Duplicant definitely isn't sleeping";
			}

			public class LASHINGOUT
			{
				public static LocString NAME = "Lashing out";

				public static LocString TOOLTIP = "This Duplicant is breaking buildings to relieve their " + UI.FormatAsLink("Stress", "STRESS");

				public static LocString NOTIFICATION_NAME = "Lashing out";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants broke buildings to relieve their " + UI.FormatAsLink("Stress", "STRESS") + ":";
			}

			public class MOVETOSUITNOTREQUIRED
			{
				public static LocString NAME = "Exiting " + UI.FormatAsLink("Exosuit", "EXOSUIT") + " area";

				public static LocString TOOLTIP = "This Duplicant is leaving an area where a suit was required";
			}

			public class NOROLE
			{
				public static LocString NAME = "No Job";

				public static LocString TOOLTIP = "This Duplicant does not have a Job Assignment" + UI.HORIZONTAL_BR_RULE + "Enter the <b>Jobs Panel</b> <color=#F44A47><b>[J]</b></color> to view all available Jobs";
			}

			public class DROPPINGUNUSEDINVENTORY
			{
				public static LocString NAME = "Dropping objects";

				public static LocString TOOLTIP = "This Duplicant is dropping what they're holding";
			}

			public class MOVINGTOSAFEAREA
			{
				public static LocString NAME = "Moving to safe area";

				public static LocString TOOLTIP = "This Duplicant is finding a less dangerous place";
			}

			public class TOILETUNREACHABLE
			{
				public static LocString NAME = "Unreachable toilet";

				public static LocString TOOLTIP = "This Duplicant cannot reach a functioning " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + " or " + UI.FormatAsLink("Lavatory", "FLUSHTOILET");

				public static LocString NOTIFICATION_NAME = "Unreachable toilet";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants cannot reach a functioning " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + " or " + UI.FormatAsLink("Lavatory", "FLUSHTOILET") + ":";
			}

			public class NOUSABLETOILETS
			{
				public static LocString NAME = "Toilet out of order";

				public static LocString TOOLTIP = "The only " + UI.FormatAsLink("Outhouses", "OUTHOUSE") + " or " + UI.FormatAsLink("Lavatories", "FLUSHTOILET") + " in this Duplicant's reach are out of order";

				public static LocString NOTIFICATION_NAME = "Toilet out of order";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants want to use an " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + " or " + UI.FormatAsLink("Lavatory", "FLUSHTOILET") + " that is out of order:";
			}

			public class NOTOILETS
			{
				public static LocString NAME = "No Outhouses";

				public static LocString TOOLTIP = "There are no " + UI.FormatAsLink("Outhouses", "OUTHOUSE") + " available for this Duplicant" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Outhouses", "OUTHOUSE") + " can be built from the " + UI.FormatAsLink("PLUMBING TAB", "MISC") + " <color=#F44A47><b>[5]</b></color>.";

				public static LocString NOTIFICATION_NAME = "No Outhouses built";

				public static LocString NOTIFICATION_TOOLTIP = UI.FormatAsLink("Outhouses", "OUTHOUSE") + " can be built from the " + UI.FormatAsLink("PLUMBING TAB", "MISC") + " <color=#F44A47><b>[5]</b></color>." + UI.HORIZONTAL_BR_RULE + "These Duplicants are in need of an " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + ":";
			}

			public class FULLBLADDER
			{
				public static LocString NAME = "Full bladder";

				public static LocString TOOLTIP = "This Duplicant would really appreciate an " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + " or " + UI.FormatAsLink("Lavatory", "FLUSHTOILET");
			}

			public class STRESSFULLYEMPTYINGBLADDER
			{
				public static LocString NAME = "Making a mess";

				public static LocString TOOLTIP = "This poor Duplicant couldn't find an " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + " in time and is super embarrassed" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Outhouses", "OUTHOUSE") + " can be built from the " + UI.FormatAsLink("PLUMBING TAB", "MISC") + " <color=#F44A47><b>[5]</b></color>";

				public static LocString NOTIFICATION_NAME = "Made a mess";

				public static LocString NOTIFICATION_TOOLTIP = "The " + UI.FormatAsLink("MOP TOOL", "MISC") + " <color=#F44A47><b>[M]</b></color> can be used to clean up Duplicant-related \"spills\"" + UI.HORIZONTAL_BR_RULE + "These Duplicants made messes that require cleaning up:\n";
			}

			public class WASHINGHANDS
			{
				public static LocString NAME = "Washing hands";

				public static LocString TOOLTIP = "This Duplicant is washing their hands";
			}

			public class SHOWERING
			{
				public static LocString NAME = "Showering";

				public static LocString TOOLTIP = "This Duplicant is gonna be squeaky clean";
			}

			public class RELAXING
			{
				public static LocString NAME = "Relaxing";

				public static LocString TOOLTIP = "This Duplicant's just taking it easy";
			}

			public class VOMITING
			{
				public static LocString NAME = "Throwing up";

				public static LocString TOOLTIP = "This Duplicant has unceremoniously hurled as the result of a " + UI.FormatAsLink("Disease", "DISEASE") + UI.HORIZONTAL_BR_RULE + "Duplicant-related \"spills\" can be cleaned up using the " + UI.FormatAsLink("MOP TOOL", "MISC") + " <color=#F44A47><b>[M]</b></color>";

				public static LocString NOTIFICATION_NAME = "Throwing up";

				public static LocString NOTIFICATION_TOOLTIP = "The " + UI.FormatAsLink("MOP TOOL", "MISC") + " <color=#F44A47><b>[M]</b></color> can be used to clean up Duplicant-related \"spills\"" + UI.HORIZONTAL_BR_RULE + "A " + UI.FormatAsLink("Disease", "DISEASE") + " has caused these Duplicants to throw up:";
			}

			public class STRESSVOMITING
			{
				public static LocString NAME = "Stress vomiting";

				public static LocString TOOLTIP = "This Duplicant is relieving their " + UI.FormatAsLink("Stress", "STRESS") + " all over the floor" + UI.HORIZONTAL_BR_RULE + "Duplicant-related \"spills\" can be cleaned up using the " + UI.FormatAsLink("MOP TOOL", "MISC") + " <color=#F44A47><b>[M]</b></color>";

				public static LocString NOTIFICATION_NAME = "Stress vomiting";

				public static LocString NOTIFICATION_TOOLTIP = "The " + UI.FormatAsLink("MOP TOOL", "MISC") + " <color=#F44A47><b>[M]</b></color> can used to clean up Duplicant-related \"spills\"" + UI.HORIZONTAL_BR_RULE + "These Duplicants became so " + UI.FormatAsLink("Stressed", "STRESS") + " they threw up:";
			}

			public class HASDISEASE
			{
				public static LocString NAME = "Feeling ill";

				public static LocString TOOLTIP = "This Duplicant has contracted a " + UI.FormatAsLink("Disease", "DISEASE") + " and requires recovery time at a " + UI.FormatAsLink("Sick Bay", "DOCTORSTATION");

				public static LocString NOTIFICATION_NAME = "Illness";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have contracted a " + UI.FormatAsLink("Disease", "DISEASE") + " and require recovery time at a " + UI.FormatAsLink("Sick Bay", "DOCTORSTATION") + ":";
			}

			public class BODYREGULATINGHEATING
			{
				public static LocString NAME = "Regulating temperature at: {TempDelta}";

				public static LocString TOOLTIP = "This Duplicant is regulating their internal temperature";
			}

			public class BODYREGULATINGCOOLING
			{
				public static LocString NAME = "Regulating temperature at: {TempDelta}";

				public static LocString TOOLTIP = "This Duplicant is regulating their internal temperature";
			}

			public class BREATHINGO2
			{
				public static LocString NAME = "Inhaling {ConsumptionRate} O<sub>2</sub>";

				public static LocString TOOLTIP = "Duplicants require " + UI.FormatAsLink("Oxygen", "OXYGEN") + " to live";
			}

			public class EMITTINGCO2
			{
				public static LocString NAME = "Exhaling {EmittingRate} CO<sub>2</sub>";

				public static LocString TOOLTIP = "Duplicants breathe out " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE");
			}

			public class PICKUPDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class STOREDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class CLEARDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class STOREFORBUILDDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class STOREFORBUILDPRIORITIZEDDELIVERSTATUS
			{
				public static LocString NAME = "Allocating {Item} to {Target}";

				public static LocString TOOLTIP = "This Duplicant is delivering materials to a {Target} construction errand";
			}

			public class BUILDDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class BUILDPRIORITIZEDSTATUS
			{
				public static LocString NAME = "Building {Target}";

				public static LocString TOOLTIP = "This Duplicant is constructing a {Target}";
			}

			public class FABRICATEDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class USEITEMDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class STOREPRIORITYDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class STORECRITICALDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class COMPOSTFLIPSTATUS
			{
				public static LocString NAME = "Going to flip compost";

				public static LocString TOOLTIP = "This Duplicant is going to flip the compost";
			}

			public class DECONSTRUCTDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class TOGGLEDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class EMPTYSTORAGEDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class HARVESTDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class SLEEPDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class EATDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class WARMUPDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class REPAIRDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class REPAIRWORKSTATUS
			{
				public static LocString NAME = "Repairing {Target}";

				public static LocString TOOLTIP = "This Duplicant is fixing the {Target}";
			}

			public class BREAKDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class BREAKWORKSTATUS
			{
				public static LocString NAME = "Breaking {Target}";

				public static LocString TOOLTIP = "This Duplicant is going totally bananas on the {Target}!";
			}

			public class EQUIPDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class COOKDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class MUSHDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class PACIFYDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class RESCUEDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class RESCUEWORKSTATUS
			{
				public static LocString NAME = "Rescuing {Target}";

				public static LocString TOOLTIP = "This Duplicant is saving {Target} from certain peril!";
			}

			public class MOPDELIVERSTATUS
			{
				public static LocString NAME = GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = GENERIC_DELIVER.TOOLTIP;
			}

			public class DIGGING
			{
				public static LocString NAME = "Digging";

				public static LocString TOOLTIP = "This Duplicant is excavating raw resources";
			}

			public class EATING
			{
				public static LocString NAME = "Eating {Target}";

				public static LocString TOOLTIP = "This Duplicant is having a meal";
			}

			public class CLEANING
			{
				public static LocString NAME = "Cleaning {Target}";

				public static LocString TOOLTIP = "This Duplicant is cleaning the {Target}";
			}

			public class LIGHTWORKEFFICIENCYBONUS
			{
				public static LocString NAME = "Lit Workspace";

				public static LocString TOOLTIP = "Better visibility from the light is allowing this Duplicant to work " + GameUtil.GetFormattedPercent(DUPLICANTSTATS.LIGHT.LIGHT_WORK_EFFICIENCY_BONUS * 100f, GameUtil.TimeSlice.None) + " faster";
			}

			public class PICKINGUP
			{
				public static LocString NAME = "Picking up {Target}";

				public static LocString TOOLTIP = "This Duplicant is retrieving {Target}";
			}

			public class MOPPING
			{
				public static LocString NAME = "Mopping";

				public static LocString TOOLTIP = "This Duplicant is cleaning up a nasty spill";
			}

			public class ARTING
			{
				public static LocString NAME = "Decorating";

				public static LocString TOOLTIP = "This Duplicant is hard at work on their art";
			}

			public class MUSHING
			{
				public static LocString NAME = "Mushing {Item}";

				public static LocString TOOLTIP = "This Duplicant is cooking a {Item}";
			}

			public class COOKING
			{
				public static LocString NAME = "Cooking {Item}";

				public static LocString TOOLTIP = "This Duplicant is cooking up a tasty {Item}";
			}

			public class RESEARCHING
			{
				public static LocString NAME = "Researching {Tech}";

				public static LocString TOOLTIP = "This Duplicant is intently researching {Tech} technology";
			}

			public class STORING
			{
				public static LocString NAME = "Storing {Item}";

				public static LocString TOOLTIP = "This Duplicant is putting {Item} away in {Target}";
			}

			public class BUILDING
			{
				public static LocString NAME = "Building {Target}";

				public static LocString TOOLTIP = "This Duplicant is constructing a {Target}";
			}

			public class EQUIPPING
			{
				public static LocString NAME = "Equipping {Target}";

				public static LocString TOOLTIP = "This Duplicant is equipping a {Target}";
			}

			public class WARMINGUP
			{
				public static LocString NAME = "Warming up";

				public static LocString TOOLTIP = "This Duplicant got too cold and is trying to warm up";
			}

			public class GENERATINGPOWER
			{
				public static LocString NAME = "Generating power";

				public static LocString TOOLTIP = "This Duplicant is using the {Target} to produce electrical power";
			}

			public class HARVESTING
			{
				public static LocString NAME = "Harvesting {Target}";

				public static LocString TOOLTIP = "This Duplicant is gathering resources from a {Target}";
			}

			public class UPROOTING
			{
				public static LocString NAME = "Uprooting {Target}";

				public static LocString TOOLTIP = "This Duplicant is digging up a {Target}";
			}

			public class EMPTYING
			{
				public static LocString NAME = "Emptying {Target}";

				public static LocString TOOLTIP = "This Duplicant is removing materials from the {Target}";
			}

			public class TOGGLING
			{
				public static LocString NAME = "Change {Target} setting";

				public static LocString TOOLTIP = "This Duplicant is changing the {Target}'s setting";
			}

			public class DECONSTRUCTING
			{
				public static LocString NAME = "Deconstructing {Target}";

				public static LocString TOOLTIP = "This Duplicant is demolishing the {Target}";
			}

			public class DISINFECTING
			{
				public static LocString NAME = "Disinfecting {Target}";

				public static LocString TOOLTIP = "This Duplicant is disinfecting {Target}";
			}

			public class FABRICATING
			{
				public static LocString NAME = "Fabricating {Target}";

				public static LocString TOOLTIP = "This Duplicant is crafting a {Target}";
			}

			public class PROCESSING
			{
				public static LocString NAME = "Refining {Target}";

				public static LocString TOOLTIP = "This Duplicant is refining {Target}";
			}

			public class CLEARING
			{
				public static LocString NAME = "Sweeping {Target}";

				public static LocString TOOLTIP = "This Duplicant is sweeping away {Target}";
			}

			public class STUDYING
			{
				public static LocString NAME = "Analyzing";

				public static LocString TOOLTIP = "This Duplicant is conducting a field study of a Natural Feature";
			}

			public class SOCIALIZING
			{
				public static LocString NAME = "Socializing";

				public static LocString TOOLTIP = "This Duplicant is using their break to hang out";
			}

			public class MINGLING
			{
				public static LocString NAME = "Mingling";

				public static LocString TOOLTIP = "This Duplicant is using their break to chat with friends";
			}

			public class DANCING
			{
				public static LocString NAME = "Dancing!";

				public static LocString TOOLTIP = "This Duplicant is getting down with their bad self";
			}

			public class GAMING
			{
				public static LocString NAME = "Playing a Game";

				public static LocString TOOLTIP = "This Duplicant is playing a video game\n\nIt looks like fun";
			}

			public class NOISEPEACEFUL
			{
				public static LocString NAME = "Peace and Quiet";

				public static LocString TOOLTIP = "This Duplicant has found a quiet place to concentrate";
			}

			public class NOISEMINOR
			{
				public static LocString NAME = "Loud Noises";

				public static LocString TOOLTIP = "This area is a bit too loud for comfort";
			}

			public class NOISEMAJOR
			{
				public static LocString NAME = "Cacophony!";

				public static LocString TOOLTIP = "It's very, very loud in here!";
			}

			public class LOWIMMUNITY
			{
				public static LocString NAME = "Under the Weather";

				public static LocString TOOLTIP = "This Duplicant has a weakened immune system and will become ill if it reaches zero";

				public static LocString NOTIFICATION_NAME = "Low Immunity";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are at risk of becoming sick:";
			}

			public abstract class TINKERING
			{
				public static LocString NAME = "Tinkering";

				public static LocString TOOLTIP = "This Duplicant is making functional improvements to a building";
			}

			public class CONTACTWITHGERMS
			{
				public static LocString NAME = "Contact with {Sickness} Germs";

				public static LocString TOOLTIP = "This Duplicant has come into contact with {Sickness} Germs and is at risk of exposure if contact continues";
			}

			public class EXPOSEDTOGERMS
			{
				public static LocString TIER1 = "Mild ";

				public static LocString TIER2 = "Medium ";

				public static LocString TIER3 = "";

				public static readonly LocString[] EXPOSURE_TIERS = new LocString[3]
				{
					TIER1,
					TIER2,
					TIER3
				};

				public static LocString NAME = "{Severity}Exposure to {Sickness} Germs";

				public static LocString TOOLTIP = "This Duplicant has been exposed to a concentration of {Sickness} Germs and is at risk of waking up sick on their next shift\n\nExposed {Source}\n\nRate of Contracting {Sickness}: {Chance}\n\nResistance Rating: {Total}\n    • Base {Sickness} Resistance: {Base}\n    • " + ATTRIBUTES.GERMRESISTANCE.NAME + ": {Dupe}\n    • {Severity} Exposure: {ExposureLevelBonus}\n\n<i>Click to jump to last exposure location</i>";
			}
		}

		public class DISEASES
		{
			public class STATUS_ITEM_TOOLTIP
			{
				public static LocString TEMPLATE = "{InfectionSource}{Duration}{Doctor}{Fatality}{Cures}{Bedrest}" + UI.HORIZONTAL_BR_RULE + "\n{Symptoms}";

				public static LocString DESCRIPTOR = "<b>{0} {1}</b>\n";

				public static LocString SYMPTOMS = "{0}\n";

				public static LocString INFECTION_SOURCE = "Contracted disease: {0}\n";

				public static LocString DURATION = "Time to recovery: {0}\n";

				public static LocString CURES = "Remedies taken: {0}\n";

				public static LocString NOMEDICINETAKEN = "Remedies taken: None\n";

				public static LocString FATALITY = "Fatal if untreated in: {0}\n";

				public static LocString BEDREST = "Sick Bay assignment will allow faster recovery\n";

				public static LocString DOCTOR_REQUIRED = "Sick Bay assignment required for recovery\n";

				public static LocString DOCTORED = "Received medical treatment, recovery speed is increased\n";
			}

			public class MEDICINE
			{
				public static LocString CURES_ANY = "Alleviates " + UI.FormatAsLink("All Diseases", "DISEASE");

				public static LocString CURES_ANY_TOOLTIP = "This is a nonspecific disease treatment that can be taken by any sick Duplicant";

				public static LocString CURES = "Alleviates {0}";

				public static LocString CURES_TOOLTIP = "This medicine is used to treat {0} and can only be taken by sick Duplicants";

				public static LocString BOOSTER = UI.FormatAsLink("Immune Booster", "IMMUNE SYSTEM");

				public static LocString BOOSTER_TOOLTIP = "Boosters can be taken by both healthy and sick Duplicants to prevent potential disease";
			}

			public class SEVERITY
			{
				public static LocString BENIGN = "Benign";

				public static LocString MINOR = "Minor";

				public static LocString MAJOR = "Major";

				public static LocString CRITICAL = "Critical";
			}

			public class TYPE
			{
				public static LocString PATHOGEN = "Illness";

				public static LocString AILMENT = "Ailment";

				public static LocString INJURY = "Injury";
			}

			public class TRIGGERS
			{
				public class TOOLTIPS
				{
					public static LocString EATCOMPLETEEDIBLE = "May cause {Diseases}";
				}

				public static LocString EATCOMPLETEEDIBLE = "May cause {Diseases}";
			}

			public class INFECTIONSOURCES
			{
				public static LocString INTERNAL_TEMPERATURE = "from extreme internal temperatures";

				public static LocString TOXIC_AREA = "through exposure to toxic areas";

				public static LocString FOOD = "by eating a germ-covered {0}";

				public static LocString AIR = "by breathing germ-filled {0}";

				public static LocString SKIN = "through skin contamination";

				public static LocString UNKNOWN = "from unknown source";
			}

			public class DESCRIPTORS
			{
				public class INFO
				{
					public static LocString FOODBORNE = "Contracted via ingestion\n" + UI.HORIZONTAL_RULE;

					public static LocString FOODBORNE_TOOLTIP = "Duplicants may contract this disease by ingesting food contaminated with these germs";

					public static LocString AIRBORNE = "Contracted via inhalation\n" + UI.HORIZONTAL_RULE;

					public static LocString AIRBORNE_TOOLTIP = "Duplicants may contract this disease by breathing air containing these germs";

					public static LocString SKINBORNE = "Contracted via physical contact\n" + UI.HORIZONTAL_RULE;

					public static LocString SKINBORNE_TOOLTIP = "Duplicants may contract this disease by touching objects contaminated with these germs";

					public static LocString SUNBORNE = "Contracted via environmental exposure\n" + UI.HORIZONTAL_RULE;

					public static LocString SUNBORNE_TOOLTIP = "Duplicants may contract this disease through exposure to hazardous environments";

					public static LocString GROWS_ON = "Multiplies in:";

					public static LocString GROWS_ON_TOOLTIP = "These substances allow germs to spread and reproduce";

					public static LocString NEUTRAL_ON = "Survives in:";

					public static LocString NEUTRAL_ON_TOOLTIP = "Germs will survive contact with these substances, but will not reproduce";

					public static LocString DIES_SLOWLY_ON = "Inhibited by:";

					public static LocString DIES_SLOWLY_ON_TOOLTIP = "Contact with these substances will slowly reduce germ numbers";

					public static LocString DIES_ON = "Killed by:";

					public static LocString DIES_ON_TOOLTIP = "Contact with these substances kills germs over time";

					public static LocString DIES_QUICKLY_ON = "Disinfected by:";

					public static LocString DIES_QUICKLY_ON_TOOLTIP = "Contact with these substances will quickly kill these germs";

					public static LocString GROWS = "Multiplies";

					public static LocString GROWS_TOOLTIP = "Doubles germ count every {0}";

					public static LocString NEUTRAL = "Survives";

					public static LocString NEUTRAL_TOOLTIP = "Germ count remains static";

					public static LocString DIES_SLOWLY = "Inhibited";

					public static LocString DIES_SLOWLY_TOOLTIP = "Halves germ count every {0}";

					public static LocString DIES = "Dies";

					public static LocString DIES_TOOLTIP = "Halves germ count every {0}";

					public static LocString DIES_QUICKLY = "Disinfected";

					public static LocString DIES_QUICKLY_TOOLTIP = "Halves germ count every {0}";

					public static LocString GROWTH_FORMAT = "    • {0}";

					public static LocString TEMPERATURE_RANGE = "Temperature range: {0} to {1}";

					public static LocString TEMPERATURE_RANGE_TOOLTIP = "These germs can survive temperatures between {0} and {1}" + UI.HORIZONTAL_BR_RULE + "They thrive in temperatures between {2} and {3}";

					public static LocString PRESSURE_RANGE = "Pressure range: {0} to {1}\n";

					public static LocString PRESSURE_RANGE_TOOLTIP = "These germs can survive between {0} and {1} of pressure" + UI.HORIZONTAL_BR_RULE + "They thrive in pressures between {2} and {3}";
				}
			}

			public class ALLDISEASES
			{
				public static LocString NAME = "All Diseases";
			}

			public class NODISEASES
			{
				public static LocString NAME = "NO";
			}

			public class FOODPOISONING
			{
				public static LocString NAME = UI.FormatAsLink("Food Poisoning", "FOODPOISONING");

				public static LocString LEGEND_HOVERTEXT = "Food Poisoning Germs present\n";
			}

			public class SLIMELUNG
			{
				public static LocString NAME = UI.FormatAsLink("Slimelung", "SLIMELUNG");

				public static LocString LEGEND_HOVERTEXT = "Slimelung Germs present\n";
			}

			public class POLLENGERMS
			{
				public static LocString NAME = UI.FormatAsLink("Floral Scent", "POLLENGERMS");

				public static LocString LEGEND_HOVERTEXT = "Floral Scent allergens present\n";
			}

			public class ZOMBIESPORES
			{
				public static LocString NAME = UI.FormatAsLink("Zombie Spores", "ZOMBIESPORES");

				public static LocString LEGEND_HOVERTEXT = "Zombie Spores present\n";
			}

			public class FOODSICKNESS
			{
				public static LocString NAME = UI.FormatAsLink("Food Poisoning", "FOODSICKNESS");

				public static LocString DESCRIPTION = "This Duplicant's last meal wasn't exactly food safe";

				public static LocString VOMIT_SYMPTOM = "Vomiting";

				public static LocString VOMIT_SYMPTOM_TOOLTIP = "Duplicants periodically vomit throughout the day, producing additional germs and losing calories";

				public static LocString DESCRIPTIVE_SYMPTOMS = "Nonlethal. A Duplicant's body \"purges\" from both ends, causing extreme fatigue.";

				public static LocString DISEASE_SOURCE_DESCRIPTOR = "Currently infected with {2}.\n\nThis Duplicant will produce {1} when vomiting.";

				public static LocString DISEASE_SOURCE_DESCRIPTOR_TOOLTIP = "This Duplicant will vomit approximately every {0}" + UI.HORIZONTAL_BR_RULE + "Each time they vomit, they will release {1} and lose calories";
			}

			public class SLIMESICKNESS
			{
				public static LocString NAME = UI.FormatAsLink("Slimelung", "SLIMESICKNESS");

				public static LocString DESCRIPTION = "This Duplicant's chest congestion is making it difficult to breathe";

				public static LocString COUGH_SYMPTOM = "Coughing";

				public static LocString COUGH_SYMPTOM_TOOLTIP = "Duplicants periodically cough up Polluted Oxygen, producing additional germs";

				public static LocString DESCRIPTIVE_SYMPTOMS = "Lethal without medical treatment. Duplicants experience coughing and shortness of breath.";

				public static LocString DISEASE_SOURCE_DESCRIPTOR = "Currently infected with {2}.\n\nThis Duplicant will produce {1} when coughing.";

				public static LocString DISEASE_SOURCE_DESCRIPTOR_TOOLTIP = "This Duplicant will cough approximately every {0}" + UI.HORIZONTAL_BR_RULE + "Each time they cough, they will release {1}";
			}

			public class ZOMBIESICKNESS
			{
				public static LocString NAME = UI.FormatAsLink("Zombie Spores", "ZOMBIESICKNESS");

				public static LocString DESCRIPTIVE_SYMPTOMS = "Duplicants lose much of their motor control and experience extreme discomfort.";

				public static LocString DESCRIPTION = "Fungal spores have infiltrated the Duplicant's head and are sending unnatural electrical impulses to their brain";

				public static LocString LEGEND_HOVERTEXT = "Area Causes Zombie Spores\n";
			}

			public class ALLERGIES
			{
				public static LocString NAME = UI.FormatAsLink("Allergic Reaction", "ALLERGIES");

				public static LocString DESCRIPTIVE_SYMPTOMS = "Allergens cause excessive sneezing fits";

				public static LocString DESCRIPTION = "Pollen and other irritants are causing this poor Duplicant's immune system to overreact, resulting in needless sneezing and congestion";
			}

			public class COLDSICKNESS
			{
				public static LocString NAME = UI.FormatAsLink("Hypothermia", "COLDSICKNESS");

				public static LocString DESCRIPTIVE_SYMPTOMS = "Nonlethal. Duplicants experience extreme body heat loss causing chills and discomfort.";

				public static LocString DESCRIPTION = "This Duplicant's thought processes have been slowed to a crawl from extreme cold exposure";

				public static LocString LEGEND_HOVERTEXT = "Area Causes Hypothermia\n";
			}

			public class SUNBURNSICKNESS
			{
				public static LocString NAME = UI.FormatAsLink("Sunburn", "SUNBURNSICKNESS");

				public static LocString DESCRIPTION = "Extreme sun exposure has given this Duplicant a nasty burn";

				public static LocString LEGEND_HOVERTEXT = "Area Causes Sunburn\n";

				public static LocString SUNEXPOSURE = "Sun Exposure";

				public static LocString DESCRIPTIVE_SYMPTOMS = "Nonlethal. Duplicants experience temporary discomfort due to dermatological damage.";
			}

			public class HEATSICKNESS
			{
				public static LocString NAME = UI.FormatAsLink("Heat Stroke", "HEATSICKNESS");

				public static LocString DESCRIPTIVE_SYMPTOMS = "Nonlethal. Duplicants experience high fever and discomfort.";

				public static LocString DESCRIPTION = "This Duplicant's thought processes have short circuited from extreme heat exposure";

				public static LocString LEGEND_HOVERTEXT = "Area Causes Heat Stroke\n";
			}

			public class PUTRIDODOUR
			{
				public static LocString NAME = UI.FormatAsLink("Trench Stench", "PUTRIDODOUR");

				public static LocString DESCRIPTION = "\nThe pungent odor wafting off this Duplicant is nauseating to their peers";

				public static LocString CRINGE_EFFECT = "Smelled a putrid odor";

				public static LocString LEGEND_HOVERTEXT = "Trench Stench Germs Present\n";
			}

			public static LocString CURED_POPUP = "Cured of {0}";

			public static LocString INFECTED_POPUP = "Became infected by {0}";

			public static LocString ADDED_POPFX = "{0}: {1} Germs";

			public static LocString NOTIFICATION_TOOLTIP = "{0} contracted {1} {2}";

			public static LocString GERMS = "Germs";

			public static LocString GERMS_CONSUMED_DESCRIPTION = "A count of the number of germs this Duplicant is host to";

			public static LocString RECUPERATING = "Recuperating";

			public static LocString INFECTION_MODIFIER = "Recently consumed {0} ({1})";

			public static LocString INFECTION_MODIFIER_SOURCE = "Fighting off {0} from {1}";

			public static LocString INFECTED_MODIFIER = "Suppressed immune system";

			public static LocString LEGEND_POSTAMBLE = "\n•  Select an infected object for more details";

			public static LocString ATTRIBUTE_MODIFIER_SYMPTOMS = "{0}: {1}";

			public static LocString ATTRIBUTE_MODIFIER_SYMPTOMS_TOOLTIP = "Modifies {0} by {1}";

			public static LocString DEATH_SYMPTOM = "Death in {0} if untreated";

			public static LocString DEATH_SYMPTOM_TOOLTIP = "Without medical treatment, this Duplicant will die of their illness in {0}";

			public static LocString RESISTANCES_PANEL_TOOLTIP = "{0}";

			public static LocString IMMUNE_FROM_MISSING_REQUIRED_TRAIT = "Immune: Does not have {0}";

			public static LocString IMMUNE_FROM_HAVING_EXLCLUDED_TRAIT = "Immune: Has {0}";

			public static LocString IMMUNE_FROM_HAVING_EXCLUDED_EFFECT = "Immunity: Has {0}";

			public static LocString CONTRACTION_PROBABILITY = "{0} of {1}'s exposures to these germs will result in {2}";
		}

		public class MODIFIERS
		{
			public class SKILLLEVEL
			{
				public static LocString NAME = "Skill Level";
			}

			public class ROOMLATRINE
			{
				public static LocString NAME = "Latrine";

				public static LocString TOOLTIP = "This Duplicant used an Outhouse in a Latrine";
			}

			public class ROOMBATHROOM
			{
				public static LocString NAME = "Washroom";

				public static LocString TOOLTIP = "This Duplicant used a Lavatory in a Washroom";
			}

			public class ROOMBARRACKS
			{
				public static LocString NAME = "Barracks";

				public static LocString TOOLTIP = "This Duplicant slept in the Barracks last night and feels refreshed";
			}

			public class ROOMBEDROOM
			{
				public static LocString NAME = "Bedroom";

				public static LocString TOOLTIP = "This Duplicant slept in a private Bedroom last night and feels extra refreshed";
			}

			public class BEDHEALTH
			{
				public static LocString NAME = "Bed Rest";

				public static LocString TOOLTIP = "This Duplicant will incrementally heal over while on Bed Rest";
			}

			public class BEDSTAMINA
			{
				public static LocString NAME = "Sleeping in a cot";

				public static LocString TOOLTIP = "This Duplicant's sleeping arrangements are adequate";
			}

			public class LUXURYBEDSTAMINA
			{
				public static LocString NAME = "Sleeping in a comfy bed";

				public static LocString TOOLTIP = "This Duplicant loves their snuggly bed";
			}

			public class BARRACKSSTAMINA
			{
				public static LocString NAME = "Barracks";

				public static LocString TOOLTIP = "This Duplicant shares sleeping quarters with others";
			}

			public class BEDROOMSTAMINA
			{
				public static LocString NAME = "Private Bedroom";

				public static LocString TOOLTIP = "This lucky Duplicant has their own private bedroom";
			}

			public class ROOMMESSHALL
			{
				public static LocString NAME = "Mess Hall";

				public static LocString TOOLTIP = "This Duplicant's most recent meal was eaten in a Mess Hall";
			}

			public class ROOMGREATHALL
			{
				public static LocString NAME = "Great Hall";

				public static LocString TOOLTIP = "This Duplicant's most recent meal was eaten in a fancy Great Hall";
			}

			public class ENTITLEMENT
			{
				public static LocString NAME = "Entitlement";

				public static LocString TOOLTIP = "Duplicants will demand better decor and accommodations with each Expertise level they gain";
			}

			public class BASEDUPLICANT
			{
				public static LocString NAME = "Duplicant";
			}

			public class HOMEOSTASIS
			{
				public static LocString NAME = "Homeostasis";
			}

			public class WARMAIR
			{
				public static LocString NAME = "Warm Air";
			}

			public class COLDAIR
			{
				public static LocString NAME = "Cold Air";
			}

			public class CLAUSTROPHOBIC
			{
				public static LocString NAME = "Claustrophobic";

				public static LocString TOOLTIP = "This Duplicant recently found themselves in an upsettingly cramped space";

				public static LocString CAUSE = "This Duplicant got so good at their job that they became claustrophobic";
			}

			public class VERTIGO
			{
				public static LocString NAME = "Vertigo";

				public static LocString TOOLTIP = "This Duplicant had to climb a tall ladder that left them dizzy and unsettled";

				public static LocString CAUSE = "This Duplicant got so good at their job they became bad at ladders";
			}

			public class UNCOMFORTABLEFEET
			{
				public static LocString NAME = "Aching Feet";

				public static LocString TOOLTIP = "This Duplicant recently walked across floor without tile, much to their chagrin";

				public static LocString CAUSE = "This Duplicant got so good at their job that their feet became sensitive";
			}

			public class PEOPLETOOCLOSEWHILESLEEPING
			{
				public static LocString NAME = "Personal Bubble Burst";

				public static LocString TOOLTIP = "This Duplicant had to sleep too close to others and it was awkward for them";

				public static LocString CAUSE = "This Duplicant got so good at their job that they stopped being comfortable sleeping near other people";
			}

			public class RESTLESS
			{
				public static LocString NAME = "Restless";

				public static LocString TOOLTIP = "This Duplicant went a few minutes without working and is now completely awash with guilt";

				public static LocString CAUSE = "This Duplicant got so good at their job that they forgot how to be comfortable doing anything else";
			}

			public class UNFASHIONABLECLOTHING
			{
				public static LocString NAME = "Fashion Crime";

				public static LocString TOOLTIP = "This Duplicant had to wear something that was an affront to fashion";

				public static LocString CAUSE = "This Duplicant got so good at their job that they became incapable of tolerating unfashionable clothing";
			}

			public class BURNINGCALORIES
			{
				public static LocString NAME = "Burning Calories";
			}

			public class EATINGCALORIES
			{
				public static LocString NAME = "Eating";
			}

			public class TEMPEXCHANGE
			{
				public static LocString NAME = "Environmental Exchange";
			}

			public class CLOTHING
			{
				public static LocString NAME = "Clothing";
			}

			public class CRYFACE
			{
				public static LocString NAME = "Cry Face";

				public static LocString TOOLTIP = "This Duplicant recently had a crying fit and it shows";

				public static LocString CAUSE = "Obtained from the Ugly Crier stress reaction";
			}

			public class SOILEDSUIT
			{
				public static LocString NAME = "Soiled Suit";

				public static LocString TOOLTIP = "This Duplicant's suit needs to be emptied of waste\n\nPreferably soon";

				public static LocString CAUSE = "Obtained when a Duplicant wears a suit filled with... \"fluids\"";
			}

			public class SHOWERED
			{
				public static LocString NAME = "Showered";

				public static LocString TOOLTIP = "This Duplicant recently had a shower and feels squeaky clean!";
			}

			public class SOREBACK
			{
				public static LocString NAME = "Sore Back";

				public static LocString TOOLTIP = "This Duplicant feels achy from sleeping on the floor last night and would like a bed";

				public static LocString CAUSE = "Obtained by sleeping on the ground";
			}

			public class GOODEATS
			{
				public static LocString NAME = "Soul Food";

				public static LocString TOOLTIP = "This Duplicant had a yummy home cooked meal and is totally stuffed";

				public static LocString CAUSE = "Obtained by eating a hearty home cooked meal";

				public static LocString DESCRIPTION = "Duplicants find this home cooked meal is emotionally comforting";
			}

			public class FRESH_AND_CLEAN
			{
				public static LocString NAME = "Refreshingly Clean";

				public static LocString TOOLTIP = "This Duplicant took a warm shower and it was great!";

				public static LocString CAUSE = "Obtained by taking a comfortably heated shower";
			}

			public class BURNED_BY_SCALDING_WATER
			{
				public static LocString NAME = "Scalded";

				public static LocString TOOLTIP = "Ouch! This Duplicant showered or was doused in water that was way too hot";

				public static LocString CAUSE = "Obtained by exposure to hot water";
			}

			public class STRESSED_BY_COLD_WATER
			{
				public static LocString NAME = "Numb";

				public static LocString TOOLTIP = "Brr! This Duplicant was showered or doused in water that was way too cold";

				public static LocString CAUSE = "Obtained by exposure to icy water";
			}

			public class SMELLEDSTINKY
			{
				public static LocString NAME = "Smelled Stinky";

				public static LocString TOOLTIP = "This Duplicant got a whiff of a certain somebody";
			}

			public class STRESSREDUCTION
			{
				public static LocString NAME = "Receiving Massage";

				public static LocString TOOLTIP = "This Duplicant's stress is just melting away";
			}

			public class STRESSREDUCTION_CLINIC
			{
				public static LocString NAME = "Receiving Clinic Massage";

				public static LocString TOOLTIP = "Clinical facilities are improving the effectiveness of this massage\n\nThis Duplicant's stress is just melting away";
			}

			public class UGLY_CRYING
			{
				public static LocString NAME = "Ugly Crying";

				public static LocString TOOLTIP = "This Duplicant is having a cathartic ugly cry as a result of stress";

				public static LocString NOTIFICATION_NAME = "Ugly Crying";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants became so " + UI.FormatAsLink("Stressed", "STRESS") + " they broke down crying:";
			}

			public class BINGE_EATING
			{
				public static LocString NAME = "Insatiable Hunger";

				public static LocString TOOLTIP = "This Duplicant is stuffing their face as a result of stress";

				public static LocString NOTIFICATION_NAME = "Binge Eating";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants became so " + UI.FormatAsLink("Stressed", "STRESS") + " they began overeating:";
			}

			public class METABOLISM_CALORIE_MODIFIER
			{
				public static LocString NAME = "Metabolism";

				public static LocString TOOLTIP = "Metabolism determines how quickly a critter burns calories";
			}

			public class WORKING
			{
				public static LocString NAME = "Working";

				public static LocString TOOLTIP = "This Duplicant is working up a sweat";
			}

			public class UNCOMFORTABLESLEEP
			{
				public static LocString NAME = "Sleeping Uncomfortably";

				public static LocString TOOLTIP = "This Duplicant collapsed on the floor from sheer exhaustion";
			}

			public class MANAGERIALDUTIES
			{
				public static LocString NAME = "Managerial Duties";

				public static LocString TOOLTIP = "Being a manager is stressful";
			}

			public class MANAGEDCOLONY
			{
				public static LocString NAME = "Managed Colony";

				public static LocString TOOLTIP = "A Duplicant is in the colony manager job";
			}

			public class FLOORSLEEP
			{
				public static LocString NAME = "Sleeping On Floor";

				public static LocString TOOLTIP = "This Duplicant is uncomfortably recovering stamina";
			}

			public class PASSEDOUTSLEEP
			{
				public static LocString NAME = "Exhausted";

				public static LocString TOOLTIP = "Lack of rest depleted this Duplicant's stamina\n\nThey passed out from the fatigue";
			}

			public class SLEEP
			{
				public static LocString NAME = "Sleeping";

				public static LocString TOOLTIP = "This Duplicant is recovering stamina";
			}

			public class RESTFULSLEEP
			{
				public static LocString NAME = "Sleeping Peacefully";

				public static LocString TOOLTIP = "This Duplicant is getting a good night's rest";
			}

			public class SLEEPY
			{
				public static LocString NAME = "Sleepy";

				public static LocString TOOLTIP = "This Duplicant is getting tired";
			}

			public class HUNGRY
			{
				public static LocString NAME = "Hungry";

				public static LocString TOOLTIP = "This Duplicant is ready for lunch";
			}

			public class STARVING
			{
				public static LocString NAME = "Starving";

				public static LocString TOOLTIP = "This Duplicant needs to eat something, soon";
			}

			public class HOT
			{
				public static LocString NAME = "Hot";

				public static LocString TOOLTIP = "This Duplicant is uncomfortably warm";
			}

			public class COLD
			{
				public static LocString NAME = "Cold";

				public static LocString TOOLTIP = "This Duplicant is uncomfortably cold";
			}

			public class WETFEET
			{
				public static LocString NAME = "Soggy Feet";

				public static LocString TOOLTIP = "This Duplicant recently stepped in liquid";

				public static LocString CAUSE = "Obtained by walking through liquid.";
			}

			public class SOAKINGWET
			{
				public static LocString NAME = "Sopping Wet";

				public static LocString TOOLTIP = "This Duplicant was recently submerged in liquid";

				public static LocString CAUSE = "Obtained from submergence in liquid.";
			}

			public class POPPEDEARDRUMS
			{
				public static LocString NAME = "Popped Eardrums";

				public static LocString TOOLTIP = "This Duplicant was exposed to an over-pressurized area that popped their eardrums";
			}

			public class ANEWHOPE
			{
				public static LocString NAME = "New Hope";

				public static LocString TOOLTIP = "This Duplicant feels pretty optimistic about their new home";
			}

			public class PRICKLEFRUITDAMAGE
			{
				public static LocString NAME = "Ouch!";

				public static LocString TOOLTIP = "This Duplicant ate a raw " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + " and it gave their mouth ouchies.";
			}

			public class NOOXYGEN
			{
				public static LocString NAME = "No Oxygen";

				public static LocString TOOLTIP = "There is no breathable air in this area";
			}

			public class LOWOXYGEN
			{
				public static LocString NAME = "Low Oxygen";

				public static LocString TOOLTIP = "The air is thin in this area";
			}

			public class MOURNING
			{
				public static LocString NAME = "Mourning";

				public static LocString TOOLTIP = "This Duplicant is grieving the loss of a friend";
			}

			public class NARCOLEPTICSLEEP
			{
				public static LocString NAME = "Narcoleptic Nap";

				public static LocString TOOLTIP = "This Duplicant just needs to rest their eyes for a second";
			}

			public class BADSLEEP
			{
				public static LocString NAME = "Unrested";

				public static LocString TOOLTIP = "This Duplicant tossed and turned all night because a light was left on where they were trying to sleep";
			}

			public class TERRIBLESLEEP
			{
				public static LocString NAME = "Dead Tired";

				public static LocString TOOLTIP = "This Duplicant didn't get any shuteye last night because of all the racket from a friend's snoring";
			}

			public class PEACEFULSLEEP
			{
				public static LocString NAME = "Well Rested";

				public static LocString TOOLTIP = "This Duplicant had a blissfully quiet sleep last night";
			}

			public class CENTEROFATTENTION
			{
				public static LocString NAME = "Center of Attention";

				public static LocString TOOLTIP = "This Duplicant feels like someone's watching over them...";
			}

			public class INSPIRED
			{
				public static LocString NAME = "Inspired";

				public static LocString TOOLTIP = "This Duplicant has had a creative vision!";
			}

			public class NEWCREWARRIVAL
			{
				public static LocString NAME = "New Friend";

				public static LocString TOOLTIP = "This Duplicant is happy to see a new face in the colony";
			}

			public class UNDERWATER
			{
				public static LocString NAME = "Underwater";

				public static LocString TOOLTIP = "This Duplicant's movement is slowed";
			}

			public class NIGHTMARES
			{
				public static LocString NAME = "Nightmares";

				public static LocString TOOLTIP = "This Duplicant was visited by something in the night";
			}

			public class WASATTACKED
			{
				public static LocString NAME = "Recently assailed";

				public static LocString TOOLTIP = "This Duplicant is stressed out after having been attacked";
			}

			public class LIGHTWOUNDS
			{
				public static LocString NAME = "Light Wounds";

				public static LocString TOOLTIP = "This Duplicant sustained injuries that are a bit uncomfortable";
			}

			public class MODERATEWOUNDS
			{
				public static LocString NAME = "Moderate Wounds";

				public static LocString TOOLTIP = "This Duplicant sustained injuries that are affecting their ability to work";
			}

			public class SEVEREWOUNDS
			{
				public static LocString NAME = "Severe Wounds";

				public static LocString TOOLTIP = "This Duplicant sustained serious injuries that are impacting their work and well-being";
			}

			public class ROTTEMPERATURE
			{
				public static LocString NAME = "Unrefrigerated";
			}

			public class ROTATMOSPHERE
			{
				public static LocString NAME = "Contaminated Air";
			}

			public class BASEROT
			{
				public static LocString NAME = "Base Decay Rate";
			}

			public class FULLBLADDER
			{
				public static LocString NAME = "Full Bladder";

				public static LocString TOOLTIP = "This Duplicant's bladder is full";
			}

			public class DIARRHEA
			{
				public static LocString NAME = "Diarrhea";

				public static LocString TOOLTIP = "This Duplicant's gut is giving them some trouble";

				public static LocString CAUSE = "Obtained by eating a disgusting meal";

				public static LocString DESCRIPTION = "Most Duplicants experience stomach upset from this meal";
			}

			public class STRESSFULYEMPTYINGBLADDER
			{
				public static LocString NAME = "Making a mess";

				public static LocString TOOLTIP = "This Duplicant had no choice but to to empty their bladder";
			}

			public class REDALERT
			{
				public static LocString NAME = "Red Alert!";

				public static LocString TOOLTIP = "The Red Alert is stressing this Duplicant out";
			}

			public class FUSSY
			{
				public static LocString NAME = "Fussy";

				public static LocString TOOLTIP = "This Duplicant is hard to please";
			}

			public class WARMINGUP
			{
				public static LocString NAME = "Warming Up";

				public static LocString TOOLTIP = "This Duplicant is trying to warm back up";
			}

			public class COOLINGDOWN
			{
				public static LocString NAME = "Cooling Down";

				public static LocString TOOLTIP = "This Duplicant is trying to cool back down";
			}

			public class DARKNESS
			{
				public static LocString NAME = "Darkness";

				public static LocString TOOLTIP = "Eep! This Duplicant doesn't like being in the dark!";
			}

			public class STEPPEDINCONTAMINATEDWATER
			{
				public static LocString NAME = "Stepped in polluted water";

				public static LocString TOOLTIP = "Gross! This Duplicant stepped in something yucky";
			}

			public class WELLFED
			{
				public static LocString NAME = "Well fed";

				public static LocString TOOLTIP = "This Duplicant feels satisfied after having a big meal";
			}

			public class STALEFOOD
			{
				public static LocString NAME = "Bad leftovers";

				public static LocString TOOLTIP = "This Duplicant is in a bad mood from having to eat stale food";
			}

			public class SMELLEDPUTRIDODOUR
			{
				public static LocString NAME = "Smelled a putrid odor";

				public static LocString TOOLTIP = "This Duplicant got a whiff of something unspeakably foul";
			}

			public class VOMITING
			{
				public static LocString NAME = "Vomiting";

				public static LocString TOOLTIP = "Better out than in, as they say";
			}

			public class BREATHING
			{
				public static LocString NAME = "Breathing";
			}

			public class HOLDINGBREATH
			{
				public static LocString NAME = "Holding breath";
			}

			public class RECOVERINGBREATH
			{
				public static LocString NAME = "Recovering breath";
			}

			public class ROTTING
			{
				public static LocString NAME = "Rotting";
			}

			public class DEAD
			{
				public static LocString NAME = "Dead";
			}

			public class TOXICENVIRONMENT
			{
				public static LocString NAME = "Toxic environment";
			}

			public class RESTING
			{
				public static LocString NAME = "Resting";
			}

			public class INTRAVENOUS_NUTRITION
			{
				public static LocString NAME = "Intravenous Feeding";
			}

			public class CATHETERIZED
			{
				public static LocString NAME = "Catheterized";

				public static LocString TOOLTIP = "Let's leave it at that";
			}

			public class NOISEPEACEFUL
			{
				public static LocString NAME = "Peace and Quiet";

				public static LocString TOOLTIP = "This Duplicant has found a quiet place to concentrate";
			}

			public class NOISEMINOR
			{
				public static LocString NAME = "Loud Noises";

				public static LocString TOOLTIP = "This area is a bit too loud for comfort";
			}

			public class NOISEMAJOR
			{
				public static LocString NAME = "Cacophony!";

				public static LocString TOOLTIP = "It's very, very loud in here!";
			}

			public class MEDICALCOT
			{
				public static LocString NAME = "Triage Cot Rest";

				public static LocString TOOLTIP = "Bedrest is improving this Duplicant's physical recovery time";
			}

			public class MEDICALCOTDOCTORED
			{
				public static LocString NAME = "Receiving treatment";

				public static LocString TOOLTIP = "This Duplicant is receiving treatment for their physical injuries";
			}

			public class DOCTOREDOFFCOTEFFECT
			{
				public static LocString NAME = "Runaway Patient";

				public static LocString TOOLTIP = "Tsk tsk!\nThis Duplicant cannot receive treatment while out of their medical bed!";
			}

			public class POSTDISEASERECOVERY
			{
				public static LocString NAME = "Feeling better";

				public static LocString TOOLTIP = "This Duplicant is up and about, but they still have some lingering effects from their illness";

				public static LocString ADDITIONAL_EFFECTS = "This Duplicant has temporary immunity to diseases from having beaten an infection";
			}

			public class IMMUNESYSTEMOVERWHELMED
			{
				public static LocString NAME = "Immune System Overwhelmed";

				public static LocString TOOLTIP = "This Duplicant's immune system is slowly being overwhelmed by a high concentration of germs";
			}

			public class MEDICINE_GENERICPILL
			{
				public static LocString NAME = "Placebo";

				public static LocString TOOLTIP = ITEMS.PILLS.PLACEBO.DESC;

				public static LocString EFFECT_DESC = "Applies the <style=\"disease\">{0}</style> effect";
			}

			public class MEDICINE_BASICBOOSTER
			{
				public static LocString NAME = ITEMS.PILLS.BASICBOOSTER.NAME;

				public static LocString TOOLTIP = ITEMS.PILLS.BASICBOOSTER.DESC;
			}

			public class MEDICINE_INTERMEDIATEBOOSTER
			{
				public static LocString NAME = ITEMS.PILLS.INTERMEDIATEBOOSTER.NAME;

				public static LocString TOOLTIP = ITEMS.PILLS.INTERMEDIATEBOOSTER.DESC;
			}

			public class SUNLIGHT_PLEASANT
			{
				public static LocString NAME = "Bright and Cheerful";

				public static LocString TOOLTIP = "The strong natural light is making this Duplicant feel light on their feet";
			}

			public class SUNLIGHT_BURNING
			{
				public static LocString NAME = "Intensely Bright";

				public static LocString TOOLTIP = "The bright light is significantly improving this Duplicant's mood, but prolonged exposure may result in burning";
			}

			public class TOOKABREAK
			{
				public static LocString NAME = "Downtime";

				public static LocString TOOLTIP = "This Duplicant has a bit of time off from work to attend to personal matters";
			}

			public class SOCIALIZED
			{
				public static LocString NAME = "Socialized";

				public static LocString TOOLTIP = "This Duplicant had some free time to hang out with buddies";
			}

			public class GOODCONVERSATION
			{
				public static LocString NAME = "Pleasant Chitchat";

				public static LocString TOOLTIP = "This Duplicant recently had a chance to chat with a friend";
			}

			public class WORKENCOURAGED
			{
				public static LocString NAME = "Appreciated";

				public static LocString TOOLTIP = "Someone saw how hard this Duplicant was working and gave them a compliment\n\nThis Duplicant feels great about themselves now!";
			}

			public class GREETING
			{
				public static LocString NAME = "Saw Friend";

				public static LocString TOOLTIP = "This Duplicant recently saw a friend in the halls and got to say \"hi\"\n\nIt wasn't even awkward!";
			}

			public class PLAYEDARCADE
			{
				public static LocString NAME = "Played Video Games";

				public static LocString TOOLTIP = "This Duplicant recently got to play some fun video games!";
			}

			public class DANCED
			{
				public static LocString NAME = "Recently Danced";

				public static LocString TOOLTIP = "This Duplicant had a chance to cut loose!\n\nLeisure activities increase Duplicants' Morale";
			}

			public class ESPRESSO
			{
				public static LocString NAME = "Drank Espresso";

				public static LocString TOOLTIP = "This Duplicant had delicious drink!\n\nLeisure activities increase Duplicants' Morale";
			}

			public class EDIBLEMINUS3
			{
				public static LocString NAME = "Grisly Meal";

				public static LocString TOOLTIP = "The food this Duplicant last ate was Grisly\n\nThey hope their next meal will be better";
			}

			public class EDIBLEMINUS2
			{
				public static LocString NAME = "Terrible Meal";

				public static LocString TOOLTIP = "The food this Duplicant last ate was Terrible\n\nThey hope their next meal will be better";
			}

			public class EDIBLEMINUS1
			{
				public static LocString NAME = "Poor Meal";

				public static LocString TOOLTIP = "The food this Duplicant last ate was Poor\n\nThey hope their next meal will be a little better";
			}

			public class EDIBLE0
			{
				public static LocString NAME = "Standard Meal";

				public static LocString TOOLTIP = "The food this Duplicant last ate was Average\n\nThey thought it was sort of okay";
			}

			public class EDIBLE1
			{
				public static LocString NAME = "Good Meal";

				public static LocString TOOLTIP = "The food this Duplicant last ate was Good\n\nThey thought it was pretty good!";
			}

			public class EDIBLE2
			{
				public static LocString NAME = "Great Meal";

				public static LocString TOOLTIP = "The food this Duplicant last ate was Great\n\nThey thought it was pretty good!";
			}

			public class EDIBLE3
			{
				public static LocString NAME = "Superb Meal";

				public static LocString TOOLTIP = "The food this Duplicant last ate was Superb\n\nThey thought it was really good!";
			}

			public class EDIBLE4
			{
				public static LocString NAME = "Ambrosial Meal";

				public static LocString TOOLTIP = "The food this Duplicant last ate was Ambrosial\n\nThey thought it was super tasty!";
			}

			public class DECORMINUS1
			{
				public static LocString NAME = "Last Cycle's Decor: Ugly";

				public static LocString TOOLTIP = "This Duplicant thought the overall decor yesterday was downright depressing";
			}

			public class DECOR0
			{
				public static LocString NAME = "Last Cycle's Decor: Poor";

				public static LocString TOOLTIP = "This Duplicant thought the overall decor yesterday was quite poor";
			}

			public class DECOR1
			{
				public static LocString NAME = "Last Cycle's Decor: Mediocre";

				public static LocString TOOLTIP = "This Duplicant had no strong opinions about the colony's decor yesterday";
			}

			public class DECOR2
			{
				public static LocString NAME = "Last Cycle's Decor: Average";

				public static LocString TOOLTIP = "This Duplicant thought the overall decor yesterday was pretty alright";
			}

			public class DECOR3
			{
				public static LocString NAME = "Last Cycle's Decor: Nice";

				public static LocString TOOLTIP = "This Duplicant thought the overall decor yesterday was quite nice!";
			}

			public class DECOR4
			{
				public static LocString NAME = "Last Cycle's Decor: Charming";

				public static LocString TOOLTIP = "This Duplicant thought the overall decor yesterday was downright charming!";
			}

			public class DECOR5
			{
				public static LocString NAME = "Last Cycle's Decor: Gorgeous";

				public static LocString TOOLTIP = "This Duplicant thought the overall decor yesterday was fantastic\n\nThey love what I've done with the place!";
			}

			public class BREAK1
			{
				public static LocString NAME = "One Shift Break";

				public static LocString TOOLTIP = "This Duplicant has had one Downtime shift in the last cycle";
			}

			public class BREAK2
			{
				public static LocString NAME = "Two Shift Break";

				public static LocString TOOLTIP = "This Duplicant has had two Downtime shifts in the last cycle";
			}

			public class BREAK3
			{
				public static LocString NAME = "Three Shift Break";

				public static LocString TOOLTIP = "This Duplicant has had three Downtime shifts in the last cycle";
			}

			public class BREAK4
			{
				public static LocString NAME = "Four Shift Break";

				public static LocString TOOLTIP = "This Duplicant has had four Downtime shifts in the last cycle";
			}

			public class BREAK5
			{
				public static LocString NAME = "Five Shift Break";

				public static LocString TOOLTIP = "This Duplicant has had five Downtime shifts in the last cycle";
			}

			public class POWERTINKER
			{
				public static LocString NAME = "Engie's Tune-Up";

				public static LocString TOOLTIP = "A skilled Duplicant has improved this generator's power output efficiency" + UI.HORIZONTAL_BR_RULE + "Applying this effect consumed one Microchip";
			}

			public class FARMTINKER
			{
				public static LocString NAME = "Farmer's Touch";

				public static LocString TOOLTIP = "A skilled Duplicant has encouraged this plant to grow a little bit faster" + UI.HORIZONTAL_BR_RULE + "Applying this effect consumed one dose of Micronutrient Fertilizer";
			}

			public class MACHINETINKER
			{
				public static LocString NAME = "Engie's Jerry Rig";

				public static LocString TOOLTIP = "A skilled Duplicant has jerry rigged this generator to temporarily run faster";
			}

			public class SPACETOURIST
			{
				public static LocString NAME = "Visited Space";

				public static LocString TOOLTIP = "This Duplicant went on a trip to space and saw the wonders of the universe";
			}

			public class SUDDENMORALEHELPER
			{
				public static LocString NAME = "Morale Upgrade Helper";

				public static LocString TOOLTIP = "This Duplicant will receive a temporary Morale bonus to buffer the new Morale system introduction";
			}

			public class EXPOSEDTOFOODGERMS
			{
				public static LocString NAME = "Food Poisoning Exposure";

				public static LocString TOOLTIP = "This Duplicant was exposed to Food Poisoning Germs and is at risk of developing the disease";
			}

			public class EXPOSEDTOSLIMEGERMS
			{
				public static LocString NAME = "Slimelung Exposure";

				public static LocString TOOLTIP = "This Duplicant was exposed to Slimelung Germs and is at risk of developing the disease";
			}

			public class EXPOSEDTOZOMBIESPORES
			{
				public static LocString NAME = "Zombie Spores Exposure";

				public static LocString TOOLTIP = "This Duplicant was exposed to Zombie Spores and is at risk of developing the disease";
			}

			public class FEELINGSICKFOODGERMS
			{
				public static LocString NAME = "Contracted: Food Poisoning";

				public static LocString TOOLTIP = "This Duplicant contracted Food Poisoning after a recent germ exposure and will begin exhibiting symptoms shortly";
			}

			public class FEELINGSICKSLIMEGERMS
			{
				public static LocString NAME = "Contracted: Slimelung";

				public static LocString TOOLTIP = "This Duplicant contracted Slimelung after a recent germ exposure and will begin exhibiting symptoms shortly";
			}

			public class FEELINGSICKZOMBIESPORES
			{
				public static LocString NAME = "Contracted: Zombie Spores";

				public static LocString TOOLTIP = "This Duplicant contracted Zombie Spores after a recent germ exposure and will begin exhibiting symptoms shortly";
			}

			public class SMELLEDFLOWERS
			{
				public static LocString NAME = "Smelled Flowers";

				public static LocString TOOLTIP = "A pleasant floral scent wafted over this Duplicant and brightened their day";
			}

			public class HISTAMINESUPPRESSION
			{
				public static LocString NAME = "Antihistamines";

				public static LocString TOOLTIP = "This Duplicant's allergic reactions have been suppressed by medication";
			}

			public class FOODSICKNESSRECOVERY
			{
				public static LocString NAME = "Food Poisoning Antibodies";

				public static LocString TOOLTIP = "This Duplicant recently recovered from Food Poisoning and is temporarily immune to the disease";
			}

			public class SLIMESICKNESSRECOVERY
			{
				public static LocString NAME = "Slimelung Antibodies";

				public static LocString TOOLTIP = "This Duplicant recently recovered from Slimelung and is temporarily immune to the disease";
			}

			public class ZOMBIESICKNESSRECOVERY
			{
				public static LocString NAME = "Zombie Spore Antibodies";

				public static LocString TOOLTIP = "This Duplicant recently recovered from Zombie Spores and is temporarily immune to the disease";
			}

			public class MESSTABLESALT
			{
				public static LocString NAME = "Salted Food";

				public static LocString TOOLTIP = "This Duplicant had the luxury of using Table Salt with their last meal at a Mess Table";
			}

			public static LocString MODIFIER_FORMAT = "{0}: {1}";

			public static LocString TIME_REMAINING = "\nTime Remaining: {0}";

			public static LocString TIME_TOTAL = "\nDuration: {0}";
		}

		public class CONGENITALTRAITS
		{
			public class NONE
			{
				public static LocString NAME = "None";

				public static LocString DESC = "This Duplicant seems pretty average overall";
			}

			public class JOSHUA
			{
				public static LocString NAME = "Cheery Disposition";

				public static LocString DESC = "This Duplicant brightens others' days wherever he goes";
			}

			public class ELLIE
			{
				public static LocString NAME = "Fastidious";

				public static LocString DESC = "This Duplicant needs things done in a very particular way";
			}

			public class LIAM
			{
				public static LocString NAME = "Germaphobe";

				public static LocString DESC = "This Duplicant has an all-consuming fear of bacteria";
			}

			public class BANHI
			{
				public static LocString NAME = "";

				public static LocString DESC = "";
			}

			public class STINKY
			{
				public static LocString NAME = "Stinkiness";

				public static LocString DESC = "This Duplicant is genetically cursed by a pungent bodily odor";
			}
		}

		public class TRAITS
		{
			public class NEEDS
			{
				public class CLAUSTROPHOBIC
				{
					public static LocString NAME = "Claustrophobic";

					public static LocString DESC = "This Duplicant feels suffocated in spaces less than four tiles high or three tiles wide";
				}

				public class FASHIONABLE
				{
					public static LocString NAME = "Fashionista";

					public static LocString DESC = "This Duplicant dies a bit inside when forced to wear unstylish clothing";
				}

				public class CLIMACOPHOBIC
				{
					public static LocString NAME = "Vertigo Prone";

					public static LocString DESC = "Climbing ladders more than four tiles tall makes this Duplicant's stomach do flips";
				}

				public class SOLITARYSLEEPER
				{
					public static LocString NAME = "Solitary Sleeper";

					public static LocString DESC = "This Duplicant prefers to sleep alone";
				}

				public class PREFERSWARMER
				{
					public static LocString NAME = "Skinny";

					public static LocString DESC = "This Duplicant doesn't have much insulation, so they are more temperature sensitive than others";
				}

				public class PREFERSCOOLER
				{
					public static LocString NAME = "Pudgy";

					public static LocString DESC = "This Duplicant has some extra insulation, so the room temperature affects them a little less";
				}

				public class SENSITIVEFEET
				{
					public static LocString NAME = "Delicate Feetsies";

					public static LocString DESC = "This Duplicant is a sensitive sole and would rather walk on tile than raw bedrock";
				}

				public class WORKAHOLIC
				{
					public static LocString NAME = "Workaholic";

					public static LocString DESC = "This Duplicant gets antsy when left idle";
				}
			}

			public class CANTRESEARCH
			{
				public static LocString NAME = "Yokel";

				public static LocString DESC = "This Duplicant isn't the brightest star in the solar system";
			}

			public class CANTBUILD
			{
				public static LocString NAME = "Unconstructive";

				public static LocString DESC = "This Duplicant is incapable of building even the most basic of structures";
			}

			public class CANTCOOK
			{
				public static LocString NAME = "Gastrophobia";

				public static LocString DESC = "This Duplicant has a deep-seated distrust of the culinary arts";
			}

			public class CANTDIG
			{
				public static LocString NAME = "Trypophobia";

				public static LocString DESC = "This Duplicant's fear of holes makes it impossible for them to dig";
			}

			public class HEMOPHOBIA
			{
				public static LocString NAME = "Squeamish";

				public static LocString DESC = "This Duplicant is of delicate disposition and cannot tend to the sick";
			}

			public class BEDSIDEMANNER
			{
				public static LocString NAME = "Caregiver";

				public static LocString DESC = "This Duplicant has good bedside manner and a healing touch";
			}

			public class MOUTHBREATHER
			{
				public static LocString NAME = "Mouth Breather";

				public static LocString DESC = "This Duplicant sucks up way more than their fair share of " + ELEMENTS.OXYGEN.NAME;
			}

			public class FUSSY
			{
				public static LocString NAME = "Fussy";

				public static LocString DESC = "Nothing's ever quite good enough for this Duplicant";
			}

			public class TWINKLETOES
			{
				public static LocString NAME = "Twinkletoes";

				public static LocString DESC = "This Duplicant is light as a feather on their feet";
			}

			public class STRONGARM
			{
				public static LocString NAME = "Buff";

				public static LocString DESC = "This Duplicant has muscles on their muscles";
			}

			public class NOODLEARMS
			{
				public static LocString NAME = "Noodle Arms";

				public static LocString DESC = "This Duplicant's arms have all the tensile strength of overcooked linguine";
			}

			public class AGGRESSIVE
			{
				public static LocString NAME = "Destructive";

				public static LocString DESC = "This Duplicant handles stress by taking their frustrations out on defenseless machines";

				public static LocString NOREPAIR = "• Will not repair buildings while above 60% Stress";
			}

			public class UGLYCRIER
			{
				public static LocString NAME = "Ugly Crier";

				public static LocString DESC = "If this Duplicant gets too stressed it won't be pretty";
			}

			public class BINGEEATER
			{
				public static LocString NAME = "Binge Eater";

				public static LocString DESC = "This Duplicant will dangerously overeat when stressed";
			}

			public class ANXIOUS
			{
				public static LocString NAME = "Anxious";

				public static LocString DESC = "This Duplicant collapses when put under too much pressure";
			}

			public class STRESSVOMITER
			{
				public static LocString NAME = "Vomiter";

				public static LocString DESC = "This Duplicant is liable to puke everywhere when stressed";
			}

			public class IRONGUT
			{
				public static LocString NAME = "Iron Gut";

				public static LocString DESC = "This Duplicant can eat just about anything without getting sick";

				public static LocString SHORT_DESC = "Immune to <b>" + DISEASES.FOODSICKNESS.NAME + "</b>";

				public static LocString SHORT_DESC_TOOLTIP = "Eating food contaminated with " + DISEASES.FOODSICKNESS.NAME + " Germs will not affect this Duplicant";
			}

			public class STRONGIMMUNESYSTEM
			{
				public static LocString NAME = "Germ Resistant";

				public static LocString DESC = "This Duplicant's immune system bounces back faster than most";
			}

			public class SCAREDYCAT
			{
				public static LocString NAME = "Pacifist";

				public static LocString DESC = "This Duplicant abhors violence";
			}

			public class ALLERGIES
			{
				public static LocString NAME = "Allergies";

				public static LocString DESC = "This Duplicant will sneeze uncontrollably when exposed to the pollen present in " + DISEASES.POLLENGERMS.NAME;

				public static LocString SHORT_DESC = "Allergic reaction to <b>" + DISEASES.POLLENGERMS.NAME + "</b>";

				public static LocString SHORT_DESC_TOOLTIP = DISEASES.ALLERGIES.DESCRIPTIVE_SYMPTOMS;
			}

			public class WEAKIMMUNESYSTEM
			{
				public static LocString NAME = "Biohazardous";

				public static LocString DESC = "All the vitamin C in space couldn't stop this Duplicant from getting sick";
			}

			public class IRRITABLEBOWEL
			{
				public static LocString NAME = "Irritable Bowel";

				public static LocString DESC = "This Duplicant needs a little extra time to \"do their business\"";
			}

			public class CALORIEBURNER
			{
				public static LocString NAME = "Bottomless Stomach";

				public static LocString DESC = "This Duplicant might actually be several black holes in a trench coat";
			}

			public class SMALLBLADDER
			{
				public static LocString NAME = "Small Bladder";

				public static LocString DESC = "This Duplicant has a tiny, pea-sized bladder. Adorable!";
			}

			public class ANEMIC
			{
				public static LocString NAME = "Anemic";

				public static LocString DESC = "This Duplicant has trouble keeping up with the others";
			}

			public class GREASEMONKEY
			{
				public static LocString NAME = "Grease Monkey";

				public static LocString DESC = "This Duplicant likes to throw a wrench into the colony's plans... in a good way";
			}

			public class MOLEHANDS
			{
				public static LocString NAME = "Mole Hands";

				public static LocString DESC = "They're great for tunneling, but finding good gloves is a nightmare";
			}

			public class FASTLEARNER
			{
				public static LocString NAME = "Quick Learner";

				public static LocString DESC = "This Duplicant's sharp as a tack and learns new skills with amazing speed";
			}

			public class SLOWLEARNER
			{
				public static LocString NAME = "Slow Learner";

				public static LocString DESC = "This Duplicant's a little slow on the uptake, but gosh do they try";
			}

			public class DIVERSLUNG
			{
				public static LocString NAME = "Diver's Lungs";

				public static LocString DESC = "This Duplicant could have been a talented opera singer in another life";
			}

			public class FLATULENCE
			{
				public static LocString NAME = "Flatulent";

				public static LocString DESC = "Some Duplicants are just full of it";

				public static LocString SHORT_DESC = "Farts frequently";

				public static LocString SHORT_DESC_TOOLTIP = "This Duplicant will periodically \"output\" " + ELEMENTS.METHANE.NAME;
			}

			public class SNORER
			{
				public static LocString NAME = "Loud Sleeper";

				public static LocString DESC = "In space, everyone can hear you snore";

				public static LocString SHORT_DESC = "Snores loudly";

				public static LocString SHORT_DESC_TOOLTIP = "This Duplicant's snoring will rudely awake nearby friends";
			}

			public class NARCOLEPSY
			{
				public static LocString NAME = "Narcoleptic";

				public static LocString DESC = "This Duplicant can and will fall asleep anytime, anyplace";

				public static LocString SHORT_DESC = "Falls asleep periodically";

				public static LocString SHORT_DESC_TOOLTIP = "This Duplicant's work will be periodically interrupted by naps";
			}

			public class INTERIORDECORATOR
			{
				public static LocString NAME = "Interior Decorator";

				public static LocString DESC = "\"Move it a little to the left...\"";
			}

			public class UNCULTURED
			{
				public static LocString NAME = "Uncultured";

				public static LocString DESC = "This Duplicant has simply no appreciation for the arts";
			}

			public class EARLYBIRD
			{
				public static LocString NAME = "Early Bird";

				public static LocString DESC = "This Duplicant always wakes up feeling fresh and efficient!";

				public static LocString EXTENDED_DESC = "• Morning: {0} bonus to all attributes\n• Duration: 5 Schedule Blocks";

				public static LocString SHORT_DESC = "Gains morning attribute bonuses";

				public static LocString SHORT_DESC_TOOLTIP = "Morning: +2 bonus to all attributes\n• Duration: 5 Schedule Blocks";
			}

			public class NIGHTOWL
			{
				public static LocString NAME = "Night Owl";

				public static LocString DESC = "This Duplicant does their best work when they'd ought to be sleeping";

				public static LocString EXTENDED_DESC = "• Nighttime: {0} bonus to all attributes\n• Duration: All Night";

				public static LocString SHORT_DESC = "Gains nighttime attribute bonuses";

				public static LocString SHORT_DESC_TOOLTIP = "Nighttime: +3 bonus to all attributes\n• Duration: All Night";
			}

			public class REGENERATION
			{
				public static LocString NAME = "Regenerative";

				public static LocString DESC = "This Duplicant is constantly regenerating health.";
			}

			public class DEEPERDIVERSLUNGS
			{
				public static LocString NAME = "Deep Diver's Lungs";

				public static LocString DESC = "This Duplicant was a talented opera singer and a snorkeling instructor.";
			}

			public class SUNNYDISPOSITION
			{
				public static LocString NAME = "Sunny Disposition";

				public static LocString DESC = "This Duplicant has an unwaveringly positive outlook on life";
			}

			public class ROCKCRUSHER
			{
				public static LocString NAME = "Beefsteak";

				public static LocString DESC = "This Duplicant's got muscles on their muscles!";
			}

			public class SIMPLETASTES
			{
				public static LocString NAME = "No Taste";

				public static LocString DESC = "This Duplicant's dull senses could barely tell the difference between Stuffed Berry and cardboard";
			}

			public class FOODIE
			{
				public static LocString NAME = "Gourmet";

				public static LocString DESC = "This Duplicant's refined palate demands only the most luxurious dishes the colony can offer";
			}

			public class ARCHAEOLOGIST
			{
				public static LocString NAME = "Relic Hunter";

				public static LocString DESC = "This Duplicant was never taught the phrase \"take only pictures, leave only footprints\"";
			}

			public static LocString TRAIT_DESCRIPTION_LIST_ENTRY = "\n• ";

			public static LocString ATTRIBUTE_MODIFIERS = "{0}: {1}";

			public static LocString CANNOT_DO_TASK = "Cannot do <b>{0} Errands</b>";

			public static LocString CANNOT_DO_TASK_TOOLTIP = "{0}: {1}";

			public static LocString REFUSES_TO_DO_TASK = "Cannot do<b>{0} Errands</b>";

			public static LocString IGNORED_EFFECTS = "Immune to <b>{0}</b>";

			public static LocString IGNORED_EFFECTS_TOOLTIP = "{0}: {1}";
		}

		public class PERSONALITIES
		{
			public class CATALINA
			{
				public static LocString NAME = "Catalina";

				public static LocString DESC = "A {0} is admired by all for her seemingly tireless work ethic. Little do people know, she's dying on the inside.";
			}

			public class NISBET
			{
				public static LocString NAME = "Nisbet";

				public static LocString DESC = "This {0} likes to punch people to show her affection. Everyone's too afraid of her to tell her it hurts.";
			}

			public class ELLIE
			{
				public static LocString NAME = "Ellie";

				public static LocString DESC = "Nothing makes an {0} happier than a big tin of glitter and a pack of unicorn stickers.";
			}

			public class RUBY
			{
				public static LocString NAME = "Ruby";

				public static LocString DESC = "This {0} asks the pressing questions, like \"Where can I get a leather jacket in space?\"";
			}

			public class LEIRA
			{
				public static LocString NAME = "Leira";

				public static LocString DESC = "{0}s just want everyone to be happy.";
			}

			public class BUBBLES
			{
				public static LocString NAME = "Bubbles";

				public static LocString DESC = "This {0} is constantly challenging others to fight her, regardless of whether or not she can actually take them.";
			}

			public class MIMA
			{
				public static LocString NAME = "Mi-Ma";

				public static LocString DESC = "Ol' {0} here can't stand lookin' at people's knees.";
			}

			public class NAILS
			{
				public static LocString NAME = "Nails";

				public static LocString DESC = "People often expect a Duplicant named \"{0}\" to be tough, but they're all pretty huge wimps.";
			}

			public class MAE
			{
				public static LocString NAME = "Mae";

				public static LocString DESC = "There's nothing a {0} can't do if she sets her mind to it.";
			}

			public class GOSSMANN
			{
				public static LocString NAME = "Gossmann";

				public static LocString DESC = "{0}s are major goofballs who can make anyone laugh.";
			}

			public class MARIE
			{
				public static LocString NAME = "Marie";

				public static LocString DESC = "This {0} is positively glowing! What's her secret? Radioactive isotopes, of course.";
			}

			public class LINDSAY
			{
				public static LocString NAME = "Lindsay";

				public static LocString DESC = "A {0} is a charming woman, unless you make the mistake of messing with one of her friends.";
			}

			public class DEVON
			{
				public static LocString NAME = "Devon";

				public static LocString DESC = "This {0} dreams of owning their own personal computer so they can start a blog full of pictures of toast.";
			}

			public class REN
			{
				public static LocString NAME = "Ren";

				public static LocString DESC = "Every {0} has this unshakable feeling that his life's already happened and he's just watching it unfold from a memory.";
			}

			public class FRANKIE
			{
				public static LocString NAME = "Frankie";

				public static LocString DESC = "There's nothing {0}s are more proud of than their thick, dignified eyebrows.";
			}

			public class BANHI
			{
				public static LocString NAME = "Banhi";

				public static LocString DESC = "The \"cool loner\" vibes that radiate off a {0} never fail to make the colony swoon.";
			}

			public class ADA
			{
				public static LocString NAME = "Ada";

				public static LocString DESC = "{0}s enjoy writing poetry in their downtime. Dark poetry.";
			}

			public class HASSAN
			{
				public static LocString NAME = "Hassan";

				public static LocString DESC = "If someone says something nice to a {0} he'll think about it nonstop for no less than three weeks.";
			}

			public class STINKY
			{
				public static LocString NAME = "Stinky";

				public static LocString DESC = "This {0} has never been invited to a party, which is a shame. His dance moves are incredible.";
			}

			public class JOSHUA
			{
				public static LocString NAME = "Joshua";

				public static LocString DESC = "{0}s are precious goobers. Other Duplicants are strangely incapable of cursing in a {0}'s presence.";
			}

			public class LIAM
			{
				public static LocString NAME = "Liam";

				public static LocString DESC = "No matter how much this {0} scrubs, he can never truly feel clean.";
			}

			public class ABE
			{
				public static LocString NAME = "Abe";

				public static LocString DESC = "{0}s are sweet, delicate flowers. They need to be treated gingerly, with great consideration for their feelings.";
			}

			public class BURT
			{
				public static LocString NAME = "Burt";

				public static LocString DESC = "This {0} always feels great after a bubble bath and a good long cry.";
			}

			public class TRAVALDO
			{
				public static LocString NAME = "Travaldo";

				public static LocString DESC = "A {0}'s monotonous voice and lack of facial expression makes it impossible for others to tell when he's messing with them.";
			}

			public class HAROLD
			{
				public static LocString NAME = "Harold";

				public static LocString DESC = "Get a bunch of {0}s together in a room, and you'll have... a bunch of {0}s together in a room.";
			}

			public class MAX
			{
				public static LocString NAME = "Max";

				public static LocString DESC = "At any given moment a {0} is viscerally reliving ten different humiliating memories.";
			}

			public class ROWAN
			{
				public static LocString NAME = "Rowan";

				public static LocString DESC = "{0}s have exceptionally large hearts and express their emotions most efficiently by yelling.";
			}

			public class OTTO
			{
				public static LocString NAME = "Otto";

				public static LocString DESC = "{0}s always insult people by accident and generally exist in a perpetual state of deep regret.";
			}

			public class TURNER
			{
				public static LocString NAME = "Turner";

				public static LocString DESC = "This {0} is paralyzed by the knowledge that others have memories and perceptions of them they can't control.";
			}

			public class NIKOLA
			{
				public static LocString NAME = "Nikola";

				public static LocString DESC = "This {0} once claimed he could build a laser so powerful it would rip the colony in half. No one asked him to prove it.";
			}

			public class MEEP
			{
				public static LocString NAME = "Meep";

				public static LocString DESC = "{0}s have a face only a two tonne Printing Pod could love.";
			}

			public class ARI
			{
				public static LocString NAME = "Ari";

				public static LocString DESC = "{0}s tend to space out from time to time, but they always pay attention when it counts.";
			}

			public class JEAN
			{
				public static LocString NAME = "Jean";

				public static LocString DESC = "Just because {0}s are a little slow doesn't mean they can't suffer from soul-crushing existential crises.";
			}

			public class CAMILLE
			{
				public static LocString NAME = "Camille";

				public static LocString DESC = "This {0} loves anything that makes her feel nostalgic, including things that haven't aged well.";
			}

			public class ASHKAN
			{
				public static LocString NAME = "Ashkan";

				public static LocString DESC = "{0}s have what can only be described as a \"seriously infectious giggle\".";
			}
		}

		public class NEEDS
		{
			public class DECOR
			{
				public static LocString NAME = "Decor Expectation";

				public static LocString PROFESSION_NAME = "Critic";

				public static LocString OBSERVED_DECOR = "Current Surroundings";

				public static LocString EXPECTATION_TOOLTIP = "Most objects have " + UI.FormatAsLink("Decor", "DECOR") + " values that alter Duplicants' opinions of their surroundings.\nThis Duplicant desires decor values of {0} or higher, and becomes " + UI.FormatAsLink("Stressed", "STRESS") + " in areas with lower decor.";

				public static LocString EXPECTATION_MOD_NAME = "Job Tier Request";
			}

			public class FOOD_QUALITY
			{
				public class ADJECTIVES
				{
					public static LocString MINUS_1 = "Grisly";

					public static LocString ZERO = "Terrible";

					public static LocString PLUS_1 = "Poor";

					public static LocString PLUS_2 = "Standard";

					public static LocString PLUS_3 = "Good";

					public static LocString PLUS_4 = "Great";

					public static LocString PLUS_5 = "Superb";

					public static LocString PLUS_6 = "Ambrosial";
				}

				public static LocString NAME = "Food Quality";

				public static LocString PROFESSION_NAME = "Gourmet";

				public static LocString EXPECTATION_TOOLTIP = "Each Duplicant has a minimum quality of " + UI.FormatAsLink("Food", "FOOD") + " they'll tolerate eating.\nThis Duplicant desires Tier {0} or better food, and becomes " + UI.FormatAsLink("Stressed", "STRESS") + " when they eat meals of lower quality.";

				public static LocString BAD_FOOD_MOD = "Food Quality";

				public static LocString NORMAL_FOOD_MOD = "Food Quality";

				public static LocString GOOD_FOOD_MOD = "Food Quality";

				public static LocString EXPECTATION_MOD_NAME = "Job Tier Request";

				public static LocString ADJECTIVE_FORMAT_POSITIVE = "{0} [{1}]";

				public static LocString ADJECTIVE_FORMAT_NEGATIVE = "{0} [{1}]";

				public static LocString FOODQUALITY = "\nFood Quality Score of {0}";

				public static LocString FOODQUALITY_EXPECTATION = "\nThis Duplicant is content to eat food with a Quality Score of {0} or higher";

				public static int ADJECTIVE_INDEX_OFFSET = -1;
			}

			public class QUALITYOFLIFE
			{
				public static LocString NAME = "Morale Requirements";

				public static LocString EXPECTATION_TOOLTIP = "The more responsibilities and stressors a Duplicant has, the more they will desire additional leisure time and improved amenities.\n\nFailing to keep a Duplicant's Morale at or above their Morale Expectation means they will not be able to unwind, causing them " + UI.FormatAsLink("Stress", "STRESS") + " over time.";

				public static LocString EXPECTATION_MOD_NAME = "Skills Learned";

				public static LocString APTITUDE_SKILLS_MOD_NAME = "Interested Skills Learned";

				public static LocString GOOD_MODIFIER = "High Morale";

				public static LocString NEUTRAL_MODIFIER = "Sufficient Morale";

				public static LocString BAD_MODIFIER = "Low Morale";
			}

			public class NOISE
			{
				public static LocString NAME = "Noise Expectation";
			}
		}

		public class ATTRIBUTES
		{
			public class CONSTRUCTION
			{
				public static LocString NAME = "Construction";

				public static LocString DESC = "Determines a Duplicant's building speed.";

				public static LocString SPEEDMODIFIER = "{0} Construction Speed";
			}

			public class SCALDINGTHRESHOLD
			{
				public static LocString NAME = "Scalding Threshold";

				public static LocString DESC = "Determines the temperature at which a Duplicant will get burned.";
			}

			public class DIGGING
			{
				public static LocString NAME = "Excavation";

				public static LocString DESC = "Determines a Duplicant's mining speed.";

				public static LocString SPEEDMODIFIER = "{0} Digging Speed";

				public static LocString ATTACK_MODIFIER = "{0} Attack Damage";
			}

			public class MACHINERY
			{
				public static LocString NAME = "Machinery";

				public static LocString DESC = "Determines how quickly a Duplicant uses machines.";

				public static LocString SPEEDMODIFIER = "{0} Machine Operation Speed";
			}

			public class LIFESUPPORT
			{
				public static LocString NAME = "Life Support";

				public static LocString DESC = "Determines how efficiently a Duplicant maintains algae terrariums, deodorizers, and water sieves.";
			}

			public class TOGGLE
			{
				public static LocString NAME = "Toggle";

				public static LocString DESC = "Determines how efficiently a Duplicant tunes machinery, flips switches, and sets sensors.";
			}

			public class MATURITYDELTA
			{
				public static LocString NAME = "Growth Speed";

				public static LocString DESC = "Determines the amount of time needed to reach maturation.";
			}

			public class ATHLETICS
			{
				public static LocString NAME = "Athletics";

				public static LocString DESC = "Determines a Duplicant's default runspeed.";

				public static LocString SPEEDMODIFIER = "{0} Runspeed";
			}

			public class DOCTOREDLEVEL
			{
				public static LocString NAME = UI.FormatAsLink("Treatment Received", "MEDICINE") + " Effect";

				public static LocString DESC = "Duplicants who receive medical care while in a Sick Bay or Disease Clinic will gain the \"Treatment Received\" effect\n\nThis effect reduces the severity of " + UI.FormatAsLink("Disease", "DISEASE") + " symptoms";
			}

			public class SNEEZYNESS
			{
				public static LocString NAME = "Sneeziness";

				public static LocString DESC = "Determines how frequently a Duplicant sneezes.";
			}

			public class GERMRESISTANCE
			{
				public class MODIFIER_DESCRIPTORS
				{
					public static LocString NEGATIVE_LARGE = "{0} (Large Loss)";

					public static LocString NEGATIVE_MEDIUM = "{0} (Medium Loss)";

					public static LocString NEGATIVE_SMALL = "{0} (Small Loss)";

					public static LocString NONE = "No Effect";

					public static LocString POSITIVE_SMALL = "{0} (Small Boost)";

					public static LocString POSITIVE_MEDIUM = "{0} (Medium Boost)";

					public static LocString POSITIVE_LARGE = "{0} (Large Boost)";
				}

				public static LocString NAME = "Germ Resistance";

				public static LocString DESC = "Duplicants with a higher Germ Resistance rating are less likely to contract germ-based " + UI.FormatAsLink("Diseases", "DISEASE") + ".";
			}

			public class LEARNING
			{
				public static LocString NAME = "Science";

				public static LocString DESC = "Determines how quickly a Duplicant conducts " + UI.FormatAsLink("Research", "RESEARCH") + " and gains " + UI.PRE_KEYWORD + "Skill Points" + UI.PST_KEYWORD + ".";

				public static LocString SPEEDMODIFIER = "{0} Skill Leveling";

				public static LocString RESEARCHSPEED = "{0} Research Speed";
			}

			public class COOKING
			{
				public static LocString NAME = "Cuisine";

				public static LocString DESC = "Determines how quickly a Duplicant prepares " + UI.FormatAsLink("Food", "FOOD") + ".";

				public static LocString SPEEDMODIFIER = "{0} Cooking Speed";
			}

			public class FERTILITYDELTA
			{
				public static LocString NAME = "Reproduction Rate";

				public static LocString DESC = "Determines the amount of time needed for a critter to lay new eggs.";
			}

			public class HAPPINESSDELTA
			{
				public static LocString NAME = "Happiness Change";

				public static LocString DESC = "Contented critters produce usable materials with increased frequency.";
			}

			public class WILDNESSDELTA
			{
				public static LocString NAME = "Wildness Change";

				public static LocString DESC = "Wild creatures can survive on fewer calories than domesticated ones.";
			}

			public class INSULATION
			{
				public static LocString NAME = "Insulation";

				public static LocString DESC = "Highly insulated Duplicants retain body heat easily, while low insulation Duplicants are easier to keep cool.";

				public static LocString SPEEDMODIFIER = "{0} Temperature Retention";
			}

			public class STRENGTH
			{
				public static LocString NAME = "Strength";

				public static LocString DESC = "Determines a Duplicant's carrying capacity and cleaning speed.";

				public static LocString CARRYMODIFIER = "{0} " + CARRYAMOUNT.NAME;

				public static LocString SPEEDMODIFIER = "{0} Tidying Speed";
			}

			public class CARING
			{
				public static LocString NAME = "Medicine";

				public static LocString DESC = "Determines a Duplicant's ability to care for sick peers.";

				public static LocString SPEEDMODIFIER = "{0} Treatment Speed";

				public static LocString FABRICATE_SPEEDMODIFIER = "{0} Medicine Fabrication Speed";
			}

			public class IMMUNITY
			{
				public static LocString NAME = "Immunity";

				public static LocString DESC = "Determines a Duplicant's disease susceptibility and recovery time.";

				public static LocString BOOST_MODIFIER = "{0} Immunity Regen";

				public static LocString BOOST_STAT = "Immunity Attribute";
			}

			public class BOTANIST
			{
				public static LocString NAME = "Agriculture";

				public static LocString DESC = "Determines how quickly and efficiently a Duplicant cultivates plants.";

				public static LocString HARVEST_SPEED_MODIFIER = "{0} Harvesting Speed";

				public static LocString TINKER_MODIFIER = "{0} Tending Speed";

				public static LocString BONUS_SEEDS = "{0} Seed Chance";
			}

			public class RANCHING
			{
				public static LocString NAME = "Husbandry";

				public static LocString DESC = "Determines how efficiently a Duplicant tends critters.";

				public static LocString EFFECTMODIFIER = "{0} Groom Effect Duration";
			}

			public class ART
			{
				public static LocString NAME = "Creativity";

				public static LocString DESC = "Determines how quickly a Duplicant produces artwork.";

				public static LocString SPEEDMODIFIER = "{0} Decorating Speed";
			}

			public class DECOR
			{
				public static LocString NAME = "Decor";

				public static LocString DESC = UI.FormatAsLink("Decor", "DECOR") + " affects a Duplicant's " + UI.FormatAsLink("Stress", "STRESS") + " and their opinion of their surroundings.";
			}

			public class THERMALCONDUCTIVITYBARRIER
			{
				public static LocString NAME = "Insulation Thickness";

				public static LocString DESC = "Insulation Thickness determines how quickly a Duplicant retains or loses body " + UI.FormatAsLink("Heat", "HEAT") + " in any given area.\n\nIt is the sum of a Duplicant's equipment and their natural Insulation values.";
			}

			public class DECORRADIUS
			{
				public static LocString NAME = "Decor Radius";

				public static LocString DESC = "The influence range of an object's " + UI.FormatAsLink("Decor", "DECOR") + " value.";
			}

			public class DECOREXPECTATION
			{
				public static LocString NAME = "Decor Morale Bonus";

				public static LocString DESC = "A Decor Morale Bonus allows Duplicants to receive " + UI.FormatAsLink("Morale", "QUALITYOFLIFE") + " boosts from lower " + UI.FormatAsLink("Decor", "DECOR") + " values.\n\nMaintaining high Morale will allow Duplicants to learn more skills.";
			}

			public class FOODEXPECTATION
			{
				public static LocString NAME = "Food Morale Bonus";

				public static LocString DESC = "A Food Morale Bonus allows Duplicants to receive " + UI.FormatAsLink("Morale", "QUALITYOFLIFE") + " boosts from lower quality " + UI.FormatAsLink("Foods", "FOOD") + ".\n\nMaintaining high Morale will allow Duplicants to learn more skills.";
			}

			public class QUALITYOFLIFEEXPECTATION
			{
				public static LocString NAME = "Morale Need";

				public static LocString DESC = "Morale Need";
			}

			public class HYGIENE
			{
				public static LocString NAME = "Hygiene";

				public static LocString DESC = UI.FormatAsLink("Hygiene", "HYGIENE") + " affects a Duplicant's sense of cleanliness.";
			}

			public class CARRYAMOUNT
			{
				public static LocString NAME = "Carrying Capacity";

				public static LocString DESC = "Determines the maximum weight that a Duplicant can carry.";
			}

			public class SPACENAVIGATION
			{
				public static LocString NAME = "Rocket Navigation Efficiency";

				public static LocString DESC = "Determines how long it takes a Duplicant to complete a space mission.";
			}

			public class QUALITYOFLIFE
			{
				public static LocString NAME = "Morale";

				public static LocString DESC = "A Duplicant's " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " must exceed their " + UI.PRE_KEYWORD + "Morale Need" + UI.PST_KEYWORD + ", or they'll begin to accumulate " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD + ".\n\n" + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " can be increased by providing Duplicants higher quality " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + ", allotting more " + UI.PRE_KEYWORD + "Downtime" + UI.PST_KEYWORD + " in\nthe colony schedule, or building better " + UI.PRE_KEYWORD + "Bathrooms" + UI.PST_KEYWORD + " and " + UI.PRE_KEYWORD + "Bedrooms" + UI.PST_KEYWORD + " for them to live in.";

				public static LocString DESC_FORMAT = "{0} / {1}";

				public static LocString TOOLTIP_EXPECTATION = "Total <b>Morale Need</b>: {0}\n    • Skills Learned: +{0}";

				public static LocString TOOLTIP_EXPECTATION_OVER = "This Duplicant has sufficiently high " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD;

				public static LocString TOOLTIP_EXPECTATION_UNDER = "This Duplicant's low " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " will cause " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD + " over time";
			}

			public class AIRCONSUMPTIONRATE
			{
				public static LocString NAME = "Air Consumption Rate";

				public static LocString DESC = "Air Consumption determines how much " + ELEMENTS.OXYGEN.NAME + " a Duplicant requires per minute to live.";
			}

			public class STRESSDELTA
			{
				public static LocString NAME = "Stress Change";

				public static LocString DESC = "Determines how quickly a Duplicant gains or reduces " + UI.FormatAsLink("Stress", "STRESS") + ".";
			}

			public class BREATHDELTA
			{
				public static LocString NAME = "Breath Change";

				public static LocString DESC = "Determines how quickly a Duplicant gains or reduces " + UI.FormatAsLink("Breath", "STRESS") + ".";
			}

			public class BLADDERDELTA
			{
				public static LocString NAME = "Bladder Change";

				public static LocString DESC = "Determines how quickly a Duplicant's bladder fills or depletes.";
			}

			public class CALORIESDELTA
			{
				public static LocString NAME = "Duplicant Metabolism";

				public static LocString DESC = "Determines how quickly a Duplicant burns or stores calories.";
			}

			public class STAMINADELTA
			{
				public static LocString NAME = "Stamina Change";

				public static LocString DESC = "";
			}

			public class TOXICITYDELTA
			{
				public static LocString NAME = "Toxicity Change";

				public static LocString DESC = "";
			}

			public class IMMUNELEVELDELTA
			{
				public static LocString NAME = "Immunity Regen";

				public static LocString DESC = "";
			}

			public class TOILETEFFICIENCY
			{
				public static LocString NAME = "Bathroom Use Speed";

				public static LocString DESC = "Determines how long a Duplicant needs to do their \"business\".";

				public static LocString SPEEDMODIFIER = "{0} Bathroom Use Speed";
			}

			public class METABOLISM
			{
				public static LocString NAME = "Critter Metabolism";

				public static LocString DESC = "Affects the rate at which a critter burns calories.";
			}

			public class ROOMTEMPERATUREPREFERENCE
			{
				public static LocString NAME = "Temperature Preference";

				public static LocString DESC = "Determines the minimum body heat a Duplicant prefers to maintain.";
			}

			public class MAXUNDERWATERTRAVELCOST
			{
				public static LocString NAME = "Underwater Movement";

				public static LocString DESC = "Determines a Duplicant's runspeed when submerged in liquid";
			}

			public class OVERHEATTEMPERATURE
			{
				public static LocString NAME = "Overheat Temperature";

				public static LocString DESC = "A building at Overheat " + UI.FormatAsLink("Temperature", "HEAT") + " will take damage and melt down if not cooled";
			}

			public class FATALTEMPERATURE
			{
				public static LocString NAME = "Meltdown Temperature";

				public static LocString DESC = "A building at Meltdown " + UI.FormatAsLink("Temperature", "HEAT") + " will lose functionality and take damage";
			}

			public class HITPOINTSDELTA
			{
				public static LocString NAME = UI.FormatAsLink("Health", "HEALTH");

				public static LocString DESC = "Health regeneration is increased when another Duplicant provides medical care to the patient";
			}

			public class DISEASECURESPEED
			{
				public static LocString NAME = UI.FormatAsLink("Disease", "DISEASE") + " Recovery Speed Bonus";

				public static LocString DESC = "Recovery speed bonus is increased when another Duplicant provides medical care to the patient";
			}

			public abstract class MACHINERYSPEED
			{
				public static LocString NAME = "Machinery Speed";

				public static LocString DESC = "Speed Bonus";
			}

			public abstract class GENERATOROUTPUT
			{
				public static LocString NAME = "Power Output";
			}

			public class INCUBATIONDELTA
			{
				public static LocString NAME = "Incubation Rate";

				public static LocString DESC = "";
			}

			public class HAPPINESS
			{
				public static LocString NAME = "Happiness";

				public static LocString DESC = "";
			}

			public class SCALEGROWTHDELTA
			{
				public static LocString NAME = "Scale Growth";

				public static LocString TOOLTIP = "Scale Growth" + UI.HORIZONTAL_BR_RULE + "How long until this creatures scales will grow back.";
			}

			public class VIABILITYDELTA
			{
				public static LocString NAME = "Viability Loss Rate";

				public static LocString DESC = "";
			}

			public static LocString VALUE = "{0}: {1}";

			public static LocString TOTAL_VALUE = "\n\nTotal <b>{1}</b>: {0}";

			public static LocString BASE_VALUE = "\nBase: {0}";

			public static LocString MODIFIER_ENTRY = "\n    • {0}: {1}";

			public static LocString UNPROFESSIONAL_NAME = "Lump";

			public static LocString UNPROFESSIONAL_DESC = "This Duplicant has no discernible skills";

			public static LocString PROFESSION_DESC = "Expertise is determined by a Duplicant's highest Attribute" + UI.HORIZONTAL_BR_RULE + "Duplicants develop higher expectations as their Expertise level increases";

			public static LocString STORED_VALUE = "Stored value";
		}

		public class ROLES
		{
			public class GROUPS
			{
				public static LocString APTITUDE_DESCRIPTION = "This Duplicant will gain <b>{1}</b> " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " when learning " + UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD + " Skills";

				public static LocString APTITUDE_DESCRIPTION_CHOREGROUP = "{2}\n\nThis Duplicant will gain <b>+{1}</b> " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " when learning " + UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD + " Skills";

				public static LocString SUITS = "Suit Wearing";
			}

			public class NO_ROLE
			{
				public static LocString NAME = UI.FormatAsLink("Unemployed", "NO_ROLE");

				public static LocString DESCRIPTION = "No job assignment.";
			}

			public class JUNIOR_ARTIST
			{
				public static LocString NAME = UI.FormatAsLink("Art Fundamentals", "JUNIOR_ARTIST");

				public static LocString DESCRIPTION = "Duplicant gains the most basic level of art skill";
			}

			public class ARTIST
			{
				public static LocString NAME = "Aesthetic Design";

				public static LocString DESCRIPTION = "Allows moderately attractive art to be created";
			}

			public class MASTER_ARTIST
			{
				public static LocString NAME = UI.FormatAsLink("Masterworks", "MASTER_ARTIST");

				public static LocString DESCRIPTION = "Enables the painting and sculpting of masterpieces";
			}

			public class JUNIOR_BUILDER
			{
				public static LocString NAME = UI.FormatAsLink("Improved Construction I", "JUNIOR_BUILDER");

				public static LocString DESCRIPTION = "Marginally improves a Duplicant's construction speeds.";
			}

			public class BUILDER
			{
				public static LocString NAME = UI.FormatAsLink("Improved Construction II", "BUILDER");

				public static LocString DESCRIPTION = "Further increases a Duplicant's construction speeds.";
			}

			public class SENIOR_BUILDER
			{
				public static LocString NAME = UI.FormatAsLink("Improved Construction III", "SENIOR_BUILDER");

				public static LocString DESCRIPTION = "Further increases a Duplicant's construction speeds.";
			}

			public class JUNIOR_RESEARCHER
			{
				public static LocString NAME = UI.FormatAsLink("Advanced Research", "JUNIOR_RESEARCHER");

				public static LocString DESCRIPTION = "Allows research of more advanced topics";
			}

			public class RESEARCHER
			{
				public static LocString NAME = UI.FormatAsLink("Field Research", "RESEARCHER");

				public static LocString DESCRIPTION = "Duplicants can perform studies on Geysers and other phenomena";
			}

			public class SENIOR_RESEARCHER
			{
				public static LocString NAME = UI.FormatAsLink("Astronomy", "SENIOR_RESEARCHER");

				public static LocString DESCRIPTION = "Enables the study of outer space";
			}

			public class JUNIOR_COOK
			{
				public static LocString NAME = UI.FormatAsLink("Grilling", "JUNIOR_COOK");

				public static LocString DESCRIPTION = "Allows the use of Electric Grills";
			}

			public class COOK
			{
				public static LocString NAME = UI.FormatAsLink("Improved Cooking I", "COOK");

				public static LocString DESCRIPTION = "Improves a Duplicant's cooking speed";
			}

			public class JUNIOR_MEDIC
			{
				public static LocString NAME = UI.FormatAsLink("Medicine Compounding", "MEDIC");

				public static LocString DESCRIPTION = "Allows compounding of medicines at the Apothecary";
			}

			public class MEDIC
			{
				public static LocString NAME = UI.FormatAsLink("Duplicant Treatment", "MEDIC");

				public static LocString DESCRIPTION = "Trains a Duplicant to administer medicine to other Duplicants";
			}

			public class SENIOR_MEDIC
			{
				public static LocString NAME = UI.FormatAsLink("Improved Medicine I", "MEDIC");

				public static LocString DESCRIPTION = "Improves the speed of medical errands";
			}

			public class MACHINE_TECHNICIAN
			{
				public static LocString NAME = UI.FormatAsLink("Improved Tinkering I", "MACHINE_TECHNICIAN");

				public static LocString DESCRIPTION = "Marginally improves a Duplicant's tinkering speeds.";
			}

			public class OIL_TECHNICIAN
			{
				public static LocString NAME = UI.FormatAsLink("Oil Engineering", "OIL_TECHNICIAN");

				public static LocString DESCRIPTION = "Allows the extraction and refinement of crude oil";
			}

			public class HAULER
			{
				public static LocString NAME = UI.FormatAsLink("Improved Carrying I", "HAULER");

				public static LocString DESCRIPTION = "Minorly increase a Duplicant's strength and carrying capacity.";
			}

			public class MATERIALS_MANAGER
			{
				public static LocString NAME = UI.FormatAsLink("Improved Carrying II", "MATERIALS_MANAGER");

				public static LocString DESCRIPTION = "Further increases a Duplicant's strength and carrying capacity for even swifter deliveries.";
			}

			public class JUNIOR_FARMER
			{
				public static LocString NAME = UI.FormatAsLink("Improved Farming I", "JUNIOR_FARMER");

				public static LocString DESCRIPTION = "Minorly increase a Duplicant's farming skills, increasing their chances of harvesting new plant seeds.";
			}

			public class FARMER
			{
				public static LocString NAME = UI.FormatAsLink("Crop Tending", "FARMER");

				public static LocString DESCRIPTION = "Enables tending plants, which will increase their growth speed.";
			}

			public class SENIOR_FARMER
			{
				public static LocString NAME = UI.FormatAsLink("Improved Farming II", "SENIOR_FARMER");

				public static LocString DESCRIPTION = "Further increases a Duplicant's farming skills.";
			}

			public class JUNIOR_MINER
			{
				public static LocString NAME = UI.FormatAsLink("Hard Digging", "JUNIOR_MINER");

				public static LocString DESCRIPTION = "Allows excavation of very firm materials such as Granite.";
			}

			public class MINER
			{
				public static LocString NAME = UI.FormatAsLink("Superhard Digging", "MINER");

				public static LocString DESCRIPTION = "Allows excavation of the element Abyssalite.";
			}

			public class SENIOR_MINER
			{
				public static LocString NAME = UI.FormatAsLink("Superduperhard Digging", "SENIOR_MINER");

				public static LocString DESCRIPTION = "Allows excavation of extremely hard materials like Diamond and Obsidian.";
			}

			public class SUIT_EXPERT
			{
				public static LocString NAME = UI.FormatAsLink("Exosuit Training", "SUIT_EXPERT");

				public static LocString DESCRIPTION = "Eliminates the runspeed loss experienced while wearing exosuits.";
			}

			public class POWER_TECHNICIAN
			{
				public static LocString NAME = UI.FormatAsLink("Electrical Engineering", "POWER_TECHNICIAN");

				public static LocString DESCRIPTION = "Enables generator tune-ups, which will temporarily provide improved power output.";
			}

			public class MECHATRONIC_ENGINEER
			{
				public static LocString NAME = UI.FormatAsLink("Mechatronics Engineering", "MECHATRONIC_ENGINEER");

				public static LocString DESCRIPTION = "Allows construction and maintenance of conveyor rail systems.";
			}

			public class HANDYMAN
			{
				public static LocString NAME = UI.FormatAsLink("Improved Strength I", "HANDYMAN");

				public static LocString DESCRIPTION = "Minorly improves a Duplicant's physical strength.";
			}

			public class PLUMBER
			{
				public static LocString NAME = UI.FormatAsLink("Plumbing", "PLUMBER");

				public static LocString DESCRIPTION = "Allows a Duplicant to empty plumbing pipes without making a mess.";
			}

			public class RANCHER
			{
				public static LocString NAME = UI.FormatAsLink("Critter Ranching", "RANCHER");

				public static LocString DESCRIPTION = "Allows a Duplicant to handle and care for critters";
			}

			public class SENIOR_RANCHER
			{
				public static LocString NAME = UI.FormatAsLink("Improved Ranching I", "SENIOR_RANCHER");

				public static LocString DESCRIPTION = "Improves a Duplicant's Ranching skill";
			}

			public class ASTRONAUTTRAINEE
			{
				public static LocString NAME = UI.FormatAsLink("Rocket Piloting", "ASTRONAUTTRAINEE");

				public static LocString DESCRIPTION = "Allows a Duplicant to command a rocket ship";
			}

			public class ASTRONAUT
			{
				public static LocString NAME = UI.FormatAsLink("Rocket Navigation", "ASTRONAUT");

				public static LocString DESCRIPTION = "Improves the speed that space missions are completed";
			}
		}

		public class THOUGHTS
		{
			public class STARVING
			{
				public static LocString TOOLTIP = "Starving";
			}

			public class HOT
			{
				public static LocString TOOLTIP = "Hot";
			}

			public class COLD
			{
				public static LocString TOOLTIP = "Cold";
			}

			public class BREAKBLADDER
			{
				public static LocString TOOLTIP = "Washroom Break";
			}

			public class FULLBLADDER
			{
				public static LocString TOOLTIP = "Full Bladder";
			}

			public class HAPPY
			{
				public static LocString TOOLTIP = "Happy";
			}

			public class UNHAPPY
			{
				public static LocString TOOLTIP = "Unhappy";
			}

			public class POORDECOR
			{
				public static LocString TOOLTIP = "Poor Decor";
			}

			public class POOR_FOOD_QUALITY
			{
				public static LocString TOOLTIP = "Lousy Meal";
			}

			public class GOOD_FOOD_QUALITY
			{
				public static LocString TOOLTIP = "Delicious Meal";
			}

			public class SLEEPY
			{
				public static LocString TOOLTIP = "Sleepy";
			}

			public class SUFFOCATING
			{
				public static LocString TOOLTIP = "Suffocating";
			}

			public class ANGRY
			{
				public static LocString TOOLTIP = "Angry";
			}

			public class RAGING
			{
				public static LocString TOOLTIP = "Raging";
			}

			public class GOTINFECTED
			{
				public static LocString TOOLTIP = "Got Infected";
			}

			public class PUTRIDODOUR
			{
				public static LocString TOOLTIP = "Smelled Something Putrid";
			}

			public class NOISY
			{
				public static LocString TOOLTIP = "Loud Area";
			}

			public class NEWROLE
			{
				public static LocString TOOLTIP = "New Skill";
			}

			public class CHATTY
			{
				public static LocString TOOLTIP = "Greeting";
			}

			public class ENCOURAGE
			{
				public static LocString TOOLTIP = "Encouraging";
			}

			public class CONVERSATION
			{
				public static LocString TOOLTIP = "Chatting";
			}
		}

		public static LocString RACE_PREFIX = "Species: {0}";

		public static LocString RACE = "Duplicant";

		public static LocString NAMETITLE = "Name: ";

		public static LocString GENDERTITLE = "Gender: ";

		public static LocString ARRIVALTIME = "Age: ";

		public static LocString ARRIVALTIME_TOOLTIP = "This {1} was printed on Cycle {0}";

		public static LocString DESC_TOOLTIP = "About {0}s";
	}
}
