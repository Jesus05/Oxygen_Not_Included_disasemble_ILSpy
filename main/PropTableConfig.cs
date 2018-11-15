using STRINGS;
using TUNING;
using UnityEngine;

public class PropTableConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PropTable", STRINGS.BUILDINGS.PREFABS.PROPTABLE.NAME, STRINGS.BUILDINGS.PREFABS.PROPTABLE.DESC, 50f, Assets.GetAnim("table_breakroom_kanim"), "off", Grid.SceneLayer.Building, 3, 1, TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER0, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
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
