using Steamworks;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenariosMenu : KModalScreen, SteamUGCService.IUGCEventHandler
{
	public const string TAG_SCENARIO = "scenario";

	public KButton textButton;

	public KButton dismissButton;

	public KButton closeButton;

	public KButton workshopButton;

	public KButton loadScenarioButton;

	[Space]
	public GameObject ugcContainer;

	public GameObject ugcButtonPrefab;

	public LocText noScenariosText;

	public RectTransform contentRoot;

	public RectTransform detailsRoot;

	public LocText scenarioTitle;

	public LocText scenarioDetails;

	private PublishedFileId_t activeItem;

	private List<GameObject> buttons = new List<GameObject>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		dismissButton.onClick += delegate
		{
			Deactivate();
		};
		LocText reference = dismissButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Title");
		reference.SetText(UI.FRONTEND.OPTIONS_SCREEN.BACK);
		closeButton.onClick += delegate
		{
			Deactivate();
		};
		workshopButton.onClick += delegate
		{
			OnClickOpenWorkshop();
		};
		RebuildScreen();
	}

	private void RebuildScreen()
	{
		foreach (GameObject button in buttons)
		{
			UnityEngine.Object.Destroy(button);
		}
		buttons.Clear();
		RebuildUGCButtons();
	}

	private void RebuildUGCButtons()
	{
		List<SteamUGCService.Subscribed> subscribed = SteamUGCService.Instance.GetSubscribed("scenario");
		bool flag = subscribed.Count > 0;
		noScenariosText.gameObject.SetActive(!flag);
		contentRoot.gameObject.SetActive(flag);
		bool flag2 = true;
		if (subscribed.Count != 0)
		{
			for (int i = 0; i < subscribed.Count; i++)
			{
				GameObject gameObject = Util.KInstantiateUI(ugcButtonPrefab, ugcContainer, false);
				gameObject.name = subscribed[i].title + "_button";
				gameObject.gameObject.SetActive(true);
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				TMP_FontAsset fontForLangage = LanguageOptionsScreen.GetFontForLangage(subscribed[i].fileId);
				LocText reference = component.GetReference<LocText>("Title");
				reference.SetText(subscribed[i].title);
				reference.font = fontForLangage;
				Texture2D previewImage = SteamUGCService.Instance.GetPreviewImage(subscribed[i].fileId);
				if ((UnityEngine.Object)previewImage != (UnityEngine.Object)null)
				{
					Image reference2 = component.GetReference<Image>("Image");
					reference2.sprite = Sprite.Create(previewImage, new Rect(Vector2.zero, new Vector2((float)previewImage.width, (float)previewImage.height)), Vector2.one * 0.5f);
				}
				KButton component2 = gameObject.GetComponent<KButton>();
				int index = i;
				PublishedFileId_t item = subscribed[index].fileId;
				component2.onClick += delegate
				{
					ShowDetails(item);
				};
				component2.onDoubleClick += delegate
				{
					LoadScenario(item);
				};
				buttons.Add(gameObject);
				if (item == activeItem)
				{
					flag2 = false;
				}
			}
		}
		if (flag2)
		{
			HideDetails();
		}
	}

	private void LoadScenario(PublishedFileId_t item)
	{
		SteamUGC.GetItemInstallInfo(item, out ulong punSizeOnDisk, out string pchFolder, 1024u, out uint punTimeStamp);
		Output.Log("LoadScenario", pchFolder, punSizeOnDisk, punTimeStamp);
		System.DateTime lastModified;
		byte[] bytesFromZip = SteamUGCService.GetBytesFromZip(item, new string[1]
		{
			".sav"
		}, out lastModified, false);
		string text = Path.Combine(SaveLoader.GetSavePrefix(), "scenario.sav");
		File.WriteAllBytes(text, bytesFromZip);
		SaveLoader.SetActiveSaveFilePath(text);
		Time.timeScale = 0f;
		App.LoadScene("backend");
	}

	private ConfirmDialogScreen GetConfirmDialog()
	{
		GameObject gameObject = KScreenManager.AddChild(base.transform.parent.gameObject, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject);
		KScreen component = gameObject.GetComponent<KScreen>();
		component.Activate();
		return component.GetComponent<ConfirmDialogScreen>();
	}

	private void ShowDetails(PublishedFileId_t item)
	{
		activeItem = item;
		SteamUGCDetails_t details = SteamUGCService.Instance.GetDetails(item);
		scenarioTitle.text = details.m_rgchTitle;
		scenarioDetails.text = details.m_rgchDescription;
		loadScenarioButton.onClick += delegate
		{
			LoadScenario(item);
		};
		detailsRoot.gameObject.SetActive(true);
	}

	private void HideDetails()
	{
		detailsRoot.gameObject.SetActive(false);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		SteamUGCService.Instance.ugcEventHandlers.Add(this);
		HideDetails();
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		SteamUGCService.Instance.ugcEventHandlers.Remove(this);
	}

	private void OnClickOpenWorkshop()
	{
		Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=457140&requiredtags[]=scenario");
	}

	public void OnUGCItemInstalled(ItemInstalled_t pCallback)
	{
	}

	public void OnUGCItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
	{
		RebuildScreen();
	}

	public void OnUGCItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
	{
		RebuildScreen();
	}

	public void OnUGCItemDownloaded(DownloadItemResult_t pCallback)
	{
	}

	public void OnUGCRefresh()
	{
		RebuildScreen();
	}
}
