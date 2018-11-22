using Klei.AI;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class DecorMonitor : GameStateMachine<DecorMonitor, DecorMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		[Serialize]
		private float cycleTotalDecor = 0f;

		[Serialize]
		private float yesterdaysTotalDecor = 0f;

		private AmountInstance amount;

		private AttributeModifier modifier;

		private List<KeyValuePair<float, string>> effectLookup = new List<KeyValuePair<float, string>>
		{
			new KeyValuePair<float, string>(-30f, "DecorMinus1"),
			new KeyValuePair<float, string>(0f, "Decor0"),
			new KeyValuePair<float, string>(30f, "Decor1"),
			new KeyValuePair<float, string>(60f, "Decor2"),
			new KeyValuePair<float, string>(90f, "Decor3"),
			new KeyValuePair<float, string>(120f, "Decor4"),
			new KeyValuePair<float, string>(3.40282347E+38f, "Decor5")
		};

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			cycleTotalDecor = 2250f;
			amount = Db.Get().Amounts.Decor.Lookup(base.gameObject);
			modifier = new AttributeModifier(Db.Get().Amounts.Decor.deltaAttribute.Id, 1f, DUPLICANTS.NEEDS.DECOR.OBSERVED_DECOR, false, false, false);
		}

		public AttributeModifier GetDecorModifier()
		{
			return modifier;
		}

		public void Update(float dt)
		{
			int cell = Grid.PosToCell(base.gameObject);
			if (Grid.IsValidCell(cell))
			{
				float decorAtCell = GameUtil.GetDecorAtCell(cell);
				cycleTotalDecor += decorAtCell * dt;
				float value = 0f;
				float num = 4.16666651f;
				if (Mathf.Abs(decorAtCell - amount.value) > 0.5f)
				{
					if (decorAtCell > amount.value)
					{
						value = 3f * num;
					}
					else if (decorAtCell < amount.value)
					{
						value = 0f - num;
					}
				}
				else
				{
					amount.value = decorAtCell;
				}
				modifier.SetValue(value);
			}
		}

		public void OnNewDay()
		{
			yesterdaysTotalDecor = cycleTotalDecor;
			cycleTotalDecor = 0f;
			Attributes attributes = base.gameObject.GetAttributes();
			AttributeInstance attributeInstance = attributes.Add(Db.Get().Attributes.DecorExpectation);
			float totalValue = attributeInstance.GetTotalValue();
			float num = yesterdaysTotalDecor / 600f;
			num += totalValue;
			Effects component = base.gameObject.GetComponent<Effects>();
			foreach (KeyValuePair<float, string> item in effectLookup)
			{
				if (num < item.Key)
				{
					component.Add(item.Value, true);
					break;
				}
			}
		}

		public float GetTodaysAverageDecor()
		{
			return cycleTotalDecor / (GameClock.Instance.GetCurrentCycleAsPercentage() * 600f);
		}

		public float GetYesterdaysAverageDecor()
		{
			return yesterdaysTotalDecor / 600f;
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleAttributeModifier("DecorSmoother", (Instance smi) => smi.GetDecorModifier(), (Instance smi) => true).Update("DecorSensing", delegate(Instance smi, float dt)
		{
			smi.Update(dt);
		}, UpdateRate.SIM_200ms, false).EventHandler(GameHashes.NewDay, (Instance smi) => GameClock.Instance, delegate(Instance smi)
		{
			smi.OnNewDay();
		});
	}
}
