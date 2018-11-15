using FMOD.Studio;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UserNavigation : KMonoBehaviour
{
	[Serializable]
	private struct NavPoint
	{
		public Vector3 pos;

		public float orthoSize;

		public static readonly NavPoint Invalid = new NavPoint
		{
			pos = Vector3.zero,
			orthoSize = 0f
		};

		public bool IsValid()
		{
			return orthoSize != 0f;
		}
	}

	[Serialize]
	private List<NavPoint> hotkeyNavPoints = new List<NavPoint>();

	public UserNavigation()
	{
		for (Action action = Action.SetUserNav1; action <= Action.SetUserNav10; action++)
		{
			hotkeyNavPoints.Add(NavPoint.Invalid);
		}
	}

	private static int GetIndex(Action action)
	{
		int result = -1;
		if (Action.SetUserNav1 <= action && action <= Action.SetUserNav10)
		{
			result = (int)(action - 15);
		}
		else if (Action.GotoUserNav1 <= action && action <= Action.GotoUserNav10)
		{
			result = (int)(action - 25);
		}
		return result;
	}

	private void SetHotkeyNavPoint(Action action, Vector3 pos, float ortho_size)
	{
		int index = GetIndex(action);
		if (index >= 0)
		{
			hotkeyNavPoints[index] = new NavPoint
			{
				pos = pos,
				orthoSize = ortho_size
			};
			EventInstance instance = KFMOD.BeginOneShot(GlobalAssets.GetSound("UserNavPoint_set", false), CameraController.Instance.GetVerticallyScaledPosition(pos));
			instance.setParameterValue("userNavPoint_ID", (float)index);
			KFMOD.EndOneShot(instance);
		}
	}

	private void GoToHotkeyNavPoint(Action action)
	{
		int index = GetIndex(action);
		if (index >= 0)
		{
			NavPoint navPoint = hotkeyNavPoints[index];
			if (navPoint.IsValid())
			{
				CameraController instance = CameraController.Instance;
				instance.SetTargetPos(navPoint.pos, navPoint.orthoSize, true);
				EventInstance instance2 = KFMOD.BeginOneShot(GlobalAssets.GetSound("UserNavPoint_recall", false), instance.GetVerticallyScaledPosition(instance.transform.GetPosition()));
				instance2.setParameterValue("userNavPoint_ID", (float)index);
				KFMOD.EndOneShot(instance2);
			}
		}
	}

	public bool Handle(KButtonEvent e)
	{
		bool flag = false;
		for (Action action = Action.GotoUserNav1; action <= Action.GotoUserNav10; action++)
		{
			if (e.TryConsume(action))
			{
				GoToHotkeyNavPoint(action);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			for (Action action2 = Action.SetUserNav1; action2 <= Action.SetUserNav10; action2++)
			{
				if (e.TryConsume(action2))
				{
					Camera baseCamera = CameraController.Instance.baseCamera;
					Vector3 position = baseCamera.transform.GetPosition();
					SetHotkeyNavPoint(action2, position, baseCamera.orthographicSize);
					flag = true;
					break;
				}
			}
		}
		return flag;
	}
}
