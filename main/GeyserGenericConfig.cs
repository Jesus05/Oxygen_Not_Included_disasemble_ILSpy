using Klei;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GeyserGenericConfig : IMultiEntityConfig
{
	public struct GeyserPrefabParams
	{
		public string id;

		public string anim;

		public int width;

		public int height;

		public StringKey nameStringKey;

		public StringKey descStringKey;

		public GeyserConfigurator.GeyserType geyserType;

		public GeyserPrefabParams(string anim, int width, int height, GeyserConfigurator.GeyserType geyserType)
		{
			id = "GeyserGeneric_" + geyserType.id;
			this.anim = anim;
			this.width = width;
			this.height = height;
			nameStringKey = new StringKey("STRINGS.CREATURES.SPECIES.GEYSER." + geyserType.id.ToUpper() + ".NAME");
			descStringKey = new StringKey("STRINGS.CREATURES.SPECIES.GEYSER." + geyserType.id.ToUpper() + ".DESC");
			this.geyserType = geyserType;
		}
	}

	private static class TEMPERATURES
	{
		public const float BELOW_FREEZING = 263.15f;

		public const float DUPE_NORMAL = 303.15f;

		public const float DUPE_HOT = 333.15f;

		public const float BELOW_BOILING = 368.15f;

		public const float ABOVE_BOILING = 383.15f;

		public const float HOT1 = 423.15f;

		public const float HOT2 = 773.15f;

		public const float MOLTEN_MAGMA = 2000f;
	}

	private static class RATES
	{
		public const float GAS_TINY_MIN = 3f;

		public const float GAS_TINY_MAX = 30f;

		public const float GAS_SMALL_MIN = 5f;

		public const float GAS_SMALL_MAX = 50f;

		public const float GAS_NORMAL_MIN = 10f;

		public const float GAS_NORMAL_MAX = 100f;

		public const float GAS_BIG_MIN = 15f;

		public const float GAS_BIG_MAX = 180f;

		public const float LIQUID_LOW_MIN = 200f;

		public const float LIQUID_LOW_MAX = 2500f;

		public const float LIQUID_NORMAL_MIN = 500f;

		public const float LIQUID_NORMAL_MAX = 5000f;

		public const float MOLTEN_NORMAL_MIN = 50f;

		public const float MOLTEN_NORMAL_MAX = 500f;

		public const float MOLTEN_BIG_MIN = 100f;

		public const float MOLTEN_BIG_MAX = 1000f;

		public const float MOLTEN_HUGE_MIN = 200f;

		public const float MOLTEN_HUGE_MAX = 2000f;
	}

	private static class MAX_PRESURES
	{
		public const float GAS = 5f;

		public const float GAS_HIGH = 15f;

		public const float MOLTEN = 150f;

		public const float LIQUID_SMALL = 50f;

		public const float LIQUID = 500f;
	}

	private static class ITERATIONS
	{
		public static class INFREQUENT_MOLTEN
		{
			public const float PCT_MIN = 0.005f;

			public const float PCT_MAX = 0.01f;

			public const float LEN_MIN = 6000f;

			public const float LEN_MAX = 12000f;
		}

		public static class FREQUENT_MOLTEN
		{
			public const float PCT_MIN = 0.0166666675f;

			public const float PCT_MAX = 0.1f;

			public const float LEN_MIN = 480f;

			public const float LEN_MAX = 1080f;
		}
	}

	public const string Steam = "steam";

	public const string HotSteam = "hot_steam";

	public const string HotWater = "hot_water";

	public const string SlushWater = "slush_water";

	public const string FilthyWater = "filthy_water";

	public const string SmallVolcano = "small_volcano";

	public const string BigVolcano = "big_volcano";

	public const string LiquidCO2 = "liquid_co2";

	public const string HotCO2 = "hot_co2";

	public const string HotHydrogen = "hot_hydrogen";

	public const string HotPO2 = "hot_po2";

	public const string SlimyPO2 = "slimy_po2";

	public const string ChlorineGas = "chlorine_gas";

	public const string Methane = "methane";

	public const string MoltenCopper = "molten_copper";

	public const string MoltenIron = "molten_iron";

	public const string MoltenGold = "molten_gold";

	public const string OilDrip = "oil_drip";

	public List<GameObject> CreatePrefabs()
	{
		List<GameObject> list = new List<GameObject>();
		List<GeyserPrefabParams> configs = GenerateConfigs();
		foreach (GeyserPrefabParams item in configs)
		{
			GeyserPrefabParams current = item;
			list.Add(CreateGeyser(current.id, current.anim, current.width, current.height, Strings.Get(current.nameStringKey), Strings.Get(current.descStringKey), current.geyserType.idHash));
		}
		GameObject gameObject = EntityTemplates.CreateEntity("GeyserGeneric", "Random Geyser Spawner", true);
		gameObject.AddOrGet<SaveLoadRoot>();
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			int num = 0;
			if (SaveLoader.Instance.worldDetailSave != null)
			{
				num = SaveLoader.Instance.worldDetailSave.globalWorldSeed;
			}
			else
			{
				Debug.LogWarning("Could not load global world seed for geysers", null);
			}
			int num2 = num;
			Vector3 position = inst.transform.GetPosition();
			int num3 = num2 + (int)position.x;
			Vector3 position2 = inst.transform.GetPosition();
			num = num3 + (int)position2.y;
			System.Random random = new System.Random(num);
			int index = random.Next(0, configs.Count);
			GeyserPrefabParams geyserPrefabParams = configs[index];
			GameObject gameObject2 = GameUtil.KInstantiate(Assets.GetPrefab(geyserPrefabParams.id), inst.transform.GetPosition(), Grid.SceneLayer.BuildingBack, null, 0);
			gameObject2.SetActive(true);
			inst.DeleteObject();
		};
		list.Add(gameObject);
		return list;
	}

	public GameObject CreateGeyser(string id, string anim, int width, int height, string name, string desc, HashedString presetType)
	{
		float mass = 2000f;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, Assets.GetAnim(anim), "inactive", Grid.SceneLayer.BuildingBack, width, height, BUILDINGS.DECOR.BONUS.TIER1, NOISE_POLLUTION.NOISY.TIER6, SimHashes.Creature, null, 293f);
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Katairite);
		component.Temperature = 372.15f;
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddOrGet<Uncoverable>();
		Geyser geyser = gameObject.AddOrGet<Geyser>();
		geyser.outputOffset = new Vector2I(0, 1);
		GeyserConfigurator geyserConfigurator = gameObject.AddOrGet<GeyserConfigurator>();
		geyserConfigurator.presetType = presetType;
		Studyable studyable = gameObject.AddOrGet<Studyable>();
		studyable.meterTrackerSymbol = "geotracker_target";
		studyable.meterAnim = "tracker";
		gameObject.AddOrGet<LoopingSounds>();
		SoundEventVolumeCache.instance.AddVolume("geyser_side_steam_kanim", "Geyser_shake_LP", NOISE_POLLUTION.NOISY.TIER5);
		SoundEventVolumeCache.instance.AddVolume("geyser_side_steam_kanim", "Geyser_erupt_LP", NOISE_POLLUTION.NOISY.TIER6);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	private List<GeyserPrefabParams> GenerateConfigs()
	{
		List<GeyserPrefabParams> list = new List<GeyserPrefabParams>();
		list.Add(new GeyserPrefabParams("geyser_gas_steam_kanim", 2, 4, new GeyserConfigurator.GeyserType("steam", SimHashes.Steam, 383.15f, 200f, 2500f, 5f, 60f, 1140f, 0.1f, 0.9f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_gas_steam_hot_kanim", 2, 4, new GeyserConfigurator.GeyserType("hot_steam", SimHashes.Steam, 773.15f, 10f, 100f, 15f, 60f, 1140f, 0.1f, 0.9f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_liquid_water_hot_kanim", 4, 2, new GeyserConfigurator.GeyserType("hot_water", SimHashes.Water, 368.15f, 500f, 5000f, 500f, 60f, 1140f, 0.1f, 0.9f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_liquid_water_slush_kanim", 4, 2, new GeyserConfigurator.GeyserType("slush_water", SimHashes.DirtyWater, 263.15f, 500f, 5000f, 500f, 60f, 1140f, 0.1f, 0.9f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_liquid_water_filthy_kanim", 4, 2, new GeyserConfigurator.GeyserType("filthy_water", SimHashes.DirtyWater, 303.15f, 500f, 5000f, 500f, 60f, 1140f, 0.1f, 0.9f, 15000f, 135000f, 0.4f, 0.8f).AddDisease(new SimUtil.DiseaseInfo
		{
			idx = Db.Get().Diseases.GetIndex("FoodPoisoning"),
			count = 20000
		})));
		list.Add(new GeyserPrefabParams("geyser_molten_volcano_small_kanim", 3, 3, new GeyserConfigurator.GeyserType("small_volcano", SimHashes.Magma, 2000f, 100f, 1000f, 150f, 6000f, 12000f, 0.005f, 0.01f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_molten_volcano_big_kanim", 3, 3, new GeyserConfigurator.GeyserType("big_volcano", SimHashes.Magma, 2000f, 200f, 2000f, 150f, 6000f, 12000f, 0.005f, 0.01f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_liquid_co2_kanim", 4, 2, new GeyserConfigurator.GeyserType("liquid_co2", SimHashes.LiquidCarbonDioxide, 218f, 3f, 30f, 50f, 60f, 1140f, 0.1f, 0.9f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_gas_co2_hot_kanim", 2, 4, new GeyserConfigurator.GeyserType("hot_co2", SimHashes.CarbonDioxide, 773.15f, 5f, 50f, 5f, 60f, 1140f, 0.1f, 0.9f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_gas_hydrogen_hot_kanim", 2, 4, new GeyserConfigurator.GeyserType("hot_hydrogen", SimHashes.Hydrogen, 773.15f, 5f, 50f, 5f, 60f, 1140f, 0.1f, 0.9f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_gas_po2_hot_kanim", 2, 4, new GeyserConfigurator.GeyserType("hot_po2", SimHashes.ContaminatedOxygen, 773.15f, 15f, 180f, 5f, 60f, 1140f, 0.1f, 0.9f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_gas_po2_slimy_kanim", 2, 4, new GeyserConfigurator.GeyserType("slimy_po2", SimHashes.ContaminatedOxygen, 333.15f, 15f, 180f, 5f, 60f, 1140f, 0.1f, 0.9f, 15000f, 135000f, 0.4f, 0.8f).AddDisease(new SimUtil.DiseaseInfo
		{
			idx = Db.Get().Diseases.GetIndex("SlimeLung"),
			count = 5000
		})));
		list.Add(new GeyserPrefabParams("geyser_gas_chlorine_kanim", 2, 4, new GeyserConfigurator.GeyserType("chlorine_gas", SimHashes.ChlorineGas, 333.15f, 15f, 180f, 5f, 60f, 1140f, 0.1f, 0.9f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_gas_methane_kanim", 2, 4, new GeyserConfigurator.GeyserType("methane", SimHashes.Methane, 423.15f, 15f, 180f, 5f, 60f, 1140f, 0.1f, 0.9f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_molten_copper_kanim", 3, 3, new GeyserConfigurator.GeyserType("molten_copper", SimHashes.MoltenCopper, 2500f, 50f, 500f, 150f, 480f, 1080f, 0.0166666675f, 0.1f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_molten_iron_kanim", 3, 3, new GeyserConfigurator.GeyserType("molten_iron", SimHashes.MoltenIron, 2800f, 50f, 500f, 150f, 480f, 1080f, 0.0166666675f, 0.1f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_molten_gold_kanim", 3, 3, new GeyserConfigurator.GeyserType("molten_gold", SimHashes.MoltenGold, 2900f, 50f, 500f, 150f, 480f, 1080f, 0.0166666675f, 0.1f, 15000f, 135000f, 0.4f, 0.8f)));
		list.Add(new GeyserPrefabParams("geyser_liquid_oil_kanim", 4, 2, new GeyserConfigurator.GeyserType("oil_drip", SimHashes.CrudeOil, 600f, 1f, 250f, 50f, 600f, 600f, 1f, 1f, 100f, 500f, 0.4f, 0.8f)));
		return list;
	}
}
