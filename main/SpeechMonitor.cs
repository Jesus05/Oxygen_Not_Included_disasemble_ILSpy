using FMOD.Studio;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpeechMonitor : GameStateMachine<SpeechMonitor, SpeechMonitor.Instance>
{
	public class Def : BaseDef
	{
	}

	public class Tuning : TuningData<Tuning>
	{
		public float randomSpeechIntervalMin;

		public float randomSpeechIntervalMax;

		public int speechCount;
	}

	public new class Instance : GameInstance
	{
		public KBatchedAnimController mouth;

		public string speechPrefix = "happy";

		public string voiceEvent;

		public EventInstance ev;

		public Instance(IStateMachineTarget master, Def def)
			: base(master)
		{
		}

		public bool IsPlayingSpeech()
		{
			return IsInsideState(base.sm.talking);
		}

		public void PlaySpeech(string speech_prefix, string voice_event)
		{
			speechPrefix = speech_prefix;
			voiceEvent = voice_event;
			GoTo(base.sm.talking);
		}

		public void DrawMouth()
		{
			KAnim.Anim.FrameElement firstFrameElement = GetFirstFrameElement(base.smi.mouth);
			if (!(firstFrameElement.symbol == HashedString.Invalid))
			{
				KAnim.Build.Symbol symbol = base.smi.mouth.AnimFiles[0].GetData().build.GetSymbol(firstFrameElement.symbol);
				KBatchedAnimController component = GetComponent<KBatchedAnimController>();
				GetComponent<SymbolOverrideController>().AddSymbolOverride(HASH_SNAPTO_MOUTH, base.smi.mouth.AnimFiles[0].GetData().build.GetSymbol(firstFrameElement.symbol), 3);
				KAnim.Build.Symbol symbol2 = KAnimBatchManager.Instance().GetBatchGroupData(component.batchGroupID).GetSymbol(HASH_SNAPTO_MOUTH);
				KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(symbol.build.batchTag);
				KAnim.Build.SymbolFrameInstance symbol_frame_instance = batchGroupData.symbolFrameInstances[symbol.firstFrameIdx + firstFrameElement.frame];
				symbol_frame_instance.buildImageIdx = GetComponent<SymbolOverrideController>().GetAtlasIdx(symbol.build.GetTexture(0));
				component.SetSymbolOverride(symbol2.firstFrameIdx, symbol_frame_instance);
			}
		}
	}

	public State satisfied;

	public State talking;

	public static string PREFIX_SAD = "sad";

	public static string PREFIX_HAPPY = "happy";

	public TargetParameter mouth;

	private static HashedString HASH_SNAPTO_MOUTH = "snapto_mouth";

	[CompilerGenerated]
	private static StateMachine<SpeechMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static StateMachine<SpeechMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static StateMachine<SpeechMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Action<Instance, float> _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static StateMachine<SpeechMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache4;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		root.Enter(CreateMouth).Exit(DestroyMouth);
		satisfied.DoNothing();
		talking.Enter(BeginTalking).Update(UpdateTalking, UpdateRate.RENDER_EVERY_TICK, false).Target(mouth)
			.OnAnimQueueComplete(satisfied)
			.Exit(EndTalking);
	}

	private static void CreateMouth(Instance smi)
	{
		smi.mouth = Util.KInstantiate(Assets.GetPrefab(MouthAnimation.ID), null, null).GetComponent<KBatchedAnimController>();
		smi.mouth.gameObject.SetActive(true);
		smi.sm.mouth.Set(smi.mouth.gameObject, smi);
	}

	private static void DestroyMouth(Instance smi)
	{
		if ((UnityEngine.Object)smi.mouth != (UnityEngine.Object)null)
		{
			Util.KDestroyGameObject(smi.mouth);
			smi.mouth = null;
		}
	}

	private static string GetRandomSpeechAnim(string speech_prefix)
	{
		return speech_prefix + UnityEngine.Random.Range(1, TuningData<Tuning>.Get().speechCount).ToString();
	}

	public static bool IsAllowedToPlaySpeech(GameObject go)
	{
		if (go.HasTag(GameTags.Dead))
		{
			return false;
		}
		if (go.GetComponent<Navigator>().IsMoving())
		{
			return true;
		}
		KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
		KAnim.Anim currentAnim = component.GetCurrentAnim();
		if (currentAnim == null)
		{
			return true;
		}
		return GameAudioSheets.Get().IsAnimAllowedToPlaySpeech(currentAnim);
	}

	public static void BeginTalking(Instance smi)
	{
		smi.ev.clearHandle();
		if (smi.voiceEvent != null)
		{
			smi.ev = VoiceSoundEvent.PlayVoice(smi.voiceEvent, smi.GetComponent<KBatchedAnimController>(), 0f, false);
		}
		if (smi.ev.isValid())
		{
			smi.mouth.Play(GetRandomSpeechAnim(smi.speechPrefix), KAnim.PlayMode.Once, 1f, 0f);
			smi.mouth.Queue(GetRandomSpeechAnim(smi.speechPrefix), KAnim.PlayMode.Once, 1f, 0f);
			smi.mouth.Queue(GetRandomSpeechAnim(smi.speechPrefix), KAnim.PlayMode.Once, 1f, 0f);
			smi.mouth.Queue(GetRandomSpeechAnim(smi.speechPrefix), KAnim.PlayMode.Once, 1f, 0f);
		}
		else
		{
			smi.mouth.Play(GetRandomSpeechAnim(smi.speechPrefix), KAnim.PlayMode.Once, 1f, 0f);
			smi.mouth.Queue(GetRandomSpeechAnim(smi.speechPrefix), KAnim.PlayMode.Once, 1f, 0f);
		}
		UpdateTalking(smi, 0f);
	}

	public static void EndTalking(Instance smi)
	{
		smi.GetComponent<SymbolOverrideController>().RemoveSymbolOverride(HASH_SNAPTO_MOUTH, 3);
	}

	public static KAnim.Anim.FrameElement GetFirstFrameElement(KBatchedAnimController controller)
	{
		KAnim.Anim.FrameElement result = default(KAnim.Anim.FrameElement);
		result.symbol = HashedString.Invalid;
		int currentFrameIndex = controller.GetCurrentFrameIndex();
		KAnimBatch batch = controller.GetBatch();
		if (currentFrameIndex == -1 || batch == null)
		{
			return result;
		}
		KAnim.Anim.Frame frame = controller.GetBatch().group.data.GetFrame(currentFrameIndex);
		if (frame == KAnim.Anim.Frame.InvalidFrame)
		{
			return result;
		}
		for (int i = 0; i < frame.numElements; i++)
		{
			int num = frame.firstElementIdx + i;
			if (num < batch.group.data.frameElements.Count)
			{
				KAnim.Anim.FrameElement frameElement = batch.group.data.frameElements[num];
				if (!(frameElement.symbol == HashedString.Invalid))
				{
					result = frameElement;
					return result;
				}
			}
		}
		return result;
	}

	public static void UpdateTalking(Instance smi, float dt)
	{
		if (smi.ev.isValid())
		{
			smi.ev.getPlaybackState(out PLAYBACK_STATE state);
			if (state != 0 && state != PLAYBACK_STATE.STARTING)
			{
				smi.GoTo(smi.sm.satisfied);
				smi.ev.clearHandle();
				return;
			}
		}
		KAnim.Anim.FrameElement firstFrameElement = GetFirstFrameElement(smi.mouth);
		if (!(firstFrameElement.symbol == HashedString.Invalid))
		{
			smi.Get<SymbolOverrideController>().AddSymbolOverride(HASH_SNAPTO_MOUTH, smi.mouth.AnimFiles[0].GetData().build.GetSymbol(firstFrameElement.symbol), 3);
		}
	}
}
