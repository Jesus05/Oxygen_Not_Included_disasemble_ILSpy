using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceCategoryHeader : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	[Serializable]
	public struct ElementReferences
	{
		public LocText LabelText;

		public LocText QuantityText;
	}

	public GameObject Prefab_ResourceEntry;

	public Transform EntryContainer;

	public Tag ResourceCategoryTag;

	public GameUtil.MeasureUnit Measure;

	public bool IsOpen;

	public ImageToggleState expandArrow;

	private Button mButton;

	public Dictionary<Tag, ResourceEntry> ResourcesDiscovered = new Dictionary<Tag, ResourceEntry>();

	public ElementReferences elements;

	public Color TextColor_Interactable;

	public Color TextColor_NonInteractable;

	private string quantityString;

	private float currentQuantity;

	private bool anyDiscovered;

	[MyCmpGet]
	private ToolTip tooltip;

	[SerializeField]
	private int minimizedFontSize;

	[SerializeField]
	private int maximizedFontSize;

	[SerializeField]
	private Color highlightColour;

	[SerializeField]
	private Color BackgroundHoverColor;

	[SerializeField]
	private Image Background;

	private float cachedAvailable = -3.40282347E+38f;

	private float cachedTotal = -3.40282347E+38f;

	private float cachedReserved = -3.40282347E+38f;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		EntryContainer.SetParent(base.transform.parent);
		EntryContainer.SetSiblingIndex(base.transform.GetSiblingIndex() + 1);
		EntryContainer.localScale = Vector3.one;
		mButton = GetComponent<Button>();
		mButton.onClick.AddListener(delegate
		{
			ToggleOpen(true);
		});
		SetInteractable(anyDiscovered);
		SetActiveColor(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		tooltip.OnToolTip = OnTooltip;
		UpdateContents();
	}

	private void SetInteractable(bool state)
	{
		if (state)
		{
			if (!IsOpen)
			{
				expandArrow.SetInactive();
			}
			else
			{
				expandArrow.SetActive();
			}
		}
		else
		{
			SetOpen(false);
			expandArrow.SetDisabled();
		}
	}

	private void SetActiveColor(bool state)
	{
		if (state)
		{
			elements.QuantityText.color = TextColor_Interactable;
			elements.LabelText.color = TextColor_Interactable;
			expandArrow.ActiveColour = TextColor_Interactable;
			expandArrow.InactiveColour = TextColor_Interactable;
			expandArrow.TargetImage.color = TextColor_Interactable;
		}
		else
		{
			elements.LabelText.color = TextColor_NonInteractable;
			elements.QuantityText.color = TextColor_NonInteractable;
			expandArrow.ActiveColour = TextColor_NonInteractable;
			expandArrow.InactiveColour = TextColor_NonInteractable;
			expandArrow.TargetImage.color = TextColor_NonInteractable;
		}
	}

	public void SetTag(Tag t, GameUtil.MeasureUnit measure)
	{
		ResourceCategoryTag = t;
		Measure = measure;
		elements.LabelText.text = t.ProperName();
		if (SaveGame.Instance.expandedResourceTags.Contains(ResourceCategoryTag))
		{
			anyDiscovered = true;
			ToggleOpen(false);
		}
	}

	private void ToggleOpen(bool play_sound)
	{
		if (!anyDiscovered)
		{
			if (play_sound)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
			}
		}
		else if (!IsOpen)
		{
			if (play_sound)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open", false));
			}
			SetOpen(true);
			elements.LabelText.fontSize = (float)maximizedFontSize;
			elements.QuantityText.fontSize = (float)maximizedFontSize;
		}
		else
		{
			if (play_sound)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
			}
			SetOpen(false);
			elements.LabelText.fontSize = (float)minimizedFontSize;
			elements.QuantityText.fontSize = (float)minimizedFontSize;
		}
	}

	private void Hover(bool is_hovering)
	{
		Background.color = ((!is_hovering) ? new Color(0f, 0f, 0f, 0f) : BackgroundHoverColor);
		List<Pickupable> list = null;
		if ((UnityEngine.Object)WorldInventory.Instance != (UnityEngine.Object)null)
		{
			list = WorldInventory.Instance.GetPickupables(ResourceCategoryTag);
		}
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (!((UnityEngine.Object)list[i] == (UnityEngine.Object)null))
				{
					KAnimControllerBase component = list[i].GetComponent<KAnimControllerBase>();
					if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
					{
						component.HighlightColour = ((!is_hovering) ? Color.black : highlightColour);
					}
				}
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Hover(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Hover(false);
	}

	public void SetOpen(bool open)
	{
		IsOpen = open;
		if (open)
		{
			expandArrow.SetActive();
			if (!SaveGame.Instance.expandedResourceTags.Contains(ResourceCategoryTag))
			{
				SaveGame.Instance.expandedResourceTags.Add(ResourceCategoryTag);
			}
		}
		else
		{
			expandArrow.SetInactive();
			SaveGame.Instance.expandedResourceTags.Remove(ResourceCategoryTag);
		}
		EntryContainer.gameObject.SetActive(IsOpen);
	}

	private void GetAmounts(bool doExtras, out float available, out float total, out float reserved)
	{
		available = 0f;
		total = 0f;
		reserved = 0f;
		HashSet<Tag> resources = null;
		if (WorldInventory.Instance.TryGetDiscoveredResourcesFromTag(ResourceCategoryTag, out resources))
		{
			foreach (Tag item in resources)
			{
				anyDiscovered = true;
				if (!ResourcesDiscovered.ContainsKey(item))
				{
					ResourcesDiscovered.Add(item, NewResourceEntry(item, Measure));
				}
				float num = WorldInventory.Instance.GetAmount(item);
				float num2 = (!doExtras) ? 0f : WorldInventory.Instance.GetTotalAmount(item);
				float num3 = (!doExtras) ? 0f : MaterialNeeds.Instance.GetAmount(item);
				if (Measure == GameUtil.MeasureUnit.kcal)
				{
					EdiblesManager.FoodInfo foodInfo = Game.Instance.ediblesManager.GetFoodInfo(item.Name);
					num *= foodInfo.CaloriesPerUnit;
					num2 *= foodInfo.CaloriesPerUnit;
					num3 *= foodInfo.CaloriesPerUnit;
				}
				available += num;
				total += num2;
				reserved += num3;
			}
		}
	}

	public void UpdateContents()
	{
		GetAmounts(false, out float available, out float total, out float reserved);
		if (available != cachedAvailable || total != cachedTotal || reserved != cachedReserved)
		{
			if (quantityString == null || currentQuantity != available)
			{
				switch (Measure)
				{
				case GameUtil.MeasureUnit.mass:
					quantityString = GameUtil.GetFormattedMass(available, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
					break;
				case GameUtil.MeasureUnit.quantity:
					quantityString = available.ToString();
					break;
				case GameUtil.MeasureUnit.kcal:
					quantityString = GameUtil.GetFormattedCalories(available, GameUtil.TimeSlice.None, true);
					break;
				}
				elements.QuantityText.text = quantityString;
				currentQuantity = available;
			}
			cachedAvailable = available;
			cachedTotal = total;
			cachedReserved = reserved;
		}
		foreach (KeyValuePair<Tag, ResourceEntry> item in ResourcesDiscovered)
		{
			item.Value.UpdateValue();
		}
		SetActiveColor(available > 0f);
		SetInteractable(anyDiscovered);
	}

	private string OnTooltip()
	{
		GetAmounts(true, out float available, out float total, out float reserved);
		string str = elements.LabelText.text + "\n";
		return str + string.Format(UI.RESOURCESCREEN.AVAILABLE_TOOLTIP, ResourceCategoryScreen.QuantityTextForMeasure(available, Measure), ResourceCategoryScreen.QuantityTextForMeasure(reserved, Measure), ResourceCategoryScreen.QuantityTextForMeasure(total, Measure));
	}

	private ResourceEntry NewResourceEntry(Tag resourceTag, GameUtil.MeasureUnit measure)
	{
		GameObject gameObject = Util.KInstantiateUI(Prefab_ResourceEntry, EntryContainer.gameObject, true);
		ResourceEntry component = gameObject.GetComponent<ResourceEntry>();
		component.SetTag(resourceTag, measure);
		return component;
	}
}
