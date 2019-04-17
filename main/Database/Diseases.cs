using Klei.AI;

namespace Database
{
	public class Diseases : ResourceSet<Disease>
	{
		public Disease FoodGerms;

		public Disease SlimeGerms;

		public Disease PollenGerms;

		public Disease ZombieSpores;

		public Diseases(ResourceSet parent)
			: base("Diseases", parent)
		{
			FoodGerms = Add(new FoodGerms());
			SlimeGerms = Add(new SlimeGerms());
			PollenGerms = Add(new PollenGerms());
			ZombieSpores = Add(new ZombieSpores());
		}

		public static bool IsValidID(string id)
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
