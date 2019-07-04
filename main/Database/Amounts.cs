using Klei.AI;

namespace Database
{
	public class Amounts : ResourceSet<Amount>
	{
		public Amount Stamina;

		public Amount Calories;

		public Amount ImmuneLevel;

		public Amount ExternalTemperature;

		public Amount Breath;

		public Amount Stress;

		public Amount Toxicity;

		public Amount Bladder;

		public Amount Decor;

		public Amount Temperature;

		public Amount HitPoints;

		public Amount AirPressure;

		public Amount Maturity;

		public Amount OldAge;

		public Amount Age;

		public Amount Fertilization;

		public Amount Illumination;

		public Amount Irrigation;

		public Amount CreatureCalories;

		public Amount Fertility;

		public Amount Viability;

		public Amount Wildness;

		public Amount Incubation;

		public Amount ScaleGrowth;

		public Amount Rot;

		public void Load()
		{
			Stamina = CreateAmount("Stamina", 0f, 100f, false, Units.Flat, 0.35f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_stamina", "attribute_stamina");
			Stamina.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle, null));
			Calories = CreateAmount("Calories", 0f, 0f, false, Units.Flat, 4000f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_calories", "attribute_calories");
			Calories.SetDisplayer(new CaloriesDisplayer());
			Temperature = CreateAmount("Temperature", 0f, 10000f, false, Units.Kelvin, 0.5f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_temperature", null);
			Temperature.SetDisplayer(new DuplicantTemperatureDeltaAsEnergyAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond));
			ExternalTemperature = CreateAmount("ExternalTemperature", 0f, 10000f, false, Units.Kelvin, 0.5f, true, "STRINGS.DUPLICANTS.STATS", null, null);
			ExternalTemperature.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond, null));
			Breath = CreateAmount("Breath", 0f, 100f, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_breath", null);
			Breath.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond, null));
			Stress = CreateAmount("Stress", 0f, 100f, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_stress", "attribute_stress");
			Stress.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			Toxicity = CreateAmount("Toxicity", 0f, 100f, true, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS.STATS", null, null);
			Toxicity.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle, null));
			Bladder = CreateAmount("Bladder", 0f, 100f, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_bladder", null);
			Bladder.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle, null));
			Decor = CreateAmount("Decor", -1000f, 1000f, false, Units.Flat, 0.0166666675f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_decor", null);
			Decor.SetDisplayer(new DecorDisplayer());
			Maturity = CreateAmount("Maturity", 0f, 0f, true, Units.Flat, 0.0009166667f, true, "STRINGS.CREATURES.STATS", "ui_icon_maturity", null);
			Maturity.SetDisplayer(new MaturityDisplayer());
			OldAge = CreateAmount("OldAge", 0f, 0f, false, Units.Flat, 0f, false, "STRINGS.CREATURES.STATS", null, null);
			Fertilization = CreateAmount("Fertilization", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS", null, null);
			Fertilization.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond, null));
			Fertility = CreateAmount("Fertility", 0f, 100f, true, Units.Flat, 0.008375f, true, "STRINGS.CREATURES.STATS", "ui_icon_fertility", null);
			Fertility.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			Wildness = CreateAmount("Wildness", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS", "ui_icon_wildness", null);
			Wildness.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			Incubation = CreateAmount("Incubation", 0f, 100f, true, Units.Flat, 0.01675f, true, "STRINGS.CREATURES.STATS", "ui_icon_incubation", null);
			Incubation.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			Viability = CreateAmount("Viability", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS", null, null);
			Viability.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			Age = CreateAmount("Age", 0f, 0f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS", "ui_icon_age", null);
			Age.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.PerCycle, null));
			Irrigation = CreateAmount("Irrigation", 0f, 1f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS", null, null);
			Irrigation.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond, null));
			HitPoints = CreateAmount("HitPoints", 0f, 0f, true, Units.Flat, 0.1675f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_hitpoints", "attribute_hitpoints");
			HitPoints.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.PerCycle, null));
			ImmuneLevel = CreateAmount("ImmuneLevel", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_immunelevel", "attribute_immunelevel");
			ImmuneLevel.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			Rot = CreateAmount("Rot", 0f, 0f, false, Units.Flat, 0f, true, "STRINGS.CREATURES.STATS", null, null);
			Rot.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			AirPressure = CreateAmount("AirPressure", 0f, 1E+09f, false, Units.Flat, 0f, true, "STRINGS.CREATURES.STATS", null, null);
			AirPressure.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond, null));
			Illumination = CreateAmount("Illumination", 0f, 1f, false, Units.Flat, 0f, true, "STRINGS.CREATURES.STATS", null, null);
			Illumination.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.None, null));
			ScaleGrowth = CreateAmount("ScaleGrowth", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS", "ui_icon_scale_growth", null);
			ScaleGrowth.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
		}

		public Amount CreateAmount(string id, float min, float max, bool show_max, Units units, float delta_threshold, bool show_in_ui, string string_root, string uiSprite = null, string thoughtSprite = null)
		{
			string text = Strings.Get(string.Format("{1}.{0}.NAME", id.ToUpper(), string_root.ToUpper()));
			string description = Strings.Get(string.Format("{1}.{0}.TOOLTIP", id.ToUpper(), string_root.ToUpper()));
			Attribute attribute = new Attribute(id + "Min", "Minimum" + text, "", "", min, Attribute.Display.Normal, false, null, null);
			Attribute attribute2 = new Attribute(id + "Max", "Maximum" + text, "", "", max, Attribute.Display.Normal, false, null, null);
			string text2 = id + "Delta";
			string name = Strings.Get($"STRINGS.DUPLICANTS.ATTRIBUTES.{text2.ToUpper()}.NAME");
			string attribute_description = Strings.Get($"STRINGS.DUPLICANTS.ATTRIBUTES.{text2.ToUpper()}.DESC");
			Attribute attribute3 = new Attribute(text2, name, "", attribute_description, 0f, Attribute.Display.Normal, false, null, null);
			Amount amount = new Amount(id, text, description, attribute, attribute2, attribute3, show_max, units, delta_threshold, show_in_ui, uiSprite, thoughtSprite);
			Db.Get().Attributes.Add(attribute);
			Db.Get().Attributes.Add(attribute2);
			Db.Get().Attributes.Add(attribute3);
			Add(amount);
			return amount;
		}
	}
}
