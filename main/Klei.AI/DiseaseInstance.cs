using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class DiseaseInstance : ModifierInstance<Disease>, ISaveLoadable
	{
		private struct CureInfo
		{
			public string name;

			public float multiplier;
		}

		public class StatesInstance : GameStateMachine<States, StatesInstance, DiseaseInstance, object>.GameInstance
		{
			private object[] componentData;

			public StatesInstance(DiseaseInstance master)
				: base(master)
			{
			}

			public void UpdateProgress(float dt)
			{
				if (!base.master.modifier.doctorRequired || base.master.IsDoctored)
				{
					float delta_value = dt * base.master.TotalCureSpeedMultiplier / base.master.modifier.SicknessDuration;
					base.sm.percentRecovered.Delta(delta_value, base.smi);
				}
				if (base.master.modifier.fatalityDuration > 0f)
				{
					if (!base.master.IsDoctored)
					{
						float delta_value2 = dt / base.master.modifier.fatalityDuration;
						base.sm.percentDied.Delta(delta_value2, base.smi);
					}
					else
					{
						base.sm.percentDied.Set(0f, base.smi);
					}
				}
			}

			public void Infect()
			{
				Disease modifier = base.master.modifier;
				componentData = modifier.Infect(base.gameObject, base.master, base.master.exposureInfo);
				if ((UnityEngine.Object)PopFXManager.Instance != (UnityEngine.Object)null)
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, string.Format(DUPLICANTS.DISEASES.INFECTED_POPUP, modifier.Name), base.gameObject.transform, 1.5f, true);
				}
			}

			public void Cure()
			{
				Disease modifier = base.master.modifier;
				base.gameObject.GetComponent<Modifiers>().diseases.Cure(modifier);
				modifier.Cure(base.gameObject, componentData);
				if ((UnityEngine.Object)PopFXManager.Instance != (UnityEngine.Object)null)
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, string.Format(DUPLICANTS.DISEASES.CURED_POPUP, modifier.Name), base.gameObject.transform, 1.5f, true);
				}
			}

			public DiseaseExposureInfo GetExposureInfo()
			{
				return base.master.ExposureInfo;
			}
		}

		public class States : GameStateMachine<States, StatesInstance, DiseaseInstance>
		{
			public FloatParameter percentRecovered;

			public FloatParameter percentDied;

			public State infected;

			public State cured;

			public State fatality_pre;

			public State fatality;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = infected;
				base.serializable = true;
				infected.Enter("Infect", delegate(StatesInstance smi)
				{
					smi.Infect();
				}).DoNotification((StatesInstance smi) => smi.master.notification).Update("UpdateProgress", delegate(StatesInstance smi, float dt)
				{
					smi.UpdateProgress(dt);
				}, UpdateRate.SIM_200ms, false)
					.ToggleStatusItem((StatesInstance smi) => smi.master.GetStatusItem(), (StatesInstance smi) => smi)
					.ParamTransition(percentRecovered, cured, (StatesInstance smi, float p) => p > 1f)
					.ParamTransition(percentDied, fatality_pre, (StatesInstance smi, float p) => p > 1f);
				cured.Enter("Cure", delegate(StatesInstance smi)
				{
					smi.master.Cure();
				});
				fatality_pre.Update("DeathByDisease", delegate(StatesInstance smi, float dt)
				{
					DeathMonitor.Instance sMI = smi.master.gameObject.GetSMI<DeathMonitor.Instance>();
					if (sMI != null)
					{
						sMI.Kill(Db.Get().Deaths.FatalDisease);
						smi.GoTo(fatality);
					}
				}, UpdateRate.SIM_200ms, false);
				fatality.DoNothing();
			}
		}

		[Serialize]
		private DiseaseExposureInfo exposureInfo;

		private StatesInstance smi;

		private StatusItem statusItem;

		private Notification notification;

		public float TotalCureSpeedMultiplier
		{
			get
			{
				AttributeInstance attributeInstance = Db.Get().Attributes.DiseaseCureSpeed.Lookup(smi.master.gameObject);
				AttributeInstance attributeInstance2 = modifier.cureSpeedBase.Lookup(smi.master.gameObject);
				float num = 1f;
				if (attributeInstance != null)
				{
					num *= attributeInstance.GetTotalValue();
				}
				if (attributeInstance2 != null)
				{
					num *= attributeInstance2.GetTotalValue();
				}
				return num;
			}
		}

		public bool IsDoctored
		{
			get
			{
				if ((UnityEngine.Object)base.gameObject == (UnityEngine.Object)null)
				{
					return false;
				}
				AttributeInstance attributeInstance = Db.Get().Attributes.DoctoredLevel.Lookup(base.gameObject);
				if (attributeInstance != null && attributeInstance.GetTotalValue() > 0f)
				{
					return true;
				}
				return false;
			}
		}

		public DiseaseExposureInfo ExposureInfo
		{
			get
			{
				return exposureInfo;
			}
			set
			{
				exposureInfo = value;
				InitializeAndStart();
			}
		}

		public DiseaseInstance(GameObject game_object, Disease disease)
			: base(game_object, disease)
		{
		}

		[OnDeserialized]
		private void OnDeserialized()
		{
			InitializeAndStart();
		}

		private void InitializeAndStart()
		{
			Disease disease = modifier;
			Func<List<Notification>, object, string> tooltip = delegate(List<Notification> notificationList, object data)
			{
				string text = string.Empty;
				for (int i = 0; i < notificationList.Count; i++)
				{
					Notification notification = notificationList[i];
					string arg = (string)notification.tooltipData;
					text += string.Format(DUPLICANTS.DISEASES.NOTIFICATION_TOOLTIP, notification.NotifierName, disease.Name, arg);
					if (i < notificationList.Count - 1)
					{
						text += "\n";
					}
				}
				return text;
			};
			string name = disease.Name;
			string title = name;
			NotificationType type = (disease.severity > Disease.Severity.Minor) ? NotificationType.Bad : NotificationType.BadMinor;
			HashedString invalid = HashedString.Invalid;
			string infectionSourceInfo = exposureInfo.infectionSourceInfo;
			notification = new Notification(title, type, invalid, tooltip, infectionSourceInfo, true, 0f, null, null);
			statusItem = new StatusItem(disease.Id, disease.Name, DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.TEMPLATE, string.Empty, (disease.severity > Disease.Severity.Minor) ? StatusItem.IconType.Exclamation : StatusItem.IconType.Info, (disease.severity > Disease.Severity.Minor) ? NotificationType.Bad : NotificationType.BadMinor, false, SimViewMode.None, 63486);
			statusItem.resolveTooltipCallback = ResolveString;
			if (smi != null)
			{
				smi.StopSM("refresh");
			}
			smi = new StatesInstance(this);
			smi.StartSM();
		}

		private string ResolveString(string str, object data)
		{
			if (smi == null)
			{
				Debug.LogWarning("Attempting to resolve string when smi is null", null);
				return str;
			}
			KSelectable component = base.gameObject.GetComponent<KSelectable>();
			str = str.Replace("{Descriptor}", string.Format(DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.DESCRIPTOR, Strings.Get("STRINGS.DUPLICANTS.DISEASES.SEVERITY." + modifier.severity.ToString().ToUpper()), Strings.Get("STRINGS.DUPLICANTS.DISEASES.TYPE." + modifier.diseaseType.ToString().ToUpper())));
			str = str.Replace("{Infectee}", component.GetProperName());
			str = str.Replace("{InfectionSource}", string.Format(DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.INFECTION_SOURCE, exposureInfo.infectionSourceInfo));
			str = ((!modifier.doctorRequired || IsDoctored) ? str.Replace("{Duration}", string.Format(DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.DURATION, GameUtil.GetFormattedCycles(GetInfectedTimeRemaining(), "F1"))) : str.Replace("{Duration}", DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.DOCTOR_REQUIRED));
			if (IsDoctored)
			{
				str = str.Replace("{Doctor}", DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.DOCTORED);
				str = str.Replace("{Fatality}", string.Empty);
			}
			else if (modifier.fatalityDuration > 0f)
			{
				str = str.Replace("{Fatality}", string.Format(DUPLICANTS.DISEASES.STATUS_ITEM_TOOLTIP.FATALITY, GameUtil.GetFormattedCycles(GetFatalityTimeRemaining(), "F1")));
			}
			List<Descriptor> symptoms = modifier.GetSymptoms();
			string text = string.Empty;
			foreach (Descriptor item in symptoms)
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "\n";
				}
				item.IncreaseIndent();
				text += item.IndentedText();
			}
			str = str.Replace("{Symptoms}", text);
			str = Regex.Replace(str, "{[^}]*}", string.Empty);
			return str;
		}

		public float GetInfectedTimeRemaining()
		{
			float sicknessDuration = modifier.SicknessDuration;
			float num = sicknessDuration * (1f - smi.sm.percentRecovered.Get(smi));
			return num / TotalCureSpeedMultiplier;
		}

		public float GetFatalityTimeRemaining()
		{
			float fatalityDuration = modifier.fatalityDuration;
			return fatalityDuration * (1f - smi.sm.percentDied.Get(smi));
		}

		public float GetPercentCured()
		{
			return (smi == null) ? 0f : smi.sm.percentRecovered.Get(smi);
		}

		public void SetPercentCured(float pct)
		{
			smi.sm.percentRecovered.Set(pct, smi);
		}

		public void Cure()
		{
			smi.Cure();
		}

		public override void OnCleanUp()
		{
			if (smi != null)
			{
				smi.StopSM("DiseaseInstance.OnCleanUp");
				smi = null;
			}
		}

		public StatusItem GetStatusItem()
		{
			return statusItem;
		}

		public List<Descriptor> GetDescriptors()
		{
			return modifier.GetDiseaseSourceDescriptors();
		}
	}
}
