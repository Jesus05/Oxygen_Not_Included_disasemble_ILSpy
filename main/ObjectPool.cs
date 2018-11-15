using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
	private List<GameObject> unused;

	private Func<GameObject> instantiator;

	public ObjectPool(Func<GameObject> instantiator, int initial_count = 0)
	{
		this.instantiator = instantiator;
		unused = new List<GameObject>();
		for (int i = 0; i < initial_count; i++)
		{
			unused.Add(instantiator());
		}
	}

	public GameObject GetInstance()
	{
		GameObject gameObject = null;
		if (unused.Count > 0)
		{
			gameObject = unused[unused.Count - 1];
			unused.RemoveAt(unused.Count - 1);
		}
		else
		{
			gameObject = instantiator();
		}
		return gameObject;
	}

	public void ReleaseInstance(GameObject go)
	{
		unused.Add(go);
	}

	public void Destroy()
	{
		for (int i = 0; i < unused.Count; i++)
		{
			UnityEngine.Object.Destroy(unused[i]);
		}
		unused.Clear();
	}
}
