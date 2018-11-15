public class TuningData<TuningType>
{
	public static TuningType _TuningData;

	public static TuningType Get()
	{
		TuningSystem.Init();
		return _TuningData;
	}
}
