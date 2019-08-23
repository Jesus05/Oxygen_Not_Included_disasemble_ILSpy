using KSerialization;
using STRINGS;
using System.Collections.Generic;
using System.IO;

namespace Database
{
	public class AtLeastOneBuildingForEachDupe : ColonyAchievementRequirement
	{
		private List<Tag> validBuildingTypes = new List<Tag>();

		public AtLeastOneBuildingForEachDupe(List<Tag> validBuildingTypes)
		{
			this.validBuildingTypes = validBuildingTypes;
		}

		public override bool Success()
		{
			int num = 0;
			foreach (BuildingComplete item in Components.BuildingCompletes.Items)
			{
				if (validBuildingTypes.Contains(item.prefabid.PrefabTag))
				{
					num++;
					if (item.prefabid.PrefabTag == (Tag)"FlushToilet" || item.prefabid.PrefabTag == (Tag)"Outhouse")
					{
						return true;
					}
				}
			}
			return Components.LiveMinionIdentities.Items.Count > 0 && num >= Components.LiveMinionIdentities.Items.Count;
		}

		public override bool Fail()
		{
			return Components.LiveMinionIdentities.Items.Count <= 0;
		}

		public override void Deserialize(IReader reader)
		{
			int num = reader.ReadInt32();
			validBuildingTypes = new List<Tag>(num);
			for (int i = 0; i < num; i++)
			{
				string name = reader.ReadKleiString();
				validBuildingTypes.Add(new Tag(name));
			}
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(validBuildingTypes.Count);
			foreach (Tag validBuildingType in validBuildingTypes)
			{
				writer.WriteKleiString(validBuildingType.ToString());
			}
		}

		public override string GetProgress(bool complete)
		{
			if (validBuildingTypes.Contains("FlushToilet"))
			{
				return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_ONE_TOILET;
			}
			if (complete)
			{
				return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_ONE_BED_PER_DUPLICANT;
			}
			int num = 0;
			foreach (BuildingComplete item in Components.BuildingCompletes.Items)
			{
				if (validBuildingTypes.Contains(item.prefabid.PrefabTag))
				{
					num++;
				}
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILING_BEDS, (!complete) ? num : Components.LiveMinionIdentities.Items.Count, Components.LiveMinionIdentities.Items.Count);
		}
	}
}
