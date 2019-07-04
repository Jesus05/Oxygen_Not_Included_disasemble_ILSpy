using System;
using System.Diagnostics;
using UnityEngine;

public class KAnim
{
	public enum PlayMode
	{
		Loop,
		Once,
		Paused
	}

	public enum LayerFlags
	{
		FG = 1
	}

	public enum SymbolFlags
	{
		Bloom = 1,
		OnLight = 2,
		SnapTo = 4,
		FG = 8
	}

	[Serializable]
	public struct AnimHashTable
	{
		public KAnimHashedString[] hashes;
	}

	[Serializable]
	[DebuggerDisplay("{id} {animFile}")]
	public class Anim
	{
		[Serializable]
		public struct Frame
		{
			public AABB3 bbox;

			public int firstElementIdx;

			public int idx;

			public int numElements;

			public static readonly Frame InvalidFrame = new Frame
			{
				idx = -1
			};

			public bool IsValid()
			{
				return idx != -1;
			}

			public static bool operator ==(Frame a, Frame b)
			{
				return a.idx == b.idx;
			}

			public static bool operator !=(Frame a, Frame b)
			{
				return a.idx != b.idx;
			}

			public override bool Equals(object obj)
			{
				Frame frame = (Frame)obj;
				return idx == frame.idx;
			}

			public override int GetHashCode()
			{
				return idx;
			}
		}

		[Serializable]
		public struct FrameElement
		{
			public KAnimHashedString fileHash;

			public KAnimHashedString symbol;

			public int symbolIdx;

			public KAnimHashedString folder;

			public int frame;

			public Matrix2x3 transform;

			public Color multColour;

			public int flags;
		}

		public string name;

		public HashedString id;

		public float frameRate;

		public int firstFrameIdx;

		public int numFrames;

		public HashedString rootSymbol;

		public HashedString hash;

		public float totalTime;

		public float scaledBoundingRadius;

		public Vector2 unScaledSize = Vector2.zero;

		public int index
		{
			get;
			private set;
		}

		public KAnimFileData animFile
		{
			get;
			private set;
		}

		public Anim(KAnimFileData anim_file, int idx)
		{
			animFile = anim_file;
			index = idx;
		}

		public int GetFrameIdx(PlayMode mode, float t)
		{
			if (numFrames > 0)
			{
				int result = 0;
				switch (mode)
				{
				case PlayMode.Loop:
					t %= totalTime;
					break;
				}
				if (t > 0f)
				{
					float num = t * frameRate + 0.49999997f;
					result = Math.Min(numFrames - 1, (int)num);
				}
				return result;
			}
			return -1;
		}

		private static KBatchGroupData GetAnimBatchGroupData(KAnimFileData animFile)
		{
			if (!animFile.batchTag.IsValid)
			{
				Debug.LogErrorFormat("Invalid batchTag for anim [{0}]", animFile.name);
			}
			Debug.Assert(animFile.batchTag.IsValid, "Invalid batch tag");
			KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(animFile.batchTag);
			if (group == null)
			{
				Debug.LogErrorFormat("Null group for tag [{0}]", animFile.batchTag);
			}
			HashedString hashedString = animFile.batchTag;
			KBatchGroupData kBatchGroupData = null;
			if (group.renderType == KAnimBatchGroup.RendererType.DontRender || group.renderType == KAnimBatchGroup.RendererType.AnimOnly)
			{
				if (!group.swapTarget.IsValid)
				{
					Debug.LogErrorFormat("Invalid swap target for group [{0}]", group.id);
				}
				hashedString = group.swapTarget;
			}
			kBatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(hashedString);
			if (kBatchGroupData == null)
			{
				Debug.LogErrorFormat("Null batch group for tag [{0}]", hashedString);
			}
			return kBatchGroupData;
		}

		public Frame GetFrame(KAnimFileData animFile, PlayMode mode, float t)
		{
			int frameIdx = GetFrameIdx(mode, t);
			if (frameIdx >= 0 && animFile.batchTag.IsValid && animFile.batchTag != KAnimBatchManager.NO_BATCH)
			{
				return GetAnimBatchGroupData(animFile).GetFrame(firstFrameIdx + frameIdx);
			}
			return Frame.InvalidFrame;
		}

		public Frame GetFrame(HashedString batchTag, int idx)
		{
			KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(batchTag);
			return batchGroupData.GetFrame(idx + firstFrameIdx);
		}

		public Anim Copy()
		{
			Anim anim = new Anim(animFile, index);
			anim.name = name;
			anim.id = id;
			anim.hash = hash;
			anim.rootSymbol = rootSymbol;
			anim.frameRate = frameRate;
			anim.firstFrameIdx = firstFrameIdx;
			anim.numFrames = numFrames;
			anim.totalTime = totalTime;
			anim.scaledBoundingRadius = scaledBoundingRadius;
			anim.unScaledSize = unScaledSize;
			return anim;
		}
	}

