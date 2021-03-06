using KSerialization;
using UnityEngine;

public class UprootedMonitor : KMonoBehaviour
{
	private int position;

	private int ground;

	[Serialize]
	public bool canBeUprooted = true;

	[Serialize]
	private bool uprooted;

	public CellOffset monitorCell = new CellOffset(0, -1);

	private HandleVector<int>.Handle partitionerEntry;

	private static readonly EventSystem.IntraObjectHandler<UprootedMonitor> OnUprootedDelegate = new EventSystem.IntraObjectHandler<UprootedMonitor>(delegate(UprootedMonitor component, object data)
	{
		if (!component.uprooted)
		{
			component.GetComponent<KPrefabID>().AddTag(GameTags.Uprooted, false);
			component.uprooted = true;
			component.Trigger(-216549700, null);
		}
	});

	public bool IsUprooted => uprooted || GetComponent<KPrefabID>().HasTag(GameTags.Uprooted);

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-216549700, OnUprootedDelegate);
		position = Grid.PosToCell(base.gameObject);
		ground = Grid.OffsetCell(position, monitorCell);
		if (Grid.IsValidCell(position) && Grid.IsValidCell(ground))
		{
			partitionerEntry = GameScenePartitioner.Instance.Add("UprootedMonitor.OnSpawn", base.gameObject, ground, GameScenePartitioner.Instance.solidChangedLayer, OnGroundChanged);
		}
		OnGroundChanged(null);
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	public bool CheckTileGrowable()
	{
		if (!canBeUprooted)
		{
			return true;
		}
		if (uprooted)
		{
			return false;
		}
		if (!IsCellSafe(position))
		{
			return false;
		}
		return true;
	}

	public bool IsCellSafe(int cell)
	{
		if (!Grid.IsCellOffsetValid(cell, monitorCell))
		{
			return false;
		}
		int i = Grid.OffsetCell(cell, monitorCell);
		return Grid.Solid[i];
	}

	public void OnGroundChanged(object callbackData)
	{
		if (!CheckTileGrowable())
		{
			GetComponent<KPrefabID>().AddTag(GameTags.Uprooted, false);
			uprooted = true;
			Trigger(-216549700, null);
		}
	}

	public static bool IsObjectUprooted(GameObject plant)
	{
		UprootedMonitor component = plant.GetComponent<UprootedMonitor>();
		if ((Object)component == (Object)null)
		{
			return false;
		}
		return component.IsUprooted;
	}
}
