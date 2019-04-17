using Klei.AI;

namespace Database
{
	public class Personalities : ResourceSet<Personality>
	{
		private class PersonalityLoader : AsyncCsvLoader<PersonalityLoader, PersonalityInfo>
		{
			public PersonalityLoader()
				: base(Assets.instance.personalitiesFile)
			{
			}

			public override void Run()
			{
				base.Run();
			}
		}

		public class PersonalityInfo : Resource
		{
			public int HeadShape;

			public int Mouth;

			public int Neck;

			public int Eyes;

			public int Hair;

			public int Body;

			public string Gender;

			public string PersonalityType;

			public string StressTrait;

			public string CongenitalTrait;

			public string Design;
		}

		public Personalities()
		{
			PersonalityInfo[] entries = AsyncLoadManager<IGlobalAsyncLoader>.AsyncLoader<PersonalityLoader>.Get().entries;
			foreach (PersonalityInfo personalityInfo in entries)
			{
				Personality resource = new Personality(personalityInfo.Name.ToUpper(), Strings.Get($"STRINGS.DUPLICANTS.PERSONALITIES.{personalityInfo.Name.ToUpper()}.NAME"), personalityInfo.Gender.ToUpper(), personalityInfo.PersonalityType, personalityInfo.StressTrait, personalityInfo.CongenitalTrait, personalityInfo.HeadShape, personalityInfo.Mouth, personalityInfo.Neck, personalityInfo.Eyes, personalityInfo.Hair, personalityInfo.Body, Strings.Get($"STRINGS.DUPLICANTS.PERSONALITIES.{personalityInfo.Name.ToUpper()}.DESC"));
				Add(resource);
			}
		}

		private void AddTrait(Personality personality, string trait_name)
		{
			Trait trait = Db.Get().traits.TryGet(trait_name);
			if (trait != null)
			{
				personality.AddTrait(trait);
			}
		}

		private void SetAttribute(Personality personality, string attribute_name, int value)
		{
			Attribute attribute = Db.Get().Attributes.TryGet(attribute_name);
			if (attribute == null)
			{
				Debug.LogWarning("Attribute does not exist: " + attribute_name);
			}
			else
			{
				personality.SetAttribute(attribute, value);
			}
		}
	}
}
