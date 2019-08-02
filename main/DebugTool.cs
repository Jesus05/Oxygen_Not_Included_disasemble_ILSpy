using System.Collections.Generic;
using UnityEngine;

public class DebugTool : DragTool
{
	public enum Type
	{
		Dig,
		Heat,
		Cool,
		ReplaceSubstance,
		FillReplaceSubstance,
		AddPressure,
		RemovePressure,
		PaintPlant,
		Clear,
		AddSelection,
		RemoveSelection,
		Deconstruct,
		Destroy,
		Sample
	}

	public static DebugTool Instance;

	public Type type;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	public void Activate(Type type)
	{
		this.type = type;
		Activate();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		PlayerController.Instance.ToolDeactivated(this);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (Grid.IsValidCell(cell))
		{
			switch (type)
			{
			case Type.PaintPlant:
				break;
			case Type.Dig:
				SimMessages.Dig(cell, -1);
				break;
			case Type.Heat:
				SimMessages.ModifyEnergy(cell, 10000f, 10000f, SimMessages.EnergySourceID.DebugHeat);
				break;
			case Type.Cool:
				SimMessages.ModifyEnergy(cell, -10000f, 10000f, SimMessages.EnergySourceID.DebugCool);
				break;
			case Type.AddPressure:
				SimMessages.ModifyMass(cell, 10000f, byte.MaxValue, 0, CellEventLogger.Instance.DebugToolModifyMass, 293f, SimHashes.Oxygen);
				break;
			case Type.RemovePressure:
				SimMessages.ModifyMass(cell, -10000f, byte.MaxValue, 0, CellEventLogger.Instance.DebugToolModifyMass, 0f, SimHashes.Oxygen);
				break;
			case Type.ReplaceSubstance:
				DoReplaceSubstance(cell);
				break;
			case Type.FillReplaceSubstance:
			{
				GameUtil.FloodFillNext.Clear();
				GameUtil.FloodFillVisited.Clear();
				SimHashes elem_hash = Grid.Element[cell].id;
				GameUtil.FloodFillConditional(cell, delegate(int check_cell)
				{
					bool result = false;
					if (Grid.Element[check_cell].id == elem_hash)
					{
						result = true;
						DoReplaceSubstance(check_cell);
					}
					return result;
				}, GameUtil.FloodFillVisited, null);
				break;
			}
			case Type.Clear:
				ClearCell(cell);
				break;
			case Type.AddSelection:
				DebugBaseTemplateButton.Instance.AddToSelection(cell);
				break;
			case Type.RemoveSelection:
				DebugBaseTemplateButton.Instance.RemoveFromSelection(cell);
				break;
			case Type.Destroy:
				DestroyCell(cell);
				break;
			case Type.Deconstruct:
				DeconstructCell(cell);
				break;
			case Type.Sample:
				DebugPaintElementScreen.Instance.SampleCell(cell);
				break;
			}
		}
	}

