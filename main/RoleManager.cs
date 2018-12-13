using Klei.AI;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RoleManager
{
	private RoleConfig noRole = new NoRole();

	private Dictionary<string, int> SlotsByRoleID = new Dictionary<string, int>();

	[Serialize]
	private List<string> achievedSlotUnlocks = new List<string>();

	private List<MinionResume> minionResumes = new List<MinionResume>();

	public RoleAssignmentRequirements roleAssignmentRequirements;

	public static readonly RolePerks rolePerks = new RolePerks();

	public Dictionary<HashedString, RoleGroup> RoleGroups = new Dictionary<HashedString, RoleGroup>
	{
		{
			"Farming",
			new RoleGroup("Farming", "Farming", DUPLICANTS.CHOREGROUPS.FARMING.NAME)
		},
		{
			"Ranching",
			new RoleGroup("Ranching", "Ranching", DUPLICANTS.CHOREGROUPS.RANCHING.NAME)
		},
		{
			"Mining",
			new RoleGroup("Mining", "Dig", DUPLICANTS.CHOREGROUPS.DIG.NAME)
		},
		{
			"Cooking",
			new RoleGroup("Cooking", "Cook", DUPLICANTS.CHOREGROUPS.COOK.NAME)
		},
		{
			"Art",
			new RoleGroup("Art", "Art", DUPLICANTS.CHOREGROUPS.ART.NAME)
		},
		{
			"Building",
			new RoleGroup("Building", "Build", DUPLICANTS.CHOREGROUPS.BUILD.NAME)
		},
		{
			"Management",
			new RoleGroup("Management", string.Empty, string.Empty)
		},
		{
			"Research",
			new RoleGroup("Research", "Research", DUPLICANTS.CHOREGROUPS.RESEARCH.NAME)
		},
		{
			"Suits",
			new RoleGroup("Suits", string.Empty, string.Empty)
		},
		{
			"Hauling",
			new RoleGroup("Hauling", "Hauling", DUPLICANTS.CHOREGROUPS.HAULING.NAME)
		},
		{
			"Technicals",
			new RoleGroup("Technicals", "MachineOperating", DUPLICANTS.CHOREGROUPS.MACHINEOPERATING.NAME)
		},
		{
			"MedicalAid",
			new RoleGroup("Doctor", "MedicalAid", DUPLICANTS.CHOREGROUPS.MEDICALAID.NAME)
		},
		{
			"Basekeeping",
			new RoleGroup("Basekeeping", "Basekeeping", DUPLICANTS.CHOREGROUPS.BASEKEEPING.NAME)
		}
	};

	public static Dictionary<string, string> roleHatIndex = new Dictionary<string, string>
	{
		{
			"NoRole",
			"hat_role_none"
		},
		{
			"JuniorFarmer",
			"hat_role_farming1"
		},
		{
			"Farmer",
			"hat_role_farming2"
		},
		{
			"SeniorFarmer",
			"hat_role_farming3"
		},
		{
			"Rancher",
			"hat_role_rancher1"
		},
		{
			"SeniorRancher",
			"hat_role_rancher2"
		},
		{
			JuniorResearcher.ID,
			"hat_role_research1"
		},
		{
			Researcher.ID,
			"hat_role_research2"
		},
		{
			SeniorResearcher.ID,
			"hat_role_research3"
		},
		{
			JuniorMiner.ID,
			"hat_role_mining1"
		},
		{
			Miner.ID,
			"hat_role_mining2"
		},
		{
			SeniorMiner.ID,
			"hat_role_mining3"
		},
		{
			JuniorCook.ID,
			"hat_role_cooking1"
		},
		{
			Cook.ID,
			"hat_role_cooking2"
		},
		{
			JuniorArtist.ID,
			"hat_role_art1"
		},
		{
			Artist.ID,
			"hat_role_art2"
		},
		{
			MasterArtist.ID,
			"hat_role_art3"
		},
		{
			"Hauler",
			"hat_role_hauling1"
		},
		{
			MaterialsManager.ID,
			"hat_role_hauling2"
		},
		{
			"SuitExpert",
			"hat_role_suits1"
		},
		{
			MachineTechnician.ID,
			"hat_role_technicals1"
		},
		{
			"PowerTechnician",
			"hat_role_technicals2"
		},
		{
			"MechatronicEngineer",
			"hat_role_engineering1"
		},
		{
			JuniorBuilder.ID,
			"hat_role_building1"
		},
		{
			Builder.ID,
			"hat_role_building2"
		},
		{
			SeniorBuilder.ID,
			"hat_role_building3"
		},
		{
			Handyman.ID,
			"hat_role_basekeeping1"
		},
		{
			Plumber.ID,
			"hat_role_basekeeping1"
		},
		{
			AstronautTrainee.ID,
			"hat_role_astronaut1"
		},
		{
			Astronaut.ID,
			"hat_role_astronaut2"
		}
	};

	private static readonly string[][] RoleRows = new string[12][]
	{
		new string[1]
		{
			"NoRole"
		},
		new string[3]
		{
			JuniorMiner.ID,
			Miner.ID,
			SeniorMiner.ID
		},
		new string[3]
		{
			JuniorBuilder.ID,
			Builder.ID,
			SeniorBuilder.ID
		},
		new string[3]
		{
			"Hauler",
			MaterialsManager.ID,
			"SuitExpert"
		},
		new string[3]
		{
			"MechatronicEngineer",
			AstronautTrainee.ID,
			Astronaut.ID
		},
		new string[3]
		{
			JuniorResearcher.ID,
			Researcher.ID,
			SeniorResearcher.ID
		},
		new string[2]
		{
			MachineTechnician.ID,
			"PowerTechnician"
		},
		new string[3]
		{
			"JuniorFarmer",
			"Farmer",
			"SeniorFarmer"
		},
		new string[2]
		{
			"Rancher",
			"SeniorRancher"
		},
		new string[2]
		{
			Handyman.ID,
			Plumber.ID
		},
		new string[2]
		{
			JuniorCook.ID,
			Cook.ID
		},
		new string[3]
		{
			JuniorArtist.ID,
			Artist.ID,
			MasterArtist.ID
		}
	};

	private Dictionary<string, int> roleRowIndex = new Dictionary<string, int>();

	public int NumberOfRows;

	private List<MinionResume> Assignees = new List<MinionResume>();

	private const int DEFAULT_MAX_SLOTS = 128;

	public List<RoleSlotUnlock> SlotUnlocks = new List<RoleSlotUnlock>
	{
		new RoleSlotUnlock("Default", "Default", "Default", new List<Tuple<string, int>>
		{
			new Tuple<string, int>("NoRole", 128),
			new Tuple<string, int>(JuniorMiner.ID, 128),
			new Tuple<string, int>(Miner.ID, 128),
			new Tuple<string, int>(SeniorMiner.ID, 128),
			new Tuple<string, int>("JuniorFarmer", 128),
			new Tuple<string, int>("Farmer", 128),
			new Tuple<string, int>("SeniorFarmer", 128),
			new Tuple<string, int>("Rancher", 128),
			new Tuple<string, int>("SeniorRancher", 128),
			new Tuple<string, int>(JuniorResearcher.ID, 128),
			new Tuple<string, int>(Researcher.ID, 128),
			new Tuple<string, int>(SeniorResearcher.ID, 128),
			new Tuple<string, int>("Hauler", 128),
			new Tuple<string, int>(JuniorBuilder.ID, 128),
			new Tuple<string, int>(Builder.ID, 128),
			new Tuple<string, int>(SeniorBuilder.ID, 128),
			new Tuple<string, int>(JuniorCook.ID, 128),
			new Tuple<string, int>(Cook.ID, 128),
			new Tuple<string, int>(MachineTechnician.ID, 128),
			new Tuple<string, int>(JuniorArtist.ID, 128),
			new Tuple<string, int>(Artist.ID, 128),
			new Tuple<string, int>(MasterArtist.ID, 128),
			new Tuple<string, int>(Handyman.ID, 128),
			new Tuple<string, int>("SuitExpert", 128),
			new Tuple<string, int>("OilTechnician", 128),
			new Tuple<string, int>("PowerTechnician", 128),
			new Tuple<string, int>(MaterialsManager.ID, 128),
			new Tuple<string, int>("MechatronicEngineer", 128),
			new Tuple<string, int>(Plumber.ID, 128),
			new Tuple<string, int>(AstronautTrainee.ID, 128),
			new Tuple<string, int>(Astronaut.ID, 128)
		}, () => true)
	};

	public List<RoleConfig> RolesConfigs
	{
		get;
		private set;
	}

	public RoleManager()
	{
		for (int i = 0; i < RoleRows.Length; i++)
		{
			string[] array = RoleRows[i];
			foreach (string key in array)
			{
				roleRowIndex[key] = i;
			}
		}
		Game.Instance.roleManager = this;
		roleAssignmentRequirements = new RoleAssignmentRequirements(this);
		InitRoleConfigs();
		RestoreSlots();
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			MinionResume component = item.GetComponent<MinionResume>();
			if ((Object)component != (Object)null)
			{
				minionResumes.Add(component);
			}
		}
		Components.LiveMinionIdentities.OnAdd += OnIDsChanged;
		Components.LiveMinionIdentities.OnRemove += OnIDsChanged;
		foreach (KeyValuePair<string, int> item2 in roleRowIndex)
		{
			if (item2.Value > NumberOfRows)
			{
				NumberOfRows = item2.Value;
			}
		}
	}

	public string GetHat(string roleID)
	{
		if (roleHatIndex.ContainsKey(roleID))
		{
			return roleHatIndex[roleID];
		}
		return string.Empty;
	}

	public int GetRowIndex(string roleID)
	{
		if (roleRowIndex.ContainsKey(roleID))
		{
			return roleRowIndex[roleID];
		}
		return -1;
	}

	private void OnIDsChanged(MinionIdentity changedID)
	{
		MinionResume component = changedID.GetComponent<MinionResume>();
		if ((Object)component == (Object)null)
		{
			foreach (MinionResume minionResume in minionResumes)
			{
				if ((Object)minionResume.GetComponent<MinionIdentity>() == (Object)changedID)
				{
					minionResumes.Remove(minionResume);
					break;
				}
			}
		}
		else
		{
			foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
			{
				if ((Object)item == (Object)changedID)
				{
					if (!minionResumes.Contains(component))
					{
						minionResumes.Add(component);
					}
					return;
				}
			}
			Unassign(component, false);
			minionResumes.Remove(component);
		}
	}

	public void RestoreRole(MinionResume resume, string roleID)
	{
		AssignToRole(roleID, resume, true, true);
	}

	public int NumberOfSlotsUnlocked(string role_id)
	{
		int num = 0;
		foreach (string achievedSlotUnlock in achievedSlotUnlocks)
		{
			foreach (Tuple<string, int> slot in GetSlotUnlock(achievedSlotUnlock).slots)
			{
				if (slot.first == role_id)
				{
					num += slot.second;
				}
			}
		}
		return num;
	}

	private void InitRoleConfigs()
	{
		List<List<RoleConfig>> list = new List<List<RoleConfig>>();
		list.Add(new List<RoleConfig>
		{
			new NoRole()
		});
		list.Add(new List<RoleConfig>
		{
			new Hauler(),
			new JuniorMiner(),
			new JuniorBuilder()
		});
		list.Add(new List<RoleConfig>
		{
			new JuniorFarmer(),
			new JuniorResearcher(),
			new JuniorCook(),
			new JuniorArtist(),
			new MachineTechnician(),
			new Handyman()
		});
		list.Add(new List<RoleConfig>
		{
			new Miner(),
			new Builder(),
			new MaterialsManager(),
			new Plumber()
		});
		list.Add(new List<RoleConfig>
		{
			new Farmer(),
			new Rancher(),
			new Researcher(),
			new Cook(),
			new Artist(),
			new PowerTechnician()
		});
		list.Add(new List<RoleConfig>
		{
			new MechatronicEngineer(),
			new SeniorMiner(),
			new SeniorBuilder()
		});
		list.Add(new List<RoleConfig>
		{
			new SeniorResearcher(),
			new SeniorFarmer(),
			new SeniorRancher(),
			new SuitExpert(),
			new MasterArtist()
		});
		list.Add(new List<RoleConfig>
		{
			new AstronautTrainee()
		});
		list.Add(new List<RoleConfig>
		{
			new Astronaut()
		});
		List<List<RoleConfig>> list2 = list;
		RolesConfigs = new List<RoleConfig>();
		for (int i = 0; i < list2.Count; i++)
		{
			foreach (RoleConfig item in list2[i])
			{
				item.SetTier(i);
				RolesConfigs.Add(item);
			}
		}
		foreach (RoleConfig rolesConfig in RolesConfigs)
		{
			SlotsByRoleID.Add(rolesConfig.id, 0);
			rolesConfig.InitRequirements();
			if (rolesConfig.id != "NoRole")
			{
				RoleGroups[rolesConfig.roleGroup].roles.Add(rolesConfig);
			}
		}
	}

	private void RestoreSlots()
	{
		foreach (RoleSlotUnlock slotUnlock in SlotUnlocks)
		{
			if (!achievedSlotUnlocks.Contains(slotUnlock.id) && slotUnlock.isSatisfied())
			{
				achievedSlotUnlocks.Add(slotUnlock.id);
			}
		}
		foreach (string achievedSlotUnlock in achievedSlotUnlocks)
		{
			UnlockSlots(SlotUnlocks.Find((RoleSlotUnlock slot) => slot.id == achievedSlotUnlock));
		}
	}

	public List<MinionResume> GetRoleAssignees(string role_id)
	{
		Assignees.Clear();
		foreach (MinionResume minionResume in minionResumes)
		{
			if (minionResume.TargetRole == role_id)
			{
				Assignees.Add(minionResume);
			}
		}
		return Assignees;
	}

	public List<MinionResume> GetRoleAssigneesWithPerk(HashedString perk_id)
	{
		Assignees.Clear();
		foreach (MinionResume minionResume in minionResumes)
		{
			if (minionResume.HasPerk(perk_id))
			{
				Assignees.Add(minionResume);
			}
		}
		return Assignees;
	}

	public RoleConfig GetRole(string id)
	{
		if (id == "JuniorResearcher ")
		{
			id = JuniorResearcher.ID;
		}
		foreach (RoleConfig rolesConfig in RolesConfigs)
		{
			if (rolesConfig.id == id)
			{
				return rolesConfig;
			}
		}
		if (id != "NoRole")
		{
			Debug.LogError("Missing role config: " + id, null);
		}
		return noRole;
	}

	public List<RoleConfig> GetRolesWithPerk(HashedString perk_id)
	{
		return RolesConfigs.FindAll((RoleConfig r) => r.HasPerk(perk_id));
	}

	private RoleSlotUnlock GetSlotUnlock(string id)
	{
		return SlotUnlocks.Find((RoleSlotUnlock r) => r.id == id);
	}

	public void UnlockSlots(RoleSlotUnlock roleSlotUnlock)
	{
		foreach (Tuple<string, int> slot in roleSlotUnlock.slots)
		{
			if (SlotsByRoleID.ContainsKey(slot.first))
			{
				Dictionary<string, int> slotsByRoleID;
				string first;
				(slotsByRoleID = SlotsByRoleID)[first = slot.first] = slotsByRoleID[first] + slot.second;
			}
		}
		if (!achievedSlotUnlocks.Contains(roleSlotUnlock.id))
		{
			achievedSlotUnlocks.Add(roleSlotUnlock.id);
		}
	}

	public string RoleTooltip(string roleID)
	{
		string empty = string.Empty;
		RoleConfig role = GetRole(roleID);
		empty = empty + "<b><size=16>" + role.name + "</size></b>";
		empty = empty + UI.HORIZONTAL_BR_RULE + role.description;
		if (roleID != "NoRole")
		{
			empty = empty + "\n\n" + RolePerkString(roleID);
			empty = empty + "\n\n" + RoleCriteriaString(roleID, null);
		}
		return empty;
	}

	public string RolePerkString(string roleID)
	{
		string text = string.Empty;
		RoleConfig role = GetRole(roleID);
		if (!(roleID == "NoRole"))
		{
			if (role.perks.Length > 0)
			{
				text = ((role.tier >= 3) ? (text + "<b>" + UI.ROLES_SCREEN.PERKS.TITLE_MORETRAINING + "</b>\n") : (text + "<b>" + UI.ROLES_SCREEN.PERKS.TITLE_BASICTRAINING + "</b>\n"));
				for (int i = 0; i < role.perks.Length; i++)
				{
					text = text + "    • " + role.perks[i].description;
					if (i < role.perks.Length - 1)
					{
						text += "\n";
					}
				}
			}
			else if (role.tier < 3)
			{
				string text2 = text;
				text = text2 + "<b>" + UI.ROLES_SCREEN.PERKS.TITLE_BASICTRAINING + "</b>\n    • " + UI.ROLES_SCREEN.PERKS.NO_PERKS;
			}
			else
			{
				string text2 = text;
				text = text2 + "<b>" + UI.ROLES_SCREEN.PERKS.TITLE_MORETRAINING + "</b>\n    • " + UI.ROLES_SCREEN.PERKS.NO_PERKS;
			}
		}
		return text;
	}

	public string RoleCriteriaString(string roleID, MinionResume resume)
	{
		string text = string.Empty;
		RoleConfig role = GetRole(roleID);
		if ((Object)resume != (Object)null)
		{
			if (resume.CurrentRole == roleID && resume.TargetRole == roleID)
			{
				if (resume.CurrentRole == "NoRole")
				{
					text += string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.ALREADY_IS_JOBLESS, resume.GetProperName());
					text += "\n\n";
				}
				else
				{
					text += string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.ALREADY_IS_ROLE, resume.GetProperName(), role.name);
					text += "\n\n";
				}
			}
			else if (resume.HasMasteredRole(roleID))
			{
				text += string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERED, resume.GetProperName(), role.name);
				text += "\n\n";
			}
			else if (resume.CurrentRole == roleID && resume.TargetRole != roleID)
			{
				text += string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.ELIGIBILITY.ELIGIBLE, resume.GetProperName(), role.name);
				text += "\n\n";
			}
			else if (CanAssignToRole(roleID, resume))
			{
				text += string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.ELIGIBILITY.ELIGIBLE, resume.GetProperName(), role.name);
				if (resume.CurrentRole != "NoRole")
				{
					text = text + "\n" + string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.WILL_BE_UNASSIGNED, resume.GetProperName(), role.name, GetRole(resume.CurrentRole).name);
				}
				text += "\n\n";
			}
			else
			{
				text += string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.ELIGIBILITY.INELIGIBLE, resume.GetProperName(), role.name);
				text += "\n\n";
			}
			AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(resume);
			int num = role.QOLExpectation();
			if (role.tier > resume.HighestTierRoleMastered() && (float)num > attributeInstance.GetTotalValue())
			{
				text = text + UIConstants.ColorPrefixRed + string.Format(UI.ROLES_SCREEN.EXPECTATION_ALERT_TARGET_JOB, attributeInstance.GetTotalValue(), num, resume.GetProperName(), role.name) + UIConstants.ColorSuffix;
				text = text + "\n" + UI.ROLES_SCREEN.EXPECTATION_ALERT_DESC_TARGET_JOB;
				text += "\n\n";
			}
			text += UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.RELEVANT_APTITUDES;
			bool flag = false;
			foreach (KeyValuePair<HashedString, float> item in resume.AptitudeByRoleGroup)
			{
				float num2 = 0f;
				string text2 = "<color=#FFFFFFFF>";
				num2 = item.Value;
				if (num2 != 0f && role.roleGroup == item.Key)
				{
					flag = true;
					text += "\n";
					text2 = ((!(num2 > 0f)) ? UIConstants.ColorPrefixRed : UIConstants.ColorPrefixGreen);
					string text3 = text;
					text = text3 + "    • " + Game.Instance.roleManager.RoleGroups[role.roleGroup].Name + ": " + text2 + "<b>" + num2 + "</b>" + UIConstants.ColorSuffix;
				}
			}
			if (!flag)
			{
				text = text + "\n    • <color=#F44A47FF>" + UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.NO_APTITUDE + "</color>";
			}
			text += "\n\n";
			text += UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.RELEVANT_ATTRIBUTES;
			Attribute[] relevantAttributes = role.relevantAttributes;
			foreach (Attribute attribute in relevantAttributes)
			{
				float num3 = 0f;
				string text4 = "<color=#FFFFFFFF>";
				text += "\n";
				num3 = resume.GetAttributes().Get(attribute).GetTotalDisplayValue();
				if (num3 != 0f)
				{
					text4 = ((!(num3 > 0f)) ? "<color=#F44A47FF>" : "<color=#BF5389FF>");
				}
				string text3 = text;
				text = text3 + "    • " + attribute.Name + ": " + text4 + "<b>" + resume.GetAttributes().Get(attribute).GetTotalDisplayValue() + "</b></color>";
			}
		}
		if ((Object)resume != (Object)null)
		{
			text += "\n\n";
		}
		if (!(roleID == "NoRole"))
		{
			if (role.requirements.Length > 0)
			{
				text = text + "<b>" + UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.TITLE + "</b>\n";
				if (!((Object)resume != (Object)null))
				{
					goto IL_054b;
				}
				goto IL_054b;
			}
			text = text + "<b>" + UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.TITLE + "</b>\n";
			text = text + "    • " + string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.NONE, role.name);
		}
		goto IL_0631;
		IL_0631:
		return text;
		IL_054b:
		for (int j = 0; j < role.requirements.Length; j++)
		{
			text += ((!((Object)resume == (Object)null) && !role.requirements[j].isSatisfied(resume)) ? "<color=#F44A47FF>" : "<color=#FFFFFF>");
			text = text + "    • " + $"{role.requirements[j].GetDescription()}";
			text += "</color>";
			if (j != role.requirements.Length - 1)
			{
				text += "\n";
			}
		}
		goto IL_0631;
	}

	public bool CanAssignToRole(string roleID, MinionResume resume)
	{
		RoleConfig role = GetRole(roleID);
		if (resume.TargetRole == roleID)
		{
			return false;
		}
		if (resume.CurrentRole == roleID && resume.TargetRole == roleID)
		{
			return false;
		}
		if (GetRoleAssignees(roleID).Count >= SlotsByRoleID[roleID])
		{
			return false;
		}
		if (DebugHandler.InstantBuildMode)
		{
			return true;
		}
		RoleAssignmentRequirement[] requirements = role.requirements;
		foreach (RoleAssignmentRequirement roleAssignmentRequirement in requirements)
		{
			if (!roleAssignmentRequirement.isSatisfied(resume))
			{
				return false;
			}
		}
		return true;
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

	public static void AddHat(string hat_idx, KBatchedAnimController controller)
	{
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		Accessory accessory = hat.Lookup(hat_idx);
		if (accessory == null)
		{
			int num = 0;
			num++;
		}
		Accessorizer component = controller.GetComponent<Accessorizer>();
		if ((Object)component != (Object)null)
		{
			Accessory accessory2 = component.GetAccessory(Db.Get().AccessorySlots.Hat);
			if (accessory2 != null)
			{
				component.RemoveAccessory(accessory2);
			}
			component.AddAccessory(accessory);
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

	public static void ApplyRoleHat(RoleConfig role, Accessorizer accessorizer, KBatchedAnimController controller)
	{
		if (role == null || string.IsNullOrEmpty(role.hat))
		{
			RemoveHat(controller);
		}
		else
		{
			AddHat(role.hat, controller);
		}
	}

	public IApproachable ClosestJobStation(GameObject from_go)
	{
		Navigator component = from_go.GetComponent<Navigator>();
		RoleStation result = null;
		float num = float.PositiveInfinity;
		foreach (RoleStation item in Components.RoleStations.Items)
		{
			float num2 = (float)component.GetNavigationCost(item);
			if (num2 < num)
			{
				num = num2;
				result = item;
			}
		}
		return result;
	}

	public void AssignToRole(string roleID, MinionResume resume, bool instant = false, bool restoring = false)
	{
		RoleConfig role = GetRole(roleID);
		if (!restoring)
		{
			Unassign(resume, true);
		}
		if (!instant && resume.CurrentRole != roleID && resume.GetComponent<ChoreProvider>().chores.Find((Chore chore) => chore.choreType == Db.Get().ChoreTypes.SwitchRole) == null && !DebugHandler.InstantBuildMode)
		{
			resume.SetTargetRole(roleID);
		}
		else
		{
			resume.OnEnterRole(roleID, !instant);
			ApplyRoleHat(role, resume.GetComponent<Accessorizer>(), resume.GetComponent<KBatchedAnimController>());
		}
		if (!restoring)
		{
			AutoAssignPersonalPriorities(roleID, resume.gameObject);
		}
		if ((Object)JobsTableScreen.Instance != (Object)null)
		{
			JobsTableScreen.Instance.Refresh(resume);
		}
	}

	public void Unassign(MinionResume resume, bool skip_refresh = false)
	{
		if (!((Object)resume == (Object)null))
		{
			ChoreConsumer component = resume.GetComponent<ChoreConsumer>();
			if ((Object)component != (Object)null)
			{
				string id = (!(resume.TargetRole != resume.CurrentRole)) ? resume.CurrentRole : resume.TargetRole;
				RoleConfig role = Game.Instance.roleManager.GetRole(id);
				if (role != null && Game.Instance.roleManager.RoleGroups.TryGetValue(role.roleGroup, out RoleGroup value))
				{
					foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
					{
						if (value.choreGroupID == resource.Id)
						{
							component.GetPersonalPriority(resource, out bool auto_assigned);
							if (auto_assigned)
							{
								int priorityBeforeAutoAssignment = component.GetPriorityBeforeAutoAssignment(resource);
								component.SetPersonalPriority(resource, priorityBeforeAutoAssignment, false);
							}
						}
					}
				}
			}
			Worker component2 = resume.GetComponent<Worker>();
			resume.GetComponent<ChoreConsumer>();
			new TakeOffHatChore(component2, Db.Get().ChoreTypes.SwitchHat);
			if (!skip_refresh && (Object)JobsTableScreen.Instance != (Object)null)
			{
				JobsTableScreen.Instance.Refresh(resume);
			}
		}
	}

	public void ResetPersonalPriorities(MinionIdentity minion)
	{
		MinionResume component = minion.GetComponent<MinionResume>();
		string roleID = (!(component.TargetRole != component.CurrentRole)) ? component.CurrentRole : component.TargetRole;
		AutoAssignPersonalPriorities(roleID, minion.gameObject);
	}

	private void AutoAssignPersonalPriorities(string roleID, GameObject minion)
	{
		if (Game.Instance.autoPrioritizeRoles)
		{
			ChoreConsumer component = minion.GetComponent<ChoreConsumer>();
			if ((Object)component != (Object)null)
			{
				RoleConfig role = Game.Instance.roleManager.GetRole(roleID);
				if (role != null && Game.Instance.roleManager.RoleGroups.TryGetValue(role.roleGroup, out RoleGroup value))
				{
					foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
					{
						if (value.choreGroupID == resource.Id)
						{
							component.SetPersonalPriority(resource, 5, true);
						}
					}
				}
			}
		}
	}
}
