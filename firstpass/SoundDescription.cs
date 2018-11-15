public struct SoundDescription
{
	public struct Parameter
	{
		public HashedString name;

		public int idx;

		public const int INVALID_IDX = -1;
	}

	public string path;

	public float falloffDistanceSq;

	public Parameter[] parameters;

	public OneShotSoundParameterUpdater[] oneShotParameterUpdaters;

	public int GetParameterIdx(HashedString name)
	{
		Parameter[] array = parameters;
		for (int i = 0; i < array.Length; i++)
		{
			Parameter parameter = array[i];
			if (parameter.name == name)
			{
				return parameter.idx;
			}
		}
		return -1;
	}
}
