using System;
using System.Collections.Generic;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class AnchorAssigningObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		private readonly IEventEmitter eventEmitter;

		private readonly IAliasProvider aliasProvider;

		private readonly HashSet<string> emittedAliases = new HashSet<string>();

		public AnchorAssigningObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor, IEventEmitter eventEmitter, IAliasProvider aliasProvider)
			: base(nextVisitor)
		{
			this.eventEmitter = eventEmitter;
			this.aliasProvider = aliasProvider;
		}

		public override bool Enter(IObjectDescriptor value, IEmitter context)
		{
			string alias = aliasProvider.GetAlias(value.Value);
			if (alias != null && !emittedAliases.Add(alias))
			{
				eventEmitter.Emit(new AliasEventInfo(value)
				{
					Alias = alias
				}, context);
				return false;
			}
			return base.Enter(value, context);
		}

		public override void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType, IEmitter context)
		{
			eventEmitter.Emit(new MappingStartEventInfo(mapping)
			{
				Anchor = aliasProvider.GetAlias(mapping.Value)
			}, context);
		}

		public override void VisitSequenceStart(IObjectDescriptor sequence, Type elementType, IEmitter context)
		{
			eventEmitter.Emit(new SequenceStartEventInfo(sequence)
			{
				Anchor = aliasProvider.GetAlias(sequence.Value)
			}, context);
		}

		public override void VisitScalar(IObjectDescriptor scalar, IEmitter context)
		{
			eventEmitter.Emit(new ScalarEventInfo(scalar)
			{
				Anchor = aliasProvider.GetAlias(scalar.Value)
			}, context);
		}
	}
}
