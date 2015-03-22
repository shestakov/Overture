using System;

namespace Overture.ChangeSets.Serializers
{
	public abstract class AttributeValueBinarySerializer<T> : IAttributeValueBinarySerializer
	{
		public byte[] Serialize(string name, Type type, object value)
		{
			if(value != null)
				return SerializeInternal(name, type, (T)value);
			if(CanBeNull)
				return new byte[0];
			throw new ArgumentException(string.Format("Serialized value cannot be null: {0} of type {1}", name, type.Name), "value");
		}

		public object Deserialize(string name, Type type, byte[] serializedValue)
		{
			if(serializedValue != null && serializedValue.Length != 0)
				return DeserializeInternal(name, type, serializedValue);
			if(CanBeNull)
				return null;
			throw new ArgumentException("Deserialized value cannot be null", "serializedValue");
		}

		protected abstract bool CanBeNull { get; }
		protected abstract byte[] SerializeInternal(string name, Type type, T value);
		protected abstract T DeserializeInternal(string name, Type type, byte[] serializedValue);
		public Type TargetType { get { return typeof (T); } }
	}
}