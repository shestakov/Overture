using System;

namespace Overture.ChangeSets.Serializers
{
	public interface IAttributeValueBinarySerializer
	{
		byte[] Serialize(string name, Type type, object value);
		object Deserialize(string name, Type type, byte[] serializedValue);
		Type TargetType { get; }
	}
}