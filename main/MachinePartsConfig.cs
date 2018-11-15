using STRINGS;
using UnityEngine;

public class MachinePartsConfig : IEntityConfig
{
	public const string ID = "MachineParts";

	public static readonly Tag TAG = TagManager.Create("MachineParts");

	public const float MASS = 5f;

	public GameObject CreatePrefab()
	{
		return EntityTemplates.CreateLooseEntity("MachineParts", ITEMS.INDUSTRIAL_PRODUCTS.MACHINE_PARTS.NAME, ITEMS.INDUSTRIAL_PRODUCTS.MACHINE_PARTS.DESC, 5f, true, Assets.GetAnim("buildingrelocate_kanim"), "idle", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, true, SimHashes.Creature, null);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
