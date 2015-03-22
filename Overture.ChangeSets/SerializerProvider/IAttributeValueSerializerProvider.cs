using System;
using Overture.ChangeSets.Serializers;

namespace Overture.ChangeSets.SerializerProvider
{
	public interface IAttributeValueSerializerProvider
	{
		IAttributeValueBinarySerializer Get(Type targetType);
	}
}