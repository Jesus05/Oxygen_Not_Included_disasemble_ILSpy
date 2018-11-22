using STRINGS;
using TUNING;
using UnityEngine;

public class GenericPillConfig : IEntityConfig
{
	public const string ID = "GenericPill";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("GenericPill", ITEMS.PILLS.PLACEBO.NAME, ITEMS.PILLS.PLACEBO.DESC, 1f, true, Assets.GetAnim("pill_1_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.GENERICPILL);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
