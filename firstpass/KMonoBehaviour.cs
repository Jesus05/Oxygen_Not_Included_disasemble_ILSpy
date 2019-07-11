using KSerialization;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class KMonoBehaviour : MonoBehaviour, IStateMachineTarget, ISaveLoadable, IUniformGridObject
{
	public static GameObject lastGameObject;

	public static KObject lastObj;

	public static bool isPoolPreInit;

	public static bool isLoadingScene;

	private KObject obj;

	private bool isInitialized = false;

	protected bool autoRegisterSimRender = true;

	protected bool simRenderLoadBalance;

	public bool isSpawned
	{
		get;
		private set;
	}

	public new Transform transform => base.transform;

	public bool isNull => (UnityEngine.Object)this == (UnityEngine.Object)null;

	public void Awake()
	{
		if (!App.IsExiting)
		{
			InitializeComponent();
		}
	}

	public void InitializeComponent()
	{
		if (!isInitialized)
		{
			if (!isPoolPreInit && Application.isPlaying && (UnityEngine.Object)lastGameObject != (UnityEngine.Object)base.gameObject)
			{
				lastGameObject = base.gameObject;
				lastObj = KObjectManager.Instance.GetOrCreateObject(base.gameObject);
			}
			obj = lastObj;
			isInitialized = true;
			MyCmp.OnAwake(this);
			if (!isPoolPreInit)
			{
				try
				{
					OnPrefabInit();
				}
				catch (Exception innerException)
				{
					string message = "Error in " + base.name + "." + GetType().Name + ".OnPrefabInit";
					throw new Exception(message, innerException);
				}
			}
		}
	}

	private void OnEnable()
	{
		if (!App.IsExiting)
		{
			OnCmpEnable();
		}
	}

	private void OnDisable()
	{
		if (!App.IsExiting && !isLoadingScene)
		{
			OnCmpDisable();
		}
	}

	public bool IsInitialized()
	{
		return isInitialized;
	}

	public void OnDestroy()
	{
		OnForcedCleanUp();
		if (!App.IsExiting)
		{
			if (isLoadingScene)
			{
				OnLoadLevel();
			}
			else
			{
				if ((UnityEngine.Object)KObjectManager.Instance != (UnityEngine.Object)null)
				{
					KObjectManager.Instance.QueueDestroy(obj);
				}
				OnCleanUp();
				SimAndRenderScheduler.instance.Remove(this);
			}
		}
	}

	public void Start()
	{
		if (!App.IsExiting)
		{
			Spawn();
		}
	}

	public void Spawn()
	{
		if (!isSpawned)
		{
			if (!isInitialized)
			{
				Debug.LogError(base.name + "." + GetType().Name + " is not initialized.");
			}
			else
			{
				isSpawned = true;
				if (autoRegisterSimRender)
				{
					SimAndRenderScheduler.instance.Add(this, simRenderLoadBalance);
				}
				MyCmp.OnStart(this);
				try
				{
					OnSpawn();
				}
				catch (Exception innerException)
				{
					string message = "Error in " + base.name + "." + GetType().Name + ".OnSpawn";
					throw new Exception(message, innerException);
				}
			}
		}
	}

	protected virtual void OnPrefabInit()
	{
	}

	protected virtual void OnSpawn()
	{
	}

	protected virtual void OnCmpEnable()
	{
	}

	protected virtual void OnCmpDisable()
	{
	}

	protected virtual void OnCleanUp()
	{
	}

	protected virtual void OnForcedCleanUp()
	{
	}

	protected virtual void OnLoadLevel()
	{
	}

	public virtual void CreateDef()
	{
	}

	public T FindOrAdd<T>() where T : KMonoBehaviour
	{
		return this.FindOrAddComponent<T>();
	}

	public void FindOrAdd<T>(ref T c) where T : KMonoBehaviour
	{
		c = FindOrAdd<T>();
	}

	public T Require<T>() where T : Component
	{
		return this.RequireComponent<T>();
	}

	public int Subscribe(int hash, Action<object> handler)
	{
		return obj.GetEventSystem().Subscribe(hash, handler);
	}

	public int Subscribe(GameObject target, int hash, Action<object> handler)
	{
		return obj.GetEventSystem().Subscribe(target, hash, handler);
	}

	public int Subscribe<ComponentType>(int hash, EventSystem.IntraObjectHandler<ComponentType> handler)
	{
		return obj.GetEventSystem().Subscribe(hash, handler);
	}

	public void Unsubscribe(int hash, Action<object> handler)
	{
		if (obj != null)
		{
			obj.GetEventSystem().Unsubscribe(hash, handler);
		}
	}

	public void Unsubscribe(int id)
	{
		obj.GetEventSystem().Unsubscribe(id);
	}

	public void Unsubscribe(GameObject target, int hash, Action<object> handler)
	{
		obj.GetEventSystem().Unsubscribe(target, hash, handler);
	}

	public void Unsubscribe<ComponentType>(int hash, EventSystem.IntraObjectHandler<ComponentType> handler, bool suppressWarnings = false)
	{
		if (obj != null)
		{
			obj.GetEventSystem().Unsubscribe(hash, handler, suppressWarnings);
		}
	}

	public void Trigger(int hash, object data = null)
	{
		if (obj != null && obj.hasEventSystem)
		{
			obj.GetEventSystem().Trigger(base.gameObject, hash, data);
		}
	}

	public static void PlaySound(string sound)
	{
		if (sound != null)
		{
			try
			{
				if ((UnityEngine.Object)SoundListenerController.Instance == (UnityEngine.Object)null)
				{
					KFMOD.PlayOneShot(sound);
				}
				else
				{
					KFMOD.PlayOneShot(sound, SoundListenerController.Instance.transform.GetPosition());
				}
			}
			catch
			{
				DebugUtil.LogWarningArgs("AUDIOERROR: Missing [" + sound + "]");
			}
		}
	}

	public static void PlaySound3DAtLocation(string sound, Vector3 location)
	{
		if ((UnityEngine.Object)SoundListenerController.Instance != (UnityEngine.Object)null)
		{
			try
			{
				KFMOD.PlayOneShot(sound, location);
			}
			catch
			{
				DebugUtil.LogWarningArgs("AUDIOERROR: Missing [" + sound + "]");
			}
		}
	}

	public void PlaySound3D(string asset)
	{
		try
		{
			KFMOD.PlayOneShot(asset, transform.GetPosition());
		}
		catch
		{
			DebugUtil.LogWarningArgs("AUDIOERROR: Missing [" + asset + "]");
		}
	}

	public virtual Vector2 PosMin()
	{
		return transform.GetPosition();
	}

	public virtual Vector2 PosMax()
	{
		return transform.GetPosition();
	}

	ComponentType IStateMachineTarget.GetComponent<ComponentType>()
	{
		return GetComponent<ComponentType>();
	}

	GameObject get_gameObject()
	{
		return base.gameObject;
	}

	GameObject IStateMachineTarget.get_gameObject()
	{
		//ILSpy generated this explicit interface implementation from .override directive in get_gameObject
		return this.get_gameObject();
	}

	string get_name()
	{
		return base.name;
	}

	string IStateMachineTarget.get_name()
	{
		//ILSpy generated this explicit interface implementation from .override directive in get_name
		return this.get_name();
	}
}
