using Klei.AI;
using STRINGS;
using System.Collections.Generic;

public class RoleAssignmentRequirements
{
	public const int SKILL_LEVEL_BASIC = 1;

	public const int SKILL_LEVEL_MEDIUM = 3;

	public PreviousRoleAssignmentRequirement HasExperience_AstronautTrainee;

	public PreviousRoleAssignmentRequirement HasExperience_JuniorMiner;

	public PreviousRoleAssignmentRequirement HasExperience_Miner;

	public PreviousRoleAssignmentRequirement HasExperience_JuniorResearcher;

	public PreviousRoleAssignmentRequirement HasExperience_Researcher;

	public PreviousRoleAssignmentRequirement HasExperience_SeniorResearcher;

	public PreviousRoleAssignmentRequirement HasExperience_JuniorBuilder;

	public PreviousRoleAssignmentRequirement HasExperience_Builder;

	public PreviousRoleAssignmentRequirement HasExperience_JuniorFarmer;

	public PreviousRoleAssignmentRequirement HasExperience_Farmer;

	public PreviousRoleAssignmentRequirement HasExperience_Rancher;

	public RoleAssignmentRequirement HasColonyLeader;

	public RoleAssignmentRequirement HasAttribute_Learning_Basic;

	public RoleAssignmentRequirement HasAttribute_Cooking_Basic;

	public RoleAssignmentRequirement HasAttribute_Digging_Basic;

	public RoleAssignmentRequirement HasAttribute_Learning_Medium;

	public RoleAssignmentRequirement HasExperience_MachineTechnician;

	public RoleAssignmentRequirement HasExperience_PowerTechnician;

	public RoleAssignmentRequirement HasExperience_MaterialsManager;

	public RoleAssignmentRequirement HasExperience_SuitExpert;

	public PreviousRoleAssignmentRequirement HasExperience_Hauler;

	public PreviousRoleAssignmentRequirement HasExperience_JuniorCook;

	public PreviousRoleAssignmentRequirement HasExperience_JuniorArtist;

	public PreviousRoleAssignmentRequirement HasExperience_Handyman;

	public PreviousRoleAssignmentRequirement HasExperience_MechatronicsEngineer;

	public RoleAssignmentRequirement CompletedAnyOtherRole;

	public ChoreGroupEnabledRequirement Can_Cook;

	public ChoreGroupEnabledRequirement Can_Dig;

	public ChoreGroupEnabledRequirement Can_Research;

	public ChoreGroupEnabledRequirement Can_Build;

	public ChoreGroupEnabledRequirement Can_Basekeep;

	public ChoreGroupEnabledRequirement Can_Art;

	public ChoreGroupEnabledRequirement Can_Haul;

	public ChoreGroupEnabledRequirement Can_Operate;

	public ChoreGroupEnabledRequirement Can_Combat;

	public ChoreGroupEnabledRequirement Can_MedicalAid;

	public ChoreGroupEnabledRequirement Can_Farming;

	public ChoreGroupEnabledRequirement Can_Ranching;

