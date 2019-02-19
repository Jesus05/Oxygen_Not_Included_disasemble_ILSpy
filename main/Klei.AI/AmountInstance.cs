using KSerialization;
using System;
using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	[DebuggerDisplay("{amount.Name} {value} ({deltaAttribute.value}/{minAttribute.value}/{maxAttribute.value})")]
	public class AmountInstance : ModifierInstance<Amount>, ISaveLoadable, ISim200ms
	{
		[Serialize]
		public float value;

		public AttributeInstance minAttribute;

		public AttributeInstance maxAttribute;

		public AttributeInstance deltaAttribute;

		public Action<float> OnDelta;

		public System.Action OnMaxValueReached;

		public bool hide;

		private bool _paused;

		public Amount amount => modifier;

		public bool paused
		{
			get
			{
				return _paused;
			}
			set
			{
				_paused = paused;
				if (_paused)
				{
					Deactivate();
				}
				else
				{
					Activate();
				}
			}
		}

		public AmountInstance(Amount amount, GameObject game_object)
			: base(game_object, amount)
		{
			Attributes attributes = game_object.GetAttributes();
			minAttribute = attributes.Add(amount.minAttribute);
			maxAttribute = attributes.Add(amount.maxAttribute);
			deltaAttribute = attributes.Add(amount.deltaAttribute);
		}

		public float GetMin()
		{
			return minAttribute.GetTotalValue();
		}

		public float GetMax()
		{
			return maxAttribute.GetTotalValue();
		}

		public float GetDelta()
		{
			return deltaAttribute.GetTotalValue();
		}

		public float SetValue(float value)
		{
			this.value = value;
			this.value = Mathf.Max(this.value, GetMin());
			this.value = Mathf.Min(this.value, GetMax());
			return this.value;
		}

		public float ApplyDelta(float delta)
		{
			float num = value;
			SetValue(value + delta);
			if (OnDelta != null)
			{
				OnDelta(delta);
			}
			if (OnMaxValueReached != null && num < GetMax() && value >= GetMax())
			{
				OnMaxValueReached();
			}
			return value;
		}

		public string GetValueString()
		{
			return amount.GetValueString(this);
		}

		public string GetDescription()
		{
			return amount.GetDescription(this);
		}

		public string GetTooltip()
		{
			return amount.GetTooltip(this);
		}

		public void Activate()
		{
			SimAndRenderScheduler.instance.Add(this, false);
		}

		public void Sim200ms(float dt)
		{
			if (dt != 0f)
			{
				float delta = GetDelta();
				if (delta != 0f)
				{
					ApplyDelta(delta * dt);
				}
			}
		}

		public void Deactivate()
		{
			SimAndRenderScheduler.instance.Remove(this);
		}
	}
}
