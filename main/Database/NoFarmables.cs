using System.IO;
using UnityEngine;

namespace Database
{
	public class NoFarmables : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			foreach (PlantablePlot item in Components.PlantablePlots.Items)
			{
				if ((Object)item.Occupant != (Object)null)
				{
					Tag[] possibleDepositObjectTags = item.possibleDepositObjectTags;
					foreach (Tag a in possibleDepositObjectTags)
					{
						if (a != GameTags.DecorSeed)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public override bool Fail()
		{
			return !Success();
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}
	}
}
