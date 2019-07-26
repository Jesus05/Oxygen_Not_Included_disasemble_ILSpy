using System.IO;

namespace Database
{
	public class CalorieSurplus : ColonyAchievementRequirement
	{
		private double surplusAmount;

		public CalorieSurplus(float surplusAmount)
		{
			this.surplusAmount = (double)surplusAmount;
		}

		public override bool Success()
		{
			return (double)(RationTracker.Get().CountRations(null, true) / 1000f) >= surplusAmount;
		}

		public override bool Fail()
		{
			return !Success();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(surplusAmount);
		}

		public override void Deserialize(IReader reader)
		{
			surplusAmount = reader.ReadDouble();
		}
	}
}
