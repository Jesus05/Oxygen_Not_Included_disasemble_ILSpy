using System;
using System.Collections.Generic;
using UnityEngine;

public class KInputManager
{
	protected List<KInputController> mControllers = new List<KInputController>();

	public static bool isMousePosLocked;

	public static Vector3 lockedMousePos;

	public static bool isFocused
	{
		get;
		private set;
	}

	public static long lastUserActionTicks
	{
		get;
		private set;
	}

	public KInputManager()
	{
		lastUserActionTicks = DateTime.Now.Ticks;
		isFocused = true;
	}

	public static void SetUserActive()
	{
		if (isFocused)
		{
			lastUserActionTicks = DateTime.Now.Ticks;
		}
	}

	public void AddController(KInputController controller)
	{
		mControllers.Add(controller);
	}

	public KInputController GetController(int controller_index)
	{
		DebugUtil.Assert(controller_index < mControllers.Count, "Assert!", string.Empty, string.Empty);
		return mControllers[controller_index];
	}

	public int GetControllerCount()
	{
		return mControllers.Count;
	}

	public KInputController GetDefaultController()
	{
		return GetController(0);
	}

	public virtual void Update()
	{
		if (isFocused)
		{
			for (int i = 0; i < mControllers.Count; i++)
			{
				mControllers[i].Update();
			}
			Dispatch();
		}
	}

	public virtual void Dispatch()
	{
		if (isFocused)
		{
			for (int i = 0; i < mControllers.Count; i++)
			{
				mControllers[i].Dispatch();
			}
		}
	}

	public virtual void OnApplicationFocus(bool focus)
	{
		isFocused = focus;
		SetUserActive();
		if (!isFocused)
		{
			foreach (KInputController mController in mControllers)
			{
				mController.HandleCancelInput();
			}
		}
	}

	public static Vector3 GetMousePos()
	{
		if (isMousePosLocked)
		{
			return lockedMousePos;
		}
		return Input.mousePosition;
	}
}
