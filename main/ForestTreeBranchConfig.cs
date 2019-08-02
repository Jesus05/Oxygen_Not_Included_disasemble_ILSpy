using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ForestTreeBranchConfig : IEntityConfig
{
	public const string ID = "ForestTreeBranch";

	public const float WOOD_AMOUNT = 300f;

	public GameObject CreatePrefab()
	{
		string id = "ForestTreeBranch";
		string name = STRINGS.CREATURES.SPECIES.WOOD_TREE.NAME;
		string desc = STRINGS.CREATURES.SPECIES.WOOD_TREE.DESC;
		float mass = 8f;
		KAnimFile anim = Assets.GetAnim("tree_kanim");
		string initialAnim = "idle_empty";
		EffectorValues tIER = DECOR.BONUS.TIER1;
		List<Tag> additionalTags = new List<Tag>();
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingFront, 1, 1, tIER, default(EffectorValues), SimHashes.Creature, additionalTags, 298.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 258.15f, 288.15f, 313.15f, 448.15f, null, true, 0f, 0.15f, "WoodLog", true, true, false, true, 12000f);
		gameObject.AddOrGet<TreeBud>();
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<BudUprootedMonitor>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
