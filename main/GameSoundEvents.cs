public static class GameSoundEvents
{
	public class Event
	{
		public HashedString Name;

		public Event(string name)
		{
			Name = name;
		}
	}

	public static Event BatteryFull = new Event("game_triggered.battery_full");

	public static Event BatteryWarning = new Event("game_triggered.battery_warning");

	public static Event BatteryDischarged = new Event("game_triggered.battery_drained");
}
