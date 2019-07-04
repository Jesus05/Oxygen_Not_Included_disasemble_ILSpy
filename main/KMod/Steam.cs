using Steamworks;
using STRINGS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KMod
{
	public class Steam : IDistributionPlatform, SteamUGCService.IClient
	{
		private Mod MakeMod(SteamUGCService.Mod subscribed)
		{
			if (subscribed != null)
			{
				if ((SteamUGC.GetItemState(subscribed.fileId) & 4) != 0)
				{
					PublishedFileId_t fileId = subscribed.fileId;
					string id = fileId.m_PublishedFileId.ToString();
					Label label = default(Label);
					label.id = id;
					label.distribution_platform = Label.DistributionPlatform.Steam;
					label.version = (long)subscribed.lastUpdateTime;
					label.title = subscribed.title;
					Label label2 = label;
					if (SteamUGC.GetItemInstallInfo(subscribed.fileId, out ulong _, out string pchFolder, 1024u, out uint _))
					{
						return new Mod(label2, subscribed.description, new ZipFile(pchFolder), UI.FRONTEND.MODS.TOOLTIPS.MANAGE_STEAM_SUBSCRIPTION, delegate
						{
							Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + id);
						});
					}
					Global.Instance.modManager.events.Add(new Event
					{
						event_type = EventType.InstallInfoInaccessible,
						mod = label2
					});
					return null;
				}
				return null;
			}
			return null;
		}

		public void UpdateMods(IEnumerable<PublishedFileId_t> added, IEnumerable<PublishedFileId_t> updated, IEnumerable<PublishedFileId_t> removed, IEnumerable<SteamUGCService.Mod> loaded_previews)
		{
			foreach (PublishedFileId_t item in added)
			{
				SteamUGCService.Mod mod = SteamUGCService.Instance.FindMod(item);
				if (mod == null)
				{
					DebugUtil.DevAssert(false, "SteamUGCService just told us this id was valid!");
				}
				else
				{
					Mod mod2 = MakeMod(mod);
					if (mod2 != null)
					{
						Global.Instance.modManager.Subscribe(mod2, this);
					}
				}
			}
			foreach (PublishedFileId_t item2 in updated)
			{
				SteamUGCService.Mod mod3 = SteamUGCService.Instance.FindMod(item2);
				if (mod3 == null)
				{
					DebugUtil.DevAssert(false, "SteamUGCService just told us this id was valid!");
				}
				else
				{
					Mod mod4 = MakeMod(mod3);
					if (mod4 != null)
					{
						Global.Instance.modManager.Update(mod4, this);
					}
				}
			}
			foreach (PublishedFileId_t item3 in removed)
			{
				PublishedFileId_t current3 = item3;
				Global.Instance.modManager.Unsubscribe(new Label
				{
					id = current3.m_PublishedFileId.ToString(),
					distribution_platform = Label.DistributionPlatform.Steam
				}, this);
			}
			if (added.Count() != 0)
			{
				Global.Instance.modManager.Sanitize(null);
			}
			else
			{
				Global.Instance.modManager.Report(null);
			}
		}
	}
}
