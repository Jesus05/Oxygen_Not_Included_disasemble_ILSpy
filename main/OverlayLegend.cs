using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayLegend : KScreen
{
	[Serializable]
	public class OverlayInfoUnit
	{
		public Sprite icon;

		public string description;

		public string tooltip;

		public Color color;

		public Color fontColor;

		public object formatData;

		public object tooltipFormatData;

		public bool sliceIcon;

		public OverlayInfoUnit(Sprite icon, string description, Color color, Color fontColor, object formatData = null, bool sliceIcon = false)
		{
			this.icon = icon;
			this.description = description;
			this.color = color;
			this.fontColor = fontColor;
			this.formatData = formatData;
			this.sliceIcon = sliceIcon;
		}
	}

	[Serializable]
	public class OverlayInfo
	{
		public string name;

		public HashedString mode;

		public List<OverlayInfoUnit> infoUnits;

		public List<GameObject> diagrams;

		public bool isProgrammaticallyPopulated;
	}

	public static OverlayLegend Instance;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private Sprite emptySprite;

	[SerializeField]
	private List<OverlayInfo> overlayInfoList;

	[SerializeField]
	private GameObject unitPrefab;

	[SerializeField]
	private GameObject activeUnitsParent;

	[SerializeField]
	private GameObject diagramsParent;

	[SerializeField]
	private GameObject inactiveUnitsParent;

	[SerializeField]
	private GameObject toolParameterMenuPrefab;

	private ToolParameterMenu filterMenu;

	private OverlayModes.Mode currentMode;

	private List<GameObject> inactiveUnitObjs;

	private List<GameObject> activeUnitObjs;

	private List<GameObject> activeDiagrams = new List<GameObject>();

	[ContextMenu("Set all fonts color")]
	public void SetAllFontsColor()
	{
		foreach (OverlayInfo overlayInfo in overlayInfoList)
		{
			for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
			{
				if (overlayInfo.infoUnits[i].fontColor == Color.clear)
				{
					overlayInfo.infoUnits[i].fontColor = Color.white;
				}
			}
		}
	}

	[ContextMenu("Set all tooltips")]
	public void SetAllTooltips()
	{
		foreach (OverlayInfo overlayInfo in overlayInfoList)
		{
			string name = overlayInfo.name;
			name = name.Replace("NAME", string.Empty);
			for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
			{
				string description = overlayInfo.infoUnits[i].description;
				description = description.Replace(name, string.Empty);
				description = name + "TOOLTIPS." + description;
				overlayInfo.infoUnits[i].tooltip = description;
			}
		}
	}

	[ContextMenu("Set Sliced for empty icons")]
	public void SetSlicedForEmptyIcons()
	{
		foreach (OverlayInfo overlayInfo in overlayInfoList)
		{
			for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
			{
				if ((UnityEngine.Object)overlayInfo.infoUnits[i].icon == (UnityEngine.Object)emptySprite)
				{
					overlayInfo.infoUnits[i].sliceIcon = true;
				}
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if ((UnityEngine.Object)Instance == (UnityEngine.Object)null)
		{
			Instance = this;
			activeUnitObjs = new List<GameObject>();
			inactiveUnitObjs = new List<GameObject>();
			foreach (OverlayInfo overlayInfo in overlayInfoList)
			{
				overlayInfo.name = Strings.Get(overlayInfo.name);
				for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
				{
					overlayInfo.infoUnits[i].description = Strings.Get(overlayInfo.infoUnits[i].description);
					if (!string.IsNullOrEmpty(overlayInfo.infoUnits[i].tooltip))
					{
						overlayInfo.infoUnits[i].tooltip = Strings.Get(overlayInfo.infoUnits[i].tooltip);
					}
				}
			}
			ClearLegend();
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	protected override void OnLoadLevel()
	{
		Instance = null;
		activeDiagrams.Clear();
		UnityEngine.Object.Destroy(base.gameObject);
		base.OnLoadLevel();
	}

	private void SetLegend(OverlayInfo overlayInfo)
	{
		if (overlayInfo == null)
		{
			ClearLegend();
		}
		else if (!overlayInfo.isProgrammaticallyPopulated && (overlayInfo.infoUnits == null || overlayInfo.infoUnits.Count == 0))
		{
			ClearLegend();
		}
		else
		{
			Show(true);
			title.text = overlayInfo.name;
			if (overlayInfo.isProgrammaticallyPopulated)
			{
				PopulateGeneratedLegend(overlayInfo, false);
			}
			else
			{
				PopulateOverlayInfoUnits(overlayInfo, false);
			}
		}
	}

	public void SetLegend(OverlayModes.Mode mode, bool refreshing = false)
	{
		if (currentMode == null || !(currentMode.ViewMode() == mode.ViewMode()) || refreshing)
		{
			ClearLegend();
			OverlayInfo legend = overlayInfoList.Find((OverlayInfo ol) => ol.mode == mode.ViewMode());
			currentMode = mode;
			SetLegend(legend);
		}
	}

	public GameObject GetFreeUnitObject()
	{
		GameObject gameObject = null;
		if (inactiveUnitObjs.Count == 0)
		{
			inactiveUnitObjs.Add(Util.KInstantiateUI(unitPrefab, inactiveUnitsParent, false));
		}
		gameObject = inactiveUnitObjs[0];
		inactiveUnitObjs.RemoveAt(0);
		activeUnitObjs.Add(gameObject);
		return gameObject;
	}

	private void RemoveActiveObjects()
	{
		while (activeUnitObjs.Count > 0)
		{
			activeUnitObjs[0].transform.Find("Icon").GetComponent<Image>().enabled = false;
			activeUnitObjs[0].GetComponentInChildren<LocText>().enabled = false;
			activeUnitObjs[0].transform.SetParent(inactiveUnitsParent.transform);
			activeUnitObjs[0].SetActive(false);
			inactiveUnitObjs.Add(activeUnitObjs[0]);
			activeUnitObjs.RemoveAt(0);
		}
	}

	public void ClearLegend()
	{
		RemoveActiveObjects();
		for (int i = 0; i < activeDiagrams.Count; i++)
		{
			if ((UnityEngine.Object)activeDiagrams[i] != (UnityEngine.Object)null)
			{
				UnityEngine.Object.Destroy(activeDiagrams[i]);
			}
		}
		activeDiagrams.Clear();
		Vector2 sizeDelta = diagramsParent.GetComponent<RectTransform>().sizeDelta;
		sizeDelta.y = 0f;
		diagramsParent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		Show(false);
	}

	public OverlayInfo GetOverlayInfo(OverlayModes.Mode mode)
	{
		for (int i = 0; i < overlayInfoList.Count; i++)
		{
			if (overlayInfoList[i].mode == mode.ViewMode())
			{
				return overlayInfoList[i];
			}
		}
		return null;
	}

	private void PopulateOverlayInfoUnits(OverlayInfo overlayInfo, bool isRefresh = false)
	{
		if (overlayInfo.infoUnits != null && overlayInfo.infoUnits.Count > 0)
		{
			activeUnitsParent.SetActive(true);
			foreach (OverlayInfoUnit infoUnit in overlayInfo.infoUnits)
			{
				GameObject freeUnitObject = GetFreeUnitObject();
				if ((UnityEngine.Object)infoUnit.icon != (UnityEngine.Object)null)
				{
					Image component = freeUnitObject.transform.Find("Icon").GetComponent<Image>();
					component.gameObject.SetActive(true);
					component.sprite = infoUnit.icon;
					component.color = infoUnit.color;
					component.enabled = true;
					component.type = (infoUnit.sliceIcon ? Image.Type.Sliced : Image.Type.Simple);
				}
				else
				{
					freeUnitObject.transform.Find("Icon").gameObject.SetActive(false);
				}
				if (!string.IsNullOrEmpty(infoUnit.description))
				{
					LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
					componentInChildren.text = string.Format(infoUnit.description, infoUnit.formatData);
					componentInChildren.color = infoUnit.fontColor;
					componentInChildren.enabled = true;
				}
				ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
				if (!string.IsNullOrEmpty(infoUnit.tooltip))
				{
					component2.toolTip = string.Format(infoUnit.tooltip, infoUnit.tooltipFormatData);
					component2.enabled = true;
				}
				else
				{
					component2.enabled = false;
				}
				freeUnitObject.SetActive(true);
				freeUnitObject.transform.SetParent(activeUnitsParent.transform);
			}
		}
		else
		{
			activeUnitsParent.SetActive(false);
		}
		if (!isRefresh)
		{
			if (overlayInfo.diagrams != null && overlayInfo.diagrams.Count > 0)
			{
				diagramsParent.SetActive(true);
				foreach (GameObject diagram in overlayInfo.diagrams)
				{
					GameObject item = Util.KInstantiateUI(diagram, diagramsParent, false);
					activeDiagrams.Add(item);
				}
			}
			else
			{
				diagramsParent.SetActive(false);
			}
		}
	}

	private void PopulateGeneratedLegend(OverlayInfo info, bool isRefresh = false)
	{
		if (isRefresh)
		{
			RemoveActiveObjects();
		}
		if (info.infoUnits != null && info.infoUnits.Count > 0)
		{
			PopulateOverlayInfoUnits(info, isRefresh);
		}
		List<LegendEntry> customLegendData = currentMode.GetCustomLegendData();
		if (customLegendData != null)
		{
			activeUnitsParent.SetActive(true);
			foreach (LegendEntry item in customLegendData)
			{
				GameObject freeUnitObject = GetFreeUnitObject();
				Image component = freeUnitObject.transform.Find("Icon").GetComponent<Image>();
				component.gameObject.SetActive(true);
				component.sprite = Assets.instance.LegendColourBox;
				component.color = item.colour;
				component.enabled = true;
				component.type = Image.Type.Simple;
				LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
				componentInChildren.text = item.name;
				componentInChildren.color = Color.white;
				componentInChildren.enabled = true;
				ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
				component2.enabled = true;
				component2.toolTip = item.desc;
				freeUnitObject.SetActive(true);
				freeUnitObject.transform.SetParent(activeUnitsParent.transform);
			}
		}
		else
		{
			activeUnitsParent.SetActive(false);
		}
		if (!isRefresh && currentMode.legendFilters != null)
		{
			GameObject gameObject = Util.KInstantiateUI(toolParameterMenuPrefab, diagramsParent, false);
			activeDiagrams.Add(gameObject);
			diagramsParent.SetActive(true);
			filterMenu = gameObject.GetComponent<ToolParameterMenu>();
			filterMenu.PopulateMenu(currentMode.legendFilters);
			filterMenu.onParametersChanged += OnFiltersChanged;
			OnFiltersChanged();
		}
	}

	private void OnFiltersChanged()
	{
		currentMode.OnFiltersChanged();
		PopulateGeneratedLegend(GetOverlayInfo(currentMode), true);
		Game.Instance.ForceOverlayUpdate();
	}

	private void DisableOverlay()
	{
		filterMenu.onParametersChanged -= OnFiltersChanged;
		filterMenu.ClearMenu();
		filterMenu.gameObject.SetActive(false);
		filterMenu = null;
	}

	private void PopulateNoiseLegend(OverlayInfo info)
	{
		if (info.infoUnits != null && info.infoUnits.Count > 0)
		{
			PopulateOverlayInfoUnits(info, false);
		}
		string[] names = Enum.GetNames(typeof(AudioEventManager.NoiseEffect));
		Array values = Enum.GetValues(typeof(AudioEventManager.NoiseEffect));
		Color[] dbColours = SimDebugView.dbColours;
		for (int i = 0; i < names.Length; i++)
		{
			GameObject freeUnitObject = GetFreeUnitObject();
			Image component = freeUnitObject.transform.Find("Icon").GetComponent<Image>();
			component.gameObject.SetActive(true);
			component.sprite = Assets.instance.LegendColourBox;
			component.color = ((i != 0) ? Color.Lerp(dbColours[i * 2], dbColours[Mathf.Min(dbColours.Length - 1, i * 2 + 1)], 0.5f) : new Color(1f, 1f, 1f, 0.7f));
			component.enabled = true;
			component.type = Image.Type.Simple;
			string str = names[i].ToUpper();
			int num = (int)values.GetValue(i);
			int num2 = (int)values.GetValue(i);
			LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
			componentInChildren.text = Strings.Get("STRINGS.UI.OVERLAYS.NOISE_POLLUTION.NAMES." + str) + " " + string.Format(UI.OVERLAYS.NOISE_POLLUTION.RANGE, num);
			componentInChildren.color = Color.white;
			componentInChildren.enabled = true;
			ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
			component2.enabled = true;
			component2.toolTip = string.Format(Strings.Get("STRINGS.UI.OVERLAYS.NOISE_POLLUTION.TOOLTIPS." + str), num, num2);
			freeUnitObject.SetActive(true);
			freeUnitObject.transform.SetParent(activeUnitsParent.transform);
		}
	}
}
