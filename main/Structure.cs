[SkipSaveFileSerialization]
public class Structure : KMonoBehaviour
{
	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private Operational operational;

	public static readonly Operational.Flag notEntombedFlag = new Operational.Flag("not_entombed", Operational.Flag.Type.Functional);

	private bool isEntombed;

	private HandleVector<int>.Handle partitionerEntry;

	public bool IsEntombed()
	{
		return isEntombed;
	}

	public static bool IsBuildingEntombed(Building building)
	{
		for (int i = 0; i < building.PlacementCells.Length; i++)
		{
			int num = building.PlacementCells[i];
			if (Grid.Element[num].IsSolid && !Grid.Foundation[num])
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Extents extents = building.GetExtents();
		partitionerEntry = GameScenePartitioner.Instance.Add("Structure.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
		OnSolidChanged(null);
	}

	private void OnSolidChanged(object data)
	{
		bool flag = IsBuildingEntombed(building);
		if (flag != isEntombed)
		{
			isEntombed = flag;
			operational.SetFlag(notEntombedFlag, !isEntombed);
			GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.Entombed, isEntombed, this);
		}
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
	}
}
