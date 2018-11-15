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

		public static int LIT_BONUS = 15;

		public static readonly EffectorValues NONE = new EffectorValues
		{
			amount = 0,
			radius = 0
		};
	}
}
