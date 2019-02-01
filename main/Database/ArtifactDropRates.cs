using TUNING;

namespace Database
{
	public class ArtifactDropRates : ResourceSet<ArtifactDropRate>
	{
		public ArtifactDropRate Bad;

		public ArtifactDropRate Mediocre;

		public ArtifactDropRate Good;

		public ArtifactDropRate Great;

		public ArtifactDropRate Amazing;

		public ArtifactDropRate Perfect;

		public ArtifactDropRates(ResourceSet parent)
			: base("ArtifactDropRates", parent)
		{
			CreateDropRates();
		}

		private void CreateDropRates()
		{
			Bad = new ArtifactDropRate();
			Bad.AddItem(DECOR.SPACEARTIFACT.TIER_NONE, 10f);
			Bad.AddItem(DECOR.SPACEARTIFACT.TIER0, 5f);
			Bad.AddItem(DECOR.SPACEARTIFACT.TIER1, 3f);
			Bad.AddItem(DECOR.SPACEARTIFACT.TIER2, 2f);
			Add(Bad);
			Mediocre = new ArtifactDropRate();
			Mediocre.AddItem(DECOR.SPACEARTIFACT.TIER_NONE, 10f);
			Mediocre.AddItem(DECOR.SPACEARTIFACT.TIER1, 5f);
			Mediocre.AddItem(DECOR.SPACEARTIFACT.TIER2, 3f);
			Mediocre.AddItem(DECOR.SPACEARTIFACT.TIER3, 2f);
			Add(Mediocre);
			Good = new ArtifactDropRate();
			Good.AddItem(DECOR.SPACEARTIFACT.TIER_NONE, 10f);
			Good.AddItem(DECOR.SPACEARTIFACT.TIER2, 5f);
			Good.AddItem(DECOR.SPACEARTIFACT.TIER3, 3f);
			Good.AddItem(DECOR.SPACEARTIFACT.TIER4, 2f);
			Add(Good);
			Great = new ArtifactDropRate();
			Great.AddItem(DECOR.SPACEARTIFACT.TIER_NONE, 10f);
			Great.AddItem(DECOR.SPACEARTIFACT.TIER3, 5f);
			Great.AddItem(DECOR.SPACEARTIFACT.TIER4, 3f);
			Great.AddItem(DECOR.SPACEARTIFACT.TIER5, 2f);
			Add(Great);
			Amazing = new ArtifactDropRate();
			Amazing.AddItem(DECOR.SPACEARTIFACT.TIER_NONE, 10f);
			Amazing.AddItem(DECOR.SPACEARTIFACT.TIER3, 3f);
			Amazing.AddItem(DECOR.SPACEARTIFACT.TIER4, 5f);
			Amazing.AddItem(DECOR.SPACEARTIFACT.TIER5, 2f);
			Add(Amazing);
			Perfect = new ArtifactDropRate();
			Perfect.AddItem(DECOR.SPACEARTIFACT.TIER_NONE, 10f);
			Perfect.AddItem(DECOR.SPACEARTIFACT.TIER4, 6f);
			Perfect.AddItem(DECOR.SPACEARTIFACT.TIER5, 4f);
			Add(Perfect);
		}
	}
}
