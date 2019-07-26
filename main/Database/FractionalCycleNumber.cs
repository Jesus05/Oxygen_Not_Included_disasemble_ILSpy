using System.IO;

namespace Database
{
	public class FractionalCycleNumber : ColonyAchievementRequirement
	{
		private float fractionalCycleNumber;

		public FractionalCycleNumber(float fractionalCycleNumber)
		{
			this.fractionalCycleNumber = fractionalCycleNumber;
		}

		public override bool Success()
		{
			int num = (int)fractionalCycleNumber;
			float num2 = fractionalCycleNumber - (float)num;
			return (float)(GameClock.Instance.GetCycle() + 1) > fractionalCycleNumber || (GameClock.Instance.GetCycle() + 1 == num && GameClock.Instance.GetCurrentCycleAsPercentage() >= num2);
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(fractionalCycleNumber);
		}

		public override void Deserialize(IReader reader)
		{
			fractionalCycleNumber = reader.ReadSingle();
		}
	}
}
