using KSerialization;
using System.IO;

namespace Database
{
	public class UpgradeAllBasicBuildings : ColonyAchievementRequirement
	{
		private Tag basicBuilding;

		private Tag upgradeBuilding;

		public UpgradeAllBasicBuildings(Tag basicBuilding, Tag upgradeBuilding)
		{
			this.basicBuilding = basicBuilding;
			this.upgradeBuilding = upgradeBuilding;
		}

		public override bool Success()
		{
			if (!Db.Get().TechItems.IsTechItemComplete(upgradeBuilding.Name))
			{
				return false;
			}
			bool result = false;
			foreach (BuildingComplete item in Components.BuildingCompletes.Items)
			{
				KPrefabID component = item.GetComponent<KPrefabID>();
				if (component.HasTag(basicBuilding))
				{
					return false;
				}
				if (component.HasTag(upgradeBuilding))
				{
					result = true;
				}
			}
			return result;
		}

		public override void Deserialize(IReader reader)
		{
			string name = reader.ReadKleiString();
			basicBuilding = new Tag(name);
			string name2 = reader.ReadKleiString();
			upgradeBuilding = new Tag(name2);
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.WriteKleiString(basicBuilding.ToString());
			writer.WriteKleiString(upgradeBuilding.ToString());
		}
	}
}
