using System;
using System.Collections.Generic;
using UnityEngine;

public class EmoteReactable : Reactable
{
	public class EmoteStep
	{
		public HashedString anim = HashedString.Invalid;

		public KAnim.PlayMode mode = KAnim.PlayMode.Once;

		public float timeout = -1f;

		public Action<GameObject> startcb;

		public Action<GameObject> finishcb;
	}

	private KBatchedAnimController kbac;

	public Expression expression;

	public Thought thought;

	private KAnimFile animset;

	private List<EmoteStep> emoteSteps = new List<EmoteStep>();

	private int currentStep = -1;

	private float elapsed;

	public EmoteReactable(GameObject gameObject, HashedString id, ChoreType chore_type, HashedString animset, int range_width = 15, int range_height = 8, float min_reactable_time = 0f, float min_reactor_time = 20f, float max_trigger_time = float.PositiveInfinity)
		: base(gameObject, id, chore_type, range_width, range_height, true, min_reactable_time, min_reactor_time, max_trigger_time)
	{
		this.animset = Assets.GetAnim(animset);
	}

	public EmoteReactable AddStep(EmoteStep step)
	{
		emoteSteps.Add(step);
		return this;
	}

	public EmoteReactable AddExpression(Expression expression)
	{
		this.expression = expression;
		return this;
	}

	public EmoteReactable AddThought(Thought thought)
	{
		this.thought = thought;
		return this;
	}

	public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
	{
		if ((UnityEngine.Object)reactor != (UnityEngine.Object)null)
		{
			return false;
		}
		if ((UnityEngine.Object)new_reactor == (UnityEngine.Object)null)
		{
			return false;
		}
		Navigator component = new_reactor.GetComponent<Navigator>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			return false;
		}
		if (!component.IsMoving())
		{
			return false;
		}
		if (component.CurrentNavType == NavType.Tube || component.CurrentNavType == NavType.Ladder || component.CurrentNavType == NavType.Pole)
		{
			return false;
		}
		return (UnityEngine.Object)gameObject != (UnityEngine.Object)new_reactor;
	}

	public override void Update(float dt)
	{
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null && (UnityEngine.Object)reactor != (UnityEngine.Object)null)
		{
			Facing component = reactor.GetComponent<Facing>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				component.Face(gameObject.transform.GetPosition());
			}
		}
		if (currentStep >= 0 && emoteSteps[currentStep].timeout > 0f && emoteSteps[currentStep].timeout < elapsed)
		{
			NextStep(null);
		}
		else
		{
			elapsed += dt;
		}
	}

	protected override void InternalBegin()
	{
		kbac = reactor.GetComponent<KBatchedAnimController>();
		kbac.AddAnimOverrides(animset, 0f);
		if (expression != null)
		{
			reactor.GetComponent<FaceGraph>().AddExpression(expression);
		}
		if (thought != null)
		{
			reactor.GetSMI<ThoughtGraph.Instance>().AddThought(thought);
		}
		NextStep(null);
	}

	protected override void InternalEnd()
	{
		if ((UnityEngine.Object)kbac != (UnityEngine.Object)null)
		{
			if (currentStep >= 0 && currentStep < emoteSteps.Count && emoteSteps[currentStep].timeout <= 0f)
			{
				kbac.onAnimComplete -= NextStep;
			}
			kbac.RemoveAnimOverrides(animset);
			kbac = null;
		}
		if ((UnityEngine.Object)reactor != (UnityEngine.Object)null)
		{
			if (expression != null)
			{
				reactor.GetComponent<FaceGraph>().RemoveExpression(expression);
			}
			if (thought != null)
			{
				reactor.GetSMI<ThoughtGraph.Instance>().RemoveThought(thought);
			}
		}
		currentStep = -1;
	}

	protected override void InternalCleanup()
	{
	}

	private void NextStep(HashedString finishedAnim)
	{
		if (currentStep >= 0 && emoteSteps[currentStep].timeout <= 0f)
		{
			kbac.onAnimComplete -= NextStep;
			if (emoteSteps[currentStep].finishcb != null)
			{
				emoteSteps[currentStep].finishcb(reactor);
			}
		}
		currentStep++;
		if (currentStep >= emoteSteps.Count)
		{
			End();
		}
		else
		{
			if (emoteSteps[currentStep].anim != HashedString.Invalid)
			{
				kbac.Play(emoteSteps[currentStep].anim, emoteSteps[currentStep].mode, 1f, 0f);
				if (kbac.IsStopped())
				{
					DebugUtil.DevAssert(false, "Emote is missing anim: " + emoteSteps[currentStep].anim);
					emoteSteps[currentStep].timeout = 0.25f;
				}
			}
			if (emoteSteps[currentStep].timeout <= 0f)
			{
				kbac.onAnimComplete += NextStep;
			}
			else
			{
				elapsed = 0f;
			}
			if (emoteSteps[currentStep].startcb != null)
			{
				emoteSteps[currentStep].startcb(reactor);
			}
		}
	}
}
