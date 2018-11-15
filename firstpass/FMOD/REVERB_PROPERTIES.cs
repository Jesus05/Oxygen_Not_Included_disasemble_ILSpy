namespace FMOD
{
	public struct REVERB_PROPERTIES
	{
		public float DecayTime;

		public float EarlyDelay;

		public float LateDelay;

		public float HFReference;

		public float HFDecayRatio;

		public float Diffusion;

		public float Density;

		public float LowShelfFrequency;

		public float LowShelfGain;

		public float HighCut;

		public float EarlyLateMix;

		public float WetLevel;

		public REVERB_PROPERTIES(float decayTime, float earlyDelay, float lateDelay, float hfReference, float hfDecayRatio, float diffusion, float density, float lowShelfFrequency, float lowShelfGain, float highCut, float earlyLateMix, float wetLevel)
		{
			DecayTime = decayTime;
			EarlyDelay = earlyDelay;
			LateDelay = lateDelay;
			HFReference = hfReference;
			HFDecayRatio = hfDecayRatio;
			Diffusion = diffusion;
			Density = density;
			LowShelfFrequency = lowShelfFrequency;
			LowShelfGain = lowShelfGain;
			HighCut = highCut;
			EarlyLateMix = earlyLateMix;
			WetLevel = wetLevel;
		}
	}
}
