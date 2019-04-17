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

		public static SaveLoad save_load;

		public static Install install;

		public static Boot boot;
	}
}
