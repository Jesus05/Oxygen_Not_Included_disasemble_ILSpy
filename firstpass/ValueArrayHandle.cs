public struct ValueArrayHandle
{
	public int handle;

	public static readonly ValueArrayHandle Invalid = new ValueArrayHandle(-1);

	public ValueArrayHandle(int handle)
	{
		this.handle = handle;
	}

	public bool IsValid()
	{
		return handle >= 0;
	}
}
