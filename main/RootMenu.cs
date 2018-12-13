using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RootMenu : KScreen
{
	private DetailsScreen detailsScreen;

	private UserMenuScreen userMenu;

	[SerializeField]
	private GameObject detailsScreenPrefab;

	[SerializeField]
	private UserMenuScreen userMenuPrefab;

	private GameObject userMenuParent;

	[SerializeField]
	private TileScreen tileScreen;

	public KScreen buildMenu;

	private List<KScreen> subMenus = new List<KScreen>();

	private TileScreen tileScreenInst;

	public GameObject selectedGO;

	public static RootMenu Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public override float GetSortKey()
	{
		return -1f;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		UIRegistry.rootMenu = this;
		Subscribe(Game.Instance.gameObject, -1503271301, OnSelectObject);
		Subscribe(Game.Instance.gameObject, 288942073, OnUIClear);
		Subscribe(Game.Instance.gameObject, -809948329, OnBuildingStatechanged);
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		detailsScreen = Util.KInstantiateUI(detailsScreenPrefab, base.gameObject, true).GetComponent<DetailsScreen>();
		detailsScreen.gameObject.SetActive(true);
		userMenuParent = detailsScreen.UserMenuPanel.gameObject;
		userMenu = Util.KInstantiateUI(userMenuPrefab.gameObject, userMenuParent, false).GetComponent<UserMenuScreen>();
		detailsScreen.gameObject.SetActive(false);
		userMenu.gameObject.SetActive(false);
	}

	private void OnClickCommon()
	{
		CloseSubMenus();
	}

	public void AddSubMenu(KScreen sub_menu)
	{
		if (sub_menu.activateOnSpawn)
		{
			sub_menu.Show(true);
		}
		subMenus.Add(sub_menu);
	}

	public void RemoveSubMenu(KScreen sub_menu)
	{
		subMenus.Remove(sub_menu);
	}

	private void CloseSubMenus()
	{
		foreach (KScreen subMenu in subMenus)
		{
			if ((Object)subMenu != (Object)null)
			{
				if (subMenu.activateOnSpawn)
				{
					subMenu.gameObject.SetActive(false);
				}
				else
				{
					subMenu.Deactivate();
				}
			}
		}
		subMenus.Clear();
	}

	private void OnSelectObject(object data)
	{
		GameObject gameObject = (GameObject)data;
		if ((Object)gameObject != (Object)null)
		{
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if ((Object)component != (Object)null && !component.IsInitialized())
			{
				return;
			}
		}
		if ((Object)gameObject != (Object)selectedGO)
		{
			selectedGO = gameObject;
			CloseSubMenus();
			if ((Object)selectedGO != (Object)null && ((Object)selectedGO.GetComponent<KPrefabID>() != (Object)null || (bool)selectedGO.GetComponent<CellSelectionObject>()))
			{
				AddSubMenu(detailsScreen);
				detailsScreen.Refresh(selectedGO);
				AddSubMenu(userMenu);
				userMenu.SetSelected(selectedGO);
				userMenu.Refresh(selectedGO);
			}
			else
			{
				userMenu.SetSelected(null);
			}
		}
	}

	private void OnBuildingStatechanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		if ((Object)gameObject == (Object)selectedGO)
		{
			OnSelectObject(gameObject);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed && e.TryConsume(Action.Escape) && SelectTool.Instance.enabled)
		{
			if (AreSubMenusOpen())
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Back", false));
				CloseSubMenus();
				SelectTool.Instance.Select(null, false);
			}
			else if (e.IsAction(Action.Escape))
			{
				if (!SelectTool.Instance.enabled)
				{
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
				}
				if (PlayerController.Instance.IsUsingDefaultTool())
				{
					if ((Object)SelectTool.Instance.selected != (Object)null)
					{
						SelectTool.Instance.Select(null, false);
					}
					else
					{
						CameraController.Instance.ForcePanningState(false);
						TogglePauseScreen();
					}
				}
				else
				{
					Game.Instance.Trigger(288942073, null);
				}
				ToolMenu.Instance.ClearSelection();
				SelectTool.Instance.Activate();
			}
		}
		base.OnKeyDown(e);
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		base.OnKeyUp(e);
		if (!e.Consumed)
		{
			if (e.TryConsume(Action.AlternateView))
			{
				if ((Object)tileScreenInst != (Object)null)
				{
					tileScreenInst.Deactivate();
					tileScreenInst = null;
				}
			}
			else if ((Object)SaveGame.Instance != (Object)null)
			{
				SaveGame.Instance.GetComponent<UserNavigation>().Handle(e);
			}
		}
	}

	public void TogglePauseScreen()
	{
		PauseScreen.Instance.Show(true);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().ESCPauseSnapshot);
		MusicManager.instance.OnEscapeMenu(true);
		MusicManager.instance.PlaySong("Music_ESC_Menu", false);
	}

	public void ExternalClose()
	{
		OnClickCommon();
	}

	private void OnUIClear(object data)
	{
		CloseSubMenus();
		if ((Object)UnityEngine.EventSystems.EventSystem.current != (Object)null)
		{
			UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
		}
		else
		{
			Debug.LogWarning("OnUIClear() Event system is null", null);
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
	}

	private bool AreSubMenusOpen()
	{
		return subMenus.Count > 0;
	}

	private KToggleMenu.ToggleInfo[] GetFillers()
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		List<KToggleMenu.ToggleInfo> list = new List<KToggleMenu.ToggleInfo>();
		foreach (Pickupable item in Components.Pickupables.Items)
		{
			KPrefabID kPrefabID = item.KPrefabID;
			if (kPrefabID.HasTag(GameTags.Filler) && hashSet.Add(kPrefabID.PrefabTag))
			{
				string text = kPrefabID.GetComponent<PrimaryElement>().Element.id.ToString();
				list.Add(new KToggleMenu.ToggleInfo(text, null, Action.NumActions));
			}
		}
		return list.ToArray();
	}
}
