using Klei;
using UnityEngine;

public static class ElementEmitterExtensions
{
	public static ElementEmitter AddElementEmitter(this GameObject prefab, SimHashes element, float emission_frequency, float emission_mass, SimUtil.DiseaseInfo disease)
	{
		ElementEmitter elementEmitter = prefab.AddOrGet<ElementEmitter>();
		elementEmitter.outputElement = new ElementConverter.OutputElement(emission_mass / emission_frequency, element, 0f, false, 0f, 0.5f, false, 1f, byte.MaxValue, 0);
		elementEmitter.emissionFrequency = emission_frequency;
		return elementEmitter;
	}
}
