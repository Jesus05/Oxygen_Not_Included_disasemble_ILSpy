using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LoopingSoundManager : KMonoBehaviour, IRenderEveryTick
{
	public class Tuning : TuningData<Tuning>
	{
		public float velocityScale;
	}

	public struct Sound
	{
		[Flags]
		public enum Flags
		{
			PLAYING = 0x1,
			PAUSE_ON_GAME_PAUSED = 0x2,
			ENABLE_CULLING = 0x4,
			ENABLE_CAMERA_SCALED_POSITION = 0x8
		}

		public EventInstance ev;

		public Transform transform;

		public KBatchedAnimController animController;

		public float falloffDistanceSq;

		public HashedString path;

		public Vector2 pos;

		public Vector2 velocity;

		public HashedString firstParameter;

		public HashedString secondParameter;

		public float firstParameterValue;

		public float secondParameterValue;

		public Flags flags;

		public bool IsPlaying => (flags & Flags.PLAYING) != (Flags)0;

		public bool ShouldPauseOnGamePaused => (flags & Flags.PAUSE_ON_GAME_PAUSED) != (Flags)0;

		public bool IsCullingEnabled => (flags & Flags.ENABLE_CULLING) != (Flags)0;

		public bool ShouldCameraScalePosition => (flags & Flags.ENABLE_CAMERA_SCALED_POSITION) != (Flags)0;
	}

	private static LoopingSoundManager instance;

	private Dictionary<HashedString, LoopingSoundParameterUpdater> parameterUpdaters = new Dictionary<HashedString, LoopingSoundParameterUpdater>();

	private KCompactedVector<Sound> sounds = new KCompactedVector<Sound>(0);

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		instance = this;
		CollectParameterUpdaters();
	}

	protected override void OnSpawn()
	{
		if ((UnityEngine.Object)SpeedControlScreen.Instance != (UnityEngine.Object)null && (UnityEngine.Object)Game.Instance != (UnityEngine.Object)null)
		{
			Game.Instance.Subscribe(-1788536802, instance.OnPauseChanged);
		}
	}

	private void CollectParameterUpdaters()
	{
		foreach (Type currentDomainType in App.GetCurrentDomainTypes())
		{
			if (!currentDomainType.IsAbstract)
			{
				bool flag = false;
				for (Type baseType = currentDomainType.BaseType; baseType != null; baseType = baseType.BaseType)
				{
					if (baseType == typeof(LoopingSoundParameterUpdater))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					LoopingSoundParameterUpdater loopingSoundParameterUpdater = (LoopingSoundParameterUpdater)Activator.CreateInstance(currentDomainType);
					DebugUtil.Assert(!parameterUpdaters.ContainsKey(loopingSoundParameterUpdater.parameter));
					parameterUpdaters[loopingSoundParameterUpdater.parameter] = loopingSoundParameterUpdater;
				}
			}
		}
	}

	public void UpdateFirstParameter(HandleVector<int>.Handle handle, HashedString parameter, float value)
	{
		Sound data = sounds.GetData(handle);
		data.firstParameterValue = value;
		data.firstParameter = parameter;
		if (data.IsPlaying)
		{
			data.ev.setParameterValueByIndex(GetSoundDescription(data.path).GetParameterIdx(parameter), value);
		}
		sounds.SetData(handle, data);
	}

	public void UpdateSecondParameter(HandleVector<int>.Handle handle, HashedString parameter, float value)
	{
		Sound data = sounds.GetData(handle);
		data.secondParameterValue = value;
		data.secondParameter = parameter;
		if (data.IsPlaying)
		{
			data.ev.setParameterValueByIndex(GetSoundDescription(data.path).GetParameterIdx(parameter), value);
		}
		sounds.SetData(handle, data);
	}

	public void UpdateVelocity(HandleVector<int>.Handle handle, Vector2 velocity)
	{
		Sound data = sounds.GetData(handle);
		data.velocity = velocity;
		sounds.SetData(handle, data);
	}

	public void RenderEveryTick(float dt)
	{
		ListPool<Sound, LoopingSoundManager>.PooledList pooledList = ListPool<Sound, LoopingSoundManager>.Allocate();
		ListPool<int, LoopingSoundManager>.PooledList pooledList2 = ListPool<int, LoopingSoundManager>.Allocate();
		ListPool<int, LoopingSoundManager>.PooledList pooledList3 = ListPool<int, LoopingSoundManager>.Allocate();
		List<Sound> dataList = sounds.GetDataList();
		bool flag = Time.timeScale == 0f;
		SoundCuller soundCuller = CameraController.Instance.soundCuller;
		for (int i = 0; i < dataList.Count; i++)
		{
			Sound sound = dataList[i];
			if ((UnityEngine.Object)sound.transform != (UnityEngine.Object)null)
			{
				sound.pos = sound.transform.GetPosition();
				if ((UnityEngine.Object)sound.animController != (UnityEngine.Object)null)
				{
					Vector3 offset = sound.animController.Offset;
					sound.pos.x += offset.x;
					sound.pos.y += offset.y;
				}
			}
			bool flag2 = !sound.IsCullingEnabled || (sound.ShouldCameraScalePosition && soundCuller.IsAudible(sound.pos, sound.falloffDistanceSq)) || soundCuller.IsAudibleNoCameraScaling(sound.pos, sound.falloffDistanceSq);
			bool isPlaying = sound.IsPlaying;
			if (flag2)
			{
				pooledList.Add(sound);
				if (!isPlaying)
				{
					SoundDescription soundDescription = GetSoundDescription(sound.path);
					sound.ev = KFMOD.CreateInstance(soundDescription.path);
					dataList[i] = sound;
					pooledList2.Add(i);
				}
			}
			else if (isPlaying)
			{
				pooledList3.Add(i);
			}
		}
		LoopingSoundParameterUpdater.Sound sound2;
		foreach (int item in pooledList2)
		{
			Sound value = dataList[item];
			SoundDescription soundDescription2 = GetSoundDescription(value.path);
			value.ev.setPaused(flag && value.ShouldPauseOnGamePaused);
			Vector2 v = value.pos;
			if (value.ShouldCameraScalePosition)
			{
				v = SoundEvent.GetCameraScaledPosition(v);
			}
			value.ev.set3DAttributes(RuntimeUtils.To3DAttributes(v));
			value.ev.start();
			value.flags |= Sound.Flags.PLAYING;
			if (value.firstParameter != HashedString.Invalid)
			{
				value.ev.setParameterValueByIndex(soundDescription2.GetParameterIdx(value.firstParameter), value.firstParameterValue);
			}
			if (value.secondParameter != HashedString.Invalid)
			{
				value.ev.setParameterValueByIndex(soundDescription2.GetParameterIdx(value.secondParameter), value.secondParameterValue);
			}
			sound2 = default(LoopingSoundParameterUpdater.Sound);
			sound2.ev = value.ev;
			sound2.path = value.path;
			sound2.description = soundDescription2;
			sound2.transform = value.transform;
			LoopingSoundParameterUpdater.Sound sound3 = sound2;
			SoundDescription.Parameter[] parameters = soundDescription2.parameters;
			for (int j = 0; j < parameters.Length; j++)
			{
				SoundDescription.Parameter parameter = parameters[j];
				LoopingSoundParameterUpdater value2 = null;
				if (parameterUpdaters.TryGetValue(parameter.name, out value2))
				{
					value2.Add(sound3);
				}
			}
			dataList[item] = value;
		}
		pooledList2.Recycle();
		foreach (int item2 in pooledList3)
		{
			Sound value3 = dataList[item2];
			SoundDescription soundDescription3 = GetSoundDescription(value3.path);
			sound2 = default(LoopingSoundParameterUpdater.Sound);
			sound2.ev = value3.ev;
			sound2.path = value3.path;
			sound2.description = soundDescription3;
			sound2.transform = value3.transform;
			LoopingSoundParameterUpdater.Sound sound4 = sound2;
			SoundDescription.Parameter[] parameters2 = soundDescription3.parameters;
			for (int k = 0; k < parameters2.Length; k++)
			{
				SoundDescription.Parameter parameter2 = parameters2[k];
				LoopingSoundParameterUpdater value4 = null;
				if (parameterUpdaters.TryGetValue(parameter2.name, out value4))
				{
					value4.Remove(sound4);
				}
			}
			if (value3.ShouldCameraScalePosition)
			{
				value3.ev.stop(STOP_MODE.IMMEDIATE);
			}
			else
			{
				value3.ev.stop(STOP_MODE.ALLOWFADEOUT);
			}
			value3.flags &= ~Sound.Flags.PLAYING;
			value3.ev.release();
			dataList[item2] = value3;
		}
		pooledList3.Recycle();
		float velocityScale = TuningData<Tuning>.Get().velocityScale;
		foreach (Sound item3 in pooledList)
		{
			Sound current3 = item3;
			ATTRIBUTES_3D attributes = SoundEvent.GetCameraScaledPosition(current3.pos).To3DAttributes();
			attributes.velocity = (current3.velocity * velocityScale).ToFMODVector();
			current3.ev.set3DAttributes(attributes);
		}
		foreach (KeyValuePair<HashedString, LoopingSoundParameterUpdater> parameterUpdater in parameterUpdaters)
		{
			parameterUpdater.Value.Update(dt);
		}
		pooledList.Recycle();
	}

	public static LoopingSoundManager Get()
	{
		return instance;
	}

	public void StopAllSounds()
	{
		foreach (Sound data in sounds.GetDataList())
		{
			Sound current = data;
			if (current.IsPlaying)
			{
				current.ev.stop(STOP_MODE.IMMEDIATE);
				current.ev.release();
			}
		}
	}

	private SoundDescription GetSoundDescription(HashedString path)
	{
		return KFMOD.GetSoundEventDescription(path);
	}

	public HandleVector<int>.Handle Add(string path, Vector2 pos, Transform transform = null, bool pause_on_game_pause = true, bool enable_culling = true, bool enable_camera_scaled_position = true)
	{
		SoundDescription soundEventDescription = KFMOD.GetSoundEventDescription(path);
		Sound.Flags flags = (Sound.Flags)0;
		if (pause_on_game_pause)
		{
			flags |= Sound.Flags.PAUSE_ON_GAME_PAUSED;
		}
		if (enable_culling)
		{
			flags |= Sound.Flags.ENABLE_CULLING;
		}
		if (enable_camera_scaled_position)
		{
			flags |= Sound.Flags.ENABLE_CAMERA_SCALED_POSITION;
		}
		KBatchedAnimController animController = null;
		if ((UnityEngine.Object)transform != (UnityEngine.Object)null)
		{
			animController = transform.GetComponent<KBatchedAnimController>();
		}
		Sound sound = default(Sound);
		sound.transform = transform;
		sound.animController = animController;
		sound.falloffDistanceSq = soundEventDescription.falloffDistanceSq;
		sound.path = path;
		sound.pos = pos;
		sound.flags = flags;
		sound.firstParameter = HashedString.Invalid;
		sound.secondParameter = HashedString.Invalid;
		Sound initial_data = sound;
		return sounds.Allocate(initial_data);
	}

	public static HandleVector<int>.Handle StartSound(string path, Vector3 pos, bool pause_on_game_pause = true, bool enable_culling = true)
	{
		if (!string.IsNullOrEmpty(path))
		{
			return Get().Add(path, pos, null, pause_on_game_pause, enable_culling, true);
		}
		Debug.LogWarning("Missing sound", null);
		return HandleVector<int>.InvalidHandle;
	}

	public static void StopSound(HandleVector<int>.Handle handle)
	{
		if (!((UnityEngine.Object)Get() == (UnityEngine.Object)null))
		{
			Sound data = Get().sounds.GetData(handle);
			if (data.IsPlaying)
			{
				data.ev.stop(STOP_MODE.ALLOWFADEOUT);
				data.ev.release();
				SoundDescription soundEventDescription = KFMOD.GetSoundEventDescription(data.path);
				SoundDescription.Parameter[] parameters = soundEventDescription.parameters;
				for (int i = 0; i < parameters.Length; i++)
				{
					SoundDescription.Parameter parameter = parameters[i];
					LoopingSoundParameterUpdater value = null;
					if (Get().parameterUpdaters.TryGetValue(parameter.name, out value))
					{
						LoopingSoundParameterUpdater.Sound sound = default(LoopingSoundParameterUpdater.Sound);
						sound.ev = data.ev;
						sound.path = data.path;
						sound.description = soundEventDescription;
						sound.transform = data.transform;
						LoopingSoundParameterUpdater.Sound sound2 = sound;
						value.Remove(sound2);
					}
				}
			}
			Get().sounds.Free(handle);
		}
	}

	private void OnPauseChanged(object data)
	{
		bool flag = (bool)data;
		foreach (Sound data2 in sounds.GetDataList())
		{
			Sound current = data2;
			if (current.IsPlaying)
			{
				current.ev.setPaused(flag && current.ShouldPauseOnGamePaused);
			}
		}
	}
}
