public class SaveActive : KScreen
{
	[MyCmpGet]
	private KBatchedAnimController controller;

	private Game.CansaveCB readyForSaveCallback;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.SetAutoSaveCallbacks(ActiveateSaveIndicator, SetActiveSaveIndicator, DeactivateSaveIndicator);
	}

	private void DoCallBack(HashedString name)
	{
		controller.onAnimComplete -= DoCallBack;
		readyForSaveCallback();
		readyForSaveCallback = null;
	}

	private void ActiveateSaveIndicator(Game.CansaveCB cb)
	{
		readyForSaveCallback = cb;
		controller.onAnimComplete += DoCallBack;
		controller.Play("working_pre", KAnim.PlayMode.Once, 1f, 0f);
	}

	private void SetActiveSaveIndicator()
	{
		controller.Play("working_loop", KAnim.PlayMode.Once, 1f, 0f);
	}

	private void DeactivateSaveIndicator()
	{
		controller.Play("working_pst", KAnim.PlayMode.Once, 1f, 0f);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
	}
}
