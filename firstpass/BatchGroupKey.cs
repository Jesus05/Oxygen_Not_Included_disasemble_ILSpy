using System;

public struct BatchGroupKey : IEquatable<BatchGroupKey>
{
	private HashedString _groupID;

	public HashedString groupID => _groupID;

	public BatchGroupKey(HashedString group_id)
	{
		_groupID = group_id;
	}

	public bool Equals(BatchGroupKey other)
	{
		return _groupID == other._groupID;
	}

	public override int GetHashCode()
	{
		return _groupID.HashValue;
	}
}
