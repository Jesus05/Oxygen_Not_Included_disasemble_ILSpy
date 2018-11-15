using System.Collections.Generic;
using UnityEngine;

public class Attack
{
	private AttackProperties properties;

	private GameObject[] targets;

	public List<Hit> Hits;

	public Attack(AttackProperties properties, GameObject[] targets)
	{
		this.properties = properties;
		this.targets = targets;
		RollHits();
	}

	private void RollHits()
	{
		for (int i = 0; i < targets.Length && i <= properties.maxHits - 1; i++)
		{
			if ((Object)targets[i] != (Object)null)
			{
				new Hit(properties, targets[i]);
			}
		}
	}
}
