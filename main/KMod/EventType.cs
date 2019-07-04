namespace KMod
{
	public enum EventType
	{
		LoadError,
		NotFound,
		InstallInfoInaccessible,
		OutOfOrder,
		ExpectedActive,
		ExpectedInactive,
		ActiveDuringCrash,
		InstallFailed,
		Installed,
		Uninstalled,
		VersionUpdate,
		AvailableContentChanged,
		RestartRequested,
		BadWorldGen,
		Deactivated
	}
}
