using KSerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class SerializedList<ItemType>
{
	[Serialize]
	private byte[] serializationBuffer;

	private List<ItemType> items = new List<ItemType>();

	public int Count => items.Count;

	public ItemType this[int idx]
	{
		get
		{
			return items[idx];
		}
	}

	public IEnumerator<ItemType> GetEnumerator()
	{
		return items.GetEnumerator();
	}

	public void Add(ItemType item)
	{
		items.Add(item);
	}

	public void Remove(ItemType item)
	{
		items.Remove(item);
	}

	public void RemoveAt(int idx)
	{
		items.RemoveAt(idx);
	}

	public bool Contains(ItemType item)
	{
		return items.Contains(item);
	}

	[OnSerializing]
	private void OnSerializing()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(items.Count);
		foreach (ItemType item in items)
		{
			binaryWriter.WriteKleiString(item.GetType().FullName);
			long position = binaryWriter.BaseStream.Position;
			binaryWriter.Write(0);
			long position2 = binaryWriter.BaseStream.Position;
			Serializer.SerializeTypeless(item, binaryWriter);
			long position3 = binaryWriter.BaseStream.Position;
			long num = position3 - position2;
			binaryWriter.BaseStream.Position = position;
			binaryWriter.Write((int)num);
			binaryWriter.BaseStream.Position = position3;
		}
		memoryStream.Flush();
		serializationBuffer = memoryStream.ToArray();
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (serializationBuffer != null)
		{
			FastReader fastReader = new FastReader(serializationBuffer);
			int num = fastReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string text = fastReader.ReadKleiString();
				int num2 = fastReader.ReadInt32();
				int position = fastReader.Position;
				Type type = Type.GetType(text);
				if (type == null)
				{
					DebugUtil.LogWarningArgs("Type no longer exists: " + text);
					fastReader.SkipBytes(num2);
				}
				else
				{
					ItemType val = (typeof(ItemType) == type) ? default(ItemType) : ((ItemType)Activator.CreateInstance(type));
					Deserializer.DeserializeTypeless(val, fastReader);
					if (fastReader.Position != position + num2)
					{
						DebugUtil.LogWarningArgs("Expected to be at offset", position + num2, "but was only at offset", fastReader.Position, ". Skipping to catch up.");
						fastReader.SkipBytes(position + num2 - fastReader.Position);
					}
					items.Add(val);
				}
			}
		}
	}
}
