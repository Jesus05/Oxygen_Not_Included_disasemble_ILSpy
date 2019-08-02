using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class WoodLogConfig : IEntityConfig
{
	public const string ID = "WoodLog";

	public static readonly Tag TAG = TagManager.Create("WoodLog");

	public GameObject CreatePrefab()
	{
		string id = "WoodLog";
		string name = ITEMS.INDUSTRIAL_PRODUCTS.WOOD.NAME;
		string desc = ITEMS.INDUSTRIAL_PRODUCTS.WOOD.DESC;
		float mass = 1f;
		bool unitMass = false;
		KAnimFile anim = Assets.GetAnim("wood_kanim");
		string initialAnim = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.CIRCLE;
		float width = 0.35f;
		float height = 0.35f;
		bool isPickupable = true;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.IndustrialIngredient);
		list.Add(GameTags.Organics);
		list = list;
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, desc, mass, unitMass, anim, initialAnim, sceneLayer, collisionShape, width, height, isPickupable, 0, SimHashes.Creature, list);
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddOrGet<SimpleMassStatusItem>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
