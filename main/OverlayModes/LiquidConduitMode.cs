namespace OverlayModes
{
	public class LiquidConduitMode : ConduitMode
	{
		public LiquidConduitMode()
			: base(OverlayScreen.LiquidVentIDs)
		{
		}

		public override SimViewMode ViewMode()
		{
			return SimViewMode.LiquidVentMap;
		}

		public override string GetSoundName()
		{
			return "LiquidVent";
		}
	}
}
