using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class SimpleNetworkCache
{
	public static void LoadFromCacheOrDownload(string cache_id, string url, int version, UnityWebRequest data_wr, Action<UnityWebRequest> callback)
	{
		string cache_folder = Util.CacheFolder();
		string cache_prefix = Path.Combine(cache_folder, cache_id);
		string version_filepath = cache_prefix + "_version";
		string data_filepath = cache_prefix + "_data";
		UnityWebRequest version_wr = new UnityWebRequest(new Uri(version_filepath, UriKind.Absolute), "GET", new DownloadHandlerBuffer(), null);
		AsyncOperation asyncOperation = version_wr.SendWebRequest();
		asyncOperation.completed += delegate
		{
			int versionFromWebRequest = GetVersionFromWebRequest(version_wr);
			if (versionFromWebRequest == version)
			{
				data_wr.uri = new Uri(data_filepath, UriKind.Absolute);
				AsyncOperation asyncOperation2 = data_wr.SendWebRequest();
				asyncOperation2.completed += delegate
				{
					if (!string.IsNullOrEmpty(data_wr.error))
					{
						Debug.LogWarning("Failure to read cached file: " + data_filepath);
						try
						{
							File.Delete(version_filepath);
							File.Delete(data_filepath);
						}
						catch
						{
							Debug.LogWarning("Failed to delete cached files");
						}
					}
					callback(data_wr);
				};
			}
			else
			{
				data_wr.url = url;
				AsyncOperation asyncOperation3 = data_wr.SendWebRequest();
				asyncOperation3.completed += delegate
				{
					if (string.IsNullOrEmpty(data_wr.error))
					{
						try
						{
							Directory.CreateDirectory(cache_folder);
							File.WriteAllBytes(data_filepath, data_wr.downloadHandler.data);
							File.WriteAllText(version_filepath, version.ToString());
						}
						catch
						{
							Debug.LogWarning("Failed to write cache files to: " + cache_prefix);
						}
					}
					callback(data_wr);
				};
			}
			version_wr.Dispose();
		};
	}

	private static int GetVersionFromWebRequest(UnityWebRequest version_wr)
	{
		if (!string.IsNullOrEmpty(version_wr.error))
		{
			return -1;
		}
		if (version_wr.downloadHandler != null && int.TryParse(version_wr.downloadHandler.text, out int result))
		{
			return result;
		}
		return -1;
	}
}
