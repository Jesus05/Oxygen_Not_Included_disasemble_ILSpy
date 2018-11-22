using STRINGS;
using TUNING;
using UnityEngine;

public class GrilledPrickleFruitConfig : IEntityConfig
{
	public const string ID = "GrilledPrickleFruit";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("GrilledPrickleFruit", ITEMS.FOOD.GRILLEDPRICKLEFRUIT.NAME, ITEMS.FOOD.GRILLEDPRICKLEFRUIT.DESC, 1f, false, Assets.GetAnim("gristleberry_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.7f, true, SimHashes.Creature, null);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.GRILLED_PRICKLEFRUIT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
