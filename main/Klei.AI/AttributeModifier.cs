using System;
using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{AttributeId}")]
	public class AttributeModifier
	{
		public string Description;

		public Func<string> DescriptionCB;

		public string AttributeId
		{
			get;
			private set;
		}

		public float Value
		{
			get;
			private set;
		}

		public bool IsMultiplier
		{
			get;
			private set;
		}

		public bool UIOnly
		{
			get;
			private set;
		}

		public bool IsReadonly
		{
			get;
			private set;
		}

		public AttributeModifier(string attribute_id, float value, string description = null, bool is_multiplier = false, bool uiOnly = false, bool is_readonly = true)
		{
			AttributeId = attribute_id;
			Value = value;
			Description = ((description != null) ? description : string.Empty);
			DescriptionCB = null;
			IsMultiplier = is_multiplier;
			UIOnly = uiOnly;
			IsReadonly = is_readonly;
		}

		public AttributeModifier(string attribute_id, float value, Func<string> description_cb, bool is_multiplier = false, bool uiOnly = false)
		{
			AttributeId = attribute_id;
			Value = value;
			DescriptionCB = description_cb;
			Description = null;
			IsMultiplier = is_multiplier;
			UIOnly = uiOnly;
		}

		public void SetValue(float value)
		{
			DebugUtil.DevAssert(!IsReadonly);
			Value = value;
		}

		public string GetDescription()
		{
			return (DescriptionCB == null) ? Description : DescriptionCB();
		}

		public string GetFormattedString(GameObject parent_instance)
		{
			IAttributeFormatter attributeFormatter = null;
			Attribute attribute = Db.Get().Attributes.TryGet(AttributeId);
			if (!IsMultiplier)
			{
				if (attribute != null)
				{
					attributeFormatter = attribute.formatter;
				}
				else
				{
					attribute = Db.Get().BuildingAttributes.TryGet(AttributeId);
					if (attribute != null)
					{
						attributeFormatter = attribute.formatter;
					}
				}
			}
			string empty = string.Empty;
			empty = ((attributeFormatter != null) ? attributeFormatter.GetFormattedModifier(this, parent_instance) : ((!IsMultiplier) ? (empty + GameUtil.GetFormattedSimple(Value, GameUtil.TimeSlice.None, null)) : (empty + GameUtil.GetFormattedPercent(Value * 100f, GameUtil.TimeSlice.None))));
			if (empty != null && empty.Length > 0 && empty[0] != '-')
			{
				empty = GameUtil.AddPositiveSign(empty, Value > 0f);
			}
			return empty;
		}

		public AttributeModifier Clone()
		{
			return new AttributeModifier(AttributeId, Value, Description, false, false, true);
		}
	}
}
