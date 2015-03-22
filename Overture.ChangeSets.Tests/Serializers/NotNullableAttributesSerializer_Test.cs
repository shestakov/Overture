using System;
using System.Collections.Generic;
using NUnit.Framework;
using Overture.ChangeSets.Serializers;

namespace Overture.ChangeSets.Tests.Serializers
{
	[TestFixture(typeof(Int32Serializer))]
	[TestFixture(typeof(Int64Serializer))]
	[TestFixture(typeof(DoubleSerializer))]
	[TestFixture(typeof(GuidSerializer))]
	[TestFixture(typeof(DateTimeOffsetSerializer))]
	public class NotNullableAttributesSerializer_Test<T>
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
			{ typeof(Int32), Int32.MaxValue },
			{ typeof(Int64), Int64.MaxValue },
			{ typeof(Double), Double.MaxValue },
			{ typeof(Guid), new Guid("00F9DF1A-D746-480D-A0E8-8D72EB9C6871") },
			{ typeof(DateTimeOffset), DateTimeOffset.UtcNow },
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
			Assert.Throws<ArgumentException>(() => serializer.Serialize(attributeName, attributeType, null));
		}
	}
}