public class BuildingGroupScreen : KScreen
{
	public static BuildingGroupScreen Instance;

	protected override void OnPrefabInit()
	{
		Instance = this;
		base.OnPrefabInit();
		ConsumeMouseScroll = true;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		ConsumeMouseScroll = true;
	}
}
