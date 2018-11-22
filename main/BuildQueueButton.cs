using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildQueueButton : KMonoBehaviour
{
	[SerializeField]
	private Image texture;

	[SerializeField]
	private GameObject closeImg;

	[SerializeField]
	private GameObject infiniteImg;

	[SerializeField]
	private GameObject unavailableImg;

	[SerializeField]
	private Sprite emptyBG;

	[SerializeField]
	private Sprite filledBG;

	[SerializeField]
	private Color unavailableSpriteColor = new Color32(120, 120, 120, byte.MaxValue);

	[SerializeField]
	private Color unavailableBGColor = new Color32(120, 120, 120, byte.MaxValue);

	[SerializeField]
	private Color defaultBGColor = new Color32(135, 69, 102, byte.MaxValue);

	[SerializeField]
	private GameObject underwayIndicator;

	[SerializeField]
	private Image ButtonBG;

	[SerializeField]
	public GameObject jumpToOrderButton;

	public ToolTip toolTip;

	private IBuildQueueOrder order;

	private GameObject visualizer;

	protected override void OnSpawn()
	{
		int num = (int)texture.rectTransform.rect.width;
		int num2 = (int)texture.rectTransform.rect.height;
		if (num == 0 || num2 == 0)
		{
			LayoutElement component = GetComponent<LayoutElement>();
			num = (int)component.minWidth;
			num2 = (int)component.minHeight;
		}
		KButton componentInChildren = GetComponentInChildren<KButton>();
		componentInChildren.onClick += ButtonClicked;
		componentInChildren.onPointerEnter += PointerEntered;
		componentInChildren.onPointerExit += PointerLeft;
	}

	public void SetUnderwayIndicator(bool active)
	{
		underwayIndicator.SetActive(active);
	}

	public void SetJumpToOrderButtonActive(bool active)
	{
		jumpToOrderButton.SetActive(active);
	}

	public void SetAvailability(string recipeName, bool currentAvailability, string str)
	{
		str = ((!string.IsNullOrEmpty(str)) ? (recipeName + "\n" + str) : (recipeName + "\n"));
		texture.color = ((!currentAvailability) ? unavailableSpriteColor : order.IconColor);
		texture.GetComponent<Image>().material = ((!currentAvailability) ? GlobalResources.Instance().AnimMaterialUIDesaturated : null);
		ButtonBG.color = ((!currentAvailability) ? unavailableBGColor : Color.white);
		str = str + "\n" + UI.UISIDESCREENS.FABRICATORSIDESCREEN.CANCEL;
		toolTip.toolTip = str;
	}

	private void ButtonClicked()
	{
		if ((Object)ButtonBG.sprite == (Object)filledBG)
		{
			ResetGraphics();
		}
	}

	private void ResetGraphics()
	{
		ButtonBG.sprite = emptyBG;
		ButtonBG.color = defaultBGColor;
		texture.color = Color.white;
		closeImg.SetActive(false);
		infiniteImg.SetActive(false);
		SetUnderwayIndicator(false);
		SetJumpToOrderButtonActive(false);
	}

	private void PointerEntered()
	{
		if ((Object)ButtonBG.sprite == (Object)filledBG)
		{
			closeImg.SetActive(true);
		}
	}

	private void PointerLeft()
	{
		if ((Object)ButtonBG.sprite == (Object)filledBG && closeImg.activeInHierarchy)
		{
			closeImg.SetActive(false);
		}
	}

	private bool CheckMaterialAvailability(IBuildQueueOrder order, out string newTooltip)
	{
		newTooltip = "";
		Dictionary<Tag, float> dictionary = order.CheckMaterialRequirements();
		bool flag = true;
		foreach (KeyValuePair<Tag, float> item in dictionary)
		{
			bool flag2 = item.Value <= 0f;
			if (!flag2 && GameTags.LiquidElements.Contains(item.Key))
			{
				Element element = ElementLoader.GetElement(item.Key);
				if (element != null && LiquidPumpingStation.IsLiquidAccessible(element))
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				string formattedByTag = GameUtil.GetFormattedByTag(item.Key, item.Value, GameUtil.TimeSlice.None);
				newTooltip += string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.QUEUED_MISSING_INGREDIENTS_TOOLTIP, formattedByTag, item.Key.ProperName());
			}
			flag = (flag && flag2);
		}
		return flag;
	}

	public void SetOrder(IBuildQueueOrder order)
	{
		if (order == null)
		{
			toolTip.ClearMultiStringTooltip();
		}
		if (this.order != order)
		{
			ResetGraphics();
			if ((Object)visualizer != (Object)null)
			{
				Object.Destroy(visualizer);
				visualizer = null;
			}
			texture.enabled = false;
			if (order != null)
			{
				texture.enabled = true;
				texture.sprite = order.Icon;
				texture.color = order.IconColor;
				ButtonBG.sprite = filledBG;
				this.order = order;
			}
			else
			{
				ButtonBG.sprite = emptyBG;
			}
		}
	}
}
