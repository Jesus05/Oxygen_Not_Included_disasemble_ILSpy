using KMod;
using System.Collections.Generic;

namespace Klei
{
	internal class SaveFileRoot
	{
		public int WidthInCells;

		public int HeightInCells;

		public Dictionary<string, byte[]> streamed;

		public string worldID;

		public List<ModInfo> requiredMods;

		public List<Label> active_mods;

		public SaveFileRoot()
		{
			streamed = new Dictionary<string, byte[]>();
		}
	}
}
