using System;
using System.Collections.Generic;
using UnityEngine;

public class SymbolOverrideController : KMonoBehaviour
{
	[Serializable]
	private struct SymbolEntry
	{
		public HashedString targetSymbol;

		[NonSerialized]
		public KAnim.Build.Symbol sourceSymbol;

		public HashedString sourceSymbolId;

		public HashedString sourceSymbolBatchTag;

		public int priority;
	}

	private struct SymbolToOverride
	{
		public KAnim.Build.Symbol sourceSymbol;

		public HashedString targetSymbol;

		public KBatchGroupData data;

		public int atlasIdx;
	}

	private struct BatchGroupInfo
	{
		public KAnim.Build build;

		public int atlasIdx;

		public KBatchGroupData data;
	}

	public bool applySymbolOverridesEveryFrame;

	[SerializeField]
	private List<SymbolEntry> symbolOverrides = new List<SymbolEntry>();

	private KAnimBatch.AtlasList atlases;

	private KBatchedAnimController animController;

	private FaceGraph faceGraph;

	private bool requiresSorting;

	public int version
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		animController = GetComponent<KBatchedAnimController>();
		DebugUtil.Assert((UnityEngine.Object)GetComponent<KBatchedAnimController>() != (UnityEngine.Object)null, "SymbolOverrideController requires KBatchedAnimController", string.Empty, string.Empty);
		DebugUtil.Assert(GetComponent<KBatchedAnimController>().usingNewSymbolOverrideSystem, "SymbolOverrideController requires usingNewSymbolOverrideSystem to be set to true. Try adding the component by calling: SymbolOverrideControllerUtil.AddToPrefab", string.Empty, string.Empty);
		for (int i = 0; i < symbolOverrides.Count; i++)
		{
			SymbolEntry value = symbolOverrides[i];
			value.sourceSymbol = KAnimBatchManager.Instance().GetBatchGroupData(value.sourceSymbolBatchTag).GetSymbol(value.sourceSymbolId);
			symbolOverrides[i] = value;
		}
		atlases = new KAnimBatch.AtlasList(0);
		faceGraph = GetComponent<FaceGraph>();
	}

	public void AddSymbolOverride(HashedString target_symbol, KAnim.Build.Symbol source_symbol, int priority = 0)
	{
		if (source_symbol == null)
		{
			throw new Exception("NULL source symbol when overriding: " + target_symbol.ToString());
		}
		SymbolEntry symbolEntry = default(SymbolEntry);
		symbolEntry.targetSymbol = target_symbol;
		symbolEntry.sourceSymbol = source_symbol;
		symbolEntry.sourceSymbolId = new HashedString(source_symbol.hash.HashValue);
		symbolEntry.sourceSymbolBatchTag = source_symbol.build.batchTag;
		symbolEntry.priority = priority;
		SymbolEntry symbolEntry2 = symbolEntry;
		int symbolOverrideIdx = GetSymbolOverrideIdx(target_symbol, priority);
		if (symbolOverrideIdx >= 0)
		{
			symbolOverrides[symbolOverrideIdx] = symbolEntry2;
		}
		else
		{
			symbolOverrides.Add(symbolEntry2);
		}
		MarkDirty();
	}

	public void RemoveSymbolOverride(HashedString target_symbol, int priority = 0)
	{
		for (int i = 0; i < symbolOverrides.Count; i++)
		{
			SymbolEntry symbolEntry = symbolOverrides[i];
			if (symbolEntry.targetSymbol == target_symbol && symbolEntry.priority == priority)
			{
				symbolOverrides.RemoveAt(i);
				break;
			}
		}
		MarkDirty();
	}

	public void RemoveAllSymbolOverrides(int priority = 0)
	{
		symbolOverrides.RemoveAll((SymbolEntry x) => x.priority >= priority);
		MarkDirty();
	}

	public int GetSymbolOverrideIdx(HashedString target_symbol, int priority = 0)
	{
		for (int i = 0; i < symbolOverrides.Count; i++)
		{
			SymbolEntry symbolEntry = symbolOverrides[i];
			if (symbolEntry.targetSymbol == target_symbol && symbolEntry.priority == priority)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetAtlasIdx(Texture2D atlas)
	{
		return atlases.GetAtlasIdx(atlas);
	}

	public void ApplyOverrides()
	{
		if (requiresSorting)
		{
			symbolOverrides.Sort((SymbolEntry x, SymbolEntry y) => x.priority - y.priority);
			requiresSorting = false;
		}
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		KAnimBatch batch = component.GetBatch();
		DebugUtil.Assert(batch != null, "Assert!", string.Empty, string.Empty);
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(component.batchGroupID);
		int count = batch.atlases.Count;
		atlases.Clear(count);
		ListPool<SymbolToOverride, SymbolOverrideController>.PooledList pooledList = ListPool<SymbolToOverride, SymbolOverrideController>.Allocate();
		ListPool<BatchGroupInfo, SymbolOverrideController>.PooledList pooledList2 = ListPool<BatchGroupInfo, SymbolOverrideController>.Allocate();
		foreach (SymbolEntry symbolOverride in symbolOverrides)
		{
			SymbolEntry current = symbolOverride;
			BatchGroupInfo item = default(BatchGroupInfo);
			foreach (BatchGroupInfo item2 in pooledList2)
			{
				BatchGroupInfo current2 = item2;
				if (current.sourceSymbol.build == current2.build)
				{
					item = current2;
				}
			}
			if (item.build == null)
			{
				BatchGroupInfo batchGroupInfo = default(BatchGroupInfo);
				batchGroupInfo.build = current.sourceSymbol.build;
				batchGroupInfo.data = KAnimBatchManager.Instance().GetBatchGroupData(current.sourceSymbol.build.batchTag);
				item = batchGroupInfo;
				Texture2D texture = current.sourceSymbol.build.GetTexture(0);
				int num = item.atlasIdx = atlases.Add(texture);
				pooledList2.Add(item);
			}
			pooledList.Add(new SymbolToOverride
			{
				sourceSymbol = current.sourceSymbol,
				targetSymbol = current.targetSymbol,
				data = item.data,
				atlasIdx = item.atlasIdx
			});
		}
		pooledList2.Recycle();
		foreach (SymbolToOverride item3 in pooledList)
		{
			SymbolToOverride current3 = item3;
			KAnim.Build.Symbol symbol = batchGroupData.GetSymbol(current3.targetSymbol);
			if (symbol != null)
			{
				KAnim.Build.Symbol sourceSymbol = current3.sourceSymbol;
				for (int i = 0; i < symbol.numFrames; i++)
				{
					int num2 = Math.Min(sourceSymbol.numFrames - 1, i);
					KAnim.Build.SymbolFrameInstance symbol_frame_instance = current3.data.symbolFrameInstances[sourceSymbol.firstFrameIdx + num2];
					symbol_frame_instance.buildImageIdx = current3.atlasIdx;
					component.SetSymbolOverride(symbol.firstFrameIdx + i, symbol_frame_instance);
				}
			}
		}
		pooledList.Recycle();
		if ((UnityEngine.Object)faceGraph != (UnityEngine.Object)null)
		{
			faceGraph.ApplyShape();
		}
	}

	public void ApplyAtlases()
	{
		KAnimBatch batch = animController.GetBatch();
		atlases.Apply(batch.matProperties);
	}

	public void MarkDirty()
	{
		if ((UnityEngine.Object)animController != (UnityEngine.Object)null)
		{
			animController.SetDirty();
		}
		version++;
		requiresSorting = true;
	}
}
