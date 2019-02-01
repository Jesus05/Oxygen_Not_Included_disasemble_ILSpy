using UnityEngine;

public class NavPathDrawer : KMonoBehaviour
{
	private PathFinder.Path path;

	public Material material;

	private Vector3 navigatorPos;

	private Navigator navigator;

	public static NavPathDrawer Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Shader shader = Shader.Find("Lines/Colored Blended");
		material = new Material(shader);
		Instance = this;
	}

	protected override void OnCleanUp()
	{
		Instance = null;
	}

	public void DrawPath(Vector3 navigator_pos, PathFinder.Path path)
	{
		navigatorPos = navigator_pos;
		navigatorPos.y += 0.5f;
		this.path = path;
	}

	public Navigator GetNavigator()
	{
		return navigator;
	}

	public void SetNavigator(Navigator navigator)
	{
		this.navigator = navigator;
	}

	public void ClearNavigator()
	{
		navigator = null;
	}

	private void DrawPath(PathFinder.Path path, Vector3 navigator_pos, Color color)
	{
		if (path.nodes != null && path.nodes.Count > 1)
		{
			GL.PushMatrix();
			material.SetPass(0);
			GL.Begin(1);
			GL.Color(color);
			GL.Vertex(navigator_pos);
			PathFinder.Path.Node node = path.nodes[1];
			int cell = node.cell;
			PathFinder.Path.Node node2 = path.nodes[1];
			GL.Vertex(NavTypeHelper.GetNavPos(cell, node2.navType));
			for (int i = 1; i < path.nodes.Count - 1; i++)
			{
				PathFinder.Path.Node node3 = path.nodes[i];
				int cell2 = node3.cell;
				PathFinder.Path.Node node4 = path.nodes[i];
				Vector3 navPos = NavTypeHelper.GetNavPos(cell2, node4.navType);
				PathFinder.Path.Node node5 = path.nodes[i + 1];
				int cell3 = node5.cell;
				PathFinder.Path.Node node6 = path.nodes[i + 1];
				Vector3 navPos2 = NavTypeHelper.GetNavPos(cell3, node6.navType);
				GL.Vertex(navPos);
				GL.Vertex(navPos2);
			}
			GL.End();
			GL.PopMatrix();
		}
	}

	private void OnPostRender()
	{
		DrawPath(path, navigatorPos, Color.white);
		path = default(PathFinder.Path);
		DebugDrawSelectedNavigator();
		if ((Object)navigator != (Object)null)
		{
			GL.PushMatrix();
			material.SetPass(0);
			GL.Begin(1);
			PathFinderQuery query = PathFinderQueries.drawNavGridQuery.Reset(null);
			navigator.RunQuery(query);
			GL.End();
			GL.PopMatrix();
		}
	}

	private void DebugDrawSelectedNavigator()
	{
		if (DebugHandler.DebugPathFinding && !((Object)SelectTool.Instance == (Object)null) && !((Object)SelectTool.Instance.selected == (Object)null))
		{
			Navigator component = SelectTool.Instance.selected.GetComponent<Navigator>();
			if (!((Object)component == (Object)null))
			{
				int mouseCell = DebugHandler.GetMouseCell();
				if (Grid.IsValidCell(mouseCell))
				{
					PathFinder.PotentialPath potential_path = new PathFinder.PotentialPath(Grid.PosToCell(component), component.CurrentNavType, component.flags);
					PathFinder.Path path = default(PathFinder.Path);
					PathFinder.UpdatePath(component.NavGrid, component.GetCurrentAbilities(), potential_path, PathFinderQueries.cellQuery.Reset(mouseCell), ref path);
					string text = "";
					string text2 = text;
					text = text2 + "Source: " + Grid.PosToCell(component) + "\n";
					text2 = text;
					text = text2 + "Dest: " + mouseCell + "\n";
					text = text + "Cost: " + path.cost;
					DrawPath(path, component.GetComponent<KAnimControllerBase>().GetPivotSymbolPosition(), Color.green);
					DebugText.Instance.Draw(text, Grid.CellToPosCCC(mouseCell, Grid.SceneLayer.Move), Color.white);
				}
			}
		}
	}
}
