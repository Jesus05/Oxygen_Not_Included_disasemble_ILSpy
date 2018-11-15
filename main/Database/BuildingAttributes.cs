using Klei.AI;

namespace Database
{
	public class BuildingAttributes : ResourceSet<Attribute>
	{
		public Attribute Decor;

		public Attribute DecorRadius;

		public Attribute NoisePollution;

		public Attribute NoisePollutionRadius;

		public Attribute Hygiene;

		public Attribute Comfort;

		public Attribute OverheatTemperature;

		public Attribute FatalTemperature;

		public BuildingAttributes(ResourceSet parent)
			: base("BuildingAttributes", parent)
		{
			Decor = Add(new Attribute("Decor", true, Attribute.Display.General, false, 0f, null, null));
			DecorRadius = Add(new Attribute("DecorRadius", true, Attribute.Display.General, false, 0f, null, null));
			NoisePollution = Add(new Attribute("NoisePollution", true, Attribute.Display.General, false, 0f, null, null));
			NoisePollutionRadius = Add(new Attribute("NoisePollutionRadius", true, Attribute.Display.General, false, 0f, null, null));
			Hygiene = Add(new Attribute("Hygiene", true, Attribute.Display.General, false, 0f, null, null));
			Comfort = Add(new Attribute("Comfort", true, Attribute.Display.General, false, 0f, null, null));
			OverheatTemperature = Add(new Attribute("OverheatTemperature", true, Attribute.Display.General, false, 0f, null, null));
			OverheatTemperature.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.ModifyOnly));
			FatalTemperature = Add(new Attribute("FatalTemperature", true, Attribute.Display.General, false, 0f, null, null));
			FatalTemperature.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.ModifyOnly));
		}
	}
}
