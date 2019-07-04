using STRINGS;
using TUNING;
using UnityEngine;

public class LettuceConfig : IEntityConfig
{
	public const string ID = "Lettuce";

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("Lettuce", ITEMS.FOOD.LETTUCE.NAME, ITEMS.FOOD.LETTUCE.DESC, 1f, false, Assets.GetAnim("sea_lettuce_leaves_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.LETTUCE);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
