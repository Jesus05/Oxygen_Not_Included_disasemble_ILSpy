using System.Collections.Generic;
using TemplateClasses;
using UnityEngine;

public class StampTool : InterfaceTool
{
	public static StampTool Instance;

	public TemplateContainer stampTemplate;

	public GameObject PlacerPrefab;

	private bool ready = true;

	private int placementCell = Grid.InvalidCell;

	private bool selectAffected = false;

	private bool deactivateOnStamp = false;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	public void Activate(TemplateContainer template, bool SelectAffected = false, bool DeactivateOnStamp = false)
	{
		stampTemplate = template;
		PlayerController.Instance.ActivateTool(this);
		selectAffected = SelectAffected;
		deactivateOnStamp = DeactivateOnStamp;
	}

	private void Update()
	{
		RefreshPreview(Grid.PosToCell(GetCursorPos()));
	}

	private Vector3 GetCursorPos()
	{
		return PlayerController.GetCursorPos(KInputManager.GetMousePos());
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		base.OnLeftClickDown(cursor_pos);
		Stamp(cursor_pos);
	}

	private void Stamp(Vector2 pos)
	{
		if (ready)
		{
			int cell = Grid.OffsetCell(Grid.PosToCell(pos), Mathf.FloorToInt((0f - stampTemplate.info.size.X) / 2f), 0);
			int cell2 = Grid.OffsetCell(Grid.PosToCell(pos), Mathf.FloorToInt(stampTemplate.info.size.X / 2f), 0);
			int cell3 = Grid.OffsetCell(Grid.PosToCell(pos), 0, 1 + Mathf.FloorToInt((0f - stampTemplate.info.size.Y) / 2f));
			int cell4 = Grid.OffsetCell(Grid.PosToCell(pos), 0, 1 + Mathf.FloorToInt(stampTemplate.info.size.Y / 2f));
			if (Grid.IsValidBuildingCell(cell) && Grid.IsValidBuildingCell(cell2) && Grid.IsValidBuildingCell(cell4) && Grid.IsValidBuildingCell(cell3))
			{
				ready = false;
				bool pauseOnComplete = SpeedControlScreen.Instance.IsPaused;
				if (SpeedControlScreen.Instance.IsPaused)
				{
					SpeedControlScreen.Instance.Unpause(true);
				}
				List<GameObject> objects_to_destroy = new List<GameObject>();
				for (int i = 0; i < stampTemplate.cells.Count; i++)
				{
					for (int j = 0; j < 34; j++)
					{
						GameObject gameObject = Grid.Objects[Grid.XYToCell((int)(pos.x + (float)stampTemplate.cells[i].location_x), (int)(pos.y + (float)stampTemplate.cells[i].location_y)), j];
						if ((Object)gameObject != (Object)null && !objects_to_destroy.Contains(gameObject))
						{
							objects_to_destroy.Add(gameObject);
						}
					}
				}
				TemplateLoader.Stamp(stampTemplate, pos, delegate
				{
					CompleteStamp(pauseOnComplete, objects_to_destroy);
				});
				if (selectAffected)
				{
					DebugBaseTemplateButton.Instance.ClearSelection();
					for (int k = 0; k < stampTemplate.cells.Count; k++)
					{
						DebugBaseTemplateButton.Instance.AddToSelection(Grid.XYToCell((int)(pos.x + (float)stampTemplate.cells[k].location_x), (int)(pos.y + (float)stampTemplate.cells[k].location_y)));
					}
				}
				if (deactivateOnStamp)
				{
					DeactivateTool(null);
				}
			}
		}
	}

	private void CompleteStamp(bool pause, List<GameObject> objects_to_destroy = null)
	{
		if (objects_to_destroy != null)
		{
			foreach (GameObject item in objects_to_destroy)
			{
				if ((Object)item != (Object)null)
				{
					Util.KDestroyGameObject(item);
				}
			}
		}
		if (pause)
		{
			SpeedControlScreen.Instance.Pause(true);
		}
		ready = true;
		OnDeactivateTool(null);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		RefreshPreview(Grid.InvalidCell);
	}

	public void RefreshPreview(int new_placement_cell)
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		foreach (Cell cell in stampTemplate.cells)
		{
			if (placementCell != Grid.InvalidCell)
			{
				int num = Grid.OffsetCell(placementCell, new CellOffset(cell.location_x, cell.location_y));
				if (Grid.IsValidCell(num))
				{
					list.Add(num);
				}
			}
			if (new_placement_cell != Grid.InvalidCell)
			{
				int num2 = Grid.OffsetCell(new_placement_cell, new CellOffset(cell.location_x, cell.location_y));
				if (Grid.IsValidCell(num2))
				{
					list2.Add(num2);
				}
			}
		}
		placementCell = new_placement_cell;
		foreach (int item in list)
		{
			if (!list2.Contains(item))
			{
				GameObject gameObject = Grid.Objects[item, 6];
				if ((Object)gameObject != (Object)null)
				{
					gameObject.DeleteObject();
				}
			}
		}
		foreach (int item2 in list2)
		{
			if (!list.Contains(item2) && (Object)Grid.Objects[item2, 6] == (Object)null)
			{
				GameObject gameObject3 = Grid.Objects[item2, 6] = Util.KInstantiate(PlacerPrefab, null, null);
				Vector3 position = Grid.CellToPosCBC(item2, visualizerLayer);
				float num3 = -0.15f;
				position.z += num3;
				gameObject3.transform.SetPosition(position);
			}
		}
	}
}
