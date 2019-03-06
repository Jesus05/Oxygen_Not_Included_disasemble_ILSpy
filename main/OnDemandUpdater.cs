using System.Collections.Generic;
using UnityEngine;

public class OnDemandUpdater : MonoBehaviour
{
	private List<IUpdateOnDemand> Updaters = new List<IUpdateOnDemand>();

	public static OnDemandUpdater Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	private void Awake()
	{
		Instance = this;
	}

	public void Register(IUpdateOnDemand updater)
	{
		if (!Updaters.Contains(updater))
		{
			Updaters.Add(updater);
		}
	}

	public void Unregister(IUpdateOnDemand updater)
	{
		if (Updaters.Contains(updater))
		{
			Updaters.Remove(updater);
		}
	}

	private void Update()
	{
		for (int i = 0; i < Updaters.Count; i++)
		{
			if (Updaters[i] != null)
			{
				Updaters[i].UpdateOnDemand();
			}
		}
	}
}
