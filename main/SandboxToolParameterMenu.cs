using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SandboxToolParameterMenu : KScreen
{
	public class SelectorValue
	{
		public class SearchFilter
		{
			public string Name;

			public Func<object, bool> condition;

			public SearchFilter parentFilter;

			public Tuple<Sprite, Color> icon;

			public SearchFilter(string Name, Func<object, bool> condition, SearchFilter parentFilter = null, Tuple<Sprite, Color> icon = null)
			{
				this.Name = Name;
				this.condition = condition;
				this.parentFilter = parentFilter;
				this.icon = icon;
			}
		}

		public GameObject row;

		public List<KeyValuePair<object, GameObject>> optionButtons;

		public KButton button;

		public object[] options;

		public Action<object> onValueChanged;

		public Func<object, string> getOptionName;

		public Func<string, object, bool> filterOptionFunction;

		public Func<object, Tuple<Sprite, Color>> getOptionSprite;

		public SearchFilter[] filters;

		public List<SearchFilter> activeFilters = new List<SearchFilter>();

		public SearchFilter currentFilter;

		public SelectorValue(object[] options, Action<object> onValueChanged, Func<object, string> getOptionName, Func<string, object, bool> filterOptionFunction, Func<object, Tuple<Sprite, Color>> getOptionSprite, SearchFilter[] filters = null)
		{
			this.options = options;
			this.onValueChanged = onValueChanged;
			this.getOptionName = getOptionName;
			this.filterOptionFunction = filterOptionFunction;
			this.getOptionSprite = getOptionSprite;
			this.filters = filters;
		}

		public bool runCurrentFilter(object obj)
		{
			if (currentFilter == null)
			{
				return true;
			}
			if (!currentFilter.condition(obj))
			{
				return false;
			}
			return true;
		}
	}

	public class SliderValue
	{
		public GameObject row;

		public string bottomSprite;

		public string topSprite;

		public float minValue;

		public float maxValue;

		public string unitString;

		public Action<float> onValueChanged;

		public string tooltip;

		public KSlider slider;

		public KNumberInputField inputField;

		public SliderValue(float minValue, float maxValue, string bottomSprite, string topSprite, string unitString, string tooltip, Action<float> onValueChanged)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.bottomSprite = bottomSprite;
			this.topSprite = topSprite;
			this.unitString = unitString;
			this.onValueChanged = onValueChanged;
			this.tooltip = tooltip;
		}

		public void SetRange(float min, float max)
		{
			minValue = min;
			maxValue = max;
			slider.minValue = minValue;
			slider.maxValue = maxValue;
			inputField.currentValue = minValue + (maxValue - minValue) / 2f;
			inputField.SetDisplayValue(inputField.currentValue.ToString());
			slider.value = minValue + (maxValue - minValue) / 2f;
			onValueChanged(minValue + (maxValue - minValue) / 2f);
		}

		public void SetValue(float value)
		{
			slider.value = value;
			inputField.currentValue = value;
			onValueChanged(value);
			RefreshDisplay();
		}

		public void RefreshDisplay()
		{
			inputField.SetDisplayValue(inputField.currentValue.ToString());
		}
	}

	public static SandboxToolParameterMenu instance;

	public SandboxSettings settings;

	[SerializeField]
	private GameObject sliderPropertyPrefab;

	[SerializeField]
	private GameObject selectorPropertyPrefab;

	private List<GameObject> inputFields = new List<GameObject>();

	public SelectorValue elementSelector;

	public SliderValue brushRadiusSlider = new SliderValue(1f, 10f, "dash", "circle_hard", string.Empty, UI.SANDBOXTOOLS.SETTINGS.BRUSH_SIZE.TOOLTIP, delegate(float value)
	{
		instance.settings.BrushSize = Mathf.RoundToInt(value);
	});

	public SliderValue noiseScaleSlider = new SliderValue(0f, 1f, "little", "lots", string.Empty, UI.SANDBOXTOOLS.SETTINGS.BRUSH_NOISE.TOOLTIP, delegate(float value)
	{
		instance.settings.NoiseScale = value;
	});

	public SliderValue noiseDensitySlider = new SliderValue(1f, 20f, "little", "lots", string.Empty, UI.SANDBOXTOOLS.SETTINGS.BRUSH_NOISE.TOOLTIP, delegate(float value)
	{
		instance.settings.NoiseDensity = value;
	});

	public SliderValue massSlider = new SliderValue(0.1f, 1000f, "action_pacify", "status_item_plant_solid", UI.UNITSUFFIXES.MASS.KILOGRAM, UI.SANDBOXTOOLS.SETTINGS.MASS.TOOLTIP, delegate(float value)
	{
		instance.settings.Mass = (float)Mathf.RoundToInt(value * 10000f) / 10000f;
	});

	public SliderValue temperatureSlider = new SliderValue(150f, 500f, "cold", "hot", UI.UNITSUFFIXES.TEMPERATURE.KELVIN, UI.SANDBOXTOOLS.SETTINGS.TEMPERATURE.TOOLTIP, delegate(float value)
	{
		instance.settings.temperature = Mathf.Clamp((float)Mathf.RoundToInt(value * 100f) / 100f, 1f, 9999f);
	});

	public SliderValue temperatureAdditiveSlider = new SliderValue(-15f, 15f, "cold", "hot", UI.UNITSUFFIXES.TEMPERATURE.KELVIN, UI.SANDBOXTOOLS.SETTINGS.TEMPERATURE_ADDITIVE.TOOLTIP, delegate(float value)
	{
		instance.settings.temperatureAdditive = (float)Mathf.RoundToInt(value * 100f) / 100f;
	});

	public SelectorValue diseaseSelector;

	public SliderValue diseaseCountSlider = new SliderValue(0f, 10000f, "status_item_barren", "germ", UI.UNITSUFFIXES.DISEASE.UNITS, UI.SANDBOXTOOLS.SETTINGS.DISEASE_COUNT.TOOLTIP, delegate(float value)
	{
		instance.settings.diseaseCount = Mathf.RoundToInt(value);
	});

	public SelectorValue entitySelector;

	public static void DestroyInstance()
	{
		instance = null;
	}

	public override float GetSortKey()
	{
		return 100f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		settings = new SandboxSettings();
		SandboxSettings sandboxSettings = settings;
		sandboxSettings.OnChangeElement = (System.Action)Delegate.Combine(sandboxSettings.OnChangeElement, (System.Action)delegate
		{
			elementSelector.button.GetComponentInChildren<LocText>().text = instance.settings.Element.name + " (" + instance.settings.Element.GetStateString() + ")";
			Tuple<Sprite, Color> uISprite = Def.GetUISprite(settings.Element, "ui", false);
			elementSelector.button.GetComponentsInChildren<Image>()[1].sprite = uISprite.first;
			elementSelector.button.GetComponentsInChildren<Image>()[1].color = uISprite.second;
			temperatureSlider.SetRange(Mathf.Max(instance.settings.Element.lowTemp - 10f, 1f), Mathf.Min(9999f, instance.settings.Element.highTemp + 10f));
			temperatureSlider.SetValue(instance.settings.Element.defaultValues.temperature);
			massSlider.SetRange(0.1f, instance.settings.Element.defaultValues.mass * 2f);
		});
		SandboxSettings sandboxSettings2 = settings;
		sandboxSettings2.OnChangeDisease = (System.Action)Delegate.Combine(sandboxSettings2.OnChangeDisease, (System.Action)delegate
		{
			diseaseSelector.button.GetComponentInChildren<LocText>().text = instance.settings.Disease.Name;
			diseaseSelector.button.GetComponentsInChildren<Image>()[1].sprite = Assets.GetSprite("germ");
			diseaseCountSlider.SetRange(0f, 1000000f);
		});
		SandboxSettings sandboxSettings3 = settings;
		sandboxSettings3.OnChangeEntity = (System.Action)Delegate.Combine(sandboxSettings3.OnChangeEntity, (System.Action)delegate
		{
			entitySelector.button.GetComponentInChildren<LocText>().text = instance.settings.Entity.GetProperName();
			Tuple<Sprite, Color> tuple = (!(settings.Entity.PrefabTag == (Tag)MinionConfig.ID)) ? Def.GetUISprite(settings.Entity.PrefabTag, "ui", false) : new Tuple<Sprite, Color>(Assets.GetSprite("ui_duplicant_portrait_placeholder"), Color.white);
			if (tuple != null)
			{
				entitySelector.button.GetComponentsInChildren<Image>()[1].sprite = tuple.first;
				entitySelector.button.GetComponentsInChildren<Image>()[1].color = tuple.second;
			}
		});
		SandboxSettings sandboxSettings4 = settings;
		sandboxSettings4.OnChangeBrushSize = (System.Action)Delegate.Combine(sandboxSettings4.OnChangeBrushSize, (System.Action)delegate
		{
			if (PlayerController.Instance.ActiveTool is BrushTool)
			{
				(PlayerController.Instance.ActiveTool as BrushTool).SetBrushSize(settings.BrushSize);
			}
		});
		SandboxSettings sandboxSettings5 = settings;
		sandboxSettings5.OnChangeNoiseScale = (System.Action)Delegate.Combine(sandboxSettings5.OnChangeNoiseScale, (System.Action)delegate
		{
			if (PlayerController.Instance.ActiveTool is BrushTool)
			{
				(PlayerController.Instance.ActiveTool as BrushTool).SetBrushSize(settings.BrushSize);
			}
		});
		SandboxSettings sandboxSettings6 = settings;
		sandboxSettings6.OnChangeNoiseDensity = (System.Action)Delegate.Combine(sandboxSettings6.OnChangeNoiseDensity, (System.Action)delegate
		{
			if (PlayerController.Instance.ActiveTool is BrushTool)
			{
				(PlayerController.Instance.ActiveTool as BrushTool).SetBrushSize(settings.BrushSize);
			}
		});
		settings.InstantBuild = true;
		activateOnSpawn = true;
		ConsumeMouseScroll = true;
	}

	public void DisableParameters()
	{
		elementSelector.row.SetActive(false);
		entitySelector.row.SetActive(false);
		brushRadiusSlider.row.SetActive(false);
		noiseScaleSlider.row.SetActive(false);
		noiseDensitySlider.row.SetActive(false);
		massSlider.row.SetActive(false);
		temperatureAdditiveSlider.row.SetActive(false);
		temperatureSlider.row.SetActive(false);
		diseaseCountSlider.row.SetActive(false);
		diseaseSelector.row.SetActive(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ConfigureElementSelector();
		ConfigureDiseaseSelector();
		ConfigureEntitySelector();
		SpawnSelector(entitySelector);
		SpawnSelector(elementSelector);
		SpawnSlider(brushRadiusSlider);
		SpawnSlider(noiseScaleSlider);
		SpawnSlider(noiseDensitySlider);
		SpawnSlider(massSlider);
		SpawnSlider(temperatureSlider);
		SpawnSlider(temperatureAdditiveSlider);
		SpawnSelector(diseaseSelector);
		SpawnSlider(diseaseCountSlider);
		if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
		{
			instance = this;
			base.gameObject.SetActive(false);
			settings.SelectElement(ElementLoader.FindElementByHash(SimHashes.Water));
			brushRadiusSlider.SetRange(1f, 10f);
			brushRadiusSlider.slider.wholeNumbers = true;
			noiseScaleSlider.SetRange(0f, 1f);
			noiseDensitySlider.SetRange(0f, 20f);
			temperatureSlider.SetRange(Mathf.Max(instance.settings.Element.lowTemp - 10f, 1f), instance.settings.Element.highTemp + 10f);
			massSlider.SetRange(0.1f, instance.settings.Element.defaultValues.mass * 2f);
			massSlider.SetValue(settings.Mass);
			settings.SelectDisease(Db.Get().Diseases.FoodPoisoning);
			settings.SelectEntity(Assets.GetPrefab("MushBar".ToTag()).GetComponent<KPrefabID>());
		}
	}

	private void ConfigureElementSelector()
	{
		Func<object, bool> func = (object element) => (element as Element).IsSolid;
		Func<object, bool> func2 = (object element) => (element as Element).IsLiquid;
		Func<object, bool> func3 = (object element) => (element as Element).IsGas;
		List<Element> commonElements = new List<Element>();
		Func<object, bool> condition = (object element) => commonElements.Contains(element as Element);
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Oxygen));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Water));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Vacuum));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Dirt));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.SandStone));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Cuprite));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Algae));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.CarbonDioxide));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Sand));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.SlimeMold));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Granite));
		List<Element> list = new List<Element>();
		foreach (Element element in ElementLoader.elements)
		{
			if (!element.disabled)
			{
				list.Add(element);
			}
		}
		list.Sort((Element a, Element b) => a.name.CompareTo(b.name));
		Element[] options = list.ToArray();
		Action<object> onValueChanged = delegate(object element)
		{
			settings.SelectElement(element as Element);
		};
		Func<object, string> getOptionName = (object element) => (element as Element).name + " (" + (element as Element).GetStateString() + ")";
		Func<string, object, bool> filterOptionFunction = delegate(string filterString, object option)
		{
			if (((option as Element).name.ToUpper() + (option as Element).GetStateString().ToUpper()).Contains(filterString.ToUpper()))
			{
				return true;
			}
			return false;
		};
		Func<object, Tuple<Sprite, Color>> getOptionSprite = (object element) => Def.GetUISprite(element as Element, "ui", false);
		SelectorValue.SearchFilter[] obj = new SelectorValue.SearchFilter[4]
		{
			new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.COMMON, condition, null, null),
			null,
			null,
			null
		};
		string name = UI.SANDBOXTOOLS.FILTERS.SOLID;
		Func<object, bool> condition2 = func;
		Tuple<Sprite, Color> uISprite = Def.GetUISprite(ElementLoader.FindElementByHash(SimHashes.SandStone), "ui", false);
		obj[1] = new SelectorValue.SearchFilter(name, condition2, null, uISprite);
		name = UI.SANDBOXTOOLS.FILTERS.LIQUID;
		condition2 = func2;
		uISprite = Def.GetUISprite(ElementLoader.FindElementByHash(SimHashes.Water), "ui", false);
		obj[2] = new SelectorValue.SearchFilter(name, condition2, null, uISprite);
		name = UI.SANDBOXTOOLS.FILTERS.GAS;
		condition2 = func3;
		uISprite = Def.GetUISprite(ElementLoader.FindElementByHash(SimHashes.Oxygen), "ui", false);
		obj[3] = new SelectorValue.SearchFilter(name, condition2, null, uISprite);
		elementSelector = new SelectorValue(options, onValueChanged, getOptionName, filterOptionFunction, getOptionSprite, obj);
	}

	private void ConfigureEntitySelector()
	{
		List<SelectorValue.SearchFilter> list = new List<SelectorValue.SearchFilter>();
		string name = UI.SANDBOXTOOLS.FILTERS.ENTITIES.FOOD;
		Func<object, bool> condition = delegate(object entity)
		{
			string idString = (entity as KPrefabID).PrefabID().ToString();
			return !(entity as KPrefabID).HasTag(GameTags.Egg) && FOOD.FOOD_TYPES_LIST.Find((EdiblesManager.FoodInfo match) => match.Id == idString) != null;
		};
		Tuple<Sprite, Color> uISprite = Def.GetUISprite(Assets.GetPrefab("MushBar"), "ui", false);
		SelectorValue.SearchFilter item = new SelectorValue.SearchFilter(name, condition, null, uISprite);
		list.Add(item);
		name = UI.SANDBOXTOOLS.FILTERS.ENTITIES.SPECIAL;
		condition = ((object entity) => (entity as KPrefabID).PrefabID().Name == MinionConfig.ID || (entity as KPrefabID).PrefabID().Name == DustCometConfig.ID || (entity as KPrefabID).PrefabID().Name == RockCometConfig.ID || (entity as KPrefabID).PrefabID().Name == IronCometConfig.ID);
		uISprite = new Tuple<Sprite, Color>(Assets.GetSprite("ui_duplicant_portrait_placeholder"), Color.white);
		SelectorValue.SearchFilter item2 = new SelectorValue.SearchFilter(name, condition, null, uISprite);
		list.Add(item2);
		SelectorValue.SearchFilter searchFilter = null;
		SelectorValue.SearchFilter searchFilter2 = null;
		name = UI.SANDBOXTOOLS.FILTERS.ENTITIES.CREATURE;
		condition = ((object entity) => false);
		uISprite = Def.GetUISprite(Assets.GetPrefab("Hatch"), "ui", false);
		searchFilter = new SelectorValue.SearchFilter(name, condition, null, uISprite);
		list.Add(searchFilter);
		List<Tag> list2 = new List<Tag>();
		foreach (GameObject item6 in Assets.GetPrefabsWithTag("CreatureBrain".ToTag()))
		{
			CreatureBrain brain = item6.GetComponent<CreatureBrain>();
			if (!list2.Contains(brain.species))
			{
				Tuple<Sprite, Color> icon = new Tuple<Sprite, Color>(CodexCache.entries[brain.species.ToString().ToUpper()].icon, CodexCache.entries[brain.species.ToString().ToUpper()].iconColor);
				list2.Add(brain.species);
				string key = "STRINGS.CREATURES.FAMILY_PLURAL." + brain.species.ToString().ToUpper();
				SelectorValue.SearchFilter item3 = new SelectorValue.SearchFilter(Strings.Get(key), delegate(object entity)
				{
					CreatureBrain component2 = Assets.GetPrefab((entity as KPrefabID).PrefabID()).GetComponent<CreatureBrain>();
					return (entity as KPrefabID).HasTag("CreatureBrain".ToString()) && component2.species == brain.species;
				}, searchFilter, icon);
				list.Add(item3);
			}
		}
		searchFilter2 = new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.ENTITIES.CREATURE_EGG, (object entity) => (entity as KPrefabID).HasTag(GameTags.Egg), searchFilter, Def.GetUISprite(Assets.GetPrefab("HatchEgg"), "ui", false));
		list.Add(searchFilter2);
		name = UI.SANDBOXTOOLS.FILTERS.ENTITIES.EQUIPMENT;
		condition = delegate(object entity)
		{
			if ((UnityEngine.Object)(entity as KPrefabID).gameObject == (UnityEngine.Object)null)
			{
				return false;
			}
			GameObject gameObject3 = (entity as KPrefabID).gameObject;
			if ((UnityEngine.Object)gameObject3 != (UnityEngine.Object)null)
			{
				return (UnityEngine.Object)gameObject3.GetComponent<Equippable>() != (UnityEngine.Object)null;
			}
			return false;
		};
		uISprite = Def.GetUISprite(Assets.GetPrefab("Funky_Vest"), "ui", false);
		SelectorValue.SearchFilter item4 = new SelectorValue.SearchFilter(name, condition, null, uISprite);
		list.Add(item4);
		name = UI.SANDBOXTOOLS.FILTERS.ENTITIES.PLANTS;
		condition = delegate(object entity)
		{
			if ((UnityEngine.Object)(entity as KPrefabID).gameObject == (UnityEngine.Object)null)
			{
				return false;
			}
			GameObject gameObject2 = (entity as KPrefabID).gameObject;
			if ((UnityEngine.Object)gameObject2 != (UnityEngine.Object)null)
			{
				return (UnityEngine.Object)gameObject2.GetComponent<Harvestable>() != (UnityEngine.Object)null || (UnityEngine.Object)gameObject2.GetComponent<WiltCondition>() != (UnityEngine.Object)null;
			}
			return false;
		};
		uISprite = Def.GetUISprite(Assets.GetPrefab("PrickleFlower"), "ui", false);
		SelectorValue.SearchFilter searchFilter3 = new SelectorValue.SearchFilter(name, condition, null, uISprite);
		list.Add(searchFilter3);
		SelectorValue.SearchFilter item5 = new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.ENTITIES.SEEDS, delegate(object entity)
		{
			if ((UnityEngine.Object)(entity as KPrefabID).gameObject == (UnityEngine.Object)null)
			{
				return false;
			}
			GameObject gameObject = (entity as KPrefabID).gameObject;
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				return (UnityEngine.Object)gameObject.GetComponent<PlantableSeed>() != (UnityEngine.Object)null;
			}
			return false;
		}, searchFilter3, Def.GetUISprite(Assets.GetPrefab("PrickleFlowerSeed"), "ui", false));
		list.Add(item5);
		List<KPrefabID> list3 = new List<KPrefabID>();
		foreach (KPrefabID prefab2 in Assets.Prefabs)
		{
			foreach (SelectorValue.SearchFilter item7 in list)
			{
				if (item7.condition(prefab2))
				{
					list3.Add(prefab2);
					break;
				}
			}
		}
		entitySelector = new SelectorValue(list3.ToArray(), delegate(object entity)
		{
			settings.SelectEntity(entity as KPrefabID);
		}, (object entity) => (entity as KPrefabID).GetProperName(), delegate(string filterString, object option)
		{
			if ((option as KPrefabID).GetProperName().ToUpper().Contains(filterString.ToUpper()))
			{
				return true;
			}
			return false;
		}, delegate(object entity)
		{
			GameObject prefab = Assets.GetPrefab((entity as KPrefabID).PrefabTag);
			if ((UnityEngine.Object)prefab != (UnityEngine.Object)null)
			{
				if (prefab.PrefabID() == (Tag)MinionConfig.ID)
				{
					return new Tuple<Sprite, Color>(Assets.GetSprite("ui_duplicant_portrait_placeholder"), Color.white);
				}
				KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.AnimFiles.Length > 0)
				{
					KAnimFile x = component.AnimFiles[0];
					if ((UnityEngine.Object)x != (UnityEngine.Object)null)
					{
						return Def.GetUISprite(prefab, "ui", false);
					}
				}
			}
			return null;
		}, list.ToArray());
	}

	private void ConfigureDiseaseSelector()
	{
		diseaseSelector = new SelectorValue(Db.Get().Diseases.resources.ToArray(), delegate(object disease)
		{
			settings.SelectDisease(disease as Disease);
		}, (object disease) => (disease as Disease).Name, delegate(string filterText, object option)
		{
			if ((option as Disease).Name.ToUpper().Contains(filterText.ToUpper()))
			{
				return true;
			}
			return false;
		}, (object disease) => new Tuple<Sprite, Color>(Assets.GetSprite("germ"), (disease as Disease).overlayColour), null);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if ((UnityEngine.Object)PlayerController.Instance.ActiveTool != (UnityEngine.Object)null && (UnityEngine.Object)instance != (UnityEngine.Object)null)
		{
			RefreshDisplay();
		}
	}

	public void RefreshDisplay()
	{
		brushRadiusSlider.row.SetActive(PlayerController.Instance.ActiveTool is BrushTool);
		if (PlayerController.Instance.ActiveTool is BrushTool)
		{
			brushRadiusSlider.SetValue((float)settings.BrushSize);
		}
		massSlider.SetValue(settings.Mass);
		temperatureSlider.SetValue(settings.temperature);
		temperatureAdditiveSlider.SetValue(settings.temperatureAdditive);
		diseaseCountSlider.SetValue((float)settings.diseaseCount);
	}

	private GameObject SpawnSelector(SelectorValue selector)
	{
		GameObject gameObject = Util.KInstantiateUI(selectorPropertyPrefab, base.gameObject, true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		GameObject panel = component.GetReference("ScrollPanel").gameObject;
		GameObject gameObject2 = component.GetReference("Content").gameObject;
		InputField reference = component.GetReference<InputField>("Filter");
		KButton reference2 = component.GetReference<KButton>("Button");
		reference2.onClick += delegate
		{
			panel.SetActive(!panel.activeSelf);
			if (panel.activeSelf)
			{
				panel.GetComponent<KScrollRect>().verticalNormalizedPosition = 1f;
			}
		};
		GameObject gameObject3 = component.GetReference("optionPrefab").gameObject;
		selector.row = gameObject;
		selector.optionButtons = new List<KeyValuePair<object, GameObject>>();
		if (selector.filters != null)
		{
			GameObject clearFilterButton = Util.KInstantiateUI(gameObject3, gameObject2, false);
			clearFilterButton.GetComponentInChildren<LocText>().text = UI.SANDBOXTOOLS.FILTERS.BACK;
			clearFilterButton.GetComponentsInChildren<Image>()[1].enabled = false;
			clearFilterButton.GetComponent<KButton>().onClick += delegate
			{
				selector.currentFilter = null;
				selector.optionButtons.ForEach(delegate(KeyValuePair<object, GameObject> test)
				{
					if (test.Key is SelectorValue.SearchFilter)
					{
						test.Value.SetActive((test.Key as SelectorValue.SearchFilter).parentFilter == null);
					}
					else
					{
						test.Value.SetActive(false);
					}
				});
				clearFilterButton.SetActive(false);
				panel.GetComponent<KScrollRect>().verticalNormalizedPosition = 1f;
			};
			SelectorValue.SearchFilter[] filters = selector.filters;
			foreach (SelectorValue.SearchFilter filter in filters)
			{
				GameObject gameObject4 = Util.KInstantiateUI(gameObject3, gameObject2, false);
				gameObject4.SetActive(filter.parentFilter == null);
				gameObject4.GetComponentInChildren<LocText>().text = filter.Name;
				if (filter.icon != null)
				{
					gameObject4.GetComponentsInChildren<Image>()[1].sprite = filter.icon.first;
					gameObject4.GetComponentsInChildren<Image>()[1].color = filter.icon.second;
				}
				gameObject4.GetComponent<KButton>().onClick += delegate
				{
					selector.currentFilter = filter;
					clearFilterButton.SetActive(true);
					selector.optionButtons.ForEach(delegate(KeyValuePair<object, GameObject> test)
					{
						if (!(test.Key is SelectorValue.SearchFilter))
						{
							test.Value.SetActive(selector.runCurrentFilter(test.Key));
						}
						else if ((test.Key as SelectorValue.SearchFilter).parentFilter == null)
						{
							test.Value.SetActive(false);
						}
						else
						{
							test.Value.SetActive((test.Key as SelectorValue.SearchFilter).parentFilter == filter);
						}
					});
					panel.GetComponent<KScrollRect>().verticalNormalizedPosition = 1f;
				};
				selector.optionButtons.Add(new KeyValuePair<object, GameObject>(filter, gameObject4));
			}
		}
		object[] options = selector.options;
		foreach (object option in options)
		{
			GameObject gameObject5 = Util.KInstantiateUI(gameObject3, gameObject2, true);
			gameObject5.GetComponentInChildren<LocText>().text = selector.getOptionName(option);
			gameObject5.GetComponent<KButton>().onClick += delegate
			{
				selector.onValueChanged(option);
				panel.SetActive(false);
			};
			Tuple<Sprite, Color> tuple = selector.getOptionSprite(option);
			gameObject5.GetComponentsInChildren<Image>()[1].sprite = tuple.first;
			gameObject5.GetComponentsInChildren<Image>()[1].color = tuple.second;
			selector.optionButtons.Add(new KeyValuePair<object, GameObject>(option, gameObject5));
			if (option is SelectorValue.SearchFilter)
			{
				gameObject5.SetActive((option as SelectorValue.SearchFilter).parentFilter == null);
			}
			else
			{
				gameObject5.SetActive(false);
			}
		}
		selector.button = reference2;
		reference.onValueChanged.AddListener(delegate(string filterString)
		{
			List<KeyValuePair<object, GameObject>> list = new List<KeyValuePair<object, GameObject>>();
			selector.optionButtons.ForEach(delegate(KeyValuePair<object, GameObject> test)
			{
				if (test.Key is SelectorValue.SearchFilter)
				{
					test.Value.SetActive((test.Key as SelectorValue.SearchFilter).Name.ToUpper().Contains(filterString.ToUpper()));
				}
			});
			object[] options2 = selector.options;
			foreach (object option2 in options2)
			{
				list = selector.optionButtons.FindAll((KeyValuePair<object, GameObject> match) => match.Key == option2);
				foreach (KeyValuePair<object, GameObject> item in list)
				{
					if (filterString == string.Empty)
					{
						item.Value.SetActive(false);
					}
					else
					{
						item.Value.SetActive(selector.filterOptionFunction(filterString, option2));
					}
				}
			}
			panel.GetComponent<KScrollRect>().verticalNormalizedPosition = 1f;
		});
		inputFields.Add(reference.gameObject);
		panel.SetActive(false);
		return gameObject;
	}

	private GameObject SpawnSlider(SliderValue value)
	{
		GameObject gameObject = Util.KInstantiateUI(sliderPropertyPrefab, base.gameObject, true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("BottomIcon").sprite = Assets.GetSprite(value.bottomSprite);
		component.GetReference<Image>("TopIcon").sprite = Assets.GetSprite(value.topSprite);
		KSlider slider = component.GetReference<KSlider>("Slider");
		KNumberInputField inputField = component.GetReference<KNumberInputField>("InputField");
		gameObject.GetComponent<ToolTip>().SetSimpleTooltip(value.tooltip);
		slider.minValue = value.minValue;
		slider.maxValue = value.maxValue;
		inputField.minValue = 0f;
		inputField.maxValue = 99999f;
		inputFields.Add(inputField.gameObject);
		value.slider = slider;
		value.inputField = inputField;
		value.row = gameObject;
		slider.onReleaseHandle += delegate
		{
			slider.value = Mathf.Round(slider.value * 10f) / 10f;
			inputField.currentValue = slider.value;
			inputField.SetDisplayValue(inputField.currentValue.ToString());
			if (value.onValueChanged != null)
			{
				value.onValueChanged(slider.value);
			}
		};
		slider.onDrag += delegate
		{
			slider.value = Mathf.Round(slider.value * 10f) / 10f;
			inputField.currentValue = slider.value;
			inputField.SetDisplayValue(inputField.currentValue.ToString());
			if (value.onValueChanged != null)
			{
				value.onValueChanged(slider.value);
			}
		};
		slider.onMove += delegate
		{
			slider.value = Mathf.Round(slider.value * 10f) / 10f;
			inputField.currentValue = slider.value;
			inputField.SetDisplayValue(inputField.currentValue.ToString());
			if (value.onValueChanged != null)
			{
				value.onValueChanged(slider.value);
			}
		};
		inputField.onEndEdit += delegate
		{
			float num = Mathf.Clamp(Mathf.Round(inputField.currentValue), inputField.minValue, inputField.maxValue);
			inputField.SetDisplayValue(num.ToString());
			slider.value = Mathf.Round(num);
			if (value.onValueChanged != null)
			{
				value.onValueChanged(num);
			}
		};
		component.GetReference<LocText>("UnitLabel").text = value.unitString;
		return gameObject;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (CheckBlockedInput())
		{
			if (!e.Consumed)
			{
				e.Consumed = true;
			}
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private bool CheckBlockedInput()
	{
		bool result = false;
		if ((UnityEngine.Object)UnityEngine.EventSystems.EventSystem.current != (UnityEngine.Object)null)
		{
			GameObject currentSelectedGameObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
			if ((UnityEngine.Object)currentSelectedGameObject != (UnityEngine.Object)null)
			{
				foreach (GameObject inputField in inputFields)
				{
					if ((UnityEngine.Object)currentSelectedGameObject == (UnityEngine.Object)inputField.gameObject)
					{
						return true;
					}
				}
				return result;
			}
		}
		return result;
	}
}
