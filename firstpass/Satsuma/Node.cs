using System;

namespace Satsuma
{
	public struct Node : IEquatable<Node>
	{
		public long Id
		{
			get;
			private set;
		}

		public static Node Invalid => new Node(0L);

		public Node(long id)
		{
			this = default(Node);
			Id = id;
		}

		public bool Equals(Node other)
		{
			return Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Node))
			{
				return false;
			}
			return Equals((Node)obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override string ToString()
		{
			return "#" + Id;
		}

		public static bool operator ==(Node a, Node b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Node a, Node b)
		{
			return !(a == b);
		}
	}
}
