using UnityEngine;

public interface IPolluter
{
	int GetRadius();

	int GetNoise();

	GameObject GetGameObject();

	void SetAttributes(Vector2 pos, int dB, GameObject go, string name = null);

	string GetName();

	Vector2 GetPosition();

	void Clear();

	void SetSplat(NoiseSplat splat);
}
