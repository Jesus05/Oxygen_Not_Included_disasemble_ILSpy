using Klei;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KGlobalAnimParser
{
	public static KAnimHashedString MISSING_SYMBOL = new KAnimHashedString("MISSING_SYMBOL");

	public static string ANIM_COMMAND_FILE = "batchgroup.yaml";

	public const float ANIM_SCALE = 0.005f;

	private Dictionary<HashedString, AnimCommandFile> commandFiles = new Dictionary<HashedString, AnimCommandFile>();

	private Dictionary<int, KAnimFileData> files = new Dictionary<int, KAnimFileData>();

	private static KGlobalAnimParser instance => Singleton<KGlobalAnimParser>.Instance;

	public static void CreateInstance()
	{
		Singleton<KGlobalAnimParser>.CreateInstance();
	}

	public static KGlobalAnimParser Get()
	{
		return instance;
	}

	public static void DestroyInstance()
	{
		if (instance != null)
		{
			instance.commandFiles.Clear();
			instance.commandFiles = null;
			instance.files.Clear();
			instance.files = null;
		}
		Singleton<KGlobalAnimParser>.DestroyInstance();
	}

	public KAnimFileData GetFile(KAnimFile anim_file)
	{
		KAnimFileData value = null;
		int instanceID = anim_file.GetInstanceID();
		if (!files.TryGetValue(instanceID, out value))
		{
			value = new KAnimFileData(anim_file.name);
			files[instanceID] = value;
		}
		return value;
	}

	public KAnimFileData Load(KAnimFile anim_file)
	{
		KAnimFileData value = null;
		int instanceID = anim_file.GetInstanceID();
		if (!files.TryGetValue(instanceID, out value))
		{
			value = GetFile(anim_file);
		}
		return value;
	}

	public static AnimCommandFile GetParseCommands(string path)
	{
		string fullName = Directory.GetParent(path).FullName;
		HashedString key = new HashedString(fullName);
		if (Get().commandFiles.ContainsKey(key))
		{
			return instance.commandFiles[key];
		}
		string text = Path.Combine(fullName, ANIM_COMMAND_FILE);
		if (File.Exists(text))
		{
			AnimCommandFile animCommandFile = YamlIO.LoadFile<AnimCommandFile>(text, null, null);
			animCommandFile.directory = "Assets/anim/" + Directory.GetParent(path).Name;
			instance.commandFiles[key] = animCommandFile;
			return animCommandFile;
		}
		return null;
	}

	public static void ParseAnimData(KBatchGroupData data, HashedString fileNameHash, FastReader reader, KAnimFileData animFile)
	{
		CheckHeader("ANIM", reader);
		uint num = reader.ReadUInt32();
		Assert(num == 5, "Invalid anim.bytes version");
		reader.ReadInt32();
		reader.ReadInt32();
		int num2 = reader.ReadInt32();
		animFile.maxVisSymbolFrames = 0;
		animFile.animCount = 0;
		animFile.frameCount = 0;
		animFile.elementCount = 0;
		animFile.firstAnimIndex = data.anims.Count;
		animFile.animBatchTag = data.groupID;
		data.animIndex.Add(fileNameHash, data.anims.Count);
		animFile.firstElementIndex = data.frameElements.Count;
		for (int i = 0; i < num2; i++)
		{
			KAnim.Anim anim = new KAnim.Anim(animFile, data.anims.Count);
			anim.name = reader.ReadKleiString();
			string text = animFile.name + "." + anim.name;
			anim.id = text;
			HashCache.Get().Add(anim.name);
			HashCache.Get().Add(text);
			anim.hash = anim.name;
			anim.rootSymbol.HashValue = reader.ReadInt32();
			anim.frameRate = reader.ReadSingle();
			anim.firstFrameIdx = data.animFrames.Count;
			anim.numFrames = reader.ReadInt32();
			anim.totalTime = (float)anim.numFrames / anim.frameRate;
			anim.scaledBoundingRadius = 0f;
			for (int j = 0; j < anim.numFrames; j++)
			{
				KAnim.Anim.Frame item = default(KAnim.Anim.Frame);
				float num3 = reader.ReadSingle();
				float num4 = reader.ReadSingle();
				float num5 = reader.ReadSingle();
				float num6 = reader.ReadSingle();
				item.bbox = new AABB3(new Vector3(num3 - num5 * 0.5f, 0f - (num4 + num6 * 0.5f), 0f) * 0.005f, new Vector3(num3 + num5 * 0.5f, 0f - (num4 - num6 * 0.5f), 0f) * 0.005f);
				float num7 = Math.Max(Math.Abs(item.bbox.max.x), Math.Abs(item.bbox.min.x));
				float num8 = Math.Max(Math.Abs(item.bbox.max.y), Math.Abs(item.bbox.min.y));
				float num9 = Math.Max(num7, num8);
				anim.unScaledSize.x = Math.Max(anim.unScaledSize.x, num7 / 0.005f);
				anim.unScaledSize.y = Math.Max(anim.unScaledSize.y, num8 / 0.005f);
				anim.scaledBoundingRadius = Math.Max(anim.scaledBoundingRadius, Mathf.Sqrt(num9 * num9 + num9 * num9));
				item.idx = data.animFrames.Count;
				item.firstElementIdx = data.frameElements.Count;
				item.numElements = reader.ReadInt32();
				int num10 = 0;
				for (int k = 0; k < item.numElements; k++)
				{
					KAnim.Anim.FrameElement item2 = default(KAnim.Anim.FrameElement);
					item2.fileHash = fileNameHash;
					item2.symbol = new KAnimHashedString(reader.ReadInt32());
					item2.frame = reader.ReadInt32();
					item2.folder = new KAnimHashedString(reader.ReadInt32());
					item2.flags = reader.ReadInt32();
					float a = reader.ReadSingle();
					float b = reader.ReadSingle();
					float g = reader.ReadSingle();
					float r = reader.ReadSingle();
					item2.multColour = new Color(r, g, b, a);
					float m = reader.ReadSingle();
					float m2 = reader.ReadSingle();
					float m3 = reader.ReadSingle();
					float m4 = reader.ReadSingle();
					float m5 = reader.ReadSingle();
					float m6 = reader.ReadSingle();
					reader.ReadSingle();
					item2.transform.m00 = m;
					item2.transform.m01 = m3;
					item2.transform.m02 = m5;
					item2.transform.m10 = m2;
					item2.transform.m11 = m4;
					item2.transform.m12 = m6;
					int symbolIndex = data.GetSymbolIndex(item2.symbol);
					if (symbolIndex == -1)
					{
						num10++;
						item2.symbol = MISSING_SYMBOL;
					}
					else
					{
						item2.symbolIdx = symbolIndex;
						data.frameElements.Add(item2);
						animFile.elementCount++;
					}
				}
				item.numElements -= num10;
				data.animFrames.Add(item);
				animFile.frameCount++;
			}
			data.AddAnim(anim);
			animFile.animCount++;
		}
		Debug.Assert(num2 == animFile.animCount);
		data.animCount[fileNameHash] = animFile.animCount;
		animFile.maxVisSymbolFrames = Math.Max(animFile.maxVisSymbolFrames, reader.ReadInt32());
		data.UpdateMaxVisibleSymbols(animFile.maxVisSymbolFrames);
		ParseHashTable(reader);
	}

	private static void ParseHashTable(FastReader reader)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int hash = reader.ReadInt32();
			string text = reader.ReadKleiString();
			HashCache.Get().Add(hash, text);
		}
	}

	public static int ParseBuildData(KBatchGroupData data, KAnimHashedString fileNameHash, FastReader reader, List<Texture2D> textures)
	{
		CheckHeader("BILD", reader);
		int num = reader.ReadInt32();
		if (num != 10 && num != 9)
		{
			Debug.LogError(fileNameHash + " has invalid build.bytes version [" + num + "]");
			return -1;
		}
		KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(data.groupID);
		if (group == null)
		{
			Debug.LogErrorFormat("[{1}] Failed to get group [{0}]", data.groupID, fileNameHash.DebuggerDisplay);
		}
		KAnim.Build build = null;
		int num2 = reader.ReadInt32();
		int num3 = reader.ReadInt32();
		build = data.AddNewBuildFile(fileNameHash);
		build.textureCount = textures.Count;
		if (textures.Count > 0)
		{
			data.AddTextures(textures);
		}
		build.symbols = new KAnim.Build.Symbol[num2];
		build.frames = new KAnim.Build.SymbolFrame[num3];
		build.name = reader.ReadKleiString();
		build.batchTag = ((!group.swapTarget.IsValid) ? data.groupID : group.target);
		build.fileHash = fileNameHash;
		int num4 = 0;
		for (int i = 0; i < build.symbols.Length; i++)
		{
			KAnimHashedString hash = new KAnimHashedString(reader.ReadInt32());
			KAnim.Build.Symbol symbol = new KAnim.Build.Symbol();
			symbol.build = build;
			symbol.hash = hash;
			if (num > 9)
			{
				symbol.path = new KAnimHashedString(reader.ReadInt32());
			}
			symbol.colourChannel = new KAnimHashedString(reader.ReadInt32());
			symbol.flags = reader.ReadInt32();
			symbol.firstFrameIdx = data.symbolFrameInstances.Count;
			symbol.numFrames = reader.ReadInt32();
			symbol.symbolIndexInSourceBuild = i;
			int num5 = 0;
			for (int j = 0; j < symbol.numFrames; j++)
			{
				KAnim.Build.SymbolFrame symbolFrame = new KAnim.Build.SymbolFrame();
				KAnim.Build.SymbolFrameInstance item = default(KAnim.Build.SymbolFrameInstance);
				item.symbolFrame = symbolFrame;
				symbolFrame.fileNameHash = fileNameHash;
				symbolFrame.sourceFrameNum = reader.ReadInt32();
				symbolFrame.duration = reader.ReadInt32();
				item.buildImageIdx = data.textureStartIndex[fileNameHash] + reader.ReadInt32();
				if (item.buildImageIdx >= textures.Count + data.textureStartIndex[fileNameHash])
				{
					Debug.LogErrorFormat("{0} Symbol: [{1}] tex count: [{2}] buildImageIdx: [{3}] group total [{4}]", fileNameHash.ToString(), symbol.hash, textures.Count, item.buildImageIdx, data.textureStartIndex[fileNameHash]);
				}
				item.symbolIdx = data.GetSymbolCount();
				num5 = Math.Max(symbolFrame.sourceFrameNum + symbolFrame.duration, num5);
				float num6 = reader.ReadSingle();
				float num7 = reader.ReadSingle();
				float num8 = reader.ReadSingle();
				float num9 = reader.ReadSingle();
				symbolFrame.bboxMin = new Vector2(num6 - num8 * 0.5f, num7 - num9 * 0.5f);
				symbolFrame.bboxMax = new Vector2(num6 + num8 * 0.5f, num7 + num9 * 0.5f);
				float x = reader.ReadSingle();
				float num10 = reader.ReadSingle();
				float x2 = reader.ReadSingle();
				float num11 = reader.ReadSingle();
				symbolFrame.uvMin = new Vector2(x, 1f - num10);
				symbolFrame.uvMax = new Vector2(x2, 1f - num11);
				build.frames[num4] = symbolFrame;
				data.symbolFrameInstances.Add(item);
				num4++;
			}
			symbol.numLookupFrames = num5;
			data.AddBuildSymbol(symbol);
			build.symbols[i] = symbol;
		}
		ParseHashTable(reader);
		return build.index;
	}

	public static void PostParse(KBatchGroupData data)
	{
		for (int i = 0; i < data.GetSymbolCount(); i++)
		{
			KAnim.Build.Symbol symbol = data.GetSymbol(i);
			if (symbol == null)
			{
				Debug.LogWarning("Symbol null for [" + data.groupID + "] idx: [" + i + "]");
			}
			else
			{
				if (symbol.numLookupFrames <= 0)
				{
					int num = symbol.numFrames;
					for (int j = symbol.firstFrameIdx; j < symbol.firstFrameIdx + symbol.numFrames; j++)
					{
						KAnim.Build.SymbolFrameInstance symbolFrameInstance = data.GetSymbolFrameInstance(j);
						num = Mathf.Max(num, symbolFrameInstance.symbolFrame.sourceFrameNum + symbolFrameInstance.symbolFrame.duration);
					}
					symbol.numLookupFrames = num;
				}
				symbol.frameLookup = new int[symbol.numLookupFrames];
				if (symbol.numLookupFrames <= 0)
				{
					Debug.LogWarning("No lookup frames for  [" + data.groupID + "] build: [" + symbol.build.name + "] idx: [" + i + "] id: [" + symbol.hash + "]");
				}
				else
				{
					for (int k = 0; k < symbol.numLookupFrames; k++)
					{
						symbol.frameLookup[k] = -1;
					}
					for (int l = symbol.firstFrameIdx; l < symbol.firstFrameIdx + symbol.numFrames; l++)
					{
						KAnim.Build.SymbolFrameInstance symbolFrameInstance2 = data.GetSymbolFrameInstance(l);
						if (symbolFrameInstance2.symbolFrame == null)
						{
							Debug.LogWarning("No symbol frame  [" + data.groupID + "] symFrameIdx: [" + l + "] id: [" + symbol.hash + "]");
						}
						else
						{
							for (int m = symbolFrameInstance2.symbolFrame.sourceFrameNum; m < symbolFrameInstance2.symbolFrame.sourceFrameNum + symbolFrameInstance2.symbolFrame.duration; m++)
							{
								if (m >= symbol.frameLookup.Length)
								{
									Debug.LogWarning("Too many lookup frames [" + m + ">=" + symbol.frameLookup.Length + "] for  [" + data.groupID + "] idx: [" + i + "] id: [" + symbol.hash + "]");
								}
								else
								{
									symbol.frameLookup[m] = l;
								}
							}
						}
					}
					string text = HashCache.Get().Get(symbol.path);
					if (!string.IsNullOrEmpty(text))
					{
						int num2 = text.IndexOf("/");
						if (num2 != -1)
						{
							string text2 = text.Substring(0, num2);
							symbol.folder = new KAnimHashedString(text2);
							HashCache.Get().Add(symbol.folder.HashValue, text2);
						}
					}
				}
			}
		}
	}

	private static void Assert(bool condition, string message)
	{
		if (!condition)
		{
			throw new Exception(message);
		}
	}

	private static void CheckHeader(string header, FastReader reader)
	{
		char[] array = reader.ReadChars(header.Length);
		int num = 0;
		while (true)
		{
			if (num >= header.Length)
			{
				return;
			}
			if (array[num] != header[num])
			{
				break;
			}
			num++;
		}
		throw new Exception("Expected " + header);
	}
}
