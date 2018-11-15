using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class SwampLilyFlowerConfig : IEntityConfig
{
	public static float SEEDS_PER_FRUIT = 1f;

	public static string ID = "SwampLilyFlower";

	public GameObject CreatePrefab()
	{
		string iD = ID;
		string name = ITEMS.INGREDIENTS.SWAMPLILYFLOWER.NAME;
		string desc = ITEMS.INGREDIENTS.SWAMPLILYFLOWER.DESC;
		float mass = 1f;
		bool unitMass = false;
		KAnimFile anim = Assets.GetAnim("swamplilyflower_kanim");
		string initialAnim = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.RECTANGLE;
		float width = 0.8f;
		float height = 0.4f;
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
