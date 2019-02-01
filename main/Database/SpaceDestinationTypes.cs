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
			Satellite = Add(new SpaceDestinationType(id, parent, name, description, iconSize, spriteName, elementTable, null, bad));
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
			}, Db.Get().ArtifactDropRates.Mediocre));
			RockyAsteroid = Add(new SpaceDestinationType("RockyAsteroid", parent, UI.SPACEDESTINATIONS.ASTEROIDS.ROCKYASTEROID.NAME, UI.SPACEDESTINATIONS.ASTEROIDS.ROCKYASTEROID.DESCRIPTION, 32, "asteroid", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.IronOre,
					new MathUtil.MinMax(100f, 200f)
				},
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
			}, Db.Get().ArtifactDropRates.Good));
			spriteName = "CarbonaceousAsteroid";
			description = UI.SPACEDESTINATIONS.ASTEROIDS.CARBONACEOUSASTEROID.NAME;
			name = UI.SPACEDESTINATIONS.ASTEROIDS.CARBONACEOUSASTEROID.DESCRIPTION;
			iconSize = 32;
			id = "asteroid";
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
			CarbonaceousAsteroid = Add(new SpaceDestinationType(spriteName, parent, description, name, iconSize, id, elementTable, null, bad));
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
				},
				{
					SimHashes.SolidMethane,
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
			}, Db.Get().ArtifactDropRates.Great));
			OrganicDwarf = Add(new SpaceDestinationType("OrganicDwarf", parent, UI.SPACEDESTINATIONS.DWARFPLANETS.ORGANICDWARF.NAME, UI.SPACEDESTINATIONS.DWARFPLANETS.ORGANICDWARF.DESCRIPTION, 64, "organicAsteroid", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.SlimeMold,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.DirtyWater,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Algae,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.CarbonDioxide,
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
			}, Db.Get().ArtifactDropRates.Great));
			id = "DustyMoon";
			name = UI.SPACEDESTINATIONS.DWARFPLANETS.DUSTYDWARF.NAME;
			description = UI.SPACEDESTINATIONS.DWARFPLANETS.DUSTYDWARF.DESCRIPTION;
			iconSize = 64;
			spriteName = "asteroid";
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
			DustyMoon = Add(new SpaceDestinationType(id, parent, name, description, iconSize, spriteName, elementTable, null, bad));
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
			}, Db.Get().ArtifactDropRates.Amazing));
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
					SimHashes.Obsidian,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Katairite,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			bad = Db.Get().ArtifactDropRates.Amazing;
			VolcanoPlanet = Add(new SpaceDestinationType(spriteName, parent, description, name, iconSize, id, elementTable, null, bad));
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
			GasGiant = Add(new SpaceDestinationType(id, parent, name, description, iconSize, spriteName, elementTable, null, bad));
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
			IceGiant = Add(new SpaceDestinationType(spriteName, parent, description, name, iconSize, id, elementTable, null, bad));
		}
	}
}
