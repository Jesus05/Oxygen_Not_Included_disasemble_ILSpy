using KSerialization;
using STRINGS;
using UnityEngine;

public class DeathMessage : TargetMessage
{
	[Serialize]
	private ResourceRef<Death> death = new ResourceRef<Death>();

	public DeathMessage()
	{
	}

	public DeathMessage(GameObject go, Death death)
		: base(go.GetComponent<KPrefabID>())
	{
		this.death.Set(death);
	}

	public override string GetSound()
	{
		return string.Empty;
	}

	public override bool PlayNotificationSound()
	{
		return false;
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.DUPLICANTDIED.NAME;
	}

	public override string GetTooltip()
	{
		return GetMessageBody();
	}

	public override string GetMessageBody()
	{
		string description = death.Get().description;
		return description.Replace("{Target}", GetTarget().GetName());
	}
}
