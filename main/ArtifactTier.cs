public class ArtifactTier
{
	public EffectorValues decorValues;

	public StringKey name_key;

	public ArtifactTier(StringKey str_key, EffectorValues values)
	{
		decorValues = values;
		name_key = str_key;
	}
}
