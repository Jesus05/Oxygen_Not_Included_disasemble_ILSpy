using STRINGS;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{base.Id}")]
	public abstract class Sickness : Resource
	{
		public abstract class SicknessComponent
		{
			public abstract object OnInfect(GameObject go, SicknessInstance diseaseInstance);

			public abstract void OnCure(GameObject go, object instance_data);

			public virtual List<Descriptor> GetSymptoms()
			{
				return null;
			}
		}

		public enum InfectionVector
		{
			Contact,
			Digestion,
			Inhalation,
			Exposure
		}

		public enum SicknessType
		{
			Pathogen,
			Ailment,
			Injury
		}

		public enum Severity
		{
			Benign,
			Minor,
			Major,
			Critical
		}

		private StringKey name;

		private StringKey descriptiveSymptoms;

		private float sicknessDuration = 600f;

		public float fatalityDuration = 0f;

		public HashedString id;

		public SicknessType sicknessType;

		public Severity severity;

		public string recoveryEffect;

		public List<InfectionVector> infectionVectors;

		private List<SicknessComponent> components = new List<SicknessComponent>();

		public Amount amount;

		public Attribute amountDeltaAttribute;

		public Attribute cureSpeedBase;

		public new string Name => Strings.Get(name);

		public float SicknessDuration => sicknessDuration;

		public StringKey DescriptiveSymptoms => descriptiveSymptoms;

		public Sickness(string id, SicknessType type, Severity severity, float immune_attack_strength, List<InfectionVector> infection_vectors, float sickness_duration, string recovery_effect = null)
			: base(id, null, null)
		{
			name = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".NAME");
			this.id = id;
			sicknessType = type;
			this.severity = severity;
			infectionVectors = infection_vectors;
			sicknessDuration = sickness_duration;
			recoveryEffect = recovery_effect;
			descriptiveSymptoms = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".DESCRIPTIVE_SYMPTOMS");
			cureSpeedBase = new Attribute(id + "CureSpeed", false, Attribute.Display.Normal, false, 0f, null, null);
			cureSpeedBase.BaseValue = 1f;
			cureSpeedBase.SetFormatter(new ToPercentAttributeFormatter(1f, GameUtil.TimeSlice.None));
			Db.Get().Attributes.Add(cureSpeedBase);
		}

		public object[] Infect(GameObject go, SicknessInstance diseaseInstance, SicknessExposureInfo exposure_info)
		{
			object[] array = new object[components.Count];
			for (int i = 0; i < components.Count; i++)
			{
				array[i] = components[i].OnInfect(go, diseaseInstance);
			}
			return array;
		}

		public void Cure(GameObject go, object[] componentData)
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].OnCure(go, componentData[i]);
			}
		}

		public List<Descriptor> GetSymptoms()
		{
			List<Descriptor> list = new List<Descriptor>();
			for (int i = 0; i < components.Count; i++)
			{
				List<Descriptor> symptoms = components[i].GetSymptoms();
				if (symptoms != null)
				{
					list.AddRange(symptoms);
				}
			}
			if (fatalityDuration > 0f)
			{
				list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.DEATH_SYMPTOM, GameUtil.GetFormattedCycles(fatalityDuration, "F1")), string.Format(DUPLICANTS.DISEASES.DEATH_SYMPTOM_TOOLTIP, GameUtil.GetFormattedCycles(fatalityDuration, "F1")), Descriptor.DescriptorType.SymptomAidable, false));
			}
			return list;
		}

		protected void AddSicknessComponent(SicknessComponent cmp)
		{
			components.Add(cmp);
		}

		public T GetSicknessComponent<T>() where T : SicknessComponent
		{
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i] is T)
				{
					return components[i] as T;
				}
			}
			return (T)null;
		}

		public virtual List<Descriptor> GetSicknessSourceDescriptors()
		{
			return new List<Descriptor>();
		}

		public List<Descriptor> GetQualitativeDescriptors()
		{
			List<Descriptor> list = new List<Descriptor>();
			using (List<InfectionVector>.Enumerator enumerator = infectionVectors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case InfectionVector.Contact:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SKINBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SKINBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					case InfectionVector.Inhalation:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.AIRBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.AIRBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					case InfectionVector.Digestion:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.FOODBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.FOODBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					case InfectionVector.Exposure:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SUNBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SUNBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					}
				}
			}
			list.Add(new Descriptor(Strings.Get(descriptiveSymptoms), "", Descriptor.DescriptorType.Information, false));
			return list;
		}
	}
}
