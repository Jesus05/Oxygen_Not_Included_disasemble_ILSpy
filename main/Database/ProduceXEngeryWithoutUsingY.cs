using KSerialization;
using STRINGS;
using System.IO;
using UnityEngine;

namespace Database
{
	public class ProduceXEngeryWithoutUsingY : ColonyAchievementRequirement
	{
		private Tag disallowedBuilding;

		private float amountToProduce;

		private float amountProduced;

		private bool usedDisallowedBuilding;

		public ProduceXEngeryWithoutUsingY(float amountToProduce, Tag disallowedBuilding)
		{
			this.disallowedBuilding = disallowedBuilding;
			this.amountToProduce = amountToProduce;
			usedDisallowedBuilding = false;
		}

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.GENERATE_X_WITHOUT_USING_Y, amountToProduce, disallowedBuilding.Name);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.GENERATE_X_WITHOUT_USING_Y_DESCRIPTION, amountToProduce, disallowedBuilding.Name);
		}

		public override bool Success()
		{
			return !usedDisallowedBuilding && amountProduced / 1000f > amountToProduce;
		}

		public override bool Fail()
		{
			return usedDisallowedBuilding;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.WriteKleiString(disallowedBuilding.ToString());
			writer.Write((double)amountProduced);
			writer.Write((double)amountToProduce);
			writer.Write((byte)(usedDisallowedBuilding ? 1 : 0));
		}

		public override void Deserialize(IReader reader)
		{
			string name = reader.ReadKleiString();
			disallowedBuilding = new Tag(name);
			amountProduced = (float)reader.ReadDouble();
			amountToProduce = (float)reader.ReadDouble();
			usedDisallowedBuilding = (reader.ReadByte() != 0);
		}

		public override void Update()
		{
			foreach (Generator generator in Game.Instance.energySim.Generators)
			{
				if (generator.JoulesAvailable > 0f)
				{
					KPrefabID component = generator.GetComponent<KPrefabID>();
					if (component.HasTag(disallowedBuilding))
					{
						usedDisallowedBuilding = true;
					}
					amountProduced = Mathf.Max(generator.JoulesAvailable, generator.JoulesAvailable + amountProduced);
				}
			}
		}
	}
}
