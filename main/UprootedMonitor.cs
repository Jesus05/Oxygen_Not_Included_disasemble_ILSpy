using KSerialization;
using UnityEngine;

public class UprootedMonitor : KMonoBehaviour
{
	private int position;

	private int ground;

	[Serialize]
	public bool canBeUprooted = true;

	[Serialize]
	private bool uprooted = false;

	public CellOffset monitorCell = new CellOffset(0, -1);

	private HandleVector<int>.Handle partitionerEntry;

	private static readonly EventSystem.IntraObjectHandler<UprootedMonitor> OnUprootedDelegate = new EventSystem.IntraObjectHandler<UprootedMonitor>(delegate(UprootedMonitor component, object data)
	{
		if (!component.uprooted)
		{
			component.uprooted = true;
			component.Trigger(-216549700, null);
		}
	});

	public bool IsUprooted => uprooted;

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
		if (canBeUprooted)
		{
			if (!uprooted)
			{
				if (IsCellSafe(position))
				{
					return true;
				}
				return false;
			}
			return false;
		}
		return true;
	}

	public bool IsCellSafe(int cell)
	{
		return CreatureHelpers.isSolidGround(ground);
	}

	public void OnGroundChanged(object callbackData)
	{
		if (!CheckTileGrowable())
		{
			uprooted = true;
			Trigger(-216549700, null);
		}
	}

	public static bool IsObjectUprooted(GameObject plant)
	{
		UprootedMonitor component = plant.GetComponent<UprootedMonitor>();
		if (!((Object)component == (Object)null))
		{
			return component.IsUprooted;
		}
		return false;
	}
}
