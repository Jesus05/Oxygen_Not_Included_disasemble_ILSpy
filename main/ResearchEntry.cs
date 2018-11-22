using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ResearchEntry : KMonoBehaviour
{
	[Header("Labels")]
	[SerializeField]
	private LocText researchName;

	[Header("Transforms")]
	[SerializeField]
	private Transform progressBarContainer;

	[SerializeField]
	private Transform lineContainer;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject iconPanel;

	[SerializeField]
	private GameObject iconPrefab;

	[SerializeField]
	private GameObject linePrefab;

	[SerializeField]
	private GameObject progressBarPrefab;

	[Header("Graphics")]
	[SerializeField]
	private Image BG;

	[SerializeField]
	private Image titleBG;

	[SerializeField]
	private Image borderHighlight;

	[SerializeField]
	private Image filterHighlight;

	[SerializeField]
	private Image filterLowlight;

	[SerializeField]
	private Sprite hoverBG;

	[SerializeField]
	private Sprite completedBG;

	[Header("Colors")]
	[SerializeField]
	private Color defaultColor = Color.blue;

	[SerializeField]
	private Color completedColor = Color.yellow;

	[SerializeField]
	private Color pendingColor = Color.magenta;

	[SerializeField]
	private Color completedHeaderColor = Color.grey;

	[SerializeField]
	private Color incompleteHeaderColor = Color.grey;

	[SerializeField]
	private Color pendingHeaderColor = Color.grey;

	private Sprite defaultBG;

	[MyCmpGet]
	private KToggle toggle;

	private ResearchScreen researchScreen;

	private Dictionary<Tech, UILineRenderer> techLineMap;

	private Tech targetTech;

	private bool isOn = true;

	private Coroutine fadeRoutine;

	public Color activeLineColor;

	public Color inactiveLineColor;

	public int lineThickness_active = 6;

	public int lineThickness_inactive = 2;

	public Material StandardUIMaterial;

	public Material DesaturatedUIMaterial;

	private Dictionary<string, GameObject> progressBarsByResearchTypeID = new Dictionary<string, GameObject>();

	public static readonly string UnlockedTechKey = "UnlockedTech";

	private Dictionary<string, object> unlockedTechMetric = new Dictionary<string, object>
	{
		{
			UnlockedTechKey,
			null
		}
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		techLineMap = new Dictionary<Tech, UILineRenderer>();
		BG.color = defaultColor;
		foreach (Tech item in targetTech.requiredTech)
		{
			float num = targetTech.width / 2f;
			Vector2 b = Vector2.zero;
			Vector2 b2 = Vector2.zero;
			Vector2 center = item.center;
			float y = center.y;
			Vector2 center2 = targetTech.center;
			if (y > center2.y + 2f)
			{
				b = new Vector2(0f, 20f);
				b2 = new Vector2(0f, -20f);
			}
			else
			{
				Vector2 center3 = item.center;
				float y2 = center3.y;
				Vector2 center4 = targetTech.center;
				if (y2 < center4.y - 2f)
				{
					b = new Vector2(0f, -20f);
					b2 = new Vector2(0f, 20f);
				}
			}
			GameObject gameObject = Util.KInstantiateUI(linePrefab, lineContainer.gameObject, true);
			UILineRenderer component = gameObject.GetComponent<UILineRenderer>();
			UILineRenderer uILineRenderer = component;
			Vector2[] obj = new Vector2[4]
			{
				new Vector2(0f, 0f) + b,
				default(Vector2),
				default(Vector2),
				default(Vector2)
			};
			ref Vector2 reference = ref obj[1];
			Vector2 center5 = targetTech.center;
			float num2 = center5.x - num;
			Vector2 center6 = item.center;
			reference = new Vector2(0f - (num2 - (center6.x + num)) / 2f, 0f) + b;
			ref Vector2 reference2 = ref obj[2];
			Vector2 center7 = targetTech.center;
			float num3 = center7.x - num;
			Vector2 center8 = item.center;
			float x = 0f - (num3 - (center8.x + num)) / 2f;
			Vector2 center9 = item.center;
			float y3 = center9.y;
			Vector2 center10 = targetTech.center;
			reference2 = new Vector2(x, y3 - center10.y) + b2;
			ref Vector2 reference3 = ref obj[3];
			Vector2 center11 = targetTech.center;
			float num4 = center11.x - num;
			Vector2 center12 = item.center;
			float x2 = 0f - (num4 - (center12.x + num)) + 2f;
			Vector2 center13 = item.center;
			float y4 = center13.y;
			Vector2 center14 = targetTech.center;
			reference3 = new Vector2(x2, y4 - center14.y) + b2;
			uILineRenderer.Points = obj;
			component.LineThickness = (float)lineThickness_inactive;
			component.color = inactiveLineColor;
			techLineMap.Add(item, component);
		}
		QueueStateChanged(false);
		if (targetTech != null)
		{
			foreach (TechInstance item2 in Research.Instance.GetResearchQueue())
			{
				if (item2.tech == targetTech)
				{
					QueueStateChanged(true);
				}
			}
		}
	}

	public void SetTech(Tech newTech)
	{
		if (newTech == null)
		{
			Debug.LogError("The research provided is null!", null);
		}
		else if (targetTech != newTech)
		{
			foreach (ResearchType type in Research.Instance.researchTypes.Types)
			{
				if (newTech.costsByResearchTypeID.ContainsKey(type.id) && newTech.costsByResearchTypeID[type.id] > 0f)
				{
					GameObject gameObject = Util.KInstantiateUI(progressBarPrefab, progressBarContainer.gameObject, true);
					Image image = gameObject.GetComponentsInChildren<Image>()[2];
					Image component = gameObject.transform.Find("Icon").GetComponent<Image>();
					image.color = type.color;
					component.sprite = type.sprite;
					progressBarsByResearchTypeID[type.id] = gameObject;
				}
			}
			if ((Object)researchScreen == (Object)null)
			{
				researchScreen = base.transform.parent.GetComponentInParent<ResearchScreen>();
			}
			if (newTech.IsComplete())
			{
				ResearchCompleted(false);
			}
			targetTech = newTech;
			researchName.text = targetTech.Name;
			string text = "";
			foreach (TechItem unlockedItem in targetTech.unlockedItems)
			{
				KPointerImage componentInChildrenOnly = GetFreeIcon().GetComponentInChildrenOnly<KPointerImage>();
				componentInChildrenOnly.transform.parent.gameObject.SetActive(true);
				if (text != "")
				{
					text += ", ";
				}
				text += unlockedItem.Name;
				string toolTip = $"{unlockedItem.Name}\n{unlockedItem.description}";
				componentInChildrenOnly.GetComponent<ToolTip>().toolTip = toolTip;
				componentInChildrenOnly.sprite = unlockedItem.UISprite();
				componentInChildrenOnly.ClearPointerEvents();
				componentInChildrenOnly.onPointerEnter += delegate
				{
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
				};
			}
			text = string.Format(UI.RESEARCHSCREEN_UNLOCKSTOOLTIP, text);
			researchName.GetComponent<ToolTip>().toolTip = $"{targetTech.Name}\n{targetTech.desc}\n\n{text}";
			toggle.ClearOnClick();
			toggle.onClick += OnResearchClicked;
			toggle.onPointerEnter += delegate
			{
				researchScreen.TurnEverythingOff();
				OnHover(true, targetTech);
			};
			toggle.soundPlayer.AcceptClickCondition = (() => !targetTech.IsComplete());
			toggle.onPointerExit += delegate
			{
				researchScreen.TurnEverythingOff();
			};
		}
	}

	public void SetEverythingOff()
	{
		if (isOn)
		{
			borderHighlight.gameObject.SetActive(false);
			foreach (KeyValuePair<Tech, UILineRenderer> item in techLineMap)
			{
				item.Value.LineThickness = (float)lineThickness_inactive;
				item.Value.color = inactiveLineColor;
			}
			isOn = false;
		}
	}

	public void SetEverythingOn()
	{
		if (!isOn)
		{
			UpdateProgressBars();
			borderHighlight.gameObject.SetActive(true);
			foreach (KeyValuePair<Tech, UILineRenderer> item in techLineMap)
			{
				item.Value.LineThickness = (float)lineThickness_active;
				item.Value.color = activeLineColor;
			}
			base.transform.SetAsLastSibling();
			isOn = true;
		}
	}

	private void OnHover(bool entered, Tech hoverSource)
	{
		SetEverythingOn();
		foreach (Tech item in targetTech.requiredTech)
		{
			ResearchEntry entry = researchScreen.GetEntry(item);
			if ((Object)entry != (Object)null)
			{
				entry.OnHover(entered, targetTech);
			}
		}
	}

	private void OnResearchClicked()
	{
		TechInstance activeResearch = Research.Instance.GetActiveResearch();
		if (activeResearch != null && activeResearch.tech != targetTech)
		{
			researchScreen.CancelResearch();
		}
		Research.Instance.SetActiveResearch(targetTech, true);
		if (DebugHandler.InstantBuildMode)
		{
			Research.Instance.CompleteQueue();
		}
		UpdateProgressBars();
	}

	private void OnResearchCanceled()
	{
		if (!targetTech.IsComplete())
		{
			toggle.ClearOnClick();
			toggle.onClick += OnResearchClicked;
			researchScreen.CancelResearch();
			Research.Instance.CancelResearch(targetTech, true);
		}
	}

	public void QueueStateChanged(bool isSelected)
	{
		if (isSelected)
		{
			if (!targetTech.IsComplete())
			{
				toggle.isOn = true;
				BG.color = pendingColor;
				titleBG.color = pendingHeaderColor;
				toggle.ClearOnClick();
				toggle.onClick += OnResearchCanceled;
			}
			else
			{
				toggle.isOn = false;
			}
			foreach (KeyValuePair<string, GameObject> item in progressBarsByResearchTypeID)
			{
				Transform child = item.Value.transform.GetChild(0);
				child.GetComponentsInChildren<Image>()[1].color = Color.white;
			}
			Image[] componentsInChildren = iconPanel.GetComponentsInChildren<Image>();
			foreach (Image image in componentsInChildren)
			{
				image.material = StandardUIMaterial;
			}
		}
		else if (targetTech.IsComplete())
		{
			toggle.isOn = false;
			BG.color = completedColor;
			titleBG.color = completedHeaderColor;
			defaultColor = completedColor;
			toggle.ClearOnClick();
			foreach (KeyValuePair<string, GameObject> item2 in progressBarsByResearchTypeID)
			{
				Transform child2 = item2.Value.transform.GetChild(0);
				child2.GetComponentsInChildren<Image>()[1].color = Color.white;
			}
			Image[] componentsInChildren2 = iconPanel.GetComponentsInChildren<Image>();
			foreach (Image image2 in componentsInChildren2)
			{
				image2.material = StandardUIMaterial;
			}
		}
		else
		{
			toggle.isOn = false;
			BG.color = defaultColor;
			titleBG.color = incompleteHeaderColor;
			toggle.ClearOnClick();
			toggle.onClick += OnResearchClicked;
			foreach (KeyValuePair<string, GameObject> item3 in progressBarsByResearchTypeID)
			{
				Transform child3 = item3.Value.transform.GetChild(0);
				child3.GetComponentsInChildren<Image>()[1].color = new Color(0.521568656f, 0.521568656f, 0.521568656f);
			}
			Image[] componentsInChildren3 = iconPanel.GetComponentsInChildren<Image>();
			foreach (Image image3 in componentsInChildren3)
			{
				image3.material = DesaturatedUIMaterial;
			}
		}
	}

	public void UpdateFilterState(string filter_string)
	{
		bool flag = false;
		if (!string.IsNullOrEmpty(filter_string))
		{
			string text = UI.StripLinkFormatting(researchName.text).ToLower();
			flag = text.Contains(filter_string);
			if (!flag)
			{
				foreach (TechItem unlockedItem in targetTech.unlockedItems)
				{
					string text2 = UI.StripLinkFormatting(unlockedItem.Name).ToLower();
					if (text2.Contains(filter_string))
					{
						flag = true;
						break;
					}
					string text3 = UI.StripLinkFormatting(unlockedItem.description).ToLower();
					if (text3.Contains(filter_string))
					{
						flag = true;
						break;
					}
				}
			}
		}
		filterHighlight.gameObject.SetActive(flag);
		filterLowlight.gameObject.SetActive(!flag && !string.IsNullOrEmpty(filter_string));
	}

	public void SetPercentage(float percent)
	{
	}

	public void UpdateProgressBars()
	{
		foreach (KeyValuePair<string, GameObject> item in progressBarsByResearchTypeID)
		{
			Transform child = item.Value.transform.GetChild(0);
			float num = 0f;
			if (targetTech.IsComplete())
			{
				num = 1f;
				child.GetComponentInChildren<LocText>().text = targetTech.costsByResearchTypeID[item.Key] + "/" + targetTech.costsByResearchTypeID[item.Key];
			}
			else
			{
				TechInstance orAdd = Research.Instance.GetOrAdd(targetTech);
				if (orAdd == null)
				{
					continue;
				}
				child.GetComponentInChildren<LocText>().text = orAdd.progressInventory.PointsByTypeID[item.Key] + "/" + targetTech.costsByResearchTypeID[item.Key];
				num = orAdd.progressInventory.PointsByTypeID[item.Key] / targetTech.costsByResearchTypeID[item.Key];
			}
			child.GetComponentsInChildren<Image>()[2].fillAmount = num;
			child.GetComponent<ToolTip>().SetSimpleTooltip(Research.Instance.researchTypes.GetResearchType(item.Key).description);
		}
	}

	private GameObject GetFreeIcon()
	{
		return Util.KInstantiateUI(iconPrefab, iconPanel, false);
	}

	private Image GetFreeLine()
	{
		return Util.KInstantiateUI<Image>(linePrefab.gameObject, base.gameObject, false);
	}

	public void ResearchCompleted(bool notify = true)
	{
		BG.color = completedColor;
		titleBG.color = completedHeaderColor;
		defaultColor = completedColor;
		if (notify)
		{
			unlockedTechMetric[UnlockedTechKey] = targetTech.Id;
			ThreadedHttps<KleiMetrics>.Instance.SendEvent(unlockedTechMetric);
		}
		toggle.ClearOnClick();
		if (notify)
		{
			ResearchCompleteMessage message = new ResearchCompleteMessage(targetTech);
			MusicManager.instance.PlaySong("Stinger_ResearchComplete", false);
			Messenger.Instance.QueueMessage(message);
		}
	}
}
