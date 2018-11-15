using Klei.AI;
using STRINGS;
using System.Collections.Generic;

public static class Expectations
{
	public static List<Expectation[]> ExpectationsByTier = new List<Expectation[]>
	{
		new Expectation[1]
		{
			QOLExpectation(1, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER0.NAME, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER0.DESCRIPTION)
		},
		new Expectation[1]
		{
			QOLExpectation(2, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER1.NAME, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER1.DESCRIPTION)
		},
		new Expectation[1]
		{
			QOLExpectation(4, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER2.NAME, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER2.DESCRIPTION)
		},
		new Expectation[1]
		{
			QOLExpectation(8, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER3.NAME, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER3.DESCRIPTION)
		},
		new Expectation[1]
		{
			QOLExpectation(12, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER4.NAME, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER4.DESCRIPTION)
		},
		new Expectation[1]
		{
			QOLExpectation(16, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER5.NAME, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER5.DESCRIPTION)
		},
		new Expectation[1]
		{
			QOLExpectation(20, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER6.NAME, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER6.DESCRIPTION)
		},
		new Expectation[1]
		{
			QOLExpectation(25, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER7.NAME, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER7.DESCRIPTION)
		},
		new Expectation[1]
		{
			QOLExpectation(30, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER8.NAME, UI.ROLES_SCREEN.EXPECTATIONS.QUALITYOFLIFE.TIER8.DESCRIPTION)
		}
	};

	private static AttributeModifier QOLModifier(int level)
	{
		return new AttributeModifier(Db.Get().Attributes.QualityOfLifeExpectation.Id, (float)level, DUPLICANTS.NEEDS.QUALITYOFLIFE.EXPECTATION_MOD_NAME, false, false, true);
	}

	private static AttributeModifierExpectation QOLExpectation(int level, string name, string description)
	{
		return new AttributeModifierExpectation("QOL_" + level.ToString(), name, description, QOLModifier(level), Assets.GetSprite("icon_category_morale"));
	}
}
