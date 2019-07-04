using STRINGS;
using TUNING;
using UnityEngine;

public class FishMeatConfig : IEntityConfig
{
	public const string ID = "FishMeat";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("FishMeat", ITEMS.FOOD.FISHMEAT.NAME, ITEMS.FOOD.FISHMEAT.DESC, 1f, false, Assets.GetAnim("creaturemeat_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.FISH_MEAT);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
