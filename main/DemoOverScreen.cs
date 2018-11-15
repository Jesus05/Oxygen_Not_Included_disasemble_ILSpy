public class DemoOverScreen : KModalScreen
{
	public KButton QuitButton;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Init();
		PlayerController.Instance.ActivateTool(SelectTool.Instance);
		SelectTool.Instance.Select(null, false);
	}

	private void Init()
	{
		QuitButton.onClick += delegate
		{
			Quit();
		};
	}

	private void Quit()
	{
		PauseScreen.TriggerQuitGame();
	}
}
