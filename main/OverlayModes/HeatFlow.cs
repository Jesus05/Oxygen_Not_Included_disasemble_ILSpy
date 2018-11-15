namespace OverlayModes
{
	public class HeatFlow : Mode
	{
		public override SimViewMode ViewMode()
		{
			return SimViewMode.HeatFlow;
		}

		public override string GetSoundName()
		{
			return "HeatFlow";
		}
	}
}
