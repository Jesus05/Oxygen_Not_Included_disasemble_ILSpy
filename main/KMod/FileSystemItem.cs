namespace KMod
{
	public struct FileSystemItem
	{
		public enum ItemType
		{
			Directory,
			File
		}

		public string name;

		public ItemType type;
	}
}
