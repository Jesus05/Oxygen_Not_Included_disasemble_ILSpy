using KSerialization;
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
	}
}
