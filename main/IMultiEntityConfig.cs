using System.Collections.Generic;
using UnityEngine;

public interface IMultiEntityConfig
{
	List<GameObject> CreatePrefabs();

	void OnPrefabInit(GameObject inst);

	void OnSpawn(GameObject inst);
}
