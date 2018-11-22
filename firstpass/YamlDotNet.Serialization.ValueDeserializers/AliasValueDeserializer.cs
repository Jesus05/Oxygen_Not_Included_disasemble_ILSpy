using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.ValueDeserializers
{
	public sealed class AliasValueDeserializer : IValueDeserializer
	{
		private sealed class AliasState : Dictionary<string, ValuePromise>, IPostDeserializationCallback
		{
			public void OnDeserialization()
			{
				foreach (ValuePromise value in base.Values)
				{
					if (!value.HasValue)
					{
						throw new AnchorNotFoundException(value.Alias.Start, value.Alias.End, $"Anchor '{value.Alias.Value}' not found");
					}
				}
			}
		}

		private sealed class ValuePromise : IValuePromise
		{
			private object value;

			public readonly AnchorAlias Alias;

			public bool HasValue
			{
				get;
				private set;
			}

			public object Value
			{
				get
				{
					if (!HasValue)
					{
						throw new InvalidOperationException("Value not set");
					}
					return value;
				}
				set
				{
					if (HasValue)
					{
						throw new InvalidOperationException("Value already set");
					}
					HasValue = true;
					this.value = value;
					if (this.ValueAvailable != null)
					{
						this.ValueAvailable(value);
					}
				}
			}

			public event Action<object> ValueAvailable;

			public ValuePromise(AnchorAlias alias)
			{
				Alias = alias;
			}

			public ValuePromise(object value)
			{
				HasValue = true;
				this.value = value;
			}
		}

		private readonly IValueDeserializer innerDeserializer;

		public AliasValueDeserializer(IValueDeserializer innerDeserializer)
		{
			if (innerDeserializer == null)
			{
				throw new ArgumentNullException("innerDeserializer");
			}
			this.innerDeserializer = innerDeserializer;
		}

		public object DeserializeValue(IParser parser, Type expectedType, SerializerState state, IValueDeserializer nestedObjectDeserializer)
		{
			AnchorAlias anchorAlias = parser.Allow<AnchorAlias>();
			if (anchorAlias == null)
			{
				string text = null;
				NodeEvent nodeEvent = parser.Peek<NodeEvent>();
				if (nodeEvent != null && !string.IsNullOrEmpty(nodeEvent.Anchor))
				{
					text = nodeEvent.Anchor;
				}
				object obj = innerDeserializer.DeserializeValue(parser, expectedType, state, nestedObjectDeserializer);
				if (text != null)
				{
					AliasState aliasState = state.Get<AliasState>();
					if (!aliasState.TryGetValue(text, out ValuePromise value))
					{
						aliasState.Add(text, new ValuePromise(obj));
					}
					else if (!value.HasValue)
					{
						value.Value = obj;
					}
					else
					{
						aliasState[text] = new ValuePromise(obj);
					}
				}
				return obj;
			}
			AliasState aliasState2 = state.Get<AliasState>();
			if (!aliasState2.TryGetValue(anchorAlias.Value, out ValuePromise value2))
			{
				value2 = new ValuePromise(anchorAlias);
				aliasState2.Add(anchorAlias.Value, value2);
			}
			return (!value2.HasValue) ? value2 : value2.Value;
		}
	}
}
