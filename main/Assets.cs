using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Assets : KMonoBehaviour, ISerializationCallbackReceiver
{
	[Serializable]
	public struct UIPrefabData
	{
		public ProgressBar ProgressBar;

		public HealthBar HealthBar;

		public GameObject ResourceVisualizer;

		public Image RegionCellBlocked;

		public RectTransform PriorityOverlayIcon;

		public RectTransform HarvestWhenReadyOverlayIcon;

		public TableScreenAssets TableScreenWidgets;
	}

	[Serializable]
	public struct TableScreenAssets
	{
		public Material DefaultUIMaterial;

		public Material DesaturatedUIMaterial;

		public GameObject MinionPortrait;

		public GameObject GenericPortrait;

		public GameObject TogglePortrait;

		public GameObject ButtonLabel;

		public GameObject ButtonLabelWhite;

		public GameObject Label;

		public GameObject LabelHeader;

		public GameObject Checkbox;

		public GameObject BlankCell;

		public GameObject SuperCheckbox_Horizontal;

		public GameObject SuperCheckbox_Vertical;

		public GameObject Spacer;

		public GameObject NumericDropDown;

		public GameObject DropDownHeader;

		public GameObject PriorityGroupSelector;

		public GameObject PriorityGroupSelectorHeader;

		public GameObject PrioritizeRowWidget;

		public GameObject PrioritizeRowHeaderWidget;
	}

	private static Action<KPrefabID> OnAddPrefab;

	public static BuildingDef[] BuildingDefs;

	public List<KPrefabID> PrefabAssets = new List<KPrefabID>();

	public static List<KPrefabID> Prefabs = new List<KPrefabID>();

	private static HashSet<Tag> CountableTags = new HashSet<Tag>();

	public Sprite[] SpriteAssets;

	public static Dictionary<HashedString, Sprite> Sprites;

	public TintedSprite[] TintedSpriteAssets;

	public static TintedSprite[] TintedSprites;

	public Texture2D[] TextureAssets;

	public static Texture2D[] Textures;

	public static TextureAtlas[] TextureAtlases;

	public TextureAtlas[] TextureAtlasAssets;

	public static Material[] Materials;

	public Material[] MaterialAssets;

	public static Shader[] Shaders;

	public Shader[] ShaderAssets;

	public static BlockTileDecorInfo[] BlockTileDecorInfos;

	public BlockTileDecorInfo[] BlockTileDecorInfoAssets;

	public Material AnimMaterialAsset;

	public static Material AnimMaterial;

	public DiseaseVisualization DiseaseVisualization;

	public Sprite LegendColourBox;

	public Texture2D invalidAreaTex;

	public UIPrefabData UIPrefabAssets;

	public static UIPrefabData UIPrefabs;

	private static Dictionary<Tag, KPrefabID> PrefabsByTag = new Dictionary<Tag, KPrefabID>();

	private static Dictionary<Tag, List<KPrefabID>> PrefabsByAdditionalTags = new Dictionary<Tag, List<KPrefabID>>();

	private static Dictionary<HashedString, KAnimFile> AnimTable = new Dictionary<HashedString, KAnimFile>();

	public KAnimFile[] AnimAssets;

	public static KAnimFile[] Anims;

	public Font DebugFontAsset;

	public static Font DebugFont;

	public SubstanceTable substanceTable;

	public static SubstanceTable SubstanceTable;

	[SerializeField]
	public TextAsset elementsFile;

	[SerializeField]
	public TextAsset elementAudio;

	[SerializeField]
	public TextAsset personalitiesFile;

	public LogicModeUI logicModeUIData;

	public CommonPlacerConfig.CommonPlacerAssets commonPlacerAssets;

	public DigPlacerConfig.DigPlacerAssets digPlacerAssets;

	public MopPlacerConfig.MopPlacerAssets mopPlacerAssets;

	public static Assets instance;

	private static Dictionary<string, string> simpleSoundEventNames = new Dictionary<string, string>();

	protected override void OnPrefabInit()
	{
		instance = this;
		if (KPlayerPrefs.HasKey("TemperatureUnit"))
		{
			GameUtil.temperatureUnit = (GameUtil.TemperatureUnit)KPlayerPrefs.GetInt("TemperatureUnit");
		}
		if (KPlayerPrefs.HasKey("MassUnit"))
		{
			GameUtil.massUnit = (GameUtil.MassUnit)KPlayerPrefs.GetInt("MassUnit");
		}
		RecipeManager.DestroyInstance();
		RecipeManager.Get();
		AnimMaterial = AnimMaterialAsset;
		Prefabs = new List<KPrefabID>(from x in PrefabAssets
		where (UnityEngine.Object)x != (UnityEngine.Object)null
		select x);
		PrefabsByTag.Clear();
		PrefabsByAdditionalTags.Clear();
		CountableTags.Clear();
		Sprites = new Dictionary<HashedString, Sprite>();
		Sprite[] spriteAssets = SpriteAssets;
		foreach (Sprite sprite in spriteAssets)
		{
			if (!((UnityEngine.Object)sprite == (UnityEngine.Object)null))
			{
				HashedString key = new HashedString(sprite.name);
				Sprites.Add(key, sprite);
			}
		}
		TintedSprites = (from x in TintedSpriteAssets
		where x != null && (UnityEngine.Object)x.sprite != (UnityEngine.Object)null
		select x).ToArray();
		Materials = (from x in MaterialAssets
		where (UnityEngine.Object)x != (UnityEngine.Object)null
		select x).ToArray();
		Textures = (from x in TextureAssets
		where (UnityEngine.Object)x != (UnityEngine.Object)null
		select x).ToArray();
		TextureAtlases = (from x in TextureAtlasAssets
		where (UnityEngine.Object)x != (UnityEngine.Object)null
		select x).ToArray();
		BlockTileDecorInfos = (from x in BlockTileDecorInfoAssets
		where (UnityEngine.Object)x != (UnityEngine.Object)null
		select x).ToArray();
		Anims = (from x in AnimAssets
		where (UnityEngine.Object)x != (UnityEngine.Object)null
		select x).ToArray();
		UIPrefabs = UIPrefabAssets;
		DebugFont = DebugFontAsset;
		AsyncLoadManager<IGlobalAsyncLoader>.Run();
		GameAudioSheets.Get().Initialize();
		SubstanceListHookup();
		BuildingDefs = new BuildingDef[0];
		foreach (KPrefabID prefabAsset in PrefabAssets)
		{
			if (!((UnityEngine.Object)prefabAsset == (UnityEngine.Object)null))
			{
				AddPrefab(prefabAsset);
			}
		}
		AnimTable.Clear();
		KAnimFile[] anims = Anims;
		foreach (KAnimFile kAnimFile in anims)
		{
			if ((UnityEngine.Object)kAnimFile != (UnityEngine.Object)null)
			{
				HashedString key2 = kAnimFile.name;
				AnimTable[key2] = kAnimFile;
			}
		}
		LegacyModMain.Load();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Db.Get();
	}

	private static void TryAddCountableTag(KPrefabID prefab)
	{
		foreach (Tag unitCategory in GameTags.UnitCategories)
		{
			if (prefab.HasTag(unitCategory))
			{
				AddCountableTag(prefab.PrefabTag);
				break;
			}
		}
	}

	public static void AddCountableTag(Tag tag)
	{
		CountableTags.Add(tag);
	}

	public static bool IsTagCountable(Tag tag)
	{
		return CountableTags.Contains(tag);
	}

	private void SubstanceListHookup()
	{
		Hashtable substanceList = new Hashtable();
		ElementsAudio.Instance.LoadData(AsyncLoadManager<IGlobalAsyncLoader>.AsyncLoader<ElementAudioFileLoader>.Get().entries);
		ElementLoader.Load(ref substanceList, elementsFile.text, substanceTable);
		SubstanceTable = substanceTable;
	}

	public static string GetSimpleSoundEventName(string path)
	{
		string value = null;
		if (!simpleSoundEventNames.TryGetValue(path, out value))
		{
			int num = path.LastIndexOf('/');
			value = ((num == -1) ? path : path.Substring(num + 1));
			simpleSoundEventNames[path] = value;
		}
		return value;
	}

	private static Def GetDef(Def[] defs, string prefab_id)
	{
		int num = defs.Length;
		for (int i = 0; i < num; i++)
		{
			if (defs[i].PrefabID == prefab_id)
			{
				return defs[i];
			}
		}
		return null;
	}

	public static BuildingDef GetBuildingDef(string prefab_id)
	{
		return (BuildingDef)GetDef(BuildingDefs, prefab_id);
	}

	public static TintedSprite GetTintedSprite(string name)
	{
		TintedSprite result = null;
		if (TintedSprites != null)
		{
			for (int i = 0; i < TintedSprites.Length; i++)
			{
				if (TintedSprites[i].sprite.name == name)
				{
					result = TintedSprites[i];
					break;
				}
			}
		}
		return result;
	}

	public static Sprite GetSprite(HashedString name)
	{
		Sprite value = null;
		Sprites.TryGetValue(name, out value);
		return value;
	}

	public static Texture2D GetTexture(string name)
	{
		Texture2D result = null;
		if (Textures != null)
		{
			for (int i = 0; i < Textures.Length; i++)
			{
				if (Textures[i].name == name)
				{
					result = Textures[i];
					break;
				}
			}
		}
		return result;
	}

	public static void AddPrefab(KPrefabID prefab)
	{
		if (!((UnityEngine.Object)prefab == (UnityEngine.Object)null))
		{
			prefab.UpdateSaveLoadTag();
			if (PrefabsByTag.ContainsKey(prefab.PrefabTag))
			{
				Debug.LogWarning("Tried loading prefab with duplicate tag, ignoring: " + prefab.PrefabTag, null);
			}
			PrefabsByTag[prefab.PrefabTag] = prefab;
			foreach (Tag tag in prefab.Tags)
			{
				if (!PrefabsByAdditionalTags.ContainsKey(tag))
				{
					PrefabsByAdditionalTags[tag] = new List<KPrefabID>();
				}
				PrefabsByAdditionalTags[tag].Add(prefab);
			}
			Prefabs.Add(prefab);
			TryAddCountableTag(prefab);
			if (OnAddPrefab != null)
			{
				OnAddPrefab(prefab);
			}
		}
	}

	public static void RegisterOnAddPrefab(Action<KPrefabID> on_add)
	{
		OnAddPrefab = (Action<KPrefabID>)Delegate.Combine(OnAddPrefab, on_add);
		foreach (KPrefabID prefab in Prefabs)
		{
			on_add(prefab);
		}
	}

	public static void UnregisterOnAddPrefab(Action<KPrefabID> on_add)
	{
		OnAddPrefab = (Action<KPrefabID>)Delegate.Remove(OnAddPrefab, on_add);
	}

	public static void ClearOnAddPrefab()
	{
		OnAddPrefab = null;
	}

	public static GameObject GetPrefab(Tag tag)
	{
		GameObject gameObject = TryGetPrefab(tag);
		if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
		{
			Debug.LogWarning("Missing prefab: " + tag, null);
		}
		return gameObject;
	}

	public static GameObject TryGetPrefab(Tag tag)
	{
		KPrefabID value = null;
		PrefabsByTag.TryGetValue(tag, out value);
		return (!((UnityEngine.Object)value != (UnityEngine.Object)null)) ? null : value.gameObject;
	}

	public static List<GameObject> GetPrefabsWithTag(Tag tag)
	{
		List<GameObject> list = new List<GameObject>();
		if (PrefabsByAdditionalTags.ContainsKey(tag))
		{
			for (int i = 0; i < PrefabsByAdditionalTags[tag].Count; i++)
			{
				list.Add(PrefabsByAdditionalTags[tag][i].gameObject);
			}
		}
		return list;
	}

	public static List<GameObject> GetPrefabsWithComponent<Type>()
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < Prefabs.Count; i++)
		{
			if (Prefabs[i].GetComponent<Type>() != null)
			{
				list.Add(Prefabs[i].gameObject);
			}
		}
		return list;
	}

	public static List<Tag> GetPrefabTagsWithComponent<Type>()
	{
		List<Tag> list = new List<Tag>();
		for (int i = 0; i < Prefabs.Count; i++)
		{
			if (Prefabs[i].GetComponent<Type>() != null)
			{
				list.Add(Prefabs[i].PrefabID());
			}
		}
		return list;
	}

	public static Assets GetInstanceEditorOnly()
	{
		Assets[] array = (Assets[])Resources.FindObjectsOfTypeAll(typeof(Assets));
		if (array == null || array.Length == 0)
		{
			return array[0];
		}
		return array[0];
	}

	public static TextureAtlas GetTextureAtlas(string name)
	{
		TextureAtlas[] textureAtlases = TextureAtlases;
		foreach (TextureAtlas textureAtlas in textureAtlases)
		{
			if (textureAtlas.name == name)
			{
				return textureAtlas;
			}
		}
		return null;
	}

	public static Material GetMaterial(string name)
	{
		Material[] materials = Materials;
		foreach (Material material in materials)
		{
			if (material.name == name)
			{
				return material;
			}
		}
		return null;
	}

	public static BlockTileDecorInfo GetBlockTileDecorInfo(string name)
	{
		BlockTileDecorInfo[] blockTileDecorInfos = BlockTileDecorInfos;
		foreach (BlockTileDecorInfo blockTileDecorInfo in blockTileDecorInfos)
		{
			if (blockTileDecorInfo.name == name)
			{
				return blockTileDecorInfo;
			}
		}
		Output.LogError("Could not find BlockTileDecorInfo named [" + name + "]");
		return null;
	}

	public static KAnimFile GetAnim(HashedString name)
	{
		if (!name.IsValid)
		{
			Debug.LogWarning("Invalid hash name", null);
			return null;
		}
		KAnimFile value = null;
		AnimTable.TryGetValue(name, out value);
		if ((UnityEngine.Object)value == (UnityEngine.Object)null)
		{
			Debug.LogWarning("Missing Anim: [" + name.ToString() + "]. You may have to run Collect Anim on the Assets prefab", null);
		}
		return value;
	}

	public void OnAfterDeserialize()
	{
		TintedSpriteAssets = (from x in TintedSpriteAssets
		where x != null && (UnityEngine.Object)x.sprite != (UnityEngine.Object)null
		select x).ToArray();
		Array.Sort(TintedSpriteAssets, (TintedSprite a, TintedSprite b) => a.name.CompareTo(b.name));
	}

	public void OnBeforeSerialize()
	{
	}

	public static void AddBuildingDef(BuildingDef def)
	{
		BuildingDefs = (from x in BuildingDefs
		where x.PrefabID != def.PrefabID
		select x).ToArray();
		BuildingDefs = BuildingDefs.Append(def);
	}
}
