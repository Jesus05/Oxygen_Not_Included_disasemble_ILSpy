using KSerialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Effects : KMonoBehaviour, ISaveLoadable, ISim1000ms
	{
		[Serializable]
		private struct SaveLoadEffect
		{
			public string id;

			public float timeRemaining;
		}

		[Serialize]
		private SaveLoadEffect[] saveLoadEffects;

		private List<EffectInstance> effects = new List<EffectInstance>();

		private List<EffectInstance> effectsThatExpire = new List<EffectInstance>();

		private List<Effect> effectImmunites = new List<Effect>();

		protected override void OnPrefabInit()
		{
			autoRegisterSimRender = false;
		}

		protected override void OnSpawn()
		{
			if (saveLoadEffects != null)
			{
				SaveLoadEffect[] array = saveLoadEffects;
				for (int i = 0; i < array.Length; i++)
				{
					SaveLoadEffect saveLoadEffect = array[i];
					if (Db.Get().effects.Exists(saveLoadEffect.id))
					{
						Effect effect = Db.Get().effects.Get(saveLoadEffect.id);
						EffectInstance effectInstance = Add(effect, true);
						if (effectInstance != null)
						{
							effectInstance.timeRemaining = saveLoadEffect.timeRemaining;
						}
					}
				}
			}
			if (effectsThatExpire.Count > 0)
			{
				SimAndRenderScheduler.instance.Add(this, simRenderLoadBalance);
			}
		}

		public EffectInstance Get(string effect_id)
		{
			foreach (EffectInstance effect in effects)
			{
				if (effect.effect.Id == effect_id)
				{
					return effect;
				}
			}
			return null;
		}

		public EffectInstance Get(Effect effect)
		{
			foreach (EffectInstance effect2 in effects)
			{
				if (effect2.effect == effect)
				{
					return effect2;
				}
			}
			return null;
		}

		public EffectInstance Add(string effect_id, bool should_save)
		{
			Effect effect = Db.Get().effects.Get(effect_id);
			return Add(effect, should_save);
		}

		public EffectInstance Add(Effect effect, bool should_save)
		{
			if (effectImmunites.Contains(effect))
			{
				return null;
			}
			bool flag = true;
			Traits component = GetComponent<Traits>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				foreach (Trait trait in component.TraitList)
				{
					if (trait.ignoredEffects != null && Array.IndexOf(trait.ignoredEffects, effect.Id) != -1)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				Attributes attributes = this.GetAttributes();
				EffectInstance effectInstance = Get(effect);
				if (!string.IsNullOrEmpty(effect.stompGroup))
				{
					for (int num = effects.Count - 1; num >= 0; num--)
					{
						if (effects[num] != effectInstance && effects[num].effect.stompGroup == effect.stompGroup)
						{
							Remove(effects[num].effect);
						}
					}
				}
				if (effectInstance == null)
				{
					effectInstance = new EffectInstance(base.gameObject, effect, should_save);
					effect.AddTo(attributes);
					effects.Add(effectInstance);
					if (effect.duration > 0f)
					{
						effectsThatExpire.Add(effectInstance);
						if (effectsThatExpire.Count == 1)
						{
							SimAndRenderScheduler.instance.Add(this, simRenderLoadBalance);
						}
					}
					Trigger(-1901442097, effect);
				}
				effectInstance.timeRemaining = effect.duration;
				return effectInstance;
			}
			return null;
		}

		public void Remove(Effect effect)
		{
			Remove(effect.Id);
		}

		public void Remove(string effect_id)
		{
			for (int i = 0; i < effectsThatExpire.Count; i++)
			{
				if (effectsThatExpire[i].effect.Id == effect_id)
				{
					int index = effectsThatExpire.Count - 1;
					effectsThatExpire[i] = effectsThatExpire[index];
					effectsThatExpire.RemoveAt(index);
					if (effectsThatExpire.Count == 0)
					{
						SimAndRenderScheduler.instance.Remove(this);
					}
					break;
				}
			}
			int num = 0;
			while (true)
			{
				if (num >= effects.Count)
				{
					return;
				}
				if (effects[num].effect.Id == effect_id)
				{
					break;
				}
				num++;
			}
			Attributes attributes = this.GetAttributes();
			EffectInstance effectInstance = effects[num];
			effectInstance.OnCleanUp();
			Effect effect = effectInstance.effect;
			effect.RemoveFrom(attributes);
			int index2 = effects.Count - 1;
			effects[num] = effects[index2];
			effects.RemoveAt(index2);
			Trigger(-1157678353, effect);
		}

		public bool HasEffect(string effect_id)
		{
			foreach (EffectInstance effect in effects)
			{
				if (effect.effect.Id == effect_id)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasEffect(Effect effect)
		{
			foreach (EffectInstance effect2 in effects)
			{
				if (effect2.effect == effect)
				{
					return true;
				}
			}
			return false;
		}

		public void Sim1000ms(float dt)
		{
			for (int i = 0; i < effectsThatExpire.Count; i++)
			{
				EffectInstance effectInstance = effectsThatExpire[i];
				if (effectInstance.IsExpired())
				{
					Remove(effectInstance.effect);
				}
				effectInstance.timeRemaining -= dt;
			}
		}

		public void AddImmunity(Effect effect)
		{
			effectImmunites.Add(effect);
		}

		public void RemoveImmunity(Effect effect)
		{
			effectImmunites.Remove(effect);
		}

		[OnSerializing]
		internal void OnSerializing()
		{
			List<SaveLoadEffect> list = new List<SaveLoadEffect>();
			foreach (EffectInstance effect in effects)
			{
				if (effect.shouldSave)
				{
					SaveLoadEffect saveLoadEffect = default(SaveLoadEffect);
					saveLoadEffect.id = effect.effect.Id;
					saveLoadEffect.timeRemaining = effect.timeRemaining;
					SaveLoadEffect item = saveLoadEffect;
					list.Add(item);
				}
			}
			saveLoadEffects = list.ToArray();
		}

		public List<EffectInstance> GetTimeLimitedEffects()
		{
			return effectsThatExpire;
		}
	}
}
