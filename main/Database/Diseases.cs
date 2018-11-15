using Klei.AI;

namespace Database
{
	public class Diseases : ResourceSet<Disease>
	{
		public Disease Dweebcephaly;

		public Disease Lazibonitis;

		public Disease FoodPoisoning;

		public Disease PutridOdour;

		public Disease Spores;

		public Disease ColdBrain;

		public Disease HeatRash;

		public Disease SlimeLung;

		public Disease Sunburn;

		public Diseases(ResourceSet parent)
			: base("Diseases", parent)
		{
			FoodPoisoning = Add(new FoodPoisoning());
			ColdBrain = Add(new ColdBrain());
			HeatRash = Add(new HeatRash());
			SlimeLung = Add(new SlimeLung());
			Sunburn = Add(new Sunburn());
			PutridOdour = Add(new PutridOdour());
			PutridOdour.Disabled = true;
			Spores = Add(new Spores());
			Spores.Disabled = true;
		}

		public static bool IsValidDiseaseID(string id)
		{
			bool result = false;
			foreach (Disease resource in Db.Get().Diseases.resources)
			{
				if (resource.Id == id)
				{
					result = true;
				}
			}
			return result;
		}

		public byte GetIndex(int hash)
		{
			Diseases diseases = Db.Get().Diseases;
			for (byte b = 0; b < diseases.Count; b = (byte)(b + 1))
			{
				Disease disease = diseases[b];
				if (hash == disease.id.GetHashCode())
				{
					return b;
				}
			}
			return byte.MaxValue;
		}

		public byte GetIndex(HashedString id)
		{
			return GetIndex(id.GetHashCode());
		}
	}
}
