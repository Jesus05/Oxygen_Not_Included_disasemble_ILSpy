using STRINGS;
using TUNING;
using UnityEngine;

public class PickledMealConfig : IEntityConfig
{
	public const string ID = "PickledMeal";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("PickledMeal", ITEMS.FOOD.PICKLEDMEAL.NAME, ITEMS.FOOD.PICKLEDMEAL.DESC, 1f, false, Assets.GetAnim("pickledmeal_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.7f, true, SimHashes.Creature, null);
		template = EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.PICKLEDMEAL);
		KPrefabID component = template.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Pickled);
		return template;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
