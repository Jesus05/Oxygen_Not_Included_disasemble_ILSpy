using Klei.AI;
using KSerialization;
using System.IO;

namespace Database
{
	public class DupesWithTrait : ColonyAchievementRequirement
	{
		private bool hasTrait;

		private string traitId;

		public DupesWithTrait(string traitId, bool hasTrait = true)
		{
			this.traitId = traitId;
		}

		public override bool Success()
		{
			foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
			{
				if (item.GetComponent<Traits>().HasTrait(traitId) != hasTrait)
				{
					return false;
				}
			}
			return true;
		}

		public override bool Fail()
		{
			return !Success();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write((byte)(hasTrait ? 1 : 0));
			writer.WriteKleiString(traitId);
		}

		public override void Deserialize(IReader reader)
		{
			hasTrait = (reader.ReadByte() != 0);
			traitId = reader.ReadKleiString();
		}
	}
}
