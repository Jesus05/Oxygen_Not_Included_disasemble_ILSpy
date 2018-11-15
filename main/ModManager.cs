using Klei;
using Steamworks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ModManager
{
	private class UGCEventHandler : SteamUGCService.IUGCEventHandler
	{
		private ModManager mgr;

		public UGCEventHandler(ModManager mgr)
		{
			this.mgr = mgr;
		}

		public void OnUGCItemInstalled(ItemInstalled_t pCallback)
		{
		}

		public void OnUGCItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
		{
		}

		public void OnUGCItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
		{
		}

		public void OnUGCItemDownloaded(DownloadItemResult_t pCallback)
		{
		}

		public void OnUGCRefresh()
		{
			mgr.UpdateSteamCodeModSubscriptions();
		}
	}

	private List<ModInfo> activeMods = new List<ModInfo>();

	private ModInfo worldGenMod;

	private ModDB db;

	private const int MAJOR_VERSION = 0;

	private const int MINOR_VERSION = 1;

	public ICollection<ModInfo> ActiveMods => activeMods;

	public ModManager()
	{
		string modRootDir = GetModRootDir();
		db = new ModDB(GetDBFilename(), modRootDir);
		Save();
	}

	public ICollection<ModInfo> GetInstalledMods()
	{
		return db.GetMods();
	}

	private static string GetModRootDir()
	{
		string path = Util.RootFolder();
		return Path.Combine(path, "mods/");
	}

	private static string GetDBFilename()
	{
		return Path.Combine(GetModRootDir(), "moddb.json");
	}

	public void Start()
	{
		string modRootDir = GetModRootDir();
		db.LoadMods(this, modRootDir);
		Save();
	}

	public void Save()
	{
		db.Save(GetDBFilename());
	}

	public void Shutdown()
	{
		foreach (ModInfo activeMod in activeMods)
		{
			Unmount(activeMod);
		}
		activeMods.Clear();
	}

	public bool IsModEnabled(ModInfo info)
	{
		return db.IsEnabled(info);
	}

	public void EnableMod(ModInfo info)
	{
		db.Enable(info);
		Save();
	}

	public void DisableMod(ModInfo info)
	{
		db.Disable(info);
		Save();
	}

	public bool IsInstalled(ModInfo info)
	{
		return db.IsInstalled(info);
	}

	public void UninstallMod(ModInfo info)
	{
		db.Uninstall(GetModRootDir(), info);
	}

	public void ActivateWorldGenMod(ModInfo info)
	{
		DeactivateWorldGenMod();
		ActivateMod(info);
		worldGenMod = info;
	}

	public void Reorder(int a_idx, int b_idx)
	{
		db.Reorder(a_idx, b_idx);
		Save();
	}

	public void DeactivateWorldGenMod()
	{
		if (worldGenMod.type == ModInfo.ModType.WorldGen)
		{
			DeactiveMod(worldGenMod);
		}
	}

	public bool ActivateMod(ModInfo info)
	{
		foreach (ModInfo activeMod in activeMods)
		{
			ModInfo current = activeMod;
			if (current.assetID == info.assetID && current.source == info.source)
			{
				return true;
			}
		}
		bool flag = Mount(info);
		if (flag)
		{
			activeMods.Add(info);
		}
		return flag;
	}

	public void DeactiveMod(ModInfo info)
	{
		int num = activeMods.FindIndex((ModInfo active) => active.assetID == info.assetID && active.source == info.source);
		if (num > 0)
		{
			Unmount(activeMods[num]);
			activeMods.RemoveAt(num);
		}
	}

	private bool Mount(ModInfo info)
	{
		bool result = false;
		switch (info.type)
		{
		case ModInfo.ModType.WorldGen:
			switch (info.source)
			{
			case ModInfo.Source.Local:
				result = MountLocalMod(info);
				break;
			case ModInfo.Source.Steam:
				result = MountSteamMod(info);
				break;
			}
			break;
		case ModInfo.ModType.Mod:
		{
			ModInfo.Source source = info.source;
			if (source == ModInfo.Source.Local || source == ModInfo.Source.Steam)
			{
				result = MountDirectory(info);
			}
			break;
		}
		}
		return result;
	}

	private bool MountLocalMod(ModInfo info)
	{
		return MountZipFile(info.assetID, info.assetID, info.assetPath);
	}

	private bool MountDirectory(ModInfo info)
	{
		string modDir = db.GetModDir(GetModRootDir(), info);
		string fullPath = Path.GetFullPath(Application.dataPath);
		PrefixFileSystem fs = new PrefixFileSystem(info.assetID, modDir, fullPath);
		Global.Instance.layeredFileSystem.AddFileSystem(fs);
		return true;
	}

	private bool UnmountDirectory(ModInfo info)
	{
		string modDir = db.GetModDir(GetModRootDir(), info);
		string fullPath = Path.GetFullPath(Application.dataPath);
		PrefixFileSystem fs = new PrefixFileSystem(info.assetID, modDir, fullPath);
		Global.Instance.layeredFileSystem.AddFileSystem(fs);
		return true;
	}

	private bool MountZipFile(string filename, string mountpoint, string id)
	{
		bool result = false;
		if (File.Exists(filename))
		{
			FileStream zip_data_stream = File.OpenRead(filename);
			ZipFileSystem fs = new ZipFileSystem(id, zip_data_stream, mountpoint);
			Global.Instance.layeredFileSystem.AddFileSystem(fs);
			result = true;
		}
		return result;
	}

	private bool MountSteamMod(ModInfo info)
	{
		ulong value = ulong.Parse(info.assetID);
		PublishedFileId_t nPublishedFileID = new PublishedFileId_t(value);
		SteamUGC.GetItemInstallInfo(nPublishedFileID, out ulong _, out string pchFolder, 1024u, out uint _);
		return MountZipFile(pchFolder, info.assetPath, info.assetID);
	}

	private void Unmount(ModInfo info)
	{
		foreach (IFileSystem fileSystem in Global.Instance.layeredFileSystem.GetFileSystems())
		{
			if (info.assetID == fileSystem.GetID())
			{
				Global.Instance.layeredFileSystem.RemoveFileSystem(fileSystem);
				break;
			}
		}
	}

	public void RegisterUGCEventHandlers(SteamUGCService event_src)
	{
		UGCEventHandler item = new UGCEventHandler(this);
		event_src.ugcEventHandlers.Add(item);
	}

	public void UpdateSteamCodeModSubscriptions()
	{
		if (!((Object)SteamUGCService.Instance == (Object)null))
		{
			List<SteamUGCService.Subscribed> subscribed = SteamUGCService.Instance.GetSubscribed("mod");
			foreach (ModInfo mod in db.GetMods())
			{
				ModInfo installed = mod;
				if (installed.source == ModInfo.Source.Steam)
				{
					int num = subscribed.FindIndex(delegate(SteamUGCService.Subscribed i)
					{
						PublishedFileId_t fileId2 = i.fileId;
						return fileId2.m_PublishedFileId.ToString() == installed.assetID;
					});
					if (num < 0)
					{
						db.Uninstall(GetModRootDir(), installed);
					}
				}
			}
			foreach (SteamUGCService.Subscribed item in subscribed)
			{
				SteamUGC.GetItemInstallInfo(item.fileId, out ulong _, out string pchFolder, 1024u, out uint _);
				PublishedFileId_t fileId = item.fileId;
				string asset_id = fileId.m_PublishedFileId.ToString();
				ModInfo mod_info = new ModInfo(ModInfo.Source.Steam, ModInfo.ModType.Mod, asset_id, item.description, pchFolder, item.lastUpdateTime);
				db.Install(GetModRootDir(), mod_info);
			}
			Save();
		}
	}
}
