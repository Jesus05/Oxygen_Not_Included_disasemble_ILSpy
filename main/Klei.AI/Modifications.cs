using KSerialization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Modifications<ModifierType, InstanceType> : ISaveLoadableDetails where ModifierType : Resource where InstanceType : ModifierInstance<ModifierType>
	{
		public List<InstanceType> ModifierList = new List<InstanceType>();

		private ResourceSet<ModifierType> resources;

		public int Count => ModifierList.Count;

		public GameObject gameObject
		{
			get;
			private set;
		}

		public InstanceType this[int idx]
		{
			get
			{
				return ModifierList[idx];
			}
		}

		public Modifications(GameObject go, ResourceSet<ModifierType> resources = null)
		{
			this.resources = resources;
			gameObject = go;
		}

		public IEnumerator<InstanceType> GetEnumerator()
		{
			return ModifierList.GetEnumerator();
		}

		public ComponentType GetComponent<ComponentType>()
		{
			return this.gameObject.GetComponent<ComponentType>();
		}

		public void Trigger(GameHashes hash, object data = null)
		{
			gameObject.GetComponent<KPrefabID>().Trigger((int)hash, data);
		}

		public virtual InstanceType CreateInstance(ModifierType modifier)
		{
			return (InstanceType)null;
		}

		public virtual InstanceType Add(InstanceType instance)
		{
			ModifierList.Add(instance);
			return instance;
		}

		public virtual void Remove(InstanceType instance)
		{
			int num = 0;
			while (true)
			{
				if (num >= ModifierList.Count)
				{
					return;
				}
				InstanceType val = ModifierList[num];
				if (val == instance)
				{
					break;
				}
				num++;
			}
			ModifierList.RemoveAt(num);
			instance.OnCleanUp();
		}

		public bool Has(ModifierType modifier)
		{
			return Get(modifier) != null;
		}

		public InstanceType Get(ModifierType modifier)
		{
			foreach (InstanceType modifier2 in ModifierList)
			{
				if (modifier2.modifier == modifier)
				{
					return modifier2;
				}
			}
			return (InstanceType)null;
		}

		public InstanceType Get(string id)
		{
			foreach (InstanceType modifier in ModifierList)
			{
				if (modifier.modifier.Id == id)
				{
					return modifier;
				}
			}
			return (InstanceType)null;
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(ModifierList.Count);
			foreach (InstanceType modifier in ModifierList)
			{
				writer.WriteKleiString(modifier.modifier.Id);
				long position = writer.BaseStream.Position;
				writer.Write(0);
				long position2 = writer.BaseStream.Position;
				Serializer.SerializeTypeless(modifier, writer);
				long position3 = writer.BaseStream.Position;
				long num = position3 - position2;
				writer.BaseStream.Position = position;
				writer.Write((int)num);
				writer.BaseStream.Position = position3;
			}
		}

		public void Deserialize(IReader reader)
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string text = reader.ReadKleiString();
				int num2 = reader.ReadInt32();
				int position = reader.Position;
				InstanceType val = Get(text);
				if (val == null && resources != null)
				{
					ModifierType val2 = resources.TryGet(text);
					if (val2 != null)
					{
						val = CreateInstance(val2);
					}
				}
				if (val == null)
				{
					if (text != "Condition")
					{
						Output.LogWarning(gameObject.name, "Missing modifier: " + text);
					}
					reader.SkipBytes(num2);
				}
				else if (!(val is ISaveLoadable))
				{
					reader.SkipBytes(num2);
				}
				else
				{
					Deserializer.DeserializeTypeless(val, reader);
					if (reader.Position != position + num2)
					{
						Output.LogWarning("Expected to be at offset", position + num2, "but was only at offset", reader.Position, ". Skipping to catch up.");
						reader.SkipBytes(position + num2 - reader.Position);
					}
				}
			}
		}
	}
}
