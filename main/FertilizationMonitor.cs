using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class FertilizationMonitor : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Def>
{
	public class Def : BaseDef, IGameObjectEffectDescriptor
	{
		public Tag wrongFertilizerTestTag;

		public PlantElementAbsorber.ConsumeInfo[] consumedElements;

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			if (consumedElements.Length <= 0)
			{
				return null;
			}
			List<Descriptor> list = new List<Descriptor>();
			PlantElementAbsorber.ConsumeInfo[] array = consumedElements;
			for (int i = 0; i < array.Length; i++)
			{
				PlantElementAbsorber.ConsumeInfo consumeInfo = array[i];
				list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.IDEAL_FERTILIZER, consumeInfo.tag.ProperName(), GameUtil.GetFormattedMass(0f - consumeInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.IDEAL_FERTILIZER, consumeInfo.tag.ProperName(), GameUtil.GetFormattedMass(consumeInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
			}
			return list;
		}
	}

	public class VariableFertilizerStates : State
	{
		public State normal;

		public State wrongFert;
	}

	public class FertilizedStates : State
	{
		public VariableFertilizerStates decaying;

		public VariableFertilizerStates absorbing;

		public State wilting;
	}

	public class ReplantedStates : State
	{
		public FertilizedStates fertilized;

		public VariableFertilizerStates starved;
	}

	public new class Instance : GameInstance, IWiltCause
	{
		public AttributeModifier consumptionRate;

		public AttributeModifier absorptionRate;

		protected AmountInstance fertilization;

		private Storage storage;

		private HandleVector<int>.Handle absorberHandle = HandleVector<int>.InvalidHandle;

		private float total_available_mass;

		public float total_fertilizer_available => total_available_mass;

		public WiltCondition.Condition[] Conditions => new WiltCondition.Condition[1]
		{
			WiltCondition.Condition.Fertilized
		};

		public string WiltStateString
		{
			get
			{
				string result = "";
				if (base.smi.IsInsideState(base.smi.sm.replanted.fertilized.decaying.wrongFert))
				{
					result = GetIncorrectFertStatusItemMajor().resolveStringCallback(CREATURES.STATUSITEMS.WRONGFERTILIZERMAJOR.NAME, this);
				}
				else if (base.smi.IsInsideState(base.smi.sm.replanted.fertilized.absorbing.wrongFert))
				{
					result = GetIncorrectFertStatusItem().resolveStringCallback(CREATURES.STATUSITEMS.WRONGFERTILIZER.NAME, this);
				}
				else if (base.smi.IsInsideState(base.smi.sm.replanted.starved))
				{
					result = GetStarvedStatusItem().resolveStringCallback(CREATURES.STATUSITEMS.NEEDSFERTILIZER.NAME, this);
				}
				else if (base.smi.IsInsideState(base.smi.sm.replanted.starved.wrongFert))
				{
					result = GetIncorrectFertStatusItemMajor().resolveStringCallback(CREATURES.STATUSITEMS.WRONGFERTILIZERMAJOR.NAME, this);
				}
				return result;
			}
		}

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			AddAmounts(base.gameObject);
			MakeModifiers();
			master.Subscribe(1309017699, SetStorage);
		}

		public virtual StatusItem GetStarvedStatusItem()
		{
			return Db.Get().CreatureStatusItems.NeedsFertilizer;
		}

		public virtual StatusItem GetIncorrectFertStatusItem()
		{
			return Db.Get().CreatureStatusItems.WrongFertilizer;
		}

		public virtual StatusItem GetIncorrectFertStatusItemMajor()
		{
			return Db.Get().CreatureStatusItems.WrongFertilizerMajor;
		}

		protected virtual void AddAmounts(GameObject gameObject)
		{
			Amounts amounts = gameObject.GetAmounts();
			fertilization = amounts.Add(new AmountInstance(Db.Get().Amounts.Fertilization, gameObject));
		}

		protected virtual void MakeModifiers()
		{
			consumptionRate = new AttributeModifier(Db.Get().Amounts.Fertilization.deltaAttribute.Id, -0.166666672f, CREATURES.STATS.FERTILIZATION.CONSUME_MODIFIER, false, false, true);
			absorptionRate = new AttributeModifier(Db.Get().Amounts.Fertilization.deltaAttribute.Id, 1.66666663f, CREATURES.STATS.FERTILIZATION.ABSORBING_MODIFIER, false, false, true);
		}

		public void SetStorage(object obj)
		{
			storage = (Storage)obj;
			base.sm.fertilizerStorage.Set(storage, base.smi);
			IrrigationMonitor.Instance.DumpIncorrectFertilizers(storage, base.smi.gameObject);
			ManualDeliveryKG[] components = base.smi.gameObject.GetComponents<ManualDeliveryKG>();
			foreach (ManualDeliveryKG manualDeliveryKG in components)
			{
				bool flag = false;
				PlantElementAbsorber.ConsumeInfo[] consumedElements = base.def.consumedElements;
				for (int j = 0; j < consumedElements.Length; j++)
				{
					PlantElementAbsorber.ConsumeInfo consumeInfo = consumedElements[j];
					if (manualDeliveryKG.requestedItemTag == consumeInfo.tag)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					manualDeliveryKG.SetStorage(storage);
					manualDeliveryKG.enabled = true;
				}
			}
		}

		public virtual bool AcceptsFertilizer()
		{
			PlantablePlot component = base.sm.fertilizerStorage.Get(this).GetComponent<PlantablePlot>();
			if (!((Object)component != (Object)null))
			{
				return false;
			}
			return component.AcceptsFertilizer;
		}

		public bool Starved()
		{
			return fertilization.value == 0f;
		}

		public void UpdateFertilization(float dt)
		{
			if (base.def.consumedElements != null && !((Object)storage == (Object)null))
			{
				bool value = true;
				bool value2 = false;
				List<GameObject> items = storage.items;
				for (int i = 0; i < base.def.consumedElements.Length; i++)
				{
					PlantElementAbsorber.ConsumeInfo consumeInfo = base.def.consumedElements[i];
					float num = 0f;
					for (int j = 0; j < items.Count; j++)
					{
						GameObject gameObject = items[j];
						if (gameObject.HasTag(consumeInfo.tag))
						{
							num += gameObject.GetComponent<PrimaryElement>().Mass;
						}
						else if (gameObject.HasTag(base.def.wrongFertilizerTestTag))
						{
							value2 = true;
						}
					}
					total_available_mass = num;
					if (num < consumeInfo.massConsumptionRate * dt)
					{
						value = false;
						break;
					}
				}
				base.sm.hasCorrectFertilizer.Set(value, base.smi);
				base.sm.hasIncorrectFertilizer.Set(value2, base.smi);
			}
		}

		public void StartAbsorbing()
		{
			if (!absorberHandle.IsValid() && base.def.consumedElements != null && base.def.consumedElements.Length != 0)
			{
				GameObject gameObject = base.smi.gameObject;
				absorberHandle = Game.Instance.plantElementAbsorbers.Add(storage, base.def.consumedElements);
			}
		}

		public void StopAbsorbing()
		{
			if (absorberHandle.IsValid())
			{
				absorberHandle = Game.Instance.plantElementAbsorbers.Remove(absorberHandle);
			}
		}
	}

	public TargetParameter fertilizerStorage;

	public BoolParameter hasCorrectFertilizer;

	public BoolParameter hasIncorrectFertilizer;

	public GameHashes ResourceRecievedEvent = GameHashes.Fertilized;

	public GameHashes ResourceDepletedEvent = GameHashes.Unfertilized;

	public State wild;

	public State unfertilizable;

	public ReplantedStates replanted;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = wild;
		base.serializable = false;
		wild.ParamTransition(fertilizerStorage, unfertilizable, (Instance smi, GameObject p) => (Object)p != (Object)null);
		unfertilizable.Enter(delegate(Instance smi)
		{
			if (smi.AcceptsFertilizer())
			{
				smi.GoTo(replanted.fertilized);
			}
		});
		replanted.Enter(delegate(Instance smi)
		{
			ManualDeliveryKG[] components = smi.gameObject.GetComponents<ManualDeliveryKG>();
			foreach (ManualDeliveryKG manualDeliveryKG in components)
			{
				manualDeliveryKG.Pause(false, "replanted");
			}
			smi.UpdateFertilization(0.0333333351f);
		}).Target(fertilizerStorage).EventHandler(GameHashes.OnStorageChange, delegate(Instance smi)
		{
			smi.UpdateFertilization(0.2f);
		})
			.Target(masterTarget);
		replanted.fertilized.DefaultState(replanted.fertilized.decaying).TriggerOnEnter(ResourceRecievedEvent, null);
		replanted.fertilized.decaying.DefaultState(replanted.fertilized.decaying.normal).ToggleAttributeModifier("Consuming", (Instance smi) => smi.consumptionRate, null).ParamTransition(hasCorrectFertilizer, replanted.fertilized.absorbing, GameStateMachine<FertilizationMonitor, Instance, IStateMachineTarget, Def>.IsTrue)
			.Update("Decaying", delegate(Instance smi, float dt)
			{
				if (smi.Starved())
				{
					smi.GoTo(replanted.starved);
				}
			}, UpdateRate.SIM_200ms, false);
		replanted.fertilized.decaying.normal.ParamTransition(hasIncorrectFertilizer, replanted.fertilized.decaying.wrongFert, GameStateMachine<FertilizationMonitor, Instance, IStateMachineTarget, Def>.IsTrue);
		replanted.fertilized.decaying.wrongFert.ParamTransition(hasIncorrectFertilizer, replanted.fertilized.decaying.normal, GameStateMachine<FertilizationMonitor, Instance, IStateMachineTarget, Def>.IsFalse);
		replanted.fertilized.absorbing.DefaultState(replanted.fertilized.absorbing.normal).ParamTransition(hasCorrectFertilizer, replanted.fertilized.decaying, GameStateMachine<FertilizationMonitor, Instance, IStateMachineTarget, Def>.IsFalse).ToggleAttributeModifier("Absorbing", (Instance smi) => smi.absorptionRate, null)
			.Enter(delegate(Instance smi)
			{
				smi.StartAbsorbing();
			})
			.EventHandler(GameHashes.Wilt, delegate(Instance smi)
			{
				smi.StopAbsorbing();
			})
			.EventHandler(GameHashes.WiltRecover, delegate(Instance smi)
			{
				smi.StartAbsorbing();
			})
			.Exit(delegate(Instance smi)
			{
				smi.StopAbsorbing();
			});
		replanted.fertilized.absorbing.normal.ParamTransition(hasIncorrectFertilizer, replanted.fertilized.absorbing.wrongFert, GameStateMachine<FertilizationMonitor, Instance, IStateMachineTarget, Def>.IsTrue);
		replanted.fertilized.absorbing.wrongFert.ParamTransition(hasIncorrectFertilizer, replanted.fertilized.absorbing.normal, GameStateMachine<FertilizationMonitor, Instance, IStateMachineTarget, Def>.IsFalse);
		replanted.starved.DefaultState(replanted.starved.normal).TriggerOnEnter(ResourceDepletedEvent, null).ParamTransition(hasCorrectFertilizer, replanted.fertilized, GameStateMachine<FertilizationMonitor, Instance, IStateMachineTarget, Def>.IsTrue);
		replanted.starved.normal.ParamTransition(hasIncorrectFertilizer, replanted.starved.wrongFert, GameStateMachine<FertilizationMonitor, Instance, IStateMachineTarget, Def>.IsTrue);
		replanted.starved.wrongFert.ParamTransition(hasIncorrectFertilizer, replanted.starved.normal, GameStateMachine<FertilizationMonitor, Instance, IStateMachineTarget, Def>.IsFalse);
	}
}
