using KSerialization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Database
{
	public class ProduceXEngeryWithoutUsingYList : ColonyAchievementRequirement
	{
		private List<Tag> disallowedBuildings = new List<Tag>();

		private float amountToProduce;

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
			return !usedDisallowedBuilding && amountProduced / 1000f > amountToProduce;
		}

		public override bool Fail()
		{
			return usedDisallowedBuilding;
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

		public override void Update()
		{
			foreach (Generator generator in Game.Instance.energySim.Generators)
			{
				if (generator.JoulesAvailable > 0f)
				{
					KPrefabID component = generator.GetComponent<KPrefabID>();
					if (component.HasAnyTags(disallowedBuildings))
					{
						usedDisallowedBuilding = true;
					}
					amountProduced = Mathf.Max(generator.JoulesAvailable, generator.JoulesAvailable + amountProduced);
				}
			}
		}
	}
}
