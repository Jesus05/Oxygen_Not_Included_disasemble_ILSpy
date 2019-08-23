using System;
using System.Collections.Generic;
using System.IO;

namespace Database
{
	public class DupesVsSolidTransferArmFetch : ColonyAchievementRequirement
	{
		public float percentage;

		public int numCycles;

		public int currentCycleCount;

		public bool armsOutPerformingDupesThisCycle;

		public DupesVsSolidTransferArmFetch(float percentage, int numCycles)
		{
			this.percentage = percentage;
			this.numCycles = numCycles;
		}

		public override bool Success()
		{
			Dictionary<int, int> fetchDupeChoreDeliveries = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().fetchDupeChoreDeliveries;
			Dictionary<int, int> fetchAutomatedChoreDeliveries = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().fetchAutomatedChoreDeliveries;
			int num = 0;
			currentCycleCount = 0;
			for (int i = GameClock.Instance.GetCycle() - numCycles; i < GameClock.Instance.GetCycle(); i++)
			{
				if (fetchAutomatedChoreDeliveries.ContainsKey(i) && (!fetchDupeChoreDeliveries.ContainsKey(i) || (float)fetchDupeChoreDeliveries[i] < (float)fetchAutomatedChoreDeliveries[i] * percentage))
				{
					num++;
					if (num >= numCycles)
					{
						currentCycleCount = numCycles;
						return true;
					}
				}
				else
				{
					currentCycleCount = Math.Max(currentCycleCount, num);
					num = 0;
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(numCycles);
			writer.Write(percentage);
		}

		public override void Deserialize(IReader reader)
		{
			numCycles = reader.ReadInt32();
			percentage = reader.ReadSingle();
		}
	}
}
