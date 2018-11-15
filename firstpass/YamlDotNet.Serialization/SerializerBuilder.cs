using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.EventEmitters;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.ObjectGraphTraversalStrategies;
using YamlDotNet.Serialization.ObjectGraphVisitors;
using YamlDotNet.Serialization.TypeInspectors;
using YamlDotNet.Serialization.TypeResolvers;

namespace YamlDotNet.Serialization
{
	public sealed class SerializerBuilder : BuilderSkeleton<SerializerBuilder>
	{
		private class ValueSerializer : IValueSerializer
		{
			private readonly IObjectGraphTraversalStrategy traversalStrategy;

			private readonly IEventEmitter eventEmitter;

			private readonly IEnumerable<IYamlTypeConverter> typeConverters;

			private readonly LazyComponentRegistrationList<IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>> preProcessingPhaseObjectGraphVisitorFactories;

			private readonly LazyComponentRegistrationList<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>> emissionPhaseObjectGraphVisitorFactories;

			public ValueSerializer(IObjectGraphTraversalStrategy traversalStrategy, IEventEmitter eventEmitter, IEnumerable<IYamlTypeConverter> typeConverters, LazyComponentRegistrationList<IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>> preProcessingPhaseObjectGraphVisitorFactories, LazyComponentRegistrationList<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>> emissionPhaseObjectGraphVisitorFactories)
			{
				this.traversalStrategy = traversalStrategy;
				this.eventEmitter = eventEmitter;
				this.typeConverters = typeConverters;
				this.preProcessingPhaseObjectGraphVisitorFactories = preProcessingPhaseObjectGraphVisitorFactories;
				this.emissionPhaseObjectGraphVisitorFactories = emissionPhaseObjectGraphVisitorFactories;
			}

			public void SerializeValue(IEmitter emitter, object value, Type type)
			{
				Type type2 = (type != null) ? type : ((value == null) ? typeof(object) : value.GetType());
				Type staticType = type ?? typeof(object);
				ObjectDescriptor graph = new ObjectDescriptor(value, type2, staticType);
				List<IObjectGraphVisitor<Nothing>> preProcessingPhaseObjectGraphVisitors = preProcessingPhaseObjectGraphVisitorFactories.BuildComponentList(typeConverters);
				foreach (IObjectGraphVisitor<Nothing> item in preProcessingPhaseObjectGraphVisitors)
				{
					traversalStrategy.Traverse(graph, item, null);
				}
				ObjectSerializer nestedObjectSerializer = delegate(object v, Type t)
				{
					SerializeValue(emitter, v, t);
				};
				IObjectGraphVisitor<IEmitter> visitor = emissionPhaseObjectGraphVisitorFactories.BuildComponentChain(new EmittingObjectGraphVisitor(eventEmitter), (IObjectGraphVisitor<IEmitter> inner) => new EmissionPhaseObjectGraphVisitorArgs(inner, eventEmitter, preProcessingPhaseObjectGraphVisitors, typeConverters, nestedObjectSerializer));
				traversalStrategy.Traverse(graph, visitor, emitter);
			}
		}

		private Func<ITypeInspector, ITypeResolver, IEnumerable<IYamlTypeConverter>, IObjectGraphTraversalStrategy> objectGraphTraversalStrategyFactory;

		private readonly LazyComponentRegistrationList<IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>> preProcessingPhaseObjectGraphVisitorFactories;

		private readonly LazyComponentRegistrationList<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>> emissionPhaseObjectGraphVisitorFactories;

		private readonly LazyComponentRegistrationList<IEventEmitter, IEventEmitter> eventEmitterFactories;

		private readonly IDictionary<Type, string> tagMappings = new Dictionary<Type, string>();

		protected override SerializerBuilder Self => this;

