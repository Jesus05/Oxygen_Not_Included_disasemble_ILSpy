using System.Collections.Generic;

public static class WorldGenProgressStages
{
	public enum Stages
	{
		Failure,
		SetupNoise,
		GenerateNoise,
		GenerateSolarSystem,
		WorldLayout,
		CompleteLayout,
		NoiseMapBuilder,
		ClearingLevel,
		Processing,
		Borders,
		ProcessRivers,
		ConvertCellsToEdges,
		DrawWorldBorder,
		SettleSim,
		DetectNaturalCavities,
		PlacingCreatures,
		Complete,
		NumberOfStages
	}

	public static KeyValuePair<Stages, float>[] StageWeights = new KeyValuePair<Stages, float>[18]
	{
		new KeyValuePair<Stages, float>(Stages.Failure, 0f),
		new KeyValuePair<Stages, float>(Stages.SetupNoise, 0.01f),
		new KeyValuePair<Stages, float>(Stages.GenerateNoise, 1f),
		new KeyValuePair<Stages, float>(Stages.GenerateSolarSystem, 0.01f),
		new KeyValuePair<Stages, float>(Stages.WorldLayout, 1f),
		new KeyValuePair<Stages, float>(Stages.CompleteLayout, 0.01f),
		new KeyValuePair<Stages, float>(Stages.NoiseMapBuilder, 9f),
		new KeyValuePair<Stages, float>(Stages.ClearingLevel, 0.5f),
		new KeyValuePair<Stages, float>(Stages.Processing, 1f),
		new KeyValuePair<Stages, float>(Stages.Borders, 0.1f),
		new KeyValuePair<Stages, float>(Stages.ProcessRivers, 0.1f),
		new KeyValuePair<Stages, float>(Stages.ConvertCellsToEdges, 0f),
		new KeyValuePair<Stages, float>(Stages.DrawWorldBorder, 0.2f),
		new KeyValuePair<Stages, float>(Stages.SettleSim, 6f),
		new KeyValuePair<Stages, float>(Stages.DetectNaturalCavities, 6f),
		new KeyValuePair<Stages, float>(Stages.PlacingCreatures, 0.01f),
		new KeyValuePair<Stages, float>(Stages.Complete, 0f),
		new KeyValuePair<Stages, float>(Stages.NumberOfStages, 0f)
	};
}
