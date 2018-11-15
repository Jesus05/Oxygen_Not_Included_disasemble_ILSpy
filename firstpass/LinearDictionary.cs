using System;
using System.Collections.Generic;

public class LinearDictionary<Key, Val> where Key : IEquatable<Key>
{
	private List<Key> keys = new List<Key>();

	private List<Val> values = new List<Val>();

	public Val this[Key key]
	{
		get
		{
			Val result = default(Val);
			int idx = GetIdx(key);
			if (idx != -1)
			{
				result = values[idx];
			}
			return result;
		}
		set
		{
			int idx = GetIdx(key);
			if (idx != -1)
			{
				values[idx] = value;
			}
			else
			{
				keys.Add(key);
				values.Add(value);
			}
		}
	}

	private int GetIdx(Key key)
	{
		int count = keys.Count;
		int result = -1;
		for (int i = 0; i < count; i++)
		{
			if (keys[i].Equals(key))
			{
				result = i;
				break;
			}
		}
		return result;
	}
}
