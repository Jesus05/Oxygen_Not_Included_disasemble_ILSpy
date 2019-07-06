using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameInstance where MasterType : IStateMachineTarget
{
	public class PreLoopPostState : State
	{
		public State pre;

		public State loop;

		public State pst;
	}

	public class WorkingState : State
	{
		public State waiting;

		public State working_pre;

		public State working_loop;

		public State working_pst;
	}

	public class GameInstance : GenericInstance
	{
		public GameInstance(MasterType master, DefType def)
			: base(master)
		{
			base.def = def;
		}

		public GameInstance(MasterType master)
			: base(master)
		{
		}

		public void Queue(string anim, KAnim.PlayMode mode = KAnim.PlayMode.Once)
		{
			StateMachineInstanceType smi = base.smi;
			((Instance)smi).GetComponent<KBatchedAnimController>().Queue(anim, mode, 1f, 0f);
		}

		public void Play(string anim, KAnim.PlayMode mode = KAnim.PlayMode.Once)
		{
			StateMachineInstanceType smi = base.smi;
			((Instance)smi).GetComponent<KBatchedAnimController>().Play(anim, mode, 1f, 0f);
		}
	}

	public class TagTransitionData : Transition
	{
		private Tag[] tags;

		private bool onRemove;

		private TargetParameter target;

		public TagTransitionData(State source_state, State target_state, int idx, Tag[] tags, bool on_remove, TargetParameter target)
			: base(tags.ToString(), (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)source_state, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)target_state, idx, (ConditionCallback)null)
		{
			this.tags = tags;
			onRemove = on_remove;
			this.target = target;
		}

		public override void Evaluate(StateMachineInstanceType smi)
		{
			if (!onRemove)
			{
				if (!HasAllTags(smi))
				{
					return;
				}
			}
			else if (HasAnyTags(smi))
			{
				return;
			}
			ExecuteTransition(smi);
		}

		private bool HasAllTags(StateMachineInstanceType smi)
		{
			KPrefabID component = target.Get(smi).GetComponent<KPrefabID>();
			for (int i = 0; i < tags.Length; i++)
			{
				if (!component.HasTag(tags[i]))
				{
					return false;
				}
			}
			return true;
		}

		private bool HasAnyTags(StateMachineInstanceType smi)
		{
			KPrefabID component = target.Get(smi).GetComponent<KPrefabID>();
			for (int i = 0; i < tags.Length; i++)
			{
				if (component.HasTag(tags[i]))
				{
					return true;
				}
			}
			return false;
		}

		private void ExecuteTransition(StateMachineInstanceType smi)
		{
			smi.GoTo(targetState);
		}

		private void OnCallback(StateMachineInstanceType smi)
		{
			if (!onRemove)
			{
				if (!HasAllTags(smi))
				{
					return;
				}
			}
			else if (HasAnyTags(smi))
			{
				return;
			}
			ExecuteTransition(smi);
		}

		public override Context Register(StateMachineInstanceType smi)
		{
			Context result = base.Register(smi);
			result.handlerId = target.Get(smi).Subscribe(-1582839653, delegate
			{
				OnCallback(smi);
			});
			return result;
		}

		public override void Unregister(StateMachineInstanceType smi, Context context)
		{
			base.Unregister(smi, context);
			target.Get(smi).Unsubscribe(context.handlerId);
		}
	}

	public class EventTransitionData : Transition
	{
		private GameHashes evtId;

		private TargetParameter target;

		private Func<StateMachineInstanceType, KMonoBehaviour> globalEventSystemCallback;

		public EventTransitionData(State source_state, State target_state, int idx, GameHashes evt, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback, ConditionCallback condition, TargetParameter target)
			: base(evt.ToString(), (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)source_state, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)target_state, idx, condition)
		{
			evtId = evt;
			this.target = target;
			globalEventSystemCallback = global_event_system_callback;
		}

		public override void Evaluate(StateMachineInstanceType smi)
		{
			if (condition != null && condition(smi))
			{
				ExecuteTransition(smi);
			}
		}

		private void ExecuteTransition(StateMachineInstanceType smi)
		{
			smi.GoTo(targetState);
		}

		private void OnCallback(StateMachineInstanceType smi)
		{
			if (condition == null || condition(smi))
			{
				ExecuteTransition(smi);
			}
		}

		public override Context Register(StateMachineInstanceType smi)
		{
			Context result = base.Register(smi);
			Action<object> handler = delegate
			{
				OnCallback(smi);
			};
			GameObject gameObject = null;
			if (globalEventSystemCallback != null)
			{
				gameObject = globalEventSystemCallback(smi).gameObject;
			}
			else
			{
				gameObject = target.Get(smi);
				if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
				{
					throw new InvalidOperationException("TargetParameter: " + target.name + " is null");
				}
			}
			result.handlerId = gameObject.Subscribe((int)evtId, handler);
			return result;
		}

		public override void Unregister(StateMachineInstanceType smi, Context context)
		{
			base.Unregister(smi, context);
			GameObject gameObject = null;
			if (globalEventSystemCallback != null)
			{
				KMonoBehaviour kMonoBehaviour = globalEventSystemCallback(smi);
				if ((UnityEngine.Object)kMonoBehaviour != (UnityEngine.Object)null)
				{
					gameObject = kMonoBehaviour.gameObject;
				}
			}
			else
			{
				gameObject = target.Get(smi);
			}
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				gameObject.Unsubscribe(context.handlerId);
			}
		}
	}

	public new class State : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
	{
		private class TransitionUpdater : UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater
		{
			private Transition.ConditionCallback condition;

			private State state;

			public TransitionUpdater(Transition.ConditionCallback condition, State state)
			{
				this.condition = condition;
				this.state = state;
			}

			public void Update(StateMachineInstanceType smi, float dt)
			{
				if (condition(smi))
				{
					smi.GoTo(state);
				}
			}
		}

		[DoNotAutoCreate]
		private TargetParameter stateTarget;

		public State root => this;

		public State master
		{
			get
			{
				stateTarget = sm.masterTarget;
				return this;
			}
		}

		private TargetParameter GetStateTarget()
		{
			if (stateTarget != null)
			{
				return stateTarget;
			}
			if (parent == null)
			{
				TargetParameter targetParameter = sm.stateTarget;
				if (targetParameter != null)
				{
					return targetParameter;
				}
				return sm.masterTarget;
			}
			State state = (State)parent;
			return state.GetStateTarget();
		}

		public int CreateDataTableEntry()
		{
			return sm.dataTableSize++;
		}

		public int CreateUpdateTableEntry()
		{
			return sm.updateTableSize++;
		}

		public State DoNothing()
		{
			return this;
		}

		private static List<Action> AddAction(string name, Callback callback, List<Action> actions, bool add_to_end)
		{
			if (actions == null)
			{
				actions = new List<Action>();
			}
			Action item = new Action(name, callback);
			if (add_to_end)
			{
				actions.Add(item);
			}
			else
			{
				actions.Insert(0, item);
			}
			return actions;
		}

		public State Target(TargetParameter target)
		{
			stateTarget = target;
			return this;
		}

		public State Update(Action<StateMachineInstanceType, float> callback, UpdateRate update_rate = UpdateRate.SIM_200ms, bool load_balance = false)
		{
			return Update(sm.name + "." + name, callback, update_rate, load_balance);
		}

		public State BatchUpdate(UpdateBucketWithUpdater<StateMachineInstanceType>.BatchUpdateDelegate batch_update, UpdateRate update_rate = UpdateRate.SIM_200ms)
		{
			return BatchUpdate(sm.name + "." + name, batch_update, update_rate);
		}

		public State Enter(Callback callback)
		{
			return Enter("Enter", callback);
		}

		public State Exit(Callback callback)
		{
			return Exit("Exit", callback);
		}

		private State InternalUpdate(string name, UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater bucket_updater, UpdateRate update_rate, bool load_balance, UpdateBucketWithUpdater<StateMachineInstanceType>.BatchUpdateDelegate batch_update = null)
		{
			int updateTableIdx = CreateUpdateTableEntry();
			if (updateActions == null)
			{
				updateActions = new List<UpdateAction>();
			}
			UpdateAction item = default(UpdateAction);
			item.updateTableIdx = updateTableIdx;
			item.updateRate = update_rate;
			item.updater = bucket_updater;
			int num = 1;
			if (load_balance)
			{
				num = Singleton<StateMachineUpdater>.Instance.GetFrameCount(update_rate);
			}
			item.buckets = new StateMachineUpdater.BaseUpdateBucket[num];
			for (int i = 0; i < num; i++)
			{
				UpdateBucketWithUpdater<StateMachineInstanceType> updateBucketWithUpdater = new UpdateBucketWithUpdater<StateMachineInstanceType>(name);
				updateBucketWithUpdater.batch_update_delegate = batch_update;
				Singleton<StateMachineUpdater>.Instance.AddBucket(update_rate, updateBucketWithUpdater);
				item.buckets[i] = updateBucketWithUpdater;
			}
			updateActions.Add(item);
			return this;
		}

		public State Update(string name, Action<StateMachineInstanceType, float> callback, UpdateRate update_rate = UpdateRate.SIM_200ms, bool load_balance = false)
		{
			return InternalUpdate(name, new BucketUpdater<StateMachineInstanceType>(callback), update_rate, load_balance, null);
		}

		public State BatchUpdate(string name, UpdateBucketWithUpdater<StateMachineInstanceType>.BatchUpdateDelegate batch_update, UpdateRate update_rate = UpdateRate.SIM_200ms)
		{
			return InternalUpdate(name, null, update_rate, false, batch_update);
		}

		public State FastUpdate(string name, UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater updater, UpdateRate update_rate = UpdateRate.SIM_200ms, bool load_balance = false)
		{
			return InternalUpdate(name, updater, update_rate, load_balance, null);
		}

		public State Enter(string name, Callback callback)
		{
			enterActions = AddAction(name, callback, enterActions, true);
			return this;
		}

		public State Exit(string name, Callback callback)
		{
			exitActions = AddAction(name, callback, exitActions, false);
			return this;
		}

		public State Toggle(string name, Callback enter_callback, Callback exit_callback)
		{
			int data_idx = CreateDataTableEntry();
			Enter("ToggleEnter(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				smi.dataTable[data_idx] = GameStateMachineHelper.HasToggleEnteredFlag;
				enter_callback(smi);
			});
			Exit("ToggleExit(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				if (smi.dataTable[data_idx] != null)
				{
					smi.dataTable[data_idx] = null;
					exit_callback(smi);
				}
			});
			return this;
		}

		private void Break(StateMachineInstanceType smi)
		{
		}

		public State BreakOnEnter()
		{
			return Enter(delegate(StateMachineInstanceType smi)
			{
				Break(smi);
			});
		}

		public State BreakOnExit()
		{
			return Exit(delegate(StateMachineInstanceType smi)
			{
				Break(smi);
			});
		}

		public State AddEffect(string effect_name)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddEffect(" + effect_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Add(effect_name, true);
			});
			return this;
		}

		public State ToggleAnims(Func<StateMachineInstanceType, HashedString> chooser_callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("EnableAnims()", delegate(StateMachineInstanceType smi)
			{
				HashedString hashedString = chooser_callback(smi);
				if (hashedString.IsValid)
				{
					KAnimFile anim2 = Assets.GetAnim(hashedString);
					if ((UnityEngine.Object)anim2 == (UnityEngine.Object)null)
					{
						Debug.LogWarning("Missing anims: " + hashedString);
					}
					else
					{
						state_target.Get<KAnimControllerBase>(smi).AddAnimOverrides(anim2, 0f);
					}
				}
			});
			Exit("Disableanims()", delegate(StateMachineInstanceType smi)
			{
				HashedString name = chooser_callback(smi);
				if (name.IsValid)
				{
					KAnimFile anim = Assets.GetAnim(name);
					if ((UnityEngine.Object)anim != (UnityEngine.Object)null)
					{
						state_target.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(anim);
					}
				}
			});
			return this;
		}

		public State ToggleAnims(string anim_file, float priority = 0f)
		{
			TargetParameter state_target = GetStateTarget();
			Toggle("ToggleAnims(" + anim_file + ")", delegate(StateMachineInstanceType smi)
			{
				KAnimFile anim2 = Assets.GetAnim(anim_file);
				if ((UnityEngine.Object)anim2 == (UnityEngine.Object)null)
				{
					Debug.LogError("Trying to add missing override anims:" + anim_file);
				}
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				kAnimControllerBase.AddAnimOverrides(anim2, priority);
			}, delegate(StateMachineInstanceType smi)
			{
				KAnimFile anim = Assets.GetAnim(anim_file);
				state_target.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(anim);
			});
			return this;
		}

		public State ToggleAttributeModifier(string modifier_name, Func<StateMachineInstanceType, AttributeModifier> callback, Func<StateMachineInstanceType, bool> condition = null)
		{
			TargetParameter state_target = GetStateTarget();
			int data_idx = CreateDataTableEntry();
			Enter("AddAttributeModifier( " + modifier_name + " )", delegate(StateMachineInstanceType smi)
			{
				if (condition == null || condition(smi))
				{
					AttributeModifier attributeModifier = callback(smi);
					DebugUtil.Assert(smi.dataTable[data_idx] == null);
					smi.dataTable[data_idx] = attributeModifier;
					state_target.Get(smi).GetAttributes().Add(attributeModifier);
				}
			});
			Exit("RemoveAttributeModifier( " + modifier_name + " )", delegate(StateMachineInstanceType smi)
			{
				if (smi.dataTable[data_idx] != null)
				{
					AttributeModifier modifier = (AttributeModifier)smi.dataTable[data_idx];
					smi.dataTable[data_idx] = null;
					GameObject gameObject = state_target.Get(smi);
					if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
					{
						gameObject.GetAttributes().Remove(modifier);
					}
				}
			});
			return this;
		}

		public State ToggleLoopingSound(string event_name, Func<StateMachineInstanceType, bool> condition = null, bool pause_on_game_pause = true, bool enable_culling = true, bool enable_camera_scaled_position = true)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("StartLoopingSound( " + event_name + " )", delegate(StateMachineInstanceType smi)
			{
				if (condition == null || condition(smi))
				{
					LoopingSounds component2 = state_target.Get(smi).GetComponent<LoopingSounds>();
					component2.StartSound(event_name, pause_on_game_pause, enable_culling, enable_camera_scaled_position);
				}
			});
			Exit("StopLoopingSound( " + event_name + " )", delegate(StateMachineInstanceType smi)
			{
				LoopingSounds component = state_target.Get(smi).GetComponent<LoopingSounds>();
				component.StopSound(event_name);
			});
			return this;
		}

		public State ToggleLoopingSound(string state_label, Func<StateMachineInstanceType, string> event_name_callback, Func<StateMachineInstanceType, bool> condition = null)
		{
			TargetParameter state_target = GetStateTarget();
			int data_idx = CreateDataTableEntry();
			Enter("StartLoopingSound( " + state_label + " )", delegate(StateMachineInstanceType smi)
			{
				if (condition == null || condition(smi))
				{
					string text = event_name_callback(smi);
					smi.dataTable[data_idx] = text;
					LoopingSounds component2 = state_target.Get(smi).GetComponent<LoopingSounds>();
					component2.StartSound(text);
				}
			});
			Exit("StopLoopingSound( " + state_label + " )", delegate(StateMachineInstanceType smi)
			{
				if (smi.dataTable[data_idx] != null)
				{
					LoopingSounds component = state_target.Get(smi).GetComponent<LoopingSounds>();
					component.StopSound((string)smi.dataTable[data_idx]);
					smi.dataTable[data_idx] = null;
				}
			});
			return this;
		}

		public State RefreshUserMenuOnEnter()
		{
			Enter("RefreshUserMenuOnEnter()", delegate(StateMachineInstanceType smi)
			{
				Game.Instance.userMenu.Refresh(smi.master.gameObject);
			});
			return this;
		}

		public State WorkableStartTransition(Func<StateMachineInstanceType, Workable> get_workable_callback, State target_state)
		{
			int data_idx = CreateDataTableEntry();
			Enter("Enter WorkableStartTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable3 = get_workable_callback(smi);
				if ((UnityEngine.Object)workable3 != (UnityEngine.Object)null)
				{
					Action<Workable.WorkableEvent> action = delegate(Workable.WorkableEvent evt)
					{
						if (evt == Workable.WorkableEvent.WorkStarted)
						{
							smi.GoTo(target_state);
						}
					};
					smi.dataTable[data_idx] = action;
					Workable workable4 = workable3;
					workable4.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(workable4.OnWorkableEventCB, action);
				}
			});
			Exit("Exit WorkableStartTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable = get_workable_callback(smi);
				if ((UnityEngine.Object)workable != (UnityEngine.Object)null)
				{
					Action<Workable.WorkableEvent> value = (Action<Workable.WorkableEvent>)smi.dataTable[data_idx];
					smi.dataTable[data_idx] = null;
					Workable workable2 = workable;
					workable2.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Remove(workable2.OnWorkableEventCB, value);
				}
			});
			return this;
		}

		public State WorkableStopTransition(Func<StateMachineInstanceType, Workable> get_workable_callback, State target_state)
		{
			int data_idx = CreateDataTableEntry();
			Enter("Enter WorkableStopTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable3 = get_workable_callback(smi);
				if ((UnityEngine.Object)workable3 != (UnityEngine.Object)null)
				{
					Action<Workable.WorkableEvent> action = delegate(Workable.WorkableEvent evt)
					{
						if (evt == Workable.WorkableEvent.WorkStopped)
						{
							smi.GoTo(target_state);
						}
					};
					smi.dataTable[data_idx] = action;
					Workable workable4 = workable3;
					workable4.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(workable4.OnWorkableEventCB, action);
				}
			});
			Exit("Exit WorkableStopTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable = get_workable_callback(smi);
				if ((UnityEngine.Object)workable != (UnityEngine.Object)null)
				{
					Action<Workable.WorkableEvent> value = (Action<Workable.WorkableEvent>)smi.dataTable[data_idx];
					smi.dataTable[data_idx] = null;
					Workable workable2 = workable;
					workable2.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Remove(workable2.OnWorkableEventCB, value);
				}
			});
			return this;
		}

		public State WorkableCompleteTransition(Func<StateMachineInstanceType, Workable> get_workable_callback, State target_state)
		{
			int data_idx = CreateDataTableEntry();
			Enter("Enter WorkableCompleteTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable3 = get_workable_callback(smi);
				if ((UnityEngine.Object)workable3 != (UnityEngine.Object)null)
				{
					Action<Workable.WorkableEvent> action = delegate(Workable.WorkableEvent evt)
					{
						if (evt == Workable.WorkableEvent.WorkCompleted)
						{
							smi.GoTo(target_state);
						}
					};
					smi.dataTable[data_idx] = action;
					Workable workable4 = workable3;
					workable4.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(workable4.OnWorkableEventCB, action);
				}
			});
			Exit("Exit WorkableCompleteTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable = get_workable_callback(smi);
				if ((UnityEngine.Object)workable != (UnityEngine.Object)null)
				{
					Action<Workable.WorkableEvent> value = (Action<Workable.WorkableEvent>)smi.dataTable[data_idx];
					smi.dataTable[data_idx] = null;
					Workable workable2 = workable;
					workable2.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Remove(workable2.OnWorkableEventCB, value);
				}
			});
			return this;
		}

		public State ToggleGravity()
		{
			TargetParameter state_target = GetStateTarget();
			int data_idx = CreateDataTableEntry();
			Enter("AddComponent<Gravity>()", delegate(StateMachineInstanceType smi)
			{
				GameObject gameObject = state_target.Get(smi);
				smi.dataTable[data_idx] = gameObject;
				GameComps.Gravities.Add(gameObject, Vector2.zero, null);
			});
			Exit("RemoveComponent<Gravity>()", delegate(StateMachineInstanceType smi)
			{
				GameObject go = (GameObject)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				GameComps.Gravities.Remove(go);
			});
			return this;
		}

		public State ToggleGravity(State landed_state)
		{
			TargetParameter state_target = GetStateTarget();
			EventTransition(GameHashes.Landed, landed_state, null);
			Toggle("GravityComponent", delegate(StateMachineInstanceType smi)
			{
				GameComps.Gravities.Add(state_target.Get(smi), Vector2.zero, null);
			}, delegate(StateMachineInstanceType smi)
			{
				GameComps.Gravities.Remove(state_target.Get(smi));
			});
			return this;
		}

		public State ToggleThought(Func<StateMachineInstanceType, Thought> chooser_callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("EnableThought()", delegate(StateMachineInstanceType smi)
			{
				Thought thought2 = chooser_callback(smi);
				StateMachineControllerExtensions.GetSMI<ThoughtGraph.Instance>(state_target.Get(smi)).AddThought(thought2);
			});
			Exit("DisableThought()", delegate(StateMachineInstanceType smi)
			{
				Thought thought = chooser_callback(smi);
				StateMachineControllerExtensions.GetSMI<ThoughtGraph.Instance>(state_target.Get(smi)).RemoveThought(thought);
			});
			return this;
		}

		public State ToggleThought(Thought thought, Func<StateMachineInstanceType, bool> condition_callback = null)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddThought(" + thought.Id + ")", delegate(StateMachineInstanceType smi)
			{
				if (condition_callback == null || condition_callback(smi))
				{
					StateMachineControllerExtensions.GetSMI<ThoughtGraph.Instance>(state_target.Get(smi)).AddThought(thought);
				}
			});
			if (condition_callback != null)
			{
				Update("ValidateThought(" + thought.Id + ")", delegate(StateMachineInstanceType smi, float dt)
				{
					if (condition_callback(smi))
					{
						StateMachineControllerExtensions.GetSMI<ThoughtGraph.Instance>(state_target.Get(smi)).AddThought(thought);
					}
					else
					{
						StateMachineControllerExtensions.GetSMI<ThoughtGraph.Instance>(state_target.Get(smi)).RemoveThought(thought);
					}
				}, UpdateRate.SIM_200ms, false);
			}
			Exit("RemoveThought(" + thought.Id + ")", delegate(StateMachineInstanceType smi)
			{
				StateMachineControllerExtensions.GetSMI<ThoughtGraph.Instance>(state_target.Get(smi)).RemoveThought(thought);
			});
			return this;
		}

		public State ToggleExpression(Func<StateMachineInstanceType, Expression> chooser_callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddExpression", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<FaceGraph>(smi).AddExpression(chooser_callback(smi));
			});
			Exit("RemoveExpression", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<FaceGraph>(smi).RemoveExpression(chooser_callback(smi));
			});
			return this;
		}

		public State ToggleExpression(Expression expression, Func<StateMachineInstanceType, bool> condition = null)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddExpression(" + expression.Id + ")", delegate(StateMachineInstanceType smi)
			{
				if (condition == null || condition(smi))
				{
					state_target.Get<FaceGraph>(smi).AddExpression(expression);
				}
			});
			if (condition != null)
			{
				Update("ValidateExpression(" + expression.Id + ")", delegate(StateMachineInstanceType smi, float dt)
				{
					if (condition(smi))
					{
						state_target.Get<FaceGraph>(smi).AddExpression(expression);
					}
					else
					{
						state_target.Get<FaceGraph>(smi).RemoveExpression(expression);
					}
				}, UpdateRate.SIM_200ms, false);
			}
			Exit("RemoveExpression(" + expression.Id + ")", delegate(StateMachineInstanceType smi)
			{
				FaceGraph faceGraph = state_target.Get<FaceGraph>(smi);
				if ((UnityEngine.Object)faceGraph != (UnityEngine.Object)null)
				{
					faceGraph.RemoveExpression(expression);
				}
			});
			return this;
		}

		public State ToggleMainStatusItem(StatusItem status_item)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddMainStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<KSelectable>(smi).SetStatusItem(Db.Get().StatusItemCategories.Main, status_item, smi);
			});
			Exit("RemoveMainStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				KSelectable kSelectable = state_target.Get<KSelectable>(smi);
				if ((UnityEngine.Object)kSelectable != (UnityEngine.Object)null)
				{
					kSelectable.SetStatusItem(Db.Get().StatusItemCategories.Main, null, null);
				}
			});
			return this;
		}

		public State ToggleCategoryStatusItem(StatusItemCategory category, StatusItem status_item, object data = null)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddCategoryStatusItem(" + category.Id + ", " + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<KSelectable>(smi).SetStatusItem(category, status_item, (data == null) ? smi : data);
			});
			Exit("RemoveCategoryStatusItem(" + category.Id + ", " + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				KSelectable kSelectable = state_target.Get<KSelectable>(smi);
				if ((UnityEngine.Object)kSelectable != (UnityEngine.Object)null)
				{
					kSelectable.SetStatusItem(category, null, null);
				}
			});
			return this;
		}

		public State ToggleStatusItem(StatusItem status_item, object data = null)
		{
			TargetParameter state_target = GetStateTarget();
			int data_idx = CreateDataTableEntry();
			Enter("AddStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				object obj = data;
				if (obj == null)
				{
					obj = smi;
				}
				Guid guid2 = state_target.Get<KSelectable>(smi).AddStatusItem(status_item, obj);
				smi.dataTable[data_idx] = guid2;
			});
			Exit("RemoveStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				KSelectable kSelectable = state_target.Get<KSelectable>(smi);
				if ((UnityEngine.Object)kSelectable != (UnityEngine.Object)null && smi.dataTable[data_idx] != null)
				{
					Guid guid = (Guid)smi.dataTable[data_idx];
					kSelectable.RemoveStatusItem(guid, false);
				}
				smi.dataTable[data_idx] = null;
			});
			return this;
		}

		public State ToggleSnapOn(string snap_on)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("SnapOn(" + snap_on + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<SnapOn>(smi).AttachSnapOnByName(snap_on);
			});
			Exit("SnapOff(" + snap_on + ")", delegate(StateMachineInstanceType smi)
			{
				SnapOn snapOn = state_target.Get<SnapOn>(smi);
				if ((UnityEngine.Object)snapOn != (UnityEngine.Object)null)
				{
					snapOn.DetachSnapOnByName(snap_on);
				}
			});
			return this;
		}

		public State ToggleTag(Tag tag)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddTag(" + tag.Name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<KPrefabID>(smi).AddTag(tag, false);
			});
			Exit("RemoveTag(" + tag.Name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<KPrefabID>(smi).RemoveTag(tag);
			});
			return this;
		}

		public State ToggleStatusItem(StatusItem status_item, Func<StateMachineInstanceType, object> callback)
		{
			return ToggleStatusItem(status_item, callback, null);
		}

		public State ToggleStatusItem(StatusItem status_item, Func<StateMachineInstanceType, object> callback, StatusItemCategory category)
		{
			TargetParameter state_target = GetStateTarget();
			int data_idx = CreateDataTableEntry();
			Enter("AddStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				if (category == null)
				{
					object data = (callback == null) ? null : callback(smi);
					Guid guid2 = state_target.Get<KSelectable>(smi).AddStatusItem(status_item, data);
					smi.dataTable[data_idx] = guid2;
				}
				else
				{
					object data2 = (callback == null) ? null : callback(smi);
					Guid guid3 = state_target.Get<KSelectable>(smi).SetStatusItem(category, status_item, data2);
					smi.dataTable[data_idx] = guid3;
				}
			});
			Exit("RemoveStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				KSelectable kSelectable = state_target.Get<KSelectable>(smi);
				if ((UnityEngine.Object)kSelectable != (UnityEngine.Object)null && smi.dataTable[data_idx] != null)
				{
					if (category == null)
					{
						Guid guid = (Guid)smi.dataTable[data_idx];
						kSelectable.RemoveStatusItem(guid, false);
					}
					else
					{
						kSelectable.SetStatusItem(category, null, null);
					}
				}
				smi.dataTable[data_idx] = null;
			});
			return this;
		}

		public State ToggleStatusItem(Func<StateMachineInstanceType, StatusItem> status_item_cb, Func<StateMachineInstanceType, object> data_callback = null)
		{
			TargetParameter state_target = GetStateTarget();
			int data_idx = CreateDataTableEntry();
			Enter("AddStatusItem(DynamicallyConstructed)", delegate(StateMachineInstanceType smi)
			{
				StatusItem statusItem = status_item_cb(smi);
				if (statusItem != null)
				{
					object data = (data_callback == null) ? null : data_callback(smi);
					Guid guid2 = state_target.Get<KSelectable>(smi).AddStatusItem(statusItem, data);
					smi.dataTable[data_idx] = guid2;
				}
			});
			Exit("RemoveStatusItem(DynamicallyConstructed)", delegate(StateMachineInstanceType smi)
			{
				KSelectable kSelectable = state_target.Get<KSelectable>(smi);
				if ((UnityEngine.Object)kSelectable != (UnityEngine.Object)null && smi.dataTable[data_idx] != null)
				{
					Guid guid = (Guid)smi.dataTable[data_idx];
					kSelectable.RemoveStatusItem(guid, false);
				}
				smi.dataTable[data_idx] = null;
			});
			return this;
		}

		public State ToggleFX(Func<StateMachineInstanceType, Instance> callback)
		{
			int data_idx = CreateDataTableEntry();
			Enter("EnableFX()", delegate(StateMachineInstanceType smi)
			{
				Instance instance2 = callback(smi);
				if (instance2 != null)
				{
					instance2.StartSM();
					smi.dataTable[data_idx] = instance2;
				}
			});
			Exit("DisableFX()", delegate(StateMachineInstanceType smi)
			{
				object obj = smi.dataTable[data_idx];
				Instance instance = (Instance)obj;
				smi.dataTable[data_idx] = null;
				instance?.StopSM("ToggleFX.Exit");
			});
			return this;
		}

		public State BehaviourComplete(Func<StateMachineInstanceType, Tag> tag_cb, bool on_exit = false)
		{
			if (on_exit)
			{
				Exit("BehaviourComplete()", delegate(StateMachineInstanceType smi)
				{
					smi.Trigger(-739654666, tag_cb(smi));
					smi.GoTo((BaseState)null);
				});
			}
			else
			{
				Enter("BehaviourComplete()", delegate(StateMachineInstanceType smi)
				{
					smi.Trigger(-739654666, tag_cb(smi));
					smi.GoTo((BaseState)null);
				});
			}
			return this;
		}

		public State BehaviourComplete(Tag tag, bool on_exit = false)
		{
			if (on_exit)
			{
				Exit("BehaviourComplete(" + tag.ToString() + ")", delegate(StateMachineInstanceType smi)
				{
					smi.Trigger(-739654666, tag);
					smi.GoTo((BaseState)null);
				});
			}
			else
			{
				Enter("BehaviourComplete(" + tag.ToString() + ")", delegate(StateMachineInstanceType smi)
				{
					smi.Trigger(-739654666, tag);
					smi.GoTo((BaseState)null);
				});
			}
			return this;
		}

		public State ToggleBehaviour(Tag behaviour_tag, Transition.ConditionCallback precondition, Action<StateMachineInstanceType> on_complete = null)
		{
			Func<object, bool> precondition_cb = (object obj) => precondition(obj as StateMachineInstanceType);
			Enter("AddPrecondition", delegate(StateMachineInstanceType smi)
			{
				if ((UnityEngine.Object)((Instance)smi).GetComponent<ChoreConsumer>() != (UnityEngine.Object)null)
				{
					((Instance)smi).GetComponent<ChoreConsumer>().AddBehaviourPrecondition(behaviour_tag, precondition_cb, smi);
				}
			});
			Exit("RemovePrecondition", delegate(StateMachineInstanceType smi)
			{
				if ((UnityEngine.Object)((Instance)smi).GetComponent<ChoreConsumer>() != (UnityEngine.Object)null)
				{
					((Instance)smi).GetComponent<ChoreConsumer>().RemoveBehaviourPrecondition(behaviour_tag, precondition_cb, smi);
				}
			});
			ToggleTag(behaviour_tag);
			if (on_complete != null)
			{
				EventHandler(GameHashes.BehaviourTagComplete, delegate(StateMachineInstanceType smi, object data)
				{
					if ((Tag)data == behaviour_tag)
					{
						on_complete(smi);
					}
				});
			}
			return this;
		}

		private void ClearChore(StateMachineInstanceType smi, int chore_data_idx, int callback_data_idx)
		{
			Chore chore = (Chore)smi.dataTable[chore_data_idx];
			if (chore != null)
			{
				Action<Chore> value = (Action<Chore>)smi.dataTable[callback_data_idx];
				smi.dataTable[chore_data_idx] = null;
				smi.dataTable[callback_data_idx] = null;
				Chore chore2 = chore;
				chore2.onExit = (Action<Chore>)Delegate.Remove(chore2.onExit, value);
				chore.Cancel("ClearGlobalChore");
			}
		}

		private Chore SetupChore(Func<StateMachineInstanceType, Chore> create_chore_callback, State success_state, State failure_state, StateMachineInstanceType smi, int chore_data_idx, int callback_data_idx, bool is_success_state_reentrant, bool is_failure_state_reentrant)
		{
			Chore chore = create_chore_callback(smi);
			chore.runUntilComplete = false;
			Action<Chore> action = delegate
			{
				bool isComplete = chore.isComplete;
				if ((isComplete && is_success_state_reentrant) || (is_failure_state_reentrant && !isComplete))
				{
					SetupChore(create_chore_callback, success_state, failure_state, smi, chore_data_idx, callback_data_idx, is_success_state_reentrant, is_failure_state_reentrant);
				}
				else
				{
					State state = success_state;
					if (!isComplete)
					{
						state = failure_state;
					}
					ClearChore(smi, chore_data_idx, callback_data_idx);
					smi.GoTo(state);
				}
			};
			Chore chore2 = chore;
			chore2.onExit = (Action<Chore>)Delegate.Combine(chore2.onExit, action);
			smi.dataTable[chore_data_idx] = chore;
			smi.dataTable[callback_data_idx] = action;
			return chore;
		}

		public State ToggleRecurringChore(Func<StateMachineInstanceType, Chore> callback, Func<StateMachineInstanceType, bool> condition = null)
		{
			int data_idx = CreateDataTableEntry();
			int callback_data_idx = CreateDataTableEntry();
			Enter("ToggleRecurringChoreEnter()", delegate(StateMachineInstanceType smi)
			{
				if (condition == null || condition(smi))
				{
					SetupChore(callback, this, this, smi, data_idx, callback_data_idx, true, true);
				}
			});
			Exit("ToggleRecurringChoreEnterExit()", delegate(StateMachineInstanceType smi)
			{
				ClearChore(smi, data_idx, callback_data_idx);
			});
			return this;
		}

		public State ToggleChore(Func<StateMachineInstanceType, Chore> callback, State target_state)
		{
			int data_idx = CreateDataTableEntry();
			int callback_data_idx = CreateDataTableEntry();
			Enter("ToggleChoreEnter()", delegate(StateMachineInstanceType smi)
			{
				SetupChore(callback, target_state, target_state, smi, data_idx, callback_data_idx, false, false);
			});
			Exit("ToggleChoreExit()", delegate(StateMachineInstanceType smi)
			{
				ClearChore(smi, data_idx, callback_data_idx);
			});
			return this;
		}

		public State ToggleChore(Func<StateMachineInstanceType, Chore> callback, State success_state, State failure_state)
		{
			int data_idx = CreateDataTableEntry();
			int callback_data_idx = CreateDataTableEntry();
			bool is_success_state_reentrant = success_state == this;
			bool is_failure_state_reentrant = failure_state == this;
			Enter("ToggleChoreEnter()", delegate(StateMachineInstanceType smi)
			{
				SetupChore(callback, success_state, failure_state, smi, data_idx, callback_data_idx, is_success_state_reentrant, is_failure_state_reentrant);
			});
			Exit("ToggleChoreExit()", delegate(StateMachineInstanceType smi)
			{
				ClearChore(smi, data_idx, callback_data_idx);
			});
			return this;
		}

		public State ToggleReactable(Func<StateMachineInstanceType, Reactable> callback)
		{
			int data_idx = CreateDataTableEntry();
			Enter(delegate(StateMachineInstanceType smi)
			{
				smi.dataTable[data_idx] = callback(smi);
			});
			Exit(delegate(StateMachineInstanceType smi)
			{
				Reactable reactable = (Reactable)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				reactable?.Cleanup();
			});
			return this;
		}

		public State RemoveEffect(string effect_name)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("RemoveEffect(" + effect_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Remove(effect_name);
			});
			return this;
		}

		public State ToggleEffect(string effect_name)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddEffect(" + effect_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Add(effect_name, false);
			});
			Exit("RemoveEffect(" + effect_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Remove(effect_name);
			});
			return this;
		}

		public State ToggleEffect(Func<StateMachineInstanceType, Effect> callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddEffect()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Add(callback(smi), false);
			});
			Exit("RemoveEffect()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Remove(callback(smi));
			});
			return this;
		}

		public State ToggleEffect(Func<StateMachineInstanceType, string> callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddEffect()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Add(callback(smi), false);
			});
			Exit("RemoveEffect()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Remove(callback(smi));
			});
			return this;
		}

		public State LogOnExit(Func<StateMachineInstanceType, string> callback)
		{
			Enter("Log()", delegate
			{
			});
			return this;
		}

		public State LogOnEnter(Func<StateMachineInstanceType, string> callback)
		{
			Exit("Log()", delegate
			{
			});
			return this;
		}

		public State ToggleUrge(Urge urge)
		{
			return ToggleUrge((StateMachineInstanceType smi) => urge);
		}

		public State ToggleUrge(Func<StateMachineInstanceType, Urge> urge_callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddUrge()", delegate(StateMachineInstanceType smi)
			{
				Urge urge2 = urge_callback(smi);
				state_target.Get<ChoreConsumer>(smi).AddUrge(urge2);
			});
			Exit("RemoveUrge()", delegate(StateMachineInstanceType smi)
			{
				Urge urge = urge_callback(smi);
				ChoreConsumer choreConsumer = state_target.Get<ChoreConsumer>(smi);
				if ((UnityEngine.Object)choreConsumer != (UnityEngine.Object)null)
				{
					choreConsumer.RemoveUrge(urge);
				}
			});
			return this;
		}

		public State OnTargetLost(TargetParameter parameter, State target_state)
		{
			ParamTransition(parameter, target_state, (StateMachineInstanceType smi, GameObject p) => (UnityEngine.Object)p == (UnityEngine.Object)null);
			return this;
		}

		public State ToggleBrain(string reason)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("StopBrain(" + reason + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Brain>(smi).Stop(reason);
			});
			Exit("ResetBrain(" + reason + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Brain>(smi).Reset(reason);
			});
			return this;
		}

		public State TriggerOnEnter(GameHashes evt, Func<StateMachineInstanceType, object> callback = null)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("Trigger(" + evt.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				GameObject go = state_target.Get(smi);
				object data = (callback == null) ? null : callback(smi);
				go.Trigger((int)evt, data);
			});
			return this;
		}

		public State TriggerOnExit(GameHashes evt)
		{
			TargetParameter state_target = GetStateTarget();
			Exit("Trigger(" + evt.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				GameObject gameObject = state_target.Get(smi);
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
				{
					gameObject.Trigger((int)evt, null);
				}
			});
			return this;
		}

		public State ToggleStateMachine(Func<StateMachineInstanceType, Instance> callback)
		{
			int data_idx = CreateDataTableEntry();
			Enter("EnableStateMachine()", delegate(StateMachineInstanceType smi)
			{
				Instance instance2 = callback(smi);
				smi.dataTable[data_idx] = instance2;
				instance2.StartSM();
			});
			Exit("DisableStateMachine()", delegate(StateMachineInstanceType smi)
			{
				Instance instance = (Instance)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				instance?.StopSM("ToggleStateMachine.Exit");
			});
			return this;
		}

		public State ToggleComponent<ComponentType>() where ComponentType : MonoBehaviour
		{
			TargetParameter state_target = this.GetStateTarget();
			this.Enter("EnableComponent(" + typeof(ComponentType).Name + ")", (Callback)delegate(StateMachineInstanceType smi)
			{
				ComponentType val2 = state_target.Get<ComponentType>(smi);
				val2.enabled = true;
			});
			this.Exit("DisableComponent(" + typeof(ComponentType).Name + ")", (Callback)delegate(StateMachineInstanceType smi)
			{
				ComponentType val = state_target.Get<ComponentType>(smi);
				val.enabled = false;
			});
			return this;
		}

		public State ToggleReserve(TargetParameter reserver, TargetParameter pickup_target, FloatParameter requested_amount, FloatParameter actual_amount)
		{
			int data_idx = CreateDataTableEntry();
			Enter("Reserve(" + pickup_target.name + ", " + requested_amount.name + ")", delegate(StateMachineInstanceType smi)
			{
				Pickupable pickupable2 = pickup_target.Get<Pickupable>(smi);
				GameObject gameObject = reserver.Get(smi);
				float num = requested_amount.Get(smi);
				float num2 = Mathf.Max(1f, Db.Get().Attributes.CarryAmount.Lookup(gameObject).GetTotalValue());
				float val = Math.Min(num, num2);
				val = Math.Min(val, pickupable2.UnreservedAmount);
				if (val <= 0f)
				{
					pickupable2.PrintReservations();
					Debug.LogError(num2 + ", " + num + ", " + pickupable2.UnreservedAmount + ", " + val);
				}
				actual_amount.Set(val, smi);
				int num3 = pickupable2.Reserve("ToggleReserve", gameObject, val);
				smi.dataTable[data_idx] = num3;
			});
			Exit("Unreserve(" + pickup_target.name + ", " + requested_amount.name + ")", delegate(StateMachineInstanceType smi)
			{
				int ticket = (int)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				Pickupable pickupable = pickup_target.Get<Pickupable>(smi);
				if ((UnityEngine.Object)pickupable != (UnityEngine.Object)null)
				{
					pickupable.Unreserve("ToggleReserve", ticket);
				}
			});
			return this;
		}

		public State ToggleWork(string work_type, Action<StateMachineInstanceType> callback, Func<StateMachineInstanceType, bool> validate_callback, State success_state, State failure_state)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("StartWork(" + work_type + ")", delegate(StateMachineInstanceType smi)
			{
				if (validate_callback(smi))
				{
					callback(smi);
				}
				else
				{
					smi.GoTo(failure_state);
				}
			});
			Update("Work()", delegate(StateMachineInstanceType smi, float dt)
			{
				if (validate_callback(smi))
				{
					Worker worker = state_target.Get<Worker>(smi);
					switch (worker.Work(dt))
					{
					case Worker.WorkResult.Success:
						smi.GoTo(success_state);
						break;
					case Worker.WorkResult.Failed:
						smi.GoTo(failure_state);
						break;
					}
				}
				else
				{
					smi.GoTo(failure_state);
				}
			}, UpdateRate.SIM_33ms, false);
			Exit("StopWork()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Worker>(smi).StopWork();
			});
			return this;
		}

		public State ToggleWork<WorkableType>(TargetParameter source_target, State success_state, State failure_state, Func<StateMachineInstanceType, bool> is_valid_cb) where WorkableType : Workable
		{
			TargetParameter state_target = this.GetStateTarget();
			this.ToggleWork(typeof(WorkableType).Name, (Action<StateMachineInstanceType>)delegate(StateMachineInstanceType smi)
			{
				Workable workable = source_target.Get<WorkableType>(smi);
				Worker worker = state_target.Get<Worker>(smi);
				worker.StartWork(new Worker.StartWorkInfo(workable));
			}, (Func<StateMachineInstanceType, bool>)((StateMachineInstanceType smi) => (UnityEngine.Object)source_target.Get<WorkableType>(smi) != (UnityEngine.Object)null && (is_valid_cb == null || is_valid_cb(smi))), success_state, failure_state);
			return this;
		}

		public State DoEat(TargetParameter source_target, FloatParameter amount, State success_state, State failure_state)
		{
			TargetParameter state_target = GetStateTarget();
			ToggleWork("Eat", delegate(StateMachineInstanceType smi)
			{
				Edible workable = source_target.Get<Edible>(smi);
				Worker worker = state_target.Get<Worker>(smi);
				float amount2 = amount.Get(smi);
				worker.StartWork(new Edible.EdibleStartWorkInfo(workable, amount2));
			}, (StateMachineInstanceType smi) => (UnityEngine.Object)source_target.Get<Edible>(smi) != (UnityEngine.Object)null, success_state, failure_state);
			return this;
		}

		public State DoSleep(TargetParameter sleeper, TargetParameter bed, State success_state, State failure_state)
		{
			TargetParameter state_target = GetStateTarget();
			ToggleWork("Sleep", delegate(StateMachineInstanceType smi)
			{
				Worker worker = state_target.Get<Worker>(smi);
				Sleepable workable = bed.Get<Sleepable>(smi);
				worker.StartWork(new Worker.StartWorkInfo(workable));
			}, (StateMachineInstanceType smi) => (UnityEngine.Object)bed.Get<Sleepable>(smi) != (UnityEngine.Object)null, success_state, failure_state);
			return this;
		}

		public State DoDelivery(TargetParameter worker_param, TargetParameter storage_param, State success_state, State failure_state)
		{
			ToggleWork("Pickup", delegate(StateMachineInstanceType smi)
			{
				Worker worker = worker_param.Get<Worker>(smi);
				Storage workable = storage_param.Get<Storage>(smi);
				worker.StartWork(new Worker.StartWorkInfo(workable));
			}, (StateMachineInstanceType smi) => (UnityEngine.Object)storage_param.Get<Storage>(smi) != (UnityEngine.Object)null, success_state, failure_state);
			return this;
		}

		public State DoPickup(TargetParameter source_target, TargetParameter result_target, FloatParameter amount, State success_state, State failure_state)
		{
			TargetParameter state_target = GetStateTarget();
			ToggleWork("Pickup", delegate(StateMachineInstanceType smi)
			{
				Pickupable pickupable = source_target.Get<Pickupable>(smi);
				Worker worker = state_target.Get<Worker>(smi);
				float amount2 = amount.Get(smi);
				worker.StartWork(new Pickupable.PickupableStartWorkInfo(pickupable, amount2, delegate(GameObject result)
				{
					result_target.Set(result, smi);
				}));
			}, (StateMachineInstanceType smi) => (UnityEngine.Object)source_target.Get<Pickupable>(smi) != (UnityEngine.Object)null || (UnityEngine.Object)result_target.Get<Pickupable>(smi) != (UnityEngine.Object)null, success_state, failure_state);
			return this;
		}

		public State ToggleNotification(Func<StateMachineInstanceType, Notification> callback)
		{
			int data_idx = CreateDataTableEntry();
			TargetParameter state_target = GetStateTarget();
			Enter("EnableNotification()", delegate(StateMachineInstanceType smi)
			{
				Notification notification2 = callback(smi);
				smi.dataTable[data_idx] = notification2;
				Notifier notifier2 = EntityTemplateExtensions.AddOrGet<Notifier>(smi.master.gameObject);
				notifier2.Add(notification2, "");
			});
			Exit("DisableNotification()", delegate(StateMachineInstanceType smi)
			{
				Notification notification = (Notification)smi.dataTable[data_idx];
				if (notification != null)
				{
					if (state_target != null)
					{
						Notifier notifier = state_target.Get<Notifier>(smi);
						if ((UnityEngine.Object)notifier != (UnityEngine.Object)null)
						{
							notifier.Remove(notification);
						}
					}
					smi.dataTable[data_idx] = null;
				}
			});
			return this;
		}

		public State DoReport(ReportManager.ReportType reportType, Func<StateMachineInstanceType, float> callback, Func<StateMachineInstanceType, string> context_callback = null)
		{
			Enter("DoReport()", delegate(StateMachineInstanceType smi)
			{
				float value = callback(smi);
				string note = (context_callback == null) ? null : context_callback(smi);
				ReportManager.Instance.ReportValue(reportType, value, note, null);
			});
			return this;
		}

		public State DoNotification(Func<StateMachineInstanceType, Notification> callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("DoNotification()", delegate(StateMachineInstanceType smi)
			{
				Notification notification = callback(smi);
				state_target.Get<Notifier>(smi).Add(notification, "");
			});
			return this;
		}

		public State DoTutorial(Tutorial.TutorialMessages msg)
		{
			Enter("DoTutorial()", delegate
			{
				Tutorial.Instance.TutorialMessage(msg);
			});
			return this;
		}

		public State ToggleScheduleCallback(string name, Func<StateMachineInstanceType, float> time_cb, Action<StateMachineInstanceType> callback)
		{
			int data_idx = CreateDataTableEntry();
			Enter("AddScheduledCallback(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				SchedulerHandle schedulerHandle2 = GameScheduler.Instance.Schedule(name, time_cb(smi), delegate(object smi_data)
				{
					callback((StateMachineInstanceType)smi_data);
				}, smi, null);
				DebugUtil.Assert(smi.dataTable[data_idx] == null);
				smi.dataTable[data_idx] = schedulerHandle2;
			});
			Exit("RemoveScheduledCallback(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				if (smi.dataTable[data_idx] != null)
				{
					SchedulerHandle schedulerHandle = (SchedulerHandle)smi.dataTable[data_idx];
					smi.dataTable[data_idx] = null;
					schedulerHandle.ClearScheduler();
				}
			});
			return this;
		}

		public State ScheduleGoTo(Func<StateMachineInstanceType, float> time_cb, BaseState state)
		{
			Enter("ScheduleGoTo(" + state.name + ")", delegate(StateMachineInstanceType smi)
			{
				smi.ScheduleGoTo(time_cb(smi), state);
			});
			return this;
		}

		public State ScheduleGoTo(float time, BaseState state)
		{
			Enter("ScheduleGoTo(" + time.ToString() + ", " + state.name + ")", delegate(StateMachineInstanceType smi)
			{
				smi.ScheduleGoTo(time, state);
			});
			return this;
		}

		public State EventHandler(GameHashes evt, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback, Callback callback)
		{
			return EventHandler(evt, global_event_system_callback, delegate(StateMachineInstanceType smi, object d)
			{
				callback(smi);
			});
		}

		public State EventHandler(GameHashes evt, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback, GameEvent.Callback callback)
		{
			if (events == null)
			{
				events = new List<StateEvent>();
			}
			TargetParameter target = GetStateTarget();
			GameEvent item = new GameEvent(evt, callback, target, global_event_system_callback);
			events.Add(item);
			return this;
		}

		public State EventHandler(GameHashes evt, Callback callback)
		{
			return EventHandler(evt, delegate(StateMachineInstanceType smi, object d)
			{
				callback(smi);
			});
		}

		public State EventHandler(GameHashes evt, GameEvent.Callback callback)
		{
			EventHandler(evt, null, callback);
			return this;
		}

		public State ParamTransition<ParameterType>(Parameter<ParameterType> parameter, State state, Parameter<ParameterType>.Callback callback)
		{
			if (parameterTransitions == null)
			{
				parameterTransitions = new List<StateMachine.ParameterTransition>();
			}
			parameterTransitions.Add((StateMachine.ParameterTransition)new Parameter<ParameterType>.Transition(parameter, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)state, callback));
			return this;
		}

		public State OnSignal(Signal signal, State state, Func<StateMachineInstanceType, bool> callback)
		{
			ParamTransition(signal, state, (StateMachineInstanceType smi, SignalParameter p) => callback(smi));
			return this;
		}

		public State OnSignal(Signal signal, State state)
		{
			ParamTransition(signal, state, null);
			return this;
		}

		public State EnterTransition(State state, Transition.ConditionCallback condition)
		{
			string str = "(Stop)";
			if (state != null)
			{
				str = state.name;
			}
			Enter("Transition(" + str + ")", delegate(StateMachineInstanceType smi)
			{
				if (condition(smi))
				{
					smi.GoTo(state);
				}
			});
			return this;
		}

		public State Transition(State state, Transition.ConditionCallback condition, UpdateRate update_rate = UpdateRate.SIM_200ms)
		{
			string str = "(Stop)";
			if (state != null)
			{
				str = state.name;
			}
			Enter("Transition(" + str + ")", delegate(StateMachineInstanceType smi)
			{
				if (condition(smi))
				{
					smi.GoTo(state);
				}
			});
			FastUpdate("Transition(" + str + ")", new TransitionUpdater(condition, state), update_rate, false);
			return this;
		}

		public State DefaultState(State default_state)
		{
			defaultState = default_state;
			return this;
		}

		public State GoTo(State state)
		{
			string str = "(null)";
			if (state != null)
			{
				str = state.name;
			}
			Update("GoTo(" + str + ")", delegate(StateMachineInstanceType smi, float dt)
			{
				smi.GoTo(state);
			}, UpdateRate.SIM_200ms, false);
			return this;
		}

		public State StopMoving()
		{
			TargetParameter target = GetStateTarget();
			Exit("StopMoving()", delegate(StateMachineInstanceType smi)
			{
				target.Get<Navigator>(smi).Stop(false);
			});
			return this;
		}

		public State OnBehaviourComplete(Tag behaviour, Action<StateMachineInstanceType> cb)
		{
			EventHandler(GameHashes.BehaviourTagComplete, delegate(StateMachineInstanceType smi, object d)
			{
				if ((Tag)d == behaviour)
				{
					cb(smi);
				}
			});
			return this;
		}

		public State MoveTo(Func<StateMachineInstanceType, int> cell_callback, State success_state = null, State fail_state = null, bool update_cell = false)
		{
			return MoveTo(cell_callback, null, success_state, fail_state, update_cell);
		}

		public State MoveTo(Func<StateMachineInstanceType, int> cell_callback, Func<StateMachineInstanceType, CellOffset[]> cell_offsets_callback, State success_state = null, State fail_state = null, bool update_cell = false)
		{
			EventTransition(GameHashes.DestinationReached, success_state, null);
			EventTransition(GameHashes.NavigationFailed, fail_state, null);
			CellOffset[] default_offset = new CellOffset[1]
			{
				default(CellOffset)
			};
			TargetParameter state_target = GetStateTarget();
			Enter("MoveTo()", delegate(StateMachineInstanceType smi)
			{
				int cell2 = cell_callback(smi);
				Navigator navigator2 = state_target.Get<Navigator>(smi);
				CellOffset[] offsets = default_offset;
				if (cell_offsets_callback != null)
				{
					offsets = cell_offsets_callback(smi);
				}
				navigator2.GoTo(cell2, offsets);
			});
			if (update_cell)
			{
				Update("MoveTo()", delegate(StateMachineInstanceType smi, float dt)
				{
					int cell = cell_callback(smi);
					Navigator navigator = state_target.Get<Navigator>(smi);
					navigator.UpdateTarget(cell);
				}, UpdateRate.SIM_200ms, false);
			}
			Exit("StopMoving()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get(smi).GetComponent<Navigator>().Stop(false);
			});
			return this;
		}

		public State MoveTo<ApproachableType>(TargetParameter move_parameter, State success_state, State fail_state = null, CellOffset[] override_offsets = null, NavTactic tactic = null) where ApproachableType : IApproachable
		{
			this.EventTransition(GameHashes.DestinationReached, success_state, (Transition.ConditionCallback)null);
			this.EventTransition(GameHashes.NavigationFailed, fail_state, (Transition.ConditionCallback)null);
			TargetParameter state_target = this.GetStateTarget();
			this.Enter("MoveTo(" + move_parameter.name + ")", (Callback)delegate(StateMachineInstanceType smi)
			{
				IApproachable approachable = (IApproachable)(object)move_parameter.Get<ApproachableType>(smi);
				KMonoBehaviour kMonoBehaviour = move_parameter.Get<KMonoBehaviour>(smi);
				if ((UnityEngine.Object)kMonoBehaviour == (UnityEngine.Object)null)
				{
					smi.GoTo(fail_state);
				}
				else
				{
					GameObject gameObject = ((Parameter<GameObject>)state_target).Get(smi);
					Navigator component = gameObject.GetComponent<Navigator>();
					if (override_offsets == null)
					{
						override_offsets = approachable.GetOffsets();
					}
					component.GoTo(kMonoBehaviour, override_offsets, tactic);
				}
			});
			this.Exit("StopMoving()", (Callback)delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Navigator>(smi).Stop(false);
			});
			return this;
		}

		public State Face(TargetParameter face_target, float x_offset = 0f)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("Face", delegate(StateMachineInstanceType smi)
			{
				Facing facing = state_target.Get<Facing>(smi);
				IApproachable approachable = face_target.Get<IApproachable>(smi);
				Vector3 position = approachable.transform.GetPosition();
				float target_x = position.x + x_offset;
				facing.Face(target_x);
			});
			return this;
		}

		public State TagTransition(Tag[] tags, State state, bool on_remove = false)
		{
			if (transitions == null)
			{
				transitions = new List<BaseTransition>();
			}
			TagTransitionData item = new TagTransitionData(this, state, transitions.Count, tags, on_remove, GetStateTarget());
			transitions.Add(item);
			return this;
		}

		public State TagTransition(Tag tag, State state, bool on_remove = false)
		{
			return TagTransition(new Tag[1]
			{
				tag
			}, state, on_remove);
		}

		public State EventTransition(GameHashes evt, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback, State state, Transition.ConditionCallback condition = null)
		{
			if (transitions == null)
			{
				transitions = new List<BaseTransition>();
			}
			TargetParameter target = GetStateTarget();
			EventTransitionData item = new EventTransitionData(this, state, transitions.Count, evt, global_event_system_callback, condition, target);
			transitions.Add(item);
			return this;
		}

		public State EventTransition(GameHashes evt, State state, Transition.ConditionCallback condition = null)
		{
			return EventTransition(evt, null, state, condition);
		}

		public State ReturnSuccess()
		{
			Enter("ReturnSuccess()", delegate(StateMachineInstanceType smi)
			{
				smi.SetStatus(Status.Success);
				smi.StopSM("GameStateMachine.ReturnSuccess()");
			});
			return this;
		}

		public State ToggleStatusItem(string name, string tooltip, string icon = "", StatusItem.IconType icon_type = StatusItem.IconType.Info, NotificationType notification_type = NotificationType.Neutral, bool allow_multiples = false, HashedString render_overlay = default(HashedString), int status_overlays = 129022, Func<string, StateMachineInstanceType, string> resolve_string_callback = null, Func<string, StateMachineInstanceType, string> resolve_tooltip_callback = null, StatusItemCategory category = null)
		{
			StatusItem statusItem = new StatusItem(longName, name, tooltip, icon, icon_type, notification_type, allow_multiples, render_overlay, status_overlays);
			if (resolve_string_callback != null)
			{
				statusItem.resolveStringCallback = ((string str, object obj) => resolve_string_callback(str, (StateMachineInstanceType)obj));
			}
			if (resolve_tooltip_callback != null)
			{
				statusItem.resolveTooltipCallback = ((string str, object obj) => resolve_tooltip_callback(str, (StateMachineInstanceType)obj));
			}
			ToggleStatusItem(statusItem, (StateMachineInstanceType smi) => smi, category);
			return this;
		}

		public State PlayAnim(string anim)
		{
			TargetParameter state_target = GetStateTarget();
			KAnim.PlayMode mode = KAnim.PlayMode.Once;
			Enter("PlayAnim(" + anim + ", " + mode.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if ((UnityEngine.Object)kAnimControllerBase != (UnityEngine.Object)null)
				{
					kAnimControllerBase.Play(anim, mode, 1f, 0f);
				}
			});
			return this;
		}

		public State PlayAnim(Func<StateMachineInstanceType, string> anim_cb, KAnim.PlayMode mode)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("PlayAnim(" + mode.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if ((UnityEngine.Object)kAnimControllerBase != (UnityEngine.Object)null)
				{
					kAnimControllerBase.Play(anim_cb(smi), mode, 1f, 0f);
				}
			});
			return this;
		}

		public State PlayAnim(string anim, KAnim.PlayMode mode)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("PlayAnim(" + anim + ", " + mode.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if ((UnityEngine.Object)kAnimControllerBase != (UnityEngine.Object)null)
				{
					kAnimControllerBase.Play(anim, mode, 1f, 0f);
				}
			});
			return this;
		}

		public State PlayAnim(string anim, KAnim.PlayMode mode, Func<StateMachineInstanceType, string> suffix_callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("PlayAnim(" + anim + ", " + mode.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				string str = "";
				if (suffix_callback != null)
				{
					str = suffix_callback(smi);
				}
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if ((UnityEngine.Object)kAnimControllerBase != (UnityEngine.Object)null)
				{
					kAnimControllerBase.Play(anim + str, mode, 1f, 0f);
				}
			});
			return this;
		}

		public State QueueAnim(string anim, bool loop = false, Func<StateMachineInstanceType, string> suffix_callback = null)
		{
			TargetParameter state_target = GetStateTarget();
			KAnim.PlayMode mode = KAnim.PlayMode.Once;
			if (loop)
			{
				mode = KAnim.PlayMode.Loop;
			}
			Enter("QueueAnim(" + anim + ", " + mode.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				string str = "";
				if (suffix_callback != null)
				{
					str = suffix_callback(smi);
				}
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if ((UnityEngine.Object)kAnimControllerBase != (UnityEngine.Object)null)
				{
					kAnimControllerBase.Queue(anim + str, mode, 1f, 0f);
				}
			});
			return this;
		}

		public State PlayAnims(Func<StateMachineInstanceType, HashedString[]> anims_callback, KAnim.PlayMode mode = KAnim.PlayMode.Once)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("PlayAnims", delegate(StateMachineInstanceType smi)
			{
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if ((UnityEngine.Object)kAnimControllerBase != (UnityEngine.Object)null)
				{
					HashedString[] anim_names = anims_callback(smi);
					kAnimControllerBase.Play(anim_names, mode);
				}
			});
			return this;
		}

		public State PlayAnims(Func<StateMachineInstanceType, HashedString[]> anims_callback, Func<StateMachineInstanceType, KAnim.PlayMode> mode_cb)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("PlayAnims", delegate(StateMachineInstanceType smi)
			{
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if ((UnityEngine.Object)kAnimControllerBase != (UnityEngine.Object)null)
				{
					HashedString[] anim_names = anims_callback(smi);
					KAnim.PlayMode mode = mode_cb(smi);
					kAnimControllerBase.Play(anim_names, mode);
				}
			});
			return this;
		}

		public State OnAnimQueueComplete(State state)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("CheckIfAnimQueueIsEmpty", delegate(StateMachineInstanceType smi)
			{
				if (state_target.Get<KBatchedAnimController>(smi).IsStopped())
				{
					smi.GoTo(state);
				}
			});
			return EventTransition(GameHashes.AnimQueueComplete, state, null);
		}
	}

	public class GameEvent : StateEvent
	{
		public delegate void Callback(StateMachineInstanceType smi, object callback_data);

		private GameHashes id;

		private TargetParameter target;

		private Callback callback;

		private Func<StateMachineInstanceType, KMonoBehaviour> globalEventSystemCallback;

		public GameEvent(GameHashes id, Callback callback, TargetParameter target, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback)
			: base(id.ToString())
		{
			this.id = id;
			this.target = target;
			this.callback = callback;
			globalEventSystemCallback = global_event_system_callback;
		}

		public override Context Subscribe(Instance smi)
		{
			Context result = base.Subscribe(smi);
			StateMachineInstanceType cast_smi = (StateMachineInstanceType)smi;
			Action<object> handler = delegate(object d)
			{
				if (!Instance.error)
				{
					callback(cast_smi, d);
				}
			};
			if (globalEventSystemCallback != null)
			{
				KMonoBehaviour kMonoBehaviour = globalEventSystemCallback(cast_smi);
				result.data = kMonoBehaviour.Subscribe((int)id, handler);
			}
			else
			{
				result.data = target.Get(cast_smi).Subscribe((int)id, handler);
			}
			return result;
		}

		public override void Unsubscribe(Instance smi, Context context)
		{
			StateMachineInstanceType val = (StateMachineInstanceType)smi;
			if (globalEventSystemCallback != null)
			{
				KMonoBehaviour kMonoBehaviour = globalEventSystemCallback(val);
				if ((UnityEngine.Object)kMonoBehaviour != (UnityEngine.Object)null)
				{
					kMonoBehaviour.Unsubscribe(context.data);
				}
			}
			else
			{
				GameObject gameObject = target.Get(val);
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
				{
					gameObject.Unsubscribe(context.data);
				}
			}
		}
	}

	public class ApproachSubState<ApproachableType> : State where ApproachableType : IApproachable
	{
		public State InitializeStates(TargetParameter mover, TargetParameter move_target, State success_state, State failure_state = null, CellOffset[] override_offsets = null, NavTactic tactic = null)
		{
			base.root.Target(mover).OnTargetLost(move_target, failure_state).MoveTo<ApproachableType>(move_target, success_state, failure_state, override_offsets, (tactic != null) ? tactic : NavigationTactics.ReduceTravelDistance);
			return this;
		}
	}

	public class DebugGoToSubState : State
	{
		public State InitializeStates(State exit_state)
		{
			base.root.Enter("GoToCursor", delegate(StateMachineInstanceType smi)
			{
				GoToCursor(smi);
			}).EventHandler(GameHashes.DebugGoTo, (StateMachineInstanceType smi) => Game.Instance, delegate(StateMachineInstanceType smi)
			{
				GoToCursor(smi);
			}).EventTransition(GameHashes.DestinationReached, exit_state, null)
				.EventTransition(GameHashes.NavigationFailed, exit_state, null);
			return this;
		}

		public void GoToCursor(StateMachineInstanceType smi)
		{
			((Instance)smi).GetComponent<Navigator>().GoTo(Grid.PosToCell(DebugHandler.GetMousePos()), new CellOffset[1]
			{
				default(CellOffset)
			});
		}
	}

	public class DropSubState : State
	{
		public State InitializeStates(TargetParameter carrier, TargetParameter item, TargetParameter drop_target, State success_state, State failure_state = null)
		{
			base.root.Target(carrier).Enter("Drop", delegate(StateMachineInstanceType smi)
			{
				Storage storage = carrier.Get<Storage>(smi);
				GameObject gameObject = item.Get(smi);
				storage.Drop(gameObject, true);
				Transform transform = drop_target.Get<Transform>(smi);
				int cell = Grid.PosToCell(transform.GetPosition());
				int cell2 = Grid.CellAbove(cell);
				gameObject.transform.SetPosition(Grid.CellToPosCCC(cell2, Grid.SceneLayer.Move));
				smi.GoTo(success_state);
			});
			return this;
		}
	}

	public class FetchSubState : State
	{
		public ApproachSubState<Pickupable> approach;

		public State pickup;

		public State success;

		public State InitializeStates(TargetParameter fetcher, TargetParameter pickup_source, TargetParameter pickup_chunk, FloatParameter requested_amount, FloatParameter actual_amount, State success_state, State failure_state = null)
		{
			Target(fetcher);
			base.root.DefaultState(approach).ToggleReserve(fetcher, pickup_source, requested_amount, actual_amount);
			ApproachSubState<Pickupable> approachSubState = approach;
			State success_state2 = pickup;
			NavTactic reduceTravelDistance = NavigationTactics.ReduceTravelDistance;
			approachSubState.InitializeStates(fetcher, pickup_source, success_state2, null, null, reduceTravelDistance).OnTargetLost(pickup_source, failure_state);
			pickup.DoPickup(pickup_source, pickup_chunk, actual_amount, success_state, failure_state);
			return this;
		}
	}

	public class HungrySubState : State
	{
		public State satisfied;

		public State hungry;

		public State InitializeStates(TargetParameter target, StatusItem status_item)
		{
			Target(target);
			base.root.DefaultState(satisfied);
			satisfied.EventTransition(GameHashes.AddUrge, hungry, (StateMachineInstanceType smi) => IsHungry(smi));
			hungry.EventTransition(GameHashes.RemoveUrge, satisfied, (StateMachineInstanceType smi) => !IsHungry(smi)).ToggleStatusItem(status_item, (object)null);
			return this;
		}

		private static bool IsHungry(StateMachineInstanceType smi)
		{
			return ((Instance)smi).GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.Eat);
		}
	}

	public class PlantAliveSubState : State
	{
		public State InitializeStates(TargetParameter plant, State death_state = null)
		{
			base.root.Target(plant).EventTransition(GameHashes.Uprooted, death_state, (StateMachineInstanceType smi) => UprootedMonitor.IsObjectUprooted(plant.Get(smi))).EventTransition(GameHashes.TooColdFatal, death_state, (StateMachineInstanceType smi) => isLethalTemperature(plant.Get(smi)))
				.EventTransition(GameHashes.TooHotFatal, death_state, (StateMachineInstanceType smi) => isLethalTemperature(plant.Get(smi)))
				.EventTransition(GameHashes.Drowned, death_state, null);
			return this;
		}

		public bool ForceUpdateStatus(GameObject plant)
		{
			TemperatureVulnerable component = plant.GetComponent<TemperatureVulnerable>();
			EntombVulnerable component2 = plant.GetComponent<EntombVulnerable>();
			UprootedMonitor component3 = plant.GetComponent<UprootedMonitor>();
			PressureVulnerable component4 = plant.GetComponent<PressureVulnerable>();
			return ((UnityEngine.Object)component == (UnityEngine.Object)null || !component.IsLethal) && ((UnityEngine.Object)component2 == (UnityEngine.Object)null || !component2.GetEntombed) && ((UnityEngine.Object)component3 == (UnityEngine.Object)null || !component3.IsUprooted) && ((UnityEngine.Object)component4 == (UnityEngine.Object)null || !component4.IsLethal);
		}

		private static bool isLethalTemperature(GameObject plant)
		{
			TemperatureVulnerable component = plant.GetComponent<TemperatureVulnerable>();
			if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
			{
				if (component.GetInternalTemperatureState != 0)
				{
					if (component.GetInternalTemperatureState != TemperatureVulnerable.TemperatureState.LethalHot)
					{
						return false;
					}
					return true;
				}
				return true;
			}
			return false;
		}
	}

	public State root = new State();

	protected static Parameter<bool>.Callback IsFalse = (StateMachineInstanceType smi, bool p) => !p;

	protected static Parameter<bool>.Callback IsTrue = (StateMachineInstanceType smi, bool p) => p;

	protected static Parameter<float>.Callback IsZero = (StateMachineInstanceType smi, float p) => p == 0f;

	protected static Parameter<float>.Callback IsLTZero = (StateMachineInstanceType smi, float p) => p < 0f;

	protected static Parameter<float>.Callback IsLTEZero = (StateMachineInstanceType smi, float p) => p <= 0f;

	protected static Parameter<float>.Callback IsGTZero = (StateMachineInstanceType smi, float p) => p > 0f;

	protected static Parameter<float>.Callback IsGTEZero = (StateMachineInstanceType smi, float p) => p >= 0f;

	protected static Parameter<float>.Callback IsOne = (StateMachineInstanceType smi, float p) => p == 1f;

	protected static Parameter<float>.Callback IsLTOne = (StateMachineInstanceType smi, float p) => p < 1f;

	protected static Parameter<float>.Callback IsLTEOne = (StateMachineInstanceType smi, float p) => p <= 1f;

	protected static Parameter<float>.Callback IsGTOne = (StateMachineInstanceType smi, float p) => p > 1f;

	protected static Parameter<float>.Callback IsGTEOne = (StateMachineInstanceType smi, float p) => p >= 1f;

	public override void InitializeStates(out BaseState default_state)
	{
		base.InitializeStates(out default_state);
	}

	public static Transition.ConditionCallback Not(Transition.ConditionCallback transition_cb)
	{
		return (StateMachineInstanceType smi) => !transition_cb(smi);
	}

	public override void BindStates()
	{
		BindState(null, root, "root");
		BindStates(root, this);
	}
}
public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType> : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.GameInstance where MasterType : IStateMachineTarget
{
}
public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType> : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget, object> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget, object> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget, object>.GameInstance
{
}
