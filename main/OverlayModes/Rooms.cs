namespace OverlayModes
{
	public class Rooms : Mode
	{
		public override SimViewMode ViewMode()
		{
			return SimViewMode.Rooms;
		}

		public override string GetSoundName()
		{
			return "Rooms";
		}
	}
}
