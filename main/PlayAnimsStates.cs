internal class PlayAnimsStates : GameStateMachine<PlayAnimsStates, PlayAnimsStates.Instance, IStateMachineTarget, PlayAnimsStates.Def>
{
	public class Def : BaseDef
	{
		public Tag tag;

		public string[] anims;

		public bool loop;

		public string statusItemName;

		public string statusItemTooltip;

		public Def(Tag tag, bool loop, string anim, string status_item_name, string status_item_tooltip)
			: this(tag, loop, new string[1]
			{
				anim
			}, status_item_name, status_item_tooltip)
		{
		}

		public Def(Tag tag, bool loop, string[] anims, string status_item_name, string status_item_tooltip)
		{
			this.tag = tag;
			this.loop = loop;
			this.anims = anims;
			statusItemName = status_item_name;
			statusItemTooltip = status_item_tooltip;
		}

		public override string ToString()
		{
			return tag.ToString() + "(PlayAnimsStates)";
		}
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, def.tag);
		}

		public void PlayAnims()
		{
			if (base.def.anims != null && base.def.anims.Length != 0)
			{
				KBatchedAnimController component = GetComponent<KBatchedAnimController>();
				for (int i = 0; i < base.def.anims.Length; i++)
				{
					KAnim.PlayMode mode = KAnim.PlayMode.Once;
					if (base.def.loop && i == base.def.anims.Length - 1)
					{
						mode = KAnim.PlayMode.Loop;
					}
					if (i == 0)
					{
						component.Play(base.def.anims[i], mode, 1f, 0f);
					}
					else
					{
						component.Queue(base.def.anims[i], mode, 1f, 0f);
					}
				}
			}
		}

		public void HandleTagsChanged(object obj)
		{
			if (!base.smi.HasTag(base.smi.def.tag))
			{
				base.smi.GoTo((BaseState)null);
			}
		}
	}

	public State animating;

	public State done;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = animating;
		root.ToggleStatusItem("Unused", "Unused", category: Db.Get().StatusItemCategories.Main, icon: "", icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 63486, resolve_string_callback: (string str, Instance smi) => smi.def.statusItemName, resolve_tooltip_callback: (string str, Instance smi) => smi.def.statusItemTooltip);
		animating.Enter("PlayAnims", delegate(Instance smi)
		{
			smi.PlayAnims();
		}).OnAnimQueueComplete(done).EventHandler(GameHashes.TagsChanged, delegate(Instance smi, object obj)
		{
			smi.HandleTagsChanged(obj);
		});
		done.BehaviourComplete((Instance smi) => smi.def.tag, false);
	}
}
