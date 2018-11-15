public class LightShapePreview : KMonoBehaviour
{
	public float radius;

	public int lux;

	public LightShape shape;

	public CellOffset offset;

	private int previousCell = -1;

	private void Update()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		if (num != previousCell)
		{
			previousCell = num;
			LightGridManager.DestroyPreview();
			LightGridManager.CreatePreview(Grid.OffsetCell(num, offset), radius, shape, lux);
		}
	}

	protected override void OnCleanUp()
	{
		LightGridManager.DestroyPreview();
	}
}
