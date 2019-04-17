using Steamworks;
using STRINGS;
using UnityEngine;

namespace KMod
{
	public class Steam : IDistributionPlatform, SteamUGCService.IUGCEventHandler
	{
		public Steam()
		{
			UpdateSubscriptions();
		}

		private Mod MakeMod(SteamUGCService.Subscribed subscribed)
		{
			if (subscribed == null)
			{
				return null;
			}
			if ((SteamUGC.GetItemState(subscribed.fileId) & 4) == 0)
			{
				return null;
			}
			PublishedFileId_t fileId = subscribed.fileId;
			string id = fileId.m_PublishedFileId.ToString();
			Label label = default(Label);
			label.id = id;
			label.distribution_platform = Label.DistributionPlatform.Steam;
			label.version = subscribed.lastUpdateTime;
			label.title = subscribed.title;
			Label label2 = label;
			if (!SteamUGC.GetItemInstallInfo(subscribed.fileId, out ulong _, out string pchFolder, 1024u, out uint _))
			{
				Global.Instance.modManager.events.Add(new Event
				{
					event_type = EventType.InstallInfoInaccessible,
					mod = label2
				});
				return null;
			}
			return new Mod(label2, subscribed.description, new ZipFile(pchFolder), UI.FRONTEND.MODS.TOOLTIPS.MANAGE_STEAM_SUBSCRIPTION, delegate
			{
				Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + id);
			});
		}

		private void UpdateSubscriptions()
		{
			foreach (SteamUGCService.Subscribed item in SteamUGCService.Instance.GetSubscribed())
			{
				Mod mod = MakeMod(item);
				if (mod != null)
				{
					Global.Instance.modManager.Subscribe(mod, this);
				}
			}
		}

		public void OnUGCItemSubscribed(RemoteStoragePublishedFileSubscribed_t pCallback)
		{
			Mod mod = MakeMod(SteamUGCService.Instance.GetSubscribed(pCallback.m_nPublishedFileId));
			if (mod != null)
			{
				Global.Instance.modManager.Subscribe(mod, this);
				Global.Instance.modManager.NotifyDialog(UI.FRONTEND.MOD_DIALOGS.STEAM_SUBSCRIBED.TITLE, UI.FRONTEND.MOD_DIALOGS.STEAM_SUBSCRIBED.MESSAGE, null);
			}
		}

		public void OnUGCItemInstalled(ItemInstalled_t pCallback)
		{
		}

		public void OnUGCItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
		{
			Mod mod = MakeMod(SteamUGCService.Instance.GetSubscribed(pCallback.m_nPublishedFileId));
			if (mod != null)
			{
				Global.Instance.modManager.Subscribe(mod, this);
				Global.Instance.modManager.NotifyDialog(UI.FRONTEND.MOD_DIALOGS.STEAM_UPDATED.TITLE, UI.FRONTEND.MOD_DIALOGS.STEAM_UPDATED.MESSAGE, null);
			}
		}

		public void OnUGCItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
		{
			Mod mod = MakeMod(SteamUGCService.Instance.GetSubscribed(pCallback.m_nPublishedFileId));
			if (mod != null)
			{
				Global.Instance.modManager.Unsubscribe(mod.label, this);
				Global.Instance.modManager.NotifyDialog(UI.FRONTEND.MOD_DIALOGS.STEAM_UNSUBSCRIBED.TITLE, UI.FRONTEND.MOD_DIALOGS.STEAM_UNSUBSCRIBED.MESSAGE, null);
			}
		}

		public void OnUGCItemDownloaded(DownloadItemResult_t pCallback)
		{
		}

		public void OnUGCRefresh()
		{
			UpdateSubscriptions();
			Global.Instance.modManager.NotifyDialog(UI.FRONTEND.MOD_DIALOGS.STEAM_REFRESH.TITLE, UI.FRONTEND.MOD_DIALOGS.STEAM_REFRESH.MESSAGE, null);
		}
	}
}
