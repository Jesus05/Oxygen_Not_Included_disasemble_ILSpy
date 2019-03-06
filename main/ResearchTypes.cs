using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class ResearchTypes
{
	public class ID
	{
		public const string ALPHA = "alpha";

		public const string BETA = "beta";

		public const string GAMMA = "gamma";
	}

	public List<ResearchType> Types = new List<ResearchType>();

	public ResearchTypes()
	{
		ResearchType item = new ResearchType("alpha", RESEARCH.TYPES.ALPHA.NAME, RESEARCH.TYPES.ALPHA.DESC, Assets.GetSprite("research_type_alpha_icon"), new Color(0.596078455f, 0.6666667f, 0.9137255f), new Recipe.Ingredient[1]
		{
			new Recipe.Ingredient("Dirt".ToTag(), 100f)
		}, 600f, "research_center_kanim", new string[1]
		{
			"ResearchCenter"
		}, RESEARCH.TYPES.ALPHA.RECIPEDESC);
		Types.Add(item);
		ResearchType item2 = new ResearchType("beta", RESEARCH.TYPES.BETA.NAME, RESEARCH.TYPES.BETA.DESC, Assets.GetSprite("research_type_beta_icon"), new Color(0.6f, 0.384313732f, 0.5686275f), new Recipe.Ingredient[1]
		{
			new Recipe.Ingredient("Water".ToTag(), 25f)
		}, 1200f, "research_center_kanim", new string[1]
		{
			"AdvancedResearchCenter"
		}, RESEARCH.TYPES.BETA.RECIPEDESC);
		Types.Add(item2);
		ResearchType item3 = new ResearchType("gamma", RESEARCH.TYPES.GAMMA.NAME, RESEARCH.TYPES.GAMMA.DESC, Assets.GetSprite("research_type_gamma_icon"), new Color32(240, 141, 44, byte.MaxValue), null, 2400f, "research_center_kanim", new string[1]
		{
			"CosmicResearchCenter"
		}, RESEARCH.TYPES.GAMMA.RECIPEDESC);
		Types.Add(item3);
	}

	public ResearchType GetResearchType(string id)
	{
		foreach (ResearchType type in Types)
		{
			if (id == type.id)
			{
				return type;
			}
		}
		Debug.LogWarning($"No research with type id {id} found", null);
		return null;
	}
}
