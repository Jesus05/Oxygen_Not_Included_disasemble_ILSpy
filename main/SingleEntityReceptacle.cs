using KSerialization;
using System.Collections.Generic;
using UnityEngine;

public class SingleEntityReceptacle : Workable, IRender1000ms
{
	public enum ReceptacleDirection
	{
		Top,
		Side,
		Bottom
	}

	[MyCmpReq]
	protected Operational operational;

	[MyCmpReq]
	protected Storage storage;

	[MyCmpGet]
	public Rotatable rotatable;

	protected FetchChore fetchChore;

	protected bool autoReplaceEntity;

	[Serialize]
	public Tag requestedEntityTag;

	[Serialize]
	private Ref<KSelectable> occupyObjectRef = new Ref<KSelectable>();

	[SerializeField]
	private List<Tag> possibleDepositTagsList = new List<Tag>();

	[SerializeField]
	protected bool destroyEntityOnDeposit;

	[SerializeField]
	protected ReceptacleDirection direction;

	public Vector3 occupyingObjectRelativePosition = new Vector3(0f, 1f, 3f);

	protected StatusItem statusItemAwaitingDelivery;

	protected StatusItem statusItemNeed;

	protected StatusItem statusItemNoneAvailable;

	private static readonly EventSystem.IntraObjectHandler<SingleEntityReceptacle> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SingleEntityReceptacle>(delegate(SingleEntityReceptacle component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public FetchChore GetActiveRequest => fetchChore;

	public bool AutoReplaceEntity => autoReplaceEntity;

	protected GameObject occupyingObject
	{
		get
		{
			if ((Object)occupyObjectRef.Get() != (Object)null)
			{
				return occupyObjectRef.Get().gameObject;
			}
			return null;
		}
		set
		{
			if ((Object)value == (Object)null)
			{
				occupyObjectRef.Set(null);
			}
			else
			{
				occupyObjectRef.Set(value.GetComponent<KSelectable>());
			}
		}
	}

	public GameObject Occupant => occupyingObject;

	public Tag[] possibleDepositObjectTags => possibleDepositTagsList.ToArray();

	public ReceptacleDirection Direction => direction;

	public void ToggleAutoReplace()
	{
		autoReplaceEntity = !autoReplaceEntity;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if ((Object)occupyingObject != (Object)null)
		{
			PositionOccupyingObject();
			SubscribeToOccupant();
		}
		UpdateStatusItem();
		if ((Object)occupyingObject == (Object)null && requestedEntityTag.IsValid)
		{
			CreateOrder(requestedEntityTag);
		}
		Subscribe(-592767678, OnOperationalChangedDelegate);
	}

	public void AddDepositTag(Tag t)
	{
		possibleDepositTagsList.Add(t);
	}

	public void SetReceptacleDirection(ReceptacleDirection d)
	{
		direction = d;
	}

	public virtual void SetPreview(Tag entityTag, bool solid = false)
	{
	}

	public virtual void CreateOrder(Tag entityTag)
	{
		requestedEntityTag = entityTag;
		CreateFetchChore(requestedEntityTag);
		SetPreview(entityTag, true);
		UpdateStatusItem();
	}

	public void Render1000ms(float dt)
	{
		UpdateStatusItem();
	}

	protected void UpdateStatusItem()
	{
		KSelectable component = GetComponent<KSelectable>();
		if ((Object)Occupant != (Object)null)
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, null, null);
		}
		else if (fetchChore != null)
		{
			bool flag = (Object)fetchChore.fetcher != (Object)null;
			if (!flag)
			{
				Tag[] tags = fetchChore.tags;
				foreach (Tag tag in tags)
				{
					if (WorldInventory.Instance.GetTotalAmount(tag) > 0f)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, statusItemAwaitingDelivery, null);
			}
			else
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, statusItemNoneAvailable, null);
			}
		}
		else
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, statusItemNeed, null);
		}
	}

	protected void CreateFetchChore(Tag entityTag)
	{
		if (fetchChore == null && entityTag.IsValid && entityTag != GameTags.Empty)
		{
			fetchChore = new FetchChore(Db.Get().ChoreTypes.FarmFetch, storage, 1f, new Tag[1]
			{
				entityTag
			}, null, null, null, true, OnFetchComplete, delegate
			{
				UpdateStatusItem();
			}, delegate
			{
				UpdateStatusItem();
			}, FetchOrder2.OperationalRequirement.Functional, 0, GameTags.ChoreTypes.FarmingChores);
			MaterialNeeds.Instance.UpdateNeed(requestedEntityTag, 1f);
			UpdateStatusItem();
		}
	}

	public virtual void OrderRemoveOccupant()
	{
		ClearOccupant();
	}

	protected virtual void ClearOccupant()
	{
		if ((bool)occupyingObject)
		{
			storage.DropAll(false);
		}
		occupyingObject = null;
		UpdateActive();
		UpdateStatusItem();
		Trigger(-731304873, occupyingObject);
	}

	public void CancelActiveRequest()
	{
		if (fetchChore != null)
		{
			MaterialNeeds.Instance.UpdateNeed(requestedEntityTag, -1f);
			fetchChore.Cancel("User canceled");
			fetchChore = null;
		}
		requestedEntityTag = Tag.Invalid;
		UpdateStatusItem();
		SetPreview(Tag.Invalid, false);
	}

	private void ClearOccupantEventHandler(object data)
	{
		ClearOccupant();
		if (autoReplaceEntity && requestedEntityTag.IsValid && requestedEntityTag != GameTags.Empty)
		{
			CreateOrder(requestedEntityTag);
		}
	}

	protected virtual void SubscribeToOccupant()
	{
		if ((Object)occupyingObject != (Object)null)
		{
			Subscribe(occupyingObject, 1969584890, ClearOccupantEventHandler);
		}
	}

	protected virtual void UnsubscribeFromOccupant()
	{
		if ((Object)occupyingObject != (Object)null)
		{
			Unsubscribe(occupyingObject, 1969584890, ClearOccupantEventHandler);
		}
	}

	private void OnFetchComplete(Chore chore)
	{
		SetPreview(Tag.Invalid, false);
		Pickupable fetchTarget = fetchChore.fetchTarget;
		MaterialNeeds.Instance.UpdateNeed(requestedEntityTag, -1f);
		KBatchedAnimController component = fetchTarget.GetComponent<KBatchedAnimController>();
		if ((Object)component != (Object)null)
		{
			component.GetBatchInstanceData().ClearOverrideTransformMatrix();
		}
		occupyingObject = SpawnOccupyingObject(fetchTarget.gameObject);
		if ((Object)occupyingObject != (Object)null)
		{
			occupyingObject.SetActive(true);
			PositionOccupyingObject();
			SubscribeToOccupant();
		}
		else
		{
			Debug.LogWarning(base.gameObject.name + " EntityReceptacle did not spawn occupying entity.", null);
		}
		fetchChore = null;
		if (!autoReplaceEntity)
		{
			requestedEntityTag = Tag.Invalid;
		}
		UpdateActive();
		UpdateStatusItem();
		if (destroyEntityOnDeposit)
		{
			Util.KDestroyGameObject(fetchTarget.gameObject);
		}
		Trigger(-731304873, occupyingObject);
	}

	public virtual GameObject SpawnOccupyingObject(GameObject depositedEntity)
	{
		return depositedEntity;
	}

	protected virtual void PositionOccupyingObject()
	{
		occupyingObject.transform.SetParent(base.gameObject.transform, false);
		if ((Object)rotatable != (Object)null)
		{
			occupyingObject.transform.SetLocalPosition(rotatable.GetRotatedOffset(occupyingObjectRelativePosition));
		}
		else
		{
			occupyingObject.transform.SetLocalPosition(occupyingObjectRelativePosition);
		}
	}

	private void UpdateActive()
	{
		if (!Equals(null) && !((Object)this == (Object)null) && !base.gameObject.Equals(null) && !((Object)base.gameObject == (Object)null))
		{
			operational.SetActive(operational.IsOperational && (Object)occupyingObject != (Object)null, false);
		}
	}

	protected override void OnCleanUp()
	{
		CancelActiveRequest();
		UnsubscribeFromOccupant();
		base.OnCleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		UpdateActive();
		if ((bool)occupyingObject)
		{
			occupyingObject.Trigger((!operational.IsOperational) ? 960378201 : 1628751838, null);
		}
	}
}