	[Serializable]
	public class Build : ISerializationCallbackReceiver
	{
		[Serializable]
		public class SymbolFrame : IComparable<SymbolFrame>
		{
			public int sourceFrameNum;

			public int duration;

			public KAnimHashedString fileNameHash;

			public Vector2 uvMin;

			public Vector2 uvMax;

			public Vector2 bboxMin;

			public Vector2 bboxMax;

			public int CompareTo(SymbolFrame obj)
			{
				return sourceFrameNum.CompareTo(obj.sourceFrameNum);
			}
		}

		public struct SymbolFrameInstance
		{
			public SymbolFrame symbolFrame;

			public int buildImageIdx;

			public int symbolIdx;
		}

		[Serializable]
		[DebuggerDisplay("{hash} {path} {folder} {colourChannel}")]
		public class Symbol : IComparable
		{
			[NonSerialized]
			public Build build;

			public KAnimHashedString hash;

			public KAnimHashedString path;

			public KAnimHashedString folder;

			public KAnimHashedString colourChannel;

			public int flags;

			public int firstFrameIdx;

			public int numFrames;

			public int numLookupFrames;

			public int[] frameLookup;

			public int index;

			public int symbolIndexInSourceBuild;

			public int GetFrameIdx(int frame)
			{
				if (frameLookup == null)
				{
					Debug.LogErrorFormat("Cant get frame [{2}] because Symbol [{0}] for build [{1}] batch [{3}] has no frameLookup", hash.ToString(), build.name, frame, build.batchTag.ToString());
				}
				if (frameLookup.Length != 0 && frame < frameLookup.Length)
				{
					frame = Math.Min(frame, frameLookup.Length - 1);
					return frameLookup[frame];
				}
				return -1;
			}

			public bool HasFrame(int frame)
			{
				int frameIdx = GetFrameIdx(frame);
				if (frameIdx < 0)
				{
					return false;
				}
				return true;
			}

			public SymbolFrameInstance GetFrame(int frame)
			{
				int frameIdx = GetFrameIdx(frame);
				KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(build.batchTag);
				return batchGroupData.GetSymbolFrameInstance(frameIdx);
			}

			public int CompareTo(object obj)
			{
				if (obj != null)
				{
					if (obj.GetType() != typeof(HashedString))
					{
						Symbol symbol = (Symbol)obj;
						return hash.HashValue.CompareTo(symbol.hash.HashValue);
					}
					HashedString hashedString = (HashedString)obj;
					return hash.HashValue.CompareTo(hashedString.HashValue);
				}
				return 1;
			}

			public bool HasFlag(SymbolFlags flag)
			{
				return (flags & (int)flag) != 0;
			}

			public Symbol Copy()
			{
				Symbol symbol = new Symbol();
				symbol.hash = hash;
				symbol.path = path;
				symbol.folder = folder;
				symbol.colourChannel = colourChannel;
				symbol.flags = flags;
				symbol.firstFrameIdx = firstFrameIdx;
				symbol.numFrames = numFrames;
				symbol.numLookupFrames = numLookupFrames;
				symbol.frameLookup = new int[frameLookup.Length];
				symbol.symbolIndexInSourceBuild = symbolIndexInSourceBuild;
				Array.Copy(frameLookup, symbol.frameLookup, symbol.frameLookup.Length);
				return symbol;
			}
		}

		public KAnimHashedString fileHash;

		public int index;

		public string name;

		public HashedString batchTag;

		public int textureStartIdx;

		public int textureCount;

		public Symbol[] symbols;

		public SymbolFrame[] frames;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (symbols != null)
			{
				for (int i = 0; i < symbols.Length; i++)
				{
					symbols[i].build = this;
				}
			}
		}

		public Symbol GetSymbolByIndex(uint index)
		{
			if (index < symbols.Length)
			{
				return symbols[index];
			}
			return null;
		}

		public Texture2D GetTexture(int index)
		{
			if (index < 0 || index >= textureCount)
			{
				Debug.LogError("Invalid texture index:" + index);
			}
			KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(batchTag);
			return batchGroupData.GetTexure(textureStartIdx + index);
		}

		public int GetSymbolOffset(KAnimHashedString symbol_name)
		{
			for (int i = 0; i < symbols.Length; i++)
			{
				if (symbols[i].hash == symbol_name)
				{
					return i;
				}
			}
			return -1;
		}

		public Symbol GetSymbol(KAnimHashedString symbol_name)
		{
			for (int i = 0; i < symbols.Length; i++)
			{
				if (symbols[i].hash == symbol_name)
				{
					return symbols[i];
				}
			}
			return null;
		}

		public override string ToString()
		{
			return name;
		}
	}
}
