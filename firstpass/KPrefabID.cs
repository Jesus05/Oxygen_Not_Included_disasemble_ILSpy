using KSerialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class KPrefabID : KMonoBehaviour, ISaveLoadable
{
	public delegate void PrefabFn(GameObject go);

	public const int InvalidInstanceID = -1;

	private static int nextUniqueID = 0;

	[ReadOnly]
	public Tag SaveLoadTag;

	public Tag PrefabTag;

	private TagBits tagBits;

	private bool dirtyTagBits = true;

	[Serialize]
	public int InstanceID;

	public int defaultLayer;

	public List<Descriptor> AdditionalRequirements;

	public List<Descriptor> AdditionalEffects;

	private HashSet<Tag> tags = new HashSet<Tag>();

	private static readonly EventSystem.IntraObjectHandler<KPrefabID> OnObjectDestroyedDelegate = new EventSystem.IntraObjectHandler<KPrefabID>(delegate(KPrefabID component, object data)
	{
		component.OnObjectDestroyed(data);
	});

	public static int NextUniqueID
	{
		get
		{
			return nextUniqueID;
		}
		set
		{
			nextUniqueID = value;
		}
	}

	public bool pendingDestruction
	{
		get;
		private set;
	}

	public bool conflicted
	{
		get;
		private set;
	}

	public HashSet<Tag> Tags
	{
		get
		{
			InitializeTags();
			return tags;
		}
	}

	public event PrefabFn instantiateFn;

	public event PrefabFn prefabInitFn;

	public event PrefabFn prefabSpawnFn;

	public void CopyTags(KPrefabID other)
	{
		foreach (Tag tag in other.tags)
		{
			tags.Add(tag);
		}
	}

	public void CopyInitFunctions(KPrefabID other)
	{
		this.instantiateFn = other.instantiateFn;
		this.prefabInitFn = other.prefabInitFn;
		this.prefabSpawnFn = other.prefabSpawnFn;
	}

	public void RunInstantiateFn()
	{
		if (this.instantiateFn != null)
		{
			this.instantiateFn(base.gameObject);
			this.instantiateFn = null;
		}
	}

	public void InitializeTags()
	{
		DebugUtil.Assert(PrefabTag.IsValid);
		if (tags.Add(PrefabTag))
		{
			dirtyTagBits = true;
		}
	}

	public void UpdateSaveLoadTag()
	{
		SaveLoadTag = new Tag(PrefabTag.Name);
	}

	public Tag GetSaveLoadTag()
	{
		return SaveLoadTag;
	}

	private void LaunderTagBits()
	{
		if (dirtyTagBits)
		{
			tagBits.ClearAll();
			foreach (Tag tag in tags)
			{
				tagBits.SetTag(tag);
			}
			dirtyTagBits = false;
		}
	}

	public void UpdateTagBits()
	{
		InitializeTags();
		LaunderTagBits();
	}

	public void AndTagBits(ref TagBits rhs)
	{
		UpdateTagBits();
		rhs.And(ref tagBits);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(1969584890, OnObjectDestroyedDelegate);
		InitializeTags();
		if (this.prefabInitFn != null)
		{
			this.prefabInitFn(base.gameObject);
			this.prefabInitFn = null;
		}
		GetComponent<IStateMachineControllerHack>()?.CreateSMIS();
	}

	protected override void OnSpawn()
	{
		GetComponent<IStateMachineControllerHack>()?.StartSMIS();
		if (this.prefabSpawnFn != null)
		{
			this.prefabSpawnFn(base.gameObject);
			this.prefabSpawnFn = null;
		}
	}

	public void AddTag(Tag tag)
	{
		DebugUtil.Assert(tag.IsValid);
		if (Tags.Add(tag))
		{
			dirtyTagBits = true;
			Trigger(-1582839653, null);
		}
	}

	public void RemoveTag(Tag tag)
	{
		if (Tags.Remove(tag))
		{
			dirtyTagBits = true;
			Trigger(-1582839653, null);
		}
	}

	public void SetTag(Tag tag, bool set)
	{
		if (set)
		{
			AddTag(tag);
		}
		else
		{
			RemoveTag(tag);
		}
	}

	public bool HasTag(Tag tag)
	{
		return Tags.Contains(tag);
	}

	public bool HasAnyTags(List<Tag> search_tags)
	{
		InitializeTags();
		foreach (Tag search_tag in search_tags)
		{
			if (tags.Contains(search_tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyTags(Tag[] search_tags)
	{
		InitializeTags();
		foreach (Tag item in search_tags)
		{
			if (tags.Contains(item))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyTags(ref TagBits search_tags)
	{
		UpdateTagBits();
		return tagBits.HasAny(ref search_tags);
	}

	public bool HasAllTags(ref TagBits search_tags)
	{
		UpdateTagBits();
		return tagBits.HasAll(ref search_tags);
	}

	public bool HasAnyTags_AssumeLaundered(ref TagBits search_tags)
	{
		return tagBits.HasAny(ref search_tags);
	}

	public bool HasAllTags_AssumeLaundered(ref TagBits search_tags)
	{
		return tagBits.HasAll(ref search_tags);
	}

	public override bool Equals(object o)
	{
		KPrefabID kPrefabID = o as KPrefabID;
		return (Object)kPrefabID != (Object)null && PrefabTag == kPrefabID.PrefabTag;
	}

	public override int GetHashCode()
	{
		return PrefabTag.GetHashCode();
	}

	public static int GetUniqueID()
	{
		return NextUniqueID++;
	}

	public string GetDebugName()
	{
		return base.name + "(" + InstanceID + ")";
	}

	protected override void OnCleanUp()
	{
		pendingDestruction = true;
		if (InstanceID != -1)
		{
			KPrefabIDTracker.Get().Unregister(this);
		}
		Trigger(1969584890, null);
	}

	[OnDeserialized]
	internal void OnDeserializedMethod()
	{
		KPrefabIDTracker kPrefabIDTracker = KPrefabIDTracker.Get();
		KPrefabID instance = kPrefabIDTracker.GetInstance(InstanceID);
		if ((bool)instance)
		{
			conflicted = true;
		}
		kPrefabIDTracker.Register(this);
	}

	private void OnObjectDestroyed(object data)
	{
		pendingDestruction = true;
	}
}
