namespace Database
{
	public class StatusItemCategories : ResourceSet<StatusItemCategory>
	{
		public StatusItemCategory Main;

		public StatusItemCategory Role;

		public StatusItemCategory Power;

		public StatusItemCategory Toilet;

		public StatusItemCategory Research;

		public StatusItemCategory Hitpoints;

		public StatusItemCategory Suffocation;

		public StatusItemCategory WoundEffects;

		public StatusItemCategory EntityReceptacle;

		public StatusItemCategory PreservationState;

		public StatusItemCategory PreservationAtmosphere;

		public StatusItemCategory PreservationTemperature;

		public StatusItemCategory ExhaustTemperature;

		public StatusItemCategory OperatingEnergy;

		public StatusItemCategory AccessControl;

		public StatusItemCategory RequiredRoom;

		public StatusItemCategory Yield;

		public StatusItemCategory Sleep;

		public StatusItemCategory Heat;

		public StatusItemCategories(ResourceSet parent)
			: base("StatusItemCategories", parent)
		{
			Main = new StatusItemCategory("Main", this, "Main");
			Role = new StatusItemCategory("Role", this, "Role");
			Power = new StatusItemCategory("Power", this, "Power");
			Toilet = new StatusItemCategory("Toilet", this, "Toilet");
			Research = new StatusItemCategory("Research", this, "Research");
			Hitpoints = new StatusItemCategory("Hitpoints", this, "Hitpoints");
			Suffocation = new StatusItemCategory("Suffocation", this, "Suffocation");
			WoundEffects = new StatusItemCategory("WoundEffects", this, "WoundEffects");
			EntityReceptacle = new StatusItemCategory("EntityReceptacle", this, "EntityReceptacle");
			PreservationState = new StatusItemCategory("PreservationState", this, "PreservationState");
			PreservationTemperature = new StatusItemCategory("PreservationTemperature", this, "PreservationTemperature");
			PreservationAtmosphere = new StatusItemCategory("PreservationAtmosphere", this, "PreservationAtmosphere");
			ExhaustTemperature = new StatusItemCategory("ExhaustTemperature", this, "ExhaustTemperature");
			OperatingEnergy = new StatusItemCategory("OperatingEnergy", this, "OperatingEnergy");
			AccessControl = new StatusItemCategory("AccessControl", this, "AccessControl");
			RequiredRoom = new StatusItemCategory("RequiredRoom", this, "RequiredRoom");
			Yield = new StatusItemCategory("Yield", this, "Yield");
			Sleep = new StatusItemCategory("Sleep", this, "Sleep");
			Heat = new StatusItemCategory("Heat", this, "Heat");
		}
	}
}
