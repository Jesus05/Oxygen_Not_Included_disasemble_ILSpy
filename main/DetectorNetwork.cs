using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class DetectorNetwork : GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>
{
	public class Def : BaseDef
	{
		public int interferenceRadius;

		public float worstWarningTime;

		public float bestWarningTime;

		public int bestNetworkSize;
	}

	public class SelfStates : State
	{
		public NetworkStates self_poor;

		public NetworkStates self_good;
	}

	public class NetworkStates : State
	{
		public State poor;

		public State good;

		public NetworkStates InitializeStates(DetectorNetwork parent)
		{
			DefaultState(poor);
			poor.ToggleStatusItem(BUILDING.STATUSITEMS.NETWORKQUALITY.NAME, BUILDING.STATUSITEMS.NETWORKQUALITY.TOOLTIP, string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, default(HashedString), 0, StringCallback, null, null).ParamTransition(parent.networkQuality, good, (Instance smi, float p) => (double)p >= 0.8);
			good.ToggleStatusItem(BUILDING.STATUSITEMS.NETWORKQUALITY.NAME, BUILDING.STATUSITEMS.NETWORKQUALITY.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 0, StringCallback, null, null).ParamTransition(parent.networkQuality, good, (Instance smi, float p) => (double)p < 0.8);
			return this;
		}

		private string StringCallback(string str, Instance smi)
		{
			MathUtil.MinMax detectTimeRange = smi.GetDetectTimeRange();
			return str.Replace("{TotalQuality}", GameUtil.GetFormattedPercent(smi.ComputeTotalDishQuality() * 100f, GameUtil.TimeSlice.None)).Replace("{WorstTime}", GameUtil.GetFormattedTime(detectTimeRange.min)).Replace("{BestTime}", GameUtil.GetFormattedTime(detectTimeRange.max));
		}
	}

	public new class Instance : GameInstance
	{
		private float closestMachinery = 3.40282347E+38f;

		private int visibleSkyCells;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			Components.DetectorNetworks.Add(this);
			base.StartSM();
		}

		public override void StopSM(string reason)
		{
			base.StopSM(reason);
			Components.DetectorNetworks.Remove(this);
		}

		public void Update(float dt)
		{
			CheckForVisibility();
			CheckForInterference();
			base.sm.selfQuality.Set(GetDishQuality(), base.smi);
			base.sm.networkQuality.Set(ComputeTotalDishQuality(), base.smi);
		}

		private void CheckForVisibility()
		{
			int start_cell = Grid.PosToCell(this);
			int num = 0;
			num += ScanVisiblityLine(start_cell, 1, 1, base.def.interferenceRadius);
			num = (visibleSkyCells = num + ScanVisiblityLine(start_cell, -1, 1, base.def.interferenceRadius));
		}

		public static int ScanVisiblityLine(int start_cell, int x_offset, int y_offset, int radius)
		{
			int num = 0;
			for (int i = 0; Mathf.Abs(i) <= radius; i++)
			{
				int num2 = Grid.OffsetCell(start_cell, i * x_offset, i * y_offset);
				if (Grid.IsValidCell(num2))
				{
					if (Grid.ExposedToSunlight[num2] < 253)
					{
						break;
					}
					num++;
				}
			}
			return num;
		}

		private void CheckForInterference()
		{
			Extents extents = new Extents(Grid.PosToCell(this), base.def.interferenceRadius);
			List<ScenePartitionerEntry> list = new List<ScenePartitionerEntry>();
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.industrialBuildings, list);
			float a = 3.40282347E+38f;
			foreach (ScenePartitionerEntry item in list)
			{
				GameObject gameObject = (GameObject)item.obj;
				if (!((Object)gameObject == (Object)base.gameObject))
				{
					float magnitude = (base.gameObject.transform.GetPosition() - gameObject.transform.GetPosition()).magnitude;
					a = Mathf.Min(a, magnitude);
				}
			}
			closestMachinery = a;
		}

		public float GetDishQuality()
		{
			if (!GetComponent<Operational>().IsOperational)
			{
				return 0f;
			}
			return Mathf.Clamp01(closestMachinery / (float)base.def.interferenceRadius) * Mathf.Clamp01((float)visibleSkyCells / ((float)base.def.interferenceRadius * 2f));
		}

		public float ComputeTotalDishQuality()
		{
			float num = 0f;
			foreach (Instance item in Components.DetectorNetworks.Items)
			{
				num += item.GetDishQuality();
			}
			return num / (float)base.def.bestNetworkSize;
		}

		public MathUtil.MinMax GetDetectTimeRange()
		{
			float t = ComputeTotalDishQuality();
			float min = Mathf.Lerp(base.def.worstWarningTime, base.def.bestWarningTime, t);
			return new MathUtil.MinMax(min, base.def.bestWarningTime);
		}
	}

	public FloatParameter selfQuality;

	public FloatParameter networkQuality;

	public State inoperational;

	public SelfStates operational;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = inoperational;
		inoperational.EventTransition(GameHashes.OperationalChanged, operational, (Instance smi) => smi.GetComponent<Operational>().IsOperational);
		operational.DefaultState(operational.self_poor.poor).Update("CheckForInterference", delegate(Instance smi, float dt)
		{
			smi.Update(dt);
		}, UpdateRate.SIM_1000ms, false).EventTransition(GameHashes.OperationalChanged, inoperational, (Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		operational.self_poor.InitializeStates(this).ToggleStatusItem(BUILDING.STATUSITEMS.DETECTORQUALITY.NAME, BUILDING.STATUSITEMS.DETECTORQUALITY.TOOLTIP, "status_item_interference", StatusItem.IconType.Custom, NotificationType.BadMinor, false, default(HashedString), 0, (string str, Instance smi) => str.Replace("{Quality}", GameUtil.GetFormattedPercent(smi.GetDishQuality() * 100f, GameUtil.TimeSlice.None)), null, null).ParamTransition(selfQuality, operational.self_good, (Instance smi, float p) => (double)p >= 0.8);
		operational.self_good.InitializeStates(this).ToggleStatusItem(BUILDING.STATUSITEMS.DETECTORQUALITY.NAME, BUILDING.STATUSITEMS.DETECTORQUALITY.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 0, (string str, Instance smi) => str.Replace("{Quality}", GameUtil.GetFormattedPercent(smi.GetDishQuality() * 100f, GameUtil.TimeSlice.None)), null, null).ParamTransition(selfQuality, operational.self_poor, (Instance smi, float p) => (double)p < 0.8);
	}
}
