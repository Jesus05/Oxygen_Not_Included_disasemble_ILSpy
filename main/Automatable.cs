using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class Automatable : KMonoBehaviour
{
	[Serialize]
	private bool automationOnly = true;

	public bool GetAutomationOnly()
	{
		return automationOnly;
	}

	public void SetAutomationOnly(bool only)
	{
		automationOnly = only;
	}

	public bool AllowedByAutomation(bool is_transfer_arm)
	{
		return !GetAutomationOnly() || is_transfer_arm;
	}
}
