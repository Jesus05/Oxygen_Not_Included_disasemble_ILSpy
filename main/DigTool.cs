using UnityEngine;

public class DigTool : DragTool
{
	public static DigTool Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (!Grid.Solid[cell])
		{
			foreach (Uprootable item in Components.Uprootables.Items)
			{
				if (Grid.PosToCell(item.gameObject) == cell)
				{
					item.MarkForUproot();
					break;
				}
				OccupyArea area = item.area;
				if ((Object)area != (Object)null && area.CheckIsOccupying(cell))
				{
					item.MarkForUproot();
				}
			}
		}
		if (DebugHandler.InstantBuildMode)
		{
			if (Grid.IsValidCell(cell) && Grid.Solid[cell] && !Grid.Foundation[cell])
			{
				WorldDamage.Instance.DestroyCell(cell, -1);
			}
		}
		else
		{
			GameObject gameObject = PlaceDig(cell, distFromOrigin);
			if ((Object)gameObject != (Object)null)
			{
				Prioritizable component = gameObject.GetComponent<Prioritizable>();
				if ((Object)component != (Object)null)
				{
					component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
				}
			}
		}
	}

	public static GameObject PlaceDig(int cell, int animationDelay = 0)
	{
		if (Grid.Solid[cell] && !Grid.Foundation[cell] && (Object)Grid.Objects[cell, 7] == (Object)null)
		{
			for (int i = 0; i < 37; i++)
			{
				if ((Object)Grid.Objects[cell, i] != (Object)null && (Object)Grid.Objects[cell, i].GetComponent<Constructable>() != (Object)null)
				{
					return null;
				}
			}
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(new Tag("DigPlacer")), null, null);
			gameObject.SetActive(true);
			Grid.Objects[cell, 7] = gameObject;
			Vector3 position = Grid.CellToPosCBC(cell, Instance.visualizerLayer);
			float num = -0.15f;
			position.z += num;
			gameObject.transform.SetPosition(position);
			gameObject.GetComponentInChildren<EasingAnimations>().PlayAnimation("ScaleUp", Mathf.Max(0f, (float)animationDelay * 0.02f));
			return gameObject;
		}
		if (!((Object)Grid.Objects[cell, 7] != (Object)null))
		{
			return null;
		}
		return Grid.Objects[cell, 7];
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenu.Instance.PriorityScreen.Show(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenu.Instance.PriorityScreen.Show(false);
	}
}
