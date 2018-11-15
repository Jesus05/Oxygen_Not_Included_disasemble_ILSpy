using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class BasicFabricConfig : IEntityConfig
{
	public static string ID = "BasicFabric";

	public GameObject CreatePrefab()
	{
		string iD = ID;
		string name = ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.NAME;
		string desc = ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.DESC;
		float mass = 1f;
		bool unitMass = true;
		KAnimFile anim = Assets.GetAnim("swampreedwool_kanim");
		string initialAnim = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.BuildingBack;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.RECTANGLE;
		float width = 0.8f;
		float height = 0.45f;
		bool isPickupable = true;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.IndustrialIngredient);
		list = list;
		GameObject gameObject = EntityTemplates.CreateLooseEntity(iD, name, desc, mass, unitMass, anim, initialAnim, sceneLayer, collisionShape, width, height, isPickupable, SimHashes.Creature, list);
		gameObject.AddOrGet<EntitySplitter>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
