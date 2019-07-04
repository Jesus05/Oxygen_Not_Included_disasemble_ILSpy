using UnityEngine;

public class VictoryScreen : KModalScreen
{
	[SerializeField]
	private KButton DismissButton;

	[SerializeField]
	private LocText descriptionText;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Init();
	}

	private void Init()
	{
		if ((bool)DismissButton)
		{
			DismissButton.onClick += delegate
			{
				Dismiss();
			};
		}
	}

	private void Retire()
	{
		if (RetireColonyUtility.SaveColonySummaryData())
		{
			Show(false);
		}
	}

	private void Dismiss()
	{
		Show(false);
	}

	public void SetAchievements(string[] achievementIDs)
	{
		string text = "";
		for (int i = 0; i < achievementIDs.Length; i++)
		{
			if (i > 0)
			{
				text += "\n";
			}
			text += GameUtil.ApplyBoldString(Db.Get().ColonyAchievements.Get(achievementIDs[i]).Name);
			text = text + "\n" + Db.Get().ColonyAchievements.Get(achievementIDs[i]).description;
		}
		descriptionText.text = text;
	}
}
