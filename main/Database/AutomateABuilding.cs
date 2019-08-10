using System.IO;
using UnityEngine;

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
					bool flag = false;
					foreach (ILogicEventReceiver receiver in network.Receivers)
					{
						GameObject gameObject = Grid.Objects[receiver.GetLogicCell(), 1];
						if ((Object)gameObject != (Object)null)
						{
							KPrefabID component = gameObject.GetComponent<KPrefabID>();
							if (!component.HasTag(GameTags.TemplateBuilding))
							{
								flag = true;
								break;
							}
						}
					}
					bool flag2 = false;
					foreach (ILogicEventSender sender in network.Senders)
					{
						GameObject gameObject2 = Grid.Objects[sender.GetLogicCell(), 1];
						if ((Object)gameObject2 != (Object)null)
						{
							KPrefabID component2 = gameObject2.GetComponent<KPrefabID>();
							if (!component2.HasTag(GameTags.TemplateBuilding))
							{
								flag2 = true;
								break;
							}
						}
					}
					return flag && flag2;
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
