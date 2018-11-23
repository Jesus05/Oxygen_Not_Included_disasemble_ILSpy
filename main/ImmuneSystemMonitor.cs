using Database;
using Klei.AI;
using Klei.CustomSettings;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ImmuneSystemMonitor : GameStateMachine<ImmuneSystemMonitor, ImmuneSystemMonitor.Instance>
{
	public class BelowToleranceState : State
	{
		public State high;

		public State low;
	}

	public new class Instance : GameInstance
	{
		public class DiseaseSourceInfo
		{
			public Tag sourceObject;

			public Disease.InfectionVector vector;

			public DiseaseSourceInfo(Tag sourceObject, Disease.InfectionVector vector)
			{
				this.sourceObject = sourceObject;
				this.vector = vector;
			}
		}

		private const float LOW_IMMUNE_LEVEL = 40f;

		[Serialize]
		public Dictionary<HashedString, DiseaseSourceInfo> lastDiseaseSources;

		public Dictionary<HashedString, AttributeModifier> activeImmuneModifiers;

		public Dictionary<HashedString, AttributeModifier> diseaseCountMultModifiers;

		private Dictionary<HashedString, AttributeModifier> attributeModifierPool = new Dictionary<HashedString, AttributeModifier>();

		private Klei.AI.Diseases activeDiseases;

		private PrimaryElement primaryElement;

		private Modifiers modifiers;

		public Effects effects;

		public AmountInstance immuneLevel;

		public AttributeModifier immuneSuppress;

		public AttributeConverterInstance immuneBoostConverter;

		private AttributeInstance immuneDelta;

		public Disease lastHighestDisease;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			immuneLevel = Db.Get().Amounts.ImmuneLevel.Lookup(base.gameObject);
			AmountInstance amountInstance = immuneLevel;
			amountInstance.OnDelta = (Action<float>)Delegate.Combine(amountInstance.OnDelta, new Action<float>(OnImmuneDelta));
			AttributeConverterInstance attributeConverterInstance = master.GetComponent<Klei.AI.AttributeConverters>().Get(Db.Get().AttributeConverters.ImmuneLevelBoost);
			immuneLevel.deltaAttribute.Add(new AttributeModifier(immuneLevel.deltaAttribute.Id, attributeConverterInstance.Evaluate(), DUPLICANTS.ATTRIBUTES.IMMUNITY.BOOST_STAT, false, false, true));
			immuneSuppress = new AttributeModifier(immuneLevel.deltaAttribute.Id, -0.025f, DUPLICANTS.DISEASES.INFECTED_MODIFIER, false, false, true);
			activeDiseases = master.GetComponent<MinionModifiers>().diseases;
			primaryElement = master.GetComponent<PrimaryElement>();
			effects = master.GetComponent<Effects>();
			lastDiseaseSources = new Dictionary<HashedString, DiseaseSourceInfo>();
			activeImmuneModifiers = new Dictionary<HashedString, AttributeModifier>();
			diseaseCountMultModifiers = new Dictionary<HashedString, AttributeModifier>();
			Klei.AI.Attributes attributes = base.gameObject.GetAttributes();
			foreach (Disease resource in Db.Get().Diseases.resources)
			{
				attributes.Add(new AttributeModifier(resource.amountDeltaAttribute.Id, -0.8333333f, resource.Name, false, false, false));
				AttributeModifier attributeModifier = new AttributeModifier(resource.amountDeltaAttribute.Id, -0.00066666666f, resource.Name, false, false, false);
				diseaseCountMultModifiers[resource.id] = attributeModifier;
				attributes.Add(attributeModifier);
			}
			GameClock.Instance.Subscribe(-722330267, OnNightTime);
			modifiers = base.gameObject.GetComponent<Modifiers>();
			immuneDelta = Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(base.gameObject);
			OxygenBreather component = GetComponent<OxygenBreather>();
			component.onSimConsume = (Action<Sim.MassConsumedCallback>)Delegate.Combine(component.onSimConsume, new Action<Sim.MassConsumedCallback>(OnAirConsumed));
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
					if (disease.infectionVectors.Contains(Disease.InfectionVector.Digestion))
					{
						float num = edible.unitsConsumed / (edible.unitsConsumed + edible.Units);
						int num2 = Mathf.CeilToInt((float)header.diseaseCount * num);
						GameComps.DiseaseContainers.ModifyDiseaseCount(handle, -num2);
						KPrefabID component = edible.GetComponent<KPrefabID>();
						InjectDisease(disease, num2, component.PrefabID(), Disease.InfectionVector.Digestion);
					}
				}
			}
		}

		public void OnAirConsumed(Sim.MassConsumedCallback mass_cb_info)
		{
			if (mass_cb_info.diseaseIdx != 255)
			{
				Disease disease = Db.Get().Diseases[mass_cb_info.diseaseIdx];
				if (disease.infectionVectors.Contains(Disease.InfectionVector.Inhalation))
				{
					InjectDisease(disease, mass_cb_info.diseaseCount, ElementLoader.elements[mass_cb_info.elemIdx].tag, Disease.InfectionVector.Inhalation);
				}
			}
		}

		public void InjectDisease(Disease disease, int count, Tag source, Disease.InfectionVector vector)
		{
			Modifiers component = base.gameObject.GetComponent<Modifiers>();
			Klei.AI.Amounts amounts = component.GetAmounts();
			AmountInstance amountInstance = amounts.Get(disease.amount);
			amountInstance.ApplyDelta((float)count);
			lastDiseaseSources[disease.id] = new DiseaseSourceInfo(source, vector);
		}

		public void TryInjectDisease(byte disease_idx, int count, Tag source, Disease.InfectionVector vector)
		{
			if (disease_idx != 255)
			{
				Disease disease = Db.Get().Diseases[disease_idx];
				if (disease.infectionVectors.Contains(vector))
				{
					InjectDisease(disease, count, source, vector);
				}
			}
		}

		private AttributeModifier CreateModifier(Disease disease, string immune_id, Klei.AI.Amounts amounts)
		{
			if (!attributeModifierPool.TryGetValue(disease.id, out AttributeModifier value))
			{
				value = new AttributeModifier(immune_id, 0f, delegate
				{
					float value2 = amounts.Get(disease.amount).value;
					return StringFormatter.Replace(DUPLICANTS.DISEASES.INFECTION_MODIFIER, "{0}", disease.Name).Replace("{1}", GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(value2)));
				}, false, false);
				attributeModifierPool[disease.id] = value;
			}
			return value;
		}

		public void UpdateImmuneSystem()
		{
			Klei.AI.Amounts amounts = modifiers.GetAmounts();
			Disease disease = null;
			float num = -1f;
			Database.Diseases diseases = Db.Get().Diseases;
			for (int i = 0; i < diseases.Count; i++)
			{
				Disease disease2 = diseases[i];
				float value = amounts.Get(disease2.amount).value;
				if (value > 0f)
				{
					if (value > num)
					{
						disease = disease2;
						num = value;
					}
					float num2 = -0.8333333f;
					float num3 = value * -0.00066666666f;
					num2 += num3;
					diseaseCountMultModifiers[disease2.id].SetValue(num3);
					float num4 = num2 * disease2.immuneAttackStrength;
					if (num4 <= -0.000166666665f && value > 1f)
					{
						if (!activeImmuneModifiers.TryGetValue(disease2.id, out AttributeModifier value2))
						{
							value2 = CreateModifier(disease2, immuneDelta.Id, amounts);
							base.gameObject.GetAttributes().Add(value2);
							activeImmuneModifiers[disease2.id] = value2;
						}
						value2.SetValue(num4);
					}
					else if (activeImmuneModifiers.ContainsKey(disease2.id))
					{
						base.gameObject.GetAttributes().Remove(activeImmuneModifiers[disease2.id]);
						activeImmuneModifiers.Remove(disease2.id);
					}
				}
			}
			lastHighestDisease = disease;
			base.sm.isLosingImmunity.Set(immuneDelta.GetTotalValue() < 0f, base.smi);
		}

		public void ClearInternalDisease()
		{
			Klei.AI.Amounts amounts = modifiers.GetAmounts();
			Database.Diseases diseases = Db.Get().Diseases;
			for (int i = 0; i < diseases.Count; i++)
			{
				Disease disease = diseases[i];
				AmountInstance amountInstance = amounts.Get(disease.amount);
				amountInstance.SetValue(0f);
				if (activeImmuneModifiers.ContainsKey(disease.id))
				{
					base.gameObject.GetAttributes().Remove(activeImmuneModifiers[disease.id]);
					activeImmuneModifiers.Remove(disease.id);
				}
			}
		}

		private void OnImmuneDelta(float delta)
		{
			if (!(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ImmuneSystem).id == "Invincible") && immuneLevel.value <= 0f && lastHighestDisease != null)
			{
				string infection_source_info;
				if (lastDiseaseSources.TryGetValue(lastHighestDisease.Id, out DiseaseSourceInfo value))
				{
					switch (value.vector)
					{
					default:
						infection_source_info = DUPLICANTS.DISEASES.INFECTIONSOURCES.UNKNOWN;
						break;
					case Disease.InfectionVector.Contact:
						infection_source_info = DUPLICANTS.DISEASES.INFECTIONSOURCES.SKIN;
						break;
					case Disease.InfectionVector.Inhalation:
						infection_source_info = string.Format(DUPLICANTS.DISEASES.INFECTIONSOURCES.AIR, value.sourceObject.ProperName());
						break;
					case Disease.InfectionVector.Digestion:
						infection_source_info = string.Format(DUPLICANTS.DISEASES.INFECTIONSOURCES.FOOD, value.sourceObject.ProperName());
						break;
					}
				}
				else
				{
					infection_source_info = DUPLICANTS.DISEASES.INFECTIONSOURCES.UNKNOWN;
				}
				activeDiseases.Infect(new DiseaseExposureInfo(lastHighestDisease.Id, infection_source_info));
			}
		}

		public AttributeModifier GetCurrentImmuneModifier(Disease disease)
		{
			AttributeModifier value = null;
			activeImmuneModifiers.TryGetValue(disease.id, out value);
			return value;
		}

		public bool IsLowImmuneLevel()
		{
			return immuneLevel.value < 40f;
		}

		public bool IsSick()
		{
			return activeDiseases.Count > 0;
		}

		private void OnNightTime(object data)
		{
			UpdateReports();
		}

		private void UpdateReports()
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseStatus, (float)primaryElement.DiseaseCount, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.GERMS, "{0}", base.master.name), base.master.gameObject.GetProperName());
		}
	}

	public BoolParameter isLosingImmunity;

	public State healthy;

	public BelowToleranceState infecting;

	public State infected;

	public State beginrecovering;

	public State recovering;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = healthy;
		base.serializable = false;
		root.EventHandler(GameHashes.EatCompleteEater, delegate(Instance smi, object obj)
		{
			smi.OnEatComplete(obj);
		}).EventTransition(GameHashes.DiseaseAdded, infected, (Instance smi) => smi.IsSick()).Transition(recovering, (Instance smi) => smi.effects.HasEffect("PostDiseaseRecovery"), UpdateRate.SIM_200ms);
		healthy.ParamTransition(isLosingImmunity, infecting, GameStateMachine<ImmuneSystemMonitor, Instance, IStateMachineTarget, object>.IsTrue).Update(delegate(Instance smi, float dt)
		{
			smi.UpdateImmuneSystem();
		}, UpdateRate.SIM_200ms, false);
		infecting.DefaultState(infecting.high).ParamTransition(isLosingImmunity, healthy, GameStateMachine<ImmuneSystemMonitor, Instance, IStateMachineTarget, object>.IsFalse).Update(delegate(Instance smi, float dt)
		{
			smi.UpdateImmuneSystem();
		}, UpdateRate.SIM_200ms, false);
		infecting.high.Transition(infecting.low, (Instance smi) => smi.IsLowImmuneLevel(), UpdateRate.SIM_200ms);
		infecting.low.Transition(infecting.high, (Instance smi) => !smi.IsLowImmuneLevel(), UpdateRate.SIM_200ms).Enter(delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_BeingInfected);
		}).ToggleStatusItem(Db.Get().DuplicantStatusItems.LowImmunity, (object)null);
		infected.Update(delegate(Instance smi, float dt)
		{
			smi.ClearInternalDisease();
		}, UpdateRate.SIM_200ms, false).ToggleAttributeModifier("suppressed by sickness", (Instance smi) => smi.immuneSuppress, null).EventTransition(GameHashes.DiseaseCured, beginrecovering, (Instance smi) => !smi.IsSick());
		beginrecovering.AddEffect("PostDiseaseRecovery").GoTo(recovering);
		recovering.Update(delegate(Instance smi, float dt)
		{
			smi.ClearInternalDisease();
		}, UpdateRate.SIM_200ms, false).Transition(healthy, (Instance smi) => !smi.effects.HasEffect("PostDiseaseRecovery"), UpdateRate.SIM_200ms);
	}
}
