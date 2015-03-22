using System;
using System.Collections.Generic;
using NUnit.Framework;
using Overture.ChangeSets.Serializers;

namespace Overture.ChangeSets.Tests.Serializers
{
	[TestFixture(typeof(NullableInt32Serializer))]
	[TestFixture(typeof(NullableInt64Serializer))]
	[TestFixture(typeof(NullableDoubleSerializer))]
	[TestFixture(typeof(NullableGuidSerializer))]
	[TestFixture(typeof(NullableDateTimeOffsetSerializer))]
	[TestFixture(typeof(StringSerializer))]
	public class NullableAttributesSerializer_Test<T>
		where T : IAttributeValueBinarySerializer, new()
	{
		[SetUp]
		public void SetUp()
		{
			serializer = new T();
			attributeType = serializer.TargetType;
		}

		private T serializer;
		private Type attributeType;
		private const string attributeName = "attribute";

		private readonly Dictionary<Type, object> values = new Dictionary<Type, object>
		{
			{ typeof(Int32?), Int32.MaxValue },
			{ typeof(Int64?), Int64.MaxValue },
			{ typeof(Double?), Double.MaxValue },
			{ typeof(Guid?), new Guid("1C90A0A0-E6FD-4B2F-BD56-E3B5AEFD6D20") },
			{ typeof(DateTimeOffset?), DateTimeOffset.UtcNow },
			{ typeof(string), "TEST" }
		};

		[Test]
		public void NotNull()
		{
			var value = values[attributeType];
			var serializedValue = serializer.Serialize(attributeName, attributeType, value);
			var deserializedValue = serializer.Deserialize(attributeName, attributeType, serializedValue);
			Assert.AreEqual(value, deserializedValue);
		}

		[Test]
		public void Null()
		{
			var serializedValue = serializer.Serialize(attributeName, attributeType, null);
			Assert.IsNotNull(serializedValue);
			Assert.IsEmpty(serializedValue);
			var deserializedValue = serializer.Deserialize(attributeName, attributeType, serializedValue);
			Assert.IsNull(deserializedValue);
		}
	}
}