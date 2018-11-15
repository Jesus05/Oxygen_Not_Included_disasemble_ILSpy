using System;

namespace TMPro
{
	[Serializable]
	public struct GlyphValueRecord
	{
		public float xPlacement;

		public float yPlacement;

		public float xAdvance;

		public float yAdvance;

		public static GlyphValueRecord operator +(GlyphValueRecord a, GlyphValueRecord b)
		{
			GlyphValueRecord result = default(GlyphValueRecord);
			result.xPlacement = a.xPlacement + b.xPlacement;
			result.yPlacement = a.yPlacement + b.yPlacement;
			result.xAdvance = a.xAdvance + b.xAdvance;
			result.yAdvance = a.yAdvance + b.yAdvance;
			return result;
		}
	}
}
