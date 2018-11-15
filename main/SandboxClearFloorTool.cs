using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class SandboxClearFloorTool : BrushTool
{
	public static SandboxClearFloorTool instance;

	private SandboxSettings settings => SandboxToolParameterMenu.instance.settings;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		SandboxToolParameterMenu.instance.gameObject.SetActive(true);
		SandboxToolParameterMenu.instance.DisableParameters();
		SandboxToolParameterMenu.instance.brushRadiusSlider.SetValue(1f);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(false);
	}

	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		foreach (int item in cellsInRadius)
		{
			colors.Add(new ToolMenu.CellColorData(item, radiusIndicatorColor));
		}
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
	}

	protected override void OnPaintCell(int cell, int distFromOrigin)
	{
		base.OnPaintCell(cell, distFromOrigin);
		bool flag = false;
		foreach (Pickupable item in Components.Pickupables.Items)
		{
			if (!((Object)item.storage != (Object)null) && Grid.PosToCell(item) == cell && (Object)Components.LiveMinionIdentities.Items.Find((MinionIdentity match) => (Object)match.gameObject == (Object)item.gameObject) == (Object)null)
			{
				if (!flag)
				{
					UISounds.PlaySound(UISounds.Sound.Negative);
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.SANDBOXTOOLS.CLEARFLOOR.DELETED, item.transform, 1.5f, false);
					flag = true;
				}
				Util.KDestroyGameObject(item.gameObject);
			}
		}
	}
}
