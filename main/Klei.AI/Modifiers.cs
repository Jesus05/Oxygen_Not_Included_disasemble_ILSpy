using KSerialization;
using System.Collections.Generic;
using System.IO;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Modifiers : KMonoBehaviour, ISaveLoadableDetails
	{
		public Amounts amounts;

		public Attributes attributes;

		public Sicknesses sicknesses;

		public string[] initialTraits;

		public List<string> initialAmounts = new List<string>();

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			amounts = new Amounts(base.gameObject);
			sicknesses = new Sicknesses(base.gameObject);
			attributes = new Attributes(base.gameObject);
			foreach (string initialAmount in initialAmounts)
			{
				amounts.Add(new AmountInstance(Db.Get().Amounts.Get(initialAmount), base.gameObject));
			}
			Traits component = GetComponent<Traits>();
			if (initialTraits != null)
			{
				string[] array = initialTraits;
				foreach (string id in array)
				{
					Trait trait = Db.Get().traits.Get(id);
					component.Add(trait);
				}
			}
		}

		public void Serialize(BinaryWriter writer)
		{
			OnSerialize(writer);
		}

		public void Deserialize(IReader reader)
		{
			OnDeserialize(reader);
		}

		public virtual void OnSerialize(BinaryWriter writer)
		{
			amounts.Serialize(writer);
			sicknesses.Serialize(writer);
		}

		public virtual void OnDeserialize(IReader reader)
		{
			amounts.Deserialize(reader);
			sicknesses.Deserialize(reader);
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			if (amounts != null)
			{
				amounts.Cleanup();
			}
		}
	}
}
