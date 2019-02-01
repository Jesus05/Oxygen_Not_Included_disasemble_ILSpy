using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KIconButtonMenu : KScreen
{
	public class ButtonInfo
	{
		public delegate void Callback();

		public string iconName;

		public string text;

		public string tooltipText;

		public string[] multiText;

		public Action shortcutKey;

		public bool isInteractable;

		public Action<ButtonInfo> onCreate;

		public System.Action onClick;

		public Action<GameObject> onRefresh;

		public Func<string> onToolTip;

		public GameObject buttonGo;

		public object userData;

		public Texture texture;

		public ButtonInfo(string iconName = "", string text = "", System.Action on_click = null, Action shortcutKey = Action.NumActions, Action<GameObject> on_refresh = null, Action<ButtonInfo> on_create = null, Texture texture = null, string tooltipText = "", bool is_interactable = true)
		{
			this.iconName = iconName;
			this.text = text;
			this.shortcutKey = shortcutKey;
			onClick = on_click;
			onRefresh = on_refresh;
			onCreate = on_create;
			this.texture = texture;
			this.tooltipText = tooltipText;
			isInteractable = is_interactable;
		}

		public string GetTooltipText()
		{
			string text = (!(tooltipText == "")) ? tooltipText : this.text;
			if (shortcutKey != Action.NumActions)
			{
				text = text + " " + GameUtil.GetHotkeyString(shortcutKey);
			}
			return text;
		}
	}

	[SerializeField]
	protected bool followGameObject;

	[SerializeField]
	protected bool keepMenuOpen;

	[SerializeField]
	protected bool automaticNavigation = true;

	[SerializeField]
	protected Transform buttonParent;

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	protected Sprite[] icons;

	[SerializeField]
	private ToggleGroup externalToggleGroup;

	protected KToggle currentlySelectedToggle;

	[NonSerialized]
	public GameObject[] buttonObjects;

	[SerializeField]
	public TextStyleSetting ToggleToolTipTextStyleSetting;

	[SerializeField]
	public string tooltipHotKeyColor = "#ff2222ff";

	protected GameObject go;

	protected IList<ButtonInfo> buttons;

	private static readonly EventSystem.IntraObjectHandler<KIconButtonMenu> OnSetActivatorDelegate = new EventSystem.IntraObjectHandler<KIconButtonMenu>(delegate(KIconButtonMenu component, object data)
	{
		component.OnSetActivator(data);
	});

	protected override void OnActivate()
	{
		base.OnActivate();
		RefreshButtons();
	}

	public void SetButtons(IList<ButtonInfo> buttons)
	{
		this.buttons = buttons;
		if (activateOnSpawn)
		{
			RefreshButtons();
		}
	}

	public virtual void RefreshButtons()
	{
		if (buttonObjects != null)
		{
			for (int i = 0; i < buttonObjects.Length; i++)
			{
				UnityEngine.Object.Destroy(buttonObjects[i]);
			}
			buttonObjects = null;
		}
		if (buttons != null && buttons.Count != 0)
		{
			buttonObjects = new GameObject[buttons.Count];
			for (int j = 0; j < buttons.Count; j++)
			{
				ButtonInfo buttonInfo = buttons[j];
				if (buttonInfo != null)
				{
					GameObject binstance = UnityEngine.Object.Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
					buttonInfo.buttonGo = binstance;
					buttonObjects[j] = binstance;
					Transform transform = null;
					transform = ((!((UnityEngine.Object)buttonParent != (UnityEngine.Object)null)) ? base.transform : buttonParent);
					binstance.transform.SetParent(transform, false);
					binstance.SetActive(true);
					binstance.name = buttonInfo.text + "Button";
					KButton component = binstance.GetComponent<KButton>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null && buttonInfo.onClick != null)
					{
						component.onClick += buttonInfo.onClick;
					}
					Image image = null;
					if ((bool)component)
					{
						image = component.fgImage;
					}
					if ((UnityEngine.Object)image != (UnityEngine.Object)null)
					{
						image.gameObject.SetActive(false);
						Sprite[] array = icons;
						foreach (Sprite sprite in array)
						{
							if ((UnityEngine.Object)sprite != (UnityEngine.Object)null && sprite.name == buttonInfo.iconName)
							{
								image.sprite = sprite;
								image.gameObject.SetActive(true);
								break;
							}
						}
					}
					if ((UnityEngine.Object)buttonInfo.texture != (UnityEngine.Object)null)
					{
						RawImage componentInChildren = binstance.GetComponentInChildren<RawImage>();
						if ((UnityEngine.Object)componentInChildren != (UnityEngine.Object)null)
						{
							componentInChildren.gameObject.SetActive(true);
							componentInChildren.texture = buttonInfo.texture;
						}
					}
					ToolTip componentInChildren2 = binstance.GetComponentInChildren<ToolTip>();
					if (buttonInfo.text != null && buttonInfo.text != "" && (UnityEngine.Object)componentInChildren2 != (UnityEngine.Object)null)
					{
						componentInChildren2.toolTip = buttonInfo.GetTooltipText();
						LocText componentInChildren3 = binstance.GetComponentInChildren<LocText>();
						if ((UnityEngine.Object)componentInChildren3 != (UnityEngine.Object)null)
						{
							componentInChildren3.text = buttonInfo.text;
						}
					}
					if (buttonInfo.onToolTip != null)
					{
						componentInChildren2.OnToolTip = buttonInfo.onToolTip;
					}
					System.Action onClick = buttonInfo.onClick;
					System.Action value = delegate
					{
						onClick.Signal();
						if (!keepMenuOpen && (UnityEngine.Object)this != (UnityEngine.Object)null)
						{
							Deactivate();
						}
						if ((UnityEngine.Object)binstance != (UnityEngine.Object)null)
						{
							KToggle component3 = binstance.GetComponent<KToggle>();
							if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
							{
								SelectToggle(component3);
							}
						}
					};
					KToggle componentInChildren4 = binstance.GetComponentInChildren<KToggle>();
					if ((UnityEngine.Object)componentInChildren4 != (UnityEngine.Object)null)
					{
						componentInChildren4.onRefresh += buttonInfo.onRefresh;
						ToggleGroup component2 = GetComponent<ToggleGroup>();
						if ((UnityEngine.Object)component2 == (UnityEngine.Object)null)
						{
							component2 = externalToggleGroup;
						}
						componentInChildren4.group = component2;
						componentInChildren4.onClick += value;
						Navigation navigation = componentInChildren4.navigation;
						navigation.mode = (automaticNavigation ? Navigation.Mode.Automatic : Navigation.Mode.None);
						componentInChildren4.navigation = navigation;
					}
					else
					{
						KBasicToggle componentInChildren5 = binstance.GetComponentInChildren<KBasicToggle>();
						if ((UnityEngine.Object)componentInChildren5 != (UnityEngine.Object)null)
						{
							componentInChildren5.onClick += value;
						}
					}
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						component.isInteractable = buttonInfo.isInteractable;
					}
					buttonInfo.onCreate.Signal(buttonInfo);
				}
			}
			Update();
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (buttons != null && base.gameObject.activeSelf && base.enabled)
		{
			for (int i = 0; i < buttons.Count; i++)
			{
				ButtonInfo buttonInfo = buttons[i];
				if (e.TryConsume(buttonInfo.shortcutKey))
				{
					buttonObjects[i].GetComponent<KButton>().PlayPointerDownSound();
					buttonObjects[i].GetComponent<KButton>().SignalClick(KKeyCode.Mouse0);
					break;
				}
			}
			base.OnKeyDown(e);
		}
	}

	protected override void OnPrefabInit()
	{
		Subscribe(315865555, OnSetActivatorDelegate);
	}

	private void OnSetActivator(object data)
	{
		go = (GameObject)data;
		Update();
	}

	private void Update()
	{
		if (followGameObject && !((UnityEngine.Object)go == (UnityEngine.Object)null) && !((UnityEngine.Object)base.canvas == (UnityEngine.Object)null))
		{
			Vector3 vector = Camera.main.WorldToViewportPoint(go.transform.GetPosition());
			RectTransform component = GetComponent<RectTransform>();
			RectTransform component2 = base.canvas.GetComponent<RectTransform>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				RectTransform rectTransform = component;
				float x = vector.x;
				Vector2 sizeDelta = component2.sizeDelta;
				float num = x * sizeDelta.x;
				Vector2 sizeDelta2 = component2.sizeDelta;
				float x2 = num - sizeDelta2.x * 0.5f;
				float y = vector.y;
				Vector2 sizeDelta3 = component2.sizeDelta;
				float num2 = y * sizeDelta3.y;
				Vector2 sizeDelta4 = component2.sizeDelta;
				rectTransform.anchoredPosition = new Vector2(x2, num2 - sizeDelta4.y * 0.5f);
			}
		}
	}

	protected void SelectToggle(KToggle selectedToggle)
	{
		if (!((UnityEngine.Object)UnityEngine.EventSystems.EventSystem.current == (UnityEngine.Object)null) && UnityEngine.EventSystems.EventSystem.current.enabled)
		{
			if ((UnityEngine.Object)currentlySelectedToggle == (UnityEngine.Object)selectedToggle)
			{
				currentlySelectedToggle = null;
			}
			else
			{
				currentlySelectedToggle = selectedToggle;
			}
			GameObject[] array = buttonObjects;
			foreach (GameObject gameObject in array)
			{
				KToggle component = gameObject.GetComponent<KToggle>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					if ((UnityEngine.Object)component == (UnityEngine.Object)currentlySelectedToggle)
					{
						component.Select();
						component.isOn = true;
					}
					else
					{
						component.Deselect();
						component.isOn = false;
					}
				}
			}
		}
	}

	public void ClearSelection()
	{
		GameObject[] array = buttonObjects;
		foreach (GameObject gameObject in array)
		{
			KToggle component = gameObject.GetComponent<KToggle>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				component.Deselect();
				component.isOn = false;
			}
			else
			{
				KBasicToggle component2 = gameObject.GetComponent<KBasicToggle>();
				if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
				{
					component2.isOn = false;
				}
			}
			ImageToggleState component3 = gameObject.GetComponent<ImageToggleState>();
			if (component3.GetIsActive())
			{
				component3.SetInactive();
			}
		}
		ToggleGroup component4 = GetComponent<ToggleGroup>();
		if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
		{
			component4.SetAllTogglesOff();
		}
		SelectToggle(null);
	}
}
