using Klei;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace KMod
{
	[JsonObject(MemberSerialization.OptIn)]
	[DebuggerDisplay("{title}")]
	public class Mod
	{
		public enum Status
		{
			NotInstalled,
			Installed,
			UninstallPending,
			ReinstallPending
		}

		[JsonProperty]
		public Label label;

		[JsonProperty]
		public Status status;

		[JsonProperty]
		public bool enabled;

		[JsonProperty]
		public int crash_count;

		public IFileSource file_source;

		public const int MAX_CRASH_COUNT = 3;

		public Content available_content
		{
			get;
			private set;
		}

		public LocString manage_tooltip
		{
			get;
			private set;
		}

		public System.Action on_managed
		{
			get;
			private set;
		}

		public bool is_managed => manage_tooltip != null;

		public string title => label.title;

		public string description
		{
			get;
			private set;
		}

		public Content loaded_content
		{
			get;
			private set;
		}

		[JsonConstructor]
		public Mod()
		{
		}

		public Mod(Label label, string description, IFileSource file_source, LocString manage_tooltip, System.Action on_managed)
		{
			enabled = false;
			this.label = label;
			status = Status.NotInstalled;
			this.description = description;
			this.file_source = file_source;
			this.manage_tooltip = manage_tooltip;
			this.on_managed = on_managed;
			loaded_content = (Content)0;
			available_content = (Content)0;
			ScanContent();
		}

		public void CopyPersistentDataTo(Mod other_mod)
		{
			other_mod.status = status;
			other_mod.enabled = enabled;
			other_mod.crash_count = crash_count;
			other_mod.loaded_content = loaded_content;
		}

		public void ScanContent()
		{
			available_content = (Content)0;
			if (file_source == null)
			{
				file_source = new Directory(label.install_path);
			}
			if (file_source.Exists())
			{
				List<FileSystemItem> list = new List<FileSystemItem>();
				file_source.GetTopLevelItems(list);
				foreach (FileSystemItem item in list)
				{
					FileSystemItem current = item;
					if (current.type == FileSystemItem.ItemType.Directory)
					{
						AddDirectory(current.name.ToLower());
					}
					else
					{
						AddFile(current.name.ToLower());
					}
				}
			}
		}

		public bool IsEmpty()
		{
			return available_content == (Content)0;
		}

		private void AddDirectory(string directory)
		{
			string text = directory.TrimEnd('/');
			if (text != null)
			{
				if (!(text == "strings"))
				{
					if (!(text == "codex"))
					{
						if (!(text == "elements"))
						{
							if (!(text == "templates"))
							{
								if (!(text == "worldgen"))
								{
									if (text == "anims")
									{
										available_content |= Content.Animation;
									}
								}
								else
								{
									available_content |= Content.LayerableFiles;
								}
							}
							else
							{
								available_content |= Content.LayerableFiles;
							}
						}
						else
						{
							available_content |= Content.LayerableFiles;
						}
					}
					else
					{
						available_content |= Content.LayerableFiles;
					}
				}
				else
				{
					available_content |= Content.Strings;
				}
			}
		}

		private void AddFile(string file)
		{
			if (file.EndsWith(".dll"))
			{
				available_content |= Content.DLL;
			}
			if (file.EndsWith(".po") || file.EndsWith(".pot"))
			{
				available_content |= Content.Translation;
			}
		}

		private static void AccumulateExtensions(Content content, List<string> extensions)
		{
			if ((content & Content.DLL) != 0)
			{
				extensions.Add(".dll");
			}
			if ((content & (Content.Strings | Content.Translation)) != 0)
			{
				extensions.Add(".po");
				extensions.Add(".pot");
			}
		}

		[Conditional("DEBUG")]
		private void Assert(bool condition, string failure_message)
		{
			if (string.IsNullOrEmpty(title))
			{
				DebugUtil.Assert(condition, string.Format("{2}\n\t{0}\n\t{1}", title, label.ToString(), failure_message));
			}
			else
			{
				DebugUtil.Assert(condition, string.Format("{1}\n\t{0}", label.ToString(), failure_message));
			}
		}

		public void Install()
		{
			if (label.distribution_platform != 0 && label.distribution_platform != Label.DistributionPlatform.Dev)
			{
				status = Status.ReinstallPending;
				if (file_source != null && FileUtil.DeleteDirectory(label.install_path, 0) && FileUtil.CreateDirectory(label.install_path, 0))
				{
					file_source.CopyTo(label.install_path, null);
					file_source = new Directory(label.install_path);
					status = Status.Installed;
				}
			}
		}

		public bool Uninstall()
		{
			if (label.distribution_platform == Label.DistributionPlatform.Local || label.distribution_platform == Label.DistributionPlatform.Dev)
			{
				return false;
			}
			enabled = false;
			if (loaded_content != 0)
			{
				Debug.Log($"Can't uninstall {label.ToString()}: still has loaded content: {loaded_content.ToString()}");
				status = Status.UninstallPending;
				return false;
			}
			if (!FileUtil.DeleteDirectory(label.install_path, 0))
			{
				Debug.Log($"Can't uninstall {label.ToString()}: directory deletion failed");
				status = Status.UninstallPending;
				return false;
			}
			status = Status.NotInstalled;
			return true;
		}

		private bool LoadStrings()
		{
			string[] array = new string[2]
			{
				"strings.pot",
				"strings.po"
			};
			string path = FSUtil.Normalize(Path.Combine(label.install_path, "strings"));
			if (!System.IO.Directory.Exists(path))
			{
				return false;
			}
			int num = 0;
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				bool flag = false;
				string[] array2 = array;
				foreach (string b in array2)
				{
					if (fileInfo.Name.ToLower() == b)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					num++;
					Dictionary<string, string> translated_strings = Localization.LoadStringsFile(fileInfo.FullName, Path.GetExtension(fileInfo.Name.ToLower()) == ".pot");
					Localization.OverloadStrings(translated_strings);
				}
			}
			return num > 0;
		}

		private bool LoadAnimation()
		{
			string path = FSUtil.Normalize(Path.Combine(label.install_path, "anims"));
			if (!System.IO.Directory.Exists(path))
			{
				return false;
			}
			int num = 0;
			ListPool<Texture2D, Mod>.PooledList pooledList = ListPool<Texture2D, Mod>.Allocate();
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				TextAsset anim_file = null;
				TextAsset build_file = null;
				pooledList.Clear();
				AssetBundle assetBundle = AssetBundle.LoadFromFile(fileInfo.FullName);
				UnityEngine.Object[] array = assetBundle.LoadAllAssets();
				UnityEngine.Object[] array2 = array;
				foreach (UnityEngine.Object @object in array2)
				{
					Texture2D texture2D = @object as Texture2D;
					if ((UnityEngine.Object)texture2D != (UnityEngine.Object)null)
					{
						pooledList.Add(texture2D);
					}
					else if (@object.name.EndsWith("_anim"))
					{
						anim_file = (@object as TextAsset);
					}
					else if (@object.name.EndsWith("_build"))
					{
						build_file = (@object as TextAsset);
					}
					else
					{
						DebugUtil.LogWarningArgs($"Unhandled asset ({@object.name}) in bundle ({fileInfo.FullName})...ignoring");
					}
				}
				if ((UnityEngine.Object)ModUtil.AddKAnim(fileInfo.Name, anim_file, build_file, pooledList) != (UnityEngine.Object)null)
				{
					num++;
				}
			}
			pooledList.Recycle();
			return num != 0;
		}

		public void Load(Content content)
		{
			content = (Content)((int)content & (int)(byte)((int)available_content & (int)(byte)(~(uint)loaded_content)));
			if ((content & Content.Strings) != 0 && LoadStrings())
			{
				loaded_content |= Content.Strings;
			}
			if ((content & Content.Translation) != 0)
			{
				loaded_content |= Content.Translation;
			}
			if ((content & Content.DLL) != 0 && DLLLoader.LoadDLLs(label.install_path))
			{
				loaded_content |= Content.DLL;
			}
			if ((content & Content.LayerableFiles) != 0)
			{
				Global.Instance.layeredFileSystem.AddFileSystem(file_source.GetFileSystem());
				loaded_content |= Content.LayerableFiles;
			}
			if ((content & Content.Animation) != 0 && LoadAnimation())
			{
				loaded_content |= Content.Animation;
			}
		}

		public void Unload(Content content)
		{
			content &= loaded_content;
			if ((content & Content.LayerableFiles) != 0)
			{
				Global.Instance.layeredFileSystem.RemoveFileSystem(file_source.GetFileSystem());
				loaded_content &= ~Content.LayerableFiles;
			}
		}

		private void SetCrashCount(int new_crash_count)
		{
			crash_count = MathUtil.Clamp(0, 3, new_crash_count);
		}

		public void Crash(bool do_disable)
		{
			SetCrashCount(crash_count + 1);
			if (do_disable)
			{
				enabled = false;
			}
		}

		public void Uncrash()
		{
			SetCrashCount((label.distribution_platform == Label.DistributionPlatform.Dev) ? (crash_count - 1) : 0);
		}

		public bool IsActive()
		{
			return loaded_content != (Content)0;
		}

		public bool AllActive(Content content)
		{
			return (loaded_content & content) == content;
		}

		public bool AllActive()
		{
			return (loaded_content & available_content) == available_content;
		}

		public bool AnyActive(Content content)
		{
			return (loaded_content & content) != (Content)0;
		}

		public bool HasContent()
		{
			return available_content != (Content)0;
		}

		public bool HasAnyContent(Content content)
		{
			return (available_content & content) != (Content)0;
		}
	}
}
