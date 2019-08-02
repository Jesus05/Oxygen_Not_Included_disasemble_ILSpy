using System;
using System.Collections.Generic;
using System.Linq;

public class SimAndRenderScheduler
{
	public struct Handle
	{
		public HandleVector<int>.Handle handle;

		public StateMachineUpdater.BaseUpdateBucket bucket;

		public bool IsValid()
		{
			return bucket != null;
		}

		public void Release()
		{
			if (bucket != null)
			{
				bucket.Remove(handle);
				bucket = null;
			}
		}
	}

	private struct Entry
	{
		public StateMachineUpdater.BaseUpdateBucket[] buckets;

		public int nextBucketIdx;
	}

	public class BaseUpdaterManager
	{
		public UpdateRate updateRate
		{
			get;
			private set;
		}

		protected BaseUpdaterManager(UpdateRate update_rate)
		{
			updateRate = update_rate;
		}
	}

	public class UpdaterManager<UpdaterType> : BaseUpdaterManager
	{
		private Dictionary<UpdaterType, Handle> updaterHandles = new Dictionary<UpdaterType, Handle>();

		private Dictionary<Type, string> bucketIds = new Dictionary<Type, string>();

		public UpdaterManager(UpdateRate update_rate)
			: base(update_rate)
		{
		}

		public void Add(UpdaterType updater, bool load_balance = false)
		{
			if (!Contains(updater))
			{
				string value = string.Empty;
				if (!bucketIds.TryGetValue(updater.GetType(), out value))
				{
					value = MakeBucketId(updater.GetType(), base.updateRate);
					bucketIds[updater.GetType()] = value;
				}
				Handle value2 = instance.Schedule<UpdaterType>(value, (UpdateBucketWithUpdater<UpdaterType>.IUpdater)this, base.updateRate, updater, load_balance);
				updaterHandles[updater] = value2;
			}
		}

		public void Remove(UpdaterType updater)
		{
			if (updaterHandles.TryGetValue(updater, out Handle value))
			{
				value.Release();
				updaterHandles.Remove(updater);
			}
		}

		public bool Contains(UpdaterType updater)
		{
			return updaterHandles.ContainsKey(updater);
		}
	}

	public class RenderEveryTickUpdater : UpdaterManager<IRenderEveryTick>, UpdateBucketWithUpdater<IRenderEveryTick>.IUpdater
	{
		public RenderEveryTickUpdater()
			: base(UpdateRate.RENDER_EVERY_TICK)
		{
		}

		public void Update(IRenderEveryTick updater, float dt)
		{
			updater.RenderEveryTick(dt);
		}
	}

	public class Render200ms : UpdaterManager<IRender200ms>, UpdateBucketWithUpdater<IRender200ms>.IUpdater
	{
		public Render200ms()
			: base(UpdateRate.RENDER_200ms)
		{
		}

		public void Update(IRender200ms updater, float dt)
		{
			updater.Render200ms(dt);
		}
	}

	public class Render1000msUpdater : UpdaterManager<IRender1000ms>, UpdateBucketWithUpdater<IRender1000ms>.IUpdater
	{
		public Render1000msUpdater()
			: base(UpdateRate.RENDER_1000ms)
		{
		}

		public void Update(IRender1000ms updater, float dt)
		{
			updater.Render1000ms(dt);
		}
	}

	public class Sim33msUpdater : UpdaterManager<ISim33ms>, UpdateBucketWithUpdater<ISim33ms>.IUpdater
	{
		public Sim33msUpdater()
			: base(UpdateRate.SIM_33ms)
		{
		}

		public void Update(ISim33ms updater, float dt)
		{
			updater.Sim33ms(dt);
		}
	}

	public class Sim200msUpdater : UpdaterManager<ISim200ms>, UpdateBucketWithUpdater<ISim200ms>.IUpdater
	{
		public Sim200msUpdater()
			: base(UpdateRate.SIM_200ms)
		{
		}

		public void Update(ISim200ms updater, float dt)
		{
			updater.Sim200ms(dt);
		}
	}

	public class Sim1000msUpdater : UpdaterManager<ISim1000ms>, UpdateBucketWithUpdater<ISim1000ms>.IUpdater
	{
		public Sim1000msUpdater()
			: base(UpdateRate.SIM_1000ms)
		{
		}

		public void Update(ISim1000ms updater, float dt)
		{
			updater.Sim1000ms(dt);
		}
	}

	public class Sim4000msUpdater : UpdaterManager<ISim4000ms>, UpdateBucketWithUpdater<ISim4000ms>.IUpdater
	{
		public Sim4000msUpdater()
			: base(UpdateRate.SIM_4000ms)
		{
		}

