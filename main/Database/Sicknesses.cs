using Klei.AI;

namespace Database
{
	public class Sicknesses : ResourceSet<Sickness>
	{
		public Sickness FoodSickness;

		public Sickness SlimeSickness;

		public Sickness ZombieSickness;

		public Sickness Allergies;

		public Sickness ColdBrain;

		public Sickness HeatRash;

		public Sickness Sunburn;

		public Sicknesses(ResourceSet parent)
			: base("Sicknesses", parent)
		{
			FoodSickness = Add(new FoodSickness());
			SlimeSickness = Add(new SlimeSickness());
			ZombieSickness = Add(new ZombieSickness());
			Allergies = Add(new Allergies());
			ColdBrain = Add(new ColdBrain());
			HeatRash = Add(new HeatRash());
			Sunburn = Add(new Sunburn());
		}

		public static bool IsValidID(string id)
		{
			bool result = false;
			foreach (Sickness resource in Db.Get().Sicknesses.resources)
			{
				if (resource.Id == id)
				{
					result = true;
				}
			}
			return result;
		}
	}
}
