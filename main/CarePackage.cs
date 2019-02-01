using System.Collections.Generic;
using UnityEngine;

public class CarePackage : StateMachineComponent<CarePackage.SMInstance>
{
	public class SMInstance : GameStateMachine<States, SMInstance, CarePackage, object>.GameInstance
	{
		public List<Chore> activeUseChores;

		public SMInstance(CarePackage master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, SMInstance, CarePackage>
	{
		public State spawn;

		public State open;

		public State pst;

		public State destroy;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = spawn;
			spawn.PlayAnim("portalbirth").OnAnimQueueComplete(open);
			open.PlayAnim("portalbirth_pst").QueueAnim("object_idle_loop", false, null).Exit(delegate(SMInstance smi)
			{
				smi.master.SpawnContents();
			})
				.ScheduleGoTo(1f, pst);
			pst.PlayAnim("object_idle_pst").ScheduleGoTo(5f, destroy);
			destroy.Enter(delegate(SMInstance smi)
			{
				Util.KDestroyGameObject(smi.master.gameObject);
			});
		}
	}

	private CarePackageInfo info;

	private Reactable reactable;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		reactable = CreateReactable();
	}

	public Reactable CreateReactable()
	{
		return new EmoteReactable(base.gameObject, "UpgradeFX", Db.Get().ChoreTypes.Emote, "anim_cheer_kanim", 15, 8, 0f, 20f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
		{
			anim = (HashedString)"cheer_pre"
		}).AddStep(new EmoteReactable.EmoteStep
		{
			anim = (HashedString)"cheer_loop"
		}).AddStep(new EmoteReactable.EmoteStep
		{
			anim = (HashedString)"cheer_pst"
		});
	}

	protected override void OnCleanUp()
	{
		reactable.Cleanup();
		base.OnCleanUp();
	}

	public void SetInfo(CarePackageInfo info)
	{
		this.info = info;
		GameObject prefab = Assets.GetPrefab(info.id);
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("Meter".ToTag()), base.gameObject, null);
		GameObject prefab2 = Assets.GetPrefab(info.id);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		KBatchedAnimController component2 = prefab2.GetComponent<KBatchedAnimController>();
		SymbolOverrideController component3 = prefab2.GetComponent<SymbolOverrideController>();
		KBatchedAnimController component4 = gameObject.GetComponent<KBatchedAnimController>();
		component4.transform.SetLocalPosition(Vector3.forward);
		component4.AnimFiles = component2.AnimFiles;
		component4.isMovable = true;
		component4.animWidth = component2.animWidth;
		component4.animHeight = component2.animHeight;
		if ((Object)component3 != (Object)null)
		{
			SymbolOverrideController symbolOverrideController = SymbolOverrideControllerUtil.AddToPrefab(gameObject);
			SymbolOverrideController.SymbolEntry[] getSymbolOverrides = component3.GetSymbolOverrides;
			for (int i = 0; i < getSymbolOverrides.Length; i++)
			{
				SymbolOverrideController.SymbolEntry symbolEntry = getSymbolOverrides[i];
				symbolOverrideController.AddSymbolOverride(symbolEntry.targetSymbol, symbolEntry.sourceSymbol, 0);
			}
		}
		component4.initialAnim = component2.initialAnim;
		component4.initialMode = KAnim.PlayMode.Loop;
		KBatchedAnimTracker component5 = gameObject.GetComponent<KBatchedAnimTracker>();
		component5.controller = component;
		component5.symbol = new HashedString("snapTO_object");
		component5.offset = new Vector3(0f, 0.5f, 0f);
		gameObject.SetActive(true);
		component.SetSymbolVisiblity("snapTO_object", false);
		new KAnimLink(component, component4);
	}

	private void SpawnContents()
	{
		GameObject gameObject = null;
		GameObject prefab = Assets.GetPrefab(info.id);
		Element element = null;
		element = ElementLoader.GetElement(info.id.ToTag());
		Vector3 position = base.transform.position + Vector3.up / 2f;
		if (element == null && (Object)prefab != (Object)null)
		{
			for (int i = 0; (float)i < info.quantity; i++)
			{
				gameObject = Util.KInstantiate(prefab, position);
				if ((Object)gameObject != (Object)null)
				{
					gameObject.SetActive(true);
				}
			}
		}
		else if (element != null)
		{
			float quantity = info.quantity;
			gameObject = element.substance.SpawnResource(position, quantity, element.defaultValues.temperature, byte.MaxValue, 0, false, true);
		}
		else
		{
			Debug.LogWarning("Can't find spawnable thing from tag " + info.id, null);
		}
		if ((Object)gameObject != (Object)null)
		{
			gameObject.SetActive(true);
		}
	}
}
