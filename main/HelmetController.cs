using UnityEngine;

public class HelmetController : KMonoBehaviour
{
	public string anim_file;

	public bool has_jets;

	private bool is_shown;

	private bool in_tube;

	private bool is_flying;

	private Navigator owner_navigator;

	private GameObject jet_go;

	private GameObject glow_go;

	private static readonly EventSystem.IntraObjectHandler<HelmetController> OnEquippedDelegate = new EventSystem.IntraObjectHandler<HelmetController>(delegate(HelmetController component, object data)
	{
		component.OnEquipped(data);
	});

	private static readonly EventSystem.IntraObjectHandler<HelmetController> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<HelmetController>(delegate(HelmetController component, object data)
	{
		component.OnUnequipped(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1617557748, OnEquippedDelegate);
		Subscribe(-170173755, OnUnequippedDelegate);
	}

	private KBatchedAnimController GetAssigneeController()
	{
		Equippable component = GetComponent<Equippable>();
		if (component.assignee != null)
		{
			GameObject assigneeGameObject = GetAssigneeGameObject(component.assignee);
			if ((bool)assigneeGameObject)
			{
				return assigneeGameObject.GetComponent<KBatchedAnimController>();
			}
		}
		return null;
	}

	private GameObject GetAssigneeGameObject(IAssignableIdentity ass_id)
	{
		GameObject result = null;
		MinionAssignablesProxy minionAssignablesProxy = ass_id as MinionAssignablesProxy;
		if ((bool)minionAssignablesProxy)
		{
			result = minionAssignablesProxy.GetTargetGameObject();
		}
		else
		{
			MinionIdentity minionIdentity = ass_id as MinionIdentity;
			if ((bool)minionIdentity)
			{
				result = minionIdentity.gameObject;
			}
		}
		return result;
	}

	private void OnEquipped(object data)
	{
		Equippable component = GetComponent<Equippable>();
		ShowHelmet();
		GameObject assigneeGameObject = GetAssigneeGameObject(component.assignee);
		assigneeGameObject.Subscribe(961737054, OnBeginRecoverBreath);
		assigneeGameObject.Subscribe(-2037519664, OnEndRecoverBreath);
		assigneeGameObject.Subscribe(1347184327, OnPathAdvanced);
		in_tube = false;
		is_flying = false;
		owner_navigator = assigneeGameObject.GetComponent<Navigator>();
	}

	private void OnUnequipped(object data)
	{
		owner_navigator = null;
		Equippable component = GetComponent<Equippable>();
		if ((Object)component != (Object)null)
		{
			HideHelmet();
			if (component.assignee != null)
			{
				GameObject assigneeGameObject = GetAssigneeGameObject(component.assignee);
				if ((bool)assigneeGameObject)
				{
					assigneeGameObject.Unsubscribe(961737054, OnBeginRecoverBreath);
					assigneeGameObject.Unsubscribe(-2037519664, OnEndRecoverBreath);
					assigneeGameObject.Unsubscribe(1347184327, OnPathAdvanced);
				}
			}
		}
	}

	private void ShowHelmet()
	{
		KBatchedAnimController assigneeController = GetAssigneeController();
		if (!((Object)assigneeController == (Object)null))
		{
			KAnimHashedString kAnimHashedString = new KAnimHashedString("snapTo_neck");
			if (!string.IsNullOrEmpty(anim_file))
			{
				KAnimFile anim = Assets.GetAnim(anim_file);
				assigneeController.GetComponent<SymbolOverrideController>().AddSymbolOverride(kAnimHashedString, anim.GetData().build.GetSymbol(kAnimHashedString), 6);
			}
			assigneeController.SetSymbolVisiblity(kAnimHashedString, true);
			is_shown = true;
			UpdateJets();
		}
	}

	private void HideHelmet()
	{
		is_shown = false;
		KBatchedAnimController assigneeController = GetAssigneeController();
		if (!((Object)assigneeController == (Object)null))
		{
			KAnimHashedString kAnimHashedString = "snapTo_neck";
			if (!string.IsNullOrEmpty(anim_file))
			{
				SymbolOverrideController component = assigneeController.GetComponent<SymbolOverrideController>();
				if ((Object)component == (Object)null)
				{
					return;
				}
				component.RemoveSymbolOverride(kAnimHashedString, 6);
			}
			assigneeController.SetSymbolVisiblity(kAnimHashedString, false);
			UpdateJets();
		}
	}

	private void UpdateJets()
	{
		if (is_shown && is_flying)
		{
			EnableJets();
		}
		else
		{
			DisableJets();
		}
	}

	private void EnableJets()
	{
		if (has_jets && !(bool)jet_go)
		{
			jet_go = AddTrackedAnim("jet", Assets.GetAnim("jetsuit_thruster_fx_kanim"), "loop", Grid.SceneLayer.Creatures, "snapTo_neck");
			glow_go = AddTrackedAnim("glow", Assets.GetAnim("jetsuit_thruster_glow_fx_kanim"), "loop", Grid.SceneLayer.Front, "snapTo_neck");
		}
	}

	private void DisableJets()
	{
		if (has_jets)
		{
			Object.Destroy(jet_go);
			jet_go = null;
			Object.Destroy(glow_go);
			glow_go = null;
		}
	}

	private GameObject AddTrackedAnim(string name, KAnimFile tracked_anim_file, string anim_clip, Grid.SceneLayer layer, string symbol_name)
	{
		KBatchedAnimController assigneeController = GetAssigneeController();
		if ((Object)assigneeController == (Object)null)
		{
			return null;
		}
		string name2 = assigneeController.name + "." + name;
		GameObject gameObject = new GameObject(name2);
		gameObject.SetActive(false);
		gameObject.transform.parent = assigneeController.transform;
		KPrefabID kPrefabID = gameObject.AddComponent<KPrefabID>();
		kPrefabID.PrefabTag = new Tag(name2);
		KBatchedAnimController kBatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kBatchedAnimController.AnimFiles = new KAnimFile[1]
		{
			tracked_anim_file
		};
		kBatchedAnimController.initialAnim = anim_clip;
		kBatchedAnimController.isMovable = true;
		kBatchedAnimController.sceneLayer = layer;
		KBatchedAnimTracker kBatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
		kBatchedAnimTracker.symbol = symbol_name;
		bool symbolVisible;
		Vector4 column = assigneeController.GetSymbolTransform(symbol_name, out symbolVisible).GetColumn(3);
		Vector3 position = column;
		position.z = Grid.GetLayerZ(layer);
		gameObject.transform.SetPosition(position);
		gameObject.SetActive(true);
		kBatchedAnimController.Play(anim_clip, KAnim.PlayMode.Loop, 1f, 0f);
		return gameObject;
	}

	private void OnBeginRecoverBreath(object data)
	{
		HideHelmet();
	}

	private void OnEndRecoverBreath(object data)
	{
		ShowHelmet();
	}

	private void OnPathAdvanced(object data)
	{
		if (!((Object)owner_navigator == (Object)null))
		{
			bool flag = owner_navigator.CurrentNavType == NavType.Hover;
			bool flag2 = owner_navigator.CurrentNavType == NavType.Tube;
			if (flag2 != in_tube)
			{
				in_tube = flag2;
				if (in_tube)
				{
					HideHelmet();
				}
				else
				{
					ShowHelmet();
				}
			}
			if (flag != is_flying)
			{
				is_flying = flag;
				UpdateJets();
			}
		}
	}
}
