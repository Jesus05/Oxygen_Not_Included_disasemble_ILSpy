using FMOD.Studio;
using STRINGS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUtilityBuildTool : DragTool
{
	protected struct PathNode
	{
		public int cell;

		public bool valid;

		public GameObject visualizer;

		public void Play(string anim)
		{
			KBatchedAnimController component = visualizer.GetComponent<KBatchedAnimController>();
			component.Play(anim, KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	private IList<Tag> selectedElements;

	private BuildingDef def;

	protected List<PathNode> path = new List<PathNode>();

	protected IUtilityNetworkMgr conduitMgr;

	private Coroutine visUpdater;

	private int buildingCount;

	private int lastCell = -1;

	private BuildingCellVisualizer previousCellConnection;

	private int previousCell;

	protected override void OnPrefabInit()
	{
		buildingCount = Random.Range(1, 14);
		canChangeDragAxis = false;
	}

	private void Play(GameObject go, string anim)
	{
		KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
		component.Play(anim, KAnim.PlayMode.Once, 1f, 0f);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
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
			component.SetDirty();
		}
		visualizer.SetActive(true);
		Play(visualizer, "None_Place");
		BuildToolHoverTextCard component2 = GetComponent<BuildToolHoverTextCard>();
		component2.currentDef = def;
		ResourceRemainingDisplayScreen.instance.ActivateDisplay(visualizer);
		IHaveUtilityNetworkMgr component3 = def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>();
		conduitMgr = component3.GetNetworkManager();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		StopVisUpdater();
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
		if ((Object)visualizer != (Object)null)
		{
			Object.Destroy(visualizer);
		}
		base.OnDeactivateTool(new_tool);
	}

	public void Activate(BuildingDef def, IList<Tag> selected_elements)
	{
		selectedElements = selected_elements;
		this.def = def;
		viewMode = def.ViewMode;
		PlayerController.Instance.ActivateTool(this);
		ResourceRemainingDisplayScreen.instance.SetResources(selected_elements, def.CraftRecipe);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (path.Count == 0)
		{
			return;
		}
		PathNode pathNode = path[path.Count - 1];
		if (pathNode.cell == cell)
		{
			return;
		}
		placeSound = GlobalAssets.GetSound("Place_building_" + def.AudioSize, false);
		Vector3 pos = Grid.CellToPos(cell);
		EventInstance instance = SoundEvent.BeginOneShot(placeSound, pos);
		if (path.Count > 1)
		{
			int num = cell;
			PathNode pathNode2 = path[path.Count - 2];
			if (num == pathNode2.cell)
			{
				if ((Object)previousCellConnection != (Object)null)
				{
					previousCellConnection.ConnectedEvent(previousCell);
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound("OutletDisconnected", false));
					previousCellConnection = null;
				}
				previousCell = cell;
				CheckForConnection(cell, def.PrefabID, string.Empty, ref previousCellConnection, false);
				PathNode pathNode3 = path[path.Count - 1];
				Object.Destroy(pathNode3.visualizer);
				PathNode pathNode4 = path[path.Count - 1];
				TileVisualizer.RefreshCell(pathNode4.cell, def.TileLayer, def.ReplacementLayer);
				path.RemoveAt(path.Count - 1);
				buildingCount = ((buildingCount != 1) ? (buildingCount - 1) : (buildingCount = 14));
				instance.setParameterValue("tileCount", (float)buildingCount);
				SoundEvent.EndOneShot(instance);
				goto IL_029c;
			}
		}
		if (!path.Exists((PathNode n) => n.cell == cell))
		{
			bool valid = CheckValidPathPiece(cell);
			path.Add(new PathNode
			{
				cell = cell,
				visualizer = null,
				valid = valid
			});
			CheckForConnection(cell, def.PrefabID, "OutletConnected", ref previousCellConnection, true);
			buildingCount = buildingCount % 14 + 1;
			instance.setParameterValue("tileCount", (float)buildingCount);
			SoundEvent.EndOneShot(instance);
		}
		goto IL_029c;
		IL_029c:
		visualizer.SetActive(path.Count < 2);
		ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(path.Count);
	}

	protected override int GetDragLength()
	{
		return path.Count;
	}

	private bool CheckValidPathPiece(int cell)
	{
		if (def.BuildLocationRule == BuildLocationRule.NotInTiles)
		{
			if ((Object)Grid.Objects[cell, 9] != (Object)null)
			{
				return false;
			}
			if (Grid.HasDoor[cell])
			{
				return false;
			}
		}
		GameObject gameObject = Grid.Objects[cell, (int)def.ObjectLayer];
		if ((Object)gameObject != (Object)null && (Object)gameObject.GetComponent<KAnimGraphTileVisualizer>() == (Object)null)
		{
			return false;
		}
		GameObject gameObject2 = Grid.Objects[cell, (int)def.TileLayer];
		if ((Object)gameObject2 != (Object)null && (Object)gameObject2.GetComponent<KAnimGraphTileVisualizer>() == (Object)null)
		{
			return false;
		}
		return true;
	}

	private bool CheckForConnection(int cell, string defName, string soundName, ref BuildingCellVisualizer outBcv, bool fireEvents = true)
	{
		Building building = GetBuilding(cell);
		if ((Object)building != (Object)null)
		{
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			if (defName.Contains("LogicWire"))
			{
				LogicPorts component = building.gameObject.GetComponent<LogicPorts>();
				if ((Object)component != (Object)null)
				{
					foreach (ILogicUIElement inputPort in component.inputPorts)
					{
						if (inputPort.GetLogicUICell() == cell)
						{
							num = inputPort.GetLogicUICell();
							break;
						}
					}
					if (num == -1)
					{
						foreach (ILogicUIElement outputPort in component.outputPorts)
						{
							if (outputPort.GetLogicUICell() == cell)
							{
								num2 = outputPort.GetLogicUICell();
								break;
							}
						}
					}
				}
			}
			else if (defName.Contains("Wire"))
			{
				num = building.GetPowerInputCell();
				num2 = building.GetPowerOutputCell();
			}
			else if (defName.Contains("Liquid"))
			{
				if (building.Def.InputConduitType == ConduitType.Liquid)
				{
					num = building.GetUtilityInputCell();
				}
				if (building.Def.OutputConduitType == ConduitType.Liquid)
				{
					num2 = building.GetUtilityOutputCell();
				}
				ElementFilter component2 = building.GetComponent<ElementFilter>();
				if ((Object)component2 != (Object)null && component2.portInfo.conduitType == ConduitType.Liquid)
				{
					num3 = component2.GetFilteredCell();
				}
			}
			else if (defName.Contains("Gas"))
			{
				if (building.Def.InputConduitType == ConduitType.Gas)
				{
					num = building.GetUtilityInputCell();
				}
				if (building.Def.OutputConduitType == ConduitType.Gas)
				{
					num2 = building.GetUtilityOutputCell();
				}
				ElementFilter component3 = building.GetComponent<ElementFilter>();
				if ((Object)component3 != (Object)null && component3.portInfo.conduitType == ConduitType.Gas)
				{
					num3 = component3.GetFilteredCell();
				}
			}
			if (cell == num || cell == num2 || cell == num3)
			{
				BuildingCellVisualizer buildingCellVisualizer = outBcv = building.gameObject.GetComponent<BuildingCellVisualizer>();
				if (((Object)buildingCellVisualizer != (Object)null) ? true : false)
				{
					if (fireEvents)
					{
						buildingCellVisualizer.ConnectedEvent(cell);
						string sound = GlobalAssets.GetSound(soundName, false);
						if (sound != null)
						{
							KMonoBehaviour.PlaySound(sound);
						}
					}
					return true;
				}
			}
		}
		outBcv = null;
		return false;
	}

	private Building GetBuilding(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 1];
		if ((Object)gameObject != (Object)null)
		{
			return gameObject.GetComponent<Building>();
		}
		return null;
	}

	protected override Mode GetMode()
	{
		return Mode.Brush;
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		if (!((Object)visualizer == (Object)null))
		{
			path.Clear();
			int cell = Grid.PosToCell(cursor_pos);
			if (Grid.IsValidCell(cell) && Grid.IsVisible(cell))
			{
				bool valid = CheckValidPathPiece(cell);
				path.Add(new PathNode
				{
					cell = cell,
					visualizer = null,
					valid = valid
				});
				CheckForConnection(cell, def.PrefabID, "OutletConnected", ref previousCellConnection, true);
			}
			visUpdater = StartCoroutine(VisUpdater());
			visualizer.GetComponent<KBatchedAnimController>().StopAndClear();
			ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(1);
			placeSound = GlobalAssets.GetSound("Place_building_" + def.AudioSize, false);
			if (placeSound != null)
			{
				buildingCount = buildingCount % 14 + 1;
				Vector3 pos = Grid.CellToPos(cell);
				EventInstance instance = SoundEvent.BeginOneShot(placeSound, pos);
				if (def.AudioSize == "small")
				{
					instance.setParameterValue("tileCount", (float)buildingCount);
				}
				SoundEvent.EndOneShot(instance);
			}
			base.OnLeftClickDown(cursor_pos);
		}
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		if (!((Object)visualizer == (Object)null))
		{
			BuildPath();
			StopVisUpdater();
			Play(visualizer, "None_Place");
			ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(0);
			base.OnLeftClickUp(cursor_pos);
		}
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		int num = Grid.PosToCell(cursorPos);
		if (lastCell != num)
		{
			lastCell = num;
		}
		if ((Object)visualizer != (Object)null)
		{
			Color c = Color.white;
			float strength = 0f;
			if (!def.IsValidPlaceLocation(visualizer, num, Orientation.Neutral, out string _))
			{
				c = Color.red;
				strength = 1f;
			}
			SetColor(visualizer, c, strength);
		}
	}

	private void SetColor(GameObject root, Color c, float strength)
	{
		KBatchedAnimController component = root.GetComponent<KBatchedAnimController>();
		if ((Object)component != (Object)null)
		{
			component.TintColour = c;
		}
	}

	protected virtual void ApplyPathToConduitSystem()
	{
	}

	private IEnumerator VisUpdater()
	{
		conduitMgr.StashVisualGrids();
		if (path.Count == 1)
		{
			PathNode node = path[0];
			path[0] = CreateVisualizer(node);
		}
		ApplyPathToConduitSystem();
		for (int i = 0; i < path.Count; i++)
		{
			PathNode pathNode = path[i];
			pathNode = CreateVisualizer(pathNode);
			path[i] = pathNode;
			string text = conduitMgr.GetVisualizerString(pathNode.cell) + "_place";
			KBatchedAnimController component = pathNode.visualizer.GetComponent<KBatchedAnimController>();
			if (component.HasAnimation(text))
			{
				pathNode.Play(text);
			}
			else
			{
				pathNode.Play(conduitMgr.GetVisualizerString(pathNode.cell));
			}
			component.TintColour = ((!def.IsValidBuildLocation(null, pathNode.cell, Orientation.Neutral, out string _)) ? Color.red : Color.white);
			TileVisualizer.RefreshCell(pathNode.cell, def.TileLayer, def.ReplacementLayer);
		}
		conduitMgr.UnstashVisualGrids();
		yield return (object)null;
		/*Error: Unable to find new state assignment for yield return*/;
	}

	private void BuildPath()
	{
		ApplyPathToConduitSystem();
		int num = 0;
		for (int i = 0; i < path.Count; i++)
		{
			PathNode pathNode = path[i];
			Vector3 vector = Grid.CellToPosCBC(pathNode.cell, Grid.SceneLayer.Building);
			UtilityConnections utilityConnections = (UtilityConnections)0;
			GameObject gameObject = Grid.Objects[pathNode.cell, (int)def.TileLayer];
			if ((Object)gameObject == (Object)null)
			{
				utilityConnections = conduitMgr.GetConnections(pathNode.cell, false);
				if ((DebugHandler.InstantBuildMode || (Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild)) && def.IsValidBuildLocation(visualizer, vector, Orientation.Neutral) && def.IsValidPlaceLocation(visualizer, vector, Orientation.Neutral, out string _))
				{
					gameObject = def.Build(pathNode.cell, Orientation.Neutral, null, selectedElements, 293.15f, true);
				}
				else
				{
					gameObject = def.TryPlace(null, vector, Orientation.Neutral, selectedElements, 0);
					if ((Object)gameObject != (Object)null)
					{
						if (!def.MaterialsAvailable(selectedElements) && !DebugHandler.InstantBuildMode)
						{
							PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, UI.TOOLTIPS.NOMATERIAL, null, vector, 1.5f, false, false);
						}
						Constructable component = gameObject.GetComponent<Constructable>();
						if (component.IconConnectionAnimation(0.1f * (float)num, num, "Wire", "OutletConnected_release") || component.IconConnectionAnimation(0.1f * (float)num, num, "Pipe", "OutletConnected_release"))
						{
							num++;
						}
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
					}
				}
			}
			else
			{
				IUtilityItem component3 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
				if (component3 != null)
				{
					utilityConnections = component3.Connections;
				}
				utilityConnections |= conduitMgr.GetConnections(pathNode.cell, false);
				if ((Object)gameObject.GetComponent<BuildingComplete>() != (Object)null)
				{
					component3.UpdateConnections(utilityConnections);
				}
			}
			if (def.ReplacementLayer != ObjectLayer.NumLayers && !DebugHandler.InstantBuildMode && (!Game.Instance.SandboxModeActive || !SandboxToolParameterMenu.instance.settings.InstantBuild) && def.IsValidBuildLocation(null, vector, Orientation.Neutral))
			{
				GameObject gameObject2 = Grid.Objects[pathNode.cell, (int)def.TileLayer];
				GameObject x = Grid.Objects[pathNode.cell, (int)def.ReplacementLayer];
				if ((Object)gameObject2 != (Object)null && (Object)x == (Object)null)
				{
					BuildingComplete component4 = gameObject2.GetComponent<BuildingComplete>();
					if ((Object)component4 != (Object)null && (Object)component4.Def != (Object)def)
					{
						Constructable component5 = def.BuildingUnderConstruction.GetComponent<Constructable>();
						component5.IsReplacementTile = true;
						gameObject = def.Instantiate(vector, Orientation.Neutral, selectedElements, 0);
						component5.IsReplacementTile = false;
						if (!def.MaterialsAvailable(selectedElements) && !DebugHandler.InstantBuildMode)
						{
							PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, UI.TOOLTIPS.NOMATERIAL, null, vector, 1.5f, false, false);
						}
						Grid.Objects[pathNode.cell, (int)def.ReplacementLayer] = gameObject;
						IUtilityItem component6 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
						if (component6 != null)
						{
							utilityConnections = component6.Connections;
						}
						utilityConnections |= conduitMgr.GetConnections(pathNode.cell, false);
						if ((Object)gameObject.GetComponent<BuildingComplete>() != (Object)null)
						{
							component6.UpdateConnections(utilityConnections);
						}
						string visualizerString = conduitMgr.GetVisualizerString(utilityConnections);
						string text = visualizerString;
						if (gameObject.GetComponent<KBatchedAnimController>().HasAnimation(visualizerString + "_place"))
						{
							text += "_place";
						}
						Play(gameObject, text);
					}
				}
			}
			if ((Object)gameObject != (Object)null)
			{
				IUtilityItem component7 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
				if (component7 != null)
				{
					component7.Connections = utilityConnections;
				}
			}
			TileVisualizer.RefreshCell(pathNode.cell, def.TileLayer, def.ReplacementLayer);
		}
		ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(0);
	}

	private PathNode CreateVisualizer(PathNode node)
	{
		if ((Object)node.visualizer == (Object)null)
		{
			Vector3 position = Grid.CellToPosCBC(node.cell, def.SceneLayer);
			GameObject gameObject = Object.Instantiate(def.BuildingPreview, position, Quaternion.identity);
			gameObject.SetActive(true);
			node.visualizer = gameObject;
		}
		return node;
	}

	private void StopVisUpdater()
	{
		for (int i = 0; i < path.Count; i++)
		{
			PathNode pathNode = path[i];
			Object.Destroy(pathNode.visualizer);
		}
		path.Clear();
		if (visUpdater != null)
		{
			StopCoroutine(visUpdater);
			visUpdater = null;
		}
	}
}
