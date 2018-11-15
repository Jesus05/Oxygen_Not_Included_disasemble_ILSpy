using UnityEngine;

public class CopySettingsTool : DragTool
{
	public static CopySettingsTool Instance;

	public GameObject Placer;

	private GameObject sourceGameObject;

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

	public void SetSourceObject(GameObject sourceGameObject)
	{
		this.sourceGameObject = sourceGameObject;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (!((Object)sourceGameObject == (Object)null) && Grid.IsValidCell(cell))
		{
			CopyBuildingSettings.ApplyCopy(cell, sourceGameObject);
		}
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		sourceGameObject = null;
	}
}
