using Overture.ChangeSets.SerializerProvider;

namespace Overture.ChangeSets.Tests.Infrastructure
{
	internal class TestSerializer
	{
		private readonly AttributeValueSerializerProvider serializerProvider = new AttributeValueSerializerProvider();

		public byte[] Serialize<T>(T value)
		{
			var targetType = typeof(T);
			return serializerProvider.Get(targetType).Serialize(targetType.Name, targetType, value);
		}
	}
}