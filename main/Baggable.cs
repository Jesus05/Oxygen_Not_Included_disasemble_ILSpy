using KSerialization;
using UnityEngine;

public class Baggable : KMonoBehaviour
{
	[SerializeField]
	private KAnimFile minionAnimOverride;

	public bool mustStandOntopOfTrapForPickup;

	[Serialize]
	public bool wrangled;

	public bool useGunForPickup;

	private static readonly EventSystem.IntraObjectHandler<Baggable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Baggable>(delegate(Baggable component, object data)
	{
		component.OnStore(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		minionAnimOverride = Assets.GetAnim("anim_restrain_creature_kanim");
		Pickupable pickupable = base.gameObject.AddOrGet<Pickupable>();
		pickupable.workAnims = new HashedString[2]
		{
			new HashedString("capture"),
			new HashedString("pickup")
		};
		pickupable.workAnimPlayMode = KAnim.PlayMode.Once;
		pickupable.workingPstComplete = HashedString.Invalid;
		pickupable.workingPstFailed = HashedString.Invalid;
		pickupable.overrideAnims = new KAnimFile[1]
		{
			minionAnimOverride
		};
		pickupable.trackOnPickup = false;
		pickupable.useGunforPickup = useGunForPickup;
		pickupable.synchronizeAnims = false;
		pickupable.SetWorkTime(3f);
		if (mustStandOntopOfTrapForPickup)
		{
			pickupable.SetOffsets(new CellOffset[2]
			{
				default(CellOffset),
				new CellOffset(0, -1)
			});
		}
		Subscribe(856640610, OnStoreDelegate);
		if ((Object)base.transform.parent != (Object)null)
		{
			if ((Object)base.transform.parent.GetComponent<Trap>() != (Object)null)
			{
				GetComponent<KBatchedAnimController>().enabled = true;
			}
			if ((Object)base.transform.parent.GetComponent<EggIncubator>() != (Object)null)
			{
				wrangled = true;
			}
		}
		if (wrangled)
		{
			SetWrangled();
		}
	}

	private void OnStore(object data)
	{
		Storage storage = data as Storage;
		if ((Object)storage != (Object)null || (data != null && (bool)data))
		{
			base.gameObject.AddTag(GameTags.Creatures.Bagged);
			if ((bool)storage && storage.HasTag(GameTags.Minion))
			{
				SetVisible(false);
			}
		}
		else
		{
			Free();
		}
	}

	private void SetVisible(bool visible)
	{
		KAnimControllerBase component = base.gameObject.GetComponent<KAnimControllerBase>();
		if ((Object)component != (Object)null && component.enabled != visible)
		{
			component.enabled = visible;
		}
		KSelectable component2 = base.gameObject.GetComponent<KSelectable>();
		if ((Object)component2 != (Object)null && component2.enabled != visible)
		{
			component2.enabled = visible;
		}
	}

	public void SetWrangled()
	{
		wrangled = true;
		Navigator component = GetComponent<Navigator>();
		if ((bool)component && component.IsValidNavType(NavType.Floor))
		{
			component.SetCurrentNavType(NavType.Floor);
		}
		base.gameObject.AddTag(GameTags.Creatures.Bagged);
	}

	public void Free()
	{
		base.gameObject.RemoveTag(GameTags.Creatures.Bagged);
		wrangled = false;
		SetVisible(true);
	}
}
