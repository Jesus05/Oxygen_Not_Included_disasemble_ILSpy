using System.Collections.Generic;
using UnityEngine;

public class AlgaeConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.Algae;

	public GameObject CreatePrefab()
	{
		return EntityTemplates.CreateSolidOreEntity(ElementID, new List<Tag>
		{
			GameTags.Life
		});
	}
}
