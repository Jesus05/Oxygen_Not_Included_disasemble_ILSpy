using System;

namespace Satsuma
{
	public struct Arc : IEquatable<Arc>
	{
		public long Id
		{
			get;
			private set;
		}

		public static Arc Invalid => new Arc(0L);

		public Arc(long id)
		{
			this = default(Arc);
			Id = id;
		}

		public bool Equals(Arc other)
		{
			return Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			if (obj is Arc)
			{
				return Equals((Arc)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override string ToString()
		{
			return "|" + Id;
		}

		public static bool operator ==(Arc a, Arc b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Arc a, Arc b)
		{
			return !(a == b);
		}
	}
}
