using System;

[Serializable]
public class AttackEffect
{
	public string effectID;

	public float effectProbability;

	public AttackEffect(string ID, float probability)
	{
		effectID = ID;
		effectProbability = probability;
	}
}
