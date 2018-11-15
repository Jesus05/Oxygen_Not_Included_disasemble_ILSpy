using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class SimpleInfoScreen : TargetScreen
{
	[DebuggerDisplay("{item.item.Name}")]
	public class StatusItemEntry : IRenderEveryTick
	{
		private enum FadeStage
		{
			IN,
			WAIT,
			OUT
		}

		public StatusItemGroup.Entry item;

		public StatusItemCategory category;

		private LayoutElement spacerLayout;

		private GameObject widget;

		private ToolTip toolTip;

		private TextStyleSetting tooltipStyle;

		public Action<StatusItemEntry> onDestroy;

		private Image image;

		private LocText text;

		public Color color;

		private FadeStage fadeStage;

		private float fade;

		private float fadeInTime;

		private float fadeOutTime = 1.8f;

		public Image GetImage => image;

		public StatusItemEntry(StatusItemGroup.Entry item, StatusItemCategory category, GameObject status_item_prefab, Transform parent, TextStyleSetting tooltip_style, Color color, bool skip_fade, Action<StatusItemEntry> onDestroy)
		{
			this.item = item;
			this.category = category;
			tooltipStyle = tooltip_style;
			this.onDestroy = onDestroy;
			this.color = color;
			widget = Util.KInstantiateUI(status_item_prefab, parent.gameObject, false);
			text = widget.GetComponentInChildren<LocText>(true);
			toolTip = widget.GetComponentInChildren<ToolTip>(true);
			image = widget.GetComponentInChildren<Image>(true);
			item.SetIcon(image);
			widget.SetActive(true);
			toolTip.OnToolTip = OnToolTip;
			fadeStage = (skip_fade ? FadeStage.WAIT : FadeStage.IN);
			SimAndRenderScheduler.instance.Add(this, false);
			Refresh();
			SetColor(1f);
		}

		internal void SetSprite(TintedSprite sprite)
		{
			if (sprite != null)
			{
				image.sprite = sprite.sprite;
			}
		}

		public int GetIndex()
		{
			return widget.transform.GetSiblingIndex();
		}

		public void SetIndex(int index)
		{
			widget.transform.SetSiblingIndex(index);
		}

		public void RenderEveryTick(float dt)
		{
			switch (fadeStage)
			{
			case FadeStage.IN:
			{
				fade = Mathf.Min(fade + Time.deltaTime / fadeInTime, 1f);
				float num2 = fade;
				SetColor(num2);
				if (fade >= 1f)
				{
					fadeStage = FadeStage.WAIT;
				}
				break;
			}
			case FadeStage.OUT:
			{
				float num = fade;
				SetColor(num);
				fade = Mathf.Max(fade - Time.deltaTime / fadeOutTime, 0f);
				if (fade <= 0f)
				{
					Destroy(true);
				}
				break;
			}
			}
		}

		private string OnToolTip()
		{
			item.ShowToolTip(toolTip, tooltipStyle);
			return string.Empty;
		}

		public void Refresh()
		{
			string name = item.GetName();
			if (name != text.text)
			{
				text.text = name;
				SetColor(1f);
			}
		}

		private void SetColor(float alpha = 1f)
		{
			Color color = new Color(this.color.r, this.color.g, this.color.b, alpha);
			image.color = color;
			text.color = color;
		}

		public void Destroy(bool immediate)
		{
			if (immediate)
			{
				if (onDestroy != null)
				{
					onDestroy(this);
				}
				SimAndRenderScheduler.instance.Remove(this);
				toolTip.OnToolTip = null;
				UnityEngine.Object.Destroy(widget);
			}
			else
			{
				fade = 0.5f;
				fadeStage = FadeStage.OUT;
			}
		}
	}

	public GameObject attributesLabelTemplate;

	public GameObject attributesLabelButtonTemplate;

	public GameObject DescriptionContainerTemplate;

	private DescriptionContainer descriptionContainer;

	public GameObject StampContainerTemplate;

	public GameObject StampPrefab;

	public GameObject VitalsPanelTemplate;

	public Sprite DefaultPortraitIcon;

	public Text StatusPanelCurrentActionLabel;

	public GameObject StatusItemPrefab;

	public Sprite statusWarningIcon;

	private CollapsibleDetailContentPanel statusItemPanel;

	private CollapsibleDetailContentPanel vitalsPanel;

	private CollapsibleDetailContentPanel fertilityPanel;

	private GameObject storagePanel;

	private GameObject infoPanel;

	private GameObject stampContainer;

	private MinionVitalsPanel vitalsContainer;

	private GameObject InfoFolder;

	private GameObject statusItemsFolder;

	public GameObject TextContainerPrefab;

	private Dictionary<string, GameObject> storageLabels = new Dictionary<string, GameObject>();

	public TextStyleSetting ToolTipStyle_Property;

	public Color statusItemTextColor_regular = Color.black;

	public Color statusItemTextColor_bad = new Color(0.956862748f, 0.2901961f, 0.2784314f);

	public Color statusItemTextColor_old = new Color(0.8235294f, 0.8235294f, 0.8235294f);

	private GameObject lastTarget;

	private bool TargetIsMinion;

	private List<StatusItemEntry> statusItems = new List<StatusItemEntry>();

	private List<StatusItemEntry> oldStatusItems = new List<StatusItemEntry>();

	private List<LocText> attributeLabels = new List<LocText>();

	private Action<object> onStorageChangeDelegate;

	private static readonly EventSystem.IntraObjectHandler<SimpleInfoScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<SimpleInfoScreen>(delegate(SimpleInfoScreen component, object data)
	{
		component.OnRefreshData(data);
	});

	public SimpleInfoScreen()
	{
		onStorageChangeDelegate = OnStorageChange;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		statusItemPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		statusItemPanel.Content.GetComponent<VerticalLayoutGroup>().padding.bottom = 10;
		statusItemPanel.HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_STATUS;
		statusItemPanel.scalerMask.hoverLock = true;
		statusItemsFolder = statusItemPanel.Content.gameObject;
		vitalsPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		vitalsPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_CONDITION);
		vitalsContainer = Util.KInstantiateUI(VitalsPanelTemplate, vitalsPanel.Content.gameObject, false).GetComponent<MinionVitalsPanel>();
		fertilityPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		fertilityPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_FERTILITY);
		infoPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		infoPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_DESCRIPTION;
		GameObject gameObject = infoPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject;
		descriptionContainer = Util.KInstantiateUI<DescriptionContainer>(DescriptionContainerTemplate, gameObject, false);
		storagePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		stampContainer = Util.KInstantiateUI(StampContainerTemplate, gameObject, false);
		Subscribe(-1514841199, OnRefreshDataDelegate);
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		Subscribe(target, -1697596308, onStorageChangeDelegate);
		Subscribe(target, -1197125120, onStorageChangeDelegate);
		RefreshStorage();
		Subscribe(target, 1059811075, OnBreedingChanceChanged);
		RefreshBreedingChance();
		vitalsPanel.SetTitle((!((UnityEngine.Object)target.GetComponent<WiltCondition>() == (UnityEngine.Object)null)) ? UI.DETAILTABS.SIMPLEINFO.GROUPNAME_REQUIREMENTS : UI.DETAILTABS.SIMPLEINFO.GROUPNAME_CONDITION);
		KSelectable component = target.GetComponent<KSelectable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			StatusItemGroup statusItemGroup = component.GetStatusItemGroup();
			if (statusItemGroup != null)
			{
				StatusItemGroup statusItemGroup2 = statusItemGroup;
				statusItemGroup2.OnAddStatusItem = (Action<StatusItemGroup.Entry, StatusItemCategory>)Delegate.Combine(statusItemGroup2.OnAddStatusItem, new Action<StatusItemGroup.Entry, StatusItemCategory>(OnAddStatusItem));
				StatusItemGroup statusItemGroup3 = statusItemGroup;
				statusItemGroup3.OnRemoveStatusItem = (Action<StatusItemGroup.Entry, bool>)Delegate.Combine(statusItemGroup3.OnRemoveStatusItem, new Action<StatusItemGroup.Entry, bool>(OnRemoveStatusItem));
				foreach (StatusItemGroup.Entry item in statusItemGroup)
				{
					StatusItemGroup.Entry current = item;
					if (current.category != null && current.category.Id == "Main")
					{
						DoAddStatusItem(current, current.category, false);
					}
				}
				foreach (StatusItemGroup.Entry item2 in statusItemGroup)
				{
					StatusItemGroup.Entry current2 = item2;
					if (current2.category == null || current2.category.Id != "Main")
					{
						DoAddStatusItem(current2, current2.category, false);
					}
				}
			}
		}
		statusItemPanel.gameObject.SetActive(true);
		statusItemPanel.scalerMask.UpdateSize();
		Refresh(true);
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnDeselectTarget(target);
		if ((UnityEngine.Object)target != (UnityEngine.Object)null)
		{
			Unsubscribe(target, -1697596308, onStorageChangeDelegate);
			Unsubscribe(target, -1197125120, onStorageChangeDelegate);
			Unsubscribe(target, 1059811075, OnBreedingChanceChanged);
		}
		KSelectable component = target.GetComponent<KSelectable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			StatusItemGroup statusItemGroup = component.GetStatusItemGroup();
			if (statusItemGroup != null)
			{
				StatusItemGroup statusItemGroup2 = statusItemGroup;
				statusItemGroup2.OnAddStatusItem = (Action<StatusItemGroup.Entry, StatusItemCategory>)Delegate.Remove(statusItemGroup2.OnAddStatusItem, new Action<StatusItemGroup.Entry, StatusItemCategory>(OnAddStatusItem));
				StatusItemGroup statusItemGroup3 = statusItemGroup;
				statusItemGroup3.OnRemoveStatusItem = (Action<StatusItemGroup.Entry, bool>)Delegate.Remove(statusItemGroup3.OnRemoveStatusItem, new Action<StatusItemGroup.Entry, bool>(OnRemoveStatusItem));
				foreach (StatusItemEntry statusItem in statusItems)
				{
					statusItem.Destroy(true);
				}
				statusItems.Clear();
				foreach (StatusItemEntry oldStatusItem in oldStatusItems)
				{
					oldStatusItem.onDestroy = null;
					oldStatusItem.Destroy(true);
				}
				oldStatusItems.Clear();
			}
		}
	}

	private void OnStorageChange(object data)
	{
		RefreshStorage();
	}

	private void OnBreedingChanceChanged(object data)
	{
		RefreshBreedingChance();
	}

	private void OnAddStatusItem(StatusItemGroup.Entry status_item, StatusItemCategory category)
	{
		DoAddStatusItem(status_item, category, false);
	}

	private void DoAddStatusItem(StatusItemGroup.Entry status_item, StatusItemCategory category, bool show_immediate = false)
	{
		GameObject gameObject = statusItemsFolder;
		StatusItemEntry statusItemEntry = new StatusItemEntry(color: (status_item.item.notificationType != NotificationType.BadMinor && status_item.item.notificationType != NotificationType.Bad && status_item.item.notificationType != NotificationType.DuplicantThreatening) ? statusItemTextColor_regular : statusItemTextColor_bad, item: status_item, category: category, status_item_prefab: StatusItemPrefab, parent: gameObject.transform, tooltip_style: ToolTipStyle_Property, skip_fade: show_immediate, onDestroy: OnStatusItemDestroy);
		statusItemEntry.SetSprite(status_item.item.sprite);
		if (category != null)
		{
			int num = -1;
			List<StatusItemEntry> list = oldStatusItems.FindAll((StatusItemEntry e) => e.category == category);
			foreach (StatusItemEntry item in list)
			{
				num = item.GetIndex();
				item.Destroy(true);
				oldStatusItems.Remove(item);
			}
			if (num != -1)
			{
				statusItemEntry.SetIndex(num);
			}
		}
		statusItems.Add(statusItemEntry);
	}

	private void OnRemoveStatusItem(StatusItemGroup.Entry status_item, bool immediate = false)
	{
		DoRemoveStatusItem(status_item, immediate);
	}

	private void DoRemoveStatusItem(StatusItemGroup.Entry status_item, bool destroy_immediate = false)
	{
		int num = 0;
		while (true)
		{
			if (num >= statusItems.Count)
			{
				return;
			}
			if (statusItems[num].item.item == status_item.item)
			{
				break;
			}
			num++;
		}
		StatusItemEntry statusItemEntry = statusItems[num];
		statusItems.RemoveAt(num);
		oldStatusItems.Add(statusItemEntry);
		statusItemEntry.Destroy(destroy_immediate);
	}

	private void OnStatusItemDestroy(StatusItemEntry item)
	{
		oldStatusItems.Remove(item);
	}

	private void Update()
	{
		Refresh(false);
	}

	private void OnRefreshData(object obj)
	{
		Refresh(false);
	}

	public void Refresh(bool force = false)
	{
		if ((UnityEngine.Object)selectedTarget != (UnityEngine.Object)lastTarget || force)
		{
			lastTarget = selectedTarget;
			if ((UnityEngine.Object)selectedTarget != (UnityEngine.Object)null)
			{
				SetPanels(selectedTarget);
				SetStamps(selectedTarget);
			}
		}
		int count = statusItems.Count;
		statusItemPanel.gameObject.SetActive(count > 0);
		for (int i = 0; i < count; i++)
		{
			statusItems[i].Refresh();
		}
		if (vitalsContainer.isActiveAndEnabled)
		{
			vitalsContainer.Refresh();
		}
		RefreshStorage();
	}

	private void SetPanels(GameObject target)
	{
		MinionIdentity component = target.GetComponent<MinionIdentity>();
		Amounts amounts = target.GetAmounts();
		PrimaryElement component2 = target.GetComponent<PrimaryElement>();
		BuildingComplete component3 = target.GetComponent<BuildingComplete>();
		BuildingUnderConstruction component4 = target.GetComponent<BuildingUnderConstruction>();
		CellSelectionObject component5 = target.GetComponent<CellSelectionObject>();
		InfoDescription component6 = target.GetComponent<InfoDescription>();
		Edible component7 = target.GetComponent<Edible>();
		attributeLabels.ForEach(delegate(LocText x)
		{
			UnityEngine.Object.Destroy(x.gameObject);
		});
		attributeLabels.Clear();
		vitalsPanel.gameObject.SetActive(amounts != null);
		string text = string.Empty;
		string text2 = string.Empty;
		if (amounts != null)
		{
			vitalsContainer.selectedEntity = selectedTarget;
			Uprootable component8 = selectedTarget.gameObject.GetComponent<Uprootable>();
			if ((UnityEngine.Object)component8 != (UnityEngine.Object)null)
			{
				vitalsPanel.gameObject.SetActive((UnityEngine.Object)component8.GetPlanterStorage != (UnityEngine.Object)null);
			}
			Growing component9 = selectedTarget.gameObject.GetComponent<Growing>();
			if ((UnityEngine.Object)component9 != (UnityEngine.Object)null)
			{
				vitalsPanel.gameObject.SetActive(true);
			}
		}
		if ((bool)component)
		{
			text = string.Empty;
		}
		else if ((bool)component6)
		{
			text = component6.description;
		}
		else if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
		{
			text = component3.Def.Effect;
			text2 = component3.Def.Desc;
		}
		else if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
		{
			text = component4.Def.Effect;
			text2 = component4.Def.Desc;
		}
		else if ((UnityEngine.Object)component7 != (UnityEngine.Object)null)
		{
			EdiblesManager.FoodInfo foodInfo = component7.FoodInfo;
			text += string.Format(UI.GAMEOBJECTEFFECTS.CALORIES, GameUtil.GetFormattedCalories(foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true));
		}
		else if ((UnityEngine.Object)component5 != (UnityEngine.Object)null)
		{
			text = component5.element.FullDescription(false);
		}
		else if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			Element element = ElementLoader.FindElementByHash(component2.ElementID);
			text = ((element == null) ? string.Empty : element.FullDescription(false));
		}
		List<Descriptor> gameObjectEffects = GameUtil.GetGameObjectEffects(target, true);
		bool flag = gameObjectEffects.Count > 0;
		descriptionContainer.gameObject.SetActive(flag);
		descriptionContainer.descriptors.gameObject.SetActive(flag);
		if (flag)
		{
			descriptionContainer.descriptors.SetDescriptors(gameObjectEffects);
		}
		descriptionContainer.description.text = text;
		descriptionContainer.flavour.text = text2;
		infoPanel.gameObject.SetActive((UnityEngine.Object)component == (UnityEngine.Object)null);
		descriptionContainer.gameObject.SetActive(infoPanel.activeSelf);
		descriptionContainer.flavour.gameObject.SetActive(text2 != string.Empty && text2 != "\n");
		if (vitalsPanel.gameObject.activeSelf && amounts.Count == 0)
		{
			vitalsPanel.gameObject.SetActive(false);
		}
	}

	private void RefreshBreedingChance()
	{
		if ((UnityEngine.Object)selectedTarget == (UnityEngine.Object)null)
		{
			fertilityPanel.gameObject.SetActive(false);
		}
		else
		{
			FertilityMonitor.Instance sMI = selectedTarget.GetSMI<FertilityMonitor.Instance>();
			if (sMI == null)
			{
				fertilityPanel.gameObject.SetActive(false);
			}
			else
			{
				int num = 0;
				foreach (FertilityMonitor.BreedingChance breedingChance in sMI.breedingChances)
				{
					List<FertilityModifier> forTag = Db.Get().FertilityModifiers.GetForTag(breedingChance.egg);
					if (forTag.Count > 0)
					{
						string text = string.Empty;
						foreach (FertilityModifier item in forTag)
						{
							text += string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_MOD_FORMAT, item.GetTooltip());
						}
						fertilityPanel.SetLabel("breeding_" + num++, string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None)), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT_TOOLTIP, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None), text));
					}
					else
					{
						fertilityPanel.SetLabel("breeding_" + num++, string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None)), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT_TOOLTIP_NOMOD, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None)));
					}
				}
				fertilityPanel.Commit();
			}
		}
	}

	private void RefreshStorage()
	{
		if ((UnityEngine.Object)selectedTarget == (UnityEngine.Object)null)
		{
			storagePanel.gameObject.SetActive(false);
		}
		else
		{
			Storage[] componentsInChildren = selectedTarget.GetComponentsInChildren<Storage>();
			if (componentsInChildren == null)
			{
				storagePanel.gameObject.SetActive(false);
			}
			else
			{
				componentsInChildren = Array.FindAll(componentsInChildren, (Storage n) => n.showInUI);
				if (componentsInChildren.Length == 0)
				{
					storagePanel.gameObject.SetActive(false);
				}
				else
				{
					storagePanel.gameObject.SetActive(true);
					string text = (!((UnityEngine.Object)selectedTarget.GetComponent<MinionIdentity>() != (UnityEngine.Object)null)) ? UI.DETAILTABS.DETAILS.GROUPNAME_CONTENTS : UI.DETAILTABS.DETAILS.GROUPNAME_MINION_CONTENTS;
					storagePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = text;
					foreach (KeyValuePair<string, GameObject> storageLabel in storageLabels)
					{
						storageLabel.Value.SetActive(false);
					}
					int num = 0;
					Storage[] array = componentsInChildren;
					GameObject select_item;
					Storage selected_storage;
					GameObject select_target;
					foreach (Storage storage in array)
					{
						foreach (GameObject item in storage.items)
						{
							if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
							{
								GameObject gameObject = AddOrGetStorageLabel(storageLabels, storagePanel, "storage_" + num.ToString());
								num++;
								if (storage.allowUIItemRemoval)
								{
									Transform transform = gameObject.transform.Find("removeAttributeButton");
									if ((UnityEngine.Object)transform != (UnityEngine.Object)null)
									{
										KButton component = transform.GetComponent<KButton>();
										component.enabled = true;
										component.gameObject.SetActive(true);
										select_item = item;
										selected_storage = storage;
										component.onClick += delegate
										{
											selected_storage.Remove(select_item);
										};
									}
								}
								PrimaryElement component2 = item.GetComponent<PrimaryElement>();
								Rottable.Instance sMI = item.GetSMI<Rottable.Instance>();
								gameObject.GetComponentInChildren<ToolTip>().ClearMultiStringTooltip();
								string unitFormattedName = GameUtil.GetUnitFormattedName(item, false);
								unitFormattedName = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_MASS, unitFormattedName, GameUtil.GetFormattedMass(component2.Mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
								unitFormattedName = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_TEMPERATURE, unitFormattedName, GameUtil.GetFormattedTemperature(component2.Temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
								if (sMI != null)
								{
									unitFormattedName += string.Format(UI.DETAILTABS.DETAILS.CONTENTS_ROTTABLE, sMI.StateString());
									gameObject.GetComponentInChildren<ToolTip>().AddMultiStringTooltip(sMI.GetToolTip(), PluginAssets.Instance.defaultTextStyleSetting);
								}
								if (component2.DiseaseIdx != 255)
								{
									unitFormattedName += string.Format(UI.DETAILTABS.DETAILS.CONTENTS_DISEASED, GameUtil.GetFormattedDisease(component2.DiseaseIdx, component2.DiseaseCount, false));
									string formattedDisease = GameUtil.GetFormattedDisease(component2.DiseaseIdx, component2.DiseaseCount, true);
									gameObject.GetComponentInChildren<ToolTip>().AddMultiStringTooltip(formattedDisease, PluginAssets.Instance.defaultTextStyleSetting);
								}
								gameObject.GetComponentInChildren<LocText>().text = unitFormattedName;
								KButton component3 = gameObject.GetComponent<KButton>();
								select_target = item;
								component3.onClick += delegate
								{
									SelectTool.Instance.Select(select_target.GetComponent<KSelectable>(), false);
								};
							}
						}
					}
					if (num == 0)
					{
						GameObject gameObject2 = AddOrGetStorageLabel(storageLabels, storagePanel, "empty");
						gameObject2.GetComponentInChildren<LocText>().text = UI.DETAILTABS.DETAILS.STORAGE_EMPTY;
					}
				}
			}
		}
	}

	private GameObject AddOrGetStorageLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject = null;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
			KButton component = gameObject.GetComponent<KButton>();
			component.ClearOnClick();
			Transform transform = gameObject.transform.Find("removeAttributeButton");
			if ((UnityEngine.Object)transform != (UnityEngine.Object)null)
			{
				KButton kButton = transform.FindComponent<KButton>();
				kButton.enabled = false;
				kButton.gameObject.SetActive(false);
				kButton.ClearOnClick();
			}
		}
		else
		{
			gameObject = Util.KInstantiate(attributesLabelButtonTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, null);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private void ShowAttributes(GameObject target)
	{
		Attributes attributes = target.GetAttributes();
		if (attributes != null)
		{
			List<AttributeInstance> list = attributes.AttributeTable.FindAll((AttributeInstance a) => a.Attribute.ShowInUI == Klei.AI.Attribute.Display.General);
			if (list.Count > 0)
			{
				descriptionContainer.descriptors.gameObject.SetActive(true);
				List<Descriptor> list2 = new List<Descriptor>();
				foreach (AttributeInstance item2 in list)
				{
					Descriptor item = new Descriptor($"{item2.Name}: {item2.GetFormattedValue()}", item2.GetAttributeValueTooltip(), Descriptor.DescriptorType.Effect, false);
					item.IncreaseIndent();
					list2.Add(item);
				}
				descriptionContainer.descriptors.SetDescriptors(list2);
			}
		}
	}

	private void SetStamps(GameObject target)
	{
		for (int i = 0; i < stampContainer.transform.childCount; i++)
		{
			UnityEngine.Object.Destroy(stampContainer.transform.GetChild(i).gameObject);
		}
		BuildingComplete component = target.GetComponent<BuildingComplete>();
		if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
		{
			return;
		}
	}
}
