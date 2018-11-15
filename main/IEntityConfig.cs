using UnityEngine;

public interface IEntityConfig
{
	GameObject CreatePrefab();

	void OnPrefabInit(GameObject inst);

	void OnSpawn(GameObject inst);
}
