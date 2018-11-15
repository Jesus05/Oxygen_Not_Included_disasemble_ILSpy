using System.Runtime.CompilerServices;
using UnityEngine;

public class FishFeeder : GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>
{
	public class Def : BaseDef
	{
	}

	public class OperationalState : State
	{
		public State on;
	}

	public new class Instance : GameInstance
	{
		public FishFeederTop fishFeederTop;

		public FishFeederBot fishFeederBot;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	public class FishFeederTop : IRenderEveryTick
	{
		private Instance smi;

		private float mass;

		private float targetMass;

		private HashedString[] ballSymbols;

		private float massPerBall;

		private float timeSinceLastBallAppeared;

		public FishFeederTop(Instance smi, HashedString[] ball_symbols, float capacity)
		{
			this.smi = smi;
			ballSymbols = ball_symbols;
			massPerBall = capacity / (float)ball_symbols.Length;
			FillFeeder(mass);
			SimAndRenderScheduler.instance.Add(this, false);
		}

		private void FillFeeder(float mass)
		{
			KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
			SymbolOverrideController component2 = smi.GetComponent<SymbolOverrideController>();
			KAnim.Build.Symbol symbol = null;
			Storage component3 = smi.GetComponent<Storage>();
			if (component3.items.Count > 0 && (Object)component3.items[0] != (Object)null)
			{
				symbol = smi.GetComponent<Storage>().items[0].GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol("algae");
			}
			for (int i = 0; i < ballSymbols.Length; i++)
			{
				bool is_visible = mass > (float)(i + 1) * massPerBall;
				component.SetSymbolVisiblity(ballSymbols[i], is_visible);
				if (symbol != null)
				{
					component2.AddSymbolOverride(ballSymbols[i], symbol, 0);
				}
			}
		}

		public void RefreshStorage()
		{
			float num = 0f;
			foreach (GameObject item in smi.GetComponent<Storage>().items)
			{
				if (!((Object)item == (Object)null))
				{
					num += item.GetComponent<PrimaryElement>().Mass;
				}
			}
			targetMass = num;
		}

		public void RenderEveryTick(float dt)
		{
			timeSinceLastBallAppeared += dt;
			if (targetMass > mass && timeSinceLastBallAppeared > 0.025f)
			{
				float num = Mathf.Min(massPerBall, targetMass - mass);
				mass += num;
				FillFeeder(mass);
				timeSinceLastBallAppeared = 0f;
			}
		}

		public void Cleanup()
		{
			SimAndRenderScheduler.instance.Remove(this);
		}
	}

	public class FishFeederBot
	{
		private KBatchedAnimController anim;

		private Storage topStorage;

		private Storage botStorage;

		private bool refreshingStorage;

		private Instance smi;

		private float massPerBall;

		private static readonly HashedString HASH_FEEDBALL = "feedball";

		public FishFeederBot(Instance smi, float mass_per_ball, HashedString[] ball_symbols)
		{
			this.smi = smi;
			massPerBall = mass_per_ball;
			anim = GameUtil.KInstantiate(Assets.GetPrefab("FishFeederBot"), smi.transform.GetPosition(), Grid.SceneLayer.Front, null, 0).GetComponent<KBatchedAnimController>();
			anim.transform.SetParent(smi.transform);
			anim.gameObject.SetActive(true);
			anim.SetSceneLayer(Grid.SceneLayer.Building);
			anim.Play("ball", KAnim.PlayMode.Once, 1f, 0f);
			anim.Stop();
			foreach (HashedString hash in ball_symbols)
			{
				anim.SetSymbolVisiblity(hash, false);
			}
			Storage[] components = smi.gameObject.GetComponents<Storage>();
			topStorage = components[0];
			botStorage = components[1];
		}

		public void RefreshStorage()
		{
			if (!refreshingStorage)
			{
				refreshingStorage = true;
				float num = 0f;
				foreach (GameObject item in botStorage.items)
				{
					if (!((Object)item == (Object)null))
					{
						num += item.GetComponent<PrimaryElement>().Mass;
						int cell = Grid.PosToCell(smi.transform.GetPosition());
						int cell2 = Grid.CellBelow(Grid.CellBelow(cell));
						item.transform.SetPosition(Grid.CellToPosCBC(cell2, Grid.SceneLayer.BuildingBack));
					}
				}
				if (num == 0f)
				{
					float num2 = 0f;
					foreach (GameObject item2 in topStorage.items)
					{
						if (!((Object)item2 == (Object)null))
						{
							num2 += item2.GetComponent<PrimaryElement>().Mass;
						}
					}
					if (num2 > 0f)
					{
						anim.SetSymbolVisiblity(HASH_FEEDBALL, true);
						anim.Play("ball", KAnim.PlayMode.Once, 1f, 0f);
						Pickupable pickupable = topStorage.items[0].GetComponent<Pickupable>().Take(massPerBall);
						KAnim.Build.Symbol symbol = pickupable.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol("algae");
						if (symbol != null)
						{
							anim.GetComponent<SymbolOverrideController>().AddSymbolOverride(HASH_FEEDBALL, symbol, 0);
						}
						botStorage.Store(pickupable.gameObject, false, false, true, false);
						int cell3 = Grid.PosToCell(smi.transform.GetPosition());
						int cell4 = Grid.CellBelow(Grid.CellBelow(cell3));
						pickupable.transform.SetPosition(Grid.CellToPosCBC(cell4, Grid.SceneLayer.BuildingUse));
					}
					else
					{
						anim.SetSymbolVisiblity(HASH_FEEDBALL, false);
					}
				}
				refreshingStorage = false;
			}
		}

		public void Cleanup()
		{
		}
	}

	public State notoperational;

	public OperationalState operational;

	public static HashedString[] ballSymbols;

	[CompilerGenerated]
	private static StateMachine<FishFeeder, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static StateMachine<FishFeeder, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static GameEvent.Callback _003C_003Ef__mg_0024cache2;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = notoperational;
		root.Enter(SetupFishFeederTopAndBot).Exit(CleanupFishFeederTopAndBot).EventHandler(GameHashes.OnStorageChange, OnStorageChange);
		notoperational.TagTransition(GameTags.Operational, operational, false);
		operational.DefaultState(operational.on).TagTransition(GameTags.Operational, notoperational, true);
		operational.on.DoNothing();
		int num = 19;
		ballSymbols = new HashedString[num];
		for (int i = 0; i < num; i++)
		{
			ballSymbols[i] = "ball" + i.ToString();
		}
	}

	private static void SetupFishFeederTopAndBot(Instance smi)
	{
		Storage storage = smi.Get<Storage>();
		smi.fishFeederTop = new FishFeederTop(smi, ballSymbols, storage.Capacity());
		smi.fishFeederTop.RefreshStorage();
		smi.fishFeederBot = new FishFeederBot(smi, 10f, ballSymbols);
		smi.fishFeederBot.RefreshStorage();
	}

	private static void CleanupFishFeederTopAndBot(Instance smi)
	{
		smi.fishFeederTop.Cleanup();
		smi.fishFeederBot.Cleanup();
	}

	private static void MoveStoredContentsToConsumeOffset(Instance smi)
	{
		foreach (GameObject item in smi.GetComponent<Storage>().items)
		{
			if (!((Object)item == (Object)null))
			{
				OnStorageChange(smi, item);
			}
		}
	}

	private static void OnStorageChange(Instance smi, object data)
	{
		GameObject x = (GameObject)data;
		if (!((Object)x == (Object)null))
		{
			smi.fishFeederTop.RefreshStorage();
			smi.fishFeederBot.RefreshStorage();
		}
	}
}
