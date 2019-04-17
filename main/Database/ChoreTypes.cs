using STRINGS;
using System.Collections.Generic;

namespace Database
{
	public class ChoreTypes : ResourceSet<ChoreType>
	{
		public ChoreType Attack;

		public ChoreType Capture;

		public ChoreType Flee;

		public ChoreType BeIncapacitated;

		public ChoreType DebugGoTo;

		public ChoreType DeliverFood;

		public ChoreType Die;

		public ChoreType GeneShuffle;

		public ChoreType Doctor;

		public ChoreType WashHands;

		public ChoreType Shower;

		public ChoreType Eat;

		public ChoreType Entombed;

		public ChoreType Idle;

		public ChoreType MoveToQuarantine;

		public ChoreType RescueIncapacitated;

		public ChoreType RecoverBreath;

		public ChoreType Sigh;

		public ChoreType Sleep;

		public ChoreType Narcolepsy;

		public ChoreType Vomit;

		public ChoreType Cough;

		public ChoreType Pee;

		public ChoreType BreakPee;

		public ChoreType TakeMedicine;

		public ChoreType GetDoctored;

		public ChoreType RestDueToDisease;

		public ChoreType SleepDueToDisease;

		public ChoreType Heal;

		public ChoreType HealCritical;

		public ChoreType EmoteIdle;

		public ChoreType Emote;

		public ChoreType EmoteHighPriority;

		public ChoreType StressEmote;

		public ChoreType StressActingOut;

		public ChoreType Relax;

		public ChoreType StressHeal;

		public ChoreType MoveToSafety;

		public ChoreType Equip;

		public ChoreType Recharge;

		public ChoreType Unequip;

		public ChoreType Warmup;

		public ChoreType Cooldown;

		public ChoreType Mop;

		public ChoreType Relocate;

		public ChoreType Toggle;

		public ChoreType Mourn;

		public ChoreType Fetch;

		public ChoreType FetchCritical;

		public ChoreType StorageFetch;

		public ChoreType Transport;

		public ChoreType RepairFetch;

		public ChoreType MachineFetch;

		public ChoreType ResearchFetch;

		public ChoreType FarmFetch;

		public ChoreType FabricateFetch;

		public ChoreType CookFetch;

		public ChoreType PowerFetch;

		public ChoreType BuildFetch;

		public ChoreType CreatureFetch;

		public ChoreType FoodFetch;

		public ChoreType Disinfect;

		public ChoreType Repair;

		public ChoreType EmptyStorage;

		public ChoreType Deconstruct;

		public ChoreType Art;

		public ChoreType Research;

		public ChoreType GeneratePower;

		public ChoreType Harvest;

		public ChoreType Uproot;

		public ChoreType CleanToilet;

		public ChoreType LiquidCooledFan;

		public ChoreType IceCooledFan;

		public ChoreType CompostWorkable;

		public ChoreType Fabricate;

		public ChoreType FarmingFabricate;

		public ChoreType PowerFabricate;

		public ChoreType Cook;

		public ChoreType Train;

		public ChoreType Ranch;

		public ChoreType Build;

		public ChoreType BuildDig;

		public ChoreType Dig;

		public ChoreType FlipCompost;

		public ChoreType PowerTinker;

		public ChoreType MachineTinker;

		public ChoreType CropTend;

		public ChoreType Depressurize;

		public ChoreType DropUnusedInventory;

		public ChoreType StressVomit;

		public ChoreType MoveTo;

		public ChoreType UglyCry;

		public ChoreType BingeEat;

		public ChoreType StressIdle;

		public ChoreType ScrubOre;

		public ChoreType SuitMarker;

		public ChoreType ReturnSuitUrgent;

		public ChoreType ReturnSuitIdle;

		public ChoreType Checkpoint;

		public ChoreType TravelTubeEntrance;

		public ChoreType LearnSkill;

		public ChoreType SwitchHat;

		public ChoreType EggSing;

		public ChoreType Astronaut;

		public ChoreType TopPriority;

		private int nextImplicitPriority = 10000;

		private const int INVALID_PRIORITY = -1;

