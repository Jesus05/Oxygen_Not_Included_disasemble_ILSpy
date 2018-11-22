using System;
using UnityEngine;

public class OrbitalMechanics : KMonoBehaviour, IRenderEveryTick
{
	[Serializable]
	private struct OrbitData
	{
		public string prefabTag;

		public float periodInCycles;

		public float yGridPercent;

		public float angle;

		public float radiusScale;

		public bool rotatesBehind;

		public float behindZ;

		public Vector3 scale;

		public float distance;
	}

	[SerializeField]
	private OrbitData[] orbitData;

	[SerializeField]
	private bool applyOverrides = false;

	[SerializeField]
	[Range(0f, 100f)]
	private float overridePercent = 0f;

	[SerializeField]
	private GameObject[] orbitingObjects;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		orbitingObjects = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Rebuild();
	}

	public void RenderEveryTick(float dt)
	{
		if (orbitData != null && orbitingObjects != null)
		{
			float time = GameClock.Instance.GetTime();
			for (int i = 0; i < orbitingObjects.Length; i++)
			{
				OrbitData data = orbitData[i];
				bool behind;
				Vector3 vector = CalculatePos(ref data, time, out behind);
				vector.y -= 0.5f;
				Vector3 position = vector;
				Vector3 vector2 = Camera.main.ViewportToWorldPoint(vector);
				position.x = vector2.x;
				Vector3 vector3 = Camera.main.ViewportToWorldPoint(vector);
				position.y = vector3.y;
				bool flag = !data.rotatesBehind || !behind;
				GameObject gameObject = orbitingObjects[i];
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
				{
					gameObject.transform.SetPosition(position);
					gameObject.transform.localScale = Vector3.one * Camera.main.orthographicSize / data.distance;
					if (gameObject.activeSelf != flag)
					{
						gameObject.SetActive(flag);
					}
				}
			}
		}
	}

	[ContextMenu("Rebuild")]
	private void Rebuild()
	{
		if (orbitingObjects != null)
		{
			GameObject[] array = orbitingObjects;
			foreach (GameObject original in array)
			{
				Util.KDestroyGameObject(original);
			}
			orbitingObjects = null;
		}
		if (orbitData != null && orbitData.Length > 0)
		{
			float time = GameClock.Instance.GetTime();
			orbitingObjects = new GameObject[orbitData.Length];
			for (int j = 0; j < orbitData.Length; j++)
			{
				OrbitData data = orbitData[j];
				GameObject prefab = Assets.GetPrefab(data.prefabTag);
				bool behind;
				Vector3 position = CalculatePos(ref data, time, out behind);
				GameObject gameObject = Util.KInstantiate(prefab, position);
				gameObject.SetActive(true);
				orbitingObjects[j] = gameObject;
			}
		}
	}

	private Vector3 CalculatePos(ref OrbitData data, float time, out bool behind)
	{
		float num = data.periodInCycles * 600f;
		float num2 = (!applyOverrides) ? (time / num - (float)(int)(time / num)) : (overridePercent / 100f);
		float f = num2 * 2f * 3.14159274f;
		float d = 0.5f * data.radiusScale;
		float yGridPercent = data.yGridPercent;
		Vector3 a = new Vector3(0.5f, yGridPercent, 0f);
		Vector3 a2 = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
		behind = (a2.z > data.behindZ);
		Quaternion rotation = Quaternion.Euler(data.angle, 0f, 0f);
		Vector3 b = rotation * (a2 * d);
		Vector3 result = a + b;
		result.z = 100f;
		return result;
	}
}
