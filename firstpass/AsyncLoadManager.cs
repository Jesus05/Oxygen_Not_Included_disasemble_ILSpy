using System;
using System.Collections.Generic;
using System.Reflection;

public static class AsyncLoadManager<AsyncLoaderType>
{
	public abstract class AsyncLoader<LoaderType> : AsyncLoader where LoaderType : class
	{
		public static LoaderType Get()
		{
			return AsyncLoadManager<AsyncLoaderType>.GetLoader(typeof(LoaderType)) as LoaderType;
		}
	}

	private struct RunLoader : IWorkItem<object>
	{
		public AsyncLoader loader;

		public void Run(object shared_data)
		{
			loader.Run();
		}
	}

	private static Dictionary<Type, AsyncLoader> loaders = new Dictionary<Type, AsyncLoader>();

	public static void Run()
	{
		List<AsyncLoader> list = new List<AsyncLoader>();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (!type.IsAbstract && typeof(AsyncLoaderType).IsAssignableFrom(type))
				{
					AsyncLoader asyncLoader = (AsyncLoader)Activator.CreateInstance(type);
					list.Add(asyncLoader);
					loaders[type] = asyncLoader;
					asyncLoader.CollectLoaders(list);
				}
			}
		}
		if (loaders.Count > 0)
		{
			WorkItemCollection<RunLoader, object> workItemCollection = new WorkItemCollection<RunLoader, object>();
			workItemCollection.Reset(null);
			foreach (AsyncLoader item in list)
			{
				workItemCollection.Add(new RunLoader
				{
					loader = item
				});
			}
			GlobalJobManager.Run(workItemCollection);
		}
	}

	public static AsyncLoader GetLoader(Type type)
	{
		return loaders[type];
	}
}
