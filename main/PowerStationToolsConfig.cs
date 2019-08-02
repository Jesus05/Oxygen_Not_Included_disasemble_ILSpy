using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class PowerStationToolsConfig : IEntityConfig
{
	public const string ID = "PowerStationTools";

	public static readonly Tag tag = TagManager.Create("PowerStationTools");

	public const float MASS = 5f;

	public GameObject CreatePrefab()
	{
		string id = "PowerStationTools";
		string name = ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.NAME;
		string desc = ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.DESC;
		float mass = 5f;
		bool unitMass = true;
		KAnimFile anim = Assets.GetAnim("kit_electrician_kanim");
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