	public RoleAssignmentRequirements(RoleManager roleManager)
	{
		HasExperience_AstronautTrainee = new PreviousRoleAssignmentRequirement(AstronautTrainee.ID);
		HasExperience_JuniorMiner = new PreviousRoleAssignmentRequirement(JuniorMiner.ID);
		HasExperience_Miner = new PreviousRoleAssignmentRequirement(Miner.ID);
		HasExperience_JuniorResearcher = new PreviousRoleAssignmentRequirement(JuniorResearcher.ID);
		HasExperience_Researcher = new PreviousRoleAssignmentRequirement(Researcher.ID);
		HasExperience_SeniorResearcher = new PreviousRoleAssignmentRequirement(SeniorResearcher.ID);
		HasExperience_JuniorBuilder = new PreviousRoleAssignmentRequirement(JuniorBuilder.ID);
		HasExperience_Builder = new PreviousRoleAssignmentRequirement(Builder.ID);
		HasExperience_JuniorFarmer = new PreviousRoleAssignmentRequirement("JuniorFarmer");
		HasExperience_Farmer = new PreviousRoleAssignmentRequirement("Farmer");
		HasExperience_Rancher = new PreviousRoleAssignmentRequirement("Rancher");
		HasExperience_Hauler = new PreviousRoleAssignmentRequirement("Hauler");
		HasExperience_MaterialsManager = new PreviousRoleAssignmentRequirement(MaterialsManager.ID);
		HasExperience_SuitExpert = new PreviousRoleAssignmentRequirement("SuitExpert");
		HasExperience_JuniorCook = new PreviousRoleAssignmentRequirement(JuniorCook.ID);
		HasExperience_MachineTechnician = new PreviousRoleAssignmentRequirement(MachineTechnician.ID);
		HasExperience_PowerTechnician = new PreviousRoleAssignmentRequirement("PowerTechnician");
		HasExperience_JuniorArtist = new PreviousRoleAssignmentRequirement(JuniorArtist.ID);
		HasExperience_Handyman = new PreviousRoleAssignmentRequirement(Handyman.ID);
		HasExperience_MechatronicsEngineer = new PreviousRoleAssignmentRequirement("MechatronicEngineer");
		HasColonyLeader = new RoleAssignmentRequirement("HasColonyLeader", UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_COLONY_LEADER.DESCRIPTION, (MinionResume resume) => true);
		HasAttribute_Learning_Basic = new RoleAssignmentRequirement("HasAttribute_Learning_Basic", string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_ATTRIBUTE_LEARNING_BASIC.DESCRIPTION, 1), (MinionResume resume) => resume.GetAttributes().Get(Db.Get().Attributes.Learning).GetTotalValue() >= 1f);
		HasAttribute_Cooking_Basic = new RoleAssignmentRequirement("HasAttribute_Cooking_Basic", string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_ATTRIBUTE_COOKING_BASIC.DESCRIPTION, 1), (MinionResume resume) => resume.GetAttributes().Get(Db.Get().Attributes.Cooking).GetTotalValue() >= 1f);
		HasAttribute_Digging_Basic = new RoleAssignmentRequirement("HasAttribute_Digging_Basic", string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_ATTRIBUTE_DIGGING_BASIC.DESCRIPTION, 1), (MinionResume resume) => resume.GetAttributes().Get(Db.Get().Attributes.Digging).GetTotalValue() >= 1f);
		HasAttribute_Learning_Medium = new RoleAssignmentRequirement("HasAttribute_Learning_Medium", string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_ATTRIBUTE_LEARNING_MEDIUM.DESCRIPTION, 3), (MinionResume resume) => resume.GetAttributes().Get(Db.Get().Attributes.Learning).GetTotalValue() >= 3f);
		CompletedAnyOtherRole = new RoleAssignmentRequirement("CompletedAnyOtherRole", UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_COMPLETED_ANY_OTHER_ROLE.DESCRIPTION, delegate(MinionResume resume)
		{
			foreach (KeyValuePair<string, bool> item in resume.MasteryByRoleID)
			{
				if (item.Value)
				{
					return true;
				}
			}
			return false;
		});
		Can_Cook = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Cook.Id);
		Can_Dig = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Dig.Id);
		Can_Research = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Research.Id);
		Can_Build = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Build.Id);
		Can_Basekeep = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Basekeeping.Id);
		Can_Art = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Art.Id);
		Can_Haul = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Hauling.Id);
		Can_Operate = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Operating.Id);
		Can_Combat = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Combat.Id);
		Can_MedicalAid = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.MedicalAid.Id);
		Can_Farming = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Farming.Id);
		Can_Ranching = new ChoreGroupEnabledRequirement(Db.Get().ChoreGroups.Ranching.Id);
	}
}
