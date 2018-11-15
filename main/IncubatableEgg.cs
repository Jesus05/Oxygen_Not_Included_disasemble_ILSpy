public class IncubatableEgg : KMonoBehaviour, IHasSortOrder
{
	public int sortOrder
	{
		get;
		set;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}
}
