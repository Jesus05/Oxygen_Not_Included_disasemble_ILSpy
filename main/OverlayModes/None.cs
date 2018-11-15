namespace OverlayModes
{
	public class None : Mode
	{
		public override SimViewMode ViewMode()
		{
			return SimViewMode.None;
		}

		public override string GetSoundName()
		{
			return "Off";
		}
	}
}
