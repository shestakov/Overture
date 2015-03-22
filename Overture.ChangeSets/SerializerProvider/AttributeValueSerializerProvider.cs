using System;
using System.Collections.Generic;
using Overture.ChangeSets.Serializers;

namespace Overture.ChangeSets.SerializerProvider
{
	public class AttributeValueSerializerProvider : IAttributeValueSerializerProvider
	{
		private readonly IAttributeValueBinarySerializer enumSerializer = new EnumSerializer();
		private readonly IAttributeValueBinarySerializer nullableEnumSerializer = new NullableEnumSerializer();
		private readonly Dictionary<Type, IAttributeValueBinarySerializer> serializers = new Dictionary<Type, IAttributeValueBinarySerializer>();

		public AttributeValueSerializerProvider()
		{
			RegisterSerializer(new Int32Serializer());
			RegisterSerializer(new NullableInt32Serializer());

			RegisterSerializer(new Int64Serializer());
			RegisterSerializer(new NullableInt64Serializer());

			RegisterSerializer(new DoubleSerializer());
			RegisterSerializer(new NullableDoubleSerializer());

			RegisterSerializer(new DateTimeOffsetSerializer());
			RegisterSerializer(new NullableDateTimeOffsetSerializer());

			RegisterSerializer(new GuidSerializer());
			RegisterSerializer(new NullableGuidSerializer());

			RegisterSerializer(new StringSerializer());
			
			RegisterSerializer(new BooleanSerializer());
			RegisterSerializer(new NullableBooleanSerializer());
		}

		public IAttributeValueBinarySerializer Get(Type targetType)
		{
			if(targetType.IsEnum)
				return enumSerializer;

			var nullableUnderlyingType = Nullable.GetUnderlyingType(targetType);
			if(nullableUnderlyingType != null && nullableUnderlyingType.IsEnum)
				return nullableEnumSerializer;

			if(serializers.ContainsKey(targetType))
				return serializers[targetType];
			throw new ArgumentException(string.Format("There is no registered AttributeValueBinarySerializer for type {0}", targetType.FullName), "targetType");
		}

		private void RegisterSerializer<T>(AttributeValueBinarySerializer<T> serializer)
		{
			var type = typeof(T);
			if(serializer.TargetType != type)
				throw new ArgumentException("Inconsistent serializer target type", "serializer");
			if(serializers.ContainsKey(type))
				throw new ArgumentException(string.Format("Serializer for type {0} already registered", type.FullName), "serializer");
			serializers.Add(type, serializer);
		}
	}
}