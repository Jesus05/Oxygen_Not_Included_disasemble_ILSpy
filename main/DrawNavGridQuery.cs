using UnityEngine;

public class DrawNavGridQuery : PathFinderQuery
{
	public DrawNavGridQuery Reset(MinionBrain brain)
	{
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (parent_cell != Grid.InvalidCell)
		{
			Color white = Color.white;
			GL.Color(white);
			GL.Vertex(Grid.CellToPosCCC(parent_cell, Grid.SceneLayer.Move));
			GL.Vertex(Grid.CellToPosCCC(cell, Grid.SceneLayer.Move));
			return false;
		}
		return false;
	}
}
