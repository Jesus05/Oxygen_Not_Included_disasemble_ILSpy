using STRINGS;
using TUNING;
using UnityEngine;

public class ForestForagePlantConfig : IEntityConfig
{
	public const string ID = "ForestForagePlant";

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("ForestForagePlant", ITEMS.FOOD.FORESTFORAGEPLANT.NAME, ITEMS.FOOD.FORESTFORAGEPLANT.DESC, 1f, false, Assets.GetAnim("podmelon_fruit_kanim"), "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, true, 0, SimHashes.Creature, null);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.FORESTFORAGEPLANT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
