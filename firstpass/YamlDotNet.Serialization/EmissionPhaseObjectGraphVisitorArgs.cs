using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
	public sealed class EmissionPhaseObjectGraphVisitorArgs
	{
		private readonly IEnumerable<IObjectGraphVisitor<Nothing>> preProcessingPhaseVisitors;

		public IObjectGraphVisitor<IEmitter> InnerVisitor
		{
			get;
			private set;
		}

		public IEventEmitter EventEmitter
		{
			get;
			private set;
		}

		public ObjectSerializer NestedObjectSerializer
		{
			get;
			private set;
		}

		public IEnumerable<IYamlTypeConverter> TypeConverters
		{
			get;
			private set;
		}

		public EmissionPhaseObjectGraphVisitorArgs(IObjectGraphVisitor<IEmitter> innerVisitor, IEventEmitter eventEmitter, IEnumerable<IObjectGraphVisitor<Nothing>> preProcessingPhaseVisitors, IEnumerable<IYamlTypeConverter> typeConverters, ObjectSerializer nestedObjectSerializer)
		{
			if (innerVisitor == null)
			{
				throw new ArgumentNullException("innerVisitor");
			}
			InnerVisitor = innerVisitor;
			if (eventEmitter == null)
			{
				throw new ArgumentNullException("eventEmitter");
			}
			EventEmitter = eventEmitter;
			if (preProcessingPhaseVisitors == null)
			{
				throw new ArgumentNullException("preProcessingPhaseVisitors");
			}
			this.preProcessingPhaseVisitors = preProcessingPhaseVisitors;
			if (typeConverters == null)
			{
				throw new ArgumentNullException("typeConverters");
			}
			TypeConverters = typeConverters;
			if (nestedObjectSerializer == null)
			{
				throw new ArgumentNullException("nestedObjectSerializer");
			}
			NestedObjectSerializer = nestedObjectSerializer;
		}

		public T GetPreProcessingPhaseObjectGraphVisitor<T>() where T : IObjectGraphVisitor<Nothing>
		{
			return preProcessingPhaseVisitors.OfType<T>().Single();
		}
	}
}
