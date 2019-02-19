using ProcGen;

internal class ZoneTile : KMonoBehaviour
{
	[MyCmpReq]
	public Building building;

	protected override void OnSpawn()
	{
		int[] placementCells = building.PlacementCells;
		foreach (int cell in placementCells)
		{
			SimMessages.ModifyCellWorldZone(cell, 0);
		}
	}

	protected override void OnCleanUp()
	{
		int[] placementCells = building.PlacementCells;
		foreach (int cell in placementCells)
		{
			SubWorld.ZoneType subWorldZoneType = World.Instance.zoneRenderData.GetSubWorldZoneType(cell);
			byte zone_id = (byte)((subWorldZoneType != SubWorld.ZoneType.Space) ? ((byte)subWorldZoneType) : 255);
			SimMessages.ModifyCellWorldZone(cell, zone_id);
		}
	}
}
