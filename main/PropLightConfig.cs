using STRINGS;
using TUNING;
using UnityEngine;

public class PropLightConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PropLight", STRINGS.BUILDINGS.PREFABS.PROPLIGHT.NAME, STRINGS.BUILDINGS.PREFABS.PROPLIGHT.DESC, 50f, Assets.GetAnim("setpiece_light_kanim"), "off", Grid.SceneLayer.Building, 1, 1, TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER0, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Steel);
		component.Temperature = 294.15f;
		OccupyArea occupyArea = gameObject.AddOrGet<OccupyArea>();
		occupyArea.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
