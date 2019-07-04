using KSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;

public abstract class StateMachine
{
	public sealed class DoNotAutoCreate : Attribute
	{
	}

	public enum Status
	{
		Initialized,
		Running,
		Failed,
		Success
	}

	public class BaseDef
	{
		public Instance CreateSMI(IStateMachineTarget master)
		{
			return Singleton<StateMachineManager>.Instance.CreateSMIFromDef(master, this);
		}

		public Type GetStateMachineType()
		{
			return GetType().DeclaringType;
		}

		public virtual void Configure(GameObject prefab)
		{
		}
	}

	public class Category : Resource
	{
		public Category(string id)
			: base(id, null, null)
		{
		}
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public abstract class Instance
	{
		public struct UpdateTableEntry
		{
			public HandleVector<int>.Handle handle;

			public StateMachineUpdater.BaseUpdateBucket bucket;
		}

		protected LoggerFSSSS log;

		protected Status status = Status.Initialized;

		protected StateMachine stateMachine;

		protected Stack<StateEvent.Context> subscribedEvents = new Stack<StateEvent.Context>();

		protected int stackSize;

		protected Parameter.Context[] parameterContexts;

		public object[] dataTable;

		public UpdateTableEntry[] updateTable;

		private Action<object> scheduleGoToCallback;

		public Action<string, Status> OnStop;

		public bool breakOnGoTo;

		public bool enableConsoleLogging;

		public bool isCrashed;

		public static bool error;

		public abstract float timeinstate
		{
			get;
		}

		public GameObject gameObject => GetMaster().gameObject;

		public Transform transform => gameObject.transform;

		public Instance(StateMachine state_machine, IStateMachineTarget master)
		{
			stateMachine = state_machine;
			CreateParameterContexts();
			log = new LoggerFSSSS(stateMachine.name, 35);
		}

		public abstract BaseState GetCurrentState();

		public abstract void GoTo(BaseState state);

		public abstract IStateMachineTarget GetMaster();

		public abstract void StopSM(string reason);

		public abstract SchedulerHandle Schedule(float time, Action<object> callback, object callback_data = null);

		public virtual void FreeResources()
		{
			stateMachine = null;
			if (subscribedEvents != null)
			{
				subscribedEvents.Clear();
			}
			subscribedEvents = null;
			parameterContexts = null;
			dataTable = null;
			updateTable = null;
		}

		public bool IsRunning()
		{
			return GetCurrentState() != null;
		}

		public void GoTo(string state_name)
		{
			BaseState state = stateMachine.GetState(state_name);
			GoTo(state);
		}

		public int GetStackSize()
		{
			return stackSize;
		}

		public StateMachine GetStateMachine()
		{
			return stateMachine;
		}

		[Conditional("UNITY_EDITOR")]
		public void Log(string a, string b = "", string c = "", string d = "")
		{
		}

		public bool IsConsoleLoggingEnabled()
		{
			return enableConsoleLogging || stateMachine.debugSettings.enableConsoleLogging;
		}

		public bool IsBreakOnGoToEnabled()
		{
			return breakOnGoTo || stateMachine.debugSettings.breakOnGoTo;
		}

		public LoggerFSSSS GetLog()
		{
			return log;
		}

		public Parameter.Context[] GetParameterContexts()
		{
			return parameterContexts;
		}

		public Parameter.Context GetParameterContext(Parameter parameter)
		{
			return parameterContexts[parameter.idx];
		}

		public Status GetStatus()
		{
			return status;
		}

		public void SetStatus(Status status)
		{
			this.status = status;
		}

		public void Error()
		{
			if (!error)
			{
				isCrashed = true;
				error = true;
				RestartWarning.ShouldWarn = true;
			}
		}

		public override string ToString()
		{
			string str = "";
			if (GetCurrentState() != null)
			{
				str = GetCurrentState().name;
			}
			else if (GetStatus() != 0)
			{
				str = GetStatus().ToString();
			}
			return stateMachine.ToString() + "(" + str + ")";
		}

		public virtual void StartSM()
		{
			if (!IsRunning())
			{
				BaseState defaultState = stateMachine.GetDefaultState();
				DebugUtil.Assert(defaultState != null);
				if (!GetComponent<StateMachineController>().Restore(this))
				{
					GoTo(defaultState);
				}
			}
		}

		public bool HasTag(Tag tag)
		{
			return GetComponent<KPrefabID>().HasTag(tag);
		}

		public bool IsInsideState(BaseState state)
		{
			BaseState currentState = GetCurrentState();
			if (currentState != null)
			{
				for (int i = 0; i < currentState.branch.Length; i++)
				{
					if (state == currentState.branch[i])
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public void ScheduleGoTo(float time, BaseState state)
		{
			if (scheduleGoToCallback == null)
			{
				scheduleGoToCallback = delegate(object d)
				{
					GoTo((BaseState)d);
				};
			}
			Schedule(time, scheduleGoToCallback, state);
		}

		public void Subscribe(int hash, Action<object> handler)
		{
			GetMaster().Subscribe(hash, handler);
		}

		public void Unsubscribe(int hash, Action<object> handler)
		{
			GetMaster().Unsubscribe(hash, handler);
		}

		public void Trigger(int hash, object data = null)
		{
			GetMaster().GetComponent<KPrefabID>().Trigger(hash, data);
		}

		public ComponentType Get<ComponentType>()
		{
			return GetComponent<ComponentType>();
		}

		public ComponentType GetComponent<ComponentType>()
		{
			return GetMaster().GetComponent<ComponentType>();
		}

		private void CreateParameterContexts()
		{
			parameterContexts = new Parameter.Context[stateMachine.parameters.Length];
			for (int i = 0; i < stateMachine.parameters.Length; i++)
			{
				parameterContexts[i] = stateMachine.parameters[i].CreateContext();
			}
		}
	}

	[DebuggerDisplay("{longName}")]
	public class BaseState
	{
		public string name;

		public string longName;

		public string debugPushName;

		public string debugPopName;

		public string debugExecuteName;

		public BaseState defaultState;

		public List<StateEvent> events;

		public List<BaseTransition> transitions;

		public List<ParameterTransition> parameterTransitions;

		public List<UpdateAction> updateActions;

		public List<Action> enterActions;

		public List<Action> exitActions;

		public BaseState[] branch;

		public BaseState parent;

		public BaseState()
		{
			branch = new BaseState[1];
			branch[0] = this;
		}

		public void FreeResources()
		{
			if (name != null)
			{
				name = null;
				if (defaultState != null)
				{
					defaultState.FreeResources();
				}
				defaultState = null;
				events = null;
				if (transitions != null)
				{
					for (int i = 0; i < transitions.Count; i++)
					{
						transitions[i].Clear();
					}
				}
				transitions = null;
				parameterTransitions = null;
				enterActions = null;
				exitActions = null;
				if (branch != null)
				{
					for (int j = 0; j < branch.Length; j++)
					{
						branch[j].FreeResources();
					}
				}
				branch = null;
				parent = null;
			}
		}

		public int GetStateCount()
		{
			return branch.Length;
		}

		public BaseState GetState(int idx)
		{
			return branch[idx];
		}
	}

	public class BaseTransition
	{
		public string name;

		public BaseState sourceState;

		public BaseState targetState;

		public BaseTransition(string name, BaseState source_state, BaseState target_state)
		{
			this.name = name;
			sourceState = source_state;
			targetState = target_state;
		}

		public void Clear()
		{
			name = null;
			if (sourceState != null)
			{
				sourceState.FreeResources();
			}
			sourceState = null;
			if (targetState != null)
			{
				targetState.FreeResources();
			}
			targetState = null;
		}
	}

	public struct UpdateAction
	{
		public int updateTableIdx;

		public UpdateRate updateRate;

		public int nextBucketIdx;

		public StateMachineUpdater.BaseUpdateBucket[] buckets;

		public object updater;
	}

	public struct Action
	{
		public string name;

		public object callback;

		public Action(string name, object callback)
		{
			this.name = name;
			this.callback = callback;
		}
	}

	public class ParameterTransition
	{
	}

	public abstract class Parameter
	{
		public abstract class Context
		{
			public Parameter parameter;

			public Context(Parameter parameter)
			{
				this.parameter = parameter;
			}

			public abstract void Serialize(BinaryWriter writer);

			public abstract void Deserialize(IReader reader);

			public virtual void Cleanup()
			{
			}

			public abstract void ShowEditor(Instance base_smi);
		}

		public string name;

		public int idx;

		public abstract Context CreateContext();
	}

	protected string name;

	protected int maxDepth;

	protected BaseState defaultState;

	protected Parameter[] parameters = new Parameter[0];

	public int dataTableSize;

	public int updateTableSize;

	public StateMachineDebuggerSettings.Entry debugSettings;

	public bool saveHistory;

	public int version
	{
		get;
		protected set;
	}

	public bool serializable
	{
		get;
		protected set;
	}

	public StateMachine()
	{
		name = GetType().FullName;
	}

	public virtual void FreeResources()
	{
		name = null;
		if (defaultState != null)
		{
			defaultState.FreeResources();
		}
		defaultState = null;
		parameters = null;
	}

	public abstract string[] GetStateNames();

	public abstract BaseState GetState(string name);

	public abstract void BindStates();

	public abstract Type GetStateMachineInstanceType();

	public virtual void InitializeStates(out BaseState default_state)
	{
		default_state = null;
	}

	public void InitializeStateMachine()
	{
		debugSettings = StateMachineDebuggerSettings.Get().CreateEntry(GetType());
		BaseState default_state = null;
		InitializeStates(out default_state);
		DebugUtil.Assert(default_state != null);
		defaultState = default_state;
	}

	public void CreateStates(object state_machine)
	{
		Type type = state_machine.GetType();
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			bool flag = false;
			object[] customAttributes = fieldInfo.GetCustomAttributes(false);
			foreach (object obj in customAttributes)
			{
				if (obj.GetType() == typeof(DoNotAutoCreate))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (fieldInfo.FieldType.IsSubclassOf(typeof(BaseState)))
				{
					BaseState baseState = (BaseState)Activator.CreateInstance(fieldInfo.FieldType);
					CreateStates(baseState);
					fieldInfo.SetValue(state_machine, baseState);
				}
				else if (fieldInfo.FieldType.IsSubclassOf(typeof(Parameter)))
				{
					Parameter parameter = (Parameter)fieldInfo.GetValue(state_machine);
					if (parameter == null)
					{
						parameter = (Parameter)Activator.CreateInstance(fieldInfo.FieldType);
						fieldInfo.SetValue(state_machine, parameter);
					}
					parameter.name = fieldInfo.Name;
					parameter.idx = parameters.Length;
					parameters = parameters.Append(parameter);
				}
				else if (fieldInfo.FieldType.IsSubclassOf(typeof(StateMachine)))
				{
					fieldInfo.SetValue(state_machine, this);
				}
			}
		}
	}

	public BaseState GetDefaultState()
	{
		return defaultState;
	}

	public int GetMaxDepth()
	{
		return maxDepth;
	}

	public override string ToString()
	{
		return name;
	}
}
public class StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> : StateMachine where StateMachineInstanceType : StateMachine.Instance where MasterType : IStateMachineTarget
{
	public class GenericInstance : Instance
	{
		public struct StackEntry
		{
			public BaseState state;

			public SchedulerGroup schedulerGroup;
		}

		private float stateEnterTime;

		private int gotoId;

		private int currentActionIdx = -1;

		private SchedulerHandle updateHandle;

		private Stack<BaseState> gotoStack = new Stack<BaseState>();

		protected Stack<Transition.Context> transitionStack = new Stack<Transition.Context>();

		protected StateMachineController controller;

		private SchedulerGroup currentSchedulerGroup;

		private StackEntry[] stateStack;

		public StateMachineType sm
		{
			get;
			private set;
		}

		protected StateMachineInstanceType smi => (StateMachineInstanceType)(Instance)this;

		public MasterType master
		{
			get;
			private set;
		}

		public DefType def
		{
			get;
			set;
		}

		public bool isMasterNull => internalSm.masterTarget.IsNull((StateMachineInstanceType)(Instance)this);

		private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> internalSm => (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>)(object)sm;

		public override float timeinstate => Time.time - stateEnterTime;

		public GenericInstance(MasterType master)
			: base((StateMachine)(object)Singleton<StateMachineManager>.Instance.CreateStateMachine<StateMachineType>(), (IStateMachineTarget)(object)master)
		{
			this.master = master;
			stateStack = new StackEntry[stateMachine.GetMaxDepth()];
			for (int i = 0; i < stateStack.Length; i++)
			{
				stateStack[i].schedulerGroup = Singleton<StateMachineManager>.Instance.CreateSchedulerGroup();
			}
			sm = (StateMachineType)(object)stateMachine;
			dataTable = new object[GetStateMachine().dataTableSize];
			updateTable = new UpdateTableEntry[GetStateMachine().updateTableSize];
			controller = ((IStateMachineTarget)master).GetComponent<StateMachineController>();
			if ((UnityEngine.Object)controller == (UnityEngine.Object)null)
			{
				controller = master.gameObject.AddComponent<StateMachineController>();
			}
			internalSm.masterTarget.Set(master.gameObject, smi);
			controller.AddStateMachineInstance(this);
		}

		protected virtual void OnCleanUp()
		{
		}

		public override void FreeResources()
		{
			updateHandle.FreeResources();
			updateHandle = default(SchedulerHandle);
			controller = null;
			if (gotoStack != null)
			{
				gotoStack.Clear();
			}
			gotoStack = null;
			if (transitionStack != null)
			{
				transitionStack.Clear();
			}
			transitionStack = null;
			if (currentSchedulerGroup != null)
			{
				currentSchedulerGroup.FreeResources();
			}
			currentSchedulerGroup = null;
			if (stateStack != null)
			{
				for (int i = 0; i < stateStack.Length; i++)
				{
					if (stateStack[i].schedulerGroup != null)
					{
						stateStack[i].schedulerGroup.FreeResources();
					}
				}
			}
			stateStack = null;
			base.FreeResources();
		}

		public override IStateMachineTarget GetMaster()
		{
			return (IStateMachineTarget)(object)master;
		}

		private void PushEvent(StateEvent evt)
		{
			StateEvent.Context t = evt.Subscribe(this);
			subscribedEvents.Push(t);
		}

		private void PopEvent()
		{
			StateEvent.Context context = subscribedEvents.Pop();
			context.stateEvent.Unsubscribe(this, context);
		}

		private void PushTransition(Transition transition)
		{
			Transition.Context t = transition.Register(smi);
			transitionStack.Push(t);
		}

		private void PopTransition(State state)
		{
			Transition.Context context = transitionStack.Pop();
			((Transition)state.transitions[context.idx]).Unregister(smi, context);
		}

		private void PushState(BaseState state)
		{
			int num = gotoId;
			currentActionIdx = -1;
			if (state.events != null)
			{
				foreach (StateEvent @event in state.events)
				{
					PushEvent(@event);
				}
			}
			if (state.transitions != null)
			{
				foreach (Transition transition3 in state.transitions)
				{
					PushTransition(transition3);
				}
			}
			if (state.parameterTransitions != null)
			{
				foreach (ParameterTransition parameterTransition3 in state.parameterTransitions)
				{
					parameterTransition3.Register(smi);
				}
			}
			if (state.updateActions != null)
			{
				for (int i = 0; i < state.updateActions.Count; i++)
				{
					UpdateAction value = state.updateActions[i];
					int updateTableIdx = value.updateTableIdx;
					int nextBucketIdx = value.nextBucketIdx;
					value.nextBucketIdx = (value.nextBucketIdx + 1) % value.buckets.Length;
					UpdateBucketWithUpdater<StateMachineInstanceType> updateBucketWithUpdater = (UpdateBucketWithUpdater<StateMachineInstanceType>)value.buckets[nextBucketIdx];
					smi.updateTable[updateTableIdx].bucket = updateBucketWithUpdater;
					smi.updateTable[updateTableIdx].handle = updateBucketWithUpdater.Add(smi, Singleton<StateMachineUpdater>.Instance.GetFrameTime(value.updateRate, updateBucketWithUpdater.frame), (UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater)value.updater);
					state.updateActions[i] = value;
				}
			}
			stateEnterTime = Time.time;
			stateStack[stackSize++].state = state;
			currentSchedulerGroup = stateStack[stackSize - 1].schedulerGroup;
			if (state.transitions != null)
			{
				foreach (Transition transition4 in state.transitions)
				{
					if (num != gotoId)
					{
						return;
					}
					transition4.Evaluate(smi);
				}
			}
			if (state.parameterTransitions != null)
			{
				foreach (ParameterTransition parameterTransition4 in state.parameterTransitions)
				{
					if (num != gotoId)
					{
						return;
					}
					parameterTransition4.Evaluate(smi);
				}
			}
			if (num == gotoId)
			{
				ExecuteActions((State)state, state.enterActions);
				if (num == gotoId)
				{
					return;
				}
			}
		}

		private void ExecuteActions(State state, List<Action> actions)
		{
			if (actions != null)
			{
				int num = gotoId;
				currentActionIdx++;
				while (currentActionIdx < actions.Count && num == gotoId)
				{
					Action action = actions[currentActionIdx];
					State.Callback callback = (State.Callback)action.callback;
					try
					{
						callback(smi);
					}
					catch (Exception ex)
					{
						if (!Instance.error)
						{
							Error();
							string text = "(NULL).";
							IStateMachineTarget master = GetMaster();
							if (!master.isNull)
							{
								KPrefabID component = master.GetComponent<KPrefabID>();
								text = ((!((UnityEngine.Object)component != (UnityEngine.Object)null)) ? ("(" + base.gameObject.name + ").") : ("(" + component.PrefabTag.ToString() + ")."));
							}
							string[] obj = new string[7]
							{
								"Exception in: ",
								text,
								stateMachine.ToString(),
								".",
								state.name,
								".",
								null
							};
							Action action2 = actions[currentActionIdx];
							obj[6] = action2.name;
							string str = string.Concat(obj);
							DebugUtil.LogErrorArgs(controller, str + "\n" + ex.ToString());
						}
					}
					currentActionIdx++;
				}
				currentActionIdx = 2147483646;
			}
		}

		private void PopState()
		{
			currentActionIdx = -1;
			StackEntry stackEntry = stateStack[--stackSize];
			BaseState state = stackEntry.state;
			if (state.parameterTransitions != null)
			{
				foreach (ParameterTransition parameterTransition in state.parameterTransitions)
				{
					parameterTransition.Unregister(smi);
				}
			}
			if (state.transitions != null)
			{
				for (int i = 0; i < state.transitions.Count; i++)
				{
					PopTransition((State)state);
				}
			}
			if (state.events != null)
			{
				for (int j = 0; j < state.events.Count; j++)
				{
					PopEvent();
				}
			}
			if (state.updateActions != null)
			{
				foreach (UpdateAction updateAction in state.updateActions)
				{
					UpdateAction current = updateAction;
					int updateTableIdx = current.updateTableIdx;
					UpdateBucketWithUpdater<StateMachineInstanceType> updateBucketWithUpdater = (UpdateBucketWithUpdater<StateMachineInstanceType>)smi.updateTable[updateTableIdx].bucket;
					smi.updateTable[updateTableIdx].bucket = null;
					updateBucketWithUpdater.Remove(smi.updateTable[updateTableIdx].handle);
				}
			}
			stackEntry.schedulerGroup.Reset();
			currentSchedulerGroup = stackEntry.schedulerGroup;
			ExecuteActions((State)state, state.exitActions);
		}

		public override SchedulerHandle Schedule(float time, Action<object> callback, object callback_data = null)
		{
			return Singleton<StateMachineManager>.Instance.Schedule(GetCurrentState().longName, time, callback, callback_data, currentSchedulerGroup);
		}

		public override void StartSM()
		{
			if ((UnityEngine.Object)controller != (UnityEngine.Object)null && !controller.HasStateMachineInstance(this))
			{
				controller.AddStateMachineInstance(this);
			}
			base.StartSM();
		}

		public override void StopSM(string reason)
		{
			if (!Instance.error)
			{
				if ((UnityEngine.Object)controller != (UnityEngine.Object)null)
				{
					controller.RemoveStateMachineInstance(this);
				}
				if (IsRunning())
				{
					gotoId++;
					while (stackSize > 0)
					{
						PopState();
					}
					if (master != null && (UnityEngine.Object)controller != (UnityEngine.Object)null)
					{
						controller.RemoveStateMachineInstance(this);
					}
					if (status == Status.Running)
					{
						SetStatus(Status.Failed);
					}
					if (OnStop != null)
					{
						OnStop(reason, status);
					}
					for (int i = 0; i < parameterContexts.Length; i++)
					{
						parameterContexts[i].Cleanup();
					}
					OnCleanUp();
				}
			}
		}

		private void FinishStateInProgress(BaseState state)
		{
			if (state.enterActions != null)
			{
				ExecuteActions((State)state, state.enterActions);
			}
		}

		public override void GoTo(BaseState base_state)
		{
			if (!App.IsExiting && !Instance.error && !isMasterNull)
			{
				try
				{
					if (IsBreakOnGoToEnabled())
					{
						Debugger.Break();
					}
					if (base_state != null)
					{
						while (base_state.defaultState != null)
						{
							base_state = base_state.defaultState;
						}
					}
					if (GetCurrentState() == null)
					{
						SetStatus(Status.Running);
					}
					if (gotoStack.Count > 100)
					{
						string text = "Potential infinite transition loop detected in state machine: " + ToString() + "\nGoto stack:\n";
						foreach (BaseState item in gotoStack)
						{
							text = text + "\n" + item.name;
						}
						Debug.LogError(text);
						Error();
					}
					else
					{
						gotoStack.Push(base_state);
						if (base_state == null)
						{
							StopSM("StateMachine.GoTo(null)");
							gotoStack.Pop();
						}
						else
						{
							int num = ++gotoId;
							State state = base_state as State;
							BaseState[] branch = state.branch;
							int i;
							for (i = 0; i < stackSize && i < branch.Length && stateStack[i].state == branch[i]; i++)
							{
							}
							int num2 = stackSize - 1;
							if (num2 >= 0 && num2 == i - 1)
							{
								FinishStateInProgress(stateStack[num2].state);
							}
							while (stackSize > i && num == gotoId)
							{
								PopState();
							}
							for (int j = i; j < branch.Length; j++)
							{
								if (num != gotoId)
								{
									break;
								}
								PushState(branch[j]);
							}
							gotoStack.Pop();
						}
					}
				}
				catch (Exception ex)
				{
					if (!Instance.error)
					{
						Error();
						string text2 = "(Stop)";
						if (base_state != null)
						{
							text2 = base_state.name;
						}
						string text3 = "(NULL).";
						IStateMachineTarget master = GetMaster();
						if (!master.isNull)
						{
							text3 = "(" + base.gameObject.name + ").";
						}
						string str = "Exception in: " + text3 + stateMachine.ToString() + ".GoTo(" + text2 + ")";
						DebugUtil.LogErrorArgs(controller, str + "\n" + ex.ToString());
					}
				}
			}
		}

		public override BaseState GetCurrentState()
		{
			if (stackSize <= 0)
			{
				return null;
			}
			return stateStack[stackSize - 1].state;
		}
	}

	public class State : BaseState
	{
		public delegate void Callback(StateMachineInstanceType smi);

		protected StateMachineType sm;
	}

	public new abstract class ParameterTransition : StateMachine.ParameterTransition
	{
		public abstract void Evaluate(StateMachineInstanceType smi);

		public abstract void Register(StateMachineInstanceType smi);

		public abstract void Unregister(StateMachineInstanceType smi);
	}

	public class Transition : BaseTransition
	{
		public delegate bool ConditionCallback(StateMachineInstanceType smi);

		public struct Context
		{
			public int idx;

			public int handlerId;

			public Context(Transition transition)
			{
				idx = transition.idx;
				handlerId = 0;
			}
		}

		public ConditionCallback condition;

		public int idx;

		public Transition(string name, State source_state, State target_state, int idx, ConditionCallback condition)
			: base(name, source_state, target_state)
		{
			this.condition = condition;
			this.idx = idx;
		}

		public virtual void Evaluate(StateMachineInstanceType smi)
		{
		}

		public virtual Context Register(StateMachineInstanceType smi)
		{
			return new Context(this);
		}

		public virtual void Unregister(StateMachineInstanceType smi, Context context)
		{
		}
	}

	public new abstract class Parameter<ParameterType> : Parameter
	{
		public delegate bool Callback(StateMachineInstanceType smi, ParameterType p);

		public class Transition : ParameterTransition
		{
			private Parameter<ParameterType> parameter;

			private Callback callback;

			private State state;

			public Transition(Parameter<ParameterType> parameter, State state, Callback callback)
			{
				this.parameter = parameter;
				this.callback = callback;
				this.state = state;
			}

			public override void Evaluate(StateMachineInstanceType smi)
			{
				if (!parameter.isSignal || callback != null)
				{
					Context context = (Context)smi.GetParameterContext(parameter);
					if (callback(smi, context.value))
					{
						smi.GoTo(state);
					}
				}
			}

			private void Trigger(StateMachineInstanceType smi)
			{
				smi.GoTo(state);
			}

			public override void Register(StateMachineInstanceType smi)
			{
				Context context = (Context)smi.GetParameterContext(parameter);
				if (parameter.isSignal && callback == null)
				{
					Context context2 = context;
					context2.onDirty = (Action<StateMachineInstanceType>)Delegate.Combine(context2.onDirty, new Action<StateMachineInstanceType>(Trigger));
				}
				else
				{
					Context context3 = context;
					context3.onDirty = (Action<StateMachineInstanceType>)Delegate.Combine(context3.onDirty, new Action<StateMachineInstanceType>(((ParameterTransition)this).Evaluate));
				}
			}

			public override void Unregister(StateMachineInstanceType smi)
			{
				Context context = (Context)smi.GetParameterContext(parameter);
				if (parameter.isSignal && callback == null)
				{
					Context context2 = context;
					context2.onDirty = (Action<StateMachineInstanceType>)Delegate.Remove(context2.onDirty, new Action<StateMachineInstanceType>(Trigger));
				}
				else
				{
					Context context3 = context;
					context3.onDirty = (Action<StateMachineInstanceType>)Delegate.Remove(context3.onDirty, new Action<StateMachineInstanceType>(((ParameterTransition)this).Evaluate));
				}
			}

			public override string ToString()
			{
				if (state == null)
				{
					return parameter.name + "->(Stop)";
				}
				return parameter.name + "->" + state.name;
			}
		}

		public new abstract class Context : Parameter.Context
		{
			public ParameterType value;

			public Action<StateMachineInstanceType> onDirty;

			public Context(Parameter parameter, ParameterType default_value)
				: base(parameter)
			{
				value = default_value;
			}

			public virtual void Set(ParameterType value, StateMachineInstanceType smi)
			{
				if (!EqualityComparer<ParameterType>.Default.Equals(value, this.value))
				{
					this.value = value;
					if (onDirty != null)
					{
						onDirty(smi);
					}
				}
			}
		}

		public ParameterType defaultValue;

		public bool isSignal;

		public Parameter()
		{
		}

		public Parameter(ParameterType default_value)
		{
			defaultValue = default_value;
		}

		public ParameterType Set(ParameterType value, StateMachineInstanceType smi)
		{
			Context context = (Context)smi.GetParameterContext(this);
			context.Set(value, smi);
			return value;
		}

		public ParameterType Get(StateMachineInstanceType smi)
		{
			Context context = (Context)smi.GetParameterContext(this);
			return context.value;
		}
	}

	public class BoolParameter : Parameter<bool>
	{
		public new class Context : Parameter<bool>.Context
		{
			public Context(Parameter parameter, bool default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
				writer.Write((byte)(value ? 1 : 0));
			}

			public override void Deserialize(IReader reader)
			{
				value = (reader.ReadByte() != 0);
			}

			public override void ShowEditor(Instance base_smi)
			{
			}
		}

		public BoolParameter()
		{
		}

		public BoolParameter(bool default_value)
			: base(default_value)
		{
		}

		public override Parameter.Context CreateContext()
		{
			return new Context(this, defaultValue);
		}
	}

	public class Vector3Parameter : Parameter<Vector3>
	{
		public new class Context : Parameter<Vector3>.Context
		{
			public Context(Parameter parameter, Vector3 default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
				writer.Write(value.x);
				writer.Write(value.y);
				writer.Write(value.z);
			}

			public override void Deserialize(IReader reader)
			{
				value.x = reader.ReadSingle();
				value.y = reader.ReadSingle();
				value.z = reader.ReadSingle();
			}

			public override void ShowEditor(Instance base_smi)
			{
			}
		}

		public Vector3Parameter()
		{
		}

		public Vector3Parameter(Vector3 default_value)
			: base(default_value)
		{
		}

		public override Parameter.Context CreateContext()
		{
			return new Context(this, defaultValue);
		}
	}

	public class EnumParameter<EnumType> : Parameter<EnumType>
	{
		public new class Context : Parameter<EnumType>.Context
		{
			public Context(Parameter parameter, EnumType default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
				writer.Write((int)(object)value);
			}

			public override void Deserialize(IReader reader)
			{
				value = (EnumType)(object)reader.ReadInt32();
			}

			public override void ShowEditor(Instance base_smi)
			{
			}
		}

		public EnumParameter(EnumType default_value)
			: base(default_value)
		{
		}

		public override Parameter.Context CreateContext()
		{
			return new Context(this, defaultValue);
		}
	}

	public class FloatParameter : Parameter<float>
	{
		public new class Context : Parameter<float>.Context
		{
			public Context(Parameter parameter, float default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
				writer.Write(value);
			}

			public override void Deserialize(IReader reader)
			{
				value = reader.ReadSingle();
			}

			public override void ShowEditor(Instance base_smi)
			{
			}
		}

		public FloatParameter()
		{
		}

		public FloatParameter(float default_value)
			: base(default_value)
		{
		}

		public float Delta(float delta_value, StateMachineInstanceType smi)
		{
			float num = Get(smi);
			num += delta_value;
			Set(num, smi);
			return num;
		}

		public float DeltaClamp(float delta_value, float min_value, float max_value, StateMachineInstanceType smi)
		{
			float num = Get(smi);
			num += delta_value;
			num = Mathf.Clamp(num, min_value, max_value);
			Set(num, smi);
			return num;
		}

		public override Parameter.Context CreateContext()
		{
			return new Context(this, defaultValue);
		}
	}

	public class IntParameter : Parameter<int>
	{
		public new class Context : Parameter<int>.Context
		{
			public Context(Parameter parameter, int default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
				writer.Write(value);
			}

			public override void Deserialize(IReader reader)
			{
				value = reader.ReadInt32();
			}

			public override void ShowEditor(Instance base_smi)
			{
			}
		}

		public IntParameter()
		{
		}

		public IntParameter(int default_value)
			: base(default_value)
		{
		}

		public int Delta(int delta_value, StateMachineInstanceType smi)
		{
			int num = Get(smi);
			num += delta_value;
			Set(num, smi);
			return num;
		}

		public override Parameter.Context CreateContext()
		{
			return new Context(this, defaultValue);
		}
	}

	public class ResourceParameter<ResourceType> : Parameter<ResourceType> where ResourceType : Resource
	{
		public new class Context : Parameter<ResourceType>.Context
		{
			public Context(Parameter parameter, ResourceType default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
				string str = "";
				if (value != null)
				{
					if (value.Guid == (ResourceGuid)null)
					{
						Debug.LogError("Cannot serialize resource with invalid guid: " + value.Id);
					}
					else
					{
						str = value.Guid.Guid;
					}
				}
				writer.WriteKleiString(str);
			}

			public override void Deserialize(IReader reader)
			{
				string text = reader.ReadKleiString();
				if (text != "")
				{
					ResourceGuid guid = new ResourceGuid(text, null);
					value = Db.Get().GetResource<ResourceType>(guid);
				}
			}

			public override void ShowEditor(Instance base_smi)
			{
			}
		}

		public ResourceParameter()
			: base((ResourceType)null)
		{
		}

		public override Parameter.Context CreateContext()
		{
			return new Context(this, defaultValue);
		}
	}

	public class ObjectParameter<ObjectType> : Parameter<ObjectType> where ObjectType : class
	{
		public new class Context : Parameter<ObjectType>.Context
		{
			public Context(Parameter parameter, ObjectType default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
			}

			public override void Deserialize(IReader reader)
			{
			}

			public override void ShowEditor(Instance base_smi)
			{
			}
		}

		public ObjectParameter()
			: base((ObjectType)null)
		{
		}

		public override Parameter.Context CreateContext()
		{
			return new Context(this, defaultValue);
		}
	}

	public class TargetParameter : Parameter<GameObject>
	{
		public new class Context : Parameter<GameObject>.Context
		{
			private StateMachineInstanceType smi;

			private int objectDestroyedHandler;

			public Context(Parameter parameter, GameObject default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
			}

			public override void Deserialize(IReader reader)
			{
			}

			public override void Cleanup()
			{
				base.Cleanup();
				if ((UnityEngine.Object)value != (UnityEngine.Object)null)
				{
					value.GetComponent<KMonoBehaviour>().Unsubscribe(objectDestroyedHandler);
					objectDestroyedHandler = 0;
				}
			}

			public override void Set(GameObject value, StateMachineInstanceType smi)
			{
				this.smi = smi;
				if ((UnityEngine.Object)base.value != (UnityEngine.Object)null)
				{
					base.value.GetComponent<KMonoBehaviour>().Unsubscribe(objectDestroyedHandler);
					objectDestroyedHandler = 0;
				}
				if ((UnityEngine.Object)value != (UnityEngine.Object)null)
				{
					objectDestroyedHandler = value.GetComponent<KMonoBehaviour>().Subscribe(1969584890, OnObjectDestroyed);
				}
				base.Set(value, smi);
			}

			private void OnObjectDestroyed(object data)
			{
				Set(null, smi);
			}

			public override void ShowEditor(Instance base_smi)
			{
			}
		}

		public TargetParameter()
			: base((GameObject)null)
		{
		}

		public SMT GetSMI<SMT>(StateMachineInstanceType smi) where SMT : Instance
		{
			GameObject gameObject = base.Get(smi);
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				SMT sMI = StateMachineControllerExtensions.GetSMI<SMT>(gameObject);
				if (sMI != null)
				{
					return sMI;
				}
				Debug.LogError(gameObject.name + " does not have state machine " + typeof(StateMachineType).Name);
			}
			return (SMT)null;
		}

		public bool IsNull(StateMachineInstanceType smi)
		{
			GameObject x = Get(smi);
			return (UnityEngine.Object)x == (UnityEngine.Object)null;
		}

		public ComponentType Get<ComponentType>(StateMachineInstanceType smi)
		{
			GameObject gameObject = base.Get(smi);
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				ComponentType component = gameObject.GetComponent<ComponentType>();
				if (component != null)
				{
					return component;
				}
				Debug.LogError(gameObject.name + " does not have component " + typeof(ComponentType).Name);
			}
			return default(ComponentType);
		}

		public void Set(KMonoBehaviour value, StateMachineInstanceType smi)
		{
			GameObject value2 = null;
			if ((UnityEngine.Object)value != (UnityEngine.Object)null)
			{
				value2 = value.gameObject;
			}
			Set(value2, smi);
		}

		public override Parameter.Context CreateContext()
		{
			return new Context(this, defaultValue);
		}
	}

