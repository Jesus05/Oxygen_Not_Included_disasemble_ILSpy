using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class BabyCrabShellConfig : IEntityConfig
{
	public const string ID = "BabyCrabShell";

	public static readonly Tag TAG = TagManager.Create("BabyCrabShell");

	public const float MASS = 5f;

	public GameObject CreatePrefab()
	{
		string id = "BabyCrabShell";
		string name = ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME;
		string desc = ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.DESC;
		float mass = 5f;
		bool unitMass = true;
		KAnimFile anim = Assets.GetAnim("crabshells_small_kanim");
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
