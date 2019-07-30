using STRINGS;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportErrorDialog : MonoBehaviour
{
	public static string MOST_RECENT_SAVEFILE;

	private System.Action confirmAction;

	private System.Action quitAction;

	private System.Action continueAction;

	public TMP_InputField messageInputField;

	public GameObject referenceMessage;

	[SerializeField]
	private KButton submitButton;

	[SerializeField]
	private KButton quitButton;

	[SerializeField]
	private KButton continueGameButton;

	[SerializeField]
	private LocText CrashLabel;

	[SerializeField]
	private LocText VCCrashLabel;

	[SerializeField]
	private Button VCLinkButton;

	[SerializeField]
	private GameObject InfoBox;

	[SerializeField]
	private GameObject uploadSaveDialog;

	[SerializeField]
	private KButton uploadSaveButton;

	[SerializeField]
	private KButton skipUploadSaveButton;

	[SerializeField]
	private LocText saveFileInfoLabel;

	public static bool hasCrash;

	private void Start()
	{
		ThreadedHttps<KleiMetrics>.Instance.EndSession(true);
		if ((bool)SpeedControlScreen.Instance)
		{
			SpeedControlScreen.Instance.Pause(false);
		}
		if ((bool)KScreenManager.Instance)
		{
			KScreenManager.Instance.DisableInput(true);
		}
		continueGameButton.onClick += OnSelect_CONTINUE;
		continueGameButton.gameObject.SetActive(!KCrashReporter.terminateOnError);
		submitButton.onClick += OnSelect_SUBMIT;
		quitButton.onClick += OnSelect_QUIT;
		uploadSaveButton.onClick += OnSelect_UPLOADSAVE;
		skipUploadSaveButton.onClick += OnSelect_SKIPUPLOADSAVE;
		messageInputField.text = UI.CRASHSCREEN.BODY;
		hasCrash = true;
	}

	private void Update()
	{
		Debug.developerConsoleVisible = false;
	}

	private void OnDestroy()
	{
		if (KCrashReporter.terminateOnError)
		{
			App.Quit();
		}
		if ((bool)KScreenManager.Instance)
		{
			KScreenManager.Instance.DisableInput(false);
		}
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			OnSelect_QUIT();
		}
	}

	public void PopupConfirmDialog(System.Action onConfirm, System.Action onQuit, System.Action onContinue)
	{
		confirmAction = onConfirm;
		quitAction = onQuit;
		continueAction = onContinue;
		continueGameButton.gameObject.SetActive(continueAction != null);
		VCCrashLabel.gameObject.SetActive(false);
		VCLinkButton.gameObject.SetActive(false);
		quitButton.gameObject.SetActive(onQuit != null);
	}

	public void OnSelect_SUBMIT()
	{
		submitButton.GetComponentInChildren<LocText>().text = UI.CRASHSCREEN.REPORTING;
		submitButton.GetComponent<KButton>().isInteractable = false;
		StartCoroutine(WaitForUIUpdateBeforeReporting());
	}

	private IEnumerator WaitForUIUpdateBeforeReporting()
	{
		yield return (object)new WaitForEndOfFrame();
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public void OnSelect_QUIT()
	{
		if (quitAction != null)
		{
			quitAction();
		}
	}

	public void OnSelect_CONTINUE()
	{
		hasCrash = false;
		if (continueAction != null)
		{
			continueAction();
		}
	}

	public void OpenRefMessage()
	{
		submitButton.gameObject.SetActive(false);
		referenceMessage.SetActive(true);
	}

	public string UserMessage()
	{
		return messageInputField.text;
	}

	private void OnSelect_UPLOADSAVE()
	{
		uploadSaveDialog.SetActive(false);
		KCrashReporter.MOST_RECENT_SAVEFILE = MOST_RECENT_SAVEFILE;
		Submit();
	}

	private void OnSelect_SKIPUPLOADSAVE()
	{
		uploadSaveDialog.SetActive(false);
		KCrashReporter.MOST_RECENT_SAVEFILE = null;
		Submit();
	}

	private void Submit()
	{
		confirmAction();
		OpenRefMessage();
	}
}
