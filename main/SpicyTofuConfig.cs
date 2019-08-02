using STRINGS;
using TUNING;
using UnityEngine;

public class SpicyTofuConfig : IEntityConfig
{
	public const string ID = "SpicyTofu";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("SpicyTofu", ITEMS.FOOD.SPICYTOFU.NAME, ITEMS.FOOD.SPICYTOFU.DESC, 1f, false, Assets.GetAnim("spicey_tofu_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, true, 0, SimHashes.Creature, null);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.SPICY_TOFU);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
