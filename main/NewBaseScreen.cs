using FMODUnity;
using Klei.AI;
using UnityEngine;

public class NewBaseScreen : KScreen
{
	public static NewBaseScreen Instance;

	[SerializeField]
	private CanvasGroup[] disabledUIElements;

	[EventRef]
	public string ScanSoundMigrated;

	[EventRef]
	public string BuildBaseSoundMigrated;

	private ITelepadDeliverable[] minionStartingStats;

	public override float GetSortKey()
	{
		return 1f;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		base.OnPrefabInit();
		TimeOfDay.Instance.SetScale(0f);
	}

	public static Vector2I SetInitialCamera()
	{
		Vector2I baseStartPos = SaveLoader.Instance.cachedGSD.baseStartPos;
		int cell = Grid.OffsetCell(0, baseStartPos.x, baseStartPos.y);
		Vector3 pos = Grid.CellToPosCCC(Grid.OffsetCell(cell, 0, -2), Grid.SceneLayer.Background);
		CameraController.Instance.SetMaxOrthographicSize(40f);
		CameraController.Instance.SnapTo(pos);
		CameraController.Instance.SetTargetPos(pos, 20f, false);
		CameraController.Instance.SetOrthographicsSize(40f);
		CameraSaveData.valid = false;
		return baseStartPos;
	}

	protected override void OnActivate()
	{
		if (disabledUIElements != null)
		{
			CanvasGroup[] array = disabledUIElements;
			foreach (CanvasGroup canvasGroup in array)
			{
				if ((Object)canvasGroup != (Object)null)
				{
					canvasGroup.interactable = false;
				}
			}
		}
		SetInitialCamera();
		if (SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Unpause(false);
		}
		Final();
	}

	public void SetStartingMinionStats(ITelepadDeliverable[] stats)
	{
		minionStartingStats = stats;
	}

	protected override void OnDeactivate()
	{
		Game.Instance.Trigger(-122303817, null);
		if (disabledUIElements != null)
		{
			CanvasGroup[] array = disabledUIElements;
			foreach (CanvasGroup canvasGroup in array)
			{
				if ((Object)canvasGroup != (Object)null)
				{
					canvasGroup.interactable = true;
				}
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		Action[] array = new Action[4]
		{
			Action.SpeedUp,
			Action.SlowDown,
			Action.TogglePause,
			Action.CycleSpeed
		};
		if (!e.Consumed)
		{
			for (int i = 0; i < array.Length && !e.TryConsume(array[i]); i++)
			{
			}
		}
	}

	private void Final()
	{
		SpeedControlScreen.Instance.Unpause(false);
		Telepad telepad = Object.FindObjectOfType<Telepad>();
		if ((bool)telepad)
		{
			SpawnMinions(Grid.PosToCell(telepad.gameObject));
		}
		Game.Instance.baseAlreadyCreated = true;
		Game.Instance.StartDelayedInitialSave();
		Deactivate();
	}

	private void SpawnMinions(int headquartersCell)
	{
		if (headquartersCell == -1)
		{
			Debug.LogWarning("No headquarters in saved base template. Cannot place minions. Confirm there is a headquarters saved to the base template, or consider creating a new one.", null);
		}
		else
		{
			Grid.CellToXY(headquartersCell, out int x, out int y);
			if (Grid.WidthInCells >= 64)
			{
				int baseLeft = SaveGame.Instance.worldGen.BaseLeft;
				int baseRight = SaveGame.Instance.worldGen.BaseRight;
				Effect a_new_hope = Db.Get().effects.Get("AnewHope");
				for (int i = 0; i < minionStartingStats.Length; i++)
				{
					int x2 = x + i % (baseRight - baseLeft) + 1;
					int y2 = y;
					int cell = Grid.XYToCell(x2, y2);
					GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), null, null);
					Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
					gameObject.transform.SetLocalPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
					gameObject.SetActive(true);
					((MinionStartingStats)minionStartingStats[i]).Apply(gameObject);
					GameScheduler.Instance.Schedule("ANewHope", 3f + 0.5f * (float)i, delegate(object m)
					{
						GameObject gameObject2 = m as GameObject;
						if (!((Object)gameObject2 == (Object)null))
						{
							gameObject2.GetComponent<Effects>().Add(a_new_hope, true);
						}
					}, gameObject, null);
				}
			}
		}
	}
}
