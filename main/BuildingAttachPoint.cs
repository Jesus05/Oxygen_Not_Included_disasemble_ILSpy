using System;
using UnityEngine;

public class BuildingAttachPoint : KMonoBehaviour
{
	[Serializable]
	public struct HardPoint
	{
		public CellOffset position;

		public Tag attachableType;

		public AttachableBuilding attachedBuilding;

		public HardPoint(CellOffset position, Tag attachableType, AttachableBuilding attachedBuilding)
		{
			this.position = position;
			this.attachableType = attachableType;
			this.attachedBuilding = attachedBuilding;
		}
	}

	public HardPoint[] points = new HardPoint[0];

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.BuildingAttachPoints.Add(this);
		TryAttachEmptyHardpoints();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	private void TryAttachEmptyHardpoints()
	{
		for (int i = 0; i < points.Length; i++)
		{
			if (!((UnityEngine.Object)points[i].attachedBuilding != (UnityEngine.Object)null))
			{
				bool flag = false;
				for (int j = 0; j < Components.AttachableBuildings.Count; j++)
				{
					if (flag)
					{
						break;
					}
					if (Components.AttachableBuildings[j].attachableToTag == points[i].attachableType && Grid.OffsetCell(Grid.PosToCell(base.gameObject), points[i].position) == Grid.PosToCell(Components.AttachableBuildings[j]))
					{
						points[i].attachedBuilding = Components.AttachableBuildings[j];
						flag = true;
					}
				}
			}
		}
	}

	public bool AcceptsAttachment(Tag type, int cell)
	{
		int cell2 = Grid.PosToCell(base.gameObject);
		for (int i = 0; i < points.Length; i++)
		{
			if (Grid.OffsetCell(cell2, points[i].position) == cell && points[i].attachableType == type)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.BuildingAttachPoints.Remove(this);
	}
}
