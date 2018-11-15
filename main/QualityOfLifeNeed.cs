using Klei.AI;
using Klei.CustomSettings;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class QualityOfLifeNeed : Need, ISim4000ms
{
	private AttributeInstance qolAttribute;

	public bool skipUpdate;

	[Serialize]
	private List<bool> breakBlocks;

	private static readonly EventSystem.IntraObjectHandler<QualityOfLifeNeed> OnScheduleBlocksTickDelegate = new EventSystem.IntraObjectHandler<QualityOfLifeNeed>(delegate(QualityOfLifeNeed component, object data)
	{
		component.OnScheduleBlocksTick(data);
	});

	private static List<string> breakLengthEffects = new List<string>
	{
		"Break1",
		"Break2",
		"Break3",
		"Break4",
		"Break5"
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		breakBlocks = new List<bool>(24);
		Attributes attributes = base.gameObject.GetAttributes();
		expectationAttribute = attributes.Add(Db.Get().Attributes.QualityOfLifeExpectation);
		base.Name = DUPLICANTS.NEEDS.QUALITYOFLIFE.NAME;
		base.ExpectationTooltip = string.Format(DUPLICANTS.NEEDS.QUALITYOFLIFE.EXPECTATION_TOOLTIP, Db.Get().Attributes.QualityOfLifeExpectation.Lookup(this).GetTotalValue());
		stressBonus = new ModifierType
		{
			modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0f, DUPLICANTS.NEEDS.QUALITYOFLIFE.GOOD_MODIFIER, false, false, false)
		};
		stressNeutral = new ModifierType
		{
			modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.008333334f, DUPLICANTS.NEEDS.QUALITYOFLIFE.NEUTRAL_MODIFIER, false, false, true)
		};
		stressPenalty = new ModifierType
		{
			modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0f, DUPLICANTS.NEEDS.QUALITYOFLIFE.BAD_MODIFIER, false, false, false),
			statusItem = Db.Get().DuplicantStatusItems.PoorQualityOfLife
		};
		qolAttribute = Db.Get().Attributes.QualityOfLife.Lookup(base.gameObject);
		Subscribe(1714332666, OnScheduleBlocksTickDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		while (breakBlocks.Count < 24)
		{
			breakBlocks.Add(false);
		}
		while (breakBlocks.Count > 24)
		{
			breakBlocks.RemoveAt(breakBlocks.Count - 1);
		}
	}

	public void Sim4000ms(float dt)
	{
		if (!skipUpdate)
		{
			float num = 0.004166667f;
			float b = 0.0416666679f;
			SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Morale);
			if (currentQualitySetting.id == "Disabled")
			{
				SetModifier(null);
			}
			else
			{
				if (currentQualitySetting.id == "Easy")
				{
					num = 0.00333333341f;
					b = 0.0166666675f;
				}
				else if (currentQualitySetting.id == "Hard")
				{
					num = 0.008333334f;
					b = 0.05f;
				}
				else if (currentQualitySetting.id == "VeryHard")
				{
					num = 0.0166666675f;
					b = 0.0833333358f;
				}
				float totalValue = qolAttribute.GetTotalValue();
				float totalValue2 = expectationAttribute.GetTotalValue();
				float num2 = totalValue2 - totalValue;
				if (totalValue < totalValue2)
				{
					stressPenalty.modifier.SetValue(Mathf.Min(num2 * num, b));
					SetModifier(stressPenalty);
				}
				else if (totalValue > totalValue2)
				{
					stressBonus.modifier.SetValue(Mathf.Max((0f - num2) * -0.0166666675f, -0.0333333351f));
					SetModifier(stressBonus);
				}
				else
				{
					SetModifier(stressNeutral);
				}
			}
		}
	}

	private void OnScheduleBlocksTick(object data)
	{
		Schedule schedule = (Schedule)data;
		ScheduleBlock block = schedule.GetBlock(Schedule.GetLastBlockIdx());
		ScheduleBlock block2 = schedule.GetBlock(Schedule.GetBlockIdx());
		bool flag = block.IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);
		bool flag2 = block2.IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);
		breakBlocks[Schedule.GetLastBlockIdx()] = flag;
		if (flag && !flag2)
		{
			int num = 0;
			foreach (bool breakBlock in breakBlocks)
			{
				if (breakBlock)
				{
					num++;
				}
			}
			ApplyBreakBonus(num);
		}
	}

	private void ApplyBreakBonus(int numBlocks)
	{
		string breakBonus = GetBreakBonus(numBlocks);
		if (breakBonus != null)
		{
			Effects component = GetComponent<Effects>();
			component.Add(breakBonus, true);
		}
	}

	public static string GetBreakBonus(int numBlocks)
	{
		int num = numBlocks - 1;
		if (num >= breakLengthEffects.Count)
		{
			return breakLengthEffects[breakLengthEffects.Count - 1];
		}
		if (num >= 0)
		{
			return breakLengthEffects[num];
		}
		return null;
	}
}
