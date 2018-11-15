using STRINGS;
using TUNING;
using UnityEngine;

public class PropFacilityDeskConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PropFacilityDesk", STRINGS.BUILDINGS.PREFABS.PROPFACILITYDESK.NAME, STRINGS.BUILDINGS.PREFABS.PROPFACILITYDESK.DESC, 50f, Assets.GetAnim("gravitas_desk_kanim"), "off", Grid.SceneLayer.Building, 4, 2, TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER0, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Granite);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<LoreBearer>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		OccupyArea component = inst.GetComponent<OccupyArea>();
		component.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		int cell = Grid.PosToCell(inst);
		CellOffset[] occupiedCellsOffsets = component.OccupiedCellsOffsets;
		foreach (CellOffset offset in occupiedCellsOffsets)
		{
			Grid.GravitasFacility[Grid.OffsetCell(cell, offset)] = true;
		}
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
