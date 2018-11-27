using STRINGS;
using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{effect.Id}")]
	public class EffectInstance : ModifierInstance<Effect>
	{
		public Effect effect;

		public bool shouldSave;

		public StatusItem statusItem;

		public float timeRemaining;

		public Reactable reactable;

		public EffectInstance(GameObject game_object, Effect effect, bool should_save)
			: base(game_object, effect)
		{
			this.effect = effect;
			shouldSave = should_save;
			ConfigureStatusItem();
			if (effect.showInUI)
			{
				KSelectable component = base.gameObject.GetComponent<KSelectable>();
				if (!component.GetStatusItemGroup().HasStatusItemID(statusItem))
				{
					component.AddStatusItem(statusItem, this);
				}
			}
			if (effect.triggerFloatingText && (Object)PopFXManager.Instance != (Object)null)
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, effect.Name, game_object.transform, 1.5f, false);
			}
			if (!string.IsNullOrEmpty(effect.emoteAnim))
			{
				ReactionMonitor.Instance sMI = base.gameObject.GetSMI<ReactionMonitor.Instance>();
				if (sMI != null)
				{
					if (effect.emoteCooldown < 0f)
					{
						SelfEmoteReactable selfEmoteReactable = (SelfEmoteReactable)new SelfEmoteReactable(game_object, effect.Name + "_Emote", Db.Get().ChoreTypes.Emote, effect.emoteAnim, 100000f, 20f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
						{
							anim = (HashedString)"react"
						});
						selfEmoteReactable.AddPrecondition(NotInATube);
						if (effect.emotePreconditions != null)
						{
							foreach (Reactable.ReactablePrecondition emotePrecondition in effect.emotePreconditions)
							{
								selfEmoteReactable.AddPrecondition(emotePrecondition);
							}
						}
						sMI.AddOneshotReactable(selfEmoteReactable);
					}
					else
					{
						reactable = new SelfEmoteReactable(game_object, effect.Name + "_Emote", Db.Get().ChoreTypes.Emote, effect.emoteAnim, effect.emoteCooldown, 20f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
						{
							anim = (HashedString)"react"
						});
						reactable.AddPrecondition(NotInATube);
						if (effect.emotePreconditions != null)
						{
							foreach (Reactable.ReactablePrecondition emotePrecondition2 in effect.emotePreconditions)
							{
								reactable.AddPrecondition(emotePrecondition2);
							}
						}
					}
				}
			}
		}

		private bool NotInATube(GameObject go, Navigator.ActiveTransition transition)
		{
			return transition.navGridTransition.start != NavType.Tube && transition.navGridTransition.end != NavType.Tube;
		}

		public override void OnCleanUp()
		{
			if (statusItem != null)
			{
				KSelectable component = base.gameObject.GetComponent<KSelectable>();
				component.RemoveStatusItem(statusItem, false);
				statusItem = null;
			}
			if (reactable != null)
			{
				reactable.Cleanup();
				reactable = null;
			}
		}

		public float GetTimeRemaining()
		{
			return timeRemaining;
		}

		public bool IsExpired()
		{
			return effect.duration > 0f && timeRemaining <= 0f;
		}

		private void ConfigureStatusItem()
		{
			statusItem = new StatusItem(effect.Id, effect.Name, effect.description, "", effect.isBad ? StatusItem.IconType.Exclamation : StatusItem.IconType.Info, effect.isBad ? NotificationType.Bad : NotificationType.Neutral, false, OverlayModes.None.ID, 2);
			statusItem.resolveStringCallback = ResolveString;
			statusItem.resolveTooltipCallback = ResolveTooltip;
		}

		private string ResolveString(string str, object data)
		{
			return str;
		}

		private string ResolveTooltip(string str, object data)
		{
			string text = str;
			EffectInstance effectInstance = (EffectInstance)data;
			string text2 = Effect.CreateTooltip(effectInstance.effect, false, "\n");
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + "\n" + text2;
			}
			if (effectInstance.effect.duration > 0f)
			{
				text = text + "\n" + string.Format(DUPLICANTS.MODIFIERS.TIME_REMAINING, GameUtil.GetFormattedCycles(GetTimeRemaining(), "F1"));
			}
			return text;
		}
	}
}
