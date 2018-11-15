using KSerialization;
using STRINGS;
using UnityEngine;

public class DiscoveredSpaceMessage : Message
{
	[Serialize]
	private Vector3 cameraFocusPos;

	private const string MUSIC_STINGER = "Stinger_Surface";

	public DiscoveredSpaceMessage()
	{
	}

	public DiscoveredSpaceMessage(Vector3 pos)
	{
		cameraFocusPos = pos;
		cameraFocusPos.z = -40f;
	}

	public override string GetSound()
	{
		return "Discover_Space";
	}

	public override string GetMessageBody()
	{
		return MISC.NOTIFICATIONS.DISCOVERED_SPACE.TOOLTIP;
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.DISCOVERED_SPACE.NAME;
	}

	public override string GetTooltip()
	{
		return null;
	}

	public override bool IsValid()
	{
		return true;
	}

	public override void OnClick()
	{
		OnDiscoveredSpaceClicked();
	}

	private void OnDiscoveredSpaceClicked()
	{
		KFMOD.PlayOneShot(GlobalAssets.GetSound(GetSound(), false));
		MusicManager.instance.PlaySong("Stinger_Surface", false);
		CameraController.Instance.SetTargetPos(cameraFocusPos, 8f, true);
		SeasonManager component = SaveGame.Instance.GetComponent<SeasonManager>();
		component.ForceBeginMeteorSeasonWithShower();
	}
}
