using STRINGS;
using TUNING;
using UnityEngine;

public class PropTallPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PropTallPlant", STRINGS.BUILDINGS.PREFABS.PROPFACILITYTALLPLANT.NAME, STRINGS.BUILDINGS.PREFABS.PROPFACILITYTALLPLANT.DESC, 50f, Assets.GetAnim("gravitas_tall_plant_kanim"), "off", Grid.SceneLayer.Building, 1, 3, TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER0, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Polypropylene);
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
