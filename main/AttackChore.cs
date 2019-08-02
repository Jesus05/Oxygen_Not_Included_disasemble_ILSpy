using System;
using UnityEngine;

public class AttackChore : Chore<AttackChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, AttackChore, object>.GameInstance
	{
		public StatesInstance(AttackChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, AttackChore>
	{
		public TargetParameter attackTarget;

		public TargetParameter attacker;

		public ApproachSubState<RangedAttackable> approachtarget;

		public State attack;

		public State success;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = approachtarget;
			root.ToggleStatusItem(Db.Get().DuplicantStatusItems.Fighting, (StatesInstance smi) => smi.master.gameObject).EventHandler(GameHashes.TargetLost, delegate(StatesInstance smi)
			{
				smi.master.Fail("target lost");
			}).Enter(delegate(StatesInstance smi)
			{
				smi.master.GetComponent<Weapon>().Configure(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
			});
			approachtarget.InitializeStates(attacker, attackTarget, attack, null, null, NavigationTactics.Range_3_ProhibitOverlap).Enter(delegate(StatesInstance smi)
			{
				smi.master.CleanUpMultitool();
				smi.master.Trigger(1039067354, attackTarget.Get(smi));
				Health component3 = attackTarget.Get(smi).GetComponent<Health>();
				if ((UnityEngine.Object)component3 == (UnityEngine.Object)null || component3.IsDefeated())
				{
					smi.StopSM("target defeated");
				}
			});
			attack.Target(attacker).Enter(delegate(StatesInstance smi)
			{
				attackTarget.Get(smi).Subscribe(1088554450, smi.master.OnTargetMoved);
				if (attackTarget != null && smi.master.multiTool == null)
				{
					smi.master.multiTool = new MultitoolController.Instance(attackTarget.Get(smi).GetComponent<Workable>(), smi.master.GetComponent<Worker>(), "attack", Assets.GetPrefab(EffectConfigs.AttackSplashId));
					smi.master.multiTool.StartSM();
				}
				attackTarget.Get(smi).Subscribe(1969584890, smi.master.OnTargetDestroyed);
				smi.ScheduleGoTo(0.5f, success);
			}).Update(delegate(StatesInstance smi, float dt)
			{
				if (smi.master.multiTool != null)
				{
					smi.master.multiTool.UpdateHitEffectTarget();
				}
			}, UpdateRate.SIM_200ms, false)
				.Exit(delegate(StatesInstance smi)
				{
					if ((UnityEngine.Object)attackTarget.Get(smi) != (UnityEngine.Object)null)
					{
						attackTarget.Get(smi).Unsubscribe(1088554450, smi.master.OnTargetMoved);
					}
				});
			success.Enter("finishAttack", delegate(StatesInstance smi)
			{
				if ((UnityEngine.Object)attackTarget.Get(smi) != (UnityEngine.Object)null)
				{
					Transform transform = attackTarget.Get(smi).transform;
					Weapon component = attacker.Get(smi).gameObject.GetComponent<Weapon>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						component.AttackTarget(transform.gameObject);
						Health component2 = attackTarget.Get(smi).GetComponent<Health>();
						if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
						{
							if (!component2.IsDefeated())
							{
								smi.GoTo(attack);
							}
							else
							{
								smi.master.CleanUpMultitool();
								smi.StopSM("target defeated");
							}
						}
					}
					else
					{
						smi.master.CleanUpMultitool();
						smi.StopSM("no weapon");
					}
				}
				else
				{
					smi.master.CleanUpMultitool();
					smi.StopSM("no target");
				}
			}).ReturnSuccess();
		}
	}

	private MultitoolController.Instance multiTool;

	public AttackChore(IStateMachineTarget target, GameObject enemy)
		: base(Db.Get().ChoreTypes.Attack, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.basic, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this);
		base.smi.sm.attackTarget.Set(enemy, base.smi);
	}

	protected override void OnStateMachineStop(string reason, StateMachine.Status status)
	{
		CleanUpMultitool();
		base.OnStateMachineStop(reason, status);
	}

	public string GetHitAnim()
	{
		Workable component = base.smi.sm.attackTarget.Get(base.smi).gameObject.GetComponent<Workable>();
		if ((bool)component)
		{
			string text = MultitoolController.GetAnimationStrings(component, gameObject.GetComponent<Worker>(), "hit")[1];
			return text.Replace("_loop", string.Empty);
		}
		return "hit";
	}

	public void OnTargetMoved(object data)
	{
		int num = Grid.PosToCell(base.smi.master.gameObject);
		if ((UnityEngine.Object)base.smi.sm.attackTarget.Get(base.smi) == (UnityEngine.Object)null)
		{
			CleanUpMultitool();
		}
		else
		{
			if (base.smi.GetCurrentState() == base.smi.sm.attack)
			{
				int num2 = Grid.PosToCell(base.smi.sm.attackTarget.Get(base.smi).gameObject);
				CellOffset[] array = null;
				IApproachable component = base.smi.sm.attackTarget.Get(base.smi).gameObject.GetComponent<IApproachable>();
				if (component != null)
				{
					array = component.GetOffsets();
					if (num == num2 || !Grid.IsCellOffsetOf(num, num2, array))
					{
						if (multiTool != null)
						{
							CleanUpMultitool();
						}
						base.smi.GoTo(base.smi.sm.approachtarget);
					}
				}
				else
				{
					Debug.Log("has no approachable");
				}
			}
			if (multiTool != null)
			{
				multiTool.UpdateHitEffectTarget();
			}
		}
	}

	public override void Begin(Precondition.Context context)
	{
		base.smi.sm.attacker.Set(context.consumerState.gameObject, base.smi);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		CleanUpMultitool();
		base.End(reason);
	}

	public void OnTargetDestroyed(object data)
	{
		Fail("target destroyed");
	}

	private void CleanUpMultitool()
	{
		if (base.smi.master.multiTool != null)
		{
			multiTool.DestroyHitEffect();
			multiTool.StopSM("attack complete");
			multiTool = null;
		}
	}
}
