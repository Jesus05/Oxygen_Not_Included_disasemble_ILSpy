using Klei.AI;
using STRINGS;
using System;
using UnityEngine;

public class BingeEatChore : Chore<BingeEatChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, BingeEatChore, object>.GameInstance
	{
		public StatesInstance(BingeEatChore master, GameObject eater)
			: base(master)
		{
			base.sm.eater.Set(eater, base.smi);
			base.sm.bingeremaining.Set(2f, base.smi);
		}

		public void FindFood()
		{
			Navigator component = GetComponent<Navigator>();
			int num = 2147483647;
			Edible edible = null;
			if (base.sm.bingeremaining.Get(base.smi) <= 0f)
			{
				GoTo(base.sm.eat_pst);
			}
			else
			{
				foreach (Edible item in Components.Edibles.Items)
				{
					if (!((UnityEngine.Object)item == (UnityEngine.Object)null) && !((UnityEngine.Object)item == (UnityEngine.Object)base.sm.ediblesource.Get<Edible>(base.smi)) && !(item.GetComponent<Pickupable>().UnreservedAmount <= 0f) && item.GetComponent<Pickupable>().CouldBePickedUpByMinion(base.gameObject))
					{
						int navigationCost = component.GetNavigationCost(item);
						if (navigationCost != -1 && navigationCost < num)
						{
							num = navigationCost;
							edible = item;
						}
					}
				}
				base.sm.ediblesource.Set(edible, base.smi);
				base.sm.requestedfoodunits.Set(base.sm.bingeremaining.Get(base.smi), base.smi);
				if ((UnityEngine.Object)edible == (UnityEngine.Object)null)
				{
					GoTo(base.sm.cantFindFood);
				}
				else
				{
					GoTo(base.sm.fetch);
				}
			}
		}

		public bool IsBingeEating()
		{
			return base.sm.isBingeEating.Get(base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, BingeEatChore>
	{
		public TargetParameter eater;

		public TargetParameter ediblesource;

		public TargetParameter ediblechunk;

		public BoolParameter isBingeEating;

		public FloatParameter requestedfoodunits;

		public FloatParameter actualfoodunits;

		public FloatParameter bingeremaining;

		public State noTarget;

		public State findfood;

		public State eat;

		public State eat_pst;

		public State cantFindFood;

		public State finish;

		public FetchSubState fetch;

		private Effect bingeEatingEffect;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = findfood;
			Target(eater);
			bingeEatingEffect = new Effect("Binge_Eating", DUPLICANTS.MODIFIERS.BINGE_EATING.NAME, DUPLICANTS.MODIFIERS.BINGE_EATING.TOOLTIP, 0f, true, false, true, null, 0f, null);
			bingeEatingEffect.Add(new AttributeModifier(Db.Get().Attributes.Decor.Id, -30f, DUPLICANTS.MODIFIERS.BINGE_EATING.NAME, false, false, true));
			bingeEatingEffect.Add(new AttributeModifier("CaloriesDelta", -6666.6665f, DUPLICANTS.MODIFIERS.BINGE_EATING.NAME, false, false, true));
			Db.Get().effects.Add(bingeEatingEffect);
			root.ToggleEffect((StatesInstance smi) => bingeEatingEffect);
			noTarget.GoTo(finish);
			eat_pst.ToggleAnims("anim_eat_overeat_kanim", 0f).PlayAnim("working_pst").OnAnimQueueComplete(finish);
			finish.Enter(delegate(StatesInstance smi)
			{
				smi.StopSM("complete/no more food");
			});
			findfood.Enter("FindFood", delegate(StatesInstance smi)
			{
				smi.FindFood();
			});
			fetch.InitializeStates(eater, ediblesource, ediblechunk, requestedfoodunits, actualfoodunits, eat, cantFindFood);
			eat.ToggleAnims("anim_eat_overeat_kanim", 0f).QueueAnim("working_loop", true, null).Enter(delegate(StatesInstance smi)
			{
				isBingeEating.Set(true, smi);
			})
				.DoEat(ediblechunk, actualfoodunits, findfood, findfood)
				.Exit("ClearIsBingeEating", delegate(StatesInstance smi)
				{
					isBingeEating.Set(false, smi);
				});
			cantFindFood.ToggleAnims("anim_interrupt_binge_eat_kanim", 0f).PlayAnim("interrupt_binge_eat").OnAnimQueueComplete(noTarget);
		}
	}

	public BingeEatChore(IStateMachineTarget target, Action<Chore> on_complete = null)
		: base(Db.Get().ChoreTypes.BingeEat, target, target.GetComponent<ChoreProvider>(), false, on_complete, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.emergency, 5, false, true, 0, (Tag[])null)
	{
		smi = new StatesInstance(this, target.gameObject);
		Subscribe(1121894420, OnEat);
	}

	private void OnEat(object data)
	{
		Edible edible = (Edible)data;
		if ((UnityEngine.Object)edible != (UnityEngine.Object)null)
		{
			smi.sm.bingeremaining.Set(Mathf.Max(0f, smi.sm.bingeremaining.Get(smi) - edible.unitsConsumed), smi);
		}
	}

	public override void Cleanup()
	{
		base.Cleanup();
		Unsubscribe(1121894420, OnEat);
	}
}
