using UnityEngine;

namespace Klei.AI
{
	public static class ModifiersExtensions
	{
		public static Attributes GetAttributes(this KMonoBehaviour cmp)
		{
			return cmp.gameObject.GetAttributes();
		}

		public static Attributes GetAttributes(this GameObject go)
		{
			Modifiers component = go.GetComponent<Modifiers>();
			if (!((Object)component != (Object)null))
			{
				return null;
			}
			return component.attributes;
		}

		public static Amounts GetAmounts(this KMonoBehaviour cmp)
		{
			return cmp.gameObject.GetAmounts();
		}

		public static Amounts GetAmounts(this GameObject go)
		{
			Modifiers component = go.GetComponent<Modifiers>();
			if (!((Object)component != (Object)null))
			{
				return null;
			}
			return component.amounts;
		}

		public static Sicknesses GetSicknesses(this KMonoBehaviour cmp)
		{
			return cmp.gameObject.GetSicknesses();
		}

		public static Sicknesses GetSicknesses(this GameObject go)
		{
			Modifiers component = go.GetComponent<Modifiers>();
			if (!((Object)component != (Object)null))
			{
				return null;
			}
			return component.sicknesses;
		}
	}
}
