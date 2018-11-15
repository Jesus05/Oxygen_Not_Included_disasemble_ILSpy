using UnityEngine;

public interface IOreConfig
{
	SimHashes ElementID
	{
		get;
	}

	GameObject CreatePrefab();
}
