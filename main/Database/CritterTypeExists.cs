using KSerialization;
using STRINGS;
using System.Collections.Generic;
using System.IO;

namespace Database
{
	public class CritterTypeExists : ColonyAchievementRequirement
	{
		private List<Tag> critterTypes = new List<Tag>();

		public CritterTypeExists(List<Tag> critterTypes)
		{
			this.critterTypes = critterTypes;
		}

		public override bool Success()
		{
			foreach (Capturable item in Components.Capturables.Items)
			{
				if (critterTypes.Contains(item.PrefabID()))
				{
					return true;
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(critterTypes.Count);
			foreach (Tag critterType in critterTypes)
			{
				writer.WriteKleiString(critterType.ToString());
			}
		}

		public override void Deserialize(IReader reader)
		{
			int num = reader.ReadInt32();
			critterTypes = new List<Tag>(num);
			for (int i = 0; i < num; i++)
			{
				string name = reader.ReadKleiString();
				critterTypes.Add(new Tag(name));
			}
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.HATCH_A_MORPH;
		}
	}
}
