using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameDisplayScreen : KScreen
{
	[Serializable]
	public class Entry
	{
		public string Name;

		public GameObject world_go;

		public GameObject display_go;

		public GameObject bars_go;

		public HealthBar healthBar;

		public ProgressBar breathBar;

		public ProgressBar suitBar;

		public ProgressBar suitFuelBar;

		public HierarchyReferences thoughtBubble;

		public HierarchyReferences thoughtBubbleConvo;

		public HierarchyReferences refs;
	}

	[SerializeField]
	private float HideDistance;

	public static NameDisplayScreen Instance;

	public GameObject nameAndBarsPrefab;

	public GameObject barsPrefab;

	public TextStyleSetting ToolTipStyle_Property;

	[SerializeField]
	private Color selectedColor;

	[SerializeField]
	private Color defaultColor;

	public int fontsize_min = 14;

	public int fontsize_max = 32;

	public float cameraDistance_fontsize_min = 6f;

	public float cameraDistance_fontsize_max = 4f;

	public List<Entry> entries = new List<Entry>();

	public bool worldSpace = true;

	private List<KCollider2D> workingList = new List<KCollider2D>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UIRegistry.nameDisplayScreen = this;
		Components.Health.Register(delegate(Health health)
		{
			RegisterComponent(health.gameObject, health, false);
		}, null);
		Components.Equipment.Register(delegate(Equipment equipment)
		{
			RegisterComponent(equipment.gameObject, equipment, false);
		}, null);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	private bool ShouldShowName(GameObject representedObject)
	{
		bool flag = (UnityEngine.Object)representedObject.GetComponent<MinionBrain>() != (UnityEngine.Object)null;
		bool flag2 = (UnityEngine.Object)representedObject.GetComponent<CommandModule>() != (UnityEngine.Object)null;
		return flag || flag2;
	}

	public void AddNewEntry(GameObject representedObject)
	{
		Entry entry = new Entry();
		entry.world_go = representedObject;
		GameObject original = (!ShouldShowName(representedObject)) ? barsPrefab : nameAndBarsPrefab;
		GameObject gameObject = entry.display_go = Util.KInstantiateUI(original, base.gameObject, true);
		if (worldSpace)
		{
			entry.display_go.transform.localScale = Vector3.one * 0.01f;
		}
		gameObject.name = representedObject.name + " character overlay";
		entry.Name = representedObject.name;
		entry.refs = gameObject.GetComponent<HierarchyReferences>();
		entries.Add(entry);
		KSelectable component = representedObject.GetComponent<KSelectable>();
		FactionAlignment component2 = representedObject.GetComponent<FactionAlignment>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				if (component2.Alignment == FactionManager.FactionID.Friendly || component2.Alignment == FactionManager.FactionID.Duplicant)
				{
					UpdateName(representedObject);
				}
			}
			else
			{
				UpdateName(representedObject);
			}
		}
	}

	public void RegisterComponent(GameObject representedObject, object component, bool force_new_entry = false)
	{
		Entry entry = (!force_new_entry) ? GetEntry(representedObject) : null;
		if (entry == null)
		{
			CharacterOverlay component2 = representedObject.GetComponent<CharacterOverlay>();
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				component2.Register();
				entry = GetEntry(representedObject);
			}
		}
		if (entry != null)
		{
			Transform reference = entry.refs.GetReference<Transform>("Bars");
			entry.bars_go = reference.gameObject;
			if (component is Health)
			{
				if (!(bool)entry.healthBar)
				{
					Health health = (Health)component;
					GameObject gameObject = Util.KInstantiateUI(ProgressBarsConfig.Instance.healthBarPrefab, reference.gameObject, false);
					gameObject.name = "Health Bar";
					health.healthBar = gameObject.GetComponent<HealthBar>();
					health.healthBar.GetComponent<KSelectable>().entityName = UI.METERS.HEALTH.TOOLTIP;
					health.healthBar.GetComponent<KSelectableHealthBar>().IsSelectable = ((UnityEngine.Object)representedObject.GetComponent<MinionBrain>() != (UnityEngine.Object)null);
					entry.healthBar = health.healthBar;
					entry.healthBar.autoHide = false;
					gameObject.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("HealthBar");
				}
				else
				{
					Debug.LogWarningFormat("Health added twice {0}", component);
				}
			}
			else if (component is OxygenBreather)
			{
				if (!(bool)entry.breathBar)
				{
					GameObject gameObject2 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, reference.gameObject, false);
					entry.breathBar = gameObject2.GetComponent<ProgressBar>();
					entry.breathBar.autoHide = false;
					gameObject2.gameObject.GetComponent<ToolTip>().AddMultiStringTooltip("Breath", ToolTipStyle_Property);
					gameObject2.name = "Breath Bar";
					gameObject2.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("BreathBar");
					gameObject2.GetComponent<KSelectable>().entityName = UI.METERS.BREATH.TOOLTIP;
				}
				else
				{
					Debug.LogWarningFormat("OxygenBreather added twice {0}", component);
				}
			}
			else if (component is Equipment)
			{
				if (!(bool)entry.suitBar)
				{
					GameObject gameObject3 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, reference.gameObject, false);
					entry.suitBar = gameObject3.GetComponent<ProgressBar>();
					entry.suitBar.autoHide = false;
					gameObject3.name = "Suit Tank Bar";
					gameObject3.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("OxygenTankBar");
					gameObject3.GetComponent<KSelectable>().entityName = UI.METERS.BREATH.TOOLTIP;
				}
				else
				{
					Debug.LogWarningFormat("SuitBar added twice {0}", component);
				}
				if (!(bool)entry.suitFuelBar)
				{
					GameObject gameObject4 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, reference.gameObject, false);
					entry.suitFuelBar = gameObject4.GetComponent<ProgressBar>();
					entry.suitFuelBar.autoHide = false;
					gameObject4.name = "Suit Fuel Bar";
					gameObject4.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("FuelTankBar");
					gameObject4.GetComponent<KSelectable>().entityName = UI.METERS.FUEL.TOOLTIP;
				}
				else
				{
					Debug.LogWarningFormat("FuelBar added twice {0}", component);
				}
			}
			else if (component is ThoughtGraph.Instance)
			{
				if (!(bool)entry.thoughtBubble)
				{
					GameObject gameObject5 = Util.KInstantiateUI(EffectPrefabs.Instance.ThoughtBubble, entry.display_go, false);
					entry.thoughtBubble = gameObject5.GetComponent<HierarchyReferences>();
					gameObject5.name = "Thought Bubble";
					GameObject gameObject6 = Util.KInstantiateUI(EffectPrefabs.Instance.ThoughtBubbleConvo, entry.display_go, false);
					entry.thoughtBubbleConvo = gameObject6.GetComponent<HierarchyReferences>();
					gameObject6.name = "Thought Bubble Convo";
				}
				else
				{
					Debug.LogWarningFormat("ThoughtGraph added twice {0}", component);
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (!App.isLoading && !App.IsExiting)
		{
			SimViewMode simViewMode = SimViewMode.None;
			if ((UnityEngine.Object)OverlayScreen.Instance != (UnityEngine.Object)null)
			{
				simViewMode = OverlayScreen.Instance.GetMode();
			}
			bool flag = !((UnityEngine.Object)Camera.main == (UnityEngine.Object)null) && Camera.main.orthographicSize < HideDistance && simViewMode == SimViewMode.None;
			int num = entries.Count;
			int num2 = 0;
			while (num2 < num)
			{
				if ((UnityEngine.Object)entries[num2].world_go != (UnityEngine.Object)null)
				{
					Vector3 vector = entries[num2].world_go.transform.GetPosition();
					if (flag && CameraController.Instance.IsVisiblePos(vector))
					{
						RectTransform component = entries[num2].display_go.GetComponent<RectTransform>();
						if ((UnityEngine.Object)CameraController.Instance != (UnityEngine.Object)null && (UnityEngine.Object)CameraController.Instance.followTarget == (UnityEngine.Object)entries[num2].world_go.transform)
						{
							vector = CameraController.Instance.followTargetPos;
						}
						else
						{
							KAnimControllerBase component2 = entries[num2].world_go.GetComponent<KAnimControllerBase>();
							if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
							{
								vector = component2.GetWorldPivot();
							}
						}
						component.anchoredPosition = ((!worldSpace) ? WorldToScreen(vector) : vector);
						entries[num2].display_go.SetActive(true);
					}
					else if (entries[num2].display_go.activeSelf)
					{
						entries[num2].display_go.SetActive(false);
					}
					if (entries[num2].world_go.HasTag(GameTags.Dead))
					{
						entries[num2].bars_go.SetActive(false);
					}
					if ((UnityEngine.Object)entries[num2].bars_go != (UnityEngine.Object)null)
					{
						GameObject bars_go = entries[num2].bars_go;
						bars_go.GetComponentsInChildren(false, workingList);
						foreach (KCollider2D working in workingList)
						{
							working.MarkDirty(false);
						}
					}
					num2++;
				}
				else
				{
					UnityEngine.Object.Destroy(entries[num2].display_go);
					num--;
					entries[num2] = entries[num];
				}
			}
			entries.RemoveRange(num, entries.Count - num);
		}
	}

	public void UpdateName(GameObject representedObject)
	{
		Entry entry = GetEntry(representedObject);
		if (entry != null)
		{
			KSelectable component = representedObject.GetComponent<KSelectable>();
			entry.display_go.name = component.GetProperName() + " character overlay";
			LocText componentInChildren = entry.display_go.GetComponentInChildren<LocText>();
			if ((UnityEngine.Object)componentInChildren != (UnityEngine.Object)null)
			{
				componentInChildren.text = component.GetProperName();
				if ((UnityEngine.Object)representedObject.GetComponent<RocketModule>() != (UnityEngine.Object)null)
				{
					componentInChildren.text = representedObject.GetComponent<RocketModule>().GetParentRocketName();
				}
			}
		}
	}

	public void SetThoughtBubbleDisplay(GameObject minion_go, bool bVisible, string hover_text, Sprite bubble_sprite, Sprite topic_sprite)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !((UnityEngine.Object)entry.thoughtBubble == (UnityEngine.Object)null))
		{
			ApplyThoughtSprite(entry.thoughtBubble, bubble_sprite, "bubble_sprite");
			ApplyThoughtSprite(entry.thoughtBubble, topic_sprite, "icon_sprite");
			entry.thoughtBubble.GetComponent<KSelectable>().entityName = hover_text;
			entry.thoughtBubble.gameObject.SetActive(bVisible);
		}
	}

	public void SetThoughtBubbleConvoDisplay(GameObject minion_go, bool bVisible, string hover_text, Sprite bubble_sprite, Sprite topic_sprite, Sprite mode_sprite)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !((UnityEngine.Object)entry.thoughtBubble == (UnityEngine.Object)null))
		{
			ApplyThoughtSprite(entry.thoughtBubbleConvo, bubble_sprite, "bubble_sprite");
			ApplyThoughtSprite(entry.thoughtBubbleConvo, topic_sprite, "icon_sprite");
			ApplyThoughtSprite(entry.thoughtBubbleConvo, mode_sprite, "icon_sprite_mode");
			entry.thoughtBubbleConvo.GetComponent<KSelectable>().entityName = hover_text;
			entry.thoughtBubbleConvo.gameObject.SetActive(bVisible);
		}
	}

	private void ApplyThoughtSprite(HierarchyReferences active_bubble, Sprite sprite, string target)
	{
		Image reference = active_bubble.GetReference<Image>(target);
		reference.sprite = sprite;
	}

	public void SetBreathDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !((UnityEngine.Object)entry.breathBar == (UnityEngine.Object)null))
		{
			entry.breathBar.SetUpdateFunc(updatePercentFull);
			entry.breathBar.gameObject.SetActive(bVisible);
		}
	}

	public void SetHealthDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !((UnityEngine.Object)entry.healthBar == (UnityEngine.Object)null))
		{
			entry.healthBar.OnChange();
			entry.healthBar.SetUpdateFunc(updatePercentFull);
			if (entry.healthBar.gameObject.activeSelf != bVisible)
			{
				entry.healthBar.gameObject.SetActive(bVisible);
			}
		}
	}

	public void SetSuitTankDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !((UnityEngine.Object)entry.suitBar == (UnityEngine.Object)null))
		{
			entry.suitBar.SetUpdateFunc(updatePercentFull);
			entry.suitBar.gameObject.SetActive(bVisible);
		}
	}

	public void SetSuitFuelDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !((UnityEngine.Object)entry.suitFuelBar == (UnityEngine.Object)null))
		{
			entry.suitFuelBar.SetUpdateFunc(updatePercentFull);
			entry.suitFuelBar.gameObject.SetActive(bVisible);
		}
	}

	private Entry GetEntry(GameObject worldObject)
	{
		return entries.Find((Entry entry) => (UnityEngine.Object)entry.world_go == (UnityEngine.Object)worldObject);
	}
}
