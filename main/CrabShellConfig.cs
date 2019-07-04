using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class CrabShellConfig : IEntityConfig
{
	public const string ID = "CrabShell";

	public static readonly Tag TAG = TagManager.Create("CrabShell");

	public const float MASS = 10f;

	public GameObject CreatePrefab()
	{
		string id = "CrabShell";
		string name = ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME;
		string desc = ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.DESC;
		float mass = 10f;
		bool unitMass = true;
		KAnimFile anim = Assets.GetAnim("crabshells_large_kanim");
		string initialAnim = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.RECTANGLE;
		float width = 0.9f;
		float height = 0.6f;
		bool isPickupable = true;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.IndustrialIngredient);
		list.Add(GameTags.Organics);
		list = list;
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, desc, mass, unitMass, anim, initialAnim, sceneLayer, collisionShape, width, height, isPickupable, 0, SimHashes.Creature, list);
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddOrGet<SimpleMassStatusItem>();
		EntityTemplates.CreateAndRegisterCompostableFromPrefab(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
