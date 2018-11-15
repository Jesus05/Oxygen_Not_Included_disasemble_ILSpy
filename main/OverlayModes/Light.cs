namespace OverlayModes
{
	public class Light : Mode
	{
		public override SimViewMode ViewMode()
		{
			return SimViewMode.Light;
		}

		public override string GetSoundName()
		{
			return "Lights";
		}
	}
}
