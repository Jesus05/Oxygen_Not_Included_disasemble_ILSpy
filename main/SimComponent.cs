using System;
using System.Diagnostics;
using UnityEngine;

public abstract class SimComponent : KMonoBehaviour, ISim200ms
{
	[SerializeField]
	protected int simHandle = -1;

	private bool simActive = true;

	private bool dirty = true;

	protected virtual void OnSimRegister(HandleVector<Game.ComplexCallbackInfo<int>>.Handle cb_handle)
	{
	}

	protected virtual void OnSimRegistered()
	{
	}

	protected virtual void OnSimActivate()
	{
	}

	protected virtual void OnSimDeactivate()
	{
	}

	protected virtual void OnSimUnregister()
	{
	}

	protected abstract Action<int> GetStaticUnregister();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SimRegister();
	}

	protected override void OnCleanUp()
	{
		SimUnregister();
		base.OnCleanUp();
	}

	public void SetSimActive(bool active)
	{
		simActive = active;
		dirty = true;
	}

	public void Sim200ms(float dt)
	{
		if (Sim.IsValidHandle(simHandle))
		{
			UpdateSimState();
		}
	}

	private void UpdateSimState()
	{
		if (dirty)
		{
			dirty = false;
			if (simActive)
			{
				OnSimActivate();
			}
			else
			{
				OnSimDeactivate();
			}
		}
	}

	private void SimRegister()
	{
		if (base.isSpawned && simHandle == -1)
		{
			simHandle = -2;
			Action<int> static_unregister = GetStaticUnregister();
			HandleVector<Game.ComplexCallbackInfo<int>>.Handle cb_handle = Game.Instance.simComponentCallbackManager.Add(delegate(int handle, object data)
			{
				OnSimRegistered(this, handle, static_unregister);
			}, this, "SimComponent.SimRegister");
			OnSimRegister(cb_handle);
		}
	}

	private void SimUnregister()
	{
		if (Sim.IsValidHandle(simHandle))
		{
			OnSimUnregister();
		}
		simHandle = -1;
	}

	private static void OnSimRegistered(SimComponent instance, int handle, Action<int> static_unregister)
	{
		if ((UnityEngine.Object)instance != (UnityEngine.Object)null)
		{
			instance.simHandle = handle;
			instance.OnSimRegistered();
		}
		else
		{
			static_unregister(handle);
		}
	}

	[Conditional("ENABLE_LOGGER")]
	protected void Log(string msg)
	{
	}
}
