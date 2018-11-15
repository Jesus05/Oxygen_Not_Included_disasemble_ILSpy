using UnityEngine;

[SkipSaveFileSerialization]
public class SolidConduitConsumer : KMonoBehaviour
{
	[SerializeField]
	public Tag capacityTag = GameTags.Any;

	[SerializeField]
	public float capacityKG = float.PositiveInfinity;

	[SerializeField]
	public bool alwaysConsume;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	public Storage storage;

	private HandleVector<int>.Handle partitionerEntry;

	private int utilityCell = -1;

	private bool consuming;

	public bool IsConsuming => consuming;

	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[utilityCell, 20];
			return (Object)gameObject != (Object)null && (Object)gameObject.GetComponent<BuildingComplete>() != (Object)null;
		}
	}

	private SolidConduitFlow GetConduitFlow()
	{
		return Game.Instance.solidConduitFlow;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		utilityCell = building.GetUtilityInputCell();
		ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[20];
		partitionerEntry = GameScenePartitioner.Instance.Add("SolidConduitConsumer.OnSpawn", base.gameObject, utilityCell, layer, OnConduitConnectionChanged);
		GetConduitFlow().AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Default);
		OnConduitConnectionChanged(null);
	}

	protected override void OnCleanUp()
	{
		GetConduitFlow().RemoveConduitUpdater(ConduitUpdate);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	private void OnConduitConnectionChanged(object data)
	{
		consuming = (consuming && IsConnected);
		Trigger(-2094018600, IsConnected);
	}

	private void ConduitUpdate(float dt)
	{
		bool flag = false;
		SolidConduitFlow conduitFlow = GetConduitFlow();
		if (IsConnected)
		{
			SolidConduitFlow.ConduitContents contents = conduitFlow.GetContents(utilityCell);
			if (contents.pickupableHandle.IsValid() && (alwaysConsume || operational.IsOperational))
			{
				float num = (!(capacityTag != GameTags.Any)) ? storage.MassStored() : storage.GetMassAvailable(capacityTag);
				float num2 = Mathf.Min(storage.RemainingCapacity(), capacityKG - num);
				Pickupable pickupable = conduitFlow.GetPickupable(contents.pickupableHandle);
				if (pickupable.PrimaryElement.Mass <= num2)
				{
					Pickupable pickupable2 = conduitFlow.RemovePickupable(utilityCell);
					if ((bool)pickupable2)
					{
						storage.Store(pickupable2.gameObject, true, false, true, false);
						flag = true;
					}
				}
			}
		}
		storage.storageNetworkID = GetConnectedNetworkID();
		consuming = flag;
	}

	private int GetConnectedNetworkID()
	{
		GameObject gameObject = Grid.Objects[utilityCell, 20];
		SolidConduit solidConduit = (!((Object)gameObject != (Object)null)) ? null : gameObject.GetComponent<SolidConduit>();
		return ((!((Object)solidConduit != (Object)null)) ? null : solidConduit.GetNetwork())?.id ?? (-1);
	}
}
