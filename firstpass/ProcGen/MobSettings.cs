using System;

namespace ProcGen
{
	[Serializable]
	public class MobSettings : IMerge<MobSettings>
	{
		public static float AmbientMobDensity = 1f;

		private TagSet mobkeys;

		public ComposableDictionary<string, Mob> MobLookupTable
		{
			get;
			private set;
		}

		public MobSettings()
		{
			MobLookupTable = new ComposableDictionary<string, Mob>();
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
				foreach (string key in MobLookupTable.Keys)
				{
					mobkeys.Add(new Tag(key));
				}
			}
			return mobkeys;
		}

		public void Merge(MobSettings other)
		{
			MobLookupTable.Merge(other.MobLookupTable);
			mobkeys = null;
		}
	}
}
