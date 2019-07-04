using ObjectCloner;
using System.Collections.Generic;

namespace ProcGen
{
	public class MutatedWorldData
	{
		public World world;

		public List<WorldTrait> traits;

		public Dictionary<string, SubWorld> subworlds;

		public Dictionary<string, FeatureSettings> features;

		public TerrainElementBandSettings biomes;

		public MobSettings mobs;

		public MutatedWorldData(World world, List<WorldTrait> traits)
		{
			this.world = SerializingCloner.Copy(world);
			if (traits == null)
			{
				this.traits = new List<WorldTrait>();
			}
			else
			{
				this.traits = new List<WorldTrait>(traits);
			}
			SettingsCache.CloneInToNewWorld(this);
			ApplyTraits();
			foreach (ElementBandConfiguration value in biomes.BiomeBackgroundElementBandConfigurations.Values)
			{
				value.ConvertBandSizeToMaxSize();
			}
		}

		private void ApplyTraits()
		{
			foreach (WorldTrait trait in traits)
			{
				ApplyTrait(trait);
			}
		}

		private void ApplyTrait(WorldTrait trait)
		{
			world.ModStartLocation(trait.startingBasePositionHorizontalMod, trait.startingBasePositionVerticalMod);
			foreach (WeightedName additionalSubworldFile in trait.additionalSubworldFiles)
			{
				world.subworldFiles.Add(additionalSubworldFile);
			}
			foreach (World.AllowedCellsFilter additionalUnknownCellFilter in trait.additionalUnknownCellFilters)
			{
				world.unknownCellsAllowedSubworlds.Add(additionalUnknownCellFilter);
			}
			foreach (KeyValuePair<string, int> globalFeatureTemplateMod in trait.globalFeatureTemplateMods)
			{
				if (!world.globalFeatureTemplates.ContainsKey(globalFeatureTemplateMod.Key))
				{
					world.globalFeatureTemplates[globalFeatureTemplateMod.Key] = 0;
				}
				Dictionary<string, int> globalFeatureTemplates;
				string key;
				(globalFeatureTemplates = world.globalFeatureTemplates)[key = globalFeatureTemplateMod.Key] = globalFeatureTemplates[key] + globalFeatureTemplateMod.Value;
			}
			foreach (KeyValuePair<string, int> globalFeatureMod in trait.globalFeatureMods)
			{
				if (!world.globalFeatures.ContainsKey(globalFeatureMod.Key))
				{
					world.globalFeatures[globalFeatureMod.Key] = 0;
				}
				Dictionary<string, int> globalFeatureTemplates;
				string key2;
				(globalFeatureTemplates = world.globalFeatures)[key2 = globalFeatureMod.Key] = globalFeatureTemplates[key2] + globalFeatureMod.Value;
			}
			foreach (KeyValuePair<string, ElementBandConfiguration> biomeBackgroundElementBandConfiguration in biomes.BiomeBackgroundElementBandConfigurations)
			{
				foreach (ElementGradient item in biomeBackgroundElementBandConfiguration.Value)
				{
					foreach (WorldTrait.ElementBandModifier elementBandModifier in trait.elementBandModifiers)
					{
						if (elementBandModifier.element == item.content)
						{
							item.Mod(elementBandModifier);
						}
					}
				}
			}
		}
	}
}
