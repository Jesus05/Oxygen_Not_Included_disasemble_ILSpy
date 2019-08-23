using UnityEngine;

public class PatchNotesScreen : KModalScreen
{
	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton okButton;

	[SerializeField]
	private KButton fullPatchNotes;

	[SerializeField]
	private KButton previousVersion;

	[SerializeField]
	private LocText changesLabel;

	private string m_patchNotesUrl;

	private string m_patchNotesText;

	private static int PatchNotesVersion = 9;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		changesLabel.text = m_patchNotesText;
		closeButton.onClick += MarkAsReadAndClose;
		closeButton.soundPlayer.widget_sound_events()[0].OverrideAssetName = "HUD_Click_Close";
		okButton.onClick += MarkAsReadAndClose;
		previousVersion.onClick += delegate
		{
			Application.OpenURL("http://support.kleientertainment.com/customer/portal/articles/2776550");
		};
		fullPatchNotes.onClick += OnPatchNotesClick;
	}

	public static bool ShouldShowScreen()
	{
		return KPlayerPrefs.GetInt("PatchNotesVersion") < PatchNotesVersion;
	}

	private void MarkAsReadAndClose()
	{
		KPlayerPrefs.SetInt("PatchNotesVersion", PatchNotesVersion);
		base.gameObject.SetActive(false);
	}

	public void UpdatePatchNotes(string patchNotesSummary, string url)
	{
		m_patchNotesUrl = url;
		m_patchNotesText = patchNotesSummary;
		changesLabel.text = m_patchNotesText;
	}

	private void OnPatchNotesClick()
	{
		Application.OpenURL(m_patchNotesUrl);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
		{
			MarkAsReadAndClose();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}
}
