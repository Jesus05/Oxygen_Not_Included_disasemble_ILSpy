using System;
using System.Runtime.CompilerServices;

public class GasAndLiquidConsumerMonitor : GameStateMachine<GasAndLiquidConsumerMonitor, GasAndLiquidConsumerMonitor.Instance, IStateMachineTarget, GasAndLiquidConsumerMonitor.Def>
{
	public class Def : BaseDef
	{
		public Diet diet;

		public float consumptionRate = 0.5f;

		public float mininmumTimeBetweenMeals = 5f;
	}

	public new class Instance : GameInstance
	{
		public int targetCell = -1;

		private Element targetElement;

		private Navigator navigator;

		private int massUnavailableFrameCount;

		[CompilerGenerated]
		private static Action<Sim.MassConsumedCallback, object> _003C_003Ef__mg_0024cache0;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			navigator = base.smi.GetComponent<Navigator>();
		}

		public void ClearTargetCell()
		{
			targetCell = -1;
			massUnavailableFrameCount = 0;
		}

		public void FindFood()
		{
			targetCell = -1;
			FindTargetGasCell();
		}

		public bool IsConsumableCell(int cell, out Element element)
		{
			element = Grid.Element[cell];
			Diet.Info[] infos = base.smi.def.diet.infos;
			foreach (Diet.Info info in infos)
			{
				if (info.IsMatch(element.tag))
				{
					return true;
				}
			}
			return false;
		}

		public void FindTargetGasCell()
		{
			ConsumableCellQuery consumableCellQuery = new ConsumableCellQuery(base.smi, 25);
			navigator.RunQuery(consumableCellQuery);
			if (consumableCellQuery.success)
			{
				targetCell = consumableCellQuery.GetResultCell();
				targetElement = consumableCellQuery.targetElement;
			}
		}

		public void Consume(float dt)
		{
			int index = Game.Instance.massConsumedCallbackManager.Add(OnMassConsumedCallback, this, "GasAndLiquidConsumerMonitor").index;
			SimMessages.ConsumeMass(Grid.PosToCell(this), targetElement.id, base.def.consumptionRate * dt, 3, index);
		}

		private static void OnMassConsumedCallback(Sim.MassConsumedCallback mcd, object data)
		{
			((Instance)data).OnMassConsumed(mcd);
		}

		private void OnMassConsumed(Sim.MassConsumedCallback mcd)
		{
			if (IsRunning())
			{
				if (mcd.mass > 0f)
				{
					massUnavailableFrameCount = 0;
					Diet.Info dietInfo = base.def.diet.GetDietInfo(targetElement.tag);
					if (dietInfo != null)
					{
						float calories = dietInfo.ConvertConsumptionMassToCalories(mcd.mass);
						CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = default(CreatureCalorieMonitor.CaloriesConsumedEvent);
						caloriesConsumedEvent.tag = targetElement.tag;
						caloriesConsumedEvent.calories = calories;
						CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent2 = caloriesConsumedEvent;
						Trigger(-2038961714, caloriesConsumedEvent2);
					}
				}
				else
				{
					massUnavailableFrameCount++;
					if (massUnavailableFrameCount >= 2)
					{
						Trigger(801383139, null);
					}
				}
			}
		}
	}

	public class ConsumableCellQuery : PathFinderQuery
	{
		public bool success;

		public Element targetElement;

		private Instance smi;

		private int maxIterations;

		public ConsumableCellQuery(Instance smi, int maxIterations)
		{
			this.smi = smi;
			this.maxIterations = maxIterations;
		}

		public override bool IsMatch(int cell, int parent_cell, int cost)
		{
			int cell2 = Grid.CellAbove(cell);
			success = (smi.IsConsumableCell(cell, out targetElement) || (Grid.IsValidCell(cell2) && smi.IsConsumableCell(cell2, out targetElement)));
			return success || --maxIterations <= 0;
		}
	}

	private State cooldown;

	private State satisfied;

	private State lookingforfood;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = cooldown;
		cooldown.Enter("ClearTargetCell", delegate(Instance smi)
		{
			smi.ClearTargetCell();
		}).ScheduleGoTo((Instance smi) => smi.def.mininmumTimeBetweenMeals, satisfied);
		satisfied.Enter("ClearTargetCell", delegate(Instance smi)
		{
			smi.ClearTargetCell();
		}).TagTransition(GameTags.Creatures.Hungry, lookingforfood, false);
		lookingforfood.ToggleBehaviour(GameTags.Creatures.WantsToEat, (Instance smi) => smi.targetCell != -1, delegate(Instance smi)
		{
			smi.GoTo(cooldown);
		}).TagTransition(GameTags.Creatures.Hungry, satisfied, true).Update("FindFood", delegate(Instance smi, float dt)
		{
			smi.FindFood();
		}, UpdateRate.SIM_1000ms, false);
	}
}
