using Klei;
using STRINGS;
using System.IO;
using UnityEngine;

namespace KMod
{
	public class Local : IDistributionPlatform
	{
		private class Header : YamlIO<Header>
		{
			public string title
			{
				get;
				set;
			}

			public string description
			{
				get;
				set;
			}
		}

		public string folder
		{
			get;
			private set;
		}

		public Label.DistributionPlatform distribution_platform
		{
			get;
			private set;
		}

		public Local(string folder, Label.DistributionPlatform distribution_platform)
		{
			this.folder = folder;
			this.distribution_platform = distribution_platform;
			DirectoryInfo directoryInfo = new DirectoryInfo(GetDirectory());
			if (directoryInfo.Exists)
			{
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					string id = directoryInfo2.Name.ToLower();
					Subscribe(id, directoryInfo2.LastWriteTime.ToFileTime(), new Directory(directoryInfo2.FullName));
				}
			}
		}

		public string GetDirectory()
		{
			return FSUtil.Normalize(Path.Combine(Manager.GetDirectory(), folder));
		}

		private void Subscribe(string id, long timestamp, IFileSource file_source)
		{
			string text = file_source.Read("mod.yaml");
			Header header = (!string.IsNullOrEmpty(text)) ? YamlIO<Header>.Parse(text, null) : null;
			if (header == null)
			{
				Header header2 = new Header();
				header2.title = id;
				header2.description = id;
				header = header2;
			}
			Label label = default(Label);
			label.id = id;
			label.distribution_platform = distribution_platform;
			label.version = (ulong)timestamp;
			label.title = header.title;
			Label label2 = label;
			Mod mod = new Mod(label2, header.description, file_source, UI.FRONTEND.MODS.TOOLTIPS.MANAGE_LOCAL_MOD, delegate
			{
				Application.OpenURL("file://" + file_source.GetRoot());
			});
			if (file_source.GetType() == typeof(Directory))
			{
				mod.status = Mod.Status.Installed;
			}
			Global.Instance.modManager.Subscribe(mod, this);
		}
	}
}
