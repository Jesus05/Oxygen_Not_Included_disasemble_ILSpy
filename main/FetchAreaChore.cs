using System;
using System.Collections.Generic;
using UnityEngine;

public class FetchAreaChore : Chore<FetchAreaChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, FetchAreaChore, object>.GameInstance
	{
		public struct Delivery
		{
			private Action<FetchChore> onCancelled;

			private Action<Chore> onFetchChoreCleanup;

			public Storage destination
			{
				get;
				private set;
			}

			public float amount
			{
				get;
				private set;
			}

			public FetchChore chore
			{
				get;
				private set;
			}

			public Delivery(Precondition.Context context, float amount_to_be_fetched, Action<FetchChore> on_cancelled)
			{
				this = default(Delivery);
				this.chore = (context.chore as FetchChore);
				amount = this.chore.originalAmount;
				destination = this.chore.destination;
				this.chore.SetOverrideTarget(context.consumerState.consumer);
				onCancelled = on_cancelled;
				onFetchChoreCleanup = OnFetchChoreCleanup;
				this.chore.FetchAreaBegin(context, amount_to_be_fetched);
				FetchChore chore = this.chore;
				chore.onCleanup = (Action<Chore>)Delegate.Combine(chore.onCleanup, onFetchChoreCleanup);
			}

			public void Complete(List<Pickupable> deliverables)
			{
				using (new KProfiler.Region("FAC.Delivery.Complete", null))
				{
					if (!((UnityEngine.Object)destination == (UnityEngine.Object)null) && !destination.IsEndOfLife())
					{
						FetchChore chore = this.chore;
						chore.onCleanup = (Action<Chore>)Delegate.Remove(chore.onCleanup, onFetchChoreCleanup);
						float num = amount;
						Pickupable pickupable = null;
						for (int i = 0; i < deliverables.Count; i++)
						{
							if (num <= 0f)
							{
								break;
							}
							if ((UnityEngine.Object)deliverables[i] == (UnityEngine.Object)null)
							{
								if (num < PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
								{
									destination.ForceStore(this.chore.tags[0], num);
								}
							}
							else
							{
								Pickupable pickupable2 = deliverables[i].Take(num);
								if ((UnityEngine.Object)pickupable2 != (UnityEngine.Object)null && pickupable2.TotalAmount > 0f)
								{
									num -= pickupable2.TotalAmount;
									destination.Store(pickupable2.gameObject, false, false, true, false);
									pickupable = pickupable2;
									if ((UnityEngine.Object)pickupable2 == (UnityEngine.Object)deliverables[i])
									{
										deliverables[i] = null;
									}
								}
							}
						}
						if ((UnityEngine.Object)this.chore.overrideTarget != (UnityEngine.Object)null)
						{
							this.chore.FetchAreaEnd(this.chore.overrideTarget.GetComponent<ChoreDriver>(), pickupable, true);
						}
						this.chore = null;
					}
				}
			}

			private void OnFetchChoreCleanup(Chore chore)
			{
				if (onCancelled != null)
				{
					onCancelled(chore as FetchChore);
				}
			}

			public void Cleanup()
			{
				if (this.chore != null)
				{
					FetchChore chore = this.chore;
					chore.onCleanup = (Action<Chore>)Delegate.Remove(chore.onCleanup, onFetchChoreCleanup);
					this.chore.FetchAreaEnd(null, null, false);
				}
			}
		}

		public struct Reservation
		{
			private int handle;

			public float amount
			{
				get;
				private set;
			}

			public Pickupable pickupable
			{
				get;
				private set;
			}

			public Reservation(ChoreConsumer consumer, Pickupable pickupable, float reservation_amount)
			{
				this = default(Reservation);
				if (reservation_amount <= 0f)
				{
					Debug.LogError("Invalid amount: " + reservation_amount);
				}
				amount = reservation_amount;
				this.pickupable = pickupable;
				handle = pickupable.Reserve("FetchAreaChore", consumer.gameObject, reservation_amount);
			}

			public void Cleanup()
			{
				if ((UnityEngine.Object)pickupable != (UnityEngine.Object)null)
				{
					pickupable.Unreserve("FetchAreaChore", handle);
				}
			}
		}

		private List<FetchChore> chores = new List<FetchChore>();

		private List<Pickupable> fetchables = new List<Pickupable>();

		private List<Reservation> reservations = new List<Reservation>();

		private List<Pickupable> deliverables = new List<Pickupable>();

		public List<Delivery> deliveries = new List<Delivery>();

		private FetchChore rootChore;

		private Precondition.Context rootContext;

		private float fetchAmountRequested;

		public bool delivering;

		public bool pickingup;

		public StatesInstance(FetchAreaChore master, Precondition.Context context)
			: base(master)
		{
			rootContext = context;
			rootChore = (context.chore as FetchChore);
		}

		public void Begin(Precondition.Context context)
		{
			base.sm.fetcher.Set(context.consumerState.gameObject, base.smi);
			chores.Clear();
			chores.Add(rootChore);
			Grid.CellToXY(Grid.PosToCell(rootChore.destination.transform.GetPosition()), out int x, out int y);
			ListPool<Precondition.Context, FetchAreaChore>.PooledList pooledList = ListPool<Precondition.Context, FetchAreaChore>.Allocate();
			ListPool<Precondition.Context, FetchAreaChore>.PooledList pooledList2 = ListPool<Precondition.Context, FetchAreaChore>.Allocate();
			if (rootChore.allowMultifetch)
			{
				GatherNearbyFetchChores(rootChore, context, x, y, 3, pooledList, pooledList2);
			}
			float num = Mathf.Max(1f, Db.Get().Attributes.CarryAmount.Lookup(context.consumerState.consumer).GetTotalValue());
			Pickupable pickupable = context.data as Pickupable;
			if ((UnityEngine.Object)pickupable == (UnityEngine.Object)null)
			{
				Debug.Assert(pooledList.Count > 0, "succeeded_contexts was empty");
				Precondition.Context context2 = pooledList[0];
				FetchChore fetchChore = (FetchChore)context2.chore;
				Debug.Assert(fetchChore != null, "fetch_chore was null");
				DebugUtil.LogWarningArgs("Missing root_fetchable for FetchAreaChore", fetchChore.destination, fetchChore.tags[0]);
				pickupable = fetchChore.FindFetchTarget(context.consumerState);
			}
			Debug.Assert((UnityEngine.Object)pickupable != (UnityEngine.Object)null, "root_fetchable was null");
			List<Pickupable> list = new List<Pickupable>();
			list.Add(pickupable);
			float num2 = pickupable.UnreservedAmount;
			float minTakeAmount = pickupable.MinTakeAmount;
			int x2 = 0;
			int y2 = 0;
			Grid.CellToXY(Grid.PosToCell(pickupable.transform.GetPosition()), out x2, out y2);
			int num3 = 6;
			x2 -= num3 / 2;
			y2 -= num3 / 2;
			ListPool<ScenePartitionerEntry, FetchAreaChore>.PooledList pooledList3 = ListPool<ScenePartitionerEntry, FetchAreaChore>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(x2, y2, num3, num3, GameScenePartitioner.Instance.pickupablesLayer, pooledList3);
			Tag prefabTag = pickupable.GetComponent<KPrefabID>().PrefabTag;
			for (int i = 0; i < pooledList3.Count; i++)
			{
				ScenePartitionerEntry scenePartitionerEntry = pooledList3[i];
				if (num2 > num)
				{
					break;
				}
				Pickupable pickupable2 = scenePartitionerEntry.obj as Pickupable;
				KPrefabID component = pickupable2.GetComponent<KPrefabID>();
				if (!(component.PrefabTag != prefabTag) && !(pickupable2.UnreservedAmount <= 0f))
				{
					component.UpdateTagBits();
					if (component.HasAllTags_AssumeLaundered(ref rootChore.requiredTagBits) && !component.HasAnyTags_AssumeLaundered(ref rootChore.forbiddenTagBits) && !list.Contains(pickupable2) && rootContext.consumerState.consumer.CanReach(pickupable2))
					{
						float unreservedAmount = pickupable2.UnreservedAmount;
						list.Add(pickupable2);
						num2 += unreservedAmount;
						if (list.Count >= 10)
						{
							break;
						}
					}
				}
			}
			pooledList3.Recycle();
			num2 = Mathf.Min(num, num2);
			if (minTakeAmount > 0f)
			{
				num2 -= num2 % minTakeAmount;
			}
			deliveries.Clear();
			float num4 = Mathf.Min(rootChore.originalAmount, num2);
			if (minTakeAmount > 0f)
			{
				num4 -= num4 % minTakeAmount;
			}
			deliveries.Add(new Delivery(rootContext, num4, OnFetchChoreCancelled));
			float num5 = num4;
			for (int j = 0; j < pooledList.Count; j++)
			{
				if (num5 >= num2)
				{
					break;
				}
				Precondition.Context context3 = pooledList[j];
				FetchChore fetchChore2 = context3.chore as FetchChore;
				if (fetchChore2 != rootChore && context3.IsSuccess() && (UnityEngine.Object)fetchChore2.overrideTarget == (UnityEngine.Object)null && (UnityEngine.Object)fetchChore2.driver == (UnityEngine.Object)null && fetchChore2.tagBits.AreEqual(ref rootChore.tagBits))
				{
					num4 = Mathf.Min(fetchChore2.originalAmount, num2 - num5);
					if (minTakeAmount > 0f)
					{
						num4 -= num4 % minTakeAmount;
					}
					chores.Add(fetchChore2);
					deliveries.Add(new Delivery(context3, num4, OnFetchChoreCancelled));
					num5 += num4;
					if (deliveries.Count >= 10)
					{
						break;
					}
				}
			}
			num5 = Mathf.Min(num5, num2);
			float num6 = num5;
			fetchables.Clear();
			for (int k = 0; k < list.Count; k++)
			{
				if (num6 <= 0f)
				{
					break;
				}
				Pickupable pickupable3 = list[k];
				num6 -= pickupable3.UnreservedAmount;
				fetchables.Add(pickupable3);
			}
			fetchAmountRequested = num5;
			reservations.Clear();
			pooledList.Recycle();
			pooledList2.Recycle();
		}

		public void End()
		{
			foreach (Delivery delivery in deliveries)
			{
				delivery.Cleanup();
			}
			deliveries.Clear();
		}

		public void SetupDelivery()
		{
			deliverables.RemoveAll((Pickupable x) => (UnityEngine.Object)x == (UnityEngine.Object)null || x.TotalAmount <= 0f);
			if (deliveries.Count > 0 && deliverables.Count > 0)
			{
				base.sm.deliveryDestination.Set(deliveries[0].destination, base.smi);
				base.sm.deliveryObject.Set(deliverables[0], base.smi);
				if ((UnityEngine.Object)deliveries[0].destination != (UnityEngine.Object)null)
				{
					if (rootContext.consumerState.hasSolidTransferArm)
					{
						if (rootContext.consumerState.consumer.IsWithinReach(deliveries[0].destination))
						{
							GoTo(base.sm.delivering.storing);
						}
						else
						{
							GoTo(base.sm.delivering.deliverfail);
						}
					}
					else
					{
						GoTo(base.sm.delivering.movetostorage);
					}
				}
				else
				{
					base.smi.GoTo(base.sm.delivering.deliverfail);
				}
			}
			else
			{
				StopSM("FetchAreaChoreComplete");
			}
		}

		public void SetupFetch()
		{
			if (reservations.Count > 0)
			{
				base.sm.fetchTarget.Set(reservations[0].pickupable, base.smi);
				base.sm.fetchResultTarget.Set(null, base.smi);
				base.sm.fetchAmount.Set(reservations[0].amount, base.smi);
				if ((UnityEngine.Object)reservations[0].pickupable != (UnityEngine.Object)null)
				{
					if (rootContext.consumerState.hasSolidTransferArm)
					{
						if (rootContext.consumerState.consumer.IsWithinReach(reservations[0].pickupable))
						{
							GoTo(base.sm.fetching.pickup);
						}
						else
						{
							GoTo(base.sm.fetching.fetchfail);
						}
					}
					else
					{
						GoTo(base.sm.fetching.movetopickupable);
					}
				}
				else
				{
					GoTo(base.sm.fetching.fetchfail);
				}
			}
			else
			{
				GoTo(base.sm.delivering.next);
			}
		}

		public void DeliverFail()
		{
			if (deliveries.Count > 0)
			{
				deliveries[0].Cleanup();
				deliveries.RemoveAt(0);
			}
			GoTo(base.sm.delivering.next);
		}

		public void DeliverComplete()
		{
			Pickupable pickupable = base.sm.deliveryObject.Get<Pickupable>(base.smi);
			if ((UnityEngine.Object)pickupable == (UnityEngine.Object)null || pickupable.TotalAmount <= 0f)
			{
				if (deliveries.Count > 0 && deliveries[0].chore.amount < PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
				{
					Delivery delivery = deliveries[0];
					Chore chore = delivery.chore;
					delivery.Complete(deliverables);
					delivery.Cleanup();
					if (deliveries.Count > 0 && deliveries[0].chore == chore)
					{
						deliveries.RemoveAt(0);
					}
					GoTo(base.sm.delivering.next);
				}
				else
				{
					base.smi.GoTo(base.sm.delivering.deliverfail);
				}
			}
			else
			{
				if (deliveries.Count > 0)
				{
					Delivery delivery2 = deliveries[0];
					Chore chore2 = delivery2.chore;
					delivery2.Complete(deliverables);
					delivery2.Cleanup();
					if (deliveries.Count > 0 && deliveries[0].chore == chore2)
					{
						deliveries.RemoveAt(0);
					}
				}
				GoTo(base.sm.delivering.next);
			}
		}

		public void FetchFail()
		{
			reservations[0].Cleanup();
			reservations.RemoveAt(0);
			GoTo(base.sm.fetching.next);
		}

		public void FetchComplete()
		{
			reservations[0].Cleanup();
			reservations.RemoveAt(0);
			GoTo(base.sm.fetching.next);
		}

		public void SetupDeliverables()
		{
			foreach (GameObject item in base.sm.fetcher.Get<Storage>(base.smi).items)
			{
				if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
				{
					KPrefabID component = item.GetComponent<KPrefabID>();
					if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
					{
						Pickupable component2 = component.GetComponent<Pickupable>();
						if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
						{
							deliverables.Add(component2);
						}
					}
				}
			}
		}

		public void ReservePickupables()
		{
			ChoreConsumer consumer = base.sm.fetcher.Get<ChoreConsumer>(base.smi);
			float num = fetchAmountRequested;
			foreach (Pickupable fetchable in fetchables)
			{
				if (num <= 0f)
				{
					break;
				}
				float num2 = Math.Min(num, fetchable.UnreservedAmount);
				num -= num2;
				Reservation item = new Reservation(consumer, fetchable, num2);
				reservations.Add(item);
			}
		}

		private void OnFetchChoreCancelled(FetchChore chore)
		{
			int num = 0;
			while (true)
			{
				if (num >= deliveries.Count)
				{
					return;
				}
				if (deliveries[num].chore == chore)
				{
					break;
				}
				num++;
			}
			if (deliveries.Count == 1)
			{
				StopSM("AllDelivericesCancelled");
			}
			else if (num == 0)
			{
				base.sm.currentdeliverycancelled.Trigger(this);
			}
			else
			{
				deliveries[num].Cleanup();
				deliveries.RemoveAt(num);
			}
		}

		public void UnreservePickupables()
		{
			foreach (Reservation reservation in reservations)
			{
				reservation.Cleanup();
			}
			reservations.Clear();
		}

		public bool SameDestination(FetchChore fetch)
		{
			foreach (FetchChore chore in chores)
			{
				if ((UnityEngine.Object)chore.destination == (UnityEngine.Object)fetch.destination)
				{
					return true;
				}
			}
			return false;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, FetchAreaChore>
	{
		public class FetchStates : State
		{
			public State next;

			public ApproachSubState<Pickupable> movetopickupable;

			public State pickup;

			public State fetchfail;

			public State fetchcomplete;
		}

		public class DeliverStates : State
		{
			public State next;

			public ApproachSubState<Storage> movetostorage;

			public State storing;

			public State deliverfail;

			public State delivercomplete;
		}

		public FetchStates fetching;

		public DeliverStates delivering;

		public TargetParameter fetcher;

		public TargetParameter fetchTarget;

		public TargetParameter fetchResultTarget;

		public FloatParameter fetchAmount;

		public TargetParameter deliveryDestination;

		public TargetParameter deliveryObject;

		public FloatParameter deliveryAmount;

		public Signal currentdeliverycancelled;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = fetching;
			Target(fetcher);
			fetching.DefaultState(fetching.next).Enter("ReservePickupables", delegate(StatesInstance smi)
			{
				smi.ReservePickupables();
			}).Exit("UnreservePickupables", delegate(StatesInstance smi)
			{
				smi.UnreservePickupables();
			})
				.Enter("pickingup-on", delegate(StatesInstance smi)
				{
					smi.pickingup = true;
				})
				.Exit("pickingup-off", delegate(StatesInstance smi)
				{
					smi.pickingup = false;
				});
			fetching.next.Enter("SetupFetch", delegate(StatesInstance smi)
			{
				smi.SetupFetch();
			});
			ApproachSubState<Pickupable> movetopickupable = fetching.movetopickupable;
			TargetParameter mover = fetcher;
			TargetParameter move_target = fetchTarget;
			State pickup = fetching.pickup;
			State fetchfail = fetching.fetchfail;
			NavTactic reduceTravelDistance = NavigationTactics.ReduceTravelDistance;
			movetopickupable.InitializeStates(mover, move_target, pickup, fetchfail, null, reduceTravelDistance);
			fetching.pickup.DoPickup(fetchTarget, fetchResultTarget, fetchAmount, fetching.fetchcomplete, fetching.fetchfail);
			fetching.fetchcomplete.Enter(delegate(StatesInstance smi)
			{
				smi.FetchComplete();
			});
			fetching.fetchfail.Enter(delegate(StatesInstance smi)
			{
				smi.FetchFail();
			});
			delivering.DefaultState(delivering.next).OnSignal(currentdeliverycancelled, delivering.deliverfail).Enter("SetupDeliverables", delegate(StatesInstance smi)
			{
				smi.SetupDeliverables();
			})
				.Enter("delivering-on", delegate(StatesInstance smi)
				{
					smi.delivering = true;
				})
				.Exit("delivering-off", delegate(StatesInstance smi)
				{
					smi.delivering = false;
				});
			delivering.next.Enter("SetupDelivery", delegate(StatesInstance smi)
			{
				smi.SetupDelivery();
			});
			ApproachSubState<Storage> movetostorage = delivering.movetostorage;
			move_target = fetcher;
			mover = deliveryDestination;
			fetchfail = delivering.storing;
			pickup = delivering.deliverfail;
			reduceTravelDistance = NavigationTactics.ReduceTravelDistance;
			movetostorage.InitializeStates(move_target, mover, fetchfail, pickup, null, reduceTravelDistance).Enter(delegate(StatesInstance smi)
			{
				if ((UnityEngine.Object)deliveryObject.Get(smi) != (UnityEngine.Object)null && (UnityEngine.Object)deliveryObject.Get(smi).GetComponent<MinionIdentity>() != (UnityEngine.Object)null)
				{
					deliveryObject.Get(smi).transform.SetLocalPosition(Vector3.zero);
					KBatchedAnimTracker component = deliveryObject.Get(smi).GetComponent<KBatchedAnimTracker>();
					component.symbol = new HashedString("snapTo_chest");
					component.offset = new Vector3(0f, 0f, 1f);
				}
			});
			delivering.storing.DoDelivery(fetcher, deliveryDestination, delivering.delivercomplete, delivering.deliverfail);
			delivering.deliverfail.Enter(delegate(StatesInstance smi)
			{
				smi.DeliverFail();
			});
			delivering.delivercomplete.Enter(delegate(StatesInstance smi)
			{
				smi.DeliverComplete();
			});
		}
	}

	public bool IsFetching => base.smi.pickingup;

	public bool IsDelivering => base.smi.delivering;

	public GameObject GetFetchTarget => base.smi.sm.fetchTarget.Get(base.smi);

	public FetchAreaChore(Precondition.Context context)
		: base(context.chore.choreType, (IStateMachineTarget)context.consumerState.consumer, context.consumerState.choreProvider, false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, context.masterPriority.priority_class, context.masterPriority.priority_value, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		showAvailabilityInHoverText = false;
		base.smi = new StatesInstance(this, context);
	}

	public override void Cleanup()
	{
		base.Cleanup();
	}

	public override void Begin(Precondition.Context context)
	{
		base.smi.Begin(context);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		base.smi.End();
		base.End(reason);
	}

	private void OnTagsChanged(object data)
	{
		if ((UnityEngine.Object)base.smi.sm.fetchTarget.Get(base.smi) != (UnityEngine.Object)null)
		{
			Fail("Tags changed");
		}
	}

	public static void GatherNearbyFetchChores(FetchChore root_chore, Precondition.Context context, int x, int y, int radius, List<Precondition.Context> succeeded_contexts, List<Precondition.Context> failed_contexts)
	{
		ListPool<ScenePartitionerEntry, FetchAreaChore>.PooledList pooledList = ListPool<ScenePartitionerEntry, FetchAreaChore>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(x - radius, y - radius, radius * 2 + 1, radius * 2 + 1, GameScenePartitioner.Instance.fetchChoreLayer, pooledList);
		for (int i = 0; i < pooledList.Count; i++)
		{
			ScenePartitionerEntry scenePartitionerEntry = pooledList[i];
			FetchChore fetchChore = scenePartitionerEntry.obj as FetchChore;
			fetchChore.CollectChoresFromGlobalChoreProvider(context.consumerState, succeeded_contexts, failed_contexts, true);
		}
		pooledList.Recycle();
	}
}