	public class SignalParameter
	{
	}

	public class Signal : Parameter<SignalParameter>
	{
		public new class Context : Parameter<SignalParameter>.Context
		{
			public Context(Parameter parameter, SignalParameter default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
			}

			public override void Deserialize(IReader reader)
			{
			}

			public override void Set(SignalParameter value, StateMachineInstanceType smi)
			{
				if (onDirty != null)
				{
					onDirty(smi);
				}
			}

			public override void ShowEditor(Instance base_smi)
			{
			}
		}

		public Signal()
			: base((SignalParameter)null)
		{
			isSignal = true;
		}

		public void Trigger(StateMachineInstanceType smi)
		{
			Context context = (Context)smi.GetParameterContext(this);
			context.Set(null, smi);
		}

		public override Parameter.Context CreateContext()
		{
			return new Context(this, defaultValue);
		}
	}

	private List<State> states = new List<State>();

	public TargetParameter masterTarget;

	[DoNotAutoCreate]
	protected TargetParameter stateTarget;

	public override string[] GetStateNames()
	{
		List<string> list = new List<string>();
		foreach (State state in states)
		{
			list.Add(state.name);
		}
		return list.ToArray();
	}

	public void Target(TargetParameter target)
	{
		stateTarget = target;
	}

	public void BindState(State parent_state, State state, string state_name)
	{
		if (parent_state != null)
		{
			state_name = parent_state.name + "." + state_name;
		}
		state.name = state_name;
		state.longName = name + "." + state_name;
		state.debugPushName = "PuS: " + state.longName;
		state.debugPopName = "PoS: " + state.longName;
		state.debugExecuteName = "EA: " + state.longName;
		List<BaseState> list = null;
		list = ((parent_state == null) ? new List<BaseState>() : new List<BaseState>(parent_state.branch));
		list.Add(state);
		state.parent = parent_state;
		state.branch = list.ToArray();
		maxDepth = Math.Max(state.branch.Length, maxDepth);
		states.Add(state);
	}

	public void BindStates(State parent_state, object state_machine)
	{
		Type type = state_machine.GetType();
		FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.FieldType.IsSubclassOf(typeof(BaseState)))
			{
				State state = (State)fieldInfo.GetValue(state_machine);
				string name = fieldInfo.Name;
				BindState(parent_state, state, name);
				BindStates(state, state);
			}
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		base.InitializeStates(out default_state);
	}

	public override void BindStates()
	{
		BindStates(null, this);
	}

	public override Type GetStateMachineInstanceType()
	{
		return typeof(StateMachineInstanceType);
	}

	public override BaseState GetState(string state_name)
	{
		foreach (State state in states)
		{
			if (state.name == state_name)
			{
				return state;
			}
		}
		return null;
	}

	public override void FreeResources()
	{
		for (int i = 0; i < states.Count; i++)
		{
			states[i].FreeResources();
		}
		states.Clear();
		base.FreeResources();
	}
}
