using KSerialization.Converters;
using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class FeatureSettings
	{
		[StringEnumConverter]
		public Room.Shape shape
		{
			get;
			private set;
		}

		public List<int> borders
		{
			get;
			private set;
		}

		public MinMax blobSize
		{
			get;
			private set;
		}

		public string forceBiome
		{
			get;
			private set;
		}

		public List<MobReference> internalMobs
		{
			get;
			private set;
		}

		public List<string> tags
		{
			get;
			private set;
		}

		public Dictionary<string, ElementChoiceGroup<WeightedSimHash>> ElementChoiceGroups
		{
			get;
			private set;
		}

		public FeatureSettings()
		{
			ElementChoiceGroups = new Dictionary<string, ElementChoiceGroup<WeightedSimHash>>();
			borders = new List<int>();
			tags = new List<string>();
			internalMobs = new List<MobReference>();
		}

		public bool HasGroup(string item)
		{
			return ElementChoiceGroups.ContainsKey(item);
		}

		public WeightedSimHash GetOneWeightedSimHash(string item, SeededRandom rnd)
		{
			if (!ElementChoiceGroups.ContainsKey(item))
			{
				Debug.LogError("Couldnt get SimHash [" + item + "]");
				return null;
			}
			return WeightedRandom.Choose(ElementChoiceGroups[item].choices, rnd);
		}
	}
}
