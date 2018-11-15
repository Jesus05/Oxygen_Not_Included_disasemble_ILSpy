using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KButtonMenu : KScreen
{
	public class ButtonInfo
	{
		public delegate void HoverCallback(GameObject hoverTarget);

		public delegate void Callback();

		public string text;

		public Action shortcutKey;

		public GameObject visualizer;

		public UnityAction onClick;

		public HoverCallback onHover;

		public FMODAsset clickSound;

		public KButton uibutton;

		public string toolTip;

		public bool isEnabled = true;

		public string[] popupOptions;

		public Action<string> onPopupClick;

		public Func<string[]> onPopulatePopup;

		public object userData;

		public ButtonInfo(string text = null, UnityAction on_click = null, Action shortcut_key = Action.NumActions, HoverCallback on_hover = null, string tool_tip = null, GameObject visualizer = null, bool is_enabled = true, string[] popup_options = null, Action<string> on_popup_click = null, Func<string[]> on_populate_popup = null)
		{
			this.text = text;
			shortcutKey = shortcut_key;
			onClick = on_click;
			onHover = on_hover;
			this.visualizer = visualizer;
			toolTip = tool_tip;
			isEnabled = is_enabled;
			uibutton = null;
			popupOptions = popup_options;
			onPopupClick = on_popup_click;
			onPopulatePopup = on_populate_popup;
		}

		public ButtonInfo(string text, Action shortcutKey, UnityAction onClick, HoverCallback onHover = null, object userData = null)
		{
			this.text = text;
			this.shortcutKey = shortcutKey;
			this.onClick = onClick;
			this.onHover = onHover;
			this.userData = userData;
			visualizer = null;
			uibutton = null;
		}

		public ButtonInfo(string text, GameObject visualizer, Action shortcutKey, UnityAction onClick, HoverCallback onHover = null, object userData = null)
		{
			this.text = text;
			this.shortcutKey = shortcutKey;
			this.onClick = onClick;
			this.onHover = onHover;
			this.visualizer = visualizer;
			this.userData = userData;
			uibutton = null;
		}
	}

	[SerializeField]
	protected bool followGameObject;

	[SerializeField]
	protected bool keepMenuOpen;

	[SerializeField]
	private Transform buttonParent;

	public GameObject buttonPrefab;

	public bool ShouldConsumeMouseScroll;

	[NonSerialized]
	public GameObject[] buttonObjects;

	protected GameObject go;

	protected IList<ButtonInfo> buttons;

	private static readonly EventSystem.IntraObjectHandler<KButtonMenu> OnSetActivatorDelegate = new EventSystem.IntraObjectHandler<KButtonMenu>(delegate(KButtonMenu component, object data)
	{
		component.OnSetActivator(data);
	});

	protected override void OnActivate()
	{
		ConsumeMouseScroll = ShouldConsumeMouseScroll;
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
		if (buttons != null)
		{
			buttonObjects = new GameObject[buttons.Count];
			for (int j = 0; j < buttons.Count; j++)
			{
				ButtonInfo binfo = buttons[j];
				GameObject gameObject = UnityEngine.Object.Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
				buttonObjects[j] = gameObject;
				Transform parent = (!((UnityEngine.Object)buttonParent != (UnityEngine.Object)null)) ? base.transform : buttonParent;
				gameObject.transform.SetParent(parent, false);
				gameObject.SetActive(true);
				gameObject.name = binfo.text + "Button";
				LocText[] componentsInChildren = gameObject.GetComponentsInChildren<LocText>(true);
				if (componentsInChildren != null)
				{
					LocText[] array = componentsInChildren;
					foreach (LocText locText in array)
					{
						locText.text = ((!(locText.name == "Hotkey")) ? binfo.text : GameUtil.GetActionString(binfo.shortcutKey));
						locText.color = ((!binfo.isEnabled) ? new Color(0.5f, 0.5f, 0.5f) : new Color(1f, 1f, 1f));
					}
				}
				ToolTip componentInChildren = gameObject.GetComponentInChildren<ToolTip>();
				if (binfo.toolTip != null && binfo.toolTip != string.Empty && (UnityEngine.Object)componentInChildren != (UnityEngine.Object)null)
				{
					componentInChildren.toolTip = binfo.toolTip;
				}
				KButton button = gameObject.GetComponent<KButton>();
				button.isInteractable = binfo.isEnabled;
				if (binfo.popupOptions == null && binfo.onPopulatePopup == null)
				{
					UnityAction onClick = binfo.onClick;
					System.Action value = delegate
					{
						onClick();
						if (!keepMenuOpen && (UnityEngine.Object)this != (UnityEngine.Object)null)
						{
							Deactivate();
						}
					};
					button.onClick += value;
				}
				else
				{
					button.onClick += delegate
					{
						SetupPopupMenu(binfo, button);
					};
				}
				binfo.uibutton = button;
				if (binfo.onHover == null)
				{
					continue;
				}
			}
			Update();
		}
	}

	private Button.ButtonClickedEvent SetupPopupMenu(ButtonInfo binfo, KButton button)
	{
		Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
		UnityAction unityAction = delegate
		{
			List<ButtonInfo> list = new List<ButtonInfo>();
			if (binfo.onPopulatePopup != null)
			{
				binfo.popupOptions = binfo.onPopulatePopup();
			}
			string[] popupOptions = binfo.popupOptions;
			foreach (string text in popupOptions)
			{
				string delegate_str = text;
				list.Add(new ButtonInfo(delegate_str, delegate
				{
					binfo.onPopupClick(delegate_str);
					if (!keepMenuOpen)
					{
						Deactivate();
					}
				}, Action.NumActions, null, null, null, true, null, null, null));
			}
			KButtonMenu component = Util.KInstantiate(ScreenPrefabs.Instance.ButtonGrid.gameObject, null, null).GetComponent<KButtonMenu>();
			component.SetButtons(list.ToArray());
			RootMenu.Instance.AddSubMenu(component);
			Game.Instance.LocalPlayer.ScreenManager.ActivateScreen(component.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
			Vector3 b = default(Vector3);
			if (Util.IsOnLeftSideOfScreen(button.transform.GetPosition()))
			{
				b.x = button.GetComponent<RectTransform>().rect.width * 0.25f;
			}
			else
			{
				b.x = (0f - button.GetComponent<RectTransform>().rect.width) * 0.25f;
			}
			component.transform.SetPosition(button.transform.GetPosition() + b);
		};
		binfo.onClick = unityAction;
		buttonClickedEvent.AddListener(unityAction);
		return buttonClickedEvent;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (buttons != null)
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

	protected override void OnDeactivate()
	{
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
}
