using KSerialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class UnstableGroundManager : KMonoBehaviour
{
	[Serializable]
	private struct EffectInfo
	{
		[HashedEnum]
		public SimHashes element;

		public GameObject prefab;
	}

	private struct EffectRuntimeInfo
	{
		public ObjectPool pool;

		public Action<GameObject> releaseFunc;
	}

	private struct SerializedInfo
	{
		public Vector3 position;

		public SimHashes element;

		public float mass;

		public float temperature;

		public int diseaseID;

		public int diseaseCount;
	}

	[SerializeField]
	private Vector3 spawnPuffOffset;

	[SerializeField]
	private Vector3 landEffectOffset;

	[SerializeField]
	private EffectInfo[] effects;

	private List<GameObject> fallingObjects = new List<GameObject>();

	private List<int> pendingCells = new List<int>();

	private Dictionary<SimHashes, EffectRuntimeInfo> runtimeInfo = new Dictionary<SimHashes, EffectRuntimeInfo>();

	[Serialize]
	private List<SerializedInfo> serializedInfo;

	protected override void OnPrefabInit()
	{
		EffectInfo[] array = effects;
		for (int i = 0; i < array.Length; i++)
		{
			EffectInfo effectInfo = array[i];
			GameObject prefab = effectInfo.prefab;
			prefab.SetActive(false);
			EffectRuntimeInfo value = default(EffectRuntimeInfo);
			ObjectPool pool = new ObjectPool(() => InstantiateObj(prefab), 16);
			value.pool = pool;
			value.releaseFunc = delegate(GameObject go)
			{
				ReleaseGO(go);
				pool.ReleaseInstance(go);
			};
			runtimeInfo[effectInfo.element] = value;
		}
	}

	private void ReleaseGO(GameObject go)
	{
		if (GameComps.Gravities.Has(go))
		{
			GameComps.Gravities.Remove(go);
		}
		go.SetActive(false);
	}

	private GameObject InstantiateObj(GameObject prefab)
	{
		GameObject gameObject = GameUtil.KInstantiate(prefab, Grid.SceneLayer.BuildingBack, null, 0);
		gameObject.SetActive(false);
		gameObject.name = "UnstablePool";
		return gameObject;
	}

	public void Spawn(int cell, Element element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		Vector3 pos = Grid.CellToPosCCC(cell, Grid.SceneLayer.TileMain);
		if (float.IsNaN(temperature) || float.IsInfinity(temperature))
		{
			Debug.LogError("Tried to spawn unstable ground with NaN temperature", null);
			temperature = 293f;
		}
		KBatchedAnimController kBatchedAnimController = Spawn(pos, element, mass, temperature, disease_idx, disease_count);
		kBatchedAnimController.Play("start", KAnim.PlayMode.Once, 1f, 0f);
		kBatchedAnimController.Play("loop", KAnim.PlayMode.Loop, 1f, 0f);
		kBatchedAnimController.gameObject.name = "Falling " + element.name;
		GameComps.Gravities.Add(kBatchedAnimController.gameObject, Vector2.zero, null);
		fallingObjects.Add(kBatchedAnimController.gameObject);
		SpawnPuff(pos, element, mass, temperature, disease_idx, disease_count);
		Substance substance = element.substance;
		if (substance != null && substance.fallingStartSound != null && CameraController.Instance.IsAudibleSound(pos, substance.fallingStartSound))
		{
			SoundEvent.PlayOneShot(substance.fallingStartSound, pos);
		}
	}

	private void SpawnOld(Vector3 pos, Element element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		if (!element.IsUnstable)
		{
			Output.LogError("Spawning falling ground with a stable element");
		}
		KBatchedAnimController kBatchedAnimController = Spawn(pos, element, mass, temperature, disease_idx, disease_count);
		GameComps.Gravities.Add(kBatchedAnimController.gameObject, Vector2.zero, null);
		kBatchedAnimController.Play("loop", KAnim.PlayMode.Loop, 1f, 0f);
		fallingObjects.Add(kBatchedAnimController.gameObject);
		kBatchedAnimController.gameObject.name = "SpawnOld " + element.name;
	}

	private void SpawnPuff(Vector3 pos, Element element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		if (!element.IsUnstable)
		{
			Output.LogError("Spawning sand puff with a stable element");
		}
		KBatchedAnimController kBatchedAnimController = Spawn(pos, element, mass, temperature, disease_idx, disease_count);
		kBatchedAnimController.Play("sandPuff", KAnim.PlayMode.Once, 1f, 0f);
		kBatchedAnimController.gameObject.name = "Puff " + element.name;
		kBatchedAnimController.transform.SetPosition(kBatchedAnimController.transform.GetPosition() + spawnPuffOffset);
	}

	private KBatchedAnimController Spawn(Vector3 pos, Element element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		if (!runtimeInfo.TryGetValue(element.id, out EffectRuntimeInfo value))
		{
			Debug.LogError(element.id.ToString() + " needs unstable ground info hookup!", null);
		}
		GameObject instance = value.pool.GetInstance();
		instance.transform.SetPosition(pos);
		if (float.IsNaN(temperature) || float.IsInfinity(temperature))
		{
			Debug.LogError("Tried to spawn unstable ground with NaN temperature", null);
			temperature = 293f;
		}
		PrimaryElement component = instance.GetComponent<PrimaryElement>();
		component.ElementID = element.id;
		component.Mass = mass;
		component.Temperature = temperature;
		instance.SetActive(true);
		component.AddDisease(disease_idx, disease_count, "UnstableGroundManager.Spawn");
		KBatchedAnimController component2 = instance.GetComponent<KBatchedAnimController>();
		component2.onDestroySelf = value.releaseFunc;
		component2.Stop();
		if (element.substance != null)
		{
			component2.TintColour = element.substance.colour;
		}
		return component2;
	}

	public List<int> GetCellsContainingFallingAbove(Vector2I cellXY)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < fallingObjects.Count; i++)
		{
			GameObject gameObject = fallingObjects[i];
			Grid.PosToXY(gameObject.transform.GetPosition(), out Vector2I xy);
			if (xy.x == cellXY.x || xy.y >= cellXY.y)
			{
				int item = Grid.PosToCell(xy);
				list.Add(item);
			}
		}
		for (int j = 0; j < pendingCells.Count; j++)
		{
			Vector2I vector2I = Grid.CellToXY(pendingCells[j]);
			if (vector2I.x == cellXY.x || vector2I.y >= cellXY.y)
			{
				list.Add(pendingCells[j]);
			}
		}
		return list;
	}

	private void RemoveFromPending(int cell)
	{
		pendingCells.Remove(cell);
	}

	private void Update()
	{
		if (!App.isLoading)
		{
			int num = 0;
			while (num < fallingObjects.Count)
			{
				GameObject gameObject = fallingObjects[num];
				if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
				{
					Vector3 position = gameObject.transform.GetPosition();
					int cell = Grid.PosToCell(position);
					int num2 = Grid.CellBelow(cell);
					if (!Grid.IsValidCell(num2) || Grid.Element[num2].IsSolid || (Grid.Properties[num2] & 4) != 0)
					{
						PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
						pendingCells.Add(cell);
						HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(delegate
						{
							RemoveFromPending(cell);
						}, false));
						SimMessages.AddRemoveSubstance(cell, component.ElementID, CellEventLogger.Instance.UnstableGround, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, true, handle.index);
						ListPool<ScenePartitionerEntry, UnstableGroundManager>.PooledList pooledList = ListPool<ScenePartitionerEntry, UnstableGroundManager>.Allocate();
						Vector2I vector2I = Grid.CellToXY(cell);
						vector2I.x = Mathf.Max(0, vector2I.x - 1);
						vector2I.y = Mathf.Min(Grid.HeightInCells - 1, vector2I.y + 1);
						GameScenePartitioner.Instance.GatherEntries(vector2I.x, vector2I.y, 3, 3, GameScenePartitioner.Instance.collisionLayer, pooledList);
						foreach (ScenePartitionerEntry item in pooledList)
						{
							if (item.obj is KCollider2D)
							{
								GameObject gameObject2 = (item.obj as KCollider2D).gameObject;
								gameObject2.Trigger(-975551167, null);
							}
						}
						pooledList.Recycle();
						if (component.Element.substance != null && component.Element.substance.fallingStopSound != null && CameraController.Instance.IsAudibleSound(position, component.Element.substance.fallingStopSound))
						{
							SoundEvent.PlayOneShot(component.Element.substance.fallingStopSound, position);
						}
						GameObject gameObject3 = GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.OreAbsorbId), position + landEffectOffset, Grid.SceneLayer.Front, null, 0);
						gameObject3.SetActive(true);
						fallingObjects[num] = fallingObjects[fallingObjects.Count - 1];
						fallingObjects.RemoveAt(fallingObjects.Count - 1);
						ReleaseGO(gameObject);
					}
					else
					{
						num++;
					}
				}
			}
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		if (fallingObjects.Count > 0)
		{
			serializedInfo = new List<SerializedInfo>();
		}
		foreach (GameObject fallingObject in fallingObjects)
		{
			PrimaryElement component = fallingObject.GetComponent<PrimaryElement>();
			byte diseaseIdx = component.DiseaseIdx;
			int diseaseID = (diseaseIdx != 255) ? Db.Get().Diseases[diseaseIdx].id.HashValue : 0;
			serializedInfo.Add(new SerializedInfo
			{
				position = fallingObject.transform.GetPosition(),
				element = component.ElementID,
				mass = component.Mass,
				temperature = component.Temperature,
				diseaseID = diseaseID,
				diseaseCount = component.DiseaseCount
			});
		}
	}

	[OnSerialized]
	private void OnSerialized()
	{
		serializedInfo = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (serializedInfo != null)
		{
			fallingObjects.Clear();
			HashedString id = default(HashedString);
			foreach (SerializedInfo item in serializedInfo)
			{
				SerializedInfo current = item;
				Element element = ElementLoader.FindElementByHash(current.element);
				id.HashValue = current.diseaseID;
				byte index = Db.Get().Diseases.GetIndex(id);
				int disease_count = current.diseaseCount;
				if (index == 255)
				{
					disease_count = 0;
				}
				SpawnOld(current.position, element, current.mass, current.temperature, index, disease_count);
			}
		}
	}
}
