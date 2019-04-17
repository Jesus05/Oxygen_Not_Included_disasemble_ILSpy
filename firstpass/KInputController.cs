using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class KInputController : IInputHandler
{
	private enum Scroll
	{
		Up,
		Down,
		NumStates
	}

	public struct KeyDefEntry
	{
		private KKeyCode mKeyCode;

		private Modifier mModifier;

		public KeyDefEntry(KKeyCode key_code, Modifier modifier)
		{
			mKeyCode = key_code;
			mModifier = modifier;
		}

		private void Print()
		{
			Debug.Log(mKeyCode.ToString() + mModifier.ToString());
		}
	}

	[DebuggerDisplay("Key: {mKeyCode} Mod: {mModifier}")]
	public class KeyDef
	{
		public KKeyCode mKeyCode;

		public Modifier mModifier;

		public bool[] mActionFlags;

		public bool mIsDown;

		public KeyDef(KKeyCode key_code, Modifier modifier)
		{
			mKeyCode = key_code;
			mModifier = modifier;
			mActionFlags = new bool[234];
		}
	}

	private List<KInputBinding> mBindings;

	private List<KInputEvent> mEvents;

	private KeyDef[] mKeyDefs = new KeyDef[0];

	private bool mDirtyBindings;

	private float[] mAxis;

	private Modifier mActiveModifiers;

	private bool[] mActionState;

	private bool[] mScrollState;

	private bool mIgnoreKeyboard;

	private bool mIgnoreMouse;

	private Dictionary<KeyDefEntry, KeyDef> mKeyDefLookup = new Dictionary<KeyDefEntry, KeyDef>();

	private static readonly KKeyCode[] altCodes = new KKeyCode[2]
	{
		KKeyCode.LeftAlt,
		KKeyCode.RightAlt
	};

	private static readonly KKeyCode[] ctrlCodes = new KKeyCode[2]
	{
		KKeyCode.LeftControl,
		KKeyCode.RightControl
	};

	private static readonly KKeyCode[] shiftCodes = new KKeyCode[2]
	{
		KKeyCode.LeftShift,
		KKeyCode.RightShift
	};

	private static readonly KKeyCode[] capsCodes = new KKeyCode[1]
	{
		KKeyCode.CapsLock
	};

	public string handlerName => "KInputController";

	public KInputHandler inputHandler
	{
		get;
		set;
	}

	public bool IsGamepad
	{
		get;
		private set;
	}

	public KInputController(bool is_gamepad)
	{
		mBindings = new List<KInputBinding>();
		mEvents = new List<KInputEvent>();
		mDirtyBindings = false;
		IsGamepad = is_gamepad;
		mAxis = new float[4];
		mActiveModifiers = Modifier.None;
		mActionState = new bool[234];
		mScrollState = new bool[2];
		inputHandler = new KInputHandler(this, this);
	}

	public void ClearBindings()
	{
		mBindings.Clear();
	}

	public void Bind(KKeyCode key_code, Modifier modifier, Action action)
	{
		mBindings.Add(new KInputBinding(key_code, modifier, action));
		mDirtyBindings = true;
	}

	public void QueueButtonEvent(KeyDef key_def, bool is_down)
	{
		if (KInputManager.isFocused)
		{
			bool[] mActionFlags = key_def.mActionFlags;
			key_def.mIsDown = is_down;
			InputEventType event_type = (!is_down) ? InputEventType.KeyUp : InputEventType.KeyDown;
			for (int i = 0; i < mActionFlags.Length; i++)
			{
				if (mActionFlags[i])
				{
					mActionState[i] = is_down;
				}
			}
			KButtonEvent item = new KButtonEvent(this, event_type, mActionFlags);
			mEvents.Add(item);
			KInputManager.SetUserActive();
		}
	}

	private void GenerateActionFlagTable()
	{
		mKeyDefLookup.Clear();
		foreach (KInputBinding mBinding in mBindings)
		{
			KeyDefEntry key = new KeyDefEntry(mBinding.mKeyCode, mBinding.mModifier);
			KeyDef value = null;
			if (!mKeyDefLookup.TryGetValue(key, out value))
			{
				value = new KeyDef(mBinding.mKeyCode, mBinding.mModifier);
				mKeyDefLookup[key] = value;
			}
			value.mActionFlags[(int)mBinding.mAction] = true;
		}
		mKeyDefs = new KeyDef[mKeyDefLookup.Count];
		mKeyDefLookup.Values.CopyTo(mKeyDefs, 0);
	}

	private bool GetKeyDown(KKeyCode key_code)
	{
		bool result = false;
		if (key_code >= KKeyCode.KleiKeys)
		{
			switch (key_code)
			{
			case KKeyCode.MouseScrollUp:
				result = mScrollState[0];
				break;
			case KKeyCode.MouseScrollDown:
				result = mScrollState[1];
				break;
			}
		}
		else
		{
			result = Input.GetKeyDown((KeyCode)key_code);
		}
		return result;
	}

	private bool GetKeyUp(KKeyCode key_code)
	{
		if (key_code >= KKeyCode.KleiKeys)
		{
			return false;
		}
		return Input.GetKeyUp((KeyCode)key_code);
	}

	public void CheckModifier(KKeyCode[] key_codes, Modifier modifier)
	{
		mActiveModifiers &= ~modifier;
		int num = 0;
		while (true)
		{
			if (num >= key_codes.Length)
			{
				return;
			}
			KKeyCode kKeyCode = key_codes[num];
			if (GetKeyDown(kKeyCode) || Input.GetKey((KeyCode)kKeyCode))
			{
				break;
			}
			num++;
		}
		mActiveModifiers |= modifier;
	}

	private void UpdateAxis()
	{
		mAxis[2] = Input.GetAxis("Mouse X");
		mAxis[3] = Input.GetAxis("Mouse Y");
	}

	private void UpdateModifiers()
	{
		CheckModifier(altCodes, Modifier.Alt);
		CheckModifier(ctrlCodes, Modifier.Ctrl);
		CheckModifier(shiftCodes, Modifier.Shift);
		CheckModifier(capsCodes, Modifier.CapsLock);
	}

	private void UpdateScrollStates()
	{
		float axis = Input.GetAxis("Mouse ScrollWheel");
		mScrollState[1] = (axis < 0f);
		mScrollState[0] = (axis > 0f);
	}

	public void ToggleKeyboard(bool active)
	{
		mIgnoreKeyboard = active;
	}

	public void ToggleMouse(bool active)
	{
		mIgnoreMouse = active;
	}

	public void Update()
	{
		if (KInputManager.isFocused)
		{
			if (mDirtyBindings)
			{
				GenerateActionFlagTable();
				mDirtyBindings = false;
			}
			if (!IsGamepad)
			{
				UpdateScrollStates();
				UpdateAxis();
				UpdateModifiers();
				KeyDef[] array = mKeyDefs;
				foreach (KeyDef keyDef in array)
				{
					int mKeyCode = (int)keyDef.mKeyCode;
					if ((!mIgnoreKeyboard || mKeyCode >= 323) && (!mIgnoreMouse || ((mKeyCode < 323 || mKeyCode >= 330) && mKeyCode != 1001 && mKeyCode != 1002)))
					{
						if (GetKeyDown(keyDef.mKeyCode) && mActiveModifiers == keyDef.mModifier)
						{
							QueueButtonEvent(keyDef, true);
						}
						if (keyDef.mIsDown && GetKeyUp(keyDef.mKeyCode))
						{
							QueueButtonEvent(keyDef, false);
						}
					}
				}
			}
		}
	}

	public void Dispatch()
	{
		foreach (KInputEvent mEvent in mEvents)
		{
			inputHandler.HandleEvent(mEvent);
		}
		mEvents.Clear();
	}

	public bool IsActive(Action action)
	{
		return mActionState[(int)action];
	}

	public float GetAxis(Axis axis)
	{
		return mAxis[(int)axis];
	}

	public void HandleCancelInput()
	{
		KeyDef[] array = mKeyDefs;
		foreach (KeyDef keyDef in array)
		{
			if (IsGamepad || (keyDef.mIsDown && keyDef.mKeyCode < KKeyCode.KleiKeys && !Input.GetKey((KeyCode)keyDef.mKeyCode)))
			{
				QueueButtonEvent(keyDef, false);
			}
		}
		UpdateModifiers();
	}

	public KKeyCode GetInputForAction(Action action)
	{
		foreach (KInputBinding mBinding in mBindings)
		{
			if (mBinding.mAction == action)
			{
				return mBinding.mKeyCode;
			}
		}
		return KKeyCode.None;
	}
}
