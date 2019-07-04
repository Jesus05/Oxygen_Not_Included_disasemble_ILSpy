using Klei.AI;
using KSerialization;
using STRINGS;
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

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.NO_DUPLICANTS_HAVE_TRAIT, Db.Get().traits.Get(traitId).Name);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.NO_DUPLICANTS_HAVE_TRAIT_DESCRIPTION, Db.Get().traits.Get(traitId).Name);
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
