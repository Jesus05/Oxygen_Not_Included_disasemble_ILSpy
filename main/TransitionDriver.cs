using System.Collections.Generic;
using UnityEngine;

public class TransitionDriver
{
	public class OverrideLayer
	{
		public OverrideLayer(Navigator navigator)
		{
		}

		public virtual void Destroy()
		{
		}

		public virtual void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
		{
		}

		public virtual void UpdateTransition(Navigator navigator, Navigator.ActiveTransition transition)
		{
		}

		public virtual void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
		{
		}
	}

	private Navigator.ActiveTransition transition;

	private Navigator navigator;

	private Vector3 targetPos;

	private bool isComplete;

	private Brain brain;

	public List<OverrideLayer> overrideLayers = new List<OverrideLayer>();

	private LoggerFS log;

	public TransitionDriver(Navigator navigator)
	{
		log = new LoggerFS("TransitionDriver", 35);
	}

	public void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		foreach (OverrideLayer overrideLayer in overrideLayers)
		{
			overrideLayer.BeginTransition(navigator, transition);
		}
		this.navigator = navigator;
		this.transition = transition;
		isComplete = false;
		Grid.SceneLayer sceneLayer = navigator.sceneLayer;
		if (transition.navGridTransition.start == NavType.Tube || transition.navGridTransition.end == NavType.Tube)
		{
			sceneLayer = Grid.SceneLayer.BuildingUse;
		}
		else if (transition.navGridTransition.start == NavType.Solid && transition.navGridTransition.end == NavType.Solid)
		{
			KBatchedAnimController component = navigator.GetComponent<KBatchedAnimController>();
			sceneLayer = Grid.SceneLayer.FXFront;
			component.SetSceneLayer(sceneLayer);
		}
		else if (transition.navGridTransition.start == NavType.Solid || transition.navGridTransition.end == NavType.Solid)
		{
			KBatchedAnimController component2 = navigator.GetComponent<KBatchedAnimController>();
			component2.SetSceneLayer(sceneLayer);
		}
		int cell = Grid.PosToCell(navigator);
		int cell2 = Grid.OffsetCell(cell, transition.x, transition.y);
		targetPos = Grid.CellToPosCBC(cell2, sceneLayer);
		if (transition.isLooping)
		{
			KAnimControllerBase component3 = navigator.GetComponent<KAnimControllerBase>();
			component3.PlaySpeedMultiplier = transition.animSpeed;
			bool flag = transition.preAnim != (HashedString)string.Empty;
			bool flag2 = component3.CurrentAnim != null && (HashedString)component3.CurrentAnim.name == transition.anim;
			if (flag && component3.CurrentAnim != null && (HashedString)component3.CurrentAnim.name == transition.preAnim)
			{
				component3.ClearQueue();
				component3.Queue(transition.anim, KAnim.PlayMode.Loop, 1f, 0f);
			}
			else if (flag2)
			{
				if (component3.PlayMode != 0)
				{
					component3.ClearQueue();
					component3.Queue(transition.anim, KAnim.PlayMode.Loop, 1f, 0f);
				}
			}
			else if (flag)
			{
				component3.Play(transition.preAnim, KAnim.PlayMode.Once, 1f, 0f);
				component3.Queue(transition.anim, KAnim.PlayMode.Loop, 1f, 0f);
			}
			else
			{
				component3.Play(transition.anim, KAnim.PlayMode.Loop, 1f, 0f);
			}
		}
		else if (transition.anim != (HashedString)null)
		{
			KAnimControllerBase component4 = navigator.GetComponent<KAnimControllerBase>();
			component4.PlaySpeedMultiplier = transition.animSpeed;
			component4.Play(transition.anim, KAnim.PlayMode.Once, 1f, 0f);
			navigator.Subscribe(-1061186183, OnAnimComplete);
		}
		if (transition.navGridTransition.y != 0)
		{
			if (transition.navGridTransition.start == NavType.RightWall)
			{
				navigator.GetComponent<Facing>().SetFacing(transition.navGridTransition.y < 0);
			}
			else if (transition.navGridTransition.start == NavType.LeftWall)
			{
				navigator.GetComponent<Facing>().SetFacing(transition.navGridTransition.y > 0);
			}
		}
		if (transition.navGridTransition.x != 0)
		{
			if (transition.navGridTransition.start == NavType.Ceiling)
			{
				navigator.GetComponent<Facing>().SetFacing(transition.navGridTransition.x > 0);
			}
			else if (transition.navGridTransition.start != NavType.LeftWall && transition.navGridTransition.start != NavType.RightWall)
			{
				navigator.GetComponent<Facing>().SetFacing(transition.navGridTransition.x < 0);
			}
		}
		brain = navigator.GetComponent<Brain>();
	}

	public void UpdateTransition(float dt)
	{
		if ((Object)this.navigator == (Object)null)
		{
			return;
		}
		foreach (OverrideLayer overrideLayer in overrideLayers)
		{
			overrideLayer.UpdateTransition(this.navigator, transition);
		}
		if (!isComplete && transition.isCompleteCB != null)
		{
			isComplete = transition.isCompleteCB();
		}
		if ((Object)brain != (Object)null && !isComplete)
		{
			goto IL_00ae;
		}
		goto IL_00ae;
		IL_00ae:
		if (transition.isLooping)
		{
			float speed = transition.speed;
			Vector3 position = this.navigator.transform.GetPosition();
			if (transition.x > 0)
			{
				position.x += dt * speed;
				if (position.x > targetPos.x)
				{
					isComplete = true;
				}
			}
			else if (transition.x < 0)
			{
				position.x -= dt * speed;
				if (position.x < targetPos.x)
				{
					isComplete = true;
				}
			}
			else
			{
				position.x = targetPos.x;
			}
			if (transition.y > 0)
			{
				position.y += dt * speed;
				if (position.y > targetPos.y)
				{
					isComplete = true;
				}
			}
			else if (transition.y < 0)
			{
				position.y -= dt * speed;
				if (position.y < targetPos.y)
				{
					isComplete = true;
				}
			}
			else
			{
				position.y = targetPos.y;
			}
			this.navigator.transform.SetPosition(position);
		}
		if (isComplete)
		{
			isComplete = false;
			Navigator navigator = this.navigator;
			navigator.SetCurrentNavType(transition.end);
			navigator.transform.SetPosition(targetPos);
			EndTransition();
			navigator.AdvancePath(true);
		}
	}

	private void OnAnimComplete(object data)
	{
		if ((Object)navigator != (Object)null)
		{
			navigator.Unsubscribe(-1061186183, OnAnimComplete);
		}
		isComplete = true;
	}

	public void EndTransition()
	{
		if ((Object)this.navigator != (Object)null)
		{
			Navigator navigator = this.navigator;
			foreach (OverrideLayer overrideLayer in overrideLayers)
			{
				overrideLayer.EndTransition(this.navigator, transition);
			}
			this.navigator = null;
			navigator.GetComponent<KAnimControllerBase>().PlaySpeedMultiplier = 1f;
			navigator.Unsubscribe(-1061186183, OnAnimComplete);
			Brain component = navigator.GetComponent<Brain>();
			if ((Object)component != (Object)null)
			{
				component.Resume("move_handler");
			}
		}
	}
}
