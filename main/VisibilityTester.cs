using UnityEngine;

public class VisibilityTester : KMonoBehaviour
{
	public static VisibilityTester Instance;

	public bool enableTesting;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	private void Update()
	{
		if (!((Object)SelectTool.Instance == (Object)null) && !((Object)SelectTool.Instance.selected == (Object)null) && enableTesting)
		{
			int num = Grid.PosToCell(SelectTool.Instance.selected);
			int mouseCell = DebugHandler.GetMouseCell();
			string empty = string.Empty;
			string text = empty;
			empty = text + "Source Cell: " + num + "\n";
			text = empty;
			empty = text + "Target Cell: " + mouseCell + "\n";
			empty = empty + "Visible: " + Grid.VisibilityTest(num, mouseCell, false);
			for (int i = 0; i < 10000; i++)
			{
				Grid.VisibilityTest(num, mouseCell, false);
			}
			DebugText.Instance.Draw(empty, Grid.CellToPosCCC(mouseCell, Grid.SceneLayer.Move), Color.white);
		}
	}
}
