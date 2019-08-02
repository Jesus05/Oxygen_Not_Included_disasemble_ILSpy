using System.IO;

namespace Database
{
	public class AutomateABuilding : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			foreach (LogicCircuitNetwork network in Game.Instance.logicCircuitSystem.GetNetworks())
			{
				if (network.Receivers.Count > 0 && network.Senders.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override void Deserialize(IReader reader)
		{
		}
	}
}
