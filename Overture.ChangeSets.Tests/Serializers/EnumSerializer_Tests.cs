using System;
using NUnit.Framework;
using Overture.ChangeSets.Serializers;

namespace Overture.ChangeSets.Tests.Serializers
{
	[TestFixture]
	public class EnumSerializer_Tests
	{
		[SetUp]
		public void SetUp()
		{
			serializer = new NullableEnumSerializer();
		}

		private enum TestEnum
		{
			TestMember = 777
		}

		private NullableEnumSerializer serializer;
		private readonly Type attributeType = typeof(TestEnum);
		private const string attributeName = "attribute";

		[Test]
		public void NotNull()
		{
			const TestEnum value = TestEnum.TestMember;
			var serializedValue = serializer.Serialize(attributeName, attributeType, value);
			Assert.AreEqual(777, BitConverter.ToInt32(serializedValue, 0));
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