using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class FarmStationToolsConfig : IEntityConfig
{
	public const string ID = "FarmStationTools";

	public static readonly Tag tag = TagManager.Create("FarmStationTools");

	public const float MASS = 5f;

	public GameObject CreatePrefab()
	{
		string id = "FarmStationTools";
		string name = ITEMS.INDUSTRIAL_PRODUCTS.FARM_STATION_TOOLS.NAME;
		string desc = ITEMS.INDUSTRIAL_PRODUCTS.FARM_STATION_TOOLS.DESC;
		float mass = 5f;
		bool unitMass = true;
		KAnimFile anim = Assets.GetAnim("kit_planttender_kanim");
		string initialAnim = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.RECTANGLE;
		float width = 0.8f;
		float height = 0.6f;
		bool isPickupable = true;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.MiscPickupable);
		list = list;
		return EntityTemplates.CreateLooseEntity(id, name, desc, mass, unitMass, anim, initialAnim, sceneLayer, collisionShape, width, height, isPickupable, 0, SimHashes.Creature, list);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
