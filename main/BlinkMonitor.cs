using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BlinkMonitor : GameStateMachine<BlinkMonitor, BlinkMonitor.Instance>
{
	public class Def : BaseDef
	{
	}

	public class Tuning : TuningData<Tuning>
	{
		public float randomBlinkIntervalMin;

		public float randomBlinkIntervalMax;
	}

	public new class Instance : GameInstance
	{
		public KBatchedAnimController eyes;

		public Instance(IStateMachineTarget master, Def def)
			: base(master)
		{
		}

		public bool IsBlinking()
		{
			return IsInsideState(base.sm.blinking);
		}

		public void Blink()
		{
			GoTo(base.sm.blinking);
		}
	}

	public State satisfied;

	public State blinking;

	public TargetParameter eyes;

	private static HashedString HASH_SNAPTO_EYES = "snapto_eyes";

	[CompilerGenerated]
	private static StateMachine<BlinkMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static StateMachine<BlinkMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Func<Instance, float> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static StateMachine<BlinkMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static Action<Instance, float> _003C_003Ef__mg_0024cache5;

	[CompilerGenerated]
	private static StateMachine<BlinkMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache6;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		root.Enter(CreateEyes).Exit(DestroyEyes);
		satisfied.ScheduleGoTo(GetRandomBlinkTime, blinking);
		blinking.EnterTransition(satisfied, GameStateMachine<BlinkMonitor, Instance, IStateMachineTarget, object>.Not(CanBlink)).Enter(BeginBlinking).Update(UpdateBlinking, UpdateRate.RENDER_EVERY_TICK, false)
			.Target(eyes)
			.OnAnimQueueComplete(satisfied)
			.Exit(EndBlinking);
	}

	private static bool CanBlink(Instance smi)
	{
		return SpeechMonitor.IsAllowedToPlaySpeech(smi.gameObject) && smi.Get<Navigator>().CurrentNavType != NavType.Ladder;
	}

	private static float GetRandomBlinkTime(Instance smi)
	{
		return UnityEngine.Random.Range(TuningData<Tuning>.Get().randomBlinkIntervalMin, TuningData<Tuning>.Get().randomBlinkIntervalMax);
	}

	private static void CreateEyes(Instance smi)
	{
		smi.eyes = Util.KInstantiate(Assets.GetPrefab(EyeAnimation.ID), null, null).GetComponent<KBatchedAnimController>();
		smi.eyes.gameObject.SetActive(true);
		smi.sm.eyes.Set(smi.eyes.gameObject, smi);
	}

	private static void DestroyEyes(Instance smi)
	{
		if ((UnityEngine.Object)smi.eyes != (UnityEngine.Object)null)
		{
			Util.KDestroyGameObject(smi.eyes);
			smi.eyes = null;
		}
	}

	public static void BeginBlinking(Instance smi)
	{
		string s = "eyes1";
		smi.eyes.Play(s, KAnim.PlayMode.Once, 1f, 0f);
		UpdateBlinking(smi, 0f);
	}

	public static void EndBlinking(Instance smi)
	{
		smi.GetComponent<SymbolOverrideController>().RemoveSymbolOverride(HASH_SNAPTO_EYES, 3);
	}

	public static void UpdateBlinking(Instance smi, float dt)
	{
		int currentFrameIndex = smi.eyes.GetCurrentFrameIndex();
		KAnimBatch batch = smi.eyes.GetBatch();
		if (currentFrameIndex != -1 && batch != null)
		{
			KAnim.Anim.Frame frame = smi.eyes.GetBatch().group.data.GetFrame(currentFrameIndex);
			if (!(frame == KAnim.Anim.Frame.InvalidFrame))
			{
				HashedString hash = HashedString.Invalid;
				for (int i = 0; i < frame.numElements; i++)
				{
					int num = frame.firstElementIdx + i;
					if (num < batch.group.data.frameElements.Count)
					{
						KAnim.Anim.FrameElement frameElement = batch.group.data.frameElements[num];
						if (!(frameElement.symbol == HashedString.Invalid))
						{
							hash = frameElement.symbol;
							break;
						}
					}
				}
				smi.GetComponent<SymbolOverrideController>().AddSymbolOverride(HASH_SNAPTO_EYES, smi.eyes.AnimFiles[0].GetData().build.GetSymbol(hash), 3);
			}
		}
	}
}
