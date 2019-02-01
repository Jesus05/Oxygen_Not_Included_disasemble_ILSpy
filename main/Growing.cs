using Klei.AI;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Growing : StateMachineComponent<Growing.StatesInstance>, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Growing, object>.GameInstance
	{
		public AttributeModifier baseGrowingRate;

		public AttributeModifier wildGrowingRate;

		public AttributeModifier getOldRate;

		public HandleVector<int>.Handle partitionerEntry;

		public StatesInstance(Growing master)
			: base(master)
		{
			baseGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, 0.00166666671f, CREATURES.STATS.MATURITY.GROWING, false, false, true);
			wildGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, 0.000416666677f, CREATURES.STATS.MATURITY.GROWINGWILD, false, false, true);
			getOldRate = new AttributeModifier(master.oldAge.deltaAttribute.Id, 1f, null, false, false, true);
		}

		public bool IsGrown()
		{
			return base.master.IsGrown();
		}

		public bool ReachedNextHarvest()
		{
			return base.master.ReachedNextHarvest();
		}

		public void ClampGrowthToHarvest()
		{
			base.master.ClampGrowthToHarvest();
		}

		public bool IsWilting()
		{
			return (Object)base.master.wiltCondition != (Object)null && base.master.wiltCondition.IsWilting();
		}

		public bool IsSleeping()
		{
			return base.master.GetSMI<CropSleepingMonitor.Instance>()?.IsSleeping() ?? false;
		}

		public bool CanExitStalled()
		{
			return !IsWilting() && !IsSleeping();
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Growing>
	{
		public class GrowingStates : State
		{
			public State wild;

			public State planted;
		}

		public class GrownStates : State
		{
			public State idle;

			public State try_self_harvest;
		}

		public GrowingStates growing;

		public State stalled;

		public GrownStates grown;

		[CompilerGenerated]
		private static StateMachine<States, StatesInstance, Growing, object>.State.Callback _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static StateMachine<States, StatesInstance, Growing, object>.State.Callback _003C_003Ef__mg_0024cache1;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = growing;
			base.serializable = true;
			root.Enter(AddToScenePartitioner).Exit(RemoveFromScenePartitioner);
			growing.EventTransition(GameHashes.Wilt, stalled, (StatesInstance smi) => smi.IsWilting()).EventTransition(GameHashes.CropSleep, stalled, (StatesInstance smi) => smi.IsSleeping()).TriggerOnEnter(GameHashes.Grow, null)
				.Update("CheckGrown", delegate(StatesInstance smi, float dt)
				{
					if (smi.ReachedNextHarvest())
					{
						smi.GoTo(grown);
					}
				}, UpdateRate.SIM_4000ms, false)
				.ToggleStatusItem(Db.Get().CreatureStatusItems.Growing, (StatesInstance smi) => smi.master.GetComponent<Growing>())
				.Enter(delegate(StatesInstance smi)
				{
					State state = (!smi.master.replanted) ? growing.wild : growing.planted;
					smi.GoTo(state);
				});
			growing.wild.ToggleAttributeModifier("GrowingWild", (StatesInstance smi) => smi.wildGrowingRate, null);
			growing.planted.ToggleAttributeModifier("Growing", (StatesInstance smi) => smi.baseGrowingRate, null);
			stalled.EventTransition(GameHashes.WiltRecover, growing, (StatesInstance smi) => smi.CanExitStalled()).EventTransition(GameHashes.CropWakeUp, growing, (StatesInstance smi) => smi.CanExitStalled());
			grown.DefaultState(grown.idle).TriggerOnEnter(GameHashes.Grow, null).Update("CheckNotGrown", delegate(StatesInstance smi, float dt)
			{
				if (!smi.ReachedNextHarvest())
				{
					smi.GoTo(growing);
				}
			}, UpdateRate.SIM_4000ms, false)
				.ToggleAttributeModifier("GettingOld", (StatesInstance smi) => smi.getOldRate, null)
				.Enter(delegate(StatesInstance smi)
				{
					smi.ClampGrowthToHarvest();
				})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.oldAge.SetValue(0f);
				});
			grown.idle.Update("CheckNotGrown", delegate(StatesInstance smi, float dt)
			{
				if (smi.master.oldAge.value >= smi.master.oldAge.GetMax())
				{
					smi.GoTo(grown.try_self_harvest);
				}
			}, UpdateRate.SIM_4000ms, false);
			grown.try_self_harvest.Enter(delegate(StatesInstance smi)
			{
				Harvestable component = smi.master.GetComponent<Harvestable>();
				if ((bool)component)
				{
					bool harvestWhenReady = component.HarvestWhenReady;
					component.ForceCancelHarvest(null);
					component.Harvest();
					if (harvestWhenReady && (Object)component != (Object)null)
					{
						component.SetHarvestWhenReady(true);
					}
				}
				smi.master.maturity.SetValue(0f);
				smi.master.oldAge.SetValue(0f);
			}).GoTo(grown.idle);
		}
	}

	public float growthTime;

	private AmountInstance maturity;

	private AmountInstance oldAge;

	private AttributeModifier baseMaturityMax;

	[Serialize]
	private bool replanted = false;

	[MyCmpGet]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private Modifiers modifiers;

	private Crop _crop;

	private static readonly EventSystem.IntraObjectHandler<Growing> OnNewGameSpawnDelegate = new EventSystem.IntraObjectHandler<Growing>(delegate(Growing component, object data)
	{
		component.OnNewGameSpawn(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Growing> OnReplantDelegate = new EventSystem.IntraObjectHandler<Growing>(delegate(Growing component, object data)
	{
		component.OnReplant(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Growing> ResetGrowthDelegate = new EventSystem.IntraObjectHandler<Growing>(delegate(Growing component, object data)
	{
		component.ResetGrowth(data);
	});

	public bool Replanted => replanted;

	private Crop crop
	{
		get
		{
			if ((Object)_crop == (Object)null)
			{
				_crop = GetComponent<Crop>();
			}
			return _crop;
		}
	}

	private static void AddToScenePartitioner(StatesInstance smi)
	{
		Extents extents = new Extents(Grid.PosToCell(smi), smi.Get<OccupyArea>().OccupiedCellsOffsets);
		smi.partitionerEntry = GameScenePartitioner.Instance.Add(smi.gameObject.name, smi.GetComponent<KPrefabID>(), extents, GameScenePartitioner.Instance.plants, null);
	}

	private static void RemoveFromScenePartitioner(StatesInstance smi)
	{
		GameScenePartitioner.Instance.Free(ref smi.partitionerEntry);
	}

	protected override void OnPrefabInit()
	{
		Amounts amounts = base.gameObject.GetAmounts();
		maturity = amounts.Add(new AmountInstance(Db.Get().Amounts.Maturity, base.gameObject));
		baseMaturityMax = new AttributeModifier(maturity.maxAttribute.Id, growthTime / 600f, null, false, false, true);
		maturity.maxAttribute.Add(baseMaturityMax);
		oldAge = amounts.Add(new AmountInstance(Db.Get().Amounts.OldAge, base.gameObject));
		base.OnPrefabInit();
		Subscribe(1119167081, OnNewGameSpawnDelegate);
		Subscribe(1309017699, OnReplantDelegate);
		Subscribe(1272413801, ResetGrowthDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		base.gameObject.AddTag(GameTags.Plant);
	}

	private void OnNewGameSpawn(object data)
	{
		maturity.SetValue(maturity.maxAttribute.GetTotalValue() * Random.Range(0f, 1f));
	}

	public void Configure(float baseGrowthTime)
	{
		growthTime = baseGrowthTime;
	}

	public bool ReachedNextHarvest()
	{
		return PercentOfCurrentHarvest() >= 1f;
	}

	public bool IsGrown()
	{
		return maturity.value == maturity.GetMax();
	}

	public bool CanGrow()
	{
		return !IsGrown();
	}

	public bool IsGrowing()
	{
		return maturity.GetDelta() > 0f;
	}

	public void ClampGrowthToHarvest()
	{
		maturity.value = maturity.GetMax();
	}

	public float PercentOfCurrentHarvest()
	{
		return maturity.value / maturity.GetMax();
	}

	public float TimeUntilNextHarvest()
	{
		float num = maturity.GetMax() - maturity.value;
		return num / maturity.GetDelta();
	}

	public float DomesticGrowthTime()
	{
		return maturity.GetMax() / base.smi.baseGrowingRate.Value;
	}

	public float WildGrowthTime()
	{
		return maturity.GetMax() / base.smi.wildGrowingRate.Value;
	}

	public float PercentGrown()
	{
		return maturity.value / maturity.GetMax();
	}

	public void OnReplant(object data)
	{
		replanted = true;
	}

	public void ResetGrowth(object data = null)
	{
		maturity.value = 0f;
	}

	public float PercentOldAge()
	{
		return oldAge.value / oldAge.GetMax();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.GROWTHTIME_SIMPLE, GameUtil.GetFormattedCycles(growthTime, "")), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.GROWTHTIME_SIMPLE, GameUtil.GetFormattedCycles(growthTime, "")), Descriptor.DescriptorType.Requirement, false));
		return list;
	}
}
