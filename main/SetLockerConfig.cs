using STRINGS;
using TUNING;
using UnityEngine;

public class SetLockerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("SetLocker", STRINGS.BUILDINGS.PREFABS.SETLOCKER.NAME, STRINGS.BUILDINGS.PREFABS.SETLOCKER.DESC, 100f, Assets.GetAnim("setpiece_locker_kanim"), "on", Grid.SceneLayer.Building, 1, 2, TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER0, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		Workable workable = gameObject.AddOrGet<Workable>();
		workable.synchronizeAnims = false;
		workable.resetProgressOnStop = true;
		SetLocker setLocker = gameObject.AddOrGet<SetLocker>();
		setLocker.overrideAnim = "anim_interacts_clothingfactory_kanim";
		setLocker.dropOffset = new Vector2I(0, 1);
		setLocker.possible_contents_ids = new string[3]
		{
			"Warm_Vest",
			"Cool_Vest",
			"Funky_Vest"
		};
		gameObject.AddOrGet<LoreBearer>();
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
