using Database;
using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ChoreConsumer : KMonoBehaviour, IPersonalPriorityManager
{
	private struct BehaviourPrecondition
	{
		public Func<object, bool> cb;

		public object arg;
	}

	private class PreconditionSnapshot
	{
		public List<Chore.Precondition.Context> succeededContexts = new List<Chore.Precondition.Context>();

		public List<Chore.Precondition.Context> failedContexts = new List<Chore.Precondition.Context>();

		public bool doFailedContextsNeedSorting = true;

		public void CopyTo(PreconditionSnapshot snapshot)
		{
			snapshot.Clear();
			snapshot.succeededContexts.AddRange(succeededContexts);
			snapshot.failedContexts.AddRange(failedContexts);
			snapshot.doFailedContextsNeedSorting = true;
		}

		public void Clear()
		{
			succeededContexts.Clear();
			failedContexts.Clear();
			doFailedContextsNeedSorting = true;
		}
	}

	public struct PriorityInfo
	{
		public int priority;

		public bool wasAutoAssigned;

		public int priorityWhenAutoAssigned;
	}

	public const int DEFAULT_PERSONAL_CHORE_PRIORITY = 3;

	public const int MIN_PERSONAL_PRIORITY = 0;

	public const int MAX_PERSONAL_PRIORITY = 5;

	[MyCmpAdd]
	public ChoreProvider choreProvider;

	[MyCmpAdd]
	public ChoreDriver choreDriver;

	[MyCmpGet]
	public Navigator navigator;

	[MyCmpGet]
	public MinionResume resume;

	[MyCmpAdd]
	private User user;

	public System.Action choreRulesChanged;

	public bool debug;

	private List<ChoreProvider> providers = new List<ChoreProvider>();

	private List<Urge> urges = new List<Urge>();

	public ChoreTable choreTable;

	private ChoreTable.Instance choreTableInstance;

	public ChoreConsumerState consumerState;

	private Dictionary<Tag, BehaviourPrecondition> behaviourPreconditions = new Dictionary<Tag, BehaviourPrecondition>();

	private PreconditionSnapshot preconditionSnapshot = new PreconditionSnapshot();

	[Serialize]
	private Dictionary<HashedString, PriorityInfo> choreGroupPriorities = new Dictionary<HashedString, PriorityInfo>();

	private Dictionary<HashedString, int> choreTypePriorities = new Dictionary<HashedString, int>();

	private List<HashedString> traitDisabledChoreGroups = new List<HashedString>();

	private List<HashedString> userDisabledChoreGroups = new List<HashedString>();

	public HashSet<Tag> preferredChoreTags = new HashSet<Tag>();

	private int stationaryReach = -1;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if ((UnityEngine.Object)ChoreGroupManager.instance != (UnityEngine.Object)null)
		{
			foreach (KeyValuePair<Tag, int> item in ChoreGroupManager.instance.DefaultChorePermission)
			{
				bool flag = false;
				foreach (HashedString userDisabledChoreGroup in userDisabledChoreGroups)
				{
					if (userDisabledChoreGroup.HashValue == item.Key.GetHashCode())
					{
						flag = true;
						break;
					}
				}
				if (!flag && item.Value == 0)
				{
					userDisabledChoreGroups.Add(new HashedString(item.Key.GetHashCode()));
				}
			}
		}
		providers.Add(choreProvider);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KPrefabID component = GetComponent<KPrefabID>();
		if (choreTable != null)
		{
			choreTableInstance = new ChoreTable.Instance(choreTable, component);
		}
		foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
		{
			bool auto_assigned;
			int personalPriority = GetPersonalPriority(resource, out auto_assigned);
			UpdateChoreTypePriorities(resource, personalPriority);
			SetPermittedByUser(resource, personalPriority != 0);
		}
		consumerState = new ChoreConsumerState(this);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (choreTableInstance != null)
		{
			choreTableInstance.OnCleanUp(GetComponent<KPrefabID>());
			choreTableInstance = null;
		}
	}

	public bool IsPermittedByUser(ChoreGroup chore_group)
	{
		return chore_group == null || !userDisabledChoreGroups.Contains(chore_group.IdHash);
	}

	public void SetPermittedByUser(ChoreGroup chore_group, bool is_allowed)
	{
		if (is_allowed)
		{
			if (userDisabledChoreGroups.Remove(chore_group.IdHash))
			{
				choreRulesChanged.Signal();
			}
		}
		else if (!userDisabledChoreGroups.Contains(chore_group.IdHash))
		{
			userDisabledChoreGroups.Add(chore_group.IdHash);
			choreRulesChanged.Signal();
		}
	}

	public bool IsPermittedByTraits(ChoreGroup chore_group)
	{
		return chore_group == null || !traitDisabledChoreGroups.Contains(chore_group.IdHash);
	}

	public void SetPermittedByTraits(ChoreGroup chore_group, bool is_enabled)
	{
		if (is_enabled)
		{
			if (traitDisabledChoreGroups.Remove(chore_group.IdHash))
			{
				choreRulesChanged.Signal();
			}
		}
		else if (!traitDisabledChoreGroups.Contains(chore_group.IdHash))
		{
			traitDisabledChoreGroups.Add(chore_group.IdHash);
			choreRulesChanged.Signal();
		}
	}

	public bool FindNextChore(ref Chore.Precondition.Context out_context)
	{
		if (debug)
		{
			int num = 0;
			num++;
		}
		preconditionSnapshot.Clear();
		consumerState.Refresh();
		if (consumerState.hasSolidTransferArm)
		{
			CellOffset offset = Grid.GetOffset(Grid.PosToCell(this));
			Extents extents = new Extents(offset.x, offset.y, stationaryReach);
			ListPool<ScenePartitionerEntry, ChoreConsumer>.PooledList pooledList = ListPool<ScenePartitionerEntry, ChoreConsumer>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.fetchChoreLayer, pooledList);
			foreach (ScenePartitionerEntry item in pooledList)
			{
				FetchChore fetchChore = item.obj as FetchChore;
				int cell = Grid.PosToCell(fetchChore.gameObject);
				if (consumerState.solidTransferArm.IsCellReachable(cell))
				{
					fetchChore.CollectChoresFromGlobalChoreProvider(consumerState, preconditionSnapshot.succeededContexts, preconditionSnapshot.failedContexts, false);
				}
			}
			pooledList.Recycle();
		}
		else
		{
			for (int i = 0; i < providers.Count; i++)
			{
				ChoreProvider choreProvider = providers[i];
				choreProvider.CollectChores(consumerState, preconditionSnapshot.succeededContexts, preconditionSnapshot.failedContexts);
			}
		}
		List<Chore.Precondition.Context> succeededContexts = preconditionSnapshot.succeededContexts;
		succeededContexts.Sort();
		bool result = false;
		if (succeededContexts.Count > 0)
		{
			Chore currentChore = choreDriver.GetCurrentChore();
			for (int num2 = succeededContexts.Count - 1; num2 >= 0; num2--)
			{
				Chore.Precondition.Context context = succeededContexts[num2];
				if (context.IsSuccess() && (currentChore == null || context.interruptPriority > currentChore.choreType.interruptPriority))
				{
					bool flag = false;
					if (currentChore != null)
					{
						for (int j = 0; j < currentChore.choreType.interruptExclusion.Count; j++)
						{
							if (context.chore.choreType.tags.Contains(currentChore.choreType.interruptExclusion[j]))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						context.chore.PrepareChore(ref context);
						out_context = context;
						result = true;
						break;
					}
				}
			}
		}
		return result;
	}

	public void AddProvider(ChoreProvider provider)
	{
		DebugUtil.Assert((UnityEngine.Object)provider != (UnityEngine.Object)null, "Assert!", string.Empty, string.Empty);
		providers.Add(provider);
	}

	public void RemoveProvider(ChoreProvider provider)
	{
		providers.Remove(provider);
	}

	public void AddUrge(Urge urge)
	{
		DebugUtil.Assert(urge != null, "Assert!", string.Empty, string.Empty);
		urges.Add(urge);
		Trigger(-736698276, urge);
	}

	public void RemoveUrge(Urge urge)
	{
		urges.Remove(urge);
		Trigger(231622047, urge);
	}

	public bool HasUrge(Urge urge)
	{
		return urges.Contains(urge);
	}

	public List<Urge> GetUrges()
	{
		return urges;
	}

	[Conditional("ENABLE_LOGGER")]
	public void Log(string evt, string param)
	{
	}

	public bool IsPermittedOrEnabled(ChoreType chore_type, Chore chore)
	{
		if (chore_type.groups.Length == 0)
		{
			return true;
		}
		for (int i = 0; i < chore_type.groups.Length; i++)
		{
			ChoreGroup chore_group = chore_type.groups[i];
			if (IsPermittedByTraits(chore_group) && IsPermittedByUser(chore_group))
			{
				return true;
			}
		}
		return false;
	}

	public void SetReach(int reach)
	{
		stationaryReach = reach;
	}

	public bool GetNavigationCost(IApproachable approachable, out int cost)
	{
		if ((bool)navigator)
		{
			cost = navigator.GetNavigationCost(approachable);
			if (cost != -1)
			{
				return true;
			}
		}
		else if (consumerState.hasSolidTransferArm)
		{
			int cell = approachable.GetCell();
			if (consumerState.solidTransferArm.IsCellReachable(cell))
			{
				cost = Grid.GetCellRange(this.NaturalBuildingCell(), cell);
				return true;
			}
		}
		cost = 0;
		return false;
	}

	public bool CanReach(IApproachable approachable)
	{
		if ((bool)navigator)
		{
			return navigator.CanReach(approachable);
		}
		if (consumerState.hasSolidTransferArm)
		{
			int cell = approachable.GetCell();
			return consumerState.solidTransferArm.IsCellReachable(cell);
		}
		return false;
	}

	public bool IsWithinReach(IApproachable approachable)
	{
		if ((bool)navigator)
		{
			if ((UnityEngine.Object)this == (UnityEngine.Object)null || (UnityEngine.Object)base.gameObject == (UnityEngine.Object)null)
			{
				return false;
			}
			return Grid.IsCellOffsetOf(Grid.PosToCell(this), approachable.GetCell(), approachable.GetOffsets());
		}
		if (consumerState.hasSolidTransferArm)
		{
			return consumerState.solidTransferArm.IsCellReachable(approachable.GetCell());
		}
		return false;
	}

	public void ShowHoverTextOnHoveredItem(Chore.Precondition.Context context, KSelectable hover_obj, HoverTextDrawer drawer, SelectToolHoverTextCard hover_text_card)
	{
		if (!context.chore.target.isNull && !((UnityEngine.Object)context.chore.target.gameObject != (UnityEngine.Object)hover_obj.gameObject))
		{
			drawer.NewLine(26);
			drawer.AddIndent(36);
			drawer.DrawText(context.chore.choreType.Name, hover_text_card.Styles_BodyText.Standard);
			if (!context.IsSuccess())
			{
				Chore.PreconditionInstance preconditionInstance = context.chore.GetPreconditions()[context.failedPreconditionId];
				string text = preconditionInstance.description;
				if (string.IsNullOrEmpty(text))
				{
					text = preconditionInstance.id;
				}
				if ((UnityEngine.Object)context.chore.driver != (UnityEngine.Object)null)
				{
					text = text.Replace("{Assignee}", context.chore.driver.GetProperName());
				}
				text = text.Replace("{Selected}", this.GetProperName());
				drawer.DrawText(" (" + text + ")", hover_text_card.Styles_BodyText.Standard);
			}
		}
	}

	public void ShowHoverTextOnHoveredItem(KSelectable hover_obj, HoverTextDrawer drawer, SelectToolHoverTextCard hover_text_card)
	{
		bool flag = false;
		foreach (Chore.Precondition.Context succeededContext in preconditionSnapshot.succeededContexts)
		{
			Chore.Precondition.Context current = succeededContext;
			if (current.chore.showAvailabilityInHoverText && !current.chore.target.isNull && !((UnityEngine.Object)current.chore.target.gameObject != (UnityEngine.Object)hover_obj.gameObject))
			{
				if (!flag)
				{
					drawer.NewLine(26);
					drawer.DrawText(DUPLICANTS.CHORES.PRECONDITIONS.HEADER.ToString().Replace("{Selected}", this.GetProperName()), hover_text_card.Styles_BodyText.Standard);
					flag = true;
				}
				ShowHoverTextOnHoveredItem(current, hover_obj, drawer, hover_text_card);
			}
		}
		foreach (Chore.Precondition.Context failedContext in preconditionSnapshot.failedContexts)
		{
			Chore.Precondition.Context current2 = failedContext;
			if (current2.chore.showAvailabilityInHoverText && !current2.chore.target.isNull && !((UnityEngine.Object)current2.chore.target.gameObject != (UnityEngine.Object)hover_obj.gameObject))
			{
				if (!flag)
				{
					drawer.NewLine(26);
					drawer.DrawText(DUPLICANTS.CHORES.PRECONDITIONS.HEADER.ToString().Replace("{Selected}", this.GetProperName()), hover_text_card.Styles_BodyText.Standard);
					flag = true;
				}
				ShowHoverTextOnHoveredItem(current2, hover_obj, drawer, hover_text_card);
			}
		}
	}

	public int GetPersonalPriority(ChoreType chore_type)
	{
		if (!choreTypePriorities.TryGetValue(chore_type.IdHash, out int value))
		{
			value = 3;
		}
		value = Mathf.Clamp(value, 0, 5);
		return value;
	}

	public int GetPersonalPriority(ChoreGroup group, out bool auto_assigned)
	{
		int value = 3;
		auto_assigned = false;
		if (choreGroupPriorities.TryGetValue(group.IdHash, out PriorityInfo value2))
		{
			auto_assigned = value2.wasAutoAssigned;
			value = value2.priority;
		}
		return Mathf.Clamp(value, 0, 5);
	}

	public int GetPriorityBeforeAutoAssignment(ChoreGroup group)
	{
		int value = 3;
		if (choreGroupPriorities.TryGetValue(group.IdHash, out PriorityInfo value2))
		{
			value = value2.priorityWhenAutoAssigned;
		}
		return Mathf.Clamp(value, 0, 5);
	}

	public void SetPersonalPriority(ChoreGroup group, int value, bool auto_assigned)
	{
		if (group.choreTypes != null)
		{
			value = Mathf.Clamp(value, 0, 5);
			if (!choreGroupPriorities.TryGetValue(group.IdHash, out PriorityInfo value2))
			{
				value2.priority = 3;
			}
			choreGroupPriorities[group.IdHash] = new PriorityInfo
			{
				priority = value,
				wasAutoAssigned = auto_assigned,
				priorityWhenAutoAssigned = ((!auto_assigned) ? (-1) : value2.priority)
			};
			UpdateChoreTypePriorities(group, value);
			SetPermittedByUser(group, value != 0);
		}
	}

	public int GetAssociatedSkillLevel(ChoreGroup group)
	{
		Klei.AI.Attributes attributes = this.GetAttributes();
		float value = attributes.GetValue(group.attribute.Id);
		return (int)value;
	}

	public bool CanRoleManageChoreGroup(ChoreGroup group)
	{
		bool result = false;
		MinionResume component = GetComponent<MinionResume>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			RoleConfig role = Game.Instance.roleManager.GetRole(component.CurrentRole);
			RoleGroup value;
			if (role != null && Game.Instance.roleManager.RoleGroups.TryGetValue(role.roleGroup, out value) && group.Id == value.choreGroupID)
			{
				result = true;
			}
		}
		return result;
	}

	private void UpdateChoreTypePriorities(ChoreGroup group, int value)
	{
		ChoreGroups choreGroups = Db.Get().ChoreGroups;
		foreach (ChoreType choreType in group.choreTypes)
		{
			int num = 0;
			foreach (ChoreGroup resource in choreGroups.resources)
			{
				if (resource.choreTypes != null)
				{
					foreach (ChoreType choreType2 in resource.choreTypes)
					{
						if (choreType2.IdHash == choreType.IdHash)
						{
							bool auto_assigned;
							int personalPriority = GetPersonalPriority(resource, out auto_assigned);
							num = Mathf.Max(num, personalPriority);
						}
					}
				}
			}
			choreTypePriorities[choreType.IdHash] = num;
		}
	}

	public void ResetPersonalPriorities()
	{
	}

	public bool RunBehaviourPrecondition(Tag tag)
	{
		BehaviourPrecondition value = default(BehaviourPrecondition);
		if (!behaviourPreconditions.TryGetValue(tag, out value))
		{
			return false;
		}
		return value.cb(value.arg);
	}

	public void AddBehaviourPrecondition(Tag tag, Func<object, bool> precondition, object arg)
	{
		DebugUtil.Assert(!behaviourPreconditions.ContainsKey(tag), "Assert!", string.Empty, string.Empty);
		behaviourPreconditions[tag] = new BehaviourPrecondition
		{
			cb = precondition,
			arg = arg
		};
	}

	public void RemoveBehaviourPrecondition(Tag tag, Func<object, bool> precondition, object arg)
	{
		behaviourPreconditions.Remove(tag);
	}

	public bool IsChoreEqualOrAboveCurrentChorePriority<StateMachineType>()
	{
		Chore currentChore = choreDriver.GetCurrentChore();
		if (currentChore == null)
		{
			return true;
		}
		return currentChore.choreType.priority <= choreTable.GetChorePriority<StateMachineType>(this);
	}

	public bool IsChoreGroupDisabled(ChoreGroup chore_group)
	{
		bool result = false;
		Traits component = base.gameObject.GetComponent<Traits>();
		foreach (Trait trait in component.TraitList)
		{
			if (trait.disabledChoreGroups != null)
			{
				ChoreGroup[] disabledChoreGroups = trait.disabledChoreGroups;
				foreach (ChoreGroup choreGroup in disabledChoreGroups)
				{
					if (choreGroup.IdHash == chore_group.IdHash)
					{
						result = true;
						break;
					}
				}
			}
		}
		return result;
	}

	public Dictionary<HashedString, PriorityInfo> GetChoreGroupPriorities()
	{
		return choreGroupPriorities;
	}

	public void SetChoreGroupPriorities(Dictionary<HashedString, PriorityInfo> priorities)
	{
		choreGroupPriorities = priorities;
	}
}
