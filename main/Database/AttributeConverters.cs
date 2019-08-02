using Klei.AI;
using STRINGS;
using System.Collections.Generic;

namespace Database
{
	public class AttributeConverters : ResourceSet<AttributeConverter>
	{
		public AttributeConverter MovementSpeed;

		public AttributeConverter ConstructionSpeed;

		public AttributeConverter DiggingSpeed;

		public AttributeConverter MachinerySpeed;

		public AttributeConverter HarvestSpeed;

		public AttributeConverter PlantTendSpeed;

		public AttributeConverter CompoundingSpeed;

		public AttributeConverter ResearchSpeed;

		public AttributeConverter TrainingSpeed;

		public AttributeConverter CookingSpeed;

		public AttributeConverter ArtSpeed;

		public AttributeConverter DoctorSpeed;

		public AttributeConverter TidyingSpeed;

		public AttributeConverter AttackDamage;

		public AttributeConverter ImmuneLevelBoost;

		public AttributeConverter ToiletSpeed;

		public AttributeConverter CarryAmountFromStrength;

		public AttributeConverter TemperatureInsulation;

		public AttributeConverter SeedHarvestChance;

		public AttributeConverter RanchingEffectDuration;

		public AttributeConverters()
		{
			ToPercentAttributeFormatter formatter = new ToPercentAttributeFormatter(1f, GameUtil.TimeSlice.None);
			StandardAttributeFormatter formatter2 = new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.None);
			MovementSpeed = Create("MovementSpeed", "Movement Speed", DUPLICANTS.ATTRIBUTES.ATHLETICS.SPEEDMODIFIER, Db.Get().Attributes.Athletics, 0.1f, 0f, formatter);
			ConstructionSpeed = Create("ConstructionSpeed", "Construction Speed", DUPLICANTS.ATTRIBUTES.CONSTRUCTION.SPEEDMODIFIER, Db.Get().Attributes.Construction, 0.25f, 0f, formatter);
			DiggingSpeed = Create("DiggingSpeed", "Digging Speed", DUPLICANTS.ATTRIBUTES.DIGGING.SPEEDMODIFIER, Db.Get().Attributes.Digging, 0.25f, 0f, formatter);
			MachinerySpeed = Create("MachinerySpeed", "Machinery Speed", DUPLICANTS.ATTRIBUTES.MACHINERY.SPEEDMODIFIER, Db.Get().Attributes.Machinery, 0.1f, 0f, formatter);
			HarvestSpeed = Create("HarvestSpeed", "Harvest Speed", DUPLICANTS.ATTRIBUTES.BOTANIST.HARVEST_SPEED_MODIFIER, Db.Get().Attributes.Botanist, 0.05f, 0f, formatter);
			PlantTendSpeed = Create("PlantTendSpeed", "Plant Tend Speed", DUPLICANTS.ATTRIBUTES.BOTANIST.TINKER_MODIFIER, Db.Get().Attributes.Botanist, 0.025f, 0f, formatter);
			CompoundingSpeed = Create("CompoundingSpeed", "Compounding Speed", DUPLICANTS.ATTRIBUTES.CARING.FABRICATE_SPEEDMODIFIER, Db.Get().Attributes.Caring, 0.1f, 0f, formatter);
			ResearchSpeed = Create("ResearchSpeed", "Research Speed", DUPLICANTS.ATTRIBUTES.LEARNING.RESEARCHSPEED, Db.Get().Attributes.Learning, 0.4f, 0f, formatter);
			TrainingSpeed = Create("TrainingSpeed", "Training Speed", DUPLICANTS.ATTRIBUTES.LEARNING.SPEEDMODIFIER, Db.Get().Attributes.Learning, 0.1f, 0f, formatter);
			CookingSpeed = Create("CookingSpeed", "Cooking Speed", DUPLICANTS.ATTRIBUTES.COOKING.SPEEDMODIFIER, Db.Get().Attributes.Cooking, 0.05f, 0f, formatter);
			ArtSpeed = Create("ArtSpeed", "Art Speed", DUPLICANTS.ATTRIBUTES.ART.SPEEDMODIFIER, Db.Get().Attributes.Art, 0.1f, 0f, formatter);
			DoctorSpeed = Create("DoctorSpeed", "Doctor Speed", DUPLICANTS.ATTRIBUTES.CARING.SPEEDMODIFIER, Db.Get().Attributes.Caring, 0.2f, 0f, formatter);
			TidyingSpeed = Create("TidyingSpeed", "Tidying Speed", DUPLICANTS.ATTRIBUTES.STRENGTH.SPEEDMODIFIER, Db.Get().Attributes.Strength, 0.25f, 0f, formatter);
			AttackDamage = Create("AttackDamage", "Attack Damage", DUPLICANTS.ATTRIBUTES.DIGGING.ATTACK_MODIFIER, Db.Get().Attributes.Digging, 0.05f, 0f, formatter);
			ImmuneLevelBoost = Create("ImmuneLevelBoost", "Immune Level Boost", DUPLICANTS.ATTRIBUTES.IMMUNITY.BOOST_MODIFIER, Db.Get().Attributes.Immunity, 0.00166666671f, 0f, new ToPercentAttributeFormatter(100f, GameUtil.TimeSlice.PerCycle));
			ToiletSpeed = Create("ToiletSpeed", "Toilet Speed", string.Empty, Db.Get().Attributes.ToiletEfficiency, 1f, -1f, formatter);
			CarryAmountFromStrength = Create("CarryAmountFromStrength", "Carry Amount", DUPLICANTS.ATTRIBUTES.STRENGTH.CARRYMODIFIER, Db.Get().Attributes.Strength, 40f, 0f, formatter2);
			TemperatureInsulation = Create("TemperatureInsulation", "Temperature Insulation", DUPLICANTS.ATTRIBUTES.INSULATION.SPEEDMODIFIER, Db.Get().Attributes.Insulation, 0.1f, 0f, formatter);
			SeedHarvestChance = Create("SeedHarvestChance", "Seed Harvest Chance", DUPLICANTS.ATTRIBUTES.BOTANIST.BONUS_SEEDS, Db.Get().Attributes.Botanist, 0.033f, 0f, formatter);
			RanchingEffectDuration = Create("RanchingEffectDuration", "Ranching Effect Duration", DUPLICANTS.ATTRIBUTES.RANCHING.EFFECTMODIFIER, Db.Get().Attributes.Ranching, 0.1f, 0f, formatter);
		}

		public AttributeConverter Create(string id, string name, string description, Attribute attribute, float multiplier, float base_value, IAttributeFormatter formatter)
		{
			AttributeConverter attributeConverter = new AttributeConverter(id, name, description, multiplier, base_value, attribute, formatter);
			Add(attributeConverter);
			attribute.converters.Add(attributeConverter);
			return attributeConverter;
		}

		public List<AttributeConverter> GetConvertersForAttribute(Attribute attrib)
		{
			List<AttributeConverter> list = new List<AttributeConverter>();
			foreach (AttributeConverter resource in resources)
			{
				if (resource.attribute == attrib)
				{
					list.Add(resource);
				}
			}
			return list;
		}
	}
}
