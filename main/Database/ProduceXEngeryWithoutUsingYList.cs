using KSerialization;
using System.Collections.Generic;
using System.IO;

namespace Database
{
	public class ProduceXEngeryWithoutUsingYList : ColonyAchievementRequirement
	{
		public List<Tag> disallowedBuildings = new List<Tag>();

		public float amountToProduce;

		private float amountProduced;

		private bool usedDisallowedBuilding;

		public ProduceXEngeryWithoutUsingYList(float amountToProduce, List<Tag> disallowedBuildings)
		{
			this.disallowedBuildings = disallowedBuildings;
			this.amountToProduce = amountToProduce;
			usedDisallowedBuilding = false;
		}

		public override bool Success()
		{
			float num = 0f;
			foreach (KeyValuePair<Tag, float> item in Game.Instance.savedInfo.powerCreatedbyGeneratorType)
			{
				if (!disallowedBuildings.Contains(item.Key))
				{
					num += item.Value;
				}
			}
			return num / 1000f > amountToProduce;
		}

		public override bool Fail()
		{
			foreach (Tag disallowedBuilding in disallowedBuildings)
			{
				if (Game.Instance.savedInfo.powerCreatedbyGeneratorType.ContainsKey(disallowedBuilding))
				{
					return true;
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(disallowedBuildings.Count);
			foreach (Tag disallowedBuilding in disallowedBuildings)
			{
				writer.WriteKleiString(disallowedBuilding.ToString());
			}
			writer.Write((double)amountProduced);
			writer.Write((double)amountToProduce);
			writer.Write((byte)(usedDisallowedBuilding ? 1 : 0));
		}

		public override void Deserialize(IReader reader)
		{
			int num = reader.ReadInt32();
			disallowedBuildings = new List<Tag>(num);
			for (int i = 0; i < num; i++)
			{
				string name = reader.ReadKleiString();
				disallowedBuildings.Add(new Tag(name));
			}
			amountProduced = (float)reader.ReadDouble();
			amountToProduce = (float)reader.ReadDouble();
			usedDisallowedBuilding = (reader.ReadByte() != 0);
		}

		public float GetProductionAmount(bool complete)
		{
			float num = 0f;
			foreach (KeyValuePair<Tag, float> item in Game.Instance.savedInfo.powerCreatedbyGeneratorType)
			{
				if (!disallowedBuildings.Contains(item.Key))
				{
					num += item.Value;
				}
			}
			return (!complete) ? num : amountToProduce;
		}
	}
}
