using STRINGS;
using System.Collections.Generic;

namespace Database
{
	public class SpaceDestinationTypes : ResourceSet<SpaceDestinationType>
	{
		public SpaceDestinationType Satellite;

		public SpaceDestinationType MetallicAsteroid;

		public SpaceDestinationType RockyAsteroid;

		public SpaceDestinationType CarbonaceousAsteroid;

		public SpaceDestinationType IcyDwarf;

		public SpaceDestinationType OrganicDwarf;

		public SpaceDestinationType DustyMoon;

		public SpaceDestinationType TerraPlanet;

		public SpaceDestinationType VolcanoPlanet;

		public SpaceDestinationType GasGiant;

		public SpaceDestinationType IceGiant;

		public SpaceDestinationType Wormhole;

		public SpaceDestinationType SaltDwarf;

		public SpaceDestinationType RustPlanet;

		public SpaceDestinationType ForestPlanet;

		public SpaceDestinationType RedDwarf;

		public SpaceDestinationType GoldAsteroid;

		public SpaceDestinationType HydrogenGiant;

		public SpaceDestinationType OilyAsteroid;

		public SpaceDestinationType ShinyPlanet;

		public SpaceDestinationType ChlorinePlanet;

		public SpaceDestinationType SaltDesertPlanet;

		public SpaceDestinationType Earth;

		public static Dictionary<SimHashes, MathUtil.MinMax> extendedElementTable = new Dictionary<SimHashes, MathUtil.MinMax>
		{
			{
				SimHashes.Niobium,
				new MathUtil.MinMax(10f, 20f)
			},
			{
				SimHashes.Katairite,
				new MathUtil.MinMax(50f, 100f)
			},
			{
				SimHashes.Isoresin,
				new MathUtil.MinMax(30f, 60f)
			},
			{
				SimHashes.Fullerene,
				new MathUtil.MinMax(0.5f, 1f)
			}
		};

