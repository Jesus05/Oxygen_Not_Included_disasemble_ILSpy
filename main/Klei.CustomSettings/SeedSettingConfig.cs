namespace Klei.CustomSettings
{
	public class SeedSettingConfig : SettingConfig
	{
		public SeedSettingConfig(string id, string label, string tooltip, bool debug_only)
			: base(id, label, tooltip, string.Empty, string.Empty, debug_only)
		{
		}

		public override SettingLevel GetLevel(string level_id)
		{
			return null;
		}
	}
}
