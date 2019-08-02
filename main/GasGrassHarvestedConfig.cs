using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class GasGrassHarvestedConfig : IEntityConfig
{
	public const string ID = "GasGrassHarvested";

	public GameObject CreatePrefab()
	{
		string id = "GasGrassHarvested";
		string name = CREATURES.SPECIES.GASGRASS.NAME;
		string desc = CREATURES.SPECIES.GASGRASS.DESC;
		float mass = 1f;
		bool unitMass = false;
		KAnimFile anim = Assets.GetAnim("harvested_gassygrass_kanim");
		string initialAnim = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.CIRCLE;
		float width = 0.25f;
		float height = 0.25f;
		bool isPickupable = true;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.Other);
		list = list;
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, desc, mass, unitMass, anim, initialAnim, sceneLayer, collisionShape, width, height, isPickupable, 0, SimHashes.Creature, list);
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
