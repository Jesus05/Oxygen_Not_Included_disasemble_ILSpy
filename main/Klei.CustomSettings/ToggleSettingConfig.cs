using System.Collections.Generic;

namespace Klei.CustomSettings
{
	public class ToggleSettingConfig : SettingConfig
	{
		public SettingLevel on_level
		{
			get;
			private set;
		}

		public SettingLevel off_level
		{
			get;
			private set;
		}

		public ToggleSettingConfig(string id, string label, string tooltip, SettingLevel off_level, SettingLevel on_level, string default_level_id, string nosweat_default_level_id, int coordinate_dimension = -1, int coordinate_dimension_width = -1, bool debug_only = false)
			: base(id, label, tooltip, default_level_id, nosweat_default_level_id, coordinate_dimension, coordinate_dimension_width, debug_only, true)
		{
			this.off_level = off_level;
			this.on_level = on_level;
		}

		public override SettingLevel GetLevel(string level_id)
		{
			if (!(on_level.id == level_id))
			{
				if (!(off_level.id == level_id))
				{
					if (!(base.default_level_id == on_level.id))
					{
						if (!(base.default_level_id == off_level.id))
						{
							Debug.LogError("Unable to find setting level for setting:" + base.id + " level: " + level_id);
							return null;
						}
						Debug.LogWarning("Unable to find level for setting:" + base.id + "(" + level_id + ") Using default level.");
						return off_level;
					}
					Debug.LogWarning("Unable to find level for setting:" + base.id + "(" + level_id + ") Using default level.");
					return on_level;
				}
				return off_level;
			}
			return on_level;
		}

		public override List<SettingLevel> GetLevels()
		{
			List<SettingLevel> list = new List<SettingLevel>();
			list.Add(off_level);
			list.Add(on_level);
			return list;
		}

		public string ToggleSettingLevelID(string current_id)
		{
			if (!(on_level.id == current_id))
			{
				return on_level.id;
			}
			return off_level.id;
		}

		public bool IsOnLevel(string level_id)
		{
			return level_id == on_level.id;
		}
	}
}
