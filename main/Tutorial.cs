using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : KMonoBehaviour, IRender1000ms
{
	public enum TutorialMessages
	{
		TM_Basics,
		TM_Welcome,
		TM_StressManagement,
		TM_Scheduling,
		TM_Mopping,
		TM_Locomotion,
		TM_Priorities,
		TM_FetchingWater,
		TM_ThermalComfort,
		TM_OverheatingBuildings,
		TM_LotsOfGerms,
		TM_BeingInfected,
		TM_DiseaseCooking,
		TM_Suits,
		TM_Morale,
		TM_Schedule,
		TM_Digging,
		TM_Power,
		TM_Insulation,
		TM_Plumbing,
		TM_COUNT
	}

	private delegate bool HideConditionDelegate();

	private delegate bool RequirementSatisfiedDelegate();

	private class Item
	{
		public Notification notification;

		public HideConditionDelegate hideCondition;

		public RequirementSatisfiedDelegate requirementSatisfied;

		public float minTimeToNotify;

		public float lastNotifyTime;
	}

	[MyCmpAdd]
	private Notifier notifier;

	[Serialize]
	private SerializedList<TutorialMessages> tutorialMessagesRemaining = new SerializedList<TutorialMessages>();

	private const string HIDDEN_TUTORIAL_PREF_KEY_PREFIX = "HideTutorial_";

	public const string HIDDEN_TUTORIAL_PREF_BUTTON_KEY = "HideTutorial_CheckState";

	private Dictionary<TutorialMessages, bool> hiddenTutorialMessages = new Dictionary<TutorialMessages, bool>();

	private int debugMessageCount = 0;

	private bool queuedPrioritiesMessage = false;

	private const float LOW_RATION_AMOUNT = 1f;

	private List<List<Item>> itemTree = new List<List<Item>>();

	private List<Item> warningItems = new List<Item>();

	private Vector3 notifierPosition;

	public List<GameObject> oxygenGenerators = new List<GameObject>();

	private int focusedOxygenGenerator = 0;

	public static Tutorial Instance
	{
		get;
		private set;
	}

	public static void ResetHiddenTutorialMessages()
	{
		IEnumerator enumerator = Enum.GetValues(typeof(TutorialMessages)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TutorialMessages key = (TutorialMessages)enumerator.Current;
				string key2 = "HideTutorial_" + key.ToString();
				KPlayerPrefs.SetInt(key2, 0);
				if ((UnityEngine.Object)Instance != (UnityEngine.Object)null)
				{
					Instance.hiddenTutorialMessages[key] = false;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		KPlayerPrefs.SetInt("HideTutorial_CheckState", 0);
	}

	private void LoadHiddenTutorialMessages()
	{
		IEnumerator enumerator = Enum.GetValues(typeof(TutorialMessages)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TutorialMessages key = (TutorialMessages)enumerator.Current;
				string key2 = "HideTutorial_" + key.ToString();
				bool value = KPlayerPrefs.GetInt(key2, 0) != 0;
				hiddenTutorialMessages[key] = value;
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void HideTutorialMessage(TutorialMessages message)
	{
		hiddenTutorialMessages[message] = true;
		string key = "HideTutorial_" + message.ToString();
		KPlayerPrefs.SetInt(key, 1);
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	private void UpdateNotifierPosition()
	{
		if (notifierPosition == Vector3.zero)
		{
			GameObject telepad = GameUtil.GetTelepad();
			if ((UnityEngine.Object)telepad != (UnityEngine.Object)null)
			{
				notifierPosition = telepad.transform.GetPosition();
			}
		}
		notifier.transform.SetPosition(notifierPosition);
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		LoadHiddenTutorialMessages();
	}

	protected override void OnSpawn()
	{
		if (tutorialMessagesRemaining.Count == 0)
		{
			for (int i = 0; i <= 20; i++)
			{
				tutorialMessagesRemaining.Add((TutorialMessages)i);
			}
		}
		List<Item> list = new List<Item>();
		list.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.NEEDTOILET.NAME, NotificationType.Tutorial, HashedString.Invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NEEDTOILET.TOOLTIP.text, null, true, 5f, delegate
			{
				PlanScreen.Instance.OpenCategoryByName("Plumbing");
			}, null, null),
			requirementSatisfied = new RequirementSatisfiedDelegate(ToiletExists)
		});
		itemTree.Add(list);
		List<Item> list2 = new List<Item>();
		list2.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.NEEDFOOD.NAME, NotificationType.Tutorial, HashedString.Invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NEEDFOOD.TOOLTIP.text, null, true, 20f, delegate
			{
				PlanScreen.Instance.OpenCategoryByName("Food");
			}, null, null),
			requirementSatisfied = new RequirementSatisfiedDelegate(FoodSourceExists)
		});
		list2.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.THERMALCOMFORT.NAME, NotificationType.Tutorial, HashedString.Invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.THERMALCOMFORT.TOOLTIP.text, null, true, 0f, null, null, null)
		});
		itemTree.Add(list2);
		List<Item> list3 = new List<Item>();
		list3.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.HYGENE_NEEDED.NAME, NotificationType.Tutorial, HashedString.Invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.HYGENE_NEEDED.TOOLTIP, null, true, 20f, delegate
			{
				PlanScreen.Instance.OpenCategoryByName("Medicine");
			}, null, null),
			requirementSatisfied = new RequirementSatisfiedDelegate(HygeneExists)
		});
		itemTree.Add(list3);
		List<Item> list4 = warningItems;
		Item item = new Item();
		Item item2 = item;
		string title = MISC.NOTIFICATIONS.NO_OXYGEN_GENERATOR.NAME;
		HashedString invalid = HashedString.Invalid;
		item2.notification = new Notification(title, NotificationType.Tutorial, invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NO_OXYGEN_GENERATOR.TOOLTIP, null, false, 0f, delegate
		{
			PlanScreen.Instance.OpenCategoryByName("Oxygen");
		}, null, null);
		item.requirementSatisfied = OxygenGeneratorBuilt;
		item.minTimeToNotify = 80f;
		item.lastNotifyTime = 0f;
		list4.Add(item);
		List<Item> list5 = warningItems;
		item = new Item();
		Item item3 = item;
		title = MISC.NOTIFICATIONS.INSUFFICIENTOXYGENLASTCYCLE.NAME;
		invalid = HashedString.Invalid;
		item3.notification = new Notification(title, NotificationType.Tutorial, invalid, OnOxygenTooltip, null, false, 0f, delegate
		{
			ZoomToNextOxygenGenerator();
		}, null, null);
		item.hideCondition = OxygenGeneratorNotBuilt;
		item.requirementSatisfied = SufficientOxygenLastCycleAndThisCycle;
		item.minTimeToNotify = 80f;
		item.lastNotifyTime = 0f;
		list5.Add(item);
		warningItems.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.UNREFRIGERATEDFOOD.NAME, NotificationType.Tutorial, HashedString.Invalid, UnrefrigeratedFoodTooltip, null, false, 0f, delegate
			{
				PlanScreen.Instance.OpenCategoryByName("Food");
			}, null, null),
			requirementSatisfied = new RequirementSatisfiedDelegate(FoodIsRefrigerated),
			minTimeToNotify = 6f,
			lastNotifyTime = 0f
		});
		warningItems.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.FOODLOW.NAME, NotificationType.Bad, HashedString.Invalid, OnLowFoodTooltip, null, false, 0f, delegate
			{
				PlanScreen.Instance.OpenCategoryByName("Food");
			}, null, null),
			requirementSatisfied = new RequirementSatisfiedDelegate(EnoughFood),
			minTimeToNotify = 10f,
			lastNotifyTime = 0f
		});
		warningItems.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.NO_MEDICAL_COTS.NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> n, object o) => MISC.NOTIFICATIONS.NO_MEDICAL_COTS.TOOLTIP, null, false, 0f, delegate
			{
				PlanScreen.Instance.OpenCategoryByName("Medicine");
			}, null, null),
			requirementSatisfied = new RequirementSatisfiedDelegate(EnoughMedicalCots),
			minTimeToNotify = 10f,
			lastNotifyTime = 0f
		});
		warningItems.Add(new Item
		{
			notification = new Notification(string.Format(UI.ENDOFDAYREPORT.TRAVELTIMEWARNING.WARNING_TITLE), NotificationType.BadMinor, HashedString.Invalid, (List<Notification> n, object d) => string.Format(UI.ENDOFDAYREPORT.TRAVELTIMEWARNING.WARNING_MESSAGE, GameUtil.GetFormattedPercent(40f, GameUtil.TimeSlice.None)), null, true, 0f, delegate
			{
				ManagementMenu.Instance.OpenReports(GameClock.Instance.GetCycle());
			}, null, null),
			requirementSatisfied = new RequirementSatisfiedDelegate(LongTravelTimes),
			minTimeToNotify = 1f,
			lastNotifyTime = 0f
		});
	}

	public Message TutorialMessage(TutorialMessages tm, bool queueMessage = true)
	{
		Message message = null;
		switch (tm)
		{
		case TutorialMessages.TM_Basics:
			message = new TutorialMessage(TutorialMessages.TM_Basics, MISC.NOTIFICATIONS.BASICCONTROLS.NAME, MISC.NOTIFICATIONS.BASICCONTROLS.MESSAGEBODY, MISC.NOTIFICATIONS.BASICCONTROLS.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_Welcome:
			message = new TutorialMessage(TutorialMessages.TM_Welcome, MISC.NOTIFICATIONS.WELCOMEMESSAGE.NAME, MISC.NOTIFICATIONS.WELCOMEMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.WELCOMEMESSAGE.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_StressManagement:
			message = new TutorialMessage(TutorialMessages.TM_StressManagement, MISC.NOTIFICATIONS.STRESSMANAGEMENTMESSAGE.NAME, MISC.NOTIFICATIONS.STRESSMANAGEMENTMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.STRESSMANAGEMENTMESSAGE.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_Scheduling:
			message = new TutorialMessage(TutorialMessages.TM_Scheduling, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.NAME, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_Mopping:
			message = new TutorialMessage(TutorialMessages.TM_Mopping, MISC.NOTIFICATIONS.MOPPINGMESSAGE.NAME, MISC.NOTIFICATIONS.MOPPINGMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.MOPPINGMESSAGE.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_Locomotion:
			message = new TutorialMessage(TutorialMessages.TM_Locomotion, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.NAME, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.TOOLTIP, "tutorials\\Locomotion_tutorial", "Tute_Locomotion", VIDEOS.LOCOMOTION);
			break;
		case TutorialMessages.TM_Priorities:
			message = new TutorialMessage(TutorialMessages.TM_Priorities, MISC.NOTIFICATIONS.PRIORITIESMESSAGE.NAME, MISC.NOTIFICATIONS.PRIORITIESMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.PRIORITIESMESSAGE.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_FetchingWater:
			message = new TutorialMessage(TutorialMessages.TM_FetchingWater, MISC.NOTIFICATIONS.FETCHINGWATERMESSAGE.NAME, MISC.NOTIFICATIONS.FETCHINGWATERMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.FETCHINGWATERMESSAGE.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_ThermalComfort:
			message = new TutorialMessage(TutorialMessages.TM_ThermalComfort, MISC.NOTIFICATIONS.THERMALCOMFORT.NAME, MISC.NOTIFICATIONS.THERMALCOMFORT.MESSAGEBODY, MISC.NOTIFICATIONS.THERMALCOMFORT.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_OverheatingBuildings:
			message = new TutorialMessage(TutorialMessages.TM_OverheatingBuildings, MISC.NOTIFICATIONS.TUTORIAL_OVERHEATING.NAME, MISC.NOTIFICATIONS.TUTORIAL_OVERHEATING.MESSAGEBODY, MISC.NOTIFICATIONS.TUTORIAL_OVERHEATING.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_LotsOfGerms:
			message = new TutorialMessage(TutorialMessages.TM_LotsOfGerms, MISC.NOTIFICATIONS.LOTS_OF_GERMS.NAME, MISC.NOTIFICATIONS.LOTS_OF_GERMS.MESSAGEBODY, MISC.NOTIFICATIONS.LOTS_OF_GERMS.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_BeingInfected:
			message = new TutorialMessage(TutorialMessages.TM_BeingInfected, MISC.NOTIFICATIONS.BEING_INFECTED.NAME, MISC.NOTIFICATIONS.BEING_INFECTED.MESSAGEBODY, MISC.NOTIFICATIONS.BEING_INFECTED.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_DiseaseCooking:
			message = new TutorialMessage(TutorialMessages.TM_DiseaseCooking, MISC.NOTIFICATIONS.DISEASE_COOKING.NAME, MISC.NOTIFICATIONS.DISEASE_COOKING.MESSAGEBODY, MISC.NOTIFICATIONS.DISEASE_COOKING.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_Suits:
			message = new TutorialMessage(TutorialMessages.TM_Suits, MISC.NOTIFICATIONS.SUITS.NAME, MISC.NOTIFICATIONS.SUITS.MESSAGEBODY, MISC.NOTIFICATIONS.SUITS.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_Morale:
			message = new TutorialMessage(TutorialMessages.TM_Morale, MISC.NOTIFICATIONS.MORALE.NAME, MISC.NOTIFICATIONS.MORALE.MESSAGEBODY, MISC.NOTIFICATIONS.MORALE.TOOLTIP, "tutorials\\Morale_tutorial", "Tute_Morale", VIDEOS.MORALE);
			break;
		case TutorialMessages.TM_Schedule:
			message = new TutorialMessage(TutorialMessages.TM_Schedule, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.NAME, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.TOOLTIP, null, null, null);
			break;
		case TutorialMessages.TM_Power:
			message = new TutorialMessage(TutorialMessages.TM_Power, MISC.NOTIFICATIONS.POWER.NAME, MISC.NOTIFICATIONS.POWER.MESSAGEBODY, MISC.NOTIFICATIONS.POWER.TOOLTIP, "tutorials\\Power_tutorial", "Tute_Power", VIDEOS.POWER);
			break;
		case TutorialMessages.TM_Digging:
			message = new TutorialMessage(TutorialMessages.TM_Digging, MISC.NOTIFICATIONS.DIGGING.NAME, MISC.NOTIFICATIONS.DIGGING.MESSAGEBODY, MISC.NOTIFICATIONS.DIGGING.TOOLTIP, "tutorials\\Digging_tutorial", "Tute_Digging", VIDEOS.DIGGING);
			break;
		case TutorialMessages.TM_Insulation:
			message = new TutorialMessage(TutorialMessages.TM_Insulation, MISC.NOTIFICATIONS.INSULATION.NAME, MISC.NOTIFICATIONS.INSULATION.MESSAGEBODY, MISC.NOTIFICATIONS.INSULATION.TOOLTIP, "tutorials\\Insulation_tutorial", "Tute_Insulation", VIDEOS.INSULATION);
			break;
		case TutorialMessages.TM_Plumbing:
			message = new TutorialMessage(TutorialMessages.TM_Plumbing, MISC.NOTIFICATIONS.PLUMBING.NAME, MISC.NOTIFICATIONS.PLUMBING.MESSAGEBODY, MISC.NOTIFICATIONS.PLUMBING.TOOLTIP, "tutorials\\Piping_tutorial", "Tute_Plumbing", VIDEOS.PLUMBING);
			break;
		}
		Debug.Assert(message != null, $"No Tutorial message: {tm.ToString()}");
		if (queueMessage)
		{
			if (!tutorialMessagesRemaining.Contains(tm))
			{
				return null;
			}
			if (hiddenTutorialMessages.ContainsKey(tm) && hiddenTutorialMessages[tm])
			{
				return null;
			}
			tutorialMessagesRemaining.Remove(tm);
			Messenger.Instance.QueueMessage(message);
		}
		return message;
	}

	private string OnOxygenTooltip(List<Notification> notifications, object data)
	{
		ReportManager.ReportEntry entry = ReportManager.Instance.YesterdaysReport.GetEntry(ReportManager.ReportType.OxygenCreated);
		string text = MISC.NOTIFICATIONS.INSUFFICIENTOXYGENLASTCYCLE.TOOLTIP;
		text = text.Replace("{EmittingRate}", GameUtil.GetFormattedMass(entry.Positive, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
		return text.Replace("{ConsumptionRate}", GameUtil.GetFormattedMass(Mathf.Abs(entry.Negative), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private string UnrefrigeratedFoodTooltip(List<Notification> notifications, object data)
	{
		string text = MISC.NOTIFICATIONS.UNREFRIGERATEDFOOD.TOOLTIP;
		List<string> list = new List<string>();
		GetUnrefrigeratedFood(list);
		for (int i = 0; i < list.Count; i++)
		{
			text = text + "\n" + list[i];
		}
		return text;
	}

	private string OnLowFoodTooltip(List<Notification> notifications, object data)
	{
		float calories = RationTracker.Get().CountRations(null, true);
		float f = (float)Components.LiveMinionIdentities.Count * -1000000f;
		return string.Format(MISC.NOTIFICATIONS.FOODLOW.TOOLTIP, GameUtil.GetFormattedCalories(calories, GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories(Mathf.Abs(f), GameUtil.TimeSlice.None, true));
	}

	public void DebugNotification()
	{
		string text = "";
		NotificationType type;
		if (debugMessageCount % 3 == 0)
		{
			type = NotificationType.Tutorial;
			text = "Warning message e.g. \"not enough oxygen\" uses Warning Color";
		}
		else if (debugMessageCount % 3 == 1)
		{
			type = NotificationType.BadMinor;
			text = "Normal message e.g. Idle. Uses Normal Color BG";
		}
		else
		{
			type = NotificationType.Bad;
			text = "Urgent important message. Uses Bad Color BG";
		}
		string arg = text;
		int num = debugMessageCount++;
		num = num;
		Notification notification = new Notification($"{arg} ({num.ToString()})", type, HashedString.Invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NEEDTOILET.TOOLTIP.text, null, true, 0f, null, null, null);
		notifier.Add(notification, "");
	}

	public void DebugNotificationMessage()
	{
		int num = debugMessageCount++;
		num = num;
		Message message = new GenericMessage("This is a message notification. " + num.ToString(), MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.TOOLTIP);
		Messenger.Instance.QueueMessage(message);
	}

	public void Render1000ms(float dt)
	{
		if (!App.isLoading && Components.LiveMinionIdentities.Count != 0)
		{
			if (itemTree.Count > 0)
			{
				List<Item> list = itemTree[0];
				for (int num = list.Count - 1; num >= 0; num--)
				{
					Item item = list[num];
					if (item != null)
					{
						if (item.requirementSatisfied == null || item.requirementSatisfied())
						{
							item.notification.Clear();
							list.RemoveAt(num);
						}
						else if (item.hideCondition != null && item.hideCondition())
						{
							item.notification.Clear();
							list.RemoveAt(num);
						}
						else
						{
							UpdateNotifierPosition();
							notifier.Add(item.notification, "");
						}
					}
				}
				if (list.Count == 0)
				{
					itemTree.RemoveAt(0);
				}
			}
			foreach (Item warningItem in warningItems)
			{
				if (warningItem.requirementSatisfied())
				{
					warningItem.notification.Clear();
					warningItem.lastNotifyTime = Time.time;
				}
				else if (warningItem.hideCondition != null && warningItem.hideCondition())
				{
					warningItem.notification.Clear();
					warningItem.lastNotifyTime = Time.time;
				}
				else if (warningItem.lastNotifyTime == 0f || Time.time - warningItem.lastNotifyTime > warningItem.minTimeToNotify)
				{
					notifier.Add(warningItem.notification, "");
					warningItem.lastNotifyTime = Time.time;
				}
			}
			if (GameClock.Instance.GetCycle() > 0 && !tutorialMessagesRemaining.Contains(TutorialMessages.TM_Priorities) && !queuedPrioritiesMessage)
			{
				queuedPrioritiesMessage = true;
				GameScheduler.Instance.Schedule("PrioritiesTutorial", 2f, delegate
				{
					Instance.TutorialMessage(TutorialMessages.TM_Priorities, true);
				}, null, null);
			}
		}
	}

	private bool OxygenGeneratorBuilt()
	{
		return oxygenGenerators.Count > 0;
	}

	private bool OxygenGeneratorNotBuilt()
	{
		return oxygenGenerators.Count == 0;
	}

	private bool SufficientOxygenLastCycleAndThisCycle()
	{
		if (ReportManager.Instance.YesterdaysReport != null)
		{
			ReportManager.ReportEntry entry = ReportManager.Instance.YesterdaysReport.GetEntry(ReportManager.ReportType.OxygenCreated);
			ReportManager.ReportEntry entry2 = ReportManager.Instance.TodaysReport.GetEntry(ReportManager.ReportType.OxygenCreated);
			return entry2.Net > 0.0001f || entry.Net > 0.0001f || (GameClock.Instance.GetCycle() < 1 && !GameClock.Instance.IsNighttime());
		}
		return true;
	}

	private bool FoodIsRefrigerated()
	{
		if (GetUnrefrigeratedFood(null) <= 0)
		{
			return true;
		}
		return false;
	}

	private int GetUnrefrigeratedFood(List<string> foods)
	{
		int num = 0;
		if ((UnityEngine.Object)WorldInventory.Instance != (UnityEngine.Object)null)
		{
			List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(GameTags.Edible);
			if (pickupables == null)
			{
				return 0;
			}
			for (int i = 0; i < pickupables.Count; i++)
			{
				if ((UnityEngine.Object)pickupables[i].storage != (UnityEngine.Object)null && ((UnityEngine.Object)pickupables[i].storage.GetComponent<RationBox>() != (UnityEngine.Object)null || (UnityEngine.Object)pickupables[i].storage.GetComponent<Refrigerator>() != (UnityEngine.Object)null) && !Rottable.IsRefrigerated(pickupables[i].gameObject) && Rottable.AtmosphereQuality(pickupables[i].gameObject) != Rottable.RotAtmosphereQuality.Sterilizing)
				{
					Rottable.Instance sMI = pickupables[i].GetSMI<Rottable.Instance>();
					if (sMI != null && sMI.RotConstitutionPercentage < 0.8f)
					{
						num++;
						foods?.Add(pickupables[i].GetProperName());
					}
				}
			}
		}
		return num;
	}

	private bool EnergySourceExists()
	{
		return Game.Instance.circuitManager.HasGenerators();
	}

	private bool BedExists()
	{
		return Components.Sleepables.Count > 0;
	}

	private bool EnoughFood()
	{
		float num = RationTracker.Get().CountRations(null, true);
		float num2 = (float)Components.LiveMinionIdentities.Count * 1000000f;
		return num / num2 > 1f;
	}

	private bool EnoughMedicalCots()
	{
		int count = Components.Clinics.Count;
		int num = 0;
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			Sicknesses sicknesses = Components.LiveMinionIdentities[i].GetSicknesses();
			foreach (SicknessInstance item in sicknesses)
			{
				if (item.Sickness.severity >= Sickness.Severity.Major)
				{
					num++;
					break;
				}
			}
		}
		return count >= num;
	}

	private bool LongTravelTimes()
	{
		int num = 3;
		if (ReportManager.Instance.reports.Count >= num)
		{
			float num2 = 0f;
			float num3 = 0f;
			for (int num4 = ReportManager.Instance.reports.Count - 1; num4 >= ReportManager.Instance.reports.Count - num; num4--)
			{
				ReportManager.ReportEntry entry = ReportManager.Instance.reports[num4].GetEntry(ReportManager.ReportType.TravelTime);
				num2 += entry.Net;
				num3 += 600f * (float)entry.contextEntries.Count;
			}
			float num5 = num2 / num3;
			return num5 <= 0.4f;
		}
		return true;
	}

	private bool FoodSourceExists()
	{
		foreach (ComplexFabricator item in Components.ComplexFabricators.Items)
		{
			if (item.GetType() == typeof(MicrobeMusher))
			{
				return true;
			}
		}
		return Components.PlantablePlots.Count > 0;
	}

	private bool HygeneExists()
	{
		return Components.HandSanitizers.Count > 0;
	}

	private bool ToiletExists()
	{
		return Components.Toilets.Count > 0;
	}

	private void ZoomToNextOxygenGenerator()
	{
		if (oxygenGenerators.Count != 0)
		{
			focusedOxygenGenerator %= oxygenGenerators.Count;
			Vector3 position = oxygenGenerators[focusedOxygenGenerator].transform.position;
			CameraController.Instance.SetTargetPos(position, 8f, true);
			focusedOxygenGenerator++;
		}
	}
}
