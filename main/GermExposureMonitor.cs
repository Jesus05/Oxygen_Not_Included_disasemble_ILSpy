using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GermExposureMonitor : GameStateMachine<GermExposureMonitor, GermExposureMonitor.Instance>
{
	public enum ExposureState
	{
		None,
		Contact,
		Exposed,
		Contracted,
		Sick
	}

	public class ExposureStatusData
	{
		public ExposureType exposure_type;

		public Instance owner;
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public new class Instance : GameInstance
	{
		[Serializable]
		public class DiseaseSourceInfo
		{
			public Tag sourceObject;

			public Sickness.InfectionVector vector;

			public float factor;

			public Vector3 position;

			public DiseaseSourceInfo(Tag sourceObject, Sickness.InfectionVector vector, float factor, Vector3 position)
			{
				this.sourceObject = sourceObject;
				this.vector = vector;
				this.factor = factor;
				this.position = position;
			}
		}

		public class InhaleTickInfo
		{
			public bool inhaled;

			public int ticks;
		}

		[Serialize]
		public Dictionary<HashedString, DiseaseSourceInfo> lastDiseaseSources;

		[Serialize]
		public Dictionary<HashedString, float> lastExposureTime;

		private Dictionary<HashedString, InhaleTickInfo> inhaleExposureTick;

		private Sicknesses sicknesses;

		private PrimaryElement primaryElement;

		private Traits traits;

		[Serialize]
		private Dictionary<string, ExposureState> exposureStates = new Dictionary<string, ExposureState>();

		[Serialize]
		private Dictionary<string, float> exposureTiers = new Dictionary<string, float>();

		private Dictionary<string, Guid> statusItemHandles = new Dictionary<string, Guid>();

		private Dictionary<string, Guid> contactStatusItemHandles = new Dictionary<string, Guid>();

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			sicknesses = master.GetComponent<MinionModifiers>().sicknesses;
			primaryElement = master.GetComponent<PrimaryElement>();
			traits = master.GetComponent<Traits>();
			lastDiseaseSources = new Dictionary<HashedString, DiseaseSourceInfo>();
			lastExposureTime = new Dictionary<HashedString, float>();
			inhaleExposureTick = new Dictionary<HashedString, InhaleTickInfo>();
			GameClock.Instance.Subscribe(-722330267, OnNightTime);
			OxygenBreather component = GetComponent<OxygenBreather>();
			component.onSimConsume = (Action<Sim.MassConsumedCallback>)Delegate.Combine(component.onSimConsume, new Action<Sim.MassConsumedCallback>(OnAirConsumed));
		}

		public override void StartSM()
		{
			base.StartSM();
			RefreshStatusItems();
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

		public void OnInhaleExposureTick(float dt)
		{
			foreach (KeyValuePair<HashedString, InhaleTickInfo> item in inhaleExposureTick)
			{
				if (item.Value.inhaled)
				{
					item.Value.inhaled = false;
					item.Value.ticks++;
				}
				else
				{
					item.Value.ticks = Mathf.Max(0, item.Value.ticks - 1);
				}
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

		public float GetGermResistance()
		{
			return Db.Get().Attributes.GermResistance.Lookup(base.gameObject).GetTotalValue();
		}

		public float GetResistanceToExposureType(ExposureType exposureType, float overrideExposureTier = -1f)
		{
			float num = overrideExposureTier;
			if (num == -1f)
			{
				num = GetExposureTier(exposureType.germ_id);
			}
			num = Mathf.Clamp(num, 1f, 3f);
			float num2 = GERM_EXPOSURE.EXPOSURE_TIER_RESISTANCE_BONUSES[(int)num - 1];
			AttributeInstance attributeInstance = Db.Get().Attributes.GermResistance.Lookup(base.gameObject);
			float totalValue = attributeInstance.GetTotalValue();
			return (float)exposureType.base_resistance + totalValue + num2;
		}

		public int AssessDigestedGerms(ExposureType exposure_type, int count)
		{
			int exposure_threshold = exposure_type.exposure_threshold;
			int val = count / exposure_threshold;
			return MathUtil.Clamp(1, 3, val);
		}

		public bool AssessInhaledGerms(ExposureType exposure_type)
		{
			inhaleExposureTick.TryGetValue(exposure_type.germ_id, out InhaleTickInfo value);
			if (value == null)
			{
				value = new InhaleTickInfo();
				inhaleExposureTick[exposure_type.germ_id] = value;
			}
			if (value.inhaled)
			{
				return false;
			}
			float exposureTier = GetExposureTier(exposure_type.germ_id);
			value.inhaled = true;
			return value.ticks >= GERM_EXPOSURE.INHALE_TICK_THRESHOLD[(int)exposureTier];
		}

		public void InjectDisease(Disease disease, int count, Tag source, Sickness.InfectionVector vector)
		{
			ExposureType[] tYPES = GERM_EXPOSURE.TYPES;
			foreach (ExposureType exposureType in tYPES)
			{
				if (disease.id == (HashedString)exposureType.germ_id && count > exposureType.exposure_threshold && HasMinExposurePeriodElapsed(exposureType.germ_id) && IsExposureValidForTraits(exposureType))
				{
					Sickness sickness = (exposureType.sickness_id == null) ? null : Db.Get().Sicknesses.Get(exposureType.sickness_id);
					if (sickness == null || sickness.infectionVectors.Contains(vector))
					{
						ExposureState exposureState = GetExposureState(exposureType.germ_id);
						float exposureTier = GetExposureTier(exposureType.germ_id);
						switch (exposureState)
						{
						case ExposureState.None:
						case ExposureState.Contact:
						{
							float resistanceToExposureType3 = GetResistanceToExposureType(exposureType, -1f);
							float contractionChance3 = GetContractionChance(resistanceToExposureType3);
							SetExposureState(exposureType.germ_id, ExposureState.Contact);
							if (contractionChance3 > 0f)
							{
								lastDiseaseSources[disease.id] = new DiseaseSourceInfo(source, vector, contractionChance3, base.transform.GetPosition());
								if (exposureType.infect_immediately)
								{
									InfectImmediately(exposureType);
								}
								else
								{
									bool flag4 = true;
									bool flag5 = vector == Sickness.InfectionVector.Inhalation;
									bool flag6 = vector == Sickness.InfectionVector.Digestion;
									int num2 = 1;
									if (flag5)
									{
										flag4 = AssessInhaledGerms(exposureType);
									}
									if (flag6)
									{
										num2 = AssessDigestedGerms(exposureType, count);
									}
									if (flag4)
									{
										if (flag5)
										{
											inhaleExposureTick[exposureType.germ_id].ticks = 0;
										}
										SetExposureState(exposureType.germ_id, ExposureState.Exposed);
										SetExposureTier(exposureType.germ_id, (float)num2);
										float amount2 = Mathf.Clamp01(contractionChance3);
										GermExposureTracker.Instance.AddExposure(exposureType, amount2);
									}
								}
							}
							break;
						}
						case ExposureState.Exposed:
							if (exposureTier < 3f)
							{
								float resistanceToExposureType = GetResistanceToExposureType(exposureType, -1f);
								float contractionChance = GetContractionChance(resistanceToExposureType);
								if (contractionChance > 0f)
								{
									lastDiseaseSources[disease.id] = new DiseaseSourceInfo(source, vector, contractionChance, base.transform.GetPosition());
									if (!exposureType.infect_immediately)
									{
										bool flag = true;
										bool flag2 = vector == Sickness.InfectionVector.Inhalation;
										bool flag3 = vector == Sickness.InfectionVector.Digestion;
										int num = 1;
										if (flag2)
										{
											flag = AssessInhaledGerms(exposureType);
										}
										if (flag3)
										{
											num = AssessDigestedGerms(exposureType, count);
										}
										if (flag)
										{
											if (flag2)
											{
												inhaleExposureTick[exposureType.germ_id].ticks = 0;
											}
											SetExposureTier(exposureType.germ_id, GetExposureTier(exposureType.germ_id) + (float)num);
											float resistanceToExposureType2 = GetResistanceToExposureType(exposureType, -1f);
											float contractionChance2 = GetContractionChance(resistanceToExposureType2);
											float value = contractionChance2 - contractionChance;
											float amount = Mathf.Clamp01(value);
											GermExposureTracker.Instance.AddExposure(exposureType, amount);
										}
									}
								}
							}
							break;
						}
					}
				}
			}
			RefreshStatusItems();
		}

		public ExposureState GetExposureState(string germ_id)
		{
			exposureStates.TryGetValue(germ_id, out ExposureState value);
			return value;
		}

		public float GetExposureTier(string germ_id)
		{
			float value = 1f;
			exposureTiers.TryGetValue(germ_id, out value);
			return Mathf.Clamp(value, 1f, 3f);
		}

		public void SetExposureState(string germ_id, ExposureState exposure_state)
		{
			exposureStates[germ_id] = exposure_state;
			RefreshStatusItems();
		}

		public void SetExposureTier(string germ_id, float tier)
		{
			tier = Mathf.Clamp(tier, 0f, 3f);
			exposureTiers[germ_id] = tier;
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
			ExposureType[] tYPES = GERM_EXPOSURE.TYPES;
			foreach (ExposureType exposureType in tYPES)
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
			ExposureType[] tYPES = GERM_EXPOSURE.TYPES;
			foreach (ExposureType exposureType in tYPES)
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

		private bool HasMinExposurePeriodElapsed(string germ_id)
		{
			lastExposureTime.TryGetValue(germ_id, out float value);
			if (value != 0f)
			{
				float num = GameClock.Instance.GetTime() - value;
				return num > 540f;
			}
			return true;
		}

		private void RefreshStatusItems()
		{
			ExposureType[] tYPES = GERM_EXPOSURE.TYPES;
			foreach (ExposureType exposureType in tYPES)
			{
				contactStatusItemHandles.TryGetValue(exposureType.germ_id, out Guid value);
				statusItemHandles.TryGetValue(exposureType.germ_id, out Guid value2);
				ExposureState exposureState = GetExposureState(exposureType.germ_id);
				if (value2 == Guid.Empty && (exposureState == ExposureState.Exposed || exposureState == ExposureState.Contracted))
				{
					KSelectable component = GetComponent<KSelectable>();
					value2 = component.AddStatusItem(Db.Get().DuplicantStatusItems.ExposedToGerms, new ExposureStatusData
					{
						exposure_type = exposureType,
						owner = this
					});
				}
				else if (value2 != Guid.Empty && exposureState != ExposureState.Exposed && exposureState != ExposureState.Contracted)
				{
					KSelectable component2 = GetComponent<KSelectable>();
					value2 = component2.RemoveStatusItem(value2, false);
				}
				statusItemHandles[exposureType.germ_id] = value2;
				if (value == Guid.Empty && exposureState == ExposureState.Contact)
				{
					KSelectable component3 = GetComponent<KSelectable>();
					value = component3.AddStatusItem(Db.Get().DuplicantStatusItems.ContactWithGerms, new ExposureStatusData
					{
						exposure_type = exposureType,
						owner = this
					});
				}
				else if (value != Guid.Empty && exposureState != ExposureState.Contact)
				{
					KSelectable component4 = GetComponent<KSelectable>();
					value = component4.RemoveStatusItem(value, false);
				}
				contactStatusItemHandles[exposureType.germ_id] = value;
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
			ExposureType[] tYPES = GERM_EXPOSURE.TYPES;
			foreach (ExposureType exposureType in tYPES)
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
					SetExposureTier(exposureType.germ_id, 0f);
				}
			}
		}

		public string GetLastDiseaseSource(string id)
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

		public Vector3 GetLastExposurePosition(string germ_id)
		{
			if (!lastDiseaseSources.TryGetValue(germ_id, out DiseaseSourceInfo value))
			{
				return base.transform.GetPosition();
			}
			return value.position;
		}

		public float GetExposureWeight(string id)
		{
			float exposureTier = GetExposureTier(id);
			if (!lastDiseaseSources.TryGetValue(id, out DiseaseSourceInfo value))
			{
				return 0f;
			}
			return value.factor * exposureTier;
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		base.serializable = false;
		root.Update(delegate(Instance smi, float dt)
		{
			smi.OnInhaleExposureTick(dt);
		}, UpdateRate.SIM_1000ms, true).EventHandler(GameHashes.EatCompleteEater, delegate(Instance smi, object obj)
		{
			smi.OnEatComplete(obj);
		}).EventHandler(GameHashes.SicknessAdded, delegate(Instance smi, object data)
		{
			smi.OnSicknessAdded(data);
		})
			.EventHandler(GameHashes.SicknessCured, delegate(Instance smi, object data)
			{
				smi.OnSicknessCured(data);
			})
			.EventHandler(GameHashes.SleepFinished, delegate(Instance smi)
			{
				smi.OnSleepFinished();
			});
	}

	public static float GetContractionChance(float rating)
	{
		return 0.5f - 0.5f * (float)Math.Tanh(0.25 * (double)rating);
	}
}
