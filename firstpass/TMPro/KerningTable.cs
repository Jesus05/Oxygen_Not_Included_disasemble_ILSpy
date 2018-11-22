using System;
using System.Collections.Generic;
using System.Linq;

namespace TMPro
{
	[Serializable]
	public class KerningTable
	{
		public List<KerningPair> kerningPairs;

		public KerningTable()
		{
			kerningPairs = new List<KerningPair>();
		}

		public void AddKerningPair()
		{
			if (kerningPairs.Count == 0)
			{
				kerningPairs.Add(new KerningPair(0u, 0u, 0f));
			}
			else
			{
				uint firstGlyph = kerningPairs.Last().firstGlyph;
				uint secondGlyph = kerningPairs.Last().secondGlyph;
				float xOffset = kerningPairs.Last().xOffset;
				kerningPairs.Add(new KerningPair(firstGlyph, secondGlyph, xOffset));
			}
		}

		public int AddKerningPair(uint first, uint second, float offset)
		{
			int num = kerningPairs.FindIndex((KerningPair item) => item.firstGlyph == first && item.secondGlyph == second);
			if (num != -1)
			{
				return -1;
			}
			kerningPairs.Add(new KerningPair(first, second, offset));
			return 0;
		}

		public int AddGlyphPairAdjustmentRecord(uint first, GlyphValueRecord firstAdjustments, uint second, GlyphValueRecord secondAdjustments)
		{
			int num = kerningPairs.FindIndex((KerningPair item) => item.firstGlyph == first && item.secondGlyph == second);
			if (num != -1)
			{
				return -1;
			}
			kerningPairs.Add(new KerningPair(first, firstAdjustments, second, secondAdjustments));
			return 0;
		}

		public void RemoveKerningPair(int left, int right)
		{
			int num = kerningPairs.FindIndex((KerningPair item) => item.firstGlyph == left && item.secondGlyph == right);
			if (num != -1)
			{
				kerningPairs.RemoveAt(num);
			}
		}

		public void RemoveKerningPair(int index)
		{
			kerningPairs.RemoveAt(index);
		}

		public void SortKerningPairs()
		{
			if (kerningPairs.Count > 0)
			{
				kerningPairs = (from s in kerningPairs
				orderby s.firstGlyph, s.secondGlyph
				select s).ToList();
			}
		}
	}
}
