using System.Collections.Generic;
using UnityEngine;

namespace Klei.CustomSettings
{
	public class ListSettingConfig : SettingConfig
	{
		public List<SettingLevel> levels
		{
			get;
			private set;
		}

		public ListSettingConfig(string id, string label, string tooltip, List<SettingLevel> levels, string default_level_id, string nosweat_default_level_id, bool debug_only)
			: base(id, label, tooltip, default_level_id, nosweat_default_level_id, debug_only)
		{
			this.levels = levels;
		}

		public void StompLevels(List<SettingLevel> levels, string default_level_id, string nosweat_default_level_id)
		{
			this.levels = levels;
			base.default_level_id = default_level_id;
			base.nosweat_default_level_id = nosweat_default_level_id;
		}

		public override SettingLevel GetLevel(string level_id)
		{
			for (int i = 0; i < levels.Count; i++)
			{
				if (levels[i].id == level_id)
				{
					return levels[i];
				}
			}
			for (int j = 0; j < levels.Count; j++)
			{
				if (levels[j].id == base.default_level_id)
				{
					return levels[j];
				}
			}
			Debug.LogError("Unable to find setting level for setting:" + base.id + " level: " + level_id);
			return null;
		}

		public string CycleSettingLevelID(string current_id, int direction)
		{
			string result = string.Empty;
			if (current_id == string.Empty)
			{
				current_id = levels[0].id;
			}
			for (int i = 0; i < levels.Count; i++)
			{
				if (levels[i].id == current_id)
				{
					int index = Mathf.Clamp(i + direction, 0, levels.Count - 1);
					result = levels[index].id;
					break;
				}
			}
			return result;
		}

		public bool IsFirstLevel(string level_id)
		{
			return levels.FindIndex((SettingLevel l) => l.id == level_id) == 0;
		}

		public bool IsLastLevel(string level_id)
		{
			return levels.FindIndex((SettingLevel l) => l.id == level_id) == levels.Count - 1;
		}
	}
}
