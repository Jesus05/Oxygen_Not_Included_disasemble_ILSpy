using Klei.AI;
using Klei.CustomSettings;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MinionIdentity : KMonoBehaviour, ISaveLoadable, IAssignableIdentity, IListableOption, ISim1000ms
{
	private class NameList
	{
		private List<string> names = new List<string>();

		private int idx;

		public NameList(TextAsset file)
		{
			string[] array = file.text.Replace("  ", " ").Replace("\r\n", "\n").Split('\n');
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(' ');
				if (array2[array2.Length - 1] != "" && array2[array2.Length - 1] != null)
				{
					names.Add(array2[array2.Length - 1]);
				}
			}
			names.Shuffle();
		}

		public string Next()
		{
			return names[idx++ % names.Count];
		}
	}

	[MyCmpReq]
	private KSelectable selectable;

	public int femaleVoiceCount;

	public int maleVoiceCount;

	[Serialize]
	private new string name;

	[Serialize]
	public string gender;

	[Serialize]
	[ReadOnly]
	public float arrivalTime;

	[Serialize]
	public int voiceIdx;

	[Serialize]
	public KCompBuilder.BodyData bodyData;

	[Serialize]
	public Ref<MinionAssignablesProxy> assignableProxy;

	public float timeLastSpoke;

	private string voiceId;

	private KAnimHashedString overrideExpression;

	private KAnimHashedString expression;

	public bool addToIdentityList = true;

	private static NameList maleNameList;

	private static NameList femaleNameList;

	private static readonly EventSystem.IntraObjectHandler<MinionIdentity> OnDiedDelegate = new EventSystem.IntraObjectHandler<MinionIdentity>(delegate(MinionIdentity component, object data)
	{
		component.OnDied(data);
	});

	[Serialize]
	public string genderStringKey
	{
		get;
		set;
	}

	[Serialize]
	public string nameStringKey
	{
		get;
		set;
	}

	public static void DestroyStatics()
	{
		maleNameList = null;
		femaleNameList = null;
	}

	protected override void OnPrefabInit()
	{
		if (name == null)
		{
			name = ChooseRandomName();
		}
		if ((UnityEngine.Object)GameClock.Instance != (UnityEngine.Object)null)
		{
			arrivalTime = (float)GameClock.Instance.GetCycle();
		}
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			KAnimControllerBase kAnimControllerBase = component;
			kAnimControllerBase.OnUpdateBounds = (Action<Bounds>)Delegate.Combine(kAnimControllerBase.OnUpdateBounds, new Action<Bounds>(OnUpdateBounds));
		}
		Subscribe(1623392196, OnDiedDelegate);
	}

	protected override void OnSpawn()
	{
		ValidateProxy();
		CleanupLimboMinions();
		PathProber component = GetComponent<PathProber>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.SetGroupProber(MinionGroupProber.Get());
		}
		SetName(name);
		if (nameStringKey == null)
		{
			nameStringKey = name;
		}
		SetGender(gender);
		if (genderStringKey == null)
		{
			genderStringKey = "NB";
		}
		if (addToIdentityList)
		{
			Components.MinionIdentities.Add(this);
			if (!base.gameObject.HasTag(GameTags.Dead))
			{
				Components.LiveMinionIdentities.Add(this);
			}
		}
		SymbolOverrideController component2 = GetComponent<SymbolOverrideController>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			Accessorizer component3 = base.gameObject.GetComponent<Accessorizer>();
			if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
			{
				bodyData = default(KCompBuilder.BodyData);
				component3.GetBodySlots(ref bodyData);
				string text = HashCache.Get().Get(component3.GetAccessory(Db.Get().AccessorySlots.HeadShape).symbol.hash);
				string str = text.Replace("headshape", "cheek");
				component2.AddSymbolOverride("snapto_cheek", Assets.GetAnim("head_swap_kanim").GetData().build.GetSymbol(str), 1);
				component2.AddSymbolOverride(Db.Get().AccessorySlots.HairAlways.targetSymbolId, component3.GetAccessory(Db.Get().AccessorySlots.Hair).symbol, 1);
				component2.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component3.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
			}
		}
		voiceId = "0";
		voiceId += (voiceIdx + 1).ToString();
		Prioritizable component4 = GetComponent<Prioritizable>();
		if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
		{
			component4.showIcon = false;
		}
		Pickupable component5 = GetComponent<Pickupable>();
		if ((UnityEngine.Object)component5 != (UnityEngine.Object)null)
		{
			component5.carryAnimOverride = Assets.GetAnim("anim_incapacitated_carrier_kanim");
		}
		ApplyCustomGameSettings();
	}

	public void ValidateProxy()
	{
		assignableProxy = MinionAssignablesProxy.InitAssignableProxy(assignableProxy, this);
	}

	private void CleanupLimboMinions()
	{
		KPrefabID component = GetComponent<KPrefabID>();
		if (component.InstanceID == -1)
		{
			Output.LogWarning("Minion with an invalid kpid! Attempting to recover...", name);
			if ((UnityEngine.Object)KPrefabIDTracker.Get().GetInstance(component.InstanceID) != (UnityEngine.Object)null)
			{
				KPrefabIDTracker.Get().Unregister(component);
			}
			component.InstanceID = KPrefabID.GetUniqueID();
			KPrefabIDTracker.Get().Register(component);
			Output.LogWarning("Restored as:", component.InstanceID);
		}
		if (component.conflicted)
		{
			Output.LogWarning("Minion with a conflicted kpid! Attempting to recover... ", component.InstanceID, name);
			if ((UnityEngine.Object)KPrefabIDTracker.Get().GetInstance(component.InstanceID) != (UnityEngine.Object)null)
			{
				KPrefabIDTracker.Get().Unregister(component);
			}
			component.InstanceID = KPrefabID.GetUniqueID();
			KPrefabIDTracker.Get().Register(component);
			Output.LogWarning("Restored as:", component.InstanceID);
		}
		assignableProxy.Get().SetTarget(this, base.gameObject);
	}

	public string GetProperName()
	{
		return base.gameObject.GetProperName();
	}

	public string GetVoiceId()
	{
		return voiceId;
	}

	public void SetName(string name)
	{
		this.name = name;
		if ((UnityEngine.Object)selectable != (UnityEngine.Object)null)
		{
			selectable.SetName(name);
		}
		base.gameObject.name = name;
		NameDisplayScreen.Instance.UpdateName(base.gameObject);
	}

	public bool IsNull()
	{
		return (UnityEngine.Object)this == (UnityEngine.Object)null;
	}

	public void SetGender(string gender)
	{
		this.gender = gender;
		selectable.SetGender(gender);
	}

	public static string ChooseRandomName()
	{
		if (femaleNameList == null)
		{
			maleNameList = new NameList(Game.Instance.maleNamesFile);
			femaleNameList = new NameList(Game.Instance.femaleNamesFile);
		}
		if (!(UnityEngine.Random.value > 0.5f))
		{
			return femaleNameList.Next();
		}
		return maleNameList.Next();
	}

	protected override void OnCleanUp()
	{
		MinionAssignablesProxy minionAssignablesProxy = assignableProxy.Get();
		if ((bool)minionAssignablesProxy && minionAssignablesProxy.target == this)
		{
			Util.KDestroyGameObject(minionAssignablesProxy.gameObject);
		}
		Components.MinionIdentities.Remove(this);
		Components.LiveMinionIdentities.Remove(this);
	}

	private void OnUpdateBounds(Bounds bounds)
	{
		KBoxCollider2D component = GetComponent<KBoxCollider2D>();
		component.offset = bounds.center;
		component.size = bounds.extents;
	}

	private void OnDied(object data)
	{
		GetSoleOwner().UnassignAll();
		GetEquipment().UnequipAll();
		Components.LiveMinionIdentities.Remove(this);
	}

	public List<Ownables> GetOwners()
	{
		return assignableProxy.Get().ownables;
	}

	public Ownables GetSoleOwner()
	{
		return assignableProxy.Get().GetComponent<Ownables>();
	}

	public Equipment GetEquipment()
	{
		return assignableProxy.Get().GetComponent<Equipment>();
	}

	public void Sim1000ms(float dt)
	{
		if (!((UnityEngine.Object)this == (UnityEngine.Object)null) && GetComponent<Navigator>().IsMoving())
		{
			Chore currentChore = GetComponent<ChoreDriver>().GetCurrentChore();
			if (currentChore != null)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.TravelTime, dt, currentChore.choreType.Name, currentChore.driver.GetProperName());
				if (currentChore is FetchAreaChore)
				{
					MinionResume component = GetComponent<MinionResume>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						component.AddExperienceIfRole("Hauler", dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
						component.AddExperienceIfRole(MaterialsManager.ID, dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
						component.AddExperienceIfRole(Handyman.ID, dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
					}
				}
			}
		}
	}

	private void ApplyCustomGameSettings()
	{
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ImmuneSystem);
		if (currentQualitySetting.id == "Compromised")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, -0.025f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.COMPROMISED.ATTRIBUTE_MODIFIER_NAME, false, false, true));
			Db.Get().Attributes.DiseaseCureSpeed.Lookup(this).Add(new AttributeModifier(Db.Get().Attributes.DiseaseCureSpeed.Id, -0.3333f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting.id == "Weak")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, -0.008333334f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting.id == "Strong")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, 0.008333334f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.ATTRIBUTE_MODIFIER_NAME, false, false, true));
			Db.Get().Attributes.DiseaseCureSpeed.Lookup(this).Add(new AttributeModifier(Db.Get().Attributes.DiseaseCureSpeed.Id, 2f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting.id == "Invincible")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, float.PositiveInfinity, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.INVINCIBLE.ATTRIBUTE_MODIFIER_NAME, false, false, true));
			Db.Get().Attributes.DiseaseCureSpeed.Lookup(this).Add(new AttributeModifier(Db.Get().Attributes.DiseaseCureSpeed.Id, 1E+08f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		SettingLevel currentQualitySetting2 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Stress);
		if (currentQualitySetting2.id == "Doomed")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.0333333351f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DOOMED.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting2.id == "Pessimistic")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.0166666675f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting2.id == "Optimistic")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.0166666675f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting2.id == "Indomitable")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, float.NegativeInfinity, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.INDOMITABLE.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		SettingLevel currentQualitySetting3 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.CalorieBurn);
		if (currentQualitySetting3.id == "VeryHard")
		{
			Db.Get().Amounts.Calories.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -1666.66663f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.VERYHARD.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting3.id == "Hard")
		{
			Db.Get().Amounts.Calories.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -833.3333f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.HARD.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting3.id == "Easy")
		{
			Db.Get().Amounts.Calories.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, 833.3333f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.EASY.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting3.id == "Disabled")
		{
			Db.Get().Amounts.Calories.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, float.PositiveInfinity, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.DISABLED.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
	}
}