		public void Update(ISim4000ms updater, float dt)
		{
			updater.Sim4000ms(dt);
		}
	}

	private static SimAndRenderScheduler _instance;

	private Dictionary<string, Entry> bucketTable = new Dictionary<string, Entry>();

	public RenderEveryTickUpdater renderEveryTick = new RenderEveryTickUpdater();

	public Render200ms render200ms = new Render200ms();

	public Render1000msUpdater render1000ms = new Render1000msUpdater();

	public Sim33msUpdater sim33ms = new Sim33msUpdater();

	public Sim200msUpdater sim200ms = new Sim200msUpdater();

	public Sim1000msUpdater sim1000ms = new Sim1000msUpdater();

	public Sim4000msUpdater sim4000ms = new Sim4000msUpdater();

	private Dictionary<Type, UpdateRate[]> typeImplementedInterfaces = new Dictionary<Type, UpdateRate[]>();

	private Dictionary<Type, UpdateRate> availableInterfaces = new Dictionary<Type, UpdateRate>();

	public static SimAndRenderScheduler instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new SimAndRenderScheduler();
			}
			return _instance;
		}
	}

	private SimAndRenderScheduler()
	{
		availableInterfaces[typeof(IRenderEveryTick)] = UpdateRate.RENDER_EVERY_TICK;
		availableInterfaces[typeof(IRender200ms)] = UpdateRate.RENDER_200ms;
		availableInterfaces[typeof(IRender1000ms)] = UpdateRate.RENDER_1000ms;
		availableInterfaces[typeof(ISim33ms)] = UpdateRate.SIM_33ms;
		availableInterfaces[typeof(ISim200ms)] = UpdateRate.SIM_200ms;
		availableInterfaces[typeof(ISim1000ms)] = UpdateRate.SIM_1000ms;
		availableInterfaces[typeof(ISim4000ms)] = UpdateRate.SIM_4000ms;
	}

	public static void DestroyInstance()
	{
		_instance = null;
	}

	private static string MakeBucketId(Type updater_type, UpdateRate update_rate)
	{
		return $"{updater_type.Name} {update_rate.ToString()}";
	}

	private UpdateRate[] GetImplementedInterfaces(Type type)
	{
		UpdateRate[] value = null;
		if (!typeImplementedInterfaces.TryGetValue(type, out value))
		{
			ListPool<UpdateRate, SimAndRenderScheduler>.PooledList pooledList = ListPool<UpdateRate, SimAndRenderScheduler>.Allocate();
			foreach (KeyValuePair<Type, UpdateRate> availableInterface in availableInterfaces)
			{
				if (availableInterface.Key.IsAssignableFrom(type))
				{
					pooledList.Add(availableInterface.Value);
				}
			}
			value = pooledList.ToArray();
			pooledList.Recycle();
			typeImplementedInterfaces[type] = value;
		}
		return value;
	}

	public static Type GetUpdateInterface(UpdateRate update_rate)
	{
		switch (update_rate)
		{
		case UpdateRate.RENDER_EVERY_TICK:
			return typeof(IRenderEveryTick);
		case UpdateRate.RENDER_200ms:
			return typeof(IRender200ms);
		case UpdateRate.RENDER_1000ms:
			return typeof(IRender1000ms);
		case UpdateRate.SIM_33ms:
			return typeof(ISim33ms);
		case UpdateRate.SIM_200ms:
			return typeof(ISim200ms);
		case UpdateRate.SIM_1000ms:
			return typeof(ISim1000ms);
		case UpdateRate.SIM_4000ms:
			return typeof(ISim4000ms);
		default:
			return null;
		}
	}

	public UpdateRate GetUpdateRate(Type updater)
	{
		if (!availableInterfaces.TryGetValue(updater, out UpdateRate value))
		{
			Debug.Assert(false, "only call this with an update interface type");
		}
		return value;
	}

	public UpdateRate GetUpdateRate<T>()
	{
		return GetUpdateRate(typeof(T));
	}

	public void Add(object obj, bool load_balance = false)
	{
		UpdateRate[] implementedInterfaces = GetImplementedInterfaces(obj.GetType());
		UpdateRate[] array = implementedInterfaces;
		for (int i = 0; i < array.Length; i++)
		{
			switch (array[i])
			{
			case UpdateRate.RENDER_EVERY_TICK:
				renderEveryTick.Add((IRenderEveryTick)obj, load_balance);
				break;
			case UpdateRate.RENDER_200ms:
				render200ms.Add((IRender200ms)obj, load_balance);
				break;
			case UpdateRate.RENDER_1000ms:
				render1000ms.Add((IRender1000ms)obj, load_balance);
				break;
			case UpdateRate.SIM_33ms:
				sim33ms.Add((ISim33ms)obj, load_balance);
				break;
			case UpdateRate.SIM_200ms:
				sim200ms.Add((ISim200ms)obj, load_balance);
				break;
			case UpdateRate.SIM_1000ms:
				sim1000ms.Add((ISim1000ms)obj, load_balance);
				break;
			case UpdateRate.SIM_4000ms:
				sim4000ms.Add((ISim4000ms)obj, load_balance);
				break;
			}
		}
	}

	public void Remove(object obj)
	{
		UpdateRate[] implementedInterfaces = GetImplementedInterfaces(obj.GetType());
		UpdateRate[] array = implementedInterfaces;
		for (int i = 0; i < array.Length; i++)
		{
			switch (array[i])
			{
			case UpdateRate.RENDER_EVERY_TICK:
				renderEveryTick.Remove((IRenderEveryTick)obj);
				break;
			case UpdateRate.RENDER_200ms:
				render200ms.Remove((IRender200ms)obj);
				break;
			case UpdateRate.RENDER_1000ms:
				render1000ms.Remove((IRender1000ms)obj);
				break;
			case UpdateRate.SIM_33ms:
				sim33ms.Remove((ISim33ms)obj);
				break;
			case UpdateRate.SIM_200ms:
				sim200ms.Remove((ISim200ms)obj);
				break;
			case UpdateRate.SIM_1000ms:
				sim1000ms.Remove((ISim1000ms)obj);
				break;
			case UpdateRate.SIM_4000ms:
				sim4000ms.Remove((ISim4000ms)obj);
				break;
			}
		}
	}

	private Entry ManifestEntry<UpdateInterface>(string name, bool load_balance)
	{
		if (bucketTable.TryGetValue(name, out Entry value))
		{
			DebugUtil.DevAssertArgs(value.buckets.Length == ((!load_balance) ? 1 : Singleton<StateMachineUpdater>.Instance.GetFrameCount(GetUpdateRate<UpdateInterface>())), "load_balance doesn't match previous registration...maybe load_balance erroneously on for a BatchUpdate type ", name, "?");
			return value;
		}
		value = default(Entry);
		UpdateRate updateRate = GetUpdateRate<UpdateInterface>();
		int num = (!load_balance) ? 1 : Singleton<StateMachineUpdater>.Instance.GetFrameCount(updateRate);
		value.buckets = new StateMachineUpdater.BaseUpdateBucket[num];
		for (int i = 0; i < num; i++)
		{
			value.buckets[i] = new UpdateBucketWithUpdater<UpdateInterface>(name);
			Singleton<StateMachineUpdater>.Instance.AddBucket(updateRate, value.buckets[i]);
		}
		return value;
	}

	public Handle Schedule<SimUpdateType>(string name, UpdateBucketWithUpdater<SimUpdateType>.IUpdater bucket_updater, UpdateRate update_rate, SimUpdateType updater, bool load_balance = false)
	{
		Entry value = ManifestEntry<SimUpdateType>(name, load_balance);
		UpdateBucketWithUpdater<SimUpdateType> updateBucketWithUpdater = (UpdateBucketWithUpdater<SimUpdateType>)value.buckets[value.nextBucketIdx];
		Handle result = default(Handle);
		result.handle = updateBucketWithUpdater.Add(updater, Singleton<StateMachineUpdater>.Instance.GetFrameTime(update_rate, updateBucketWithUpdater.frame), bucket_updater);
		result.bucket = updateBucketWithUpdater;
		value.nextBucketIdx = (value.nextBucketIdx + 1) % value.buckets.Length;
		bucketTable[name] = value;
		return result;
	}

	public void Reset()
	{
		_instance = null;
	}

	public void RegisterBatchUpdate<UpdateInterface, T>(UpdateBucketWithUpdater<UpdateInterface>.BatchUpdateDelegate batch_update)
	{
		string text = MakeBucketId(typeof(T), GetUpdateRate<UpdateInterface>());
		Entry value = ManifestEntry<UpdateInterface>(text, false);
		DebugUtil.DevAssert(GetImplementedInterfaces(typeof(T)).Contains(GetUpdateRate<UpdateInterface>()), "T does not implement the UpdateInterface it is registering for BatchUpdate under");
		DebugUtil.DevAssert(value.buckets.Length == 1, "don't do a batch update with load balancing because load balancing will produce many small batches which is inefficient");
		((UpdateBucketWithUpdater<UpdateInterface>)value.buckets[0]).batch_update_delegate = batch_update;
		bucketTable[text] = value;
	}
}