		public SerializerBuilder()
		{
			typeInspectorFactories.Add(typeof(CachedTypeInspector), (ITypeInspector inner) => new CachedTypeInspector(inner));
			typeInspectorFactories.Add(typeof(NamingConventionTypeInspector), (ITypeInspector inner) => (namingConvention == null) ? inner : new NamingConventionTypeInspector(inner, namingConvention));
			typeInspectorFactories.Add(typeof(YamlAttributesTypeInspector), (ITypeInspector inner) => new YamlAttributesTypeInspector(inner));
			typeInspectorFactories.Add(typeof(YamlAttributeOverridesInspector), (ITypeInspector inner) => (overrides == null) ? inner : new YamlAttributeOverridesInspector(inner, overrides.Clone()));
			preProcessingPhaseObjectGraphVisitorFactories = new LazyComponentRegistrationList<IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>>();
			preProcessingPhaseObjectGraphVisitorFactories.Add(typeof(AnchorAssigner), (IEnumerable<IYamlTypeConverter> typeConverters) => new AnchorAssigner(typeConverters));
			emissionPhaseObjectGraphVisitorFactories = new LazyComponentRegistrationList<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>>();
			emissionPhaseObjectGraphVisitorFactories.Add(typeof(CustomSerializationObjectGraphVisitor), (EmissionPhaseObjectGraphVisitorArgs args) => new CustomSerializationObjectGraphVisitor(args.InnerVisitor, args.TypeConverters, args.NestedObjectSerializer));
			emissionPhaseObjectGraphVisitorFactories.Add(typeof(AnchorAssigningObjectGraphVisitor), (EmissionPhaseObjectGraphVisitorArgs args) => new AnchorAssigningObjectGraphVisitor(args.InnerVisitor, args.EventEmitter, args.GetPreProcessingPhaseObjectGraphVisitor<AnchorAssigner>()));
			emissionPhaseObjectGraphVisitorFactories.Add(typeof(DefaultExclusiveObjectGraphVisitor), (EmissionPhaseObjectGraphVisitorArgs args) => new DefaultExclusiveObjectGraphVisitor(args.InnerVisitor));
			eventEmitterFactories = new LazyComponentRegistrationList<IEventEmitter, IEventEmitter>();
			eventEmitterFactories.Add(typeof(TypeAssigningEventEmitter), (IEventEmitter inner) => new TypeAssigningEventEmitter(inner, false, tagMappings));
			objectGraphTraversalStrategyFactory = ((ITypeInspector typeInspector, ITypeResolver typeResolver, IEnumerable<IYamlTypeConverter> typeConverters) => new FullObjectGraphTraversalStrategy(typeInspector, typeResolver, 50, namingConvention ?? new NullNamingConvention()));
			WithTypeResolver(new DynamicTypeResolver());
		}

		public SerializerBuilder WithEventEmitter<TEventEmitter>(Func<IEventEmitter, TEventEmitter> eventEmitterFactory) where TEventEmitter : IEventEmitter
		{
			return WithEventEmitter(eventEmitterFactory, delegate(IRegistrationLocationSelectionSyntax<IEventEmitter> w)
			{
				w.OnTop();
			});
		}

