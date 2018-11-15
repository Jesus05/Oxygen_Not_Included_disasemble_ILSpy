internal class ElementAudioFileLoader : AsyncCsvLoader<ElementAudioFileLoader, ElementsAudio.ElementAudioConfig>
{
	public ElementAudioFileLoader()
		: base(Assets.instance.elementAudio)
	{
	}

	public override void Run()
	{
		base.Run();
	}
}
