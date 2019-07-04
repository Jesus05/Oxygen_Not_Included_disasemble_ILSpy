using Klei;
using Newtonsoft.Json;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace KMod
{
	public class Manager
	{
		public delegate void OnUpdate(object change_source);

		private class PersistentData
		{
			public int version;

			public List<Mod> mods;

			public PersistentData()
			{
			}

			public PersistentData(int version, List<Mod> mods)
			{
				this.version = version;
				this.mods = mods;
			}
		}

		public const Content all_content = Content.LayerableFiles | Content.Strings | Content.DLL | Content.Translation | Content.Animation;

		public const Content boot_content = Content.Strings | Content.DLL | Content.Translation | Content.Animation;

		public const Content install_content = Content.DLL;

		public const Content on_demand_content = Content.LayerableFiles;

		public List<IDistributionPlatform> distribution_platforms = new List<IDistributionPlatform>();

		public List<Mod> mods = new List<Mod>();

		public List<Event> events = new List<Event>();

		private bool dirty = true;

		public OnUpdate on_update;

		private const int IO_OP_RETRY_COUNT = 5;

		private bool load_user_mod_loader_dll = true;

		private int current_version = 1;

		public Manager()
		{
			Debug.Log("Load mod database");
			string filename = GetFilename();
			try
			{
				FileUtil.DoIOAction(delegate
				{
					if (File.Exists(filename))
					{
						string value = File.ReadAllText(filename);
						PersistentData persistentData = JsonConvert.DeserializeObject<PersistentData>(value);
						mods = persistentData.mods;
					}
				}, 5);
			}
			catch (Exception)
			{
				Debug.LogWarningFormat(UI.FRONTEND.MODS.DB_CORRUPT, filename);
				mods = new List<Mod>();
			}
			List<Mod> list = new List<Mod>();
			bool flag = false;
			foreach (Mod mod in mods)
			{
				if (mod.status == Mod.Status.UninstallPending)
				{
					Debug.LogFormat("Latent uninstall of mod {0} from {1}", mod.title, mod.label.install_path);
					if (mod.Uninstall())
					{
						list.Add(mod);
					}
					else
					{
						DebugUtil.Assert(mod.status == Mod.Status.UninstallPending);
						Debug.LogFormat("\t...failed to uninstall mod {0}", mod.title);
					}
					if (mod.status != Mod.Status.UninstallPending)
					{
						flag = true;
					}
				}
			}
			foreach (Mod item in list)
			{
				mods.Remove(item);
			}
			foreach (Mod mod2 in mods)
			{
				mod2.ScanContent();
			}
			if (flag)
			{
				Save();
			}
		}

		public static string GetDirectory()
		{
			string path = Util.RootFolder();
			return Path.Combine(path, "mods/");
		}

		public void Shutdown()
		{
			foreach (Mod mod in mods)
			{
				mod.Unload(Content.LayerableFiles);
			}
		}

		public void Sanitize(GameObject parent)
		{
			ListPool<Label, Manager>.PooledList pooledList = ListPool<Label, Manager>.Allocate();
			foreach (Mod mod in mods)
			{
				if (!mod.is_subscribed)
				{
					pooledList.Add(mod.label);
				}
			}
			foreach (Label item in pooledList)
			{
				Unsubscribe(item, this);
			}
			pooledList.Recycle();
			Report(parent);
		}

		public bool HaveMods()
		{
			foreach (Mod mod in mods)
			{
				if (mod.status == Mod.Status.Installed && mod.HasContent())
				{
					return true;
				}
			}
			return false;
		}

		public bool HaveLoadedMods()
		{
			foreach (Mod mod in mods)
			{
				if (mod.status != 0 && mod.IsActive())
				{
					return true;
				}
			}
			return false;
		}

		private void Install(Mod mod)
		{
			if (mod.status == Mod.Status.NotInstalled)
			{
				Debug.LogFormat("\tInstalling mod: {0}", mod.title);
				mod.Install();
				if (mod.status == Mod.Status.Installed)
				{
					Debug.Log("\tSuccessfully installed.");
					events.Add(new Event
					{
						event_type = EventType.Installed,
						mod = mod.label
					});
				}
				else
				{
					Debug.Log("\tFailed install. Will install on restart.");
					events.Add(new Event
					{
						event_type = EventType.InstallFailed,
						mod = mod.label
					});
					events.Add(new Event
					{
						event_type = EventType.RestartRequested,
						mod = mod.label
					});
				}
			}
		}

		private void Uninstall(Mod mod)
		{
			if (mod.status != 0)
			{
				Debug.LogFormat("\tUninstalling mod {0}", mod.title);
				mod.Uninstall();
				if (mod.status == Mod.Status.UninstallPending)
				{
					Debug.Log("\tFailed. Will re-install on restart.");
					mod.status = Mod.Status.ReinstallPending;
					events.Add(new Event
					{
						event_type = EventType.RestartRequested,
						mod = mod.label
					});
				}
			}
		}

		public void Subscribe(Mod mod, object caller)
		{
			Debug.LogFormat("Subscribe to mod {0}", mod.title);
			Mod mod2 = mods.Find((Mod candidate) => mod.label.Match(candidate.label));
			mod.is_subscribed = true;
			if (mod2 == null)
			{
				mods.Add(mod);
				Install(mod);
			}
			else
			{
				Debug.LogFormat("\tAlready subscribed mod: {0}", mod.title);
				if (mod2.status == Mod.Status.UninstallPending)
				{
					mod2.status = Mod.Status.Installed;
					events.Add(new Event
					{
						event_type = EventType.Installed,
						mod = mod2.label
					});
				}
				bool flag = mod2.label.version != mod.label.version;
				bool flag2 = mod2.available_content != mod.available_content;
				bool flag3 = (flag || flag2 || mod2.status == Mod.Status.ReinstallPending) ? true : false;
				if (flag)
				{
					events.Add(new Event
					{
						event_type = EventType.VersionUpdate,
						mod = mod.label
					});
				}
				if (flag2)
				{
					events.Add(new Event
					{
						event_type = EventType.AvailableContentChanged,
						mod = mod.label
					});
				}
				mod2.CopyPersistentDataTo(mod);
				int index = mods.IndexOf(mod2);
				mods.RemoveAt(index);
				mods.Insert(index, mod);
				if (flag3 || mod.status == Mod.Status.NotInstalled)
				{
					bool enabled = mod.enabled;
					if (flag3)
					{
						Uninstall(mod);
					}
					Install(mod);
					mod.enabled = enabled;
					if (mod.enabled && (mod.available_content & (Content.Strings | Content.DLL | Content.Translation | Content.Animation)) != 0)
					{
						events.Add(new Event
						{
							event_type = EventType.RestartRequested,
							mod = mod.label
						});
					}
				}
				else
				{
					mod.file_source = mod2.file_source;
				}
			}
			dirty = true;
			Update(caller);
		}

		public void Update(Mod mod, object caller)
		{
			Debug.LogFormat("Update mod {0}", mod.title);
			Mod mod2 = mods.Find((Mod candidate) => mod.label.Match(candidate.label));
			DebugUtil.DevAssert(string.IsNullOrEmpty(mod2.label.id), "Should be subscribed to a mod we are getting an Update notification for");
			if (mod2.status != Mod.Status.UninstallPending)
			{
				events.Add(new Event
				{
					event_type = EventType.VersionUpdate,
					mod = mod.label
				});
				mod2.CopyPersistentDataTo(mod);
				int index = mods.IndexOf(mod2);
				mods.RemoveAt(index);
				mods.Insert(index, mod);
				bool enabled = mod.enabled;
				Uninstall(mod);
				Install(mod);
				mod.enabled = enabled;
				if (mod.enabled && (mod.available_content & (Content.Strings | Content.DLL | Content.Translation | Content.Animation)) != 0)
				{
					events.Add(new Event
					{
						event_type = EventType.RestartRequested,
						mod = mod.label
					});
				}
				dirty = true;
				Update(caller);
			}
		}

		public void Unsubscribe(Label label, object caller)
		{
			Debug.LogFormat("Unsubscribe from mod {0}", label.ToString());
			int num = 0;
			foreach (Mod mod2 in mods)
			{
				if (mod2.label.Match(label))
				{
					Debug.LogFormat("\t...found it: {0}", mod2.title);
					break;
				}
				num++;
			}
			if (num == mods.Count)
			{
				Debug.LogFormat("\t...not found");
			}
			else
			{
				Mod mod = mods[num];
				mod.enabled = false;
				mod.Unload(Content.LayerableFiles);
				events.Add(new Event
				{
					event_type = EventType.Uninstalled,
					mod = mod.label
				});
				if (mod.IsActive())
				{
					Debug.LogFormat("\tCould not unload all content provided by mod {0} : {1}\nUninstall will likely fail", mod.title, mod.label.ToString());
					events.Add(new Event
					{
						event_type = EventType.RestartRequested,
						mod = mod.label
					});
				}
				if (mod.status == Mod.Status.Installed)
				{
					Debug.LogFormat("\tUninstall mod {0} : {1}", mod.title, mod.label.ToString());
					mod.Uninstall();
				}
				if (mod.status == Mod.Status.NotInstalled)
				{
					Debug.LogFormat("\t...success. Removing from management list {0} : {1}", mod.title, mod.label.ToString());
					mods.RemoveAt(num);
				}
				dirty = true;
				Update(caller);
			}
		}

		public bool IsInDevMode()
		{
			return mods.Exists((Mod mod) => mod.enabled && mod.label.distribution_platform == Label.DistributionPlatform.Dev);
		}

		public void Load(Content content)
		{
			if ((content & Content.DLL) != 0 && load_user_mod_loader_dll)
			{
				if (!DLLLoader.LoadUserModLoaderDLL())
				{
					Debug.Log("ModLoader.dll failed to load. Either it is not present or it encountered an error");
				}
				load_user_mod_loader_dll = false;
			}
			Debug.LogFormat("Load content ({0}) for mods:", content);
			foreach (Mod mod in mods)
			{
				if (mod.enabled)
				{
					mod.Load(content);
					if (mod.IsActive())
					{
						Debug.LogFormat("\t{0}", mod.title);
					}
				}
			}
			bool flag = false;
			bool flag2 = IsInDevMode();
			foreach (Mod mod2 in mods)
			{
				Content content2 = mod2.loaded_content & content;
				Content content3 = mod2.available_content & content;
				if (mod2.enabled && content2 != content3)
				{
					mod2.Crash(!flag2);
					if (!mod2.enabled)
					{
						flag = true;
						events.Add(new Event
						{
							event_type = EventType.Deactivated,
							mod = mod2.label
						});
					}
					Debug.LogFormat("Failed to load mod {0}...disabling", mod2.title);
					events.Add(new Event
					{
						event_type = EventType.LoadError,
						mod = mod2.label
					});
				}
			}
			if (flag)
			{
				Save();
			}
		}

		public void Unload(Content content)
		{
			foreach (Mod mod in mods)
			{
				mod.Unload(content);
			}
		}

		public void Update(object change_source)
		{
			if (dirty)
			{
				dirty = false;
				Save();
				if (on_update != null)
				{
					on_update(change_source);
				}
			}
		}

		public bool MatchFootprint(List<Label> footprint, Content relevant_content)
		{
			if (footprint != null)
			{
				bool flag = true;
				bool flag2 = true;
				bool flag3 = false;
				int num = -1;
				Func<Label, Mod, bool> is_match = (Label label, Mod mod) => mod.label.Match(label);
				foreach (Label item in footprint)
				{
					bool flag4 = false;
					for (int i = num + 1; i != mods.Count; i++)
					{
						Mod mod2 = mods[i];
						num = i;
						Content content = mod2.available_content & relevant_content;
						bool flag5 = content != (Content)0;
						if (is_match(item, mod2))
						{
							if (flag5)
							{
								if (!mod2.enabled)
								{
									events.Add(new Event
									{
										event_type = EventType.ExpectedActive,
										mod = item
									});
									flag = false;
								}
								else if (!mod2.AllActive(content))
								{
									events.Add(new Event
									{
										event_type = EventType.LoadError,
										mod = item
									});
								}
							}
							flag4 = true;
							break;
						}
						if (flag5 && mod2.enabled)
						{
							events.Add(new Event
							{
								event_type = EventType.ExpectedInactive,
								mod = mod2.label
							});
							flag3 = true;
						}
					}
					if (!flag4)
					{
						events.Add(new Event
						{
							event_type = ((!mods.Exists((Mod candidate) => is_match(item, candidate))) ? EventType.NotFound : EventType.OutOfOrder),
							mod = item
						});
						flag2 = false;
					}
				}
				for (int j = num + 1; j != mods.Count; j++)
				{
					Mod mod3 = mods[j];
					if ((mod3.available_content & relevant_content) != 0 && mod3.enabled)
					{
						events.Add(new Event
						{
							event_type = EventType.ExpectedInactive,
							mod = mod3.label
						});
						flag3 = true;
					}
				}
				return flag2 && flag && !flag3;
			}
			return true;
		}

		private string GetFilename()
		{
			return FileSystem.Normalize(Path.Combine(GetDirectory(), "mods.json"));
		}

		public static void Dialog(GameObject parent = null, string title = null, string text = null, string confirm_text = null, System.Action on_confirm = null, string cancel_text = null, System.Action on_cancel = null, string configurable_text = null, System.Action on_configurable_clicked = null, Sprite image_sprite = null, bool activateBlackBackground = true)
		{
			ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent ?? Global.Instance.globalCanvas);
			confirmDialogScreen.PopupConfirmDialog(text, on_confirm, on_cancel, configurable_text, on_configurable_clicked, title, confirm_text, cancel_text, image_sprite, activateBlackBackground);
		}

		private static string MakeModList(List<Event> events, EventType event_type)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			foreach (Event @event in events)
			{
				Event current = @event;
				if (current.event_type == event_type)
				{
					stringBuilder.AppendLine(current.mod.title);
				}
			}
			return stringBuilder.ToString();
		}

		private static string MakeEventList(List<Event> events)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			string title = null;
			string title_tooltip = null;
			foreach (Event @event in events)
			{
				Event current = @event;
				Event.GetUIStrings(current.event_type, out title, out title_tooltip);
				stringBuilder.AppendFormat("{0}: {1}", title, current.mod.title);
				if (!string.IsNullOrEmpty(current.details))
				{
					stringBuilder.AppendFormat(" ({0})", current.details);
				}
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}

		private static string MakeModList(List<Event> events)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			HashSetPool<string, Manager>.PooledHashSet pooledHashSet = HashSetPool<string, Manager>.Allocate();
			foreach (Event @event in events)
			{
				Event current = @event;
				if (pooledHashSet.Add(current.mod.title))
				{
					stringBuilder.AppendLine(current.mod.title);
				}
			}
			pooledHashSet.Recycle();
			return stringBuilder.ToString();
		}

		private void LoadFailureDialog(GameObject parent)
		{
			if (events.Count != 0)
			{
				foreach (Event @event in events)
				{
					Event current = @event;
					if (current.event_type == EventType.LoadError)
					{
						foreach (Mod mod in mods)
						{
							if (mod.label.distribution_platform != 0 && mod.label.distribution_platform != Label.DistributionPlatform.Dev && mod.label.Match(current.mod))
							{
								mod.status = Mod.Status.ReinstallPending;
							}
						}
					}
				}
				dirty = true;
				Update(this);
				string title = UI.FRONTEND.MOD_DIALOGS.LOAD_FAILURE.TITLE;
				string text = string.Format(UI.FRONTEND.MOD_DIALOGS.LOAD_FAILURE.MESSAGE, MakeModList(events, EventType.LoadError));
				string confirm_text = UI.FRONTEND.MOD_DIALOGS.RESTART.OK;
				string cancel_text = UI.FRONTEND.MOD_DIALOGS.RESTART.CANCEL;
				Dialog(parent, title, text, confirm_text, App.instance.Restart, cancel_text, delegate
				{
				}, null, null, null, true);
				events.Clear();
			}
		}

		private void DevRestartDialog(GameObject parent, bool is_crash)
		{
			if (events.Count != 0)
			{
				if (is_crash)
				{
					string title = UI.FRONTEND.MOD_DIALOGS.MOD_ERRORS_ON_BOOT.TITLE;
					string text = string.Format(UI.FRONTEND.MOD_DIALOGS.MOD_ERRORS_ON_BOOT.DEV_MESSAGE, MakeEventList(events));
					string confirm_text = UI.FRONTEND.MOD_DIALOGS.RESTART.OK;
					string cancel_text = UI.FRONTEND.MOD_DIALOGS.RESTART.CANCEL;
					Dialog(parent, title, text, confirm_text, delegate
					{
						foreach (Mod mod in mods)
						{
							mod.enabled = false;
						}
						dirty = true;
						Update(this);
						App.instance.Restart();
					}, cancel_text, delegate
					{
					}, null, null, null, true);
				}
				else
				{
					string cancel_text = UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.TITLE;
					string confirm_text = string.Format(UI.FRONTEND.MOD_DIALOGS.RESTART.DEV_MESSAGE, MakeEventList(events));
					string text = UI.FRONTEND.MOD_DIALOGS.RESTART.OK;
					string title = UI.FRONTEND.MOD_DIALOGS.RESTART.CANCEL;
					Dialog(parent, cancel_text, confirm_text, text, delegate
					{
						App.instance.Restart();
					}, title, delegate
					{
					}, null, null, null, true);
				}
				events.Clear();
			}
		}

		public void RestartDialog(string title, string message_format, System.Action on_cancel, bool with_details, GameObject parent, string cancel_text = null)
		{
			if (events.Count != 0)
			{
				string text = string.Format(message_format, (!with_details) ? MakeModList(events) : MakeEventList(events));
				string confirm_text = UI.FRONTEND.MOD_DIALOGS.RESTART.OK;
				string cancel_text2 = cancel_text ?? ((string)UI.FRONTEND.MOD_DIALOGS.RESTART.CANCEL);
				Dialog(parent, title, text, confirm_text, App.instance.Restart, cancel_text2, on_cancel, null, null, null, true);
				events.Clear();
			}
		}

		public void NotifyDialog(string title, string message_format, GameObject parent)
		{
			if (events.Count != 0)
			{
				Dialog(parent, title, string.Format(message_format, MakeEventList(events)), null, null, null, null, null, null, null, true);
				events.Clear();
			}
		}

		public void HandleCrash()
		{
			Debug.Log("Error occurred with mods active. Disabling all mods (unless dev mods active).");
			bool flag = IsInDevMode();
			foreach (Mod mod in mods)
			{
				if (mod.enabled)
				{
					events.Add(new Event
					{
						event_type = EventType.ActiveDuringCrash,
						mod = mod.label
					});
					mod.Crash(!flag);
					if (!flag)
					{
						events.Add(new Event
						{
							event_type = EventType.Deactivated,
							mod = mod.label
						});
					}
				}
			}
			dirty = true;
			Update(this);
		}

		public void HandleErrors(List<YamlIO.Error> world_gen_errors)
		{
			string value = FileSystem.Normalize(GetDirectory());
			ListPool<Mod, Manager>.PooledList pooledList = ListPool<Mod, Manager>.Allocate();
			foreach (YamlIO.Error world_gen_error in world_gen_errors)
			{
				YamlIO.Error current = world_gen_error;
				string text = (current.file.source == null) ? string.Empty : FileSystem.Normalize(current.file.source.GetRoot());
				YamlIO.LogError(current, text.Contains(value));
				if (current.severity != YamlIO.Error.Severity.Recoverable && text.Contains(value))
				{
					foreach (Mod mod in mods)
					{
						if (mod.enabled && text.Contains(mod.label.install_path))
						{
							events.Add(new Event
							{
								event_type = EventType.BadWorldGen,
								mod = mod.label,
								details = Path.GetFileName(current.file.full_path)
							});
							break;
						}
					}
				}
			}
			bool flag = IsInDevMode();
			foreach (Mod item in pooledList)
			{
				item.Crash(!flag);
				if (!flag)
				{
					events.Add(new Event
					{
						event_type = EventType.Deactivated,
						mod = item.label
					});
				}
				dirty = true;
			}
			pooledList.Recycle();
			Update(this);
		}

		public void Report(GameObject parent)
		{
			if (events.Count != 0)
			{
				for (int i = 0; i < events.Count; i++)
				{
					Event @event = events[i];
					for (int num = events.Count - 1; num != i; num--)
					{
						Event event2 = events[num];
						if (event2.event_type == @event.event_type)
						{
							Event event3 = events[num];
							if (event3.mod.Match(@event.mod))
							{
								Event event4 = events[num];
								if (event4.details == @event.details)
								{
									events.RemoveAt(num);
								}
							}
						}
					}
				}
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				foreach (Event event5 in events)
				{
					Event current = event5;
					switch (current.event_type)
					{
					case EventType.ActiveDuringCrash:
						flag = true;
						break;
					case EventType.LoadError:
						flag2 = true;
						break;
					case EventType.VersionUpdate:
					case EventType.AvailableContentChanged:
					case EventType.RestartRequested:
						flag3 = true;
						break;
					case EventType.Deactivated:
						if ((FindMod(current.mod).available_content & (Content.Strings | Content.DLL | Content.Translation | Content.Animation)) != 0)
						{
							flag3 = true;
						}
						break;
					}
				}
				flag3 = (flag || flag2 || flag3);
				bool flag4 = IsInDevMode();
				if (flag3 && flag4)
				{
					DevRestartDialog(parent, flag);
				}
				else if (flag2)
				{
					LoadFailureDialog(parent);
				}
				else if (flag)
				{
					RestartDialog(UI.FRONTEND.MOD_DIALOGS.MOD_ERRORS_ON_BOOT.TITLE, UI.FRONTEND.MOD_DIALOGS.MOD_ERRORS_ON_BOOT.MESSAGE, null, false, parent, null);
				}
				else if (flag3)
				{
					RestartDialog(UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.TITLE, UI.FRONTEND.MOD_DIALOGS.RESTART.MESSAGE, null, true, parent, null);
				}
				else
				{
					NotifyDialog(UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.TITLE, (!flag4) ? UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.MESSAGE : UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.DEV_MESSAGE, parent);
				}
			}
		}

		public bool Save()
		{
			if (FileUtil.CreateDirectory(GetDirectory(), 5))
			{
				FileStream stream = FileUtil.Create(GetFilename(), 5);
				try
				{
					if (stream == null)
					{
						return false;
					}
					using (StreamWriter streamWriter = FileUtil.DoIODialog(() => new StreamWriter(stream), GetFilename(), null, 5))
					{
						if (streamWriter == null)
						{
							return false;
						}
						string value = JsonConvert.SerializeObject(new PersistentData(current_version, mods), Formatting.Indented);
						streamWriter.Write(value);
					}
				}
				finally
				{
					if (stream != null)
					{
						((IDisposable)stream).Dispose();
					}
				}
				return true;
			}
			return false;
		}

		public Mod FindMod(Label label)
		{
			foreach (Mod mod in mods)
			{
				if (mod.label.Equals(label))
				{
					return mod;
				}
			}
			return null;
		}

		public bool IsModEnabled(Label id)
		{
			return FindMod(id)?.enabled ?? false;
		}

		public bool EnableMod(Label id, bool enabled, object caller)
		{
			Mod mod = FindMod(id);
			if (mod != null)
			{
				if (mod.enabled != enabled)
				{
					mod.enabled = enabled;
					if (enabled)
					{
						mod.Load(Content.LayerableFiles);
					}
					else
					{
						mod.Unload(Content.LayerableFiles);
					}
					dirty = true;
					Update(caller);
					return true;
				}
				return false;
			}
			return false;
		}

		public void Reinsert(int source_index, int target_index, object caller)
		{
			DebugUtil.Assert(source_index != target_index);
			if (source_index >= -1 && mods.Count > source_index && target_index >= -1 && mods.Count >= target_index)
			{
				Mod item = mods[source_index];
				mods.RemoveAt(source_index);
				if (source_index < target_index)
				{
					target_index--;
				}
				if (target_index == mods.Count)
				{
					mods.Add(item);
				}
				else
				{
					mods.Insert(target_index, item);
				}
				dirty = true;
				Update(caller);
			}
		}

		public void SendMetricsEvent()
		{
			ListPool<string, Manager>.PooledList pooledList = ListPool<string, Manager>.Allocate();
			foreach (Mod mod in mods)
			{
				if (mod.enabled)
				{
					pooledList.Add(mod.title);
				}
			}
			DictionaryPool<string, object, Manager>.PooledDictionary pooledDictionary = DictionaryPool<string, object, Manager>.Allocate();
			pooledDictionary["ModCount"] = pooledList.Count;
			pooledDictionary["Mods"] = pooledList;
			ThreadedHttps<KleiMetrics>.Instance.SendEvent(pooledDictionary);
			pooledDictionary.Recycle();
			pooledList.Recycle();
			KCrashReporter.haveActiveMods = (pooledList.Count > 0);
		}
	}
}
