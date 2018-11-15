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

		public Diseases diseases;

		public string[] initialTraits;

		public List<string> initialAmounts = new List<string>();

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			amounts = new Amounts(base.gameObject);
			diseases = new Diseases(base.gameObject);
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
			diseases.Serialize(writer);
		}

		public virtual void OnDeserialize(IReader reader)
		{
			amounts.Deserialize(reader);
			diseases.Deserialize(reader);
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
