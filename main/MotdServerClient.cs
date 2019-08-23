using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class MotdServerClient
{
	public class MotdResponse
	{
		public int version
		{
			get;
			set;
		}

		public string image_header_text
		{
			get;
			set;
		}

		public int image_version
		{
			get;
			set;
		}

		public string image_url
		{
			get;
			set;
		}

		public string image_link_url
		{
			get;
			set;
		}

		public string news_header_text
		{
			get;
			set;
		}

		public string news_body_text
		{
			get;
			set;
		}

		public string patch_notes_summary
		{
			get;
			set;
		}

		public string patch_notes_link_url
		{
			get;
			set;
		}

		public string last_update_time
		{
			get;
			set;
		}

		public string next_update_time
		{
			get;
			set;
		}

		public string update_text_override
		{
			get;
			set;
		}

		public Texture2D image_texture
		{
			get;
			set;
		}
	}

	private const string MotdLocalImagePath = "motd_local/image.png";

	private Action<MotdResponse, string> m_callback;

	private static string MotdServerUrl => "https://klei-motd.s3.amazonaws.com/oni/" + GetLocalePathSuffix();

	private static string MotdLocalPath => "motd_local/" + GetLocalePathSuffix();

	private static string GetLocalePathSuffix()
	{
		string str = string.Empty;
		Localization.Locale locale = Localization.GetLocale();
		if (locale != null)
		{
			Localization.Language lang = locale.Lang;
			if (lang == Localization.Language.Korean || lang == Localization.Language.Russian || lang == Localization.Language.Chinese)
			{
				str = locale.Code + "/";
			}
		}
		return str + "motd.json";
	}

	public void GetMotd(Action<MotdResponse, string> cb)
	{
		m_callback = cb;
		MotdResponse localResponse = GetLocalMotd(MotdLocalPath);
		GetWebMotd(MotdServerUrl, localResponse, delegate(MotdResponse response, string err)
		{
			if (err == null)
			{
				doCallback(response, err);
			}
			else
			{
				Debug.LogWarning("Could not retrieve web motd from " + MotdServerUrl + ", falling back to local - err: " + err);
				doCallback(localResponse, null);
			}
		});
	}

	private MotdResponse GetLocalMotd(string filePath)
	{
		TextAsset textAsset = Resources.Load<TextAsset>(filePath.Replace(".json", string.Empty));
		MotdResponse motdResponse = JsonConvert.DeserializeObject<MotdResponse>(textAsset.ToString());
		motdResponse.image_texture = Resources.Load<Texture2D>("motd_local/image.png");
		return motdResponse;
	}

	private void GetWebMotd(string url, MotdResponse localMotd, Action<MotdResponse, string> cb)
	{
		Action<string, string> cb2 = delegate(string response, string err)
		{
			if (err != null)
			{
				cb(null, err);
			}
			else
			{
				MotdResponse responseStruct = JsonConvert.DeserializeObject<MotdResponse>(response, new JsonSerializerSettings
				{
					Error = (EventHandler<ErrorEventArgs>)delegate(object sender, ErrorEventArgs args)
					{
						args.ErrorContext.Handled = true;
					}
				});
				if (responseStruct == null)
				{
					cb(null, "Invalid json from server:" + response);
				}
				else if (responseStruct.version <= localMotd.version)
				{
					Debug.Log("Using local MOTD at version: " + localMotd.version + ", web version at " + responseStruct.version);
					cb(localMotd, null);
				}
				else
				{
					UnityWebRequest data_wr = new UnityWebRequest
					{
						downloadHandler = new DownloadHandlerTexture()
					};
					SimpleNetworkCache.LoadFromCacheOrDownload("motd_image", responseStruct.image_url, responseStruct.image_version, data_wr, delegate(UnityWebRequest wr)
					{
						string arg = null;
						if (string.IsNullOrEmpty(wr.error))
						{
							Debug.Log("Using web MOTD at version: " + responseStruct.version + ", local version at " + localMotd.version);
							responseStruct.image_texture = DownloadHandlerTexture.GetContent(wr);
						}
						else
						{
							arg = "SimpleNetworkCache - " + wr.error;
						}
						cb(responseStruct, arg);
						wr.Dispose();
					});
				}
			}
		};
		getAsyncRequest(url, cb2);
	}

	private void getAsyncRequest(string url, Action<string, string> cb)
	{
		UnityWebRequest motdRequest = UnityWebRequest.Get(url);
		motdRequest.SetRequestHeader("Content-Type", "application/json");
		AsyncOperation asyncOperation = motdRequest.SendWebRequest();
		asyncOperation.completed += delegate
		{
			cb(motdRequest.downloadHandler.text, motdRequest.error);
			motdRequest.Dispose();
		};
	}

	public void UnregisterCallback()
	{
		m_callback = null;
	}

	private void doCallback(MotdResponse response, string error)
	{
		if (m_callback != null)
		{
			m_callback(response, error);
		}
		else
		{
			Debug.Log("Motd Response receieved, but callback was unregistered");
		}
	}
}
