namespace Klei.CustomSettings
{
	public abstract class SettingConfig
	{
		public string id
		{
			get;
			private set;
		}

		public string label
		{
			get;
			private set;
		}

		public string tooltip
		{
			get;
			private set;
		}

		public string default_level_id
		{
			get;
			protected set;
		}

		public string nosweat_default_level_id
		{
			get;
			protected set;
		}

		public bool debug_only
		{
			get;
			protected set;
		}

		public SettingConfig(string id, string label, string tooltip, string default_level_id, string nosweat_default_level_id, bool debug_only)
		{
			this.id = id;
			this.label = label;
			this.tooltip = tooltip;
			this.default_level_id = default_level_id;
			this.nosweat_default_level_id = nosweat_default_level_id;
			this.debug_only = debug_only;
		}

		public abstract SettingLevel GetLevel(string level_id);

		public bool IsDefaultLevel(string level_id)
		{
			return level_id == default_level_id;
		}
	}
}
