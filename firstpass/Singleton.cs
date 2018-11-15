public abstract class Singleton<T> where T : class, new()
{
	private static T _instance;

	private static object _lock = new object();

	public static T Instance
	{
		get
		{
			lock (_lock)
			{
				return _instance;
			}
		}
	}

	public static void CreateInstance()
	{
		lock (_lock)
		{
			if (_instance == null)
			{
				_instance = new T();
			}
		}
	}

	public static void DestroyInstance()
	{
		lock (_lock)
		{
			_instance = (T)null;
		}
	}
}
