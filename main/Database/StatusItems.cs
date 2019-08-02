using System.Diagnostics;

namespace Database
{
	public class StatusItems : ResourceSet<StatusItem>
	{
		[DebuggerDisplay("{Id}")]
		public class StatusItemInfo : Resource
		{
			public string Type;

			public string Tooltip;

			public bool IsIconTinted;

			public StatusItem.IconType IconType;

			public string Icon;

			public string SoundPath;

			public bool ShouldNotify;

			public float NotificationDelay;

			public NotificationType NotificationType;

			public bool AllowMultiples;

			public string Effect;

			public HashedString Overlay;

			public HashedString SecondOverlay;
		}

		public StatusItems(string id, ResourceSet parent)
			: base(id, parent)
		{
		}
	}
}
