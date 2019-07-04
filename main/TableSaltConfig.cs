using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class TableSaltConfig : IEntityConfig
{
	public static string ID = "TableSalt";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(ID, ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.NAME, ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.DESC, 1f, false, Assets.GetAnim("seed_saltPlant_kanim"), "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.45f, true, SORTORDER.BUILDINGELEMENTS + TableSaltTuning.SORTORDER, SimHashes.Salt, new List<Tag>
		{
			GameTags.MiscPickupable
		});
		gameObject.AddOrGet<EntitySplitter>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
