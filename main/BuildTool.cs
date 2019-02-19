using FMOD.Studio;
using Rendering;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class BuildTool : DragTool
{
	[SerializeField]
	private TextStyleSetting tooltipStyle;

	private int lastCell = -1;

	private int lastDragCell = -1;

	private IList<Tag> selectedElements;

	private BuildingDef def;

	private Orientation buildingOrientation;

	private GameObject source;

	private ToolTip tooltip;

	public static BuildTool Instance;

	private bool active;

	private int buildingCount;

	public int GetLastCell => lastCell;

	public Orientation GetBuildingOrientation => buildingOrientation;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		tooltip = GetComponent<ToolTip>();
		buildingCount = Random.Range(1, 14);
		canChangeDragAxis = false;
	}

	protected override void OnActivateTool()
	{
		lastDragCell = -1;
		if ((Object)visualizer != (Object)null)
		{
			ClearTilePreview();
			Object.Destroy(visualizer);
		}
		active = true;
		base.OnActivateTool();
		buildingOrientation = Orientation.Neutral;
		placementPivot = def.placementPivot;
		Vector3 cursorPos = PlayerController.GetCursorPos(KInputManager.GetMousePos());
		GameObject buildingPreview = def.BuildingPreview;
		Vector3 position = cursorPos;
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Ore;
		int gameLayer = LayerMask.NameToLayer("Place");
		visualizer = GameUtil.KInstantiate(buildingPreview, position, sceneLayer, null, gameLayer);
		KBatchedAnimController component = visualizer.GetComponent<KBatchedAnimController>();
		if ((Object)component != (Object)null)
		{
			component.visibilityType = KAnimControllerBase.VisibilityType.Always;
			component.isMovable = true;
			component.Offset = def.GetVisualizerOffset();
			component.Offset += def.placementPivot;
			component.name = component.GetComponent<KPrefabID>().GetDebugName() + "_visualizer";
		}
		visualizer.SetActive(true);
		UpdateVis(cursorPos);
		BuildToolHoverTextCard component2 = GetComponent<BuildToolHoverTextCard>();
		component2.currentDef = def;
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
		lastDragCell = -1;
		if (active)
		{
			active = false;
			GridCompositor.Instance.ToggleMajor(false);
			buildingOrientation = Orientation.Neutral;
			HideToolTip();
			ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
			ClearTilePreview();
			Object.Destroy(visualizer);
			if ((Object)new_tool == (Object)SelectTool.Instance)
			{
				Game.Instance.Trigger(-1190690038, null);
			}
			base.OnDeactivateTool(new_tool);
		}
	}

	public void Activate(BuildingDef def, IList<Tag> selected_elements, GameObject source = null)
	{
		selectedElements = selected_elements;
		this.def = def;
		this.source = source;
		viewMode = def.ViewMode;
		ResourceRemainingDisplayScreen.instance.SetResources(selected_elements, def.CraftRecipe);
		PlayerController.Instance.ActivateTool(this);
		OnActivateTool();
	}

	public void Deactivate()
	{
		selectedElements = null;
		SelectTool.Instance.Activate();
		def = null;
		source = null;
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
	}

	private void ClearTilePreview()
	{
		if (Grid.IsValidBuildingCell(lastCell) && def.IsTilePiece)
		{
			GameObject gameObject = Grid.Objects[lastCell, (int)def.TileLayer];
			if ((Object)visualizer == (Object)gameObject)
			{
				Grid.Objects[lastCell, (int)def.TileLayer] = null;
			}
			if (def.isKAnimTile)
			{
				GameObject x = null;
				if (def.ReplacementLayer != ObjectLayer.NumLayers)
				{
					x = Grid.Objects[lastCell, (int)def.ReplacementLayer];
				}
				if (((Object)gameObject == (Object)null || (Object)gameObject.GetComponent<Constructable>() == (Object)null) && ((Object)x == (Object)null || (Object)x == (Object)visualizer))
				{
					World.Instance.blockTileRenderer.RemoveBlock(def, SimHashes.Void, lastCell);
					TileVisualizer.RefreshCell(lastCell, def.TileLayer, def.ReplacementLayer);
				}
			}
		}
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		cursorPos -= placementPivot;
		base.OnMouseMove(cursorPos);
		UpdateVis(cursorPos);
	}

	private void UpdateVis(Vector3 pos)
	{
		bool flag = def.IsValidPlaceLocation(visualizer, pos, buildingOrientation, out string _);
		bool flag2 = def.IsValidReplaceLocation(pos, buildingOrientation, def.ReplacementLayer, def.ObjectLayer);
		flag = (flag || flag2);
		if ((Object)visualizer != (Object)null)
		{
			Color c = Color.white;
			float strength = 0f;
			if (!flag)
			{
				c = Color.red;
				strength = 1f;
			}
			SetColor(visualizer, c, strength);
		}
		int num = Grid.PosToCell(pos);
		if ((Object)def != (Object)null)
		{
			Vector3 vector = Grid.CellToPosCBC(num, def.SceneLayer);
			visualizer.transform.SetPosition(vector);
			base.transform.SetPosition(vector - Vector3.up * 0.5f);
			if (def.IsTilePiece)
			{
				ClearTilePreview();
				if (Grid.IsValidBuildingCell(num))
				{
					GameObject gameObject = Grid.Objects[num, (int)def.TileLayer];
					if ((Object)gameObject == (Object)null)
					{
						Grid.Objects[num, (int)def.TileLayer] = visualizer;
					}
					if (def.isKAnimTile)
					{
						GameObject x = null;
						if (def.ReplacementLayer != ObjectLayer.NumLayers)
						{
							x = Grid.Objects[num, (int)def.ReplacementLayer];
						}
						if ((Object)gameObject == (Object)null || ((Object)gameObject.GetComponent<Constructable>() == (Object)null && (Object)x == (Object)null))
						{
							TileVisualizer.RefreshCell(num, def.TileLayer, def.ReplacementLayer);
							if ((Object)def.BlockTileAtlas != (Object)null)
							{
								int renderLayer = LayerMask.NameToLayer("Overlay");
								BlockTileRenderer blockTileRenderer = World.Instance.blockTileRenderer;
								blockTileRenderer.SetInvalidPlaceCell(num, !flag);
								if (lastCell != num)
								{
									blockTileRenderer.SetInvalidPlaceCell(lastCell, false);
								}
								blockTileRenderer.AddBlock(renderLayer, def, SimHashes.Void, num);
							}
						}
					}
				}
			}
			if (lastCell != num)
			{
				lastCell = num;
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.RotateBuilding))
		{
			if ((Object)visualizer != (Object)null)
			{
				Rotatable component = visualizer.GetComponent<Rotatable>();
				if ((Object)component != (Object)null)
				{
					KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Rotate", false));
					buildingOrientation = component.Rotate();
					if (Grid.IsValidBuildingCell(lastCell))
					{
						Vector3 pos = Grid.CellToPosCCC(lastCell, Grid.SceneLayer.Building);
						UpdateVis(pos);
					}
				}
			}
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (!((Object)visualizer == (Object)null) && cell != lastDragCell)
		{
			int num = Grid.PosToCell(visualizer);
			if (num == cell)
			{
				lastDragCell = cell;
				ClearTilePreview();
				Vector3 vector = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
				GameObject gameObject = null;
				if (DebugHandler.InstantBuildMode || (Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild))
				{
					if (def.IsValidBuildLocation(visualizer, vector, buildingOrientation) && def.IsValidPlaceLocation(visualizer, vector, buildingOrientation, out string _))
					{
						gameObject = def.Build(cell, buildingOrientation, null, selectedElements, 293.15f, false);
						if ((Object)source != (Object)null)
						{
							source.DeleteObject();
						}
					}
				}
				else
				{
					gameObject = def.TryPlace(visualizer, vector, buildingOrientation, selectedElements, 0);
					if ((Object)gameObject == (Object)null && def.ReplacementLayer != ObjectLayer.NumLayers)
					{
						if (!Grid.ObjectLayers[(int)def.TileLayer].ContainsKey(cell))
						{
							return;
						}
						GameObject gameObject2 = Grid.ObjectLayers[(int)def.TileLayer][cell];
						if ((Object)gameObject2 != (Object)null && (Object)Grid.Objects[cell, (int)def.ReplacementLayer] == (Object)null)
						{
							BuildingComplete component = gameObject2.GetComponent<BuildingComplete>();
							if ((Object)component != (Object)null && component.Def.Replaceable && def.CanReplace(gameObject2) && ((Object)component.Def != (Object)def || selectedElements[0] != gameObject2.GetComponent<PrimaryElement>().Element.tag))
							{
								gameObject = def.TryReplaceTile(visualizer, vector, buildingOrientation, selectedElements, 0);
								Grid.Objects[cell, (int)def.ReplacementLayer] = gameObject;
							}
						}
					}
					if ((Object)gameObject != (Object)null)
					{
						Prioritizable component2 = gameObject.GetComponent<Prioritizable>();
						if ((Object)component2 != (Object)null)
						{
							if ((Object)BuildMenu.Instance != (Object)null)
							{
								component2.SetMasterPriority(BuildMenu.Instance.GetBuildingPriority());
							}
							if ((Object)PlanScreen.Instance != (Object)null)
							{
								component2.SetMasterPriority(PlanScreen.Instance.GetBuildingPriority());
							}
						}
						if ((Object)source != (Object)null)
						{
							source.Trigger(2121280625, gameObject);
						}
					}
				}
				if ((Object)gameObject != (Object)null)
				{
					if (def.MaterialsAvailable(selectedElements) || DebugHandler.InstantBuildMode)
					{
						placeSound = GlobalAssets.GetSound("Place_Building_" + def.AudioSize, false);
						if (placeSound != null)
						{
							buildingCount = buildingCount % 14 + 1;
							EventInstance instance = SoundEvent.BeginOneShot(placeSound, vector);
							if (def.AudioSize == "small")
							{
								instance.setParameterValue("tileCount", (float)buildingCount);
							}
							SoundEvent.EndOneShot(instance);
						}
					}
					else
					{
						PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, UI.TOOLTIPS.NOMATERIAL, null, vector, 1.5f, false, false);
					}
					Rotatable component3 = gameObject.GetComponent<Rotatable>();
					if ((Object)component3 != (Object)null)
					{
						component3.SetOrientation(buildingOrientation);
					}
				}
			}
		}
	}

	protected override Mode GetMode()
	{
		return Mode.Brush;
	}

	private void SetColor(GameObject root, Color c, float strength)
	{
		KBatchedAnimController component = root.GetComponent<KBatchedAnimController>();
		if ((Object)component != (Object)null)
		{
			component.TintColour = c;
		}
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

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		base.OnLeftClickDown(cursor_pos);
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		base.OnLeftClickUp(cursor_pos);
	}
}
