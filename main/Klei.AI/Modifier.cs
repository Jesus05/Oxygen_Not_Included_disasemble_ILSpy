using System.Collections.Generic;

namespace Klei.AI
{
	public class Modifier : Resource
	{
		public string description;

		public List<AttributeModifier> SelfModifiers = new List<AttributeModifier>();

		public Modifier(string id, string name, string description)
			: base(id, name)
		{
			this.description = description;
		}

		public void Add(AttributeModifier modifier)
		{
			if (modifier.AttributeId != string.Empty)
			{
				SelfModifiers.Add(modifier);
			}
		}

		public virtual void AddTo(Attributes attributes)
		{
			foreach (AttributeModifier selfModifier in SelfModifiers)
			{
				attributes.Add(selfModifier);
			}
		}

		public virtual void RemoveFrom(Attributes attributes)
		{
			foreach (AttributeModifier selfModifier in SelfModifiers)
			{
				attributes.Remove(selfModifier);
			}
		}
	}
}
