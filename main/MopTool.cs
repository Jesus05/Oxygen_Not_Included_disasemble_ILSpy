using STRINGS;
using UnityEngine;

public class MopTool : DragTool
{
	private GameObject Placer;

	public static MopTool Instance;

	private SimHashes Element;

	public static float maxMopAmt = 150f;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Placer = Assets.GetPrefab(new Tag("MopPlacer"));
		interceptNumberKeysForPriority = true;
		Instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (Grid.IsValidCell(cell))
		{
			if (DebugHandler.InstantBuildMode)
			{
				if (Grid.IsValidCell(cell))
				{
					Moppable.MopCell(cell, 1000000f, null);
				}
			}
			else
			{
				GameObject gameObject = Grid.Objects[cell, 8];
				if (!Grid.Solid[cell] && (Object)gameObject == (Object)null && Grid.Element[cell].IsLiquid)
				{
					bool flag = Grid.Solid[Grid.CellBelow(cell)];
					bool flag2 = Grid.Mass[cell] <= maxMopAmt;
					if (flag && flag2)
					{
						gameObject = (Grid.Objects[cell, 8] = Util.KInstantiate(Placer, null, null));
						Vector3 position = Grid.CellToPosCBC(cell, visualizerLayer);
						float num = -0.15f;
						position.z += num;
						gameObject.transform.SetPosition(position);
						gameObject.SetActive(true);
					}
					else
					{
						string text = UI.TOOLS.MOP.TOO_MUCH_LIQUID;
						if (!flag)
						{
							text = UI.TOOLS.MOP.NOT_ON_FLOOR;
						}
						PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, text, null, Grid.CellToPosCBC(cell, visualizerLayer), 1.5f, false, false);
					}
				}
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
