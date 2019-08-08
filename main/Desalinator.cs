using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Desalinator : StateMachineComponent<Desalinator.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Desalinator, object>.GameInstance
	{
		public Chore emptyChore;

		public bool HasSalt => base.master.storage.Has(ElementLoader.FindElementByHash(SimHashes.Salt).tag);

		public StatesInstance(Desalinator smi)
			: base(smi)
		{
		}

		public bool IsFull()
		{
			return base.master.SaltStorageLeft <= 0f;
		}

		public bool IsSaltRemoved()
		{
			return !HasSalt;
		}

		public void CreateEmptyChore()
		{
			if (emptyChore != null)
			{
				emptyChore.Cancel("dupe");
			}
			DesalinatorWorkableEmpty component = base.master.GetComponent<DesalinatorWorkableEmpty>();
			emptyChore = new WorkChore<DesalinatorWorkableEmpty>(Db.Get().ChoreTypes.EmptyDesalinator, component, null, true, OnEmptyComplete, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, true, true);
		}

		public void CancelEmptyChore()
		{
			if (emptyChore != null)
			{
				emptyChore.Cancel("Cancelled");
				emptyChore = null;
			}
		}

		private void OnEmptyComplete(Chore chore)
		{
			emptyChore = null;
			Tag tag = GameTagExtensions.Create(SimHashes.Salt);
			ListPool<GameObject, Desalinator>.PooledList pooledList = ListPool<GameObject, Desalinator>.Allocate();
			base.master.storage.Find(tag, pooledList);
			foreach (GameObject item in pooledList)
			{
				base.master.storage.Drop(item, true);
			}
			pooledList.Recycle();
		}

		public void UpdateStorageLeft()
		{
			Tag tag = GameTagExtensions.Create(SimHashes.Salt);
			base.master.SaltStorageLeft = base.master.maxSalt - base.master.storage.GetMassAvailable(tag);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Desalinator>
	{
		public class OnStates : State
		{
			public State waiting;

			public State working_pre;

			public State working;

			public State working_pst;
		}

		public State off;

		public OnStates on;

		public State full;

		public State fullWaitingForEmpty;

		public State earlyEmpty;

		public State earlyWaitingForEmpty;

		public State empty;

		private static readonly HashedString[] FULL_ANIMS = new HashedString[2]
		{
			"working_pst",
			"off"
		};

		public FloatParameter saltStorageLeft = new FloatParameter(0f);

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, (StatesInstance smi) => smi.master.operational.IsOperational);
			on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, off, (StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(on.waiting);
			on.waiting.EventTransition(GameHashes.OnStorageChange, on.working_pre, (StatesInstance smi) => smi.master.CheckEnoughMassToConvert());
			on.working_pre.PlayAnim("working_pre").OnAnimQueueComplete(on.working);
			on.working.Enter(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).QueueAnim("working_loop", true, null).ParamTransition(saltStorageLeft, full, (StatesInstance smi, float p) => smi.IsFull())
				.EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
				{
					smi.UpdateStorageLeft();
				})
				.EventTransition(GameHashes.OnStorageChange, on.working_pst, (StatesInstance smi) => !smi.master.CheckCanConvert())
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				});
			on.working_pst.PlayAnim("working_pst").OnAnimQueueComplete(on.waiting);
			earlyEmpty.PlayAnims((StatesInstance smi) => FULL_ANIMS, KAnim.PlayMode.Once).OnAnimQueueComplete(earlyWaitingForEmpty);
			earlyWaitingForEmpty.Enter(delegate(StatesInstance smi)
			{
				smi.CreateEmptyChore();
			}).Exit(delegate(StatesInstance smi)
			{
				smi.CancelEmptyChore();
			}).EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => smi.IsSaltRemoved());
			full.PlayAnims((StatesInstance smi) => FULL_ANIMS, KAnim.PlayMode.Once).OnAnimQueueComplete(fullWaitingForEmpty);
			fullWaitingForEmpty.Enter(delegate(StatesInstance smi)
			{
				smi.CreateEmptyChore();
			}).Exit(delegate(StatesInstance smi)
			{
				smi.CancelEmptyChore();
			}).ToggleMainStatusItem(Db.Get().BuildingStatusItems.DesalinatorNeedsEmptying)
				.EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => smi.IsSaltRemoved());
			empty.PlayAnim("off").Enter("ResetStorage", delegate(StatesInstance smi)
			{
				smi.master.SaltStorageLeft = smi.master.maxSalt;
			}).GoTo(on.waiting);
		}
	}

	[MyCmpGet]
	private Operational operational;

	private ManualDeliveryKG[] deliveryComponents;

	[MyCmpReq]
	private Storage storage;

	[Serialize]
	public float maxSalt = 1000f;

	[Serialize]
	private float _storageLeft = 1000f;

	private ElementConverter[] converters;

	private static readonly EventSystem.IntraObjectHandler<Desalinator> OnConduitConnectionChangedDelegate = new EventSystem.IntraObjectHandler<Desalinator>(delegate(Desalinator component, object data)
	{
		component.OnConduitConnectionChanged(data);
	});

	public float SaltStorageLeft
	{
		get
		{
			return _storageLeft;
		}
		set
		{
			_storageLeft = value;
			base.smi.sm.saltStorageLeft.Set(value, base.smi);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		deliveryComponents = GetComponents<ManualDeliveryKG>();
		OnConduitConnectionChanged(GetComponent<ConduitConsumer>().IsConnected);
		Subscribe(-2094018600, OnConduitConnectionChangedDelegate);
		base.smi.StartSM();
	}

	private void OnConduitConnectionChanged(object data)
	{
		bool pause = (bool)data;
		ManualDeliveryKG[] array = deliveryComponents;
		foreach (ManualDeliveryKG manualDeliveryKG in array)
		{
			Element element = ElementLoader.GetElement(manualDeliveryKG.requestedItemTag);
			if (element != null && element.IsLiquid)
			{
				manualDeliveryKG.Pause(pause, "pipe connected");
			}
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.smi.GetCurrentState() != base.smi.sm.full && base.smi.HasSalt && base.smi.emptyChore == null)
		{
			UserMenu userMenu = Game.Instance.userMenu;
			GameObject gameObject = base.gameObject;
			string iconName = "status_item_desalinator_needs_emptying";
			string text = UI.USERMENUACTIONS.EMPTYDESALINATOR.NAME;
			System.Action on_click = delegate
			{
				base.smi.GoTo(base.smi.sm.earlyEmpty);
			};
			string tooltipText = UI.USERMENUACTIONS.CLEANTOILET.TOOLTIP;
			userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true), 1f);
		}
	}

	private bool CheckCanConvert()
	{
		if (converters == null)
		{
			converters = GetComponents<ElementConverter>();
		}
		for (int i = 0; i < converters.Length; i++)
		{
			if (converters[i].CanConvertAtAll())
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckEnoughMassToConvert()
	{
		if (converters == null)
		{
			converters = GetComponents<ElementConverter>();
		}
		for (int i = 0; i < converters.Length; i++)
		{
			if (converters[i].HasEnoughMassToStartConverting())
			{
				return true;
			}
		}
		return false;
	}
}
