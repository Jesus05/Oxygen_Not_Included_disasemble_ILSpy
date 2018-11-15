using UnityEngine;

public class AntiCluster : KMonoBehaviour, ISim200ms
{
	private int previousCell = Grid.InvalidCell;

	public void Sim200ms(float dt)
	{
		int num = Grid.PosToCell(this);
		UpdateCell(previousCell, num);
		if (previousCell != Grid.InvalidCell)
		{
			UpdateCell(Grid.CellAbove(previousCell), Grid.CellAbove(num));
		}
		else
		{
			UpdateCell(previousCell, Grid.CellAbove(num));
		}
		previousCell = num;
	}

	private void UpdateCell(int previous_cell, int current_cell)
	{
		if (previous_cell != Grid.InvalidCell && previous_cell != current_cell && (Object)Grid.Objects[previous_cell, 0] == (Object)base.gameObject)
		{
			Grid.Objects[previous_cell, 0] = null;
		}
		GameObject gameObject = Grid.Objects[current_cell, 0];
		if ((Object)gameObject == (Object)null)
		{
			Grid.Objects[current_cell, 0] = base.gameObject;
		}
		else
		{
			KPrefabID component = GetComponent<KPrefabID>();
			KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
			if (component.InstanceID > component2.InstanceID)
			{
				Grid.Objects[current_cell, 0] = base.gameObject;
			}
		}
	}
}
