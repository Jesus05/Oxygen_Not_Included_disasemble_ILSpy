namespace MIConvexHull
{
	internal sealed class ConnectorList
	{
		private FaceConnector last;

		public FaceConnector First
		{
			get;
			private set;
		}

		private void AddFirst(FaceConnector connector)
		{
			First.Previous = connector;
			connector.Next = First;
			First = connector;
		}

		public void Add(FaceConnector element)
		{
			if (last != null)
			{
				last.Next = element;
			}
			element.Previous = last;
			last = element;
			if (First == null)
			{
				First = element;
			}
		}

		public void Remove(FaceConnector connector)
		{
			if (connector.Previous != null)
			{
				connector.Previous.Next = connector.Next;
			}
			else if (connector.Previous == null)
			{
				First = connector.Next;
			}
			if (connector.Next != null)
			{
				connector.Next.Previous = connector.Previous;
			}
			else if (connector.Next == null)
			{
				last = connector.Previous;
			}
			connector.Next = null;
			connector.Previous = null;
		}
	}
}
