using System.IO;

namespace Database
{
	public class AverageDupeDecorOverCycles : ColonyAchievementRequirement
	{
		private int averageDecor;

		private int numCycles;

		private bool trackingCycles;

		private int startCycle;

		public AverageDupeDecorOverCycles(int averageDecor, int numCycles)
		{
			this.averageDecor = averageDecor;
			this.numCycles = numCycles;
		}

		public override bool Success()
		{
			if (Components.LiveMinionIdentities.Count > 0)
			{
				float num = 0f;
				foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
				{
					DecorMonitor.Instance sMI = item.GetSMI<DecorMonitor.Instance>();
					if (sMI != null)
					{
						num = sMI.GetTodaysAverageDecor();
					}
				}
				float num2 = num / (float)Components.LiveMinionIdentities.Count;
				if (num2 > (float)averageDecor)
				{
					if (!trackingCycles)
					{
						trackingCycles = true;
						startCycle = 0;
					}
					startCycle++;
				}
				else
				{
					trackingCycles = false;
				}
				return num2 > (float)averageDecor && startCycle >= numCycles;
			}
			return false;
		}

		public override void Deserialize(IReader reader)
		{
			averageDecor = reader.ReadInt32();
			numCycles = reader.ReadInt32();
			trackingCycles = (reader.ReadByte() != 0);
			startCycle = reader.ReadInt32();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(averageDecor);
			writer.Write(numCycles);
			writer.Write((byte)(trackingCycles ? 1 : 0));
			writer.Write(startCycle);
		}
	}
}
