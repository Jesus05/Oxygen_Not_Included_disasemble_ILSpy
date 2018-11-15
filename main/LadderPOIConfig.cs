using STRINGS;
using TUNING;
using UnityEngine;

public class LadderPOIConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		int num = 1;
		int height = 1;
		string id = "PropLadder";
		string name = STRINGS.BUILDINGS.PREFABS.PROPLADDER.NAME;
		string desc = STRINGS.BUILDINGS.PREFABS.PROPLADDER.DESC;
		float mass = 50f;
		int width = num;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, Assets.GetAnim("ladder_poi_kanim"), "off", Grid.SceneLayer.Building, width, height, TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER0, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Polypropylene);
		component.Temperature = 294.15f;
		Ladder ladder = gameObject.AddOrGet<Ladder>();
		ladder.upwardsMovementSpeedMultiplier = 1.5f;
		ladder.downwardsMovementSpeedMultiplier = 1.5f;
		gameObject.AddOrGet<AnimTileable>();
		OccupyArea obj = gameObject.AddOrGet<OccupyArea>();
		Object.DestroyImmediate(obj);
		obj = gameObject.AddOrGet<OccupyArea>();
		obj.OccupiedCellsOffsets = EntityTemplates.GenerateOffsets(num, height);
		obj.objectLayers = new ObjectLayer[1]
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
