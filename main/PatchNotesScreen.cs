using STRINGS;
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

	private static int PatchNotesVersion = 9;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		changesLabel.text = string.Format(UI.FRONTEND.PATCHNOTESSCREEN.BODY, UI.FRONTEND.PATCHNOTESSCREEN.PATCHNOTES);
		closeButton.onClick += MarkAsReadAndClose;
		closeButton.soundPlayer.widget_sound_events()[0].OverrideAssetName = "HUD_Click_Close";
		okButton.onClick += MarkAsReadAndClose;
		fullPatchNotes.onClick += delegate
		{
			Application.OpenURL("http://forums.kleientertainment.com/forum/137-oxygen-not-included-latest-content-update/");
		};
		previousVersion.onClick += delegate
		{
			Application.OpenURL("http://support.kleientertainment.com/customer/portal/articles/2776550");
		};
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
