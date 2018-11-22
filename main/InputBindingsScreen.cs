using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputBindingsScreen : KModalScreen
{
	private const string ROOT_KEY = "STRINGS.INPUT_BINDINGS.";

	[SerializeField]
	private OptionsMenuScreen optionsScreen;

	[SerializeField]
	private ConfirmDialogScreen confirmPrefab;

	public KButton backButton;

	public KButton resetButton;

	public KButton closeButton;

	public KButton prevScreenButton;

	public KButton nextScreenButton;

	private bool waitingForKeyPress = false;

	private Action actionToRebind = Action.NumActions;

	private bool ignoreRootConflicts = false;

	private KButton activeButton = null;

	[SerializeField]
	private LocText screenTitle;

	[SerializeField]
	private GameObject parent;

	[SerializeField]
	private GameObject entryPrefab;

	private ConfirmDialogScreen confirmDialog;

	private int activeScreen = -1;

	private List<string> screens = new List<string>();

	private UIPool<HorizontalLayoutGroup> entryPool;

	private static readonly KeyCode[] validKeys = new KeyCode[111]
	{
		KeyCode.Backspace,
		KeyCode.Tab,
		KeyCode.Clear,
		KeyCode.Return,
		KeyCode.Pause,
		KeyCode.Space,
		KeyCode.Exclaim,
		KeyCode.DoubleQuote,
		KeyCode.Hash,
		KeyCode.Dollar,
		KeyCode.Ampersand,
		KeyCode.Quote,
		KeyCode.LeftParen,
		KeyCode.RightParen,
		KeyCode.Asterisk,
		KeyCode.Plus,
		KeyCode.Comma,
		KeyCode.Minus,
		KeyCode.Period,
		KeyCode.Slash,
		KeyCode.Alpha0,
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.Alpha8,
		KeyCode.Alpha9,
		KeyCode.Colon,
		KeyCode.Semicolon,
		KeyCode.Less,
		KeyCode.Equals,
		KeyCode.Greater,
		KeyCode.Question,
		KeyCode.At,
		KeyCode.LeftBracket,
		KeyCode.Backslash,
		KeyCode.RightBracket,
		KeyCode.Caret,
		KeyCode.Underscore,
		KeyCode.BackQuote,
		KeyCode.A,
		KeyCode.B,
		KeyCode.C,
		KeyCode.D,
		KeyCode.E,
		KeyCode.F,
		KeyCode.G,
		KeyCode.H,
		KeyCode.I,
		KeyCode.J,
		KeyCode.K,
		KeyCode.L,
		KeyCode.M,
		KeyCode.N,
		KeyCode.O,
		KeyCode.P,
		KeyCode.Q,
		KeyCode.R,
		KeyCode.S,
		KeyCode.T,
		KeyCode.U,
		KeyCode.V,
		KeyCode.W,
		KeyCode.X,
		KeyCode.Y,
		KeyCode.Z,
		KeyCode.Delete,
		KeyCode.Keypad0,
		KeyCode.Keypad1,
		KeyCode.Keypad2,
		KeyCode.Keypad3,
		KeyCode.Keypad4,
		KeyCode.Keypad5,
		KeyCode.Keypad6,
		KeyCode.Keypad7,
		KeyCode.Keypad8,
		KeyCode.Keypad9,
		KeyCode.KeypadPeriod,
		KeyCode.KeypadDivide,
		KeyCode.KeypadMultiply,
		KeyCode.KeypadMinus,
		KeyCode.KeypadPlus,
		KeyCode.KeypadEnter,
		KeyCode.KeypadEquals,
		KeyCode.UpArrow,
		KeyCode.DownArrow,
		KeyCode.RightArrow,
		KeyCode.LeftArrow,
		KeyCode.Insert,
		KeyCode.Home,
		KeyCode.End,
		KeyCode.PageUp,
		KeyCode.PageDown,
		KeyCode.F1,
		KeyCode.F2,
		KeyCode.F3,
		KeyCode.F4,
		KeyCode.F5,
		KeyCode.F6,
		KeyCode.F7,
		KeyCode.F8,
		KeyCode.F9,
		KeyCode.F10,
		KeyCode.F11,
		KeyCode.F12,
		KeyCode.F13,
		KeyCode.F14,
		KeyCode.F15
	};

	public override bool IsModal()
	{
		return true;
	}

	private bool IsKeyDown(KeyCode key_code)
	{
		return Input.GetKey(key_code) || Input.GetKeyDown(key_code);
	}

	private string GetModifierString(Modifier modifiers)
	{
		string text = "";
		IEnumerator enumerator = Enum.GetValues(typeof(Modifier)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Modifier modifier = (Modifier)enumerator.Current;
				if ((modifiers & modifier) != 0)
				{
					text = text + " + " + modifier.ToString();
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		return text;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		entryPrefab.SetActive(false);
		prevScreenButton.onClick += OnPrevScreen;
		nextScreenButton.onClick += OnNextScreen;
	}

	protected override void OnActivate()
	{
		CollectScreens();
		string text = screens[activeScreen];
		string key = "STRINGS.INPUT_BINDINGS." + text.ToUpper() + ".NAME";
		screenTitle.text = Strings.Get(key);
		closeButton.onClick += OnBack;
		backButton.onClick += OnBack;
		resetButton.onClick += OnReset;
		BuildDisplay();
	}

	private void CollectScreens()
	{
		screens.Clear();
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry bindingEntry = GameInputMapping.KeyBindings[i];
			if (bindingEntry.mGroup != null && bindingEntry.mRebindable && !screens.Contains(bindingEntry.mGroup))
			{
				if (bindingEntry.mGroup == "Root")
				{
					activeScreen = screens.Count;
				}
				if (BuildMenu.UseHotkeyBuildMenu() || !bindingEntry.mIgnoreRootConflics)
				{
					screens.Add(bindingEntry.mGroup);
				}
			}
		}
	}

	protected override void OnDeactivate()
	{
		GameInputMapping.SaveBindings();
		DestroyDisplay();
	}

	private LocString GetActionString(Action action)
	{
		return null;
	}

	private string GetBindingText(BindingEntry binding)
	{
		string text = GameUtil.GetKeycodeLocalized(binding.mKeyCode);
		if (binding.mKeyCode != KKeyCode.LeftAlt && binding.mKeyCode != KKeyCode.RightAlt && binding.mKeyCode != KKeyCode.LeftControl && binding.mKeyCode != KKeyCode.RightControl && binding.mKeyCode != KKeyCode.LeftShift && binding.mKeyCode != KKeyCode.RightShift)
		{
			text += GetModifierString(binding.mModifier);
		}
		return text;
	}

	private void BuildDisplay()
	{
		string text = screens[activeScreen];
		string key = "STRINGS.INPUT_BINDINGS." + text.ToUpper() + ".NAME";
		screenTitle.text = Strings.Get(key);
		if (entryPool == null)
		{
			entryPool = new UIPool<HorizontalLayoutGroup>(entryPrefab.GetComponent<HorizontalLayoutGroup>());
		}
		DestroyDisplay();
		int num = 0;
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry binding = GameInputMapping.KeyBindings[i];
			if (binding.mGroup == screens[activeScreen] && binding.mRebindable)
			{
				GameObject gameObject = entryPool.GetFreeElement(parent, true).gameObject;
				LocText componentInChildren = gameObject.transform.GetChild(0).GetComponentInChildren<LocText>();
				string key2 = "STRINGS.INPUT_BINDINGS." + binding.mGroup.ToUpper() + "." + binding.mAction.ToString().ToUpper();
				componentInChildren.text = Strings.Get(key2);
				LocText key_label = gameObject.transform.GetChild(1).GetComponentInChildren<LocText>();
				key_label.text = GetBindingText(binding);
				KButton button = gameObject.GetComponentInChildren<KButton>();
				button.onClick += delegate
				{
					waitingForKeyPress = true;
					actionToRebind = binding.mAction;
					ignoreRootConflicts = binding.mIgnoreRootConflics;
					activeButton = button;
					key_label.text = UI.FRONTEND.INPUT_BINDINGS_SCREEN.WAITING_FOR_INPUT;
				};
				gameObject.transform.SetSiblingIndex(num);
				num++;
			}
		}
	}

	private void DestroyDisplay()
	{
		entryPool.ClearAll();
	}

	private void Update()
	{
		if (waitingForKeyPress)
		{
			Modifier modifier = Modifier.None;
			modifier = (Modifier)((int)modifier | ((IsKeyDown(KeyCode.LeftAlt) || IsKeyDown(KeyCode.RightAlt)) ? 1 : 0));
			modifier = (Modifier)((int)modifier | ((IsKeyDown(KeyCode.LeftControl) || IsKeyDown(KeyCode.RightControl)) ? 2 : 0));
			modifier = (Modifier)((int)modifier | ((IsKeyDown(KeyCode.LeftShift) || IsKeyDown(KeyCode.RightShift)) ? 4 : 0));
			modifier = (Modifier)((int)modifier | (IsKeyDown(KeyCode.CapsLock) ? 8 : 0));
			bool flag = false;
			for (int i = 0; i < validKeys.Length; i++)
			{
				KeyCode keyCode = validKeys[i];
				if (Input.GetKeyDown(keyCode))
				{
					KKeyCode kkey_code = (KKeyCode)keyCode;
					Bind(kkey_code, modifier);
					flag = true;
				}
			}
			if (!flag)
			{
				float axis = Input.GetAxis("Mouse ScrollWheel");
				KKeyCode kKeyCode = KKeyCode.None;
				if (axis < 0f)
				{
					kKeyCode = KKeyCode.MouseScrollDown;
				}
				else if (axis > 0f)
				{
					kKeyCode = KKeyCode.MouseScrollUp;
				}
				if (kKeyCode != 0)
				{
					Bind(kKeyCode, modifier);
					flag = true;
				}
			}
		}
	}

	private BindingEntry GetDuplicatedBinding(string activeScreen, BindingEntry new_binding)
	{
		BindingEntry result = default(BindingEntry);
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry bindingEntry = GameInputMapping.KeyBindings[i];
			if (new_binding.IsBindingEqual(bindingEntry) && (bindingEntry.mGroup == null || bindingEntry.mGroup == activeScreen || bindingEntry.mGroup == "Root" || activeScreen == "Root") && (!(activeScreen == "Root") || !bindingEntry.mIgnoreRootConflics) && (!(bindingEntry.mGroup == "Root") || !new_binding.mIgnoreRootConflics))
			{
				return bindingEntry;
			}
		}
		return result;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (waitingForKeyPress)
		{
			e.Consumed = true;
		}
		else if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
		{
			Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		e.Consumed = true;
	}

	private void OnBack()
	{
		string text;
		switch (NumUnboundActions())
		{
		case 0:
			Deactivate();
			return;
		case 1:
		{
			BindingEntry firstUnbound = GetFirstUnbound();
			text = string.Format(UI.FRONTEND.INPUT_BINDINGS_SCREEN.UNBOUND_ACTION, firstUnbound.mAction.ToString());
			break;
		}
		default:
			text = UI.FRONTEND.INPUT_BINDINGS_SCREEN.MULTIPLE_UNBOUND_ACTIONS;
			break;
		}
		confirmDialog = Util.KInstantiateUI(confirmPrefab.gameObject, base.transform.gameObject, false).GetComponent<ConfirmDialogScreen>();
		confirmDialog.PopupConfirmDialog(text, delegate
		{
			Deactivate();
		}, delegate
		{
			confirmDialog.Deactivate();
		}, null, null, null, null, null, null);
		confirmDialog.gameObject.SetActive(true);
	}

	private int NumUnboundActions()
	{
		int num = 0;
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry bindingEntry = GameInputMapping.KeyBindings[i];
			if (bindingEntry.mKeyCode == KKeyCode.None && (BuildMenu.UseHotkeyBuildMenu() || !bindingEntry.mIgnoreRootConflics))
			{
				num++;
			}
		}
		return num;
	}

	private BindingEntry GetFirstUnbound()
	{
		BindingEntry result = default(BindingEntry);
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry result2 = GameInputMapping.KeyBindings[i];
			if (result2.mKeyCode == KKeyCode.None)
			{
				return result2;
			}
		}
		return result;
	}

	private void OnReset()
	{
		GameInputMapping.KeyBindings = (BindingEntry[])GameInputMapping.DefaultBindings.Clone();
		Global.Instance.GetInputManager().RebindControls();
		BuildDisplay();
	}

	public void OnPrevScreen()
	{
		if (activeScreen > 0)
		{
			activeScreen--;
		}
		else
		{
			activeScreen = screens.Count - 1;
		}
		BuildDisplay();
	}

	public void OnNextScreen()
	{
		if (activeScreen < screens.Count - 1)
		{
			activeScreen++;
		}
		else
		{
			activeScreen = 0;
		}
		BuildDisplay();
	}

	private void Bind(KKeyCode kkey_code, Modifier modifier)
	{
		BindingEntry bindingEntry = new BindingEntry(screens[activeScreen], GamepadButton.NumButtons, kkey_code, modifier, actionToRebind, true, ignoreRootConflicts);
		int num = 0;
		while (true)
		{
			if (num >= GameInputMapping.KeyBindings.Length)
			{
				return;
			}
			BindingEntry bindingEntry2 = GameInputMapping.KeyBindings[num];
			if (bindingEntry2.mRebindable && bindingEntry2.mAction == actionToRebind)
			{
				break;
			}
			num++;
		}
		BindingEntry duplicatedBinding = GetDuplicatedBinding(screens[activeScreen], bindingEntry);
		GameInputMapping.KeyBindings[num] = bindingEntry;
		LocText componentInChildren = activeButton.GetComponentInChildren<LocText>();
		componentInChildren.text = GetBindingText(bindingEntry);
		if (duplicatedBinding.mAction != 0 && duplicatedBinding.mAction != actionToRebind)
		{
			confirmDialog = Util.KInstantiateUI(confirmPrefab.gameObject, base.transform.gameObject, false).GetComponent<ConfirmDialogScreen>();
			string key = "STRINGS.INPUT_BINDINGS." + duplicatedBinding.mGroup.ToUpper() + "." + duplicatedBinding.mAction.ToString().ToUpper();
			string arg = Strings.Get(key);
			string bindingText = GetBindingText(duplicatedBinding);
			string text = string.Format(UI.FRONTEND.INPUT_BINDINGS_SCREEN.DUPLICATE, arg, bindingText);
			Unbind(duplicatedBinding.mAction);
			confirmDialog.PopupConfirmDialog(text, null, null, null, null, null, null, null, null);
			confirmDialog.gameObject.SetActive(true);
		}
		Global.Instance.GetInputManager().RebindControls();
		waitingForKeyPress = false;
		actionToRebind = Action.NumActions;
		activeButton = null;
		BuildDisplay();
	}

	private void Unbind(Action action)
	{
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry bindingEntry = GameInputMapping.KeyBindings[i];
			if (bindingEntry.mAction == action)
			{
				bindingEntry.mKeyCode = KKeyCode.None;
				bindingEntry.mModifier = Modifier.None;
				GameInputMapping.KeyBindings[i] = bindingEntry;
			}
		}
	}
}
