using UnityEngine;

public static class SingletonResource<T> where T : ResourceFile
{
	private static T StaticInstance;

	public static T Get()
	{
		if ((Object)StaticInstance == (Object)null)
		{
			StaticInstance = Resources.Load<T>(typeof(T).Name);
			StaticInstance.Initialize();
		}
		return StaticInstance;
	}
}
