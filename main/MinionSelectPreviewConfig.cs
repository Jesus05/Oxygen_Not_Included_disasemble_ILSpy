using Klei.AI;
using UnityEngine;
using UnityEngine.UI;

public class MinionSelectPreviewConfig : IEntityConfig
{
	public static string ID = "MinionSelectPreview";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID, true);
		RectTransform rectTransform = gameObject.AddOrGet<RectTransform>();
		rectTransform.anchorMin = new Vector2(0f, 0f);
		rectTransform.anchorMax = new Vector2(1f, 1f);
		rectTransform.pivot = new Vector2(0.5f, 0f);
		rectTransform.anchoredPosition = new Vector2(0f, 0f);
		rectTransform.sizeDelta = new Vector2(0f, 0f);
		LayoutElement layoutElement = gameObject.AddOrGet<LayoutElement>();
		layoutElement.preferredHeight = 100f;
		layoutElement.preferredWidth = 100f;
		gameObject.AddOrGet<Effects>();
		gameObject.AddOrGet<Traits>();
		MinionModifiers minionModifiers = gameObject.AddOrGet<MinionModifiers>();
		minionModifiers.initialTraits = new string[1]
		{
			MinionConfig.MINION_BASE_TRAIT_ID
		};
		MinionConfig.AddMinionAmounts(minionModifiers);
		gameObject.AddOrGet<AttributeLevels>();
		gameObject.AddOrGet<AttributeConverters>();
		MinionIdentity minionIdentity = gameObject.AddOrGet<MinionIdentity>();
		minionIdentity.addToIdentityList = false;
		BoxCollider2D boxCollider2D = gameObject.AddOrGet<BoxCollider2D>();
		boxCollider2D.size = new Vector2(1f, 1f);
		gameObject.AddOrGet<FaceGraph>();
		gameObject.AddOrGet<Accessorizer>();
		KBatchedAnimController kBatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.materialType = KAnimBatchGroup.MaterialType.UI;
		kBatchedAnimController.animScale = 0.5f;
		kBatchedAnimController.setScaleFromAnim = false;
		kBatchedAnimController.animOverrideSize = new Vector2(100f, 120f);
		kBatchedAnimController.AnimFiles = new KAnimFile[4]
		{
			Assets.GetAnim("body_comp_default_kanim"),
			Assets.GetAnim("anim_construction_default_kanim"),
			Assets.GetAnim("anim_idles_default_kanim"),
			Assets.GetAnim("anim_cheer_kanim")
		};
		SymbolOverrideControllerUtil.AddToPrefab(gameObject);
		MinionConfig.ConfigureSymbols(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}
}
