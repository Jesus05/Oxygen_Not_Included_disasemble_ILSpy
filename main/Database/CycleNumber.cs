using STRINGS;
using System.IO;

namespace Database
{
	public class CycleNumber : VictoryColonyAchievementRequirement
	{
		private int cycleNumber;

		public CycleNumber(int cycleNumber = 100)
		{
			this.cycleNumber = cycleNumber;
		}

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_CYCLE, cycleNumber);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_CYCLE_DESCRIPTION, cycleNumber);
		}

		public override bool Success()
		{
			return GameClock.Instance.GetCycle() + 1 >= cycleNumber;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(cycleNumber);
		}

		public override void Deserialize(IReader reader)
		{
			cycleNumber = reader.ReadInt32();
		}
	}
}
