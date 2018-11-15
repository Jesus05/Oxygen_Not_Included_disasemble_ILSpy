using Klei.AI;

namespace Database
{
	public class CritterAttributes : ResourceSet<Attribute>
	{
		public Attribute Happiness;

		public Attribute Metabolism;

		public CritterAttributes(ResourceSet parent)
			: base("CritterAttributes", parent)
		{
			Happiness = Add(new Attribute("Happiness", false, Attribute.Display.General, false, 0f, null, null));
			Metabolism = Add(new Attribute("Metabolism", false, Attribute.Display.Details, false, 0f, null, null));
			Metabolism.SetFormatter(new ToPercentAttributeFormatter(100f, GameUtil.TimeSlice.None));
		}
	}
}
