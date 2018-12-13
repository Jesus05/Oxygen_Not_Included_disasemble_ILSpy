using Klei.AI;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class RelaxationPoint : Workable, IEffectDescriptor
{
	public class RelaxationPointSM : GameStateMachine<RelaxationPointSM, RelaxationPointSM.Instance, RelaxationPoint>
	{
		public new class Instance : GameInstance
		{
			public Instance(RelaxationPoint master)
				: base(master)
			{
			}
		}

		public State unoperational;

		public State operational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unoperational;
			unoperational.EventTransition(GameHashes.OperationalChanged, operational, (Instance smi) => smi.GetComponent<Operational>().IsOperational).PlayAnim("off");
			operational.ToggleChore((Instance smi) => smi.master.CreateWorkChore(), unoperational);
		}
	}

	[MyCmpGet]
	private RoomTracker roomTracker;

	[Serialize]
	protected float stopStressingValue;

	public float stressModificationValue;

	public float roomStressModificationValue;

	private RelaxationPointSM.Instance smi;

	private static Effect stressReductionEffect;

	private static Effect roomStressReductionEffect;

	public RelaxationPoint()
	{
		showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GetComponent<KPrefabID>().AddTag(TagManager.Create("RelaxationPoint", MISC.TAGS.RELAXATION_POINT));
		if (stressReductionEffect == null)
		{
			stressReductionEffect = CreateEffect();
			roomStressReductionEffect = CreateRoomEffect();
		}
	}

	public Effect CreateEffect()
	{
		Effect effect = new Effect("StressReduction", DUPLICANTS.MODIFIERS.STRESSREDUCTION.NAME, DUPLICANTS.MODIFIERS.STRESSREDUCTION.TOOLTIP, 0f, true, false, false, null, 0f, null);
		AttributeModifier modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, stressModificationValue / 600f, DUPLICANTS.MODIFIERS.STRESSREDUCTION.NAME, false, false, true);
		effect.Add(modifier);
		return effect;
	}

	public Effect CreateRoomEffect()
	{
		Effect effect = new Effect("RoomRelaxationEffect", DUPLICANTS.MODIFIERS.STRESSREDUCTION_CLINIC.NAME, DUPLICANTS.MODIFIERS.STRESSREDUCTION_CLINIC.TOOLTIP, 0f, true, false, false, null, 0f, null);
		AttributeModifier modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, roomStressModificationValue / 600f, DUPLICANTS.MODIFIERS.STRESSREDUCTION_CLINIC.NAME, false, false, true);
		effect.Add(modifier);
		return effect;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		smi = new RelaxationPointSM.Instance(this);
		smi.StartSM();
		SetWorkTime(float.PositiveInfinity);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if ((Object)roomTracker != (Object)null && roomTracker.room != null && roomTracker.room.roomType == Db.Get().RoomTypes.MassageClinic)
		{
			worker.GetComponent<Effects>().Add(roomStressReductionEffect, false);
		}
		else
		{
			worker.GetComponent<Effects>().Add(stressReductionEffect, false);
		}
		GetComponent<Operational>().SetActive(true, false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup(worker.gameObject);
		if (amountInstance.value <= stopStressingValue)
		{
			return true;
		}
		base.OnWorkTick(worker, dt);
		return false;
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.GetComponent<Effects>().Remove(stressReductionEffect);
		worker.GetComponent<Effects>().Remove(roomStressReductionEffect);
		GetComponent<Operational>().SetActive(false, false);
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
	}

	protected virtual WorkChore<RelaxationPoint> CreateWorkChore()
	{
		return new WorkChore<RelaxationPoint>(Db.Get().ChoreTypes.Relax, this, null, null, false, null, null, null, false, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect);
		list.Add(item);
		return list;
	}
}
