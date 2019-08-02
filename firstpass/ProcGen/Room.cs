using KSerialization.Converters;
using System.Collections.Generic;

namespace ProcGen
{
	public class Room : SampleDescriber
	{
		public enum Shape
		{
			Circle,
			Oval,
			Blob,
			Line,
			Square,
			TallThin,
			ShortWide,
			Template,
			PhysicalLayout,
			Splat
		}

		public enum Selection
		{
			None,
			OneOfEach,
			NOfEach,
			Weighted,
			WeightedBucket,
			WeightedResample,
			PickOneWeighted
		}

		private List<WeightedMob>.Enumerator mobIter;

		private List<WeightedMob> bucket;

		[StringEnumConverter]
		public Shape shape
		{
			get;
			private set;
		}

		[StringEnumConverter]
		public Selection mobselection
		{
			get;
			private set;
		}

		public List<WeightedMob> mobs
		{
			get;
			private set;
		}

		public Room()
		{
			mobs = new List<WeightedMob>();
		}

		public void ResetMobs(SeededRandom rnd)
		{
			if (mobselection == Selection.WeightedBucket)
			{
				if (bucket == null)
				{
					bucket = new List<WeightedMob>();
					for (int i = 0; i < mobs.Count; i++)
					{
						for (int j = 0; (float)j < mobs[i].weight; j++)
						{
							bucket.Add(new WeightedMob(mobs[i].tag, 1f));
						}
					}
				}
				bucket.ShuffleSeeded(rnd.RandomSource());
				mobIter = bucket.GetEnumerator();
			}
			else
			{
				mobIter = mobs.GetEnumerator();
			}
		}

		public WeightedMob GetNextMob(SeededRandom rnd)
		{
			WeightedMob result = null;
			switch (mobselection)
			{
			case Selection.Weighted:
				result = WeightedRandom.Choose(mobs, rnd);
				break;
			case Selection.OneOfEach:
			case Selection.WeightedBucket:
				if (mobIter.MoveNext())
				{
					result = mobIter.Current;
				}
				break;
			}
			return result;
		}
	}
}
