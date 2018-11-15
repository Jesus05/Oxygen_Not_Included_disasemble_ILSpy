using ProcGen;

internal class ZoneTile : KMonoBehaviour
{
	public int width = 1;

	public int height = 1;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int cell = Grid.PosToCell(this);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				int cell2 = Grid.OffsetCell(cell, i, j);
				SimMessages.ModifyCellWorldZone(cell2, 0);
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		int cell = Grid.PosToCell(this);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				int cell2 = Grid.OffsetCell(cell, i, j);
				SubWorld.ZoneType subWorldZoneType = World.Instance.zoneRenderData.GetSubWorldZoneType(cell2);
				byte zone_id = (byte)((subWorldZoneType != SubWorld.ZoneType.Space) ? ((byte)subWorldZoneType) : 255);
				SimMessages.ModifyCellWorldZone(cell2, zone_id);
			}
		}
	}
}
