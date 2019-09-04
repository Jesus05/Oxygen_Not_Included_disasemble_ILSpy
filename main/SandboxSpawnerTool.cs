using System.Collections.Generic;
using UnityEngine;

public class SandboxSpawnerTool : InterfaceTool
{
	protected Color radiusIndicatorColor = new Color(0.5f, 0.7f, 0.5f, 0.2f);

	private int currentCell;

	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		colors.Add(new ToolMenu.CellColorData(currentCell, radiusIndicatorColor));
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		currentCell = Grid.PosToCell(cursorPos);
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		Place(Grid.PosToCell(cursor_pos));
	}

	private void Place(int cell)
	{
		if (Grid.IsValidBuildingCell(cell))
		{
			if (SandboxToolParameterMenu.instance.settings.Entity.PrefabID() == (Tag)MinionConfig.ID)
			{
				SpawnMinion();
			}
			else if ((Object)SandboxToolParameterMenu.instance.settings.Entity.GetComponent<Building>() != (Object)null)
			{
				BuildingDef def = SandboxToolParameterMenu.instance.settings.Entity.GetComponent<Building>().Def;
				def.Build(cell, Orientation.Neutral, null, def.DefaultElements(), 298.15f, true, -1f);
			}
			else
			{
				GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(SandboxToolParameterMenu.instance.settings.Entity.PrefabTag), Grid.CellToPosCBC(currentCell, Grid.SceneLayer.Creatures), Grid.SceneLayer.Creatures, null, 0);
				gameObject.SetActive(true);
			}
			UISounds.PlaySound(UISounds.Sound.ClickObject);
		}
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		SandboxToolParameterMenu.instance.gameObject.SetActive(true);
		SandboxToolParameterMenu.instance.DisableParameters();
		SandboxToolParameterMenu.instance.entitySelector.row.SetActive(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(false);
	}

	private void SpawnMinion()
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), null, null);
		gameObject.name = Assets.GetPrefab(MinionConfig.ID).name;
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		Vector3 position = Grid.CellToPosCBC(currentCell, Grid.SceneLayer.Move);
		gameObject.transform.SetLocalPosition(position);
		gameObject.SetActive(true);
		MinionStartingStats minionStartingStats = new MinionStartingStats(false, null);
		minionStartingStats.Apply(gameObject);
	}
}
