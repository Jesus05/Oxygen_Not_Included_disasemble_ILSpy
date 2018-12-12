using Klei.AI;
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

	private struct DiseaseSortInfo
	{
		public float sortkey;

		public Disease disease;

		public DiseaseSortInfo(Disease d)
		{
			disease = d;
			sortkey = CalculateHUE(d.overlayColour);
		}
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

	private ToolParameterMenu toolParameterMenu;

	private HashedString currentMode = OverlayModes.None.ID;

	private List<GameObject> inactiveUnitObjs;

	private List<GameObject> activeUnitObjs;

	private List<GameObject> activeDiagrams = new List<GameObject>();

	private Dictionary<string, ToolParameterMenu.ToggleState> diseaseOverlayFilters = CreateDefaultFilters();

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
			name = name.Replace("NAME", "");
			for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
			{
				string description = overlayInfo.infoUnits[i].description;
				description = description.Replace(name, "");
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

	private void OnChamberChanged()
	{
		if (!(currentMode != OverlayModes.Rooms.ID))
		{
			SetLegend(OverlayModes.Rooms.ID, true);
		}
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
				if (overlayInfo.mode == OverlayModes.Disease.ID)
				{
					PopulateDiseaseLegend(overlayInfo);
				}
				else if (overlayInfo.mode == OverlayModes.Rooms.ID)
				{
					PopulateRoomsLegend(overlayInfo);
				}
			}
			else
			{
				PopulateOverlayInfoUnits(overlayInfo);
			}
		}
	}

	public void SetLegend(HashedString mode, bool refreshing = false)
	{
		if (!(currentMode == mode) || refreshing)
		{
			ClearLegend();
			OverlayInfo overlayInfo = overlayInfoList.Find((OverlayInfo ol) => ol.mode == mode);
			if (mode == OverlayModes.Temperature.ID)
			{
				int num = SimDebugView.Instance.temperatureThresholds.Length - 1;
				for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
				{
					overlayInfo.infoUnits[i].color = SimDebugView.Instance.temperatureThresholds[num - i].color;
					overlayInfo.infoUnits[i].tooltip = UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE;
					overlayInfo.infoUnits[i].tooltipFormatData = GameUtil.GetFormattedTemperature(SimDebugView.Instance.temperatureThresholds[num - i].value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false);
				}
			}
			else if (mode == OverlayModes.HeatFlow.ID)
			{
				overlayInfo.infoUnits[0].tooltip = UI.OVERLAYS.HEATFLOW.TOOLTIPS.HEATING;
				overlayInfo.infoUnits[1].tooltip = UI.OVERLAYS.HEATFLOW.TOOLTIPS.NEUTRAL;
				overlayInfo.infoUnits[2].tooltip = UI.OVERLAYS.HEATFLOW.TOOLTIPS.COOLING;
			}
			SetLegend(overlayInfo);
			currentMode = mode;
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

	public void ClearLegend()
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

	public OverlayInfo GetOverlayInfo(HashedString mode)
	{
		for (int i = 0; i < overlayInfoList.Count; i++)
		{
			if (overlayInfoList[i].mode == mode)
			{
				return overlayInfoList[i];
			}
		}
		return null;
	}

	private void PopulateOverlayInfoUnits(OverlayInfo overlayInfo)
	{
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

	private static float CalculateHUE(Color32 colour)
	{
		byte b = Math.Max(colour.r, Math.Max(colour.g, colour.b));
		byte b2 = Math.Min(colour.r, Math.Min(colour.g, colour.b));
		float result = 0f;
		int num = b - b2;
		if (num == 0)
		{
			result = 0f;
		}
		else if (b == colour.r)
		{
			result = (float)(colour.g - colour.b) / (float)num % 6f;
		}
		else if (b == colour.g)
		{
			result = (float)(colour.b - colour.r) / (float)num + 2f;
		}
		else if (b == colour.b)
		{
			result = (float)(colour.r - colour.g) / (float)num + 4f;
		}
		return result;
	}

	private void PopulateDiseaseLegend(OverlayInfo info)
	{
		if (info.infoUnits != null && info.infoUnits.Count > 0)
		{
			PopulateOverlayInfoUnits(info);
		}
		List<DiseaseSortInfo> list = new List<DiseaseSortInfo>();
		foreach (Disease resource in Db.Get().Diseases.resources)
		{
			list.Add(new DiseaseSortInfo(resource));
		}
		list.Sort((DiseaseSortInfo a, DiseaseSortInfo b) => a.sortkey.CompareTo(b.sortkey));
		foreach (DiseaseSortInfo item in list)
		{
			DiseaseSortInfo current2 = item;
			if (current2.disease.diseaseType == Disease.DiseaseType.Pathogen)
			{
				GameObject freeUnitObject = GetFreeUnitObject();
				Image component = freeUnitObject.transform.Find("Icon").GetComponent<Image>();
				component.gameObject.SetActive(true);
				component.sprite = Assets.instance.LegendColourBox;
				component.color = current2.disease.overlayColour;
				component.enabled = true;
				component.type = Image.Type.Simple;
				LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
				componentInChildren.text = current2.disease.Name;
				componentInChildren.color = Color.white;
				componentInChildren.enabled = true;
				ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
				component2.enabled = true;
				component2.toolTip = current2.disease.overlayLegendHovertext.ToString();
				freeUnitObject.SetActive(true);
				freeUnitObject.transform.SetParent(activeUnitsParent.transform);
			}
		}
		GameObject gameObject = Util.KInstantiateUI(toolParameterMenuPrefab, diagramsParent, false);
		activeDiagrams.Add(gameObject);
		diagramsParent.SetActive(true);
		toolParameterMenu = gameObject.GetComponent<ToolParameterMenu>();
		toolParameterMenu.PopulateMenu(diseaseOverlayFilters);
		toolParameterMenu.onParametersChanged += OnDiseaseFiltersChanged;
		OnDiseaseFiltersChanged();
	}

	public void DisableDiseaseOverlay()
	{
		toolParameterMenu.onParametersChanged -= OnDiseaseFiltersChanged;
		toolParameterMenu.ClearMenu();
		toolParameterMenu.gameObject.SetActive(false);
		toolParameterMenu = null;
	}

	private bool InFilter(string layer, Dictionary<string, ToolParameterMenu.ToggleState> filter)
	{
		return (filter.ContainsKey(ToolParameterMenu.FILTERLAYERS.ALL) && filter[ToolParameterMenu.FILTERLAYERS.ALL] == ToolParameterMenu.ToggleState.On) || (filter.ContainsKey(layer) && filter[layer] == ToolParameterMenu.ToggleState.On);
	}

	private static Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters()
	{
		Dictionary<string, ToolParameterMenu.ToggleState> dictionary = new Dictionary<string, ToolParameterMenu.ToggleState>();
		dictionary.Add(ToolParameterMenu.FILTERLAYERS.ALL, ToolParameterMenu.ToggleState.On);
		dictionary.Add(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, ToolParameterMenu.ToggleState.Off);
		dictionary.Add(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, ToolParameterMenu.ToggleState.Off);
		return dictionary;
	}

	private void OnDiseaseFiltersChanged()
	{
		Game.Instance.showGasConduitDisease = InFilter(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, diseaseOverlayFilters);
		Game.Instance.showLiquidConduitDisease = InFilter(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, diseaseOverlayFilters);
		Game.Instance.ForceOverlayUpdate();
	}

	private void PopulateNoiseLegend(OverlayInfo info)
	{
		if (info.infoUnits != null && info.infoUnits.Count > 0)
		{
			PopulateOverlayInfoUnits(info);
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

	private void PopulateRoomsLegend(OverlayInfo info)
	{
		for (int i = 0; i < Db.Get().RoomTypes.Count; i++)
		{
			RoomType roomType = Db.Get().RoomTypes[i];
			GameObject freeUnitObject = GetFreeUnitObject();
			LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
			componentInChildren.enabled = true;
			componentInChildren.text = roomType.Name + "\n" + roomType.effect;
			Image component = freeUnitObject.transform.Find("Icon").GetComponent<Image>();
			component.gameObject.SetActive(true);
			component.sprite = Assets.instance.LegendColourBox;
			component.color = roomType.category.color;
			component.enabled = true;
			component.type = Image.Type.Simple;
			ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
			component2.enabled = true;
			component2.ClearMultiStringTooltip();
			component2.AddMultiStringTooltip(roomType.GetCriteriaString(), null);
			if (roomType.effects != null && roomType.effects.Length > 0)
			{
				component2.AddMultiStringTooltip(roomType.GetRoomEffectsString(), null);
			}
			freeUnitObject.SetActive(true);
			freeUnitObject.transform.SetParent(activeUnitsParent.transform);
		}
	}
}
