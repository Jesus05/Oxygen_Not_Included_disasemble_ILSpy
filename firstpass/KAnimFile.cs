using System.Collections.Generic;
using UnityEngine;

public class KAnimFile : ScriptableObject
{
	public class Mod
	{
		public byte[] anim;

		public byte[] build;

		public List<Texture2D> textures = new List<Texture2D>();

		public bool IsValid()
		{
			return anim != null;
		}
	}

	public const string ANIM_ROOT_PATH = "Assets/anim";

	[SerializeField]
	private TextAsset animFile;

	[SerializeField]
	private TextAsset buildFile;

	[SerializeField]
	private List<Texture2D> textures = new List<Texture2D>();

	public Mod mod;

	private KAnimFileData data;

	private HashedString _batchTag;

	public string homedirectory = string.Empty;

	public byte[] animBytes => (mod != null) ? mod.anim : ((!((Object)animFile != (Object)null)) ? null : animFile.bytes);

	public byte[] buildBytes => (mod != null) ? mod.build : ((!((Object)buildFile != (Object)null)) ? null : buildFile.bytes);

	public List<Texture2D> textureList => (mod != null) ? mod.textures : textures;

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

	public void Initialize(TextAsset anim, TextAsset build, IList<Texture2D> textures)
	{
		animFile = anim;
		buildFile = build;
		this.textures.AddRange(textures);
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
