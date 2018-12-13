namespace STRINGS
{
	public class BUILDINGS
	{
		public class PREFABS
		{
			public class HEADQUARTERSCOMPLETE
			{
				public static LocString NAME = UI.FormatAsLink("Printing Pod", "HEADQUARTERS");
			}

			public class AIRCONDITIONER
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Regulator", "AIRCONDITIONER");

				public static LocString DESC = "A thermo regulator doesn't remove heat, but relocates it to a new area.";

				public static LocString EFFECT = "Cools the " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " piped through it, but outputs " + UI.FormatAsLink("Heat", "HEAT") + " in its immediate vicinity.";
			}

			public class ALGAEDISTILLERY
			{
				public static LocString NAME = UI.FormatAsLink("Algae Distiller", "ALGAEDISTILLERY");

				public static LocString DESC = "Algae distillers convert disease-causing slime into algae for oxygen production.";

				public static LocString EFFECT = "Refines " + UI.FormatAsLink("Slime", "SLIMEMOLD") + " into " + UI.FormatAsLink("Algae", "ALGAE") + ".";
			}

			public class OXYLITEREFINERY
			{
				public static LocString NAME = UI.FormatAsLink("Oxylite Refinery", "OXYLITEREFINERY");

				public static LocString DESC = "Oxylite is a solid and easily transportable source of consumable oxygen.";

				public static LocString EFFECT = "Synthesizes " + UI.FormatAsLink("Oxylite", "OXYROCK") + " using " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and a small amount of " + UI.FormatAsLink("Gold", "GOLD") + ".";
			}

			public class FERTILIZERMAKER
			{
				public static LocString NAME = UI.FormatAsLink("Fertilizer Synthesizer", "FERTILIZERMAKER");

				public static LocString DESC = "Fertilizer synthesizers convert polluted dirt into fertilizer for domestic plants.";

				public static LocString EFFECT = "Uses " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + " to produce " + UI.FormatAsLink("Fertilizer", "FERTILIZER") + ".";
			}

			public class ALGAEHABITAT
			{
				public static LocString NAME = UI.FormatAsLink("Algae Terrarium", "ALGAEHABITAT");

				public static LocString DESC = "Algae colony, Duplicant colony... we're more alike than we are different.";

				public static LocString EFFECT = "Consumes " + UI.FormatAsLink("Algae", "ALGAE") + " to produce " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and remove some " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + ".\n\nGains a 10% efficiency boost in direct " + UI.FormatAsLink("Light", "LIGHT") + ".";

				public static LocString SIDESCREEN_TITLE = "Empty " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + " Threshold";
			}

			public class BATTERY
			{
				public static LocString NAME = UI.FormatAsLink("Battery", "BATTERY");

				public static LocString DESC = "Batteries allow extra power from generators to be stored for later, rather than lost.";

				public static LocString EFFECT = "Stores runoff " + UI.FormatAsLink("Power", "POWER") + " from generators, but loses charge over time.";

				public static LocString CHARGE_LOSS = "{Battery} charge loss";
			}

			public class AIRBORNECREATURELURE
			{
				public static LocString NAME = UI.FormatAsLink("Critter Lure", "AIRBORNECREATURELURE");

				public static LocString DESC = "Lures can relocate Pufts or Shine Bugs to specific locations in your base.";

				public static LocString EFFECT = "Attracts one type of airborne critter.\n\nMust be baited with " + UI.FormatAsLink("Slime", "SLIMEMOLD") + " or " + UI.FormatAsLink("Phosphorite", "PHOSPHORITE") + ".";
			}

			public class BATTERYMEDIUM
			{
				public static LocString NAME = UI.FormatAsLink("Jumbo Battery", "BATTERYMEDIUM");

				public static LocString DESC = "Larger batteries hold more power and keep systems running longer before recharging.";

				public static LocString EFFECT = "Stores most runoff " + UI.FormatAsLink("Power", "POWER") + " from generators, but loses charge over time.";
			}

			public class BATTERYSMART
			{
				public static LocString NAME = UI.FormatAsLink("Smart Battery", "BATTERYSMART");

				public static LocString DESC = "Smart batteries can automatically turn systems off when there is no charge to sustain them.";

				public static LocString EFFECT = "Stores most runoff " + UI.FormatAsLink("Power", "POWER") + " from generators, but loses charge over time.\n\nLogic input becomes " + UI.FormatAsLink("Active", "LOGIC") + " when charged above the set threshold.";

				public static LocString LOGIC_PORT_DESC = "" + UI.FormatAsLink("Active", "LOGIC") + " if charge exceeds threshold";

				public static LocString ACTIVATE_TOOLTIP = "Logic input will become " + UI.FormatAsLink("Active", "LOGIC") + " when battery is less than {0}% charged";

				public static LocString DEACTIVATE_TOOLTIP = "Logic input will go on " + UI.FormatAsLink("Standby", "LOGIC") + " when battery is more than {0}% charged";

				public static LocString SIDESCREEN_TITLE = "Logic Activation Parameters";

				public static LocString SIDESCREEN_ACTIVATE = "Active:";

				public static LocString SIDESCREEN_DEACTIVATE = "Standby:";
			}

			public class BED
			{
				public static LocString NAME = UI.FormatAsLink("Cot", "BED");

				public static LocString DESC = "Duplicants without a bed will develop sore backs from sleeping on the floor.";

				public static LocString EFFECT = "Gives one Duplicant a place to sleep.\n\nDuplicants will automatically return to their cots to sleep at night.";
			}

			public class BOTTLEEMPTIER
			{
				public static LocString NAME = UI.FormatAsLink("Bottle Emptier", "BOTTLEEMPTIER");

				public static LocString DESC = "A bottle emptier's Element Filter can be used to designate areas for specific liquid storage.";

				public static LocString EFFECT = "Empties bottled " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " back into the world.";
			}

			public class BOTTLEEMPTIERGAS
			{
				public static LocString NAME = UI.FormatAsLink("Canister Emptier", "BOTTLEEMPTIERGAS");

				public static LocString DESC = "A canister emptier's Element Filter can be used to designate areas for specific gas storage.";

				public static LocString EFFECT = "Empties " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " canisters back into the world.";
			}

			public class CARGOBAY
			{
				public static LocString NAME = UI.FormatAsLink("Cargo Bay", "CARGOBAY");

				public static LocString DESC = "Duplicants will fill cargo bays with whatever resources they find during their space missions.";

				public static LocString EFFECT = "Allows Duplicants to store any " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class SPECIALCARGOBAY
			{
				public static LocString NAME = UI.FormatAsLink("Biological Cargo Bay", "SPECIALCARGOBAY");

				public static LocString DESC = "Biological cargo bays allow Duplicants to retrieve alien plants and wildlife from space.";

				public static LocString EFFECT = "Allows Duplicants to store unusual or organic resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class COMMANDMODULE
			{
				public static LocString NAME = UI.FormatAsLink("Command Capsule", "COMMANDMODULE");

				public static LocString DESC = "At least one astronaut must be assigned to the command module to pilot a rocket.";

				public static LocString EFFECT = "Contains passenger seating for Duplicant " + UI.FormatAsLink("Astronauts", "ASTRONAUT") + ".\n\nA Command Capsule must be the last module installed at the top of a rocket.";
			}

			public class RESEARCHMODULE
			{
				public static LocString NAME = UI.FormatAsLink("Research Module", "RESEARCHMODULE");

				public static LocString DESC = "Data banks can be used at virtual planetariums to produce additional research points.";

				public static LocString EFFECT = "Completes one " + UI.FormatAsLink("Research Task", "RESEARCH") + " per space mission.\n\nProduces a small Data Bank regardless of mission destination.\n\nGenerated " + UI.FormatAsLink("Research Points", "RESEARCH") + " become available upon the rocket's return.";
			}

			public class TOURISTMODULE
			{
				public static LocString NAME = UI.FormatAsLink("Sight-Seeing Module", "TOURISTMODULE");

				public static LocString DESC = "An astronaut must accompany sight seeing Duplicants on rocket flights.";

				public static LocString EFFECT = "Allows one non-Astronaut Duplicant to visit space.\n\nSight-Seeing rocket flights decrease " + UI.FormatAsLink("Stress", "STRESS") + ".";
			}

			public class GANTRY
			{
				public static LocString NAME = UI.FormatAsLink("Gantry", "GANTRY");

				public static LocString DESC = "A gantry can be built over rocket pieces where ladders and tile cannot.";

				public static LocString EFFECT = "Provides scaffolding across rocket modules to allow Duplicant access.";
			}

			public class WATERCOOLER
			{
				public static LocString NAME = UI.FormatAsLink("Water Cooler", "WATERCOOLER");

				public static LocString DESC = "Chatting with friends improves Duplicants' moods and reduces their stress.";

				public static LocString EFFECT = "Provides a gathering place for Duplicants during Downtime.\n\nImproves Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class ARCADEMACHINE
			{
				public static LocString NAME = UI.FormatAsLink("Arcade Cabinet", "ARCADEMACHINE");

				public static LocString DESC = "Komet Kablam-O!\nFor up to two players.";

				public static LocString EFFECT = "Allows Duplicants to play video games on their breaks.\n\nIncreases Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class PHONOBOX
			{
				public static LocString NAME = UI.FormatAsLink("Jukebot", "PHONOBOX");

				public static LocString DESC = "Dancing helps Duplicants get their feelings out.";

				public static LocString EFFECT = "Plays music for Duplicants to dance to on their breaks.\n\nIncreases Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class ESPRESSOMACHINE
			{
				public static LocString NAME = UI.FormatAsLink("Espresso Machine", "ESPRESSOMACHINE");

				public static LocString DESC = "A shot of espresso helps Duplicants relax after a long day.";

				public static LocString EFFECT = "Provides refreshment for Duplicants on their breaks.\n\nIncreases Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class CHECKPOINT
			{
				public static LocString NAME = UI.FormatAsLink("Duplicant Checkpoint", "CHECKPOINT");

				public static LocString DESC = "Checkpoints can be connected to automated sensors to determine when it's safe to enter.";

				public static LocString EFFECT = "Allows Duplicants to pass when " + UI.FormatAsLink("Active", "LOGIC") + ".\n\nPrevents Duplicants from passing when on " + UI.FormatAsLink("Standby", "LOGIC") + ".";

				public static LocString LOGIC_PORT_DESC = "Duplicant Stop/Go";
			}

			public class FIREPOLE
			{
				public static LocString NAME = UI.FormatAsLink("Fire Pole", "FIREPOLE");

				public static LocString DESC = "Fire poles provide quick downward travel, while ladders provide better upward movement.";

				public static LocString EFFECT = "Allows rapid Duplicant descent.\n\nSignificantly slows upward climbing.";
			}

			public class FLOORSWITCH
			{
				public static LocString NAME = UI.FormatAsLink("Weight Plate", "FLOORSWITCH");

				public static LocString DESC = "Weight plates can be used to turn on amenities only when Duplicants pass by.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " when an object or Duplicant is placed atop of it.\n\nCannot be triggered by " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " or " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + ".";

				public static LocString LOGIC_PORT_DESC = UI.FormatAsLink("Active", "LOGIC") + "/" + UI.FormatAsLink("Inactive", "LOGIC");
			}

			public class KILN
			{
				public static LocString NAME = UI.FormatAsLink("Kiln", "KILN");

				public static LocString DESC = "Kilns can also be used to refine coal into pure carbon.";

				public static LocString EFFECT = "Fires " + UI.FormatAsLink("Clay", "CLAY") + " to produce " + UI.FormatAsLink("Ceramic", "CERAMIC") + ".\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class LIQUIDFUELTANK
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Fuel Tank", "LIQUIDFUELTANK");

				public static LocString DESC = "Storing additional fuel increases the distance a rocket can travel before returning.";

				public static LocString EFFECT = "Stores the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " fuel piped into it to supply rocket engines.\n\nThe stored fuel type is determined by the rocket engine it is built upon.";
			}

			public class OXIDIZERTANK
			{
				public static LocString NAME = UI.FormatAsLink("Solid Oxidizer Tank", "OXIDIZERTANK");

				public static LocString DESC = "Solid oxidizers allows rocket fuel to be efficiently burned in the vacuum of space.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Oxylite", "OXYROCK") + " for burning rocket fuels.";
			}

			public class OXIDIZERTANKLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Oxidizer Tank", "LIQUIDOXIDIZERTANK");

				public static LocString DESC = "Liquid oxygen improves the thrust-to-mass ratio of rocket fuels.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Liquid Oxygen", "LIQUIDOXYGEN") + " for burning rocket fuels.";
			}

			public class LIQUIDCONDITIONER
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Aquatuner", "LIQUIDCONDITIONER");

				public static LocString DESC = "A thermo aquatuner cools liquid and outputs the heat elsewhere.";

				public static LocString EFFECT = "Cools the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " piped through it, but outputs " + UI.FormatAsLink("Heat", "HEAT") + " in its immediate vicinity.";
			}

			public class LIQUIDCARGOBAY
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Cargo Tank", "LIQUIDCARGOBAY");

				public static LocString DESC = "Duplicants will fill cargo tanks with whatever resources they find during their space missions.";

				public static LocString EFFECT = "Allows Duplicants to store any " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class LUXURYBED
			{
				public static LocString NAME = UI.FormatAsLink("Comfy Bed", "LUXURYBED");

				public static LocString DESC = "Duplicants prefer comfy beds to cots and will gain more stamina back from sleeping in them.";

				public static LocString EFFECT = "Provides a sleeping area for one Duplicant and restores additional " + UI.FormatAsLink("Stamina", "STAMINA") + ".\n\nDuplicants will automatically sleep in their assigned beds at night.";
			}

			public class MEDICALCOT
			{
				public static LocString NAME = UI.FormatAsLink("Med-Bed", "MEDICALCOT");

				public static LocString DESC = "Duplicants use med-beds to recover from sickness and receive medical aid from peers.";

				public static LocString EFFECT = "Improves recovery from " + UI.FormatAsLink("Diseases", "DISEASE") + ".\n\nAccelerates " + UI.FormatAsLink("Health", "HEALTH") + " restoration and the healing of physical injuries.";
			}

			public class MASSAGETABLE
			{
				public static LocString NAME = UI.FormatAsLink("Massage Table", "MASSAGETABLE");

				public static LocString DESC = "Massage tables quickly reduce extreme stress, at the cost of power production.";

				public static LocString EFFECT = "Rapidly reduces " + UI.FormatAsLink("Stress", "STRESS") + " for the Duplicant user.\n\nDuplicants will automatically seek a massage table when " + UI.FormatAsLink("Stress", "STRESS") + " exceeds breaktime range.";

				public static LocString ACTIVATE_TOOLTIP = "Duplicants must take a break when their stress reaches {0}";

				public static LocString DEACTIVATE_TOOLTIP = "Breaktime ends when stress is reduced to {0}";
			}

			public class CEILINGLIGHT
			{
				public static LocString NAME = UI.FormatAsLink("Ceiling Light", "CEILINGLIGHT");

				public static LocString DESC = "Light reduces Duplicant stress and is required to grow certain plants.";

				public static LocString EFFECT = "Provides " + UI.FormatAsLink("Light", "LIGHT") + " when " + UI.FormatAsLink("Powered", "POWER") + ".";
			}

			public class AIRFILTER
			{
				public static LocString NAME = UI.FormatAsLink("Deodorizer", "AIRFILTER");

				public static LocString DESC = "Oh! Citrus scented!";

				public static LocString EFFECT = "Uses " + UI.FormatAsLink("Sand", "SAND") + " to filter " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " from the air, reducing " + UI.FormatAsLink("Disease", "DISEASE") + " spread.";
			}

			public class CANVAS
			{
				public static LocString NAME = UI.FormatAsLink("Blank Canvas", "CANVAS");

				public static LocString DESC = "Once built, a Duplicant can paint a blank canvas to produce a decorative painting.";

				public static LocString EFFECT = "Increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be painted by a Duplicant.";

				public static LocString POORQUALITYNAME = "Crude Painting";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Painting";

				public static LocString EXCELLENTQUALITYNAME = "Masterpiece";
			}

			public class CANVASWIDE
			{
				public static LocString NAME = UI.FormatAsLink("Landscape Canvas", "CANVASWIDE");

				public static LocString DESC = "Once built, a Duplicant can paint a blank canvas to produce a decorative painting.";

				public static LocString EFFECT = "Moderately increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be painted by a Duplicant.";

				public static LocString POORQUALITYNAME = "Crude Painting";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Painting";

				public static LocString EXCELLENTQUALITYNAME = "Masterpiece";
			}

			public class CANVASTALL
			{
				public static LocString NAME = UI.FormatAsLink("Portrait Canvas", "CANVASTALL");

				public static LocString DESC = "Once built, a Duplicant can paint a blank canvas to produce a decorative painting.";

				public static LocString EFFECT = "Moderately increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be painted by a Duplicant.";

				public static LocString POORQUALITYNAME = "Crude Painting";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Painting";

				public static LocString EXCELLENTQUALITYNAME = "Masterpiece";
			}

			public class CO2SCRUBBER
			{
				public static LocString NAME = UI.FormatAsLink("Carbon Skimmer", "CO2SCRUBBER");

				public static LocString DESC = "Carbon skimmers remove large amounts of carbon dioxide, but produce no breathable air.";

				public static LocString EFFECT = "Uses " + UI.FormatAsLink("Water", "WATER") + " to filter " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " from the air.";
			}

			public class COMPOST
			{
				public static LocString NAME = UI.FormatAsLink("Compost", "COMPOST");

				public static LocString DESC = "Composts safely deal with biological waste, producing fresh dirt.";

				public static LocString EFFECT = "Reduces " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + " and other compostables down into " + UI.FormatAsLink("Dirt", "DIRT") + ".";
			}

			public class COOKINGSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Electric Grill", "COOKINGSTATION");

				public static LocString DESC = "Proper cooking eliminates foodborne disease and produces tasty, stress-relieving meals.";

				public static LocString EFFECT = "Cooks a wide variety of improved " + UI.FormatAsLink("Foods", "FOOD") + ".\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class DININGTABLE
			{
				public static LocString NAME = UI.FormatAsLink("Mess Table", "DININGTABLE");

				public static LocString DESC = "Duplicants prefer to dine at a table, rather than eat off the floor.";

				public static LocString EFFECT = "Gives one Duplicant a place to eat.\n\nDuplicants will automatically eat at their assigned table when hungry.";
			}

			public class DOOR
			{
				public static class CONTROL_STATE
				{
					public class OPEN
					{
						public static LocString NAME = "Open";

						public static LocString TOOLTIP = "This door will remain open";
					}

					public class CLOSE
					{
						public static LocString NAME = "Lock";

						public static LocString TOOLTIP = "Nothing may pass through";
					}

					public class AUTO
					{
						public static LocString NAME = "Auto";

						public static LocString TOOLTIP = "Duplicants open and close this door as needed";
					}
				}

				public static LocString NAME = UI.FormatAsLink("Pneumatic Door", "DOOR");

				public static LocString DESC = "Door controls can be used to prevent Duplicants from entering restricted areas.";

				public static LocString EFFECT = "Encloses areas without blocking " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " or " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " flow.\n\nSets Duplicant Access Permissions for area restriction.\n\nWild " + UI.FormatAsLink("Critters", "CRITTERS") + " cannot pass through doors.";

				public static LocString PRESSURE_SUIT_REQUIRED = UI.FormatAsLink("Atmo Suit", "ATMO_SUIT") + " required {0}";

				public static LocString PRESSURE_SUIT_NOT_REQUIRED = UI.FormatAsLink("Atmo Suit", "ATMO_SUIT") + " not required {0}";

				public static LocString ABOVE = "above";

				public static LocString BELOW = "below";

				public static LocString LEFT = "on left";

				public static LocString RIGHT = "on right";

				public static LocString LOGIC_PORT_DESC = "Open/Close";
			}

			public class ELECTROLYZER
			{
				public static LocString NAME = UI.FormatAsLink("Electrolyzer", "ELECTROLYZER");

				public static LocString DESC = "Water goes in one end, life sustaining oxygen comes out the other.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Water", "WATER") + " into " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + ".\n\nBecomes idle when the area reaches maximum pressure capacity.";
			}

			public class POWERTRANSFORMERSMALL
			{
				public static LocString NAME = UI.FormatAsLink("Power Transformer", "POWERTRANSFORMERSMALL");

				public static LocString DESC = "Transformers prevent wires and buildings from frying when on circuits with powerful generators.";

				public static LocString EFFECT = "Protects circuits from overloading by increasing or decreasing " + UI.FormatAsLink("Power", "POWER") + " flow.\n\nStores the " + UI.FormatAsLink("Power", "POWER") + " that is fed into it.";
			}

			public class POWERTRANSFORMER
			{
				public static LocString NAME = UI.FormatAsLink("Large Power Transformer", "POWERTRANSFORMER");

				public static LocString DESC = "Shh! It only transforms when no one's looking.";

				public static LocString EFFECT = "Protects circuits from overloading by increasing or decreasing " + UI.FormatAsLink("Power", "POWER") + " flow.\n\nStores a great deal of " + UI.FormatAsLink("Power", "POWER") + ".";
			}

			public class FLOORLAMP
			{
				public static LocString NAME = UI.FormatAsLink("Lamp", "FLOORLAMP");

				public static LocString DESC = "The emitting radius of a building's light can be viewed in the Light Overlay.";

				public static LocString EFFECT = "Provides " + UI.FormatAsLink("Light", "LIGHT") + " when " + UI.FormatAsLink("Powered", "POWER") + ".";
			}

			public class FLOWERVASE
			{
				public static LocString NAME = UI.FormatAsLink("Flower Pot", "FLOWERVASE");

				public static LocString DESC = "Flower pots allow decorative plants to be moved to new locations.";

				public static LocString EFFECT = "Houses a single " + UI.FormatAsLink("Plant", "PLANTS") + " when sown with a " + UI.FormatAsLink("Seed", "PLANTS") + ".\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class FLOWERVASEWALL
			{
				public static LocString NAME = UI.FormatAsLink("Wall Pot", "FLOWERVASEWALL");

				public static LocString DESC = "Placing a plant in a wall pot can add a spot of decor to otherwise bare walls.";

				public static LocString EFFECT = "Houses a single " + UI.FormatAsLink("Plant", "PLANTS") + " when sown with a " + UI.FormatAsLink("Seed", "PLANTS") + ".\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be hung from a wall.";
			}

			public class FLOWERVASEHANGING
			{
				public static LocString NAME = UI.FormatAsLink("Hanging Pot", "FLOWERVASEHANGING");

				public static LocString DESC = "Hanging pots can add a bit of decor to a room, without interferring with buildings on the floor.";

				public static LocString EFFECT = "Houses a single " + UI.FormatAsLink("Plant", "PLANTS") + " when sown with a " + UI.FormatAsLink("Seed", "PLANTS") + ".\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be hung from a ceiling.";
			}

			public class FLOWERVASEHANGINGFANCY
			{
				public static LocString NAME = UI.FormatAsLink("Aero Pot", "FLOWERVASEHANGINGFANCY");

				public static LocString DESC = "Aero pots can be hung from the ceiling and have extremely high decor values.";

				public static LocString EFFECT = "Houses a single " + UI.FormatAsLink("Plant", "PLANTS") + " when sown with a " + UI.FormatAsLink("Seed", "PLANTS") + ".\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be hung from a ceiling.";
			}

			public class FLUSHTOILET
			{
				public static LocString NAME = UI.FormatAsLink("Lavatory", "FLUSHTOILET");

				public static LocString DESC = "Lavatories transmit fewer germs to Duplicants' skin and require no manual emptying.";

				public static LocString EFFECT = "Gives Duplicants a place to " + UI.FormatAsLink("Hygienically", "HYGIENE") + " relieve themselves.";
			}

			public class FACILITYBACKWALLWINDOW
			{
				public static LocString NAME = UI.FormatAsLink("Window", "");

				public static LocString DESC = "";

				public static LocString EFFECT = "A tall, thin window.";
			}

			public class SHOWER
			{
				public static LocString NAME = UI.FormatAsLink("Shower", "SHOWER");

				public static LocString DESC = "Showers remove the \"Grimy\" effect and kill all germs present on Duplicant's skin.";

				public static LocString EFFECT = "Improves Duplicant " + UI.FormatAsLink("Hygiene", "HYGIENE") + " and removes " + UI.FormatAsLink("Germs", "DISEASE") + ".";
			}

			public class CONDUIT
			{
				public class STATUS_ITEM
				{
					public static LocString NAME = "Marked for Emptying";

					public static LocString TOOLTIP = "Awaiting a " + UI.FormatAsLink("Plumber", "PLUMBER") + " to clear this pipe";
				}
			}

			public class GASCARGOBAY
			{
				public static LocString NAME = UI.FormatAsLink("Gas Cargo Canister", "GASCARGOBAY");

				public static LocString DESC = "Duplicants will fill cargo canisters with whatever resources they find during their space missions.";

				public static LocString EFFECT = "Allows Duplicants to store any " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class GASCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pipe", "GASCONDUIT");

				public static LocString DESC = "Gas pipes are used to connect the inputs and outputs of ventilated buildings.";

				public static LocString EFFECT = "Carries " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " between " + UI.FormatAsLink("Outputs", "GASPIPING") + " and " + UI.FormatAsLink("Intakes", "GASPIPING") + ".\n\nCan be run through wall and floor tile.";
			}

			public class GASCONDUITBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Gas Bridge", "GASCONDUITBRIDGE");

				public static LocString DESC = "Separate pipe systems prevent mingled contents from causing building damage.";

				public static LocString EFFECT = "Runs one " + UI.FormatAsLink("Gas Pipe", "GASPIPING") + " section over another without joining them.\n\nCan be run through wall and floor tile.";
			}

			public class GASCONDUITPREFERENTIALFLOW
			{
				public static LocString NAME = UI.FormatAsLink("Priority Gas Flow", "GASCONDUITPREFERENTIALFLOW");

				public static LocString DESC = "Priority flows ensure important buildings are filled first when on a system with other buildings.";

				public static LocString EFFECT = "Diverts " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " to a secondary input when its primary input overflows.";
			}

			public class LIQUIDCONDUITPREFERENTIALFLOW
			{
				public static LocString NAME = UI.FormatAsLink("Priority Liquid Flow", "LIQUIDCONDUITPREFERENTIALFLOW");

				public static LocString DESC = "Priority flows ensure important buildings are filled first when on a system with other buildings.";

				public static LocString EFFECT = "Diverts " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " to a secondary input when its primary input overflows.";
			}

			public class GASCONDUITOVERFLOW
			{
				public static LocString NAME = UI.FormatAsLink("Gas Overflow Valve", "GASCONDUITOVERFLOW");

				public static LocString DESC = "Overflow valves can be used to prioritize which buildings should receive precious resources first.";

				public static LocString EFFECT = "Fills a secondary" + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " output only when its primary output is blocked.";
			}

			public class LIQUIDCONDUITOVERFLOW
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Overflow Valve", "LIQUIDCONDUITOVERFLOW");

				public static LocString DESC = "Overflow valves can be used to prioritize which buildings should receive precious resources first.";

				public static LocString EFFECT = "Fills a secondary" + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " output only when its primary output is blocked.";
			}

			public class GASFILTER
			{
				public static LocString NAME = UI.FormatAsLink("Gas Filter", "GASFILTER");

				public static LocString DESC = "Gas filters send the selected filter gas into a special pipe, expelling everything else through a second output.";

				public static LocString EFFECT = "Sieves one " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " from the air, sending it into a dedicated " + UI.FormatAsLink("Pipe", "GASPIPING") + ".";

				public static LocString STATUS_ITEM = "Filters: {0}";

				public static LocString ELEMENT_NOT_SPECIFIED = "Not Specified";
			}

			public class GASPERMEABLEMEMBRANE
			{
				public static LocString NAME = UI.FormatAsLink("Airflow Tile", "GASPERMEABLEMEMBRANE");

				public static LocString DESC = "Building with airflow tile promotes better gas circulation within a colony.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nBlocks " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " flow without obstructing " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".";
			}

			public class GASPUMP
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pump", "GASPUMP");

				public static LocString DESC = "Piping a pump's intake to another building's output will send gas to that building.";

				public static LocString EFFECT = "Draws in " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " runs it through " + UI.FormatAsLink("Pipes", "GASPIPING") + ".\n\nMust be immersed in " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".";
			}

			public class GASMINIPUMP
			{
				public static LocString NAME = UI.FormatAsLink("Mini Gas Pump", "GASMINIPUMP");

				public static LocString DESC = "Mini pumps are useful for moving small quantities of gas with minimum power draw.";

				public static LocString EFFECT = "Draws in a small amount of " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " runs it through " + UI.FormatAsLink("Pipes", "GASPIPING") + ".\n\nMust be immersed in " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".";
			}

			public class GASVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Gas Valve", "GASVALVE");

				public static LocString DESC = "Valves control the amount of gas that moves through pipes, preventing waste.";

				public static LocString EFFECT = "Controls the " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " volume permitted through " + UI.FormatAsLink("Pipes", "GASPIPING") + ".";
			}

			public class GASLOGICVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Gas Shutoff", "GASLOGICVALVE");

				public static LocString DESC = "Automated piping saves time and resources by removing the need for Duplicant management.";

				public static LocString EFFECT = "Automatically turns " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " flow on or off using " + UI.FormatAsLink("Automation", "LOGIC") + " technology.";
			}

			public class GASVENT
			{
				public static LocString NAME = UI.FormatAsLink("Gas Vent", "GASVENT");

				public static LocString DESC = "Vents are an exit point for gases from ventilation systems.";

				public static LocString EFFECT = "Releases " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " from " + UI.FormatAsLink("Gas Pipes", "GASPIPING") + ".";
			}

			public class GASVENTHIGHPRESSURE
			{
				public static LocString NAME = UI.FormatAsLink("High Pressure Gas Vent", "GASVENTHIGHPRESSURE");

				public static LocString DESC = "High pressure Vents can exhaust gas into more highly pressurized environments.";

				public static LocString EFFECT = "Releases " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " from " + UI.FormatAsLink("Gas Pipes", "GASPIPING") + " into high pressure locations.";
			}

			public class GASBOTTLER
			{
				public static LocString NAME = UI.FormatAsLink("Canister Filler", "GASBOTTLER");

				public static LocString DESC = "Canisters allow Duplicants to manually delivery gases from place to place.";

				public static LocString EFFECT = "Automatically stores piped " + UI.FormatAsLink("Gases", "ELEMENTS_GAS") + " into canisters for manual transport.";
			}

			public class GENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Coal Generator", "GENERATOR");

				public static LocString DESC = "Coal generators produce more energy than manual generators, but emit heat and exhaust.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Carbon", "CARBON") + " into electrical " + UI.FormatAsLink("Power", "POWER") + ".\n\nProduces " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + ".";

				public static LocString OVERPRODUCTION = "{Generator} overproduction";
			}

			public class KEROSENEENGINE
			{
				public static LocString NAME = UI.FormatAsLink("Petroleum Engine", "KEROSENEENGINE");

				public static LocString DESC = "Rockets can be used to send Duplicants into space and retrieve rare resources.";

				public static LocString EFFECT = "Burns " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " to propel rockets for space exploration.\n\nThe engine of a rocket must be built first before more rocket modules may be added.";
			}

			public class HYDROGENENGINE
			{
				public static LocString NAME = UI.FormatAsLink("Hydrogen Engine", "HYDROGENENGINE");

				public static LocString DESC = "Hydrogen engines can propel rockets further than steam or petroleum engines.";

				public static LocString EFFECT = "Burns " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + " to propel rockets for space exploration.\n\nThe engine of a rocket must be built first before more rocket modules may be added.";
			}

			public class GENERICFABRICATOR
			{
				public static LocString NAME = UI.FormatAsLink("Omniprinter", "GENERICFABRICATOR");

				public static LocString DESC = "Omniprinters are incapable of printing organic matter.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Raw Mineral", "RAWMINERAL") + " into unique materials and objects.";
			}

			public class GRAVE
			{
				public static LocString NAME = UI.FormatAsLink("Tasteful Memorial", "GRAVE");

				public static LocString DESC = "Burying dead Duplicants reduces health hazards and stress on the colony.";

				public static LocString EFFECT = "Provides a final resting place for deceased Duplicants.\n\nLiving Duplicants will automatically place an unburied corpse inside.";
			}

			public class HEADQUARTERS
			{
				public static LocString NAME = UI.FormatAsLink("Printing Pod", "HEADQUARTERS");

				public static LocString DESC = "New Duplicants will come out here, but thank goodness, they'll never go back in.";

				public static LocString EFFECT = "An exceptionally advanced bioprinter of unknown origin.\n\nIt periodically produces new Duplicants.";
			}

			public class HYDROGENGENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Hydrogen Generator", "HYDROGENGENERATOR");

				public static LocString DESC = "Hydrogen generators are exceptionally efficient power sources and emit next to no waste.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + " into electrical " + UI.FormatAsLink("Power", "POWER") + ".";
			}

			public class METHANEGENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Natural Gas Generator", "METHANEGENERATOR");

				public static LocString DESC = "Natural gas generators leak polluted water and are best built above a waste reservoir.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Natural Gas", "METHANE") + " into electrical " + UI.FormatAsLink("Power", "POWER") + ".\n\nProduces " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " and " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + ".";
			}

			public class PETROLEUMGENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Petroleum Generator", "PETROLEUMGENERATOR");

				public static LocString DESC = "Petroleum generators have a high energy output but produce a great deal of waste.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " into electrical " + UI.FormatAsLink("Power", "POWER") + ".\n\nProduces " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " and " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + ".";
			}

			public class HYDROPONICFARM
			{
				public static LocString NAME = UI.FormatAsLink("Hydroponic Farm", "HYDROPONICFARM");

				public static LocString DESC = "Hydroponic farms reduce Duplicant traffic by automating liquid delivery with irrigation pipes.";

				public static LocString EFFECT = "Grows one " + UI.FormatAsLink("Plant", "PLANTS") + " from a " + UI.FormatAsLink("Seed", "PLANTS") + ".\n\nCan be used as floor tile and rotated before construction.\n\nMust be irrigated through " + UI.FormatAsLink("Liquid Piping", "LIQUIDPIPING") + ".";
			}

			public class INSULATEDGASCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Insulated Gas Pipe", "INSULATEDGASCONDUIT");

				public static LocString DESC = "Pipe insulation prevents gas contents from significantly changing temperature in transit.";

				public static LocString EFFECT = "Carries " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " with minimal change in " + UI.FormatAsLink("Temperature", "HEAT") + ".\n\nCan be run through wall and floor tile.";
			}

			public class GASCONDUITRADIANT
			{
				public static LocString NAME = UI.FormatAsLink("Radiant Gas Pipe", "GASCONDUITRADIANT");

				public static LocString DESC = "Radiant pipes pumping cold gas can be run through hot areas to help cool them down.";

				public static LocString EFFECT = "Carries " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ", allowing extreme " + UI.FormatAsLink("Temperature", "HEAT") + " exchange with the surrounding environment.\n\nCan be run through wall and floor tile.";
			}

			public class INSULATEDLIQUIDCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Insulated Liquid Pipe", "INSULATEDLIQUIDCONDUIT");

				public static LocString DESC = "Pipe insulation prevents liquid contents from significantly changing temperature in transit.";

				public static LocString EFFECT = "Carries " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " with minimal change in " + UI.FormatAsLink("Temperature", "HEAT") + ".\n\nCan be run through wall and floor tile.";
			}

			public class LIQUIDCONDUITRADIANT
			{
				public static LocString NAME = UI.FormatAsLink("Radiant Liquid Pipe", "LIQUIDCONDUITRADIANT");

				public static LocString DESC = "Radiant pipes pumping cold liquid can be run through hot areas to help cool them down.";

				public static LocString EFFECT = "Carries " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ", allowing extreme " + UI.FormatAsLink("Temperature", "HEAT") + " exchange with the surrounding environment.\n\nCan be run through wall and floor tile.";
			}

			public class INSULATEDWIRE
			{
				public static LocString NAME = UI.FormatAsLink("Insulated Wire", "INSULATEDWIRE");

				public static LocString DESC = "This stuff won't go melting if things get heated.";

				public static LocString EFFECT = "Connects buildings to " + UI.FormatAsLink("Power", "POWER") + " sources in extreme " + UI.FormatAsLink("Heat", "HEAT") + ".\n\nCan be run through wall and floor tile.";
			}

			public class INSULATIONTILE
			{
				public static LocString NAME = UI.FormatAsLink("Insulated Tile", "INSULATIONTILE");

				public static LocString DESC = "The lowered thermal conductivity of insulated tile slows heat from passing through them.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nReduces " + UI.FormatAsLink("Heat", "HEAT") + " transfer between walls, retaining ambient heat in an area.";
			}

			public class EXTERIORWALL
			{
				public static LocString NAME = UI.FormatAsLink("Drywall", "EXTERIORWALL");

				public static LocString DESC = "Drywall can be used in conjunction with tile to build airtight rooms on the surface.";

				public static LocString EFFECT = "Prevents " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " and " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " loss in space.\n\nBuilds an insulating backwall behind buildings.";
			}

			public class FARMTILE
			{
				public static LocString NAME = UI.FormatAsLink("Farm Tile", "FARMTILE");

				public static LocString DESC = "Duplicants can deliver fertilizer and liquids to farm tile, accelerating plant growth.";

				public static LocString EFFECT = "Grows one " + UI.FormatAsLink("Plant", "PLANTS") + " from a " + UI.FormatAsLink("Seed", "PLANTS") + ".\n\nCan be used as floor tile and rotated before construction.";
			}

			public class LADDER
			{
				public static LocString NAME = UI.FormatAsLink("Ladder", "LADDER");

				public static LocString DESC = "(That means they climb it.)";

				public static LocString EFFECT = "Enables vertical mobility for Duplicants.";
			}

			public class LADDERFAST
			{
				public static LocString NAME = UI.FormatAsLink("Plastic Ladder", "LADDERFAST");

				public static LocString DESC = "Plastic ladders are mildly antiseptic and can help limit the spread of germs in a colony.";

				public static LocString EFFECT = "Increases Duplicant climbing speed.";
			}

			public class LIQUIDCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pipe", "LIQUIDCONDUIT");

				public static LocString DESC = "Liquid pipes are used to connect the inputs and outputs of plumbed buildings.";

				public static LocString EFFECT = "Carries " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " between " + UI.FormatAsLink("Outputs", "LIQUIDPIPING") + " and " + UI.FormatAsLink("Intakes", "LIQUIDPIPING") + ".\n\nCan be run through wall and floor tile.";
			}

			public class LIQUIDCONDUITBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Bridge", "LIQUIDCONDUITBRIDGE");

				public static LocString DESC = "Separate pipe systems prevent mingled contents from causing building damage.";

				public static LocString EFFECT = "Runs one " + UI.FormatAsLink("Liquid Pipe", "LIQUIDPIPING") + " section over another without joining them.\n\nCan be run through wall and floor tile.";
			}

			public class LIQUIDCOOLEDFAN
			{
				public static LocString NAME = UI.FormatAsLink("Hydrofan", "LIQUIDCOOLEDFAN");

				public static LocString DESC = "A Duplicant can work a hydrofan to temporarily cool small areas as needed.";

				public static LocString EFFECT = "Dissipates a small amount of the " + UI.FormatAsLink("Heat", "HEAT") + ".";
			}

			public class BURNER
			{
				public static LocString NAME = UI.FormatAsLink("Burner", "BURNER");

				public static LocString DESC = "Burners reach incredible temperatures and can quickly melt large areas.";

				public static LocString EFFECT = "Produces a searing " + UI.FormatAsLink("Heat Jet", "HEAT") + " using " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + ".";
			}

			public class CREATURETRAP
			{
				public static LocString NAME = UI.FormatAsLink("Critter Trap", "CREATURETRAP");

				public static LocString DESC = "Traps cannot catch flying critters.";

				public static LocString EFFECT = "Captures a living " + UI.FormatAsLink("Critter", "CRITTERS") + " for transport.\n\nSingle use.";
			}

			public class CREATUREDELIVERYPOINT
			{
				public static LocString NAME = UI.FormatAsLink("Critter Drop-Off", "CREATUREDELIVERYPOINT");

				public static LocString DESC = "Duplicants automatically bring captured critters to these relocation points for release.";

				public static LocString EFFECT = "Releases trapped " + UI.FormatAsLink("Critters", "CRITTERS") + " back into the world.\n\nCan be used multiple times.";
			}

			public class LIQUIDFILTER
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Filter", "LIQUIDFILTER");

				public static LocString DESC = "Liquid filters send the selected liquid into a special pipe, expelling everything else through a second output.";

				public static LocString EFFECT = "Sieves one " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " out of a mix, sending it into a dedicated " + UI.FormatAsLink("Pipe", "LIQUIDPIPING") + ".";
			}

			public class LIQUIDPUMP
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pump", "LIQUIDPUMP");

				public static LocString DESC = "Piping a pump's intake to another building's output will send liquid to that building.";

				public static LocString EFFECT = "Draws in " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " and runs it through " + UI.FormatAsLink("Pipes", "LIQUIDPIPING") + ".\n\nMust be submerged in " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".";
			}

			public class LIQUIDMINIPUMP
			{
				public static LocString NAME = UI.FormatAsLink("Mini Liquid Pump", "LIQUIDMINIPUMP");

				public static LocString DESC = "Mini pumps are useful for moving small quantities of liquid with minimum power draw.";

				public static LocString EFFECT = "Draws in a small amount of " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " and runs it through " + UI.FormatAsLink("Pipes", "LIQUIDPIPING") + ".\n\nMust be submerged in " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".";
			}

			public class LIQUIDPUMPINGSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Pitcher Pump", "LIQUIDPUMPINGSTATION");

				public static LocString DESC = "Pitcher pumps are used to fetch water and other liquids for delivery to buildings.";

				public static LocString EFFECT = "Manually pumps " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " into bottles for transport.\n\nDuplicants can only carry liquids that are bottled.";
			}

			public class LIQUIDVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Valve", "LIQUIDVALVE");

				public static LocString DESC = "Valves control the amount of liquid that moves through pipes, preventing waste.";

				public static LocString EFFECT = "Controls the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " volume permitted through " + UI.FormatAsLink("Pipes", "LIQUIDPIPING") + ".";
			}

			public class LIQUIDLOGICVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Shutoff", "LIQUIDLOGICVALVE");

				public static LocString DESC = "Automated piping saves time and resources by removing the need for Duplicant management.";

				public static LocString EFFECT = "Automatically turns " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " flow on or off using " + UI.FormatAsLink("Automation", "LOGIC") + " technology.";
			}

			public class LIQUIDVENT
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Vent", "LIQUIDVENT");

				public static LocString DESC = "Vents are an exit point for liquids from plumbing systems.";

				public static LocString EFFECT = "Releases " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " from " + UI.FormatAsLink("Liquid Pipes", "LIQUIDPIPING") + ".";
			}

			public class MANUALGENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Manual Generator", "MANUALGENERATOR");

				public static LocString DESC = "Watching Duplicants run on it is adorable... the electrical power is just an added bonus.";

				public static LocString EFFECT = "Converts manual labor into electrical " + UI.FormatAsLink("Power", "POWER") + ".";
			}

			public class MANUALPRESSUREDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Manual Airlock", "MANUALPRESSUREDOOR");

				public static LocString DESC = "Airlocks can quarter off dangerous areas or prevent gases from seeping into the colony.";

				public static LocString EFFECT = "Blocks " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " and " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " flow, maintaining pressure between areas.\n\nSets Duplicant Access Permissions for area restriction.\n\nWild " + UI.FormatAsLink("Critters", "CRITTERS") + " cannot pass through doors.";
			}

			public class MEDICALBED
			{
				public static LocString NAME = UI.FormatAsLink("Pharma Chamber", "MEDICALBED");

				public static LocString DESC = "Pharma Chambers decrease patients' disease recovery time, but cannot heal physical injuries.";

				public static LocString EFFECT = "Greatly accelerates recovery from " + UI.FormatAsLink("Diseases", "DISEASE") + ".";
			}

			public class MESHTILE
			{
				public static LocString NAME = UI.FormatAsLink("Mesh Tile", "MESHTILE");

				public static LocString DESC = "Mesh tile can be used to make Duplicant pathways in areas where liquid needs to flow.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nDoes not obstruct " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " or " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " flow.";
			}

			public class PLASTICTILE
			{
				public static LocString NAME = UI.FormatAsLink("Plastic Tile", "PLASTICTILE");

				public static LocString DESC = "Plastic tile is mildly antiseptic and can help limit the spread of germs in a colony.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nSignificantly increases Duplicant runspeed.";
			}

			public class GLASSTILE
			{
				public static LocString NAME = UI.FormatAsLink("Window Tile", "GLASSTILE");

				public static LocString DESC = "Window tiles provide a barrier against liquid and gas and are completely transparent.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nAllows " + UI.FormatAsLink("Light", "LIGHT") + " and " + UI.FormatAsLink("Decor Values", "DECOR") + " to pass through.";
			}

			public class METALTILE
			{
				public static LocString NAME = UI.FormatAsLink("Metal Tile", "METALTILE");

				public static LocString DESC = "Heat travels much more quickly through metal tile than other types of flooring.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nSignificantly increases Duplicant runspeed.";
			}

			public class BUNKERTILE
			{
				public static LocString NAME = UI.FormatAsLink("Bunker Tile", "BUNKERTILE");

				public static LocString DESC = "Bunker tile can be used to build strong shelters in otherwise dangerous environments.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nCan withstand extreme pressures and impacts.";
			}

			public class CARPETTILE
			{
				public static LocString NAME = UI.FormatAsLink("Carpeted Tile", "CARPETTILE");

				public static LocString DESC = "Carpeted tile is a luxury that remains decorative even when other tile is stacked atop it.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class MOULDINGTILE
			{
				public static LocString NAME = UI.FormatAsLink("Trimming Tile", "MOUDLINGTILE");

				public static LocString DESC = "Trimming is used as purely decorative lining for walls and structures.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class MICROBEMUSHER
			{
				public static LocString NAME = UI.FormatAsLink("Microbe Musher", "MICROBEMUSHER");

				public static LocString DESC = "Food from a musher will keep Duplicants alive, but may cause stress and disease over time.";

				public static LocString EFFECT = "Produces low quality " + UI.FormatAsLink("Food", "FOOD") + " using common ingredients.\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class MINERALDEOXIDIZER
			{
				public static LocString NAME = UI.FormatAsLink("Oxygen Diffuser", "MINERALDEOXIDIZER");

				public static LocString DESC = "Oxygen diffusers are inefficient, but can output enough " + UI.FormatAsLink("Oxygen", "OXYGEN") + " to keep a colony breathing.";

				public static LocString EFFECT = "Converts large amounts of " + UI.FormatAsLink("Algae", "ALGAE") + " into " + UI.FormatAsLink("Oxygen", "OXYGEN") + ".\n\nBecomes idle when the area reaches maximum pressure capacity.";
			}

			public class ORESCRUBBER
			{
				public static LocString NAME = UI.FormatAsLink("Ore Scrubber", "ORESCRUBBER");

				public static LocString DESC = "Ore scrubbers sanitize freshly mined resources before bringing them into the colony.";

				public static LocString EFFECT = "Kills a significant amount of " + UI.FormatAsLink("Germs", "DISEASE") + " present on " + UI.FormatAsLink("Raw Ore", "RAWMINERAL") + ".";
			}

			public class OUTHOUSE
			{
				public static LocString NAME = UI.FormatAsLink("Outhouse", "OUTHOUSE");

				public static LocString DESC = "The colony that eats together, excretes together.";

				public static LocString EFFECT = "Gives Duplicants a place to relieve themselves.\n\nRequires no " + UI.FormatAsLink("Piping", "GASPIPING") + ".\n\nMust be periodically emptied of " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + ".";
			}

			public class APOTHECARY
			{
				public static LocString NAME = UI.FormatAsLink("Apothecary", "APOTHECARY");

				public static LocString DESC = "All-natural remedies to cure what ails you.";

				public static LocString EFFECT = "Produces " + UI.FormatAsLink("Medicine", "MEDICINE") + " to cure most basic " + UI.FormatAsLink("Diseases", "DISEASE") + ".\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class PLANTERBOX
			{
				public static LocString NAME = UI.FormatAsLink("Planter Box", "PLANTERBOX");

				public static LocString DESC = "Domestically grown seeds mature more quickly than wild plants.";

				public static LocString EFFECT = "Grows one " + UI.FormatAsLink("Plant", "PLANTS") + " from a " + UI.FormatAsLink("Seed", "PLANTS") + ".";
			}

			public class PRESSUREDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Mechanized Airlock", "PRESSUREDOOR");

				public static LocString DESC = "Mechanized airlocks have the same function as other doors, but open and close more quickly.";

				public static LocString EFFECT = "Blocks " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " and " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " flow, maintaining pressure between areas.\n\nSets Duplicant Access Permissions for area restriction.\n\nFunctions as a " + UI.FormatAsLink("Manual Airlock", "MANUALPRESSUREDOOR") + " when no " + UI.FormatAsLink("Power", "POWER") + " is available.";
			}

			public class BUNKERDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Bunker Door", "BUNKERDOOR");

				public static LocString DESC = "A massive, slow-moving door which is nearly indestructible.";

				public static LocString EFFECT = "Blocks " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " and " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " flow, maintaining pressure between areas.\n\nSets Duplicant Access Permissions for area restriction.\n\nCan withstand extremely high pressures and impacts.";
			}

			public class RATIONBOX
			{
				public static LocString NAME = UI.FormatAsLink("Ration Box", "RATIONBOX");

				public static LocString DESC = "Ration boxes keep food safe from hungry critters, but don't slow food spoilage.";

				public static LocString EFFECT = "Stores a small amount of " + UI.FormatAsLink("Food", "FOOD") + ".\n\nFood must be delivered to boxes by Duplicants.";
			}

			public class REFRIGERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Refrigerator", "REFRIGERATOR");

				public static LocString DESC = "Food spoilage can be slowed by certain ambient conditions as well as by refrigerators.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Food", "FOOD") + " at an ideal " + UI.FormatAsLink("Temperature", "HEAT") + " to prevent spoilage.";

				public static LocString LOGIC_PORT_DESC = "Full/Not Full";
			}

			public class ROLESTATION
			{
				public static LocString NAME = UI.FormatAsLink("Jobs Board", "ROLESTATION");

				public static LocString DESC = "Employment can permanently improve Duplicants' skills and teach them unique traits.";

				public static LocString EFFECT = "Allows Duplicants to be assigned to specialized " + UI.FormatAsLink("Jobs", "JOBS") + ".\n\nUnlocks " + UI.FormatAsLink("Hats", "JOBS") + ".";
			}

			public class RESEARCHCENTER
			{
				public static LocString NAME = UI.FormatAsLink("Research Station", "RESEARCHCENTER");

				public static LocString DESC = "Research stations are necessary for unlocking all research tiers.";

				public static LocString EFFECT = "Conducts " + UI.FormatAsLink("Novice Research", "RESEARCH") + " to unlock new technologies.";
			}

			public class ADVANCEDRESEARCHCENTER
			{
				public static LocString NAME = UI.FormatAsLink("Super Computer", "ADVANCEDRESEARCHCENTER");

				public static LocString DESC = "Super computers unlock higher technology tiers than research stations alone.";

				public static LocString EFFECT = "Conducts " + UI.FormatAsLink("Advanced Research", "RESEARCH") + " to unlock new technologies.\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Super Computer Researching", UI.StripLinkFormatting(Researcher.ID)) + " trait.";
			}

			public class COSMICRESEARCHCENTER
			{
				public static LocString NAME = UI.FormatAsLink("Virtual Planetarium", "COSMICRESEARCHCENTER");

				public static LocString DESC = "Virtual planetariums allow the simulated exploration of locations discovered with telescopes.";

				public static LocString EFFECT = "Conducts " + UI.FormatAsLink("Interstellar Research", "RESEARCH") + " using data from " + UI.FormatAsLink("Telescopes", "TELESCOPE") + " and " + UI.FormatAsLink("Research Modules", "RESEARCHMODULE") + ".\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Planetarium Researching", UI.StripLinkFormatting(SeniorResearcher.ID)) + " trait.";
			}

			public class TELESCOPE
			{
				public static LocString NAME = UI.FormatAsLink("Telescope", "TELESCOPE");

				public static LocString DESC = "Telescopes are necessary for learning starmaps and conducting rocket missions.";

				public static LocString EFFECT = "Maps Starmap destinations and produces " + UI.FormatAsLink("Interstellar Research", "RESEARCH") + ".\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Geographical Analysis", UI.StripLinkFormatting(Researcher.ID)) + " trait.\n\nBuilding must be exposed to space to function.";

				public static LocString REQUIREMENT_TOOLTIP = "A steady {0} supply is required to sustain working Duplicants.";
			}

			public class SCULPTURE
			{
				public static LocString NAME = UI.FormatAsLink("Large Sculpting Block", "SCULPTURE");

				public static LocString DESC = "Duplicants with high art skill will produce better, more decorative sculptures.";

				public static LocString EFFECT = "Moderately increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";

				public static LocString POORQUALITYNAME = "\"Abstract\" Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Average Sculpture";

				public static LocString EXCELLENTQUALITYNAME = "Genius Sculpture";
			}

			public class ICESCULPTURE
			{
				public static LocString NAME = UI.FormatAsLink("Ice Block", "ICESCULPTURE");

				public static LocString DESC = "Prone to melting.";

				public static LocString EFFECT = "Majorly increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";

				public static LocString POORQUALITYNAME = "\"Abstract\" Ice Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Average Ice Sculpture";

				public static LocString EXCELLENTQUALITYNAME = "Genius Ice Sculpture";
			}

			public class MARBLESCULPTURE
			{
				public static LocString NAME = UI.FormatAsLink("Marble Block", "MARBLESCULPTURE");

				public static LocString DESC = "Duplicants with high art skill will produce better, more decorative sculptures.";

				public static LocString EFFECT = "Majorly increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";

				public static LocString POORQUALITYNAME = "\"Abstract\" Marble Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Average Marble Sculpture";

				public static LocString EXCELLENTQUALITYNAME = "Genius Marble Sculpture";
			}

			public class METALSCULPTURE
			{
				public static LocString NAME = UI.FormatAsLink("Metal Block", "METALSCULPTURE");

				public static LocString DESC = "Duplicants with high art skill will produce better, more decorative sculptures.";

				public static LocString EFFECT = "Majorly increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";

				public static LocString POORQUALITYNAME = "\"Abstract\" Metal Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Average Metal Sculpture";

				public static LocString EXCELLENTQUALITYNAME = "Genius Metal Sculpture";
			}

			public class SMALLSCULPTURE
			{
				public static LocString NAME = UI.FormatAsLink("Sculpting Block", "SMALLSCULPTURE");

				public static LocString DESC = "Duplicants with high art skill will produce better, more decorative sculptures.";

				public static LocString EFFECT = "Minorly increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";

				public static LocString POORQUALITYNAME = "\"Abstract\" Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Average Sculpture";

				public static LocString EXCELLENTQUALITYNAME = "Genius Sculpture";
			}

			public class SHEARINGSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Shearing Station", "SHEARINGSTATION");

				public static LocString DESC = "Shearing stations allow " + UI.FormatAsLink("Dreckos", "DRECKO") + " to be safely sheared for useful raw materials.";

				public static LocString EFFECT = "Allows the assigned Rancher to shear Dreckos.";
			}

			public class SUITMARKER
			{
				public static LocString NAME = UI.FormatAsLink("Atmo Suit Checkpoint", "SUITMARKER");

				public static LocString DESC = "A checkpoint must have an atmo suit dock built on the opposite side its arrow faces.";

				public static LocString EFFECT = "Marks a threshold where Duplicants must change into or out of " + UI.FormatAsLink("Atmo Suits", "ATMO_SUIT") + ".\n\nMust be built next to an " + UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER") + ".\n\nCan be rotated before construction.";
			}

			public class SUITLOCKER
			{
				public static LocString NAME = UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER");

				public static LocString DESC = "Docks can refill atmo suits with air or empty them of waste, but can only charge one suit at a time.";

				public static LocString EFFECT = "Stores and recharges " + UI.FormatAsLink("Atmo Suits", "ATMO_SUIT") + ".\n\nBuild next to an " + UI.FormatAsLink("Atmo Suit Checkpoint", "SUITMARKER") + " to make Duplicants change into suits when passing by.";
			}

			public class JETSUITMARKER
			{
				public static LocString NAME = UI.FormatAsLink("Jet Suit Checkpoint", "JETSUITMARKER");

				public static LocString DESC = "A checkpoint must have a jet suit dock built on the opposite side its arrow faces.";

				public static LocString EFFECT = "Marks a threshold where Duplicants must change into or out of " + UI.FormatAsLink("Jet Suits", "JET_SUIT") + ".\n\nMust be built next to a " + UI.FormatAsLink("Jet Suit Dock", "JETSUITLOCKER") + "\n\nCan be rotated before construction.";
			}

			public class JETSUITLOCKER
			{
				public static LocString NAME = UI.FormatAsLink("Jet Suit Dock", "JETSUITLOCKER");

				public static LocString DESC = "Docks can refill jet suits with air and fuel, or empty them of waste.";

				public static LocString EFFECT = "Stores and refuels " + UI.FormatAsLink("Jet Suits", "JET_SUIT") + " with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and " + UI.FormatAsLink("Petroleum", "PETROLEUM") + ".\n\nBuild next to a " + UI.FormatAsLink("Jet Suit Checkpoint", "JETSUITMARKER") + " to make Duplicants change into suits when passing by.";
			}

			public class SUITFABRICATOR
			{
				public static LocString NAME = UI.FormatAsLink("Exosuit Forge", "SUITFABRICATOR");

				public static LocString DESC = "Exosuits can be filled with oxygen to allow Duplicants to safely enter uninhabitable areas.";

				public static LocString EFFECT = "Forges protective " + UI.FormatAsLink("Exosuits", "EXOSUIT") + " for Duplicants to wear.\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class CLOTHINGFABRICATOR
			{
				public static LocString NAME = UI.FormatAsLink("Textile Loom", "CLOTHINGFABRICATOR");

				public static LocString DESC = "A textile loom can be used to spin Reed Fiber into wearable Duplicant clothing.";

				public static LocString EFFECT = "Tailors Duplicant " + UI.FormatAsLink("Clothing", "EQUIPMENT") + " items.\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class SOLIDBOOSTER
			{
				public static LocString NAME = UI.FormatAsLink("Solid Fuel Thruster", "SOLIDBOOSTER");

				public static LocString DESC = "Additional thrusters allow rockets to reach far away space destinations.";

				public static LocString EFFECT = "Burns " + UI.FormatAsLink("Refined Iron", "IRON") + " and " + UI.FormatAsLink("Oxylite", "OXYROCK") + " to increase rocket exploration distance.";
			}

			public class SPACEHEATER
			{
				public static LocString NAME = UI.FormatAsLink("Space Heater", "SPACEHEATER");

				public static LocString DESC = "A space heater will radiate heat for as long as it's powered.";

				public static LocString EFFECT = "Radiates a moderate amount of " + UI.FormatAsLink("Heat", "HEAT") + ".";
			}

			public class STORAGELOCKER
			{
				public static LocString NAME = UI.FormatAsLink("Storage Bin", "STORAGELOCKER");

				public static LocString DESC = "Resources left on the floor become \"debris\" and lower decor when not put away.";

				public static LocString EFFECT = "Stores the resources of your choosing.";
			}

			public class STORAGELOCKERSMART
			{
				public static LocString NAME = UI.FormatAsLink("Smart Storage Bin", "STORAGELOCKERSMART");

				public static LocString DESC = "Smart storage bins allow for the automation of resource organization based on type and mass.";

				public static LocString EFFECT = "Stores the resources of your choosing.\n\nLogic input becomes " + UI.FormatAsLink("Active", "LOGIC") + " when bin is filled above the set threshold.";
			}

			public class LIQUIDRESERVOIR
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Reservoir", "LIQUIDRESERVOIR");

				public static LocString DESC = "Reservoirs cannot receive manually delivered resources.";

				public static LocString EFFECT = "Stores any " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources piped into it.";
			}

			public class GASRESERVOIR
			{
				public static LocString NAME = UI.FormatAsLink("Gas Reservoir", "GASRESERVOIR");

				public static LocString DESC = "Reservoirs cannot receive manually delivered resources.";

				public static LocString EFFECT = "Stores any " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " resources piped into it.";
			}

			public class LIQUIDHEATER
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Tepidizer", "LIQUIDHEATER");

				public static LocString DESC = "Liquid tepidizers kill waterborne germs and heat liquid to a perfect showering temperature.";

				public static LocString EFFECT = "Warms large bodies of " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".\n\nMust be fully submerged.";
			}

			public class SWITCH
			{
				public static LocString NAME = UI.FormatAsLink("Switch", "SWITCH");

				public static LocString DESC = "Switches do not affect buildings preceding them on a circuit, but control all buildings after.";

				public static LocString EFFECT = "Turns " + UI.FormatAsLink("Power", "POWER") + " on or off.\n\nDoes not affect circuitry preceding the switch.";

				public static LocString TURN_ON = "Turn On";

				public static LocString TURN_OFF = "Turn Off";
			}

			public class LOGICPOWERRELAY
			{
				public static LocString NAME = UI.FormatAsLink("Power Shutoff", "LOGICPOWERRELAY");

				public static LocString DESC = "Automated systems save power and time by removing the need for Duplicant management.";

				public static LocString EFFECT = "Automatically turns " + UI.FormatAsLink("Power", "POWER") + " on or off using " + UI.FormatAsLink("Automation", "LOGIC") + " technology.\n\nDoes not affect circuitry preceding the switch.";

				public static LocString LOGIC_PORT_DESC = "On/Off";
			}

			public class TEMPERATURECONTROLLEDSWITCH
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Switch", "TEMPERATURECONTROLLEDSWITCH");

				public static LocString DESC = "Automated switches can be used to manage circuits in areas where Duplicants cannot enter.";

				public static LocString EFFECT = "Automatically turns " + UI.FormatAsLink("Power", "POWER") + " on or off using ambient " + UI.FormatAsLink("Temperature", "HEAT") + ".\n\nDoes not affect circuitry preceding the switch.";
			}

			public class PRESSURESWITCHLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Hydro Switch", "PRESSURESWITCHLIQUID");

				public static LocString DESC = "A hydro switch shuts off power when the liquid pressure surrounding it surpasses the set threshold.";

				public static LocString EFFECT = "Automatically turns " + UI.FormatAsLink("Power", "POWER") + " on or off using ambient " + UI.FormatAsLink("Liquid Pressure", "PRESSURE") + ".\n\nDoes not affect circuitry preceding the switch.\n\nMust be submerged in " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".";
			}

			public class PRESSURESWITCHGAS
			{
				public static LocString NAME = UI.FormatAsLink("Atmo Switch", "PRESSURESWITCHGAS");

				public static LocString DESC = "An atmo switch shuts off power when the air pressure surrounding it surpasses the set threshold.";

				public static LocString EFFECT = "Automatically turns " + UI.FormatAsLink("Power", "POWER") + " on or off using ambient " + UI.FormatAsLink("Gas", "PRESSURE") + " pressure.\n\nDoes not affect circuitry preceding the switch.";
			}

			public class TILE
			{
				public static LocString NAME = UI.FormatAsLink("Tile", "TILE");

				public static LocString DESC = "Tile can be used to bridge gaps and get to unreachable areas.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nIncreases Duplicant runspeed.";
			}

			public class WATERPURIFIER
			{
				public static LocString NAME = UI.FormatAsLink("Water Sieve", "WATERPURIFIER");

				public static LocString DESC = "Sieves cannot kill germs and pass any disease they receive into their waste and water output.";

				public static LocString EFFECT = "Produces clean " + UI.FormatAsLink("Water", "WATER") + " from " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + " using " + UI.FormatAsLink("Sand", "SAND") + ".\n\nProduces " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + ".";
			}

			public class DISTILLATIONCOLUMN
			{
				public static LocString NAME = UI.FormatAsLink("Distillation Column", "DISTILLATIONCOLUMN");

				public static LocString DESC = "Gets hot and steamy.";

				public static LocString EFFECT = string.Format("Separates any {0} piped through it into " + UI.FormatAsLink("Steam", "STEAM") + " and {1}", ELEMENTS.DIRTYWATER.NAME.text, ELEMENTS.TOXICSAND.NAME.text);
			}

			public class WIRE
			{
				public static LocString NAME = UI.FormatAsLink("Wire", "WIRE");

				public static LocString DESC = "Electrical wire is used to connect generators, batteries, and buildings.";

				public static LocString EFFECT = "Connects buildings to " + UI.FormatAsLink("Power", "POWER") + " sources.\n\nCan be run through wall and floor tile.";
			}

			public class WIREBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Wire Bridge", "WIREBRIDGE");

				public static LocString DESC = "Splitting generators onto separate systems can prevent power overloads and wasted electricity.";

				public static LocString EFFECT = "Runs one wire section over another without joining them.\n\nCan be run through wall and floor tile.";
			}

			public class HIGHWATTAGEWIRE
			{
				public static LocString NAME = UI.FormatAsLink("Heavi-Watt Wire", "HIGHWATTAGEWIRE");

				public static LocString DESC = "Higher wattage wire is used to avoid power overloads, particularly for strong generators.";

				public static LocString EFFECT = "Carries more " + UI.FormatAsLink("Wattage", "POWER") + " than regular " + UI.FormatAsLink("Wire", "WIRE") + " without overloading.\n\nCan be run through wall and floor tile.";
			}

			public class WIREBRIDGEHIGHWATTAGE
			{
				public static LocString NAME = UI.FormatAsLink("Heavi-Watt Joint Plate", "WIREBRIDGEHIGHWATTAGE");

				public static LocString DESC = "Joint plates can run Heavi wires through walls without leaking gas or liquid.";

				public static LocString EFFECT = "Allows " + UI.FormatAsLink("Heavi-Watt Wire", "HIGHWATTAGEWIRE") + " to be run through wall and floor tile.\n\nFunctions as regular tile.";
			}

			public class WIREREFINED
			{
				public static LocString NAME = UI.FormatAsLink("Conductive Wire", "WIREREFINED");

				public static LocString DESC = "My Duplicants prefer the look of conductive wire to the regular raggedy stuff.";

				public static LocString EFFECT = "Connects buildings to " + UI.FormatAsLink("Power", "POWER") + " sources.\n\nCan be run through wall and floor tile.";
			}

			public class WIREREFINEDBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Conductive Wire Bridge", "WIREREFINEDBRIDGE");

				public static LocString DESC = "Splitting generators onto separate systems can prevent power overloads and wasted electricity.";

				public static LocString EFFECT = "Carries more " + UI.FormatAsLink("Wattage", "POWER") + " than a regular " + UI.FormatAsLink("Wire Bridge", "WIREBRIDGE") + " without overloading.\n\nRuns one wire section over another without joining them.\n\nCan be run through wall and floor tile.";
			}

			public class WIREREFINEDHIGHWATTAGE
			{
				public static LocString NAME = UI.FormatAsLink("Heavi Conductive Wire", "WIREREFINEDHIGHWATTAGE");

				public static LocString DESC = "Higher wattage wire is used to avoid power overloads, particularly for strong generators.";

				public static LocString EFFECT = "Carries more " + UI.FormatAsLink("Wattage", "POWER") + " than regular " + UI.FormatAsLink("WIRE", "WIRE") + " without overloading.\n\nCan be run through wall and floor tile.";
			}

			public class WIREREFINEDBRIDGEHIGHWATTAGE
			{
				public static LocString NAME = UI.FormatAsLink("Heavi Conductive Joint Plate", "WIREREFINEDBRIDGEHIGHWATTAGE");

				public static LocString DESC = "Joint plates can run Heavi wires through walls without leaking gas or liquid.";

				public static LocString EFFECT = "Carries more " + UI.FormatAsLink("Wattage", "POWER") + " than a regular " + UI.FormatAsLink("Heavi-Watt Joint Plate", "WIREBRIDGEHIGHWATTAGE") + " without overloading.\n\nAllows " + UI.FormatAsLink("Heavi-Watt Wire", "HIGHWATTAGEWIRE") + " to be run through wall and floor tile.\n\nCan be run through wall and floor tile.";
			}

			public class HANDSANITIZER
			{
				public static LocString NAME = UI.FormatAsLink("Hand Sanitizer", "HANDSANITIZER");

				public static LocString DESC = "Hand sanitizers kill germs more effectively than wash basins.";

				public static LocString EFFECT = "Removes most " + UI.FormatAsLink("Germs", "DISEASE") + " from Duplicants.\n\nGerm-covered Duplicants use Hand Sanitizers when passing by in the selected direction.";
			}

			public class WASHBASIN
			{
				public static LocString NAME = UI.FormatAsLink("Wash Basin", "WASHBASIN");

				public static LocString DESC = "Germ spread can be reduced by building wash basins where Duplicants are likely to get dirty.";

				public static LocString EFFECT = "Removes some " + UI.FormatAsLink("Germs", "DISEASE") + " from Duplicants.\n\nGerm-covered Duplicants use Wash Basins when passing by in the selected direction.";
			}

			public class WASHSINK
			{
				public static LocString NAME = UI.FormatAsLink("Sink", "WASHSINK");

				public static LocString DESC = "Sinks are plumbed and do not need to be manually emptied or refilled.";

				public static LocString EFFECT = "Removes " + UI.FormatAsLink("Germs", "DISEASE") + " from Duplicants.\n\nGerm-covered Duplicants use Sinks when passing by in the selected direction.";
			}

			public class TILEPOI
			{
				public static LocString NAME = UI.FormatAsLink("Tile", "TILEPOI");

				public static LocString DESC = "";

				public static LocString EFFECT = "Used to build the walls and floor of rooms.";
			}

			public class POLYMERIZER
			{
				public static LocString NAME = UI.FormatAsLink("Polymer Press", "POLYMERIZER");

				public static LocString DESC = "Plastic can be used to craft unique buildings and goods.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " into raw " + UI.FormatAsLink("Plastic", "POLYPROPYLENE") + ".";
			}

			public class DIRECTIONALWORLDPUMPLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Channel", "DIRECTIONALWORLDPUMPLIQUID");

				public static LocString DESC = "Channels move more volume than pumps and require no power, but need sufficient pressure to function.";

				public static LocString EFFECT = "Directionally moves large volumes of " + UI.FormatAsLink("LIQUID", "ELEMENTS_LIQUID") + " through a channel.\n\nCan be used as floor tile and rotated before construction.";
			}

			public class STEAMTURBINE
			{
				public static LocString NAME = UI.FormatAsLink("Steam Turbine", "STEAMTURBINE");

				public static LocString DESC = "Useful for converting the geothermal energy of magma into usable power.";

				public static LocString EFFECT = "Generates exceptional electrical " + UI.FormatAsLink("Power", "POWER") + " using pressurized, " + UI.FormatAsLink("Scalding", "HEAT") + " " + UI.FormatAsLink("Steam", "STEAM") + ".\n\nOutputs significantly cooler " + UI.FormatAsLink("Steam", "STEAM") + " than it receives.\n\nAir pressure beneath this building must be higher than pressure above for air to flow.";
			}

			public class STEAMENGINE
			{
				public static LocString NAME = UI.FormatAsLink("Steam Engine", "STEAMENGINE");

				public static LocString DESC = "Rockets can be used to send Duplicants into space and retrieve rare resources.";

				public static LocString EFFECT = "Utilizes " + UI.FormatAsLink("Steam", "STEAM") + " to propel rockets for space exploration.\n\nThe engine of a rocket must be built first before more rocket modules may be added.";
			}

			public class SOLARPANEL
			{
				public static LocString NAME = UI.FormatAsLink("Solar Panel", "SOLARPANEL");

				public static LocString DESC = "Solar panels convert high intensity Sunlight into usable power.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Sunlight", "LIGHT") + " into electrical " + UI.FormatAsLink("Power", "POWER") + ".\n\nMust be exposed to space.";
			}

			public class COMETDETECTOR
			{
				public static LocString NAME = UI.FormatAsLink("Space Scanner", "COMETDETECTOR");

				public static LocString DESC = "Networks of many scanners will detect objects more efficiently than one scanner alone.";

				public static LocString EFFECT = "Sends an " + UI.FormatAsLink("Active Signal", "LOGIC") + " to its logic circuit when it detects incoming objects from space.\n\nCan be configured to detect incoming meteor showers or returning space rockets.";
			}

			public class OILREFINERY
			{
				public static LocString NAME = UI.FormatAsLink("Oil Refinery", "OILREFINERY");

				public static LocString DESC = "Petroleum can only be produced from the refinement of crude oil.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + " into " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " and " + UI.FormatAsLink("Natural Gas", "METHANE") + ".";
			}

			public class OILWELLCAP
			{
				public static LocString NAME = UI.FormatAsLink("Oil Well", "OILWELLCAP");

				public static LocString DESC = "Water pumped into an oil reservoir cannot be recovered.";

				public static LocString EFFECT = "Extracts " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + " using clean " + UI.FormatAsLink("Water", "WATER") + ".\n\nMust be built atop an " + UI.FormatAsLink("Oil Reservoir", "OIL_WELL") + ".";
			}

			public class METALREFINERY
			{
				public static LocString NAME = UI.FormatAsLink("Metal Refinery", "METALREFINERY");

				public static LocString DESC = "Refined metals are necessary to build advanced electronics and technologies.";

				public static LocString EFFECT = "Produces " + UI.FormatAsLink("Refined Metals", "REFINEDMETAL") + " from raw " + UI.FormatAsLink("Metal Ore", "RAWMETAL") + ".\n\nOutputs significantly " + UI.FormatAsLink("Heated", "HEAT") + " " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".\n\nDuplicants will not fabricate items unless recipes are queued.";

				public static LocString RECIPE_DESCRIPTION = "Extracts pure {0} from {1}.";
			}

			public class GLASSFORGE
			{
				public static LocString NAME = UI.FormatAsLink("Glass Forge", "GLASSFORGE");

				public static LocString DESC = "Glass can be used to construct window tile.";

				public static LocString EFFECT = "Produces " + UI.FormatAsLink("Molten Glass", "MOLTENGLASS") + " from raw " + UI.FormatAsLink("Sand", "SAND") + ".\n\nOutputs " + UI.FormatAsLink("High Temperature", "HEAT") + " " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".\n\nDuplicants will not fabricate items unless recipes are queued.";

				public static LocString RECIPE_DESCRIPTION = "Extracts pure {0} from {1}.";
			}

			public class ROCKCRUSHER
			{
				public static LocString NAME = UI.FormatAsLink("Rock Granulator", "ROCKCRUSHER");

				public static LocString DESC = "Sand is used in an array of liquid and air filtering processes.";

				public static LocString EFFECT = "Produces " + UI.FormatAsLink("Sand", "SAND") + " from " + UI.FormatAsLink("Raw Mineral", "RAWMINERAL") + ".\n\nInefficiently produces " + UI.FormatAsLink("Refined Metals", "REFINEDMETAL") + " from raw " + UI.FormatAsLink("Metal Ore", "RAWMETAL") + ".\n\nDuplicants will not fabricate items unless recipes are queued.";

				public static LocString RECIPE_DESCRIPTION = "Crushes {0} into {1}.";

				public static LocString METAL_RECIPE_DESCRIPTION = "Crushes {1} into " + UI.FormatAsLink("Sand", "SAND") + " and pure {0}.";

				public static LocString LIME_RECIPE_DESCRIPTION = "Crushes {1} into {0}";

				public static LocString LIME_FROM_LIMESTONE_RECIPE_DESCRIPTION = "Crushes {0} into {1} and a small amount of pure {2}";
			}

			public class SUPERMATERIALREFINERY
			{
				public static LocString NAME = UI.FormatAsLink("Molecular Forge", "SUPERMATERIALREFINERY");

				public static LocString DESC = "Rare materials can be procured through rocket missions into space.";

				public static LocString EFFECT = "Processes " + UI.FormatAsLink("Rare Materials", "RAREMATERIALS") + " into advanced industrial goods.\n\nRare materials can be retrieved from space missions.\n\nDuplicants will not fabricate items unless recipes are queued.";

				public static LocString SUPERCOOLANT_RECIPE_DESCRIPTION = "Super Coolant is an industrial grade " + UI.FormatAsLink("Fullerene", "FULLERENE") + " coolant.";

				public static LocString SUPERINSULATOR_RECIPE_DESCRIPTION = "Insulation reduces " + UI.FormatAsLink("Heat Transfer", "HEAT") + " and is composed of recrystallized " + UI.FormatAsLink("Abyssalite", "KATAIRITE") + ".";

				public static LocString TEMPCONDUCTORSOLID_RECIPE_DESCRIPTION = "Thermium is an industrial metal alloy formulated to maximize " + UI.FormatAsLink("Heat Transfer", "HEAT") + " and thermal dispersion.";

				public static LocString VISCOGEL_RECIPE_DESCRIPTION = "Visco-Gel is a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " polymer with high surface tension.";
			}

			public class THERMALBLOCK
			{
				public static LocString NAME = UI.FormatAsLink("Tempshift Plate", "THERMALBLOCK");

				public static LocString DESC = "Construction materials have different thermal properties that affect how well they retain heat.";

				public static LocString EFFECT = "Accelerates or buffers " + UI.FormatAsLink("Heat", "HEAT") + " dispersal depending on the construction material used.\n\nHas a small area of effect.";
			}

			public class POWERCONTROLSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Power Control Station", "POWERCONTROLSTATION");

				public static LocString DESC = "Only one Duplicant may be assigned to a Station at a time.";

				public static LocString EFFECT = "Produces " + UI.FormatAsLink("Microchip", "POWER_STATION_TOOLS") + " to increase the " + UI.FormatAsLink("Power", "POWER") + " output of generators.\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Tune Up", UI.StripLinkFormatting("PowerTechnician")) + " trait.\n\nThis building is a necessary component of the Power Plant room.";
			}

			public class FARMSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Farm Station", "FARMSTATION");

				public static LocString DESC = "This station only has an effect on crops grown within the same room.";

				public static LocString EFFECT = "Produces " + UI.FormatAsLink("Micronutrient Fertilizer", "FARM_STATION_TOOLS") + " to increase " + UI.FormatAsLink("Plant", "PLANTS") + " growth rates.\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Crop Tending", UI.StripLinkFormatting("Farmer")) + " trait.\n\nThis building is a necessary component of the Greenhouse room.";
			}

			public class FISHDELIVERYPOINT
			{
				public static LocString NAME = UI.FormatAsLink("Fish Release", "FISHDELIVERYPOINT");

				public static LocString DESC = "This building must be built above liquid to prevent fish from suffocating.";

				public static LocString EFFECT = "Releases trapped " + UI.FormatAsLink("Pacu", "PACU") + " back into the world.\n\nCan be used multiple times.";
			}

			public class FISHFEEDER
			{
				public static LocString NAME = UI.FormatAsLink("Fish Feeder", "FISHFEEDER");

				public static LocString DESC = "Build this feeder above a body of water to feed the fish within.";

				public static LocString EFFECT = "Automatically dispenses stored " + UI.FormatAsLink("Critter", "CRITTERS") + "food into the area below.\n\nDispenses once per day.";
			}

			public class FISHTRAP
			{
				public static LocString NAME = UI.FormatAsLink("Fish Trap", "FISHTRAP");

				public static LocString DESC = "Trapped fish will automatically be bagged for transport.";

				public static LocString EFFECT = "Attracts and traps swimming " + UI.FormatAsLink("Pacu", "PACU") + ".\n\nSingle use.";
			}

			public class RANCHSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Grooming Station", "RANCHSTATION");

				public static LocString DESC = "Grooming critters make them look nice, smell pretty, feel happy, and produce more.";

				public static LocString EFFECT = "Allows the assigned " + UI.FormatAsLink("Rancher", "RANCHER") + " to care for " + UI.FormatAsLink("Critters", "CRITTERS") + ".\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Critter Wrangling", UI.StripLinkFormatting("Rancher")) + " trait.\n\nThis building is a necessary component of the Stable room.";
			}

			public class MACHINESHOP
			{
				public static LocString NAME = UI.FormatAsLink("Mechanics Station", "MACHINESHOP");

				public static LocString DESC = "Duplicants will only improve the efficiency of buildings in the same room as this station.";

				public static LocString EFFECT = "Allows the assigned " + UI.FormatAsLink("Engineer", "MACHINE_TECHNICIAN") + " to improve building production efficiency.\n\nThis building is a necessary component of the Machine Shop room.";
			}

			public class MASSIVEHEATSINK
			{
				public static LocString NAME = UI.FormatAsLink("Anti Entropy Thermo-Nullifier", "MASSIVEHEATSINK");

				public static LocString DESC = "";

				public static LocString EFFECT = "A self-sustaining machine powered by what appears to be refined " + UI.FormatAsLink("Neutronium", "UNOBTANIUM") + ".\n\nAbsorbs and neutralizes " + UI.FormatAsLink("Heat", "HEAT") + " energy when submersed in " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + ".";
			}

			public class LOGICWIRE
			{
				public static LocString NAME = UI.FormatAsLink("Automation Wire", "LOGICWIRE");

				public static LocString DESC = "Automation wire is used to connect electronic components to the inputs of automation gates.";

				public static LocString EFFECT = "Connects automatable buildings to " + UI.FormatAsLink("Automation Gates", "LOGIC") + ".\n\nCan be run through wall and floor tile.";
			}

			public class LOGICWIREBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Automation Wire Bridge", "LOGICWIREBRIDGE");

				public static LocString DESC = "Wire bridges allow multiple automation grids to exist in a small area without connecting.";

				public static LocString EFFECT = "Runs one " + UI.FormatAsLink("Automation Wire", "LOGICWIRE") + " section over another without joining them.\n\nCan be run through wall and floor tile.";
			}

			public class LOGICGATEAND
			{
				public static LocString NAME = UI.FormatAsLink("AND Gate", "LOGICGATEAND");

				public static LocString DESC = "This gate only turns buildings on when both the input buildings are on active the same time.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " when the systems connected to its Inputs are both " + UI.FormatAsLink("Active", "LOGIC") + ".\n\nGoes into " + UI.FormatAsLink("Standby", "LOGIC") + " when one or both Inputs are on " + UI.FormatAsLink("Standby", "LOGIC") + ".";
			}

			public class LOGICGATEOR
			{
				public static LocString NAME = UI.FormatAsLink("OR Gate", "LOGICGATEOR");

				public static LocString DESC = "This gate can only turn buildings on if it has one or more active inputs connected to it.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " if one or both of the systems connected to its Inputs are " + UI.FormatAsLink("Active", "LOGIC") + ".\n\nGoes into " + UI.FormatAsLink("Standby", "LOGIC") + " when neither Inputs are " + UI.FormatAsLink("Active", "LOGIC") + ".";
			}

			public class LOGICGATENOT
			{
				public static LocString NAME = UI.FormatAsLink("NOT Gate", "LOGICGATENOT");

				public static LocString DESC = "This gate reverses logic, turning buildings on when it receives off values and vice versa.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " if the system connected to its Input is on " + UI.FormatAsLink("Standby", "LOGIC") + ".\n\nGoes into " + UI.FormatAsLink("Standby", "LOGIC") + " when its Input is " + UI.FormatAsLink("Active", "LOGIC") + ".";
			}

			public class LOGICGATEXOR
			{
				public static LocString NAME = UI.FormatAsLink("XOR Gate", "LOGICGATEXOR");

				public static LocString DESC = "This gate needs exactly one of its inputs to be active to turn on buildings.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " if one of the systems connected to its Inputs is " + UI.FormatAsLink("Active", "LOGIC") + ".\n\nGoes into " + UI.FormatAsLink("Standby", "LOGIC") + " if both or neither Inputs are " + UI.FormatAsLink("Active", "LOGIC") + ".";
			}

			public class LOGICGATEBUFFER
			{
				public static LocString NAME = UI.FormatAsLink("BUFFER Gate", "LOGICGATEBUFFER");

				public static LocString DESC = "This gate will keep buildings running for a short time after the input building is deactivated.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " if the system connected to its Input is " + UI.FormatAsLink("Active", "LOGIC") + ".\n\nStays " + UI.FormatAsLink("Active", "LOGIC") + " for a short time after its Input enters " + UI.FormatAsLink("Standby", "LOGIC") + ".";
			}

			public class LOGICGATEFILTER
			{
				public static LocString NAME = UI.FormatAsLink("FILTER Gate", "LOGICGATEFILTER");

				public static LocString DESC = "This gate will keep buildings turned off for a short time after the input building is activated.";

				public static LocString EFFECT = "Enters " + UI.FormatAsLink("Standby", "LOGIC") + " if the system connected to its Input is also on " + UI.FormatAsLink("Standby", "LOGIC") + ".\n\nStays in " + UI.FormatAsLink("Standby", "LOGIC") + " for a short time after its Input becomes " + UI.FormatAsLink("Active", "LOGIC") + ".";
			}

			public class LOGICMEMORY
			{
				public static LocString NAME = UI.FormatAsLink("Memory Toggle", "LOGICMEMORY");

				public static LocString DESC = "Once active, a memory toggle keeps buildings on until the reset port receives an active signal.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " when the " + UI.FormatAsLink("Logic", "Set") + " port (S) is " + UI.FormatAsLink("Active", "LOGIC") + ".\n\nGoes to " + UI.FormatAsLink("Standby", "LOGIC") + " if the reset port (R) becomes " + UI.FormatAsLink("Active", "LOGIC") + ".";

				public static LocString STATUS_ITEM_VALUE = "Current Value: {0}";

				public static LocString READ_PORT_DESC = "Current Value";

				public static LocString SET_PORT_DESC = "Make Active";

				public static LocString RESET_PORT_DESC = "Make Inactive";
			}

			public class LOGICSWITCH
			{
				public static LocString NAME = UI.FormatAsLink("Signal Switch", "LOGICSWITCH");

				public static LocString DESC = "Signal switches do not turn grids on and off like power switches, but add an additional signal.";

				public static LocString EFFECT = "Can be made " + UI.FormatAsLink("Active", "LOGIC") + " or " + UI.FormatAsLink("Standby", "LOGIC") + " on an " + UI.FormatAsLink("Automation", "LOGIC") + " grid.\n\nMust be manually toggled by a Duplicant.";

				public static LocString LOGIC_PORT_DESC = "Active/Standby";
			}

			public class LOGICPRESSURESENSORGAS
			{
				public static LocString NAME = UI.FormatAsLink("Atmo Sensor", "LOGICPRESSURESENSORGAS");

				public static LocString DESC = "Atmo Sensors can be used to prevent excess oxygen production and overpressurization.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " or on " + UI.FormatAsLink("Standby", "LOGIC") + " when " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " pressure enters the chosen range.";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on ambient " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " pressure";
			}

			public class LOGICPRESSURESENSORLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Hydro Sensor", "LOGICPRESSURESENSORLIQUID");

				public static LocString DESC = "A hydro sensor can tell a pump to refill its basin as soon as it contains too little liquid.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " or on " + UI.FormatAsLink("Standby", "LOGIC") + " when " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " pressure enters the chosen range.\n\nMust be submerged in " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on ambient " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " Pressure";
			}

			public class LOGICTEMPERATURESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Sensor", "LOGICTEMPERATURESENSOR");

				public static LocString DESC = "Thermo sensors can auto-disable buildings when they approach dangerous temperatures.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " or on " + UI.FormatAsLink("Standby", "LOGIC") + " when ambient " + UI.FormatAsLink("Temperature", "HEAT") + " enters the chosen range.";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on Ambient " + UI.FormatAsLink("Temperature", "HEAT") + "";
			}

			public class LOGICTIMEOFDAYSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Clock Sensor", "LOGICTIMEOFDAYSENSOR");

				public static LocString DESC = "Clock sensors ensure that systems always turn on at the same time, day or night, every cycle.";

				public static LocString EFFECT = "Sets an automatic " + UI.FormatAsLink("Active", "LOGIC") + " and " + UI.FormatAsLink("Standby", "LOGIC") + " schedule using a timer.";
			}

			public class LOGICCRITTERCOUNTSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Critter Sensor", "LOGICCRITTERCOUNTSENSOR");

				public static LocString DESC = "Detecting critter populations can help adjust their automated feeding and care regiments.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " or on " + UI.FormatAsLink("Standby", "LOGIC") + " depending on the number of eggs and critters in a room.";
			}

			public class LOGICDISEASESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Germ Sensor", "LOGICDISEASESENSOR");

				public static LocString DESC = "Automatically detecting germ populations can help block off or clean up dangerous areas.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " or on " + UI.FormatAsLink("Standby", "LOGIC") + " depending on quantity of surrounding " + UI.FormatAsLink("Germs", "DISEASE") + ".";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on ambient " + UI.FormatAsLink("Germs", "DISEASE");
			}

			public class LOGICELEMENTSENSORGAS
			{
				public static LocString NAME = UI.FormatAsLink("Gas Element Sensor", "LOGICELEMENTSENSORGAS");

				public static LocString DESC = "These sensors can detect the presence of a specific gas and alter systems accordingly.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " when the chosen " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " is detected.\n\nRemains on " + UI.FormatAsLink("Standby", "LOGIC") + " when the selected " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " is not present.";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on the ambient " + UI.FormatAsLink("Gas", "ELEMENTS_GAS");
			}

			public class LOGICELEMENTSENSORLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Element Sensor", "LOGICELEMENTSENSORLIQUID");

				public static LocString DESC = "These sensors can detect the presence of a specific liquid and alter systems accordingly.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " when the chosen " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " is detected.\n\nRemains on " + UI.FormatAsLink("Standby", "LOGIC") + " when the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " is not present.";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on the ambient " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID");
			}

			public class GASCONDUITDISEASESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pipe Germ Sensor", "GASCONDUITDISEASESENSOR");

				public static LocString DESC = "Germ sensors are used to control automation behaviour in the presence of disease.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " or on " + UI.FormatAsLink("Standby", "LOGIC") + " depending on the internal " + UI.FormatAsLink("Germ", "DISEASE") + " count of the pipe.";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on internal " + UI.FormatAsLink("Germs", "DISEASE");
			}

			public class LIQUIDCONDUITDISEASESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pipe Germ Sensor", "LIQUIDCONDUITDISEASESENSOR");

				public static LocString DESC = "Germ sensors are used to control automation behaviour in the presence of disease.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " or on " + UI.FormatAsLink("Standby", "LOGIC") + " depending on the internal " + UI.FormatAsLink("Germ", "DISEASE") + " count of the pipe.";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on internal " + UI.FormatAsLink("Germs", "DISEASE");
			}

			public class GASCONDUITELEMENTSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pipe Element Sensor", "GASCONDUITELEMENTSENSOR");

				public static LocString DESC = "Element sensors can be used to detect the presence of a specific gas in a pipe.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " when the chosen " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " is detected.";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on the internal " + UI.FormatAsLink("Gas", "ELEMENTS_GAS");
			}

			public class LIQUIDCONDUITELEMENTSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pipe Element Sensor", "LIQUIDCONDUITELEMENTSENSOR");

				public static LocString DESC = "Element sensors can be used to detect the presence of a specific liquid in a pipe.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " when the chosen " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " is detected.";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on the internal " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID");
			}

			public class GASCONDUITTEMPERATURESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pipe Thermo Sensor", "GASCONDUITTEMPERATURESENSOR");

				public static LocString DESC = "Thermo sensors can disable buildings when their pipe contents reach a certain temperature.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " or on " + UI.FormatAsLink("Standby", "LOGIC") + " when contents enter the chosen " + UI.FormatAsLink("Temperature", "HEAT") + " range.";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on internal " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + UI.FormatAsLink("Temperature", "HEAT");
			}

			public class LIQUIDCONDUITTEMPERATURESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pipe Thermo Sensor", "LIQUIDCONDUITTEMPERATURESENSOR");

				public static LocString DESC = "Thermo sensors can disable buildings when their pipe contents reach a certain temperature.";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " or on " + UI.FormatAsLink("Standby", "LOGIC") + " when contents enter the chosen " + UI.FormatAsLink("Temperature", "HEAT") + " range.";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on internal " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + UI.FormatAsLink("Temperature", "HEAT");
			}

			public class TRAVELTUBEENTRANCE
			{
				public static LocString NAME = UI.FormatAsLink("Transit Tube Access", "TRAVELTUBEENTRANCE");

				public static LocString DESC = "Access points are required for Duplicants to enter tubes, but are not required to exit them.";

				public static LocString EFFECT = "Allows Duplicants to enter the connected " + UI.FormatAsLink("Transit Tube", "TRAVELTUBE") + " system.\n\nStops drawing " + UI.FormatAsLink("Power", "POWER") + " once fully charged.";
			}

			public class TRAVELTUBE
			{
				public static LocString NAME = UI.FormatAsLink("Transit Tube", "TRAVELTUBE");

				public static LocString DESC = "Duplicants will only exit a transit tube when a safe landing area is available beneath it.";

				public static LocString EFFECT = "Quickly transports Duplicants from a " + UI.FormatAsLink("Transit Tube Access", "TRAVELTUBEENTRANCE") + " to the tube's end.\n\nOnly transports Duplicants.";
			}

			public class TRAVELTUBEWALLBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Transit Tube Crossing", "TRAVELTUBEWALLBRIDGE");

				public static LocString DESC = "Tube crossings can run transit tubes through walls without leaking gas or liquid.";

				public static LocString EFFECT = "Allows " + UI.FormatAsLink("Transit Tubes", "TRAVELTUBE") + " to be run through wall and floor tile.\n\nFunctions as regular tile.";
			}

			public class SOLIDCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT");

				public static LocString DESC = "Rails automatically move materials where they'll be needed most, saving Duplicants the walk.";

				public static LocString EFFECT = "Transports " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " on a track between " + UI.FormatAsLink("Conveyor Loader", "SOLIDCONDUITINBOX") + " and " + UI.FormatAsLink("Conveyor Receptacle", "SOLIDCONDUITOUTBOX") + ".\n\nCan be run through wall and floor tile.";
			}

			public class SOLIDCONDUITINBOX
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Loader", "SOLIDCONDUITINBOX");

				public static LocString DESC = "The filters on a loader can be used to determine what materials should be sent down the rail.";

				public static LocString EFFECT = "Loads " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " onto " + UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") + " for transport.\n\nOnly loads the resources of your choosing.";
			}

			public class SOLIDCONDUITOUTBOX
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Receptacle", "SOLIDCONDUITOUTBOX");

				public static LocString DESC = "When materials reach the end of a rail they enter a receptacle to be used by Duplicants.";

				public static LocString EFFECT = "Unloads " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " from a " + UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") + " into storage.";
			}

			public class SOLIDTRANSFERARM
			{
				public static LocString NAME = UI.FormatAsLink("Auto-Sweeper", "SOLIDTRANSFERARM");

				public static LocString DESC = "An auto-sweeper's suction range can be viewed at any time by clicking on the building.";

				public static LocString EFFECT = "Automates " + UI.FormatAsLink("Sweeping", "CHORES") + " and " + UI.FormatAsLink("Supplying", "CHORES") + " errands by sucking up all nearby " + UI.FormatAsLink("Debris", "DECOR") + ".\n\nMaterials are automatically delivered to any " + UI.FormatAsLink("Conveyor Loader", "SOLIDCONDUITINBOX") + ", " + UI.FormatAsLink("Conveyor Receptacle", "SOLIDCONDUITOUTBOX") + ", storage, or buildings within range.";
			}

			public class SOLIDCONDUITBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Bridge", "SOLIDCONDUITBRIDGE");

				public static LocString DESC = "Separating rail systems helps prevent materials from reaching the wrong destinations.";

				public static LocString EFFECT = "Runs one " + UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") + " section over another without joining them.\n\nCan be run through wall and floor tile.";
			}

			public class AUTOMINER
			{
				public static LocString NAME = UI.FormatAsLink("Robo-Miner", "AUTOMINER");

				public static LocString DESC = "A robo-miner's range can be viewed at any time by selecting the building.";

				public static LocString EFFECT = "Automatically digs out all materials in a set range.";
			}

			public class CREATUREFEEDER
			{
				public static LocString NAME = UI.FormatAsLink("Critter Feeder", "CREATUREFEEDER");

				public static LocString DESC = "Critters tend to stay close to their food source and will wander less when given a Feeder.";

				public static LocString EFFECT = "Automatically dispenses food for hungry " + UI.FormatAsLink("Critters", "CRITTERS") + ".";
			}

			public class ITEMPEDESTAL
			{
				public static LocString NAME = UI.FormatAsLink("Pedestal", "ITEMPEDESTAL");

				public static LocString DESC = "Perception can be drastically changed by a bit of thoughtful presentation.";

				public static LocString EFFECT = "Displays a single object, doubling its " + UI.FormatAsLink("Decor", "DECOR") + " value.\n\nObjects with negative decor will gain some positive decor when displayed.";

				public static LocString DISPLAYED_ITEM_FMT = "Displayed {0}";
			}

			public class CROWNMOULDING
			{
				public static LocString NAME = UI.FormatAsLink("Crown Moulding", "CROWNMOULDING");

				public static LocString DESC = "Crown moulding is used as purely decorative trim for ceilings.";

				public static LocString EFFECT = "Used to decorate the ceilings of rooms.\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class CORNERMOULDING
			{
				public static LocString NAME = UI.FormatAsLink("Corner Moulding", "CORNERMOULDING");

				public static LocString DESC = "Corner moulding is used as purely decorative trim for ceiling corners.";

				public static LocString EFFECT = "Used to decorate the ceiling corners of rooms.\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class EGGINCUBATOR
			{
				public static LocString NAME = UI.FormatAsLink("Incubator", "EGGINCUBATOR");

				public static LocString DESC = "Incubators can maintain the ideal internal conditions for several species of critter egg.";

				public static LocString EFFECT = "Incubates " + UI.FormatAsLink("Critter", "CRITTERS") + " eggs until ready to hatch.\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Critter Wrangling", "Rancher") + " trait.";
			}

			public class EGGCRACKER
			{
				public static LocString NAME = UI.FormatAsLink("Egg Cracker", "EGGCRACKER");

				public static LocString DESC = "Raw eggs are an ingredient in some high quality food recipes.";

				public static LocString EFFECT = "Converts viable " + UI.FormatAsLink("Critter", "CRITTERS") + " eggs into cooking ingredients.\n\nCracked Eggs cannot hatch.\n\nDuplicants will not crack eggs unless tasks are queued.";

				public static LocString RECIPE_DESCRIPTION = "Turns {0} into {1}.";

				public static LocString RESULT_DESCRIPTION = "Cracked {0}";
			}

			public class ASTRONAUTTRAININGCENTER
			{
				public static LocString NAME = UI.FormatAsLink("Space Cadet Centrifuge", "ASTRONAUTTRAININGCENTER");

				public static LocString DESC = "Duplicants must complete astronaut training in order to pilot space rockets.";

				public static LocString EFFECT = "Trains Duplicants to become " + UI.FormatAsLink("Astronauts", UI.StripLinkFormatting(Astronaut.ID)) + ".\n\nDuplicants must possess the " + UI.FormatAsLink("Astronaut-in-Training", UI.StripLinkFormatting(AstronautTrainee.ID)) + " trait to receive training.";
			}

			public class POIBUNKEREXTERIORDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Security Door", "POIBUNKEREXTERIORDOOR");

				public static LocString EFFECT = "A strong door with a sophisticated genetic lock.";

				public static LocString DESC = "";
			}

			public class POIDOORINTERNAL
			{
				public static LocString NAME = UI.FormatAsLink("Security Door", "POIDOORINTERNAL");

				public static LocString EFFECT = "A strong door with a sophisticated genetic lock.";

				public static LocString DESC = "";
			}

			public class POIFACILITYDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Lobby Doors", "FACILITYDOOR");

				public static LocString EFFECT = "Large double doors that were once the main entrance to a large facility.";

				public static LocString DESC = "";
			}

			public class VENDINGMACHINE
			{
				public static LocString NAME = "Vending Machine";

				public static LocString DESC = "A pristine " + UI.FormatAsLink("Field Ration", "FIELDRATION") + " dispenser.";
			}

			public class GENESHUFFLER
			{
				public static LocString NAME = "Neural Vacillator";

				public static LocString DESC = "A massive synthetic brain, suspended in saline solution.\n\nThere is a chair attached to the device with room for one person.";
			}

			public class PROPTABLE
			{
				public static LocString NAME = "Table";

				public static LocString DESC = "A table and some chairs.";
			}

			public class PROPDESK
			{
				public static LocString NAME = "Computer Desk";

				public static LocString DESC = "An intact office desk, decorated with several personal belongings and a barely functioning computer.";
			}

			public class PROPFACILITYCHAIR
			{
				public static LocString NAME = "Lobby Chair";

				public static LocString DESC = "A chair where visitors can comfortably wait before their appointments.";
			}

			public class PROPFACILITYCOUCH
			{
				public static LocString NAME = "Lobby Couch";

				public static LocString DESC = "A couch where visitors can comfortably wait before their appointments.";
			}

			public class PROPFACILITYDESK
			{
				public static LocString NAME = "Director's Desk";

				public static LocString DESC = "A spotless desk filled with impeccably organized office supplies.\n\nA photo peeks out from beneath the desk pad, depicting two beaming young women in caps and gowns.\n\nThe photo is quite old.";
			}

			public class PROPFACILITYTABLE
			{
				public static LocString NAME = "Coffee Table";

				public static LocString DESC = "A low coffee table that may have once held old science magazines.";
			}

			public class PROPFACILITYSTATUE
			{
				public static LocString NAME = "Gravitas Monument";

				public static LocString DESC = "A large, modern sculpture that sits in the center of the lobby.\n\nIt's an artistic cross between an hourglass shape and a double helix.";
			}

			public class PROPFACILITYCHANDELIER
			{
				public static LocString NAME = "Chandelier";

				public static LocString DESC = "A large chandelier that hangs from the ceiling.\n\nIt does not appear to function.";
			}

			public class PROPFACILITYGLOBEDROORS
			{
				public static LocString NAME = "Filing Cabinet";

				public static LocString DESC = "A filing cabinet for storing hard copy employee records.\n\nThe contents have been shredded.";
			}

			public class PROPFACILITYDISPLAY1
			{
				public static LocString NAME = "Electronic Display";

				public static LocString DESC = "An electronic display projecting the blueprint of a familiar device.\n\nIt looks like a Printing Pod.";
			}

			public class PROPFACILITYDISPLAY2
			{
				public static LocString NAME = "Electronic Display";

				public static LocString DESC = "An electronic display projecting the blueprint of a familiar device.\n\nIt looks like a Mining Gun.";
			}

			public class PROPFACILITYDISPLAY3
			{
				public static LocString NAME = "Electronic Display";

				public static LocString DESC = "An electronic display projecting the blueprint of a strange device.\n\nPerhaps these displays were used to entice visitors.";
			}

			public class PROPFACILITYTALLPLANT
			{
				public static LocString NAME = "Office Plant";

				public static LocString DESC = "It's survived the vacuum of space by virtue of being plastic.";
			}

			public class PROPFACILITYLAMP
			{
				public static LocString NAME = "Light Fixture";

				public static LocString DESC = "A long light fixture that hangs from the ceiling.\n\nIt does not appear to function.";
			}

			public class PROPFACILITYWALLDEGREE
			{
				public static LocString NAME = "Doctorate Degree";

				public static LocString DESC = "Certification in Applied Physics, awarded in recognition of one \"Jacquelyn A. Stern\".";
			}

			public class PROPFACILITYPAINTING
			{
				public static LocString NAME = "Landscape Portrait";

				public static LocString DESC = "A painting featuring a copse of fir trees and a magnificent mountain range on the horizon.\n\nThe air in the room prickles with the sensation that I'm not meant to be here.";
			}

			public class PROPRECEPTIONDESK
			{
				public static LocString NAME = "Reception Desk";

				public static LocString DESC = "A full coffee cup and a note abandoned mid sentence sit behind the desk.\n\nIt gives me an eerie feeling, as if the receptionist has stepped out and will return any moment.";
			}

			public class PROPELEVATOR
			{
				public static LocString NAME = "Broken Elevator";

				public static LocString DESC = "Out of service.\n\nThe buttons inside indicate it went down more than a dozen floors at one point in time.";
			}

			public class SETLOCKER
			{
				public static LocString NAME = "Locker";

				public static LocString DESC = "A basic metal locker.\n\nIt contains an assortment of personal effects.";
			}

			public class PROPLIGHT
			{
				public static LocString NAME = "Light Fixture";

				public static LocString DESC = "An elegant ceiling lamp, slightly worse for wear.";
			}

			public class PROPLADDER
			{
				public static LocString NAME = "Ladder";

				public static LocString DESC = "A hard plastic ladder.";
			}

			public class PROPSKELETON
			{
				public static LocString NAME = "Model Skeleton";

				public static LocString DESC = "A detailed anatomical model.\n\nIt appears to be made of resin.";
			}

			public class PROPSURFACESATELLITE1
			{
				public static LocString NAME = "Crashed Satellite";

				public static LocString DESC = "All that remains of a once peacefully orbiting satellite.";
			}

			public class PROPSURFACESATELLITE2
			{
				public static LocString NAME = "Wrecked Satellite";

				public static LocString DESC = "All that remains of a once peacefully orbiting satellite.";
			}

			public class PROPSURFACESATELLITE3
			{
				public static LocString NAME = "Crushed Satellite";

				public static LocString DESC = "All that remains of a once peacefully orbiting satellite.";
			}

			public class PROPCLOCK
			{
				public static LocString NAME = "Clock";

				public static LocString DESC = "A simple wall clock.\n\nIt is no longer ticking.";
			}
		}

		public static class DAMAGESOURCES
		{
			public static LocString NOTIFICATION_TOOLTIP = "A {0} sustained damage from {1}";

			public static LocString CONDUIT_CONTENTS_FROZE = "pipe contents becoming too cold";

			public static LocString CONDUIT_CONTENTS_BOILED = "pipe contents becoming too hot";

			public static LocString BUILDING_OVERHEATED = "overheating";

			public static LocString BAD_INPUT_ELEMENT = "receiving an incorrect substance";

			public static LocString MINION_DESTRUCTION = "an angry Duplicant. Rude!";

			public static LocString LIQUID_PRESSURE = "neighboring liquid pressure";

			public static LocString CIRCUIT_OVERLOADED = "an overloaded circuit";

			public static LocString MICROMETEORITE = "micrometeorite";

			public static LocString COMET = "falling space rocks";

			public static LocString ROCKET = "rocket engine";
		}

		public static class AUTODISINFECTABLE
		{
			public static class ENABLE_AUTODISINFECT
			{
				public static LocString NAME = "Enable Disinfect";

				public static LocString TOOLTIP = "Automatically disinfect this building when it becomes contaminated";
			}

			public static class DISABLE_AUTODISINFECT
			{
				public static LocString NAME = "Disable Disinfect";

				public static LocString TOOLTIP = "Do not automatically disinfect this building";
			}

			public static class NO_DISEASE
			{
				public static LocString TOOLTIP = "This building is clean";
			}
		}

		public static class DISINFECTABLE
		{
			public static class ENABLE_DISINFECT
			{
				public static LocString NAME = "Disinfect";

				public static LocString TOOLTIP = "Mark this building for disinfection";
			}

			public static class DISABLE_DISINFECT
			{
				public static LocString NAME = "Cancel Disinfect";

				public static LocString TOOLTIP = "Cancel this disinfect order";
			}

			public static class NO_DISEASE
			{
				public static LocString TOOLTIP = "This building is already clean";
			}
		}

		public static class REPAIRABLE
		{
			public static class ENABLE_AUTOREPAIR
			{
				public static LocString NAME = "Enable Autorepair";

				public static LocString TOOLTIP = "Automatically repair this building when damaged";
			}

			public static class DISABLE_AUTOREPAIR
			{
				public static LocString NAME = "Disable Autorepair";

				public static LocString TOOLTIP = "Only repair this building when ordered";
			}
		}

		public static class AUTOMATABLE
		{
			public static class ENABLE_AUTOMATIONONLY
			{
				public static LocString NAME = "Disable Manual";

				public static LocString TOOLTIP = "This building's storage may be accessed by Auto-Sweepers only" + UI.HORIZONTAL_BR_RULE + "Duplicants will not be permitted to add or remove materials from this building";
			}

			public static class DISABLE_AUTOMATIONONLY
			{
				public static LocString NAME = "Enable Manual";

				public static LocString TOOLTIP = "This building's storage may be accessed by both Duplicants and Auto-Sweeper buildings";
			}
		}
	}
}
