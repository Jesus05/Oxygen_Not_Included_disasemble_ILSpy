namespace OverlayModes
{
	public class Priorities : Mode
	{
		public override SimViewMode ViewMode()
		{
			return SimViewMode.Priorities;
		}

		public override string GetSoundName()
		{
			return "Priorities";
		}
	}
}
