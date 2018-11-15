using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TMPro
{
	[Serializable]
	public class KerningPair
	{
		[FormerlySerializedAs("AscII_Left")]
		[SerializeField]
		private uint m_FirstGlyph;

		[SerializeField]
		private GlyphValueRecord m_FirstGlyphAdjustments;

		[FormerlySerializedAs("AscII_Right")]
		[SerializeField]
		private uint m_SecondGlyph;

		[SerializeField]
		private GlyphValueRecord m_SecondGlyphAdjustments;

		[FormerlySerializedAs("XadvanceOffset")]
		public float xOffset;

		public uint firstGlyph
		{
			get
			{
				return m_FirstGlyph;
			}
			set
			{
				m_FirstGlyph = value;
			}
		}

		public GlyphValueRecord firstGlyphAdjustments => m_FirstGlyphAdjustments;

		public uint secondGlyph
		{
			get
			{
				return m_SecondGlyph;
			}
			set
			{
				m_SecondGlyph = value;
			}
		}

		public GlyphValueRecord secondGlyphAdjustments => m_SecondGlyphAdjustments;

		public KerningPair()
		{
			m_FirstGlyph = 0u;
			m_FirstGlyphAdjustments = default(GlyphValueRecord);
			m_SecondGlyph = 0u;
			m_SecondGlyphAdjustments = default(GlyphValueRecord);
		}

		public KerningPair(uint left, uint right, float offset)
		{
			firstGlyph = left;
			m_SecondGlyph = right;
			xOffset = offset;
		}

		public KerningPair(uint firstGlyph, GlyphValueRecord firstGlyphAdjustments, uint secondGlyph, GlyphValueRecord secondGlyphAdjustments)
		{
			m_FirstGlyph = firstGlyph;
			m_FirstGlyphAdjustments = firstGlyphAdjustments;
			m_SecondGlyph = secondGlyph;
			m_SecondGlyphAdjustments = secondGlyphAdjustments;
		}

		internal void ConvertLegacyKerningData()
		{
			m_FirstGlyphAdjustments.xAdvance = xOffset;
		}
	}
}
