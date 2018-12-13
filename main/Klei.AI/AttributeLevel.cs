using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TUNING;
using UnityEngine;

namespace Klei.AI
{
	public class AttributeLevel
	{
		public float experience;

		public int level;

		public AttributeInstance attribute;

		public AttributeModifier modifier;

		public Notification notification;

		[CompilerGenerated]
		private static Func<List<Notification>, object, string> _003C_003Ef__mg_0024cache0;

		public AttributeLevel(AttributeInstance attribute)
		{
			notification = new Notification(MISC.NOTIFICATIONS.LEVELUP.NAME, NotificationType.Good, HashedString.Invalid, OnLevelUpTooltip, null, true, 0f, null, null);
			this.attribute = attribute;
		}

		public int GetLevel()
		{
			return level;
		}

		public void Apply(AttributeLevels levels)
		{
			Attributes attributes = levels.GetAttributes();
			if (modifier != null)
			{
				attributes.Remove(modifier);
				modifier = null;
			}
			modifier = new AttributeModifier(attribute.Id, (float)GetLevel(), DUPLICANTS.MODIFIERS.SKILLLEVEL.NAME, false, false, true);
			attributes.Add(modifier);
		}

		public void SetExperience(float experience)
		{
			this.experience = experience;
		}

		public void SetLevel(int level)
		{
			this.level = level;
		}

		public float GetExperienceForNextLevel()
		{
			float f = (float)level / (float)DUPLICANTSTATS.ATTRIBUTE_LEVELING.MAX_GAINED_ATTRIBUTE_LEVEL;
			float num = Mathf.Pow(f, DUPLICANTSTATS.ATTRIBUTE_LEVELING.EXPERIENCE_LEVEL_POWER);
			float num2 = num * (float)DUPLICANTSTATS.ATTRIBUTE_LEVELING.TARGET_MAX_LEVEL_CYCLE * 600f;
			float f2 = ((float)level + 1f) / (float)DUPLICANTSTATS.ATTRIBUTE_LEVELING.MAX_GAINED_ATTRIBUTE_LEVEL;
			float num3 = Mathf.Pow(f2, DUPLICANTSTATS.ATTRIBUTE_LEVELING.EXPERIENCE_LEVEL_POWER);
			float num4 = num3 * (float)DUPLICANTSTATS.ATTRIBUTE_LEVELING.TARGET_MAX_LEVEL_CYCLE * 600f;
			return num4 - num2;
		}

		public float GetPercentComplete()
		{
			return experience / GetExperienceForNextLevel();
		}

		public void LevelUp(AttributeLevels levels)
		{
			level++;
			experience = 0f;
			Apply(levels);
			experience = 0f;
			if ((UnityEngine.Object)PopFXManager.Instance != (UnityEngine.Object)null)
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, attribute.modifier.Name, levels.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
			}
			levels.GetComponent<Notifier>().Add(notification, string.Format(MISC.NOTIFICATIONS.LEVELUP.SUFFIX, attribute.modifier.Name, level));
			StateMachine.Instance instance = new UpgradeFX.Instance(levels.GetComponent<KMonoBehaviour>(), new Vector3(0f, 0f, -0.1f));
			ReportManager.Instance.ReportValue(ReportManager.ReportType.LevelUp, 1f, levels.GetProperName(), null);
			instance.StartSM();
			levels.Trigger(-110704193, attribute.Id);
		}

		public bool AddExperience(AttributeLevels levels, float experience)
		{
			if (level >= DUPLICANTSTATS.ATTRIBUTE_LEVELING.MAX_GAINED_ATTRIBUTE_LEVEL)
			{
				return false;
			}
			this.experience += experience;
			this.experience = Mathf.Max(0f, this.experience);
			if (this.experience >= GetExperienceForNextLevel())
			{
				LevelUp(levels);
				return true;
			}
			return false;
		}

		private static string OnLevelUpTooltip(List<Notification> notifications, object data)
		{
			return MISC.NOTIFICATIONS.LEVELUP.TOOLTIP + notifications.ReduceMessages(false);
		}
	}
}
