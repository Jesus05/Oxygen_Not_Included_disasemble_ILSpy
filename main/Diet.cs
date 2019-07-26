using System.Collections.Generic;

public class Diet
{
	public class Info
	{
		public HashSet<Tag> consumedTags
		{
			get;
			private set;
		}

		public Tag producedElement
		{
			get;
			private set;
		}

		public float caloriesPerKg
		{
			get;
			private set;
		}

		public float producedConversionRate
		{
			get;
			private set;
		}

		public byte diseaseIdx
		{
			get;
			private set;
		}

		public float diseasePerKgProduced
		{
			get;
			private set;
		}

		public bool produceSolidTile
		{
			get;
			private set;
		}

		public bool eatsPlantsDirectly
		{
			get;
			private set;
		}

		public Info(HashSet<Tag> consumed_tags, Tag produced_element, float calories_per_kg, float produced_conversion_rate = 1f, string disease_id = null, float disease_per_kg_produced = 0f, bool produce_solid_tile = false, bool eats_plants_directly = false)
		{
			consumedTags = consumed_tags;
			producedElement = produced_element;
			caloriesPerKg = calories_per_kg;
			producedConversionRate = produced_conversion_rate;
			if (!string.IsNullOrEmpty(disease_id))
			{
				diseaseIdx = Db.Get().Diseases.GetIndex(disease_id);
			}
			else
			{
				diseaseIdx = byte.MaxValue;
			}
			produceSolidTile = produce_solid_tile;
			eatsPlantsDirectly = eats_plants_directly;
		}

		public bool IsMatch(Tag tag)
		{
			return consumedTags.Contains(tag);
		}

		public bool IsMatch(HashSet<Tag> tags)
		{
			if (tags.Count >= consumedTags.Count)
			{
				foreach (Tag consumedTag in consumedTags)
				{
					if (tags.Contains(consumedTag))
					{
						return true;
					}
				}
				return false;
			}
			foreach (Tag tag in tags)
			{
				if (consumedTags.Contains(tag))
				{
					return true;
				}
			}
			return false;
		}

		public float ConvertCaloriesToConsumptionMass(float calories)
		{
			return calories / caloriesPerKg;
		}

		public float ConvertConsumptionMassToCalories(float mass)
		{
			return caloriesPerKg * mass;
		}

		public float ConvertConsumptionMassToProducedMass(float consumed_mass)
		{
			return consumed_mass * producedConversionRate;
		}
	}

	public List<KeyValuePair<Tag, float>> consumedTags;

	public List<KeyValuePair<Tag, float>> producedTags;

	public bool eatsPlantsDirectly = false;

	private Dictionary<Tag, Info> consumedTagToInfo = new Dictionary<Tag, Info>();

	public Info[] infos
	{
		get;
		private set;
	}

	public Diet(params Info[] infos)
	{
		this.infos = infos;
		consumedTags = new List<KeyValuePair<Tag, float>>();
		producedTags = new List<KeyValuePair<Tag, float>>();
		foreach (Info info in infos)
		{
			if (info.eatsPlantsDirectly)
			{
				eatsPlantsDirectly = true;
			}
			foreach (Tag consumedTag in info.consumedTags)
			{
				if (consumedTags.FindIndex((KeyValuePair<Tag, float> e) => e.Key == consumedTag) == -1)
				{
					consumedTags.Add(new KeyValuePair<Tag, float>(consumedTag, info.caloriesPerKg));
				}
				if (consumedTagToInfo.ContainsKey(consumedTag))
				{
					Debug.LogError("Duplicate diet entry: " + consumedTag);
				}
				consumedTagToInfo[consumedTag] = info;
			}
			if (info.producedElement != Tag.Invalid && producedTags.FindIndex((KeyValuePair<Tag, float> e) => e.Key == info.producedElement) == -1)
			{
				producedTags.Add(new KeyValuePair<Tag, float>(info.producedElement, info.producedConversionRate));
			}
		}
	}

	public Info GetDietInfo(Tag tag)
	{
		Info value = null;
		consumedTagToInfo.TryGetValue(tag, out value);
		return value;
	}
}
