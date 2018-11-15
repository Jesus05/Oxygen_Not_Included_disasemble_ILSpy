using UnityEngine;

public class RangedAttackable : AttackableBase
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.preferUnreservedCell = true;
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	public new int GetCell()
	{
		return Grid.PosToCell(this);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 0.5f, 0.5f, 0.15f);
		CellOffset[] offsets = GetOffsets();
		foreach (CellOffset offset in offsets)
		{
			Gizmos.DrawCube(new Vector3(0.5f, 0.5f, 0f) + Grid.CellToPos(Grid.OffsetCell(Grid.PosToCell(base.gameObject), offset)), Vector3.one);
		}
	}
}
