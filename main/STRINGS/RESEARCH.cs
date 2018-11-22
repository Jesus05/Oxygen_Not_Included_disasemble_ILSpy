namespace STRINGS
{
	public class RESEARCH
	{
		public class MESSAGING
		{
			public static LocString NORESEARCHSELECTED = "No research selected";

			public static LocString RESEARCHTYPEREQUIRED = "{0} required";

			public static LocString RESEARCHTYPEALSOREQUIRED = "{0} also required";

			public static LocString NO_RESEARCHER_ROLE = "No Researchers assigned";

			public static LocString NO_RESEARCHER_ROLE_TOOLTIP = "The selected research focus requires an advanced type of research to complete\n\nAssign a Duplicant to the Research Assistant job using the Jobs Panel <color=#F44A47>[J]</color> to enable this errand";

			public static LocString MISSING_RESEARCH_STATION = "Missing Research Station";

			public static LocString MISSING_RESEARCH_STATION_TOOLTIP = "The selected research focus requires a {0} to perform\n\nOpen the Stations Tab <color=#F44A47>[=]</color> of the Build Menu to construct one";
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

		public class TECHS
		{
			public class JOBS
			{
				public static LocString NAME = UI.FormatAsLink("Jobs", "JOBS");

				public static LocString DESC = "Assign Duplicants jobs";
			}

			public class IMPROVEDOXYGEN
			{
				public static LocString NAME = UI.FormatAsLink("Air Systems", "IMPROVEDOXYGEN");

				public static LocString DESC = "Make the air in the colony clean and breathable.";
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

				public static LocString DESC = "Manage and care for critters.";
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

				public static LocString DESC = "Perfect " + UI.FormatAsLink("Temperature", "HEAT") + " changing technologies to keep my colony at the perfect Kelvin.";
			}

			public class HVAC
			{
				public static LocString NAME = UI.FormatAsLink("HVAC", "HVAC");

				public static LocString DESC = "Regulate " + UI.FormatAsLink("Temperature", "HEAT") + " in the colony for " + UI.FormatAsLink("Plant", "PLANTS") + " cultivation and Duplicant comfort.";
			}

			public class LIQUIDTEMPERATURE
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Tuning", "LIQUIDTEMPERATURE");

				public static LocString DESC = "Easily manipulate " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " " + UI.FormatAsLink("heat", "Temperatures") + " with these technologies.";
			}

			public class INSULATION
			{
				public static LocString NAME = UI.FormatAsLink("Insulation", "INSULATION");

				public static LocString DESC = "Improve " + UI.FormatAsLink("Heat", "Heat") + " distribution in my base and guard buildings from extreme temperatures.";
			}

			public class PRESSUREMANAGEMENT
			{
				public static LocString NAME = UI.FormatAsLink("Pressure Management", "PRESSUREMANAGEMENT");

				public static LocString DESC = "Unlock technologies to manage colony pressure and atmosphere.";
			}

			public class DIRECTEDAIRSTREAMS
			{
				public static LocString NAME = UI.FormatAsLink("Decontamination", "DIRECTEDAIRSTREAMS");

				public static LocString DESC = "Instruments to help reduce " + UI.FormatAsLink("Germ", "DISEASE") + " spread in my base.";
			}

			public class LIQUIDPIPING
			{
				public static LocString NAME = UI.FormatAsLink("Plumbing", "LIQUIDPIPING");

				public static LocString DESC = "Rudimentary technologies for installing " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " infrastructure.";
			}

			public class LUXURY
			{
				public static LocString NAME = UI.FormatAsLink("Home Luxuries", "LUXURY");

				public static LocString DESC = "Luxury amenities for advanced " + UI.FormatAsLink("Stress", "STRESS") + " reduction.";
			}

			public class IMPROVEDLIQUIDPIPING
			{
				public static LocString NAME = UI.FormatAsLink("Improved Plumbing", "IMPROVEDLIQUIDPIPING");

				public static LocString DESC = UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " infrastructure capable of withstanding more intense conditions, such as " + UI.FormatAsLink("Heat", "Heat") + " and pressure.";
			}

			public class PRECISIONPLUMBING
			{
				public static LocString NAME = UI.FormatAsLink("Precision Plumbing", "PRECISIONPLUMBING");

				public static LocString DESC = "Precise control of the flow and temperature of " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " allows the creation of " + UI.FormatAsLink("Morale", "QUALITYOFLIFE") + "-enhancing devices.";
			}

			public class SANITATIONSCIENCES
			{
				public static LocString NAME = UI.FormatAsLink("Sanitation", "SANITATIONSCIENCES");

				public static LocString DESC = "Make daily ablutions less of a hassle.";
			}

			public class MEDICALRESEARCH
			{
				public static LocString NAME = UI.FormatAsLink("Medical Research", "MEDICALRESEARCH");

				public static LocString DESC = "Basic medical knowledge to fight the common " + UI.FormatAsLink("Diseases", "DISEASE") + " that plague Duplicants.";
			}

			public class MEDBAY
			{
				public static LocString NAME = UI.FormatAsLink("Healthcare", "MEDBAY");

				public static LocString DESC = "Prevent injury and " + UI.FormatAsLink("Disease", "DISEASE") + " from running rampant in the colony.";
			}

			public class ADVANCEDFILTRATION
			{
				public static LocString NAME = UI.FormatAsLink("Filtration", "ADVANCEDFILTRATION");

				public static LocString DESC = "Basic technologies for filtering " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " and " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".";
			}

			public class POWERREGULATION
			{
				public static LocString NAME = UI.FormatAsLink("Power Regulation", "POWERREGULATION");

				public static LocString DESC = "Prevent wasted " + UI.FormatAsLink("Power", "POWER") + " with improved electrical tools.";
			}

			public class COMBUSTION
			{
				public static LocString NAME = UI.FormatAsLink("Internal Combustion", "COMBUSTION");

				public static LocString DESC = "Crude fuel-powered generators for automatic " + UI.FormatAsLink("Power", "POWER") + " production.";
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

				public static LocString DESC = "Majorly improve " + UI.FormatAsLink("Decor", "DECOR") + " by allowing Duplicants artistic and emotional expression.";
			}

			public class ADVANCEDPOWERREGULATION
			{
				public static LocString NAME = UI.FormatAsLink("Advanced Power Regulation", "ADVANCEDPOWERREGULATION");

				public static LocString DESC = "Circuit components required for large scale " + UI.FormatAsLink("Power", "POWER") + " management.";
			}

			public class PLASTICS
			{
				public static LocString NAME = UI.FormatAsLink("Plastic Manufacturing", "PLASTICS");

				public static LocString DESC = "Stable, lightweight, durable. Plastics are useful in a wide array of applications.";
			}

			public class CLOTHING
			{
				public static LocString NAME = UI.FormatAsLink("Self Expression", "CLOTHING");

				public static LocString DESC = "Produce leisurely luxury items for Duplicants.";
			}

			public class SUITS
			{
				public static LocString NAME = UI.FormatAsLink("Environmental Protection", "SUITS");

				public static LocString DESC = "Craft the equipment necessary to survive in extreme conditions and environments.";
			}

			public class DISTILLATION
			{
				public static LocString NAME = UI.FormatAsLink("Distillation", "DISTILLATION");

				public static LocString DESC = "Distill difficult mixtures down to their most useful parts.";
			}

			public class CATALYTICS
			{
				public static LocString NAME = UI.FormatAsLink("Catalytics", "CATALYTICS");

				public static LocString DESC = "Advanced gas manipulation using catalysts.";
			}

			public class ADVANCEDRESEARCH
			{
				public static LocString NAME = UI.FormatAsLink("Advanced Research", "ADVANCEDRESEARCH");

				public static LocString DESC = "The tools my colony needs to conduct more advanced and in-depth research.";
			}

			public class LOGICCONTROL
			{
				public static LocString NAME = UI.FormatAsLink("Automatic Control", "LOGICCONTROL");

				public static LocString DESC = "Switches for controlling colony buildings.";
			}

			public class LOGICCIRCUITS
			{
				public static LocString NAME = UI.FormatAsLink("Advanced Automation", "LOGICCIRCUITS");

				public static LocString DESC = "Logic gates and wiring to allow me to program my base.";
			}

			public class VALVEMINIATURIZATION
			{
				public static LocString NAME = UI.FormatAsLink("Valve Miniaturization", "VALVEMINIATURIZATION");

				public static LocString DESC = "Smaller, more efficient pumps for those low-throughput situations.";
			}

			public class PRETTYGOODCONDUCTORS
			{
				public static LocString NAME = UI.FormatAsLink("Low-Resistance Conductors", "PRETTYGOODCONDUCTORS");

				public static LocString DESC = "Pure-core wires that can handle more current without overloading.";
			}

			public class RENEWABLEENERGY
			{
				public static LocString NAME = UI.FormatAsLink("Renewable Energy", "RENEWABLEENERGY");

				public static LocString DESC = "Clean " + UI.FormatAsLink("Power", "POWER") + " production";
			}

			public class BASICREFINEMENT
			{
				public static LocString NAME = UI.FormatAsLink("Brute-Force Refinement", "BASICREFINEMENT");

				public static LocString DESC = "When life gives you rocks, make rock-aid! (Or sand.)";
			}

			public class METALREFINEMENT
			{
				public static LocString NAME = UI.FormatAsLink("Metallurgy", "METALREFINEMENT");

				public static LocString DESC = "High-temperature pure metal extraction.";
			}

			public class REFINEDOBJECTS
			{
				public static LocString NAME = UI.FormatAsLink("Refined Objects", "REFINEDOBJECTS");

				public static LocString DESC = "Using simple materials to improve base function.";
			}

			public class GENERICSENSORS
			{
				public static LocString NAME = UI.FormatAsLink("Generic Sensors", "GENERICSENSORS");

				public static LocString DESC = "Drive automation in brand new ways.";
			}

			public class DUPETRAFFICCONTROL
			{
				public static LocString NAME = UI.FormatAsLink("Computing", "DUPETRAFFICCONTROL");

				public static LocString DESC = "More advanced components allow more advanced Duplicant-machine interactions";
			}

			public class SMELTING
			{
				public static LocString NAME = UI.FormatAsLink("Smelting", "SMELTING");

				public static LocString DESC = "High temperatures allow more efficient metal fabrication.";
			}

			public class TRAVELTUBES
			{
				public static LocString NAME = UI.FormatAsLink("Transit Tubes", "TRAVELTUBES");

				public static LocString DESC = "Get Duplicants around the base quickly, safely, and in style!";
			}

			public class SMARTSTORAGE
			{
				public static LocString NAME = UI.FormatAsLink("Smart Storage", "SMARTSTORAGE");

				public static LocString DESC = "Automate the storage of solids.";
			}

			public class SOLIDTRANSPORT
			{
				public static LocString NAME = UI.FormatAsLink("Solid Transport", "SOLIDTRANSPORT");

				public static LocString DESC = "Save so much wear and tear on tired Duplicant feet.";
			}

			public class HIGHTEMPFORGING
			{
				public static LocString NAME = UI.FormatAsLink("High-Temperature Forging", "HIGHTEMPFORGING");

				public static LocString DESC = "Heat allows the creation of entirely new materials.";
			}

			public class SKYDETECTORS
			{
				public static LocString NAME = UI.FormatAsLink("Celestial Detection", "SKYDETECTORS");

				public static LocString DESC = "Space: It's not quite as empty as you might think.";
			}

			public class REFRACTIVEDECOR
			{
				public static LocString NAME = UI.FormatAsLink("Refractive Decor", "REFRACTIVEDECOR");

				public static LocString DESC = "The ultimate solution to ugly things? Make them transparent.";
			}

			public class JETPACKS
			{
				public static LocString NAME = UI.FormatAsLink("Jetpacks", "JETPACKS");

				public static LocString DESC = "Continue your space program";
			}

			public class BASICROCKETRY
			{
				public static LocString NAME = UI.FormatAsLink("Basic Rocketry", "BASICROCKETRY");

				public static LocString DESC = "Start your space program";
			}

			public class ENGINESI
			{
				public static LocString NAME = UI.FormatAsLink("Solid Fuel Combustion", "ENGINESI");

				public static LocString DESC = "Continue your space program";
			}

			public class ENGINESII
			{
				public static LocString NAME = UI.FormatAsLink("Hydrocarbon Combustion", "ENGINESII");

				public static LocString DESC = "Continue your space program";
			}

			public class ENGINESIII
			{
				public static LocString NAME = UI.FormatAsLink("Cryofuel Combustion", "ENGINESIII");

				public static LocString DESC = "Continue your space program";
			}

			public class CARGOI
			{
				public static LocString NAME = UI.FormatAsLink("Solid Cargos", "CARGOI");

				public static LocString DESC = "Continue your space program";
			}

			public class CARGOII
			{
				public static LocString NAME = UI.FormatAsLink("Liquid and Gas Cargos", "CARGOII");

				public static LocString DESC = "Continue your space program";
			}

			public class CARGOIII
			{
				public static LocString NAME = UI.FormatAsLink("Special Cargos", "CARGOIII");

				public static LocString DESC = "Continue your space program";
			}

			public class INDUSTRIALSTORAGE
			{
				public static LocString NAME = UI.FormatAsLink("Industrial Storage", "INDUSTRIALSTORAGE");

				public static LocString DESC = "Industrial storage of liquids and gases";
			}
		}
	}
}
