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

	public string homedirectory = string.Empty;

	public HashedString batchTag
	{
		get
		{
			if (_batchTag.IsValid)
			{
				return _batchTag;
			}
			if (homedirectory == null || homedirectory == string.Empty)
			{
				return KAnimBatchManager.NO_BATCH;
			}
			_batchTag = KAnimGroupFile.GetGroupFile().GetGroupForHomeDirectory(new HashedString(homedirectory));
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
