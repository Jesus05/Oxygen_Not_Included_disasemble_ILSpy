using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Core
{
	public sealed class MergingParser : IParser
	{
		private sealed class ParsingEventCollection : IEnumerable<LinkedListNode<ParsingEvent>>, IEnumerable
		{
			private readonly LinkedList<ParsingEvent> _events;

			private readonly HashSet<LinkedListNode<ParsingEvent>> _deleted;

			private readonly Dictionary<string, LinkedListNode<ParsingEvent>> _references;

			public ParsingEventCollection()
			{
				_events = new LinkedList<ParsingEvent>();
				_deleted = new HashSet<LinkedListNode<ParsingEvent>>();
				_references = new Dictionary<string, LinkedListNode<ParsingEvent>>();
			}

			public void AddAfter(LinkedListNode<ParsingEvent> node, IEnumerable<ParsingEvent> items)
			{
				foreach (ParsingEvent item in items)
				{
					node = _events.AddAfter(node, item);
				}
			}

			public void Add(ParsingEvent item)
			{
				LinkedListNode<ParsingEvent> node = _events.AddLast(item);
				AddReference(item, node);
			}

			public void MarkDeleted(LinkedListNode<ParsingEvent> node)
			{
				_deleted.Add(node);
			}

			public void CleanMarked()
			{
				foreach (LinkedListNode<ParsingEvent> item in _deleted)
				{
					_events.Remove(item);
				}
			}

			public IEnumerable<LinkedListNode<ParsingEvent>> FromAnchor(string anchor)
			{
				LinkedListNode<ParsingEvent> node = _references[anchor].Next;
				IEnumerator<LinkedListNode<ParsingEvent>> iterator = GetEnumerator(node);
				if (iterator.MoveNext())
				{
					yield return iterator.Current;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}

			public IEnumerator<LinkedListNode<ParsingEvent>> GetEnumerator()
			{
				return GetEnumerator(_events.First);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			private IEnumerator<LinkedListNode<ParsingEvent>> GetEnumerator(LinkedListNode<ParsingEvent> node)
			{
				if (node != null)
				{
					yield return node;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}

			private void AddReference(ParsingEvent item, LinkedListNode<ParsingEvent> node)
			{
				if (item is MappingStart)
				{
					MappingStart mappingStart = (MappingStart)item;
					string anchor = mappingStart.Anchor;
					if (!string.IsNullOrEmpty(anchor))
					{
						_references[anchor] = node;
					}
				}
			}
		}

		private sealed class ParsingEventCloner : IParsingEventVisitor
		{
			private ParsingEvent clonedEvent;

			public ParsingEvent Clone(ParsingEvent e)
			{
				e.Accept(this);
				return clonedEvent;
			}

			void IParsingEventVisitor.Visit(AnchorAlias e)
			{
				clonedEvent = new AnchorAlias(e.Value, e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(StreamStart e)
			{
				throw new NotSupportedException();
			}

			void IParsingEventVisitor.Visit(StreamEnd e)
			{
				throw new NotSupportedException();
			}

			void IParsingEventVisitor.Visit(DocumentStart e)
			{
				throw new NotSupportedException();
			}

			void IParsingEventVisitor.Visit(DocumentEnd e)
			{
				throw new NotSupportedException();
			}

			void IParsingEventVisitor.Visit(Scalar e)
			{
				clonedEvent = new Scalar(null, e.Tag, e.Value, e.Style, e.IsPlainImplicit, e.IsQuotedImplicit, e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(SequenceStart e)
			{
				clonedEvent = new SequenceStart(null, e.Tag, e.IsImplicit, e.Style, e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(SequenceEnd e)
			{
				clonedEvent = new SequenceEnd(e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(MappingStart e)
			{
				clonedEvent = new MappingStart(null, e.Tag, e.IsImplicit, e.Style, e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(MappingEnd e)
			{
				clonedEvent = new MappingEnd(e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(Comment e)
			{
				throw new NotSupportedException();
			}
		}

		private readonly ParsingEventCollection _events;

		private readonly IParser _innerParser;

		private IEnumerator<LinkedListNode<ParsingEvent>> _iterator;

		private bool _merged;

		public ParsingEvent Current
		{
			get
			{
				if (_iterator.Current == null)
				{
					return null;
				}
				return _iterator.Current.Value;
			}
		}

		public MergingParser(IParser innerParser)
		{
			_events = new ParsingEventCollection();
			_merged = false;
			_iterator = _events.GetEnumerator();
			_innerParser = innerParser;
		}

		public bool MoveNext()
		{
			if (!_merged)
			{
				Merge();
				_events.CleanMarked();
				_iterator = _events.GetEnumerator();
				_merged = true;
			}
			return _iterator.MoveNext();
		}

		private void Merge()
		{
			while (_innerParser.MoveNext())
			{
				_events.Add(_innerParser.Current);
			}
			foreach (LinkedListNode<ParsingEvent> @event in _events)
			{
				if (IsMergeToken(@event))
				{
					_events.MarkDeleted(@event);
					if (!HandleMerge(@event.Next))
					{
						throw new SemanticErrorException(@event.Value.Start, @event.Value.End, "Unrecognized merge key pattern");
					}
				}
			}
		}

		private bool HandleMerge(LinkedListNode<ParsingEvent> node)
		{
			if (node != null)
			{
				if (!(node.Value is AnchorAlias))
				{
					if (!(node.Value is SequenceStart))
					{
						return false;
					}
					return HandleSequence(node);
				}
				return HandleAnchorAlias(node);
			}
			return false;
		}

		private bool IsMergeToken(LinkedListNode<ParsingEvent> node)
		{
			if (!(node.Value is Scalar))
			{
				return false;
			}
			Scalar scalar = node.Value as Scalar;
			return scalar.Value == "<<";
		}

		private bool HandleAnchorAlias(LinkedListNode<ParsingEvent> node)
		{
			if (node != null && node.Value is AnchorAlias)
			{
				AnchorAlias anchorAlias = (AnchorAlias)node.Value;
				IEnumerable<ParsingEvent> mappingEvents = GetMappingEvents(anchorAlias.Value);
				_events.AddAfter(node, mappingEvents);
				_events.MarkDeleted(node);
				return true;
			}
			return false;
		}

		private bool HandleSequence(LinkedListNode<ParsingEvent> node)
		{
			if (node != null && node.Value is SequenceStart)
			{
				_events.MarkDeleted(node);
				while (node != null)
				{
					if (node.Value is SequenceEnd)
					{
						_events.MarkDeleted(node);
						return true;
					}
					LinkedListNode<ParsingEvent> next = node.Next;
					HandleMerge(next);
					node = next;
				}
				return true;
			}
			return false;
		}

		private IEnumerable<ParsingEvent> GetMappingEvents(string anchor)
		{
			ParsingEventCloner cloner = new ParsingEventCloner();
			int nesting = 0;
			return from e in (from e in _events.FromAnchor(anchor)
			select e.Value).TakeWhile((ParsingEvent e) => (nesting += e.NestingIncrease) >= 0)
			select cloner.Clone(e);
		}
	}
}
