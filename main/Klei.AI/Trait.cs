using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class Trait : Modifier
	{
		public float Rating;

		public bool ShouldSave;

		public bool PositiveTrait;

		public bool ValidStarterTrait;

		public Action<GameObject> OnAddTrait = null;

		public Func<string> ExtendedTooltip;

		public ChoreGroup[] disabledChoreGroups;

		public bool isTaskBeingRefused;

		public string[] ignoredEffects;

		public Trait(string id, string name, string description, float rating, bool should_save, ChoreGroup[] disallowed_chore_groups, bool positive_trait, bool is_valid_starter_trait)
			: base(id, name, description)
		{
			Rating = rating;
			ShouldSave = should_save;
			disabledChoreGroups = disallowed_chore_groups;
			PositiveTrait = positive_trait;
			ValidStarterTrait = is_valid_starter_trait;
			ignoredEffects = new string[0];
		}

		public void AddIgnoredEffects(string[] effects)
		{
			List<string> list = new List<string>(ignoredEffects);
			list.AddRange(effects);
			ignoredEffects = list.ToArray();
		}

		public string GetTooltip()
		{
			string text = description;
			foreach (AttributeModifier selfModifier in SelfModifiers)
			{
				Attribute attribute = Db.Get().Attributes.Get(selfModifier.AttributeId);
				text += string.Format(DUPLICANTS.TRAITS.ATTRIBUTE_MODIFIERS, attribute.Name, selfModifier.GetFormattedString(null));
			}
			if (disabledChoreGroups != null)
			{
				string format = DUPLICANTS.TRAITS.CANNOT_DO_TASK;
				if (isTaskBeingRefused)
				{
					format = DUPLICANTS.TRAITS.REFUSES_TO_DO_TASK;
				}
				ChoreGroup[] array = disabledChoreGroups;
				foreach (ChoreGroup choreGroup in array)
				{
					text += string.Format(format, choreGroup.Name);
				}
			}
			if (ignoredEffects != null && ignoredEffects.Length > 0)
			{
				string[] array2 = ignoredEffects;
				foreach (string text2 in array2)
				{
					string arg = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text2.ToUpper() + ".NAME");
					text += string.Format(DUPLICANTS.TRAITS.IGNORED_EFFECTS, arg);
				}
			}
			if (ExtendedTooltip != null)
			{
				Delegate[] invocationList = ExtendedTooltip.GetInvocationList();
				foreach (Delegate @delegate in invocationList)
				{
					Func<string> func = (Func<string>)@delegate;
					text = text + "\n" + func();
				}
			}
			return text;
		}

		public override void AddTo(Attributes attributes)
		{
			base.AddTo(attributes);
			ChoreConsumer component = attributes.gameObject.GetComponent<ChoreConsumer>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && disabledChoreGroups != null)
			{
				ChoreGroup[] array = disabledChoreGroups;
				foreach (ChoreGroup chore_group in array)
				{
					component.SetPermittedByTraits(chore_group, false);
				}
			}
		}

		public override void RemoveFrom(Attributes attributes)
		{
			base.RemoveFrom(attributes);
			ChoreConsumer component = attributes.gameObject.GetComponent<ChoreConsumer>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && disabledChoreGroups != null)
			{
				ChoreGroup[] array = disabledChoreGroups;
				foreach (ChoreGroup chore_group in array)
				{
					component.SetPermittedByTraits(chore_group, true);
				}
			}
		}
	}
}
