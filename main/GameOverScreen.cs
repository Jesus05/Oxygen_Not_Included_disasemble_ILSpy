public class GameOverScreen : KModalScreen
{
	public KButton DismissButton;

	public KButton QuitButton;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Init();
	}

	private void Init()
	{
		if ((bool)QuitButton)
		{
			QuitButton.onClick += delegate
			{
				Quit();
			};
		}
		if ((bool)DismissButton)
		{
			DismissButton.onClick += delegate
			{
				Dismiss();
			};
		}
	}

	private void Quit()
	{
		PauseScreen.TriggerQuitGame();
	}

	private void Dismiss()
	{
		Show(false);
	}
}
