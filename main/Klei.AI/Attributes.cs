using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class Attributes
	{
		public List<AttributeInstance> AttributeTable = new List<AttributeInstance>();

		public GameObject gameObject;

		public int Count => AttributeTable.Count;

		public Attributes(GameObject game_object)
		{
			gameObject = game_object;
		}

		public IEnumerator<AttributeInstance> GetEnumerator()
		{
			return AttributeTable.GetEnumerator();
		}

		public AttributeInstance Add(Attribute attribute)
		{
			AttributeInstance attributeInstance = Get(attribute.Id);
			if (attributeInstance == null)
			{
				attributeInstance = new AttributeInstance(gameObject, attribute);
				AttributeTable.Add(attributeInstance);
			}
			return attributeInstance;
		}

		public void Add(AttributeModifier modifier)
		{
			Get(modifier.AttributeId)?.Add(modifier);
		}

		public float GetValuePercent(string attribute_id)
		{
			float result = 1f;
			AttributeInstance attributeInstance = Get(attribute_id);
			if (attributeInstance != null)
			{
				result = attributeInstance.GetTotalValue() / attributeInstance.GetBaseValue();
			}
			else
			{
				Debug.LogError("Could not find attribute " + attribute_id, null);
			}
			return result;
		}

		public AttributeInstance Get(string attribute_id)
		{
			for (int i = 0; i < AttributeTable.Count; i++)
			{
				if (AttributeTable[i].Id == attribute_id)
				{
					return AttributeTable[i];
				}
			}
			return null;
		}

		public AttributeInstance Get(Attribute attribute)
		{
			return Get(attribute.Id);
		}

		public float GetValue(string id)
		{
			float result = 0f;
			AttributeInstance attributeInstance = Get(id);
			if (attributeInstance != null)
			{
				result = attributeInstance.GetTotalValue();
			}
			else
			{
				Debug.LogError("Could not find attribute " + id, null);
			}
			return result;
		}

		public void Remove(AttributeModifier modifier)
		{
			if (modifier != null)
			{
				Get(modifier.AttributeId)?.Remove(modifier);
			}
		}

		public AttributeInstance GetProfession()
		{
			AttributeInstance attributeInstance = null;
			using (IEnumerator<AttributeInstance> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AttributeInstance current = enumerator.Current;
					if (current.modifier.IsProfession)
					{
						if (attributeInstance == null)
						{
							attributeInstance = current;
						}
						else if (attributeInstance.GetTotalValue() < current.GetTotalValue())
						{
							attributeInstance = current;
						}
					}
				}
				return attributeInstance;
			}
		}

		public string GetProfessionString(bool longform = true)
		{
			AttributeInstance profession = GetProfession();
			if ((int)profession.GetTotalValue() == 0)
			{
				return string.Format((!longform) ? UI.ATTRIBUTELEVEL_SHORT : UI.ATTRIBUTELEVEL, 0, DUPLICANTS.ATTRIBUTES.UNPROFESSIONAL_NAME);
			}
			return string.Format((!longform) ? UI.ATTRIBUTELEVEL_SHORT : UI.ATTRIBUTELEVEL, (int)profession.GetTotalValue(), profession.modifier.ProfessionName);
		}

		public string GetProfessionDescriptionString()
		{
			AttributeInstance profession = GetProfession();
			if ((int)profession.GetTotalValue() == 0)
			{
				return DUPLICANTS.ATTRIBUTES.UNPROFESSIONAL_DESC;
			}
			return string.Format(DUPLICANTS.ATTRIBUTES.PROFESSION_DESC, profession.modifier.Name);
		}
	}
}
