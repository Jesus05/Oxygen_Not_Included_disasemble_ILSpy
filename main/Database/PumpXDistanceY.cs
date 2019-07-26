using System.IO;
using UnityEngine;

namespace Database
{
	public class PumpXDistanceY : ColonyAchievementRequirement
	{
		private ConduitType conduitType = ConduitType.Liquid;

		private SimHashes element;

		private double distance;

		public PumpXDistanceY(SimHashes element, float distance)
		{
			this.element = element;
			conduitType = ((!ElementLoader.GetElement(element.CreateTag()).IsLiquid) ? ConduitType.Gas : ConduitType.Liquid);
			this.distance = (double)distance;
		}

		public override bool Success()
		{
			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(conduitType);
			ConduitFlow flowManager = Conduit.GetFlowManager(conduitType);
			foreach (UtilityNetwork network in networkManager.GetNetworks())
			{
				FlowUtilityNetwork flowUtilityNetwork = network as FlowUtilityNetwork;
				if (flowUtilityNetwork != null)
				{
					foreach (FlowUtilityNetwork.IItem sink in flowUtilityNetwork.sinks)
					{
						ConduitConsumer component = sink.GameObject.GetComponent<ConduitConsumer>();
						if (!((Object)component == (Object)null) && component.IsConnected && component.lastConsumedElement == element)
						{
							Vector3 a = Grid.CellToPos(sink.Cell);
							foreach (FlowUtilityNetwork.IItem source in flowUtilityNetwork.sources)
							{
								ConduitDispenser component2 = source.GameObject.GetComponent<ConduitDispenser>();
								if (!((Object)component2 == (Object)null) && component2.IsConnected)
								{
									ConduitFlow.ConduitContents conduitContents = component2.ConduitContents;
									if (conduitContents.element == element)
									{
										Vector3 b = Grid.CellToPos(source.Cell);
										if ((double)Vector3.Distance(a, b) > distance)
										{
											return true;
										}
									}
								}
							}
						}
					}
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write((int)element);
			writer.Write((int)conduitType);
			writer.Write(distance);
		}

		public override void Deserialize(IReader reader)
		{
			element = (SimHashes)reader.ReadInt32();
			conduitType = (ConduitType)reader.ReadInt32();
			distance = reader.ReadDouble();
		}
	}
}
