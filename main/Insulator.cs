using UnityEngine;

[SkipSaveFileSerialization]
public class Insulator : KMonoBehaviour
{
	[MyCmpReq]
	private Building building;

	[SerializeField]
	public CellOffset offset = CellOffset.none;

	protected override void OnSpawn()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		cell = Grid.OffsetCell(cell, offset);
		SimMessages.SetInsulation(cell, building.Def.ThermalConductivity);
	}

	protected override void OnCleanUp()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		cell = Grid.OffsetCell(cell, offset);
		SimMessages.SetInsulation(cell, 1f);
	}
}
