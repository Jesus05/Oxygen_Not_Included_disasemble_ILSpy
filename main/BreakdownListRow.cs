using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreakdownListRow : KMonoBehaviour
{
	public enum Status
	{
		Default,
		Red,
		Green,
		Yellow
	}

	private static Color[] statusColour = new Color[4]
	{
		new Color(0.34117648f, 0.368627459f, 0.458823532f, 1f),
		new Color(0.721568644f, 0.384313732f, 0f, 1f),
		new Color(0.384313732f, 0.721568644f, 0f, 1f),
		new Color(0.721568644f, 0.721568644f, 0f, 1f)
	};

	public Image dotOutlineImage;

	public Image dotInsideImage;

	public Image iconImage;

	public Image checkmarkImage;

	public LocText nameLabel;

	public LocText valueLabel;

	private bool isHighlighted;

	private bool isDisabled;

	private bool isImportant;

	private ToolTip tooltip;

	[SerializeField]
	private Sprite statusSuccessIcon;

	[SerializeField]
	private Sprite statusWarningIcon;

	[SerializeField]
	private Sprite statusFailureIcon;

	public void ShowData(string name, string value)
	{
		base.gameObject.transform.localScale = Vector3.one;
		nameLabel.text = name;
		valueLabel.text = value;
		dotOutlineImage.gameObject.SetActive(true);
		Vector2 vector = Vector2.one * 0.6f;
		dotOutlineImage.rectTransform.localScale.Set(vector.x, vector.y, 1f);
		dotInsideImage.gameObject.SetActive(true);
		dotInsideImage.color = statusColour[0];
		iconImage.gameObject.SetActive(false);
		checkmarkImage.gameObject.SetActive(false);
		SetHighlighted(false);
		SetImportant(false);
	}

	public void ShowStatusData(string name, string value, Status dotColor)
	{
		ShowData(name, value);
		dotOutlineImage.gameObject.SetActive(true);
		dotInsideImage.gameObject.SetActive(true);
		iconImage.gameObject.SetActive(false);
		checkmarkImage.gameObject.SetActive(false);
		SetStatusColor(dotColor);
	}

	public void SetStatusColor(Status dotColor)
	{
		checkmarkImage.gameObject.SetActive(dotColor != Status.Default);
		checkmarkImage.color = statusColour[(int)dotColor];
		switch (dotColor)
		{
		case Status.Green:
			checkmarkImage.sprite = statusSuccessIcon;
			break;
		case Status.Yellow:
			checkmarkImage.sprite = statusWarningIcon;
			break;
		case Status.Red:
			checkmarkImage.sprite = statusFailureIcon;
			break;
		}
	}

	public void ShowCheckmarkData(string name, string value, Status status)
	{
		ShowData(name, value);
		dotOutlineImage.gameObject.SetActive(true);
		dotOutlineImage.rectTransform.localScale = Vector3.one;
		dotInsideImage.gameObject.SetActive(true);
		iconImage.gameObject.SetActive(false);
		SetStatusColor(status);
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
		Vector2 vector = Vector2.one * 0.8f;
		dotOutlineImage.rectTransform.localScale.Set(vector.x, vector.y, 1f);
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
		dotOutlineImage.rectTransform.localScale = Vector3.one;
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

	public void ClearTooltip()
	{
		if ((Object)tooltip != (Object)null)
		{
			tooltip.ClearMultiStringTooltip();
		}
	}

	public void SetValue(string value)
	{
		valueLabel.text = value;
	}
}
