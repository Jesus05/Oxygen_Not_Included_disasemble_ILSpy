using Klei.AI;
using System.Collections.Generic;

public class Personality : Resource
{
	public class StartingAttribute
	{
		public Attribute attribute;

		public int value;

		public StartingAttribute(Attribute attribute, int value)
		{
			this.attribute = attribute;
			this.value = value;
		}
	}

	public List<StartingAttribute> attributes = new List<StartingAttribute>();

	public List<Trait> traits = new List<Trait>();

	public int headShape;

	public int mouth;

	public int neck;

	public int eyes;

	public int hair;

	public int body;

	public string nameStringKey;

	public string genderStringKey;

	public string personalityType;

	public string stresstrait;

	public string congenitaltrait;

	public string unformattedDescription;

	public string description => GetDescription();

	public Personality(string name_string_key, string name, string Gender, string PersonalityType, string StressTrait, string CongenitalTrait, int headShape, int mouth, int neck, int eyes, int hair, int body, string description)
		: base(name, name)
	{
		nameStringKey = name_string_key;
		genderStringKey = Gender;
		personalityType = PersonalityType;
		stresstrait = StressTrait;
		congenitaltrait = CongenitalTrait;
		unformattedDescription = description;
		this.headShape = headShape;
		this.mouth = mouth;
		this.neck = neck;
		this.eyes = eyes;
		this.hair = hair;
		this.body = body;
	}

	public string GetDescription()
	{
		unformattedDescription = unformattedDescription.Replace("{0}", Name);
		return unformattedDescription;
	}

	public void SetAttribute(Attribute attribute, int value)
	{
		StartingAttribute item = new StartingAttribute(attribute, value);
		attributes.Add(item);
	}

	public void AddTrait(Trait trait)
	{
		traits.Add(trait);
	}
}
