using System.IO;

namespace Database
{
	public class BeforeCycleNumber : ColonyAchievementRequirement
	{
		private int cycleNumber;

		public BeforeCycleNumber(int cycleNumber = 100)
		{
			this.cycleNumber = cycleNumber;
		}

		public override bool Success()
		{
			return GameClock.Instance.GetCycle() + 1 <= cycleNumber;
		}

		public override bool Fail()
		{
			return !Success();
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
