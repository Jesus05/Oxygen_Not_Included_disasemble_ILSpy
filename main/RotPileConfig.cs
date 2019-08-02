using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class RotPileConfig : IEntityConfig
{
	public static string ID = "RotPile";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(ID, ITEMS.FOOD.ROTPILE.NAME, ITEMS.FOOD.ROTPILE.DESC, 1f, false, Assets.GetAnim("rotfood_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Organics, false);
		component.AddTag(GameTags.Compostable, false);
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddOrGet<OccupyArea>();
		gameObject.AddOrGet<Modifiers>();
		gameObject.AddOrGet<RotPile>();
		DecorProvider decorProvider = gameObject.AddComponent<DecorProvider>();
		decorProvider.SetValues(DECOR.PENALTY.TIER2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<DecorProvider>().overrideName = ITEMS.FOOD.ROTPILE.NAME;
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
