using System.Collections.Generic;

namespace Database
{
	public class ArtifactDropRate : Resource
	{
		public List<Tuple<ArtifactTier, float>> rates = new List<Tuple<ArtifactTier, float>>();

		public float totalWeight;

		public void AddItem(ArtifactTier tier, float weight)
		{
			rates.Add(new Tuple<ArtifactTier, float>(tier, weight));
			totalWeight += weight;
		}

		public float GetTierWeight(ArtifactTier tier)
		{
			float result = 0f;
			foreach (Tuple<ArtifactTier, float> rate in rates)
			{
				if (rate.first == tier)
				{
					result = rate.second;
				}
			}
			return result;
		}
	}
}
