using STRINGS;

namespace TUNING
{
	public class DECOR
	{
		public class BONUS
		{
			public static readonly EffectorValues TIER0 = new EffectorValues
			{
				amount = 10,
				radius = 1
			};

			public static readonly EffectorValues TIER1 = new EffectorValues
			{
				amount = 15,
				radius = 2
			};

			public static readonly EffectorValues TIER2 = new EffectorValues
			{
				amount = 20,
				radius = 3
			};

			public static readonly EffectorValues TIER3 = new EffectorValues
			{
				amount = 25,
				radius = 4
			};

			public static readonly EffectorValues TIER4 = new EffectorValues
			{
				amount = 30,
				radius = 5
			};

			public static readonly EffectorValues TIER5 = new EffectorValues
			{
				amount = 35,
				radius = 6
			};

			public static readonly EffectorValues TIER6 = new EffectorValues
			{
				amount = 50,
				radius = 7
			};

			public static readonly EffectorValues TIER7 = new EffectorValues
			{
				amount = 80,
				radius = 7
			};

			public static readonly EffectorValues TIER8 = new EffectorValues
			{
				amount = 200,
				radius = 8
			};
		}

		public class PENALTY
		{
			public static readonly EffectorValues TIER0 = new EffectorValues
			{
				amount = -5,
				radius = 1
			};

			public static readonly EffectorValues TIER1 = new EffectorValues
			{
				amount = -10,
				radius = 2
			};

			public static readonly EffectorValues TIER2 = new EffectorValues
			{
				amount = -15,
				radius = 3
			};

			public static readonly EffectorValues TIER3 = new EffectorValues
			{
				amount = -20,
				radius = 4
			};

			public static readonly EffectorValues TIER4 = new EffectorValues
			{
				amount = -20,
				radius = 5
			};

			public static readonly EffectorValues TIER5 = new EffectorValues
			{
				amount = -25,
				radius = 6
			};
		}

		public class SPACEARTIFACT
		{
			public static readonly ArtifactTier TIER_NONE = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER_NONE.key, NONE);

			public static readonly ArtifactTier TIER0 = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER0.key, BONUS.TIER0);

			public static readonly ArtifactTier TIER1 = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER1.key, BONUS.TIER2);

			public static readonly ArtifactTier TIER2 = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER2.key, BONUS.TIER4);

			public static readonly ArtifactTier TIER3 = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER3.key, BONUS.TIER5);

			public static readonly ArtifactTier TIER4 = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER4.key, BONUS.TIER6);

			public static readonly ArtifactTier TIER5 = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER5.key, BONUS.TIER7);
		}

		public static int LIT_BONUS = 15;

		public static readonly EffectorValues NONE = new EffectorValues
		{
			amount = 0,
			radius = 0
		};
	}
}
