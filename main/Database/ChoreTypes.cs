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

		public ChoreType OperateFetch;

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

		public ChoreType MachineFetch;

		public ChoreType CropTend;

		public ChoreType Depressurize;

		public ChoreType Transport;

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

		public ChoreType SwitchRole;

		public ChoreType SwitchHat;

		public ChoreType EggSing;

		public ChoreType Astronaut;

		private int nextImplicitPriority = 10000;

		private const int INVALID_PRIORITY = -1;

		public ChoreTypes(ResourceSet parent)
			: base("ChoreTypes", parent)
		{
			Die = Add("Die", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DIE.NAME, DUPLICANTS.CHORES.DIE.STATUS, DUPLICANTS.CHORES.DIE.TOOLTIP, false, -1);
			Entombed = Add("Entombed", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.ENTOMBED.NAME, DUPLICANTS.CHORES.ENTOMBED.STATUS, DUPLICANTS.CHORES.ENTOMBED.TOOLTIP, false, -1);
			SuitMarker = Add("SuitMarker", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.WASHHANDS.NAME, DUPLICANTS.CHORES.WASHHANDS.STATUS, DUPLICANTS.CHORES.WASHHANDS.TOOLTIP, false, -1);
			Checkpoint = Add("Checkpoint", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.CHECKPOINT.NAME, DUPLICANTS.CHORES.CHECKPOINT.STATUS, DUPLICANTS.CHORES.CHECKPOINT.TOOLTIP, false, -1);
			TravelTubeEntrance = Add("TravelTubeEntrance", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.TRAVELTUBEENTRANCE.NAME, DUPLICANTS.CHORES.TRAVELTUBEENTRANCE.STATUS, DUPLICANTS.CHORES.TRAVELTUBEENTRANCE.TOOLTIP, false, -1);
			WashHands = Add("WashHands", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.WASHHANDS.NAME, DUPLICANTS.CHORES.WASHHANDS.STATUS, DUPLICANTS.CHORES.WASHHANDS.TOOLTIP, false, -1);
			HealCritical = Add("HealCritical", new string[0], "HealCritical", new string[3]
			{
				"Vomit",
				"Cough",
				"EmoteHighPriority"
			}, DUPLICANTS.CHORES.HEAL.NAME, DUPLICANTS.CHORES.HEAL.STATUS, DUPLICANTS.CHORES.HEAL.TOOLTIP, false, -1);
			BeIncapacitated = Add("BeIncapacitated", new string[0], "BeIncapacitated", new string[0], DUPLICANTS.CHORES.BEINCAPACITATED.NAME, DUPLICANTS.CHORES.BEINCAPACITATED.STATUS, DUPLICANTS.CHORES.BEINCAPACITATED.TOOLTIP, false, -1);
			GeneShuffle = Add("GeneShuffle", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.GENESHUFFLE.NAME, DUPLICANTS.CHORES.GENESHUFFLE.STATUS, DUPLICANTS.CHORES.GENESHUFFLE.TOOLTIP, false, -1);
			DebugGoTo = Add("DebugGoTo", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DEBUGGOTO.NAME, DUPLICANTS.CHORES.DEBUGGOTO.STATUS, DUPLICANTS.CHORES.MOVETO.TOOLTIP, false, -1);
			MoveTo = Add("MoveTo", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.MOVETO.NAME, DUPLICANTS.CHORES.MOVETO.STATUS, DUPLICANTS.CHORES.MOVETO.TOOLTIP, false, -1);
			DropUnusedInventory = Add("DropUnusedInventory", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DROPUNUSEDINVENTORY.NAME, DUPLICANTS.CHORES.DROPUNUSEDINVENTORY.STATUS, DUPLICANTS.CHORES.DROPUNUSEDINVENTORY.TOOLTIP, false, -1);
			Pee = Add("Pee", new string[0], "Pee", new string[0], DUPLICANTS.CHORES.PEE.NAME, DUPLICANTS.CHORES.PEE.STATUS, DUPLICANTS.CHORES.PEE.TOOLTIP, false, -1);
			RecoverBreath = Add("RecoverBreath", new string[0], "RecoverBreath", new string[0], DUPLICANTS.CHORES.RECOVERBREATH.NAME, DUPLICANTS.CHORES.RECOVERBREATH.STATUS, DUPLICANTS.CHORES.RECOVERBREATH.TOOLTIP, false, -1);
			Flee = Add("Flee", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.FLEE.NAME, DUPLICANTS.CHORES.FLEE.STATUS, DUPLICANTS.CHORES.FLEE.TOOLTIP, false, -1);
			MoveToQuarantine = Add("MoveToQuarantine", new string[0], "MoveToQuarantine", new string[0], DUPLICANTS.CHORES.MOVETOQUARANTINE.NAME, DUPLICANTS.CHORES.MOVETOQUARANTINE.STATUS, DUPLICANTS.CHORES.MOVETOQUARANTINE.TOOLTIP, false, -1);
			EmoteIdle = Add("EmoteIdle", new string[0], "EmoteIdle", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.TOOLTIP, false, -1);
			Emote = Add("Emote", new string[0], "Emote", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.TOOLTIP, false, -1);
			EmoteHighPriority = Add("EmoteHighPriority", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.TOOLTIP, false, -1);
			StressEmote = Add("StressEmote", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.TOOLTIP, false, -1);
			StressVomit = Add("StressVomit", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.STRESSVOMIT.NAME, DUPLICANTS.CHORES.STRESSVOMIT.STATUS, DUPLICANTS.CHORES.STRESSVOMIT.TOOLTIP, false, -1);
			UglyCry = Add("UglyCry", new string[0], string.Empty, new string[1]
			{
				"MoveTo"
			}, DUPLICANTS.CHORES.UGLY_CRY.NAME, DUPLICANTS.CHORES.UGLY_CRY.STATUS, DUPLICANTS.CHORES.UGLY_CRY.TOOLTIP, false, -1);
			BingeEat = Add("BingeEat", new string[0], string.Empty, new string[1]
			{
				"MoveTo"
			}, DUPLICANTS.CHORES.BINGE_EAT.NAME, DUPLICANTS.CHORES.BINGE_EAT.STATUS, DUPLICANTS.CHORES.BINGE_EAT.TOOLTIP, false, -1);
			StressActingOut = Add("StressActingOut", new string[0], string.Empty, new string[1]
			{
				"MoveTo"
			}, DUPLICANTS.CHORES.STRESSACTINGOUT.NAME, DUPLICANTS.CHORES.STRESSACTINGOUT.STATUS, DUPLICANTS.CHORES.STRESSACTINGOUT.TOOLTIP, false, -1);
			Vomit = Add("Vomit", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.VOMIT.NAME, DUPLICANTS.CHORES.VOMIT.STATUS, DUPLICANTS.CHORES.VOMIT.TOOLTIP, false, -1);
			Cough = Add("Cough", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.COUGH.NAME, DUPLICANTS.CHORES.COUGH.STATUS, DUPLICANTS.CHORES.COUGH.TOOLTIP, false, -1);
			StressIdle = Add("StressIdle", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.STRESSIDLE.NAME, DUPLICANTS.CHORES.STRESSIDLE.STATUS, DUPLICANTS.CHORES.STRESSIDLE.TOOLTIP, false, -1);
			RescueIncapacitated = Add("RescueIncapacitated", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RESCUEINCAPACITATED.NAME, DUPLICANTS.CHORES.RESCUEINCAPACITATED.STATUS, DUPLICANTS.CHORES.RESCUEINCAPACITATED.TOOLTIP, false, -1);
			BreakPee = Add("BreakPee", new string[0], "Pee", new string[0], DUPLICANTS.CHORES.BREAK_PEE.NAME, DUPLICANTS.CHORES.BREAK_PEE.STATUS, DUPLICANTS.CHORES.BREAK_PEE.TOOLTIP, false, -1);
			Eat = Add("Eat", new string[0], "Eat", new string[0], DUPLICANTS.CHORES.EAT.NAME, DUPLICANTS.CHORES.EAT.STATUS, DUPLICANTS.CHORES.EAT.TOOLTIP, false, -1);
			Narcolepsy = Add("Narcolepsy", new string[0], "Narcolepsy", new string[0], DUPLICANTS.CHORES.NARCOLEPSY.NAME, DUPLICANTS.CHORES.NARCOLEPSY.STATUS, DUPLICANTS.CHORES.NARCOLEPSY.TOOLTIP, false, -1);
			ReturnSuitUrgent = Add("ReturnSuitUrgent", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RETURNSUIT.NAME, DUPLICANTS.CHORES.RETURNSUIT.STATUS, DUPLICANTS.CHORES.RETURNSUIT.TOOLTIP, false, -1);
			SleepDueToDisease = Add("SleepDueToDisease", new string[0], "Sleep", new string[3]
			{
				"Vomit",
				"Cough",
				"EmoteHighPriority"
			}, DUPLICANTS.CHORES.RESTDUETODISEASE.NAME, DUPLICANTS.CHORES.RESTDUETODISEASE.STATUS, DUPLICANTS.CHORES.RESTDUETODISEASE.TOOLTIP, false, -1);
			Sleep = Add("Sleep", new string[0], "Sleep", new string[0], DUPLICANTS.CHORES.SLEEP.NAME, DUPLICANTS.CHORES.SLEEP.STATUS, DUPLICANTS.CHORES.SLEEP.TOOLTIP, false, -1);
			TakeMedicine = Add("TakeMedicine", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.TAKEMEDICINE.NAME, DUPLICANTS.CHORES.TAKEMEDICINE.STATUS, DUPLICANTS.CHORES.TAKEMEDICINE.TOOLTIP, false, -1);
			RestDueToDisease = Add("RestDueToDisease", new string[0], "RestDueToDisease", new string[3]
			{
				"Vomit",
				"Cough",
				"EmoteHighPriority"
			}, DUPLICANTS.CHORES.RESTDUETODISEASE.NAME, DUPLICANTS.CHORES.RESTDUETODISEASE.STATUS, DUPLICANTS.CHORES.RESTDUETODISEASE.TOOLTIP, false, -1);
			ScrubOre = Add("ScrubOre", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.SCRUBORE.NAME, DUPLICANTS.CHORES.SCRUBORE.STATUS, DUPLICANTS.CHORES.SCRUBORE.TOOLTIP, false, -1);
			DeliverFood = Add("DeliverFood", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DELIVERFOOD.NAME, DUPLICANTS.CHORES.DELIVERFOOD.STATUS, DUPLICANTS.CHORES.DELIVERFOOD.TOOLTIP, false, -1);
			Sigh = Add("Sigh", new string[0], "Emote", new string[0], DUPLICANTS.CHORES.SIGH.NAME, DUPLICANTS.CHORES.SIGH.STATUS, DUPLICANTS.CHORES.SIGH.TOOLTIP, false, -1);
			Heal = Add("Heal", new string[0], "Heal", new string[3]
			{
				"Vomit",
				"Cough",
				"EmoteHighPriority"
			}, DUPLICANTS.CHORES.HEAL.NAME, DUPLICANTS.CHORES.HEAL.STATUS, DUPLICANTS.CHORES.HEAL.TOOLTIP, false, -1);
			Shower = Add("Shower", new string[0], "Shower", new string[0], DUPLICANTS.CHORES.SHOWER.NAME, DUPLICANTS.CHORES.SHOWER.STATUS, DUPLICANTS.CHORES.SHOWER.TOOLTIP, false, -1);
			StressHeal = Add("StressHeal", new string[0], string.Empty, new string[1]
			{
				string.Empty
			}, DUPLICANTS.CHORES.STRESSHEAL.NAME, DUPLICANTS.CHORES.STRESSHEAL.STATUS, DUPLICANTS.CHORES.STRESSHEAL.TOOLTIP, false, -1);
			Relax = Add("Relax", new string[0], string.Empty, new string[1]
			{
				"Sleep"
			}, DUPLICANTS.CHORES.RELAX.NAME, DUPLICANTS.CHORES.RELAX.STATUS, DUPLICANTS.CHORES.RELAX.TOOLTIP, false, -1);
			Equip = Add("Equip", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.EQUIP.NAME, DUPLICANTS.CHORES.EQUIP.STATUS, DUPLICANTS.CHORES.EQUIP.TOOLTIP, false, -1);
			Recharge = Add("Recharge", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RECHARGE.NAME, DUPLICANTS.CHORES.RECHARGE.STATUS, DUPLICANTS.CHORES.RECHARGE.TOOLTIP, false, -1);
			SwitchHat = Add("SwitchHat", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.SWITCHROLE.NAME, DUPLICANTS.CHORES.SWITCHROLE.STATUS, DUPLICANTS.CHORES.SWITCHROLE.TOOLTIP, false, -1);
			SwitchRole = Add("SwitchRole", new string[0], "SwitchRole", new string[0], DUPLICANTS.CHORES.SWITCHROLE.NAME, DUPLICANTS.CHORES.SWITCHROLE.STATUS, DUPLICANTS.CHORES.SWITCHROLE.TOOLTIP, false, -1);
			Unequip = Add("Unequip", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.UNEQUIP.NAME, DUPLICANTS.CHORES.UNEQUIP.STATUS, DUPLICANTS.CHORES.UNEQUIP.TOOLTIP, false, -1);
			Mourn = Add("Mourn", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.MOURN.NAME, DUPLICANTS.CHORES.MOURN.STATUS, DUPLICANTS.CHORES.MOURN.TOOLTIP, false, -1);
			Attack = Add("Attack", new string[1]
			{
				"Combat"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.ATTACK.NAME, DUPLICANTS.CHORES.ATTACK.STATUS, DUPLICANTS.CHORES.ATTACK.TOOLTIP, false, 5000);
			Doctor = Add("DoctorChore", new string[1]
			{
				"MedicalAid"
			}, "Doctor", new string[0], DUPLICANTS.CHORES.DOCTOR.NAME, DUPLICANTS.CHORES.DOCTOR.STATUS, DUPLICANTS.CHORES.DOCTOR.TOOLTIP, false, 5000);
			Toggle = Add("Toggle", new string[1]
			{
				"Toggle"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.TOGGLE.NAME, DUPLICANTS.CHORES.TOGGLE.STATUS, DUPLICANTS.CHORES.TOGGLE.TOOLTIP, true, 5000);
			Capture = Add("Capture", new string[1]
			{
				"Ranching"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.CAPTURE.NAME, DUPLICANTS.CHORES.CAPTURE.STATUS, DUPLICANTS.CHORES.CAPTURE.TOOLTIP, false, 5000);
			CreatureFetch = Add("CreatureFetch", new string[1]
			{
				"Ranching"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FETCHCREATURE.NAME, DUPLICANTS.CHORES.FETCHCREATURE.STATUS, DUPLICANTS.CHORES.FETCHCREATURE.TOOLTIP, false, 5000);
			EggSing = Add("EggSing", new string[1]
			{
				"Ranching"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.SINGTOEGG.NAME, DUPLICANTS.CHORES.SINGTOEGG.STATUS, DUPLICANTS.CHORES.SINGTOEGG.TOOLTIP, false, 5000);
			Astronaut = Add("Astronaut", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.ASTRONAUT.NAME, DUPLICANTS.CHORES.ASTRONAUT.STATUS, DUPLICANTS.CHORES.ASTRONAUT.TOOLTIP, false, 5000);
			OperateFetch = Add("FetchCritical", new string[2]
			{
				"Hauling",
				"LifeSupport"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false, 5000);
			Art = Add("Art", new string[1]
			{
				"Art"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.ART.NAME, DUPLICANTS.CHORES.ART.STATUS, DUPLICANTS.CHORES.ART.TOOLTIP, false, 5000);
			EmptyStorage = Add("EmptyStorage", new string[2]
			{
				"Basekeeping",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.EMPTYSTORAGE.NAME, DUPLICANTS.CHORES.EMPTYSTORAGE.STATUS, DUPLICANTS.CHORES.EMPTYSTORAGE.TOOLTIP, false, 5000);
			Mop = Add("Mop", new string[1]
			{
				"Basekeeping"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.MOP.NAME, DUPLICANTS.CHORES.MOP.STATUS, DUPLICANTS.CHORES.MOP.TOOLTIP, true, 5000);
			Relocate = Add("Relocate", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RELOCATE.NAME, DUPLICANTS.CHORES.RELOCATE.STATUS, DUPLICANTS.CHORES.RELOCATE.TOOLTIP, true, 5000);
			Disinfect = Add("Disinfect", new string[1]
			{
				"Basekeeping"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.DISINFECT.NAME, DUPLICANTS.CHORES.DISINFECT.STATUS, DUPLICANTS.CHORES.DISINFECT.TOOLTIP, true, 5000);
			Repair = Add("Repair", new string[1]
			{
				"Basekeeping"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.REPAIR.NAME, DUPLICANTS.CHORES.REPAIR.STATUS, DUPLICANTS.CHORES.REPAIR.TOOLTIP, false, 5000);
			Deconstruct = Add("Deconstruct", new string[1]
			{
				"Build"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.DECONSTRUCT.NAME, DUPLICANTS.CHORES.DECONSTRUCT.STATUS, DUPLICANTS.CHORES.DECONSTRUCT.TOOLTIP, false, 5000);
			Research = Add("Research", new string[1]
			{
				"Research"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.RESEARCH.NAME, DUPLICANTS.CHORES.RESEARCH.STATUS, DUPLICANTS.CHORES.RESEARCH.TOOLTIP, false, 5000);
			ResearchFetch = Add("ResearchFetch", new string[2]
			{
				"Research",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false, 5000);
			GeneratePower = Add("GeneratePower", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[1]
			{
				"StressHeal"
			}, DUPLICANTS.CHORES.GENERATEPOWER.NAME, DUPLICANTS.CHORES.GENERATEPOWER.STATUS, DUPLICANTS.CHORES.GENERATEPOWER.TOOLTIP, false, 5000);
			CropTend = Add("CropTend", new string[1]
			{
				"Farming"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.CROP_TEND.NAME, DUPLICANTS.CHORES.CROP_TEND.STATUS, DUPLICANTS.CHORES.CROP_TEND.TOOLTIP, false, 5000);
			PowerTinker = Add("PowerTinker", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.POWER_TINKER.NAME, DUPLICANTS.CHORES.POWER_TINKER.STATUS, DUPLICANTS.CHORES.POWER_TINKER.TOOLTIP, false, 5000);
			MachineTinker = Add("MachineTinker", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.POWER_TINKER.NAME, DUPLICANTS.CHORES.POWER_TINKER.STATUS, DUPLICANTS.CHORES.POWER_TINKER.TOOLTIP, false, 5000);
			MachineFetch = Add("MachineFetch", new string[2]
			{
				"MachineOperating",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false, 5000);
			Harvest = Add("Harvest", new string[1]
			{
				"Farming"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.HARVEST.NAME, DUPLICANTS.CHORES.HARVEST.STATUS, DUPLICANTS.CHORES.HARVEST.TOOLTIP, false, 5000);
			FarmFetch = Add("FarmFetch", new string[2]
			{
				"Farming",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.HARVEST.NAME, DUPLICANTS.CHORES.HARVEST.STATUS, DUPLICANTS.CHORES.HARVEST.TOOLTIP, false, 5000);
			Uproot = Add("Uproot", new string[1]
			{
				"Farming"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.UPROOT.NAME, DUPLICANTS.CHORES.UPROOT.STATUS, DUPLICANTS.CHORES.UPROOT.TOOLTIP, false, 5000);
			CleanToilet = Add("CleanToilet", new string[1]
			{
				"Basekeeping"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.CLEANTOILET.NAME, DUPLICANTS.CHORES.CLEANTOILET.STATUS, DUPLICANTS.CHORES.CLEANTOILET.TOOLTIP, false, 5000);
			LiquidCooledFan = Add("LiquidCooledFan", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.LIQUIDCOOLEDFAN.NAME, DUPLICANTS.CHORES.LIQUIDCOOLEDFAN.STATUS, DUPLICANTS.CHORES.LIQUIDCOOLEDFAN.TOOLTIP, false, 5000);
			Train = Add("Train", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.TRAIN.NAME, DUPLICANTS.CHORES.TRAIN.STATUS, DUPLICANTS.CHORES.TRAIN.TOOLTIP, false, 5000);
			Cook = Add("Cook", new string[1]
			{
				"Cook"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.COOK.NAME, DUPLICANTS.CHORES.COOK.STATUS, DUPLICANTS.CHORES.COOK.TOOLTIP, false, 5000);
			CookFetch = Add("CookFetch", new string[2]
			{
				"Cook",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false, 5000);
			Ranch = Add("Ranch", new string[1]
			{
				"Ranching"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.RANCH.NAME, DUPLICANTS.CHORES.RANCH.STATUS, DUPLICANTS.CHORES.RANCH.TOOLTIP, false, 5000);
			PowerFetch = Add("PowerFetch", new string[2]
			{
				"MachineOperating",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false, 5000);
			FlipCompost = Add("FlipCompost", new string[1]
			{
				"Farming"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FLIPCOMPOST.NAME, DUPLICANTS.CHORES.FLIPCOMPOST.STATUS, DUPLICANTS.CHORES.FLIPCOMPOST.TOOLTIP, false, 5000);
			Depressurize = Add("Depressurize", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.DEPRESSURIZE.NAME, DUPLICANTS.CHORES.DEPRESSURIZE.STATUS, DUPLICANTS.CHORES.DEPRESSURIZE.TOOLTIP, false, 5000);
			FarmingFabricate = Add("FarmingFabricate", new string[1]
			{
				"Farming"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FABRICATE.NAME, DUPLICANTS.CHORES.FABRICATE.STATUS, DUPLICANTS.CHORES.FABRICATE.TOOLTIP, false, 5000);
			PowerFabricate = Add("PowerFabricate", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FABRICATE.NAME, DUPLICANTS.CHORES.FABRICATE.STATUS, DUPLICANTS.CHORES.FABRICATE.TOOLTIP, false, 5000);
			Fabricate = Add("Fabricate", new string[1]
			{
				"MachineOperating"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FABRICATE.NAME, DUPLICANTS.CHORES.FABRICATE.STATUS, DUPLICANTS.CHORES.FABRICATE.TOOLTIP, false, 5000);
			FabricateFetch = Add("FabricateFetch", new string[2]
			{
				"MachineOperating",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false, 5000);
			FoodFetch = Add("FoodFetch", new string[1]
			{
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false, 5000);
			Transport = Add("Transport", new string[2]
			{
				"Hauling",
				"Basekeeping"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.TRANSPORT.NAME, DUPLICANTS.CHORES.TRANSPORT.STATUS, DUPLICANTS.CHORES.TRANSPORT.TOOLTIP, true, 5000);
			Build = Add("Build", new string[1]
			{
				"Build"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.BUILD.NAME, DUPLICANTS.CHORES.BUILD.STATUS, DUPLICANTS.CHORES.BUILD.TOOLTIP, true, 5000);
			BuildDig = Add("BuildDig", new string[2]
			{
				"Build",
				"Dig"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.BUILDDIG.NAME, DUPLICANTS.CHORES.BUILDDIG.STATUS, DUPLICANTS.CHORES.BUILDDIG.TOOLTIP, true, 5000);
			BuildFetch = Add("BuildFetch", new string[2]
			{
				"Build",
				"Hauling"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.BUILDFETCH.NAME, DUPLICANTS.CHORES.BUILDFETCH.STATUS, DUPLICANTS.CHORES.BUILDFETCH.TOOLTIP, true, 5000);
			Dig = Add("Dig", new string[1]
			{
				"Dig"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.DIG.NAME, DUPLICANTS.CHORES.DIG.STATUS, DUPLICANTS.CHORES.DIG.TOOLTIP, false, 5000);
			Fetch = Add("Fetch", new string[1]
			{
				"Storage"
			}, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false, 5000);
			MoveToSafety = Add("MoveToSafety", new string[0], "MoveToSafety", new string[0], DUPLICANTS.CHORES.MOVETOSAFETY.NAME, DUPLICANTS.CHORES.MOVETOSAFETY.STATUS, DUPLICANTS.CHORES.MOVETOSAFETY.TOOLTIP, false, -1);
			ReturnSuitIdle = Add("ReturnSuitIdle", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RETURNSUIT.NAME, DUPLICANTS.CHORES.RETURNSUIT.STATUS, DUPLICANTS.CHORES.RETURNSUIT.TOOLTIP, false, -1);
			Idle = Add("IdleChore", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.IDLE.NAME, DUPLICANTS.CHORES.IDLE.STATUS, DUPLICANTS.CHORES.IDLE.TOOLTIP, false, -1);
			ChoreType[][] array = new ChoreType[27][]
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
					Attack
				},
				new ChoreType[1]
				{
					Flee
				},
				new ChoreType[3]
				{
					SwitchRole,
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
				new ChoreType[56]
				{
					DeliverFood,
					Sigh,
					EmptyStorage,
					Repair,
					Disinfect,
					Shower,
					CleanToilet,
					LiquidCooledFan,
					SuitMarker,
					Checkpoint,
					TravelTubeEntrance,
					WashHands,
					Doctor,
					Recharge,
					OperateFetch,
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
				Debug.LogError(text, null);
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
				Debug.LogError(text2, null);
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

		private ChoreType Add(string id, string[] chore_groups, string urge, string[] interrupt_exclusion, string name, string status_message, string tooltip, bool skip_implicit_priority_change, int explicit_priority = -1)
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
			ChoreType result = new ChoreType(id, this, chore_groups, urge, name, status_message, tooltip, list.ToArray(), nextImplicitPriority, explicit_priority);
			if (!skip_implicit_priority_change)
			{
				nextImplicitPriority -= 100;
			}
			return result;
		}
	}
}
