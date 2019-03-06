using System;
using UnityEngine;

public class RequiresFoundation : KGameObjectComponentManager<RequiresFoundation.Data>, IKComponentManager
{
	public struct Data
	{
		public int cell;

		public int width;

		public int height;

		public BuildLocationRule buildRule;

		public HandleVector<int>.Handle solidPartitionerEntry;

		public HandleVector<int>.Handle buildingPartitionerEntry;

		public bool solid;

		public GameObject go;
	}

	public static readonly Operational.Flag solidFoundation = new Operational.Flag("solid_foundation", Operational.Flag.Type.Functional);

	public HandleVector<int>.Handle Add(GameObject go)
	{
		BuildingDef def = go.GetComponent<Building>().Def;
		int cell = Grid.PosToCell(go.transform.GetPosition());
		Data data = default(Data);
		data.cell = cell;
		data.width = def.WidthInCells;
		data.height = def.HeightInCells;
		data.buildRule = def.BuildLocationRule;
		data.solid = true;
		data.go = go;
		Data data2 = data;
		HandleVector<int>.Handle h = Add(go, data2);
		if (def.ContinuouslyCheckFoundation)
		{
			Action<object> event_callback = delegate
			{
				OnSolidChanged(h);
			};
			Vector2I vector2I = Grid.CellToXY(cell);
			int xOffset = BuildingDef.GetXOffset(def.WidthInCells);
			data2.solidPartitionerEntry = GameScenePartitioner.Instance.Add("RequiresFoundation.Add", go, vector2I.x + xOffset, vector2I.y - 1, def.WidthInCells, def.HeightInCells + 1, GameScenePartitioner.Instance.solidChangedLayer, event_callback);
			data2.buildingPartitionerEntry = GameScenePartitioner.Instance.Add("RequiresFoundation.Add", go, vector2I.x + xOffset, vector2I.y - 1, def.WidthInCells, def.HeightInCells + 1, GameScenePartitioner.Instance.objectLayers[1], event_callback);
			SetData(h, data2);
			OnSolidChanged(h);
		}
		return h;
	}

	protected override void OnCleanUp(HandleVector<int>.Handle h)
	{
		Data data = GetData(h);
		GameScenePartitioner.Instance.Free(ref data.solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref data.buildingPartitionerEntry);
		SetData(h, data);
	}

	private void OnSolidChanged(HandleVector<int>.Handle h)
	{
		Data data = GetData(h);
		SimCellOccupier component = data.go.GetComponent<SimCellOccupier>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null || component.IsReady())
		{
			Rotatable component2 = data.go.GetComponent<Rotatable>();
			Orientation orientation = ((UnityEngine.Object)component2 != (UnityEngine.Object)null) ? component2.GetOrientation() : Orientation.Neutral;
			bool is_solid = BuildingDef.CheckFoundation(data.cell, orientation, data.buildRule, data.width, data.height);
			UpdateSolidState(is_solid, ref data);
			SetData(h, data);
		}
	}

	private void UpdateSolidState(bool is_solid, ref Data data)
	{
		if (data.solid != is_solid)
		{
			data.solid = is_solid;
			Operational component = data.go.GetComponent<Operational>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				component.SetFlag(solidFoundation, is_solid);
			}
			data.go.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.MissingFoundation, !is_solid, this);
		}
	}
}
