using UnityEngine;
using UnityEngine.UI;

public class KTabMenuHeader : KMonoBehaviour
{
	public delegate void OnClick(int id);

	[SerializeField]
	private RectTransform prefab;

	public TextStyleSetting TextStyle_Active;

	public TextStyleSetting TextStyle_Inactive;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ActivateTabArtwork(0);
	}

	public void Add(string name, OnClick onClick, int id)
	{
		GameObject gameObject = Util.KInstantiateUI(prefab.gameObject, null, false);
		gameObject.SetActive(true);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.transform.SetParent(base.transform, false);
		component.name = name;
		Text componentInChildren = component.GetComponentInChildren<Text>();
		if ((Object)componentInChildren != (Object)null)
		{
			componentInChildren.text = name.ToUpper();
		}
		ActivateTabArtwork(id);
		KButton component2 = gameObject.GetComponent<KButton>();
		component2.onClick += delegate
		{
			onClick(id);
		};
	}

	public void Add(Sprite icon, string name, OnClick onClick, int id, string tooltip = "")
	{
		GameObject gameObject = Util.KInstantiateUI(prefab.gameObject, null, false);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.transform.SetParent(base.transform, false);
		component.name = name;
		if (tooltip == string.Empty)
		{
			component.GetComponent<ToolTip>().toolTip = name;
		}
		else
		{
			component.GetComponent<ToolTip>().toolTip = tooltip;
		}
		ActivateTabArtwork(id);
		TabHeaderIcon componentInChildren = component.GetComponentInChildren<TabHeaderIcon>();
		if ((bool)componentInChildren)
		{
			componentInChildren.TitleText.text = name;
		}
		KToggle component2 = gameObject.GetComponent<KToggle>();
		if ((bool)component2 && (bool)component2.fgImage)
		{
			component2.fgImage.sprite = icon;
		}
		component2.group = GetComponent<ToggleGroup>();
		component2.onClick += delegate
		{
			onClick(id);
		};
	}

	public void Activate(int itemIdx, int previouslyActiveTabIdx)
	{
		int childCount = base.transform.childCount;
		if (itemIdx < childCount)
		{
			for (int i = 0; i < childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					KButton componentInChildren = child.GetComponentInChildren<KButton>();
					if ((Object)componentInChildren != (Object)null)
					{
						Text componentInChildren2 = componentInChildren.GetComponentInChildren<Text>();
						if ((Object)componentInChildren2 != (Object)null && i == itemIdx)
						{
							ActivateTabArtwork(itemIdx);
						}
					}
					KToggle component = child.GetComponent<KToggle>();
					if ((Object)component != (Object)null)
					{
						ActivateTabArtwork(itemIdx);
						if (i == itemIdx)
						{
							component.Select();
						}
						else
						{
							component.Deselect();
						}
					}
				}
			}
		}
	}

	public void SetTabEnabled(int tabIdx, bool enabled)
	{
		if (tabIdx < base.transform.childCount)
		{
			base.transform.GetChild(tabIdx).gameObject.SetActive(enabled);
		}
	}

	public virtual void ActivateTabArtwork(int tabIdx)
	{
		if (tabIdx < base.transform.childCount)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				ImageToggleState component = base.transform.GetChild(i).GetComponent<ImageToggleState>();
				if ((Object)component != (Object)null)
				{
					if (i == tabIdx)
					{
						component.SetActive();
					}
					else
					{
						component.SetInactive();
					}
				}
				Canvas componentInChildren = base.transform.GetChild(i).GetComponentInChildren<Canvas>(true);
				if ((Object)componentInChildren != (Object)null)
				{
					componentInChildren.overrideSorting = (tabIdx == i);
				}
				SetTextStyleSetting componentInChildren2 = base.transform.GetChild(i).GetComponentInChildren<SetTextStyleSetting>();
				if ((Object)componentInChildren2 != (Object)null && (Object)TextStyle_Active != (Object)null && (Object)TextStyle_Inactive != (Object)null)
				{
					if (i == tabIdx)
					{
						componentInChildren2.SetStyle(TextStyle_Active);
					}
					else
					{
						componentInChildren2.SetStyle(TextStyle_Inactive);
					}
				}
			}
		}
	}
}
