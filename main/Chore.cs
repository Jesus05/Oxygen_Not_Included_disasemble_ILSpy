using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class Chore
{
	public delegate bool PreconditionFn(ref Precondition.Context context, object data);

	public struct PreconditionInstance
	{
		public string id;

		public string description;

		public int sortOrder;

		public PreconditionFn fn;

		public object data;
	}

	public struct Precondition
	{
		[DebuggerDisplay("{chore.GetType()}, {chore.gameObject.name}")]
		public struct Context : IComparable<Context>, IEquatable<Context>
		{
			public PrioritySetting masterPriority;

			public int personalPriority;

			public int priority;

			public int priorityMod;

			public int interruptPriority;

			public int cost;

			public int consumerPriority;

			public Chore chore;

			public ChoreConsumerState consumerState;

			public int failedPreconditionId;

			public object data;

			public bool isAttemptingOverride;

			public ChoreType choreTypeForPermission;

			public Context(Chore chore, ChoreConsumerState consumer_state, bool is_attempting_override, object data = null)
			{
				masterPriority = chore.masterPriority;
				personalPriority = consumer_state.consumer.GetPersonalPriority(chore.choreType);
				priority = 0;
				priorityMod = chore.priorityMod;
				consumerPriority = 0;
				interruptPriority = 0;
				cost = 0;
				this.chore = chore;
				consumerState = consumer_state;
				failedPreconditionId = -1;
				isAttemptingOverride = is_attempting_override;
				this.data = data;
				choreTypeForPermission = chore.choreType;
				SetPriority(chore);
			}

			public void Set(Chore chore, ChoreConsumerState consumer_state, bool is_attempting_override, object data = null)
			{
				masterPriority = chore.masterPriority;
				priority = 0;
				priorityMod = chore.priorityMod;
				consumerPriority = 0;
				interruptPriority = 0;
				cost = 0;
				this.chore = chore;
				consumerState = consumer_state;
				failedPreconditionId = -1;
				isAttemptingOverride = is_attempting_override;
				this.data = data;
				choreTypeForPermission = chore.choreType;
				SetPriority(chore);
			}

			public void SetPriority(Chore chore)
			{
				priority = ((!Game.Instance.advancedPersonalPriorities) ? chore.choreType.priority : chore.choreType.explicitPriority);
				priorityMod = chore.priorityMod;
				interruptPriority = chore.choreType.interruptPriority;
			}

			public bool IsSuccess()
			{
				return failedPreconditionId == -1;
			}

			public bool IsPotentialSuccess()
			{
				if (IsSuccess())
				{
					return true;
				}
				if ((UnityEngine.Object)chore.driver == (UnityEngine.Object)consumerState.choreDriver)
				{
					return true;
				}
				if (failedPreconditionId != -1)
				{
					if (failedPreconditionId >= 0 && failedPreconditionId < chore.preconditions.Count)
					{
						PreconditionInstance preconditionInstance = chore.preconditions[failedPreconditionId];
						return preconditionInstance.id == ChorePreconditions.instance.IsMoreSatisfyingLate.id;
					}
					DebugUtil.DevLogErrorFormat("failedPreconditionId out of range {0}/{1}", failedPreconditionId, chore.preconditions.Count);
				}
				return false;
			}

			public void RunPreconditions()
			{
				if (chore.debug)
				{
					int num = 0;
					num++;
					if (consumerState.consumer.debug)
					{
						num++;
						Debugger.Break();
					}
				}
				if (chore.arePreconditionsDirty)
				{
					chore.preconditions.Sort((PreconditionInstance x, PreconditionInstance y) => x.sortOrder.CompareTo(y.sortOrder));
					chore.arePreconditionsDirty = false;
				}
				int num2 = 0;
				while (true)
				{
					if (num2 >= chore.preconditions.Count)
					{
						return;
					}
					PreconditionInstance preconditionInstance = chore.preconditions[num2];
					if (!preconditionInstance.fn(ref this, preconditionInstance.data))
					{
						break;
					}
					num2++;
				}
				failedPreconditionId = num2;
			}

			public int CompareTo(Context obj)
			{
				bool flag = failedPreconditionId != -1;
				bool flag2 = obj.failedPreconditionId != -1;
				if (flag == flag2)
				{
					int num = masterPriority.priority_class - obj.masterPriority.priority_class;
					if (num != 0)
					{
						return num;
					}
					int num2 = personalPriority - obj.personalPriority;
					if (num2 != 0)
					{
						return num2;
					}
					int num3 = masterPriority.priority_value - obj.masterPriority.priority_value;
					if (num3 != 0)
					{
						return num3;
					}
					int num4 = priority - obj.priority;
					if (num4 != 0)
					{
						return num4;
					}
					int num5 = priorityMod - obj.priorityMod;
					if (num5 != 0)
					{
						return num5;
					}
					int num6 = consumerPriority - obj.consumerPriority;
					if (num6 != 0)
					{
						return num6;
					}
					int num7 = obj.cost - cost;
					if (num7 != 0)
					{
						return num7;
					}
					if (chore == null && obj.chore == null)
					{
						return 0;
					}
					if (chore == null)
					{
						return -1;
					}
					if (obj.chore == null)
					{
						return 1;
					}
					return chore.id - obj.chore.id;
				}
				return (!flag) ? 1 : (-1);
			}

			public override bool Equals(object obj)
			{
				Context obj2 = (Context)obj;
				return CompareTo(obj2) == 0;
			}

			public bool Equals(Context other)
			{
				return CompareTo(other) == 0;
			}

			public override int GetHashCode()
			{
				return ((ValueType)this).GetHashCode();
			}

			public static bool operator ==(Context x, Context y)
			{
				return x.CompareTo(y) == 0;
			}

			public static bool operator !=(Context x, Context y)
			{
				return x.CompareTo(y) != 0;
			}
		}

		public string id;

		public string description;

		public int sortOrder;

		public PreconditionFn fn;
	}

	private static int nextId;

	public bool isExpanded;

	public bool showAvailabilityInHoverText = true;

	public PrioritySetting masterPriority;

	public Action<Chore> onExit;

	public Action<Chore> onComplete;

	private Action<Chore> onBegin;

	private Action<Chore> onEnd;

	public Action<Chore> onCleanup;

	public bool debug;

	private List<PreconditionInstance> preconditions = new List<PreconditionInstance>();

	private bool arePreconditionsDirty;

	public bool addToDailyReport;

	public ReportManager.ReportType reportType;

	private Prioritizable prioritizable;

	public const int MAX_PLAYER_BASIC_PRIORITY = 9;

	public const int MIN_PLAYER_BASIC_PRIORITY = 1;

	public const int MAX_PLAYER_HIGH_PRIORITY = 0;

	public const int MIN_PLAYER_HIGH_PRIORITY = 0;

	public const int MAX_PLAYER_EMERGENCY_PRIORITY = 1;

	public const int MIN_PLAYER_EMERGENCY_PRIORITY = 1;

	public const int DEFAULT_BASIC_PRIORITY = 5;

	public const int MAX_BASIC_PRIORITY = 10;

	public const int MIN_BASIC_PRIORITY = 0;

	public static bool ENABLE_PERSONAL_PRIORITIES = true;

	public static PrioritySetting DefaultPrioritySetting = new PrioritySetting(PriorityScreen.PriorityClass.basic, 5);

	public int id
	{
		get;
		private set;
	}

	public ChoreDriver driver
	{
		get;
		set;
	}

	public ChoreDriver lastDriver
	{
		get;
		set;
	}

	public ChoreType choreType
	{
		get;
		set;
	}

	public ChoreProvider provider
	{
		get;
		set;
	}

	public ChoreConsumer overrideTarget
	{
		get;
		private set;
	}

	public bool isComplete
	{
		get;
		protected set;
	}

	public IStateMachineTarget target
	{
		get;
		protected set;
	}

	public Tag[] choreTags
	{
		get;
		private set;
	}

	public bool runUntilComplete
	{
		get;
		set;
	}

	public int priorityMod
	{
		get;
		set;
	}

	public abstract GameObject gameObject
	{
		get;
	}

	public abstract bool isNull
	{
		get;
	}

	public bool IsPreemptable
	{
		get;
		protected set;
	}

	public Chore(ChoreType chore_type, ChoreProvider chore_provider, Tag[] chore_tags, bool run_until_complete, Action<Chore> on_complete, Action<Chore> on_begin, Action<Chore> on_end, PriorityScreen.PriorityClass priority_class, int priority_value, bool is_preemptable, bool allow_in_context_menu, int priority_mod, bool add_to_daily_report, ReportManager.ReportType report_type)
	{
		if (priority_value == 2147483647)
		{
			priority_class = PriorityScreen.PriorityClass.topPriority;
			priority_value = 2;
		}
		if (priority_value < 1 || priority_value > 9)
		{
			Debug.LogErrorFormat("Priority Value Out Of Range: {0}", priority_value);
		}
		masterPriority = new PrioritySetting(priority_class, priority_value);
		priorityMod = priority_mod;
		id = ++nextId;
		if ((UnityEngine.Object)chore_provider == (UnityEngine.Object)null)
		{
			chore_provider = GlobalChoreProvider.Instance;
			DebugUtil.Assert((UnityEngine.Object)chore_provider != (UnityEngine.Object)null);
		}
		choreType = chore_type;
		choreTags = chore_tags;
		runUntilComplete = run_until_complete;
		onComplete = on_complete;
		onEnd = on_end;
		onBegin = on_begin;
		IsPreemptable = is_preemptable;
		AddPrecondition(ChorePreconditions.instance.IsValid, null);
		AddPrecondition(ChorePreconditions.instance.IsPermitted, null);
		AddPrecondition(ChorePreconditions.instance.IsPreemptable, null);
		AddPrecondition(ChorePreconditions.instance.HasUrge, null);
		AddPrecondition(ChorePreconditions.instance.IsMoreSatisfyingEarly, null);
		AddPrecondition(ChorePreconditions.instance.IsMoreSatisfyingLate, null);
		AddPrecondition(ChorePreconditions.instance.IsOverrideTargetNullOrMe, null);
		chore_provider.AddChore(this);
	}

	protected abstract StateMachine.Instance GetSMI();

	public bool InProgress()
	{
		return (UnityEngine.Object)driver != (UnityEngine.Object)null;
	}

	public bool IsValid()
	{
		return (UnityEngine.Object)provider != (UnityEngine.Object)null;
	}

	public virtual void Cleanup()
	{
		ClearPrioritizable();
	}

	public void SetPriorityMod(int priorityMod)
	{
		this.priorityMod = priorityMod;
	}

	public List<PreconditionInstance> GetPreconditions()
	{
		if (arePreconditionsDirty)
		{
			preconditions.Sort((PreconditionInstance x, PreconditionInstance y) => x.sortOrder.CompareTo(y.sortOrder));
			arePreconditionsDirty = false;
		}
		return preconditions;
	}

	protected void SetPrioritizable(Prioritizable prioritizable)
	{
		if ((UnityEngine.Object)prioritizable != (UnityEngine.Object)null && prioritizable.IsPrioritizable())
		{
			this.prioritizable = prioritizable;
			masterPriority = prioritizable.GetMasterPriority();
			prioritizable.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(prioritizable.onPriorityChanged, new Action<PrioritySetting>(OnMasterPriorityChanged));
		}
	}

	private void ClearPrioritizable()
	{
		if ((UnityEngine.Object)prioritizable != (UnityEngine.Object)null)
		{
			Prioritizable obj = prioritizable;
			obj.onPriorityChanged = (Action<PrioritySetting>)Delegate.Remove(obj.onPriorityChanged, new Action<PrioritySetting>(OnMasterPriorityChanged));
		}
	}

	private void OnMasterPriorityChanged(PrioritySetting priority)
	{
		masterPriority = priority;
	}

	public void SetOverrideTarget(ChoreConsumer chore_consumer)
	{
		string text = "null";
		if ((UnityEngine.Object)chore_consumer != (UnityEngine.Object)null)
		{
			text = chore_consumer.name;
		}
		overrideTarget = chore_consumer;
		Fail("New override target");
	}

	public void AddPrecondition(Precondition precondition, object data = null)
	{
		arePreconditionsDirty = true;
		preconditions.Add(new PreconditionInstance
		{
			id = precondition.id,
			description = precondition.description,
			sortOrder = precondition.sortOrder,
			fn = precondition.fn,
			data = data
		});
	}

	public virtual void CollectChores(ChoreConsumerState consumer_state, List<Precondition.Context> succeeded_contexts, List<Precondition.Context> failed_contexts, bool is_attempting_override)
	{
		Precondition.Context item = new Precondition.Context(this, consumer_state, is_attempting_override, null);
		item.RunPreconditions();
		if (item.IsSuccess())
		{
			succeeded_contexts.Add(item);
		}
		else
		{
			failed_contexts.Add(item);
		}
	}

	public bool SatisfiesUrge(Urge urge)
	{
		return urge == choreType.urge;
	}

	public ReportManager.ReportType GetReportType()
	{
		return reportType;
	}

	public virtual void PrepareChore(ref Precondition.Context context)
	{
	}

	public virtual string ResolveString(string str)
	{
		return str;
	}

	public virtual void Begin(Precondition.Context context)
	{
		if ((UnityEngine.Object)driver != (UnityEngine.Object)null)
		{
			Debug.LogErrorFormat("Chore.Begin driver already set {0} {1} {2}, provider {3}, driver {4} -> {5}", id, GetType(), choreType.Id, provider, driver, context.consumerState.choreDriver);
		}
		if ((UnityEngine.Object)provider == (UnityEngine.Object)null)
		{
			Debug.LogErrorFormat("Chore.Begin provider is null {0} {1} {2}, provider {3}, driver {4}", id, GetType(), choreType.Id, provider, driver);
		}
		driver = context.consumerState.choreDriver;
		StateMachine.Instance sMI = GetSMI();
		StateMachine.Instance instance = sMI;
		instance.OnStop = (Action<string, StateMachine.Status>)Delegate.Combine(instance.OnStop, new Action<string, StateMachine.Status>(OnStateMachineStop));
		KSelectable component = driver.GetComponent<KSelectable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.Main, GetStatusItem(), this);
		}
		sMI.StartSM();
		if (onBegin != null)
		{
			onBegin(this);
		}
	}

	protected virtual void End(string reason)
	{
		if ((UnityEngine.Object)driver != (UnityEngine.Object)null)
		{
			KSelectable component = driver.GetComponent<KSelectable>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.Main, null, null);
			}
		}
		StateMachine.Instance sMI = GetSMI();
		StateMachine.Instance instance = sMI;
		instance.OnStop = (Action<string, StateMachine.Status>)Delegate.Remove(instance.OnStop, new Action<string, StateMachine.Status>(OnStateMachineStop));
		sMI.StopSM(reason);
		if (!((UnityEngine.Object)driver == (UnityEngine.Object)null))
		{
			lastDriver = driver;
			driver = null;
			if (onEnd != null)
			{
				onEnd(this);
			}
			if (onExit != null)
			{
				onExit(this);
			}
			driver = null;
		}
	}

	protected void Succeed(string reason)
	{
		if (RemoveFromProvider())
		{
			isComplete = true;
			if (onComplete != null)
			{
				onComplete(this);
			}
			if (addToDailyReport)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, -1f, choreType.Name, GameUtil.GetChoreName(this, null));
			}
			End(reason);
			Cleanup();
		}
	}

	protected virtual StatusItem GetStatusItem()
	{
		return choreType.statusItem;
	}

	public virtual void Fail(string reason)
	{
		if (!((UnityEngine.Object)provider == (UnityEngine.Object)null) && !((UnityEngine.Object)driver == (UnityEngine.Object)null))
		{
			if (!runUntilComplete)
			{
				Cancel(reason);
			}
			else
			{
				End(reason);
			}
		}
	}

	public void Cancel(string reason)
	{
		if (RemoveFromProvider())
		{
			if (addToDailyReport)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, -1f, choreType.Name, GameUtil.GetChoreName(this, null));
			}
			End(reason);
			Cleanup();
		}
	}

	protected virtual void OnStateMachineStop(string reason, StateMachine.Status status)
	{
		if (status == StateMachine.Status.Success)
		{
			Succeed(reason);
		}
		else
		{
			Fail(reason);
		}
	}

	private bool RemoveFromProvider()
	{
		if ((UnityEngine.Object)provider != (UnityEngine.Object)null)
		{
			provider.RemoveChore(this);
			return true;
		}
		return false;
	}

	public virtual bool CanPreempt(Precondition.Context context)
	{
		return IsPreemptable;
	}

	protected virtual void ShowCustomEditor(string filter, int width)
	{
	}

	public virtual string GetReportName(string context = null)
	{
		if (context == null || choreType.reportName == null)
		{
			return choreType.Name;
		}
		return string.Format(choreType.reportName, context);
	}
}
public class Chore<StateMachineInstanceType> : Chore, IStateMachineTarget where StateMachineInstanceType : StateMachine.Instance
{
	public StateMachineInstanceType smi
	{
		get;
		protected set;
	}

