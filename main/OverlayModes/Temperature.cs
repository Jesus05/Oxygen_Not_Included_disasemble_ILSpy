namespace OverlayModes
{
	public class Temperature : Mode
	{
		public override SimViewMode ViewMode()
		{
			return SimViewMode.TemperatureMap;
		}

		public override string GetSoundName()
		{
			return "Temperature";
		}

		public override void Enable()
		{
			Infrared.Instance.SetMode(Infrared.Mode.Infrared);
			CameraController.Instance.ToggleColouredOverlayView(true);
			base.Enable();
		}

		public override void Disable()
		{
			Infrared.Instance.SetMode(Infrared.Mode.Disabled);
			CameraController.Instance.ToggleColouredOverlayView(false);
			base.Disable();
		}
	}
}
