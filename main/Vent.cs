using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Vent : KMonoBehaviour, IEffectDescriptor
{
	public enum State
	{
		Invalid,
		Ready,
		Blocked,
		OverPressure
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, Vent, object>.GameInstance
	{
		private Exhaust exhaust;

		public StatesInstance(Vent master)
			: base(master)
		{
			exhaust = master.GetComponent<Exhaust>();
		}

		public bool NeedsExhaust()
		{
			return (UnityEngine.Object)exhaust != (UnityEngine.Object)null && base.master.GetEndPointState() != State.Ready && base.master.endpointType == Endpoint.Source;
		}

		public bool Blocked()
		{
			return base.master.GetEndPointState() == State.Blocked && base.master.endpointType != Endpoint.Source;
		}

		public bool OverPressure()
		{
			return (UnityEngine.Object)exhaust != (UnityEngine.Object)null && base.master.GetEndPointState() == State.OverPressure && base.master.endpointType != Endpoint.Source;
		}

		public void CheckTransitions()
		{
			if (NeedsExhaust())
			{
				base.smi.GoTo(base.sm.needExhaust);
			}
			else if (Blocked())
			{
				base.smi.GoTo(base.sm.blocked);
			}
			else if (OverPressure())
			{
				base.smi.GoTo(base.sm.overPressure);
			}
			else
			{
				base.smi.GoTo(base.sm.idle);
			}
		}

		public StatusItem SelectStatusItem(StatusItem gas_status_item, StatusItem liquid_status_item)
		{
			return (base.master.conduitType != ConduitType.Gas) ? liquid_status_item : gas_status_item;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Vent>
	{
		public State idle;

		public State blocked;

		public State overPressure;

		public State needExhaust;

		public State venting;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			root.Update("CheckTransitions", delegate(StatesInstance smi, float dt)
			{
				smi.CheckTransitions();
			}, UpdateRate.SIM_200ms, false);
			blocked.ToggleStatusItem((StatesInstance smi) => smi.SelectStatusItem(Db.Get().BuildingStatusItems.GasVentObstructed, Db.Get().BuildingStatusItems.LiquidVentObstructed), null);
			overPressure.ToggleStatusItem((StatesInstance smi) => smi.SelectStatusItem(Db.Get().BuildingStatusItems.GasVentOverPressure, Db.Get().BuildingStatusItems.LiquidVentOverPressure), null);
		}
	}

	private int cell = -1;

	private int sortKey = 0;

	[Serialize]
	public Dictionary<SimHashes, float> lifeTimeVentMass = new Dictionary<SimHashes, float>();

	private StatesInstance smi;

	[SerializeField]
	public ConduitType conduitType = ConduitType.Gas;

	[SerializeField]
	public Endpoint endpointType = Endpoint.Source;

	[SerializeField]
	public float overpressureMass = 1f;

	[NonSerialized]
	public bool showConnectivityIcons = true;

	[NonSerialized]
	[MyCmpGet]
	public Structure structure;

	public int SortKey
	{
		get
		{
			return sortKey;
		}
		set
		{
			sortKey = value;
		}
	}

	public bool IsBlocked => GetEndPointState() != State.Ready;

	public void UpdateVentedMass(SimHashes element, float mass)
	{
		if (!lifeTimeVentMass.ContainsKey(element))
		{
			lifeTimeVentMass.Add(element, mass);
		}
		else
		{
			Dictionary<SimHashes, float> dictionary;
			SimHashes key;
			(dictionary = lifeTimeVentMass)[key = element] = dictionary[key] + mass;
		}
	}

	public float GetVentedMass(SimHashes element)
	{
		if (!lifeTimeVentMass.ContainsKey(element))
		{
			return 0f;
		}
		return lifeTimeVentMass[element];
	}

	protected override void OnSpawn()
	{
		Building component = GetComponent<Building>();
		cell = component.GetUtilityOutputCell();
		smi = new StatesInstance(this);
		smi.StartSM();
	}

	public State GetEndPointState()
	{
		State result = State.Invalid;
		switch (endpointType)
		{
		case Endpoint.Source:
			result = (IsConnected() ? State.Ready : State.Blocked);
			break;
		case Endpoint.Sink:
		{
			result = State.Ready;
			int num = cell;
			if (!IsValidOutputCell(num))
			{
				result = ((!Grid.Solid[num]) ? State.OverPressure : State.Blocked);
			}
			break;
		}
		}
		return result;
	}

	public bool IsConnected()
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(conduitType);
		UtilityNetwork networkForCell = networkManager.GetNetworkForCell(cell);
		if (networkForCell == null)
		{
			return false;
		}
		return (networkForCell as FlowUtilityNetwork).HasSinks;
	}

	private bool IsValidOutputCell(int output_cell)
	{
		bool result = false;
		if (((UnityEngine.Object)structure == (UnityEngine.Object)null || !structure.IsEntombed()) && !Grid.Solid[output_cell])
		{
			result = (Grid.Mass[output_cell] < overpressureMass);
		}
		return result;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		string formattedMass = GameUtil.GetFormattedMass(overpressureMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.OVER_PRESSURE_MASS, formattedMass), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.OVER_PRESSURE_MASS, formattedMass), Descriptor.DescriptorType.Effect, false));
		return list;
	}
}
