using STRINGS;
using System;
using UnityEngine;

public class DigPlacerConfig : CommonPlacerConfig, IEntityConfig
{
	[Serializable]
	public class DigPlacerAssets
	{
		public Material[] materials;
	}

	public static string ID = "DigPlacer";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = CreatePrefab(ID, MISC.PLACERS.DIGPLACER.NAME, Assets.instance.digPlacerAssets.materials[0]);
		Diggable diggable = gameObject.AddOrGet<Diggable>();
		diggable.workTime = 5f;
		diggable.synchronizeAnims = false;
		diggable.workAnims = new HashedString[2]
		{
			"place",
			"release"
		};
		diggable.materials = Assets.instance.digPlacerAssets.materials;
		diggable.materialDisplay = gameObject.GetComponentInChildren<MeshRenderer>(true);
		gameObject.AddOrGet<CancellableDig>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}
}
