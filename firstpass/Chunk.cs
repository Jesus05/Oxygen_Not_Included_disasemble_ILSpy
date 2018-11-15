using KSerialization;
using System.Runtime.Serialization;

[SerializationConfig(MemberSerialization.OptOut)]
public class Chunk
{
	public enum State
	{
		Unprocessed,
		GeneratedNoise,
		Processed,
		Loaded
	}

	public State state;

	public Vector2I offset;

	public Vector2I size;

	public float[] data;

	public float[] overrides;

	public float[] density;

	public float[] heatOffset;

	public float[] defaultTemp;

	public Chunk()
	{
		state = State.Unprocessed;
		data = null;
		overrides = null;
		density = null;
		heatOffset = null;
		defaultTemp = null;
	}

	public Chunk(int x, int y, int width, int height)
	{
		offset = new Vector2I(x, y);
		size = new Vector2I(width, height);
	}

	[OnDeserializing]
	internal void OnDeserializingMethod()
	{
		int x = size.x;
		int y = size.y;
		data = new float[x * y];
		overrides = new float[x * y];
		density = new float[x * y];
		heatOffset = new float[x * y];
		defaultTemp = new float[x * y];
		state = State.Loaded;
	}
}