	public void DoReplaceSubstance(int cell)
	{
		if (Grid.IsValidBuildingCell(cell))
		{
			Element element = (!DebugPaintElementScreen.Instance.paintElement.isOn) ? ElementLoader.elements[Grid.ElementIdx[cell]] : ElementLoader.FindElementByHash(DebugPaintElementScreen.Instance.element);
			if (element == null)
			{
				element = ElementLoader.FindElementByHash(SimHashes.Vacuum);
			}
			byte b = (!DebugPaintElementScreen.Instance.paintDisease.isOn) ? Grid.DiseaseIdx[cell] : DebugPaintElementScreen.Instance.diseaseIdx;
			float num = (!DebugPaintElementScreen.Instance.paintTemperature.isOn) ? Grid.Temperature[cell] : DebugPaintElementScreen.Instance.temperature;
			float num2 = (!DebugPaintElementScreen.Instance.paintMass.isOn) ? Grid.Mass[cell] : DebugPaintElementScreen.Instance.mass;
			int num3 = (!DebugPaintElementScreen.Instance.paintDiseaseCount.isOn) ? Grid.DiseaseCount[cell] : DebugPaintElementScreen.Instance.diseaseCount;
			if (num == -1f)
			{
				num = element.defaultValues.temperature;
			}
			if (num2 == -1f)
			{
				num2 = element.defaultValues.mass;
			}
			if (DebugPaintElementScreen.Instance.affectCells.isOn)
			{
				SimMessages.ReplaceElement(cell, element.id, CellEventLogger.Instance.DebugTool, num2, num, b, num3, -1);
				if (DebugPaintElementScreen.Instance.set_prevent_fow_reveal)
				{
					Grid.Visible[cell] = 0;
					Grid.PreventFogOfWarReveal[cell] = true;
				}
				else if (DebugPaintElementScreen.Instance.set_allow_fow_reveal && Grid.PreventFogOfWarReveal[cell])
				{
					Grid.PreventFogOfWarReveal[cell] = false;
				}
			}
			if (DebugPaintElementScreen.Instance.affectBuildings.isOn)
			{
				List<GameObject> list = new List<GameObject>();
				list.Add(Grid.Objects[cell, 1]);
				list.Add(Grid.Objects[cell, 2]);
				list.Add(Grid.Objects[cell, 9]);
				list.Add(Grid.Objects[cell, 16]);
				list.Add(Grid.Objects[cell, 12]);
				list.Add(Grid.Objects[cell, 16]);
				list.Add(Grid.Objects[cell, 26]);
				foreach (GameObject item in list)
				{
					if ((Object)item != (Object)null)
					{
						PrimaryElement component = item.GetComponent<PrimaryElement>();
						if (num > 0f)
						{
							component.Temperature = num;
						}
						if (num3 > 0 && b != 255)
						{
							component.ModifyDiseaseCount(-2147483648, "DebugTool.DoReplaceSubstance");
							component.AddDisease(b, num3, "DebugTool.DoReplaceSubstance");
						}
					}
				}
			}
		}
	}

	public void DeconstructCell(int cell)
	{
		bool instantBuildMode = DebugHandler.InstantBuildMode;
		DebugHandler.InstantBuildMode = true;
		DeconstructTool.Instance.DeconstructCell(cell);
		if (!instantBuildMode)
		{
			DebugHandler.InstantBuildMode = false;
		}
	}

	public void DestroyCell(int cell)
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(Grid.Objects[cell, 2]);
		list.Add(Grid.Objects[cell, 1]);
		list.Add(Grid.Objects[cell, 12]);
		list.Add(Grid.Objects[cell, 16]);
		list.Add(Grid.Objects[cell, 0]);
		list.Add(Grid.Objects[cell, 26]);
		foreach (GameObject item in list)
		{
			if ((Object)item != (Object)null)
			{
				Object.Destroy(item);
			}
		}
		ClearCell(cell);
		if (ElementLoader.elements[Grid.ElementIdx[cell]].id == SimHashes.Void)
		{
			SimMessages.ReplaceElement(cell, SimHashes.Void, CellEventLogger.Instance.DebugTool, 0f, 0f, byte.MaxValue, 0, -1);
		}
		else
		{
			SimMessages.ReplaceElement(cell, SimHashes.Vacuum, CellEventLogger.Instance.DebugTool, 0f, 0f, byte.MaxValue, 0, -1);
		}
	}

	public void ClearCell(int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		ListPool<ScenePartitionerEntry, DebugTool>.PooledList pooledList = ListPool<ScenePartitionerEntry, DebugTool>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(vector2I.x, vector2I.y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
		for (int i = 0; i < pooledList.Count; i++)
		{
			ScenePartitionerEntry scenePartitionerEntry = pooledList[i];
			Pickupable pickupable = scenePartitionerEntry.obj as Pickupable;
			if ((Object)pickupable != (Object)null && (Object)pickupable.GetComponent<MinionBrain>() == (Object)null)
			{
				Util.KDestroyGameObject(pickupable.gameObject);
			}
		}
		pooledList.Recycle();
	}
}
