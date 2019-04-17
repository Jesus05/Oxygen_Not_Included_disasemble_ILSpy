using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KIconToggleMenu : KScreen
{
	public delegate void OnSelect(ToggleInfo toggleInfo);

	public class ToggleInfo
	{
		public string text;

		public object userData;

		public string icon;

		public string tooltip;

		public string tooltipHeader;

		public KToggle toggle;

		public Action hotKey;

		public Func<Sprite> getSpriteCB;

		public KToggle prefabOverride;

		public KToggle instanceOverride;

		public ToggleInfo(string text, string icon, object user_data = null, Action hotkey = Action.NumActions, string tooltip = "", string tooltip_header = "")
		{
			this.text = text;
			userData = user_data;
			this.icon = icon;
			hotKey = hotkey;
			this.tooltip = tooltip;
			tooltipHeader = tooltip_header;
		}

		public ToggleInfo(string text, object user_data, Action hotkey, Func<Sprite> get_sprite_cb)
		{
			this.text = text;
			userData = user_data;
			hotKey = hotkey;
			getSpriteCB = get_sprite_cb;
		}
	}

	[SerializeField]
	private Transform toggleParent;

	[SerializeField]
	private KToggle prefab;

	[SerializeField]
	private ToggleGroup group;

	[SerializeField]
	private Sprite[] icons;

	[SerializeField]
	public TextStyleSetting ToggleToolTipTextStyleSetting;

	[SerializeField]
	public TextStyleSetting ToggleToolTipHeaderTextStyleSetting;

	[SerializeField]
	protected bool repeatKeyDownToggles = true;

	protected KToggle currentlySelectedToggle;

	protected IList<ToggleInfo> toggleInfo;

	protected List<KToggle> toggles = new List<KToggle>();

	private List<KToggle> dontDestroyToggles = new List<KToggle>();

	protected int selected = -1;

	public event OnSelect onSelect;

	public void Setup(IList<ToggleInfo> toggleInfo)
	{
		this.toggleInfo = toggleInfo;
		RefreshButtons();
	}

	protected void Setup()
	{
		RefreshButtons();
	}

	protected virtual void RefreshButtons()
	{
		foreach (KToggle toggle in toggles)
		{
			if ((UnityEngine.Object)toggle != (UnityEngine.Object)null)
			{
				if (!dontDestroyToggles.Contains(toggle))
				{
					UnityEngine.Object.Destroy(toggle.gameObject);
				}
				else
				{
					toggle.ClearOnClick();
				}
			}
		}
		toggles.Clear();
		dontDestroyToggles.Clear();
		if (this.toggleInfo != null)
		{
			Transform transform = (!((UnityEngine.Object)toggleParent != (UnityEngine.Object)null)) ? base.transform : toggleParent;
			for (int i = 0; i < this.toggleInfo.Count; i++)
			{
				int idx = i;
				ToggleInfo toggleInfo = this.toggleInfo[i];
				KToggle kToggle;
				if (!((UnityEngine.Object)toggleInfo.instanceOverride != (UnityEngine.Object)null))
				{
					kToggle = ((!(bool)toggleInfo.prefabOverride) ? Util.KInstantiateUI<KToggle>(prefab.gameObject, transform.gameObject, true) : Util.KInstantiateUI<KToggle>(toggleInfo.prefabOverride.gameObject, transform.gameObject, true));
				}
				else
				{
					kToggle = toggleInfo.instanceOverride;
					dontDestroyToggles.Add(kToggle);
				}
				kToggle.Deselect();
				kToggle.gameObject.name = "Toggle:" + toggleInfo.text;
				kToggle.group = group;
				kToggle.onClick += delegate
				{
					OnClick(idx);
				};
				Transform transform2 = kToggle.transform.Find("Text");
				if ((UnityEngine.Object)transform2 != (UnityEngine.Object)null)
				{
					LocText component = transform2.GetComponent<LocText>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						component.text = toggleInfo.text;
					}
				}
				ToolTip component2 = kToggle.GetComponent<ToolTip>();
				if ((bool)component2)
				{
					if (toggleInfo.tooltipHeader != string.Empty)
					{
						component2.AddMultiStringTooltip(toggleInfo.tooltipHeader, (!((UnityEngine.Object)ToggleToolTipHeaderTextStyleSetting != (UnityEngine.Object)null)) ? ToggleToolTipTextStyleSetting : ToggleToolTipHeaderTextStyleSetting);
						if ((UnityEngine.Object)ToggleToolTipHeaderTextStyleSetting == (UnityEngine.Object)null)
						{
							Debug.Log("!");
						}
					}
					component2.AddMultiStringTooltip(GameUtil.ReplaceHotkeyString(toggleInfo.tooltip, toggleInfo.hotKey), ToggleToolTipTextStyleSetting);
				}
				if (toggleInfo.getSpriteCB != null)
				{
					kToggle.fgImage.sprite = toggleInfo.getSpriteCB();
				}
				else if (toggleInfo.icon != null)
				{
					kToggle.fgImage.sprite = Assets.GetSprite(toggleInfo.icon);
				}
				toggleInfo.toggle = kToggle;
				toggles.Add(kToggle);
			}
		}
	}

	public Sprite GetIcon(string name)
	{
		Sprite[] array = icons;
		foreach (Sprite sprite in array)
		{
			if (sprite.name == name)
			{
				return sprite;
			}
		}
		return null;
	}

	public virtual void ClearSelection()
	{
		if (toggles != null)
		{
			foreach (KToggle toggle in toggles)
			{
				toggle.Deselect();
				toggle.ClearAnimState();
			}
			selected = -1;
		}
	}

	private void OnClick(int i)
	{
		if (this.onSelect != null)
		{
			selected = i;
			this.onSelect(toggleInfo[i]);
			if (!toggles[i].isOn)
			{
				selected = -1;
			}
			for (int j = 0; j < toggles.Count; j++)
			{
				if (j != selected)
				{
					toggles[j].isOn = false;
				}
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (toggles != null && toggleInfo != null)
		{
			int num = 0;
			while (true)
			{
				if (num >= toggleInfo.Count)
				{
					return;
				}
				if (toggles[num].isActiveAndEnabled)
				{
					Action hotKey = toggleInfo[num].hotKey;
					if (hotKey != Action.NumActions && e.TryConsume(hotKey))
					{
						break;
					}
				}
				num++;
			}
			if (selected != num || repeatKeyDownToggles)
			{
				toggles[num].Click();
				if (selected == num)
				{
					toggles[num].Deselect();
				}
				selected = num;
			}
		}
	}

	public virtual void Close()
	{
		ClearSelection();
		Show(false);
	}
}
