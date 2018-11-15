namespace STRINGS
{
	public class ELEMENTS
	{
		public class STATE
		{
			public static LocString SOLID = "Solid";

			public static LocString LIQUID = "Liquid";

			public static LocString GAS = "Gas";

			public static LocString VACUUM = "None";
		}

		public class MATERIAL_MODIFIERS
		{
			public class TOOLTIP
			{
				public static LocString EFFECTS_HEADER = "Buildings constructed from this material will have these properties";

				public static LocString DECOR = "This material will add {0} to the finished building's Decor";

				public static LocString OVERHEATTEMPERATURE = "This material will add {0} to the finished building's Overheat Temperature";

				public static LocString HIGH_THERMAL_CONDUCTIVITY = "This material disperses heat because energy transfers quickly through materials with high thermal conductivity\n\nBetween two objects, the rate of heat transfer will be determined by the object with the lowest Thermal Conductivity\n\nThermal Conductivity: {1} W per degree K difference (Oxygen: 0.024 W)";

				public static LocString LOW_THERMAL_CONDUCTIVITY = "This material retains heat because energy transfers slowly through materials with low thermal conductivity\n\nBetween two objects, the rate of heat transfer will be determined by the object with the lowest Thermal Conductivity\n\nThermal Conductivity: {1} W per degree K difference (Oxygen: 0.024 W)";

				public static LocString LOW_SPECIFIC_HEAT_CAPACITY = "Thermally Reactive materials require little energy to raise in temperature, and therefore heat and cool quickly\n\nSpecific Heat Capacity: {1} DTU to raise 1g by 1K";

				public static LocString HIGH_SPECIFIC_HEAT_CAPACITY = "Slow Heating materials require a large amount of energy to raise in temperature, and therefore heat and cool slowly\n\nSpecific Heat Capacity: {1} DTU to raise 1g by 1K";
			}

			public static LocString EFFECTS_HEADER = "<b>Resource Effects:</b>";

			public static LocString DECOR = UI.FormatAsLink("Decor", "DECOR") + ": {0}";

			public static LocString OVERHEATTEMPERATURE = UI.FormatAsLink("Overheat Temperature", "HEAT") + ": {0}";

			public static LocString HIGH_THERMAL_CONDUCTIVITY = UI.FormatAsLink("High Thermal Conductivity", "HEAT");

			public static LocString LOW_THERMAL_CONDUCTIVITY = UI.FormatAsLink("Insulator", "HEAT");

			public static LocString LOW_SPECIFIC_HEAT_CAPACITY = UI.FormatAsLink("Thermally Reactive", "HEAT");

			public static LocString HIGH_SPECIFIC_HEAT_CAPACITY = UI.FormatAsLink("Slow Heating", "HEAT");
		}

		public class HARDNESS
		{
			public static LocString NA = "N/A";

			public static LocString SOFT = "{0} (Soft)";

			public static LocString VERYSOFT = "{0} (Very Soft)";

			public static LocString FIRM = "{0} (Firm)";

			public static LocString VERYFIRM = "{0} (Very Firm)";

			public static LocString NEARLYIMPENETRABLE = "{0} (Nearly Impenetrable)";

			public static LocString IMPENETRABLE = "{0} (Impenetrable)";
		}

		public class AEROGEL
		{
			public static LocString NAME = UI.FormatAsLink("Aerogel", "AEROGEL");

			public static LocString DESC = string.Empty;
		}

		public class ALGAE
		{
			public static LocString NAME = UI.FormatAsLink("Algae", "ALGAE");

			public static LocString DESC = "Algae is a cluster of non-motile, single-celled lifeforms.\n\nIt can be used to produce " + OXYGEN.NAME + " when grown in Algae Terrariums.";
		}

		public class BLEACHSTONE
		{
			public static LocString NAME = UI.FormatAsLink("Bleach Stone", "BLEACHSTONE");

			public static LocString DESC = "Bleach stone is an unstable compound that emits toxic " + CHLORINEGAS.NAME + ".\n\nIt is useful in " + UI.FormatAsLink("Hygienic", "HYGIENE") + " processes.";
		}

		public class BITUMEN
		{
			public static LocString NAME = UI.FormatAsLink("Bitumen", "BITUMEN");

			public static LocString DESC = "Bitumen is a sticky viscous residue from " + PETROLEUM.NAME + " production";
		}

		public class BOTTLEDWATER
		{
			public static LocString NAME = UI.FormatAsLink("Water", "BOTTLEDWATER");

			public static LocString DESC = "(H<sub>2</sub>O) Clean " + WATER.NAME + ", prepped for transport.";
		}

		public class CARBON
		{
			public static LocString NAME = UI.FormatAsLink("Coal", "CARBON");

			public static LocString DESC = "(C) Coal is a combustible fossil fuel composed of carbon.\n\nIt is useful in " + UI.FormatAsLink("Power", "POWER") + " production.";
		}

		public class REFINEDCARBON
		{
			public static LocString NAME = UI.FormatAsLink("Refined Carbon", "REFINEDCARBON");

			public static LocString DESC = "(C) Refined carbon is solid element purified from raw " + CARBON.NAME + ".";
		}

		public class CARBONDIOXIDE
		{
			public static LocString NAME = UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE");

			public static LocString DESC = "(CO<sub>2</sub>) Carbon Dioxide toxic, atomically heavy chemical compound in a " + UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " state.\n\nIt tends to sink below other gases.";
		}

		public class CARBONFIBRE
		{
			public static LocString NAME = UI.FormatAsLink("Carbon Fiber", "CARBONFIBRE");

			public static LocString DESC = "Carbon Fiber is a " + UI.FormatAsLink("RefinedMineral", "Manufactured Material") + " with high tensile strength.";
		}

		public class CARBONGAS
		{
			public static LocString NAME = UI.FormatAsLink("Carbon", "CARBONGAS");

			public static LocString DESC = "(C) Carbon is an abundant, versatile element heated into a " + UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " state.";
		}

		public class CHLORINE
		{
			public static LocString NAME = UI.FormatAsLink("Chlorine", "CHLORINE");

			public static LocString DESC = "(Cl) Chlorine is an extremely toxic element in a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class CHLORINEGAS
		{
			public static LocString NAME = UI.FormatAsLink("Chlorine", "CHLORINEGAS");

			public static LocString DESC = "(Cl) Chlorine is an extremely toxic chemical in a " + UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " state.";
		}

		public class CLAY
		{
			public static LocString NAME = UI.FormatAsLink("Clay", "CLAY");

			public static LocString DESC = "Clay is a soft, naturally occurring composite of stone and soil that hardens at high " + UI.FormatAsLink("Temperatures", "HEAT") + ".\n\nIt is a reliable <b>Construction Material.</b>";
		}

		public class BRICK
		{
			public static LocString NAME = UI.FormatAsLink("Brick", "BRICK");

			public static LocString DESC = "Brick is a hard, brittle material formed from heated " + CLAY.NAME + ".\n\nIt is a reliable Construction Material.";
		}

		public class CERAMIC
		{
			public static LocString NAME = UI.FormatAsLink("Ceramic", "CERAMIC");

			public static LocString DESC = "Ceramic is a hard, brittle material formed from heated " + CLAY.NAME + ".\n\nIt is a reliable Construction Material.";
		}

		public class CEMENT
		{
			public static LocString NAME = UI.FormatAsLink("Cement", "CEMENT");

			public static LocString DESC = "Cement is a refined building material used for assembling advanced buildings.";
		}

		public class CEMENTMIX
		{
			public static LocString NAME = UI.FormatAsLink("Cement Mix", "CEMENTMIX");

			public static LocString DESC = "Cement Mix can be used to create " + CEMENT.NAME + " for advanced building assembly.";
		}

		public class CONTAMINATEDOXYGEN
		{
			public static LocString NAME = UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN");

			public static LocString DESC = "(O<sub>2</sub>) Polluted Oxygen is dirty, unfiltered air.\n\nIt is breathable.";
		}

		public class COPPER
		{
			public static LocString NAME = UI.FormatAsLink("Copper", "COPPER");

			public static LocString DESC = "(Cu) Copper is a conductive " + UI.FormatAsLink("Metal", "RAWMETAL") + ".\n\nIt is suitable for building " + UI.FormatAsLink("Power", "POWER") + " systems.";
		}

		public class COPPERGAS
		{
			public static LocString NAME = UI.FormatAsLink("Copper", "COPPERGAS");

			public static LocString DESC = "(Cu) Copper is a conductive " + UI.FormatAsLink("Metal", "RAWMETAL") + " heated into a " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".";
		}

		public class CREATURE
		{
			public static LocString NAME = UI.FormatAsLink("Genetic Ooze", "CREATURE");

			public static LocString DESC = "(DuPe) Ooze is a slurry of water, carbon, and dozens and dozens of trace elements.\n\nDuplicants are printed from pure " + UI.FormatAsLink("Ooze", "SOLID") + ".";
		}

		public class CRUDEOIL
		{
			public static LocString NAME = UI.FormatAsLink("Crude Oil", "CRUDEOIL");

			public static LocString DESC = "Crude Oil is a raw potential " + UI.FormatAsLink("Power", "POWER") + " source composed of billions of dead, primordial organisms.";
		}

		public class PETROLEUM
		{
			public static LocString NAME = UI.FormatAsLink("Petroleum", "PETROLEUM");

			public static LocString NAME_TWO = UI.FormatAsLink("Petroleum", "PETROLEUM");

			public static LocString DESC = "Petroleum is a " + UI.FormatAsLink("Power", "POWER") + " source refined from " + CRUDEOIL.NAME + ".\n\nIt is also an essential ingredient in the production of " + POLYPROPYLENE.NAME + ".";
		}

		public class SOURGAS
		{
			public static LocString NAME = UI.FormatAsLink("Sour Gas", "SOURGAS");

			public static LocString NAME_TWO = UI.FormatAsLink("Sour Gas", "SOURGAS");

			public static LocString DESC = "Sour Gas is a hydrocarbon " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " containing high concentrations of hydrogen sulfide.\n\nIt is a byproduct of highly heated " + UI.FormatAsLink("Petroleum", "PETROLEUM") + ".";
		}

		public class CRUSHEDICE
		{
			public static LocString NAME = UI.FormatAsLink("Crushed Ice", "CRUSHEDICE");

			public static LocString DESC = "(H<sub>2</sub>0) A slush of crushed, semi-solid ice.";
		}

		public class CRUSHEDROCK
		{
			public static LocString NAME = UI.FormatAsLink("Crushed Rock", "CRUSHEDROCK");

			public static LocString DESC = "Crushed Rock is " + IGNEOUSROCK.NAME + " crushed into a mechanical mixture.";
		}

		public class CUPRITE
		{
			public static LocString NAME = UI.FormatAsLink("Copper Ore", "CUPRITE");

			public static LocString DESC = "(Cu<sub>2</sub>0) Copper Ore is a conductive " + UI.FormatAsLink("Metal", "RAWMETAL") + ".\n\nIt is suitable for building " + UI.FormatAsLink("Power", "POWER") + " systems.";
		}

		public class DIAMOND
		{
			public static LocString NAME = UI.FormatAsLink("Diamond", "DIAMOND");

			public static LocString DESC = "(C) Diamond is industrial-grade, high density carbon.\n\nIt is very difficult to excavate.";
		}

		public class DIRT
		{
			public static LocString NAME = UI.FormatAsLink("Dirt", "DIRT");

			public static LocString DESC = "Dirt is a soft, nutrient-rich substance capable of supporting life.\n\nIt is necessary in some forms of " + UI.FormatAsLink("Food", "FOOD") + " production.";
		}

		public class DIRTYICE
		{
			public static LocString NAME = UI.FormatAsLink("Polluted Ice", "DIRTYICE");

			public static LocString DESC = "Polluted Ice is dirty, unfiltered water frozen into a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class DIRTYWATER
		{
			public static LocString NAME = UI.FormatAsLink("Polluted Water", "DIRTYWATER");

			public static LocString DESC = "Polluted Water is dirty, unfiltered " + WATER.NAME + ".\n\nIt is not fit for consumption.";
		}

		public class ELECTRUM
		{
			public static LocString NAME = UI.FormatAsLink("Electrum", "ELECTRUM");

			public static LocString DESC = "Electrum is a conductive " + UI.FormatAsLink("Metal", "RAWMETAL") + " alloy composed of gold and silver.\n\nIt is suitable for building " + UI.FormatAsLink("Power", "POWER") + " systems.";
		}

		public class FERTILIZER
		{
			public static LocString NAME = UI.FormatAsLink("Fertilizer", "FERTILIZER");

			public static LocString DESC = "Fertilizer is a processed mixture of biological nutrients.\n\nIt aids in the growth of certain " + UI.FormatAsLink("plant", "Plants") + ".";
		}

		public class PONDSCUM
		{
			public static LocString NAME = UI.FormatAsLink("Pondscum", "PONDSCUM");

			public static LocString DESC = "Pondscum is a soft, naturally occurring composite of biological nutrients.\n\nIt may be processed into " + FERTILIZER.NAME + " and aids in the growth of certain " + UI.FormatAsLink("plant", "Plants") + ".";
		}

		public class FOOLSGOLD
		{
			public static LocString NAME = UI.FormatAsLink("Pyrite", "FOOLSGOLD");

			public static LocString DESC = "(FeS<sub>2</sub>) Pyrite is a conductive " + UI.FormatAsLink("Metal", "RAWMETAL") + ".\n\nAlso known as \"Fool's Gold\", is suitable for building " + UI.FormatAsLink("Power", "POWER") + " systems.";
		}

		public class FULLERENE
		{
			public static LocString NAME = UI.FormatAsLink("Fullerene", "FULLERENE");

			public static LocString DESC = "(C<sub>60</sub>) Fullerene is a form of " + UI.FormatAsLink("Carbon", "CARBON") + " consisting of spherical molecules.";
		}

		public class GLASS
		{
			public static LocString NAME = UI.FormatAsLink("Glass", "GLASS");

			public static LocString DESC = "Glass is a brittle, transparent substance formed from " + SAND.NAME + " fired at high temperatures.";
		}

		public class GOLD
		{
			public static LocString NAME = UI.FormatAsLink("Gold", "GOLD");

			public static LocString DESC = "(Au) Gold is a conductive precious " + UI.FormatAsLink("Metal", "RAWMETAL") + ".\n\nIt is suitable for building " + UI.FormatAsLink("Power", "POWER") + " systems.";
		}

		public class GOLDAMALGAM
		{
			public static LocString NAME = UI.FormatAsLink("Gold Amalgam", "GOLDAMALGAM");

			public static LocString DESC = "Gold Amalgam is a conductive amalgam of gold and mercury.\n\nIt is suitable for building " + UI.FormatAsLink("Power", "POWER") + " systems.";
		}

		public class GOLDGAS
		{
			public static LocString NAME = UI.FormatAsLink("Gold", "GOLDGAS");

			public static LocString DESC = "(Au) Gold is a conductive precious " + UI.FormatAsLink("Metal", "RAWMETAL") + ", heated into a " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".";
		}

		public class GRANITE
		{
			public static LocString NAME = UI.FormatAsLink("Granite", "GRANITE");

			public static LocString DESC = "Granite is a dense composite of " + IGNEOUSROCK.NAME + ".\n\nIt is useful as a Construction Material.";
		}

		public class HELIUM
		{
			public static LocString NAME = UI.FormatAsLink("Helium", "HELIUM");

			public static LocString DESC = "(He) Helium is an atomically lightweight, chemical " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".";
		}

		public class HYDROGEN
		{
			public static LocString NAME = UI.FormatAsLink("Hydrogen", "HYDROGEN");

			public static LocString DESC = "(H) Hydrogen is the universe's most common and atomically light element in a " + UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " state.";
		}

		public class ICE
		{
			public static LocString NAME = UI.FormatAsLink("Ice", "ICE");

			public static LocString DESC = "(H<sub>2</sub>0) Ice is clean water frozen into a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class IGNEOUSROCK
		{
			public static LocString NAME = UI.FormatAsLink("Igneous Rock", "IGNEOUSROCK");

			public static LocString DESC = "Igneous Rock is a composite of solidified volcanic rock.\n\nIt is useful as a Construction Material.";
		}

		public class ISORESIN
		{
			public static LocString NAME = UI.FormatAsLink("Isoresin", "ISORESIN");

			public static LocString DESC = "Isoresin is a crystalized sap composed of long-chain polymers.\n\nIt is used in the production of rare, high grade materials.";
		}

		public class IRON
		{
			public static LocString NAME = UI.FormatAsLink("Iron", "IRON");

			public static LocString DESC = "(Fe) Iron is a common industrial " + UI.FormatAsLink("Metal", "RAWMETAL") + ".";
		}

		public class IRONINGOT
		{
			public static LocString NAME = UI.FormatAsLink("Iron", "IRONINGOT");

			public static LocString DESC = "(Fe) Iron is refined " + IRONORE.NAME + ".";
		}

		public class IRONGAS
		{
			public static LocString NAME = UI.FormatAsLink("Iron", "IRONGAS");

			public static LocString DESC = "(Fe) Iron is a common industrial " + UI.FormatAsLink("Metal", "RAWMETAL") + ", heated into a " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".";
		}

		public class IRONORE
		{
			public static LocString NAME = UI.FormatAsLink("Iron Ore", "IRONORE");

			public static LocString DESC = "(Fe) Iron Ore is a soft " + UI.FormatAsLink("Metal", "RAWMETAL") + ".\n\nIt is suitable for building " + UI.FormatAsLink("Power", "POWER") + " systems.";
		}

		public class KATAIRITE
		{
			public static LocString NAME = UI.FormatAsLink("Abyssalite", "KATAIRITE");

			public static LocString DESC = "(Ab) Abyssalite is a resilient, crystalline element.";
		}

		public class LIME
		{
			public static LocString NAME = UI.FormatAsLink("Lime", "LIME");

			public static LocString DESC = "(CaCO<sub>3</sub>) Lime is a mineral commonly found in " + UI.FormatAsLink("Critter", "CRITTERS") + " egg shells.\n\nIt is useful as a Construction Material.";
		}

		public class FOSSIL
		{
			public static LocString NAME = UI.FormatAsLink("Fossil", "FOSSIL");

			public static LocString DESC = "Fossil is organic matter, highly compressed and hardened into a mineral state.\n\nIt is useful as a Construction Material.";
		}

		public class LIQUIDCARBONDIOXIDE
		{
			public static LocString NAME = UI.FormatAsLink("Carbon Dioxide", "LIQUIDCARBONDIOXIDE");

			public static LocString DESC = "(CO<sub>2</sub>) Carbon Dioxide is a toxic chemical compound. This selection is currently in a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class LIQUIDHELIUM
		{
			public static LocString NAME = UI.FormatAsLink("Helium", "LIQUIDHELIUM");

			public static LocString DESC = "(He) Helium is an atomically lightweight chemical element cooled into a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class LIQUIDHYDROGEN
		{
			public static LocString NAME = UI.FormatAsLink("Hydrogen", "LIQUIDHYDROGEN");

			public static LocString DESC = "(H) Hydrogen is a chemical " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".\n\nIt freezes most substances that come into contact with it.";
		}

		public class LIQUIDOXYGEN
		{
			public static LocString NAME = UI.FormatAsLink("Oxygen", "LIQUIDOXYGEN");

			public static LocString DESC = "(O<sub>2</sub>) Oxygen is a breathable chemical.\n\nThis selection is in a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class LIQUIDMETHANE
		{
			public static LocString NAME = UI.FormatAsLink("Methane", "LIQUIDMETHANE");

			public static LocString DESC = "(CH<sub>4</sub>) Methane is an alkane.\n\nThis selection is in a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class LIQUIDPHOSPHORUS
		{
			public static LocString NAME = UI.FormatAsLink("Phosphorus", "LIQUIDPHOSPHORUS");

			public static LocString DESC = "(P) Phosphorus is a chemical element.\n\nThis selection is in a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class LIQUIDPROPANE
		{
			public static LocString NAME = UI.FormatAsLink("Propane", "LIQUIDPROPANE");

			public static LocString DESC = "(C<sub>3</sub>H<sub>8</sub>) Propane is an alkane in a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.\n\nIt is useful in " + UI.FormatAsLink("Power", "POWER") + " production.";
		}

		public class LIQUIDSULFUR
		{
			public static LocString NAME = UI.FormatAsLink("Liquid Sulfur", "LIQUIDSULFUR");

			public static LocString DESC = "(S) Sulfur is a common chemical element and byproduct of " + UI.FormatAsLink("Natural Gas", "METHANE") + " production.\n\nThis selection is in a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class MAFICROCK
		{
			public static LocString NAME = UI.FormatAsLink("Mafic Rock", "MAFICROCK");

			public static LocString DESC = "Mafic Rock an " + UI.FormatAsLink("Iron", "IRON") + "-rich variation of " + UI.FormatAsLink("Igneous Rock", "IGNEOUSROCK") + ".\n\nIt is useful as a Construction Material.";
		}

		public class MAGMA
		{
			public static LocString NAME = UI.FormatAsLink("Magma", "MAGMA");

			public static LocString DESC = "Magma is a composite of " + IGNEOUSROCK.NAME + " heated into a molten, " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class MERCURY
		{
			public static LocString NAME = UI.FormatAsLink("Mercury", "MERCURY");

			public static LocString DESC = "(Hg) Mercury is a toxic, metallic " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".";
		}

		public class MERCURYGAS
		{
			public static LocString NAME = UI.FormatAsLink("Mercury", "MERCURYGAS");

			public static LocString DESC = "(Hg) Mercury is a toxic " + UI.FormatAsLink("Metal", "RAWMETAL") + " heated into a " + UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " state.";
		}

		public class METHANE
		{
			public static LocString NAME = UI.FormatAsLink("Natural Gas", "METHANE");

			public static LocString DESC = "Natural Gas is a mixture of various alkanes in a " + UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " state.\n\nIt is useful in " + UI.FormatAsLink("Power", "POWER") + " production.";
		}

		public class MOLTENCARBON
		{
			public static LocString NAME = UI.FormatAsLink("Carbon", "MOLTENCARBON");

			public static LocString DESC = "(C) Carbon is an abundant, versatile element heated into a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class MOLTENCOPPER
		{
			public static LocString NAME = UI.FormatAsLink("Copper", "MOLTENCOPPER");

			public static LocString DESC = "(Cu) Copper is a conductive " + UI.FormatAsLink("Metal", "RAWMETAL") + " heated into a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class MOLTENGLASS
		{
			public static LocString NAME = UI.FormatAsLink("Molten Glass", "MOLTENGLASS");

			public static LocString DESC = "Molten Glass is a composite of granular rock heated into a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class MOLTENGOLD
		{
			public static LocString NAME = UI.FormatAsLink("Gold", "MOLTENGOLD");

			public static LocString DESC = "(Au) Gold is a conductive precious " + UI.FormatAsLink("Metal", "RAWMETAL") + " heated into a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class MOLTENIRON
		{
			public static LocString NAME = UI.FormatAsLink("Iron", "MOLTENIRON");

			public static LocString DESC = "(Fe) Iron is a common industrial " + UI.FormatAsLink("Metal", "RAWMETAL") + " heated into a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class MOLTENNIOBIUM
		{
			public static LocString NAME = UI.FormatAsLink("Niobium", "MOLTENNIOBIUM");

			public static LocString DESC = "(Nb) Niobium is a " + UI.FormatAsLink("Rare Metal", "RAREMATERIALS") + " heated into a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class MOLTENTUNGSTEN
		{
			public static LocString NAME = UI.FormatAsLink("Tungsten", "MOLTENTUNGSTEN");

			public static LocString DESC = "(W) Tungsten is a crystalline " + UI.FormatAsLink("Metal", "RAWMETAL") + " heated into a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class MOLTENTUNGSTENDISELENIDE
		{
			public static LocString NAME = UI.FormatAsLink("Tungsten Diselenide", "MOLTENTUNGSTENDISELENIDE");

			public static LocString DESC = "(WSe<sub>2</sub>) Tungsten Diselenide is an inorganic " + UI.FormatAsLink("Metal", "RAWMETAL") + " compound heated into a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class MOLTENSTEEL
		{
			public static LocString NAME = UI.FormatAsLink("Steel", "MOLTENSTEEL");

			public static LocString DESC = "Steel is a " + UI.FormatAsLink("Metal", "RAWMETAL") + " alloy of iron and carbon, heated into a hazardous " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " state.";
		}

		public class NIOBIUM
		{
			public static LocString NAME = UI.FormatAsLink("Niobium", "NIOBIUM");

			public static LocString DESC = "(Nb) Niobium is a " + UI.FormatAsLink("Rare Metal", "RAREMATERIALS") + " with many practical applications in metallurgy and superconductor " + UI.FormatAsLink("Research", "RESEARCH") + ".";
		}

		public class NIOBIUMGAS
		{
			public static LocString NAME = UI.FormatAsLink("Niobium", "NIOBIUMGAS");

			public static LocString DESC = "(Nb) Niobium is a " + UI.FormatAsLink("Rare Metal", "RAREMATERIALS") + ".\n\nThis selection is in a " + UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " state.";
		}

		public class OBSIDIAN
		{
			public static LocString NAME = UI.FormatAsLink("Obsidian", "OBSIDIAN");

			public static LocString DESC = "Obsidian is a brittle composite of volcanic " + GLASS.NAME + ".";
		}

		public class OXYGEN
		{
			public static LocString NAME = UI.FormatAsLink("Oxygen", "OXYGEN");

			public static LocString DESC = "(O<sub>2</sub>) Oxygen is an atomically lightweight and breathable " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ", necessary for sustaining life.\n\nIt tends to rise above other gases.";
		}

		public class OXYROCK
		{
			public static LocString NAME = UI.FormatAsLink("Oxylite", "OXYROCK");

			public static LocString DESC = "(Ir<sub>3</sub>O<sub>2</sub>) Oxylite is a chemical compound that slowly emits breathable " + OXYGEN.NAME + ".\n\nExcavating Oxylite increases its emission rate, but depletes the ore more rapidly.";
		}

		public class PHOSPHATENODULES
		{
			public static LocString NAME = UI.FormatAsLink("Phosphate Nodules", "PHOSPHATENODULES");

			public static LocString DESC = "(PO<sup>3-</sup><sub>4</sub>) Nodules of sedimentary rock containing high concentrations of phosphate.";
		}

		public class PHOSPHORITE
		{
			public static LocString NAME = UI.FormatAsLink("Phosphorite", "PHOSPHORITE");

			public static LocString DESC = "Phosphorite is a composite of sedimentary rock, saturated with phosphate.";
		}

		public class PHOSPHORUS
		{
			public static LocString NAME = UI.FormatAsLink("Phosphorus", "PHOSPHORUS");

			public static LocString DESC = "(P) Phosphorus is a chemical element in its " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class PHOSPHORUSGAS
		{
			public static LocString NAME = UI.FormatAsLink("Phosphorus", "PHOSPHORUSGAS");

			public static LocString DESC = "(P) Phosphorus is a chemical element in a " + UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " state.";
		}

		public class PROPANE
		{
			public static LocString NAME = UI.FormatAsLink("Propane", "PROPANE");

			public static LocString DESC = "(C<sub>3</sub>H<sub>8</sub>) Propane is a natural alkane " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".\n\nIt is useful in " + UI.FormatAsLink("Power", "POWER") + " production.";
		}

		public class RADIUM
		{
			public static LocString NAME = UI.FormatAsLink("Radium", "RADIUM");

			public static LocString DESC = "(Ra) Radium is a " + UI.FormatAsLink("Light", "LIGHT") + " emitting radioactive substance.\n\nIt is useful as a " + UI.FormatAsLink("Power", "POWER") + " source.";
		}

		public class ROCKGAS
		{
			public static LocString NAME = UI.FormatAsLink("Rock Gas", "ROCKGAS");

			public static LocString DESC = "Rock Gas is rock that has been superheated into a " + UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " state.";
		}

		public class REGOLITH
		{
			public static LocString NAME = UI.FormatAsLink("Regolith", "REGOLITH");

			public static LocString DESC = "Regolith is a sandy substance composed of the various particles that collect atop terrestrial objects.\n\nIt is useful as a Filtration Medium.";
		}

		public class SAND
		{
			public static LocString NAME = UI.FormatAsLink("Sand", "SAND");

			public static LocString DESC = "Sand is a composite of granular rock.\n\nIt is useful as a Filtration Medium.";
		}

		public class SANDCEMENT
		{
			public static LocString NAME = UI.FormatAsLink("Sand Cement", "SANDCEMENT");

			public static LocString DESC = string.Empty;
		}

		public class SANDSTONE
		{
			public static LocString NAME = UI.FormatAsLink("Sandstone", "SANDSTONE");

			public static LocString DESC = "Sandstone is a composite of relatively soft sedimentary rock.\n\nIt is useful as a Construction Material.";
		}

		public class SEDIMENTARYROCK
		{
			public static LocString NAME = UI.FormatAsLink("Sedimentary Rock", "SEDIMENTARYROCK");

			public static LocString DESC = "Sedimentary Rock is a hardened composite of sediment layers.\n\nIt is useful as a Construction Material.";
		}

		public class SLIMEMOLD
		{
			public static LocString NAME = UI.FormatAsLink("Slime", "SLIMEMOLD");

			public static LocString DESC = "Slime is a thick biomixture of algae, fungi, and mucopolysaccharides.\n\nIt can be distilled into " + ALGAE.NAME + " and is useful in some " + OXYGEN.NAME + " production processes.";
		}

		public class SNOW
		{
			public static LocString NAME = UI.FormatAsLink("Snow", "SNOW");

			public static LocString DESC = "(H<sub>2</sub>0) Snow is a mass of loose, crystalline ice particles.\n\nIt becomes " + WATER.NAME + " when melted.";
		}

		public class SOLIDCARBONDIOXIDE
		{
			public static LocString NAME = UI.FormatAsLink("Carbon Dioxide", "SOLIDCARBONDIOXIDE");

			public static LocString DESC = "(CO<sub>2</sub>) Carbon Dioxide is a toxic compound in a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class SOLIDCHLORINE
		{
			public static LocString NAME = UI.FormatAsLink("Chlorine", "SOLIDCHLORINE");

			public static LocString DESC = "(Cl) Chlorine is a toxic chemical element in a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class SOLIDCRUDEOIL
		{
			public static LocString NAME = UI.FormatAsLink("Crude Oil", "SOLIDCRUDEOIL");

			public static LocString DESC = string.Empty;
		}

		public class SOLIDHYDROGEN
		{
			public static LocString NAME = UI.FormatAsLink("Hydrogen", "SOLIDHYDROGEN");

			public static LocString DESC = "(H) Hydrogen is the universe's most common element in a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class SOLIDMERCURY
		{
			public static LocString NAME = UI.FormatAsLink("Mercury", "SOLIDMERCURY");

			public static LocString DESC = "(Hg) Mercury is a toxic " + UI.FormatAsLink("Metal", "RAWMETAL") + " in a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class SOLIDOXYGEN
		{
			public static LocString NAME = UI.FormatAsLink("Oxygen", "SOLIDOXYGEN");

			public static LocString DESC = "(O<sub>2</sub>) Oxygen is a breathable element in a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class SOLIDMETHANE
		{
			public static LocString NAME = UI.FormatAsLink("Methane", "SOLIDMETHANE");

			public static LocString DESC = "(CH<sub>4</sub>) Methane is an alkane in a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class SOLIDNAPHTHA
		{
			public static LocString NAME = UI.FormatAsLink("Naphtha", "SOLIDNAPHTHA");

			public static LocString DESC = "Naphtha is a distilled hydrocarbon mixture in a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class SOLIDPETROLEUM
		{
			public static LocString NAME = UI.FormatAsLink("Petroleum", "SOLIDPETROLEUM");

			public static LocString DESC = "Petroleum is a " + UI.FormatAsLink("Power", "POWER") + " source.\n\nThis selection is in a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class SOLIDPROPANE
		{
			public static LocString NAME = UI.FormatAsLink("Propane", "SOLIDPROPANE");

			public static LocString DESC = "(C<sub>3</sub>H<sub>8</sub>) Propane is a natural gas in a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class SOLIDSUPERCOOLANT
		{
			public static LocString NAME = UI.FormatAsLink("Super Coolant", "SOLIDSUPERCOOLANT");

			public static LocString DESC = "Super Coolant is an industrial grade " + UI.FormatAsLink("Fullerene", "FULLERENE") + " coolant.\n\nThis selection is in a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class SOLIDVISCOGEL
		{
			public static LocString NAME = UI.FormatAsLink("Visco-Gel", "SOLIDVISCOGEL");

			public static LocString DESC = "Visco-Gel is a polymer that has high surface tension when in " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " form.\n\nThis selection is in a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.";
		}

		public class STEAM
		{
			public static LocString NAME = UI.FormatAsLink("Steam", "STEAM");

			public static LocString DESC = "(H<sub>2</sub>0) Steam is water that has been heated into a scalding " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".";
		}

		public class STEEL
		{
			public static LocString NAME = UI.FormatAsLink("Steel", "STEEL");

			public static LocString DESC = "Steel is a " + UI.FormatAsLink("Metal Alloy", "REFINEDMETAL") + " composed of iron and carbon.";
		}

		public class STEELGAS
		{
			public static LocString NAME = UI.FormatAsLink("Steel", "STEELGAS");

			public static LocString DESC = "Steel is a superheated " + UI.FormatAsLink("Metal", "RAWMETAL") + " " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " composed of iron and carbon.";
		}

		public class SULFUR
		{
			public static LocString NAME = UI.FormatAsLink("Sulfur", "SULFUR");

			public static LocString DESC = "(S) Sulfur is a common chemical element and byproduct of " + UI.FormatAsLink("Natural Gas", "METHANE") + " production.";
		}

		public class SULFURGAS
		{
			public static LocString NAME = UI.FormatAsLink("Sulfur", "SULFURGAS");

			public static LocString DESC = "(S) Sulfur is a common chemical element and byproduct of " + UI.FormatAsLink("Natural Gas", "METHANE") + " production.\n\nThis selection is in a " + UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " state.";
		}

		public class SUPERCOOLANT
		{
			public static LocString NAME = UI.FormatAsLink("Super Coolant", "SUPERCOOLANT");

			public static LocString DESC = "Super Coolant is an industrial grade coolant that utilizes the unusual energy states of " + UI.FormatAsLink("Fullerene", "FULLERENE") + ".";
		}

		public class SUPERCOOLANTGAS
		{
			public static LocString NAME = UI.FormatAsLink("Super Coolant", "SUPERCOOLANTGAS");

			public static LocString DESC = "Super Coolant is an industrial grade " + UI.FormatAsLink("Fullerene", "FULLERENE") + " coolant.\n\nThis selection is in a " + UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " state.";
		}

		public class SUPERINSULATOR
		{
			public static LocString NAME = UI.FormatAsLink("Insulation", "SUPERINSULATOR");

			public static LocString DESC = "Insulation reduces " + UI.FormatAsLink("Heat Transfer", "HEAT") + " and is composed of recrystalized " + UI.FormatAsLink("Abyssalite", "KATAIRITE") + ".";
		}

		public class TEMPCONDUCTORSOLID
		{
			public static LocString NAME = UI.FormatAsLink("Thermium", "TEMPCONDUCTORSOLID");

			public static LocString DESC = "Thermium is an industrial metal alloy formulated to maximize " + UI.FormatAsLink("Heat Transfer", "HEAT") + " and thermal dispersion.";
		}

		public class TUNGSTEN
		{
			public static LocString NAME = UI.FormatAsLink("Tungsten", "TUNGSTEN");

			public static LocString DESC = "(W) Tungsten is an extremely tough crystalline " + UI.FormatAsLink("Metal", "RAWMETAL") + ".\n\nIt is suitable for building " + UI.FormatAsLink("Power", "POWER") + " systems.";
		}

		public class TUNGSTENGAS
		{
			public static LocString NAME = UI.FormatAsLink("Tungsten", "TUNGSTENGAS");

			public static LocString DESC = "(W) Tungsten is a superheated crystalline " + UI.FormatAsLink("Metal", "RAWMETAL") + ".\n\nThis selection is in a " + UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " state.";
		}

		public class TUNGSTENDISELENIDE
		{
			public static LocString NAME = UI.FormatAsLink("Tungsten Diselenide", "TUNGSTENDISELENIDE");

			public static LocString DESC = "(WSe<sub>2</sub>) Tungsten Diselenide is an inorganic " + UI.FormatAsLink("Metal", "RAWMETAL") + " compound with a crystalline structure.\n\nIt is suitable for building " + UI.FormatAsLink("Power", "POWER") + " systems.";
		}

		public class TUNGSTENDISELENIDEGAS
		{
			public static LocString NAME = UI.FormatAsLink("Tungsten Diselenide", "TUNGSTENDISELENIDEGAS");

			public static LocString DESC = "(WSe<sub>2</sub>) Tungsten Diselenide is a superheated " + UI.FormatAsLink("Metal", "RAWMETAL") + " compound in a " + UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " state.";
		}

		public class TOXICSAND
		{
			public static LocString NAME = UI.FormatAsLink("Polluted Dirt", "TOXICSAND");

			public static LocString DESC = "Polluted Dirt is toxic biological waste.\n\nIt emits " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " over time.";
		}

		public class UNOBTANIUM
		{
			public static LocString NAME = UI.FormatAsLink("Neutronium", "UNOBTANIUM");

			public static LocString DESC = "(Nt) Neutronium is a mysterious and extremely resilient Metallic element.\n\nIt cannot be excavated by any Duplicant mining tool.";
		}

		public class VACUUM
		{
			public static LocString NAME = UI.FormatAsLink("Vacuum", "VACUUM");

			public static LocString DESC = "A vacuum is a space devoid of all matter.";
		}

		public class VISCOGEL
		{
			public static LocString NAME = UI.FormatAsLink("Visco-Gel", "VISCOGEL");

			public static LocString DESC = "Visco-Gel is a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " polymer with high surface tension, preventing typical liquid flow and allowing for unusual configurations.";
		}

		public class VOID
		{
			public static LocString NAME = UI.FormatAsLink("Void", "VOID");

			public static LocString DESC = "Cold, infinite nothingness.";
		}

		public class WATER
		{
			public static LocString NAME = UI.FormatAsLink("Water", "WATER");

			public static LocString DESC = "(H<sub>2</sub>O) Clean " + NAME + ", suitable for consumption.";
		}

		public class WOLFRAMITE
		{
			public static LocString NAME = UI.FormatAsLink("Wolframite", "WOLFRAMITE");

			public static LocString DESC = "((Fe,Mn)WO<sub>4</sub>) Wolframite is a dense Metallic element in a " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " state.\n\nIt is a source of " + TUNGSTEN.NAME + " and is suitable for building " + UI.FormatAsLink("Power", "POWER") + " systems.";
		}

		public class POLYPROPYLENE
		{
			public static LocString NAME = UI.FormatAsLink("Plastic", "POLYPROPYLENE");

			public static LocString DESC = "(C<sub>3</sub>H<sub>6</sub>)<sub>n</sub> " + NAME + " is a thermoplastic polymer.\n\nIt is useful as a raw Plastic feedstock.";

			public static LocString BUILD_DESC = "Buildings made of this material have antiseptic properties";
		}

		public class NAPHTHA
		{
			public static LocString NAME = UI.FormatAsLink("Naphtha", "NAPHTHA");

			public static LocString DESC = "Naphtha a distilled hydrocarbon mixture produced from the burning of " + POLYPROPYLENE.NAME + ".";
		}

		public class SLABS
		{
			public static LocString NAME = UI.FormatAsLink("Building Slab", "SLABS");

			public static LocString DESC = "Slabs are a refined mineral building block used for assembling advanced buildings.";
		}

		public static LocString ELEMENTDESCSOLID = "Resource Type: {0}\nMelting point: {1}\nHardness: {2}";

		public static LocString ELEMENTDESCLIQUID = "Resource Type: {0}\nFreezing point: {1}\nEvaporation point: {2}";

		public static LocString ELEMENTDESCGAS = "Resource Type: {0}\nCondensation point: {1}";

		public static LocString ELEMENTDESCVACUUM = "Resource Type: {0}";

		public static LocString BREATHABLEDESC = "<color=#{0}>({1})</color>";

		public static LocString THERMALPROPERTIES = "\nSpecific Heat Capacity: {SPECIFIC_HEAT_CAPACITY}\nThermal Conductivity: {THERMAL_CONDUCTIVITY}";

		public static LocString ELEMENTPROPERTIES = "Properties: {0}";
	}
}
