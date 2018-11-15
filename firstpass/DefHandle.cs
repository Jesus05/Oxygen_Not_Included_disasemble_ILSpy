using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DefHandle
{
	[SerializeField]
	private int defIdx;

	private static List<object> defs = new List<object>();

	public bool IsValid()
	{
		return defIdx > 0;
	}

	public DefType Get<DefType>() where DefType : class, new()
	{
		if (defIdx == 0)
		{
			defs.Add((object)new DefType());
			defIdx = defs.Count;
		}
		return defs[defIdx - 1] as DefType;
	}

	public void Set<DefType>(DefType value) where DefType : class, new()
	{
		defs.Add((object)value);
		defIdx = defs.Count;
	}
}
