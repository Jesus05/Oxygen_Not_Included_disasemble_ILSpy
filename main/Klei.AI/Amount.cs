using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{Id}")]
	public class Amount : Resource
	{
		public string description;

		public bool showMax;

		public Units units;

		public float visualDeltaThreshold;

		public Attribute minAttribute;

		public Attribute maxAttribute;

		public Attribute deltaAttribute;

		public bool showInUI;

		public string uiSprite;

		public string thoughtSprite;

		public IAmountDisplayer displayer;

		public Amount(string id, string name, string description, Attribute min_attribute, Attribute max_attribute, Attribute delta_attribute, bool show_max, Units units, float visual_delta_threshold, bool show_in_ui, string uiSprite = null, string thoughtSprite = null)
		{
			Id = id;
			Name = name;
			this.description = description;
			minAttribute = min_attribute;
			maxAttribute = max_attribute;
			deltaAttribute = delta_attribute;
			showMax = show_max;
			this.units = units;
			visualDeltaThreshold = visual_delta_threshold;
			showInUI = show_in_ui;
			this.uiSprite = uiSprite;
			this.thoughtSprite = thoughtSprite;
		}

		public void SetDisplayer(IAmountDisplayer displayer)
		{
			this.displayer = displayer;
			minAttribute.SetFormatter(displayer.Formatter);
			maxAttribute.SetFormatter(displayer.Formatter);
			deltaAttribute.SetFormatter(displayer.Formatter);
		}

		public AmountInstance Lookup(Component cmp)
		{
			return Lookup(cmp.gameObject);
		}

		public AmountInstance Lookup(GameObject go)
		{
			return go.GetAmounts()?.Get(this);
		}

		public void Copy(GameObject to, GameObject from)
		{
			AmountInstance amountInstance = Lookup(to);
			AmountInstance amountInstance2 = Lookup(from);
			amountInstance.value = amountInstance2.value;
		}

		public string GetValueString(AmountInstance instance)
		{
			return displayer.GetValueString(this, instance);
		}

		public string GetDescription(AmountInstance instance)
		{
			return displayer.GetDescription(this, instance);
		}

		public string GetTooltip(AmountInstance instance)
		{
			return displayer.GetTooltip(this, instance);
		}
	}
}