		public SerializerBuilder WithEventEmitter<TEventEmitter>(Func<IEventEmitter, TEventEmitter> eventEmitterFactory, Action<IRegistrationLocationSelectionSyntax<IEventEmitter>> where) where TEventEmitter : IEventEmitter
		{
			if (eventEmitterFactory == null)
			{
				throw new ArgumentNullException("eventEmitterFactory");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(eventEmitterFactories.CreateRegistrationLocationSelector(typeof(TEventEmitter), (Func<IEventEmitter, IEventEmitter>)((IEventEmitter inner) => (IEventEmitter)(object)eventEmitterFactory(inner))));
			return ((BuilderSkeleton<SerializerBuilder>)this).Self;
		}

		public SerializerBuilder WithEventEmitter<TEventEmitter>(WrapperFactory<IEventEmitter, IEventEmitter, TEventEmitter> eventEmitterFactory, Action<ITrackingRegistrationLocationSelectionSyntax<IEventEmitter>> where) where TEventEmitter : IEventEmitter
		{
			if (eventEmitterFactory == null)
			{
				throw new ArgumentNullException("eventEmitterFactory");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(eventEmitterFactories.CreateTrackingRegistrationLocationSelector(typeof(TEventEmitter), (Func<IEventEmitter, IEventEmitter, IEventEmitter>)((IEventEmitter wrapped, IEventEmitter inner) => (IEventEmitter)(object)eventEmitterFactory(wrapped, inner))));
			return ((BuilderSkeleton<SerializerBuilder>)this).Self;
		}

		public SerializerBuilder WithoutEventEmitter<TEventEmitter>() where TEventEmitter : IEventEmitter
		{
			return WithoutEventEmitter(typeof(TEventEmitter));
		}

		public SerializerBuilder WithoutEventEmitter(Type eventEmitterType)
		{
			if (eventEmitterType == null)
			{
				throw new ArgumentNullException("eventEmitterType");
			}
			eventEmitterFactories.Remove(eventEmitterType);
			return this;
		}

		public SerializerBuilder WithTagMapping(string tag, Type type)
		{
			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (tagMappings.TryGetValue(type, out string value))
			{
				throw new ArgumentException($"Type already has a registered tag '{value}' for type '{type.FullName}'", "type");
			}
			tagMappings.Add(type, tag);
			return this;
		}

		public SerializerBuilder WithoutTagMapping(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!tagMappings.Remove(type))
			{
				throw new KeyNotFoundException($"Tag for type '{type.FullName}' is not registered");
			}
			return this;
		}

		public SerializerBuilder EnsureRoundtrip()
		{
			objectGraphTraversalStrategyFactory = ((ITypeInspector typeInspector, ITypeResolver typeResolver, IEnumerable<IYamlTypeConverter> typeConverters) => new RoundtripObjectGraphTraversalStrategy(typeConverters, typeInspector, typeResolver, 50));
			WithEventEmitter((IEventEmitter inner) => new TypeAssigningEventEmitter(inner, true, tagMappings), delegate(IRegistrationLocationSelectionSyntax<IEventEmitter> loc)
			{
				loc.InsteadOf<TypeAssigningEventEmitter>();
			});
			return WithTypeInspector((ITypeInspector inner) => new ReadableAndWritablePropertiesTypeInspector(inner), delegate(IRegistrationLocationSelectionSyntax<ITypeInspector> loc)
			{
				loc.OnBottom();
			});
		}

		public SerializerBuilder DisableAliases()
		{
			preProcessingPhaseObjectGraphVisitorFactories.Remove(typeof(AnchorAssigner));
			emissionPhaseObjectGraphVisitorFactories.Remove(typeof(AnchorAssigningObjectGraphVisitor));
			return this;
		}

		public SerializerBuilder EmitDefaults()
		{
			emissionPhaseObjectGraphVisitorFactories.Remove(typeof(DefaultExclusiveObjectGraphVisitor));
			return this;
		}

		public SerializerBuilder JsonCompatible()
		{
			return WithTypeConverter(new GuidConverter(true), delegate(IRegistrationLocationSelectionSyntax<IYamlTypeConverter> w)
			{
				w.InsteadOf<GuidConverter>();
			}).WithEventEmitter((IEventEmitter inner) => new JsonEventEmitter(inner), delegate(IRegistrationLocationSelectionSyntax<IEventEmitter> loc)
			{
				loc.InsteadOf<TypeAssigningEventEmitter>();
			});
		}

		public SerializerBuilder WithPreProcessingPhaseObjectGraphVisitor<TObjectGraphVisitor>(TObjectGraphVisitor objectGraphVisitor) where TObjectGraphVisitor : IObjectGraphVisitor<Nothing>
		{
			return WithPreProcessingPhaseObjectGraphVisitor(objectGraphVisitor, delegate(IRegistrationLocationSelectionSyntax<IObjectGraphVisitor<Nothing>> w)
			{
				w.OnTop();
			});
		}

		public SerializerBuilder WithPreProcessingPhaseObjectGraphVisitor<TObjectGraphVisitor>(TObjectGraphVisitor objectGraphVisitor, Action<IRegistrationLocationSelectionSyntax<IObjectGraphVisitor<Nothing>>> where) where TObjectGraphVisitor : IObjectGraphVisitor<Nothing>
		{
			if (objectGraphVisitor == null)
			{
				throw new ArgumentNullException("objectGraphVisitor");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(preProcessingPhaseObjectGraphVisitorFactories.CreateRegistrationLocationSelector(typeof(TObjectGraphVisitor), (Func<IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>>)((IEnumerable<IYamlTypeConverter> _) => (IObjectGraphVisitor<Nothing>)(object)objectGraphVisitor)));
			return this;
		}

		public SerializerBuilder WithPreProcessingPhaseObjectGraphVisitor<TObjectGraphVisitor>(WrapperFactory<IObjectGraphVisitor<Nothing>, TObjectGraphVisitor> objectGraphVisitorFactory, Action<ITrackingRegistrationLocationSelectionSyntax<IObjectGraphVisitor<Nothing>>> where) where TObjectGraphVisitor : IObjectGraphVisitor<Nothing>
		{
			if (objectGraphVisitorFactory == null)
			{
				throw new ArgumentNullException("objectGraphVisitorFactory");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(preProcessingPhaseObjectGraphVisitorFactories.CreateTrackingRegistrationLocationSelector(typeof(TObjectGraphVisitor), (Func<IObjectGraphVisitor<Nothing>, IEnumerable<IYamlTypeConverter>, IObjectGraphVisitor<Nothing>>)((IObjectGraphVisitor<Nothing> wrapped, IEnumerable<IYamlTypeConverter> _) => (IObjectGraphVisitor<Nothing>)(object)objectGraphVisitorFactory(wrapped))));
			return this;
		}

		public SerializerBuilder WithoutPreProcessingPhaseObjectGraphVisitor<TObjectGraphVisitor>() where TObjectGraphVisitor : IObjectGraphVisitor<Nothing>
		{
			return WithoutPreProcessingPhaseObjectGraphVisitor(typeof(TObjectGraphVisitor));
		}

		public SerializerBuilder WithoutPreProcessingPhaseObjectGraphVisitor(Type objectGraphVisitorType)
		{
			if (objectGraphVisitorType == null)
			{
				throw new ArgumentNullException("objectGraphVisitorType");
			}
			preProcessingPhaseObjectGraphVisitorFactories.Remove(objectGraphVisitorType);
			return this;
		}

		public SerializerBuilder WithEmissionPhaseObjectGraphVisitor<TObjectGraphVisitor>(Func<EmissionPhaseObjectGraphVisitorArgs, TObjectGraphVisitor> objectGraphVisitorFactory) where TObjectGraphVisitor : IObjectGraphVisitor<IEmitter>
		{
			return WithEmissionPhaseObjectGraphVisitor(objectGraphVisitorFactory, delegate(IRegistrationLocationSelectionSyntax<IObjectGraphVisitor<IEmitter>> w)
			{
				w.OnTop();
			});
		}

		public SerializerBuilder WithEmissionPhaseObjectGraphVisitor<TObjectGraphVisitor>(Func<EmissionPhaseObjectGraphVisitorArgs, TObjectGraphVisitor> objectGraphVisitorFactory, Action<IRegistrationLocationSelectionSyntax<IObjectGraphVisitor<IEmitter>>> where) where TObjectGraphVisitor : IObjectGraphVisitor<IEmitter>
		{
			if (objectGraphVisitorFactory == null)
			{
				throw new ArgumentNullException("objectGraphVisitorFactory");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(emissionPhaseObjectGraphVisitorFactories.CreateRegistrationLocationSelector(typeof(TObjectGraphVisitor), (Func<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>>)((EmissionPhaseObjectGraphVisitorArgs args) => (IObjectGraphVisitor<IEmitter>)(object)objectGraphVisitorFactory(args))));
			return this;
		}

		public SerializerBuilder WithEmissionPhaseObjectGraphVisitor<TObjectGraphVisitor>(WrapperFactory<EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>, TObjectGraphVisitor> objectGraphVisitorFactory, Action<ITrackingRegistrationLocationSelectionSyntax<IObjectGraphVisitor<IEmitter>>> where) where TObjectGraphVisitor : IObjectGraphVisitor<IEmitter>
		{
			if (objectGraphVisitorFactory == null)
			{
				throw new ArgumentNullException("objectGraphVisitorFactory");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(emissionPhaseObjectGraphVisitorFactories.CreateTrackingRegistrationLocationSelector(typeof(TObjectGraphVisitor), (Func<IObjectGraphVisitor<IEmitter>, EmissionPhaseObjectGraphVisitorArgs, IObjectGraphVisitor<IEmitter>>)((IObjectGraphVisitor<IEmitter> wrapped, EmissionPhaseObjectGraphVisitorArgs args) => (IObjectGraphVisitor<IEmitter>)(object)objectGraphVisitorFactory(wrapped, args))));
			return this;
		}

		public SerializerBuilder WithoutEmissionPhaseObjectGraphVisitor<TObjectGraphVisitor>() where TObjectGraphVisitor : IObjectGraphVisitor<IEmitter>
		{
			return WithoutEmissionPhaseObjectGraphVisitor(typeof(TObjectGraphVisitor));
		}

		public SerializerBuilder WithoutEmissionPhaseObjectGraphVisitor(Type objectGraphVisitorType)
		{
			if (objectGraphVisitorType == null)
			{
				throw new ArgumentNullException("objectGraphVisitorType");
			}
			emissionPhaseObjectGraphVisitorFactories.Remove(objectGraphVisitorType);
			return this;
		}

		public Serializer Build()
		{
			return Serializer.FromValueSerializer(BuildValueSerializer());
		}

		public IValueSerializer BuildValueSerializer()
		{
			IEnumerable<IYamlTypeConverter> enumerable = BuildTypeConverters();
			ITypeInspector arg = BuildTypeInspector();
			IObjectGraphTraversalStrategy traversalStrategy = objectGraphTraversalStrategyFactory(arg, typeResolver, enumerable);
			IEventEmitter eventEmitter = eventEmitterFactories.BuildComponentChain(new WriterEventEmitter());
			return new ValueSerializer(traversalStrategy, eventEmitter, enumerable, preProcessingPhaseObjectGraphVisitorFactories.Clone(), emissionPhaseObjectGraphVisitorFactories.Clone());
		}
	}
}
