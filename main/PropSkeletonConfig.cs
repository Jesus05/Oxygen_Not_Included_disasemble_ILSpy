using STRINGS;
using TUNING;
using UnityEngine;

public class PropSkeletonConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PropSkeleton", STRINGS.BUILDINGS.PREFABS.PROPSKELETON.NAME, STRINGS.BUILDINGS.PREFABS.PROPSKELETON.DESC, 50f, Assets.GetAnim("skeleton_poi_kanim"), "off", Grid.SceneLayer.Building, 1, 2, TUNING.BUILDINGS.DECOR.BONUS.TIER5, NOISE_POLLUTION.NOISY.TIER0, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Creature);
		component.Temperature = 294.15f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		OccupyArea component = inst.GetComponent<OccupyArea>();
		component.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
