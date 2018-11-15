[SkipSaveFileSerialization]
public class Floodable : KMonoBehaviour
{
	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpGet]
	private SimCellOccupier simCellOccupier;

	[MyCmpReq]
	private Operational operational;

	public static Operational.Flag notFloodedFlag = new Operational.Flag("not_flooded", Operational.Flag.Type.Functional);

	private bool isFlooded;

	private HandleVector<int>.Handle partitionerEntry;

	public bool IsFlooded => isFlooded;

	public BuildingDef Def => building.Def;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		partitionerEntry = GameScenePartitioner.Instance.Add("Floodable.OnSpawn", base.gameObject, building.GetExtents(), GameScenePartitioner.Instance.liquidChangedLayer, OnElementChanged);
		OnElementChanged(null);
	}

	private void OnElementChanged(object data)
	{
		bool flag = false;
		for (int i = 0; i < building.PlacementCells.Length; i++)
		{
			int cell = building.PlacementCells[i];
			if (Grid.IsSubstantialLiquid(cell, 0.35f))
			{
				flag = true;
				break;
			}
		}
		if (flag != isFlooded)
		{
			isFlooded = flag;
			operational.SetFlag(notFloodedFlag, !isFlooded);
			GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.Flooded, isFlooded, this);
		}
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
	}
}
