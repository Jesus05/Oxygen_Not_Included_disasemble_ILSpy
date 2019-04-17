using Klei;
using KSerialization.Converters;
using System.Collections.Generic;

namespace ProcGen
{
	public class FeatureSettings : YamlIO<FeatureSettings>
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

		public List<string> excludeTags
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
			excludeTags = new List<string>();
		}

		public bool HasGroup(string item)
		{
			return ElementChoiceGroups.ContainsKey(item);
		}

		public WeightedSimHash GetOneWeightedSimHash(string item, SeededRandom rnd)
		{
			if (ElementChoiceGroups.ContainsKey(item))
			{
				return WeightedRandom.Choose(ElementChoiceGroups[item].choices, rnd);
			}
			Debug.LogError("Couldnt get SimHash [" + item + "]");
			return null;
		}
	}
}
