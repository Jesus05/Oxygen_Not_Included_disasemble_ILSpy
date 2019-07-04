using STRINGS;
using System;
using UnityEngine;

public class JetSuitLocker : StateMachineComponent<JetSuitLocker.StatesInstance>, ISecondaryInput
{
	public class States : GameStateMachine<States, StatesInstance, JetSuitLocker>
	{
		public class ChargingStates : State
		{
			public State notoperational;

			public State operational;

			public State nofuel;
		}

		public State empty;

		public ChargingStates charging;

		public State charged;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = empty;
			base.serializable = true;
			root.Update("RefreshMeter", delegate(StatesInstance smi, float dt)
			{
				smi.master.RefreshMeter();
			}, UpdateRate.RENDER_200ms, false);
			empty.EventTransition(GameHashes.OnStorageChange, charging, (StatesInstance smi) => (UnityEngine.Object)smi.master.GetStoredOutfit() != (UnityEngine.Object)null);
			charging.DefaultState(charging.notoperational).EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => (UnityEngine.Object)smi.master.GetStoredOutfit() == (UnityEngine.Object)null).Transition(charged, (StatesInstance smi) => smi.master.IsSuitFullyCharged(), UpdateRate.SIM_200ms);
			charging.notoperational.TagTransition(GameTags.Operational, charging.operational, false);
			charging.operational.TagTransition(GameTags.Operational, charging.notoperational, true).Transition(charging.nofuel, (StatesInstance smi) => !smi.master.HasFuel(), UpdateRate.SIM_200ms).Update("FuelSuit", delegate(StatesInstance smi, float dt)
			{
				smi.master.FuelSuit(dt);
			}, UpdateRate.SIM_1000ms, false);
			charging.nofuel.TagTransition(GameTags.Operational, charging.notoperational, true).Transition(charging.operational, (StatesInstance smi) => smi.master.HasFuel(), UpdateRate.SIM_200ms).ToggleStatusItem(BUILDING.STATUSITEMS.SUIT_LOCKER.NO_FUEL.NAME, BUILDING.STATUSITEMS.SUIT_LOCKER.NO_FUEL.TOOLTIP, "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, default(HashedString), 0, null, null, null);
			charged.EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => (UnityEngine.Object)smi.master.GetStoredOutfit() == (UnityEngine.Object)null);
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, JetSuitLocker, object>.GameInstance
	{
		public StatesInstance(JetSuitLocker jet_suit_locker)
			: base(jet_suit_locker)
		{
		}
	}

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private SuitLocker suit_locker;

	[MyCmpReq]
	private KBatchedAnimController anim_controller;

	public const float FUEL_CAPACITY = 100f;

	[SerializeField]
	public ConduitPortInfo portInfo;

	private int secondaryInputCell = -1;

	private FlowUtilityNetwork.NetworkItem flowNetworkItem;

	private ConduitConsumer fuel_consumer;

	private Tag fuel_tag;

	private MeterController o2_meter;

	private MeterController fuel_meter;

	public float FuelAvailable
	{
		get
		{
			GameObject fuel = GetFuel();
			float result = 0f;
			if ((UnityEngine.Object)fuel != (UnityEngine.Object)null)
			{
				result = fuel.GetComponent<PrimaryElement>().Mass / 100f;
				result = Math.Min(result, 1f);
			}
			return result;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		fuel_tag = SimHashes.Petroleum.CreateTag();
		fuel_consumer = base.gameObject.AddComponent<ConduitConsumer>();
		fuel_consumer.conduitType = portInfo.conduitType;
		fuel_consumer.consumptionRate = 10f;
		fuel_consumer.capacityTag = fuel_tag;
		fuel_consumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		fuel_consumer.forceAlwaysSatisfied = true;
		fuel_consumer.capacityKG = 100f;
		fuel_consumer.useSecondaryInput = true;
		RequireInputs requireInputs = base.gameObject.AddComponent<RequireInputs>();
		requireInputs.conduitConsumer = fuel_consumer;
		requireInputs.SetRequirements(false, true);
		int cell = Grid.PosToCell(base.transform.GetPosition());
		CellOffset rotatedOffset = building.GetRotatedOffset(portInfo.offset);
		secondaryInputCell = Grid.OffsetCell(cell, rotatedOffset);
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(portInfo.conduitType);
		flowNetworkItem = new FlowUtilityNetwork.NetworkItem(portInfo.conduitType, Endpoint.Sink, secondaryInputCell, base.gameObject);
		networkManager.AddToNetworks(secondaryInputCell, flowNetworkItem, true);
		fuel_meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target_1", "meter_petrol", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Vector3.zero, "meter_target_1");
		o2_meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target_2", "meter_oxygen", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Vector3.zero, "meter_target_2");
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(portInfo.conduitType);
		networkManager.RemoveFromNetworks(secondaryInputCell, flowNetworkItem, true);
		base.OnCleanUp();
	}

	private GameObject GetFuel()
	{
		return storage.FindFirst(fuel_tag);
	}

	public bool IsSuitFullyCharged()
	{
		return suit_locker.IsSuitFullyCharged();
	}

	public KPrefabID GetStoredOutfit()
	{
		return suit_locker.GetStoredOutfit();
	}

	private void FuelSuit(float dt)
	{
		KPrefabID storedOutfit = suit_locker.GetStoredOutfit();
		if (!((UnityEngine.Object)storedOutfit == (UnityEngine.Object)null))
		{
			GameObject fuel = GetFuel();
			if (!((UnityEngine.Object)fuel == (UnityEngine.Object)null))
			{
				PrimaryElement component = fuel.GetComponent<PrimaryElement>();
				if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
				{
					JetSuitTank component2 = storedOutfit.GetComponent<JetSuitTank>();
					float a = 375f * dt / 600f;
					a = Mathf.Min(a, 25f - component2.amount);
					a = Mathf.Min(component.Mass, a);
					component.Mass -= a;
					component2.amount += a;
				}
			}
		}
	}

	ConduitType ISecondaryInput.GetSecondaryConduitType()
	{
		return portInfo.conduitType;
	}

	CellOffset ISecondaryInput.GetSecondaryConduitOffset()
	{
		return portInfo.offset;
	}

	public bool HasFuel()
	{
		GameObject fuel = GetFuel();
		if (!((UnityEngine.Object)fuel != (UnityEngine.Object)null))
		{
			return false;
		}
		return fuel.GetComponent<PrimaryElement>().Mass > 0f;
	}

	private void RefreshMeter()
	{
		o2_meter.SetPositionPercent(suit_locker.OxygenAvailable);
		fuel_meter.SetPositionPercent(FuelAvailable);
		anim_controller.SetSymbolVisiblity("oxygen_yes_bloom", suit_locker.IsOxygenTankFull());
		anim_controller.SetSymbolVisiblity("petrol_yes_bloom", IsFuelTankFull());
	}

	public bool IsFuelTankFull()
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (!((UnityEngine.Object)storedOutfit != (UnityEngine.Object)null))
		{
			return false;
		}
		JetSuitTank component = storedOutfit.GetComponent<JetSuitTank>();
		if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
		{
			return component.PercentFull() >= 1f;
		}
		return true;
	}
}
