using UnityEngine;

public class PlaceTool : DragTool
{
	[SerializeField]
	private TextStyleSetting tooltipStyle;

	private Tag previewTag;

	private Placeable source;

	private ToolTip tooltip;

	public static PlaceTool Instance;

	private bool active = false;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		tooltip = GetComponent<ToolTip>();
	}

	protected override void OnActivateTool()
	{
		active = true;
		base.OnActivateTool();
		GameObject prefab = Assets.GetPrefab(previewTag);
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		int gameLayer = LayerMask.NameToLayer("Place");
		visualizer = GameUtil.KInstantiate(prefab, sceneLayer, null, gameLayer);
		KBatchedAnimController component = visualizer.GetComponent<KBatchedAnimController>();
		if ((Object)component != (Object)null)
		{
			component.visibilityType = KAnimControllerBase.VisibilityType.Always;
			component.isMovable = true;
		}
		visualizer.SetActive(true);
		ShowToolTip();
		BuildToolHoverTextCard component2 = GetComponent<BuildToolHoverTextCard>();
		component2.currentDef = null;
		ResourceRemainingDisplayScreen.instance.ActivateDisplay(visualizer);
		if ((Object)component == (Object)null)
		{
			visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
		}
		else
		{
			component.SetLayer(LayerMask.NameToLayer("Place"));
		}
		GridCompositor.Instance.ToggleMajor(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		active = false;
		GridCompositor.Instance.ToggleMajor(false);
		HideToolTip();
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
		Object.Destroy(visualizer);
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound(GetDeactivateSound(), false));
		base.OnDeactivateTool(new_tool);
	}

	public void Activate(Placeable source, Tag previewTag)
	{
		this.source = source;
		this.previewTag = previewTag;
		PlayerController.Instance.ActivateTool(this);
	}

	public void Deactivate()
	{
		SelectTool.Instance.Activate();
		source = null;
		previewTag = Tag.Invalid;
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (!((Object)visualizer == (Object)null))
		{
			bool flag = false;
			EntityPreview component = visualizer.GetComponent<EntityPreview>();
			if (component.Valid)
			{
				if (DebugHandler.InstantBuildMode)
				{
					source.Place(cell);
				}
				else
				{
					source.QueuePlacement(cell);
				}
				flag = true;
			}
			if (flag)
			{
				Deactivate();
			}
		}
	}

	protected override Mode GetMode()
	{
		return Mode.Brush;
	}

	private void ShowToolTip()
	{
		ToolTipScreen.Instance.SetToolTip(tooltip);
	}

	private void HideToolTip()
	{
		ToolTipScreen.Instance.ClearToolTip(tooltip);
	}

	public void Update()
	{
		if (active)
		{
			KBatchedAnimController component = visualizer.GetComponent<KBatchedAnimController>();
			if ((Object)component != (Object)null)
			{
				component.SetLayer(LayerMask.NameToLayer("Place"));
			}
		}
	}

	public override string GetDeactivateSound()
	{
		return "HUD_Click_Deselect";
	}
}
