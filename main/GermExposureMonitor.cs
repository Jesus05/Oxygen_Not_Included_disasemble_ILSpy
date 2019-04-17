using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class GermExposureMonitor : GameStateMachine<GermExposureMonitor, GermExposureMonitor.Instance>
{
	public enum ExposureState
	{
		None,
		Exposed,
		Contracted,
		Sick
	}

	public class ExposureType
	{
		public string germ_id;

		public string sickness_id;

		public string infection_effect;

		public int exposure_threshold = 100;

		public float contraction_rate;

		public bool infect_immediately;

		public List<string> required_traits;

		public List<string> excluded_traits;

		public List<string> excluded_effects;
	}

	public new class Instance : GameInstance
	{
		public class DiseaseSourceInfo
		{
			public Tag sourceObject;

			public Sickness.InfectionVector vector;

			public float factor;

			public DiseaseSourceInfo(Tag sourceObject, Sickness.InfectionVector vector, float factor)
			{
				this.sourceObject = sourceObject;
				this.vector = vector;
				this.factor = factor;
			}
		}

		[Serialize]
		public Dictionary<HashedString, DiseaseSourceInfo> lastDiseaseSources;

		private Sicknesses sicknesses;

		private PrimaryElement primaryElement;

		private Modifiers modifiers;

		private Traits traits;

		private Dictionary<string, ExposureState> exposureStates = new Dictionary<string, ExposureState>();

		private Dictionary<string, Guid> statusItemHandles = new Dictionary<string, Guid>();

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			sicknesses = master.GetComponent<MinionModifiers>().sicknesses;
			primaryElement = master.GetComponent<PrimaryElement>();
			traits = master.GetComponent<Traits>();
			lastDiseaseSources = new Dictionary<HashedString, DiseaseSourceInfo>();
			GameClock.Instance.Subscribe(-722330267, OnNightTime);
			modifiers = base.gameObject.GetComponent<Modifiers>();
			OxygenBreather component = GetComponent<OxygenBreather>();
			component.onSimConsume = (Action<Sim.MassConsumedCallback>)Delegate.Combine(component.onSimConsume, new Action<Sim.MassConsumedCallback>(OnAirConsumed));
		}

		public override void StartSM()
		{
			base.StartSM();
		}

		public override void StopSM(string reason)
		{
			GameClock.Instance.Unsubscribe(-722330267, OnNightTime);
			base.StopSM(reason);
		}

		public void OnEatComplete(object obj)
		{
			Edible edible = (Edible)obj;
			HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(edible.gameObject);
			if (handle != HandleVector<int>.InvalidHandle)
			{
				DiseaseHeader header = GameComps.DiseaseContainers.GetHeader(handle);
				if (header.diseaseIdx != 255)
				{
					Disease disease = Db.Get().Diseases[header.diseaseIdx];
					float num = edible.unitsConsumed / (edible.unitsConsumed + edible.Units);
					int num2 = Mathf.CeilToInt((float)header.diseaseCount * num);
					GameComps.DiseaseContainers.ModifyDiseaseCount(handle, -num2);
					KPrefabID component = edible.GetComponent<KPrefabID>();
					InjectDisease(disease, num2, component.PrefabID(), Sickness.InfectionVector.Digestion);
				}
			}
		}

		public void OnAirConsumed(Sim.MassConsumedCallback mass_cb_info)
		{
			if (mass_cb_info.diseaseIdx != 255)
			{
				Disease disease = Db.Get().Diseases[mass_cb_info.diseaseIdx];
				InjectDisease(disease, mass_cb_info.diseaseCount, ElementLoader.elements[mass_cb_info.elemIdx].tag, Sickness.InfectionVector.Inhalation);
			}
		}

		public void TryInjectDisease(byte disease_idx, int count, Tag source, Sickness.InfectionVector vector)
		{
			if (disease_idx != 255)
			{
				Disease disease = Db.Get().Diseases[disease_idx];
				InjectDisease(disease, count, source, vector);
			}
		}

		public void InjectDisease(Disease disease, int count, Tag source, Sickness.InfectionVector vector)
		{
			ExposureType[] exposureTypes = GermExposureMonitor.exposureTypes;
			foreach (ExposureType exposureType in exposureTypes)
			{
				if (disease.id == (HashedString)exposureType.germ_id && count > exposureType.exposure_threshold && IsExposureValidForTraits(exposureType))
				{
					Sickness sickness = (exposureType.sickness_id == null) ? null : Db.Get().Sicknesses.Get(exposureType.sickness_id);
					if ((sickness == null || sickness.infectionVectors.Contains(vector)) && GetExposureState(exposureType.germ_id) == ExposureState.None)
					{
						AttributeInstance attributeInstance = Db.Get().Attributes.GermSusceptibility.Lookup(base.gameObject);
						float totalValue = attributeInstance.GetTotalValue();
						float num = exposureType.contraction_rate * totalValue;
						if (num > 0f)
						{
							lastDiseaseSources[disease.id] = new DiseaseSourceInfo(source, vector, num);
							if (exposureType.infect_immediately)
							{
								InfectImmediately(exposureType);
							}
							else
							{
								SetExposureState(exposureType.germ_id, ExposureState.Exposed);
								GermExposureTracker.Instance.AddExposure(exposureType, num);
							}
						}
					}
				}
			}
			RefreshStatusItems();
		}

		public ExposureState GetExposureState(string germ_id)
		{
			ExposureState value;
			bool flag = exposureStates.TryGetValue(germ_id, out value);
			return value;
		}

		public void SetExposureState(string germ_id, ExposureState exposure_state)
		{
			exposureStates[germ_id] = exposure_state;
			RefreshStatusItems();
		}

		public void ContractGerms(string germ_id)
		{
			ExposureState exposureState = GetExposureState(germ_id);
			DebugUtil.DevAssert(exposureState == ExposureState.Exposed, "Duplicant is contracting a sickness but was never exposed to it!");
			SetExposureState(germ_id, ExposureState.Contracted);
		}

		public void OnSicknessAdded(object sickness_instance_data)
		{
			SicknessInstance sicknessInstance = (SicknessInstance)sickness_instance_data;
			ExposureType[] exposureTypes = GermExposureMonitor.exposureTypes;
			foreach (ExposureType exposureType in exposureTypes)
			{
				if (exposureType.sickness_id == sicknessInstance.Sickness.Id)
				{
					SetExposureState(exposureType.germ_id, ExposureState.Sick);
				}
			}
		}

		public void OnSicknessCured(object sickness_instance_data)
		{
			SicknessInstance sicknessInstance = (SicknessInstance)sickness_instance_data;
			ExposureType[] exposureTypes = GermExposureMonitor.exposureTypes;
			foreach (ExposureType exposureType in exposureTypes)
			{
				if (exposureType.sickness_id == sicknessInstance.Sickness.Id)
				{
					SetExposureState(exposureType.germ_id, ExposureState.None);
				}
			}
		}

		private bool IsExposureValidForTraits(ExposureType exposure_type)
		{
			if (exposure_type.required_traits != null && exposure_type.required_traits.Count > 0)
			{
				foreach (string required_trait in exposure_type.required_traits)
				{
					if (!traits.HasTrait(required_trait))
					{
						return false;
					}
				}
			}
			if (exposure_type.excluded_traits != null && exposure_type.excluded_traits.Count > 0)
			{
				foreach (string excluded_trait in exposure_type.excluded_traits)
				{
					if (traits.HasTrait(excluded_trait))
					{
						return false;
					}
				}
			}
			if (exposure_type.excluded_effects != null && exposure_type.excluded_effects.Count > 0)
			{
				Effects component = base.master.GetComponent<Effects>();
				foreach (string excluded_effect in exposure_type.excluded_effects)
				{
					if (component.HasEffect(excluded_effect))
					{
						return false;
					}
				}
			}
			return true;
		}

		private void RefreshStatusItems()
		{
			ExposureType[] exposureTypes = GermExposureMonitor.exposureTypes;
			foreach (ExposureType exposureType in exposureTypes)
			{
				statusItemHandles.TryGetValue(exposureType.germ_id, out Guid value);
				ExposureState exposureState = GetExposureState(exposureType.germ_id);
				if (value == Guid.Empty && (exposureState == ExposureState.Exposed || exposureState == ExposureState.Contracted))
				{
					KSelectable component = GetComponent<KSelectable>();
					value = component.AddStatusItem(Db.Get().DuplicantStatusItems.ExposedToGerms, exposureType.sickness_id);
				}
				else if (value != Guid.Empty && exposureState != ExposureState.Exposed && exposureState != ExposureState.Contracted)
				{
					KSelectable component2 = GetComponent<KSelectable>();
					value = component2.RemoveStatusItem(Db.Get().DuplicantStatusItems.ExposedToGerms, false);
				}
				statusItemHandles[exposureType.germ_id] = value;
			}
		}

		private void OnNightTime(object data)
		{
			UpdateReports();
		}

		private void UpdateReports()
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseStatus, (float)primaryElement.DiseaseCount, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.GERMS, "{0}", base.master.name), base.master.gameObject.GetProperName());
		}

		public void InfectImmediately(ExposureType exposure_type)
		{
			if (exposure_type.infection_effect != null)
			{
				Effects component = base.master.GetComponent<Effects>();
				component.Add(exposure_type.infection_effect, true);
			}
			if (exposure_type.sickness_id != null)
			{
				string lastDiseaseSource = GetLastDiseaseSource(exposure_type.germ_id);
				SicknessExposureInfo exposure_info = new SicknessExposureInfo(exposure_type.sickness_id, lastDiseaseSource);
				sicknesses.Infect(exposure_info);
			}
		}

		public void OnSleepFinished()
		{
			ExposureType[] exposureTypes = GermExposureMonitor.exposureTypes;
			foreach (ExposureType exposureType in exposureTypes)
			{
				if (!exposureType.infect_immediately)
				{
					ExposureState exposureState = GetExposureState(exposureType.germ_id);
					if (exposureState == ExposureState.Exposed)
					{
						SetExposureState(exposureType.germ_id, ExposureState.None);
					}
					if (exposureState == ExposureState.Contracted)
					{
						SetExposureState(exposureType.germ_id, ExposureState.Sick);
						string lastDiseaseSource = GetLastDiseaseSource(exposureType.germ_id);
						SicknessExposureInfo exposure_info = new SicknessExposureInfo(exposureType.sickness_id, lastDiseaseSource);
						sicknesses.Infect(exposure_info);
					}
				}
			}
		}

		private string GetLastDiseaseSource(string id)
		{
			if (lastDiseaseSources.TryGetValue(id, out DiseaseSourceInfo value))
			{
				switch (value.vector)
				{
				default:
					return DUPLICANTS.DISEASES.INFECTIONSOURCES.UNKNOWN;
				case Sickness.InfectionVector.Contact:
					return DUPLICANTS.DISEASES.INFECTIONSOURCES.SKIN;
				case Sickness.InfectionVector.Inhalation:
					return string.Format(DUPLICANTS.DISEASES.INFECTIONSOURCES.AIR, value.sourceObject.ProperName());
				case Sickness.InfectionVector.Digestion:
					return string.Format(DUPLICANTS.DISEASES.INFECTIONSOURCES.FOOD, value.sourceObject.ProperName());
				}
			}
			return DUPLICANTS.DISEASES.INFECTIONSOURCES.UNKNOWN;
		}

		public float GetExposureWeight(string id)
		{
			if (lastDiseaseSources.TryGetValue(id, out DiseaseSourceInfo value))
			{
				return value.factor;
			}
			return 0f;
		}
	}

	private const int MIN_GERM_EXPOSURE_THRESHOLD = 100;

	public static ExposureType[] exposureTypes = new ExposureType[5]
	{
		new ExposureType
		{
			germ_id = "FoodPoisoning",
			sickness_id = "FoodSickness",
			contraction_rate = 0.3f,
			excluded_traits = new List<string>
			{
				"IronGut"
			}
		},
		new ExposureType
		{
			germ_id = "SlimeLung",
			sickness_id = "SlimeSickness",
			contraction_rate = 0.3f
		},
		new ExposureType
		{
			germ_id = "ZombieSpores",
			sickness_id = "ZombieSickness",
			exposure_threshold = 1,
			contraction_rate = 0.9f
		},
		new ExposureType
		{
			germ_id = "PollenGerms",
			sickness_id = "Allergies",
			exposure_threshold = 1,
			contraction_rate = 0.9f,
			infect_immediately = true,
			required_traits = new List<string>
			{
				"Allergies"
			},
			excluded_effects = new List<string>
			{
				"HistamineSuppression"
			}
		},
		new ExposureType
		{
			germ_id = "PollenGerms",
			infection_effect = "SmelledFlowers",
			exposure_threshold = 1,
			contraction_rate = 0.9f,
			infect_immediately = true,
			excluded_traits = new List<string>
			{
				"Allergies"
			}
		}
	};

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		base.serializable = false;
		root.EventHandler(GameHashes.EatCompleteEater, delegate(Instance smi, object obj)
		{
			smi.OnEatComplete(obj);
		}).EventHandler(GameHashes.SicknessAdded, delegate(Instance smi, object data)
		{
			smi.OnSicknessAdded(data);
		}).EventHandler(GameHashes.SicknessCured, delegate(Instance smi, object data)
		{
			smi.OnSicknessCured(data);
		})
			.EventHandler(GameHashes.SleepFinished, delegate(Instance smi)
			{
				smi.OnSleepFinished();
			});
	}
}
