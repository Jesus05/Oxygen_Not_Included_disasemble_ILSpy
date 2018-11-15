using System.Collections.Generic;
using UnityEngine;

public class EntombedItemVisualizer : KMonoBehaviour
{
	private struct Data
	{
		public int refCount;

		public KBatchedAnimController controller;
	}

	[SerializeField]
	private GameObject entombedItemPrefab;

	private static readonly string[] EntombedVisualizerAnims = new string[4]
	{
		"idle1",
		"idle2",
		"idle3",
		"idle4"
	};

	private ObjectPool entombedItemPool;

	private Dictionary<int, Data> cellEntombedCounts = new Dictionary<int, Data>();

	public void Clear()
	{
		cellEntombedCounts.Clear();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		entombedItemPool = new ObjectPool(InstantiateEntombedObject, 32);
	}

	public bool AddItem(int cell)
	{
		bool result = false;
		if ((Object)Grid.Objects[cell, 9] == (Object)null)
		{
			result = true;
			cellEntombedCounts.TryGetValue(cell, out Data value);
			if (value.refCount == 0)
			{
				GameObject instance = entombedItemPool.GetInstance();
				instance.transform.SetPosition(Grid.CellToPosCCC(cell, Grid.SceneLayer.FXFront));
				instance.transform.rotation = Quaternion.Euler(0f, 0f, Random.value * 360f);
				KBatchedAnimController component = instance.GetComponent<KBatchedAnimController>();
				int num = Random.Range(0, EntombedVisualizerAnims.Length);
				string s = component.initialAnim = EntombedVisualizerAnims[num];
				instance.SetActive(true);
				component.Play(s, KAnim.PlayMode.Once, 1f, 0f);
				value.controller = component;
			}
			value.refCount++;
			cellEntombedCounts[cell] = value;
		}
		return result;
	}

	public void RemoveItem(int cell)
	{
		if (cellEntombedCounts.TryGetValue(cell, out Data value))
		{
			value.refCount--;
			if (value.refCount == 0)
			{
				ReleaseVisualizer(cell, value);
			}
			else
			{
				cellEntombedCounts[cell] = value;
			}
		}
	}

	public void ForceClear(int cell)
	{
		if (cellEntombedCounts.TryGetValue(cell, out Data value))
		{
			ReleaseVisualizer(cell, value);
		}
	}

	private void ReleaseVisualizer(int cell, Data data)
	{
		if ((Object)data.controller != (Object)null)
		{
			data.controller.gameObject.SetActive(false);
			entombedItemPool.ReleaseInstance(data.controller.gameObject);
		}
		cellEntombedCounts.Remove(cell);
	}

	public bool IsEntombedItem(int cell)
	{
		int result;
		if (cellEntombedCounts.ContainsKey(cell))
		{
			Data data = cellEntombedCounts[cell];
			result = ((data.refCount > 0) ? 1 : 0);
		}
		else
		{
			result = 0;
		}
		return (byte)result != 0;
	}

	private GameObject InstantiateEntombedObject()
	{
		GameObject gameObject = GameUtil.KInstantiate(entombedItemPrefab, Grid.SceneLayer.FXFront, null, 0);
		gameObject.SetActive(false);
		return gameObject;
	}
}
