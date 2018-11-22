using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MinionResume : KMonoBehaviour, ISaveLoadable, ISim200ms
{
	[MyCmpReq]
	private MinionIdentity identity;

	[Serialize]
	public Dictionary<string, float> ExperienceByRoleID = new Dictionary<string, float>();

	[Serialize]
	public Dictionary<string, bool> MasteryByRoleID = new Dictionary<string, bool>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeByRoleGroup = new Dictionary<HashedString, float>();

	[Serialize]
	private string currentRole = "NoRole";

	[Serialize]
	private string targetRole = "NoRole";

	private RoleConfig currentRoleConfig;

	private KSelectable selectable;

	public MinionIdentity GetIdentity => identity;

	public string CurrentRole => currentRole;

	public string TargetRole => targetRole;

	[OnDeserialized]
	private void OnDeserializedMethod()
	{
		if (currentRole != "NoRole")
		{
			currentRoleConfig = Game.Instance.roleManager.GetRole(currentRole);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		selectable = GetComponent<KSelectable>();
		UpdateStatusItem();
		ExperienceByRoleID["NoRole"] = 0f;
		foreach (RoleConfig rolesConfig in Game.Instance.roleManager.RolesConfigs)
		{
			if (!MasteryByRoleID.ContainsKey(rolesConfig.id))
			{
				MasteryByRoleID.Add(rolesConfig.id, false);
			}
			if (!ExperienceByRoleID.ContainsKey(rolesConfig.id))
			{
				AddExperience(rolesConfig.id, 0f, true);
			}
			if (ExperienceByRoleID[rolesConfig.id] >= rolesConfig.experienceRequired)
			{
				MasteryByRoleID[rolesConfig.id] = true;
			}
			if (MasteryByRoleID[rolesConfig.id])
			{
				ExperienceByRoleID[rolesConfig.id] = rolesConfig.experienceRequired;
			}
			if (!AptitudeByRoleGroup.ContainsKey(rolesConfig.roleGroup))
			{
				AddAptitude(rolesConfig.roleGroup, 0f);
			}
		}
		UpgradeExperienceAndMastery();
		foreach (KeyValuePair<string, bool> item in MasteryByRoleID)
		{
			if (!(item.Key == currentRole) && item.Value)
			{
				RolePerk[] perks = Game.Instance.roleManager.GetRole(item.Key).perks;
				foreach (RolePerk rolePerk in perks)
				{
					if (rolePerk.OnRemove != null)
					{
						rolePerk.OnRemove(this);
					}
					if (rolePerk.OnApply != null)
					{
						rolePerk.OnApply(this);
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(currentRole))
		{
			Game.Instance.roleManager.RestoreRole(this, currentRole);
			if (currentRole != targetRole)
			{
				if (targetRole == "NoRole")
				{
					Game.Instance.roleManager.Unassign(this, false);
				}
				else
				{
					Game.Instance.roleManager.AssignToRole(targetRole, this, false, true);
				}
			}
		}
	}

	private void UpdateStatusItem()
	{
		if (string.IsNullOrEmpty(currentRole) || currentRole == "NoRole")
		{
			SetCurrentRole("NoRole");
			selectable.SetStatusItem(Db.Get().StatusItemCategories.Role, Db.Get().DuplicantStatusItems.NoRole, this);
		}
		else
		{
			selectable.SetStatusItem(Db.Get().StatusItemCategories.Role, Db.Get().DuplicantStatusItems.Role, this);
		}
	}

	private void UpgradeExperienceAndMastery()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, float> item in ExperienceByRoleID)
		{
			if (item.Value > 0f)
			{
				RoleAssignmentRequirement[] requirements = Game.Instance.roleManager.GetRole(item.Key).requirements;
				foreach (RoleAssignmentRequirement roleAssignmentRequirement in requirements)
				{
					if (roleAssignmentRequirement is PreviousRoleAssignmentRequirement && !list.Contains((roleAssignmentRequirement as PreviousRoleAssignmentRequirement).previousRoleID))
					{
						list.Add((roleAssignmentRequirement as PreviousRoleAssignmentRequirement).previousRoleID);
					}
				}
			}
		}
		foreach (string item2 in list)
		{
			MasteryByRoleID[item2] = true;
			ExperienceByRoleID[item2] = Game.Instance.roleManager.GetRole(item2).experienceRequired;
		}
	}

	public bool HasMasteredRole(string roleId)
	{
		return MasteryByRoleID[roleId];
	}

	public void UpdateUrge()
	{
		if (targetRole != currentRole && targetRole != "NoRole")
		{
			if (!base.gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.SwitchRole))
			{
				base.gameObject.GetComponent<ChoreConsumer>().AddUrge(Db.Get().Urges.SwitchRole);
			}
		}
		else
		{
			base.gameObject.GetComponent<ChoreConsumer>().RemoveUrge(Db.Get().Urges.SwitchRole);
		}
	}

	public RoleConfig GetCurrentRoleConfig()
	{
		return currentRoleConfig;
	}

	public void SetCurrentRole(string role_id)
	{
		currentRole = role_id;
		if (role_id == "NoRole")
		{
			currentRoleConfig = null;
		}
		else
		{
			currentRoleConfig = Game.Instance.roleManager.GetRole(currentRole);
		}
	}

	public bool IsChoreGroupInCurrentRoleGroup(ChoreGroup choregroup)
	{
		if (!(CurrentRole == "NoRole"))
		{
			RoleConfig role = Game.Instance.roleManager.GetRole(currentRole);
			return Game.Instance.roleManager.RoleGroups[role.roleGroup].choreGroupID == choregroup.Id;
		}
		return false;
	}

	public void SetTargetRole(string newRole)
	{
		targetRole = newRole;
		UpdateUrge();
	}

	public void AssumeTargetRole()
	{
		OnEnterRole(targetRole, true);
	}

	public void OnExitRole()
	{
		RoleConfig roleConfig = null;
		if (CurrentRole != null && CurrentRole != "NoRole")
		{
			roleConfig = Game.Instance.roleManager.GetRole(CurrentRole);
			SetCurrentRole("NoRole");
		}
		if (roleConfig != null)
		{
			if (!MasteryByRoleID[roleConfig.id])
			{
				RolePerk[] perks = roleConfig.perks;
				foreach (RolePerk rolePerk in perks)
				{
					if (rolePerk.OnRemove != null)
					{
						rolePerk.OnRemove(this);
					}
				}
			}
			UpdateStatusItem();
			UpdateExpectations();
			UpdateUrge();
			Game.Instance.Trigger(-1523247426, this);
		}
	}

	public void OnEnterRole(string newRole, bool changeTargetRole = true)
	{
		if (newRole == "JuniorResearcher ")
		{
			newRole = JuniorResearcher.ID;
		}
		if (newRole != "NoRole")
		{
			GameScheduler.Instance.Schedule("MoraleTutorial", 5f, delegate
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Morale);
			}, null, null);
		}
		RoleManager.ApplyRoleHat(Game.Instance.roleManager.GetRole(targetRole), GetComponent<Accessorizer>(), GetComponent<KBatchedAnimController>());
		if (changeTargetRole)
		{
			targetRole = newRole;
		}
		SetCurrentRole(newRole);
		StatusItem status_item = (!(newRole == "NoRole")) ? Db.Get().DuplicantStatusItems.Role : Db.Get().DuplicantStatusItems.NoRole;
		selectable.SetStatusItem(Db.Get().StatusItemCategories.Role, status_item, this);
		RoleConfig role = Game.Instance.roleManager.GetRole(newRole);
		Trigger(540773776, newRole);
		AddExperience(newRole, 0f, true);
		RolePerk[] perks = role.perks;
		foreach (RolePerk rolePerk in perks)
		{
			if (rolePerk.OnApply != null)
			{
				rolePerk.OnApply(this);
			}
		}
		UpdateExpectations();
		Game.Instance.Trigger(-1523247426, this);
		UpdateStatusItem();
		UpdateUrge();
		ChoreProvider component = GetComponent<ChoreProvider>();
		component.chores.Find((Chore test) => test is TakeOffHatChore)?.Cancel("User Canceled");
	}

	private string GetExperienceString()
	{
		return "";
	}

	public string GetCurrentRoleString()
	{
		string id = targetRole;
		return Game.Instance.roleManager.GetRole(id).name;
	}

	public string GetCurrentRoleDescription()
	{
		string id = targetRole;
		return Game.Instance.roleManager.GetRole(id).description;
	}

	public void Sim200ms(float dt)
	{
		if (!string.IsNullOrEmpty(CurrentRole) && CurrentRole != "NoRole" && !GetComponent<KPrefabID>().HasTag(GameTags.Dead))
		{
			AddExperience(CurrentRole, dt * ROLES.PASSIVE_EXPERIENCE_SCALE, false);
		}
	}

	public void AddExperience(string roleID, float amount, bool respectAptitude = true)
	{
		if (!(roleID == "NoRole"))
		{
			RoleConfig role = Game.Instance.roleManager.GetRole(roleID);
			float value = 0f;
			ExperienceByRoleID.TryGetValue(roleID, out value);
			float value2 = 0f;
			if (role.id != "NoRole" && !AptitudeByRoleGroup.TryGetValue(role.roleGroup, out value2))
			{
				AptitudeByRoleGroup.Add(role.roleGroup, 0f);
			}
			float num = (!respectAptitude) ? amount : (amount * (1f + value2 * (ROLES.APTITUDE_EXPERIENCE_SCALE / 100f)));
			bool flag = value != role.experienceRequired && num > 0f && value + num >= role.experienceRequired;
			if (flag)
			{
				MasteryByRoleID[role.id] = true;
			}
			value = Mathf.Clamp(value + num, 0f, role.experienceRequired);
			ExperienceByRoleID[roleID] = value;
			if ((UnityEngine.Object)selectable == (UnityEngine.Object)null)
			{
				selectable = GetComponent<KSelectable>();
			}
			if (value >= role.experienceRequired && flag)
			{
				OnRoleMastered();
			}
			if (currentRole == roleID)
			{
				selectable.SetStatusItem(Db.Get().StatusItemCategories.Role, Db.Get().DuplicantStatusItems.Role, this);
			}
		}
	}

	public void UpdateExpectations()
	{
		int num = HighestTierRole();
		foreach (KeyValuePair<string, float> item in ExperienceByRoleID)
		{
			RoleConfig role = Game.Instance.roleManager.GetRole(item.Key);
			if (item.Key == currentRole || item.Value >= Game.Instance.roleManager.GetRole(item.Key).experienceRequired)
			{
				num = Math.Max(role.tier, num);
			}
		}
		foreach (Expectation[] item2 in Expectations.ExpectationsByTier)
		{
			Expectation[] array = item2;
			foreach (Expectation expectation in array)
			{
				expectation.OnRemove(this);
			}
		}
		Expectation[] array2 = Expectations.ExpectationsByTier[num];
		foreach (Expectation expectation2 in array2)
		{
			expectation2.OnApply(this);
		}
	}

	public int HighestTierRole()
	{
		int num = 0;
		foreach (KeyValuePair<string, float> item in ExperienceByRoleID)
		{
			RoleConfig role = Game.Instance.roleManager.GetRole(item.Key);
			if (item.Key == currentRole || item.Value >= Game.Instance.roleManager.GetRole(item.Key).experienceRequired)
			{
				num = Math.Max(role.tier, num);
			}
		}
		return num;
	}

	public int HighestTierRoleMastered()
	{
		int num = 0;
		foreach (KeyValuePair<string, bool> item in MasteryByRoleID)
		{
			if (item.Value)
			{
				RoleConfig role = Game.Instance.roleManager.GetRole(item.Key);
				num = Math.Max(role.tier, num);
			}
		}
		return num;
	}

	private void OnRoleMastered()
	{
		RoleMasteredMessage message = new RoleMasteredMessage(this);
		MusicManager.instance.PlaySong("Stinger_JobMastered", false);
		Messenger.Instance.QueueMessage(message);
		if ((UnityEngine.Object)PopFXManager.Instance != (UnityEngine.Object)null)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, DUPLICANTS.ROLES.ROLE_MASTERED, base.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
		}
		StateMachine.Instance instance = new UpgradeFX.Instance(base.gameObject.GetComponent<KMonoBehaviour>(), new Vector3(0f, 0f, -0.1f));
		instance.StartSM();
	}

	public void AddAptitude(HashedString roleGroupID, float amount)
	{
		if (!AptitudeByRoleGroup.ContainsKey(roleGroupID))
		{
			AptitudeByRoleGroup.Add(roleGroupID, 0f);
		}
		Dictionary<HashedString, float> aptitudeByRoleGroup;
		HashedString key;
		(aptitudeByRoleGroup = AptitudeByRoleGroup)[key = roleGroupID] = aptitudeByRoleGroup[key] + amount;
	}

	public void AddExperienceIfRole(string roleID, float amount)
	{
		if (CurrentRole == roleID)
		{
			AddExperience(roleID, amount, true);
		}
	}

	public bool HasPerk(HashedString perk)
	{
		foreach (RoleConfig rolesConfig in Game.Instance.roleManager.RolesConfigs)
		{
			if (rolesConfig.HasPerk(perk) && MasteryByRoleID[rolesConfig.id])
			{
				return true;
			}
		}
		return currentRoleConfig != null && currentRoleConfig.HasPerk(perk);
	}

	public bool HasPerk(RolePerk perk)
	{
		foreach (RoleConfig rolesConfig in Game.Instance.roleManager.RolesConfigs)
		{
			if (rolesConfig.HasPerk(perk) && MasteryByRoleID.ContainsKey(rolesConfig.id) && MasteryByRoleID[rolesConfig.id])
			{
				return true;
			}
		}
		return currentRoleConfig != null && currentRoleConfig.HasPerk(perk);
	}
}
