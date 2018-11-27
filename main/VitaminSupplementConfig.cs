using STRINGS;
using TUNING;
using UnityEngine;

public class VitaminSupplementConfig : IEntityConfig
{
	public const string ID = "VitaminSupplement";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("VitaminSupplement", ITEMS.PILLS.VITAMINSUPPLEMENT.NAME, ITEMS.PILLS.VITAMINSUPPLEMENT.DESC, 1f, true, Assets.GetAnim("pill_2_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.VITAMINSUPPLEMENT);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
