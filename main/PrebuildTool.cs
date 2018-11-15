using UnityEngine;

public class PrebuildTool : InterfaceTool
{
	public static PrebuildTool Instance;

	private BuildingDef def;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	protected override void OnActivateTool()
	{
		viewMode = def.ViewMode;
		base.OnActivateTool();
	}

	public void Activate(BuildingDef def, PlanScreen.RequirementsState reqState)
	{
		this.def = def;
		PlayerController.Instance.ActivateTool(this);
		PrebuildToolHoverTextCard component = GetComponent<PrebuildToolHoverTextCard>();
		component.currentReqState = reqState;
		component.currentDef = def;
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		UISounds.PlaySound(UISounds.Sound.Negative);
		base.OnLeftClickDown(cursor_pos);
	}
}
