using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Workable : KMonoBehaviour, ISaveLoadable, IApproachable
{
	public enum WorkableEvent
	{
		WorkStarted,
		WorkCompleted,
		WorkStopped
	}

	public struct AnimInfo
	{
		public KAnimFile[] overrideAnims;

		public StateMachine.Instance smi;
	}

	public float workTime;

	public Vector3 AnimOffset = Vector3.zero;

	protected bool showProgressBar = true;

	protected StatusItem workerStatusItem;

	protected StatusItem workingStatusItem;

	protected Guid workStatusItemHandle;

	protected OffsetTracker offsetTracker;

	protected AttributeConverter attributeConverter;

	protected float minimumAttributeMultiplier = 0.5f;

	public bool resetProgressOnStop;

	protected bool shouldTransferDiseaseWithWorker = true;

	protected float attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;

	public bool triggerWorkReactions = true;

	[SerializeField]
	[Tooltip("What layer does the dupe switch to when interacting with the building")]
	public Grid.SceneLayer workLayer = Grid.SceneLayer.Move;

	[SerializeField]
	[Serialize]
	protected float workTimeRemaining = float.PositiveInfinity;

	[SerializeField]
	public KAnimFile[] overrideAnims;

	[SerializeField]
	protected HashedString multitoolContext;

	[SerializeField]
	protected Tag multitoolHitEffectTag;

	[SerializeField]
	[Tooltip("Whether to user the KAnimSynchronizer or not")]
	public bool synchronizeAnims = true;

	[SerializeField]
	[Tooltip("Whether to display number of uses in the details panel")]
	public bool trackUses = false;

	[Serialize]
	protected int numberOfUses;

	public Action<WorkableEvent> OnWorkableEventCB;

	private int roleUpdateHandle = -1;

	public HashedString requiredRolePerk;

	[SerializeField]
	protected bool shouldShowRolePerkStatusItem = true;

	protected StatusItem readyForRoleWorkStatusItem;

	public HashedString[] workAnims = new HashedString[2]
	{
		"working_pre",
		"working_loop"
	};

	public HashedString workingPstComplete = "working_pst";

	public HashedString workingPstFailed = "working_pst";

	public KAnim.PlayMode workAnimPlayMode = KAnim.PlayMode.Loop;

	public bool faceTargetWhenWorking = false;

	protected ProgressBar progressBar = null;

	public Worker worker
	{
		get;
		protected set;
	}

	public float WorkTimeRemaining
	{
		get
		{
			return workTimeRemaining;
		}
		set
		{
			workTimeRemaining = value;
		}
	}

	public bool preferUnreservedCell
	{
		get;
		set;
	}

	public virtual float GetWorkTime()
	{
		return workTime;
	}

	public Worker GetWorker()
	{
		return worker;
	}

	public virtual float GetPercentComplete()
	{
		return (!(workTimeRemaining <= workTime)) ? (-1f) : (1f - workTimeRemaining / workTime);
	}

	public virtual AnimInfo GetAnim(Worker worker)
	{
		AnimInfo result = default(AnimInfo);
		if (overrideAnims != null && overrideAnims.Length > 0)
		{
			result.overrideAnims = overrideAnims;
		}
		if (multitoolContext.IsValid && multitoolHitEffectTag.IsValid)
		{
			result.smi = new MultitoolController.Instance(this, worker, multitoolContext, Assets.GetPrefab(multitoolHitEffectTag));
		}
		return result;
	}

	public virtual HashedString[] GetWorkAnims(Worker worker)
	{
		return workAnims;
	}

	public virtual KAnim.PlayMode GetWorkAnimPlayMode()
	{
		return workAnimPlayMode;
	}

	public virtual HashedString GetWorkPstAnim(Worker worker, bool successfully_completed)
	{
		if (!successfully_completed)
		{
			return workingPstFailed;
		}
		return workingPstComplete;
	}

	public virtual Vector3 GetWorkOffset()
	{
		return Vector3.zero;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().MiscStatusItems.Using;
		workingStatusItem = Db.Get().MiscStatusItems.Operating;
		readyForRoleWorkStatusItem = Db.Get().BuildingStatusItems.RequiresRolePerk;
		workTime = GetWorkTime();
		workTimeRemaining = Mathf.Min(workTimeRemaining, workTime);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (shouldShowRolePerkStatusItem && requiredRolePerk.IsValid)
		{
			if (roleUpdateHandle != -1)
			{
				Game.Instance.Unsubscribe(roleUpdateHandle);
			}
			roleUpdateHandle = Game.Instance.Subscribe(-1523247426, UpdateStatusItem);
		}
		UpdateStatusItem(null);
	}

	protected virtual void UpdateStatusItem(object data = null)
	{
		KSelectable component = GetComponent<KSelectable>();
		if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
		{
			component.RemoveStatusItem(workStatusItemHandle, false);
			if ((UnityEngine.Object)worker == (UnityEngine.Object)null)
			{
				if (shouldShowRolePerkStatusItem && requiredRolePerk.IsValid)
				{
					if (Game.Instance.roleManager.GetRoleAssigneesWithPerk(requiredRolePerk).Count == 0)
					{
						workStatusItemHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.ColonyLacksRequiredRolePerk, requiredRolePerk);
					}
					else
					{
						workStatusItemHandle = component.AddStatusItem(readyForRoleWorkStatusItem, requiredRolePerk);
					}
				}
			}
			else if (workingStatusItem != null)
			{
				workStatusItemHandle = component.AddStatusItem(workingStatusItem, this);
			}
		}
	}

	protected override void OnLoadLevel()
	{
		overrideAnims = null;
		base.OnLoadLevel();
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}

	public void StartWork(Worker worker_to_start)
	{
		worker = worker_to_start;
		UpdateStatusItem(null);
		if (showProgressBar)
		{
			ShowProgressBar(true);
		}
		OnStartWork(worker);
		if ((UnityEngine.Object)worker != (UnityEngine.Object)null)
		{
			string conversationTopic = GetConversationTopic();
			if (conversationTopic != null)
			{
				worker.Trigger(937885943, conversationTopic);
			}
		}
		if (OnWorkableEventCB != null)
		{
			OnWorkableEventCB(WorkableEvent.WorkStarted);
		}
		numberOfUses++;
	}

	public bool WorkTick(Worker worker, float dt)
	{
		bool flag = false;
		if (dt > 0f)
		{
			workTimeRemaining -= dt;
			flag = OnWorkTick(worker, dt);
		}
		return flag || workTimeRemaining < 0f;
	}

	public virtual float GetEfficiencyMultiplier(Worker worker)
	{
		if (attributeConverter == null)
		{
			return 1f;
		}
		AttributeConverterInstance converter = worker.GetComponent<AttributeConverters>().GetConverter(attributeConverter.Id);
		return Mathf.Max(1f + converter.Evaluate(), minimumAttributeMultiplier);
	}

	public virtual Klei.AI.Attribute GetWorkAttribute()
	{
		if (attributeConverter == null)
		{
			return null;
		}
		return attributeConverter.attribute;
	}

	public virtual string GetConversationTopic()
	{
		KPrefabID component = GetComponent<KPrefabID>();
		return (!component.HasTag(GameTags.NotConversationTopic)) ? component.PrefabTag.Name : null;
	}

	public virtual void AwardExperience(float work_dt, MinionResume resume)
	{
	}

	public void SetAttributeConverter(AttributeConverter attributeConverter)
	{
		this.attributeConverter = attributeConverter;
	}

	public float GetAttributeExperienceMultiplier()
	{
		return attributeExperienceMultiplier;
	}

	protected virtual bool OnWorkTick(Worker worker, float dt)
	{
		return false;
	}

	public void StopWork(Worker workerToStop, bool aborted)
	{
		if ((UnityEngine.Object)worker == (UnityEngine.Object)workerToStop && aborted)
		{
			OnAbortWork(workerToStop);
		}
		if (shouldTransferDiseaseWithWorker)
		{
			TransferDiseaseWithWorker(workerToStop);
		}
		if (OnWorkableEventCB != null)
		{
			OnWorkableEventCB(WorkableEvent.WorkStopped);
		}
		OnStopWork(workerToStop);
		if (resetProgressOnStop)
		{
			workTimeRemaining = GetWorkTime();
		}
		ShowProgressBar(false);
		worker = null;
		UpdateStatusItem(null);
	}

	public virtual StatusItem GetWorkerStatusItem()
	{
		return workerStatusItem;
	}

	public void SetWorkerStatusItem(StatusItem item)
	{
		workerStatusItem = item;
	}

	public void CompleteWork(Worker worker)
	{
		if (shouldTransferDiseaseWithWorker)
		{
			TransferDiseaseWithWorker(worker);
		}
		OnCompleteWork(worker);
		if (OnWorkableEventCB != null)
		{
			OnWorkableEventCB(WorkableEvent.WorkCompleted);
		}
		if (OnWorkableEventCB != null)
		{
			OnWorkableEventCB(WorkableEvent.WorkStopped);
		}
		workTimeRemaining = GetWorkTime();
		ShowProgressBar(false);
	}

	protected virtual void OnStartWork(Worker worker)
	{
	}

	protected virtual void OnStopWork(Worker worker)
	{
	}

	protected virtual void OnCompleteWork(Worker worker)
	{
	}

	protected virtual void OnAbortWork(Worker worker)
	{
	}

	public void SetOffsets(CellOffset[] offsets)
	{
		if (offsetTracker != null)
		{
			offsetTracker.Clear();
		}
		offsetTracker = new StandardOffsetTracker(offsets);
	}

	public void SetOffsetTable(CellOffset[][] offset_table)
	{
		if (offsetTracker != null)
		{
			offsetTracker.Clear();
		}
		offsetTracker = new OffsetTableTracker(offset_table, this);
	}

	public virtual CellOffset[] GetOffsets(int cell)
	{
		if (offsetTracker == null)
		{
			offsetTracker = new StandardOffsetTracker(new CellOffset[1]
			{
				default(CellOffset)
			});
		}
		return offsetTracker.GetOffsets(cell);
	}

	public CellOffset[] GetOffsets()
	{
		return GetOffsets(Grid.PosToCell(this));
	}

	public void SetWorkTime(float work_time)
	{
		workTime = work_time;
		workTimeRemaining = work_time;
	}

	public bool ShouldFaceTargetWhenWorking()
	{
		return faceTargetWhenWorking;
	}

	public virtual Vector3 GetFacingTarget()
	{
		return base.transform.GetPosition();
	}

	public void ShowProgressBar(bool show)
	{
		if (show)
		{
			if ((UnityEngine.Object)progressBar == (UnityEngine.Object)null)
			{
				progressBar = ProgressBar.CreateProgressBar(this, GetPercentComplete);
			}
			progressBar.gameObject.SetActive(true);
		}
		else if ((UnityEngine.Object)progressBar != (UnityEngine.Object)null)
		{
			progressBar.gameObject.DeleteObject();
			progressBar = null;
		}
	}

	protected override void OnCleanUp()
	{
		ShowProgressBar(false);
		if (offsetTracker != null)
		{
			offsetTracker.Clear();
		}
		if (roleUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(roleUpdateHandle);
		}
		base.OnCleanUp();
		OnWorkableEventCB = null;
	}

	public virtual Vector3 GetTargetPoint()
	{
		Vector3 result = base.transform.GetPosition();
		float y = result.y + 0.65f;
		KBoxCollider2D component = GetComponent<KBoxCollider2D>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			result = component.bounds.center;
		}
		result.y = y;
		result.z = 0f;
		return result;
	}

	public int GetNavigationCost(Navigator navigator, int cell)
	{
		return navigator.GetNavigationCost(cell, GetOffsets(cell));
	}

	public int GetNavigationCost(Navigator navigator)
	{
		return GetNavigationCost(navigator, Grid.PosToCell(this));
	}

	private void TransferDiseaseWithWorker(Worker worker)
	{
		if (!((UnityEngine.Object)this == (UnityEngine.Object)null) && !((UnityEngine.Object)worker == (UnityEngine.Object)null))
		{
			TransferDiseaseWithWorker(base.gameObject, worker.gameObject);
		}
	}

	public static void TransferDiseaseWithWorker(GameObject workable, GameObject worker)
	{
		if (!((UnityEngine.Object)workable == (UnityEngine.Object)null) && !((UnityEngine.Object)worker == (UnityEngine.Object)null))
		{
			PrimaryElement component = workable.GetComponent<PrimaryElement>();
			if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
			{
				PrimaryElement component2 = worker.GetComponent<PrimaryElement>();
				if (!((UnityEngine.Object)component2 == (UnityEngine.Object)null))
				{
					SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
					invalid.idx = component2.DiseaseIdx;
					invalid.count = (int)((float)component2.DiseaseCount * 0.33f);
					SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid;
					invalid2.idx = component.DiseaseIdx;
					invalid2.count = (int)((float)component.DiseaseCount * 0.33f);
					component2.ModifyDiseaseCount(-invalid.count, "Workable.TransferDiseaseWithWorker");
					component.ModifyDiseaseCount(-invalid2.count, "Workable.TransferDiseaseWithWorker");
					if (invalid.count > 0)
					{
						component.AddDisease(invalid.idx, invalid.count, "Workable.TransferDiseaseWithWorker");
					}
					if (invalid2.count > 0)
					{
						component2.AddDisease(invalid2.idx, invalid2.count, "Workable.TransferDiseaseWithWorker");
					}
				}
			}
		}
	}

	public virtual List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (trackUses)
		{
			Descriptor item = new Descriptor(string.Format(BUILDING.DETAILS.USE_COUNT, numberOfUses), string.Format(BUILDING.DETAILS.USE_COUNT_TOOLTIP, numberOfUses), Descriptor.DescriptorType.Detail, false);
			list.Add(item);
		}
		return list;
	}

	[ContextMenu("Refresh Reachability")]
	public void RefreshReachability()
	{
		if (offsetTracker != null)
		{
			offsetTracker.ForceRefresh();
		}
	}

	Transform get_transform()
	{
		return base.transform;
	}

	Transform IApproachable.get_transform()
	{
		//ILSpy generated this explicit interface implementation from .override directive in get_transform
		return this.get_transform();
	}
}
