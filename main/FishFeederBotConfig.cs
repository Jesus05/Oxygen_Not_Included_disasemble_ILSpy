using UnityEngine;

public class FishFeederBotConfig : IEntityConfig
{
	public const string ID = "FishFeederBot";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity("FishFeederBot", "FishFeederBot", true);
		KBatchedAnimController kBatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.AnimFiles = new KAnimFile[1]
		{
			Assets.GetAnim("fishfeeder_kanim")
		};
		kBatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
		SymbolOverrideControllerUtil.AddToPrefab(kBatchedAnimController.gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
