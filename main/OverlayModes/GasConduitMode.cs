namespace OverlayModes
{
	public class GasConduitMode : ConduitMode
	{
		public GasConduitMode()
			: base(OverlayScreen.GasVentIDs)
		{
		}

		public override SimViewMode ViewMode()
		{
			return SimViewMode.GasVentMap;
		}

		public override string GetSoundName()
		{
			return "GasVent";
		}
	}
}
