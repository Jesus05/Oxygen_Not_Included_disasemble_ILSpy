using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ToolMenu : KScreen
{
	public class ToolInfo
	{
		public string text;

		public string icon;

		public Action hotkey;

		public string toolName;

		public ToolCollection collection;

		public string tooltip;

		public SimViewMode viewMode;

		public bool forceViewMode;

		public KToggle toggle;

		public Action<object> onSelectCallback;

		public object toolData;

		public ToolInfo(string text, string icon_name, Action hotkey, string ToolName, ToolCollection toolCollection, string tooltip = "", SimViewMode associatedViewMode = SimViewMode.None, bool forceViewMode = false, Action<object> onSelectCallback = null, object toolData = null)
		{
			this.text = text;
			icon = icon_name;
			this.hotkey = hotkey;
			toolName = ToolName;
			collection = toolCollection;
			toolCollection.tools.Add(this);
			this.tooltip = tooltip;
			viewMode = associatedViewMode;
			this.forceViewMode = forceViewMode;
			this.onSelectCallback = onSelectCallback;
			this.toolData = toolData;
		}
	}

	public class ToolCollection
	{
		public string text;

		public string icon;

		public string tooltip;

		public bool useInfoMenu;

		public GameObject toggle;

		public List<ToolInfo> tools = new List<ToolInfo>();

		public GameObject UIMenuDisplay;

		public GameObject MaskContainer;

		public Action hotkey;

		public ToolCollection(string text, string icon_name, string tooltip = "", bool useInfoMenu = false, Action hotkey = Action.NumActions)
		{
			this.text = text;
			icon = icon_name;
			this.tooltip = tooltip;
			this.useInfoMenu = useInfoMenu;
			this.hotkey = hotkey;
		}
	}

	public struct CellColorData
	{
		public int cell;

		public Color color;

		public CellColorData(int cell, Color color)
		{
			this.cell = cell;
			this.color = color;
		}
	}

	public static ToolMenu Instance;

	public GameObject Prefab_collectionContainer;

	public GameObject Prefab_collectionContainerWindow;

	public PriorityScreen Prefab_priorityScreen;

	public GameObject toolIconPrefab;

	public GameObject sandboxToolIconPrefab;

	public GameObject collectionIconPrefab;

	public GameObject prefabToolRow;

	[SerializeField]
	private Sprite[] icons;

	private PriorityScreen priorityScreen;

	public ToolParameterMenu toolParameterMenu;

	public GameObject sandboxToolParameterMenu;

	private GameObject toolEffectDisplayPlane;

	private Texture2D toolEffectDisplayPlaneTexture;

	public Material toolEffectDisplayMaterial;

	private byte[] toolEffectDisplayBytes;

	private List<ToolCollection[]> rows = new List<ToolCollection[]>();

	public ToolCollection[] rowBasicTools;

	public ToolCollection[] rowSandboxTools;

	public ToolCollection currentlySelectedCollection;

	public ToolInfo currentlySelectedTool;

	private Coroutine activeOpenAnimationRoutine;

	private Coroutine activeCloseAnimationRoutine;

	private HashSet<Action> boundRootActions = new HashSet<Action>();

	private HashSet<Action> boundSubgroupActions = new HashSet<Action>();

	[SerializeField]
	public TextStyleSetting ToggleToolTipTextStyleSetting;

	[SerializeField]
	public TextStyleSetting CategoryLabelTextStyle_LeftAlign;

	private int smallCollectionMax = 5;

	private HashSet<CellColorData> colors = new HashSet<CellColorData>();

	public PriorityScreen PriorityScreen => priorityScreen;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public override float GetSortKey()
	{
		return 5f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		priorityScreen = Util.KInstantiateUI<PriorityScreen>(Prefab_priorityScreen.gameObject, base.gameObject, false);
		priorityScreen.InstantiateButtons(OnPriorityClicked, false);
	}

	protected override void OnSpawn()
	{
		activateOnSpawn = true;
		base.OnSpawn();
		SetData();
		rows.ForEach(delegate(ToolCollection[] row)
		{
			InstantiateCollectionsUI(row);
		});
		rows.ForEach(delegate(ToolCollection[] row)
		{
			BuildRowToggles(row);
		});
		rows.ForEach(delegate(ToolCollection[] row)
		{
			BuildToolToggles(row);
		});
		ChooseCollection(null, true);
		priorityScreen.gameObject.SetActive(false);
		ToggleSandboxUI(null);
		Game.Instance.Subscribe(-1948169901, ToggleSandboxUI);
		ResetToolDisplayPlane();
	}

	private void ResetToolDisplayPlane()
	{
		toolEffectDisplayPlane = CreateToolDisplayPlane("Overlay", World.Instance.transform);
		toolEffectDisplayPlaneTexture = CreatePlaneTexture(out toolEffectDisplayBytes, Grid.WidthInCells, Grid.HeightInCells);
		toolEffectDisplayPlane.GetComponent<Renderer>().sharedMaterial = toolEffectDisplayMaterial;
		toolEffectDisplayPlane.GetComponent<Renderer>().sharedMaterial.mainTexture = toolEffectDisplayPlaneTexture;
		toolEffectDisplayPlane.transform.SetLocalPosition(new Vector3(Grid.WidthInMeters / 2f, Grid.HeightInMeters / 2f, -6f));
		RefreshToolDisplayPlaneColor();
	}

	private GameObject CreateToolDisplayPlane(string layer, Transform parent)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		gameObject.name = "toolEffectDisplayPlane";
		gameObject.SetLayerRecursively(LayerMask.NameToLayer(layer));
		UnityEngine.Object.Destroy(gameObject.GetComponent<Collider>());
		if ((UnityEngine.Object)parent != (UnityEngine.Object)null)
		{
			gameObject.transform.SetParent(parent);
		}
		gameObject.transform.SetPosition(Vector3.zero);
		gameObject.transform.localScale = new Vector3(Grid.WidthInMeters / -10f, 1f, Grid.HeightInMeters / -10f);
		gameObject.transform.eulerAngles = new Vector3(270f, 0f, 0f);
		gameObject.GetComponent<MeshRenderer>().reflectionProbeUsage = ReflectionProbeUsage.Off;
		return gameObject;
	}

	private Texture2D CreatePlaneTexture(out byte[] textureBytes, int width, int height)
	{
		textureBytes = new byte[width * height * 4];
		Texture2D texture2D = new Texture2D(width, height, TextureUtil.TextureFormatToGraphicsFormat(TextureFormat.RGBA32), TextureCreationFlags.None);
		texture2D.name = "toolEffectDisplayPlane";
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.filterMode = FilterMode.Point;
		return texture2D;
	}

	private void Update()
	{
		RefreshToolDisplayPlaneColor();
	}

	private void RefreshToolDisplayPlaneColor()
	{
		if ((UnityEngine.Object)PlayerController.Instance.ActiveTool == (UnityEngine.Object)null || (UnityEngine.Object)PlayerController.Instance.ActiveTool == (UnityEngine.Object)SelectTool.Instance)
		{
			toolEffectDisplayPlane.SetActive(false);
		}
		else
		{
			PlayerController.Instance.ActiveTool.GetOverlayColorData(out colors);
			Array.Clear(toolEffectDisplayBytes, 0, toolEffectDisplayBytes.Length);
			if (colors != null)
			{
				foreach (CellColorData color in colors)
				{
					CellColorData current = color;
					if (Grid.IsValidCell(current.cell))
					{
						int num = current.cell * 4;
						if (num >= 0)
						{
							toolEffectDisplayBytes[num] = (byte)(Mathf.Min(current.color.r, 1f) * 255f);
							toolEffectDisplayBytes[num + 1] = (byte)(Mathf.Min(current.color.g, 1f) * 255f);
							toolEffectDisplayBytes[num + 2] = (byte)(Mathf.Min(current.color.b, 1f) * 255f);
							toolEffectDisplayBytes[num + 3] = (byte)(Mathf.Min(current.color.a, 1f) * 255f);
						}
					}
				}
			}
			if (!toolEffectDisplayPlane.activeSelf)
			{
				toolEffectDisplayPlane.SetActive(true);
			}
			toolEffectDisplayPlaneTexture.LoadRawTextureData(toolEffectDisplayBytes);
			toolEffectDisplayPlaneTexture.Apply();
		}
	}

	public void ToggleSandboxUI(object data = null)
	{
		ClearSelection();
		PlayerController.Instance.ActivateTool(SelectTool.Instance);
		rowSandboxTools[0].toggle.transform.parent.gameObject.SetActive(Game.Instance.SandboxModeActive);
	}

	private void SetData()
	{
		ToolCollection toolCollection = new ToolCollection(UI.TOOLS.SANDBOX.BRUSH.NAME, "brush", string.Empty, false, Action.NumActions);
		new ToolInfo(UI.TOOLS.SANDBOX.BRUSH.NAME, "brush", Action.SandboxBrush, "SandboxBrushTool", toolCollection, UI.SANDBOXTOOLS.SETTINGS.BRUSH.TOOLTIP, SimViewMode.None, false, null, null);
		ToolCollection toolCollection2 = new ToolCollection(UI.TOOLS.SANDBOX.SPRINKLE.NAME, "sprinkle", string.Empty, false, Action.NumActions);
		new ToolInfo(UI.TOOLS.SANDBOX.SPRINKLE.NAME, "sprinkle", Action.SandboxSprinkle, "SandboxSprinkleTool", toolCollection2, UI.SANDBOXTOOLS.SETTINGS.SPRINKLE.TOOLTIP, SimViewMode.None, false, null, null);
		ToolCollection toolCollection3 = new ToolCollection(UI.TOOLS.SANDBOX.FLOOD.NAME, "flood", string.Empty, false, Action.NumActions);
		new ToolInfo(UI.TOOLS.SANDBOX.FLOOD.NAME, "flood", Action.SandboxFlood, "SandboxFloodTool", toolCollection3, UI.SANDBOXTOOLS.SETTINGS.FLOOD.TOOLTIP, SimViewMode.None, false, null, null);
		ToolCollection toolCollection4 = new ToolCollection(UI.TOOLS.SANDBOX.SAMPLE.NAME, "sample", string.Empty, false, Action.NumActions);
		new ToolInfo(UI.TOOLS.SANDBOX.SAMPLE.NAME, "sample", Action.SandboxSample, "SandboxSampleTool", toolCollection4, UI.SANDBOXTOOLS.SETTINGS.SAMPLE.TOOLTIP, SimViewMode.None, false, null, null);
		ToolCollection toolCollection5 = new ToolCollection(UI.TOOLS.SANDBOX.HEATGUN.NAME, "brush", string.Empty, false, Action.NumActions);
		new ToolInfo(UI.TOOLS.SANDBOX.HEATGUN.NAME, "brush", Action.SandboxHeatGun, "SandboxHeatTool", toolCollection5, UI.SANDBOXTOOLS.SETTINGS.HEATGUN.TOOLTIP, SimViewMode.None, false, null, null);
		ToolCollection toolCollection6 = new ToolCollection(UI.TOOLS.SANDBOX.SPAWNER.NAME, "spawn", string.Empty, false, Action.NumActions);
		new ToolInfo(UI.TOOLS.SANDBOX.SPAWNER.NAME, "spawn", Action.SandboxSpawnEntity, "SandboxSpawnerTool", toolCollection6, UI.SANDBOXTOOLS.SETTINGS.SPAWNER.TOOLTIP, SimViewMode.None, false, null, null);
		ToolCollection toolCollection7 = new ToolCollection(UI.TOOLS.SANDBOX.CLEAR_FLOOR.NAME, "clear_floor", string.Empty, false, Action.NumActions);
		new ToolInfo(UI.TOOLS.SANDBOX.CLEAR_FLOOR.NAME, "clear_floor", Action.SandboxClearFloor, "SandboxClearFloorTool", toolCollection7, UI.SANDBOXTOOLS.SETTINGS.CLEAR_FLOOR.TOOLTIP, SimViewMode.None, false, null, null);
		ToolCollection toolCollection8 = new ToolCollection(UI.TOOLS.SANDBOX.DESTROY.NAME, "destroy", string.Empty, false, Action.NumActions);
		new ToolInfo(UI.TOOLS.SANDBOX.DESTROY.NAME, "destroy", Action.SandboxDestroy, "SandboxDestroyerTool", toolCollection8, UI.SANDBOXTOOLS.SETTINGS.DESTROY.TOOLTIP, SimViewMode.None, false, null, null);
		ToolCollection toolCollection9 = new ToolCollection(UI.TOOLS.SANDBOX.FOW.NAME, "brush", string.Empty, false, Action.NumActions);
		new ToolInfo(UI.TOOLS.SANDBOX.FOW.NAME, "brush", Action.SandboxReveal, "SandboxFOWTool", toolCollection9, UI.SANDBOXTOOLS.SETTINGS.FOW.TOOLTIP, SimViewMode.None, false, null, null);
		rowSandboxTools = new ToolCollection[9]
		{
			toolCollection,
			toolCollection2,
			toolCollection3,
			toolCollection4,
			toolCollection5,
			toolCollection6,
			toolCollection7,
			toolCollection8,
			toolCollection9
		};
		ToolCollection toolCollection10 = new ToolCollection(UI.TOOLS.DECONSTRUCT.NAME, "icon_action_deconstruct", UI.TOOLTIPS.DECONSTRUCTBUTTON, false, Action.BuildingDeconstruct);
		new ToolInfo(UI.TOOLS.DECONSTRUCT.NAME, "icon_action_deconstruct", Action.BuildingDeconstruct, "DeconstructTool", toolCollection10, UI.TOOLTIPS.DECONSTRUCTBUTTON, SimViewMode.None, false, null, null);
		ToolCollection toolCollection11 = new ToolCollection(UI.TOOLS.CANCEL.NAME, "icon_action_cancel", UI.TOOLTIPS.CANCELBUTTON, false, Action.BuildingCancel);
		new ToolInfo(UI.TOOLS.CANCEL.NAME, "icon_action_cancel", Action.BuildingCancel, "CancelTool", toolCollection11, UI.TOOLTIPS.CANCELBUTTON, SimViewMode.None, false, null, null);
		ToolCollection toolCollection12 = new ToolCollection(UI.TOOLS.DIG.NAME, "icon_action_dig", string.Empty, false, Action.Dig);
		new ToolInfo(UI.TOOLS.DIG.NAME, "icon_action_dig", Action.Dig, "DigTool", toolCollection12, UI.TOOLTIPS.DIGBUTTON, SimViewMode.None, false, null, null);
		ToolCollection toolCollection13 = new ToolCollection(UI.TOOLS.PRIORITIESCATEGORY.NAME, "icon_action_prioritize", UI.TOOLTIPS.PRIORITIZEMAINBUTTON, false, Action.AccessPrioritizeCollection);
		new ToolInfo(UI.TOOLS.PRIORITIZE.NAME, "icon_action_prioritize", Action.Prioritize, "PrioritizeTool", toolCollection13, UI.TOOLTIPS.PRIORITIZEBUTTON, SimViewMode.None, false, null, null);
		ToolCollection toolCollection14 = new ToolCollection(UI.TOOLS.MARKFORSTORAGE.NAME, "icon_action_store", UI.TOOLTIPS.CLEARBUTTON, false, Action.NumActions);
		new ToolInfo(UI.TOOLS.MARKFORSTORAGE.NAME, "icon_action_store", Action.Clear, "ClearTool", toolCollection14, UI.TOOLTIPS.CLEARBUTTON, SimViewMode.None, false, null, null);
		ToolCollection toolCollection15 = new ToolCollection(UI.TOOLS.MOP.NAME, "icon_action_mop", UI.TOOLTIPS.MOPBUTTON, false, Action.NumActions);
		new ToolInfo(UI.TOOLS.MOP.NAME, "icon_action_mop", Action.Mop, "MopTool", toolCollection15, UI.TOOLTIPS.MOPBUTTON, SimViewMode.None, false, null, null);
		ToolCollection toolCollection16 = new ToolCollection(UI.TOOLS.DISINFECT.NAME, "icon_action_disinfect", UI.TOOLTIPS.DISINFECTBUTTON, false, Action.NumActions);
		new ToolInfo(UI.TOOLS.DISINFECT.NAME, "icon_action_disinfect", Action.Disinfect, "DisinfectTool", toolCollection16, UI.TOOLTIPS.DISINFECTBUTTON, SimViewMode.None, false, null, null);
		ToolCollection toolCollection17 = new ToolCollection(UI.TOOLS.ATTACK.NAME, "icon_action_attack", string.Empty, false, Action.Attack);
		new ToolInfo(UI.TOOLS.ATTACK.NAME, "icon_action_attack", Action.Attack, "AttackTool", toolCollection17, UI.TOOLTIPS.ATTACKBUTTON, SimViewMode.None, false, null, null);
		ToolCollection toolCollection18 = new ToolCollection(UI.TOOLS.CAPTURE.NAME, "icon_action_capture", string.Empty, false, Action.Capture);
		new ToolInfo(UI.TOOLS.CAPTURE.NAME, "icon_action_capture", Action.Capture, "CaptureTool", toolCollection18, UI.TOOLTIPS.CAPTUREBUTTON, SimViewMode.None, false, null, null);
		ToolCollection toolCollection19 = new ToolCollection(UI.TOOLS.HARVEST.NAME, "icon_action_harvest", string.Empty, false, Action.Harvest);
		new ToolInfo(UI.TOOLS.HARVEST.NAME, "icon_action_harvest", Action.Harvest, "HarvestTool", toolCollection19, UI.TOOLTIPS.HARVESTBUTTON, SimViewMode.None, false, null, null);
		ToolCollection toolCollection20 = new ToolCollection(UI.TOOLS.EMPTY_PIPE.NAME, "icon_action_empty_pipes", string.Empty, false, Action.Harvest);
		new ToolInfo(UI.TOOLS.EMPTY_PIPE.NAME, "icon_action_empty_pipes", Action.EmptyPipe, "EmptyPipeTool", toolCollection20, UI.TOOLS.EMPTY_PIPE.TOOLTIP, SimViewMode.None, false, null, null);
		rowBasicTools = new ToolCollection[11]
		{
			toolCollection12,
			toolCollection18,
			toolCollection19,
			toolCollection20,
			toolCollection13,
			toolCollection14,
			toolCollection15,
			toolCollection16,
			toolCollection10,
			toolCollection17,
			toolCollection11
		};
		rows.Add(rowSandboxTools);
		rows.Add(rowBasicTools);
	}

	private void InstantiateCollectionsUI(ToolCollection[] collections)
	{
		GameObject parent = Util.KInstantiateUI(prefabToolRow, base.gameObject, true);
		for (int i = 0; i < collections.Length; i++)
		{
			ToolCollection tc = collections[i];
			tc.toggle = Util.KInstantiateUI((collections[i].tools.Count > 1) ? collectionIconPrefab : ((collections != rowSandboxTools) ? toolIconPrefab : sandboxToolIconPrefab), parent, true);
			KToggle component = tc.toggle.GetComponent<KToggle>();
			component.soundPlayer.Enabled = false;
			component.onClick += delegate
			{
				if (currentlySelectedCollection == tc && tc.tools.Count >= 1)
				{
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound(), false));
				}
				ChooseCollection(tc, true);
			};
			if (tc.tools != null)
			{
				GameObject gameObject = null;
				if (tc.tools.Count < smallCollectionMax)
				{
					gameObject = Util.KInstantiateUI(Prefab_collectionContainer, parent, true);
					gameObject.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() - 1);
					gameObject.transform.localScale = Vector3.one;
					gameObject.rectTransform().sizeDelta = new Vector2((float)(tc.tools.Count * 75), 50f);
					tc.MaskContainer = gameObject.GetComponentInChildren<Mask>().gameObject;
					gameObject.SetActive(false);
				}
				else
				{
					gameObject = Util.KInstantiateUI(Prefab_collectionContainerWindow, parent, true);
					gameObject.transform.localScale = Vector3.one;
					gameObject.GetComponentInChildren<LocText>().SetText(tc.text.ToUpper());
					tc.MaskContainer = gameObject.GetComponentInChildren<GridLayoutGroup>().gameObject;
					gameObject.SetActive(false);
				}
				tc.UIMenuDisplay = gameObject;
				for (int j = 0; j < tc.tools.Count; j++)
				{
					ToolInfo ti = tc.tools[j];
					GameObject gameObject2 = Util.KInstantiateUI((collections != rowSandboxTools) ? toolIconPrefab : sandboxToolIconPrefab, tc.MaskContainer, true);
					gameObject2.name = ti.text;
					ti.toggle = gameObject2.GetComponent<KToggle>();
					if (ti.collection.tools.Count > 1)
					{
						RectTransform rectTransform = null;
						rectTransform = ti.toggle.gameObject.GetComponentInChildren<SetTextStyleSetting>().rectTransform();
						if (gameObject2.name.Length > 12)
						{
							rectTransform.GetComponent<SetTextStyleSetting>().SetStyle(CategoryLabelTextStyle_LeftAlign);
							RectTransform rectTransform2 = rectTransform;
							Vector2 anchoredPosition = rectTransform.anchoredPosition;
							rectTransform2.anchoredPosition = new Vector2(16f, anchoredPosition.y);
						}
					}
					ti.toggle.onClick += delegate
					{
						ChooseTool(ti);
					};
					tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapse(delegate
					{
						SetToggleState(tc.toggle.GetComponent<KToggle>(), false);
						tc.UIMenuDisplay.SetActive(false);
					});
				}
			}
		}
	}

	private void ChooseTool(ToolInfo tool)
	{
		if (currentlySelectedTool != tool)
		{
			if (currentlySelectedTool != tool)
			{
				currentlySelectedTool = tool;
				if (currentlySelectedTool != null && currentlySelectedTool.onSelectCallback != null)
				{
					currentlySelectedTool.onSelectCallback(currentlySelectedTool);
				}
			}
			if (currentlySelectedTool != null)
			{
				currentlySelectedCollection = currentlySelectedTool.collection;
				InterfaceTool[] tools = PlayerController.Instance.tools;
				foreach (InterfaceTool interfaceTool in tools)
				{
					if (currentlySelectedTool.toolName == interfaceTool.name)
					{
						UISounds.PlaySound(UISounds.Sound.ClickObject);
						PlayerController.Instance.ActivateTool(interfaceTool);
						if (tool.forceViewMode && OverlayScreen.Instance.GetMode() != tool.viewMode)
						{
							Game.Instance.gameObject.Trigger(1248612973, tool.viewMode);
						}
						break;
					}
				}
			}
			else
			{
				PlayerController.Instance.ActivateTool(SelectTool.Instance);
			}
			rows.ForEach(delegate(ToolCollection[] row)
			{
				RefreshRowDisplay(row);
			});
		}
	}

	private void RefreshRowDisplay(ToolCollection[] row)
	{
		foreach (ToolCollection tc in row)
		{
			if (currentlySelectedTool != null && currentlySelectedTool.collection == tc)
			{
				if (!tc.UIMenuDisplay.activeSelf || tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapsing)
				{
					if (tc.tools.Count > 1)
					{
						tc.UIMenuDisplay.SetActive(true);
						if (tc.tools.Count < smallCollectionMax)
						{
							float speedScale = Mathf.Clamp(1f - (float)tc.tools.Count * 0.15f, 0.5f, 1f);
							tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().speedScale = speedScale;
						}
						tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Expand(delegate
						{
							SetToggleState(tc.toggle.GetComponent<KToggle>(), true);
						});
					}
					else
					{
						currentlySelectedTool = tc.tools[0];
					}
				}
			}
			else if (tc.UIMenuDisplay.activeSelf && !tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapsing && tc.tools.Count > 0)
			{
				tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapse(delegate
				{
					SetToggleState(tc.toggle.GetComponent<KToggle>(), false);
					tc.UIMenuDisplay.SetActive(false);
				});
			}
			for (int j = 0; j < tc.tools.Count; j++)
			{
				if (tc.tools[j] == currentlySelectedTool)
				{
					SetToggleState(tc.tools[j].toggle, true);
				}
				else
				{
					SetToggleState(tc.tools[j].toggle, false);
				}
			}
		}
	}

	public void TurnLargeCollectionOff()
	{
		if (currentlySelectedCollection != null && currentlySelectedCollection.tools.Count > smallCollectionMax)
		{
			ChooseCollection(null, true);
		}
	}

	private void ChooseCollection(ToolCollection collection, bool autoSelectTool = true)
	{
		if (collection == currentlySelectedCollection)
		{
			if (collection != null && collection.tools.Count > 1)
			{
				currentlySelectedCollection = null;
				if (currentlySelectedTool != null)
				{
					ChooseTool(null);
				}
			}
			else if (currentlySelectedTool != null && currentlySelectedCollection.tools.Contains(currentlySelectedTool) && currentlySelectedCollection.tools.Count == 1)
			{
				currentlySelectedCollection = null;
				ChooseTool(null);
			}
		}
		else
		{
			currentlySelectedCollection = collection;
		}
		rows.ForEach(delegate(ToolCollection[] row)
		{
			OpenOrCloseCollectionsInRow(row, true);
		});
	}

	private void OpenOrCloseCollectionsInRow(ToolCollection[] row, bool autoSelectTool = true)
	{
		foreach (ToolCollection tc in row)
		{
			if (currentlySelectedCollection == tc)
			{
				if ((currentlySelectedCollection.tools != null && currentlySelectedCollection.tools.Count == 1) || autoSelectTool)
				{
					ChooseTool(currentlySelectedCollection.tools[0]);
				}
			}
			else if (tc.UIMenuDisplay.activeSelf && !tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapsing)
			{
				tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapse(delegate
				{
					SetToggleState(tc.toggle.GetComponent<KToggle>(), false);
					tc.UIMenuDisplay.SetActive(false);
				});
			}
			SetToggleState(tc.toggle.GetComponent<KToggle>(), currentlySelectedCollection == tc);
		}
	}

	private IEnumerator CloseCollection(ToolCollection tc)
	{
		Animator anim = tc.UIMenuDisplay.GetComponent<Animator>();
		float speedMultiplier = 1f;
		float speedAdjustmentPerTool = 0.125f;
		anim.speed = speedMultiplier;
		anim.speed = 1f - speedAdjustmentPerTool * (float)(tc.tools.Count - 1);
		anim.Play("Close");
		float length = anim.GetCurrentAnimatorStateInfo(0).length + 0.05f;
		float remaining = length;
		if (remaining >= 0f)
		{
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		SetToggleState(tc.toggle.GetComponent<KToggle>(), false);
		tc.UIMenuDisplay.SetActive(false);
	}

	private void SetToggleState(KToggle toggle, bool state)
	{
		if (state)
		{
			toggle.Select();
			toggle.isOn = true;
		}
		else
		{
			toggle.Deselect();
			toggle.isOn = false;
		}
	}

	public void ClearSelection()
	{
		if (currentlySelectedCollection != null)
		{
			ChooseCollection(null, true);
		}
		if (currentlySelectedTool != null)
		{
			ChooseTool(null);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (e.IsAction(Action.ToggleSandboxTools))
			{
				if (Application.isEditor)
				{
					Output.Log("Force-enabling sandbox mode because we're in editor.");
					SaveGame.Instance.sandboxEnabled = true;
				}
				if (SaveGame.Instance.sandboxEnabled)
				{
					Game.Instance.SandboxModeActive = !Game.Instance.SandboxModeActive;
				}
			}
			foreach (ToolCollection[] row in rows)
			{
				if (row != rowSandboxTools || Game.Instance.SandboxModeActive)
				{
					for (int i = 0; i < row.Length; i++)
					{
						Action toolHotkey = row[i].hotkey;
						if (toolHotkey != Action.NumActions && e.IsAction(toolHotkey) && (currentlySelectedCollection == null || (currentlySelectedCollection != null && currentlySelectedCollection.tools.Find((ToolInfo t) => GameInputMapping.CompareActionKeyCodes(t.hotkey, toolHotkey)) == null)))
						{
							if (currentlySelectedCollection != row[i])
							{
								ChooseCollection(row[i], false);
								ChooseTool(row[i].tools[0]);
							}
							else if (currentlySelectedCollection.tools.Count > 1)
							{
								e.Consumed = true;
								ChooseCollection(null, true);
								ChooseTool(null);
								string sound = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound(), false);
								if (sound != null)
								{
									KMonoBehaviour.PlaySound(sound);
								}
							}
							break;
						}
						for (int j = 0; j < row[i].tools.Count; j++)
						{
							if ((currentlySelectedCollection == null && row[i].tools.Count == 1) || currentlySelectedCollection == row[i] || (currentlySelectedCollection != null && currentlySelectedCollection.tools.Count == 1 && row[i].tools.Count == 1))
							{
								Action hotkey = row[i].tools[j].hotkey;
								if (e.IsAction(hotkey) && e.TryConsume(hotkey))
								{
									if (row[i].tools.Count == 1 && currentlySelectedCollection != row[i])
									{
										ChooseCollection(row[i], false);
									}
									else if (currentlySelectedTool != row[i].tools[j])
									{
										ChooseTool(row[i].tools[j]);
									}
								}
								else if (GameInputMapping.CompareActionKeyCodes(e.GetAction(), hotkey))
								{
									e.Consumed = true;
								}
							}
						}
					}
				}
			}
			if ((currentlySelectedTool != null || currentlySelectedCollection != null) && !e.Consumed)
			{
				if (e.TryConsume(Action.Escape))
				{
					string sound2 = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound(), false);
					if (sound2 != null)
					{
						KMonoBehaviour.PlaySound(sound2);
					}
					if (currentlySelectedCollection != null)
					{
						ChooseCollection(null, true);
					}
					if (currentlySelectedTool != null)
					{
						ChooseTool(null);
					}
					SelectTool.Instance.Activate();
				}
			}
			else if (!PlayerController.Instance.IsUsingDefaultTool() && !e.Consumed && e.TryConsume(Action.Escape))
			{
				SelectTool.Instance.Activate();
			}
		}
		base.OnKeyDown(e);
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if ((currentlySelectedTool != null || currentlySelectedCollection != null) && !e.Consumed)
			{
				if (PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
				{
					string sound = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound(), false);
					if (sound != null)
					{
						KMonoBehaviour.PlaySound(sound);
					}
					if (currentlySelectedCollection != null)
					{
						ChooseCollection(null, true);
					}
					if (currentlySelectedTool != null)
					{
						ChooseTool(null);
					}
					SelectTool.Instance.Activate();
				}
			}
			else if (!PlayerController.Instance.IsUsingDefaultTool() && !e.Consumed && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
			{
				SelectTool.Instance.Activate();
				string sound2 = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound(), false);
				if (sound2 != null)
				{
					KMonoBehaviour.PlaySound(sound2);
				}
			}
		}
		base.OnKeyUp(e);
	}

	protected void BuildRowToggles(ToolCollection[] row)
	{
		for (int i = 0; i < row.Length; i++)
		{
			ToolCollection toolCollection = row[i];
			if (!((UnityEngine.Object)toolCollection.toggle == (UnityEngine.Object)null))
			{
				GameObject toggle = toolCollection.toggle;
				Sprite[] array = icons;
				foreach (Sprite sprite in array)
				{
					if ((UnityEngine.Object)sprite != (UnityEngine.Object)null && sprite.name == toolCollection.icon)
					{
						Image component = toggle.transform.Find("FG").GetComponent<Image>();
						component.sprite = sprite;
						break;
					}
				}
				Transform transform = toggle.transform.Find("Text");
				if ((UnityEngine.Object)transform != (UnityEngine.Object)null)
				{
					LocText component2 = transform.GetComponent<LocText>();
					if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
					{
						component2.text = toolCollection.text;
					}
				}
				ToolTip component3 = toggle.GetComponent<ToolTip>();
				if ((bool)component3)
				{
					if (row[i].tools.Count == 1)
					{
						string hotkeyString = GameUtil.GetHotkeyString(row[i].tools[0].hotkey);
						component3.AddMultiStringTooltip(row[i].tools[0].tooltip + " " + hotkeyString, ToggleToolTipTextStyleSetting);
					}
					else
					{
						string text = row[i].tooltip;
						if (row[i].hotkey != Action.NumActions)
						{
							text = text + " " + GameUtil.GetHotkeyString(row[i].hotkey);
						}
						component3.AddMultiStringTooltip(text, ToggleToolTipTextStyleSetting);
					}
				}
			}
		}
	}

	protected void BuildToolToggles(ToolCollection[] row)
	{
		foreach (ToolCollection toolCollection in row)
		{
			if (!((UnityEngine.Object)toolCollection.toggle == (UnityEngine.Object)null))
			{
				for (int j = 0; j < toolCollection.tools.Count; j++)
				{
					GameObject gameObject = toolCollection.tools[j].toggle.gameObject;
					Sprite[] array = icons;
					foreach (Sprite sprite in array)
					{
						if ((UnityEngine.Object)sprite != (UnityEngine.Object)null && sprite.name == toolCollection.tools[j].icon)
						{
							Image component = gameObject.transform.Find("FG").GetComponent<Image>();
							component.sprite = sprite;
							break;
						}
					}
					Transform transform = gameObject.transform.Find("Text");
					if ((UnityEngine.Object)transform != (UnityEngine.Object)null)
					{
						LocText component2 = transform.GetComponent<LocText>();
						if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
						{
							component2.text = toolCollection.tools[j].text;
						}
					}
					ToolTip component3 = gameObject.GetComponent<ToolTip>();
					if ((bool)component3)
					{
						string str = (toolCollection.tools.Count <= 1) ? GameUtil.GetHotkeyString(toolCollection.tools[j].hotkey) : (GameUtil.GetHotkeyString(toolCollection.hotkey) + "+ " + GameUtil.GetHotkeyString(toolCollection.tools[j].hotkey));
						component3.AddMultiStringTooltip(toolCollection.tools[j].tooltip + " " + str, ToggleToolTipTextStyleSetting);
					}
				}
			}
		}
	}

	public bool HasUniqueKeyBindings()
	{
		bool result = true;
		boundRootActions.Clear();
		foreach (ToolCollection[] row in rows)
		{
			ToolCollection[] array = row;
			foreach (ToolCollection toolCollection in array)
			{
				if (boundRootActions.Contains(toolCollection.hotkey))
				{
					result = false;
					break;
				}
				boundRootActions.Add(toolCollection.hotkey);
				boundSubgroupActions.Clear();
				foreach (ToolInfo tool in toolCollection.tools)
				{
					if (boundSubgroupActions.Contains(tool.hotkey))
					{
						result = false;
						break;
					}
					boundSubgroupActions.Add(tool.hotkey);
				}
			}
		}
		return result;
	}

	private void OnPriorityClicked(PrioritySetting priority)
	{
		priorityScreen.SetScreenPriority(priority, false);
	}
}
