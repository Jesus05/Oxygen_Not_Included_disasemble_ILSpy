using STRINGS;
using TUNING;
using UnityEngine;

public class MeatConfig : IEntityConfig
{
	public const string ID = "Meat";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("Meat", ITEMS.FOOD.MEAT.NAME, ITEMS.FOOD.MEAT.DESC, 1f, false, Assets.GetAnim("creaturemeat_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.MEAT);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
