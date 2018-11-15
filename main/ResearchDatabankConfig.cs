using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ResearchDatabankConfig : IEntityConfig
{
	public const string ID = "ResearchDatabank";

	public static readonly Tag TAG = TagManager.Create("ResearchDatabank");

	public const float MASS = 1f;

	public GameObject CreatePrefab()
	{
		string id = "ResearchDatabank";
		string name = ITEMS.INDUSTRIAL_PRODUCTS.RESEARCH_DATABANK.NAME;
		string desc = ITEMS.INDUSTRIAL_PRODUCTS.RESEARCH_DATABANK.DESC;
		float mass = 1f;
		bool unitMass = true;
		KAnimFile anim = Assets.GetAnim("floppy_disc_kanim");
		string initialAnim = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.CIRCLE;
		float width = 0.35f;
		float height = 0.35f;
		bool isPickupable = true;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.IndustrialIngredient);
		list.Add(GameTags.Experimental);
		list = list;
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, desc, mass, unitMass, anim, initialAnim, sceneLayer, collisionShape, width, height, isPickupable, SimHashes.Creature, list);
		EntitySplitter entitySplitter = gameObject.AddOrGet<EntitySplitter>();
		entitySplitter.maxStackSize = (float)ROCKETRY.DESTINATION_RESEARCH.BASIC;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
