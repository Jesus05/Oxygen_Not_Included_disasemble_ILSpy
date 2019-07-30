namespace KMod
{
	public static class Testing
	{
		public enum DLLLoading
		{
			NoTesting,
			Fail,
			UseModLoaderDLLExclusively
		}

		public enum SaveLoad
		{
			NoTesting,
			FailSave,
			FailLoad
		}

		public enum Install
		{
			NoTesting,
			ForceUninstall,
			ForceReinstall,
			ForceUpdate
		}

		public enum Boot
		{
			NoTesting,
			Crash
		}

		public static DLLLoading dll_loading;

		public const SaveLoad SAVE_LOAD = SaveLoad.NoTesting;

		public const Install INSTALL = Install.NoTesting;

		public const Boot BOOT = Boot.NoTesting;
	}
}
