using System;
using System.Collections.Generic;
using YamlDotNet.Serialization.NodeDeserializers;
using YamlDotNet.Serialization.NodeTypeResolvers;
using YamlDotNet.Serialization.ObjectFactories;
using YamlDotNet.Serialization.TypeInspectors;
using YamlDotNet.Serialization.TypeResolvers;
using YamlDotNet.Serialization.ValueDeserializers;

namespace YamlDotNet.Serialization
{
	public sealed class DeserializerBuilder : BuilderSkeleton<DeserializerBuilder>
	{
		private IObjectFactory objectFactory = new DefaultObjectFactory();

		private readonly LazyComponentRegistrationList<Nothing, INodeDeserializer> nodeDeserializerFactories;

		private readonly LazyComponentRegistrationList<Nothing, INodeTypeResolver> nodeTypeResolverFactories;

		private readonly Dictionary<string, Type> tagMappings;

		private bool ignoreUnmatched;

		protected override DeserializerBuilder Self => this;

		public DeserializerBuilder()
		{
			tagMappings = new Dictionary<string, Type>
			{
				{
					"tag:yaml.org,2002:map",
					typeof(Dictionary<object, object>)
				},
				{
					"tag:yaml.org,2002:bool",
					typeof(bool)
				},
				{
					"tag:yaml.org,2002:float",
					typeof(double)
				},
				{
					"tag:yaml.org,2002:int",
					typeof(int)
				},
				{
					"tag:yaml.org,2002:str",
					typeof(string)
				},
				{
					"tag:yaml.org,2002:timestamp",
					typeof(DateTime)
				}
			};
			typeInspectorFactories.Add(typeof(CachedTypeInspector), (ITypeInspector inner) => new CachedTypeInspector(inner));
			typeInspectorFactories.Add(typeof(NamingConventionTypeInspector), (ITypeInspector inner) => (namingConvention == null) ? inner : new NamingConventionTypeInspector(inner, namingConvention));
			typeInspectorFactories.Add(typeof(YamlAttributesTypeInspector), (ITypeInspector inner) => new YamlAttributesTypeInspector(inner));
			typeInspectorFactories.Add(typeof(YamlAttributeOverridesInspector), (ITypeInspector inner) => (overrides == null) ? inner : new YamlAttributeOverridesInspector(inner, overrides.Clone()));
			typeInspectorFactories.Add(typeof(ReadableAndWritablePropertiesTypeInspector), (ITypeInspector inner) => new ReadableAndWritablePropertiesTypeInspector(inner));
			nodeDeserializerFactories = new LazyComponentRegistrationList<Nothing, INodeDeserializer>
			{
				{
					typeof(YamlConvertibleNodeDeserializer),
					(Nothing _) => new YamlConvertibleNodeDeserializer(objectFactory)
				},
				{
					typeof(YamlSerializableNodeDeserializer),
					(Nothing _) => new YamlSerializableNodeDeserializer(objectFactory)
				},
				{
					typeof(TypeConverterNodeDeserializer),
					(Nothing _) => new TypeConverterNodeDeserializer(BuildTypeConverters())
				},
				{
					typeof(NullNodeDeserializer),
					(Nothing _) => new NullNodeDeserializer()
				},
				{
					typeof(ScalarNodeDeserializer),
					(Nothing _) => new ScalarNodeDeserializer()
				},
				{
					typeof(ArrayNodeDeserializer),
					(Nothing _) => new ArrayNodeDeserializer()
				},
				{
					typeof(DictionaryNodeDeserializer),
					(Nothing _) => new DictionaryNodeDeserializer(objectFactory)
				},
				{
					typeof(CollectionNodeDeserializer),
					(Nothing _) => new CollectionNodeDeserializer(objectFactory)
				},
				{
					typeof(EnumerableNodeDeserializer),
					(Nothing _) => new EnumerableNodeDeserializer()
				},
				{
					typeof(ObjectNodeDeserializer),
					(Nothing _) => new ObjectNodeDeserializer(objectFactory, BuildTypeInspector(), ignoreUnmatched)
				}
			};
			nodeTypeResolverFactories = new LazyComponentRegistrationList<Nothing, INodeTypeResolver>
			{
				{
					typeof(YamlConvertibleTypeResolver),
					(Nothing _) => new YamlConvertibleTypeResolver()
				},
				{
					typeof(YamlSerializableTypeResolver),
					(Nothing _) => new YamlSerializableTypeResolver()
				},
				{
					typeof(TagNodeTypeResolver),
					(Nothing _) => new TagNodeTypeResolver(tagMappings)
				},
				{
					typeof(PreventUnknownTagsNodeTypeResolver),
					(Nothing _) => new PreventUnknownTagsNodeTypeResolver()
				},
				{
					typeof(DefaultContainersNodeTypeResolver),
					(Nothing _) => new DefaultContainersNodeTypeResolver()
				}
			};
			WithTypeResolver(new StaticTypeResolver());
		}

		public DeserializerBuilder WithObjectFactory(IObjectFactory objectFactory)
		{
			if (objectFactory == null)
			{
				throw new ArgumentNullException("objectFactory");
			}
			this.objectFactory = objectFactory;
			return this;
		}

		public DeserializerBuilder WithObjectFactory(Func<Type, object> objectFactory)
		{
			if (objectFactory == null)
			{
				throw new ArgumentNullException("objectFactory");
			}
			return WithObjectFactory(new LambdaObjectFactory(objectFactory));
		}

