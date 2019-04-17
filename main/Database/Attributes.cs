using Klei.AI;

namespace Database
{
	public class Attributes : ResourceSet<Attribute>
	{
		public Attribute Construction;

		public Attribute Digging;

		public Attribute Machinery;

		public Attribute Athletics;

		public Attribute Learning;

		public Attribute Cooking;

		public Attribute Caring;

		public Attribute Strength;

		public Attribute Art;

		public Attribute Botanist;

		public Attribute Ranching;

		public Attribute LifeSupport;

		public Attribute Toggle;

		public Attribute PowerTinker;

		public Attribute FarmTinker;

		public Attribute SpaceNavigation;

		public Attribute Immunity;

		public Attribute GermSusceptibility;

		public Attribute Insulation;

		public Attribute ThermalConductivityBarrier;

		public Attribute Decor;

		public Attribute FoodQuality;

		public Attribute ScaldingThreshold;

		public Attribute GeneratorOutput;

		public Attribute MachinerySpeed;

		public Attribute DecorExpectation;

		public Attribute FoodExpectation;

		public Attribute RoomTemperaturePreference;

		public Attribute QualityOfLifeExpectation;

		public Attribute AirConsumptionRate;

		public Attribute MaxUnderwaterTravelCost;

		public Attribute ToiletEfficiency;

		public Attribute Sneezyness;

		public Attribute DiseaseCureSpeed;

		public Attribute DoctoredLevel;

		public Attribute CarryAmount;

		public Attribute QualityOfLife;

		public Attributes(ResourceSet parent)
			: base("Attributes", parent)
		{
			Construction = Add(new Attribute("Construction", true, Attribute.Display.Skill, true, 0f, null, null));
			Construction.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Digging = Add(new Attribute("Digging", true, Attribute.Display.Skill, true, 0f, null, null));
			Digging.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Machinery = Add(new Attribute("Machinery", true, Attribute.Display.Skill, true, 0f, null, null));
			Machinery.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Athletics = Add(new Attribute("Athletics", true, Attribute.Display.Skill, true, 0f, null, null));
			Athletics.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Learning = Add(new Attribute("Learning", true, Attribute.Display.Skill, true, 0f, null, null));
			Learning.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Cooking = Add(new Attribute("Cooking", true, Attribute.Display.Skill, true, 0f, null, null));
			Cooking.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Art = Add(new Attribute("Art", true, Attribute.Display.Skill, true, 0f, null, null));
			Art.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Strength = Add(new Attribute("Strength", true, Attribute.Display.Skill, true, 0f, null, null));
			Strength.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Caring = Add(new Attribute("Caring", true, Attribute.Display.Skill, true, 0f, null, null));
			Caring.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Botanist = Add(new Attribute("Botanist", true, Attribute.Display.Skill, true, 0f, null, null));
			Botanist.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Ranching = Add(new Attribute("Ranching", true, Attribute.Display.Skill, true, 0f, null, null));
			Ranching.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			PowerTinker = Add(new Attribute("PowerTinker", true, Attribute.Display.Normal, true, 0f, null, null));
			PowerTinker.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			FarmTinker = Add(new Attribute("FarmTinker", true, Attribute.Display.Normal, true, 0f, null, null));
			FarmTinker.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			SpaceNavigation = Add(new Attribute("SpaceNavigation", true, Attribute.Display.Normal, true, 0f, null, null));
			SpaceNavigation.SetFormatter(new PercentAttributeFormatter());
			Immunity = Add(new Attribute("Immunity", true, Attribute.Display.Details, false, 0f, null, null));
			Immunity.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			ThermalConductivityBarrier = Add(new Attribute("ThermalConductivityBarrier", false, Attribute.Display.Details, false, 0f, null, null));
			ThermalConductivityBarrier.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Distance, GameUtil.TimeSlice.None));
			Insulation = Add(new Attribute("Insulation", false, Attribute.Display.General, true, 0f, null, null));
			Decor = Add(new Attribute("Decor", false, Attribute.Display.General, false, 0f, null, null));
			Decor.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			FoodQuality = Add(new Attribute("FoodQuality", false, Attribute.Display.General, false, 0f, null, null));
			FoodQuality.SetFormatter(new FoodQualityAttributeFormatter());
			ScaldingThreshold = Add(new Attribute("ScaldingThreshold", false, Attribute.Display.General, false, 0f, null, null));
			ScaldingThreshold.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.None));
			GeneratorOutput = Add(new Attribute("GeneratorOutput", false, Attribute.Display.General, false, 0f, null, null));
			GeneratorOutput.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.None));
			MachinerySpeed = Add(new Attribute("MachinerySpeed", false, Attribute.Display.General, false, 1f, null, null));
			MachinerySpeed.SetFormatter(new PercentAttributeFormatter());
			DecorExpectation = Add(new Attribute("DecorExpectation", false, Attribute.Display.Expectation, false, 0f, null, null));
			DecorExpectation.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			FoodExpectation = Add(new Attribute("FoodExpectation", false, Attribute.Display.Expectation, false, 0f, null, null));
			FoodExpectation.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			RoomTemperaturePreference = Add(new Attribute("RoomTemperaturePreference", false, Attribute.Display.Normal, false, 0f, null, null));
			RoomTemperaturePreference.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.None));
			QualityOfLifeExpectation = Add(new Attribute("QualityOfLifeExpectation", false, Attribute.Display.Normal, false, 0f, null, null));
			QualityOfLifeExpectation.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			AirConsumptionRate = Add(new Attribute("AirConsumptionRate", false, Attribute.Display.Normal, false, 0f, null, null));
			AirConsumptionRate.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond));
			MaxUnderwaterTravelCost = Add(new Attribute("MaxUnderwaterTravelCost", false, Attribute.Display.Normal, false, 0f, null, null));
			ToiletEfficiency = Add(new Attribute("ToiletEfficiency", false, Attribute.Display.Details, false, 0f, null, null));
			ToiletEfficiency.SetFormatter(new ToPercentAttributeFormatter(1f, GameUtil.TimeSlice.None));
			Sneezyness = Add(new Attribute("Sneezyness", false, Attribute.Display.Details, false, 0f, null, null));
			DiseaseCureSpeed = Add(new Attribute("DiseaseCureSpeed", false, Attribute.Display.Normal, false, 0f, null, null));
			DiseaseCureSpeed.BaseValue = 1f;
			DiseaseCureSpeed.SetFormatter(new ToPercentAttributeFormatter(1f, GameUtil.TimeSlice.None));
			DoctoredLevel = Add(new Attribute("DoctoredLevel", false, Attribute.Display.Never, false, 0f, null, null));
			CarryAmount = Add(new Attribute("CarryAmount", false, Attribute.Display.Details, false, 0f, null, null));
			CarryAmount.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.None));
			QualityOfLife = Add(new Attribute("QualityOfLife", false, Attribute.Display.Details, false, 0f, "ui_icon_qualityoflife", "attribute_qualityoflife"));
			QualityOfLife.SetFormatter(new QualityOfLifeAttributeFormatter());
			GermSusceptibility = Add(new Attribute("GermSusceptibility", false, Attribute.Display.Details, false, 1f, "ui_icon_immunelevel", "attribute_immunelevel"));
			GermSusceptibility.SetFormatter(new PercentAttributeFormatter());
			LifeSupport = Add(new Attribute("LifeSupport", true, Attribute.Display.Never, false, 0f, null, null));
			LifeSupport.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			Toggle = Add(new Attribute("Toggle", true, Attribute.Display.Never, false, 0f, null, null));
			Toggle.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
		}
	}
}
