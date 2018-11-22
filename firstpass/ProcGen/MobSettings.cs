using Klei;
using System.Collections.Generic;

namespace ProcGen
{
	public class MobSettings : YamlIO<MobSettings>
	{
		public static int AmbientMobDensity = 1;

		private TagSet mobkeys = null;

		public Dictionary<string, Mob> MobLookupTable
		{
			get;
			private set;
		}

		public MobSettings()
		{
			MobLookupTable = new Dictionary<string, Mob>();
		}

		public bool HasMob(string id)
		{
			return MobLookupTable.ContainsKey(id);
		}

		public Mob GetMob(string id)
		{
			Mob value = null;
			MobLookupTable.TryGetValue(id, out value);
			return value;
		}

		public TagSet GetMobTags()
		{
			if (mobkeys == null)
			{
				mobkeys = new TagSet();
				Dictionary<string, Mob>.Enumerator enumerator = MobLookupTable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					mobkeys.Add(new Tag(enumerator.Current.Key));
				}
			}
			return mobkeys;
		}
	}
}
