using UnityEngine;
using UnityEngine.UI;

public class TitleBar : KMonoBehaviour
{
	public LocText titleText;

	public LocText subtextText;

	public GameObject WarningNotification;

	public Text NotificationText;

	public Image NotificationIcon;

	public Sprite techIcon;

	public Sprite materialIcon;

	public TitleBarPortrait portrait;

	public bool userEditable = false;

	public bool setCameraControllerState = true;

	public void SetTitle(string Name)
	{
		titleText.text = Name;
	}

	public void SetSubText(string subtext, string tooltip = "")
	{
		subtextText.text = subtext;
		subtextText.GetComponent<ToolTip>().toolTip = tooltip;
	}

	public void SetWarningActve(bool state)
	{
		WarningNotification.SetActive(state);
	}

	public void SetWarning(Sprite icon, string label)
	{
		SetWarningActve(true);
		NotificationIcon.sprite = icon;
		NotificationText.text = label;
	}

	public void SetPortrait(GameObject target)
	{
		portrait.SetPortrait(target);
	}
}