		public DeserializerBuilder WithNodeDeserializer(INodeDeserializer nodeDeserializer)
		{
			return WithNodeDeserializer(nodeDeserializer, delegate(IRegistrationLocationSelectionSyntax<INodeDeserializer> w)
			{
				w.OnTop();
			});
		}

		public DeserializerBuilder WithNodeDeserializer(INodeDeserializer nodeDeserializer, Action<IRegistrationLocationSelectionSyntax<INodeDeserializer>> where)
		{
			if (nodeDeserializer == null)
			{
				throw new ArgumentNullException("nodeDeserializer");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(nodeDeserializerFactories.CreateRegistrationLocationSelector(nodeDeserializer.GetType(), (Nothing _) => nodeDeserializer));
			return this;
		}

		public DeserializerBuilder WithNodeDeserializer<TNodeDeserializer>(WrapperFactory<INodeDeserializer, TNodeDeserializer> nodeDeserializerFactory, Action<ITrackingRegistrationLocationSelectionSyntax<INodeDeserializer>> where) where TNodeDeserializer : INodeDeserializer
		{
			if (nodeDeserializerFactory == null)
			{
				throw new ArgumentNullException("nodeDeserializerFactory");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(nodeDeserializerFactories.CreateTrackingRegistrationLocationSelector(typeof(TNodeDeserializer), (Func<INodeDeserializer, Nothing, INodeDeserializer>)((INodeDeserializer wrapped, Nothing _) => (INodeDeserializer)(object)nodeDeserializerFactory(wrapped))));
			return this;
		}

		public DeserializerBuilder WithoutNodeDeserializer<TNodeDeserializer>() where TNodeDeserializer : INodeDeserializer
		{
			return WithoutNodeDeserializer(typeof(TNodeDeserializer));
		}

		public DeserializerBuilder WithoutNodeDeserializer(Type nodeDeserializerType)
		{
			if (nodeDeserializerType == null)
			{
				throw new ArgumentNullException("nodeDeserializerType");
			}
			nodeDeserializerFactories.Remove(nodeDeserializerType);
			return this;
		}

		public DeserializerBuilder WithNodeTypeResolver(INodeTypeResolver nodeTypeResolver)
		{
			return WithNodeTypeResolver(nodeTypeResolver, delegate(IRegistrationLocationSelectionSyntax<INodeTypeResolver> w)
			{
				w.OnTop();
			});
		}

		public DeserializerBuilder WithNodeTypeResolver(INodeTypeResolver nodeTypeResolver, Action<IRegistrationLocationSelectionSyntax<INodeTypeResolver>> where)
		{
			if (nodeTypeResolver == null)
			{
				throw new ArgumentNullException("nodeTypeResolver");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(nodeTypeResolverFactories.CreateRegistrationLocationSelector(nodeTypeResolver.GetType(), (Nothing _) => nodeTypeResolver));
			return this;
		}

		public DeserializerBuilder WithNodeTypeResolver<TNodeTypeResolver>(WrapperFactory<INodeTypeResolver, TNodeTypeResolver> nodeTypeResolverFactory, Action<ITrackingRegistrationLocationSelectionSyntax<INodeTypeResolver>> where) where TNodeTypeResolver : INodeTypeResolver
		{
			if (nodeTypeResolverFactory == null)
			{
				throw new ArgumentNullException("nodeTypeResolverFactory");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(nodeTypeResolverFactories.CreateTrackingRegistrationLocationSelector(typeof(TNodeTypeResolver), (Func<INodeTypeResolver, Nothing, INodeTypeResolver>)((INodeTypeResolver wrapped, Nothing _) => (INodeTypeResolver)(object)nodeTypeResolverFactory(wrapped))));
			return this;
		}

		public DeserializerBuilder WithoutNodeTypeResolver<TNodeTypeResolver>() where TNodeTypeResolver : INodeTypeResolver
		{
			return WithoutNodeTypeResolver(typeof(TNodeTypeResolver));
		}

		public DeserializerBuilder WithoutNodeTypeResolver(Type nodeTypeResolverType)
		{
			if (nodeTypeResolverType == null)
			{
				throw new ArgumentNullException("nodeTypeResolverType");
			}
			nodeTypeResolverFactories.Remove(nodeTypeResolverType);
			return this;
		}

		public DeserializerBuilder WithTagMapping(string tag, Type type)
		{
			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (tagMappings.TryGetValue(tag, out Type value))
			{
				throw new ArgumentException($"Type already has a registered type '{value.FullName}' for tag '{tag}'", "tag");
			}
			tagMappings.Add(tag, type);
			return this;
		}

		public DeserializerBuilder WithoutTagMapping(string tag)
		{
			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}
			if (!tagMappings.Remove(tag))
			{
				throw new KeyNotFoundException($"Tag '{tag}' is not registered");
			}
			return this;
		}

		public DeserializerBuilder IgnoreUnmatchedProperties()
		{
			ignoreUnmatched = true;
			return this;
		}

		public Deserializer Build()
		{
			return Deserializer.FromValueDeserializer(BuildValueDeserializer());
		}

		public IValueDeserializer BuildValueDeserializer()
		{
			return new AliasValueDeserializer(new NodeValueDeserializer(nodeDeserializerFactories.BuildComponentList(), nodeTypeResolverFactories.BuildComponentList()));
		}
	}
}
