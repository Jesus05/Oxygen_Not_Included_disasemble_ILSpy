using System.Diagnostics;

[DebuggerDisplay("{name}")]
public class KAnimFileData
{
	public const int NO_RECORD = -1;

	public int index;

	public HashedString batchTag;

	public int buildIndex;

	public HashedString animBatchTag;

	public int firstAnimIndex;

	public int animCount;

	public int frameCount;

	public int firstElementIndex;

	public int elementCount;

	public int maxVisSymbolFrames;

	public string name
	{
		get;
		private set;
	}

	public KAnimHashedString hashName
	{
		get;
		private set;
	}

	public KAnim.Build build
	{
		get
		{
			if (buildIndex == -1)
			{
				return null;
			}
			KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(batchTag);
			if (batchGroupData == null)
			{
				Debug.LogErrorFormat("[{0}] No such batch group [{1}]", name, batchTag.ToString());
			}
			return batchGroupData.GetBuild(buildIndex);
		}
	}

	public KAnimFileData(string name)
	{
		this.name = name;
		firstAnimIndex = -1;
		buildIndex = -1;
		firstElementIndex = -1;
		animCount = 0;
		frameCount = 0;
		elementCount = 0;
		maxVisSymbolFrames = 0;
		hashName = new KAnimHashedString(name);
	}

	public KAnim.Anim GetAnim(int index)
	{
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(animBatchTag);
		if (batchGroupData == null)
		{
			Debug.LogError($"[{name}] No such batch group [{animBatchTag.ToString()}]", null);
		}
		return batchGroupData.GetAnim(index + firstAnimIndex);
	}

	public KAnim.Anim.FrameElement GetAnimFrameElement(int index)
	{
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(animBatchTag);
		if (batchGroupData == null)
		{
			Debug.LogErrorFormat("[{0}] No such batch group [{1}]", name, animBatchTag.ToString());
		}
		return batchGroupData.GetFrameElement(firstElementIndex + index);
	}

	public KAnim.Anim.FrameElement FindAnimFrameElement(KAnimHashedString symbolName)
	{
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(animBatchTag);
		return batchGroupData.frameElements.Find((KAnim.Anim.FrameElement match) => match.symbol == symbolName);
	}
}
