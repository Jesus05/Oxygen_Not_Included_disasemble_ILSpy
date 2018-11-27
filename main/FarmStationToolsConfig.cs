using STRINGS;
using UnityEngine;

public class FarmStationToolsConfig : IEntityConfig
{
	public const string ID = "FarmStationTools";

	public static readonly Tag tag = TagManager.Create("FarmStationTools");

	public const float MASS = 5f;

	public GameObject CreatePrefab()
	{
		return EntityTemplates.CreateLooseEntity("FarmStationTools", ITEMS.INDUSTRIAL_PRODUCTS.FARM_STATION_TOOLS.NAME, ITEMS.INDUSTRIAL_PRODUCTS.FARM_STATION_TOOLS.DESC, 5f, true, Assets.GetAnim("kit_planttender_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, true, 0, SimHashes.Creature, null);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
