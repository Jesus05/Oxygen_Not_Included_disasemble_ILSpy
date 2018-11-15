namespace Klei.CustomSettings
{
	public class SettingLevel
	{
		public string id
		{
			get;
			private set;
		}

		public string tooltip
		{
			get;
			private set;
		}

		public string label
		{
			get;
			private set;
		}

		public object userdata
		{
			get;
			private set;
		}

		public SettingLevel(string id, string label, string tooltip, object userdata = null)
		{
			this.id = id;
			this.label = label;
			this.tooltip = tooltip;
			this.userdata = userdata;
		}
	}
}
