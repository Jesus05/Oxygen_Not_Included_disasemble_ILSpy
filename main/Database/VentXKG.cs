using STRINGS;
using System.IO;
using UnityEngine;

namespace Database
{
	public class VentXKG : ColonyAchievementRequirement
	{
		private SimHashes element;

		private float kilogramsToVent;

		public VentXKG(SimHashes element, float kilogramsToVent)
		{
			this.element = element;
			this.kilogramsToVent = kilogramsToVent;
		}

		public override bool Success()
		{
			float num = 0f;
			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(ConduitType.Gas);
			foreach (UtilityNetwork network in networkManager.GetNetworks())
			{
				FlowUtilityNetwork flowUtilityNetwork = network as FlowUtilityNetwork;
				if (flowUtilityNetwork != null)
				{
					foreach (FlowUtilityNetwork.IItem sink in flowUtilityNetwork.sinks)
					{
						Vent component = sink.GameObject.GetComponent<Vent>();
						if ((Object)component != (Object)null)
						{
							num += component.GetVentedMass(element);
						}
					}
				}
			}
			return num >= kilogramsToVent;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write((int)element);
			writer.Write(kilogramsToVent);
		}

		public override void Deserialize(IReader reader)
		{
			element = (SimHashes)reader.ReadInt32();
			kilogramsToVent = reader.ReadSingle();
		}

		public override string GetProgress(bool complete)
		{
			float num = 0f;
			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(ConduitType.Gas);
			foreach (UtilityNetwork network in networkManager.GetNetworks())
			{
				FlowUtilityNetwork flowUtilityNetwork = network as FlowUtilityNetwork;
				if (flowUtilityNetwork != null)
				{
					foreach (FlowUtilityNetwork.IItem sink in flowUtilityNetwork.sinks)
					{
						Vent component = sink.GameObject.GetComponent<Vent>();
						if ((Object)component != (Object)null)
						{
							num += component.GetVentedMass(element);
						}
					}
				}
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.VENTED_MASS, GameUtil.GetFormattedMass((!complete) ? num : kilogramsToVent, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"), GameUtil.GetFormattedMass(kilogramsToVent, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"));
		}
	}
}
