public class Hud : KScreen
{
	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Help))
		{
			GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ControlsScreen.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		}
	}
}
