using System.Collections.Generic;
using UnityEngine;

public class KAnimFile : ScriptableObject
{
	public const string ANIM_ROOT_PATH = "Assets/anim";

	public TextAsset animFile;

	public TextAsset buildFile;

	public List<Texture2D> textures = new List<Texture2D>();

	private KAnimFileData data;

	private HashedString _batchTag;

	public string homedirectory = "";

	public HashedString batchTag
	{
		get
		{
			if (!_batchTag.IsValid)
			{
				if (homedirectory != null && !(homedirectory == ""))
				{
					_batchTag = KAnimGroupFile.GetGroupFile().GetGroupForHomeDirectory(new HashedString(homedirectory));
					return _batchTag;
				}
				return KAnimBatchManager.NO_BATCH;
			}
			return _batchTag;
		}
	}

	public KAnimFileData GetData()
	{
		if (data == null)
		{
			KGlobalAnimParser kGlobalAnimParser = KGlobalAnimParser.Get();
			if (kGlobalAnimParser != null)
			{
				data = kGlobalAnimParser.Load(this);
			}
		}
		return data;
	}
}
