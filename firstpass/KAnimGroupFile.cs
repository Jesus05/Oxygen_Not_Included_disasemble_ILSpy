using Klei;
using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimGroupFile : ScriptableObject
{
	[Serializable]
	public class Group
	{
		[SerializeField]
		public HashedString id;

		[SerializeField]
		public string commandDirectory = "";

		[SerializeField]
		public List<KAnimFile> files = new List<KAnimFile>();

		[SerializeField]
		public KAnimBatchGroup.RendererType renderType;

		[SerializeField]
		public int maxVisibleSymbols;

		[SerializeField]
		public int maxGroupSize;

		[SerializeField]
		public HashedString target;

		[SerializeField]
		public HashedString swapTarget;

		[SerializeField]
		public HashedString animTarget;

		public Group(HashedString tag)
		{
			id = tag;
		}
	}

	public class GroupFile : YamlIO<GroupFile>
	{
		public string groupID
		{
			get;
			set;
		}

		public string commandDirectory
		{
			get;
			set;
		}
	}

	private const string MASTER_GROUP_FILE = "animgrouptags";

	public const int MAX_ANIMS_PER_GROUP = 10;

	private static KAnimGroupFile groupfile = null;

	private Dictionary<int, KAnimFileData> fileData = new Dictionary<int, KAnimFileData>();

	[SerializeField]
	private List<Group> groups = new List<Group>();

	[SerializeField]
	private List<Pair<HashedString, HashedString>> currentGroup = new List<Pair<HashedString, HashedString>>();

	private static bool hasCompletedLoadAll = false;

	public static void DestroyInstance()
	{
		groupfile = null;
	}

	public static string GetFilePath()
	{
		return "Assets/anim/resources/animgrouptags.asset";
	}

	public static KAnimGroupFile GetGroupFile()
	{
		if ((UnityEngine.Object)groupfile == (UnityEngine.Object)null)
		{
			groupfile = (KAnimGroupFile)Resources.Load("animgrouptags", typeof(KAnimGroupFile));
		}
		return groupfile;
	}

	public static void SetGroupFile(KAnimGroupFile file)
	{
		groupfile = file;
		groupfile.Sort();
	}

	public static Group AddDynamicGroup(HashedString tag)
	{
		GetGroupFile();
		List<Group> data = groupfile.GetData();
		Group group = new Group(tag);
		data.Add(group);
		return group;
	}

	public static Group GetGroup(HashedString tag)
	{
		Group result = null;
		GetGroupFile();
		List<Group> data = groupfile.GetData();
		for (int i = 0; i < data.Count; i++)
		{
			Group group = data[i];
			if (group.id == tag || group.target == tag)
			{
				result = group;
				break;
			}
		}
		return result;
	}

	public HashedString GetGroupForHomeDirectory(HashedString homedirectory)
	{
		for (int i = 0; i < currentGroup.Count; i++)
		{
			Pair<HashedString, HashedString> pair = currentGroup[i];
			if (pair.first == homedirectory)
			{
				Pair<HashedString, HashedString> pair2 = currentGroup[i];
				return pair2.second;
			}
		}
		return default(HashedString);
	}

	public List<Group> GetData()
	{
		return new List<Group>(groups);
	}

	public void Reset()
	{
		groups = new List<Group>();
		currentGroup = new List<Pair<HashedString, HashedString>>();
	}

	private int AddGroup(AnimCommandFile akf, GroupFile gf, KAnimFile file)
	{
		bool flag = akf.IsSwap(file);
		HashedString groupId = new HashedString(gf.groupID);
		int num = groups.FindIndex((Group t) => t.id == groupId);
		if (num == -1)
		{
			num = groups.Count;
			Group group = new Group(groupId);
			group.commandDirectory = akf.directory;
			group.maxGroupSize = akf.MaxGroupSize;
			group.renderType = akf.RendererType;
			int num2 = groups.FindIndex((Group t) => t.commandDirectory == group.commandDirectory);
			if (num2 == -1)
			{
				if (flag)
				{
					if (!string.IsNullOrEmpty(akf.TargetBuild))
					{
						group.target = new HashedString(akf.TargetBuild);
					}
					if (group.renderType != KAnimBatchGroup.RendererType.DontRender)
					{
						group.renderType = KAnimBatchGroup.RendererType.DontRender;
						group.swapTarget = new HashedString(akf.SwapTargetBuild);
					}
				}
				if (akf.Type == AnimCommandFile.ConfigType.AnimOnly)
				{
					group.target = new HashedString(akf.TargetBuild);
					group.renderType = KAnimBatchGroup.RendererType.AnimOnly;
					group.animTarget = new HashedString(akf.AnimTargetBuild);
					group.swapTarget = new HashedString(akf.SwapTargetBuild);
				}
			}
			groups.Add(group);
		}
		return num;
	}

	public bool AddAnimFile(GroupFile gf, AnimCommandFile akf, KAnimFile file)
	{
		int groupIndex = AddGroup(akf, gf, file);
		return AddFile(groupIndex, file);
	}

	private bool AddFile(int groupIndex, KAnimFile file)
	{
		if (groups[groupIndex].files.Contains(file))
		{
			return false;
		}
		Pair<HashedString, HashedString> pair = new Pair<HashedString, HashedString>(file.homedirectory, groups[groupIndex].id);
		bool flag = false;
		for (int i = 0; i < currentGroup.Count; i++)
		{
			Pair<HashedString, HashedString> pair2 = currentGroup[i];
			if (pair2.first == (HashedString)file.homedirectory)
			{
				currentGroup[i] = pair;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			currentGroup.Add(pair);
		}
		groups[groupIndex].files.Add(file);
		return true;
	}

	public void LoadAll()
	{
		fileData.Clear();
		for (int i = 0; i < groups.Count; i++)
		{
			if (!groups[i].id.IsValid)
			{
				Debug.LogErrorFormat("Group invalid groupIndex [{0}]", i);
			}
			KBatchGroupData kBatchGroupData = null;
			kBatchGroupData = ((!groups[i].target.IsValid) ? KAnimBatchManager.Instance().GetBatchGroupData(groups[i].id) : KAnimBatchManager.Instance().GetBatchGroupData(groups[i].target));
			HashedString batchTag = groups[i].id;
			if (groups[i].renderType == KAnimBatchGroup.RendererType.AnimOnly)
			{
				if (!groups[i].swapTarget.IsValid)
				{
					continue;
				}
				kBatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(groups[i].swapTarget);
				batchTag = groups[i].swapTarget;
			}
			for (int j = 0; j < groups[i].files.Count; j++)
			{
				KAnimFile kAnimFile = groups[i].files[j];
				if ((UnityEngine.Object)kAnimFile != (UnityEngine.Object)null && (UnityEngine.Object)kAnimFile.buildFile != (UnityEngine.Object)null && !fileData.ContainsKey(kAnimFile.GetInstanceID()))
				{
					if (kAnimFile.buildFile.bytes == null || kAnimFile.buildFile.bytes.Length == 0)
					{
						Debug.LogWarning("Build File [" + kAnimFile.buildFile.name + "] has 0 bytes", null);
					}
					else
					{
						HashedString hash = new HashedString(kAnimFile.name);
						HashCache.Get().Add(hash.HashValue, kAnimFile.name);
						KAnimFileData file = KGlobalAnimParser.Get().GetFile(kAnimFile);
						file.maxVisSymbolFrames = 0;
						file.batchTag = batchTag;
						file.buildIndex = KGlobalAnimParser.ParseBuildData(kBatchGroupData, hash, new FastReader(kAnimFile.buildFile.bytes), kAnimFile.textures);
						fileData.Add(kAnimFile.GetInstanceID(), file);
					}
				}
			}
		}
		for (int k = 0; k < groups.Count; k++)
		{
			if (groups[k].renderType == KAnimBatchGroup.RendererType.AnimOnly)
			{
				KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(groups[k].swapTarget);
				KBatchGroupData batchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(groups[k].animTarget);
				for (int l = 0; l < batchGroupData.builds.Count; l++)
				{
					KAnim.Build build = batchGroupData.builds[l];
					if (build != null && build.symbols != null)
					{
						for (int m = 0; m < build.symbols.Length; m++)
						{
							KAnim.Build.Symbol symbol = build.symbols[m];
							if (symbol != null && symbol.hash.IsValid() && batchGroupData2.GetFirstIndex(symbol.hash) == -1)
							{
								KAnim.Build.Symbol symbol2 = new KAnim.Build.Symbol();
								symbol2.build = build;
								symbol2.hash = symbol.hash;
								symbol2.path = symbol.path;
								symbol2.colourChannel = symbol.colourChannel;
								symbol2.flags = symbol.flags;
								symbol2.firstFrameIdx = batchGroupData2.symbolFrameInstances.Count;
								symbol2.numFrames = symbol.numFrames;
								symbol2.symbolIndexInSourceBuild = batchGroupData2.frameElementSymbols.Count;
								for (int n = 0; n < symbol2.numFrames; n++)
								{
									KAnim.Build.SymbolFrameInstance symbolFrameInstance = batchGroupData.GetSymbolFrameInstance(n + symbol.firstFrameIdx);
									KAnim.Build.SymbolFrameInstance item = default(KAnim.Build.SymbolFrameInstance);
									item.symbolFrame = symbolFrameInstance.symbolFrame;
									item.buildImageIdx = -1;
									item.symbolIdx = batchGroupData2.GetSymbolCount();
									batchGroupData2.symbolFrameInstances.Add(item);
								}
								batchGroupData2.AddBuildSymbol(symbol2);
							}
						}
					}
				}
			}
		}
		for (int num = 0; num < groups.Count; num++)
		{
			if (!groups[num].id.IsValid)
			{
				Debug.LogErrorFormat("Group invalid groupIndex [{0}]", num);
			}
			if (groups[num].renderType != KAnimBatchGroup.RendererType.DontRender)
			{
				KBatchGroupData kBatchGroupData2 = null;
				if (groups[num].animTarget.IsValid)
				{
					kBatchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(groups[num].animTarget);
					if (kBatchGroupData2 == null)
					{
						Debug.LogErrorFormat("Anim group is null for [{0}] -> [{1}]", groups[num].id, groups[num].animTarget);
					}
				}
				else
				{
					kBatchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(groups[num].id);
					if (kBatchGroupData2 == null)
					{
						Debug.LogErrorFormat("Anim group is null for [{0}]", groups[num].id);
					}
				}
				for (int num2 = 0; num2 < groups[num].files.Count; num2++)
				{
					KAnimFile kAnimFile2 = groups[num].files[num2];
					if ((UnityEngine.Object)kAnimFile2 != (UnityEngine.Object)null && (UnityEngine.Object)kAnimFile2.animFile != (UnityEngine.Object)null)
					{
						if (kAnimFile2.animFile.bytes == null || kAnimFile2.animFile.bytes.Length == 0)
						{
							Debug.LogWarning("Anim File [" + kAnimFile2.animFile.name + "] has 0 bytes", null);
						}
						else
						{
							if (!fileData.ContainsKey(kAnimFile2.GetInstanceID()))
							{
								KAnimFileData file2 = KGlobalAnimParser.Get().GetFile(kAnimFile2);
								file2.maxVisSymbolFrames = 0;
								file2.batchTag = groups[num].id;
								fileData.Add(kAnimFile2.GetInstanceID(), file2);
							}
							HashedString fileNameHash = new HashedString(kAnimFile2.name);
							FastReader reader = new FastReader(kAnimFile2.animFile.bytes);
							KAnimFileData animFile = fileData[kAnimFile2.GetInstanceID()];
							KGlobalAnimParser.ParseAnimData(kBatchGroupData2, fileNameHash, reader, animFile);
						}
					}
				}
			}
		}
		for (int num3 = 0; num3 < groups.Count; num3++)
		{
			if (!groups[num3].id.IsValid)
			{
				Debug.LogErrorFormat("Group invalid groupIndex [{0}]", num3);
			}
			KBatchGroupData kBatchGroupData3 = null;
			if (groups[num3].target.IsValid)
			{
				kBatchGroupData3 = KAnimBatchManager.Instance().GetBatchGroupData(groups[num3].target);
				if (kBatchGroupData3 == null)
				{
					Debug.LogErrorFormat("Group is null for  [{0}] target [{1}]", groups[num3].id, groups[num3].target);
				}
			}
			else
			{
				kBatchGroupData3 = KAnimBatchManager.Instance().GetBatchGroupData(groups[num3].id);
				if (kBatchGroupData3 == null)
				{
					Debug.LogErrorFormat("Group is null for [{0}]", groups[num3].id);
				}
			}
			KGlobalAnimParser.PostParse(kBatchGroupData3);
		}
		hasCompletedLoadAll = true;
	}

	private void Sort()
	{
		for (int i = 0; i < groups.Count; i++)
		{
			groups[i].files.RemoveAll((KAnimFile f) => (UnityEngine.Object)f == (UnityEngine.Object)null || f.name == null);
		}
		groups.RemoveAll((Group f) => f == null || f.files.Count == 0);
		groups.Sort((Group file0, Group file1) => file0.id.HashValue.CompareTo(file1.id.HashValue));
		for (int j = 0; j < groups.Count; j++)
		{
			if (groups[j].files.Count != 1)
			{
				List<KAnimFile> list = groups[j].files.FindAll((KAnimFile f) => (UnityEngine.Object)f.buildFile != (UnityEngine.Object)null);
				groups[j].files.RemoveAll((KAnimFile f) => (UnityEngine.Object)f.buildFile != (UnityEngine.Object)null);
				list.Sort((KAnimFile file0, KAnimFile file1) => (file0.homedirectory + file0.name).CompareTo(file1.homedirectory + file1.name));
				groups[j].files.Sort((KAnimFile file0, KAnimFile file1) => (file0.homedirectory + file0.name).CompareTo(file1.homedirectory + file1.name));
				groups[j].files.InsertRange(0, list);
			}
		}
	}
}
