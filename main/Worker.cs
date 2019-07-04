using Klei.AI;
using System;
using UnityEngine;

public class Worker : KMonoBehaviour
{
	public enum State
	{
		Idle,
		Working,
		PendingCompletion,
		Completing
	}

	public class StartWorkInfo
	{
		public Workable workable
		{
			get;
			set;
		}

		public StartWorkInfo(Workable workable)
		{
			this.workable = workable;
		}
	}

	public enum WorkResult
	{
		Success,
		InProgress,
		Failed
	}

	private const float EARLIEST_REACT_TIME = 1f;

	[MyCmpGet]
	private Facing facing;

	[MyCmpGet]
	private MinionResume resume;

	private float workPendingCompletionTime;

	private int onWorkChoreDisabledHandle;

	public object workCompleteData;

	private Workable.AnimInfo animInfo;

	private KAnimSynchronizer kanimSynchronizer;

	private StatusItemGroup.Entry previousStatusItem;

	private StateMachine.Instance smi;

	private bool successFullyCompleted;

	private Vector3 workAnimOffset = Vector3.zero;

	public bool usesMultiTool = true;

	private Reactable passerbyReactable;

	public State state
	{
		get;
		private set;
	}

	public StartWorkInfo startWorkInfo
	{
		get;
		private set;
	}

	public Workable workable
	{
		get
		{
			if (startWorkInfo == null)
			{
				return null;
			}
			return startWorkInfo.workable;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		state = State.Idle;
		Subscribe(1485595942, OnChoreInterrupt);
	}

	private string GetWorkableDebugString()
	{
		if (!((UnityEngine.Object)workable == (UnityEngine.Object)null))
		{
			return workable.name;
		}
		return "Null";
	}

	public void CompleteWork()
	{
		successFullyCompleted = false;
		state = State.Idle;
		if ((UnityEngine.Object)workable != (UnityEngine.Object)null)
		{
			if (workable.triggerWorkReactions && workable.GetWorkTime() > 30f)
			{
				string conversationTopic = workable.GetConversationTopic();
				if (!conversationTopic.IsNullOrWhiteSpace())
				{
					CreateCompletionReactable(conversationTopic);
				}
			}
			DetachAnimOverrides();
			workable.CompleteWork(this);
		}
		InternalStopWork(workable, false);
	}

	public WorkResult Work(float dt)
	{
		if (state != State.PendingCompletion)
		{
			if (state != State.Completing)
			{
				if ((UnityEngine.Object)workable != (UnityEngine.Object)null)
				{
					if ((bool)facing)
					{
						if (workable.ShouldFaceTargetWhenWorking())
						{
							facing.Face(workable.GetFacingTarget());
						}
						else
						{
							Rotatable component = workable.GetComponent<Rotatable>();
							bool flag = (UnityEngine.Object)component != (UnityEngine.Object)null && component.GetOrientation() == Orientation.FlipH;
							Vector3 position = facing.transform.GetPosition();
							position += ((!flag) ? Vector3.right : Vector3.left);
							facing.Face(position);
						}
					}
					Klei.AI.Attribute workAttribute = workable.GetWorkAttribute();
					if (workAttribute != null && workAttribute.IsTrainable)
					{
						float attributeExperienceMultiplier = workable.GetAttributeExperienceMultiplier();
						GetComponent<AttributeLevels>().AddExperience(workAttribute.Id, dt, attributeExperienceMultiplier);
					}
					string skillExperienceSkillGroup = workable.GetSkillExperienceSkillGroup();
					if ((UnityEngine.Object)resume != (UnityEngine.Object)null && skillExperienceSkillGroup != null)
					{
						float skillExperienceMultiplier = workable.GetSkillExperienceMultiplier();
						resume.AddExperienceWithAptitude(skillExperienceSkillGroup, dt, skillExperienceMultiplier);
					}
					float efficiencyMultiplier = workable.GetEfficiencyMultiplier(this);
					float dt2 = dt * efficiencyMultiplier * 1f;
					if (workable.WorkTick(this, dt2) && state == State.Working)
					{
						successFullyCompleted = true;
						StartPlayingPostAnim();
					}
				}
				return WorkResult.InProgress;
			}
			if (!successFullyCompleted)
			{
				StopWork();
				return WorkResult.Failed;
			}
			CompleteWork();
			return WorkResult.Success;
		}
		if (!GetComponent<KAnimControllerBase>().IsStopped() && !(Time.time - workPendingCompletionTime > 4f / Mathf.Max(Time.timeScale, 1f)))
		{
			return WorkResult.InProgress;
		}
		Navigator component2 = GetComponent<Navigator>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			NavGrid.NavTypeData navTypeData = component2.NavGrid.GetNavTypeData(component2.CurrentNavType);
			if (navTypeData.idleAnim.IsValid)
			{
				GetComponent<KAnimControllerBase>().Play(navTypeData.idleAnim, KAnim.PlayMode.Once, 1f, 0f);
			}
		}
		if (!successFullyCompleted)
		{
			StopWork();
			return WorkResult.Failed;
		}
		CompleteWork();
		return WorkResult.Success;
	}

