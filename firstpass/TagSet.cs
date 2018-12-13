using KSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
[SerializationConfig(MemberSerialization.OptIn)]
public class TagSet : ICollection<Tag>, ICollection, IEnumerable<Tag>, IEnumerable
{
	[Serialize]
	[SerializeField]
	private List<Tag> tags = new List<Tag>();

	public int Count => tags.Count;

	public bool IsReadOnly => false;

	public Tag this[int i]
	{
		get
		{
			return tags[i];
		}
	}

	public bool IsSynchronized
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public object SyncRoot
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public TagSet()
	{
		tags = new List<Tag>();
	}

	public TagSet(TagSet other)
	{
		tags = new List<Tag>(other.tags);
	}

	public TagSet(Tag[] other)
	{
		tags = new List<Tag>(other);
	}

	public TagSet(IEnumerable<string> others)
	{
		tags = new List<Tag>();
		IEnumerator<string> enumerator = others.GetEnumerator();
		while (enumerator.MoveNext())
		{
			tags.Add(new Tag(enumerator.Current));
		}
	}

	public TagSet(params TagSet[] others)
	{
		tags = new List<Tag>();
		for (int i = 0; i < others.Length; i++)
		{
			tags.AddRange(others[i]);
		}
	}

	public TagSet(params string[] others)
	{
		tags = new List<Tag>();
		for (int i = 0; i < others.Length; i++)
		{
			tags.Add(new Tag(others[i]));
		}
	}

	public void Add(Tag item)
	{
		if (!tags.Contains(item))
		{
			tags.Add(item);
		}
	}

	public void Union(TagSet others)
	{
		for (int i = 0; i < others.tags.Count; i++)
		{
			if (!tags.Contains(others.tags[i]))
			{
				tags.Add(others.tags[i]);
			}
		}
	}

	public void Clear()
	{
		tags.Clear();
	}

	public bool Contains(Tag item)
	{
		return tags.Contains(item);
	}

	public bool ContainsAll(TagSet others)
	{
		for (int i = 0; i < others.tags.Count; i++)
		{
			if (!tags.Contains(others.tags[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool ContainsOne(TagSet others)
	{
		for (int i = 0; i < others.tags.Count; i++)
		{
			if (tags.Contains(others.tags[i]))
			{
				return true;
			}
		}
		return false;
	}

	public void CopyTo(Tag[] array, int arrayIndex)
	{
		tags.CopyTo(array, arrayIndex);
	}

	public bool Remove(Tag item)
	{
		return tags.Remove(item);
	}

	public void Remove(TagSet other)
	{
		for (int i = 0; i < other.tags.Count; i++)
		{
			if (tags.Contains(other.tags[i]))
			{
				tags.Remove(other.tags[i]);
			}
		}
	}

	public IEnumerator<Tag> GetEnumerator()
	{
		return tags.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public override string ToString()
	{
		if (tags.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(tags[0].Name);
			for (int i = 1; i < tags.Count; i++)
			{
				stringBuilder.Append(", ");
				stringBuilder.Append(tags[i].Name);
			}
			return stringBuilder.ToString();
		}
		return string.Empty;
	}

	public string GetTagDescription()
	{
		if (tags.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(TagDescriptions.GetDescription(tags[0].ToString()));
			for (int i = 1; i < tags.Count; i++)
			{
				stringBuilder.Append(", ");
				stringBuilder.Append(TagDescriptions.GetDescription(tags[i].ToString()));
			}
			return stringBuilder.ToString();
		}
		return string.Empty;
	}

	public void CopyTo(Array array, int index)
	{
		throw new NotImplementedException();
	}
}
