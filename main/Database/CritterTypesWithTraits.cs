using KSerialization;
using System.Collections.Generic;
using System.IO;

namespace Database
{
	public class CritterTypesWithTraits : ColonyAchievementRequirement
	{
		private Dictionary<Tag, bool> critterTypesToCheck = new Dictionary<Tag, bool>();

		private Tag trait;

		private bool hasTrait;

		public CritterTypesWithTraits(List<Tag> critterTypes, bool hasTrait = true)
		{
			foreach (Tag critterType in critterTypes)
			{
				if (!critterTypesToCheck.ContainsKey(critterType))
				{
					critterTypesToCheck.Add(critterType, false);
				}
			}
			this.hasTrait = hasTrait;
			trait = GameTags.Creatures.Wild;
		}

		public override void Update()
		{
			foreach (Capturable item in Components.Capturables.Items)
			{
				if (item.HasTag(trait) == hasTrait && critterTypesToCheck.ContainsKey(item.PrefabID()))
				{
					critterTypesToCheck[item.PrefabID()] = true;
				}
			}
		}

		public override bool Success()
		{
			foreach (KeyValuePair<Tag, bool> item in critterTypesToCheck)
			{
				if (!item.Value)
				{
					return false;
				}
			}
			return true;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(critterTypesToCheck.Count);
			foreach (KeyValuePair<Tag, bool> item in critterTypesToCheck)
			{
				writer.WriteKleiString(item.Key.ToString());
				writer.Write((byte)(item.Value ? 1 : 0));
			}
			writer.Write((byte)(hasTrait ? 1 : 0));
		}

		public override void Deserialize(IReader reader)
		{
			critterTypesToCheck = new Dictionary<Tag, bool>();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string name = reader.ReadKleiString();
				bool value = reader.ReadByte() != 0;
				critterTypesToCheck.Add(new Tag(name), value);
			}
			hasTrait = (reader.ReadByte() != 0);
			trait = GameTags.Creatures.Wild;
		}
	}
}
