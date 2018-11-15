public class Painting : Artable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		multitoolContext = "paint";
		multitoolHitEffectTag = "fx_paint_splash";
	}
}
