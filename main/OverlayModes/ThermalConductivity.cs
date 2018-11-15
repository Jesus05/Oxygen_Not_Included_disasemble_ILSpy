namespace OverlayModes
{
	public class ThermalConductivity : Mode
	{
		public override SimViewMode ViewMode()
		{
			return SimViewMode.ThermalConductivity;
		}

		public override string GetSoundName()
		{
			return "HeatFlow";
		}
	}
}
