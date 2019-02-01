using KSerialization;
using System.Collections.Generic;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Traits : KMonoBehaviour, ISaveLoadable
	{
		public List<Trait> TraitList = new List<Trait>();

		[Serialize]
		private List<string> TraitIds = new List<string>();

		public List<string> GetTraitIds()
		{
			return TraitIds;
		}

		public void SetTraitIds(List<string> traits)
		{
			TraitIds = traits;
		}

		protected override void OnSpawn()
		{
			foreach (string traitId in TraitIds)
			{
				if (Db.Get().traits.Exists(traitId))
				{
					Trait trait = Db.Get().traits.Get(traitId);
					AddInternal(trait);
				}
			}
		}

		private void AddInternal(Trait trait)
		{
			if (!HasTrait(trait))
			{
				TraitList.Add(trait);
				trait.AddTo(this.GetAttributes());
				if (trait.OnAddTrait != null)
				{
					trait.OnAddTrait(base.gameObject);
				}
			}
		}

		public void Add(Trait trait)
		{
			if (trait.ShouldSave)
			{
				TraitIds.Add(trait.Id);
			}
			AddInternal(trait);
		}

		public bool HasTrait(string trait_id)
		{
			bool result = false;
			foreach (Trait trait in TraitList)
			{
				if (trait.Id == trait_id)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool HasTrait(Trait trait)
		{
			foreach (Trait trait2 in TraitList)
			{
				if (trait2 == trait)
				{
					return true;
				}
			}
			return false;
		}

		public void Clear()
		{
			while (TraitList.Count > 0)
			{
				Remove(TraitList[0]);
			}
		}

		public void Remove(Trait trait)
		{
			int num = 0;
			while (true)
			{
				if (num >= TraitList.Count)
				{
					return;
				}
				if (TraitList[num] == trait)
				{
					break;
				}
				num++;
			}
			TraitList.RemoveAt(num);
			TraitIds.Remove(trait.Id);
			trait.RemoveFrom(this.GetAttributes());
		}
	}
}
