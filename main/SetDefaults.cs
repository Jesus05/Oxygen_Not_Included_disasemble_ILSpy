using System;
using System.Runtime.CompilerServices;

public class SetDefaults
{
	[CompilerGenerated]
	private static Func<string, string> _003C_003Ef__mg_0024cache0;

	public static void Initialize()
	{
		KSlider.DefaultSounds[0] = GlobalAssets.GetSound("Slider_Start", false);
		KSlider.DefaultSounds[1] = GlobalAssets.GetSound("Slider_Move", false);
		KSlider.DefaultSounds[2] = GlobalAssets.GetSound("Slider_End", false);
		KSlider.DefaultSounds[3] = GlobalAssets.GetSound("Slider_Boundary_Low", false);
		KSlider.DefaultSounds[4] = GlobalAssets.GetSound("Slider_Boundary_High", false);
		KScrollRect.DefaultSounds[KScrollRect.SoundType.OnMouseScroll] = GlobalAssets.GetSound("Mousewheel_Move", false);
		WidgetSoundPlayer.getSoundPath = GetSoundPath;
	}

	private static string GetSoundPath(string sound_name)
	{
		return GlobalAssets.GetSound(sound_name, false);
	}
}
