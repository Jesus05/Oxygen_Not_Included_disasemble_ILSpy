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

		public HashedString viewMode;

		public bool forceViewMode;

		public KToggle toggle;

		public Action<object> onSelectCallback;

		public object toolData;

		public ToolInfo(string text, string icon_name, Action hotkey, string ToolName, ToolCollection toolCollection, string tooltip = "", bool forceViewMode = false, Action<object> onSelectCallback = null, object toolData = null)
		{
			this.text = text;
			icon = icon_name;
			this.hotkey = hotkey;
			toolName = ToolName;
			collection = toolCollection;
			toolCollection.tools.Add(this);
			this.tooltip = tooltip;
			this.forceViewMode = forceViewMode;
			this.onSelectCallback = onSelectCallback;
			this.toolData = toolData;
			viewMode = OverlayModes.None.ID;
		}
	}

	public class ToolCollection
	{
		public string text;

		public string icon;

		public string tooltip;

		public bool useInfoMenu;

		public bool largeIcon;

		public GameObject toggle;

		public List<ToolInfo> tools = new List<ToolInfo>();

		public GameObject UIMenuDisplay;

		public GameObject MaskContainer;

		public Action hotkey;

		public ToolCollection(string text, string icon_name, string tooltip = "", bool useInfoMenu = false, Action hotkey = Action.NumActions, bool largeIcon = false)
		{
			this.text = text;
			icon = icon_name;
			this.tooltip = tooltip;
			this.useInfoMenu = useInfoMenu;
			this.hotkey = hotkey;
			this.largeIcon = largeIcon;
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

	public GameObject toolIconLargePrefab;

	public GameObject sandboxToolIconPrefab;

	public GameObject collectionIconPrefab;

	public GameObject prefabToolRow;

	public GameObject largeToolSet;

	public GameObject smallToolSet;

	public GameObject smallToolBottomRow;

	public GameObject smallToolTopRow;

	public GameObject sandboxToolSet;

	[SerializeField]
	private List<Sprite> icons = new List<Sprite>();

	private PriorityScreen priorityScreen;

	public ToolParameterMenu toolParameterMenu;

	public GameObject sandboxToolParameterMenu;

	private GameObject toolEffectDisplayPlane;

	private Texture2D toolEffectDisplayPlaneTexture;

	public Material toolEffectDisplayMaterial;

	private byte[] toolEffectDisplayBytes;

	private List<List<ToolCollection>> rows = new List<List<ToolCollection>>();

	public List<ToolCollection> basicTools = new List<ToolCollection>();

	public List<ToolCollection> sandboxTools = new List<ToolCollection>();

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
		CreateSandBoxTools();
		CreateBasicTools();
		rows.Add(sandboxTools);
		rows.Add(basicTools);
		rows.ForEach(delegate(List<ToolCollection> row)
		{
			InstantiateCollectionsUI(row);
		});
		rows.ForEach(delegate(List<ToolCollection> row)
		{
			BuildRowToggles(row);
		});
		rows.ForEach(delegate(List<ToolCollection> row)
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
		sandboxTools[0].toggle.transform.parent.transform.parent.gameObject.SetActive(Game.Instance.SandboxModeActive);
	}

	public static ToolCollection CreateToolCollection(LocString collection_name, string icon_name, Action hotkey, string tool_name, LocString tooltip, bool largeIcon)
	{
		string text = collection_name;
		bool largeIcon2 = largeIcon;
		ToolCollection toolCollection = new ToolCollection(text, icon_name, string.Empty, false, Action.NumActions, largeIcon2);
		new ToolInfo(collection_name, icon_name, hotkey, tool_name, toolCollection, tooltip, false, null, null);
		return toolCollection;
	}

	private void CreateSandBoxTools()
	{
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.BRUSH.NAME, "brush", Action.SandboxBrush, "SandboxBrushTool", UI.SANDBOXTOOLS.SETTINGS.BRUSH.TOOLTIP, false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.SPRINKLE.NAME, "sprinkle", Action.SandboxSprinkle, "SandboxSprinkleTool", UI.SANDBOXTOOLS.SETTINGS.SPRINKLE.TOOLTIP, false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.FLOOD.NAME, "flood", Action.SandboxFlood, "SandboxFloodTool", UI.SANDBOXTOOLS.SETTINGS.FLOOD.TOOLTIP, false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.SAMPLE.NAME, "sample", Action.SandboxSample, "SandboxSampleTool", UI.SANDBOXTOOLS.SETTINGS.SAMPLE.TOOLTIP, false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.HEATGUN.NAME, "brush", Action.SandboxHeatGun, "SandboxHeatTool", UI.SANDBOXTOOLS.SETTINGS.HEATGUN.TOOLTIP, false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.SPAWNER.NAME, "spawn", Action.SandboxSpawnEntity, "SandboxSpawnerTool", UI.SANDBOXTOOLS.SETTINGS.SPAWNER.TOOLTIP, false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.CLEAR_FLOOR.NAME, "clear_floor", Action.SandboxClearFloor, "SandboxClearFloorTool", UI.SANDBOXTOOLS.SETTINGS.CLEAR_FLOOR.TOOLTIP, false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.DESTROY.NAME, "destroy", Action.SandboxDestroy, "SandboxDestroyerTool", UI.SANDBOXTOOLS.SETTINGS.DESTROY.TOOLTIP, false));
		sandboxTools.Add(CreateToolCollection(UI.TOOLS.SANDBOX.FOW.NAME, "brush", Action.SandboxReveal, "SandboxFOWTool", UI.SANDBOXTOOLS.SETTINGS.FOW.TOOLTIP, false));
	}

	private void CreateBasicTools()
	{
		basicTools.Add(CreateToolCollection(UI.TOOLS.DIG.NAME, "icon_action_dig", Action.Dig, "DigTool", UI.TOOLTIPS.DIGBUTTON, true));
		basicTools.Add(CreateToolCollection(UI.TOOLS.CANCEL.NAME, "icon_action_cancel", Action.BuildingCancel, "CancelTool", UI.TOOLTIPS.CANCELBUTTON, true));
		basicTools.Add(CreateToolCollection(UI.TOOLS.DECONSTRUCT.NAME, "icon_action_deconstruct", Action.BuildingDeconstruct, "DeconstructTool", UI.TOOLTIPS.DECONSTRUCTBUTTON, true));
		basicTools.Add(CreateToolCollection(UI.TOOLS.PRIORITIZE.NAME, "icon_action_prioritize", Action.Prioritize, "PrioritizeTool", UI.TOOLTIPS.PRIORITIZEBUTTON, true));
		basicTools.Add(CreateToolCollection(UI.TOOLS.DISINFECT.NAME, "icon_action_disinfect", Action.Disinfect, "DisinfectTool", UI.TOOLTIPS.DISINFECTBUTTON, false));
		basicTools.Add(CreateToolCollection(UI.TOOLS.MARKFORSTORAGE.NAME, "icon_action_store", Action.Clear, "ClearTool", UI.TOOLTIPS.CLEARBUTTON, false));
		basicTools.Add(CreateToolCollection(UI.TOOLS.ATTACK.NAME, "icon_action_attack", Action.Attack, "AttackTool", UI.TOOLTIPS.ATTACKBUTTON, false));
		basicTools.Add(CreateToolCollection(UI.TOOLS.MOP.NAME, "icon_action_mop", Action.Mop, "MopTool", UI.TOOLTIPS.MOPBUTTON, false));
		basicTools.Add(CreateToolCollection(UI.TOOLS.CAPTURE.NAME, "icon_action_capture", Action.Capture, "CaptureTool", UI.TOOLTIPS.CAPTUREBUTTON, false));
		basicTools.Add(CreateToolCollection(UI.TOOLS.HARVEST.NAME, "icon_action_harvest", Action.Harvest, "HarvestTool", UI.TOOLTIPS.HARVESTBUTTON, false));
		basicTools.Add(CreateToolCollection(UI.TOOLS.EMPTY_PIPE.NAME, "icon_action_empty_pipes", Action.EmptyPipe, "EmptyPipeTool", UI.TOOLS.EMPTY_PIPE.TOOLTIP, false));
	}

	private void InstantiateCollectionsUI(IList<ToolCollection> collections)
	{
		GameObject parent = Util.KInstantiateUI(prefabToolRow, base.gameObject, true);
		GameObject gameObject = Util.KInstantiateUI(largeToolSet, parent, true);
		GameObject gameObject2 = Util.KInstantiateUI(smallToolSet, parent, true);
		GameObject gameObject3 = Util.KInstantiateUI(smallToolBottomRow, gameObject2, true);
		GameObject gameObject4 = Util.KInstantiateUI(smallToolTopRow, gameObject2, true);
		GameObject gameObject5 = Util.KInstantiateUI(sandboxToolSet, parent, true);
		bool flag = true;
		for (int i = 0; i < collections.Count; i++)
		{
			GameObject parent2;
			if (collections == sandboxTools)
			{
				parent2 = gameObject5;
			}
			else if (collections[i].largeIcon)
			{
				parent2 = gameObject;
			}
			else
			{
				parent2 = ((!flag) ? gameObject3 : gameObject4);
				flag = !flag;
			}
			ToolCollection tc = collections[i];
			tc.toggle = Util.KInstantiateUI((collections[i].tools.Count > 1) ? collectionIconPrefab : ((collections == sandboxTools) ? sandboxToolIconPrefab : ((!collections[i].largeIcon) ? toolIconPrefab : toolIconLargePrefab)), parent2, true);
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
				GameObject gameObject6 = null;
				if (tc.tools.Count < smallCollectionMax)
				{
					gameObject6 = Util.KInstantiateUI(Prefab_collectionContainer, parent2, true);
					gameObject6.transform.SetSiblingIndex(gameObject6.transform.GetSiblingIndex() - 1);
					gameObject6.transform.localScale = Vector3.one;
					gameObject6.rectTransform().sizeDelta = new Vector2((float)(tc.tools.Count * 75), 50f);
					tc.MaskContainer = gameObject6.GetComponentInChildren<Mask>().gameObject;
					gameObject6.SetActive(false);
				}
				else
				{
					gameObject6 = Util.KInstantiateUI(Prefab_collectionContainerWindow, parent2, true);
					gameObject6.transform.localScale = Vector3.one;
					gameObject6.GetComponentInChildren<LocText>().SetText(tc.text.ToUpper());
					tc.MaskContainer = gameObject6.GetComponentInChildren<GridLayoutGroup>().gameObject;
					gameObject6.SetActive(false);
				}
				tc.UIMenuDisplay = gameObject6;
				for (int j = 0; j < tc.tools.Count; j++)
				{
					ToolInfo ti = tc.tools[j];
					GameObject gameObject7 = Util.KInstantiateUI((collections == sandboxTools) ? sandboxToolIconPrefab : ((!collections[i].largeIcon) ? toolIconPrefab : toolIconLargePrefab), tc.MaskContainer, true);
					gameObject7.name = ti.text;
					ti.toggle = gameObject7.GetComponent<KToggle>();
					if (ti.collection.tools.Count > 1)
					{
						RectTransform rectTransform = null;
						rectTransform = ti.toggle.gameObject.GetComponentInChildren<SetTextStyleSetting>().rectTransform();
						if (gameObject7.name.Length > 12)
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
		if (gameObject.transform.childCount == 0)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		if (gameObject3.transform.childCount == 0 && gameObject4.transform.childCount == 0)
		{
			UnityEngine.Object.Destroy(gameObject2);
		}
		if (gameObject5.transform.childCount == 0)
		{
			UnityEngine.Object.Destroy(gameObject5);
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
			rows.ForEach(delegate(List<ToolCollection> row)
			{
				RefreshRowDisplay(row);
			});
		}
	}

	private void RefreshRowDisplay(IList<ToolCollection> row)
	{
		for (int i = 0; i < row.Count; i++)
		{
			ToolCollection tc = row[i];
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
		rows.ForEach(delegate(List<ToolCollection> row)
		{
			OpenOrCloseCollectionsInRow(row, true);
		});
	}

	private void OpenOrCloseCollectionsInRow(IList<ToolCollection> row, bool autoSelectTool = true)
	{
		for (int i = 0; i < row.Count; i++)
		{
			ToolCollection tc = row[i];
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
			foreach (List<ToolCollection> row in rows)
			{
				if (row != sandboxTools || Game.Instance.SandboxModeActive)
				{
					for (int i = 0; i < row.Count; i++)
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

	protected void BuildRowToggles(IList<ToolCollection> row)
	{
		for (int i = 0; i < row.Count; i++)
		{
			ToolCollection toolCollection = row[i];
			if (!((UnityEngine.Object)toolCollection.toggle == (UnityEngine.Object)null))
			{
				GameObject toggle = toolCollection.toggle;
				foreach (Sprite icon in icons)
				{
					if ((UnityEngine.Object)icon != (UnityEngine.Object)null && icon.name == toolCollection.icon)
					{
						Image component = toggle.transform.Find("FG").GetComponent<Image>();
						component.sprite = icon;
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
						string newString = GameUtil.ReplaceHotkeyString(row[i].tools[0].tooltip, row[i].tools[0].hotkey);
						component3.AddMultiStringTooltip(newString, ToggleToolTipTextStyleSetting);
					}
					else
					{
						string text = row[i].tooltip;
						if (row[i].hotkey != Action.NumActions)
						{
							text = GameUtil.ReplaceHotkeyString(text, row[i].hotkey);
						}
						component3.AddMultiStringTooltip(text, ToggleToolTipTextStyleSetting);
					}
				}
			}
		}
	}

	protected void BuildToolToggles(IList<ToolCollection> row)
	{
		for (int i = 0; i < row.Count; i++)
		{
			ToolCollection toolCollection = row[i];
			if (!((UnityEngine.Object)toolCollection.toggle == (UnityEngine.Object)null))
			{
				for (int j = 0; j < toolCollection.tools.Count; j++)
				{
					GameObject gameObject = toolCollection.tools[j].toggle.gameObject;
					foreach (Sprite icon in icons)
					{
						if ((UnityEngine.Object)icon != (UnityEngine.Object)null && icon.name == toolCollection.tools[j].icon)
						{
							Image component = gameObject.transform.Find("FG").GetComponent<Image>();
							component.sprite = icon;
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
						string newString = (toolCollection.tools.Count <= 1) ? GameUtil.ReplaceHotkeyString(toolCollection.tools[j].tooltip, toolCollection.tools[j].hotkey) : GameUtil.ReplaceHotkeyString(toolCollection.tools[j].tooltip, toolCollection.hotkey, toolCollection.tools[j].hotkey);
						component3.AddMultiStringTooltip(newString, ToggleToolTipTextStyleSetting);
					}
				}
			}
		}
	}

	public bool HasUniqueKeyBindings()
	{
		bool result = true;
		boundRootActions.Clear();
		foreach (List<ToolCollection> row in rows)
		{
			foreach (ToolCollection item in row)
			{
				if (boundRootActions.Contains(item.hotkey))
				{
					result = false;
					break;
				}
				boundRootActions.Add(item.hotkey);
				boundSubgroupActions.Clear();
				foreach (ToolInfo tool in item.tools)
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
