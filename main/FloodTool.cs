using System;
using System.Collections.Generic;
using UnityEngine;

public class FloodTool : InterfaceTool
{
	public Func<int, bool> floodCriteria;

	public Action<HashSet<int>> paintArea;

	protected Color32 areaColour = new Color(0.5f, 0.7f, 0.5f, 0.2f);

	protected int mouseCell = -1;

	public HashSet<int> Flood(int startCell)
	{
		HashSet<int> visited_cells = new HashSet<int>();
		HashSet<int> hashSet = new HashSet<int>();
		GameUtil.FloodFillConditional(startCell, floodCriteria, visited_cells, hashSet);
		return hashSet;
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		base.OnLeftClickDown(cursor_pos);
		paintArea(Flood(Grid.PosToCell(cursor_pos)));
	}

	public override void OnMouseMove(Vector3 cursor_pos)
	{
		base.OnMouseMove(cursor_pos);
		mouseCell = Grid.PosToCell(cursor_pos);
	}
}
