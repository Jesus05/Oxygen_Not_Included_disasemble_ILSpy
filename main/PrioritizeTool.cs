using UnityEngine;

public class PrioritizeTool : DragTool
{
	public GameObject Placer;

	public static PrioritizeTool Instance;

	public Texture2D[] cursors;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		interceptNumberKeysForPriority = true;
		Instance = this;
		visualizer = Util.KInstantiate(visualizer, null, null);
		viewMode = OverlayModes.Priorities.ID;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		PrioritySetting lastSelectedPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
		int num = 0;
		for (int i = 0; i < 39; i++)
		{
			GameObject gameObject = Grid.Objects[cell, i];
			if ((Object)gameObject != (Object)null)
			{
				Prioritizable component = gameObject.GetComponent<Prioritizable>();
				if ((Object)component != (Object)null && component.showIcon && component.IsPrioritizable())
				{
					component.SetMasterPriority(lastSelectedPriority);
					num++;
				}
			}
		}
		if (num > 0)
		{
			PriorityScreen.PlayPriorityConfirmSound(lastSelectedPriority);
		}
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenu.Instance.PriorityScreen.ShowDiagram(true);
		ToolMenu.Instance.PriorityScreen.Show(true);
		ToolMenu.Instance.PriorityScreen.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenu.Instance.PriorityScreen.Show(false);
		ToolMenu.Instance.PriorityScreen.ShowDiagram(false);
		ToolMenu.Instance.PriorityScreen.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public void Update()
	{
		PrioritySetting lastSelectedPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
		int num = 0;
		if (lastSelectedPriority.priority_class >= PriorityScreen.PriorityClass.high)
		{
			num += 9;
		}
		if (lastSelectedPriority.priority_class >= PriorityScreen.PriorityClass.emergency)
		{
			num = num;
		}
		num += lastSelectedPriority.priority_value;
		Texture2D mainTexture = cursors[num - 1];
		MeshRenderer componentInChildren = visualizer.GetComponentInChildren<MeshRenderer>();
		if ((Object)componentInChildren != (Object)null)
		{
			componentInChildren.material.mainTexture = mainTexture;
		}
	}
}
