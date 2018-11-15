using Klei;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceEntry : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public Tag Resource;

	public GameUtil.MeasureUnit Measure;

	public LocText NameLabel;

	public LocText QuantityLabel;

	public Image image;

	[SerializeField]
	private Color AvailableColor;

	[SerializeField]
	private Color UnavailableColor;

	[SerializeField]
	private Color OverdrawnColor;

	[SerializeField]
	private Color HighlightColor;

	[SerializeField]
	private Color BackgroundHoverColor;

	[SerializeField]
	private Image Background;

	[MyCmpGet]
	private ToolTip tooltip;

	[MyCmpReq]
	private Button button;

	private int selectionIdx;

	private float currentQuantity = -3.40282347E+38f;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		QuantityLabel.color = AvailableColor;
		NameLabel.color = AvailableColor;
		button.onClick.AddListener(OnClick);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		tooltip.OnToolTip = OnToolTip;
	}

	private void OnClick()
	{
		List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(Resource);
		if (pickupables != null)
		{
			Pickupable pickupable = null;
			for (int i = 0; i < pickupables.Count; i++)
			{
				selectionIdx++;
				int index = selectionIdx % pickupables.Count;
				pickupable = pickupables[index];
				if ((Object)pickupable != (Object)null && !pickupable.HasTag(GameTags.StoredPrivate))
				{
					break;
				}
			}
			if ((Object)pickupable != (Object)null)
			{
				Transform transform = pickupable.transform;
				if ((Object)pickupable.storage != (Object)null)
				{
					transform = pickupable.storage.transform;
				}
				SelectTool.Instance.SelectAndFocus(transform.transform.GetPosition(), transform.GetComponent<KSelectable>(), Vector3.zero);
				for (int j = 0; j < pickupables.Count; j++)
				{
					Pickupable pickupable2 = pickupables[j];
					if ((Object)pickupable2 != (Object)null)
					{
						KAnimControllerBase component = pickupable2.GetComponent<KAnimControllerBase>();
						if ((Object)component != (Object)null)
						{
							component.HighlightColour = HighlightColor;
						}
					}
				}
			}
		}
	}

	private void GetAmounts(bool doExtras, out float available, out float total, out float reserved)
	{
		available = WorldInventory.Instance.GetAmount(Resource);
		total = ((!doExtras) ? 0f : WorldInventory.Instance.GetTotalAmount(Resource));
		reserved = ((!doExtras) ? 0f : MaterialNeeds.Instance.GetAmount(Resource));
		if (Measure == GameUtil.MeasureUnit.kcal)
		{
			EdiblesManager.FoodInfo foodInfo = Game.Instance.ediblesManager.GetFoodInfo(Resource.Name);
			available *= foodInfo.CaloriesPerUnit;
			total *= foodInfo.CaloriesPerUnit;
			reserved *= foodInfo.CaloriesPerUnit;
		}
	}

	public void UpdateValue()
	{
		SetName(Resource.ProperName());
		bool allowInsufficientMaterialBuild = GenericGameSettings.instance.allowInsufficientMaterialBuild;
		GetAmounts(allowInsufficientMaterialBuild, out float available, out float total, out float reserved);
		if (currentQuantity != available)
		{
			currentQuantity = available;
			QuantityLabel.text = ResourceCategoryScreen.QuantityTextForMeasure(available, Measure);
		}
		Color color = AvailableColor;
		if (reserved > total)
		{
			color = OverdrawnColor;
		}
		else if (available == 0f)
		{
			color = UnavailableColor;
		}
		if (QuantityLabel.color != color)
		{
			QuantityLabel.color = color;
		}
		if (NameLabel.color != color)
		{
			NameLabel.color = color;
		}
	}

	private string OnToolTip()
	{
		GetAmounts(true, out float available, out float total, out float reserved);
		string str = NameLabel.text + "\n";
		return str + string.Format(UI.RESOURCESCREEN.AVAILABLE_TOOLTIP, ResourceCategoryScreen.QuantityTextForMeasure(available, Measure), ResourceCategoryScreen.QuantityTextForMeasure(reserved, Measure), ResourceCategoryScreen.QuantityTextForMeasure(total, Measure));
	}

	public void SetName(string name)
	{
		NameLabel.text = name;
	}

	public void SetTag(Tag t, GameUtil.MeasureUnit measure)
	{
		Resource = t;
		Measure = measure;
	}

	private void Hover(bool is_hovering)
	{
		if (!((Object)WorldInventory.Instance == (Object)null))
		{
			if (is_hovering)
			{
				Background.color = BackgroundHoverColor;
			}
			else
			{
				Background.color = new Color(0f, 0f, 0f, 0f);
			}
			List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(Resource);
			if (pickupables != null)
			{
				for (int i = 0; i < pickupables.Count; i++)
				{
					if (!((Object)pickupables[i] == (Object)null))
					{
						KAnimControllerBase component = pickupables[i].GetComponent<KAnimControllerBase>();
						if (!((Object)component == (Object)null))
						{
							if (is_hovering)
							{
								component.HighlightColour = HighlightColor;
							}
							else
							{
								component.HighlightColour = Color.black;
							}
						}
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

	public void SetSprite(Tag t)
	{
		Element element = ElementLoader.GetElement(Resource.Name);
		if (element != null)
		{
			Sprite uISpriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(element.substance.anim, "ui", false);
			if ((Object)uISpriteFromMultiObjectAnim != (Object)null)
			{
				image.sprite = uISpriteFromMultiObjectAnim;
			}
		}
	}

	public void SetSprite(Sprite sprite)
	{
		image.sprite = sprite;
	}
}
