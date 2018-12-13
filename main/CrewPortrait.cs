using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CrewPortrait : KMonoBehaviour
{
	public Image targetImage;

	public bool startTransparent;

	public bool useLabels = true;

	[SerializeField]
	public KBatchedAnimController controller;

	public float animScaleBase = 0.2f;

	public LocText duplicantName;

	public LocText duplicantJob;

	public LocText subTitle;

	public bool useDefaultExpression = true;

	private bool requiresRefresh;

	private bool areEventsRegistered;

	private static readonly HashedString snapTo_neck = new HashedString("snapTo_neck");

	private static readonly HashedString snapTo_pivot = new HashedString("snapTo_pivot");

	private static readonly HashedString snapTo_rgthand = new HashedString("snapTo_rgthand");

	private static readonly HashedString snapTo_chest = new HashedString("snapTo_chest");

	public IAssignableIdentity identityObject
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (startTransparent)
		{
			StartCoroutine(AlphaIn());
		}
		requiresRefresh = true;
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(RefreshScale));
	}

	private IEnumerator AlphaIn()
	{
		SetAlpha(0f);
		float i = 0f;
		if (i < 1f)
		{
			SetAlpha(i);
			yield return (object)0;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		SetAlpha(1f);
	}

	private void OnRoleChanged(object data)
	{
		if (!((UnityEngine.Object)controller == (UnityEngine.Object)null))
		{
			RefreshHat(identityObject, controller);
		}
	}

	private void RegisterEvents()
	{
		if (!areEventsRegistered)
		{
			KMonoBehaviour kMonoBehaviour = identityObject as KMonoBehaviour;
			if (!((UnityEngine.Object)kMonoBehaviour == (UnityEngine.Object)null))
			{
				kMonoBehaviour.Subscribe(540773776, OnRoleChanged);
				areEventsRegistered = true;
			}
		}
	}

	private void UnregisterEvents()
	{
		if (areEventsRegistered)
		{
			areEventsRegistered = false;
			KMonoBehaviour kMonoBehaviour = identityObject as KMonoBehaviour;
			if (!((UnityEngine.Object)kMonoBehaviour == (UnityEngine.Object)null))
			{
				kMonoBehaviour.Unsubscribe(540773776, OnRoleChanged);
			}
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		RegisterEvents();
		ForceRefresh();
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		UnregisterEvents();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		UnregisterEvents();
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Remove(instance.OnResize, new System.Action(RefreshScale));
	}

	public void SetIdentityObject(IAssignableIdentity identity, bool jobEnabled = true)
	{
		UnregisterEvents();
		identityObject = identity;
		RegisterEvents();
		targetImage.enabled = true;
		if (identityObject != null)
		{
			targetImage.enabled = false;
		}
		if ((useLabels && identity is MinionIdentity) || identity is MinionAssignablesProxy)
		{
			SetDuplicantJobTitleActive(jobEnabled);
		}
		requiresRefresh = true;
	}

	public void SetSubTitle(string newTitle)
	{
		if ((UnityEngine.Object)subTitle != (UnityEngine.Object)null)
		{
			if (string.IsNullOrEmpty(newTitle))
			{
				subTitle.gameObject.SetActive(false);
			}
			else
			{
				subTitle.gameObject.SetActive(true);
				subTitle.SetText(newTitle);
			}
		}
	}

	public void SetDuplicantJobTitleActive(bool state)
	{
		if ((UnityEngine.Object)duplicantJob != (UnityEngine.Object)null && duplicantJob.gameObject.activeInHierarchy != state)
		{
			duplicantJob.gameObject.SetActive(state);
		}
	}

	public void ForceRefresh()
	{
		requiresRefresh = true;
	}

	public void Update()
	{
		if (requiresRefresh && ((UnityEngine.Object)controller == (UnityEngine.Object)null || controller.enabled))
		{
			requiresRefresh = false;
			Rebuild();
			RefreshScale();
		}
	}

	private void RefreshScale()
	{
		float num = 1f;
		if ((UnityEngine.Object)GameScreenManager.Instance != (UnityEngine.Object)null && (UnityEngine.Object)GameScreenManager.Instance.ssOverlayCanvas != (UnityEngine.Object)null)
		{
			num = GameScreenManager.Instance.ssOverlayCanvas.GetComponent<KCanvasScaler>().GetCanvasScale();
		}
		if ((UnityEngine.Object)controller != (UnityEngine.Object)null)
		{
			controller.animScale = animScaleBase * (1f / num);
		}
	}

	private void Rebuild()
	{
		if ((UnityEngine.Object)controller == (UnityEngine.Object)null)
		{
			controller = GetComponentInChildren<KBatchedAnimController>();
			if ((UnityEngine.Object)controller == (UnityEngine.Object)null)
			{
				if ((UnityEngine.Object)targetImage != (UnityEngine.Object)null)
				{
					targetImage.enabled = true;
				}
				Debug.LogWarning("Controller for [" + base.name + "] null", null);
				return;
			}
		}
		SetPortraitData(identityObject, controller, useDefaultExpression);
		if (useLabels && (UnityEngine.Object)duplicantName != (UnityEngine.Object)null)
		{
			duplicantName.SetText(identityObject.GetProperName());
			if (identityObject is MinionIdentity && (UnityEngine.Object)duplicantJob != (UnityEngine.Object)null)
			{
				duplicantJob.SetText((identityObject == null) ? string.Empty : (identityObject as MinionIdentity).GetComponent<MinionResume>().GetCurrentRoleString());
				duplicantJob.GetComponent<ToolTip>().toolTip = (identityObject as MinionIdentity).GetComponent<MinionResume>().GetCurrentRoleDescription();
			}
		}
	}

	private static void RefreshHat(IAssignableIdentity identityObject, KBatchedAnimController controller)
	{
		MinionIdentity minionIdentity = identityObject as MinionIdentity;
		if (!((UnityEngine.Object)minionIdentity == (UnityEngine.Object)null))
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				RoleConfig role = null;
				if (!string.IsNullOrEmpty(component.CurrentRole))
				{
					role = Game.Instance.roleManager.GetRole(component.CurrentRole);
				}
				RoleManager.ApplyRoleHat(role, component.GetComponent<Accessorizer>(), controller);
			}
		}
	}

	public static void SetPortraitData(IAssignableIdentity identityObject, KBatchedAnimController controller, bool useDefaultExpression = true)
	{
		controller.gameObject.SetActive(true);
		if (identityObject != null)
		{
			MinionIdentity minionIdentity = identityObject as MinionIdentity;
			if ((UnityEngine.Object)minionIdentity == (UnityEngine.Object)null)
			{
				MinionAssignablesProxy minionAssignablesProxy = identityObject as MinionAssignablesProxy;
				if ((UnityEngine.Object)minionAssignablesProxy != (UnityEngine.Object)null && minionAssignablesProxy.target != null)
				{
					minionIdentity = (minionAssignablesProxy.target as MinionIdentity);
				}
			}
			if (!((UnityEngine.Object)minionIdentity == (UnityEngine.Object)null))
			{
				SymbolOverrideController component = controller.GetComponent<SymbolOverrideController>();
				component.RemoveAllSymbolOverrides(0);
				Accessorizer component2 = minionIdentity.GetComponent<Accessorizer>();
				foreach (AccessorySlot resource in Db.Get().AccessorySlots.resources)
				{
					Accessory accessory = component2.GetAccessory(resource);
					if (accessory != null)
					{
						component.AddSymbolOverride(resource.targetSymbolId, accessory.symbol, 0);
						controller.SetSymbolVisiblity(resource.targetSymbolId, true);
					}
				}
				component.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component2.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
				RefreshHat(identityObject, controller);
				float animScale = 1f;
				if ((UnityEngine.Object)GameScreenManager.Instance != (UnityEngine.Object)null && (UnityEngine.Object)GameScreenManager.Instance.ssOverlayCanvas != (UnityEngine.Object)null)
				{
					animScale = 0.2f * (1f / GameScreenManager.Instance.ssOverlayCanvas.GetComponent<KCanvasScaler>().GetUserScale());
				}
				controller.animScale = animScale;
				string s = "ui";
				controller.Play(s, KAnim.PlayMode.Loop, 1f, 0f);
				controller.SetSymbolVisiblity(snapTo_neck, false);
				controller.SetSymbolVisiblity(snapTo_pivot, false);
				controller.SetSymbolVisiblity(snapTo_rgthand, false);
				controller.SetSymbolVisiblity(snapTo_chest, false);
			}
		}
	}

	public void SetAlpha(float value)
	{
		if (!((UnityEngine.Object)controller == (UnityEngine.Object)null))
		{
			Color32 tintColour = controller.TintColour;
			if ((float)(int)tintColour.a != value)
			{
				controller.TintColour = new Color(1f, 1f, 1f, value);
			}
		}
	}
}
