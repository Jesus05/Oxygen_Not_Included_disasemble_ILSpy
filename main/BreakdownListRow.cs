using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreakdownListRow : KMonoBehaviour
{
	public Image dotOutlineImage;

	public Image dotInsideImage;

	public Image iconImage;

	public Image checkmarkImage;

	public LocText nameLabel;

	public LocText valueLabel;

	private bool isHighlighted = false;

	private bool isDisabled = false;

	private bool isImportant = false;

	private ToolTip tooltip;

	public void ShowData(string name, string value)
	{
		base.gameObject.transform.localScale = Vector2.one;
		nameLabel.text = name;
		valueLabel.text = value;
		dotOutlineImage.gameObject.SetActive(true);
		dotOutlineImage.rectTransform.localScale = Vector2.one * 0.6f;
		dotInsideImage.gameObject.SetActive(true);
		dotInsideImage.color = new Color(0.34117648f, 0.368627459f, 0.458823532f, 1f);
		iconImage.gameObject.SetActive(false);
		checkmarkImage.gameObject.SetActive(false);
		SetHighlighted(false);
		SetImportant(false);
	}

	public void ShowStatusData(string name, string value, Color dotColor)
	{
		ShowData(name, value);
		dotOutlineImage.gameObject.SetActive(true);
		dotInsideImage.gameObject.SetActive(true);
		dotInsideImage.color = dotColor;
		iconImage.gameObject.SetActive(false);
		checkmarkImage.gameObject.SetActive(false);
	}

	public void SetStatusColor(Color dotColor)
	{
		dotInsideImage.color = dotColor;
	}

	public void ShowCheckmarkData(string name, string value, bool completed)
	{
		ShowData(name, value);
		dotOutlineImage.gameObject.SetActive(true);
		dotOutlineImage.rectTransform.localScale = Vector2.one;
		dotInsideImage.gameObject.SetActive(true);
		dotInsideImage.color = ((!completed) ? new Color(0.1882353f, 0.203921571f, 0.2627451f, 1f) : new Color(0.34117648f, 0.368627459f, 0.458823532f, 1f));
		iconImage.gameObject.SetActive(false);
		checkmarkImage.gameObject.SetActive(true);
		checkmarkImage.color = ((!completed) ? new Color(1f, 1f, 1f, 0.15f) : new Color(0.384313732f, 0.721568644f, 0f, 1f));
	}

	public void ShowIconData(string name, string value, Sprite sprite)
	{
		ShowData(name, value);
		dotOutlineImage.gameObject.SetActive(false);
		dotInsideImage.gameObject.SetActive(false);
		iconImage.gameObject.SetActive(true);
		checkmarkImage.gameObject.SetActive(false);
		iconImage.sprite = sprite;
		iconImage.color = Color.white;
	}

	public void ShowIconData(string name, string value, Sprite sprite, Color spriteColor)
	{
		ShowIconData(name, value, sprite);
		iconImage.color = spriteColor;
	}

	public void SetHighlighted(bool highlighted)
	{
		isHighlighted = highlighted;
		dotOutlineImage.rectTransform.localScale = Vector2.one * 0.8f;
		nameLabel.alpha = ((!isHighlighted) ? 0.5f : 0.9f);
		valueLabel.alpha = ((!isHighlighted) ? 0.5f : 0.9f);
	}

	public void SetDisabled(bool disabled)
	{
		isDisabled = disabled;
		nameLabel.alpha = ((!isDisabled) ? 0.5f : 0.4f);
		valueLabel.alpha = ((!isDisabled) ? 0.5f : 0.4f);
	}

	public void SetImportant(bool important)
	{
		isImportant = important;
		dotOutlineImage.rectTransform.localScale = Vector2.one;
		nameLabel.alpha = ((!isImportant) ? 0.5f : 1f);
		valueLabel.alpha = ((!isImportant) ? 0.5f : 1f);
		nameLabel.fontStyle = (isImportant ? FontStyles.Bold : FontStyles.Normal);
		valueLabel.fontStyle = (isImportant ? FontStyles.Bold : FontStyles.Normal);
	}

	public void HideIcon()
	{
		dotOutlineImage.gameObject.SetActive(false);
		dotInsideImage.gameObject.SetActive(false);
		iconImage.gameObject.SetActive(false);
		checkmarkImage.gameObject.SetActive(false);
	}

	public void AddTooltip(string tooltipText)
	{
		if ((Object)tooltip == (Object)null)
		{
			tooltip = base.gameObject.AddComponent<ToolTip>();
		}
		tooltip.SetSimpleTooltip(tooltipText);
	}

	public void SetValue(string value)
	{
		valueLabel.text = value;
	}
}
