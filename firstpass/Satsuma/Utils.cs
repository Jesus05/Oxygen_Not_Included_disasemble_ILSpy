using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Satsuma
{
	internal static class Utils
	{
		public static double LargestPowerOfTwo(double d)
		{
			long num = BitConverter.DoubleToInt64Bits(d);
			num &= 0x7FF0000000000000;
			if (num == 9218868437227405312L)
			{
				num = 9214364837600034816L;
			}
			return BitConverter.Int64BitsToDouble(num);
		}

		public static V MakeEntry<K, V>(Dictionary<K, V> dict, K key) where V : new()
		{
			if (dict.TryGetValue(key, out V value))
			{
				return value;
			}
			return dict[key] = new V();
		}

		public static void RemoveAll<T>(HashSet<T> set, Func<T, bool> condition)
		{
			foreach (T item in set.Where(condition).ToList())
			{
				set.Remove(item);
			}
		}

		public static void RemoveAll<K, V>(Dictionary<K, V> dict, Func<K, bool> condition)
		{
			foreach (K item in dict.Keys.Where(condition).ToList())
			{
				dict.Remove(item);
			}
		}

		public static void RemoveLast<T>(List<T> list, T element) where T : IEquatable<T>
		{
			int num = list.Count - 1;
			while (true)
			{
				if (num < 0)
				{
					return;
				}
				if (((IEquatable<T>)element).Equals(list[num]))
				{
					break;
				}
				num--;
			}
			list.RemoveAt(num);
		}

		public static IEnumerable<XElement> ElementsLocal(XElement xParent, string localName)
		{
			return from x in xParent.Elements()
			where x.Name.LocalName == localName
			select x;
		}

		public static XElement ElementLocal(XElement xParent, string localName)
		{
			return ElementsLocal(xParent, localName).FirstOrDefault();
		}
	}
}
