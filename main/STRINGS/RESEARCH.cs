namespace STRINGS
{
	public class RESEARCH
	{
		public class MESSAGING
		{
			public static LocString NORESEARCHSELECTED = "No research selected";

			public static LocString RESEARCHTYPEREQUIRED = "{0} required";

			public static LocString RESEARCHTYPEALSOREQUIRED = "{0} also required";

			public static LocString NO_RESEARCHER_SKILL = "No Researchers assigned";

			public static LocString NO_RESEARCHER_SKILL_TOOLTIP = "The selected research focus requires an advanced type of research to complete\n\nAssign a Duplicant to the Research Assistant job using the " + UI.PRE_KEYWORD + "Jobs Panel" + UI.PST_KEYWORD + " " + UI.FormatAsHotkey("[J]") + " to enable this errand";

			public static LocString MISSING_RESEARCH_STATION = "Missing Research Station";

			public static LocString MISSING_RESEARCH_STATION_TOOLTIP = "The selected research focus requires a {0} to perform\n\nOpen the " + UI.FormatAsBuildMenuTab("Stations Tab") + " " + UI.FormatAsHotkey("[0]") + " of the Build Menu to construct one";
		}

		public class TYPES
		{
			public class ALPHA
			{
				public static LocString NAME = "Novice Research";

				public static LocString DESC = UI.FormatAsLink("Novice Research", "RESEARCH") + " is required to unlock basic technologies.\nIt can be conducted at a " + BUILDINGS.PREFABS.RESEARCHCENTER.NAME + ".";

				public static LocString RECIPEDESC = "Unlocks rudimentary technologies.";
			}

			public class BETA
			{
				public static LocString NAME = "Advanced Research";

				public static LocString DESC = UI.FormatAsLink("Advanced Research", "RESEARCH") + " is required to unlock improved technologies.\nIt can be conducted at a " + BUILDINGS.PREFABS.ADVANCEDRESEARCHCENTER.NAME + ".";

				public static LocString RECIPEDESC = "Unlocks improved technologies.";
			}

			public class GAMMA
			{
				public static LocString NAME = "Interstellar Research";

				public static LocString DESC = UI.FormatAsLink("Interstellar Research", "RESEARCH") + " is required to unlock space technologies.\nIt can be conducted at a " + BUILDINGS.PREFABS.COSMICRESEARCHCENTER.NAME + ".";

				public static LocString RECIPEDESC = "Unlocks cutting-edge technologies.";
			}
		}

		public class OTHER_TECH_ITEMS
		{
			public class AUTOMATION_OVERLAY
			{
				public static LocString NAME = "Automation Overlay";

				public static LocString DESC = "Enables access to the Automation Overlay.";
			}

			public class SUITS_OVERLAY
			{
				public static LocString NAME = "Exosuit Overlay";

				public static LocString DESC = "Enables access to the Exosuit Overlay.";
			}

			public class JET_SUIT
			{
				public static LocString NAME = "Jet Suit Pattern";

				public static LocString DESC = "Enables fabrication of Jet Suits at the Exosuit Forge.";
			}

			public class BETA_RESEARCH_POINT
			{
				public static LocString NAME = "Advanced Research Capability";

				public static LocString DESC = "Allows Advanced Research Points to be accumulated, unlocking higher technology tiers.";
			}

			public class GAMMA_RESEARCH_POINT
			{
				public static LocString NAME = "Interstellar Research Capability";

				public static LocString DESC = "Allows Interstellar Research Points to be accumulated, unlocking higher technology tiers.";
			}

			public class CONVEYOR_OVERLAY
			{
				public static LocString NAME = "Conveyor Overlay";

				public static LocString DESC = "Enables access to the Conveyor Overlay.";
			}
		}

		public class TREES
		{
			public static LocString TITLE_FOOD = "Food";

			public static LocString TITLE_POWER = "Power";

			public static LocString TITLE_SOLIDS = "Solid Material";

			public static LocString TITLE_COLONYDEVELOPMENT = "Colony Development";

			public static LocString TITLE_MEDICINE = "Medicine";

			public static LocString TITLE_LIQUIDS = "Liquids";

			public static LocString TITLE_GASES = "Gases";

			public static LocString TITLE_SUITS = "Exosuits";

			public static LocString TITLE_DECOR = "Decor";

			public static LocString TITLE_COMPUTERS = "Computers";

			public static LocString TITLE_ROCKETS = "Rocketry";
		}

		public class TECHS
		{
			public class JOBS
			{
				public static LocString NAME = UI.FormatAsLink("Employment", "JOBS");

				public static LocString DESC = "Exchange the skill points earned by Duplicants for new traits and abilities.";
			}

			public class IMPROVEDOXYGEN
			{
				public static LocString NAME = UI.FormatAsLink("Air Systems", "IMPROVEDOXYGEN");

				public static LocString DESC = "Maintain clean, breathable air in the colony.";
			}

			public class FARMINGTECH
			{
				public static LocString NAME = UI.FormatAsLink("Basic Farming", "FARMINGTECH");

				public static LocString DESC = "Learn the introductory principles of " + UI.FormatAsLink("Plant", "PLANTS") + " domestication.";
			}

			public class AGRICULTURE
			{
				public static LocString NAME = UI.FormatAsLink("Agriculture", "AGRICULTURE");

				public static LocString DESC = "Master the agricultural art of crop raising.";
			}

			public class RANCHING
			{
				public static LocString NAME = UI.FormatAsLink("Ranching", "RANCHING");

				public static LocString DESC = "Tame and care for wild critters.";
			}

			public class ANIMALCONTROL
			{
				public static LocString NAME = UI.FormatAsLink("Animal Control", "ANIMALCONTROL");

				public static LocString DESC = "Useful techniques to manage critter populations in the colony.";
			}

			public class FINEDINING
			{
				public static LocString NAME = UI.FormatAsLink("Meal Preparation", "FINEDINING");

				public static LocString DESC = "Prepare more nutritious " + UI.FormatAsLink("Food", "FOOD") + " and store it longer before spoiling.";
			}

			public class FINERDINING
			{
				public static LocString NAME = UI.FormatAsLink("Gourmet Meal Preparation", "FINERDINING");

				public static LocString DESC = "Raise colony Morale by cooking the most delicious, high-quality " + UI.FormatAsLink("Foods", "FOOD") + ".";
			}

			public class GASPIPING
			{
				public static LocString NAME = UI.FormatAsLink("Ventilation", "GASPIPING");

				public static LocString DESC = "Rudimentary technologies for installing " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " infrastructure.";
			}

			public class IMPROVEDGASPIPING
			{
				public static LocString NAME = UI.FormatAsLink("Improved Ventilation", "IMPROVEDGASPIPING");

				public static LocString DESC = UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " infrastructure capable of withstanding more intense conditions, such as " + UI.FormatAsLink("Heat", "Heat") + " and pressure.";
			}

			public class TEMPERATUREMODULATION
			{
				public static LocString NAME = UI.FormatAsLink("Temperature Modulation", "TEMPERATUREMODULATION");

				public static LocString DESC = "Precise " + UI.FormatAsLink("Temperature", "HEAT") + " altering technologies to keep my colony at the perfect Kelvin.";
			}

			public class HVAC
			{
				public static LocString NAME = UI.FormatAsLink("HVAC", "HVAC");

				public static LocString DESC = "Regulate " + UI.FormatAsLink("Temperature", "HEAT") + " in the colony for " + UI.FormatAsLink("Plant", "PLANTS") + " cultivation and Duplicant comfort.";
			}

			public class LIQUIDTEMPERATURE
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Tuning", "LIQUIDTEMPERATURE");

				public static LocString DESC = "Easily manipulate " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " " + UI.FormatAsLink("Heat", "Temperatures") + " with these temperature regulating technologies.";
			}

			public class INSULATION
			{
				public static LocString NAME = UI.FormatAsLink("Insulation", "INSULATION");

				public static LocString DESC = "Improve " + UI.FormatAsLink("Heat", "Heat") + " distribution within the colony and guard buildings from extreme temperatures.";
			}

			public class PRESSUREMANAGEMENT
			{
				public static LocString NAME = UI.FormatAsLink("Pressure Management", "PRESSUREMANAGEMENT");

				public static LocString DESC = "Unlock technologies to manage colony pressure and atmosphere.";
			}

			public class DIRECTEDAIRSTREAMS
			{
				public static LocString NAME = UI.FormatAsLink("Decontamination", "DIRECTEDAIRSTREAMS");

				public static LocString DESC = "Instruments to help reduce " + UI.FormatAsLink("Germ", "DISEASE") + " spread within the base.";
			}

			public class LIQUIDFILTERING
			{
				public static LocString NAME = UI.FormatAsLink("Liquid-Based Refinement Processes", "LIQUIDFILTERING");

				public static LocString DESC = "Use pumped liquids to remove " + UI.FormatAsLink("Salt", "SALT") + " from " + UI.FormatAsLink("Brine", "BRINE") + " or pull " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " from the air.";
			}

			public class LIQUIDPIPING
			{
				public static LocString NAME = UI.FormatAsLink("Plumbing", "LIQUIDPIPING");

				public static LocString DESC = "Rudimentary technologies for installing " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " infrastructure.";
			}

			public class IMPROVEDLIQUIDPIPING
			{
				public static LocString NAME = UI.FormatAsLink("Improved Plumbing", "IMPROVEDLIQUIDPIPING");

				public static LocString DESC = UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " infrastructure capable of withstanding more intense conditions, such as " + UI.FormatAsLink("Heat", "Heat") + " and pressure.";
			}

			public class PRECISIONPLUMBING
			{
				public static LocString NAME = UI.FormatAsLink("Advanced Caffeination", "PRECISIONPLUMBING");

				public static LocString DESC = "Let Duplicants relax after a long day of subterranean digging with a shot of warm beanjuice.";
			}

			public class SANITATIONSCIENCES
			{
				public static LocString NAME = UI.FormatAsLink("Sanitation", "SANITATIONSCIENCES");

				public static LocString DESC = "Make daily ablutions less of a hassle.";
			}

			public class MEDICINEI
			{
				public static LocString NAME = UI.FormatAsLink("Pharmacology", "MEDICINEI");

				public static LocString DESC = "Compound natural cures to fight the most common " + UI.FormatAsLink("Sicknesses", "SICKNESSES") + " that plague Duplicants.";
			}

			public class MEDICINEII
			{
				public static LocString NAME = UI.FormatAsLink("Medical Equipment", "MEDICINEII");

				public static LocString DESC = "The basic necessities doctors need to facilitate patient care.";
			}

			public class MEDICINEIII
			{
				public static LocString NAME = UI.FormatAsLink("Pathogen Diagnostics", "MEDICINEIII");

				public static LocString DESC = "Stop Germs at the source using special medical automation technology.";
			}

			public class MEDICINEIV
			{
				public static LocString NAME = UI.FormatAsLink("Micro-Targeted Medicine", "MEDICINEIV");

				public static LocString DESC = "State of the art equipment to conquer the most stubborn of illnesses.";
			}

			public class ADVANCEDFILTRATION
			{
				public static LocString NAME = UI.FormatAsLink("Filtration", "ADVANCEDFILTRATION");

				public static LocString DESC = "Basic technologies for filtering " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " and " + UI.FormatAsLink("Gases", "ELEMENTS_GAS") + ".";
			}

			public class POWERREGULATION
			{
				public static LocString NAME = UI.FormatAsLink("Power Regulation", "POWERREGULATION");

				public static LocString DESC = "Prevent wasted " + UI.FormatAsLink("Power", "POWER") + " with improved electrical tools.";
			}

			public class COMBUSTION
			{
				public static LocString NAME = UI.FormatAsLink("Internal Combustion", "COMBUSTION");

				public static LocString DESC = "Fuel-powered generators for crude yet powerful " + UI.FormatAsLink("Power", "POWER") + " production.";
			}

			public class IMPROVEDCOMBUSTION
			{
				public static LocString NAME = UI.FormatAsLink("Fossil Fuels", "IMPROVEDCOMBUSTION");

				public static LocString DESC = "Burn dirty fuels for exceptional " + UI.FormatAsLink("Power", "POWER") + " production.";
			}

			public class INTERIORDECOR
			{
				public static LocString NAME = UI.FormatAsLink("Interior Decor", "INTERIORDECOR");

				public static LocString DESC = UI.FormatAsLink("Decor", "DECOR") + " boosting items to counteract the gloom of underground living.";
			}

			public class ARTISTRY
			{
				public static LocString NAME = UI.FormatAsLink("Artistic Expression", "ARTISTRY");

				public static LocString DESC = "Majorly improve " + UI.FormatAsLink("Decor", "DECOR") + " by giving Duplicants the tools of artistic and emotional expression.";
			}

			public class CLOTHING
			{
				public static LocString NAME = UI.FormatAsLink("Textile Production", "CLOTHING");

				public static LocString DESC = "Bring Duplicants the " + UI.FormatAsLink("Morale", "MORALE") + " boosting benefits of soft, cushy fabrics.";
			}

			public class ACOUSTICS
			{
				public static LocString NAME = UI.FormatAsLink("Sound Amplifiers", "ACOUSTICS");

				public static LocString DESC = "Precise control of the audio spectrum allows Duplicants to get funky.";
			}

			public class AMPLIFIERS
			{
				public static LocString NAME = UI.FormatAsLink("Power Amplifiers", "AMPLIFIERS");

				public static LocString DESC = "Further increased efficacy of " + UI.FormatAsLink("Power", "POWER") + " management to prevent those wasted joules.";
			}

			public class LUXURY
			{
				public static LocString NAME = UI.FormatAsLink("Home Luxuries", "LUXURY");

				public static LocString DESC = "Luxury amenities for advanced " + UI.FormatAsLink("Stress", "STRESS") + " reduction.";
			}

			public class FINEART
			{
				public static LocString NAME = UI.FormatAsLink("Fine Art", "FINEART");

				public static LocString DESC = "Broader options for artistic " + UI.FormatAsLink("Decor", "DECOR") + " improvements.";
			}

			public class REFRACTIVEDECOR
			{
				public static LocString NAME = UI.FormatAsLink("High Culture", "REFRACTIVEDECOR");

				public static LocString DESC = "New methods for working with extremely high quality art materials.";
			}

			public class RENAISSANCEART
			{
				public static LocString NAME = UI.FormatAsLink("Renaissance Art", "RENAISSANCEART");

				public static LocString DESC = "The kind of art that culture legacies are made of.";
			}

			public class GLASSFURNISHINGS
			{
				public static LocString NAME = UI.FormatAsLink("Glass Blowing", "GLASSFURNISHINGS");

				public static LocString DESC = "The decorative benefits of glass are both apparent and transparent.";
			}

			public class ADVANCEDPOWERREGULATION
			{
				public static LocString NAME = UI.FormatAsLink("Advanced Power Regulation", "ADVANCEDPOWERREGULATION");

				public static LocString DESC = "Circuit components required for large scale " + UI.FormatAsLink("Power", "POWER") + " management.";
			}

			public class PLASTICS
			{
				public static LocString NAME = UI.FormatAsLink("Plastic Manufacturing", "PLASTICS");

				public static LocString DESC = "Stable, lightweight, durable. Plastics are useful for a wide array of applications.";
			}

			public class SUITS
			{
				public static LocString NAME = UI.FormatAsLink("Hazard Protection", "SUITS");

				public static LocString DESC = "Vital gear for surviving in extreme conditions and environments.";
			}

			public class DISTILLATION
			{
				public static LocString NAME = UI.FormatAsLink("Distillation", "DISTILLATION");

				public static LocString DESC = "Distill difficult mixtures down to their most useful parts.";
			}

			public class CATALYTICS
			{
				public static LocString NAME = UI.FormatAsLink("Catalytics", "CATALYTICS");

				public static LocString DESC = "Advanced gas manipulation using unique catalysts.";
			}

			public class ADVANCEDRESEARCH
			{
				public static LocString NAME = UI.FormatAsLink("Advanced Research", "ADVANCEDRESEARCH");

				public static LocString DESC = "The tools my colony needs to conduct more advanced, in-depth research.";
			}

			public class LOGICCONTROL
			{
				public static LocString NAME = UI.FormatAsLink("Smart Home", "LOGICCONTROL");

				public static LocString DESC = "Switches that grant full control of building operations within the colony.";
			}

			public class LOGICCIRCUITS
			{
				public static LocString NAME = UI.FormatAsLink("Advanced Automation", "LOGICCIRCUITS");

				public static LocString DESC = "The only limit to colony automation is my own imagination.";
			}

			public class VALVEMINIATURIZATION
			{
				public static LocString NAME = UI.FormatAsLink("Valve Miniaturization", "VALVEMINIATURIZATION");

				public static LocString DESC = "Smaller, more efficient pumps for those low-throughput situations.";
			}

			public class PRETTYGOODCONDUCTORS
			{
				public static LocString NAME = UI.FormatAsLink("Low-Resistance Conductors", "PRETTYGOODCONDUCTORS");

				public static LocString DESC = "Pure-core wires that can handle more " + UI.FormatAsLink("Electrical", "POWER") + " current without overloading.";
			}

			public class RENEWABLEENERGY
			{
				public static LocString NAME = UI.FormatAsLink("Renewable Energy", "RENEWABLEENERGY");

				public static LocString DESC = "Clean, sustainable " + UI.FormatAsLink("Power", "POWER") + " production that produces little to no waste.";
			}

			public class BASICREFINEMENT
			{
				public static LocString NAME = UI.FormatAsLink("Brute-Force Refinement", "BASICREFINEMENT");

				public static LocString DESC = "Low-tech refinement methods for producing clay and renewable sources of sand.";
			}

			public class METALREFINEMENT
			{
				public static LocString NAME = UI.FormatAsLink("", "METALREFINEMENT");

				public static LocString DESC = "";
			}

			public class REFINEDOBJECTS
			{
				public static LocString NAME = UI.FormatAsLink("Refined Renovations", "REFINEDOBJECTS");

				public static LocString DESC = "Improve base infrastructure with new objects crafted from " + UI.FormatAsLink("Refined Metals", "REFINEDMETAL") + ".";
			}

			public class GENERICSENSORS
			{
				public static LocString NAME = UI.FormatAsLink("Generic Sensors", "GENERICSENSORS");

				public static LocString DESC = "Drive automation in a variety of new, inventive ways.";
			}

			public class DUPETRAFFICCONTROL
			{
				public static LocString NAME = UI.FormatAsLink("Computing", "DUPETRAFFICCONTROL");

				public static LocString DESC = "Virtually extend the boundaries of Duplicant imagination.";
			}

			public class SMELTING
			{
				public static LocString NAME = UI.FormatAsLink("Smelting", "SMELTING");

				public static LocString DESC = "High temperatures facilitate the production of purer, special use metal resources.";
			}

			public class TRAVELTUBES
			{
				public static LocString NAME = UI.FormatAsLink("Transit Tubes", "TRAVELTUBES");

				public static LocString DESC = "A wholly futuristic way to move Duplicants around the base.";
			}

			public class SMARTSTORAGE
			{
				public static LocString NAME = UI.FormatAsLink("Smart Storage", "SMARTSTORAGE");

				public static LocString DESC = "Completely automate the storage of solid resources.";
			}

			public class SOLIDTRANSPORT
			{
				public static LocString NAME = UI.FormatAsLink("Solid Transport", "SOLIDTRANSPORT");

				public static LocString DESC = "Free Duplicants from the drudgery of day-to-day material deliveries with new methods of automation.";
			}

			public class HIGHTEMPFORGING
			{
				public static LocString NAME = UI.FormatAsLink("Superheated Forging", "HIGHTEMPFORGING");

				public static LocString DESC = "Craft entirely new materials by harnessing the most extreme temperatures.";
			}

			public class SKYDETECTORS
			{
				public static LocString NAME = UI.FormatAsLink("Celestial Detection", "SKYDETECTORS");

				public static LocString DESC = "Turn Duplicants' eyes to the skies and discover what undiscovered wonders await out there.";
			}

			public class JETPACKS
			{
				public static LocString NAME = UI.FormatAsLink("Jetpacks", "JETPACKS");

				public static LocString DESC = "Objectively the most stylish way for Duplicants to get around.";
			}

			public class BASICROCKETRY
			{
				public static LocString NAME = UI.FormatAsLink("Introductory Rocketry", "BASICROCKETRY");

				public static LocString DESC = "Everything required for launching the colony's very first space program.";
			}

			public class ENGINESI
			{
				public static LocString NAME = UI.FormatAsLink("Solid Fuel Combustion", "ENGINESI");

				public static LocString DESC = "Rockets that fly further, longer.";
			}

			public class ENGINESII
			{
				public static LocString NAME = UI.FormatAsLink("Hydrocarbon Combustion", "ENGINESII");

				public static LocString DESC = "Delve deeper into the vastness of space than ever before.";
			}

			public class ENGINESIII
			{
				public static LocString NAME = UI.FormatAsLink("Cryofuel Combustion", "ENGINESIII");

				public static LocString DESC = "With this technology, the sky is your oyster. Go exploring!";
			}

			public class CARGOI
			{
				public static LocString NAME = UI.FormatAsLink("Solid Cargo", "CARGOI");

				public static LocString DESC = "Make extra use of journeys into space by mining and storing useful resources.";
			}

			public class CARGOII
			{
				public static LocString NAME = UI.FormatAsLink("Liquid and Gas Cargo", "CARGOII");

				public static LocString DESC = "Mine precious liquids and gases from the far reaches of space, and return with them to the colony.";
			}

			public class CARGOIII
			{
				public static LocString NAME = UI.FormatAsLink("Unique Cargo", "CARGOIII");

				public static LocString DESC = "Allow Duplicants to bring mementos from space back to their friends, or take them to the see the stars firsthand.";
			}

			public class INDUSTRIALSTORAGE
			{
				public static LocString NAME = UI.FormatAsLink("", "INDUSTRIALSTORAGE");

				public static LocString DESC = "";
			}
		}
	}
}