	public override GameObject gameObject => base.target.gameObject;

	public Transform transform => base.target.gameObject.transform;

	public string name => gameObject.name;

	public override bool isNull => base.target.isNull;

	public Chore(ChoreType chore_type, IStateMachineTarget target, ChoreProvider chore_provider, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null, PriorityScreen.PriorityClass master_priority_class = PriorityScreen.PriorityClass.basic, int master_priority_value = 5, bool is_preemptable = false, bool allow_in_context_menu = true, int priority_mod = 0, Tag[] chore_tags = null, bool add_to_daily_report = false, ReportManager.ReportType report_type = ReportManager.ReportType.WorkTime)
		: base(chore_type, chore_provider, chore_tags, run_until_complete, on_complete, on_begin, on_end, master_priority_class, master_priority_value, is_preemptable, allow_in_context_menu, priority_mod, add_to_daily_report, report_type)
	{
		base.target = target;
		target.Subscribe(1969584890, OnTargetDestroyed);
		reportType = report_type;
		addToDailyReport = add_to_daily_report;
		if (addToDailyReport)
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, 1f, chore_type.Name, GameUtil.GetChoreName(this, null));
		}
	}

	protected override StateMachine.Instance GetSMI()
	{
		return smi;
	}

	public int Subscribe(int hash, Action<object> handler)
	{
		return GetComponent<KPrefabID>().Subscribe(hash, handler);
	}

	public void Unsubscribe(int hash, Action<object> handler)
	{
		GetComponent<KPrefabID>().Unsubscribe(hash, handler);
	}

	public void Unsubscribe(int id)
	{
		GetComponent<KPrefabID>().Unsubscribe(id);
	}

	public void Trigger(int hash, object data = null)
	{
		GetComponent<KPrefabID>().Trigger(hash, data);
	}

	public ComponentType GetComponent<ComponentType>()
	{
		return base.target.GetComponent<ComponentType>();
	}

	public override string ResolveString(string str)
	{
		if (!base.target.isNull)
		{
			str = str.Replace("{Target}", base.target.gameObject.GetProperName());
		}
		return base.ResolveString(str);
	}

	public override void Cleanup()
	{
		base.Cleanup();
		if (base.target != null)
		{
			base.target.Unsubscribe(1969584890, OnTargetDestroyed);
		}
		if (onCleanup != null)
		{
			onCleanup(this);
		}
	}

	private void OnTargetDestroyed(object data)
	{
		Cancel("Target Destroyed");
	}

	public override bool CanPreempt(Precondition.Context context)
	{
		return base.CanPreempt(context);
	}
}
