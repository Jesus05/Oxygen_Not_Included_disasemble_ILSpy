public class GameInputManager : KInputManager
{
	public GameInputManager(BindingEntry[] default_keybindings)
	{
		GameInputMapping.SetDefaultKeyBindings(default_keybindings);
		GameInputMapping.LoadBindings();
		AddKeyboardMouseController();
	}

	public KInputController AddKeyboardMouseController()
	{
		KInputController kInputController = new KInputController(false);
		BindingEntry[] bindingEntries = GameInputMapping.GetBindingEntries();
		for (int i = 0; i < bindingEntries.Length; i++)
		{
			BindingEntry bindingEntry = bindingEntries[i];
			kInputController.Bind(bindingEntry.mKeyCode, bindingEntry.mModifier, bindingEntry.mAction);
		}
		AddController(kInputController);
		return kInputController;
	}

	public KInputController AddGamepadController(int gamepad_index)
	{
		KInputController kInputController = new KInputController(true);
		BindingEntry[] bindingEntries = GameInputMapping.GetBindingEntries();
		for (int i = 0; i < bindingEntries.Length; i++)
		{
			BindingEntry bindingEntry = bindingEntries[i];
			kInputController.Bind(BindingEntry.GetGamepadKeyCode(gamepad_index, bindingEntry.mButton), Modifier.None, bindingEntry.mAction);
		}
		AddController(kInputController);
		return kInputController;
	}

	public void RebindControls()
	{
		foreach (KInputController mController in mControllers)
		{
			mController.ClearBindings();
			BindingEntry[] bindingEntries = GameInputMapping.GetBindingEntries();
			for (int i = 0; i < bindingEntries.Length; i++)
			{
				BindingEntry bindingEntry = bindingEntries[i];
				mController.Bind(bindingEntry.mKeyCode, bindingEntry.mModifier, bindingEntry.mAction);
			}
			mController.HandleCancelInput();
		}
	}

	public override void Update()
	{
		if (KInputManager.isFocused)
		{
			base.Update();
		}
	}

	public override void OnApplicationFocus(bool focusStatus)
	{
		base.OnApplicationFocus(focusStatus);
	}
}
