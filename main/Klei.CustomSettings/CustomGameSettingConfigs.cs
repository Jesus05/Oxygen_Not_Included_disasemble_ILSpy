using STRINGS;
using System.Collections.Generic;

namespace Klei.CustomSettings
{
	public static class CustomGameSettingConfigs
	{
		public static SettingConfig ImmuneSystem = new ListSettingConfig("ImmuneSystem", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.TOOLTIP, new List<SettingLevel>
		{
			new SettingLevel("Compromised", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.COMPROMISED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.COMPROMISED.TOOLTIP, null),
			new SettingLevel("Weak", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.TOOLTIP, null),
			new SettingLevel("Default", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.DEFAULT.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.DEFAULT.TOOLTIP, null),
			new SettingLevel("Strong", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.TOOLTIP, null),
			new SettingLevel("Invincible", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.INVINCIBLE.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.INVINCIBLE.TOOLTIP, null)
		}, "Default", "Strong", false);

		public static SettingConfig Stress = new ListSettingConfig("Stress", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.TOOLTIP, new List<SettingLevel>
		{
			new SettingLevel("Doomed", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DOOMED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DOOMED.TOOLTIP, null),
			new SettingLevel("Pessimistic", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.TOOLTIP, null),
			new SettingLevel("Default", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DEFAULT.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DEFAULT.TOOLTIP, null),
			new SettingLevel("Optimistic", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.TOOLTIP, null),
			new SettingLevel("Indomitable", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.INDOMITABLE.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.INDOMITABLE.TOOLTIP, null)
		}, "Default", "Optimistic", false);

		public static SettingConfig Morale = new ListSettingConfig("Morale", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.TOOLTIP, new List<SettingLevel>
		{
			new SettingLevel("VeryHard", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.VERYHARD.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.VERYHARD.TOOLTIP, null),
			new SettingLevel("Hard", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.HARD.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.HARD.TOOLTIP, null),
			new SettingLevel("Default", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.DEFAULT.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.DEFAULT.TOOLTIP, null),
			new SettingLevel("Easy", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.EASY.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.EASY.TOOLTIP, null),
			new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.DISABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.DISABLED.TOOLTIP, null)
		}, "Default", "Easy", false);

		public static SettingConfig CalorieBurn = new ListSettingConfig("CalorieBurn", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.TOOLTIP, new List<SettingLevel>
		{
			new SettingLevel("VeryHard", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.VERYHARD.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.VERYHARD.TOOLTIP, null),
			new SettingLevel("Hard", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.HARD.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.HARD.TOOLTIP, null),
			new SettingLevel("Default", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.DEFAULT.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.DEFAULT.TOOLTIP, null),
			new SettingLevel("Easy", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.EASY.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.EASY.TOOLTIP, null),
			new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.DISABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.DISABLED.TOOLTIP, null)
		}, "Default", "Easy", false);

		public static SettingConfig StressBreaks = new ToggleSettingConfig("StressBreaks", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.TOOLTIP, new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.LEVELS.DISABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.LEVELS.DISABLED.TOOLTIP, null), new SettingLevel("Default", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.LEVELS.DEFAULT.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.LEVELS.DEFAULT.TOOLTIP, null), "Default", "Default", false);

		public static SettingConfig WorldgenSeed = new SeedSettingConfig("WorldgenSeed", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLDGEN_SEED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLDGEN_SEED.TOOLTIP, false);

		public static ListSettingConfig World = new ListSettingConfig("World", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLD_CHOICE.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLD_CHOICE.TOOLTIP, null, null, null, true);

		public static SettingConfig SandboxMode = new ToggleSettingConfig("SandboxMode", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.TOOLTIP, new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.LEVELS.DISABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.LEVELS.DISABLED.TOOLTIP, null), new SettingLevel("Enabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.LEVELS.ENABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.LEVELS.ENABLED.TOOLTIP, null), "Disabled", "Disabled", false);
	}
}
