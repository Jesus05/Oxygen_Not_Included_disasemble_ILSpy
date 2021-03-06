using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EatChore : Chore<EatChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, EatChore, object>.GameInstance
	{
		private int locatorCell;

		public StatesInstance(EatChore master)
			: base(master)
		{
		}

		public void UpdateMessStation()
		{
			Ownables soleOwner = base.sm.eater.Get(base.smi).GetComponent<MinionIdentity>().GetSoleOwner();
			List<Assignable> preferredAssignables = Game.Instance.assignmentManager.GetPreferredAssignables(soleOwner, Db.Get().AssignableSlots.MessStation);
			if (preferredAssignables.Count == 0)
			{
				soleOwner.AutoAssignSlot(Db.Get().AssignableSlots.MessStation);
				preferredAssignables = Game.Instance.assignmentManager.GetPreferredAssignables(soleOwner, Db.Get().AssignableSlots.MessStation);
			}
			Assignable value = (preferredAssignables.Count <= 0) ? null : preferredAssignables[0];
			base.smi.sm.messstation.Set(value, base.smi);
		}

		public bool UseSalt()
		{
			if (base.smi.sm.messstation != null && (UnityEngine.Object)base.smi.sm.messstation.Get(base.smi) != (UnityEngine.Object)null)
			{
				MessStation component = base.smi.sm.messstation.Get(base.smi).GetComponent<MessStation>();
				return (UnityEngine.Object)component != (UnityEngine.Object)null && component.HasSalt;
			}
			return false;
		}

		public void CreateLocator()
		{
			int num = base.sm.eater.Get<Sensors>(base.smi).GetSensor<SafeCellSensor>().GetCellQuery();
			if (num == Grid.InvalidCell)
			{
				num = Grid.PosToCell(base.sm.eater.Get<Transform>(base.smi).GetPosition());
			}
			Vector3 pos = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			Grid.Reserved[num] = true;
			GameObject value = ChoreHelpers.CreateLocator("EatLocator", pos);
			base.sm.locator.Set(value, this);
			locatorCell = num;
		}

		public void DestroyLocator()
		{
			Grid.Reserved[locatorCell] = false;
			ChoreHelpers.DestroyLocator(base.sm.locator.Get(this));
			base.sm.locator.Set(null, this);
		}

		public void SetZ(GameObject go, float z)
		{
			Vector3 position = go.transform.GetPosition();
			position.z = z;
			go.transform.SetPosition(position);
		}

		public void ApplyRoomEffects()
		{
			Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.sm.messstation.Get(base.smi).gameObject);
			if (roomOfGameObject != null)
			{
				RoomType roomType = roomOfGameObject.roomType;
				roomType.TriggerRoomEffects(base.sm.messstation.Get(base.smi).gameObject.GetComponent<KPrefabID>(), base.sm.eater.Get(base.smi).gameObject.GetComponent<Effects>());
			}
		}

		public void ApplySaltEffect()
		{
			Storage component = base.sm.messstation.Get(base.smi).gameObject.GetComponent<Storage>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.Has(TableSaltConfig.ID.ToTag()))
			{
				component.ConsumeIgnoringDisease(TableSaltConfig.ID.ToTag(), TableSaltTuning.CONSUMABLE_RATE);
				Worker component2 = base.sm.eater.Get(base.smi).gameObject.GetComponent<Worker>();
				Effects component3 = component2.GetComponent<Effects>();
				component3.Add("MessTableSalt", true);
				base.sm.messstation.Get(base.smi).gameObject.Trigger(1356255274, null);
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, EatChore>
	{
		public class EatOnFloorState : State
		{
			public ApproachSubState<IApproachable> moveto;

			public State eat;
		}

		public class EatAtMessStationState : State
		{
			public ApproachSubState<MessStation> moveto;

			public State eat;
		}

		public TargetParameter eater;

		public TargetParameter ediblesource;

		public TargetParameter ediblechunk;

		public TargetParameter messstation;

		public FloatParameter requestedfoodunits;

		public FloatParameter actualfoodunits;

		public TargetParameter locator;

		public FetchSubState fetch;

		public EatOnFloorState eatonfloorstate;

		public EatAtMessStationState eatatmessstation;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = fetch;
			Target(eater);
			root.Enter("SetMessStation", delegate(StatesInstance smi)
			{
				smi.UpdateMessStation();
			}).EventHandler(GameHashes.AssignablesChanged, delegate(StatesInstance smi)
			{
				smi.UpdateMessStation();
			});
			fetch.InitializeStates(eater, ediblesource, ediblechunk, requestedfoodunits, actualfoodunits, eatatmessstation, null);
			eatatmessstation.DefaultState(eatatmessstation.moveto).ParamTransition(messstation, eatonfloorstate, (StatesInstance smi, GameObject p) => (UnityEngine.Object)p == (UnityEngine.Object)null);
			eatatmessstation.moveto.InitializeStates(eater, messstation, eatatmessstation.eat, eatonfloorstate, null, null);
			eatatmessstation.eat.ToggleAnims("anim_eat_table_kanim", 0f).DoEat(ediblechunk, actualfoodunits, null, null).Enter(delegate(StatesInstance smi)
			{
				smi.SetZ(eater.Get(smi), Grid.GetLayerZ(Grid.SceneLayer.BuildingFront));
				smi.ApplyRoomEffects();
				smi.ApplySaltEffect();
			})
				.Exit(delegate(StatesInstance smi)
				{
					smi.SetZ(eater.Get(smi), Grid.GetLayerZ(Grid.SceneLayer.Move));
				});
			eatonfloorstate.DefaultState(eatonfloorstate.moveto).Enter("CreateLocator", delegate(StatesInstance smi)
			{
				smi.CreateLocator();
			}).Exit("DestroyLocator", delegate(StatesInstance smi)
			{
				smi.DestroyLocator();
			});
			eatonfloorstate.moveto.InitializeStates(eater, locator, eatonfloorstate.eat, eatonfloorstate.eat, null, null);
			eatonfloorstate.eat.ToggleAnims("anim_eat_floor_kanim", 0f).DoEat(ediblechunk, actualfoodunits, null, null);
		}
	}

	public static readonly Precondition EdibleIsNotNull = new Precondition
	{
		id = "EdibleIsNotNull",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.EDIBLE_IS_NOT_NULL,
		fn = (PreconditionFn)delegate(ref Precondition.Context context, object data)
		{
			return (UnityEngine.Object)null != (UnityEngine.Object)context.consumerState.consumer.GetSMI<RationMonitor.Instance>().GetEdible();
		}
	};

	public EatChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.Eat, master, master.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.PersonalTime)
	{
		base.smi = new StatesInstance(this);
		showAvailabilityInHoverText = false;
		AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		AddPrecondition(EdibleIsNotNull, null);
	}

	public override void Begin(Precondition.Context context)
	{
		if ((UnityEngine.Object)context.consumerState.consumer == (UnityEngine.Object)null)
		{
			Debug.LogError("EATCHORE null context.consumer");
		}
		else
		{
			RationMonitor.Instance sMI = context.consumerState.consumer.GetSMI<RationMonitor.Instance>();
			if (sMI == null)
			{
				Debug.LogError("EATCHORE null RationMonitor.Instance");
			}
			else
			{
				Edible edible = sMI.GetEdible();
				if ((UnityEngine.Object)edible.gameObject == (UnityEngine.Object)null)
				{
					Debug.LogError("EATCHORE null edible.gameObject");
				}
				else if (base.smi == null)
				{
					Debug.LogError("EATCHORE null smi");
				}
				else if (base.smi.sm == null)
				{
					Debug.LogError("EATCHORE null smi.sm");
				}
				else if (base.smi.sm.ediblesource == null)
				{
					Debug.LogError("EATCHORE null smi.sm.ediblesource");
				}
				else
				{
					base.smi.sm.ediblesource.Set(edible.gameObject, base.smi);
					KCrashReporter.Assert(edible.FoodInfo.CaloriesPerUnit > 0f, edible.GetProperName() + " has invalid calories per unit. Will result in NaNs");
					AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(gameObject);
					float num = (amountInstance.GetMax() - amountInstance.value) / edible.FoodInfo.CaloriesPerUnit;
					KCrashReporter.Assert(num > 0f, "EatChore is requesting an invalid amount of food");
					base.smi.sm.requestedfoodunits.Set(num, base.smi);
					base.smi.sm.eater.Set(context.consumerState.gameObject, base.smi);
					base.Begin(context);
				}
			}
		}
	}
}
