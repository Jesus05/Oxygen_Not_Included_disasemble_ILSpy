using STRINGS;
using TUNING;
using UnityEngine;

public class SalsaConfig : IEntityConfig
{
	public const string ID = "Salsa";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("Salsa", ITEMS.FOOD.SALSA.NAME, ITEMS.FOOD.SALSA.DESC, 1f, false, Assets.GetAnim("zestysalsa_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.5f, true, SimHashes.Creature, null);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.SALSA);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
