using STRINGS;
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

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CALORIE_SURPLUS, GameUtil.GetFormattedCalories((!complete) ? RationTracker.Get().CountRations(null, true) : ((float)surplusAmount), GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories((float)surplusAmount, GameUtil.TimeSlice.None, true));
		}
	}
}
