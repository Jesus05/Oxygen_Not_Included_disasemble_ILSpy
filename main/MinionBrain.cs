using ProcGen;
using UnityEngine;

public class MinionBrain : Brain
{
	[MyCmpReq]
	public Navigator Navigator;

	[MyCmpGet]
	public OxygenBreather OxygenBreather;

	private float lastResearchCompleteEmoteTime;

	private static readonly EventSystem.IntraObjectHandler<MinionBrain> AnimTrackStoredItemDelegate = new EventSystem.IntraObjectHandler<MinionBrain>(delegate(MinionBrain component, object data)
	{
		component.AnimTrackStoredItem(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MinionBrain> OnUnstableGroundImpactDelegate = new EventSystem.IntraObjectHandler<MinionBrain>(delegate(MinionBrain component, object data)
	{
		component.OnUnstableGroundImpact(data);
	});

	public bool IsCellClear(int cell)
	{
		if (!Grid.Reserved[cell])
		{
			GameObject gameObject = Grid.Objects[cell, 0];
			if (!((Object)gameObject != (Object)null) || !((Object)base.gameObject != (Object)gameObject) || gameObject.GetComponent<Navigator>().IsMoving())
			{
				return true;
			}
			return false;
		}
		return false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Navigator.SetAbilities(new MinionPathFinderAbilities(Navigator));
		Subscribe(-1697596308, AnimTrackStoredItemDelegate);
		Subscribe(-975551167, OnUnstableGroundImpactDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Storage component = GetComponent<Storage>();
		foreach (GameObject item in component.items)
		{
			AddAnimTracker(item);
		}
		Game.Instance.Subscribe(-107300940, OnResearchComplete);
	}

	private void AnimTrackStoredItem(object data)
	{
		Storage component = GetComponent<Storage>();
		GameObject gameObject = (GameObject)data;
		RemoveTracker(gameObject);
		if (component.items.Contains(gameObject))
		{
			AddAnimTracker(gameObject);
		}
	}

	private void AddAnimTracker(GameObject go)
	{
		KAnimControllerBase component = go.GetComponent<KAnimControllerBase>();
		if (!((Object)component == (Object)null) && component.AnimFiles != null && component.AnimFiles.Length > 0 && (Object)component.AnimFiles[0] != (Object)null && component.GetComponent<Pickupable>().trackOnPickup)
		{
			KBatchedAnimTracker kBatchedAnimTracker = go.AddComponent<KBatchedAnimTracker>();
			kBatchedAnimTracker.useTargetPoint = false;
			kBatchedAnimTracker.fadeOut = false;
			kBatchedAnimTracker.symbol = new HashedString("snapTo_chest");
			kBatchedAnimTracker.forceAlwaysVisible = true;
		}
	}

	private void RemoveTracker(GameObject go)
	{
		KBatchedAnimTracker component = go.GetComponent<KBatchedAnimTracker>();
		if ((Object)component != (Object)null)
		{
			Object.Destroy(component);
		}
	}

	public override void UpdateBrain()
	{
		base.UpdateBrain();
		if (!((Object)Game.Instance == (Object)null) && !Game.Instance.savedInfo.discoveredSurface)
		{
			int cell = Grid.PosToCell(base.gameObject);
			SubWorld.ZoneType subWorldZoneType = World.Instance.zoneRenderData.GetSubWorldZoneType(cell);
			if (subWorldZoneType == SubWorld.ZoneType.Space)
			{
				Game.Instance.savedInfo.discoveredSurface = true;
				Vector3 position = base.gameObject.transform.GetPosition();
				DiscoveredSpaceMessage message = new DiscoveredSpaceMessage(position);
				Messenger.Instance.QueueMessage(message);
				Game.Instance.Trigger(-818188514, base.gameObject);
			}
		}
	}

	private void RegisterReactEmotePair(string reactable_id, string kanim_file_name, float max_trigger_time)
	{
		if (!((Object)base.gameObject == (Object)null))
		{
			ReactionMonitor.Instance sMI = base.gameObject.GetSMI<ReactionMonitor.Instance>();
			if (sMI != null)
			{
				EmoteChore emoteChore = new EmoteChore(base.gameObject.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteIdle, kanim_file_name, new HashedString[1]
				{
					"react"
				}, null);
				SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(base.gameObject, reactable_id, Db.Get().ChoreTypes.Cough, kanim_file_name, max_trigger_time, 20f, float.PositiveInfinity);
				emoteChore.PairReactable(selfEmoteReactable);
				selfEmoteReactable.AddStep(new EmoteReactable.EmoteStep
				{
					anim = (HashedString)"react"
				});
				selfEmoteReactable.PairEmote(emoteChore);
				sMI.AddOneshotReactable(selfEmoteReactable);
			}
		}
	}

	private void OnResearchComplete(object data)
	{
		if (Time.time - lastResearchCompleteEmoteTime > 1f)
		{
			RegisterReactEmotePair("ResearchComplete", "anim_react_research_complete_kanim", 3f);
			lastResearchCompleteEmoteTime = Time.time;
		}
	}

	private void OnUnstableGroundImpact(object data)
	{
		RegisterReactEmotePair("UnstableGroundShock", "anim_react_shock_kanim", 1f);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Game.Instance.Unsubscribe(-107300940, OnResearchComplete);
	}
}
