using System.Collections.Generic;

public struct TagBits
{
	private static Dictionary<Tag, TagBits> tagTable = new Dictionary<Tag, TagBits>();

	private static Dictionary<int, Tag> inverseTagTable = new Dictionary<int, Tag>();

	private ulong bits0;

	private ulong bits1;

	private ulong bits2;

	private ulong bits3;

	private ulong bits4;

	public TagBits(Tag tag)
	{
		bits0 = 0uL;
		bits1 = 0uL;
		bits2 = 0uL;
		bits3 = 0uL;
		bits4 = 0uL;
		SetTag(tag);
	}

	public TagBits(Tag[] tags)
	{
		bits0 = 0uL;
		bits1 = 0uL;
		bits2 = 0uL;
		bits3 = 0uL;
		bits4 = 0uL;
		if (tags != null)
		{
			for (int i = 0; i < tags.Length; i++)
			{
				SetTag(tags[i]);
			}
		}
	}

	public List<Tag> GetTagsVerySlow()
	{
		List<Tag> list = new List<Tag>();
		GetTagsVerySlow(0, bits0, list);
		GetTagsVerySlow(1, bits1, list);
		GetTagsVerySlow(2, bits2, list);
		GetTagsVerySlow(3, bits3, list);
		GetTagsVerySlow(4, bits4, list);
		return list;
	}

	public void GetTagsVerySlow(int bits_idx, ulong bits, List<Tag> tags)
	{
		for (int i = 0; i < 64; i++)
		{
			if (((long)bits & (1L << i)) != 0)
			{
				int key = 64 * bits_idx + i;
				tags.Add(inverseTagTable[key]);
			}
		}
	}

	private static TagBits GetTagBits(Tag tag)
	{
		if (!tagTable.TryGetValue(tag, out TagBits value))
		{
			int count = tagTable.Count;
			value.SetFlag(count);
			tagTable.Add(tag, value);
			inverseTagTable[count] = tag;
			if (tagTable.Count >= 320)
			{
				string text = "Out of tag bits:";
				foreach (KeyValuePair<Tag, TagBits> item in tagTable)
				{
					text = text + "\n" + item.Key.ToString();
				}
				Debug.LogError(text, null);
			}
		}
		return value;
	}

	private void SetFlag(int flag_idx)
	{
		if (flag_idx < 64)
		{
			bits0 |= (ulong)(1L << flag_idx);
		}
		else if (flag_idx < 128)
		{
			bits1 |= (ulong)(1L << flag_idx);
		}
		else if (flag_idx < 192)
		{
			bits2 |= (ulong)(1L << flag_idx);
		}
		else if (flag_idx < 256)
		{
			bits3 |= (ulong)(1L << flag_idx);
		}
		else if (flag_idx < 320)
		{
			bits4 |= (ulong)(1L << flag_idx);
		}
		else
		{
			Debug.LogError("Out of bits!", null);
		}
	}

	public void SetTag(Tag tag)
	{
		TagBits tagBits = GetTagBits(tag);
		bits0 |= tagBits.bits0;
		bits1 |= tagBits.bits1;
		bits2 |= tagBits.bits2;
		bits3 |= tagBits.bits3;
		bits4 |= tagBits.bits4;
	}

	public void Clear(Tag tag)
	{
		TagBits tagBits = GetTagBits(tag);
		bits0 &= ~tagBits.bits0;
		bits1 &= ~tagBits.bits1;
		bits2 &= ~tagBits.bits2;
		bits3 &= ~tagBits.bits3;
		bits4 &= ~tagBits.bits4;
	}

	public bool HasAll(TagBits tag_bits)
	{
		return (bits0 & tag_bits.bits0) == tag_bits.bits0 && (bits1 & tag_bits.bits1) == tag_bits.bits1 && (bits2 & tag_bits.bits2) == tag_bits.bits2 && (bits3 & tag_bits.bits3) == tag_bits.bits3 && (bits4 & tag_bits.bits4) == tag_bits.bits4;
	}

	public bool HasAny(TagBits tag_bits)
	{
		return ((bits0 & tag_bits.bits0) | (bits1 & tag_bits.bits1) | (bits2 & tag_bits.bits2) | (bits3 & tag_bits.bits3) | (bits4 & tag_bits.bits4)) != 0;
	}

	public bool AreEqual(TagBits tag_bits)
	{
		return tag_bits.bits0 == bits0 && tag_bits.bits1 == bits1 && tag_bits.bits2 == bits2 && tag_bits.bits3 == bits3 && tag_bits.bits4 == bits4;
	}

	public static implicit operator TagBits(Tag tag)
	{
		return new TagBits(tag);
	}

	public static implicit operator TagBits(string tag)
	{
		return new TagBits(new Tag(tag));
	}

	public static TagBits operator &(TagBits a, TagBits b)
	{
		TagBits result = default(TagBits);
		result.bits0 = (a.bits0 & b.bits0);
		result.bits1 = (a.bits1 & b.bits1);
		result.bits2 = (a.bits2 & b.bits2);
		result.bits3 = (a.bits3 & b.bits3);
		result.bits4 = (a.bits4 & b.bits4);
		return result;
	}

	public static TagBits operator ~(TagBits tag_bits)
	{
		TagBits result = default(TagBits);
		result.bits0 = ~tag_bits.bits0;
		result.bits1 = ~tag_bits.bits1;
		result.bits2 = ~tag_bits.bits2;
		result.bits3 = ~tag_bits.bits3;
		result.bits4 = ~tag_bits.bits4;
		return result;
	}
}