	private void StartPlayingPostAnim()
	{
		if ((UnityEngine.Object)workable != (UnityEngine.Object)null && !workable.alwaysShowProgressBar)
		{
			workable.ShowProgressBar(false);
		}
		GetComponent<KPrefabID>().AddTag(GameTags.PreventChoreInterruption);
		state = State.PendingCompletion;
		workPendingCompletionTime = Time.time;
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		HashedString workPstAnim = workable.GetWorkPstAnim(this, successFullyCompleted);
		if (workPstAnim.IsValid)
		{
			if ((UnityEngine.Object)workable != (UnityEngine.Object)null && workable.synchronizeAnims)
			{
				KAnimControllerBase component2 = workable.GetComponent<KAnimControllerBase>();
				if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.HasAnimation(workPstAnim))
				{
					component2.Play(workPstAnim, KAnim.PlayMode.Once, 1f, 0f);
				}
			}
			else
			{
				component.Play(workPstAnim, KAnim.PlayMode.Once, 1f, 0f);
			}
		}
		else
		{
			state = State.Completing;
		}
		Trigger(-1142962013, this);
	}

	private void InternalStopWork(Workable target_workable, bool is_aborted)
	{
		state = State.Idle;
		base.gameObject.RemoveTag(GameTags.PerformingWorkRequest);
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		component.Offset -= workAnimOffset;
		workAnimOffset = Vector3.zero;
		GetComponent<KPrefabID>().RemoveTag(GameTags.PreventChoreInterruption);
		DetachAnimOverrides();
		ClearPasserbyReactable();
		AnimEventHandler component2 = GetComponent<AnimEventHandler>();
		if ((bool)component2)
		{
			component2.ClearContext();
		}
		if (previousStatusItem.item != null)
		{
			GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, previousStatusItem.item, previousStatusItem.data);
		}
		if ((UnityEngine.Object)target_workable != (UnityEngine.Object)null)
		{
			target_workable.Unsubscribe(onWorkChoreDisabledHandle);
			target_workable.StopWork(this, is_aborted);
		}
		if (smi != null)
		{
			smi.StopSM("stopping work");
			smi = null;
		}
		Vector3 position = base.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
		base.transform.SetPosition(position);
		startWorkInfo = null;
	}

	private void OnChoreInterrupt(object data)
	{
		if (state == State.Working)
		{
			successFullyCompleted = false;
			StartPlayingPostAnim();
		}
	}

	private void OnWorkChoreDisabled(object data)
	{
		string text = data as string;
		ChoreConsumer component = GetComponent<ChoreConsumer>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null && (UnityEngine.Object)component.choreDriver != (UnityEngine.Object)null)
		{
			Chore currentChore = component.choreDriver.GetCurrentChore();
			currentChore.Fail((text == null) ? "WorkChoreDisabled" : text);
		}
	}

	public void StopWork()
	{
		if (state == State.PendingCompletion || state == State.Completing)
		{
			state = State.Idle;
			if (successFullyCompleted)
			{
				CompleteWork();
			}
			else
			{
				InternalStopWork(workable, true);
			}
		}
		else if (state == State.Working)
		{
			if ((UnityEngine.Object)workable != (UnityEngine.Object)null && workable.synchronizeAnims)
			{
				KBatchedAnimController component = workable.GetComponent<KBatchedAnimController>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					HashedString workPstAnim = workable.GetWorkPstAnim(this, false);
					if (workPstAnim.IsValid)
					{
						component.Play(workPstAnim, KAnim.PlayMode.Once, 1f, 0f);
						component.SetPositionPercent(1f);
					}
				}
			}
			InternalStopWork(workable, true);
		}
	}

	public void StartWork(StartWorkInfo start_work_info)
	{
		startWorkInfo = start_work_info;
		Game.Instance.StartedWork();
		if (state != 0)
		{
			string text = "";
			if ((UnityEngine.Object)workable != (UnityEngine.Object)null)
			{
				text = workable.name;
			}
			Debug.LogError(base.name + "." + text + ".state should be idle but instead it's:" + state.ToString());
		}
		string name = workable.GetType().Name;
		try
		{
			base.gameObject.AddTag(GameTags.PerformingWorkRequest);
			state = State.Working;
			if ((UnityEngine.Object)workable != (UnityEngine.Object)null)
			{
				animInfo = workable.GetAnim(this);
				if (animInfo.smi != null)
				{
					smi = animInfo.smi;
					smi.StartSM();
				}
				Vector3 position = base.transform.GetPosition();
				position.z = Grid.GetLayerZ(workable.workLayer);
				base.transform.SetPosition(position);
				KAnimControllerBase component = GetComponent<KAnimControllerBase>();
				if (animInfo.smi == null)
				{
					AttachOverrideAnims(component);
				}
				HashedString[] workAnims = workable.GetWorkAnims(this);
				KAnim.PlayMode workAnimPlayMode = workable.GetWorkAnimPlayMode();
				Vector3 vector = workAnimOffset = workable.GetWorkOffset();
				component.Offset += vector;
				if (usesMultiTool && animInfo.smi == null && workAnims != null)
				{
					if (workable.synchronizeAnims)
					{
						KAnimControllerBase component2 = workable.GetComponent<KAnimControllerBase>();
						if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
						{
							kanimSynchronizer = component2.GetSynchronizer();
							if (kanimSynchronizer != null)
							{
								kanimSynchronizer.Add(component);
							}
						}
						component2.Play(workAnims, workAnimPlayMode);
					}
					else
					{
						component.Play(workAnims, workAnimPlayMode);
					}
				}
			}
			workable.StartWork(this);
			if ((UnityEngine.Object)workable == (UnityEngine.Object)null)
			{
				Debug.LogWarning("Stopped work as soon as I started. This is usually a sign that a chore is open when it shouldn't be or that it's preconditions are wrong.");
			}
			else
			{
				onWorkChoreDisabledHandle = workable.Subscribe(2108245096, OnWorkChoreDisabled);
				if (workable.triggerWorkReactions && workable.WorkTimeRemaining > 10f)
				{
					CreatePasserbyReactable();
				}
				KSelectable component3 = GetComponent<KSelectable>();
				previousStatusItem = component3.GetStatusItem(Db.Get().StatusItemCategories.Main);
				component3.SetStatusItem(Db.Get().StatusItemCategories.Main, workable.GetWorkerStatusItem(), workable);
			}
		}
		catch (Exception ex)
		{
			string str = "Exception in: Worker.StartWork(" + name + ")";
			DebugUtil.LogErrorArgs(this, str + "\n" + ex.ToString());
			throw;
		}
	}

	private void AttachOverrideAnims(KAnimControllerBase worker_controller)
	{
		if (animInfo.overrideAnims != null && animInfo.overrideAnims.Length > 0)
		{
			for (int i = 0; i < animInfo.overrideAnims.Length; i++)
			{
				worker_controller.AddAnimOverrides(animInfo.overrideAnims[i], 0f);
			}
		}
	}

	private void DetachAnimOverrides()
	{
		if (animInfo.overrideAnims != null)
		{
			KAnimControllerBase component = GetComponent<KAnimControllerBase>();
			if (kanimSynchronizer != null)
			{
				kanimSynchronizer.Remove(component);
				kanimSynchronizer = null;
			}
			for (int i = 0; i < animInfo.overrideAnims.Length; i++)
			{
				component.RemoveAnimOverrides(animInfo.overrideAnims[i]);
			}
			animInfo.overrideAnims = null;
		}
	}

	private void CreateCompletionReactable(string topic)
	{
		if (!(GameClock.Instance.GetTime() / 600f < 1f))
		{
			EmoteReactable emoteReactable = OneshotReactableLocator.CreateOneshotReactable(base.gameObject, 3f, "WorkCompleteAcknowledgement", Db.Get().ChoreTypes.Emote, "anim_clapcheer_kanim", 9, 5, 100f);
			emoteReactable.AddStep(new EmoteReactable.EmoteStep
			{
				anim = (HashedString)"clapcheer_pre",
				startcb = new Action<GameObject>(GetReactionEffect)
			}).AddStep(new EmoteReactable.EmoteStep
			{
				anim = (HashedString)"clapcheer_loop"
			}).AddStep(new EmoteReactable.EmoteStep
			{
				anim = (HashedString)"clapcheer_pst",
				finishcb = (Action<GameObject>)delegate(GameObject r)
				{
					r.Trigger(937885943, topic);
				}
			})
				.AddPrecondition(ReactorIsOnFloor);
			Tuple<Sprite, Color> tuple = null;
			tuple = Def.GetUISprite(topic, "ui", true);
			if (tuple != null)
			{
				Thought thought = new Thought("Completion_" + topic, null, tuple.first, "mode_satisfaction", "conversation_short", "bubble_conversation", SpeechMonitor.PREFIX_HAPPY, "", true, 4f);
				emoteReactable.AddThought(thought);
			}
		}
	}

	public void CreatePasserbyReactable()
	{
		if (!(GameClock.Instance.GetTime() / 600f < 1f) && passerbyReactable == null)
		{
			passerbyReactable = new EmoteReactable(base.gameObject, "WorkPasserbyAcknowledgement", Db.Get().ChoreTypes.Emote, "anim_react_thumbsup_kanim", 5, 5, 30f, 720f * TuningData<DupeGreetingManager.Tuning>.Get().greetingDelayMultiplier, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
			{
				anim = (HashedString)"react",
				startcb = new Action<GameObject>(GetReactionEffect)
			}).AddThought(Db.Get().Thoughts.Encourage).AddPrecondition(ReactorIsOnFloor)
				.AddPrecondition(ReactorIsFacingMe);
		}
	}

	private void GetReactionEffect(GameObject reactor)
	{
		Effects component = GetComponent<Effects>();
		component.Add("WorkEncouraged", true);
	}

	private bool ReactorIsOnFloor(GameObject reactor, Navigator.ActiveTransition transition)
	{
		return transition.end == NavType.Floor;
	}

	private bool ReactorIsFacingMe(GameObject reactor, Navigator.ActiveTransition transition)
	{
		Facing component = reactor.GetComponent<Facing>();
		Vector3 position = base.transform.GetPosition();
		float x = position.x;
		Vector3 position2 = reactor.transform.GetPosition();
		return x < position2.x == component.GetFacing();
	}

	public void ClearPasserbyReactable()
	{
		if (passerbyReactable != null)
		{
			passerbyReactable.Cleanup();
			passerbyReactable = null;
		}
	}
}
