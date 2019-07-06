using System.Collections.Generic;

[SkipSaveFileSerialization]
public class Operational : KMonoBehaviour
{
	public class Flag
	{
		public enum Type
		{
			Requirement,
			Functional
		}

		public string Name;

		public Type FlagType;

		public Flag(string name, Type type)
		{
			Name = name;
			FlagType = type;
		}
	}

	public Dictionary<Flag, bool> Flags = new Dictionary<Flag, bool>();

	public bool IsOperational
	{
		get;
		private set;
	}

	public bool IsFunctional
	{
		get;
		private set;
	}

	public bool IsActive
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		UpdateFunctional();
		UpdateOperational();
	}

	public bool IsOperationalType(Flag.Type type)
	{
		if (type != Flag.Type.Functional)
		{
			return IsOperational;
		}
		return IsFunctional;
	}

	public void SetFlag(Flag flag, bool value)
	{
		bool value2 = false;
		if (Flags.TryGetValue(flag, out value2))
		{
			if (value2 != value)
			{
				Flags[flag] = value;
				Trigger(187661686, flag);
			}
		}
		else
		{
			Flags[flag] = value;
			Trigger(187661686, flag);
		}
		if (flag.FlagType == Flag.Type.Functional && value != IsFunctional)
		{
			UpdateFunctional();
		}
		if (value != IsOperational)
		{
			UpdateOperational();
		}
	}

	public bool GetFlag(Flag flag)
	{
		bool value = false;
		Flags.TryGetValue(flag, out value);
		return value;
	}

	private void UpdateFunctional()
	{
		bool isFunctional = true;
		foreach (KeyValuePair<Flag, bool> flag in Flags)
		{
			if (flag.Key.FlagType == Flag.Type.Functional && !flag.Value)
			{
				isFunctional = false;
				break;
			}
		}
		IsFunctional = isFunctional;
		Trigger(-1852328367, IsFunctional);
	}

	private void UpdateOperational()
	{
		Dictionary<Flag, bool>.Enumerator enumerator = Flags.GetEnumerator();
		bool flag = true;
		while (enumerator.MoveNext())
		{
			if (!enumerator.Current.Value)
			{
				flag = false;
				break;
			}
		}
		if (flag != IsOperational)
		{
			IsOperational = flag;
			if (!IsOperational)
			{
				SetActive(false, false);
			}
			if (IsOperational)
			{
				GetComponent<KPrefabID>().AddTag(GameTags.Operational, false);
			}
			else
			{
				GetComponent<KPrefabID>().RemoveTag(GameTags.Operational);
			}
			Trigger(-592767678, IsOperational);
			Game.Instance.Trigger(-809948329, base.gameObject);
		}
	}

	public void SetActive(bool value, bool force_ignore = false)
	{
		if (IsActive != value)
		{
			IsActive = value;
			Trigger(824508782, this);
			Game.Instance.Trigger(-809948329, base.gameObject);
		}
	}
}