		public ChoreTypes(ResourceSet parent)
			: base("ChoreTypes", parent)
		{
			Die = Add("Die", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DIE.NAME, DUPLICANTS.CHORES.DIE.STATUS, DUPLICANTS.CHORES.DIE.TOOLTIP, false, -1, null);
			Entombed = Add("Entombed", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.ENTOMBED.NAME, DUPLICANTS.CHORES.ENTOMBED.STATUS, DUPLICANTS.CHORES.ENTOMBED.TOOLTIP, false, -1, null);
			SuitMarker = Add("SuitMarker", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.WASHHANDS.NAME, DUPLICANTS.CHORES.WASHHANDS.STATUS, DUPLICANTS.CHORES.WASHHANDS.TOOLTIP, false, -1, null);
			Checkpoint = Add("Checkpoint", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.CHECKPOINT.NAME, DUPLICANTS.CHORES.CHECKPOINT.STATUS, DUPLICANTS.CHORES.CHECKPOINT.TOOLTIP, false, -1, null);
			TravelTubeEntrance = Add("TravelTubeEntrance", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.TRAVELTUBEENTRANCE.NAME, DUPLICANTS.CHORES.TRAVELTUBEENTRANCE.STATUS, DUPLICANTS.CHORES.TRAVELTUBEENTRANCE.TOOLTIP, false, -1, null);
			WashHands = Add("WashHands", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.WASHHANDS.NAME, DUPLICANTS.CHORES.WASHHANDS.STATUS, DUPLICANTS.CHORES.WASHHANDS.TOOLTIP, false, -1, null);
			HealCritical = Add("HealCritical", new string[0], "HealCritical", new string[3]
			{
				"Vomit",
				"Cough",
				"EmoteHighPriority"
			}, DUPLICANTS.CHORES.HEAL.NAME, DUPLICANTS.CHORES.HEAL.STATUS, DUPLICANTS.CHORES.HEAL.TOOLTIP, false, -1, null);
			BeIncapacitated = Add("BeIncapacitated", new string[0], "BeIncapacitated", new string[0], DUPLICANTS.CHORES.BEINCAPACITATED.NAME, DUPLICANTS.CHORES.BEINCAPACITATED.STATUS, DUPLICANTS.CHORES.BEINCAPACITATED.TOOLTIP, false, -1, null);
			GeneShuffle = Add("GeneShuffle", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.GENESHUFFLE.NAME, DUPLICANTS.CHORES.GENESHUFFLE.STATUS, DUPLICANTS.CHORES.GENESHUFFLE.TOOLTIP, false, -1, null);
			DebugGoTo = Add("DebugGoTo", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DEBUGGOTO.NAME, DUPLICANTS.CHORES.DEBUGGOTO.STATUS, DUPLICANTS.CHORES.MOVETO.TOOLTIP, false, -1, null);
			MoveTo = Add("MoveTo", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.MOVETO.NAME, DUPLICANTS.CHORES.MOVETO.STATUS, DUPLICANTS.CHORES.MOVETO.TOOLTIP, false, -1, null);
			DropUnusedInventory = Add("DropUnusedInventory", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DROPUNUSEDINVENTORY.NAME, DUPLICANTS.CHORES.DROPUNUSEDINVENTORY.STATUS, DUPLICANTS.CHORES.DROPUNUSEDINVENTORY.TOOLTIP, false, -1, null);
			Pee = Add("Pee", new string[0], "Pee", new string[0], DUPLICANTS.CHORES.PEE.NAME, DUPLICANTS.CHORES.PEE.STATUS, DUPLICANTS.CHORES.PEE.TOOLTIP, false, -1, null);
			RecoverBreath = Add("RecoverBreath", new string[0], "RecoverBreath", new string[0], DUPLICANTS.CHORES.RECOVERBREATH.NAME, DUPLICANTS.CHORES.RECOVERBREATH.STATUS, DUPLICANTS.CHORES.RECOVERBREATH.TOOLTIP, false, -1, null);
			Flee = Add("Flee", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.FLEE.NAME, DUPLICANTS.CHORES.FLEE.STATUS, DUPLICANTS.CHORES.FLEE.TOOLTIP, false, -1, null);
			MoveToQuarantine = Add("MoveToQuarantine", new string[0], "MoveToQuarantine", new string[0], DUPLICANTS.CHORES.MOVETOQUARANTINE.NAME, DUPLICANTS.CHORES.MOVETOQUARANTINE.STATUS, DUPLICANTS.CHORES.MOVETOQUARANTINE.TOOLTIP, false, -1, null);
			EmoteIdle = Add("EmoteIdle", new string[0], "EmoteIdle", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.TOOLTIP, false, -1, null);
			Emote = Add("Emote", new string[0], "Emote", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.TOOLTIP, false, -1, null);
			EmoteHighPriority = Add("EmoteHighPriority", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.TOOLTIP, false, -1, null);
			StressEmote = Add("StressEmote", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.TOOLTIP, false, -1, null);
			StressVomit = Add("StressVomit", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.STRESSVOMIT.NAME, DUPLICANTS.CHORES.STRESSVOMIT.STATUS, DUPLICANTS.CHORES.STRESSVOMIT.TOOLTIP, false, -1, null);
			UglyCry = Add("UglyCry", new string[0], string.Empty, new string[1]
			{
				"MoveTo"
			}, DUPLICANTS.CHORES.UGLY_CRY.NAME, DUPLICANTS.CHORES.UGLY_CRY.STATUS, DUPLICANTS.CHORES.UGLY_CRY.TOOLTIP, false, -1, null);
			BingeEat = Add("BingeEat", new string[0], string.Empty, new string[1]
			{
				"MoveTo"
			}, DUPLICANTS.CHORES.BINGE_EAT.NAME, DUPLICANTS.CHORES.BINGE_EAT.STATUS, DUPLICANTS.CHORES.BINGE_EAT.TOOLTIP, false, -1, null);
			StressActingOut = Add("StressActingOut", new string[0], string.Empty, new string[1]
			{
				"MoveTo"
			}, DUPLICANTS.CHORES.STRESSACTINGOUT.NAME, DUPLICANTS.CHORES.STRESSACTINGOUT.STATUS, DUPLICANTS.CHORES.STRESSACTINGOUT.TOOLTIP, false, -1, null);
			Vomit = Add("Vomit", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.VOMIT.NAME, DUPLICANTS.CHORES.VOMIT.STATUS, DUPLICANTS.CHORES.VOMIT.TOOLTIP, false, -1, null);
			Cough = Add("Cough", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.COUGH.NAME, DUPLICANTS.CHORES.COUGH.STATUS, DUPLICANTS.CHORES.COUGH.TOOLTIP, false, -1, null);
			SwitchHat = Add("SwitchHat", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.LEARNSKILL.NAME, DUPLICANTS.CHORES.LEARNSKILL.STATUS, DUPLICANTS.CHORES.LEARNSKILL.TOOLTIP, false, -1, null);
			StressIdle = Add("StressIdle", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.STRESSIDLE.NAME, DUPLICANTS.CHORES.STRESSIDLE.STATUS, DUPLICANTS.CHORES.STRESSIDLE.TOOLTIP, false, -1, null);
			RescueIncapacitated = Add("RescueIncapacitated", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RESCUEINCAPACITATED.NAME, DUPLICANTS.CHORES.RESCUEINCAPACITATED.STATUS, DUPLICANTS.CHORES.RESCUEINCAPACITATED.TOOLTIP, false, -1, null);
			BreakPee = Add("BreakPee", new string[0], "Pee", new string[0], DUPLICANTS.CHORES.BREAK_PEE.NAME, DUPLICANTS.CHORES.BREAK_PEE.STATUS, DUPLICANTS.CHORES.BREAK_PEE.TOOLTIP, false, -1, null);
			Eat = Add("Eat", new string[0], "Eat", new string[0], DUPLICANTS.CHORES.EAT.NAME, DUPLICANTS.CHORES.EAT.STATUS, DUPLICANTS.CHORES.EAT.TOOLTIP, false, -1, null);
			Narcolepsy = Add("Narcolepsy", new string[0], "Narcolepsy", new string[0], DUPLICANTS.CHORES.NARCOLEPSY.NAME, DUPLICANTS.CHORES.NARCOLEPSY.STATUS, DUPLICANTS.CHORES.NARCOLEPSY.TOOLTIP, false, -1, null);
			ReturnSuitUrgent = Add("ReturnSuitUrgent", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RETURNSUIT.NAME, DUPLICANTS.CHORES.RETURNSUIT.STATUS, DUPLICANTS.CHORES.RETURNSUIT.TOOLTIP, false, -1, null);
			SleepDueToDisease = Add("SleepDueToDisease", new string[0], "Sleep", new string[3]
			{
				"Vomit",
				"Cough",
				"EmoteHighPriority"
			}, DUPLICANTS.CHORES.RESTDUETODISEASE.NAME, DUPLICANTS.CHORES.RESTDUETODISEASE.STATUS, DUPLICANTS.CHORES.RESTDUETODISEASE.TOOLTIP, false, -1, null);
			Sleep = Add("Sleep", new string[0], "Sleep", new string[0], DUPLICANTS.CHORES.SLEEP.NAME, DUPLICANTS.CHORES.SLEEP.STATUS, DUPLICANTS.CHORES.SLEEP.TOOLTIP, false, -1, null);
			TakeMedicine = Add("TakeMedicine", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.TAKEMEDICINE.NAME, DUPLICANTS.CHORES.TAKEMEDICINE.STATUS, DUPLICANTS.CHORES.TAKEMEDICINE.TOOLTIP, false, -1, null);
			GetDoctored = Add("GetDoctored", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.GETDOCTORED.NAME, DUPLICANTS.CHORES.GETDOCTORED.STATUS, DUPLICANTS.CHORES.GETDOCTORED.TOOLTIP, false, -1, null);
			RestDueToDisease = Add("RestDueToDisease", new string[0], "RestDueToDisease", new string[3]
			{
				"Vomit",
				"Cough",
				"EmoteHighPriority"
			}, DUPLICANTS.CHORES.RESTDUETODISEASE.NAME, DUPLICANTS.CHORES.RESTDUETODISEASE.STATUS, DUPLICANTS.CHORES.RESTDUETODISEASE.TOOLTIP, false, -1, null);
			ScrubOre = Add("ScrubOre", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.SCRUBORE.NAME, DUPLICANTS.CHORES.SCRUBORE.STATUS, DUPLICANTS.CHORES.SCRUBORE.TOOLTIP, false, -1, null);
			DeliverFood = Add("DeliverFood", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DELIVERFOOD.NAME, DUPLICANTS.CHORES.DELIVERFOOD.STATUS, DUPLICANTS.CHORES.DELIVERFOOD.TOOLTIP, false, -1, null);
			Sigh = Add("Sigh", new string[0], "Emote", new string[0], DUPLICANTS.CHORES.SIGH.NAME, DUPLICANTS.CHORES.SIGH.STATUS, DUPLICANTS.CHORES.SIGH.TOOLTIP, false, -1, null);
			Heal = Add("Heal", new string[0], "Heal", new string[3]
			{
				"Vomit",
				"Cough",
				"EmoteHighPriority"
			}, DUPLICANTS.CHORES.HEAL.NAME, DUPLICANTS.CHORES.HEAL.STATUS, DUPLICANTS.CHORES.HEAL.TOOLTIP, false, -1, null);
			Shower = Add("Shower", new string[0], "Shower", new string[0], DUPLICANTS.CHORES.SHOWER.NAME, DUPLICANTS.CHORES.SHOWER.STATUS, DUPLICANTS.CHORES.SHOWER.TOOLTIP, false, -1, null);
			LearnSkill = Add("LearnSkill", new string[0], "LearnSkill", new string[0], DUPLICANTS.CHORES.LEARNSKILL.NAME, DUPLICANTS.CHORES.LEARNSKILL.STATUS, DUPLICANTS.CHORES.LEARNSKILL.TOOLTIP, false, -1, null);
			Equip = Add("Equip", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.EQUIP.NAME, DUPLICANTS.CHORES.EQUIP.STATUS, DUPLICANTS.CHORES.EQUIP.TOOLTIP, false, -1, null);
			StressHeal = Add("StressHeal", new string[0], string.Empty, new string[1]
			{
				string.Empty
			}, DUPLICANTS.CHORES.STRESSHEAL.NAME, DUPLICANTS.CHORES.STRESSHEAL.STATUS, DUPLICANTS.CHORES.STRESSHEAL.TOOLTIP, false, -1, null);
			Relax = Add("Relax", new string[0], string.Empty, new string[1]
			{
				"Sleep"
			}, DUPLICANTS.CHORES.RELAX.NAME, DUPLICANTS.CHORES.RELAX.STATUS, DUPLICANTS.CHORES.RELAX.TOOLTIP, false, -1, null);
			Recharge = Add("Recharge", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RECHARGE.NAME, DUPLICANTS.CHORES.RECHARGE.STATUS, DUPLICANTS.CHORES.RECHARGE.TOOLTIP, false, -1, null);
			Unequip = Add("Unequip", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.UNEQUIP.NAME, DUPLICANTS.CHORES.UNEQUIP.STATUS, DUPLICANTS.CHORES.UNEQUIP.TOOLTIP, false, -1, null);
			Mourn = Add("Mourn", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.MOURN.NAME, DUPLICANTS.CHORES.MOURN.STATUS, DUPLICANTS.CHORES.MOURN.TOOLTIP, false, -1, null);
			TopPriority = Add("TopPriority", new string[0], string.Empty, new string[0], string.Empty, string.Empty, string.Empty, false, -1, null);
			Attack = Add("Attack", new string[1]
			{
				"Combat"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.ATTACK.NAME, DUPLICANTS.CHORES.ATTACK.STATUS, DUPLICANTS.CHORES.ATTACK.TOOLTIP, false, 5000, null);
			Doctor = Add("DoctorChore", new string[1]
			{
				"MedicalAid"
			}, "Doctor", new string[0], DUPLICANTS.CHORES.DOCTOR.NAME, DUPLICANTS.CHORES.DOCTOR.STATUS, DUPLICANTS.CHORES.DOCTOR.TOOLTIP, false, 5000, null);
			Toggle = Add("Toggle", new string[1]
			{
				"Toggle"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.TOGGLE.NAME, DUPLICANTS.CHORES.TOGGLE.STATUS, DUPLICANTS.CHORES.TOGGLE.TOOLTIP, true, 5000, null);
			Capture = Add("Capture", new string[1]
			{
				"Ranching"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.CAPTURE.NAME, DUPLICANTS.CHORES.CAPTURE.STATUS, DUPLICANTS.CHORES.CAPTURE.TOOLTIP, false, 5000, null);
			CreatureFetch = Add("CreatureFetch", new string[1]
			{
				"Ranching"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FETCHCREATURE.NAME, DUPLICANTS.CHORES.FETCHCREATURE.STATUS, DUPLICANTS.CHORES.FETCHCREATURE.TOOLTIP, false, 5000, null);
			EggSing = Add("EggSing", new string[1]
			{
				"Ranching"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.SINGTOEGG.NAME, DUPLICANTS.CHORES.SINGTOEGG.STATUS, DUPLICANTS.CHORES.SINGTOEGG.TOOLTIP, false, 5000, null);
			Astronaut = Add("Astronaut", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.ASTRONAUT.NAME, DUPLICANTS.CHORES.ASTRONAUT.STATUS, DUPLICANTS.CHORES.ASTRONAUT.TOOLTIP, false, 5000, null);
			FetchCritical = Add("FetchCritical", new string[2]
			{
				"Hauling",
				"LifeSupport"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FETCHCRITICAL.NAME, DUPLICANTS.CHORES.FETCHCRITICAL.STATUS, DUPLICANTS.CHORES.FETCHCRITICAL.TOOLTIP, false, 5000, DUPLICANTS.CHORES.FETCHCRITICAL.REPORT_NAME);
			Art = Add("Art", new string[1]
			{
				"Art"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.ART.NAME, DUPLICANTS.CHORES.ART.STATUS, DUPLICANTS.CHORES.ART.TOOLTIP, false, 5000, null);
			EmptyStorage = Add("EmptyStorage", new string[2]
			{
				"Basekeeping",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.EMPTYSTORAGE.NAME, DUPLICANTS.CHORES.EMPTYSTORAGE.STATUS, DUPLICANTS.CHORES.EMPTYSTORAGE.TOOLTIP, false, 5000, null);
			Mop = Add("Mop", new string[1]
			{
				"Basekeeping"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.MOP.NAME, DUPLICANTS.CHORES.MOP.STATUS, DUPLICANTS.CHORES.MOP.TOOLTIP, true, 5000, null);
			Relocate = Add("Relocate", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RELOCATE.NAME, DUPLICANTS.CHORES.RELOCATE.STATUS, DUPLICANTS.CHORES.RELOCATE.TOOLTIP, true, 5000, null);
			Disinfect = Add("Disinfect", new string[1]
			{
				"Basekeeping"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.DISINFECT.NAME, DUPLICANTS.CHORES.DISINFECT.STATUS, DUPLICANTS.CHORES.DISINFECT.TOOLTIP, true, 5000, null);
			Repair = Add("Repair", new string[1]
			{
				"Basekeeping"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.REPAIR.NAME, DUPLICANTS.CHORES.REPAIR.STATUS, DUPLICANTS.CHORES.REPAIR.TOOLTIP, false, 5000, null);
			RepairFetch = Add("RepairFetch", new string[2]
			{
				"Basekeeping",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.REPAIRFETCH.NAME, DUPLICANTS.CHORES.REPAIRFETCH.STATUS, DUPLICANTS.CHORES.REPAIRFETCH.TOOLTIP, false, 5000, null);
			Deconstruct = Add("Deconstruct", new string[1]
			{
				"Build"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.DECONSTRUCT.NAME, DUPLICANTS.CHORES.DECONSTRUCT.STATUS, DUPLICANTS.CHORES.DECONSTRUCT.TOOLTIP, false, 5000, null);
			Research = Add("Research", new string[1]
			{
				"Research"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.RESEARCH.NAME, DUPLICANTS.CHORES.RESEARCH.STATUS, DUPLICANTS.CHORES.RESEARCH.TOOLTIP, false, 5000, null);
			ResearchFetch = Add("ResearchFetch", new string[2]
			{
				"Research",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.RESEARCHFETCH.NAME, DUPLICANTS.CHORES.RESEARCHFETCH.STATUS, DUPLICANTS.CHORES.RESEARCHFETCH.TOOLTIP, false, 5000, null);
			GeneratePower = Add("GeneratePower", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[1]
			{
				"StressHeal"
			}, DUPLICANTS.CHORES.GENERATEPOWER.NAME, DUPLICANTS.CHORES.GENERATEPOWER.STATUS, DUPLICANTS.CHORES.GENERATEPOWER.TOOLTIP, false, 5000, null);
			CropTend = Add("CropTend", new string[1]
			{
				"Farming"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.CROP_TEND.NAME, DUPLICANTS.CHORES.CROP_TEND.STATUS, DUPLICANTS.CHORES.CROP_TEND.TOOLTIP, false, 5000, null);
			PowerTinker = Add("PowerTinker", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.POWER_TINKER.NAME, DUPLICANTS.CHORES.POWER_TINKER.STATUS, DUPLICANTS.CHORES.POWER_TINKER.TOOLTIP, false, 5000, null);
			MachineTinker = Add("MachineTinker", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.POWER_TINKER.NAME, DUPLICANTS.CHORES.POWER_TINKER.STATUS, DUPLICANTS.CHORES.POWER_TINKER.TOOLTIP, false, 5000, null);
			MachineFetch = Add("MachineFetch", new string[2]
			{
				"MachineOperating",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.MACHINEFETCH.NAME, DUPLICANTS.CHORES.MACHINEFETCH.STATUS, DUPLICANTS.CHORES.MACHINEFETCH.TOOLTIP, false, 5000, DUPLICANTS.CHORES.MACHINEFETCH.REPORT_NAME);
			Harvest = Add("Harvest", new string[1]
			{
				"Farming"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.HARVEST.NAME, DUPLICANTS.CHORES.HARVEST.STATUS, DUPLICANTS.CHORES.HARVEST.TOOLTIP, false, 5000, null);
			FarmFetch = Add("FarmFetch", new string[2]
			{
				"Farming",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FARMFETCH.NAME, DUPLICANTS.CHORES.FARMFETCH.STATUS, DUPLICANTS.CHORES.FARMFETCH.TOOLTIP, false, 5000, null);
			Uproot = Add("Uproot", new string[1]
			{
				"Farming"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.UPROOT.NAME, DUPLICANTS.CHORES.UPROOT.STATUS, DUPLICANTS.CHORES.UPROOT.TOOLTIP, false, 5000, null);
			CleanToilet = Add("CleanToilet", new string[1]
			{
				"Basekeeping"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.CLEANTOILET.NAME, DUPLICANTS.CHORES.CLEANTOILET.STATUS, DUPLICANTS.CHORES.CLEANTOILET.TOOLTIP, false, 5000, null);
			LiquidCooledFan = Add("LiquidCooledFan", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.LIQUIDCOOLEDFAN.NAME, DUPLICANTS.CHORES.LIQUIDCOOLEDFAN.STATUS, DUPLICANTS.CHORES.LIQUIDCOOLEDFAN.TOOLTIP, false, 5000, null);
			IceCooledFan = Add("IceCooledFan", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.ICECOOLEDFAN.NAME, DUPLICANTS.CHORES.ICECOOLEDFAN.STATUS, DUPLICANTS.CHORES.ICECOOLEDFAN.TOOLTIP, false, 5000, null);
			Train = Add("Train", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.TRAIN.NAME, DUPLICANTS.CHORES.TRAIN.STATUS, DUPLICANTS.CHORES.TRAIN.TOOLTIP, false, 5000, null);
			Cook = Add("Cook", new string[1]
			{
				"Cook"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.COOK.NAME, DUPLICANTS.CHORES.COOK.STATUS, DUPLICANTS.CHORES.COOK.TOOLTIP, false, 5000, null);
			CookFetch = Add("CookFetch", new string[2]
			{
				"Cook",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.COOKFETCH.NAME, DUPLICANTS.CHORES.COOKFETCH.STATUS, DUPLICANTS.CHORES.COOKFETCH.TOOLTIP, false, 5000, null);
			Ranch = Add("Ranch", new string[1]
			{
				"Ranching"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.RANCH.NAME, DUPLICANTS.CHORES.RANCH.STATUS, DUPLICANTS.CHORES.RANCH.TOOLTIP, false, 5000, null);
			PowerFetch = Add("PowerFetch", new string[2]
			{
				"MachineOperating",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.POWERFETCH.NAME, DUPLICANTS.CHORES.POWERFETCH.STATUS, DUPLICANTS.CHORES.POWERFETCH.TOOLTIP, false, 5000, DUPLICANTS.CHORES.POWERFETCH.REPORT_NAME);
			FlipCompost = Add("FlipCompost", new string[1]
			{
				"Farming"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FLIPCOMPOST.NAME, DUPLICANTS.CHORES.FLIPCOMPOST.STATUS, DUPLICANTS.CHORES.FLIPCOMPOST.TOOLTIP, false, 5000, null);
			Depressurize = Add("Depressurize", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.DEPRESSURIZE.NAME, DUPLICANTS.CHORES.DEPRESSURIZE.STATUS, DUPLICANTS.CHORES.DEPRESSURIZE.TOOLTIP, false, 5000, null);
			FarmingFabricate = Add("FarmingFabricate", new string[1]
			{
				"Farming"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FABRICATE.NAME, DUPLICANTS.CHORES.FABRICATE.STATUS, DUPLICANTS.CHORES.FABRICATE.TOOLTIP, false, 5000, null);
			PowerFabricate = Add("PowerFabricate", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FABRICATE.NAME, DUPLICANTS.CHORES.FABRICATE.STATUS, DUPLICANTS.CHORES.FABRICATE.TOOLTIP, false, 5000, null);
			Fabricate = Add("Fabricate", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FABRICATE.NAME, DUPLICANTS.CHORES.FABRICATE.STATUS, DUPLICANTS.CHORES.FABRICATE.TOOLTIP, false, 5000, null);
			FabricateFetch = Add("FabricateFetch", new string[2]
			{
				"MachineOperating",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FABRICATEFETCH.NAME, DUPLICANTS.CHORES.FABRICATEFETCH.STATUS, DUPLICANTS.CHORES.FABRICATEFETCH.TOOLTIP, false, 5000, DUPLICANTS.CHORES.FABRICATEFETCH.REPORT_NAME);
			FoodFetch = Add("FoodFetch", new string[1]
			{
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FOODFETCH.NAME, DUPLICANTS.CHORES.FOODFETCH.STATUS, DUPLICANTS.CHORES.FOODFETCH.TOOLTIP, false, 5000, DUPLICANTS.CHORES.FOODFETCH.REPORT_NAME);
			Transport = Add("Transport", new string[2]
			{
				"Hauling",
				"Basekeeping"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.TRANSPORT.NAME, DUPLICANTS.CHORES.TRANSPORT.STATUS, DUPLICANTS.CHORES.TRANSPORT.TOOLTIP, true, 5000, null);
			Build = Add("Build", new string[1]
			{
				"Build"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.BUILD.NAME, DUPLICANTS.CHORES.BUILD.STATUS, DUPLICANTS.CHORES.BUILD.TOOLTIP, true, 5000, null);
			BuildDig = Add("BuildDig", new string[2]
			{
				"Build",
				"Dig"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.BUILDDIG.NAME, DUPLICANTS.CHORES.BUILDDIG.STATUS, DUPLICANTS.CHORES.BUILDDIG.TOOLTIP, true, 5000, null);
			BuildFetch = Add("BuildFetch", new string[2]
			{
				"Build",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.BUILDFETCH.NAME, DUPLICANTS.CHORES.BUILDFETCH.STATUS, DUPLICANTS.CHORES.BUILDFETCH.TOOLTIP, true, 5000, null);
			Dig = Add("Dig", new string[1]
			{
				"Dig"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.DIG.NAME, DUPLICANTS.CHORES.DIG.STATUS, DUPLICANTS.CHORES.DIG.TOOLTIP, false, 5000, null);
			Fetch = Add("Fetch", new string[1]
			{
				"Storage"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false, 5000, DUPLICANTS.CHORES.FETCH.REPORT_NAME);
			StorageFetch = Add("StorageFetch", new string[1]
			{
				"Storage"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.STORAGEFETCH.NAME, DUPLICANTS.CHORES.STORAGEFETCH.STATUS, DUPLICANTS.CHORES.STORAGEFETCH.TOOLTIP, true, 5000, DUPLICANTS.CHORES.STORAGEFETCH.REPORT_NAME);
			MoveToSafety = Add("MoveToSafety", new string[0], "MoveToSafety", new string[0], DUPLICANTS.CHORES.MOVETOSAFETY.NAME, DUPLICANTS.CHORES.MOVETOSAFETY.STATUS, DUPLICANTS.CHORES.MOVETOSAFETY.TOOLTIP, false, -1, null);
			ReturnSuitIdle = Add("ReturnSuitIdle", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RETURNSUIT.NAME, DUPLICANTS.CHORES.RETURNSUIT.STATUS, DUPLICANTS.CHORES.RETURNSUIT.TOOLTIP, false, -1, null);
			Idle = Add("IdleChore", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.IDLE.NAME, DUPLICANTS.CHORES.IDLE.STATUS, DUPLICANTS.CHORES.IDLE.TOOLTIP, false, -1, null);
			ChoreType[][] array = new ChoreType[29][]
			{
				new ChoreType[1]
				{
					Die
				},
				new ChoreType[1]
				{
					Entombed
				},
				new ChoreType[1]
				{
					HealCritical
				},
				new ChoreType[2]
				{
					BeIncapacitated,
					GeneShuffle
				},
				new ChoreType[1]
				{
					DebugGoTo
				},
				new ChoreType[1]
				{
					StressVomit
				},
				new ChoreType[1]
				{
					MoveTo
				},
				new ChoreType[1]
				{
					RecoverBreath
				},
				new ChoreType[1]
				{
					ReturnSuitUrgent
				},
				new ChoreType[1]
				{
					UglyCry
				},
				new ChoreType[1]
				{
					BingeEat
				},
				new ChoreType[8]
				{
					EmoteHighPriority,
					StressActingOut,
					Vomit,
					Cough,
					Pee,
					StressIdle,
					RescueIncapacitated,
					SwitchHat
				},
				new ChoreType[1]
				{
					MoveToQuarantine
				},
				new ChoreType[1]
				{
					TopPriority
				},
				new ChoreType[1]
				{
					Attack
				},
				new ChoreType[1]
				{
					Flee
				},
				new ChoreType[3]
				{
					LearnSkill,
					Eat,
					BreakPee
				},
				new ChoreType[1]
				{
					TakeMedicine
				},
				new ChoreType[3]
				{
					Heal,
					SleepDueToDisease,
					RestDueToDisease
				},
				new ChoreType[2]
				{
					Sleep,
					Narcolepsy
				},
				new ChoreType[1]
				{
					GetDoctored
				},
				new ChoreType[1]
				{
					Emote
				},
				new ChoreType[1]
				{
					Mourn
				},
				new ChoreType[1]
				{
					StressHeal
				},
				new ChoreType[1]
				{
					Relax
				},
				new ChoreType[2]
				{
					Equip,
					Unequip
				},
				new ChoreType[59]
				{
					DeliverFood,
					Sigh,
					EmptyStorage,
					Repair,
					Disinfect,
					Shower,
					CleanToilet,
					LiquidCooledFan,
					IceCooledFan,
					SuitMarker,
					Checkpoint,
					TravelTubeEntrance,
					WashHands,
					Doctor,
					Recharge,
					FetchCritical,
					ScrubOre,
					Ranch,
					MoveToSafety,
					Relocate,
					Research,
					ResearchFetch,
					Mop,
					Toggle,
					Deconstruct,
					Capture,
					CreatureFetch,
					EggSing,
					Fetch,
					Transport,
					Art,
					GeneratePower,
					CropTend,
					PowerTinker,
					MachineTinker,
					DropUnusedInventory,
					Harvest,
					FarmFetch,
					Uproot,
					FarmingFabricate,
					PowerFabricate,
					Fabricate,
					Train,
					Cook,
					Build,
					Dig,
					BuildDig,
					FlipCompost,
					Depressurize,
					BuildFetch,
					CookFetch,
					MachineFetch,
					PowerFetch,
					FabricateFetch,
					StressEmote,
					FoodFetch,
					StorageFetch,
					RepairFetch,
					Astronaut
				},
				new ChoreType[2]
				{
					ReturnSuitIdle,
					EmoteIdle
				},
				new ChoreType[1]
				{
					Idle
				}
			};
			string text = string.Empty;
			int num = 100000;
			ChoreType[][] array2 = array;
			foreach (ChoreType[] array3 in array2)
			{
				ChoreType[] array4 = array3;
				foreach (ChoreType choreType in array4)
				{
					if (choreType.interruptPriority != 0)
					{
						text = text + "Interrupt priority set more than once: " + choreType.Id;
					}
					choreType.interruptPriority = num;
				}
				num -= 100;
			}
			if (!string.IsNullOrEmpty(text))
			{
				Debug.LogError(text);
			}
			string text2 = string.Empty;
			foreach (ChoreType resource in resources)
			{
				if (resource.interruptPriority == 0)
				{
					text2 = text2 + "Interrupt priority missing for: " + resource.Id + "\n";
				}
			}
			if (!string.IsNullOrEmpty(text2))
			{
				Debug.LogError(text2);
			}
		}

		public ChoreType GetByHash(HashedString id_hash)
		{
			int num = resources.FindIndex((ChoreType item) => item.IdHash == id_hash);
			if (num != -1)
			{
				return resources[num];
			}
			return null;
		}

		private ChoreType Add(string id, string[] chore_groups, string urge, string[] interrupt_exclusion, string name, string status_message, string tooltip, bool skip_implicit_priority_change, int explicit_priority = -1, string report_name = null)
		{
			List<Tag> list = new List<Tag>();
			for (int i = 0; i < interrupt_exclusion.Length; i++)
			{
				list.Add(TagManager.Create(interrupt_exclusion[i]));
			}
			if (explicit_priority == -1)
			{
				explicit_priority = nextImplicitPriority;
			}
			ChoreType choreType = new ChoreType(id, this, chore_groups, urge, name, status_message, tooltip, list.ToArray(), nextImplicitPriority, explicit_priority);
			if (!skip_implicit_priority_change)
			{
				nextImplicitPriority -= 100;
			}
			if (report_name != null)
			{
				choreType.reportName = report_name;
			}
			return choreType;
		}
	}
}
