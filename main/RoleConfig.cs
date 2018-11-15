using Klei.AI;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class RoleConfig : IListableOption
{
	public float experienceRequired = 100f;

	public Attribute[] relevantAttributes = new Attribute[0];

	public string id
	{
		get;
		protected set;
	}

	public string name
	{
		get;
		protected set;
	}

	public string description
	{
		get;
		protected set;
	}

	public HashedString roleGroup
	{
		get;
		protected set;
	}

	public string hat
	{
		get;
		protected set;
	}

	public int tier
	{
		get;
		protected set;
	}

	public RolePerk[] perks
	{
		get;
		protected set;
	}

	public RoleAssignmentRequirement[] requirements
	{
		get;
		protected set;
	}

	public Expectation[] expectations
	{
		get;
		protected set;
	}

	public string GetProperName()
	{
		return name;
	}

	public void SetTier(int tier)
	{
		this.tier = tier;
		experienceRequired = ROLES.BASIC_ROLE_MASTERY_EXPERIENCE_REQUIRED * (float)tier;
	}

	public virtual void InitRequirements()
	{
	}

	public bool HasPerk(RolePerk perk)
	{
		for (int i = 0; i < perks.Length; i++)
		{
			if (perks[i] == perk)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasPerk(HashedString perk_id)
	{
		for (int i = 0; i < perks.Length; i++)
		{
			if (perks[i].id == perk_id)
			{
				return true;
			}
		}
		return false;
	}

	public int QOLExpectation()
	{
		Expectation[] array = Expectations.ExpectationsByTier[tier];
		Expectation[] array2 = array;
		foreach (Expectation expectation in array2)
		{
			AttributeModifierExpectation attributeModifierExpectation = expectation as AttributeModifierExpectation;
			if (attributeModifierExpectation != null && attributeModifierExpectation.modifier.AttributeId == Db.Get().Attributes.QualityOfLifeExpectation.Id)
			{
				return Mathf.RoundToInt(attributeModifierExpectation.modifier.Value);
			}
		}
		return 0;
	}

	public virtual void GatherNearbyFetchChores(FetchChore root_chore, Chore.Precondition.Context context, int x, int y, int radius, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> failed_contexts)
	{
		FetchAreaChore.GatherNearbyFetchChores(root_chore, context, x, y, radius, succeeded_contexts, failed_contexts);
	}
}
