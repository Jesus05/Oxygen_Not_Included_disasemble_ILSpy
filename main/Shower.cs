using Klei;
using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class Shower : Workable, IEffectDescriptor, IGameObjectEffectDescriptor
{
	public class ShowerSM : GameStateMachine<ShowerSM, ShowerSM.Instance, Shower>
	{
		public class OperationalState : State
		{
			public State not_ready;

			public State ready;
		}

		public new class Instance : GameInstance
		{
			private Operational operational;

			private ConduitConsumer consumer;

			private ConduitDispenser dispenser;

			public bool IsOperational => operational.IsOperational && consumer.IsConnected && dispenser.IsConnected;

			public Instance(Shower master)
				: base(master)
			{
				operational = master.GetComponent<Operational>();
				consumer = master.GetComponent<ConduitConsumer>();
				dispenser = master.GetComponent<ConduitDispenser>();
			}

			public void SetActive(bool active)
			{
				operational.SetActive(active, false);
			}

			private bool HasSufficientMass()
			{
				bool result = false;
				PrimaryElement primaryElement = GetComponent<Storage>().FindPrimaryElement(SimHashes.Water);
				if ((Object)primaryElement != (Object)null)
				{
					result = (primaryElement.Mass >= 5f);
				}
				return result;
			}

			public bool OutputFull()
			{
				PrimaryElement primaryElement = GetComponent<Storage>().FindPrimaryElement(SimHashes.DirtyWater);
				if (!((Object)primaryElement != (Object)null))
				{
					return false;
				}
				return primaryElement.Mass >= 5f;
			}

			public bool IsReady()
			{
				if (HasSufficientMass())
				{
					if (!OutputFull())
					{
						return true;
					}
					return false;
				}
				return false;
			}
		}

		public State unoperational;

		public OperationalState operational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unoperational;
			root.Update(UpdateStatusItems, UpdateRate.SIM_200ms, false);
			unoperational.EventTransition(GameHashes.OperationalChanged, operational, (Instance smi) => smi.IsOperational).PlayAnim("off");
			operational.DefaultState(operational.not_ready).EventTransition(GameHashes.OperationalChanged, unoperational, (Instance smi) => !smi.IsOperational);
			operational.not_ready.EventTransition(GameHashes.OnStorageChange, operational.ready, (Instance smi) => smi.IsReady()).PlayAnim("off");
			operational.ready.ToggleChore(CreateShowerChore, operational.not_ready);
		}

		private Chore CreateShowerChore(Instance smi)
		{
			ChoreType shower = Db.Get().ChoreTypes.Shower;
			Shower master = smi.master;
			ScheduleBlockType hygiene = Db.Get().ScheduleBlockTypes.Hygiene;
			return new WorkChore<Shower>(shower, master, null, true, null, null, null, false, hygiene, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
		}

		private void UpdateStatusItems(Instance smi, float dt)
		{
			if (smi.OutputFull())
			{
				smi.master.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull, this);
			}
			else
			{
				smi.master.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull, false);
			}
		}
	}

	private ShowerSM.Instance smi;

	public static string SHOWER_EFFECT = "Showered";

	public SimHashes outputTargetElement;

	public float fractionalDiseaseRemoval;

	public int absoluteDiseaseRemoval;

	private SimUtil.DiseaseInfo accumulatedDisease;

	public const float WATER_PER_USE = 5f;

	private static readonly string[] EffectsRemoved = new string[2]
	{
		"SoakingWet",
		"WetFeet"
	};

	private Shower()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		resetProgressOnStop = true;
		smi = new ShowerSM.Instance(this);
		smi.StartSM();
	}

	protected override void OnStartWork(Worker worker)
	{
		HygieneMonitor.Instance sMI = worker.GetSMI<HygieneMonitor.Instance>();
		base.WorkTimeRemaining = workTime * sMI.GetDirtiness();
		accumulatedDisease = SimUtil.DiseaseInfo.Invalid;
		smi.SetActive(true);
		base.OnStartWork(worker);
	}

	protected override void OnStopWork(Worker worker)
	{
		smi.SetActive(false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		Effects component = worker.GetComponent<Effects>();
		for (int i = 0; i < EffectsRemoved.Length; i++)
		{
			string effect_id = EffectsRemoved[i];
			component.Remove(effect_id);
		}
		component.Add(SHOWER_EFFECT, true);
		worker.GetSMI<HygieneMonitor.Instance>()?.SetDirtiness(0f);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		PrimaryElement component = worker.GetComponent<PrimaryElement>();
		if (component.DiseaseCount > 0)
		{
			SimUtil.DiseaseInfo diseaseInfo = default(SimUtil.DiseaseInfo);
			diseaseInfo.idx = component.DiseaseIdx;
			diseaseInfo.count = Mathf.CeilToInt((float)component.DiseaseCount * (1f - Mathf.Pow(fractionalDiseaseRemoval, dt)) - (float)absoluteDiseaseRemoval);
			SimUtil.DiseaseInfo b = diseaseInfo;
			component.ModifyDiseaseCount(-b.count, "Shower.RemoveDisease");
			accumulatedDisease = SimUtil.CalculateFinalDiseaseInfo(accumulatedDisease, b);
			Storage component2 = GetComponent<Storage>();
			PrimaryElement primaryElement = component2.FindPrimaryElement(outputTargetElement);
			if ((Object)primaryElement != (Object)null)
			{
				PrimaryElement component3 = primaryElement.GetComponent<PrimaryElement>();
				component3.AddDisease(accumulatedDisease.idx, accumulatedDisease.count, "Shower.RemoveDisease");
				accumulatedDisease = SimUtil.DiseaseInfo.Invalid;
			}
		}
		return false;
	}

	protected override void OnAbortWork(Worker worker)
	{
		base.OnAbortWork(worker);
		worker.GetSMI<HygieneMonitor.Instance>()?.SetDirtiness(1f - GetPercentComplete());
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (EffectsRemoved.Length > 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.BUILDINGEFFECTS.REMOVESEFFECTSUBTITLE, UI.BUILDINGEFFECTS.TOOLTIPS.REMOVESEFFECTSUBTITLE, Descriptor.DescriptorType.Effect);
			list.Add(item);
			for (int i = 0; i < EffectsRemoved.Length; i++)
			{
				string text = EffectsRemoved[i];
				string arg = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".NAME");
				string arg2 = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".CAUSE");
				Descriptor item2 = default(Descriptor);
				item2.IncreaseIndent();
				item2.SetupDescriptor("• " + string.Format(UI.BUILDINGEFFECTS.REMOVEDEFFECT, arg), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REMOVEDEFFECT, arg2), Descriptor.DescriptorType.Effect);
				list.Add(item2);
			}
		}
		Effect.AddModifierDescriptions(base.gameObject, list, SHOWER_EFFECT, true);
		return list;
	}
}
