using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.TypeInspectors;

namespace YamlDotNet.Serialization
{
	public abstract class BuilderSkeleton<TBuilder> where TBuilder : BuilderSkeleton<TBuilder>
	{
		internal INamingConvention namingConvention;

		internal ITypeResolver typeResolver;

		internal readonly YamlAttributeOverrides overrides;

		internal readonly LazyComponentRegistrationList<Nothing, IYamlTypeConverter> typeConverterFactories;

		internal readonly LazyComponentRegistrationList<ITypeInspector, ITypeInspector> typeInspectorFactories;

		protected abstract TBuilder Self
		{
			get;
		}

		internal BuilderSkeleton()
		{
			overrides = new YamlAttributeOverrides();
			typeConverterFactories = new LazyComponentRegistrationList<Nothing, IYamlTypeConverter>();
			typeConverterFactories.Add(typeof(GuidConverter), (Nothing _) => new GuidConverter(false));
			typeConverterFactories.Add(typeof(SystemTypeConverter), (Nothing _) => new SystemTypeConverter());
			typeInspectorFactories = new LazyComponentRegistrationList<ITypeInspector, ITypeInspector>();
		}

		internal ITypeInspector BuildTypeInspector()
		{
			return LazyComponentRegistrationListExtensions.BuildComponentChain<ITypeInspector>(typeInspectorFactories, (ITypeInspector)new ReadablePropertiesTypeInspector(typeResolver));
		}

		public TBuilder WithNamingConvention(INamingConvention namingConvention)
		{
			if (namingConvention == null)
			{
				throw new ArgumentNullException("namingConvention");
			}
			this.namingConvention = namingConvention;
			return Self;
		}

		public TBuilder WithTypeResolver(ITypeResolver typeResolver)
		{
			if (typeResolver == null)
			{
				throw new ArgumentNullException("typeResolver");
			}
			this.typeResolver = typeResolver;
			return Self;
		}

		public TBuilder WithAttributeOverride<TClass>(Expression<Func<TClass, object>> propertyAccessor, Attribute attribute)
		{
			this.overrides.Add<TClass>(propertyAccessor, attribute);
			return this.Self;
		}

		public TBuilder WithAttributeOverride(Type type, string member, Attribute attribute)
		{
			overrides.Add(type, member, attribute);
			return Self;
		}

		public TBuilder WithTypeConverter(IYamlTypeConverter typeConverter)
		{
			return WithTypeConverter(typeConverter, delegate(IRegistrationLocationSelectionSyntax<IYamlTypeConverter> w)
			{
				w.OnTop();
			});
		}

		public TBuilder WithTypeConverter(IYamlTypeConverter typeConverter, Action<IRegistrationLocationSelectionSyntax<IYamlTypeConverter>> where)
		{
			if (typeConverter == null)
			{
				throw new ArgumentNullException("typeConverter");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(typeConverterFactories.CreateRegistrationLocationSelector(typeConverter.GetType(), (Nothing _) => typeConverter));
			return Self;
		}

		public TBuilder WithTypeConverter<TYamlTypeConverter>(WrapperFactory<IYamlTypeConverter, IYamlTypeConverter> typeConverterFactory, Action<ITrackingRegistrationLocationSelectionSyntax<IYamlTypeConverter>> where) where TYamlTypeConverter : IYamlTypeConverter
		{
			if (typeConverterFactory == null)
			{
				throw new ArgumentNullException("typeConverterFactory");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(this.typeConverterFactories.CreateTrackingRegistrationLocationSelector(typeof(TYamlTypeConverter), (Func<IYamlTypeConverter, Nothing, IYamlTypeConverter>)((IYamlTypeConverter wrapped, Nothing _) => typeConverterFactory(wrapped))));
			return this.Self;
		}

		public TBuilder WithoutTypeConverter<TYamlTypeConverter>() where TYamlTypeConverter : IYamlTypeConverter
		{
			return this.WithoutTypeConverter(typeof(TYamlTypeConverter));
		}

		public TBuilder WithoutTypeConverter(Type converterType)
		{
			if (converterType == null)
			{
				throw new ArgumentNullException("converterType");
			}
			typeConverterFactories.Remove(converterType);
			return Self;
		}

		public TBuilder WithTypeInspector<TTypeInspector>(Func<ITypeInspector, TTypeInspector> typeInspectorFactory) where TTypeInspector : ITypeInspector
		{
			return WithTypeInspector(typeInspectorFactory, delegate(IRegistrationLocationSelectionSyntax<ITypeInspector> w)
			{
				w.OnTop();
			});
		}

		public TBuilder WithTypeInspector<TTypeInspector>(Func<ITypeInspector, TTypeInspector> typeInspectorFactory, Action<IRegistrationLocationSelectionSyntax<ITypeInspector>> where) where TTypeInspector : ITypeInspector
		{
			if (typeInspectorFactory == null)
			{
				throw new ArgumentNullException("typeInspectorFactory");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(this.typeInspectorFactories.CreateRegistrationLocationSelector(typeof(TTypeInspector), (Func<ITypeInspector, ITypeInspector>)((ITypeInspector inner) => (ITypeInspector)(object)typeInspectorFactory(inner))));
			return this.Self;
		}

		public TBuilder WithTypeInspector<TTypeInspector>(WrapperFactory<ITypeInspector, ITypeInspector, TTypeInspector> typeInspectorFactory, Action<ITrackingRegistrationLocationSelectionSyntax<ITypeInspector>> where) where TTypeInspector : ITypeInspector
		{
			if (typeInspectorFactory == null)
			{
				throw new ArgumentNullException("typeInspectorFactory");
			}
			if (where == null)
			{
				throw new ArgumentNullException("where");
			}
			where(this.typeInspectorFactories.CreateTrackingRegistrationLocationSelector(typeof(TTypeInspector), (Func<ITypeInspector, ITypeInspector, ITypeInspector>)((ITypeInspector wrapped, ITypeInspector inner) => (ITypeInspector)(object)typeInspectorFactory(wrapped, inner))));
			return this.Self;
		}

		public TBuilder WithoutTypeInspector<TTypeInspector>() where TTypeInspector : ITypeInspector
		{
			return this.WithoutTypeInspector(typeof(ITypeInspector));
		}

		public TBuilder WithoutTypeInspector(Type inspectorType)
		{
			if (inspectorType == null)
			{
				throw new ArgumentNullException("inspectorType");
			}
			typeInspectorFactories.Remove(inspectorType);
			return Self;
		}

		protected IEnumerable<IYamlTypeConverter> BuildTypeConverters()
		{
			return LazyComponentRegistrationListExtensions.BuildComponentList<IYamlTypeConverter>(typeConverterFactories);
		}
	}
}
