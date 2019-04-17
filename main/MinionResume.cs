using Database;
using Klei.AI;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MinionResume : KMonoBehaviour, ISaveLoadable, ISim200ms
{
	[MyCmpReq]
	private MinionIdentity identity;

	[Serialize]
	public Dictionary<string, bool> MasteryByRoleID = new Dictionary<string, bool>();

	[Serialize]
	public Dictionary<string, bool> MasteryBySkillID = new Dictionary<string, bool>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeByRoleGroup = new Dictionary<HashedString, float>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeBySkillGroup = new Dictionary<HashedString, float>();

	[Serialize]
	private string currentRole = "NoRole";

	[Serialize]
	private string targetRole = "NoRole";

	[Serialize]
	private string currentHat;

	[Serialize]
	private string targetHat;

	private Dictionary<string, bool> ownedHats = new Dictionary<string, bool>();

	[Serialize]
	private float totalExperienceGained;

	private KSelectable selectable;

	private AttributeModifier skillsMoraleExpectationModifier;

	public float DEBUG_PassiveExperienceGained;

	public float DEBUG_ActiveExperienceGained;

	public float DEBUG_SecondsAlive;

	public MinionIdentity GetIdentity => identity;

	public float TotalExperienceGained => totalExperienceGained;

	public int TotalSkillPointsGained
	{
		get
		{
			float f = TotalExperienceGained / (float)SKILLS.TARGET_SKILLS_CYCLE / 600f;
			float num = Mathf.Pow(f, 1f / SKILLS.EXPERIENCE_LEVEL_POWER);
			return Mathf.FloorToInt(num * (float)SKILLS.TARGET_SKILLS_EARNED);
		}
	}

	public int SkillsMastered
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
			{
				if (item.Value)
				{
					num++;
				}
			}
			return num;
		}
	}

	public int AvailableSkillpoints => TotalSkillPointsGained - SkillsMastered;

	public string CurrentRole => currentRole;

	public string CurrentHat => currentHat;

	public string TargetHat => targetHat;

	public string TargetRole => targetRole;

	[OnDeserialized]
	private void OnDeserializedMethod()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 7))
		{
			foreach (KeyValuePair<string, bool> item in MasteryByRoleID)
			{
				if (item.Value && item.Key != "NoRole")
				{
					ForceAddSkillPoint();
				}
			}
			foreach (KeyValuePair<HashedString, float> item2 in AptitudeByRoleGroup)
			{
				AptitudeBySkillGroup[item2.Key] = item2.Value;
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.MinionResumes.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		selectable = GetComponent<KSelectable>();
		foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
		{
			if (item.Value)
			{
				Skill skill = Db.Get().Skills.Get(item.Key);
				foreach (SkillPerk perk in skill.perks)
				{
					if (perk.OnRemove != null)
					{
						perk.OnRemove(this);
					}
					if (perk.OnApply != null)
					{
						perk.OnApply(this);
					}
				}
				if (!ownedHats.ContainsKey(skill.hat))
				{
					ownedHats.Add(skill.hat, true);
				}
			}
		}
		UpdateExpectations();
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		ApplyHat(currentHat, component);
	}

	public void RestoreResume(Dictionary<string, bool> MasteryBySkillID, Dictionary<HashedString, float> AptitudeBySkillGroup, float totalExperienceGained)
	{
		this.MasteryBySkillID = MasteryBySkillID;
		this.AptitudeBySkillGroup = AptitudeBySkillGroup;
		this.totalExperienceGained = totalExperienceGained;
	}

	protected override void OnCleanUp()
	{
		Components.MinionResumes.Remove(this);
		base.OnCleanUp();
	}

	public bool HasMasteredSkill(string skillId)
	{
		return MasteryBySkillID.ContainsKey(skillId) && MasteryBySkillID[skillId];
	}

	public void UpdateUrge()
	{
		if (targetHat != currentHat)
		{
			if (!base.gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.LearnSkill))
			{
				base.gameObject.GetComponent<ChoreConsumer>().AddUrge(Db.Get().Urges.LearnSkill);
			}
		}
		else
		{
			base.gameObject.GetComponent<ChoreConsumer>().RemoveUrge(Db.Get().Urges.LearnSkill);
		}
	}

	public void SetHats(string current, string target)
	{
		currentHat = current;
		targetHat = target;
	}

	public void SetCurrentRole(string role_id)
	{
		currentRole = role_id;
	}

	private void ApplySkillPerks(string skillId)
	{
		Skill skill = Db.Get().Skills.Get(skillId);
		foreach (SkillPerk perk in skill.perks)
		{
			if (perk.OnApply != null)
			{
				perk.OnApply(this);
			}
		}
	}

	private void RemoveSkillPerks(string skillId)
	{
		Skill skill = Db.Get().Skills.Get(skillId);
		foreach (SkillPerk perk in skill.perks)
		{
			if (perk.OnRemove != null)
			{
				perk.OnRemove(this);
			}
		}
	}

	public void Sim200ms(float dt)
	{
		DEBUG_SecondsAlive += dt;
		if (!GetComponent<KPrefabID>().HasTag(GameTags.Dead))
		{
			DEBUG_PassiveExperienceGained += dt * SKILLS.PASSIVE_EXPERIENCE_PORTION;
			AddExperience(dt * SKILLS.PASSIVE_EXPERIENCE_PORTION);
		}
	}

	public bool CheckSkillTraitDisabled(string skillId)
	{
		Skill skill = Db.Get().Skills.Get(skillId);
		string choreGroupID = Db.Get().SkillGroups.Get(skill.skillGroup).choreGroupID;
		if (!string.IsNullOrEmpty(choreGroupID))
		{
			Traits component = GetComponent<Traits>();
			foreach (Trait trait in component.TraitList)
			{
				if (trait.disabledChoreGroups != null)
				{
					ChoreGroup[] disabledChoreGroups = trait.disabledChoreGroups;
					foreach (ChoreGroup choreGroup in disabledChoreGroups)
					{
						if (choreGroup.Id == choreGroupID)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public bool CanMasterSkill(string skillId)
	{
		Skill skill = Db.Get().Skills.Get(skillId);
		if (CheckSkillTraitDisabled(skillId))
		{
			return false;
		}
		if (AvailableSkillpoints < 1)
		{
			return false;
		}
		for (int i = 0; i < skill.priorSkills.Count; i++)
		{
			if (!HasMasteredSkill(skill.priorSkills[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool OwnsHat(string hatId)
	{
		return ownedHats.ContainsKey(hatId) && ownedHats[hatId];
	}

	public void SkillLearned()
	{
		if (base.gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.LearnSkill))
		{
			base.gameObject.GetComponent<ChoreConsumer>().RemoveUrge(Db.Get().Urges.LearnSkill);
		}
		foreach (string item in ownedHats.Keys.ToList())
		{
			ownedHats[item] = true;
		}
		if (targetHat != null && currentHat != targetHat)
		{
			new PutOnHatChore(this, Db.Get().ChoreTypes.SwitchHat);
		}
	}

	public void MasterSkill(string skillId)
	{
		if (!base.gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.LearnSkill))
		{
			base.gameObject.GetComponent<ChoreConsumer>().AddUrge(Db.Get().Urges.LearnSkill);
		}
		MasteryBySkillID[skillId] = true;
		ApplySkillPerks(skillId);
		UpdateExpectations();
		TriggerMasterSkillEvents();
		if (!ownedHats.ContainsKey(Db.Get().Skills.Get(skillId).hat))
		{
			ownedHats.Add(Db.Get().Skills.Get(skillId).hat, false);
		}
	}

	public void UnmasterSkill(string skillId)
	{
		if (MasteryBySkillID.ContainsKey(skillId))
		{
			MasteryBySkillID.Remove(skillId);
			RemoveSkillPerks(skillId);
			UpdateExpectations();
			TriggerMasterSkillEvents();
		}
	}

	private void TriggerMasterSkillEvents()
	{
		Trigger(540773776, null);
		Game.Instance.Trigger(-1523247426, this);
	}

	public void ForceAddSkillPoint()
	{
		AddExperience(CalculateNextExperienceBar() - totalExperienceGained);
	}

	public float CalculateNextExperienceBar()
	{
		float f = (float)(TotalSkillPointsGained + 1) / (float)SKILLS.TARGET_SKILLS_EARNED;
		float num = Mathf.Pow(f, SKILLS.EXPERIENCE_LEVEL_POWER);
		return num * (float)SKILLS.TARGET_SKILLS_CYCLE * 600f;
	}

	public float CalculatePreviousExperienceBar()
	{
		float f = (float)TotalSkillPointsGained / (float)SKILLS.TARGET_SKILLS_EARNED;
		float num = Mathf.Pow(f, SKILLS.EXPERIENCE_LEVEL_POWER);
		return num * (float)SKILLS.TARGET_SKILLS_CYCLE * 600f;
	}

	private void UpdateExpectations()
	{
		int num = 0;
		foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
		{
			if (item.Value)
			{
				Skill skill = Db.Get().Skills.Get(item.Key);
				num += skill.tier + 1;
				float value = 0f;
				if (AptitudeBySkillGroup.TryGetValue(new HashedString(skill.skillGroup), out value))
				{
					num -= (int)value;
				}
			}
		}
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(this);
		if (skillsMoraleExpectationModifier != null)
		{
			attributeInstance.Remove(skillsMoraleExpectationModifier);
			skillsMoraleExpectationModifier = null;
		}
		if (num > 0)
		{
			skillsMoraleExpectationModifier = new AttributeModifier(attributeInstance.Id, (float)num, DUPLICANTS.NEEDS.QUALITYOFLIFE.EXPECTATION_MOD_NAME, false, false, true);
			attributeInstance.Add(skillsMoraleExpectationModifier);
		}
	}

	private void OnSkillPointGained()
	{
		Game.Instance.Trigger(1505456302, this);
		SkillMasteredMessage message = new SkillMasteredMessage(this);
		MusicManager.instance.PlaySong("Stinger_JobMastered", false);
		Messenger.Instance.QueueMessage(message);
		if ((Object)PopFXManager.Instance != (Object)null)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, MISC.NOTIFICATIONS.SKILL_POINT_EARNED.NAME, base.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
		}
		StateMachine.Instance instance = new UpgradeFX.Instance(base.gameObject.GetComponent<KMonoBehaviour>(), new Vector3(0f, 0f, -0.1f));
		instance.StartSM();
	}

	public void SetAptitude(HashedString skillGroupID, float amount)
	{
		AptitudeBySkillGroup[skillGroupID] = amount;
	}

	public float GetAptitudeExperienceMultiplier(HashedString skillGroupId, float buildingFrequencyMultiplier)
	{
		float value = 0f;
		AptitudeBySkillGroup.TryGetValue(skillGroupId, out value);
		return 1f + value * SKILLS.APTITUDE_EXPERIENCE_MULTIPLIER * buildingFrequencyMultiplier;
	}

	public void AddExperience(float amount)
	{
		float num = totalExperienceGained;
		float num2 = CalculateNextExperienceBar();
		totalExperienceGained += amount;
		if (totalExperienceGained >= num2 && num < num2)
		{
			OnSkillPointGained();
		}
	}

	public void AddExperienceWithAptitude(string skillGroupId, float amount, float buildingMultiplier)
	{
		float num = amount * GetAptitudeExperienceMultiplier(skillGroupId, buildingMultiplier) * SKILLS.ACTIVE_EXPERIENCE_PORTION;
		DEBUG_ActiveExperienceGained += num;
		AddExperience(num);
	}

	public bool HasPerk(HashedString perkId)
	{
		foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
		{
			if (item.Value && Db.Get().Skills.Get(item.Key).GivesPerk(perkId))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasPerk(SkillPerk perk)
	{
		foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
		{
			if (item.Value && Db.Get().Skills.Get(item.Key).GivesPerk(perk))
			{
				return true;
			}
		}
		return false;
	}

	public void RemoveHat()
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		RemoveHat(component);
	}

	public static void RemoveHat(KBatchedAnimController controller)
	{
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		Accessorizer component = controller.GetComponent<Accessorizer>();
		if ((Object)component != (Object)null)
		{
			Accessory accessory = component.GetAccessory(hat);
			if (accessory != null)
			{
				component.RemoveAccessory(accessory);
			}
		}
		else
		{
			controller.GetComponent<SymbolOverrideController>().TryRemoveSymbolOverride(hat.targetSymbolId, 4);
		}
		controller.SetSymbolVisiblity(hat.targetSymbolId, false);
		controller.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, false);
		controller.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, true);
	}

	public static void AddHat(string hat_id, KBatchedAnimController controller)
	{
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		Accessory accessory = hat.Lookup(hat_id);
		if (accessory == null)
		{
			Debug.LogWarning("Missing hat: " + hat_id);
		}
		Accessorizer component = controller.GetComponent<Accessorizer>();
		if ((Object)component != (Object)null)
		{
			Accessory accessory2 = component.GetAccessory(Db.Get().AccessorySlots.Hat);
			if (accessory2 != null)
			{
				component.RemoveAccessory(accessory2);
			}
			if (accessory != null)
			{
				component.AddAccessory(accessory);
			}
		}
		else
		{
			SymbolOverrideController component2 = controller.GetComponent<SymbolOverrideController>();
			component2.TryRemoveSymbolOverride(hat.targetSymbolId, 4);
			component2.AddSymbolOverride(hat.targetSymbolId, accessory.symbol, 4);
		}
		controller.SetSymbolVisiblity(hat.targetSymbolId, true);
		controller.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, true);
		controller.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, false);
	}

	public void ApplyTargetHat()
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		ApplyHat(targetHat, component);
		currentHat = targetHat;
		targetHat = null;
	}

	public static void ApplyHat(string hat_id, KBatchedAnimController controller)
	{
		if (hat_id.IsNullOrWhiteSpace())
		{
			RemoveHat(controller);
		}
		else
		{
			AddHat(hat_id, controller);
		}
	}

	public string GetSkillsSubtitle()
	{
		return "Total Skill Points: " + TotalSkillPointsGained;
	}

	public static bool AnyMinionHasPerk(string perk)
	{
		List<MinionResume> list = new List<MinionResume>();
		foreach (MinionResume item in Components.MinionResumes.Items)
		{
			if (item.HasPerk(perk))
			{
				return true;
			}
		}
		return false;
	}

	public static bool AnyOtherMinionHasPerk(string perk, MinionResume me)
	{
		List<MinionResume> list = new List<MinionResume>();
		foreach (MinionResume item in Components.MinionResumes.Items)
		{
			if (!((Object)item == (Object)me) && item.HasPerk(perk))
			{
				return true;
			}
		}
		return false;
	}

	public void ResetSkillLevels(bool returnSkillPoints = true)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
		{
			if (item.Value)
			{
				list.Add(item.Key);
			}
		}
		foreach (string item2 in list)
		{
			UnmasterSkill(item2);
		}
	}
}
