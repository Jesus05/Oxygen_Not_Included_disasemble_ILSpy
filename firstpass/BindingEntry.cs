using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

public struct BindingEntry : IEquatable<BindingEntry>
{
	[JsonIgnore]
	public string mGroup;

	[JsonIgnore]
	public bool mRebindable;

	[JsonIgnore]
	public bool mIgnoreRootConflics;

	[JsonConverter(typeof(StringEnumConverter))]
	public GamepadButton mButton;

	[JsonConverter(typeof(StringEnumConverter))]
	public KKeyCode mKeyCode;

	[JsonConverter(typeof(StringEnumConverter))]
	public Action mAction;

	[JsonConverter(typeof(StringEnumConverter))]
	public Modifier mModifier;

	public BindingEntry(string group, GamepadButton button, KKeyCode key_code, Modifier modifier, Action action, bool rebindable = true, bool ignore_root_conflicts = false)
	{
		mGroup = group;
		mButton = button;
		mKeyCode = key_code;
		mAction = action;
		mModifier = modifier;
		mRebindable = rebindable;
		mIgnoreRootConflics = ignore_root_conflicts;
	}

	public static KKeyCode GetGamepadKeyCode(int gamepad_number, GamepadButton button)
	{
		switch (gamepad_number)
		{
		case 0:
			return (KKeyCode)(button + 350);
		case 1:
			return (KKeyCode)(button + 370);
		case 2:
			return (KKeyCode)(button + 390);
		case 3:
			return (KKeyCode)(button + 410);
		default:
			DebugUtil.Assert(false, "Assert!", string.Empty, string.Empty);
			return KKeyCode.None;
		}
	}

	public bool Equals(BindingEntry other)
	{
		return this == other;
	}

	public static bool operator ==(BindingEntry a, BindingEntry b)
	{
		return a.mGroup == b.mGroup && a.mButton == b.mButton && a.mKeyCode == b.mKeyCode && a.mAction == b.mAction && a.mModifier == b.mModifier && a.mRebindable == b.mRebindable;
	}

	public bool IsBindingEqual(BindingEntry other)
	{
		return mButton == other.mButton && mKeyCode == other.mKeyCode && mModifier == other.mModifier;
	}

	public static bool operator !=(BindingEntry a, BindingEntry b)
	{
		return !(a == b);
	}

	public override bool Equals(object o)
	{
		if (!(o is BindingEntry))
		{
			return false;
		}
		BindingEntry b = (BindingEntry)o;
		return this == b;
	}

	public override int GetHashCode()
	{
		return (int)mButton ^ (int)mKeyCode ^ (int)mAction;
	}
}