		public SpaceDestinationTypes(ResourceSet parent)
			: base("SpaceDestinations", parent)
		{
			string id = "Satellite";
			string name = UI.SPACEDESTINATIONS.DEBRIS.SATELLITE.NAME;
			string description = UI.SPACEDESTINATIONS.DEBRIS.SATELLITE.DESCRIPTION;
			int iconSize = 16;
			string spriteName = "asteroid";
			Dictionary<SimHashes, MathUtil.MinMax> elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Steel,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Copper,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Glass,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			ArtifactDropRate bad = Db.Get().ArtifactDropRates.Bad;
			Satellite = Add(new SpaceDestinationType(id, parent, name, description, iconSize, spriteName, elementTable, null, bad, 64000000, 63994000, 18, true));
			MetallicAsteroid = Add(new SpaceDestinationType("MetallicAsteroid", parent, UI.SPACEDESTINATIONS.ASTEROIDS.METALLICASTEROID.NAME, UI.SPACEDESTINATIONS.ASTEROIDS.METALLICASTEROID.DESCRIPTION, 32, "nebula", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Iron,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Copper,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Obsidian,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int>
			{
				{
					"HatchMetal",
					3
				}
			}, Db.Get().ArtifactDropRates.Mediocre, 128000000, 127988000, 12, true));
			RockyAsteroid = Add(new SpaceDestinationType("RockyAsteroid", parent, UI.SPACEDESTINATIONS.ASTEROIDS.ROCKYASTEROID.NAME, UI.SPACEDESTINATIONS.ASTEROIDS.ROCKYASTEROID.DESCRIPTION, 32, "new_12", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Cuprite,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SedimentaryRock,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.IgneousRock,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int>
			{
				{
					"HatchHard",
					3
				}
			}, Db.Get().ArtifactDropRates.Good, 128000000, 127988000, 18, true));
			spriteName = "CarbonaceousAsteroid";
			description = UI.SPACEDESTINATIONS.ASTEROIDS.CARBONACEOUSASTEROID.NAME;
			name = UI.SPACEDESTINATIONS.ASTEROIDS.CARBONACEOUSASTEROID.DESCRIPTION;
			iconSize = 32;
			id = "new_08";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.RefinedCarbon,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Carbon,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Diamond,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Mediocre;
			CarbonaceousAsteroid = Add(new SpaceDestinationType(spriteName, parent, description, name, iconSize, id, elementTable, null, bad, 128000000, 127988000, 6, true));
			IcyDwarf = Add(new SpaceDestinationType("IcyDwarf", parent, UI.SPACEDESTINATIONS.DWARFPLANETS.ICYDWARF.NAME, UI.SPACEDESTINATIONS.DWARFPLANETS.ICYDWARF.DESCRIPTION, 64, "icyMoon", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Ice,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidCarbonDioxide,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidOxygen,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int>
			{
				{
					"ColdBreatherSeed",
					3
				},
				{
					"ColdWheatSeed",
					4
				}
			}, Db.Get().ArtifactDropRates.Great, 256000000, 255982000, 24, true));
			OrganicDwarf = Add(new SpaceDestinationType("OrganicDwarf", parent, UI.SPACEDESTINATIONS.DWARFPLANETS.ORGANICDWARF.NAME, UI.SPACEDESTINATIONS.DWARFPLANETS.ORGANICDWARF.DESCRIPTION, 64, "organicAsteroid", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.SlimeMold,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Algae,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.ContaminatedOxygen,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int>
			{
				{
					"Moo",
					1
				},
				{
					"GasGrassSeed",
					4
				}
			}, Db.Get().ArtifactDropRates.Great, 256000000, 255982000, 30, true));
			id = "DustyMoon";
			name = UI.SPACEDESTINATIONS.DWARFPLANETS.DUSTYDWARF.NAME;
			description = UI.SPACEDESTINATIONS.DWARFPLANETS.DUSTYDWARF.DESCRIPTION;
			iconSize = 64;
			spriteName = "new_05";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Regolith,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.MaficRock,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SedimentaryRock,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Amazing;
			DustyMoon = Add(new SpaceDestinationType(id, parent, name, description, iconSize, spriteName, elementTable, null, bad, 256000000, 255982000, 42, true));
			TerraPlanet = Add(new SpaceDestinationType("TerraPlanet", parent, UI.SPACEDESTINATIONS.PLANETS.TERRAPLANET.NAME, UI.SPACEDESTINATIONS.PLANETS.TERRAPLANET.DESCRIPTION, 96, "terra", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Water,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Algae,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Oxygen,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Dirt,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int>
			{
				{
					"PrickleFlowerSeed",
					4
				},
				{
					"PacuEgg",
					4
				}
			}, Db.Get().ArtifactDropRates.Amazing, 384000000, 383980000, 54, true));
			spriteName = "VolcanoPlanet";
			description = UI.SPACEDESTINATIONS.PLANETS.VOLCANOPLANET.NAME;
			name = UI.SPACEDESTINATIONS.PLANETS.VOLCANOPLANET.DESCRIPTION;
			iconSize = 96;
			id = "planet";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Magma,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.IgneousRock,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Katairite,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Amazing;
			VolcanoPlanet = Add(new SpaceDestinationType(spriteName, parent, description, name, iconSize, id, elementTable, null, bad, 384000000, 383980000, 54, true));
			id = "GasGiant";
			name = UI.SPACEDESTINATIONS.GIANTS.GASGIANT.NAME;
			description = UI.SPACEDESTINATIONS.GIANTS.GASGIANT.DESCRIPTION;
			iconSize = 96;
			spriteName = "gasGiant";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Methane,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Hydrogen,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Perfect;
			GasGiant = Add(new SpaceDestinationType(id, parent, name, description, iconSize, spriteName, elementTable, null, bad, 384000000, 383980000, 60, true));
			spriteName = "IceGiant";
			description = UI.SPACEDESTINATIONS.GIANTS.ICEGIANT.NAME;
			name = UI.SPACEDESTINATIONS.GIANTS.ICEGIANT.DESCRIPTION;
			iconSize = 96;
			id = "icyMoon";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Ice,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidCarbonDioxide,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidOxygen,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidMethane,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Perfect;
			IceGiant = Add(new SpaceDestinationType(spriteName, parent, description, name, iconSize, id, elementTable, null, bad, 384000000, 383980000, 60, true));
			SaltDwarf = Add(new SpaceDestinationType("SaltDwarf", parent, UI.SPACEDESTINATIONS.DWARFPLANETS.SALTDWARF.NAME, UI.SPACEDESTINATIONS.DWARFPLANETS.SALTDWARF.DESCRIPTION, 64, "new_01", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.SaltWater,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidCarbonDioxide,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Brine,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int>
			{
				{
					"SaltPlantSeed",
					3
				}
			}, Db.Get().ArtifactDropRates.Bad, 256000000, 255982000, 30, true));
			id = "RustPlanet";
			name = UI.SPACEDESTINATIONS.PLANETS.RUSTPLANET.NAME;
			description = UI.SPACEDESTINATIONS.PLANETS.RUSTPLANET.DESCRIPTION;
			iconSize = 96;
			spriteName = "new_06";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Rust,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidCarbonDioxide,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Perfect;
			RustPlanet = Add(new SpaceDestinationType(id, parent, name, description, iconSize, spriteName, elementTable, null, bad, 384000000, 383980000, 60, true));
			ForestPlanet = Add(new SpaceDestinationType("ForestPlanet", parent, UI.SPACEDESTINATIONS.PLANETS.FORESTPLANET.NAME, UI.SPACEDESTINATIONS.PLANETS.FORESTPLANET.DESCRIPTION, 96, "new_07", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.AluminumOre,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidOxygen,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int>
			{
				{
					"Squirrel",
					1
				},
				{
					"ForestTreeSeed",
					4
				}
			}, Db.Get().ArtifactDropRates.Mediocre, 384000000, 383980000, 24, true));
			spriteName = "RedDwarf";
			description = UI.SPACEDESTINATIONS.DWARFPLANETS.REDDWARF.NAME;
			name = UI.SPACEDESTINATIONS.DWARFPLANETS.REDDWARF.DESCRIPTION;
			iconSize = 64;
			id = "sun";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Aluminum,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.LiquidMethane,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Fossil,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Amazing;
			RedDwarf = Add(new SpaceDestinationType(spriteName, parent, description, name, iconSize, id, elementTable, null, bad, 256000000, 255982000, 42, true));
			id = "GoldAsteroid";
			name = UI.SPACEDESTINATIONS.ASTEROIDS.GOLDASTEROID.NAME;
			description = UI.SPACEDESTINATIONS.ASTEROIDS.GOLDASTEROID.DESCRIPTION;
			iconSize = 32;
			spriteName = "new_02";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Gold,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Fullerene,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.FoolsGold,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Bad;
			GoldAsteroid = Add(new SpaceDestinationType(id, parent, name, description, iconSize, spriteName, elementTable, null, bad, 128000000, 127988000, 90, true));
			spriteName = "HeliumGiant";
			description = UI.SPACEDESTINATIONS.GIANTS.HYDROGENGIANT.NAME;
			name = UI.SPACEDESTINATIONS.GIANTS.HYDROGENGIANT.DESCRIPTION;
			iconSize = 96;
			id = "new_11";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.LiquidHydrogen,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Water,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Niobium,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Mediocre;
			HydrogenGiant = Add(new SpaceDestinationType(spriteName, parent, description, name, iconSize, id, elementTable, null, bad, 384000000, 383980000, 78, true));
			id = "OilyAsteriod";
			name = UI.SPACEDESTINATIONS.ASTEROIDS.OILYASTEROID.NAME;
			description = UI.SPACEDESTINATIONS.ASTEROIDS.OILYASTEROID.DESCRIPTION;
			iconSize = 32;
			spriteName = "new_09";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.SolidMethane,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidCarbonDioxide,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.CrudeOil,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Petroleum,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Mediocre;
			OilyAsteroid = Add(new SpaceDestinationType(id, parent, name, description, iconSize, spriteName, elementTable, null, bad, 128000000, 127988000, 12, true));
			spriteName = "ShinyPlanet";
			description = UI.SPACEDESTINATIONS.PLANETS.SHINYPLANET.NAME;
			name = UI.SPACEDESTINATIONS.PLANETS.SHINYPLANET.DESCRIPTION;
			iconSize = 96;
			id = "new_04";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Tungsten,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Wolframite,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Good;
			ShinyPlanet = Add(new SpaceDestinationType(spriteName, parent, description, name, iconSize, id, elementTable, null, bad, 384000000, 383980000, 84, true));
			id = "ChlorinePlanet";
			name = UI.SPACEDESTINATIONS.PLANETS.CHLORINEPLANET.NAME;
			description = UI.SPACEDESTINATIONS.PLANETS.CHLORINEPLANET.DESCRIPTION;
			iconSize = 96;
			spriteName = "new_10";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.SolidChlorine,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.BleachStone,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Bad;
			ChlorinePlanet = Add(new SpaceDestinationType(id, parent, name, description, iconSize, spriteName, elementTable, null, bad, 256000000, 255982000, 90, true));
			SaltDesertPlanet = Add(new SpaceDestinationType("SaltDesertPlanet", parent, UI.SPACEDESTINATIONS.PLANETS.SALTDESERTPLANET.NAME, UI.SPACEDESTINATIONS.PLANETS.SALTDESERTPLANET.DESCRIPTION, 96, "new_10", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Salt,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.CrushedRock,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int>
			{
				{
					"Crab",
					1
				}
			}, Db.Get().ArtifactDropRates.Bad, 384000000, 383980000, 60, true));
			spriteName = "Wormhole";
			description = UI.SPACEDESTINATIONS.WORMHOLE.NAME;
			name = UI.SPACEDESTINATIONS.WORMHOLE.DESCRIPTION;
			iconSize = 96;
			id = "new_03";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Vacuum,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Perfect;
			Wormhole = Add(new SpaceDestinationType(spriteName, parent, description, name, iconSize, id, elementTable, null, bad, 0, 0, 0, true));
			id = "Earth";
			name = UI.SPACEDESTINATIONS.EARTH.NAME;
			description = UI.SPACEDESTINATIONS.EARTH.DESCRIPTION;
			iconSize = 96;
			spriteName = "earth";
			elementTable = new Dictionary<SimHashes, MathUtil.MinMax>();
			bad = Db.Get().ArtifactDropRates.None;
			Earth = Add(new SpaceDestinationType(id, parent, name, description, iconSize, spriteName, elementTable, null, bad, 0, 0, 0, false));
		}
	}
}
