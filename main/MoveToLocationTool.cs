using UnityEngine;

public class MoveToLocationTool : InterfaceTool
{
	public static MoveToLocationTool Instance;

	private Navigator targetNavigator;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		visualizer = Util.KInstantiate(visualizer, null, null);
	}

	public void Activate(Navigator navigator)
	{
		targetNavigator = navigator;
		PlayerController.Instance.ActivateTool(this);
	}

	public bool CanMoveTo(int target_cell)
	{
		return targetNavigator.CanReach(target_cell);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		visualizer.gameObject.SetActive(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		visualizer.gameObject.SetActive(false);
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		base.OnLeftClickDown(cursor_pos);
		if ((Object)targetNavigator != (Object)null)
		{
			int mouseCell = DebugHandler.GetMouseCell();
			MoveToLocationMonitor.Instance sMI = targetNavigator.GetSMI<MoveToLocationMonitor.Instance>();
			if (CanMoveTo(mouseCell) && sMI != null)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
				sMI.MoveToLocation(mouseCell);
				SelectTool.Instance.Activate();
			}
			else
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
			}
		}
	}

	private void RefreshColor()
	{
		Color c = new Color(0.91f, 0.21f, 0.2f);
		if (CanMoveTo(DebugHandler.GetMouseCell()))
		{
			c = Color.white;
		}
		SetColor(visualizer, c);
	}

	public override void OnMouseMove(Vector3 cursor_pos)
	{
		base.OnMouseMove(cursor_pos);
		RefreshColor();
	}

	private void SetColor(GameObject root, Color c)
	{
		root.GetComponentInChildren<MeshRenderer>().material.color = c;
	}
}
