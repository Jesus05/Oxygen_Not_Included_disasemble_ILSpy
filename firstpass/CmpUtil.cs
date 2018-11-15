using System;
using System.Collections.Generic;

public class CmpUtil
{
	private static Dictionary<Type, CmpFns> sCmpFns = new Dictionary<Type, CmpFns>();

	public static CmpFns GetCmpFns(string type_name)
	{
		return GetCmpFns(Type.GetType(type_name));
	}

	public static CmpFns GetCmpFns(Type type)
	{
		CmpFns value = null;
		if (!sCmpFns.TryGetValue(type, out value))
		{
			value = new CmpFns(type);
			sCmpFns[type] = value;
		}
		return value;
	}
}
