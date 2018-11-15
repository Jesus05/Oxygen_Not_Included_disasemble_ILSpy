using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class AttributeModifierDisease : Disease.DiseaseComponent
	{
		private AttributeModifier[] attributeModifiers;

		public AttributeModifier[] Modifers => attributeModifiers;

		public AttributeModifierDisease(AttributeModifier[] attribute_modifiers)
		{
			attributeModifiers = attribute_modifiers;
		}

		public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
		{
			Attributes attributes = go.GetAttributes();
			for (int i = 0; i < attributeModifiers.Length; i++)
			{
				AttributeModifier modifier = attributeModifiers[i];
				attributes.Add(modifier);
			}
			return null;
		}

		public override void OnCure(GameObject go, object instance_data)
		{
			Attributes attributes = go.GetAttributes();
			for (int i = 0; i < attributeModifiers.Length; i++)
			{
				AttributeModifier modifier = attributeModifiers[i];
				attributes.Remove(modifier);
			}
		}

		public override List<Descriptor> GetSymptoms()
		{
			List<Descriptor> list = new List<Descriptor>();
			AttributeModifier[] array = attributeModifiers;
			foreach (AttributeModifier attributeModifier in array)
			{
				Attribute attribute = Db.Get().Attributes.Get(attributeModifier.AttributeId);
				list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.ATTRIBUTE_MODIFIER_SYMPTOMS, attribute.Name, attributeModifier.GetFormattedString(null, false)), string.Format(DUPLICANTS.DISEASES.ATTRIBUTE_MODIFIER_SYMPTOMS_TOOLTIP, attribute.Name, attributeModifier.GetFormattedString(null, false)), Descriptor.DescriptorType.Symptom, false));
			}
			return list;
		}
	}
}
