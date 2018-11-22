using UnityEngine;

public class HelmetController : KMonoBehaviour
{
	public string anim_file = "helm_oxygen_kanim";

	public bool has_jets = false;

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
		KBatchedAnimController result = null;
		Equippable component = GetComponent<Equippable>();
		Transform transform = null;
		if (component.assignee != null)
		{
			transform = ((!(component.assignee is MinionIdentity)) ? (component.assignee as MinionAssignablesProxy).GetTargetGameObject().transform : (component.assignee as MinionIdentity).transform);
			result = transform.GetComponent<KBatchedAnimController>();
		}
		return result;
	}

	private void OnEquipped(object data)
	{
		Equippable component = GetComponent<Equippable>();
		ShowHelmet();
		GameObject gameObject = (!(component.assignee is MinionIdentity)) ? (component.assignee as MinionAssignablesProxy).GetTargetGameObject() : (component.assignee as MinionIdentity).gameObject;
		gameObject.Subscribe(961737054, OnBeginRecoverBreath);
		gameObject.Subscribe(-2037519664, OnEndRecoverBreath);
		gameObject.Subscribe(1347184327, OnPathAdvanced);
		in_tube = false;
		is_flying = false;
		owner_navigator = gameObject.GetComponent<Navigator>();
	}

	private void OnUnequipped(object data)
	{
		owner_navigator = null;
		Equippable component = GetComponent<Equippable>();
		if ((Object)component != (Object)null)
		{
			HideHelmet();
			IAssignableIdentity assignee = component.assignee;
			if (assignee != null)
			{
				GameObject go = (!(component.assignee is MinionIdentity)) ? (component.assignee as MinionAssignablesProxy).GetTargetGameObject() : (component.assignee as MinionIdentity).gameObject;
				go.Unsubscribe(961737054, OnBeginRecoverBreath);
				go.Unsubscribe(-2037519664, OnEndRecoverBreath);
				go.Unsubscribe(1347184327, OnPathAdvanced);
			}
		}
	}

	private void ShowHelmet()
	{
		KBatchedAnimController assigneeController = GetAssigneeController();
		if (!((Object)assigneeController == (Object)null))
		{
			KAnimFile anim = Assets.GetAnim(anim_file);
			KAnimHashedString kAnimHashedString = new KAnimHashedString("snapTo_neck");
			assigneeController.GetComponent<SymbolOverrideController>().AddSymbolOverride(kAnimHashedString, anim.GetData().build.GetSymbol(kAnimHashedString), 6);
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
			SymbolOverrideController component = assigneeController.GetComponent<SymbolOverrideController>();
			if (!((Object)component == (Object)null))
			{
				component.RemoveSymbolOverride(kAnimHashedString, 6);
				assigneeController.SetSymbolVisiblity(kAnimHashedString, false);
				UpdateJets();
			}
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

	private GameObject AddTrackedAnim(string name, KAnimFile anim_file, string anim_clip, Grid.SceneLayer layer, string symbol_name)
	{
		KBatchedAnimController assigneeController = GetAssigneeController();
		if (!((Object)assigneeController == (Object)null))
		{
			string name2 = assigneeController.name + "." + name;
			GameObject gameObject = new GameObject(name2);
			gameObject.SetActive(false);
			gameObject.transform.parent = assigneeController.transform;
			KPrefabID kPrefabID = gameObject.AddComponent<KPrefabID>();
			kPrefabID.PrefabTag = new Tag(name2);
			KBatchedAnimController kBatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
			kBatchedAnimController.AnimFiles = new KAnimFile[1]
			{
				anim_file
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
		return null;
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
